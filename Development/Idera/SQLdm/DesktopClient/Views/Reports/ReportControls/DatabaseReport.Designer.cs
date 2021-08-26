namespace Idera.SQLdm.DesktopClient.Views.Reports.ReportControls
{
    partial class DatabaseReport
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
            this.databaseBrowseButton = new Infragistics.Win.Misc.UltraButton();
            this.databaseTextbox = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.panel1.SuspendLayout();
            this.filterPanel.SuspendLayout();
            this.SuspendLayout();
            // 
            // panel1
            // 
            this.panel1.Size = new System.Drawing.Size(791, 101);
            // 
            // reportViewer
            // 
            this.reportViewer.Size = new System.Drawing.Size(791, 517);
            // 
            // filterPanel
            // 
            this.filterPanel.Controls.Add(this.databaseTextbox);
            this.filterPanel.Controls.Add(this.databaseBrowseButton);
            this.filterPanel.Controls.Add(this.label1);
            this.filterPanel.Size = new System.Drawing.Size(791, 101);
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
            this.tagsLabel.Location = new System.Drawing.Point(309, 63);
            // 
            // periodLabel
            // 
            this.periodLabel.Location = new System.Drawing.Point(36, 10);
            // 
            // sampleLabel
            // 
            this.sampleLabel.Location = new System.Drawing.Point(31, 37);
            // 
            // instanceLabel
            // 
            this.instanceLabel.Location = new System.Drawing.Point(315, 10);
            // 
            // periodCombo
            // 
            this.periodCombo.Location = new System.Drawing.Point(83, 6);
            // 
            // sampleSizeCombo
            // 
            this.sampleSizeCombo.Location = new System.Drawing.Point(83, 33);
            // 
            // instanceCombo
            // 
            this.instanceCombo.Location = new System.Drawing.Point(363, 8);
            this.instanceCombo.Size = new System.Drawing.Size(235, 21);
            this.instanceCombo.SelectedIndexChanged += new System.EventHandler(this.instanceCombo_SelectedIndexChanged);
            // 
            // tagsComboBox
            // 
            this.tagsComboBox.Location = new System.Drawing.Point(350, 59);
            // 
            // databaseBrowseButton
            // 
            this.databaseBrowseButton.Location = new System.Drawing.Point(570, 34);
            this.databaseBrowseButton.Name = "databaseBrowseButton";
            this.databaseBrowseButton.Size = new System.Drawing.Size(30, 20);
            this.databaseBrowseButton.TabIndex = 19;
            this.databaseBrowseButton.Text = "...";
            this.databaseBrowseButton.Click += new System.EventHandler(this.databaseBrowseButton_Click);
            // 
            // databaseTextbox
            // 
            this.databaseTextbox.Location = new System.Drawing.Point(363, 35);
            this.databaseTextbox.Name = "databaseTextbox";
            this.databaseTextbox.ReadOnly = true;
            this.databaseTextbox.Size = new System.Drawing.Size(202, 20);
            this.databaseTextbox.TabIndex = 20;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(21)))), ((int)(((byte)(66)))), ((int)(((byte)(139)))));
            this.label1.Location = new System.Drawing.Point(300, 38);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(56, 13);
            this.label1.TabIndex = 21;
            this.label1.Text = "Database:";
            // 
            // DatabaseReport
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.Name = "DatabaseReport";
            this.panel1.ResumeLayout(false);
            this.filterPanel.ResumeLayout(false);
            this.filterPanel.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        protected System.Windows.Forms.TextBox databaseTextbox;
        protected Infragistics.Win.Misc.UltraButton databaseBrowseButton;
        protected System.Windows.Forms.Label label1;


    }
}
