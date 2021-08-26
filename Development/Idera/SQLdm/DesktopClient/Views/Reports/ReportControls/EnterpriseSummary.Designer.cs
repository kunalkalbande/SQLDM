namespace Idera.SQLdm.DesktopClient.Views.Reports.ReportControls
{
    partial class EnterpriseSummary
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
            this.filterPanel.Size = new System.Drawing.Size(752, 68);
            // 
            // tagsLabel
            // 
            this.tagsLabel.Location = new System.Drawing.Point(10, 10);
            // 
            // periodLabel
            // 
            this.periodLabel.Location = new System.Drawing.Point(276, 116);
            this.periodLabel.Visible = false;
            // 
            // sampleLabel
            // 
            this.sampleLabel.Location = new System.Drawing.Point(489, 116);
            this.sampleLabel.Visible = false;
            // 
            // instanceLabel
            // 
            this.instanceLabel.Location = new System.Drawing.Point(10, 116);
            this.instanceLabel.Visible = false;
            // 
            // periodCombo
            // 
            this.periodCombo.Location = new System.Drawing.Point(323, 113);
            this.periodCombo.Visible = false;
            // 
            // sampleSizeCombo
            // 
            this.sampleSizeCombo.Location = new System.Drawing.Point(540, 113);
            this.sampleSizeCombo.Visible = false;
            // 
            // instanceCombo
            // 
            this.instanceCombo.Location = new System.Drawing.Point(56, 113);
            this.instanceCombo.Visible = false;
            // 
            // tagsComboBox
            // 
            this.tagsComboBox.Location = new System.Drawing.Point(45, 7);
            this.tagsComboBox.Size = new System.Drawing.Size(300, 21);
            // 
            // reportInstructionsControl
            // 
            this.reportInstructionsControl.ReportInstructions = "1. Select a Tag to filter the SQL Servers to be returned in the report.\r\n2. Click" +
                " Run Report.";
            this.reportInstructionsControl.Size = new System.Drawing.Size(752, 536);
            // 
            // rangeLabel
            // 
            this.rangeLabel.Visible = false;
            // 
            // rangeInfoLabel
            // 
            this.rangeInfoLabel.Visible = false;
            // 
            // EnterpriseSummary
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.Name = "EnterpriseSummary";
            this.panel1.ResumeLayout(false);
            this.filterPanel.ResumeLayout(false);
            this.filterPanel.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion
    }
}
