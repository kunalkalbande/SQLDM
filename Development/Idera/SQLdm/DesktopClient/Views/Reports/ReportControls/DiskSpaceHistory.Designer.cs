namespace Idera.SQLdm.DesktopClient.Views.Reports.ReportControls
{
    partial class DiskSpaceHistory
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(DiskSpaceHistory));
            Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomDropDownEditorButton dropDownEditorButton2 = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomDropDownEditorButton("DropDownList");
            Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomDropDownEditorButton dropDownEditorButton1 = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomDropDownEditorButton("DropDownList");
            //this.label2 = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomLabel();
            //this.compareDisk = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomTextBox();
            //this.compareDiskBrowseButton = new Infragistics.Win.Misc.UltraButton();
            //this.compareDateStartPicker = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomDateTimePicker();
            //this.label3 = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomLabel();
            //this.compareDiskEndDate = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomTextBox();
            //this.label4 = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomLabel();
            this.label5 = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomLabel();
            this.chartTypeCombo = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomComboBox();
            //this.groupBox1 = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomGroupBox();
            this.startHoursLbl = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomLabel();
            this.startHoursTimeEditor = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomTimeComboEditor();
            this.endHoursLbl = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomLabel();
            this.endHoursTimeEditor = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomTimeComboEditor();
            this.diskTextbox = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomTextBox();
            this.diskBrowseButton = new Infragistics.Win.Misc.UltraButton();
            this.label1 = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomLabel();
            this.panel1.SuspendLayout();
            this.filterPanel.SuspendLayout();
            //this.groupBox1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.startHoursTimeEditor)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.endHoursTimeEditor)).BeginInit();
            this.SuspendLayout();
            
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

            this.filterPanel.Controls.Add(this.diskTextbox);
            this.filterPanel.Controls.Add(this.diskBrowseButton);
            this.filterPanel.Controls.Add(this.label1);
            this.filterPanel.Controls.Add(this.endHoursTimeEditor);
            this.filterPanel.Controls.Add(this.endHoursLbl);
            this.filterPanel.Controls.Add(this.startHoursTimeEditor);
            this.filterPanel.Controls.Add(this.startHoursLbl);
            //this.filterPanel.Controls.Add(this.groupBox1);
            this.filterPanel.Controls.Add(this.label5);
            this.filterPanel.Controls.Add(this.chartTypeCombo);
            this.filterPanel.Size = new System.Drawing.Size(833, 176);
            this.filterPanel.Controls.SetChildIndex(this.rangeInfoLabel, 0);
            this.filterPanel.Controls.SetChildIndex(this.rangeLabel, 0);
            this.filterPanel.Controls.SetChildIndex(this.label1, 0);
            this.filterPanel.Controls.SetChildIndex(this.diskBrowseButton, 0);
            this.filterPanel.Controls.SetChildIndex(this.diskTextbox, 0);
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
            //this.filterPanel.Controls.SetChildIndex(this.groupBox1, 0);
            this.filterPanel.Controls.SetChildIndex(this.startHoursLbl, 0);
            this.filterPanel.Controls.SetChildIndex(this.startHoursTimeEditor, 0);
            this.filterPanel.Controls.SetChildIndex(this.endHoursLbl, 0);
            this.filterPanel.Controls.SetChildIndex(this.endHoursTimeEditor, 0);
            this.tagsLabel.Visible = false;
            this.tagsComboBox.Visible = false;
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
            this.periodCombo.Location = new System.Drawing.Point(78, 60);//SQLdm 9.1 (Ankit Srivastava) - Fixed Rally Defect DE44555 - shifted controls 6 pixels right
            this.periodCombo.Size = new System.Drawing.Size(300, 21);
            this.periodCombo.TabIndex = 3;
            this.periodCombo.SelectedIndexChanged += new System.EventHandler(this.periodCombo_SelectedIndexChanged);
            // 
            // sampleSizeCombo
            // 
            this.sampleSizeCombo.Location = new System.Drawing.Point(78, 113);//SQLdm 9.1 (Ankit Srivastava) - Fixed Rally Defect DE44555 - shifted controls 6 pixels right
            this.sampleSizeCombo.Size = new System.Drawing.Size(300, 21);
            this.sampleSizeCombo.TabIndex = 4;
            // 
            // instanceCombo
            // 
            this.instanceCombo.Location = new System.Drawing.Point(78, 7);//SQLdm 9.1 (Ankit Srivastava) - Fixed Rally Defect DE44555 - shifted controls 6 pixels right
            this.instanceCombo.Size = new System.Drawing.Size(300, 21);
            this.instanceCombo.TabIndex = 0;
            this.instanceCombo.SelectedIndexChanged += new System.EventHandler(this.instanceCombo_SelectedIndexChanged);
            // 
            // diskBrowseButton
            // 
            this.diskBrowseButton.ShowOutline = false;
            this.diskBrowseButton.ShowFocusRect = false;
            this.diskBrowseButton.Location = new System.Drawing.Point(677, 7);
            this.diskBrowseButton.Name = "diskBrowseButton";
            this.diskBrowseButton.Size = new System.Drawing.Size(30, 20);
            this.diskBrowseButton.TabIndex = 19;
            this.diskBrowseButton.Text = "...";
            this.diskBrowseButton.Click += new System.EventHandler(this.diskBrowseButton_Click);
            // 
            // diskTextbox
            // 
            this.diskTextbox.BackColor = System.Drawing.SystemColors.Window;
            this.diskTextbox.TabStop = false;
            this.diskTextbox.Location = new System.Drawing.Point(460, 7);
            this.diskTextbox.Name = "diskTextbox";
            this.diskTextbox.ReadOnly = true;
            this.diskTextbox.Size = new System.Drawing.Size(214, 20);
            this.diskTextbox.TabIndex = 20;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.ForeColor = System.Drawing.SystemColors.ControlText;
            this.label1.Location = new System.Drawing.Point(404, 10);//SQLdm 9.1 (Ankit Srivastava) - Fixed Rally Defect DE44555 - shifted controls 6 pixels right
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(61, 13);
            this.label1.TabIndex = 21;
            this.label1.Text = "Disk:";
            // 
            // reportInstructionsControl
            // 
            this.reportInstructionsControl.ReportInstructions = resources.GetString("reportInstructionsControl.ReportInstructions1");
            this.reportInstructionsControl.Size = new System.Drawing.Size(833, 428);
            // 
            // rangeLabel
            // 
            this.rangeLabel.Location = new System.Drawing.Point(78, 87);//SQLdm 9.1 (Ankit Srivastava) - Fixed Rally Defect DE44555 - shifted controls 6 pixels right
            this.rangeLabel.Size = new System.Drawing.Size(300, 21);
            // 
            // rangeInfoLabel
            // 
            this.rangeInfoLabel.Location = new System.Drawing.Point(10, 91);
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(10, 38);
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
            "SQL Data Used MB",
            "SQL Data Free MB",
            "SQL Log File MB",
            "Non-SQL Disk Usage MB",
            "Free Disk Space in MB",
            "Disk Reads Per Sec",
            "Disk Writes Per Sec"});
            this.chartTypeCombo.Location = new System.Drawing.Point(78, 35);//SQLdm 9.1 (Ankit Srivastava) - Fixed Rally Defect DE44555 - shifted controls 6 pixels right
            this.chartTypeCombo.Name = "chartTypeCombo";
            this.chartTypeCombo.Size = new System.Drawing.Size(300, 21);
            this.chartTypeCombo.TabIndex = 5;
            this.chartTypeCombo.SelectedIndexChanged += new System.EventHandler(this.chartTypeCombo_SelectedIndexChanged);
            // 
            // groupBox1
            // 
            //this.groupBox1.Controls.Add(this.label4);
            //this.groupBox1.Controls.Add(this.compareDiskEndDate);
            //this.groupBox1.Controls.Add(this.compareDateStartPicker);
            //this.groupBox1.Controls.Add(this.compareDiskBrowseButton);
            //this.groupBox1.Controls.Add(this.compareDisk);
            //this.groupBox1.Controls.Add(this.label2);
            //this.groupBox1.Controls.Add(this.label3);
            //this.groupBox1.ForeColor = System.Drawing.SystemColors.ControlText;
            //this.groupBox1.Location = new System.Drawing.Point(401, 32);
            //this.groupBox1.Name = "groupBox1";
            //this.groupBox1.Size = new System.Drawing.Size(365, 76);
            //this.groupBox1.TabIndex = 48;
            //this.groupBox1.TabStop = false;
            //this.groupBox1.Text = "Compare Disk (optional)";
            // 
            // startHoursLbl
            // 
            this.startHoursLbl.AutoSize = true;
            this.startHoursLbl.Location = new System.Drawing.Point(403, 116);//SQLdm 9.1 (Ankit Srivastava) - Fixed Rally Defect DE44555 - shifted controls 6 pixels right
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
            this.startHoursTimeEditor.Location = new System.Drawing.Point(472, 113);//SQLdm 9.1 (Ankit Srivastava) - Fixed Rally Defect DE44555 - shifted controls 6 pixels right
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
            this.endHoursLbl.Location = new System.Drawing.Point(580, 116);//SQLdm 9.1 (Ankit Srivastava) - Fixed Rally Defect DE44555 - shifted controls 6 pixels right
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
            this.endHoursTimeEditor.Location = new System.Drawing.Point(646, 113);//SQLdm 9.1 (Ankit Srivastava) - Fixed Rally Defect DE44555 - shifted controls 6 pixels right
            this.endHoursTimeEditor.MaskInput = "{time}";
            this.endHoursTimeEditor.Name = "endHoursTimeEditor";
            this.endHoursTimeEditor.Size = new System.Drawing.Size(92, 21);
            this.endHoursTimeEditor.TabIndex = 52;
            this.endHoursTimeEditor.Time = System.TimeSpan.Parse("23:00:00");
            this.endHoursTimeEditor.Value = new System.DateTime(2012, 5, 11, 23, 0, 0, 0);
            this.endHoursTimeEditor.ValueChanged += new System.EventHandler(this.endHoursTimeEditor_ValueChanged);
            // 
            // DiskSpaceHistory
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.Name = "DiskSpaceHistory";
            this.Size = new System.Drawing.Size(833, 607);
            this.panel1.ResumeLayout(false);
            this.filterPanel.ResumeLayout(false);
            this.filterPanel.PerformLayout();
            //this.groupBox1.ResumeLayout(false);
            //this.groupBox1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.startHoursTimeEditor)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.endHoursTimeEditor)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        //protected Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomLabel label2;
        //protected Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomTextBox compareDisk;
        //private System.Windows.Forms.DateTimePicker compareDateStartPicker;
        //protected Infragistics.Win.Misc.UltraButton compareDiskBrowseButton;
        //protected Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomLabel label4;
        //private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomTextBox compareDiskEndDate;
        //protected Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomLabel label3;
        //private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomGroupBox groupBox1;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomLabel label5;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomComboBox chartTypeCombo;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomTimeComboEditor endHoursTimeEditor;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomLabel endHoursLbl;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomTimeComboEditor startHoursTimeEditor;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomLabel startHoursLbl;
        protected Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomTextBox diskTextbox;
        protected Infragistics.Win.Misc.UltraButton diskBrowseButton;
        protected Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomLabel label1;
    }
}
