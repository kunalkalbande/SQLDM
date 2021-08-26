namespace Idera.SQLdm.DesktopClient.Views.Reports.ReportControls
{
    partial class TransactionLogStatistics
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(TransactionLogStatistics));
            Infragistics.Win.UltraWinEditors.DropDownEditorButton dropDownEditorButton2 = new Infragistics.Win.UltraWinEditors.DropDownEditorButton("DropDownList");
            Infragistics.Win.UltraWinEditors.DropDownEditorButton dropDownEditorButton1 = new Infragistics.Win.UltraWinEditors.DropDownEditorButton("DropDownList");
            this.label5 = new System.Windows.Forms.Label();
            this.chartTypeCombo = new System.Windows.Forms.ComboBox();
            this.startHoursTimeEditor = new Idera.SQLdm.Common.UI.Controls.TimeComboEditor();
            this.endHoursTimeEditor = new Idera.SQLdm.Common.UI.Controls.TimeComboEditor();
            this.startHoursLbl = new System.Windows.Forms.Label();
            this.endHoursLbl = new System.Windows.Forms.Label();
            this.panel1.SuspendLayout();
            this.filterPanel.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.startHoursTimeEditor)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.endHoursTimeEditor)).BeginInit();
            this.SuspendLayout();
            // 
            // databaseTextbox
            // 
            this.databaseTextbox.BackColor = System.Drawing.SystemColors.Window;
            this.databaseTextbox.Location = new System.Drawing.Point(462, 7);
            this.databaseTextbox.Size = new System.Drawing.Size(267, 20);
            this.databaseTextbox.TabIndex = 1;
            this.databaseTextbox.TabStop = false;
            // 
            // databaseBrowseButton
            // 
            this.databaseBrowseButton.Location = new System.Drawing.Point(733, 7);
            this.databaseBrowseButton.ShowFocusRect = false;
            this.databaseBrowseButton.ShowOutline = false;
            this.databaseBrowseButton.TabIndex = 2;
            // 
            // label1
            // 
            this.label1.ForeColor = System.Drawing.SystemColors.ControlText;
            this.label1.Location = new System.Drawing.Point(401, 10);
            // 
            // panel1
            // 
            this.panel1.Size = new System.Drawing.Size(833, 142);
            // 
            // reportViewer
            // 
            this.reportViewer.Size = new System.Drawing.Size(833, 462);
            // 
            // filterPanel
            // 
            this.filterPanel.Controls.Add(this.endHoursLbl);
            this.filterPanel.Controls.Add(this.startHoursLbl);
            this.filterPanel.Controls.Add(this.endHoursTimeEditor);
            this.filterPanel.Controls.Add(this.startHoursTimeEditor);
            this.filterPanel.Controls.Add(this.label5);
            this.filterPanel.Controls.Add(this.chartTypeCombo);
            this.filterPanel.Size = new System.Drawing.Size(833, 142);
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
            this.filterPanel.Controls.SetChildIndex(this.startHoursTimeEditor, 0);
            this.filterPanel.Controls.SetChildIndex(this.endHoursTimeEditor, 0);
            this.filterPanel.Controls.SetChildIndex(this.startHoursLbl, 0);
            this.filterPanel.Controls.SetChildIndex(this.endHoursLbl, 0);
            // 
            // tagsLabel
            // 
            this.tagsLabel.Location = new System.Drawing.Point(459, 198);
            this.tagsLabel.Visible = false;
            // 
            // periodLabel
            // 
            this.periodLabel.Location = new System.Drawing.Point(10, 37);
            // 
            // sampleLabel
            // 
            this.sampleLabel.Location = new System.Drawing.Point(10, 85);
            // 
            // instanceLabel
            // 
            this.instanceLabel.Location = new System.Drawing.Point(10, 10);
            // 
            // periodCombo
            // 
            this.periodCombo.Location = new System.Drawing.Point(72, 34);
            this.periodCombo.Size = new System.Drawing.Size(300, 21);
            this.periodCombo.TabIndex = 3;
            this.periodCombo.SelectedIndexChanged += new System.EventHandler(this.periodCombo_SelectedIndexChanged);
            // 
            // sampleSizeCombo
            // 
            this.sampleSizeCombo.Location = new System.Drawing.Point(72, 82);
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
            this.reportInstructionsControl.Size = new System.Drawing.Size(833, 462);
            // 
            // rangeLabel
            // 
            this.rangeLabel.Location = new System.Drawing.Point(72, 58);
            this.rangeLabel.Size = new System.Drawing.Size(300, 21);
            // 
            // rangeInfoLabel
            // 
            this.rangeInfoLabel.Location = new System.Drawing.Point(10, 62);
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(394, 37);
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
            "Cache Hit Ratio",
            "Cache Reads",
            "File Size",
            "Flush Waits",
            "Flushes",
            "Kilobytes Flushed"});
            this.chartTypeCombo.Location = new System.Drawing.Point(462, 34);
            this.chartTypeCombo.Name = "chartTypeCombo";
            this.chartTypeCombo.Size = new System.Drawing.Size(300, 21);
            this.chartTypeCombo.TabIndex = 5;
            this.chartTypeCombo.SelectedIndexChanged += new System.EventHandler(this.chartTypeCombo_SelectedIndexChanged);
            // 
            // startHoursTimeEditor
            // 
            dropDownEditorButton2.Key = "DropDownList";
            dropDownEditorButton2.PreferredDropDownSize = new System.Drawing.Size(0, 0);
            this.startHoursTimeEditor.ButtonsRight.Add(dropDownEditorButton2);
            this.startHoursTimeEditor.DropDownButtonDisplayStyle = Infragistics.Win.ButtonDisplayStyle.Never;
            this.startHoursTimeEditor.ListInterval = System.TimeSpan.Parse("01:00:00");
            this.startHoursTimeEditor.Location = new System.Drawing.Point(462, 61);
            this.startHoursTimeEditor.MaskInput = "{time}";
            this.startHoursTimeEditor.Name = "startHoursTimeEditor";
            this.startHoursTimeEditor.Size = new System.Drawing.Size(92, 21);
            this.startHoursTimeEditor.TabIndex = 48;
            this.startHoursTimeEditor.Time = System.TimeSpan.Parse("00:00:00");
            this.startHoursTimeEditor.ValueChanged += new System.EventHandler(this.startHoursTimeEditor_ValueChanged);
            // 
            // endHoursTimeEditor
            // 
            dropDownEditorButton1.Key = "DropDownList";
            dropDownEditorButton1.PreferredDropDownSize = new System.Drawing.Size(0, 0);
            this.endHoursTimeEditor.ButtonsRight.Add(dropDownEditorButton1);
            this.endHoursTimeEditor.DateTime = new System.DateTime(2012, 5, 3, 23, 0, 0, 0);
            this.endHoursTimeEditor.DropDownButtonDisplayStyle = Infragistics.Win.ButtonDisplayStyle.Never;
            this.endHoursTimeEditor.ListInterval = System.TimeSpan.Parse("01:00:00");
            this.endHoursTimeEditor.Location = new System.Drawing.Point(670, 61);
            this.endHoursTimeEditor.MaskInput = "{time}";
            this.endHoursTimeEditor.Name = "endHoursTimeEditor";
            this.endHoursTimeEditor.Size = new System.Drawing.Size(92, 21);
            this.endHoursTimeEditor.TabIndex = 49;
            this.endHoursTimeEditor.Time = System.TimeSpan.Parse("23:00:00");
            this.endHoursTimeEditor.Value = new System.DateTime(2012, 5, 3, 23, 0, 0, 0);
            this.endHoursTimeEditor.ValueChanged += new System.EventHandler(this.endHoursTimeEditor_ValueChanged);
            // 
            // startHoursLbl
            // 
            this.startHoursLbl.AutoSize = true;
            this.startHoursLbl.Location = new System.Drawing.Point(394, 65);
            this.startHoursLbl.Name = "startHoursLbl";
            this.startHoursLbl.Size = new System.Drawing.Size(63, 13);
            this.startHoursLbl.TabIndex = 50;
            this.startHoursLbl.Text = "Start Hours:";
            // 
            // endHoursLbl
            // 
            this.endHoursLbl.AutoSize = true;
            this.endHoursLbl.Location = new System.Drawing.Point(604, 65);
            this.endHoursLbl.Name = "endHoursLbl";
            this.endHoursLbl.Size = new System.Drawing.Size(60, 13);
            this.endHoursLbl.TabIndex = 51;
            this.endHoursLbl.Text = "End Hours:";
            // 
            // TransactionLogStatistics
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.Name = "TransactionLogStatistics";
            this.Size = new System.Drawing.Size(833, 607);
            this.panel1.ResumeLayout(false);
            this.filterPanel.ResumeLayout(false);
            this.filterPanel.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.startHoursTimeEditor)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.endHoursTimeEditor)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.ComboBox chartTypeCombo;
        private System.Windows.Forms.Label endHoursLbl;
        private System.Windows.Forms.Label startHoursLbl;
        private Common.UI.Controls.TimeComboEditor endHoursTimeEditor;
        private Common.UI.Controls.TimeComboEditor startHoursTimeEditor;
    }
}
