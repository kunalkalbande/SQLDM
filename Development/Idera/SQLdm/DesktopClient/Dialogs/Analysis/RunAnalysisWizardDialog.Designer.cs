using Idera.SQLdm.DesktopClient.Controls.Analysis;
using Idera.SQLdm.DesktopClient.Properties;
using System.Drawing;

namespace Idera.SQLdm.DesktopClient.Dialogs.Analysis
{
    partial class RunAnalysisWizard
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
            bool isDarkThemeSelected = Properties.Settings.Default.ColorScheme == "Dark";
            System.Windows.Forms.TreeNode treeNode1 = new System.Windows.Forms.TreeNode("Category Four.One");
            System.Windows.Forms.TreeNode treeNode2 = new System.Windows.Forms.TreeNode("Category Four.Two");
            System.Windows.Forms.TreeNode treeNode3 = new System.Windows.Forms.TreeNode("Category Four");
            System.Windows.Forms.TreeNode treeNode4 = new System.Windows.Forms.TreeNode("Category Five");
            System.Windows.Forms.TreeNode treeNode5 = new System.Windows.Forms.TreeNode("All Categories", new System.Windows.Forms.TreeNode[] {
            treeNode1,
            treeNode2,
            treeNode3,
            treeNode4});
            Infragistics.Win.Appearance appearance1 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance2 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance3 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance4 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance5 = new Infragistics.Win.Appearance();
            Infragistics.Win.UltraWinTabControl.UltraTab ultraTab1 = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomUltraTab();
            Infragistics.Win.UltraWinTabControl.UltraTab ultraTab2 = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomUltraTab();
            this.ultraTabPageControl3 = new Infragistics.Win.UltraWinTabControl.UltraTabPageControl();
            this.PanelAnalysisCategories = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomPanel ();
            this.labelCategoryTree = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomLabel();
            this.TreeViewCategories = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomTreeView();
            this.smoothingContentPropertyPage = new Idera.SQLdm.DesktopClient.Controls.Office2007PropertyPage(isDarkThemeSelected);
            this.ultraTabPageControl8 = new Infragistics.Win.UltraWinTabControl.UltraTabPageControl();
            this.numericUpDown1 = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomNumericUpDown();
            this.panelFilterQuestion1 = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomPanel ();
            this.label5 = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomLabel();
            this.panel2 = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomPanel ();
            this.label3 = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomLabel();
            this.lblFilterQuestion1 = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomLabel();
            this.panelFilterQuestion2 = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomPanel ();
            this.label2 = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomLabel();
            this.label1 = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomLabel();
            this.txtQueryFromDiagnose = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomTextBox();
            this._databaseFilterTypeComboBox = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomComboBox();
            this.label4 = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomLabel();
            this.office2007PropertyPage7 = new Idera.SQLdm.DesktopClient.Controls.Office2007PropertyPage(isDarkThemeSelected);
            this.ultraTabPageControl1 = new Infragistics.Win.UltraWinTabControl.UltraTabPageControl();
            this.treeView1 = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomTreeView();
            this.textBox1 = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomTextBox();
            this.btnCancel = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomButton();
            this.btnNext = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomButton();
            this.btnBack = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomButton();
            this.panel1 = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomPanel ();
            this.txtTitle = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomTextBox();
            this.txtDescription = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomTextBox();
            this.ultraTabPageControl2 = new Infragistics.Win.UltraWinTabControl.UltraTabPageControl();
            this.ultraTabSharedControlsPage1 = new Infragistics.Win.UltraWinTabControl.UltraTabSharedControlsPage();
            this.panel4 = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomPanel ();
            this.btnAnalysisWizardNext = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomButton();
            this.btnAnalysisCategoryWizardCancel = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomButton();
            this.btnAnalysisCategoryWizardFinish = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomButton();
            this.btnAnalysisCategoryWizardHelp = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomButton();
            this.tabControl = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomUltraTabControl(true);
            this.ultraTabSharedControlsPage2 = new Infragistics.Win.UltraWinTabControl.UltraTabSharedControlsPage();
            this.button1 = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomButton();
            this.button2 = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomButton();
            this.label6 = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomLabel();
            this.ultraTabPageControl3.SuspendLayout();
            this.PanelAnalysisCategories.SuspendLayout();
            this.ultraTabPageControl8.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDown1)).BeginInit();
            this.panel2.SuspendLayout();
            this.panelFilterQuestion1.SuspendLayout();
            this.panelFilterQuestion2.SuspendLayout();
            this.ultraTabPageControl1.SuspendLayout();
            this.panel1.SuspendLayout();
            this.ultraTabSharedControlsPage1.SuspendLayout();
            this.panel4.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.tabControl)).BeginInit();
            this.tabControl.SuspendLayout();
            this.SuspendLayout();
            // 
            // ultraTabPageControl3
            // 
            this.ultraTabPageControl3.Controls.Add(this.PanelAnalysisCategories);
            this.ultraTabPageControl3.Controls.Add(this.smoothingContentPropertyPage);
            this.ultraTabPageControl3.Location = new System.Drawing.Point(-10000, -10000);
            this.ultraTabPageControl3.Location = new System.Drawing.Point(141, 0);
            this.ultraTabPageControl3.Name = "ultraTabPageControl3";
            this.ultraTabPageControl3.Size = new System.Drawing.Size(528, 372);
            // 
            // PanelAnalysisCategories
            // 
            this.PanelAnalysisCategories.BackColor = System.Drawing.SystemColors.ActiveCaption;
            this.PanelAnalysisCategories.Controls.Add(this.labelCategoryTree);
            this.PanelAnalysisCategories.Controls.Add(this.TreeViewCategories);
            this.PanelAnalysisCategories.Location = new System.Drawing.Point(13, 46);
            this.PanelAnalysisCategories.Name = "PanelAnalysisCategories";
            this.PanelAnalysisCategories.Size = new System.Drawing.Size(508, 325);
            this.PanelAnalysisCategories.TabIndex = 1;
            // 
            // labelCategoryTree
            // 
            this.labelCategoryTree.AutoSize = true;
            this.labelCategoryTree.Location = new System.Drawing.Point(230, 7);
            this.labelCategoryTree.MaximumSize = new System.Drawing.Size(250, 50);
            this.labelCategoryTree.MinimumSize = new System.Drawing.Size(50, 50);
            this.labelCategoryTree.Name = "labelCategoryTree";
            this.labelCategoryTree.Size = new System.Drawing.Size(50, 50);
            this.labelCategoryTree.TabIndex = 1;
            // 
            // TreeViewCategories
            // 
            this.TreeViewCategories.CheckBoxes = true;
            this.TreeViewCategories.Dock = System.Windows.Forms.DockStyle.Left;
            this.TreeViewCategories.Location = new System.Drawing.Point(0, 0);
            this.TreeViewCategories.Name = "TreeViewCategories";
            this.TreeViewCategories.Size = new System.Drawing.Size(224, 325);
            this.TreeViewCategories.TabIndex = 0;
            this.TreeViewCategories.AfterCheck += new System.Windows.Forms.TreeViewEventHandler(this.node_AfterCheck);
            this.TreeViewCategories.AfterSelect += new System.Windows.Forms.TreeViewEventHandler(this.TreeViewCategories_AfterSelect);
            this.TreeViewCategories.Scrollable = true;
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
            this.smoothingContentPropertyPage.ContentPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.smoothingContentPropertyPage.ContentPanel.FillStyle = Idera.SQLdm.DesktopClient.Controls.GradientPanelFillStyle.Solid;
            this.smoothingContentPropertyPage.ContentPanel.Location = new System.Drawing.Point(0, 55);
            this.smoothingContentPropertyPage.ContentPanel.Name = "ContentPanel";
            this.smoothingContentPropertyPage.ContentPanel.Padding = new System.Windows.Forms.Padding(1);
            this.smoothingContentPropertyPage.ContentPanel.ShowBorder = false;
            this.smoothingContentPropertyPage.ContentPanel.Size = new System.Drawing.Size(528, 317);
            this.smoothingContentPropertyPage.ContentPanel.TabIndex = 1;
            this.smoothingContentPropertyPage.Dock = System.Windows.Forms.DockStyle.Fill;
            this.smoothingContentPropertyPage.HeaderImage = global::Idera.SQLdm.DesktopClient.Properties.Resources.AlertsFeature1;
            this.smoothingContentPropertyPage.Location = new System.Drawing.Point(0, 0);
            this.smoothingContentPropertyPage.Name = "smoothingContentPropertyPage";
            this.smoothingContentPropertyPage.Size = new System.Drawing.Size(528, 372);
            this.smoothingContentPropertyPage.TabIndex = 0;
            this.smoothingContentPropertyPage.Text = "Customize when an alert should be raised.";
            // 
            // ultraTabPageControl8
            // 
            this.ultraTabPageControl8.Controls.Add(this.label6);
            this.ultraTabPageControl8.Controls.Add(this.numericUpDown1);
            this.ultraTabPageControl8.Controls.Add(this.label5);
            this.ultraTabPageControl8.Controls.Add(this.panel2);
            this.ultraTabPageControl8.Controls.Add(this.panelFilterQuestion1);
            this.ultraTabPageControl8.Controls.Add(this.panelFilterQuestion2);
            this.ultraTabPageControl8.Controls.Add(this.label1);
            this.ultraTabPageControl8.Controls.Add(this.txtQueryFromDiagnose);
            this.ultraTabPageControl8.Controls.Add(this._databaseFilterTypeComboBox);
            this.ultraTabPageControl8.Controls.Add(this.label4);
            this.ultraTabPageControl8.Controls.Add(this.office2007PropertyPage7);
            this.ultraTabPageControl8.Location = new System.Drawing.Point(-10000, -10000);
            this.ultraTabPageControl8.Name = "ultraTabPageControl8";
            this.ultraTabPageControl8.Size = new System.Drawing.Size(528, 372);
            // 
            // numericUpDown1
            // 
            this.numericUpDown1.Location = new System.Drawing.Point(166, 237);
            this.numericUpDown1.Value = 30;
            this.numericUpDown1.Maximum = new decimal(new int[] {
            500,
            0,
            0,
            0});
            this.numericUpDown1.Name = "numericUpDown1";
            this.numericUpDown1.Size = new System.Drawing.Size(67, 20);
            this.numericUpDown1.TabIndex = 18;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.BackColor = System.Drawing.Color.White;
            this.label5.Location = new System.Drawing.Point(14, 239);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(134, 13);
            this.label5.TabIndex = 17;
            this.label5.Text = "Duration in Minutes (1-500)";
            // 
            // panel2
            // 
            this.panel2.BackColor = System.Drawing.Color.White;
            this.panel2.Controls.Add(this.label3);
            this.panel2.ForeColor = System.Drawing.SystemColors.ControlText;
            this.panel2.Location = new System.Drawing.Point(17, 193);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(406, 31);
            this.panel2.TabIndex = 16;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.ForeColor = System.Drawing.Color.SlateBlue;
            this.label3.Location = new System.Drawing.Point(6, 10);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(322, 13);
            this.label3.TabIndex = 0;
            this.label3.Text = "How long should SQLDM should spend collecting diagnostic data?";


            // 
            // panelFilterQuestion1
            // 
            this.panelFilterQuestion1.BackColor = System.Drawing.Color.White;
            this.panelFilterQuestion1.Controls.Add(this.lblFilterQuestion1);
            this.panelFilterQuestion1.Location = new System.Drawing.Point(17, 49);
            this.panelFilterQuestion1.Name = "panelFilterQuestion1";
            this.panelFilterQuestion1.Size = new System.Drawing.Size(407, 35);
            this.panelFilterQuestion1.TabIndex = 12;
            // 
            // lblFilterQuestion1
            // 
            this.lblFilterQuestion1.AutoSize = true;
            this.lblFilterQuestion1.ForeColor = System.Drawing.Color.SlateBlue;
            this.lblFilterQuestion1.Location = new System.Drawing.Point(6, 10);
            this.lblFilterQuestion1.Name = "lblFilterQuestion1";
            this.lblFilterQuestion1.Size = new System.Drawing.Size(296, 13);
            this.lblFilterQuestion1.TabIndex = 0;
            this.lblFilterQuestion1.Text = "Would you like to analyze workload from specific application?";
            // 
            // panelFilterQuestion2
            // 
            this.panelFilterQuestion2.BackColor = System.Drawing.Color.White;
            this.panelFilterQuestion2.Controls.Add(this.label2);
            this.panelFilterQuestion2.Location = new System.Drawing.Point(17, 117);
            this.panelFilterQuestion2.Name = "panelFilterQuestion2";
            this.panelFilterQuestion2.Size = new System.Drawing.Size(407, 30);
            this.panelFilterQuestion2.TabIndex = 13;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.ForeColor = System.Drawing.Color.SlateBlue;
            this.label2.Location = new System.Drawing.Point(6, 10);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(253, 13);
            this.label2.TabIndex = 0;
            this.label2.Text = "Would you like to limit analysis to a single database?";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.BackColor = System.Drawing.Color.White;
            this.label1.Location = new System.Drawing.Point(14, 91);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(149, 13);
            this.label1.TabIndex = 11;
            this.label1.Text = "Application Name (% wildcard)";
            // 
            // txtQueryFromDiagnose
            // 
            this.txtQueryFromDiagnose.Location = new System.Drawing.Point(166, 91);
            this.txtQueryFromDiagnose.Name = "txtQueryFromDiagnose";
            this.txtQueryFromDiagnose.Size = new System.Drawing.Size(258, 20);
            this.txtQueryFromDiagnose.TabIndex = 10;
            // 
            // _databaseFilterTypeComboBox
            // 
            this._databaseFilterTypeComboBox.FormattingEnabled = true;
            this._databaseFilterTypeComboBox.Location = new System.Drawing.Point(166, 158);
            this._databaseFilterTypeComboBox.Name = "_databaseFilterTypeComboBox";
            this._databaseFilterTypeComboBox.Size = new System.Drawing.Size(258, 21);
            this._databaseFilterTypeComboBox.TabIndex = 15;
            this._databaseFilterTypeComboBox.SelectedIndexChanged += new System.EventHandler(this._databaseFilterTypeComboBox_SelectedIndexChanged);
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.BackColor = System.Drawing.Color.White;
            this.label4.Location = new System.Drawing.Point(14, 159);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(93, 13);
            this.label4.TabIndex = 14;
            this.label4.Text = "Target Application";
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
            this.office2007PropertyPage7.ContentPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.office2007PropertyPage7.ContentPanel.FillStyle = Idera.SQLdm.DesktopClient.Controls.GradientPanelFillStyle.Solid;
            this.office2007PropertyPage7.ContentPanel.Location = new System.Drawing.Point(0, 55);
            this.office2007PropertyPage7.ContentPanel.Name = "ContentPanel";
            this.office2007PropertyPage7.ContentPanel.Padding = new System.Windows.Forms.Padding(1);
            this.office2007PropertyPage7.ContentPanel.ShowBorder = false;
            this.office2007PropertyPage7.ContentPanel.Size = new System.Drawing.Size(528, 317);
            this.office2007PropertyPage7.ContentPanel.TabIndex = 1;
            this.office2007PropertyPage7.Dock = System.Windows.Forms.DockStyle.Fill;
            this.office2007PropertyPage7.HeaderImage = global::Idera.SQLdm.DesktopClient.Properties.Resources.FilterLarge;
            this.office2007PropertyPage7.Location = new System.Drawing.Point(0, 0);
            this.office2007PropertyPage7.Name = "office2007PropertyPage7";
            this.office2007PropertyPage7.Size = new System.Drawing.Size(528, 372);
            this.office2007PropertyPage7.TabIndex = 1;
            this.office2007PropertyPage7.Text = "Filter out objects for which you don\'t wish to receive alerts.";
            // 
            // ultraTabPageControl1
            // 
            this.ultraTabPageControl1.Controls.Add(this.treeView1);
            this.ultraTabPageControl1.Controls.Add(this.textBox1);
            this.ultraTabPageControl1.Location = new System.Drawing.Point(0, 0);
            this.ultraTabPageControl1.Name = "ultraTabPageControl1";
            this.ultraTabPageControl1.Size = new System.Drawing.Size(609, 518);
            // 
            // treeView1
            // 
            this.treeView1.CheckBoxes = true;
            this.treeView1.LineColor = System.Drawing.Color.Empty;
            this.treeView1.Location = new System.Drawing.Point(137, 170);
            this.treeView1.Name = "treeView1";
            treeNode1.Name = "Node7";
            treeNode1.Text = "Category Four.One";
            treeNode2.Name = "Node8";
            treeNode2.Text = "Category Four.Two";
            treeNode3.Name = "Node5";
            treeNode3.Text = "Category Four";
            treeNode4.Name = "Node6";
            treeNode4.Text = "Category Five";
            treeNode5.Name = "Node1";
            treeNode5.Text = "All Categories";
            this.treeView1.Nodes.AddRange(new System.Windows.Forms.TreeNode[] {
            treeNode5});
            this.treeView1.Size = new System.Drawing.Size(203, 200);
            this.treeView1.TabIndex = 14;
            // 
            // textBox1
            // 
            this.textBox1.Location = new System.Drawing.Point(137, 144);
            this.textBox1.Name = "textBox1";
            this.textBox1.Size = new System.Drawing.Size(100, 20);
            this.textBox1.TabIndex = 13;
            this.textBox1.Text = "Categories:";
            // 
            // btnCancel
            // 
            this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnCancel.Location = new System.Drawing.Point(522, 461);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(75, 23);
            this.btnCancel.TabIndex = 12;
            this.btnCancel.Text = "Cancel";
            this.btnCancel.UseVisualStyleBackColor = true;
            // 
            // btnNext
            // 
            this.btnNext.Location = new System.Drawing.Point(407, 546);
            this.btnNext.Name = "btnNext";
            this.btnNext.Size = new System.Drawing.Size(75, 23);
            this.btnNext.TabIndex = 11;
            this.btnNext.Text = "Next";
            this.btnNext.UseVisualStyleBackColor = true;
            // 
            // btnBack
            // 
            this.btnBack.Location = new System.Drawing.Point(360, 354);
            this.btnBack.Name = "btnBack";
            this.btnBack.Size = new System.Drawing.Size(74, 23);
            this.btnBack.TabIndex = 10;
            this.btnBack.Text = "Back";
            this.btnBack.UseVisualStyleBackColor = true;
            // 
            // panel1
            // 
            this.panel1.BackColor = System.Drawing.Color.White;
            this.panel1.Controls.Add(this.txtTitle);
            this.panel1.Controls.Add(this.txtDescription);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Top;
            this.panel1.Location = new System.Drawing.Point(0, 0);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(609, 100);
            this.panel1.TabIndex = 0;
            // 
            // txtTitle
            // 
            this.txtTitle.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtTitle.Location = new System.Drawing.Point(19, 12);
            this.txtTitle.Name = "txtTitle";
            this.txtTitle.Size = new System.Drawing.Size(159, 20);
            this.txtTitle.TabIndex = 1;
            this.txtTitle.Text = "Select Analysis Categories";
            // 
            // txtDescription
            // 
            this.txtDescription.Location = new System.Drawing.Point(41, 38);
            this.txtDescription.Multiline = true;
            this.txtDescription.Name = "txtDescription";
            this.txtDescription.Size = new System.Drawing.Size(556, 50);
            this.txtDescription.TabIndex = 0;
            this.txtDescription.Text = "Select the analysis categories to be included in the prescriptive analysis of thi" +
    "s SQL Server.";
            // 
            // ultraTabPageControl2
            // 
            this.ultraTabPageControl2.Location = new System.Drawing.Point(-10000, -10000);
            this.ultraTabPageControl2.Name = "ultraTabPageControl2";
            this.ultraTabPageControl2.Size = new System.Drawing.Size(609, 516);
            // 
            // ultraTabSharedControlsPage1
            // 
            this.ultraTabSharedControlsPage1.Controls.Add(this.btnCancel);
            this.ultraTabSharedControlsPage1.Controls.Add(this.btnNext);
            this.ultraTabSharedControlsPage1.Controls.Add(this.btnBack);
            this.ultraTabSharedControlsPage1.Controls.Add(this.panel1);
            this.ultraTabSharedControlsPage1.Location = new System.Drawing.Point(-10000, -10000);
            this.ultraTabSharedControlsPage1.Name = "ultraTabSharedControlsPage1";
            this.ultraTabSharedControlsPage1.Size = new System.Drawing.Size(609, 516);
            // 
            // panel4
            // 
            this.panel4.Controls.Add(this.btnAnalysisWizardNext);
            this.panel4.Controls.Add(this.btnAnalysisCategoryWizardCancel);
            this.panel4.Controls.Add(this.btnAnalysisCategoryWizardFinish);
            this.panel4.Controls.Add(this.btnAnalysisCategoryWizardHelp);
            this.panel4.Location = new System.Drawing.Point(72, 948);
            this.panel4.Name = "panel4";
            this.panel4.Size = new System.Drawing.Size(585, 59);
            this.panel4.TabIndex = 3;
            // 
            // btnAnalysisWizardNext
            // 
            this.btnAnalysisWizardNext.Location = new System.Drawing.Point(306, 19);
            this.btnAnalysisWizardNext.Name = "btnAnalysisWizardNext";
            this.btnAnalysisWizardNext.Size = new System.Drawing.Size(75, 23);
            this.btnAnalysisWizardNext.TabIndex = 4;
            this.btnAnalysisWizardNext.Text = "Next";
            this.btnAnalysisWizardNext.UseVisualStyleBackColor = true;
            // 
            // btnAnalysisCategoryWizardCancel
            // 
            this.btnAnalysisCategoryWizardCancel.Location = new System.Drawing.Point(475, 19);
            this.btnAnalysisCategoryWizardCancel.Name = "btnAnalysisCategoryWizardCancel";
            this.btnAnalysisCategoryWizardCancel.Size = new System.Drawing.Size(75, 23);
            this.btnAnalysisCategoryWizardCancel.TabIndex = 3;
            this.btnAnalysisCategoryWizardCancel.Text = "Cancel";
            this.btnAnalysisCategoryWizardCancel.UseVisualStyleBackColor = true;
            this.btnAnalysisCategoryWizardCancel.Click += new System.EventHandler(this.btnAnalysisCategoryWizardCancel_Click);
            // 
            // btnAnalysisCategoryWizardFinish
            // 
            this.btnAnalysisCategoryWizardFinish.Location = new System.Drawing.Point(394, 19);
            this.btnAnalysisCategoryWizardFinish.Name = "btnAnalysisCategoryWizardFinish";
            this.btnAnalysisCategoryWizardFinish.Size = new System.Drawing.Size(75, 23);
            this.btnAnalysisCategoryWizardFinish.TabIndex = 2;
            this.btnAnalysisCategoryWizardFinish.Text = "Finish";
            this.btnAnalysisCategoryWizardFinish.UseVisualStyleBackColor = true;
            this.btnAnalysisCategoryWizardFinish.Click += new System.EventHandler(this.btnAnalysisCategoryWizardFinish_Click);
            // 
            // btnAnalysisCategoryWizardHelp
            // 
            this.btnAnalysisCategoryWizardHelp.Location = new System.Drawing.Point(12, 19);
            this.btnAnalysisCategoryWizardHelp.Name = "btnAnalysisCategoryWizardHelp";
            this.btnAnalysisCategoryWizardHelp.Size = new System.Drawing.Size(75, 23);
            this.btnAnalysisCategoryWizardHelp.TabIndex = 0;
            this.btnAnalysisCategoryWizardHelp.Text = "Help";
            this.btnAnalysisCategoryWizardHelp.UseVisualStyleBackColor = true;
            // 
            // tabControl
            // 
            appearance1.ImageBackground = global::Idera.SQLdm.DesktopClient.Properties.Resources.SelectedTabBackground;
            appearance1.ImageBackgroundStretchMargins = new Infragistics.Win.ImageBackgroundStretchMargins(1, 1, 1, 1);
            appearance1.ImageBackgroundStyle = Infragistics.Win.ImageBackgroundStyle.Stretched;
            this.tabControl.ActiveTabAppearance = appearance1;
            appearance2.BackColor = System.Drawing.Color.Transparent;
            appearance2.TextHAlignAsString = "Left";
            this.tabControl.Appearance = appearance2;
            appearance3.BackColor = System.Drawing.Color.DarkGray;
            this.tabControl.ClientAreaAppearance = appearance3;
            this.tabControl.Controls.Add(this.ultraTabSharedControlsPage2);
            this.tabControl.Controls.Add(this.ultraTabPageControl8);
            this.tabControl.Controls.Add(this.ultraTabPageControl3);
            this.tabControl.Dock = System.Windows.Forms.DockStyle.Top;
            appearance4.ImageBackground = global::Idera.SQLdm.DesktopClient.Properties.Resources.HotTrackTabBackground;
            appearance4.ImageBackgroundStretchMargins = new Infragistics.Win.ImageBackgroundStretchMargins(1, 1, 1, 1);
            appearance4.ImageBackgroundStyle = Infragistics.Win.ImageBackgroundStyle.Stretched;
            this.tabControl.HotTrackAppearance = appearance4;
            this.tabControl.InterTabSpacing = new Infragistics.Win.DefaultableInteger(1);
            this.tabControl.Location = new System.Drawing.Point(0, 0);
            this.tabControl.MinTabWidth = 26;
            this.tabControl.Name = "tabControl";
            this.tabControl.SharedControlsPage = this.ultraTabSharedControlsPage2;
            this.tabControl.Size = new System.Drawing.Size(669, 372);
            this.tabControl.Style = Infragistics.Win.UltraWinTabControl.UltraTabControlStyle.StateButtons;
            this.tabControl.TabButtonStyle = Infragistics.Win.UIElementButtonStyle.Flat;
            appearance5.BackColor = Settings.Default.ColorScheme == "Dark" ? ColorTranslator.FromHtml(DarkThemeColorConstants.BackColor) : System.Drawing.Color.White;
            appearance5.BorderColor = System.Drawing.Color.Black;
            this.tabControl.TabHeaderAreaAppearance = appearance5;
            this.tabControl.TabIndex = 5;
            this.tabControl.TabOrientation = Infragistics.Win.UltraWinTabs.TabOrientation.LeftTop;
            this.tabControl.TabPageMargins.Left = 1;
            ultraTab1.Key = "CategoryTab";
            ultraTab1.TabPage = this.ultraTabPageControl3;
            ultraTab1.Text = "Select Categories";
            ultraTab1.ToolTipText = "Configure time a metric must exceed its configured threshold before an alert is g" +
    "enerated.";
            ultraTab2.Key = "FiltersTab";
            ultraTab2.TabPage = this.ultraTabPageControl8;
            ultraTab2.Text = "Filters";
            this.tabControl.Tabs.AddRange(new Infragistics.Win.UltraWinTabControl.UltraTab[] {
            ultraTab1,
            ultraTab2});
            this.tabControl.TabSize = new System.Drawing.Size(0, 140);
            this.tabControl.TextOrientation = Infragistics.Win.UltraWinTabs.TextOrientation.Horizontal;
            this.tabControl.UseAppStyling = false;
            this.tabControl.UseHotTracking = Infragistics.Win.DefaultableBoolean.True;
            this.tabControl.UseOsThemes = Infragistics.Win.DefaultableBoolean.False;
            // 
            // ultraTabSharedControlsPage2
            // 
            this.ultraTabSharedControlsPage2.Location = new System.Drawing.Point(-10000, -10000);
            this.ultraTabSharedControlsPage2.Name = "ultraTabSharedControlsPage2";
            this.ultraTabSharedControlsPage2.Size = new System.Drawing.Size(528, 372);
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(588, 378);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(75, 23);
            this.button1.TabIndex = 6;
            this.button1.Text = "Cancel";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.btnAnalysisCategoryWizardCancel_Click);
            // 
            // button2
            // 
            this.button2.Location = new System.Drawing.Point(491, 378);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(88, 23);
            this.button2.TabIndex = 7;
            this.button2.Text = "Run Analysis";
            this.button2.UseVisualStyleBackColor = true;
            this.button2.Click += new System.EventHandler(this.btnAnalysisCategoryWizardFinish_Click);
            // 
            // label6
            // 
            this.label6.BackColor = System.Drawing.Color.White;
            this.label6.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Italic, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label6.Location = new System.Drawing.Point(14, 266);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(410, 32);
            this.label6.TabIndex = 19;
            this.label6.Text = "Note: Analysis of collected data may take several additional minutes and will not" +
    " begin untill the collection process has completed.";
            // 
            // RunAnalysisWizard
            // 
            this.AcceptButton = this.btnNext;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.btnCancel;
            this.ClientSize = new System.Drawing.Size(669, 406);
            this.Controls.Add(this.button2);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.tabControl);
            this.Controls.Add(this.panel4);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.Fixed3D;
            this.HelpButton = true;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.MinimumSize = new System.Drawing.Size(500, 43);
            this.Name = "RunAnalysisWizard";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Run Analysis";
            this.Load += new System.EventHandler(this.RunAnalysisWizard_Load);
            this.ultraTabPageControl3.ResumeLayout(false);
            this.PanelAnalysisCategories.ResumeLayout(false);
            this.PanelAnalysisCategories.PerformLayout();
            this.ultraTabPageControl8.ResumeLayout(false);
            this.ultraTabPageControl8.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDown1)).EndInit();
            this.panel2.ResumeLayout(false);
            this.panel2.PerformLayout();
            this.panelFilterQuestion1.ResumeLayout(false);
            this.panelFilterQuestion1.PerformLayout();
            this.panelFilterQuestion2.ResumeLayout(false);
            this.panelFilterQuestion2.PerformLayout();
            this.ultraTabPageControl1.ResumeLayout(false);
            this.ultraTabPageControl1.PerformLayout();
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.ultraTabSharedControlsPage1.ResumeLayout(false);
            this.panel4.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.tabControl)).EndInit();
            this.tabControl.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        void OnCurrentThemeChanged(object sender, System.EventArgs e)
        {
            SetPropertiesTheme();
        }

        void SetPropertiesTheme()
        {
            var propertiesThemeManager = new Controls.PropertiesThemeManager();
            propertiesThemeManager.UpdatePropertyTheme(smoothingContentPropertyPage);
            propertiesThemeManager.UpdatePropertyTheme(office2007PropertyPage7);
        }

        #endregion

        private Infragistics.Win.UltraWinTabControl.UltraTabControl ultraTabControl;
        private Infragistics.Win.UltraWinTabControl.UltraTabSharedControlsPage ultraTabSharedControlsPage1;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomPanel  panel1;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomTextBox txtTitle;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomTextBox txtDescription;
        private Infragistics.Win.UltraWinTabControl.UltraTabPageControl ultraTabPageControl1;
        private Infragistics.Win.UltraWinTabControl.UltraTabPageControl ultraTabPageControl2;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomButton btnCancel;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomButton btnNext;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomButton btnBack;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomTextBox textBox1;
        private System.Windows.Forms.TreeView treeView1;
        public FiltersSettingsTab filtersSettings;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomPanel  panel4;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomButton btnAnalysisCategoryWizardCancel;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomButton btnAnalysisCategoryWizardFinish;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomButton btnAnalysisCategoryWizardHelp;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomButton btnAnalysisWizardNext;
        private Infragistics.Win.UltraWinTabControl.UltraTabControl tabControl;
        private Infragistics.Win.UltraWinTabControl.UltraTabSharedControlsPage ultraTabSharedControlsPage2;
        private Infragistics.Win.UltraWinTabControl.UltraTabPageControl ultraTabPageControl8;
        private Controls.Office2007PropertyPage office2007PropertyPage7;
        private Infragistics.Win.UltraWinTabControl.UltraTabPageControl ultraTabPageControl3;
        private Controls.Office2007PropertyPage smoothingContentPropertyPage;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomPanel  panelFilterQuestion1;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomLabel lblFilterQuestion1;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomPanel  panelFilterQuestion2;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomLabel label2;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomLabel label1;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomTextBox txtQueryFromDiagnose;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomComboBox _databaseFilterTypeComboBox;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomLabel label4;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomButton button1;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomButton button2;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomPanel  PanelAnalysisCategories;
        private System.Windows.Forms.TreeView TreeViewCategories;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomLabel labelCategoryTree;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomLabel label5;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomPanel  panel2;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomLabel label3;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomNumericUpDown numericUpDown1;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomLabel label6;
    }
}