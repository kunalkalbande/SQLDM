namespace Idera.SQLdm.DesktopClient.Views.Reports.ReportControls
{
    partial class DatabaseStatistics
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(DatabaseStatistics));
            Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomDropDownEditorButton dropDownEditorButton2 = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomDropDownEditorButton("DropDownList");
            Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomDropDownEditorButton dropDownEditorButton1 = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomDropDownEditorButton("DropDownList");
            this.label2 = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomLabel();
            this.compareDatabase = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomTextBox();
            this.compareDatabaseBrowseButton = new Infragistics.Win.Misc.UltraButton();
            this.compareDateStartPicker = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomDateTimePicker();
            this.label3 = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomLabel();
            this.compareDatabaseEndDate = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomTextBox();
            this.label4 = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomLabel();
            this.label5 = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomLabel();
            this.chartTypeCombo = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomComboBox();
            this.groupBox1 = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomGroupBox();
            this.startHoursLbl = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomLabel();
            this.startHoursTimeEditor = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomTimeComboEditor();
            this.endHoursLbl = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomLabel();
            this.endHoursTimeEditor = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomTimeComboEditor();
            this.panel1.SuspendLayout();
            this.filterPanel.SuspendLayout();
            this.groupBox1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.startHoursTimeEditor)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.endHoursTimeEditor)).BeginInit();
            this.SuspendLayout();
            // 
            // databaseTextbox
            // 
            this.databaseTextbox.BackColor = System.Drawing.SystemColors.Window;
            this.databaseTextbox.Location = new System.Drawing.Point(72, 34);
            this.databaseTextbox.Size = new System.Drawing.Size(267, 20);
            this.databaseTextbox.TabIndex = 1;
            this.databaseTextbox.TabStop = false;
            // 
            // databaseBrowseButton
            // 
            this.databaseBrowseButton.Location = new System.Drawing.Point(343, 34);
            this.databaseBrowseButton.ShowFocusRect = false;
            this.databaseBrowseButton.ShowOutline = false;
            this.databaseBrowseButton.TabIndex = 2;
            // 
            // label1
            // 
            this.label1.ForeColor = System.Drawing.SystemColors.ControlText;
            this.label1.Location = new System.Drawing.Point(11, 37);
            // 
            // panel1
            // 
            this.panel1.Size = new System.Drawing.Size(833, 176);
            // 
            // reportViewer
            // 
            this.reportViewer.Size = new System.Drawing.Size(833, 428);
            // 
            // filterPanel
            // 
            this.filterPanel.Controls.Add(this.endHoursTimeEditor);
            this.filterPanel.Controls.Add(this.endHoursLbl);
            this.filterPanel.Controls.Add(this.startHoursTimeEditor);
            this.filterPanel.Controls.Add(this.startHoursLbl);
            this.filterPanel.Controls.Add(this.groupBox1);
            this.filterPanel.Controls.Add(this.label5);
            this.filterPanel.Controls.Add(this.chartTypeCombo);
            this.filterPanel.Size = new System.Drawing.Size(833, 176);
            this.filterPanel.Controls.SetChildIndex(this.rangeInfoLabel, 0);
            this.filterPanel.Controls.SetChildIndex(this.rangeLabel, 0);
            this.filterPanel.Controls.SetChildIndex(this.label1, 0);
            this.filterPanel.Controls.SetChildIndex(this.databaseBrowseButton, 0);
            this.filterPanel.Controls.SetChildIndex(this.databaseTextbox, 0);
            this.filterPanel.Controls.SetChildIndex(this.tagsLabel, 0);
            this.filterPanel.Controls.SetChildIndex(this.tagsComboBox, 0);
            this.filterPanel.Controls.SetChildIndex(this.periodLabel, 0);
            this.filterPanel.Controls.SetChildIndex(this.periodCombo, 0);
            this.filterPanel.Controls.SetChildIndex(this.sampleLabel, 0);
            this.filterPanel.Controls.SetChildIndex(this.sampleSizeCombo, 0);
            this.filterPanel.Controls.SetChildIndex(this.instanceLabel, 0);
            this.filterPanel.Controls.SetChildIndex(this.instanceCombo, 0);
            this.filterPanel.Controls.SetChildIndex(this.chartTypeCombo, 0);
            this.filterPanel.Controls.SetChildIndex(this.label5, 0);
            this.filterPanel.Controls.SetChildIndex(this.groupBox1, 0);
            this.filterPanel.Controls.SetChildIndex(this.startHoursLbl, 0);
            this.filterPanel.Controls.SetChildIndex(this.startHoursTimeEditor, 0);
            this.filterPanel.Controls.SetChildIndex(this.endHoursLbl, 0);
            this.filterPanel.Controls.SetChildIndex(this.endHoursTimeEditor, 0);
            // 
            // tagsLabel
            // 
            this.tagsLabel.Location = new System.Drawing.Point(459, 198);
            this.tagsLabel.Visible = false;
            // 
            // periodLabel
            // 
            this.periodLabel.Location = new System.Drawing.Point(10, 63);
            // 
            // sampleLabel
            // 
            this.sampleLabel.Location = new System.Drawing.Point(10, 116);
            // 
            // instanceLabel
            // 
            this.instanceLabel.Location = new System.Drawing.Point(10, 10);
            // 
            // periodCombo
            // 
            this.periodCombo.Location = new System.Drawing.Point(72, 60);
            this.periodCombo.Size = new System.Drawing.Size(300, 21);
            this.periodCombo.TabIndex = 3;
            this.periodCombo.SelectedIndexChanged += new System.EventHandler(this.periodCombo_SelectedIndexChanged);
            // 
            // sampleSizeCombo
            // 
            this.sampleSizeCombo.Location = new System.Drawing.Point(72, 113);
            this.sampleSizeCombo.Size = new System.Drawing.Size(300, 21);
            this.sampleSizeCombo.TabIndex = 4;
            // 
            // instanceCombo
            // 
            this.instanceCombo.Location = new System.Drawing.Point(72, 7);
            this.instanceCombo.Size = new System.Drawing.Size(300, 21);
            this.instanceCombo.TabIndex = 0;
            this.instanceCombo.SelectedIndexChanged += new System.EventHandler(this.instanceCombo_SelectedIndexChanged);
            // 
            // tagsComboBox
            // 
            this.tagsComboBox.Location = new System.Drawing.Point(500, 194);
            this.tagsComboBox.Visible = false;
            // 
            // reportInstructionsControl
            // 
            this.reportInstructionsControl.ReportInstructions = resources.GetString("reportInstructionsControl.ReportInstructions");
            this.reportInstructionsControl.Size = new System.Drawing.Size(833, 428);
            // 
            // rangeLabel
            // 
            this.rangeLabel.Location = new System.Drawing.Point(72, 87);
            this.rangeLabel.Size = new System.Drawing.Size(300, 21);
            // 
            // rangeInfoLabel
            // 
            this.rangeInfoLabel.Location = new System.Drawing.Point(10, 91);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.ForeColor = System.Drawing.SystemColors.ControlText;
            this.label2.Location = new System.Drawing.Point(11, 22);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(56, 13);
            this.label2.TabIndex = 25;
            this.label2.Text = "Database:";
            // 
            // compareDatabase
            // 
            this.compareDatabase.BackColor = System.Drawing.SystemColors.Window;
            this.compareDatabase.Location = new System.Drawing.Point(73, 19);
            this.compareDatabase.Name = "compareDatabase";
            this.compareDatabase.ReadOnly = true;
            this.compareDatabase.Size = new System.Drawing.Size(250, 20);
            this.compareDatabase.TabIndex = 0;
            this.compareDatabase.TabStop = false;
            // 
            // compareDatabaseBrowseButton
            // 
            this.compareDatabaseBrowseButton.Location = new System.Drawing.Point(329, 19);
            this.compareDatabaseBrowseButton.Name = "compareDatabaseBrowseButton";
            this.compareDatabaseBrowseButton.ShowFocusRect = false;
            this.compareDatabaseBrowseButton.ShowOutline = false;
            this.compareDatabaseBrowseButton.Size = new System.Drawing.Size(30, 20);
            this.compareDatabaseBrowseButton.TabIndex = 1;
            this.compareDatabaseBrowseButton.Text = "...";
            this.compareDatabaseBrowseButton.Click += new System.EventHandler(this.compareDatabaseBrowseButton_Click);
            // 
            // compareDateStartPicker
            // 
            this.compareDateStartPicker.CustomFormat = "";
            this.compareDateStartPicker.Format = System.Windows.Forms.DateTimePickerFormat.Short;
            this.compareDateStartPicker.Location = new System.Drawing.Point(73, 45);
            this.compareDateStartPicker.Name = "compareDateStartPicker";
            this.compareDateStartPicker.Size = new System.Drawing.Size(109, 20);
            this.compareDateStartPicker.TabIndex = 2;
            this.compareDateStartPicker.ValueChanged += new System.EventHandler(this.compareDateStartPicker_ValueChanged);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.ForeColor = System.Drawing.SystemColors.ControlText;
            this.label3.Location = new System.Drawing.Point(11, 49);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(40, 13);
            this.label3.TabIndex = 29;
            this.label3.Text = "Period:";
            // 
            // compareDatabaseEndDate
            // 
            this.compareDatabaseEndDate.BackColor = System.Drawing.SystemColors.Window;
            this.compareDatabaseEndDate.Location = new System.Drawing.Point(223, 45);
            this.compareDatabaseEndDate.Name = "compareDatabaseEndDate";
            this.compareDatabaseEndDate.ReadOnly = true;
            this.compareDatabaseEndDate.Size = new System.Drawing.Size(100, 20);
            this.compareDatabaseEndDate.TabIndex = 3;
            this.compareDatabaseEndDate.TabStop = false;
            this.compareDatabaseEndDate.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.ForeColor = System.Drawing.Color.Black;
            this.label4.Location = new System.Drawing.Point(195, 49);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(16, 13);
            this.label4.TabIndex = 31;
            this.label4.Text = "to";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(398, 10);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(62, 13);
            this.label5.TabIndex = 47;
            this.label5.Text = "Chart Type:";
            // 
            // chartTypeCombo
            // 
            this.chartTypeCombo.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.chartTypeCombo.FormattingEnabled = true;
            this.chartTypeCombo.Items.AddRange(new object[] {
            "Data File Size MB",
            "Log File Size MB",
            "Data Size MB",
            "Data Growth %",
            "Reads Per Second",
            "Writes Per Second",
            "Transactions Per Second"});
            this.chartTypeCombo.Location = new System.Drawing.Point(466, 7);
            this.chartTypeCombo.Name = "chartTypeCombo";
            this.chartTypeCombo.Size = new System.Drawing.Size(300, 21);
            this.chartTypeCombo.TabIndex = 5;
            this.chartTypeCombo.SelectedIndexChanged += new System.EventHandler(this.chartTypeCombo_SelectedIndexChanged);
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.label4);
            this.groupBox1.Controls.Add(this.compareDatabaseEndDate);
            this.groupBox1.Controls.Add(this.compareDateStartPicker);
            this.groupBox1.Controls.Add(this.compareDatabaseBrowseButton);
            this.groupBox1.Controls.Add(this.compareDatabase);
            this.groupBox1.Controls.Add(this.label2);
            this.groupBox1.Controls.Add(this.label3);
            this.groupBox1.ForeColor = System.Drawing.SystemColors.ControlText;
            this.groupBox1.Location = new System.Drawing.Point(401, 32);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(365, 76);
            this.groupBox1.TabIndex = 48;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Compare Database (optional)";
            // 
            // startHoursLbl
            // 
            this.startHoursLbl.AutoSize = true;
            this.startHoursLbl.Location = new System.Drawing.Point(397, 116);
            this.startHoursLbl.Name = "startHoursLbl";
            this.startHoursLbl.Size = new System.Drawing.Size(63, 13);
            this.startHoursLbl.TabIndex = 49;
            this.startHoursLbl.Text = "Start Hours:";
            // 
            // startHoursTimeEditor
            // 
            dropDownEditorButton2.Key = "DropDownList";
            dropDownEditorButton2.PreferredDropDownSize = new System.Drawing.Size(0, 0);
            this.startHoursTimeEditor.ButtonsRight.Add(dropDownEditorButton2);
            this.startHoursTimeEditor.DropDownButtonDisplayStyle = Infragistics.Win.ButtonDisplayStyle.Never;
            this.startHoursTimeEditor.ListInterval = System.TimeSpan.Parse("01:00:00");
            this.startHoursTimeEditor.Location = new System.Drawing.Point(466, 113);
            this.startHoursTimeEditor.MaskInput = "{time}";
            this.startHoursTimeEditor.Name = "startHoursTimeEditor";
            this.startHoursTimeEditor.Size = new System.Drawing.Size(92, 21);
            this.startHoursTimeEditor.TabIndex = 50;
            this.startHoursTimeEditor.Time = System.TimeSpan.Parse("00:00:00");
            this.startHoursTimeEditor.ValueChanged += new System.EventHandler(this.startHoursTimeEditor_ValueChanged);
            // 
            // endHoursLbl
            // 
            this.endHoursLbl.AutoSize = true;
            this.endHoursLbl.Location = new System.Drawing.Point(574, 116);
            this.endHoursLbl.Name = "endHoursLbl";
            this.endHoursLbl.Size = new System.Drawing.Size(60, 13);
            this.endHoursLbl.TabIndex = 51;
            this.endHoursLbl.Text = "End Hours:";
            // 
            // endHoursTimeEditor
            // 
            dropDownEditorButton1.Key = "DropDownList";
            dropDownEditorButton1.PreferredDropDownSize = new System.Drawing.Size(0, 0);
            this.endHoursTimeEditor.ButtonsRight.Add(dropDownEditorButton1);
            this.endHoursTimeEditor.DateTime = new System.DateTime(2012, 5, 11, 23, 0, 0, 0);
            this.endHoursTimeEditor.DropDownButtonDisplayStyle = Infragistics.Win.ButtonDisplayStyle.Never;
            this.endHoursTimeEditor.ListInterval = System.TimeSpan.Parse("01:00:00");
            this.endHoursTimeEditor.Location = new System.Drawing.Point(640, 113);
            this.endHoursTimeEditor.MaskInput = "{time}";
            this.endHoursTimeEditor.Name = "endHoursTimeEditor";
            this.endHoursTimeEditor.Size = new System.Drawing.Size(92, 21);
            this.endHoursTimeEditor.TabIndex = 52;
            this.endHoursTimeEditor.Time = System.TimeSpan.Parse("23:00:00");
            this.endHoursTimeEditor.Value = new System.DateTime(2012, 5, 11, 23, 0, 0, 0);
            this.endHoursTimeEditor.ValueChanged += new System.EventHandler(this.endHoursTimeEditor_ValueChanged);
            // 
            // DatabaseStatistics
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.Name = "DatabaseStatistics";
            this.Size = new System.Drawing.Size(833, 607);
            this.panel1.ResumeLayout(false);
            this.filterPanel.ResumeLayout(false);
            this.filterPanel.PerformLayout();
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.startHoursTimeEditor)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.endHoursTimeEditor)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        protected Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomLabel label2;
        protected Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomTextBox compareDatabase;
        private System.Windows.Forms.DateTimePicker compareDateStartPicker;
        protected Infragistics.Win.Misc.UltraButton compareDatabaseBrowseButton;
        protected Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomLabel label4;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomTextBox compareDatabaseEndDate;
        protected Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomLabel label3;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomGroupBox groupBox1;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomLabel label5;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomComboBox chartTypeCombo;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomTimeComboEditor endHoursTimeEditor;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomLabel endHoursLbl;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomTimeComboEditor startHoursTimeEditor;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomLabel startHoursLbl;
    }
}
