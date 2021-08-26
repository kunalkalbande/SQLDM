namespace Idera.SQLdm.DesktopClient.Views.Reports.ReportControls
{
    partial class AlwaysOnTopology
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
            this.runReportButton = new Infragistics.Win.Misc.UltraButton();
            this.availabilityGroupLabel = new System.Windows.Forms.Label();
            this.availabilityGroupCombo = new System.Windows.Forms.ComboBox();
            this.panel1.SuspendLayout();
            this.filterPanel.SuspendLayout();
            this.SuspendLayout();
            // 
            // panel1
            // 
            this.panel1.Size = new System.Drawing.Size(752, 68);
            // 
            // reportViewer
            // 
            this.reportViewer.Size = new System.Drawing.Size(752, 536);
            // 
            // filterPanel
            // 
            this.filterPanel.Controls.Add(this.availabilityGroupCombo);
            this.filterPanel.Controls.Add(this.availabilityGroupLabel);
            this.filterPanel.Size = new System.Drawing.Size(752, 68);
            this.filterPanel.Controls.SetChildIndex(this.availabilityGroupLabel, 0);
            this.filterPanel.Controls.SetChildIndex(this.availabilityGroupCombo, 0);
            this.filterPanel.Controls.SetChildIndex(this.rangeInfoLabel, 0);
            this.filterPanel.Controls.SetChildIndex(this.rangeLabel, 0);
            this.filterPanel.Controls.SetChildIndex(this.periodLabel, 0);
            this.filterPanel.Controls.SetChildIndex(this.sampleLabel, 0);
            this.filterPanel.Controls.SetChildIndex(this.sampleSizeCombo, 0);
            this.filterPanel.Controls.SetChildIndex(this.periodCombo, 0);
            this.filterPanel.Controls.SetChildIndex(this.tagsComboBox, 0);
            this.filterPanel.Controls.SetChildIndex(this.instanceCombo, 0);
            this.filterPanel.Controls.SetChildIndex(this.instanceLabel, 0);
            this.filterPanel.Controls.SetChildIndex(this.tagsLabel, 0);
            // 
            // tagsLabel
            // 
            this.tagsLabel.Location = new System.Drawing.Point(10, 500);
            this.tagsLabel.Visible = false;
            // 
            // periodLabel
            // 
            this.periodLabel.Location = new System.Drawing.Point(446, 510);
            this.periodLabel.Visible = false;
            // 
            // sampleLabel
            // 
            this.sampleLabel.Location = new System.Drawing.Point(442, 537);
            this.sampleLabel.Visible = false;
            // 
            // instanceLabel
            // 
            this.instanceLabel.Location = new System.Drawing.Point(10, 537);
            this.instanceLabel.Visible = false;
            // 
            // periodCombo
            // 
            this.periodCombo.Location = new System.Drawing.Point(493, 507);
            this.periodCombo.Visible = false;
            // 
            // sampleSizeCombo
            // 
            this.sampleSizeCombo.Location = new System.Drawing.Point(493, 534);
            this.sampleSizeCombo.Visible = false;
            // 
            // instanceCombo
            // 
            this.instanceCombo.Location = new System.Drawing.Point(57, 534);
            this.instanceCombo.Size = new System.Drawing.Size(300, 21);
            this.instanceCombo.TabIndex = 1;
            this.instanceCombo.Visible = false;
            // 
            // tagsComboBox
            // 
            this.tagsComboBox.Location = new System.Drawing.Point(57, 500);
            this.tagsComboBox.Size = new System.Drawing.Size(300, 21);
            this.tagsComboBox.TabIndex = 0;
            this.tagsComboBox.Visible = false;
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
            this.reportInstructionsControl.ReportInstructions = "1. Select an Availability Group on which to report; otherwise, the report include" +
                "s all data associated with the default value.\r\n2. Click Run Report.";
            this.reportInstructionsControl.Size = new System.Drawing.Size(752, 536);
            // 
            // rangeLabel
            // 
            this.rangeLabel.Location = new System.Drawing.Point(81, 562);
            this.rangeLabel.Visible = false;
            // 
            // rangeInfoLabel
            // 
            this.rangeInfoLabel.Location = new System.Drawing.Point(33, 566);
            this.rangeInfoLabel.Visible = false;
            // 
            // availabilityGroupLabel
            // 
            this.availabilityGroupLabel.AutoSize = true;
            this.availabilityGroupLabel.Location = new System.Drawing.Point(13, 10);
            this.availabilityGroupLabel.Name = "availabilityGroupLabel";
            this.availabilityGroupLabel.Size = new System.Drawing.Size(94, 13);
            this.availabilityGroupLabel.TabIndex = 30;
            this.availabilityGroupLabel.Text = "Availability Group: ";
            // 
            // availabilityGroupCombo
            // 
            this.availabilityGroupCombo.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.availabilityGroupCombo.FormattingEnabled = true;
            this.availabilityGroupCombo.Location = new System.Drawing.Point(114, 6);
            this.availabilityGroupCombo.Name = "availabilityGroupCombo";
            this.availabilityGroupCombo.Size = new System.Drawing.Size(300, 21);
            this.availabilityGroupCombo.Sorted = true;
            this.availabilityGroupCombo.TabIndex = 1;
            // 
            // AlwaysOnTopology
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.Name = "AlwaysOnTopology";
            this.panel1.ResumeLayout(false);
            this.filterPanel.ResumeLayout(false);
            this.filterPanel.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        protected System.Windows.Forms.ComboBox availabilityGroupCombo;
        private System.Windows.Forms.Label availabilityGroupLabel;
        private Infragistics.Win.Misc.UltraButton runReportButton;

    }
}
