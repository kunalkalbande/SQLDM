namespace Idera.SQLdm.DesktopClient.Views.Reports.ReportControls
{
    partial class DeadlockReport
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(DeadlockReport));
            this.panel1.SuspendLayout();
            this.filterPanel.SuspendLayout();
            this.SuspendLayout();
            // 
            // databaseTextbox
            // 
            this.databaseTextbox.BackColor = System.Drawing.SystemColors.Window;
            this.databaseTextbox.Location = new System.Drawing.Point(72, 34);
            this.databaseTextbox.Size = new System.Drawing.Size(267, 20);
            this.databaseTextbox.TabIndex = 1;
            this.databaseTextbox.TabStop = false;
            // 
            // databaseBrowseButton
            // 
            this.databaseBrowseButton.Location = new System.Drawing.Point(343, 34);
            this.databaseBrowseButton.ShowFocusRect = false;
            this.databaseBrowseButton.ShowOutline = false;
            this.databaseBrowseButton.TabIndex = 2;
            // 
            // label1
            // 
            this.label1.ForeColor = System.Drawing.SystemColors.ControlText;
            this.label1.Location = new System.Drawing.Point(11, 37);
            // 
            // panel1
            // 
            this.panel1.Size = new System.Drawing.Size(833, 176);
            // 
            // reportViewer
            // 
            this.reportViewer.Size = new System.Drawing.Size(833, 428);
            // 
            // filterPanel
            // 
            this.filterPanel.Size = new System.Drawing.Size(833, 176);
            this.filterPanel.Controls.SetChildIndex(this.rangeInfoLabel, 0);
            this.filterPanel.Controls.SetChildIndex(this.rangeLabel, 0);
            this.filterPanel.Controls.SetChildIndex(this.label1, 0);
            this.filterPanel.Controls.SetChildIndex(this.databaseBrowseButton, 0);
            this.filterPanel.Controls.SetChildIndex(this.databaseTextbox, 0);
            this.filterPanel.Controls.SetChildIndex(this.tagsLabel, 0);
            this.filterPanel.Controls.SetChildIndex(this.tagsComboBox, 0);
            this.filterPanel.Controls.SetChildIndex(this.periodLabel, 0);
            this.filterPanel.Controls.SetChildIndex(this.periodCombo, 0);
            this.filterPanel.Controls.SetChildIndex(this.sampleLabel, 0);
            this.filterPanel.Controls.SetChildIndex(this.sampleSizeCombo, 0);
            this.filterPanel.Controls.SetChildIndex(this.instanceLabel, 0);
            this.filterPanel.Controls.SetChildIndex(this.instanceCombo, 0);
            // 
            // tagsLabel
            // 
            this.tagsLabel.Location = new System.Drawing.Point(459, 198);
            this.tagsLabel.Visible = false;
            // 
            // periodLabel
            // 
            this.periodLabel.Location = new System.Drawing.Point(10, 63);

            // 
            // sampleLabel
            //
            this.sampleLabel.Visible = false;
            // 
            // instanceLabel
            // 
            this.instanceLabel.Location = new System.Drawing.Point(10, 10);
            // 
            // periodCombo
            // 
            this.periodCombo.Location = new System.Drawing.Point(72, 60);
            this.periodCombo.Size = new System.Drawing.Size(300, 21);
            this.periodCombo.TabIndex = 3;
            // 
            // sampleSizeCombo
            // 
            this.sampleSizeCombo.Visible = false;

            //
            // instanceCombo
            // 
            this.instanceCombo.Location = new System.Drawing.Point(72, 7);
            this.instanceCombo.Size = new System.Drawing.Size(300, 21);
            this.instanceCombo.TabIndex = 0;
            this.instanceCombo.SelectedIndexChanged += new System.EventHandler(this.instanceCombo_SelectedIndexChanged);
            // 
            // tagsComboBox
            // 
            this.tagsComboBox.Location = new System.Drawing.Point(500, 194);
            this.tagsComboBox.Visible = false;
            // 
            // reportInstructionsControl
            // 
            this.reportInstructionsControl.ReportInstructions = resources.GetString("reportInstructionsControl.ReportInstructions");
            this.reportInstructionsControl.Size = new System.Drawing.Size(833, 428);
            // 
            // rangeLabel
            // 
            this.rangeLabel.Location = new System.Drawing.Point(72, 87);
            this.rangeLabel.Size = new System.Drawing.Size(300, 21);
            // 
            // rangeInfoLabel
            // 
            this.rangeInfoLabel.Location = new System.Drawing.Point(10, 91);
            // 
            // DeadlockReport
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.Name = "DeadlockReport";
            this.Size = new System.Drawing.Size(833, 607);
            this.panel1.ResumeLayout(false);
            this.filterPanel.ResumeLayout(false);
            this.filterPanel.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

    }
}
