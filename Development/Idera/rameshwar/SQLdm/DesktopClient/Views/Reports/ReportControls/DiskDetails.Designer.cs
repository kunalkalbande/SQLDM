namespace Idera.SQLdm.DesktopClient.Views.Reports.ReportControls
{
   partial class DiskDetails
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(DiskDetails));
            Infragistics.Win.UltraWinEditors.DropDownEditorButton dropDownEditorButton2 = new Infragistics.Win.UltraWinEditors.DropDownEditorButton("DropDownList");
            Infragistics.Win.UltraWinEditors.DropDownEditorButton dropDownEditorButton1 = new Infragistics.Win.UltraWinEditors.DropDownEditorButton("DropDownList");
            this.textBox2 = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.driveNamesCombo = new System.Windows.Forms.ComboBox();
            this.forecastTypeCombo = new System.Windows.Forms.ComboBox();
            this.showTablesCheckbox = new System.Windows.Forms.CheckBox();
            this.startHoursTimeEditor = new Idera.SQLdm.Common.UI.Controls.TimeComboEditor();
            this.endHoursTimeEditor = new Idera.SQLdm.Common.UI.Controls.TimeComboEditor();
            this.startHoursLbl = new System.Windows.Forms.Label();
            this.endHoursLbl = new System.Windows.Forms.Label();
            this.cmbAxis = new System.Windows.Forms.ComboBox();
            this.label5 = new System.Windows.Forms.Label();
            this.panel1.SuspendLayout();
            this.filterPanel.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.startHoursTimeEditor)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.endHoursTimeEditor)).BeginInit();
            this.SuspendLayout();
            // 
            // panel1
            // 
            this.panel1.Size = new System.Drawing.Size(752, 176);
            // 
            // reportViewer
            // 
            this.reportViewer.Size = new System.Drawing.Size(752, 428);
            // 
            // filterPanel
            // 
            this.filterPanel.Controls.Add(this.cmbAxis);
            this.filterPanel.Controls.Add(this.label5);
            this.filterPanel.Controls.Add(this.showTablesCheckbox);
            this.filterPanel.Controls.Add(this.driveNamesCombo);
            this.filterPanel.Controls.Add(this.label4);
            this.filterPanel.Controls.Add(this.startHoursLbl);
            this.filterPanel.Controls.Add(this.endHoursLbl);
            this.filterPanel.Controls.Add(this.endHoursTimeEditor);
            this.filterPanel.Controls.Add(this.startHoursTimeEditor);
            this.filterPanel.Size = new System.Drawing.Size(752, 176);
            this.filterPanel.Controls.SetChildIndex(this.startHoursTimeEditor, 0);
            this.filterPanel.Controls.SetChildIndex(this.endHoursTimeEditor, 0);
            this.filterPanel.Controls.SetChildIndex(this.endHoursLbl, 0);
            this.filterPanel.Controls.SetChildIndex(this.startHoursLbl, 0);
            this.filterPanel.Controls.SetChildIndex(this.label4, 0);
            this.filterPanel.Controls.SetChildIndex(this.driveNamesCombo, 0);
            this.filterPanel.Controls.SetChildIndex(this.showTablesCheckbox, 0);
            this.filterPanel.Controls.SetChildIndex(this.sampleLabel, 0);
            this.filterPanel.Controls.SetChildIndex(this.periodLabel, 0);
            this.filterPanel.Controls.SetChildIndex(this.rangeInfoLabel, 0);
            this.filterPanel.Controls.SetChildIndex(this.rangeLabel, 0);
            this.filterPanel.Controls.SetChildIndex(this.sampleSizeCombo, 0);
            this.filterPanel.Controls.SetChildIndex(this.periodCombo, 0);
            this.filterPanel.Controls.SetChildIndex(this.instanceCombo, 0);
            this.filterPanel.Controls.SetChildIndex(this.tagsComboBox, 0);
            this.filterPanel.Controls.SetChildIndex(this.instanceLabel, 0);
            this.filterPanel.Controls.SetChildIndex(this.tagsLabel, 0);
            this.filterPanel.Controls.SetChildIndex(this.label5, 0);
            this.filterPanel.Controls.SetChildIndex(this.cmbAxis, 0);
            // 
            // tagsLabel
            // 
            this.tagsLabel.Location = new System.Drawing.Point(500, 193);
            this.tagsLabel.Visible = false;
            // 
            // periodLabel
            // 
            this.periodLabel.Location = new System.Drawing.Point(10, 37);
            // 
            // sampleLabel
            // 
            this.sampleLabel.Location = new System.Drawing.Point(10, 89);
            // 
            // instanceLabel
            // 
            this.instanceLabel.Location = new System.Drawing.Point(10, 10);
            // 
            // periodCombo
            // 
            this.periodCombo.Location = new System.Drawing.Point(78, 34);
            this.periodCombo.Size = new System.Drawing.Size(300, 21);
            this.periodCombo.TabIndex = 0;
            this.periodCombo.SelectedIndexChanged += new System.EventHandler(this.periodCombo_SelectedIndexChanged);
            // 
            // sampleSizeCombo
            // 
            this.sampleSizeCombo.Location = new System.Drawing.Point(78, 86);
            this.sampleSizeCombo.Size = new System.Drawing.Size(300, 21);
            this.sampleSizeCombo.TabIndex = 1;
            // 
            // instanceCombo
            // 
            this.instanceCombo.Location = new System.Drawing.Point(78, 7);
            this.instanceCombo.Size = new System.Drawing.Size(300, 21);
            this.instanceCombo.TabIndex = 2;
            this.instanceCombo.SelectionChangeCommitted += new System.EventHandler(this.instanceCombo_SelectionChangeCommitted);
            // 
            // tagsComboBox
            // 
            this.tagsComboBox.Location = new System.Drawing.Point(535, 190);
            this.tagsComboBox.TabIndex = 0;
            this.tagsComboBox.Visible = false;
            // 
            // reportInstructionsControl
            // 
            this.reportInstructionsControl.ReportInstructions = resources.GetString("reportInstructionsControl.ReportInstructions");
            this.reportInstructionsControl.Size = new System.Drawing.Size(752, 428);
            // 
            // rangeLabel
            // 
            this.rangeLabel.Location = new System.Drawing.Point(78, 60);
            this.rangeLabel.Size = new System.Drawing.Size(300, 21);
            // 
            // rangeInfoLabel
            // 
            this.rangeInfoLabel.Location = new System.Drawing.Point(10, 64);
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
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(398, 10);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(66, 13);
            this.label4.TabIndex = 28;
            this.label4.Text = "Drive Name:";
            // 
            // driveNamesCombo
            // 
            this.driveNamesCombo.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.driveNamesCombo.FormattingEnabled = true;
            this.driveNamesCombo.Location = new System.Drawing.Point(473, 7);
            this.driveNamesCombo.Name = "driveNamesCombo";
            this.driveNamesCombo.Size = new System.Drawing.Size(269, 21);
            this.driveNamesCombo.Sorted = true;
            this.driveNamesCombo.TabIndex = 3;
            // 
            // forecastTypeCombo
            // 
            this.forecastTypeCombo.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.forecastTypeCombo.FormattingEnabled = true;
            this.forecastTypeCombo.Location = new System.Drawing.Point(461, 34);
            this.forecastTypeCombo.Name = "forecastTypeCombo";
            this.forecastTypeCombo.Size = new System.Drawing.Size(200, 21);
            this.forecastTypeCombo.TabIndex = 5;
            // 
            // showTablesCheckbox
            // 
            this.showTablesCheckbox.AutoSize = true;
            this.showTablesCheckbox.Location = new System.Drawing.Point(401, 38);
            this.showTablesCheckbox.Name = "showTablesCheckbox";
            this.showTablesCheckbox.Size = new System.Drawing.Size(118, 17);
            this.showTablesCheckbox.TabIndex = 30;
            this.showTablesCheckbox.Text = "Show Tabular Data";
            this.showTablesCheckbox.UseVisualStyleBackColor = true;
            // 
            // startHoursTimeEditor
            // 
            dropDownEditorButton2.Key = "DropDownList";
            dropDownEditorButton2.PreferredDropDownSize = new System.Drawing.Size(0, 0);
            this.startHoursTimeEditor.ButtonsRight.Add(dropDownEditorButton2);
            this.startHoursTimeEditor.DropDownButtonDisplayStyle = Infragistics.Win.ButtonDisplayStyle.Never;
            this.startHoursTimeEditor.ListInterval = System.TimeSpan.Parse("01:00:00");
            this.startHoursTimeEditor.Location = new System.Drawing.Point(473, 64);
            this.startHoursTimeEditor.MaskInput = "{time}";
            this.startHoursTimeEditor.Name = "startHoursTimeEditor";
            this.startHoursTimeEditor.Size = new System.Drawing.Size(92, 21);
            this.startHoursTimeEditor.TabIndex = 31;
            this.startHoursTimeEditor.Time = System.TimeSpan.Parse("00:00:00");
            this.startHoursTimeEditor.ValueChanged += new System.EventHandler(this.startHoursTimeEditor_ValueChanged);
            // 
            // endHoursTimeEditor
            // 
            dropDownEditorButton1.Key = "DropDownList";
            dropDownEditorButton1.PreferredDropDownSize = new System.Drawing.Size(0, 0);
            this.endHoursTimeEditor.ButtonsRight.Add(dropDownEditorButton1);
            this.endHoursTimeEditor.DateTime = new System.DateTime(2012, 4, 24, 23, 0, 0, 0);
            this.endHoursTimeEditor.DropDownButtonDisplayStyle = Infragistics.Win.ButtonDisplayStyle.Never;
            this.endHoursTimeEditor.ListInterval = System.TimeSpan.Parse("01:00:00");
            this.endHoursTimeEditor.Location = new System.Drawing.Point(650, 64);
            this.endHoursTimeEditor.MaskInput = "{time}";
            this.endHoursTimeEditor.Name = "endHoursTimeEditor";
            this.endHoursTimeEditor.Size = new System.Drawing.Size(92, 21);
            this.endHoursTimeEditor.TabIndex = 32;
            this.endHoursTimeEditor.Time = System.TimeSpan.Parse("23:00:00");
            this.endHoursTimeEditor.Value = new System.DateTime(2012, 4, 24, 23, 0, 0, 0);
            this.endHoursTimeEditor.ValueChanged += new System.EventHandler(this.endHoursTimeEditor_ValueChanged);
            // 
            // startHoursLbl
            // 
            this.startHoursLbl.AutoSize = true;
            this.startHoursLbl.Location = new System.Drawing.Point(398, 68);
            this.startHoursLbl.Name = "startHoursLbl";
            this.startHoursLbl.Size = new System.Drawing.Size(63, 13);
            this.startHoursLbl.TabIndex = 33;
            this.startHoursLbl.Text = "Start Hours:";
            // 
            // endHoursLbl
            // 
            this.endHoursLbl.AutoSize = true;
            this.endHoursLbl.Location = new System.Drawing.Point(584, 68);
            this.endHoursLbl.Name = "endHoursLbl";
            this.endHoursLbl.Size = new System.Drawing.Size(60, 13);
            this.endHoursLbl.TabIndex = 34;
            this.endHoursLbl.Text = "End Hours:";
            // 
            // cmbAxis
            // 
            this.cmbAxis.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbAxis.FormattingEnabled = true;
            this.cmbAxis.Location = new System.Drawing.Point(78, 113);
            this.cmbAxis.Name = "cmbAxis";
            this.cmbAxis.Size = new System.Drawing.Size(300, 21);
            this.cmbAxis.Sorted = true;
            this.cmbAxis.TabIndex = 45;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(10, 116);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(62, 13);
            this.label5.TabIndex = 44;
            this.label5.Text = "Chart Type:";
            // 
            // DiskDetails
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.Name = "DiskDetails";
            this.panel1.ResumeLayout(false);
            this.filterPanel.ResumeLayout(false);
            this.filterPanel.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.startHoursTimeEditor)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.endHoursTimeEditor)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox textBox2;
        private System.Windows.Forms.ComboBox driveNamesCombo;
      private System.Windows.Forms.Label label4;
      private System.Windows.Forms.ComboBox forecastTypeCombo;
      private System.Windows.Forms.CheckBox showTablesCheckbox;
      private System.Windows.Forms.Label startHoursLbl;
      private System.Windows.Forms.Label endHoursLbl;
      private Common.UI.Controls.TimeComboEditor endHoursTimeEditor;
      private Common.UI.Controls.TimeComboEditor startHoursTimeEditor;
      private System.Windows.Forms.ComboBox cmbAxis;
      private System.Windows.Forms.Label label5;
    }
}
