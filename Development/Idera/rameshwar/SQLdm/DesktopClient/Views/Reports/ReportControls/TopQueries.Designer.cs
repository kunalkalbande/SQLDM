namespace Idera.SQLdm.DesktopClient.Views.Reports.ReportControls
{
    partial class TopQueries
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(TopQueries));
            Infragistics.Win.UltraWinEditors.DropDownEditorButton dropDownEditorButton2 = new Infragistics.Win.UltraWinEditors.DropDownEditorButton("DropDownList");
            Infragistics.Win.UltraWinEditors.DropDownEditorButton dropDownEditorButton1 = new Infragistics.Win.UltraWinEditors.DropDownEditorButton("DropDownList");
            this.numStatements = new System.Windows.Forms.NumericUpDown();
            this.label1 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.minDuration = new System.Windows.Forms.NumericUpDown();
            this.label5 = new System.Windows.Forms.Label();
            this.minReads = new System.Windows.Forms.NumericUpDown();
            this.label6 = new System.Windows.Forms.Label();
            this.minWrites = new System.Windows.Forms.NumericUpDown();
            this.label7 = new System.Windows.Forms.Label();
            this.minCPU = new System.Windows.Forms.NumericUpDown();
            this.label8 = new System.Windows.Forms.Label();
            this.minExecutions = new System.Windows.Forms.NumericUpDown();
            this.label9 = new System.Windows.Forms.Label();
            this.showStoredProcedures = new System.Windows.Forms.CheckBox();
            this.showSQLStatements = new System.Windows.Forms.CheckBox();
            this.showBatches = new System.Windows.Forms.CheckBox();
            this.sigMode = new System.Windows.Forms.CheckBox();
            this.caseInsensitive = new System.Windows.Forms.CheckBox();
            this.AdvancedFilterLinkLabel = new System.Windows.Forms.LinkLabel();
            this.startHoursLbl = new System.Windows.Forms.Label();
            this.endHoursLbl = new System.Windows.Forms.Label();
            this.startHoursTimeEditor = new Idera.SQLdm.Common.UI.Controls.TimeComboEditor();
            this.endHoursTimeEditor = new Idera.SQLdm.Common.UI.Controls.TimeComboEditor();
            this.panel1.SuspendLayout();
            this.filterPanel.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numStatements)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.minDuration)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.minReads)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.minWrites)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.minCPU)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.minExecutions)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.startHoursTimeEditor)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.endHoursTimeEditor)).BeginInit();
            this.SuspendLayout();
            // 
            // panel1
            // 
            this.panel1.Size = new System.Drawing.Size(752, 188);
            // 
            // reportViewer
            // 
            this.reportViewer.Size = new System.Drawing.Size(752, 416);
            this.reportViewer.ReportExport += ExportDisableHyperlinkEventHandler;
            // 
            // filterPanel
            // 
            this.filterPanel.Controls.Add(this.endHoursTimeEditor);
            this.filterPanel.Controls.Add(this.startHoursTimeEditor);
            this.filterPanel.Controls.Add(this.endHoursLbl);
            this.filterPanel.Controls.Add(this.startHoursLbl);
            this.filterPanel.Controls.Add(this.AdvancedFilterLinkLabel);
            this.filterPanel.Controls.Add(this.caseInsensitive);
            this.filterPanel.Controls.Add(this.label8);
            this.filterPanel.Controls.Add(this.minCPU);
            this.filterPanel.Controls.Add(this.minReads);
            this.filterPanel.Controls.Add(this.numStatements);
            this.filterPanel.Controls.Add(this.minDuration);
            this.filterPanel.Controls.Add(this.label1);
            this.filterPanel.Controls.Add(this.label7);
            this.filterPanel.Controls.Add(this.label4);
            this.filterPanel.Controls.Add(this.label5);
            this.filterPanel.Controls.Add(this.sigMode);
            this.filterPanel.Controls.Add(this.showBatches);
            this.filterPanel.Controls.Add(this.showSQLStatements);
            this.filterPanel.Controls.Add(this.showStoredProcedures);
            this.filterPanel.Controls.Add(this.minExecutions);
            this.filterPanel.Controls.Add(this.label9);
            this.filterPanel.Controls.Add(this.minWrites);
            this.filterPanel.Controls.Add(this.label6);
            this.filterPanel.Size = new System.Drawing.Size(752, 188);
            this.filterPanel.Controls.SetChildIndex(this.rangeInfoLabel, 0);
            this.filterPanel.Controls.SetChildIndex(this.rangeLabel, 0);
            this.filterPanel.Controls.SetChildIndex(this.label6, 0);
            this.filterPanel.Controls.SetChildIndex(this.minWrites, 0);
            this.filterPanel.Controls.SetChildIndex(this.label9, 0);
            this.filterPanel.Controls.SetChildIndex(this.minExecutions, 0);
            this.filterPanel.Controls.SetChildIndex(this.showStoredProcedures, 0);
            this.filterPanel.Controls.SetChildIndex(this.showSQLStatements, 0);
            this.filterPanel.Controls.SetChildIndex(this.showBatches, 0);
            this.filterPanel.Controls.SetChildIndex(this.sigMode, 0);
            this.filterPanel.Controls.SetChildIndex(this.label5, 0);
            this.filterPanel.Controls.SetChildIndex(this.label4, 0);
            this.filterPanel.Controls.SetChildIndex(this.label7, 0);
            this.filterPanel.Controls.SetChildIndex(this.label1, 0);
            this.filterPanel.Controls.SetChildIndex(this.minDuration, 0);
            this.filterPanel.Controls.SetChildIndex(this.numStatements, 0);
            this.filterPanel.Controls.SetChildIndex(this.minReads, 0);
            this.filterPanel.Controls.SetChildIndex(this.minCPU, 0);
            this.filterPanel.Controls.SetChildIndex(this.periodCombo, 0);
            this.filterPanel.Controls.SetChildIndex(this.periodLabel, 0);
            this.filterPanel.Controls.SetChildIndex(this.label8, 0);
            this.filterPanel.Controls.SetChildIndex(this.tagsLabel, 0);
            this.filterPanel.Controls.SetChildIndex(this.instanceLabel, 0);
            this.filterPanel.Controls.SetChildIndex(this.tagsComboBox, 0);
            this.filterPanel.Controls.SetChildIndex(this.sampleLabel, 0);
            this.filterPanel.Controls.SetChildIndex(this.instanceCombo, 0);
            this.filterPanel.Controls.SetChildIndex(this.sampleSizeCombo, 0);
            this.filterPanel.Controls.SetChildIndex(this.caseInsensitive, 0);
            this.filterPanel.Controls.SetChildIndex(this.AdvancedFilterLinkLabel, 0);
            this.filterPanel.Controls.SetChildIndex(this.startHoursLbl, 0);
            this.filterPanel.Controls.SetChildIndex(this.endHoursLbl, 0);
            this.filterPanel.Controls.SetChildIndex(this.startHoursTimeEditor, 0);
            this.filterPanel.Controls.SetChildIndex(this.endHoursTimeEditor, 0);
            // 
            // tagsLabel
            // 
            this.tagsLabel.Location = new System.Drawing.Point(469, 180);
            this.tagsLabel.Visible = false;
            // 
            // periodLabel
            // 
            this.periodLabel.Location = new System.Drawing.Point(10, 37);
            // 
            // sampleLabel
            // 
            this.sampleLabel.Location = new System.Drawing.Point(421, 180);
            this.sampleLabel.Visible = false;
            // 
            // instanceLabel
            // 
            this.instanceLabel.Location = new System.Drawing.Point(10, 10);
            // 
            // periodCombo
            // 
            this.periodCombo.Location = new System.Drawing.Point(57, 34);
            this.periodCombo.Size = new System.Drawing.Size(300, 21);
            this.periodCombo.TabIndex = 1;
            this.periodCombo.SelectedIndexChanged += new System.EventHandler(this.periodCombo_SelectedIndexChanged);
            // 
            // sampleSizeCombo
            // 
            this.sampleSizeCombo.Location = new System.Drawing.Point(472, 177);
            this.sampleSizeCombo.Visible = false;
            // 
            // instanceCombo
            // 
            this.instanceCombo.Location = new System.Drawing.Point(57, 7);
            this.instanceCombo.Size = new System.Drawing.Size(300, 21);
            this.instanceCombo.TabIndex = 0;
            // 
            // tagsComboBox
            // 
            this.tagsComboBox.Location = new System.Drawing.Point(509, 177);
            this.tagsComboBox.Visible = false;
            // 
            // reportInstructionsControl
            // 
            this.reportInstructionsControl.ReportInstructions = resources.GetString("reportInstructionsControl.ReportInstructions");
            this.reportInstructionsControl.Size = new System.Drawing.Size(752, 416);
            // 
            // rangeLabel
            // 
            this.rangeLabel.Location = new System.Drawing.Point(57, 60);
            this.rangeLabel.Size = new System.Drawing.Size(300, 21);
            // 
            // rangeInfoLabel
            // 
            this.rangeInfoLabel.Location = new System.Drawing.Point(10, 64);
            // 
            // numStatements
            // 
            this.numStatements.Location = new System.Drawing.Point(487, 8);
            this.numStatements.Maximum = new decimal(new int[] {
            2147483647,
            0,
            0,
            0});
            this.numStatements.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.numStatements.Name = "numStatements";
            this.numStatements.Size = new System.Drawing.Size(65, 20);
            this.numStatements.TabIndex = 5;
            this.numStatements.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.numStatements.Leave += new System.EventHandler(this.numStatements_Leave);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(383, 11);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(98, 13);
            this.label1.TabIndex = 26;
            this.label1.Text = "Number of Queries:";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(568, 9);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(95, 13);
            this.label4.TabIndex = 31;
            this.label4.Text = "Min. Duration (ms):";
            // 
            // minDuration
            // 
            this.minDuration.Location = new System.Drawing.Point(669, 7);
            this.minDuration.Maximum = new decimal(new int[] {
            2147483647,
            0,
            0,
            0});
            this.minDuration.Name = "minDuration";
            this.minDuration.Size = new System.Drawing.Size(65, 20);
            this.minDuration.TabIndex = 8;
            this.minDuration.Leave += new System.EventHandler(this.minDuration_Leave);
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(568, 61);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(64, 13);
            this.label5.TabIndex = 33;
            this.label5.Text = "Min. Reads:";
            // 
            // minReads
            // 
            this.minReads.Location = new System.Drawing.Point(669, 59);
            this.minReads.Maximum = new decimal(new int[] {
            2147483647,
            0,
            0,
            0});
            this.minReads.Name = "minReads";
            this.minReads.Size = new System.Drawing.Size(65, 20);
            this.minReads.TabIndex = 10;
            this.minReads.Leave += new System.EventHandler(this.minReads_Leave);
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(383, 62);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(63, 13);
            this.label6.TabIndex = 35;
            this.label6.Text = "Min. Writes:";
            // 
            // minWrites
            // 
            this.minWrites.Location = new System.Drawing.Point(487, 60);
            this.minWrites.Maximum = new decimal(new int[] {
            2147483647,
            0,
            0,
            0});
            this.minWrites.Name = "minWrites";
            this.minWrites.Size = new System.Drawing.Size(65, 20);
            this.minWrites.TabIndex = 7;
            this.minWrites.Leave += new System.EventHandler(this.minWrites_Leave);
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(568, 35);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(55, 13);
            this.label7.TabIndex = 37;
            this.label7.Text = "Min. CPU:";
            // 
            // minCPU
            // 
            this.minCPU.Location = new System.Drawing.Point(669, 33);
            this.minCPU.Maximum = new decimal(new int[] {
            2147483647,
            0,
            0,
            0});
            this.minCPU.Name = "minCPU";
            this.minCPU.Size = new System.Drawing.Size(65, 20);
            this.minCPU.TabIndex = 9;
            this.minCPU.Leave += new System.EventHandler(this.minCPU_Leave);
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(153, 64);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(0, 13);
            this.label8.TabIndex = 39;
            // 
            // minExecutions
            // 
            this.minExecutions.Location = new System.Drawing.Point(487, 34);
            this.minExecutions.Maximum = new decimal(new int[] {
            2147483647,
            0,
            0,
            0});
            this.minExecutions.Name = "minExecutions";
            this.minExecutions.Size = new System.Drawing.Size(65, 20);
            this.minExecutions.TabIndex = 6;
            this.minExecutions.Leave += new System.EventHandler(this.minExecutions_Leave);
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Location = new System.Drawing.Point(383, 37);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(85, 13);
            this.label9.TabIndex = 40;
            this.label9.Text = "Min. Executions:";
            // 
            // showStoredProcedures
            // 
            this.showStoredProcedures.AutoSize = true;
            this.showStoredProcedures.Checked = true;
            this.showStoredProcedures.CheckState = System.Windows.Forms.CheckState.Checked;
            this.showStoredProcedures.Location = new System.Drawing.Point(58, 109);
            this.showStoredProcedures.Name = "showStoredProcedures";
            this.showStoredProcedures.Size = new System.Drawing.Size(152, 17);
            this.showStoredProcedures.TabIndex = 4;
            this.showStoredProcedures.Text = "Include Stored Procedures";
            this.showStoredProcedures.UseVisualStyleBackColor = true;
            // 
            // showSQLStatements
            // 
            this.showSQLStatements.AutoSize = true;
            this.showSQLStatements.Checked = true;
            this.showSQLStatements.CheckState = System.Windows.Forms.CheckState.Checked;
            this.showSQLStatements.Location = new System.Drawing.Point(58, 132);
            this.showSQLStatements.Name = "showSQLStatements";
            this.showSQLStatements.Size = new System.Drawing.Size(141, 17);
            this.showSQLStatements.TabIndex = 2;
            this.showSQLStatements.Text = "Include SQL Statements";
            this.showSQLStatements.UseVisualStyleBackColor = true;
            // 
            // showBatches
            // 
            this.showBatches.AutoSize = true;
            this.showBatches.Checked = true;
            this.showBatches.CheckState = System.Windows.Forms.CheckState.Checked;
            this.showBatches.Location = new System.Drawing.Point(58, 86);
            this.showBatches.Name = "showBatches";
            this.showBatches.Size = new System.Drawing.Size(103, 17);
            this.showBatches.TabIndex = 3;
            this.showBatches.Text = "Include Batches";
            this.showBatches.UseVisualStyleBackColor = true;
            // 
            // sigMode
            // 
            this.sigMode.AutoSize = true;
            this.sigMode.Checked = true;
            this.sigMode.CheckState = System.Windows.Forms.CheckState.Checked;
            this.sigMode.Location = new System.Drawing.Point(256, 86);
            this.sigMode.Name = "sigMode";
            this.sigMode.Size = new System.Drawing.Size(101, 17);
            this.sigMode.TabIndex = 11;
            this.sigMode.Text = "Signature Mode";
            this.sigMode.UseVisualStyleBackColor = true;
            // 
            // caseInsensitive
            // 
            this.caseInsensitive.AutoSize = true;
            this.caseInsensitive.Checked = true;
            this.caseInsensitive.CheckState = System.Windows.Forms.CheckState.Checked;
            this.caseInsensitive.Location = new System.Drawing.Point(256, 109);
            this.caseInsensitive.Name = "caseInsensitive";
            this.caseInsensitive.Size = new System.Drawing.Size(103, 17);
            this.caseInsensitive.TabIndex = 41;
            this.caseInsensitive.Text = "Case Insensitive";
            this.caseInsensitive.UseVisualStyleBackColor = true;
            // 
            // AdvancedFilterLinkLabel
            // 
            this.AdvancedFilterLinkLabel.AutoSize = true;
            this.AdvancedFilterLinkLabel.Location = new System.Drawing.Point(385, 113);
            this.AdvancedFilterLinkLabel.Name = "AdvancedFilterLinkLabel";
            this.AdvancedFilterLinkLabel.Size = new System.Drawing.Size(81, 13);
            this.AdvancedFilterLinkLabel.TabIndex = 42;
            this.AdvancedFilterLinkLabel.TabStop = true;
            this.AdvancedFilterLinkLabel.Text = "Advanced Filter";
            this.AdvancedFilterLinkLabel.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.AdvancedFilterLinkLabel_LinkClicked);
            // 
            // startHoursLbl
            // 
            this.startHoursLbl.AutoSize = true;
            this.startHoursLbl.Location = new System.Drawing.Point(383, 88);
            this.startHoursLbl.Name = "startHoursLbl";
            this.startHoursLbl.Size = new System.Drawing.Size(63, 13);
            this.startHoursLbl.TabIndex = 43;
            this.startHoursLbl.Text = "Start Hours:";
            // 
            // endHoursLbl
            // 
            this.endHoursLbl.AutoSize = true;
            this.endHoursLbl.Location = new System.Drawing.Point(568, 88);
            this.endHoursLbl.Name = "endHoursLbl";
            this.endHoursLbl.Size = new System.Drawing.Size(60, 13);
            this.endHoursLbl.TabIndex = 44;
            this.endHoursLbl.Text = "End Hours:";
            // 
            // startHoursTimeEditor
            // 
            dropDownEditorButton2.Key = "DropDownList";
            dropDownEditorButton2.PreferredDropDownSize = new System.Drawing.Size(0, 0);
            this.startHoursTimeEditor.ButtonsRight.Add(dropDownEditorButton2);
            this.startHoursTimeEditor.DropDownButtonDisplayStyle = Infragistics.Win.ButtonDisplayStyle.Never;
            this.startHoursTimeEditor.ListInterval = System.TimeSpan.Parse("01:00:00");
            this.startHoursTimeEditor.Location = new System.Drawing.Point(452, 86);
            this.startHoursTimeEditor.MaskInput = "{time}";
            this.startHoursTimeEditor.Name = "startHoursTimeEditor";
            this.startHoursTimeEditor.Size = new System.Drawing.Size(92, 21);
            this.startHoursTimeEditor.TabIndex = 45;
            this.startHoursTimeEditor.Time = System.TimeSpan.Parse("00:00:00");
            this.startHoursTimeEditor.ValueChanged += new System.EventHandler(this.startHoursTimeEditor_ValueChanged);
            // 
            // endHoursTimeEditor
            // 
            dropDownEditorButton1.Key = "DropDownList";
            dropDownEditorButton1.PreferredDropDownSize = new System.Drawing.Size(0, 0);
            this.endHoursTimeEditor.ButtonsRight.Add(dropDownEditorButton1);
            this.endHoursTimeEditor.DateTime = new System.DateTime(2012, 5, 10, 23, 0, 0, 0);
            this.endHoursTimeEditor.DropDownButtonDisplayStyle = Infragistics.Win.ButtonDisplayStyle.Never;
            this.endHoursTimeEditor.ListInterval = System.TimeSpan.Parse("01:00:00");
            this.endHoursTimeEditor.Location = new System.Drawing.Point(634, 87);
            this.endHoursTimeEditor.MaskInput = "{time}";
            this.endHoursTimeEditor.Name = "endHoursTimeEditor";
            this.endHoursTimeEditor.Size = new System.Drawing.Size(92, 21);
            this.endHoursTimeEditor.TabIndex = 46;
            this.endHoursTimeEditor.Time = System.TimeSpan.Parse("23:00:00");
            this.endHoursTimeEditor.Value = new System.DateTime(2012, 5, 10, 23, 0, 0, 0);
            this.endHoursTimeEditor.ValueChanged += new System.EventHandler(this.endHoursTimeEditor_ValueChanged);
            // 
            // TopQueries
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.Name = "TopQueries";
            this.panel1.ResumeLayout(false);
            this.filterPanel.ResumeLayout(false);
            this.filterPanel.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numStatements)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.minDuration)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.minReads)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.minWrites)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.minCPU)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.minExecutions)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.startHoursTimeEditor)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.endHoursTimeEditor)).EndInit();
            this.ResumeLayout(false);
            MainContentPanel.GotFocus += new System.EventHandler(MainContentPanelGotFocus);
        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.NumericUpDown numStatements;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.NumericUpDown minReads;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.NumericUpDown minDuration;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.NumericUpDown minCPU;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.NumericUpDown minWrites;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.NumericUpDown minExecutions;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.CheckBox showStoredProcedures;
        private System.Windows.Forms.CheckBox showBatches;
        private System.Windows.Forms.CheckBox showSQLStatements;
        private System.Windows.Forms.CheckBox sigMode;
        private System.Windows.Forms.CheckBox caseInsensitive;
        private System.Windows.Forms.LinkLabel AdvancedFilterLinkLabel;
        private Common.UI.Controls.TimeComboEditor endHoursTimeEditor;
        private Common.UI.Controls.TimeComboEditor startHoursTimeEditor;
        private System.Windows.Forms.Label endHoursLbl;
        private System.Windows.Forms.Label startHoursLbl;
    }
}
