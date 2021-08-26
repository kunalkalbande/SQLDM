namespace Idera.SQLdm.DesktopClient.Views.Reports.ReportControls
{
    partial class MirroringOverview
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
            this.chkShowProblemsOnly = new System.Windows.Forms.CheckBox();
            this.panel1.SuspendLayout();
            this.panel2.SuspendLayout();
            this.filterPanel.SuspendLayout();
            this.SuspendLayout();
            // 
            // panel1
            // 
            this.panel1.Size = new System.Drawing.Size(791, 95);
            // 
            // panel2
            // 
            this.panel2.Location = new System.Drawing.Point(0, 95);
            this.panel2.Size = new System.Drawing.Size(791, 523);
            // 
            // toggleToolbarButton
            // 
            this.toggleToolbarButton.Location = new System.Drawing.Point(2, 59);
            // 
            // reportViewer
            // 
            this.reportViewer.Size = new System.Drawing.Size(791, 523);
            // 
            // filterPanel
            // 
            this.filterPanel.Controls.Add(this.chkShowProblemsOnly);
            this.filterPanel.Size = new System.Drawing.Size(791, 95);
            this.filterPanel.Controls.SetChildIndex(this.chkShowProblemsOnly, 0);
            this.filterPanel.Controls.SetChildIndex(this.tagsLabel, 0);
            this.filterPanel.Controls.SetChildIndex(this.runReportButton, 0);
            this.filterPanel.Controls.SetChildIndex(this.clearFilterButton, 0);
            this.filterPanel.Controls.SetChildIndex(this.toggleToolbarButton, 0);
            this.filterPanel.Controls.SetChildIndex(this.cancelReportButton, 0);
            this.filterPanel.Controls.SetChildIndex(this.periodLabel, 0);
            this.filterPanel.Controls.SetChildIndex(this.sampleLabel, 0);
            this.filterPanel.Controls.SetChildIndex(this.instanceLabel, 0);
            this.filterPanel.Controls.SetChildIndex(this.periodCombo, 0);
            this.filterPanel.Controls.SetChildIndex(this.sampleSizeCombo, 0);
            this.filterPanel.Controls.SetChildIndex(this.instanceCombo, 0);
            this.filterPanel.Controls.SetChildIndex(this.tagsComboBox, 0);
            // 
            // runReportButton
            // 
            this.runReportButton.Location = new System.Drawing.Point(506, 59);
            // 
            // clearFilterButton
            // 
            this.clearFilterButton.Location = new System.Drawing.Point(410, 59);
            // 
            // cancelReportButton
            // 
            this.cancelReportButton.Location = new System.Drawing.Point(314, 59);
            // 
            // chkShowProblemsOnly
            // 
            this.chkShowProblemsOnly.AutoSize = true;
            this.chkShowProblemsOnly.Location = new System.Drawing.Point(566, 11);
            this.chkShowProblemsOnly.Name = "chkShowProblemsOnly";
            this.chkShowProblemsOnly.Size = new System.Drawing.Size(123, 17);
            this.chkShowProblemsOnly.TabIndex = 25;
            this.chkShowProblemsOnly.Text = "Show Problems Only";
            this.chkShowProblemsOnly.UseVisualStyleBackColor = true;
            // 
            // MirroringCurrentStatus
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.Name = "MirroringOverview";
            this.panel1.ResumeLayout(false);
            this.panel2.ResumeLayout(false);
            this.filterPanel.ResumeLayout(false);
            this.filterPanel.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.CheckBox chkShowProblemsOnly;
    }
}
