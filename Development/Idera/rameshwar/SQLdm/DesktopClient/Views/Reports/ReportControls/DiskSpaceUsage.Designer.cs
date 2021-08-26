namespace Idera.SQLdm.DesktopClient.Views.Reports.ReportControls
{
    partial class DiskSpaceUsage
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
            //SQLDM-26805 - Vamshi Krishna - Code changes to exclude the disks in the disk space usage report
            this.diskTextbox = new System.Windows.Forms.TextBox();
            this.diskBrowseButton = new Infragistics.Win.Misc.UltraButton();
            this.label1 = new System.Windows.Forms.Label();

            this.cboOrderBy = new System.Windows.Forms.ComboBox();
            this.lblOrderBy = new System.Windows.Forms.Label();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(DiskSpaceUsage));
            this.panel1.SuspendLayout();
            this.filterPanel.SuspendLayout();
            this.SuspendLayout();
            // 
            // filterPanel
            //
            //SQLDM-26805 - Vamshi Krishna - Code changes to exclude the disks in the disk space usage report
            this.filterPanel.Controls.Add(this.diskTextbox);
            this.filterPanel.Controls.Add(this.diskBrowseButton);
            this.filterPanel.Controls.Add(this.label1);
            this.filterPanel.Controls.Add(this.lblOrderBy);
            this.filterPanel.Controls.Add(this.cboOrderBy);
            this.filterPanel.Controls.SetChildIndex(this.label1, 0);
            this.filterPanel.Controls.SetChildIndex(this.diskBrowseButton, 0);
            this.filterPanel.Controls.SetChildIndex(this.diskTextbox, 0);

            // 
            // panel1
            // 
            this.panel1.Size = new System.Drawing.Size(752, 125);
            // 
            // reportViewer
            // 
            this.reportViewer.Size = new System.Drawing.Size(752, 509);
            // 
            // filterPanel
            // 
            this.filterPanel.Size = new System.Drawing.Size(752, 125);
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
            // lblOrderBy
            // 
            this.lblOrderBy.Location = new System.Drawing.Point(10, 64);
            this.lblOrderBy.ForeColor = System.Drawing.SystemColors.ControlText;
            this.lblOrderBy.Text = "Sort:";
            this.lblOrderBy.AutoSize = true;
            this.lblOrderBy.Name = "lblOrderBy";
            this.lblOrderBy.Size = new System.Drawing.Size(41, 13);
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
            //SQLDM-26805 - Vamshi Krishna - Code changes to exclude the disks in the disk space usage report
            // 
            // diskBrowseButton
            // 
            this.diskBrowseButton.ShowOutline = false;
            this.diskBrowseButton.ShowFocusRect = false;
            this.diskBrowseButton.Location = new System.Drawing.Point(677, 7);
            this.diskBrowseButton.Name = "diskBrowseButton";
            this.diskBrowseButton.Size = new System.Drawing.Size(30, 20);
            this.diskBrowseButton.TabIndex = 19;
            this.diskBrowseButton.Text = "...";
            this.diskBrowseButton.Click += new System.EventHandler(this.diskBrowseButton_Click);
            // 
            // diskTextbox
            // 
            this.diskTextbox.BackColor = System.Drawing.SystemColors.Window;
            this.diskTextbox.TabStop = false;
            this.diskTextbox.Location = new System.Drawing.Point(460, 7);
            this.diskTextbox.Name = "diskTextbox";
            this.diskTextbox.ReadOnly = true;
            this.diskTextbox.Size = new System.Drawing.Size(214, 20);
            this.diskTextbox.TabIndex = 20;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.ForeColor = System.Drawing.SystemColors.ControlText;
            this.label1.Location = new System.Drawing.Point(404, 10);//SQLdm 9.1 (Ankit Srivastava) - Fixed Rally Defect DE44555 - shifted controls 6 pixels right
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(61, 13);
            this.label1.TabIndex = 21;
            this.label1.Text = "Disk:";// 
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
            // cboOrderBy
            // 
            this.cboOrderBy.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboOrderBy.Location = new System.Drawing.Point(57, 61);
            this.cboOrderBy.Size = new System.Drawing.Size(300, 21);
            this.cboOrderBy.Items.AddRange(new object[] {
            "Instance Name",
            "Disk Free Space Ascending",
            "Disk Free Space Descending"});
            this.cboOrderBy.FormattingEnabled = true;
            this.cboOrderBy.Name = "cboOrderBy";
            this.cboOrderBy.Sorted = false;
            // 
            // reportInstructionsControl
            // 
            this.reportInstructionsControl.ReportInstructions = resources.GetString("reportInstructionsControl.ReportInstructions1");
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
            this.Name = "DiskSpaceUsage";
            this.panel1.ResumeLayout(false);
            this.filterPanel.ResumeLayout(false);
            this.filterPanel.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion


        protected System.Windows.Forms.TextBox diskTextbox;
        protected Infragistics.Win.Misc.UltraButton diskBrowseButton;
        protected System.Windows.Forms.Label label1;
        System.Windows.Forms.ComboBox cboOrderBy;
        System.Windows.Forms.Label lblOrderBy;
    }
}
