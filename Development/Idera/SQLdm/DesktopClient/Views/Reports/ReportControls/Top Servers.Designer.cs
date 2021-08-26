using System.Windows.Forms;

namespace Idera.SQLdm.DesktopClient.Views.Reports.ReportControls
{
	partial class TopServersReport
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
			if(disposing && (components != null))
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(TopServersReport));
            Infragistics.Win.UltraWinEditors.DropDownEditorButton dropDownEditorButton2 = new Infragistics.Win.UltraWinEditors.DropDownEditorButton("DropDownList");
            Infragistics.Win.UltraWinEditors.DropDownEditorButton dropDownEditorButton1 = new Infragistics.Win.UltraWinEditors.DropDownEditorButton("DropDownList");
            this.label5 = new System.Windows.Forms.Label();
            this.numberOfServers = new System.Windows.Forms.NumericUpDown();
            this.waitThreshold = new System.Windows.Forms.NumericUpDown(); // This is to take wait threshold input. Aditya Shukla (SQLdm 8.6)
            this.startHoursTimeEditor = new Idera.SQLdm.Common.UI.Controls.TimeComboEditor();
            this.endHoursTimeEditor = new Idera.SQLdm.Common.UI.Controls.TimeComboEditor();
            this.startHoursLbl = new System.Windows.Forms.Label();
            this.endHoursLbl = new System.Windows.Forms.Label();
            this.waitThresholdLbl = new System.Windows.Forms.Label();// This is a label for Wait Threshold input control. Aditya Shukla (SQLdm 8.6)
            this.panel1.SuspendLayout();
            this.filterPanel.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numberOfServers)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.startHoursTimeEditor)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.endHoursTimeEditor)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.waitThreshold)).BeginInit();// Aditya Shukla (SQLdm 8.6)
            this.SuspendLayout();
            // 
            // panel1
            // 
            this.panel1.Size = new System.Drawing.Size(752, 148);
            // 
            // reportViewer
            // 
            this.reportViewer.Size = new System.Drawing.Size(752, 456);
            this.reportViewer.TabIndex = 0;
            // 
            // filterPanel
            // 
            this.filterPanel.Controls.Add(this.endHoursLbl);
            this.filterPanel.Controls.Add(this.startHoursLbl);
            this.filterPanel.Controls.Add(this.endHoursTimeEditor);
            this.filterPanel.Controls.Add(this.startHoursTimeEditor);
            this.filterPanel.Controls.Add(this.numberOfServers);
            this.filterPanel.Controls.Add(this.waitThreshold); // Putting the control on the panel. Aditya Shukla (SQLdm 8.6)
            this.filterPanel.Controls.Add(this.label5);
            this.filterPanel.Controls.Add(this.waitThresholdLbl);// Putting the control on the panel. Aditya Shukla (SQLdm 8.6)
            this.filterPanel.Size = new System.Drawing.Size(752, 148);
            this.filterPanel.Controls.SetChildIndex(this.rangeInfoLabel, 0);
            this.filterPanel.Controls.SetChildIndex(this.rangeLabel, 0);
            this.filterPanel.Controls.SetChildIndex(this.sampleLabel, 0);
            this.filterPanel.Controls.SetChildIndex(this.sampleSizeCombo, 0);
            this.filterPanel.Controls.SetChildIndex(this.instanceLabel, 0);
            this.filterPanel.Controls.SetChildIndex(this.instanceCombo, 0);
            this.filterPanel.Controls.SetChildIndex(this.periodLabel, 0);
            this.filterPanel.Controls.SetChildIndex(this.periodCombo, 0);
            this.filterPanel.Controls.SetChildIndex(this.tagsLabel, 0);
            this.filterPanel.Controls.SetChildIndex(this.tagsComboBox, 0);
            this.filterPanel.Controls.SetChildIndex(this.label5, 0);
            this.filterPanel.Controls.SetChildIndex(this.numberOfServers, 0);
            this.filterPanel.Controls.SetChildIndex(this.startHoursTimeEditor, 0);
            this.filterPanel.Controls.SetChildIndex(this.endHoursTimeEditor, 0);
            this.filterPanel.Controls.SetChildIndex(this.startHoursLbl, 0);
            this.filterPanel.Controls.SetChildIndex(this.endHoursLbl, 0);
            this.filterPanel.Controls.SetChildIndex(this.waitThresholdLbl, 0); // Aditya Shukla (SQLdm 8.6)
            this.filterPanel.Controls.SetChildIndex(this.waitThreshold, 0); // Aditya Shukla (SQLdm 8.6)
            // 
            // tagsLabel
            // 
            this.tagsLabel.Location = new System.Drawing.Point(10, 10);
            // 
            // periodLabel
            // 
            this.periodLabel.Location = new System.Drawing.Point(10, 37);
            // 
            // sampleLabel
            // 
            this.sampleLabel.Location = new System.Drawing.Point(442, 38);
            this.sampleLabel.Visible = false;
            // 
            // instanceLabel
            // 
            this.instanceLabel.Location = new System.Drawing.Point(447, 66);
            this.instanceLabel.Visible = false;
            // 
            // periodCombo
            // 
            this.periodCombo.Location = new System.Drawing.Point(114, 34);
            this.periodCombo.Size = new System.Drawing.Size(300, 21);
            this.periodCombo.TabIndex = 1;
            this.periodCombo.SelectedIndexChanged += new System.EventHandler(this.periodCombo_SelectedIndexChanged);
            // 
            // sampleSizeCombo
            // 
            this.sampleSizeCombo.Location = new System.Drawing.Point(494, 34);
            this.sampleSizeCombo.Size = new System.Drawing.Size(252, 21);
            this.sampleSizeCombo.Visible = false;
            // 
            // instanceCombo
            // 
            this.instanceCombo.Location = new System.Drawing.Point(494, 62);
            this.instanceCombo.Size = new System.Drawing.Size(252, 21);
            this.instanceCombo.Visible = false;
            // 
            // tagsComboBox
            // 
            this.tagsComboBox.Location = new System.Drawing.Point(114, 7);
            this.tagsComboBox.Size = new System.Drawing.Size(300, 21);
            this.tagsComboBox.TabIndex = 0;
            // 
            // reportInstructionsControl
            // 
            this.reportInstructionsControl.ReportInstructions = resources.GetString("reportInstructionsControl.ReportInstructions");
            this.reportInstructionsControl.Size = new System.Drawing.Size(752, 456);
            // 
            // rangeLabel
            // 
            this.rangeLabel.Location = new System.Drawing.Point(114, 61);
            this.rangeLabel.Size = new System.Drawing.Size(300, 21);
            // 
            // rangeInfoLabel
            // 
            this.rangeInfoLabel.Location = new System.Drawing.Point(10, 65);
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.ForeColor = System.Drawing.SystemColors.ControlText;
            this.label5.Location = new System.Drawing.Point(10, 89);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(98, 13);
            this.label5.TabIndex = 13;
            this.label5.Text = "Number of Servers:";
            // 
            // numberOfServers
            // 
            this.numberOfServers.Location = new System.Drawing.Point(114, 87);
            this.numberOfServers.Maximum = new decimal(new int[] {
            10000,
            0,
            0,
            0});
            this.numberOfServers.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.numberOfServers.Name = "numberOfServers";
            this.numberOfServers.Size = new System.Drawing.Size(58, 20);
            this.numberOfServers.TabIndex = 2;
            this.numberOfServers.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.numberOfServers.Value = new decimal(new int[] {
            5,
            0,
            0,
            0});
            this.numberOfServers.Leave += new System.EventHandler(this.numberOfServers_Leave);
            // 
            // waitThresholdLbl setting the design time properties for the label. Aditya Shukla (SQLdm 8.6) START
            // 
            this.waitThresholdLbl.AutoSize = true;
            this.waitThresholdLbl.ForeColor = System.Drawing.SystemColors.ControlText;
            this.waitThresholdLbl.Location = new System.Drawing.Point(200, 89);
            this.waitThresholdLbl.Name = "waitThresholdLbl";
            this.waitThresholdLbl.Size = new System.Drawing.Size(98, 13);
            this.waitThresholdLbl.TabIndex = 13;
            this.waitThresholdLbl.Text = "Wait Threshold(ms):";
            // waitThresholdLbl setting the design time properties for the label. Aditya Shukla (SQLdm 8.6) END
            // 
            // waitThreshold setting the design time properties for the numeric up down control. Aditya Shukla (SQLdm 8.6) START
            // 
            this.waitThreshold.Location = new System.Drawing.Point(304, 87);
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
            this.waitThreshold.Size = new System.Drawing.Size(58, 20);
            this.waitThreshold.TabIndex = 3;
            this.waitThreshold.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.waitThreshold.Value = new decimal(new int[] {
            0,
            0,
            0,
            0});
            this.waitThreshold.Leave += new System.EventHandler(this.waitThreshold_Leave); // This method checks the input validity
            // waitThreshold setting the design time properties for the numeric up down control. Aditya Shukla (SQLdm 8.6) END
            // 
            // startHoursTimeEditor
            // 
            dropDownEditorButton2.Key = "DropDownList";
            dropDownEditorButton2.PreferredDropDownSize = new System.Drawing.Size(0, 0);
            this.startHoursTimeEditor.ButtonsRight.Add(dropDownEditorButton2);
            this.startHoursTimeEditor.DropDownButtonDisplayStyle = Infragistics.Win.ButtonDisplayStyle.Never;
            this.startHoursTimeEditor.ListInterval = System.TimeSpan.Parse("01:00:00");
            this.startHoursTimeEditor.Location = new System.Drawing.Point(494, 6);
            this.startHoursTimeEditor.MaskInput = "{time}";
            this.startHoursTimeEditor.Name = "startHoursTimeEditor";
            this.startHoursTimeEditor.Size = new System.Drawing.Size(92, 21);
            this.startHoursTimeEditor.TabIndex = 30;
            this.startHoursTimeEditor.Time = System.TimeSpan.Parse("00:00:00");
            this.startHoursTimeEditor.ValueChanged += new System.EventHandler(this.startHoursTimeEditor_ValueChanged);
            // 
            // endHoursTimeEditor
            // 
            dropDownEditorButton1.Key = "DropDownList";
            dropDownEditorButton1.PreferredDropDownSize = new System.Drawing.Size(0, 0);
            this.endHoursTimeEditor.ButtonsRight.Add(dropDownEditorButton1);
            this.endHoursTimeEditor.DateTime = new System.DateTime(2012, 5, 4, 23, 0, 0, 0);
            this.endHoursTimeEditor.DropDownButtonDisplayStyle = Infragistics.Win.ButtonDisplayStyle.Never;
            this.endHoursTimeEditor.ListInterval = System.TimeSpan.Parse("01:00:00");
            this.endHoursTimeEditor.Location = new System.Drawing.Point(654, 6);
            this.endHoursTimeEditor.MaskInput = "{time}";
            this.endHoursTimeEditor.Name = "endHoursTimeEditor";
            this.endHoursTimeEditor.Size = new System.Drawing.Size(92, 21);
            this.endHoursTimeEditor.TabIndex = 31;
            this.endHoursTimeEditor.Time = System.TimeSpan.Parse("23:00:00");
            this.endHoursTimeEditor.Value = new System.DateTime(2012, 5, 4, 23, 0, 0, 0);
            this.endHoursTimeEditor.ValueChanged += new System.EventHandler(this.endHoursTimeEditor_ValueChanged);
            // 
            // startHoursLbl
            // 
            this.startHoursLbl.AutoSize = true;
            this.startHoursLbl.Location = new System.Drawing.Point(425, 10);
            this.startHoursLbl.Name = "startHoursLbl";
            this.startHoursLbl.Size = new System.Drawing.Size(63, 13);
            this.startHoursLbl.TabIndex = 32;
            this.startHoursLbl.Text = "Start Hours:";
            // 
            // endHoursLbl
            // 
            this.endHoursLbl.AutoSize = true;
            this.endHoursLbl.Location = new System.Drawing.Point(588, 10);
            this.endHoursLbl.Name = "endHoursLbl";
            this.endHoursLbl.Size = new System.Drawing.Size(60, 13);
            this.endHoursLbl.TabIndex = 33;
            this.endHoursLbl.Text = "End Hours:";
            // 
            // TopServersReport
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.Name = "TopServersReport";
            this.panel1.ResumeLayout(false);
            this.filterPanel.ResumeLayout(false);
            this.filterPanel.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.startHoursTimeEditor)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.endHoursTimeEditor)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.waitThreshold)).EndInit(); // Aditya Shukla (SQLdm 8.6)
            ((System.ComponentModel.ISupportInitialize)(this.numberOfServers)).EndInit();
            this.ResumeLayout(false);

		}

		#endregion

        private System.Windows.Forms.Label label5;
        private NumericUpDown numberOfServers;
        private Label endHoursLbl;
        private Label startHoursLbl;
        private Common.UI.Controls.TimeComboEditor endHoursTimeEditor;
        private Common.UI.Controls.TimeComboEditor startHoursTimeEditor;
        //Added a new filter for wait threshold - Aditya Shukla (SQLdm 8.6)
        private Label waitThresholdLbl;
        private NumericUpDown waitThreshold;
	}
}
