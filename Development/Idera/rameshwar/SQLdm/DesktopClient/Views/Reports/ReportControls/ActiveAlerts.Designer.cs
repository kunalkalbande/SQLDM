namespace Idera.SQLdm.DesktopClient.Views.Reports.ReportControls
{
    partial class ActiveAlerts
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ActiveAlerts));
            this.panel1.SuspendLayout();
            this.filterPanel.SuspendLayout();
            this.SuspendLayout();
            // 
            // panel1
            // 
            this.panel1.Size = new System.Drawing.Size(752, 95);
            // 
            // reportViewer
            // 
            this.reportViewer.Size = new System.Drawing.Size(752, 509);
            // 
            // filterPanel
            // 
            this.filterPanel.Size = new System.Drawing.Size(752, 95);
            // 
            // tagsLabel
            // 
            this.tagsLabel.Location = new System.Drawing.Point(10, 10);
            // 
            // periodLabel
            // 
            this.periodLabel.Location = new System.Drawing.Point(405, 13);
            this.periodLabel.Visible = false;
            // 
            // sampleLabel
            // 
            this.sampleLabel.Location = new System.Drawing.Point(401, 37);
            this.sampleLabel.Visible = false;
            // 
            // instanceLabel
            // 
            this.instanceLabel.Location = new System.Drawing.Point(10, 37);
            // 
            // periodCombo
            // 
            this.periodCombo.Location = new System.Drawing.Point(452, 10);
            this.periodCombo.Visible = false;
            // 
            // sampleSizeCombo
            // 
            this.sampleSizeCombo.Location = new System.Drawing.Point(452, 34);
            this.sampleSizeCombo.Visible = false;
            // 
            // instanceCombo
            // 
            this.instanceCombo.Location = new System.Drawing.Point(57, 34);
            this.instanceCombo.Size = new System.Drawing.Size(300, 21);
            this.instanceCombo.TabIndex = 1;
            // 
            // tagsComboBox
            // 
            this.tagsComboBox.Location = new System.Drawing.Point(57, 7);
            this.tagsComboBox.Size = new System.Drawing.Size(300, 21);
            this.tagsComboBox.TabIndex = 0;
            // 
            // reportInstructionsControl
            // 
            this.reportInstructionsControl.ReportInstructions = resources.GetString("reportInstructionsControl.ReportInstructions");
            this.reportInstructionsControl.Size = new System.Drawing.Size(752, 509);
            // 
            // rangeLabel
            // 
            this.rangeLabel.Visible = false;
            // 
            // rangeInfoLabel
            // 
            this.rangeInfoLabel.Visible = false;
            // 
            // ActiveAlerts
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.Name = "ActiveAlerts";
            this.panel1.ResumeLayout(false);
            this.filterPanel.ResumeLayout(false);
            this.filterPanel.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion
    }
}
