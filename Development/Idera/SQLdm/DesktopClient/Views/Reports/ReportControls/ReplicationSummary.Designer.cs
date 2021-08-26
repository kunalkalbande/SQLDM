namespace Idera.SQLdm.DesktopClient.Views.Reports.ReportControls
{
    partial class ReplicationSummary
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ReplicationSummary));
            Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomDropDownEditorButton dropDownEditorButton5 = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomDropDownEditorButton("DropDownList");
            Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomDropDownEditorButton dropDownEditorButton6 = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomDropDownEditorButton("DropDownList");
            this.showTablesCheckbox = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomCheckBox();
            this.startHoursTimeEditor = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomTimeComboEditor();
            this.endHoursTimeEditor = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomTimeComboEditor();
            this.startHoursLbl = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomLabel();
            this.endHoursLbl = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomLabel();
            this.showBaselineCheckBox = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomCheckBox();
            this.panel1.SuspendLayout();
            this.filterPanel.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.startHoursTimeEditor)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.endHoursTimeEditor)).BeginInit();
            this.SuspendLayout();
            // 
            // panel1
            // 
            this.panel1.Size = new System.Drawing.Size(752, 170);
            // 
            // reportViewer
            // 
            this.reportViewer.Size = new System.Drawing.Size(752, 434);
            // 
            // filterPanel
            // 
            this.filterPanel.Controls.Add(this.showBaselineCheckBox);
            this.filterPanel.Controls.Add(this.showTablesCheckbox);
            this.filterPanel.Controls.Add(this.startHoursTimeEditor);
            this.filterPanel.Controls.Add(this.startHoursLbl);
            this.filterPanel.Controls.Add(this.endHoursTimeEditor);
            this.filterPanel.Controls.Add(this.endHoursLbl);
            this.filterPanel.Size = new System.Drawing.Size(752, 170);
            this.filterPanel.Controls.SetChildIndex(this.endHoursLbl, 0);
            this.filterPanel.Controls.SetChildIndex(this.endHoursTimeEditor, 0);
            this.filterPanel.Controls.SetChildIndex(this.startHoursLbl, 0);
            this.filterPanel.Controls.SetChildIndex(this.startHoursTimeEditor, 0);
            this.filterPanel.Controls.SetChildIndex(this.rangeInfoLabel, 0);
            this.filterPanel.Controls.SetChildIndex(this.rangeLabel, 0);
            this.filterPanel.Controls.SetChildIndex(this.showTablesCheckbox, 0);
            this.filterPanel.Controls.SetChildIndex(this.tagsLabel, 0);
            this.filterPanel.Controls.SetChildIndex(this.periodLabel, 0);
            this.filterPanel.Controls.SetChildIndex(this.sampleLabel, 0);
            this.filterPanel.Controls.SetChildIndex(this.instanceLabel, 0);
            this.filterPanel.Controls.SetChildIndex(this.periodCombo, 0);
            this.filterPanel.Controls.SetChildIndex(this.sampleSizeCombo, 0);
            this.filterPanel.Controls.SetChildIndex(this.instanceCombo, 0);
            this.filterPanel.Controls.SetChildIndex(this.tagsComboBox, 0);
            this.filterPanel.Controls.SetChildIndex(this.showBaselineCheckBox, 0);
            // 
            // tagsLabel
            // 
            this.tagsLabel.Location = new System.Drawing.Point(459, 194);
            this.tagsLabel.Visible = false;
            // 
            // periodLabel
            // 
            this.periodLabel.Location = new System.Drawing.Point(10, 37);
            // 
            // sampleLabel
            // 
            this.sampleLabel.Location = new System.Drawing.Point(10, 90);
            // 
            // instanceLabel
            // 
            this.instanceLabel.Location = new System.Drawing.Point(10, 10);
            // 
            // periodCombo
            // 
            this.periodCombo.Location = new System.Drawing.Point(61, 34);
            this.periodCombo.Size = new System.Drawing.Size(300, 21);
            this.periodCombo.TabIndex = 0;
            this.periodCombo.SelectedIndexChanged += new System.EventHandler(this.periodCombo_SelectedIndexChanged);
            // 
            // sampleSizeCombo
            // 
            this.sampleSizeCombo.Location = new System.Drawing.Point(61, 87);
            this.sampleSizeCombo.Size = new System.Drawing.Size(300, 21);
            this.sampleSizeCombo.TabIndex = 1;
            // 
            // instanceCombo
            // 
            this.instanceCombo.Location = new System.Drawing.Point(61, 7);
            this.instanceCombo.Size = new System.Drawing.Size(300, 21);
            this.instanceCombo.TabIndex = 2;
            // 
            // tagsComboBox
            // 
            this.tagsComboBox.Location = new System.Drawing.Point(499, 191);
            this.tagsComboBox.Visible = false;
            // 
            // reportInstructionsControl
            // 
            this.reportInstructionsControl.ReportInstructions = resources.GetString("reportInstructionsControl.ReportInstructions");
            this.reportInstructionsControl.Size = new System.Drawing.Size(752, 434);
            // 
            // rangeLabel
            // 
            this.rangeLabel.Location = new System.Drawing.Point(61, 61);
            this.rangeLabel.Size = new System.Drawing.Size(300, 21);
            // 
            // rangeInfoLabel
            // 
            this.rangeInfoLabel.Location = new System.Drawing.Point(10, 65);
            // 
            // showTablesCheckbox
            // 
            this.showTablesCheckbox.AutoSize = true;
            this.showTablesCheckbox.Location = new System.Drawing.Point(61, 114);
            this.showTablesCheckbox.Name = "showTablesCheckbox";
            this.showTablesCheckbox.Size = new System.Drawing.Size(118, 17);
            this.showTablesCheckbox.TabIndex = 3;
            this.showTablesCheckbox.Text = "Show Tabular Data";
            this.showTablesCheckbox.UseVisualStyleBackColor = true;
            // 
            // startHoursTimeEditor
            // 
            dropDownEditorButton5.Key = "DropDownList";
            dropDownEditorButton5.PreferredDropDownSize = new System.Drawing.Size(0, 0);
            this.startHoursTimeEditor.ButtonsRight.Add(dropDownEditorButton5);
            this.startHoursTimeEditor.DropDownButtonDisplayStyle = Infragistics.Win.ButtonDisplayStyle.Never;
            this.startHoursTimeEditor.ListInterval = System.TimeSpan.Parse("01:00:00");
            this.startHoursTimeEditor.Location = new System.Drawing.Point(462, 7);
            this.startHoursTimeEditor.MaskInput = "{time}";
            this.startHoursTimeEditor.Name = "startHoursTimeEditor";
            this.startHoursTimeEditor.Size = new System.Drawing.Size(92, 21);
            this.startHoursTimeEditor.TabIndex = 30;
            this.startHoursTimeEditor.Time = System.TimeSpan.Parse("00:00:00");
            this.startHoursTimeEditor.ValueChanged += new System.EventHandler(this.startHoursTimeEditor_ValueChanged);
            // 
            // endHoursTimeEditor
            // 
            dropDownEditorButton6.Key = "DropDownList";
            dropDownEditorButton6.PreferredDropDownSize = new System.Drawing.Size(0, 0);
            this.endHoursTimeEditor.ButtonsRight.Add(dropDownEditorButton6);
            this.endHoursTimeEditor.DateTime = new System.DateTime(2012, 5, 2, 23, 0, 0, 0);
            this.endHoursTimeEditor.DropDownButtonDisplayStyle = Infragistics.Win.ButtonDisplayStyle.Never;
            this.endHoursTimeEditor.ListInterval = System.TimeSpan.Parse("01:00:00");
            this.endHoursTimeEditor.Location = new System.Drawing.Point(636, 7);
            this.endHoursTimeEditor.MaskInput = "{time}";
            this.endHoursTimeEditor.Name = "endHoursTimeEditor";
            this.endHoursTimeEditor.Size = new System.Drawing.Size(92, 21);
            this.endHoursTimeEditor.TabIndex = 31;
            this.endHoursTimeEditor.Time = System.TimeSpan.Parse("23:00:00");
            this.endHoursTimeEditor.Value = new System.DateTime(2012, 5, 2, 23, 0, 0, 0);
            this.endHoursTimeEditor.ValueChanged += new System.EventHandler(this.endHoursTimeEditor_ValueChanged);
            // 
            // startHoursLbl
            // 
            this.startHoursLbl.AutoSize = true;
            this.startHoursLbl.Location = new System.Drawing.Point(393, 10);
            this.startHoursLbl.Name = "startHoursLbl";
            this.startHoursLbl.Size = new System.Drawing.Size(63, 13);
            this.startHoursLbl.TabIndex = 32;
            this.startHoursLbl.Text = "Start Hours:";
            // 
            // endHoursLbl
            // 
            this.endHoursLbl.AutoSize = true;
            this.endHoursLbl.Location = new System.Drawing.Point(570, 10);
            this.endHoursLbl.Name = "endHoursLbl";
            this.endHoursLbl.Size = new System.Drawing.Size(60, 13);
            this.endHoursLbl.TabIndex = 33;
            this.endHoursLbl.Text = "End Hours:";
            // 
            // showBaselineCheckBox
            // 
            this.showBaselineCheckBox.AutoSize = true;
            this.showBaselineCheckBox.Location = new System.Drawing.Point(185, 114);
            this.showBaselineCheckBox.Name = "showBaselineCheckBox";
            this.showBaselineCheckBox.Size = new System.Drawing.Size(96, 17);
            this.showBaselineCheckBox.TabIndex = 35;
            this.showBaselineCheckBox.Text = "Show Baseline";
            this.showBaselineCheckBox.UseVisualStyleBackColor = true;
            this.showBaselineCheckBox.Checked = false;
            this.showBaselineCheckBox.CheckedChanged += new System.EventHandler(showBaselineCheckBox_CheckedChanged);
            // 
            // ReplicationSummary
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Name = "ReplicationSummary";
            this.panel1.ResumeLayout(false);
            this.filterPanel.ResumeLayout(false);
            this.filterPanel.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.startHoursTimeEditor)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.endHoursTimeEditor)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomCheckBox showTablesCheckbox;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomTimeComboEditor startHoursTimeEditor;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomLabel startHoursLbl;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomTimeComboEditor endHoursTimeEditor;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomLabel endHoursLbl;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomCheckBox showBaselineCheckBox;
    }
}
