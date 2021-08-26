namespace Idera.SQLdm.DesktopClient.Views.Reports.ReportControls
{
    partial class TopDatabases
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(TopDatabases));
            Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomDropDownEditorButton dropDownEditorButton2 = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomDropDownEditorButton("DropDownList");
            Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomDropDownEditorButton dropDownEditorButton1 = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomDropDownEditorButton("DropDownList");
            this.minTransactions = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomNumericUpDown();
            this.minGrowth = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomNumericUpDown();
            this.minWritten = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomNumericUpDown();
            this.minReads = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomNumericUpDown();
            this.numDatabases = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomNumericUpDown();
            this.waitThreshold = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomNumericUpDown();// This is to take wait threshold input. Aditya Shukla (SQLdm 8.6)
            this.label6 = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomLabel();
            this.minSize = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomNumericUpDown();
            this.label2 = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomLabel();
            this.label7 = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomLabel();
            this.label4 = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomLabel();
            this.label5 = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomLabel();
            this.label9 = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomLabel();
            this.dbName = new Idera.SQLdm.DesktopClient.Controls.AllowDeleteTextBox();
            this.label3 = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomLabel();
            this.label8 = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomLabel();
            this.includeSystem = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomCheckBox();
            this.startHoursLbl = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomLabel();
            this.endHoursLbl = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomLabel();
            this.waitThresholdLbl = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomLabel();// This is a label for Wait Threshold input control. Aditya Shukla (SQLdm 8.6)
            this.startHoursTimeEditor = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomTimeComboEditor();
            this.endHoursTimeEditor = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomTimeComboEditor();
            this.panel1.SuspendLayout();
            this.filterPanel.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.minTransactions)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.minGrowth)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.minWritten)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.minReads)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numDatabases)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.minSize)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.startHoursTimeEditor)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.endHoursTimeEditor)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.waitThreshold)).BeginInit();// Aditya Shukla (SQLdm 8.6)
            this.SuspendLayout();
            // 
            // panel1
            // 
            this.panel1.Size = new System.Drawing.Size(863, 163);
            // 
            // reportViewer
            // 
            this.reportViewer.Size = new System.Drawing.Size(863, 441);
            // 
            // filterPanel
            // 
            this.filterPanel.Controls.Add(this.endHoursTimeEditor);
            this.filterPanel.Controls.Add(this.startHoursTimeEditor);
            this.filterPanel.Controls.Add(this.endHoursLbl);
            this.filterPanel.Controls.Add(this.startHoursLbl);
            this.filterPanel.Controls.Add(this.includeSystem);
            this.filterPanel.Controls.Add(this.label8);
            this.filterPanel.Controls.Add(this.label3);
            this.filterPanel.Controls.Add(this.dbName);
            this.filterPanel.Controls.Add(this.minTransactions);
            this.filterPanel.Controls.Add(this.minGrowth);
            this.filterPanel.Controls.Add(this.minWritten);
            this.filterPanel.Controls.Add(this.minReads);
            this.filterPanel.Controls.Add(this.numDatabases);
            this.filterPanel.Controls.Add(this.waitThreshold);// Putting the control on the panel. Aditya Shukla (SQLdm 8.6)
            this.filterPanel.Controls.Add(this.label6);
            this.filterPanel.Controls.Add(this.minSize);
            this.filterPanel.Controls.Add(this.label2);
            this.filterPanel.Controls.Add(this.label7);
            this.filterPanel.Controls.Add(this.label4);
            this.filterPanel.Controls.Add(this.label5);
            this.filterPanel.Controls.Add(this.label9);
            this.filterPanel.Controls.Add(this.waitThresholdLbl);// Putting the control on the panel. Aditya Shukla (SQLdm 8.6)
            this.filterPanel.Size = new System.Drawing.Size(863, 163);
            this.filterPanel.Controls.SetChildIndex(this.rangeInfoLabel, 0);
            this.filterPanel.Controls.SetChildIndex(this.rangeLabel, 0);
            this.filterPanel.Controls.SetChildIndex(this.tagsComboBox, 0);
            this.filterPanel.Controls.SetChildIndex(this.sampleLabel, 0);
            this.filterPanel.Controls.SetChildIndex(this.tagsLabel, 0);
            this.filterPanel.Controls.SetChildIndex(this.sampleSizeCombo, 0);
            this.filterPanel.Controls.SetChildIndex(this.periodCombo, 0);
            this.filterPanel.Controls.SetChildIndex(this.periodLabel, 0);
            this.filterPanel.Controls.SetChildIndex(this.instanceLabel, 0);
            this.filterPanel.Controls.SetChildIndex(this.instanceCombo, 0);
            this.filterPanel.Controls.SetChildIndex(this.label9, 0);
            this.filterPanel.Controls.SetChildIndex(this.label5, 0);
            this.filterPanel.Controls.SetChildIndex(this.label4, 0);
            this.filterPanel.Controls.SetChildIndex(this.label7, 0);
            this.filterPanel.Controls.SetChildIndex(this.label2, 0);
            this.filterPanel.Controls.SetChildIndex(this.minSize, 0);
            this.filterPanel.Controls.SetChildIndex(this.label6, 0);
            this.filterPanel.Controls.SetChildIndex(this.numDatabases, 0);
            this.filterPanel.Controls.SetChildIndex(this.minReads, 0);
            this.filterPanel.Controls.SetChildIndex(this.minWritten, 0);
            this.filterPanel.Controls.SetChildIndex(this.minGrowth, 0);
            this.filterPanel.Controls.SetChildIndex(this.minTransactions, 0);
            this.filterPanel.Controls.SetChildIndex(this.dbName, 0);
            this.filterPanel.Controls.SetChildIndex(this.label3, 0);
            this.filterPanel.Controls.SetChildIndex(this.label8, 0);
            this.filterPanel.Controls.SetChildIndex(this.includeSystem, 0);
            this.filterPanel.Controls.SetChildIndex(this.startHoursLbl, 0);
            this.filterPanel.Controls.SetChildIndex(this.endHoursLbl, 0);
            this.filterPanel.Controls.SetChildIndex(this.startHoursTimeEditor, 0);
            this.filterPanel.Controls.SetChildIndex(this.endHoursTimeEditor, 0);
            this.filterPanel.Controls.SetChildIndex(this.waitThresholdLbl, 0);// Aditya Shukla (SQLdm 8.6)
            this.filterPanel.Controls.SetChildIndex(this.waitThreshold, 0);// Aditya Shukla (SQLdm 8.6)
            // 
            // tagsLabel
            // 
            this.tagsLabel.Location = new System.Drawing.Point(523, 222);
            // 
            // periodLabel
            // 
            this.periodLabel.Location = new System.Drawing.Point(10, 36);
            // 
            // sampleLabel
            // 
            this.sampleLabel.Location = new System.Drawing.Point(512, 249);
            this.sampleLabel.Visible = false;
            // 
            // instanceLabel
            // 
            this.instanceLabel.Location = new System.Drawing.Point(10, 10);
            // 
            // periodCombo
            // 
            this.periodCombo.Location = new System.Drawing.Point(103, 33);
            this.periodCombo.Size = new System.Drawing.Size(300, 21);
            this.periodCombo.TabIndex = 1;
            this.periodCombo.SelectedIndexChanged += new System.EventHandler(this.periodCombo_SelectedIndexChanged);
            // 
            // sampleSizeCombo
            // 
            this.sampleSizeCombo.Location = new System.Drawing.Point(564, 245);
            this.sampleSizeCombo.Visible = false;
            // 
            // instanceCombo
            // 
            this.instanceCombo.Location = new System.Drawing.Point(103, 6);
            this.instanceCombo.Size = new System.Drawing.Size(300, 21);
            this.instanceCombo.TabIndex = 0;
            // 
            // tagsComboBox
            // 
            this.tagsComboBox.Location = new System.Drawing.Point(564, 218);
            // 
            // reportInstructionsControl
            // 
            this.reportInstructionsControl.ReportInstructions = resources.GetString("reportInstructionsControl.ReportInstructions");
            this.reportInstructionsControl.Size = new System.Drawing.Size(863, 441);
            // 
            // rangeLabel
            // 
            this.rangeLabel.Location = new System.Drawing.Point(103, 59);
            this.rangeLabel.Size = new System.Drawing.Size(300, 21);
            // 
            // rangeInfoLabel
            // 
            this.rangeInfoLabel.Location = new System.Drawing.Point(10, 63);
            // 
            // minTransactions
            // 
            this.minTransactions.Location = new System.Drawing.Point(554, 32);
            this.minTransactions.Maximum = new decimal(new int[] {
            2147483647,
            0,
            0,
            0});
            this.minTransactions.Name = "minTransactions";
            this.minTransactions.Size = new System.Drawing.Size(65, 20);
            this.minTransactions.TabIndex = 4;
            this.minTransactions.Leave += new System.EventHandler(this.minTransactions_Leave);
            // 
            // minGrowth
            // 
            this.minGrowth.Location = new System.Drawing.Point(754, 32);
            this.minGrowth.Maximum = new decimal(new int[] {
            2147483647,
            0,
            0,
            0});
            this.minGrowth.Name = "minGrowth";
            this.minGrowth.Size = new System.Drawing.Size(65, 20);
            this.minGrowth.TabIndex = 7;
            this.minGrowth.Leave += new System.EventHandler(this.minGrowth_Leave);
            // 
            // minWritten
            // 
            this.minWritten.Location = new System.Drawing.Point(554, 58);
            this.minWritten.Maximum = new decimal(new int[] {
            2147483647,
            0,
            0,
            0});
            this.minWritten.Name = "minWritten";
            this.minWritten.Size = new System.Drawing.Size(65, 20);
            this.minWritten.TabIndex = 5;
            this.minWritten.Leave += new System.EventHandler(this.minWritten_Leave);
            // 
            // minReads
            // 
            this.minReads.Location = new System.Drawing.Point(754, 58);
            this.minReads.Maximum = new decimal(new int[] {
            2147483647,
            0,
            0,
            0});
            this.minReads.Name = "minReads";
            this.minReads.Size = new System.Drawing.Size(65, 20);
            this.minReads.TabIndex = 8;
            this.minReads.Leave += new System.EventHandler(this.minReads_Leave);
            // 
            // numDatabases
            // 
            this.numDatabases.Location = new System.Drawing.Point(554, 6);
            this.numDatabases.Maximum = new decimal(new int[] {
            2147483647,
            0,
            0,
            0});
            this.numDatabases.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.numDatabases.Name = "numDatabases";
            this.numDatabases.Size = new System.Drawing.Size(65, 20);
            this.numDatabases.TabIndex = 3;
            this.numDatabases.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.numDatabases.Leave += new System.EventHandler(this.numDatabases_Leave);
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(635, 33);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(75, 13);
            this.label6.TabIndex = 48;
            this.label6.Text = "Min Growth %:";
            // 
            // minSize
            // 
            this.minSize.Location = new System.Drawing.Point(754, 6);
            this.minSize.Maximum = new decimal(new int[] {
            2147483647,
            0,
            0,
            0});
            this.minSize.Name = "minSize";
            this.minSize.Size = new System.Drawing.Size(65, 20);
            this.minSize.TabIndex = 6;
            this.minSize.Leave += new System.EventHandler(this.minSize_Leave);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(430, 8);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(113, 13);
            this.label2.TabIndex = 42;
            this.label2.Text = "Number of Databases:";
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(430, 34);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(113, 13);
            this.label7.TabIndex = 50;
            this.label7.Text = "Min Transactions/sec:";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(635, 8);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(104, 13);
            this.label4.TabIndex = 44;
            this.label4.Text = "Min. Data Size (MB):";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(430, 60);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(118, 13);
            this.label5.TabIndex = 46;
            this.label5.Text = "Min. Bytes Written/sec:";
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Location = new System.Drawing.Point(635, 59);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(110, 13);
            this.label9.TabIndex = 52;
            this.label9.Text = "Min. Bytes Read/sec:";
            // 
            // dbName
            // 
            this.dbName.Location = new System.Drawing.Point(103, 85);
            this.dbName.Name = "dbName";
            this.dbName.Size = new System.Drawing.Size(300, 20);
            this.dbName.TabIndex = 2;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(10, 88);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(87, 13);
            this.label3.TabIndex = 54;
            this.label3.Text = "Database Name:";
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Italic, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label8.Location = new System.Drawing.Point(312, 108);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(91, 13);
            this.label8.TabIndex = 55;
            this.label8.Text = "use % as wildcard";
            // 
            // includeSystem
            // 
            this.includeSystem.AutoSize = true;
            this.includeSystem.Location = new System.Drawing.Point(430, 86);//SQL DM 8.6 (vineet) - Fixing defect DE43584. Changed the location of UI control.
            this.includeSystem.Name = "includeSystem";
            this.includeSystem.Size = new System.Drawing.Size(152, 17);
            this.includeSystem.TabIndex = 10;
            this.includeSystem.Text = "Include System Databases";
            this.includeSystem.UseVisualStyleBackColor = true;
            // 
            // startHoursLbl
            // 
            this.startHoursLbl.AutoSize = true;
            this.startHoursLbl.Location = new System.Drawing.Point(433, 108);//SQL DM 8.6 (vineet) - Fixing defect DE43584. Changed the location of UI control.
            this.startHoursLbl.Name = "startHoursLbl";
            this.startHoursLbl.Size = new System.Drawing.Size(63, 13);
            this.startHoursLbl.TabIndex = 56;
            this.startHoursLbl.Text = "Start Hours:";
            // 
            // endHoursLbl
            // 
            this.endHoursLbl.AutoSize = true;
            this.endHoursLbl.Location = new System.Drawing.Point(635, 108);//SQL DM 8.6 (vineet) - Fixing defect DE43584. Changed the location of UI control.
            this.endHoursLbl.Name = "endHoursLbl";
            this.endHoursLbl.Size = new System.Drawing.Size(60, 13);
            this.endHoursLbl.TabIndex = 57;
            this.endHoursLbl.Text = "End Hours:";
            // 
            // startHoursTimeEditor
            // 
            dropDownEditorButton2.Key = "DropDownList";
            dropDownEditorButton2.PreferredDropDownSize = new System.Drawing.Size(0, 0);
            this.startHoursTimeEditor.ButtonsRight.Add(dropDownEditorButton2);
            this.startHoursTimeEditor.DropDownButtonDisplayStyle = Infragistics.Win.ButtonDisplayStyle.Never;
            this.startHoursTimeEditor.ListInterval = System.TimeSpan.Parse("01:00:00");
            this.startHoursTimeEditor.Location = new System.Drawing.Point(499, 106);//SQL DM 8.6 (vineet) - Fixing defect DE43584. Changed the location of UI control.
            this.startHoursTimeEditor.MaskInput = "{time}";
            this.startHoursTimeEditor.Name = "startHoursTimeEditor";
            this.startHoursTimeEditor.Size = new System.Drawing.Size(92, 21);
            this.startHoursTimeEditor.TabIndex = 58;
            this.startHoursTimeEditor.Time = System.TimeSpan.Parse("00:00:00");
            this.startHoursTimeEditor.ValueChanged += new System.EventHandler(this.startHoursTimeEditor_ValueChanged);
            // 
            // endHoursTimeEditor
            // 
            dropDownEditorButton1.Key = "DropDownList";
            dropDownEditorButton1.PreferredDropDownSize = new System.Drawing.Size(0, 0);
            this.endHoursTimeEditor.ButtonsRight.Add(dropDownEditorButton1);
            this.endHoursTimeEditor.DateTime = new System.DateTime(2012, 5, 11, 23, 0, 0, 0);
            this.endHoursTimeEditor.DropDownButtonDisplayStyle = Infragistics.Win.ButtonDisplayStyle.Never;
            this.endHoursTimeEditor.ListInterval = System.TimeSpan.Parse("01:00:00");
            this.endHoursTimeEditor.Location = new System.Drawing.Point(701, 106);//SQL DM 8.6 (vineet) - Fixing defect DE43584. Changed the location of UI control.
            this.endHoursTimeEditor.MaskInput = "{time}";
            this.endHoursTimeEditor.Name = "endHoursTimeEditor";
            this.endHoursTimeEditor.Size = new System.Drawing.Size(92, 21);
            this.endHoursTimeEditor.TabIndex = 59;
            this.endHoursTimeEditor.Time = System.TimeSpan.Parse("23:00:00");
            this.endHoursTimeEditor.Value = new System.DateTime(2012, 5, 11, 23, 0, 0, 0);
            this.endHoursTimeEditor.ValueChanged += new System.EventHandler(this.endHoursTimeEditor_ValueChanged);
            // 
            // waitThresholdLbl setting the design time properties for the label. Aditya Shukla (SQLdm 8.6) START
            // 
            this.waitThresholdLbl.AutoSize = true;
            this.waitThresholdLbl.ForeColor = System.Drawing.SystemColors.ControlText;
            this.waitThresholdLbl.Location = new System.Drawing.Point(635, 85);
            this.waitThresholdLbl.Name = "waitThresholdLbl";
            this.waitThresholdLbl.Size = new System.Drawing.Size(98, 13);
            this.waitThresholdLbl.TabIndex = 60;
            this.waitThresholdLbl.Text = "Wait Threshold(ms):";
            // waitThresholdLbl setting the design time properties for the label. Aditya Shukla (SQLdm 8.6) END
            // 
            // waitThreshold setting the design time properties for the numeric up down control. Aditya Shukla (SQLdm 8.6) START
            // 
            this.waitThreshold.Location = new System.Drawing.Point(754, 84);
            this.waitThreshold.Maximum = new decimal(new int[] {
            600000,
            0,
            0,
            0});
            this.waitThreshold.Minimum = new decimal(new int[] {
            0,
            0,
            0,
            0});
            this.waitThreshold.Name = "waitThreshold";
            this.waitThreshold.Size = new System.Drawing.Size(65, 20);
            this.waitThreshold.TabIndex = 9;
            this.waitThreshold.Value = new decimal(new int[] {
            0,
            0,
            0,
            0});
            this.waitThreshold.Leave += new System.EventHandler(this.waitThreshold_Leave); // This method checks the input validity
            // waitThreshold setting the design time properties for the numeric up down control. Aditya Shukla (SQLdm 8.6) END
            // 
            // TopDatabases
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.Name = "TopDatabases";
            this.Size = new System.Drawing.Size(863, 607);
            this.panel1.ResumeLayout(false);
            this.filterPanel.ResumeLayout(false);
            this.filterPanel.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.minTransactions)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.minGrowth)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.minWritten)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.minReads)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numDatabases)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.minSize)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.startHoursTimeEditor)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.endHoursTimeEditor)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.waitThreshold)).EndInit();// Aditya Shukla (SQLdm 8.6)
            this.ResumeLayout(false);

        }

        #endregion

        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomNumericUpDown minTransactions;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomNumericUpDown minGrowth;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomNumericUpDown minWritten;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomNumericUpDown minReads;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomNumericUpDown numDatabases;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomLabel label6;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomNumericUpDown minSize;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomLabel label2;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomLabel label7;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomLabel label4;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomLabel label5;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomLabel label9;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomLabel label8;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomLabel label3;
        private Controls.AllowDeleteTextBox dbName;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomCheckBox includeSystem;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomTimeComboEditor endHoursTimeEditor;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomTimeComboEditor startHoursTimeEditor;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomLabel endHoursLbl;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomLabel startHoursLbl;
        //Added a new filter for wait threshold - Aditya Shukla (SQLdm 8.6)
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomLabel waitThresholdLbl;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomNumericUpDown waitThreshold;
    }
}
