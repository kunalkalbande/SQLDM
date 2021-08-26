namespace Idera.SQLdm.DesktopClient.Views.Reports.ReportControls
{
    partial class TopDatabaseApplications
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(TopDatabaseApplications));
            Infragistics.Win.UltraWinEditors.DropDownEditorButton dropDownEditorButton2 = new Infragistics.Win.UltraWinEditors.DropDownEditorButton("DropDownList");
            Infragistics.Win.UltraWinEditors.DropDownEditorButton dropDownEditorButton1 = new Infragistics.Win.UltraWinEditors.DropDownEditorButton("DropDownList");
            this.label3 = new System.Windows.Forms.Label();
            this.dbName = new Idera.SQLdm.DesktopClient.Controls.AllowDeleteTextBox();
            this.minWrites = new System.Windows.Forms.NumericUpDown();
            this.minCPU = new System.Windows.Forms.NumericUpDown();
            this.minReads = new System.Windows.Forms.NumericUpDown();
            this.label6 = new System.Windows.Forms.Label();
            this.label7 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.chartTypeCombo = new System.Windows.Forms.ComboBox();
            this.label1 = new System.Windows.Forms.Label();
            this.numStatements = new System.Windows.Forms.NumericUpDown();
            this.label2 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.startHoursTimeEditor = new Idera.SQLdm.Common.UI.Controls.TimeComboEditor();
            this.endHoursTimeEditor = new Idera.SQLdm.Common.UI.Controls.TimeComboEditor();
            this.startHoursLbl = new System.Windows.Forms.Label();
            this.endHoursLbl = new System.Windows.Forms.Label();
            this.panel1.SuspendLayout();
            this.filterPanel.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.minWrites)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.minCPU)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.minReads)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numStatements)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.startHoursTimeEditor)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.endHoursTimeEditor)).BeginInit();
            this.SuspendLayout();
            // 
            // panel1
            // 
            this.panel1.Size = new System.Drawing.Size(812, 175);
            // 
            // reportViewer
            // 
            this.reportViewer.Size = new System.Drawing.Size(812, 429);
            // 
            // filterPanel
            // 
            this.filterPanel.Controls.Add(this.endHoursLbl);
            this.filterPanel.Controls.Add(this.startHoursLbl);
            this.filterPanel.Controls.Add(this.endHoursTimeEditor);
            this.filterPanel.Controls.Add(this.startHoursTimeEditor);
            this.filterPanel.Controls.Add(this.label4);
            this.filterPanel.Controls.Add(this.numStatements);
            this.filterPanel.Controls.Add(this.label2);
            this.filterPanel.Controls.Add(this.label1);
            this.filterPanel.Controls.Add(this.chartTypeCombo);
            this.filterPanel.Controls.Add(this.minWrites);
            this.filterPanel.Controls.Add(this.minCPU);
            this.filterPanel.Controls.Add(this.minReads);
            this.filterPanel.Controls.Add(this.label6);
            this.filterPanel.Controls.Add(this.label7);
            this.filterPanel.Controls.Add(this.label5);
            this.filterPanel.Controls.Add(this.label3);
            this.filterPanel.Controls.Add(this.dbName);
            this.filterPanel.Size = new System.Drawing.Size(812, 175);
            this.filterPanel.Controls.SetChildIndex(this.rangeInfoLabel, 0);
            this.filterPanel.Controls.SetChildIndex(this.rangeLabel, 0);
            this.filterPanel.Controls.SetChildIndex(this.tagsLabel, 0);
            this.filterPanel.Controls.SetChildIndex(this.periodLabel, 0);
            this.filterPanel.Controls.SetChildIndex(this.sampleLabel, 0);
            this.filterPanel.Controls.SetChildIndex(this.instanceLabel, 0);
            this.filterPanel.Controls.SetChildIndex(this.periodCombo, 0);
            this.filterPanel.Controls.SetChildIndex(this.sampleSizeCombo, 0);
            this.filterPanel.Controls.SetChildIndex(this.instanceCombo, 0);
            this.filterPanel.Controls.SetChildIndex(this.tagsComboBox, 0);
            this.filterPanel.Controls.SetChildIndex(this.dbName, 0);
            this.filterPanel.Controls.SetChildIndex(this.label3, 0);
            this.filterPanel.Controls.SetChildIndex(this.label5, 0);
            this.filterPanel.Controls.SetChildIndex(this.label7, 0);
            this.filterPanel.Controls.SetChildIndex(this.label6, 0);
            this.filterPanel.Controls.SetChildIndex(this.minReads, 0);
            this.filterPanel.Controls.SetChildIndex(this.minCPU, 0);
            this.filterPanel.Controls.SetChildIndex(this.minWrites, 0);
            this.filterPanel.Controls.SetChildIndex(this.chartTypeCombo, 0);
            this.filterPanel.Controls.SetChildIndex(this.label1, 0);
            this.filterPanel.Controls.SetChildIndex(this.label2, 0);
            this.filterPanel.Controls.SetChildIndex(this.numStatements, 0);
            this.filterPanel.Controls.SetChildIndex(this.label4, 0);
            this.filterPanel.Controls.SetChildIndex(this.startHoursTimeEditor, 0);
            this.filterPanel.Controls.SetChildIndex(this.endHoursTimeEditor, 0);
            this.filterPanel.Controls.SetChildIndex(this.startHoursLbl, 0);
            this.filterPanel.Controls.SetChildIndex(this.endHoursLbl, 0);
            // 
            // tagsLabel
            // 
            this.tagsLabel.Location = new System.Drawing.Point(384, 175);
            this.tagsLabel.Visible = false;
            // 
            // periodLabel
            // 
            this.periodLabel.Location = new System.Drawing.Point(10, 39);
            // 
            // sampleLabel
            // 
            this.sampleLabel.Location = new System.Drawing.Point(373, 202);
            this.sampleLabel.Visible = false;
            // 
            // instanceLabel
            // 
            this.instanceLabel.Location = new System.Drawing.Point(10, 10);
            // 
            // periodCombo
            // 
            this.periodCombo.Location = new System.Drawing.Point(78, 34);
            this.periodCombo.Size = new System.Drawing.Size(300, 21);
            this.periodCombo.TabIndex = 1;
            this.periodCombo.SelectedIndexChanged += new System.EventHandler(this.periodCombo_SelectedIndexChanged);
            // 
            // sampleSizeCombo
            // 
            this.sampleSizeCombo.Location = new System.Drawing.Point(424, 199);
            this.sampleSizeCombo.Visible = false;
            // 
            // instanceCombo
            // 
            this.instanceCombo.Location = new System.Drawing.Point(78, 7);
            this.instanceCombo.Size = new System.Drawing.Size(300, 21);
            this.instanceCombo.TabIndex = 0;
            // 
            // tagsComboBox
            // 
            this.tagsComboBox.Location = new System.Drawing.Point(424, 172);
            this.tagsComboBox.Visible = false;
            // 
            // reportInstructionsControl
            // 
            this.reportInstructionsControl.ReportInstructions = resources.GetString("reportInstructionsControl.ReportInstructions");
            this.reportInstructionsControl.Size = new System.Drawing.Size(812, 429);
            // 
            // rangeLabel
            // 
            this.rangeLabel.Location = new System.Drawing.Point(78, 61);
            this.rangeLabel.Size = new System.Drawing.Size(300, 21);
            // 
            // rangeInfoLabel
            // 
            this.rangeInfoLabel.Location = new System.Drawing.Point(10, 65);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(399, 63);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(87, 13);
            this.label3.TabIndex = 33;
            this.label3.Text = "Database Name:";
            // 
            // dbName
            // 
            this.dbName.Location = new System.Drawing.Point(524, 60);
            this.dbName.Name = "dbName";
            this.dbName.Size = new System.Drawing.Size(250, 20);
            this.dbName.TabIndex = 7;
            // 
            // minWrites
            // 
            this.minWrites.Increment = new decimal(new int[] {
            10,
            0,
            0,
            0});
            this.minWrites.Location = new System.Drawing.Point(699, 8);
            this.minWrites.Maximum = new decimal(new int[] {
            100000000,
            0,
            0,
            0});
            this.minWrites.Name = "minWrites";
            this.minWrites.Size = new System.Drawing.Size(75, 20);
            this.minWrites.TabIndex = 4;
            this.minWrites.Leave += new System.EventHandler(this.minWrites_Leave);
            // 
            // minCPU
            // 
            this.minCPU.Increment = new decimal(new int[] {
            10,
            0,
            0,
            0});
            this.minCPU.Location = new System.Drawing.Point(524, 34);
            this.minCPU.Maximum = new decimal(new int[] {
            100000000,
            0,
            0,
            0});
            this.minCPU.Name = "minCPU";
            this.minCPU.Size = new System.Drawing.Size(75, 20);
            this.minCPU.TabIndex = 5;
            this.minCPU.Leave += new System.EventHandler(this.minCPU_Leave);
            // 
            // minReads
            // 
            this.minReads.Increment = new decimal(new int[] {
            10,
            0,
            0,
            0});
            this.minReads.Location = new System.Drawing.Point(699, 34);
            this.minReads.Maximum = new decimal(new int[] {
            100000000,
            0,
            0,
            0});
            this.minReads.Name = "minReads";
            this.minReads.Size = new System.Drawing.Size(75, 20);
            this.minReads.TabIndex = 6;
            this.minReads.Leave += new System.EventHandler(this.minReads_Leave);
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(610, 10);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(84, 13);
            this.label6.TabIndex = 41;
            this.label6.Text = "Minimum Writes:";
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(399, 36);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(76, 13);
            this.label7.TabIndex = 43;
            this.label7.Text = "Minimum CPU:";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(610, 36);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(85, 13);
            this.label5.TabIndex = 39;
            this.label5.Text = "Minimum Reads:";
            // 
            // chartTypeCombo
            // 
            this.chartTypeCombo.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.chartTypeCombo.FormattingEnabled = true;
            this.chartTypeCombo.Items.AddRange(new object[] {
            "CPU",
            "Reads",
            "Writes"});
            this.chartTypeCombo.Location = new System.Drawing.Point(78, 87);
            this.chartTypeCombo.Name = "chartTypeCombo";
            this.chartTypeCombo.Size = new System.Drawing.Size(300, 21);
            this.chartTypeCombo.TabIndex = 2;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(10, 91);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(62, 13);
            this.label1.TabIndex = 45;
            this.label1.Text = "Chart Type:";
            // 
            // numStatements
            // 
            this.numStatements.Location = new System.Drawing.Point(524, 8);
            this.numStatements.Maximum = new decimal(new int[] {
            20,
            0,
            0,
            0});
            this.numStatements.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.numStatements.Name = "numStatements";
            this.numStatements.Size = new System.Drawing.Size(75, 20);
            this.numStatements.TabIndex = 3;
            this.numStatements.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.numStatements.Leave += new System.EventHandler(this.numStatements_Leave);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(399, 10);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(119, 13);
            this.label2.TabIndex = 47;
            this.label2.Text = "Number of Applications:";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(567, 83);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(207, 13);
            this.label4.TabIndex = 48;
            this.label4.Text = "use % as wildcard; leave blank to return all";
            this.label4.Click += new System.EventHandler(this.label4_Click);
            // 
            // startHoursTimeEditor
            // 
            dropDownEditorButton2.Key = "DropDownList";
            dropDownEditorButton2.PreferredDropDownSize = new System.Drawing.Size(0, 0);
            this.startHoursTimeEditor.ButtonsRight.Add(dropDownEditorButton2);
            this.startHoursTimeEditor.DropDownButtonDisplayStyle = Infragistics.Win.ButtonDisplayStyle.Never;
            this.startHoursTimeEditor.ListInterval = System.TimeSpan.Parse("01:00:00");
            this.startHoursTimeEditor.Location = new System.Drawing.Point(79, 114);
            this.startHoursTimeEditor.MaskInput = "{time}";
            this.startHoursTimeEditor.Name = "startHoursTimeEditor";
            this.startHoursTimeEditor.Size = new System.Drawing.Size(92, 21);
            this.startHoursTimeEditor.TabIndex = 49;
            this.startHoursTimeEditor.Time = System.TimeSpan.Parse("00:00:00");
            this.startHoursTimeEditor.ValueChanged += new System.EventHandler(this.startHoursTimeEditor_ValueChanged);
            // 
            // endHoursTimeEditor
            // 
            dropDownEditorButton1.Key = "DropDownList";
            dropDownEditorButton1.PreferredDropDownSize = new System.Drawing.Size(0, 0);
            this.endHoursTimeEditor.ButtonsRight.Add(dropDownEditorButton1);
            this.endHoursTimeEditor.DateTime = new System.DateTime(2012, 5, 7, 23, 0, 0, 0);
            this.endHoursTimeEditor.DropDownButtonDisplayStyle = Infragistics.Win.ButtonDisplayStyle.Never;
            this.endHoursTimeEditor.ListInterval = System.TimeSpan.Parse("01:00:00");
            this.endHoursTimeEditor.Location = new System.Drawing.Point(286, 114);
            this.endHoursTimeEditor.MaskInput = "{time}";
            this.endHoursTimeEditor.Name = "endHoursTimeEditor";
            this.endHoursTimeEditor.Size = new System.Drawing.Size(92, 21);
            this.endHoursTimeEditor.TabIndex = 50;
            this.endHoursTimeEditor.Time = System.TimeSpan.Parse("23:00:00");
            this.endHoursTimeEditor.Value = new System.DateTime(2012, 5, 7, 23, 0, 0, 0);
            this.endHoursTimeEditor.ValueChanged += new System.EventHandler(this.endHoursTimeEditor_ValueChanged);
            // 
            // startHoursLbl
            // 
            this.startHoursLbl.AutoSize = true;
            this.startHoursLbl.Location = new System.Drawing.Point(10, 118);
            this.startHoursLbl.Name = "startHoursLbl";
            this.startHoursLbl.Size = new System.Drawing.Size(63, 13);
            this.startHoursLbl.TabIndex = 51;
            this.startHoursLbl.Text = "Start Hours:";
            // 
            // endHoursLbl
            // 
            this.endHoursLbl.AutoSize = true;
            this.endHoursLbl.Location = new System.Drawing.Point(220, 118);
            this.endHoursLbl.Name = "endHoursLbl";
            this.endHoursLbl.Size = new System.Drawing.Size(60, 13);
            this.endHoursLbl.TabIndex = 52;
            this.endHoursLbl.Text = "End Hours:";
            // 
            // TopDatabaseApplications
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.Name = "TopDatabaseApplications";
            this.Size = new System.Drawing.Size(812, 607);
            this.panel1.ResumeLayout(false);
            this.filterPanel.ResumeLayout(false);
            this.filterPanel.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.minWrites)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.minCPU)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.minReads)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numStatements)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.startHoursTimeEditor)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.endHoursTimeEditor)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Label label3;
        private Controls.AllowDeleteTextBox dbName;
        private System.Windows.Forms.NumericUpDown minWrites;
        private System.Windows.Forms.NumericUpDown minCPU;
        private System.Windows.Forms.NumericUpDown minReads;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.ComboBox chartTypeCombo;
        private System.Windows.Forms.NumericUpDown numStatements;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label endHoursLbl;
        private System.Windows.Forms.Label startHoursLbl;
        private Common.UI.Controls.TimeComboEditor endHoursTimeEditor;
        private Common.UI.Controls.TimeComboEditor startHoursTimeEditor;
    }
}
