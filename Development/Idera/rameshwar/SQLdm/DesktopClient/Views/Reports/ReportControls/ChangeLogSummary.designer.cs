using System;

namespace Idera.SQLdm.DesktopClient.Views.Reports.ReportControls
{
    partial class ChangeLogSummary
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ChangeLogSummary));
            Infragistics.Win.UltraWinEditors.DropDownEditorButton dropDownEditorButton2 = new Infragistics.Win.UltraWinEditors.DropDownEditorButton("DropDownList");
            Infragistics.Win.UltraWinEditors.DropDownEditorButton dropDownEditorButton1 = new Infragistics.Win.UltraWinEditors.DropDownEditorButton("DropDownList");
            this.runReportButton = new Infragistics.Win.Misc.UltraButton();
            this.actionTypeLabel = new System.Windows.Forms.Label();
            this.actionTypeCombo = new System.Windows.Forms.ComboBox();
            this.repositoryUserLabel = new System.Windows.Forms.Label();
            this.repositoryUserCombo = new System.Windows.Forms.ComboBox();
            this.workstationLabel = new System.Windows.Forms.Label();
            this.workstationCombo = new System.Windows.Forms.ComboBox();
            this.workstationUserLabel = new System.Windows.Forms.Label();
            this.workstationUserCombo = new System.Windows.Forms.ComboBox();
            this.startHoursLbl = new System.Windows.Forms.Label();
            this.startHoursTimeEditor = new Idera.SQLdm.Common.UI.Controls.TimeComboEditor();
            this.endHoursLbl = new System.Windows.Forms.Label();
            this.endHoursTimeEditor = new Idera.SQLdm.Common.UI.Controls.TimeComboEditor();
            this.panel1.SuspendLayout();
            this.filterPanel.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.startHoursTimeEditor)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.endHoursTimeEditor)).BeginInit();
            this.SuspendLayout();
            // 
            // panel1
            // 
            this.panel1.Size = new System.Drawing.Size(752, 148);
            // 
            // reportViewer
            // 
            this.reportViewer.Size = new System.Drawing.Size(752, 649);
            // 
            // filterPanel
            // 
            this.filterPanel.Controls.Add(this.endHoursTimeEditor);
            this.filterPanel.Controls.Add(this.endHoursLbl);
            this.filterPanel.Controls.Add(this.startHoursLbl);
            this.filterPanel.Controls.Add(this.startHoursTimeEditor);
            this.filterPanel.Controls.Add(this.workstationUserCombo);
            this.filterPanel.Controls.Add(this.workstationUserLabel);
            this.filterPanel.Controls.Add(this.workstationCombo);
            this.filterPanel.Controls.Add(this.workstationLabel);
            this.filterPanel.Controls.Add(this.repositoryUserCombo);
            this.filterPanel.Controls.Add(this.repositoryUserLabel);
            this.filterPanel.Controls.Add(this.actionTypeCombo);
            this.filterPanel.Controls.Add(this.actionTypeLabel);
            this.filterPanel.Size = new System.Drawing.Size(752, 148);
            this.filterPanel.Controls.SetChildIndex(this.tagsLabel, 0);
            this.filterPanel.Controls.SetChildIndex(this.instanceLabel, 0);
            this.filterPanel.Controls.SetChildIndex(this.instanceCombo, 0);
            this.filterPanel.Controls.SetChildIndex(this.tagsComboBox, 0);
            this.filterPanel.Controls.SetChildIndex(this.actionTypeLabel, 0);
            this.filterPanel.Controls.SetChildIndex(this.actionTypeCombo, 0);
            this.filterPanel.Controls.SetChildIndex(this.repositoryUserLabel, 0);
            this.filterPanel.Controls.SetChildIndex(this.repositoryUserCombo, 0);
            this.filterPanel.Controls.SetChildIndex(this.workstationLabel, 0);
            this.filterPanel.Controls.SetChildIndex(this.workstationCombo, 0);
            this.filterPanel.Controls.SetChildIndex(this.workstationUserLabel, 0);
            this.filterPanel.Controls.SetChildIndex(this.workstationUserCombo, 0);
            this.filterPanel.Controls.SetChildIndex(this.rangeLabel, 0);
            this.filterPanel.Controls.SetChildIndex(this.sampleLabel, 0);
            this.filterPanel.Controls.SetChildIndex(this.rangeInfoLabel, 0);
            this.filterPanel.Controls.SetChildIndex(this.periodLabel, 0);
            this.filterPanel.Controls.SetChildIndex(this.periodCombo, 0);
            this.filterPanel.Controls.SetChildIndex(this.startHoursTimeEditor, 0);
            this.filterPanel.Controls.SetChildIndex(this.startHoursLbl, 0);
            this.filterPanel.Controls.SetChildIndex(this.endHoursLbl, 0);
            this.filterPanel.Controls.SetChildIndex(this.sampleSizeCombo, 0);
            this.filterPanel.Controls.SetChildIndex(this.endHoursTimeEditor, 0);
            // 
            // tagsLabel
            // 
            this.tagsLabel.Location = new System.Drawing.Point(33, 500);
            // 
            // periodLabel
            // 
            this.periodLabel.Location = new System.Drawing.Point(4, 65);
            // 
            // sampleLabel
            // 
            this.sampleLabel.Location = new System.Drawing.Point(372, 284);
            this.sampleLabel.Visible = false;
            // 
            // instanceLabel
            // 
            this.instanceLabel.Location = new System.Drawing.Point(349, 500);
            // 
            // periodCombo
            // 
            this.periodCombo.Location = new System.Drawing.Point(73, 62);
            this.periodCombo.Size = new System.Drawing.Size(290, 21);
            this.periodCombo.TabIndex = 0;
            this.periodCombo.SelectedIndexChanged += new System.EventHandler(this.periodCombo_SelectedIndexChanged);
            // 
            // sampleSizeCombo
            // 
            this.sampleSizeCombo.Location = new System.Drawing.Point(422, 281);
            this.sampleSizeCombo.Size = new System.Drawing.Size(275, 21);
            this.sampleSizeCombo.TabIndex = 1;
            this.sampleSizeCombo.Visible = false;
            // 
            // instanceCombo
            // 
            this.instanceCombo.Location = new System.Drawing.Point(349, 500);
            // 
            // tagsComboBox
            // 
            this.tagsComboBox.Location = new System.Drawing.Point(81, 500);
            // 
            // runReportButton
            // 
            this.runReportButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.runReportButton.ButtonStyle = Infragistics.Win.UIElementButtonStyle.WindowsVistaButton;
            this.runReportButton.Location = new System.Drawing.Point(671, 5);
            this.runReportButton.Name = "runReportButton";
            this.runReportButton.ShowFocusRect = false;
            this.runReportButton.ShowOutline = false;
            this.runReportButton.TabIndex = 0;
            this.runReportButton.Text = "Run Report";
            this.runReportButton.UseAppStyling = false;
            this.runReportButton.UseOsThemes = Infragistics.Win.DefaultableBoolean.False;
            // 
            // reportInstructionsControl
            // 
            this.reportInstructionsControl.ReportInstructions = resources.GetString("reportInstructionsControl.ReportInstructions");
            this.reportInstructionsControl.Size = new System.Drawing.Size(752, 649);
            // 
            // rangeLabel
            // 
            this.rangeLabel.Location = new System.Drawing.Point(73, 86);
            this.rangeLabel.Size = new System.Drawing.Size(290, 21);
            // 
            // rangeInfoLabel
            // 
            this.rangeInfoLabel.Location = new System.Drawing.Point(4, 90);
            // 
            // actionTypeLabel
            // 
            this.actionTypeLabel.AutoSize = true;
            this.actionTypeLabel.Location = new System.Drawing.Point(4, 10);
            this.actionTypeLabel.Name = "actionTypeLabel";
            this.actionTypeLabel.Size = new System.Drawing.Size(40, 13);
            this.actionTypeLabel.TabIndex = 38;
            this.actionTypeLabel.Text = "Action:";
            // 
            // actionTypeCombo
            // 
            this.actionTypeCombo.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.actionTypeCombo.FormattingEnabled = true;
            this.actionTypeCombo.Location = new System.Drawing.Point(73, 7);
            this.actionTypeCombo.Name = "actionTypeCombo";
            this.actionTypeCombo.Size = new System.Drawing.Size(290, 21);
            this.actionTypeCombo.TabIndex = 2;
            // 
            // repositoryUserLabel
            // 
            this.repositoryUserLabel.AutoSize = true;
            this.repositoryUserLabel.Location = new System.Drawing.Point(369, 10);
            this.repositoryUserLabel.Name = "repositoryUserLabel";
            this.repositoryUserLabel.Size = new System.Drawing.Size(85, 13);
            this.repositoryUserLabel.TabIndex = 37;
            this.repositoryUserLabel.Text = "Repository User:";
            // 
            // repositoryUserCombo
            // 
            this.repositoryUserCombo.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.repositoryUserCombo.FormattingEnabled = true;
            this.repositoryUserCombo.Location = new System.Drawing.Point(462, 7);
            this.repositoryUserCombo.Name = "repositoryUserCombo";
            this.repositoryUserCombo.Size = new System.Drawing.Size(288, 21);
            this.repositoryUserCombo.TabIndex = 3;
            this.repositoryUserCombo.DropDown += new System.EventHandler(this.repositoryUserCombo_DropDown);
            // 
            // workstationLabel
            // 
            this.workstationLabel.AutoSize = true;
            this.workstationLabel.Location = new System.Drawing.Point(4, 37);
            this.workstationLabel.Name = "workstationLabel";
            this.workstationLabel.Size = new System.Drawing.Size(67, 13);
            this.workstationLabel.TabIndex = 36;
            this.workstationLabel.Text = "Workstation:";
            // 
            // workstationCombo
            // 
            this.workstationCombo.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.workstationCombo.FormattingEnabled = true;
            this.workstationCombo.Location = new System.Drawing.Point(73, 34);
            this.workstationCombo.Name = "workstationCombo";
            this.workstationCombo.Size = new System.Drawing.Size(290, 21);
            this.workstationCombo.TabIndex = 4;
            this.workstationCombo.DropDown += new System.EventHandler(this.workstationCombo_DropDown);
            // 
            // workstationUserLabel
            // 
            this.workstationUserLabel.AutoSize = true;
            this.workstationUserLabel.Location = new System.Drawing.Point(369, 37);
            this.workstationUserLabel.Name = "workstationUserLabel";
            this.workstationUserLabel.Size = new System.Drawing.Size(92, 13);
            this.workstationUserLabel.TabIndex = 35;
            this.workstationUserLabel.Text = "Workstation User:";
            // 
            // workstationUserCombo
            // 
            this.workstationUserCombo.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.workstationUserCombo.FormattingEnabled = true;
            this.workstationUserCombo.Location = new System.Drawing.Point(462, 34);
            this.workstationUserCombo.Name = "workstationUserCombo";
            this.workstationUserCombo.Size = new System.Drawing.Size(288, 21);
            this.workstationUserCombo.TabIndex = 5;
            this.workstationUserCombo.DropDown += new System.EventHandler(this.workstationUserCombo_DropDown);
            // 
            // startHoursLbl
            // 
            this.startHoursLbl.AutoSize = true;
            this.startHoursLbl.Location = new System.Drawing.Point(450, 500);
            this.startHoursLbl.Name = "startHoursLbl";
            this.startHoursLbl.Size = new System.Drawing.Size(63, 13);
            this.startHoursLbl.TabIndex = 30;
            this.startHoursLbl.Text = "Start Hours:";
            this.startHoursLbl.Visible = false;
            // 
            // startHoursTimeEditor
            // 
            dropDownEditorButton2.Key = "DropDownList";
            dropDownEditorButton2.PreferredDropDownSize = new System.Drawing.Size(0, 0);
            this.startHoursTimeEditor.ButtonsRight.Add(dropDownEditorButton2);
            this.startHoursTimeEditor.DropDownButtonDisplayStyle = Infragistics.Win.ButtonDisplayStyle.Never;
            this.startHoursTimeEditor.ListInterval = System.TimeSpan.Parse("01:00:00");
            this.startHoursTimeEditor.Location = new System.Drawing.Point(520, 500);
            this.startHoursTimeEditor.MaskInput = "{time}";
            this.startHoursTimeEditor.Name = "startHoursTimeEditor";
            this.startHoursTimeEditor.Size = new System.Drawing.Size(92, 21);
            this.startHoursTimeEditor.TabIndex = 31;
            this.startHoursTimeEditor.Time = System.TimeSpan.Parse("00:00:00");
            this.startHoursTimeEditor.Visible = false;
            this.startHoursTimeEditor.ValueChanged += new System.EventHandler(this.startHoursTimeEditor_ValueChanged);
            // 
            // endHoursLbl
            // 
            this.endHoursLbl.AutoSize = true;
            this.endHoursLbl.Location = new System.Drawing.Point(625, 500);
            this.endHoursLbl.Name = "endHoursLbl";
            this.endHoursLbl.Size = new System.Drawing.Size(60, 13);
            this.endHoursLbl.TabIndex = 32;
            this.endHoursLbl.Text = "End Hours:";
            this.endHoursLbl.Visible = false;
            // 
            // endHoursTimeEditor
            // 
            dropDownEditorButton1.Key = "DropDownList";
            dropDownEditorButton1.PreferredDropDownSize = new System.Drawing.Size(0, 0);
            this.endHoursTimeEditor.ButtonsRight.Add(dropDownEditorButton1);
            this.endHoursTimeEditor.DateTime = new System.DateTime(2012, 6, 21, 23, 0, 0, 0);
            this.endHoursTimeEditor.DropDownButtonDisplayStyle = Infragistics.Win.ButtonDisplayStyle.Never;
            this.endHoursTimeEditor.ListInterval = System.TimeSpan.Parse("01:00:00");
            this.endHoursTimeEditor.Location = new System.Drawing.Point(692, 500);
            this.endHoursTimeEditor.MaskInput = "{time}";
            this.endHoursTimeEditor.Name = "endHoursTimeEditor";
            this.endHoursTimeEditor.Size = new System.Drawing.Size(92, 21);
            this.endHoursTimeEditor.TabIndex = 33;
            this.endHoursTimeEditor.Time = System.TimeSpan.Parse("23:00:00");
            this.endHoursTimeEditor.Value = new System.DateTime(2012, 6, 21, 23, 0, 0, 0);
            this.endHoursTimeEditor.Visible = false;
            this.endHoursTimeEditor.ValueChanged += new System.EventHandler(this.endHoursTimeEditor_ValueChanged);
            // 
            // ChangeLogSummary
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Name = "ChangeLogSummary";
            this.Size = new System.Drawing.Size(752, 800);
            this.panel1.ResumeLayout(false);
            this.filterPanel.ResumeLayout(false);
            this.filterPanel.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.startHoursTimeEditor)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.endHoursTimeEditor)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label actionTypeLabel;
        private System.Windows.Forms.ComboBox actionTypeCombo;
        private System.Windows.Forms.Label repositoryUserLabel;
        private System.Windows.Forms.ComboBox repositoryUserCombo;
        private System.Windows.Forms.Label workstationLabel;
        private System.Windows.Forms.ComboBox workstationCombo;
        private System.Windows.Forms.Label workstationUserLabel;
        private System.Windows.Forms.ComboBox workstationUserCombo;
        private System.Windows.Forms.Label startHoursLbl;
        private Idera.SQLdm.Common.UI.Controls.TimeComboEditor startHoursTimeEditor;
        private System.Windows.Forms.Label endHoursLbl;
        private Idera.SQLdm.Common.UI.Controls.TimeComboEditor endHoursTimeEditor;
        private Infragistics.Win.Misc.UltraButton runReportButton;
        //private Infragistics.Win.Misc.UltraButton runReportButton;
    }
}
