namespace Idera.SQLdm.DesktopClient.Views.Reports
{
    partial class ReportControl
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
            Infragistics.Win.Appearance appearance6 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance7 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance8 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance4 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance1 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance2 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance3 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance5 = new Infragistics.Win.Appearance();
            this.splitContainer = new System.Windows.Forms.SplitContainer();
            this.sharedPanel = new System.Windows.Forms.Panel();
            this.showPointLabels = new System.Windows.Forms.CheckBox();
            this.clearFilter = new Infragistics.Win.Misc.UltraButton();
            this.cancelReport = new Infragistics.Win.Misc.UltraButton();
            this.viewReportButton = new Infragistics.Win.Misc.UltraButton();
            this.label1 = new System.Windows.Forms.Label();
            this.filterOptionsPanel = new System.Windows.Forms.Panel();
            this.queryPanel = new System.Windows.Forms.Panel();
            this.chkExpandAll = new System.Windows.Forms.CheckBox();
            this.expand_collapseButton = new Infragistics.Win.Misc.UltraButton();
            this.executionsLabel1 = new System.Windows.Forms.Label();
            this.executionsLabel = new System.Windows.Forms.Label();
            this.executions = new System.Windows.Forms.TextBox();
            this.minTopNLabel = new System.Windows.Forms.Label();
            this.linkLabel1 = new System.Windows.Forms.LinkLabel();
            this.label5 = new System.Windows.Forms.Label();
            this.storedProcs = new System.Windows.Forms.CheckBox();
            this.topN = new System.Windows.Forms.TextBox();
            this.batches = new System.Windows.Forms.CheckBox();
            this.label4 = new System.Windows.Forms.Label();
            this.sqlStmts = new System.Windows.Forms.CheckBox();
            this.minDuration = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.samplePanel = new System.Windows.Forms.Panel();
            this.sampleSizeCombo = new Infragistics.Win.UltraWinEditors.UltraComboEditor();
            this.groupByLabel = new System.Windows.Forms.Label();
            this.periodPanel = new System.Windows.Forms.Panel();
            this.periodCombo = new Infragistics.Win.UltraWinEditors.UltraComboEditor();
            this.periodFilterLabel = new System.Windows.Forms.Label();
            this.tablesPanel = new System.Windows.Forms.Panel();
            this.tablesFilterLabel = new System.Windows.Forms.Label();
            this.tablesButton = new Infragistics.Win.Misc.UltraButton();
            this.tablesTextBox = new System.Windows.Forms.TextBox();
            this.databasePanel = new System.Windows.Forms.Panel();
            this.databaseFilterLabel = new System.Windows.Forms.Label();
            this.databaseButton = new Infragistics.Win.Misc.UltraButton();
            this.databaseTextBox = new System.Windows.Forms.TextBox();
            this.databasesPanel = new System.Windows.Forms.Panel();
            this.databasesFilterLabel = new System.Windows.Forms.Label();
            this.databasesButton = new Infragistics.Win.Misc.UltraButton();
            this.databasesTextBox = new System.Windows.Forms.TextBox();
            this.serverPanel = new System.Windows.Forms.Panel();
            this.serverFilterLabel = new System.Windows.Forms.Label();
            this.instanceCombo = new Infragistics.Win.UltraWinEditors.UltraComboEditor();
            this.serversPanel = new System.Windows.Forms.Panel();
            this.label2 = new System.Windows.Forms.Label();
            this.serversButton = new Infragistics.Win.Misc.UltraButton();
            this.serversTextBox = new System.Windows.Forms.TextBox();
            this.reportViewer = new Microsoft.Reporting.WinForms.ReportViewer();
            this.cancelledLabel = new System.Windows.Forms.Label();
            this.reportViewerErrorLabel = new System.Windows.Forms.Label();
            this.noDataForTableReport = new System.Windows.Forms.Label();
            this.noDataLabel = new System.Windows.Forms.Label();
            this.waitForDataLabel = new System.Windows.Forms.Label();
            this.reportDescriptionControl = new Idera.SQLdm.DesktopClient.Views.Reports.ReportDescriptionControl();
            this.splitContainer.Panel1.SuspendLayout();
            this.splitContainer.Panel2.SuspendLayout();
            this.splitContainer.SuspendLayout();
            this.sharedPanel.SuspendLayout();
            this.filterOptionsPanel.SuspendLayout();
            this.queryPanel.SuspendLayout();
            this.samplePanel.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.sampleSizeCombo)).BeginInit();
            this.periodPanel.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.periodCombo)).BeginInit();
            this.tablesPanel.SuspendLayout();
            this.databasePanel.SuspendLayout();
            this.databasesPanel.SuspendLayout();
            this.serverPanel.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.instanceCombo)).BeginInit();
            this.serversPanel.SuspendLayout();
            this.SuspendLayout();
            // 
            // splitContainer
            // 
            this.splitContainer.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer.FixedPanel = System.Windows.Forms.FixedPanel.Panel1;
            this.splitContainer.IsSplitterFixed = true;
            this.splitContainer.Location = new System.Drawing.Point(0, 0);
            this.splitContainer.Name = "splitContainer";
            this.splitContainer.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // splitContainer.Panel1
            // 
            this.splitContainer.Panel1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(128)))), ((int)(((byte)(255)))));
            this.splitContainer.Panel1.Controls.Add(this.sharedPanel);
            this.splitContainer.Panel1.Controls.Add(this.filterOptionsPanel);
            // 
            // splitContainer.Panel2
            // 
            this.splitContainer.Panel2.Controls.Add(this.reportViewer);
            this.splitContainer.Panel2.Controls.Add(this.cancelledLabel);
            this.splitContainer.Panel2.Controls.Add(this.reportViewerErrorLabel);
            this.splitContainer.Panel2.Controls.Add(this.noDataForTableReport);
            this.splitContainer.Panel2.Controls.Add(this.noDataLabel);
            this.splitContainer.Panel2.Controls.Add(this.waitForDataLabel);
            this.splitContainer.Panel2.Controls.Add(this.reportDescriptionControl);
            this.splitContainer.Size = new System.Drawing.Size(492, 795);
            this.splitContainer.SplitterDistance = 400;
            this.splitContainer.SplitterWidth = 1;
            this.splitContainer.TabIndex = 0;
            // 
            // sharedPanel
            // 
            this.sharedPanel.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(227)))), ((int)(((byte)(239)))), ((int)(((byte)(255)))));
            this.sharedPanel.Controls.Add(this.showPointLabels);
            this.sharedPanel.Controls.Add(this.clearFilter);
            this.sharedPanel.Controls.Add(this.cancelReport);
            this.sharedPanel.Controls.Add(this.viewReportButton);
            this.sharedPanel.Controls.Add(this.label1);
            this.sharedPanel.Dock = System.Windows.Forms.DockStyle.Top;
            this.sharedPanel.Location = new System.Drawing.Point(0, 316);
            this.sharedPanel.Name = "sharedPanel";
            this.sharedPanel.Size = new System.Drawing.Size(492, 37);
            this.sharedPanel.TabIndex = 1;
            // 
            // showPointLabels
            // 
            this.showPointLabels.AutoSize = true;
            this.showPointLabels.Location = new System.Drawing.Point(12, 13);
            this.showPointLabels.Name = "showPointLabels";
            this.showPointLabels.Size = new System.Drawing.Size(114, 17);
            this.showPointLabels.TabIndex = 4;
            this.showPointLabels.Text = "Show Point Labels";
            this.showPointLabels.UseVisualStyleBackColor = true;
            // 
            // clearFilter
            // 
            this.clearFilter.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            appearance6.Image = global::Idera.SQLdm.DesktopClient.Properties.Resources.funnel_delete;
            this.clearFilter.Appearance = appearance6;
            this.clearFilter.Location = new System.Drawing.Point(289, 8);
            this.clearFilter.Name = "clearFilter";
            this.clearFilter.Size = new System.Drawing.Size(94, 27);
            this.clearFilter.TabIndex = 3;
            this.clearFilter.Text = "Clear Filter";
            this.clearFilter.Click += new System.EventHandler(this.clearFilter_Click);
            // 
            // cancelReport
            // 
            this.cancelReport.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            appearance7.Image = global::Idera.SQLdm.DesktopClient.Properties.Resources.document_stop;
            this.cancelReport.Appearance = appearance7;
            this.cancelReport.Location = new System.Drawing.Point(189, 8);
            this.cancelReport.Name = "cancelReport";
            this.cancelReport.Size = new System.Drawing.Size(94, 27);
            this.cancelReport.TabIndex = 1;
            this.cancelReport.Text = "Cancel";
            this.cancelReport.Click += new System.EventHandler(this.cancelReport_Click);
            // 
            // viewReportButton
            // 
            this.viewReportButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            appearance8.Image = global::Idera.SQLdm.DesktopClient.Properties.Resources.RunReport;
            this.viewReportButton.Appearance = appearance8;
            this.viewReportButton.Location = new System.Drawing.Point(389, 8);
            this.viewReportButton.Name = "viewReportButton";
            this.viewReportButton.Size = new System.Drawing.Size(94, 27);
            this.viewReportButton.TabIndex = 2;
            this.viewReportButton.Text = "Run Report";
            this.viewReportButton.Click += new System.EventHandler(this.viewReportButton_Click);
            // 
            // label1
            // 
            this.label1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.label1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(101)))), ((int)(((byte)(147)))), ((int)(((byte)(207)))));
            this.label1.Location = new System.Drawing.Point(8, 4);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(475, 1);
            this.label1.TabIndex = 0;
            // 
            // filterOptionsPanel
            // 
            this.filterOptionsPanel.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(227)))), ((int)(((byte)(239)))), ((int)(((byte)(255)))));
            this.filterOptionsPanel.Controls.Add(this.queryPanel);
            this.filterOptionsPanel.Controls.Add(this.samplePanel);
            this.filterOptionsPanel.Controls.Add(this.periodPanel);
            this.filterOptionsPanel.Controls.Add(this.tablesPanel);
            this.filterOptionsPanel.Controls.Add(this.databasePanel);
            this.filterOptionsPanel.Controls.Add(this.databasesPanel);
            this.filterOptionsPanel.Controls.Add(this.serverPanel);
            this.filterOptionsPanel.Controls.Add(this.serversPanel);
            this.filterOptionsPanel.Dock = System.Windows.Forms.DockStyle.Top;
            this.filterOptionsPanel.Location = new System.Drawing.Point(0, 0);
            this.filterOptionsPanel.Name = "filterOptionsPanel";
            this.filterOptionsPanel.Padding = new System.Windows.Forms.Padding(0, 8, 0, 0);
            this.filterOptionsPanel.Size = new System.Drawing.Size(492, 316);
            this.filterOptionsPanel.TabIndex = 0;
            // 
            // queryPanel
            // 
            this.queryPanel.AutoSize = true;
            this.queryPanel.BackColor = System.Drawing.Color.Transparent;
            this.queryPanel.Controls.Add(this.chkExpandAll);
            this.queryPanel.Controls.Add(this.expand_collapseButton);
            this.queryPanel.Controls.Add(this.executionsLabel1);
            this.queryPanel.Controls.Add(this.executionsLabel);
            this.queryPanel.Controls.Add(this.executions);
            this.queryPanel.Controls.Add(this.minTopNLabel);
            this.queryPanel.Controls.Add(this.linkLabel1);
            this.queryPanel.Controls.Add(this.label5);
            this.queryPanel.Controls.Add(this.storedProcs);
            this.queryPanel.Controls.Add(this.topN);
            this.queryPanel.Controls.Add(this.batches);
            this.queryPanel.Controls.Add(this.label4);
            this.queryPanel.Controls.Add(this.sqlStmts);
            this.queryPanel.Controls.Add(this.minDuration);
            this.queryPanel.Controls.Add(this.label3);
            this.queryPanel.Dock = System.Windows.Forms.DockStyle.Top;
            this.queryPanel.Location = new System.Drawing.Point(0, 197);
            this.queryPanel.Name = "queryPanel";
            this.queryPanel.Size = new System.Drawing.Size(492, 88);
            this.queryPanel.TabIndex = 7;
            // 
            // chkExpandAll
            // 
            this.chkExpandAll.AutoSize = true;
            this.chkExpandAll.Location = new System.Drawing.Point(286, 66);
            this.chkExpandAll.Name = "chkExpandAll";
            this.chkExpandAll.Size = new System.Drawing.Size(137, 17);
            this.chkExpandAll.TabIndex = 16;
            this.chkExpandAll.Text = "Expand All Subsections";
            this.chkExpandAll.UseVisualStyleBackColor = true;
            this.chkExpandAll.CheckedChanged += new System.EventHandler(this.expand_collapseButton_Click);
            // 
            // expand_collapseButton
            // 
            this.expand_collapseButton.Anchor = System.Windows.Forms.AnchorStyles.Right;
            appearance4.Image = global::Idera.SQLdm.DesktopClient.Properties.Resources.TaskShow_16;
            this.expand_collapseButton.Appearance = appearance4;
            this.expand_collapseButton.Enabled = false;
            this.expand_collapseButton.Location = new System.Drawing.Point(389, 58);
            this.expand_collapseButton.Name = "expand_collapseButton";
            this.expand_collapseButton.Size = new System.Drawing.Size(94, 27);
            this.expand_collapseButton.TabIndex = 15;
            this.expand_collapseButton.Text = "Expand All";
            this.expand_collapseButton.Visible = false;
            this.expand_collapseButton.Click += new System.EventHandler(this.expand_collapseButton_Click);
            // 
            // executionsLabel1
            // 
            this.executionsLabel1.AutoSize = true;
            this.executionsLabel1.Location = new System.Drawing.Point(373, 38);
            this.executionsLabel1.Name = "executionsLabel1";
            this.executionsLabel1.Size = new System.Drawing.Size(65, 13);
            this.executionsLabel1.TabIndex = 14;
            this.executionsLabel1.Text = "Execution(s)";
            // 
            // executionsLabel
            // 
            this.executionsLabel.AutoSize = true;
            this.executionsLabel.Location = new System.Drawing.Point(283, 38);
            this.executionsLabel.Name = "executionsLabel";
            this.executionsLabel.Size = new System.Drawing.Size(34, 13);
            this.executionsLabel.TabIndex = 12;
            this.executionsLabel.Text = "> or =";
            // 
            // executions
            // 
            this.executions.Location = new System.Drawing.Point(323, 35);
            this.executions.Name = "executions";
            this.executions.Size = new System.Drawing.Size(44, 20);
            this.executions.TabIndex = 11;
            this.executions.Text = "0";
            this.executions.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.executions.TextChanged += new System.EventHandler(this.WorstPerformingFilterTextChanged);
            this.executions.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.KeyPress_NumbersOnly);
            // 
            // minTopNLabel
            // 
            this.minTopNLabel.AutoSize = true;
            this.minTopNLabel.Location = new System.Drawing.Point(189, 67);
            this.minTopNLabel.Name = "minTopNLabel";
            this.minTopNLabel.Size = new System.Drawing.Size(63, 13);
            this.minTopNLabel.TabIndex = 10;
            this.minTopNLabel.Text = "(1 or higher)";
            // 
            // linkLabel1
            // 
            this.linkLabel1.AutoSize = true;
            this.linkLabel1.Location = new System.Drawing.Point(405, 9);
            this.linkLabel1.Name = "linkLabel1";
            this.linkLabel1.Size = new System.Drawing.Size(65, 13);
            this.linkLabel1.TabIndex = 4;
            this.linkLabel1.TabStop = true;
            this.linkLabel1.Text = "Advanced...";
            this.linkLabel1.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.linkLabel1_LinkClicked);
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(9, 9);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(66, 13);
            this.label5.TabIndex = 0;
            this.label5.Text = "Event types:";
            // 
            // storedProcs
            // 
            this.storedProcs.AutoSize = true;
            this.storedProcs.Checked = true;
            this.storedProcs.CheckState = System.Windows.Forms.CheckState.Checked;
            this.storedProcs.Location = new System.Drawing.Point(286, 8);
            this.storedProcs.Name = "storedProcs";
            this.storedProcs.Size = new System.Drawing.Size(113, 17);
            this.storedProcs.TabIndex = 3;
            this.storedProcs.Text = "Stored procedures";
            this.storedProcs.UseVisualStyleBackColor = true;
            // 
            // topN
            // 
            this.topN.Location = new System.Drawing.Point(142, 64);
            this.topN.MaxLength = 2;
            this.topN.Name = "topN";
            this.topN.Size = new System.Drawing.Size(44, 20);
            this.topN.TabIndex = 8;
            this.topN.Text = "10";
            this.topN.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.topN.TextChanged += new System.EventHandler(this.WorstPerformingFilterTextChanged);
            this.topN.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.KeyPress_NumbersOnly);
            // 
            // batches
            // 
            this.batches.AutoSize = true;
            this.batches.Checked = true;
            this.batches.CheckState = System.Windows.Forms.CheckState.Checked;
            this.batches.Location = new System.Drawing.Point(192, 8);
            this.batches.Name = "batches";
            this.batches.Size = new System.Drawing.Size(88, 17);
            this.batches.TabIndex = 2;
            this.batches.Text = "SQL batches";
            this.batches.UseVisualStyleBackColor = true;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(9, 67);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(127, 13);
            this.label4.TabIndex = 7;
            this.label4.Text = "Top N queries per server:";
            // 
            // sqlStmts
            // 
            this.sqlStmts.AutoSize = true;
            this.sqlStmts.Checked = true;
            this.sqlStmts.CheckState = System.Windows.Forms.CheckState.Checked;
            this.sqlStmts.Location = new System.Drawing.Point(85, 8);
            this.sqlStmts.Name = "sqlStmts";
            this.sqlStmts.Size = new System.Drawing.Size(101, 17);
            this.sqlStmts.TabIndex = 1;
            this.sqlStmts.Text = "SQL statements";
            this.sqlStmts.UseVisualStyleBackColor = true;
            // 
            // minDuration
            // 
            this.minDuration.Location = new System.Drawing.Point(142, 35);
            this.minDuration.MaxLength = 6;
            this.minDuration.Name = "minDuration";
            this.minDuration.Size = new System.Drawing.Size(44, 20);
            this.minDuration.TabIndex = 6;
            this.minDuration.Text = "500";
            this.minDuration.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.minDuration.TextChanged += new System.EventHandler(this.WorstPerformingFilterTextChanged);
            this.minDuration.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.KeyPress_NumbersOnly);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(9, 38);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(114, 13);
            this.label3.TabIndex = 5;
            this.label3.Text = "Minimum duration (ms):";
            // 
            // samplePanel
            // 
            this.samplePanel.AutoSize = true;
            this.samplePanel.BackColor = System.Drawing.Color.Transparent;
            this.samplePanel.Controls.Add(this.sampleSizeCombo);
            this.samplePanel.Controls.Add(this.groupByLabel);
            this.samplePanel.Dock = System.Windows.Forms.DockStyle.Top;
            this.samplePanel.Location = new System.Drawing.Point(0, 170);
            this.samplePanel.Name = "samplePanel";
            this.samplePanel.Size = new System.Drawing.Size(492, 27);
            this.samplePanel.TabIndex = 6;
            // 
            // sampleSizeCombo
            // 
            this.sampleSizeCombo.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.sampleSizeCombo.DropDownStyle = Infragistics.Win.DropDownStyle.DropDownList;
            this.sampleSizeCombo.Location = new System.Drawing.Point(85, 3);
            this.sampleSizeCombo.Name = "sampleSizeCombo";
            this.sampleSizeCombo.Size = new System.Drawing.Size(399, 21);
            this.sampleSizeCombo.TabIndex = 1;
            // 
            // groupByLabel
            // 
            this.groupByLabel.AutoSize = true;
            this.groupByLabel.Location = new System.Drawing.Point(9, 6);
            this.groupByLabel.Name = "groupByLabel";
            this.groupByLabel.Size = new System.Drawing.Size(68, 13);
            this.groupByLabel.TabIndex = 0;
            this.groupByLabel.Text = "Sample Size:";
            // 
            // periodPanel
            // 
            this.periodPanel.AutoSize = true;
            this.periodPanel.BackColor = System.Drawing.Color.Transparent;
            this.periodPanel.Controls.Add(this.periodCombo);
            this.periodPanel.Controls.Add(this.periodFilterLabel);
            this.periodPanel.Dock = System.Windows.Forms.DockStyle.Top;
            this.periodPanel.Location = new System.Drawing.Point(0, 143);
            this.periodPanel.Name = "periodPanel";
            this.periodPanel.Size = new System.Drawing.Size(492, 27);
            this.periodPanel.TabIndex = 5;
            // 
            // periodCombo
            // 
            this.periodCombo.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.periodCombo.DropDownStyle = Infragistics.Win.DropDownStyle.DropDownList;
            this.periodCombo.Enabled = false;
            this.periodCombo.Location = new System.Drawing.Point(85, 3);
            this.periodCombo.Name = "periodCombo";
            this.periodCombo.Size = new System.Drawing.Size(399, 21);
            this.periodCombo.TabIndex = 1;
            this.periodCombo.SelectionChanged += new System.EventHandler(this.periodCombo_SelectionChanged);
            // 
            // periodFilterLabel
            // 
            this.periodFilterLabel.AutoSize = true;
            this.periodFilterLabel.Location = new System.Drawing.Point(9, 6);
            this.periodFilterLabel.Name = "periodFilterLabel";
            this.periodFilterLabel.Size = new System.Drawing.Size(40, 13);
            this.periodFilterLabel.TabIndex = 0;
            this.periodFilterLabel.Text = "Period:";
            // 
            // tablesPanel
            // 
            this.tablesPanel.AutoSize = true;
            this.tablesPanel.BackColor = System.Drawing.Color.Transparent;
            this.tablesPanel.Controls.Add(this.tablesFilterLabel);
            this.tablesPanel.Controls.Add(this.tablesButton);
            this.tablesPanel.Controls.Add(this.tablesTextBox);
            this.tablesPanel.Dock = System.Windows.Forms.DockStyle.Top;
            this.tablesPanel.Location = new System.Drawing.Point(0, 116);
            this.tablesPanel.Name = "tablesPanel";
            this.tablesPanel.Size = new System.Drawing.Size(492, 27);
            this.tablesPanel.TabIndex = 4;
            // 
            // tablesFilterLabel
            // 
            this.tablesFilterLabel.AutoSize = true;
            this.tablesFilterLabel.Location = new System.Drawing.Point(9, 6);
            this.tablesFilterLabel.Name = "tablesFilterLabel";
            this.tablesFilterLabel.Size = new System.Drawing.Size(42, 13);
            this.tablesFilterLabel.TabIndex = 2;
            this.tablesFilterLabel.Text = "Tables:";
            // 
            // tablesButton
            // 
            this.tablesButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            appearance1.TextHAlignAsString = "Center";
            appearance1.TextVAlignAsString = "Middle";
            this.tablesButton.Appearance = appearance1;
            this.tablesButton.Location = new System.Drawing.Point(459, 1);
            this.tablesButton.Name = "tablesButton";
            this.tablesButton.Size = new System.Drawing.Size(24, 23);
            this.tablesButton.TabIndex = 0;
            this.tablesButton.Text = "...";
            this.tablesButton.Click += new System.EventHandler(this.tablesButton_Click);
            // 
            // tablesTextBox
            // 
            this.tablesTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.tablesTextBox.BackColor = System.Drawing.SystemColors.Control;
            this.tablesTextBox.Location = new System.Drawing.Point(85, 3);
            this.tablesTextBox.Name = "tablesTextBox";
            this.tablesTextBox.ReadOnly = true;
            this.tablesTextBox.Size = new System.Drawing.Size(368, 20);
            this.tablesTextBox.TabIndex = 1;
            // 
            // databasePanel
            // 
            this.databasePanel.AutoSize = true;
            this.databasePanel.BackColor = System.Drawing.Color.Transparent;
            this.databasePanel.Controls.Add(this.databaseFilterLabel);
            this.databasePanel.Controls.Add(this.databaseButton);
            this.databasePanel.Controls.Add(this.databaseTextBox);
            this.databasePanel.Dock = System.Windows.Forms.DockStyle.Top;
            this.databasePanel.Location = new System.Drawing.Point(0, 89);
            this.databasePanel.Name = "databasePanel";
            this.databasePanel.Size = new System.Drawing.Size(492, 27);
            this.databasePanel.TabIndex = 3;
            // 
            // databaseFilterLabel
            // 
            this.databaseFilterLabel.AutoSize = true;
            this.databaseFilterLabel.Location = new System.Drawing.Point(9, 6);
            this.databaseFilterLabel.Name = "databaseFilterLabel";
            this.databaseFilterLabel.Size = new System.Drawing.Size(56, 13);
            this.databaseFilterLabel.TabIndex = 3;
            this.databaseFilterLabel.Text = "Database:";
            // 
            // databaseButton
            // 
            this.databaseButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            appearance2.TextHAlignAsString = "Center";
            appearance2.TextVAlignAsString = "Middle";
            this.databaseButton.Appearance = appearance2;
            this.databaseButton.Location = new System.Drawing.Point(459, 1);
            this.databaseButton.Name = "databaseButton";
            this.databaseButton.Size = new System.Drawing.Size(24, 23);
            this.databaseButton.TabIndex = 0;
            this.databaseButton.Text = "...";
            this.databaseButton.Click += new System.EventHandler(this.databaseButton_Click);
            // 
            // databaseTextBox
            // 
            this.databaseTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.databaseTextBox.BackColor = System.Drawing.SystemColors.Control;
            this.databaseTextBox.Location = new System.Drawing.Point(85, 3);
            this.databaseTextBox.Name = "databaseTextBox";
            this.databaseTextBox.ReadOnly = true;
            this.databaseTextBox.Size = new System.Drawing.Size(368, 20);
            this.databaseTextBox.TabIndex = 1;
            // 
            // databasesPanel
            // 
            this.databasesPanel.AutoSize = true;
            this.databasesPanel.BackColor = System.Drawing.Color.Transparent;
            this.databasesPanel.Controls.Add(this.databasesFilterLabel);
            this.databasesPanel.Controls.Add(this.databasesButton);
            this.databasesPanel.Controls.Add(this.databasesTextBox);
            this.databasesPanel.Dock = System.Windows.Forms.DockStyle.Top;
            this.databasesPanel.Location = new System.Drawing.Point(0, 62);
            this.databasesPanel.Name = "databasesPanel";
            this.databasesPanel.Size = new System.Drawing.Size(492, 27);
            this.databasesPanel.TabIndex = 2;
            // 
            // databasesFilterLabel
            // 
            this.databasesFilterLabel.AutoSize = true;
            this.databasesFilterLabel.Location = new System.Drawing.Point(9, 6);
            this.databasesFilterLabel.Name = "databasesFilterLabel";
            this.databasesFilterLabel.Size = new System.Drawing.Size(61, 13);
            this.databasesFilterLabel.TabIndex = 2;
            this.databasesFilterLabel.Text = "Databases:";
            // 
            // databasesButton
            // 
            this.databasesButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            appearance3.TextHAlignAsString = "Center";
            appearance3.TextVAlignAsString = "Middle";
            this.databasesButton.Appearance = appearance3;
            this.databasesButton.Location = new System.Drawing.Point(459, 1);
            this.databasesButton.Name = "databasesButton";
            this.databasesButton.Size = new System.Drawing.Size(24, 23);
            this.databasesButton.TabIndex = 0;
            this.databasesButton.Text = "...";
            this.databasesButton.Click += new System.EventHandler(this.databasesButton_Click);
            // 
            // databasesTextBox
            // 
            this.databasesTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.databasesTextBox.BackColor = System.Drawing.SystemColors.Control;
            this.databasesTextBox.Location = new System.Drawing.Point(85, 3);
            this.databasesTextBox.Name = "databasesTextBox";
            this.databasesTextBox.ReadOnly = true;
            this.databasesTextBox.Size = new System.Drawing.Size(368, 20);
            this.databasesTextBox.TabIndex = 1;
            // 
            // serverPanel
            // 
            this.serverPanel.AutoSize = true;
            this.serverPanel.BackColor = System.Drawing.Color.Transparent;
            this.serverPanel.Controls.Add(this.serverFilterLabel);
            this.serverPanel.Controls.Add(this.instanceCombo);
            this.serverPanel.Dock = System.Windows.Forms.DockStyle.Top;
            this.serverPanel.Location = new System.Drawing.Point(0, 35);
            this.serverPanel.Name = "serverPanel";
            this.serverPanel.Size = new System.Drawing.Size(492, 27);
            this.serverPanel.TabIndex = 1;
            // 
            // serverFilterLabel
            // 
            this.serverFilterLabel.AutoSize = true;
            this.serverFilterLabel.Location = new System.Drawing.Point(9, 6);
            this.serverFilterLabel.Name = "serverFilterLabel";
            this.serverFilterLabel.Size = new System.Drawing.Size(41, 13);
            this.serverFilterLabel.TabIndex = 3;
            this.serverFilterLabel.Text = "Server:";
            // 
            // instanceCombo
            // 
            this.instanceCombo.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.instanceCombo.DropDownStyle = Infragistics.Win.DropDownStyle.DropDownList;
            this.instanceCombo.Location = new System.Drawing.Point(85, 3);
            this.instanceCombo.Name = "instanceCombo";
            this.instanceCombo.ShowOverflowIndicator = true;
            this.instanceCombo.Size = new System.Drawing.Size(398, 21);
            this.instanceCombo.SortStyle = Infragistics.Win.ValueListSortStyle.AscendingByValue;
            this.instanceCombo.TabIndex = 2;
            this.instanceCombo.ValueChanged += new System.EventHandler(this.instanceCombo_ValueChanged);
            this.instanceCombo.BeforeDropDown += new System.ComponentModel.CancelEventHandler(this.instanceCombo_BeforeDropDown);
            // 
            // serversPanel
            // 
            this.serversPanel.AutoSize = true;
            this.serversPanel.BackColor = System.Drawing.Color.Transparent;
            this.serversPanel.Controls.Add(this.label2);
            this.serversPanel.Controls.Add(this.serversButton);
            this.serversPanel.Controls.Add(this.serversTextBox);
            this.serversPanel.Dock = System.Windows.Forms.DockStyle.Top;
            this.serversPanel.Location = new System.Drawing.Point(0, 8);
            this.serversPanel.Name = "serversPanel";
            this.serversPanel.Size = new System.Drawing.Size(492, 27);
            this.serversPanel.TabIndex = 0;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(9, 6);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(46, 13);
            this.label2.TabIndex = 4;
            this.label2.Text = "Servers:";
            // 
            // serversButton
            // 
            this.serversButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            appearance5.TextHAlignAsString = "Center";
            appearance5.TextVAlignAsString = "Middle";
            this.serversButton.Appearance = appearance5;
            this.serversButton.Location = new System.Drawing.Point(459, 1);
            this.serversButton.Name = "serversButton";
            this.serversButton.Size = new System.Drawing.Size(24, 23);
            this.serversButton.TabIndex = 0;
            this.serversButton.Text = "...";
            this.serversButton.Click += new System.EventHandler(this.serversButton_Click);
            // 
            // serversTextBox
            // 
            this.serversTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.serversTextBox.BackColor = System.Drawing.SystemColors.Control;
            this.serversTextBox.Location = new System.Drawing.Point(85, 3);
            this.serversTextBox.Name = "serversTextBox";
            this.serversTextBox.ReadOnly = true;
            this.serversTextBox.Size = new System.Drawing.Size(368, 20);
            this.serversTextBox.TabIndex = 1;
            // 
            // reportViewer
            // 
            this.reportViewer.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.reportViewer.Dock = System.Windows.Forms.DockStyle.Fill;
            this.reportViewer.LocalReport.ReportEmbeddedResource = "";
            this.reportViewer.Location = new System.Drawing.Point(0, 275);
            this.reportViewer.Name = "reportViewer";
            this.reportViewer.ShowRefreshButton = false;
            this.reportViewer.Size = new System.Drawing.Size(492, 119);
            this.reportViewer.TabIndex = 4;
            this.reportViewer.Visible = false;
            this.reportViewer.RenderingComplete += new Microsoft.Reporting.WinForms.RenderingCompleteEventHandler(this.reportViewer_RenderingComplete);
            this.reportViewer.RenderingBegin += new System.ComponentModel.CancelEventHandler(this.reportViewer_RenderingBegin);
            this.reportViewer.ReportRefresh += new System.ComponentModel.CancelEventHandler(this.reportViewer_ReportRefresh);
            this.reportViewer.ReportError += new Microsoft.Reporting.WinForms.ReportErrorEventHandler(this.reportViewer_ReportError);
            this.reportViewer.Drillthrough += new Microsoft.Reporting.WinForms.DrillthroughEventHandler(this.reportViewer_Drillthrough);
            // 
            // cancelledLabel
            // 
            this.cancelledLabel.Dock = System.Windows.Forms.DockStyle.Top;
            this.cancelledLabel.Location = new System.Drawing.Point(0, 220);
            this.cancelledLabel.Name = "cancelledLabel";
            this.cancelledLabel.Size = new System.Drawing.Size(492, 55);
            this.cancelledLabel.TabIndex = 3;
            this.cancelledLabel.Text = "Report processing was cancelled.";
            this.cancelledLabel.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.cancelledLabel.Visible = false;
            // 
            // reportViewerErrorLabel
            // 
            this.reportViewerErrorLabel.Dock = System.Windows.Forms.DockStyle.Top;
            this.reportViewerErrorLabel.Location = new System.Drawing.Point(0, 165);
            this.reportViewerErrorLabel.Name = "reportViewerErrorLabel";
            this.reportViewerErrorLabel.Size = new System.Drawing.Size(492, 55);
            this.reportViewerErrorLabel.TabIndex = 2;
            this.reportViewerErrorLabel.Text = "The report generator experienced an internal error.  This may indicate that the q" +
                "uery did not return enough data for the report.  ";
            this.reportViewerErrorLabel.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.reportViewerErrorLabel.Visible = false;
            // 
            // noDataForTableReport
            // 
            this.noDataForTableReport.Dock = System.Windows.Forms.DockStyle.Top;
            this.noDataForTableReport.Location = new System.Drawing.Point(0, 110);
            this.noDataForTableReport.Name = "noDataForTableReport";
            this.noDataForTableReport.Size = new System.Drawing.Size(492, 55);
            this.noDataForTableReport.TabIndex = 5;
            this.noDataForTableReport.Text = "No data was returned for the specified query parameters.  Note that a given table" +
                " must be monitored for two days to collect enough data to chart.";
            this.noDataForTableReport.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.noDataForTableReport.Visible = false;
            // 
            // noDataLabel
            // 
            this.noDataLabel.Dock = System.Windows.Forms.DockStyle.Top;
            this.noDataLabel.Location = new System.Drawing.Point(0, 55);
            this.noDataLabel.Name = "noDataLabel";
            this.noDataLabel.Size = new System.Drawing.Size(492, 55);
            this.noDataLabel.TabIndex = 1;
            this.noDataLabel.Text = "No data was returned for the specified query parameters.";
            this.noDataLabel.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.noDataLabel.Visible = false;
            // 
            // waitForDataLabel
            // 
            this.waitForDataLabel.Dock = System.Windows.Forms.DockStyle.Top;
            this.waitForDataLabel.Location = new System.Drawing.Point(0, 0);
            this.waitForDataLabel.Name = "waitForDataLabel";
            this.waitForDataLabel.Size = new System.Drawing.Size(492, 55);
            this.waitForDataLabel.TabIndex = 0;
            this.waitForDataLabel.Text = "Please wait while the report data is retrieved.";
            this.waitForDataLabel.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.waitForDataLabel.Visible = false;
            // 
            // reportDescriptionControl
            // 
            this.reportDescriptionControl.BackColor = System.Drawing.Color.White;
            this.reportDescriptionControl.DescriptionText = "< Report description >";
            this.reportDescriptionControl.Dock = System.Windows.Forms.DockStyle.Fill;
            this.reportDescriptionControl.GettingStartedText = "< List steps to create the report >";
            this.reportDescriptionControl.Location = new System.Drawing.Point(0, 0);
            this.reportDescriptionControl.Name = "reportDescriptionControl";
            this.reportDescriptionControl.Size = new System.Drawing.Size(492, 394);
            this.reportDescriptionControl.TabIndex = 4;
            this.reportDescriptionControl.TitleText = "< Report Title >";
            this.reportDescriptionControl.Visible = false;
            // 
            // ReportControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.White;
            this.Controls.Add(this.splitContainer);
            this.Name = "ReportControl";
            this.Size = new System.Drawing.Size(492, 795);
            this.splitContainer.Panel1.ResumeLayout(false);
            this.splitContainer.Panel2.ResumeLayout(false);
            this.splitContainer.ResumeLayout(false);
            this.sharedPanel.ResumeLayout(false);
            this.sharedPanel.PerformLayout();
            this.filterOptionsPanel.ResumeLayout(false);
            this.filterOptionsPanel.PerformLayout();
            this.queryPanel.ResumeLayout(false);
            this.queryPanel.PerformLayout();
            this.samplePanel.ResumeLayout(false);
            this.samplePanel.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.sampleSizeCombo)).EndInit();
            this.periodPanel.ResumeLayout(false);
            this.periodPanel.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.periodCombo)).EndInit();
            this.tablesPanel.ResumeLayout(false);
            this.tablesPanel.PerformLayout();
            this.databasePanel.ResumeLayout(false);
            this.databasePanel.PerformLayout();
            this.databasesPanel.ResumeLayout(false);
            this.databasesPanel.PerformLayout();
            this.serverPanel.ResumeLayout(false);
            this.serverPanel.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.instanceCombo)).EndInit();
            this.serversPanel.ResumeLayout(false);
            this.serversPanel.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.SplitContainer splitContainer;
        private System.Windows.Forms.Panel filterOptionsPanel;
        private Microsoft.Reporting.WinForms.ReportViewer reportViewer;
        private System.Windows.Forms.TextBox databasesTextBox;
        private System.Windows.Forms.Panel serversPanel;
        private System.Windows.Forms.Panel databasesPanel;
        private System.Windows.Forms.Panel tablesPanel;
        private System.Windows.Forms.TextBox tablesTextBox;
        private System.Windows.Forms.Panel serverPanel;
        private System.Windows.Forms.Panel sharedPanel;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label periodFilterLabel;
        private System.Windows.Forms.Label groupByLabel;
        private System.Windows.Forms.Panel databasePanel;
        private System.Windows.Forms.TextBox databaseTextBox;
        private System.Windows.Forms.Label waitForDataLabel;
        private System.Windows.Forms.Label noDataLabel;
        private Infragistics.Win.UltraWinEditors.UltraComboEditor sampleSizeCombo;
        private Infragistics.Win.UltraWinEditors.UltraComboEditor periodCombo;
        private ReportDescriptionControl reportDescriptionControl;
        private System.Windows.Forms.Label reportViewerErrorLabel;
        private System.Windows.Forms.Panel periodPanel;
        private System.Windows.Forms.Panel queryPanel;
        private System.Windows.Forms.CheckBox sqlStmts;
        private System.Windows.Forms.CheckBox storedProcs;
        private System.Windows.Forms.CheckBox batches;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.TextBox minDuration;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox topN;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.LinkLabel linkLabel1;
        private System.Windows.Forms.Panel samplePanel;
        private System.Windows.Forms.Label cancelledLabel;
        private Infragistics.Win.Misc.UltraButton viewReportButton;
        private Infragistics.Win.Misc.UltraButton cancelReport;
        private Infragistics.Win.Misc.UltraButton serversButton;
        private Infragistics.Win.Misc.UltraButton databasesButton;
        private Infragistics.Win.Misc.UltraButton databaseButton;
        private Infragistics.Win.Misc.UltraButton tablesButton;
        private Infragistics.Win.Misc.UltraButton clearFilter;
        private System.Windows.Forms.Label noDataForTableReport;
        private Infragistics.Win.UltraWinEditors.UltraComboEditor instanceCombo;
        private System.Windows.Forms.Label serverFilterLabel;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox serversTextBox;
        private System.Windows.Forms.Label databasesFilterLabel;
        private System.Windows.Forms.Label tablesFilterLabel;
        private System.Windows.Forms.Label databaseFilterLabel;
        private System.Windows.Forms.Label minTopNLabel;
        private System.Windows.Forms.TextBox executions;
        private System.Windows.Forms.Label executionsLabel;
        private System.Windows.Forms.Label executionsLabel1;
        private Infragistics.Win.Misc.UltraButton expand_collapseButton;
        private System.Windows.Forms.CheckBox showPointLabels;
        private System.Windows.Forms.CheckBox chkExpandAll;
    }
}
