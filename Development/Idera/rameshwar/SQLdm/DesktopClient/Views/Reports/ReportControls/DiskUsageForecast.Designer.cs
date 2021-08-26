using Idera.SQLdm.DesktopClient.Controls;

namespace Idera.SQLdm.DesktopClient.Views.Reports.ReportControls
{
    partial class DiskUsageForecast
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
            if (disposing)
            {
                if (forecastParabola != null) forecastParabola.Dispose();
                if (forecastLinear   != null) forecastLinear.Dispose();
                if (forecastCubic    != null) forecastCubic.Dispose();
                if (forecastQuartic  != null) forecastQuartic.Dispose();

                if(components != null)
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(DiskUsageForecast));
            Infragistics.Win.UltraWinEditors.DropDownEditorButton dropDownEditorButton2 = new Infragistics.Win.UltraWinEditors.DropDownEditorButton("DropDownList");
            Infragistics.Win.UltraWinEditors.DropDownEditorButton dropDownEditorButton1 = new Infragistics.Win.UltraWinEditors.DropDownEditorButton("DropDownList");
            this.forecastUnits = new System.Windows.Forms.NumericUpDown();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.forecastTypeCombo = new System.Windows.Forms.ComboBox();
            this.textBox2 = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
           // this.driveNamesCombo = new System.Windows.Forms.ComboBox();
            this.driveNamesCombo = new CheckedComboBox();
            this.chkShowAvailable = new System.Windows.Forms.CheckBox();
            this.label5 = new System.Windows.Forms.Label();
            this.startHoursTimeEditor = new Idera.SQLdm.Common.UI.Controls.TimeComboEditor();
            this.endHoursTimeEditor = new Idera.SQLdm.Common.UI.Controls.TimeComboEditor();
            this.startHoursLbl = new System.Windows.Forms.Label();
            this.endHoursLbl = new System.Windows.Forms.Label();
            this.showTablesCheckbox = new System.Windows.Forms.CheckBox();
            this.panel1.SuspendLayout();
            this.filterPanel.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.forecastUnits)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.startHoursTimeEditor)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.endHoursTimeEditor)).BeginInit();
            this.SuspendLayout();
            // 
            // panel1
            // 
            this.panel1.Size = new System.Drawing.Size(752, 149);
            // 
            // reportViewer
            // 
            this.reportViewer.Size = new System.Drawing.Size(752, 455);
            // 
            // filterPanel
            // 
            this.filterPanel.Controls.Add(this.showTablesCheckbox);
            this.filterPanel.Controls.Add(this.endHoursLbl);
            this.filterPanel.Controls.Add(this.startHoursLbl);
            this.filterPanel.Controls.Add(this.endHoursTimeEditor);
            this.filterPanel.Controls.Add(this.startHoursTimeEditor);
            this.filterPanel.Controls.Add(this.label5);
            this.filterPanel.Controls.Add(this.chkShowAvailable);
            this.filterPanel.Controls.Add(this.label1);
            this.filterPanel.Controls.Add(this.driveNamesCombo);
            this.filterPanel.Controls.Add(this.forecastUnits);
            this.filterPanel.Controls.Add(this.label4);
            this.filterPanel.Controls.Add(this.label2);
            this.filterPanel.Controls.Add(this.forecastTypeCombo);
            this.filterPanel.Size = new System.Drawing.Size(752, 149);
            this.filterPanel.Controls.SetChildIndex(this.showTablesCheckbox, 0);
            this.filterPanel.Controls.SetChildIndex(this.rangeInfoLabel, 0);
            this.filterPanel.Controls.SetChildIndex(this.tagsComboBox, 0);
            this.filterPanel.Controls.SetChildIndex(this.rangeLabel, 0);
            this.filterPanel.Controls.SetChildIndex(this.instanceCombo, 0);
            this.filterPanel.Controls.SetChildIndex(this.instanceLabel, 0);
            this.filterPanel.Controls.SetChildIndex(this.forecastTypeCombo, 0);
            this.filterPanel.Controls.SetChildIndex(this.label2, 0);
            this.filterPanel.Controls.SetChildIndex(this.label4, 0);
            this.filterPanel.Controls.SetChildIndex(this.forecastUnits, 0);
            this.filterPanel.Controls.SetChildIndex(this.periodLabel, 0);
            this.filterPanel.Controls.SetChildIndex(this.sampleLabel, 0);
           // this.filterPanel.Controls.SetChildIndex(this.driveNamesCombo, 0);
            this.filterPanel.Controls.SetChildIndex(this.sampleSizeCombo, 0);
            this.filterPanel.Controls.SetChildIndex(this.tagsLabel, 0);
            this.filterPanel.Controls.SetChildIndex(this.periodCombo, 0);
            this.filterPanel.Controls.SetChildIndex(this.label1, 0);
            this.filterPanel.Controls.SetChildIndex(this.chkShowAvailable, 0);
            this.filterPanel.Controls.SetChildIndex(this.label5, 0);
            this.filterPanel.Controls.SetChildIndex(this.startHoursTimeEditor, 0);
            this.filterPanel.Controls.SetChildIndex(this.endHoursTimeEditor, 0);
            this.filterPanel.Controls.SetChildIndex(this.startHoursLbl, 0);
            this.filterPanel.Controls.SetChildIndex(this.endHoursLbl, 0);
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
            this.periodCombo.Location = new System.Drawing.Point(61, 34);
            this.periodCombo.Size = new System.Drawing.Size(300, 21);
            this.periodCombo.TabIndex = 0;
            this.periodCombo.SelectedIndexChanged += new System.EventHandler(this.periodCombo_SelectedIndexChanged);
            // 
            // sampleSizeCombo
            // 
            this.sampleSizeCombo.Location = new System.Drawing.Point(61, 86);
            this.sampleSizeCombo.Size = new System.Drawing.Size(300, 21);
            this.sampleSizeCombo.TabIndex = 1;
            // 
            // instanceCombo
            // 
            this.instanceCombo.Location = new System.Drawing.Point(61, 7);
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
            this.reportInstructionsControl.Size = new System.Drawing.Size(752, 455);
            // 
            // rangeLabel
            // 
            this.rangeLabel.Location = new System.Drawing.Point(61, 60);
            this.rangeLabel.Size = new System.Drawing.Size(300, 21);
            // 
            // rangeInfoLabel
            // 
            this.rangeInfoLabel.Location = new System.Drawing.Point(10, 64);
            // 
            // forecastUnits
            // 
            this.forecastUnits.Location = new System.Drawing.Point(461, 61);
            this.forecastUnits.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.forecastUnits.Name = "forecastUnits";
            this.forecastUnits.Size = new System.Drawing.Size(40, 20);
            this.forecastUnits.TabIndex = 4;
            this.forecastUnits.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.forecastUnits.Value = new decimal(new int[] {
            10,
            0,
            0,
            0});
            this.forecastUnits.Leave += new System.EventHandler(this.forecastUnits_Leave);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.ForeColor = System.Drawing.SystemColors.ControlText;
            this.label1.Location = new System.Drawing.Point(377, 63);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(78, 13);
            this.label1.TabIndex = 20;
            this.label1.Text = "Forecast Units:";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.ForeColor = System.Drawing.SystemColors.ControlText;
            this.label2.Location = new System.Drawing.Point(377, 37);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(78, 13);
            this.label2.TabIndex = 21;
            this.label2.Text = "Forecast Type:";
            // 
            // forecastTypeCombo
            // 
            this.forecastTypeCombo.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.forecastTypeCombo.FormattingEnabled = true;
            this.forecastTypeCombo.Location = new System.Drawing.Point(461, 34);
            this.forecastTypeCombo.Name = "forecastTypeCombo";
            this.forecastTypeCombo.Size = new System.Drawing.Size(256, 21);
            this.forecastTypeCombo.TabIndex = 5;
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
            this.label4.Location = new System.Drawing.Point(377, 10);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(66, 13);
            this.label4.TabIndex = 28;
            this.label4.Text = "Drive Name:";
            // 
            // driveNamesCombo
            // 
            /*this.driveNamesCombo.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.driveNamesCombo.FormattingEnabled = true;
            this.driveNamesCombo.Location = new System.Drawing.Point(461, 7);
            this.driveNamesCombo.Name = "driveNamesCombo";
            this.driveNamesCombo.Size = new System.Drawing.Size(256, 21);
            this.driveNamesCombo.Sorted = true;
            this.driveNamesCombo.TabIndex = 3;*/

            /*this.driveNamesCombo.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.driveNamesCombo.CheckOnClick = true;
            this.driveNamesCombo.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawVariable;
            this.driveNamesCombo.DropDownHeight = 1;
            this.driveNamesCombo.FormattingEnabled = true;
            this.driveNamesCombo.IntegralHeight = false;
            this.driveNamesCombo.Location = new System.Drawing.Point(461, 7);
            this.driveNamesCombo.Name = "driveNamesCombo";
            this.driveNamesCombo.Size = new System.Drawing.Size(256, 21);
            this.driveNamesCombo.TabIndex = 3;
            this.driveNamesCombo.ValueSeparator = ", ";*/
           // this.driveNamesCombo.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.driveNamesCombo.CheckOnClick = true;
            this.driveNamesCombo.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawVariable;
            this.driveNamesCombo.DropDownHeight = 1;
            this.driveNamesCombo.FormattingEnabled = true;
            this.driveNamesCombo.IntegralHeight = false;
            this.driveNamesCombo.Location = new System.Drawing.Point(461,7);
            this.driveNamesCombo.Name = "ccb";
            this.driveNamesCombo.Size = new System.Drawing.Size(382, 21);
            this.driveNamesCombo.TabIndex = 0;
            this.driveNamesCombo.ValueSeparator = ",";
            this.driveNamesCombo.DropDownHeaderText = "All";
            // 
            // chkShowAvailable
            // 
            this.chkShowAvailable.AutoSize = true;
            this.chkShowAvailable.Location = new System.Drawing.Point(702, 63);
            this.chkShowAvailable.Name = "chkShowAvailable";
            this.chkShowAvailable.Size = new System.Drawing.Size(15, 14);
            this.chkShowAvailable.TabIndex = 30;
            this.chkShowAvailable.UseVisualStyleBackColor = true;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(559, 63);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(117, 13);
            this.label5.TabIndex = 31;
            this.label5.Text = "Show Available Space:";
            // 
            // startHoursTimeEditor
            // 
            dropDownEditorButton2.Key = "DropDownList";
            dropDownEditorButton2.PreferredDropDownSize = new System.Drawing.Size(0, 0);
            this.startHoursTimeEditor.ButtonsRight.Add(dropDownEditorButton2);
            this.startHoursTimeEditor.DropDownButtonDisplayStyle = Infragistics.Win.ButtonDisplayStyle.Never;
            this.startHoursTimeEditor.ListInterval = System.TimeSpan.Parse("01:00:00");
            this.startHoursTimeEditor.Location = new System.Drawing.Point(461, 85);
            this.startHoursTimeEditor.MaskInput = "{time}";
            this.startHoursTimeEditor.Name = "startHoursTimeEditor";
            this.startHoursTimeEditor.Size = new System.Drawing.Size(92, 21);
            this.startHoursTimeEditor.TabIndex = 32;
            this.startHoursTimeEditor.Time = System.TimeSpan.Parse("00:00:00");
            this.startHoursTimeEditor.ValueChanged += new System.EventHandler(this.startHoursTimeEditor_ValueChanged);
            // 
            // endHoursTimeEditor
            // 
            dropDownEditorButton1.Key = "DropDownList";
            dropDownEditorButton1.PreferredDropDownSize = new System.Drawing.Size(0, 0);
            this.endHoursTimeEditor.ButtonsRight.Add(dropDownEditorButton1);
            this.endHoursTimeEditor.DateTime = new System.DateTime(2012, 4, 30, 23, 0, 0, 0);
            this.endHoursTimeEditor.DropDownButtonDisplayStyle = Infragistics.Win.ButtonDisplayStyle.Never;
            this.endHoursTimeEditor.ListInterval = System.TimeSpan.Parse("01:00:00");
            this.endHoursTimeEditor.Location = new System.Drawing.Point(625, 85);
            this.endHoursTimeEditor.MaskInput = "{time}";
            this.endHoursTimeEditor.Name = "endHoursTimeEditor";
            this.endHoursTimeEditor.Size = new System.Drawing.Size(92, 21);
            this.endHoursTimeEditor.TabIndex = 33;
            this.endHoursTimeEditor.Time = System.TimeSpan.Parse("23:00:00");
            this.endHoursTimeEditor.Value = new System.DateTime(2012, 4, 30, 23, 0, 0, 0);
            this.endHoursTimeEditor.ValueChanged += new System.EventHandler(this.endHoursTimeEditor_ValueChanged);
            // 
            // startHoursLbl
            // 
            this.startHoursLbl.AutoSize = true;
            this.startHoursLbl.Location = new System.Drawing.Point(377, 89);
            this.startHoursLbl.Name = "startHoursLbl";
            this.startHoursLbl.Size = new System.Drawing.Size(63, 13);
            this.startHoursLbl.TabIndex = 34;
            this.startHoursLbl.Text = "Start Hours:";
            // 
            // endHoursLbl
            // 
            this.endHoursLbl.AutoSize = true;
            this.endHoursLbl.Location = new System.Drawing.Point(559, 89);
            this.endHoursLbl.Name = "endHoursLbl";
            this.endHoursLbl.Size = new System.Drawing.Size(60, 13);
            this.endHoursLbl.TabIndex = 35;
            this.endHoursLbl.Text = "End Hours:";
            // 
            // showTablesCheckbox
            // 
            this.showTablesCheckbox.AutoSize = true;
            this.showTablesCheckbox.Checked = true;
            this.showTablesCheckbox.CheckState = System.Windows.Forms.CheckState.Checked;
            this.showTablesCheckbox.Location = new System.Drawing.Point(461, 89);
            this.showTablesCheckbox.Name = "showTablesCheckbox";
            this.showTablesCheckbox.Size = new System.Drawing.Size(118, 17);
            this.showTablesCheckbox.TabIndex = 36;
            this.showTablesCheckbox.Text = "Show Tabular Data";
            this.showTablesCheckbox.UseVisualStyleBackColor = true;
            // 
            // DiskUsageForecast
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.Name = "DiskUsageForecast";
            this.panel1.ResumeLayout(false);
            this.filterPanel.ResumeLayout(false);
            this.filterPanel.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.forecastUnits)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.startHoursTimeEditor)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.endHoursTimeEditor)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.CheckBox showTablesCheckbox;
        private System.Windows.Forms.NumericUpDown forecastUnits;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.ComboBox forecastTypeCombo;
        private System.Windows.Forms.TextBox textBox2;
      //  private System.Windows.Forms.ComboBox driveNamesCombo;
        private CheckedComboBox driveNamesCombo;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.CheckBox chkShowAvailable;
        private System.Windows.Forms.Label endHoursLbl;
        private System.Windows.Forms.Label startHoursLbl;
        private Common.UI.Controls.TimeComboEditor endHoursTimeEditor;
        private Common.UI.Controls.TimeComboEditor startHoursTimeEditor;
       
    }
}
