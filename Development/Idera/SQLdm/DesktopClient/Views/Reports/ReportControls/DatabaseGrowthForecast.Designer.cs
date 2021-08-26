namespace Idera.SQLdm.DesktopClient.Views.Reports.ReportControls
{
    partial class DatabaseGrowthForecast
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(DatabaseGrowthForecast));
            Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomDropDownEditorButton dropDownEditorButton2 = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomDropDownEditorButton("DropDownList");
            Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomDropDownEditorButton dropDownEditorButton1 = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomDropDownEditorButton("DropDownList");
            this.forecastUnits = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomNumericUpDown();
            this.label3 = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomLabel();
            this.lblForecastType = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomLabel();
            this.forecastTypeCombo = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomComboBox();
            this.chkShowAvailableSpace = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomCheckBox();
            this.label4 = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomLabel();
            this.startHoursTimeEditor = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomTimeComboEditor();
            this.endHoursTimeEditor = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomTimeComboEditor();
            this.startHoursLbl = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomLabel();
            this.endHoursLbl = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomLabel();
            this.showTablesCheckbox = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomCheckBox();
            this.panel1.SuspendLayout();
            this.filterPanel.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.forecastUnits)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.startHoursTimeEditor)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.endHoursTimeEditor)).BeginInit();
            this.SuspendLayout();
            // 
            // databaseTextbox
            // 
            this.databaseTextbox.Location = new System.Drawing.Point(83, 34);
            this.databaseTextbox.Size = new System.Drawing.Size(264, 20);
            this.databaseTextbox.TabIndex = 7;
            this.databaseTextbox.TabStop = false;
            // 
            // databaseBrowseButton
            // 
            this.databaseBrowseButton.Location = new System.Drawing.Point(353, 34);
            this.databaseBrowseButton.ShowFocusRect = false;
            this.databaseBrowseButton.ShowOutline = false;
            this.databaseBrowseButton.TabIndex = 3;
            // 
            // label1
            // 
            this.label1.ForeColor = System.Drawing.SystemColors.ControlText;
            this.label1.Location = new System.Drawing.Point(10, 37);
            this.label1.Size = new System.Drawing.Size(67, 13);
            this.label1.Text = "Database(s):";
            // 
            // panel1
            // 
            this.panel1.Size = new System.Drawing.Size(752, 175);
            // 
            // reportViewer
            // 
            this.reportViewer.Size = new System.Drawing.Size(752, 429);
            // 
            // filterPanel
            // 
            this.filterPanel.Controls.Add(this.showTablesCheckbox);
            this.filterPanel.Controls.Add(this.endHoursLbl);
            this.filterPanel.Controls.Add(this.startHoursLbl);
            this.filterPanel.Controls.Add(this.endHoursTimeEditor);
            this.filterPanel.Controls.Add(this.startHoursTimeEditor);
            this.filterPanel.Controls.Add(this.label4);
            this.filterPanel.Controls.Add(this.chkShowAvailableSpace);
            this.filterPanel.Controls.Add(this.forecastTypeCombo);
            this.filterPanel.Controls.Add(this.lblForecastType);
            this.filterPanel.Controls.Add(this.forecastUnits);
            this.filterPanel.Controls.Add(this.label3);
            this.filterPanel.Size = new System.Drawing.Size(752, 175);
            this.filterPanel.TabIndex = 0;
            this.filterPanel.Controls.SetChildIndex(this.rangeInfoLabel, 0);
            this.filterPanel.Controls.SetChildIndex(this.rangeLabel, 0);
            this.filterPanel.Controls.SetChildIndex(this.tagsLabel, 0);
            this.filterPanel.Controls.SetChildIndex(this.tagsComboBox, 0);
            this.filterPanel.Controls.SetChildIndex(this.label1, 0);
            this.filterPanel.Controls.SetChildIndex(this.databaseBrowseButton, 0);
            this.filterPanel.Controls.SetChildIndex(this.databaseTextbox, 0);
            this.filterPanel.Controls.SetChildIndex(this.periodLabel, 0);
            this.filterPanel.Controls.SetChildIndex(this.periodCombo, 0);
            this.filterPanel.Controls.SetChildIndex(this.sampleLabel, 0);
            this.filterPanel.Controls.SetChildIndex(this.sampleSizeCombo, 0);
            this.filterPanel.Controls.SetChildIndex(this.instanceLabel, 0);
            this.filterPanel.Controls.SetChildIndex(this.instanceCombo, 0);
            this.filterPanel.Controls.SetChildIndex(this.label3, 0);
            this.filterPanel.Controls.SetChildIndex(this.forecastUnits, 0);
            this.filterPanel.Controls.SetChildIndex(this.lblForecastType, 0);
            this.filterPanel.Controls.SetChildIndex(this.forecastTypeCombo, 0);
            this.filterPanel.Controls.SetChildIndex(this.chkShowAvailableSpace, 0);
            this.filterPanel.Controls.SetChildIndex(this.label4, 0);
            this.filterPanel.Controls.SetChildIndex(this.startHoursTimeEditor, 0);
            this.filterPanel.Controls.SetChildIndex(this.endHoursTimeEditor, 0);
            this.filterPanel.Controls.SetChildIndex(this.startHoursLbl, 0);
            this.filterPanel.Controls.SetChildIndex(this.endHoursLbl, 0);
            this.filterPanel.Controls.SetChildIndex(this.showTablesCheckbox, 0);
            // 
            // tagsLabel
            // 
            this.tagsLabel.Location = new System.Drawing.Point(462, 200);
            this.tagsLabel.Visible = false;
            // 
            // periodLabel
            // 
            this.periodLabel.Location = new System.Drawing.Point(10, 63);
            // 
            // sampleLabel
            // 
            this.sampleLabel.Location = new System.Drawing.Point(10, 115);
            // 
            // instanceLabel
            // 
            this.instanceLabel.Location = new System.Drawing.Point(10, 10);
            // 
            // periodCombo
            // 
            this.periodCombo.Location = new System.Drawing.Point(83, 60);
            this.periodCombo.Size = new System.Drawing.Size(300, 21);
            this.periodCombo.TabIndex = 0;
            this.periodCombo.SelectedIndexChanged += new System.EventHandler(this.periodCombo_SelectedIndexChanged);
            // 
            // sampleSizeCombo
            // 
            this.sampleSizeCombo.Location = new System.Drawing.Point(83, 112);
            this.sampleSizeCombo.Size = new System.Drawing.Size(300, 21);
            this.sampleSizeCombo.TabIndex = 1;
            // 
            // instanceCombo
            // 
            this.instanceCombo.Location = new System.Drawing.Point(83, 7);
            this.instanceCombo.Size = new System.Drawing.Size(300, 21);
            this.instanceCombo.TabIndex = 2;
            // 
            // tagsComboBox
            // 
            this.tagsComboBox.Location = new System.Drawing.Point(503, 196);
            this.tagsComboBox.Visible = false;
            // 
            // reportInstructionsControl
            // 
            this.reportInstructionsControl.ReportInstructions = resources.GetString("reportInstructionsControl.ReportInstructions");
            this.reportInstructionsControl.Size = new System.Drawing.Size(752, 429);
            // 
            // rangeLabel
            // 
            this.rangeLabel.Location = new System.Drawing.Point(83, 86);
            this.rangeLabel.Size = new System.Drawing.Size(300, 21);
            // 
            // rangeInfoLabel
            // 
            this.rangeInfoLabel.Location = new System.Drawing.Point(10, 90);
            // 
            // forecastUnits
            // 
            this.forecastUnits.Location = new System.Drawing.Point(518, 34);
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
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.ForeColor = System.Drawing.SystemColors.ControlText;
            this.label3.Location = new System.Drawing.Point(400, 36);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(78, 13);
            this.label3.TabIndex = 23;
            this.label3.Text = "Forecast Units:";
            // 
            // showTablesCheckbox
            // 
            this.showTablesCheckbox.AutoSize = true;
            this.showTablesCheckbox.Checked = true;
            this.showTablesCheckbox.CheckState = System.Windows.Forms.CheckState.Checked;
            this.showTablesCheckbox.Location = new System.Drawing.Point(400, 114);
            this.showTablesCheckbox.Name = "showTablesCheckbox";
            this.showTablesCheckbox.Size = new System.Drawing.Size(118, 17);
            this.showTablesCheckbox.TabIndex = 3;
            this.showTablesCheckbox.Text = "Show Tabular Data";
            this.showTablesCheckbox.UseVisualStyleBackColor = true;
            // 
            // lblForecastType
            // 
            this.lblForecastType.AutoSize = true;
            this.lblForecastType.ForeColor = System.Drawing.SystemColors.ControlText;
            this.lblForecastType.Location = new System.Drawing.Point(400, 10);
            this.lblForecastType.Name = "lblForecastType";
            this.lblForecastType.Size = new System.Drawing.Size(78, 13);
            this.lblForecastType.TabIndex = 25;
            this.lblForecastType.Text = "Forecast Type:";
            // 
            // forecastTypeCombo
            // 
            this.forecastTypeCombo.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.forecastTypeCombo.FormattingEnabled = true;
            this.forecastTypeCombo.Location = new System.Drawing.Point(518, 7);
            this.forecastTypeCombo.Name = "forecastTypeCombo";
            this.forecastTypeCombo.Size = new System.Drawing.Size(200, 21);
            this.forecastTypeCombo.TabIndex = 5;
            // 
            // chkShowAvailableSpace
            // 
            this.chkShowAvailableSpace.AutoSize = true;
            this.chkShowAvailableSpace.Location = new System.Drawing.Point(520, 62);
            this.chkShowAvailableSpace.Name = "chkShowAvailableSpace";
            this.chkShowAvailableSpace.Size = new System.Drawing.Size(15, 14);
            this.chkShowAvailableSpace.TabIndex = 30;
            this.chkShowAvailableSpace.UseVisualStyleBackColor = true;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(400, 63);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(114, 13);
            this.label4.TabIndex = 31;
            this.label4.Text = "Show Available Space";
            // 
            // startHoursTimeEditor
            // 
            dropDownEditorButton2.Key = "DropDownList";
            dropDownEditorButton2.PreferredDropDownSize = new System.Drawing.Size(0, 0);
            this.startHoursTimeEditor.ButtonsRight.Add(dropDownEditorButton2);
            this.startHoursTimeEditor.DropDownButtonDisplayStyle = Infragistics.Win.ButtonDisplayStyle.Never;
            this.startHoursTimeEditor.ListInterval = System.TimeSpan.Parse("01:00:00");
            this.startHoursTimeEditor.Location = new System.Drawing.Point(518, 86);
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
            this.endHoursTimeEditor.Location = new System.Drawing.Point(518, 112);
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
            this.startHoursLbl.Location = new System.Drawing.Point(400, 90);
            this.startHoursLbl.Name = "startHoursLbl";
            this.startHoursLbl.Size = new System.Drawing.Size(63, 13);
            this.startHoursLbl.TabIndex = 34;
            this.startHoursLbl.Text = "Start Hours:";
            // 
            // endHoursLbl
            // 
            this.endHoursLbl.AutoSize = true;
            this.endHoursLbl.Location = new System.Drawing.Point(400, 115);
            this.endHoursLbl.Name = "endHoursLbl";
            this.endHoursLbl.Size = new System.Drawing.Size(60, 13);
            this.endHoursLbl.TabIndex = 35;
            this.endHoursLbl.Text = "End Hours:";
            // 
            // DatabaseGrowthForecast
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.Name = "DatabaseGrowthForecast";
            this.panel1.ResumeLayout(false);
            this.filterPanel.ResumeLayout(false);
            this.filterPanel.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.forecastUnits)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.startHoursTimeEditor)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.endHoursTimeEditor)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomCheckBox showTablesCheckbox;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomNumericUpDown forecastUnits;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomLabel label3;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomComboBox forecastTypeCombo;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomLabel lblForecastType;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomLabel label4;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomCheckBox chkShowAvailableSpace;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomLabel endHoursLbl;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomLabel startHoursLbl;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomTimeComboEditor endHoursTimeEditor;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomTimeComboEditor startHoursTimeEditor;
    }
}
