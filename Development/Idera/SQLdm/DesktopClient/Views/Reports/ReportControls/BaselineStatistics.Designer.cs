namespace Idera.SQLdm.DesktopClient.Views.Reports.ReportControls
{
    partial class BaselineStatistics
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(BaselineStatistics));
            Infragistics.Win.UltraWinEditors.DropDownEditorButton dropDownEditorButton3 = new Infragistics.Win.UltraWinEditors.DropDownEditorButton("DropDownList");
            Infragistics.Win.UltraWinEditors.DropDownEditorButton dropDownEditorButton4 = new Infragistics.Win.UltraWinEditors.DropDownEditorButton("DropDownList");
            Infragistics.Win.UltraWinEditors.DropDownEditorButton dropDownEditorButton1 = new Infragistics.Win.UltraWinEditors.DropDownEditorButton("DropDownList");
            Infragistics.Win.UltraWinEditors.DropDownEditorButton dropDownEditorButton2 = new Infragistics.Win.UltraWinEditors.DropDownEditorButton("DropDownList");
            this.textBox2 = new System.Windows.Forms.TextBox();
            this.startHoursTimeEditor = new Idera.SQLdm.Common.UI.Controls.TimeComboEditor();
            this.endHoursTimeEditor = new Idera.SQLdm.Common.UI.Controls.TimeComboEditor();
            this.startHoursLbl = new System.Windows.Forms.Label();
            this.endHoursLbl = new System.Windows.Forms.Label();
            this.metricCombo = new System.Windows.Forms.ComboBox();
            this.lblMetric = new System.Windows.Forms.Label();
            this.compareToPanel = new System.Windows.Forms.GroupBox();
            this.compareEndHoursTimeEditor = new Idera.SQLdm.Common.UI.Controls.TimeComboEditor();
            this.compareStartHoursTimeEditor = new Idera.SQLdm.Common.UI.Controls.TimeComboEditor();
            this.compareEndHours = new System.Windows.Forms.Label();
            this.compareStartHours = new System.Windows.Forms.Label();
            this.toLabel = new System.Windows.Forms.Label();
            this.compareEndDateText = new System.Windows.Forms.TextBox();
            this.compareStartDateTimePicker = new System.Windows.Forms.DateTimePicker();
            this.comparePeriodLabel = new System.Windows.Forms.Label();
            this.compareMetricLabel = new System.Windows.Forms.Label();
            this.compareServerLabel = new System.Windows.Forms.Label();
            this.compareTagLabel = new System.Windows.Forms.Label();
            this.compareMetricCombo = new System.Windows.Forms.ComboBox();
            this.compareSameServerChkBox = new System.Windows.Forms.CheckBox();
            this.compareInstanceCombo = new System.Windows.Forms.ComboBox();
            this.compareTagsCombo = new System.Windows.Forms.ComboBox();
            this.panel1.SuspendLayout();
            this.filterPanel.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.startHoursTimeEditor)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.endHoursTimeEditor)).BeginInit();
            this.compareToPanel.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.compareEndHoursTimeEditor)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.compareStartHoursTimeEditor)).BeginInit();
            this.SuspendLayout();
            // 
            // panel1
            // 
            this.panel1.Size = new System.Drawing.Size(752, 233);
            // 
            // reportViewer
            // 
            this.reportViewer.Size = new System.Drawing.Size(752, 371);
            // 
            // filterPanel
            // 
            this.filterPanel.Controls.Add(this.compareToPanel);
            this.filterPanel.Controls.Add(this.startHoursTimeEditor);
            this.filterPanel.Controls.Add(this.endHoursTimeEditor);
            this.filterPanel.Controls.Add(this.startHoursLbl);
            this.filterPanel.Controls.Add(this.endHoursLbl);
            this.filterPanel.Controls.Add(this.metricCombo);
            this.filterPanel.Controls.Add(this.lblMetric);
            this.filterPanel.Size = new System.Drawing.Size(752, 233);
            this.filterPanel.Controls.SetChildIndex(this.lblMetric, 0);
            this.filterPanel.Controls.SetChildIndex(this.metricCombo, 0);
            this.filterPanel.Controls.SetChildIndex(this.endHoursLbl, 0);
            this.filterPanel.Controls.SetChildIndex(this.startHoursLbl, 0);
            this.filterPanel.Controls.SetChildIndex(this.endHoursTimeEditor, 0);
            this.filterPanel.Controls.SetChildIndex(this.startHoursTimeEditor, 0);
            this.filterPanel.Controls.SetChildIndex(this.compareToPanel, 0);
            this.filterPanel.Controls.SetChildIndex(this.periodCombo, 0);
            this.filterPanel.Controls.SetChildIndex(this.sampleLabel, 0);
            this.filterPanel.Controls.SetChildIndex(this.periodLabel, 0);
            this.filterPanel.Controls.SetChildIndex(this.rangeInfoLabel, 0);
            this.filterPanel.Controls.SetChildIndex(this.rangeLabel, 0);
            this.filterPanel.Controls.SetChildIndex(this.sampleSizeCombo, 0);
            this.filterPanel.Controls.SetChildIndex(this.tagsLabel, 0);
            this.filterPanel.Controls.SetChildIndex(this.instanceLabel, 0);
            this.filterPanel.Controls.SetChildIndex(this.instanceCombo, 0);
            this.filterPanel.Controls.SetChildIndex(this.tagsComboBox, 0);
            // 
            // tagsLabel
            // 
            this.tagsLabel.Location = new System.Drawing.Point(10, 13);
            // 
            // periodLabel
            // 
            this.periodLabel.Location = new System.Drawing.Point(10, 94);
            // 
            // sampleLabel
            // 
            this.sampleLabel.Location = new System.Drawing.Point(10, 146);
            // 
            // instanceLabel
            // 
            this.instanceLabel.Location = new System.Drawing.Point(10, 40);
            // 
            // periodCombo
            // 
            this.periodCombo.Location = new System.Drawing.Point(61, 91);
            this.periodCombo.Size = new System.Drawing.Size(300, 21);
            this.periodCombo.TabIndex = 0;
            this.periodCombo.SelectedIndexChanged += new System.EventHandler(this.periodCombo_SelectedIndexChanged);
            // 
            // sampleSizeCombo
            // 
            this.sampleSizeCombo.Location = new System.Drawing.Point(61, 143);
            this.sampleSizeCombo.Size = new System.Drawing.Size(300, 21);
            this.sampleSizeCombo.TabIndex = 1;
            // 
            // instanceCombo
            // 
            this.instanceCombo.Location = new System.Drawing.Point(61, 37);
            this.instanceCombo.Size = new System.Drawing.Size(300, 21);
            this.instanceCombo.TabIndex = 2;
            // 
            // tagsComboBox
            // 
            this.tagsComboBox.Location = new System.Drawing.Point(61, 10);
            this.tagsComboBox.Size = new System.Drawing.Size(300, 21);
            this.tagsComboBox.TabIndex = 0;
            // 
            // reportInstructionsControl
            // 
            this.reportInstructionsControl.ReportInstructions = resources.GetString("reportInstructionsControl.ReportInstructions");
            this.reportInstructionsControl.Size = new System.Drawing.Size(752, 371);
            // 
            // rangeLabel
            // 
            this.rangeLabel.Location = new System.Drawing.Point(61, 117);
            this.rangeLabel.Size = new System.Drawing.Size(300, 21);
            // 
            // rangeInfoLabel
            // 
            this.rangeInfoLabel.Location = new System.Drawing.Point(10, 121);
            // 
            // textBox2
            // 
            this.textBox2.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.textBox2.Location = new System.Drawing.Point(2, 35);
            this.textBox2.Name = "textBox2";
            this.textBox2.Size = new System.Drawing.Size(403, 13);
            this.textBox2.TabIndex = 0;
            this.textBox2.Text = "This is a description of the reprtreport shows you stuff";
            // 
            // startHoursTimeEditor
            // 
            dropDownEditorButton3.Key = "DropDownList";
            dropDownEditorButton3.PreferredDropDownSize = new System.Drawing.Size(0, 0);
            this.startHoursTimeEditor.ButtonsRight.Add(dropDownEditorButton3);
            this.startHoursTimeEditor.DropDownButtonDisplayStyle = Infragistics.Win.ButtonDisplayStyle.Never;
            this.startHoursTimeEditor.ListInterval = System.TimeSpan.Parse("01:00:00");
            this.startHoursTimeEditor.Location = new System.Drawing.Point(82, 170);
            this.startHoursTimeEditor.MaskInput = "{time}";
            this.startHoursTimeEditor.Name = "startHoursTimeEditor";
            this.startHoursTimeEditor.Size = new System.Drawing.Size(92, 21);
            this.startHoursTimeEditor.TabIndex = 30;
            this.startHoursTimeEditor.Time = System.TimeSpan.Parse("00:00:00");
            this.startHoursTimeEditor.ValueChanged += new System.EventHandler(this.startHoursTimeEditor_ValueChanged);
            // 
            // endHoursTimeEditor
            // 
            dropDownEditorButton4.Key = "DropDownList";
            dropDownEditorButton4.PreferredDropDownSize = new System.Drawing.Size(0, 0);
            this.endHoursTimeEditor.ButtonsRight.Add(dropDownEditorButton4);
            this.endHoursTimeEditor.DateTime = new System.DateTime(2012, 5, 28, 23, 0, 0, 0);
            this.endHoursTimeEditor.DropDownButtonDisplayStyle = Infragistics.Win.ButtonDisplayStyle.Never;
            this.endHoursTimeEditor.ListInterval = System.TimeSpan.Parse("01:00:00");
            this.endHoursTimeEditor.Location = new System.Drawing.Point(269, 170);
            this.endHoursTimeEditor.MaskInput = "{time}";
            this.endHoursTimeEditor.Name = "endHoursTimeEditor";
            this.endHoursTimeEditor.Size = new System.Drawing.Size(92, 21);
            this.endHoursTimeEditor.TabIndex = 31;
            this.endHoursTimeEditor.Time = System.TimeSpan.Parse("23:00:00");
            this.endHoursTimeEditor.Value = new System.DateTime(2012, 5, 28, 23, 0, 0, 0);
            this.endHoursTimeEditor.ValueChanged += new System.EventHandler(this.endHoursTimeEditor_ValueChanged);
            // 
            // startHoursLbl
            // 
            this.startHoursLbl.AutoSize = true;
            this.startHoursLbl.Location = new System.Drawing.Point(10, 174);
            this.startHoursLbl.Name = "startHoursLbl";
            this.startHoursLbl.Size = new System.Drawing.Size(63, 13);
            this.startHoursLbl.TabIndex = 32;
            this.startHoursLbl.Text = "Start Hours:";
            // 
            // endHoursLbl
            // 
            this.endHoursLbl.AutoSize = true;
            this.endHoursLbl.Location = new System.Drawing.Point(203, 174);
            this.endHoursLbl.Name = "endHoursLbl";
            this.endHoursLbl.Size = new System.Drawing.Size(60, 13);
            this.endHoursLbl.TabIndex = 33;
            this.endHoursLbl.Text = "End Hours:";
            // 
            // metricCombo
            // 
            this.metricCombo.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.metricCombo.FormattingEnabled = true;
            this.metricCombo.Location = new System.Drawing.Point(61, 64);
            this.metricCombo.Name = "metricCombo";
            this.metricCombo.Size = new System.Drawing.Size(300, 21);
            this.metricCombo.TabIndex = 34;
            this.metricCombo.SelectedValueChanged += new System.EventHandler(this.metricCombo_SelectedValueChanged);

            // 
            // lblMetric
            // 
            this.lblMetric.AutoSize = true;
            this.lblMetric.Location = new System.Drawing.Point(10, 67);
            this.lblMetric.Name = "lblMetric";
            this.lblMetric.Size = new System.Drawing.Size(42, 13);
            this.lblMetric.TabIndex = 35;
            this.lblMetric.Text = "Metric: ";
            // 
            // compareToPanel
            // 
            this.compareToPanel.Controls.Add(this.compareEndHoursTimeEditor);
            this.compareToPanel.Controls.Add(this.compareStartHoursTimeEditor);
            this.compareToPanel.Controls.Add(this.compareEndHours);
            this.compareToPanel.Controls.Add(this.compareStartHours);
            this.compareToPanel.Controls.Add(this.toLabel);
            this.compareToPanel.Controls.Add(this.compareEndDateText);
            this.compareToPanel.Controls.Add(this.compareStartDateTimePicker);
            this.compareToPanel.Controls.Add(this.comparePeriodLabel);
            this.compareToPanel.Controls.Add(this.compareMetricLabel);
            this.compareToPanel.Controls.Add(this.compareServerLabel);
            this.compareToPanel.Controls.Add(this.compareTagLabel);
            this.compareToPanel.Controls.Add(this.compareMetricCombo);
            this.compareToPanel.Controls.Add(this.compareSameServerChkBox);
            this.compareToPanel.Controls.Add(this.compareInstanceCombo);
            this.compareToPanel.Controls.Add(this.compareTagsCombo);
            this.compareToPanel.Location = new System.Drawing.Point(379, 10);
            this.compareToPanel.Name = "compareToPanel";
            this.compareToPanel.Size = new System.Drawing.Size(357, 181);
            this.compareToPanel.TabIndex = 36;
            this.compareToPanel.TabStop = false;
            this.compareToPanel.Text = "Compare to (optional)";
            // 
            // compareEndHoursTimeEditor
            // 
            dropDownEditorButton1.Key = "DropDownList";
            dropDownEditorButton1.PreferredDropDownSize = new System.Drawing.Size(0, 0);
            this.compareEndHoursTimeEditor.ButtonsRight.Add(dropDownEditorButton1);
            this.compareEndHoursTimeEditor.DateTime = new System.DateTime(2012, 5, 30, 23, 0, 0, 0);
            this.compareEndHoursTimeEditor.DropDownButtonDisplayStyle = Infragistics.Win.ButtonDisplayStyle.Never;
            this.compareEndHoursTimeEditor.ListInterval = System.TimeSpan.Parse("01:00:00");
            this.compareEndHoursTimeEditor.Location = new System.Drawing.Point(259, 151);
            this.compareEndHoursTimeEditor.MaskInput = "{time}";
            this.compareEndHoursTimeEditor.Name = "compareEndHoursTimeEditor";
            this.compareEndHoursTimeEditor.Size = new System.Drawing.Size(92, 21);
            this.compareEndHoursTimeEditor.TabIndex = 14;
            this.compareEndHoursTimeEditor.Time = System.TimeSpan.Parse("23:00:00");
            this.compareEndHoursTimeEditor.Value = new System.DateTime(2012, 5, 30, 23, 0, 0, 0);
            this.compareEndHoursTimeEditor.ValueChanged += new System.EventHandler(this.compareEndHoursTimeEditor_ValueChanged);
            // 
            // compareStartHoursTimeEditor
            // 
            dropDownEditorButton2.Key = "DropDownList";
            dropDownEditorButton2.PreferredDropDownSize = new System.Drawing.Size(0, 0);
            this.compareStartHoursTimeEditor.ButtonsRight.Add(dropDownEditorButton2);
            this.compareStartHoursTimeEditor.DropDownButtonDisplayStyle = Infragistics.Win.ButtonDisplayStyle.Never;
            this.compareStartHoursTimeEditor.ListInterval = System.TimeSpan.Parse("01:00:00");
            this.compareStartHoursTimeEditor.Location = new System.Drawing.Point(90, 151);
            this.compareStartHoursTimeEditor.MaskInput = "{time}";
            this.compareStartHoursTimeEditor.Name = "compareStartHoursTimeEditor";
            this.compareStartHoursTimeEditor.Size = new System.Drawing.Size(92, 21);
            this.compareStartHoursTimeEditor.TabIndex = 13;
            this.compareStartHoursTimeEditor.Time = System.TimeSpan.Parse("00:00:00");
            this.compareStartHoursTimeEditor.ValueChanged += new System.EventHandler(this.compareStartHoursTimeEditor_ValueChanged);
            // 
            // compareEndHours
            // 
            this.compareEndHours.AutoSize = true;
            this.compareEndHours.Location = new System.Drawing.Point(198, 155);
            this.compareEndHours.Name = "compareEndHours";
            this.compareEndHours.Size = new System.Drawing.Size(55, 13);
            this.compareEndHours.TabIndex = 12;
            this.compareEndHours.Text = "End Hour:";
            // 
            // compareStartHours
            // 
            this.compareStartHours.AutoSize = true;
            this.compareStartHours.Location = new System.Drawing.Point(10, 155);
            this.compareStartHours.Name = "compareStartHours";
            this.compareStartHours.Size = new System.Drawing.Size(58, 13);
            this.compareStartHours.TabIndex = 11;
            this.compareStartHours.Text = "Start Hour:";
            // 
            // toLabel
            // 
            this.toLabel.AutoSize = true;
            this.toLabel.Location = new System.Drawing.Point(212, 128);
            this.toLabel.Name = "toLabel";
            this.toLabel.Size = new System.Drawing.Size(16, 13);
            this.toLabel.TabIndex = 10;
            this.toLabel.Text = "to";
            // 
            // compareEndDateText
            // 
            this.compareEndDateText.Enabled = false;
            this.compareEndDateText.Location = new System.Drawing.Point(242, 125);
            this.compareEndDateText.Name = "compareEndDateText";
            this.compareEndDateText.Size = new System.Drawing.Size(109, 20);
            this.compareEndDateText.TabIndex = 9;
            // 
            // compareStartDateTimePicker
            // 
            this.compareStartDateTimePicker.Format = System.Windows.Forms.DateTimePickerFormat.Short;
            this.compareStartDateTimePicker.Location = new System.Drawing.Point(90, 125);
            this.compareStartDateTimePicker.Name = "compareStartDateTimePicker";
            this.compareStartDateTimePicker.Size = new System.Drawing.Size(109, 20);
            this.compareStartDateTimePicker.TabIndex = 8;
            this.compareStartDateTimePicker.ValueChanged += new System.EventHandler(this.compareStartDateTimePicker_ValueChanged);
            // 
            // comparePeriodLabel
            // 
            this.comparePeriodLabel.AutoSize = true;
            this.comparePeriodLabel.Location = new System.Drawing.Point(10, 128);
            this.comparePeriodLabel.Name = "comparePeriodLabel";
            this.comparePeriodLabel.Size = new System.Drawing.Size(40, 13);
            this.comparePeriodLabel.TabIndex = 7;
            this.comparePeriodLabel.Text = "Period:";
            // 
            // compareMetricLabel
            // 
            this.compareMetricLabel.AutoSize = true;
            this.compareMetricLabel.Location = new System.Drawing.Point(10, 101);
            this.compareMetricLabel.Name = "compareMetricLabel";
            this.compareMetricLabel.Size = new System.Drawing.Size(39, 13);
            this.compareMetricLabel.TabIndex = 6;
            this.compareMetricLabel.Text = "Metric:";
            // 
            // compareServerLabel
            // 
            this.compareServerLabel.AutoSize = true;
            this.compareServerLabel.Location = new System.Drawing.Point(10, 48);
            this.compareServerLabel.Name = "compareServerLabel";
            this.compareServerLabel.Size = new System.Drawing.Size(41, 13);
            this.compareServerLabel.TabIndex = 5;
            this.compareServerLabel.Text = "Server:";
            // 
            // compareTagLabel
            // 
            this.compareTagLabel.AutoSize = true;
            this.compareTagLabel.Location = new System.Drawing.Point(10, 21);
            this.compareTagLabel.Name = "compareTagLabel";
            this.compareTagLabel.Size = new System.Drawing.Size(29, 13);
            this.compareTagLabel.TabIndex = 4;
            this.compareTagLabel.Text = "Tag:";
            // 
            // compareMetricCombo
            // 
            this.compareMetricCombo.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.compareMetricCombo.FormattingEnabled = true;
            this.compareMetricCombo.Location = new System.Drawing.Point(90, 98);
            this.compareMetricCombo.Name = "compareMetricCombo";
            this.compareMetricCombo.Size = new System.Drawing.Size(261, 21);
            this.compareMetricCombo.TabIndex = 2;
            this.compareMetricCombo.SelectedValueChanged += new System.EventHandler(this.metricCombo_SelectedValueChanged);
            // 
            // compareSameServerChkBox
            // 
            this.compareSameServerChkBox.AutoSize = true;
            this.compareSameServerChkBox.Location = new System.Drawing.Point(90, 72);
            this.compareSameServerChkBox.Name = "compareSameServerChkBox";
            this.compareSameServerChkBox.Size = new System.Drawing.Size(150, 17);
            this.compareSameServerChkBox.TabIndex = 15;
            this.compareSameServerChkBox.Text = "Compare with same server";
            this.compareSameServerChkBox.UseVisualStyleBackColor = true;
            this.compareSameServerChkBox.Click += new System.EventHandler(this.compareSameServerChkBox_Click);
            // 
            // compareInstanceCombo
            // 
            this.compareInstanceCombo.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.compareInstanceCombo.FormattingEnabled = true;
            this.compareInstanceCombo.Location = new System.Drawing.Point(90, 45);
            this.compareInstanceCombo.Name = "compareInstanceCombo";
            this.compareInstanceCombo.Size = new System.Drawing.Size(261, 21);
            this.compareInstanceCombo.TabIndex = 1;
            this.compareInstanceCombo.DropDown += new System.EventHandler(this.compareInstanceCombo_DropDown);
            this.compareInstanceCombo.SelectedIndexChanged += new System.EventHandler(this.compareInstanceCombo_SelectedIndexChanged);
            // 
            // compareTagsCombo
            // 
            this.compareTagsCombo.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.compareTagsCombo.FormattingEnabled = true;
            this.compareTagsCombo.Location = new System.Drawing.Point(90, 18);
            this.compareTagsCombo.Name = "compareTagsCombo";
            this.compareTagsCombo.Size = new System.Drawing.Size(261, 21);
            this.compareTagsCombo.TabIndex = 0;
            this.compareTagsCombo.SelectedIndexChanged += new System.EventHandler(this.compareTagsCombo_SelectedIndexChanged);
            // 
            // BaselineStatistics
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.Name = "BaselineStatistics";
            this.panel1.ResumeLayout(false);
            this.filterPanel.ResumeLayout(false);
            this.filterPanel.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.startHoursTimeEditor)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.endHoursTimeEditor)).EndInit();
            this.compareToPanel.ResumeLayout(false);
            this.compareToPanel.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.compareEndHoursTimeEditor)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.compareStartHoursTimeEditor)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TextBox textBox2;
        private System.Windows.Forms.Label endHoursLbl;
        private System.Windows.Forms.Label startHoursLbl;
        private Common.UI.Controls.TimeComboEditor endHoursTimeEditor;
        private Common.UI.Controls.TimeComboEditor startHoursTimeEditor;
        private System.Windows.Forms.ComboBox metricCombo;
        private System.Windows.Forms.Label lblMetric;
        private System.Windows.Forms.GroupBox compareToPanel;
        private System.Windows.Forms.Label comparePeriodLabel;
        private System.Windows.Forms.Label compareMetricLabel;
        private System.Windows.Forms.Label compareServerLabel;
        private System.Windows.Forms.Label compareTagLabel;
        private System.Windows.Forms.ComboBox compareMetricCombo;
        private System.Windows.Forms.ComboBox compareInstanceCombo;
        private System.Windows.Forms.ComboBox compareTagsCombo;
        private System.Windows.Forms.DateTimePicker compareStartDateTimePicker;
        private System.Windows.Forms.TextBox compareEndDateText;
        private System.Windows.Forms.Label toLabel;
        private Common.UI.Controls.TimeComboEditor compareEndHoursTimeEditor;
        private Common.UI.Controls.TimeComboEditor compareStartHoursTimeEditor;
        private System.Windows.Forms.Label compareEndHours;
        private System.Windows.Forms.Label compareStartHours;
        private System.Windows.Forms.CheckBox compareSameServerChkBox;
    }
}
