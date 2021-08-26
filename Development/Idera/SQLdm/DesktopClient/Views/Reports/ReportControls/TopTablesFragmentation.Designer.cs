namespace Idera.SQLdm.DesktopClient.Views.Reports.ReportControls
{
    partial class TopTablesFragmentation
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
                if (logicalFrag != null) logicalFrag.Dispose();
                if (tableSize   != null) tableSize.Dispose();

                if (components != null)
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(TopTablesFragmentation));
            this.sizeFilter = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomNumericUpDown();
            this.chartType = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomComboBox();
            this.label2 = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomLabel();
            this.label4 = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomLabel();
            this.sizeComparison = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomComboBox();
            this.scanDensityComparison = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomComboBox();
            this.scanDensityFilter = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomNumericUpDown();
            this.label3 = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomLabel();
            this.groupBox1 = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomGroupBox();
            this.label5 = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomLabel();
            this.tableCount = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomNumericUpDown();
            this.panel1.SuspendLayout();
            this.filterPanel.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.sizeFilter)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.scanDensityFilter)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.tableCount)).BeginInit();
            this.SuspendLayout();
            // 
            // databaseTextbox
            // 
            this.databaseTextbox.Location = new System.Drawing.Point(527, 27);
            // 
            // databaseBrowseButton
            // 
            this.databaseBrowseButton.Location = new System.Drawing.Point(734, 27);
            this.databaseBrowseButton.ShowFocusRect = false;
            this.databaseBrowseButton.ShowOutline = false;
            this.databaseBrowseButton.TabIndex = 5;
            // 
            // label1
            // 
            this.label1.ForeColor = System.Drawing.SystemColors.ControlText;
            this.label1.Location = new System.Drawing.Point(418, 30);
            // 
            // panel1
            // 
            this.panel1.Size = new System.Drawing.Size(786, 202);
            // 
            // reportViewer
            // 
            this.reportViewer.Size = new System.Drawing.Size(786, 402);
            // 
            // filterPanel
            // 
            this.filterPanel.Controls.Add(this.label3);
            this.filterPanel.Controls.Add(this.label2);
            this.filterPanel.Controls.Add(this.sizeComparison);
            this.filterPanel.Controls.Add(this.scanDensityFilter);
            this.filterPanel.Controls.Add(this.sizeFilter);
            this.filterPanel.Controls.Add(this.scanDensityComparison);
            this.filterPanel.Controls.Add(this.label4);
            this.filterPanel.Controls.Add(this.tableCount);
            this.filterPanel.Controls.Add(this.label5);
            this.filterPanel.Controls.Add(this.chartType);
            this.filterPanel.Controls.Add(this.groupBox1);
            this.filterPanel.Size = new System.Drawing.Size(786, 202);
            this.filterPanel.Controls.SetChildIndex(this.groupBox1, 0);
            this.filterPanel.Controls.SetChildIndex(this.chartType, 0);
            this.filterPanel.Controls.SetChildIndex(this.label5, 0);
            this.filterPanel.Controls.SetChildIndex(this.tableCount, 0);
            this.filterPanel.Controls.SetChildIndex(this.label4, 0);
            this.filterPanel.Controls.SetChildIndex(this.rangeInfoLabel, 0);
            this.filterPanel.Controls.SetChildIndex(this.rangeLabel, 0);
            this.filterPanel.Controls.SetChildIndex(this.sampleLabel, 0);
            this.filterPanel.Controls.SetChildIndex(this.tagsLabel, 0);
            this.filterPanel.Controls.SetChildIndex(this.periodLabel, 0);
            this.filterPanel.Controls.SetChildIndex(this.sampleSizeCombo, 0);
            this.filterPanel.Controls.SetChildIndex(this.tagsComboBox, 0);
            this.filterPanel.Controls.SetChildIndex(this.periodCombo, 0);
            this.filterPanel.Controls.SetChildIndex(this.instanceCombo, 0);
            this.filterPanel.Controls.SetChildIndex(this.instanceLabel, 0);
            this.filterPanel.Controls.SetChildIndex(this.scanDensityComparison, 0);
            this.filterPanel.Controls.SetChildIndex(this.sizeFilter, 0);
            this.filterPanel.Controls.SetChildIndex(this.scanDensityFilter, 0);
            this.filterPanel.Controls.SetChildIndex(this.sizeComparison, 0);
            this.filterPanel.Controls.SetChildIndex(this.label1, 0);
            this.filterPanel.Controls.SetChildIndex(this.databaseBrowseButton, 0);
            this.filterPanel.Controls.SetChildIndex(this.databaseTextbox, 0);
            this.filterPanel.Controls.SetChildIndex(this.label2, 0);
            this.filterPanel.Controls.SetChildIndex(this.label3, 0);
            // 
            // tagsLabel
            // 
            this.tagsLabel.Location = new System.Drawing.Point(422, 260);
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
            this.periodCombo.Location = new System.Drawing.Point(85, 34);
            this.periodCombo.Size = new System.Drawing.Size(300, 21);
            this.periodCombo.TabIndex = 1;
            // 
            // sampleSizeCombo
            // 
            this.sampleSizeCombo.Location = new System.Drawing.Point(85, 86);
            this.sampleSizeCombo.Size = new System.Drawing.Size(300, 21);
            this.sampleSizeCombo.TabIndex = 2;
            // 
            // instanceCombo
            // 
            this.instanceCombo.Location = new System.Drawing.Point(85, 7);
            this.instanceCombo.Size = new System.Drawing.Size(300, 21);
            this.instanceCombo.TabIndex = 0;
            // 
            // tagsComboBox
            // 
            this.tagsComboBox.Location = new System.Drawing.Point(463, 256);
            this.tagsComboBox.Visible = false;
            // 
            // reportInstructionsControl
            // 
            this.reportInstructionsControl.ReportInstructions = resources.GetString("reportInstructionsControl.ReportInstructions");
            this.reportInstructionsControl.Size = new System.Drawing.Size(786, 402);
            // 
            // rangeLabel
            // 
            this.rangeLabel.Location = new System.Drawing.Point(85, 60);
            this.rangeLabel.Size = new System.Drawing.Size(300, 21);
            // 
            // rangeInfoLabel
            // 
            this.rangeInfoLabel.Location = new System.Drawing.Point(10, 64);
            // 
            // sizeFilter
            // 
            this.sizeFilter.Location = new System.Drawing.Point(697, 53);
            this.sizeFilter.Maximum = new decimal(new int[] {
            2147483647,
            0,
            0,
            0});
            this.sizeFilter.Name = "sizeFilter";
            this.sizeFilter.Size = new System.Drawing.Size(67, 20);
            this.sizeFilter.TabIndex = 7;
            this.sizeFilter.Leave += new System.EventHandler(this.sizeFilter_Leave);
            // 
            // chartType
            // 
            this.chartType.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.chartType.FormattingEnabled = true;
            this.chartType.Location = new System.Drawing.Point(85, 113);
            this.chartType.Name = "chartType";
            this.chartType.Size = new System.Drawing.Size(300, 21);
            this.chartType.TabIndex = 3;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(418, 57);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(55, 13);
            this.label2.TabIndex = 26;
            this.label2.Text = "Size (MB):";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(10, 116);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(62, 13);
            this.label4.TabIndex = 30;
            this.label4.Text = "Chart Type:";
            // 
            // sizeComparison
            // 
            this.sizeComparison.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.sizeComparison.FormattingEnabled = true;
            this.sizeComparison.Items.AddRange(new object[] {
            "greater than or equal to...",
            "less than or equal to..."});
            this.sizeComparison.Location = new System.Drawing.Point(527, 53);
            this.sizeComparison.Name = "sizeComparison";
            this.sizeComparison.Size = new System.Drawing.Size(164, 21);
            this.sizeComparison.TabIndex = 6;
            // 
            // scanDensityComparison
            // 
            this.scanDensityComparison.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.scanDensityComparison.FormattingEnabled = true;
            this.scanDensityComparison.Items.AddRange(new object[] {
            "greater than or equal to...",
            "less than or equal to..."});
            this.scanDensityComparison.Location = new System.Drawing.Point(527, 80);
            this.scanDensityComparison.Name = "scanDensityComparison";
            this.scanDensityComparison.Size = new System.Drawing.Size(164, 21);
            this.scanDensityComparison.TabIndex = 8;
            // 
            // scanDensityFilter
            // 
            this.scanDensityFilter.Location = new System.Drawing.Point(697, 81);
            this.scanDensityFilter.Maximum = new decimal(new int[] {
            2147483647,
            0,
            0,
            0});
            this.scanDensityFilter.Name = "scanDensityFilter";
            this.scanDensityFilter.Size = new System.Drawing.Size(67, 20);
            this.scanDensityFilter.TabIndex = 9;
            this.scanDensityFilter.Leave += new System.EventHandler(this.scanDensityFilter_Leave);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(418, 83);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(107, 13);
            this.label3.TabIndex = 32;
            this.label3.Text = "Table Fragmentation:";
            // 
            // groupBox1
            // 
            this.groupBox1.ForeColor = System.Drawing.SystemColors.ControlText;
            this.groupBox1.Location = new System.Drawing.Point(404, 7);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(376, 105);
            this.groupBox1.TabIndex = 35;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Optional Filters";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(10, 142);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(68, 13);
            this.label5.TabIndex = 36;
            this.label5.Text = "Table Count:";
            // 
            // tableCount
            // 
            this.tableCount.Location = new System.Drawing.Point(85, 140);
            this.tableCount.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.tableCount.Name = "tableCount";
            this.tableCount.Size = new System.Drawing.Size(67, 20);
            this.tableCount.TabIndex = 4;
            this.tableCount.Value = new decimal(new int[] {
            5,
            0,
            0,
            0});
            this.tableCount.Leave += new System.EventHandler(this.tableCount_Leave);
            // 
            // TopTablesFragmentation
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.Name = "TopTablesFragmentation";
            this.Size = new System.Drawing.Size(786, 607);
            this.panel1.ResumeLayout(false);
            this.filterPanel.ResumeLayout(false);
            this.filterPanel.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.sizeFilter)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.scanDensityFilter)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.tableCount)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomComboBox chartType;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomNumericUpDown sizeFilter;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomLabel label2;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomLabel label4;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomComboBox sizeComparison;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomComboBox scanDensityComparison;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomNumericUpDown scanDensityFilter;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomLabel label3;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomGroupBox groupBox1;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomNumericUpDown tableCount;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomLabel label5;
    }
}
