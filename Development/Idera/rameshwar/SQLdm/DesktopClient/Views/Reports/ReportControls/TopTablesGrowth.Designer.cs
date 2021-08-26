namespace Idera.SQLdm.DesktopClient.Views.Reports.ReportControls
{
    partial class TopTablesGrowth
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
                if (textSize    != null) textSize.Dispose();
                if (dataSize    != null) dataSize.Dispose();
                if (totalSize   != null) totalSize.Dispose();
                if (tableGrowth != null) tableGrowth.Dispose();
                if (indexSize   != null) indexSize.Dispose();
                if (rowGrowth   != null) rowGrowth.Dispose();
                if (numRows     != null) numRows.Dispose();

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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(TopTablesGrowth));
            this.sizeFilter = new System.Windows.Forms.NumericUpDown();
            this.chartType = new System.Windows.Forms.ComboBox();
            this.label2 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.sizeComparison = new System.Windows.Forms.ComboBox();
            this.RowComparison = new System.Windows.Forms.ComboBox();
            this.rowFilter = new System.Windows.Forms.NumericUpDown();
            this.label3 = new System.Windows.Forms.Label();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.label5 = new System.Windows.Forms.Label();
            this.tableCount = new System.Windows.Forms.NumericUpDown();
            this.panel1.SuspendLayout();
            this.filterPanel.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.sizeFilter)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.rowFilter)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.tableCount)).BeginInit();
            this.SuspendLayout();
            // 
            // databaseTextbox
            // 
            this.databaseTextbox.Location = new System.Drawing.Point(481, 28);
            this.databaseTextbox.TabIndex = 5;
            this.databaseTextbox.TabStop = false;
            // 
            // databaseBrowseButton
            // 
            this.databaseBrowseButton.Location = new System.Drawing.Point(688, 28);
            this.databaseBrowseButton.ShowFocusRect = false;
            this.databaseBrowseButton.ShowOutline = false;
            this.databaseBrowseButton.TabIndex = 5;
            // 
            // label1
            // 
            this.label1.ForeColor = System.Drawing.SystemColors.ControlText;
            this.label1.Location = new System.Drawing.Point(418, 31);
            // 
            // panel1
            // 
            this.panel1.Size = new System.Drawing.Size(752, 201);
            // 
            // reportViewer
            // 
            this.reportViewer.Size = new System.Drawing.Size(752, 403);
            // 
            // filterPanel
            // 
            this.filterPanel.Controls.Add(this.tableCount);
            this.filterPanel.Controls.Add(this.label5);
            this.filterPanel.Controls.Add(this.RowComparison);
            this.filterPanel.Controls.Add(this.rowFilter);
            this.filterPanel.Controls.Add(this.label3);
            this.filterPanel.Controls.Add(this.sizeComparison);
            this.filterPanel.Controls.Add(this.label4);
            this.filterPanel.Controls.Add(this.chartType);
            this.filterPanel.Controls.Add(this.sizeFilter);
            this.filterPanel.Controls.Add(this.label2);
            this.filterPanel.Controls.Add(this.groupBox1);
            this.filterPanel.Size = new System.Drawing.Size(752, 201);
            this.filterPanel.Controls.SetChildIndex(this.rangeInfoLabel, 0);
            this.filterPanel.Controls.SetChildIndex(this.rangeLabel, 0);
            this.filterPanel.Controls.SetChildIndex(this.groupBox1, 0);
            this.filterPanel.Controls.SetChildIndex(this.label2, 0);
            this.filterPanel.Controls.SetChildIndex(this.sizeFilter, 0);
            this.filterPanel.Controls.SetChildIndex(this.chartType, 0);
            this.filterPanel.Controls.SetChildIndex(this.label4, 0);
            this.filterPanel.Controls.SetChildIndex(this.sizeComparison, 0);
            this.filterPanel.Controls.SetChildIndex(this.label3, 0);
            this.filterPanel.Controls.SetChildIndex(this.rowFilter, 0);
            this.filterPanel.Controls.SetChildIndex(this.RowComparison, 0);
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
            this.filterPanel.Controls.SetChildIndex(this.label5, 0);
            this.filterPanel.Controls.SetChildIndex(this.tableCount, 0);
            // 
            // tagsLabel
            // 
            this.tagsLabel.Location = new System.Drawing.Point(440, 215);
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
            this.periodCombo.Location = new System.Drawing.Point(84, 34);
            this.periodCombo.Size = new System.Drawing.Size(300, 21);
            this.periodCombo.TabIndex = 1;
            // 
            // sampleSizeCombo
            // 
            this.sampleSizeCombo.Location = new System.Drawing.Point(84, 86);
            this.sampleSizeCombo.Size = new System.Drawing.Size(300, 21);
            this.sampleSizeCombo.TabIndex = 2;
            // 
            // instanceCombo
            // 
            this.instanceCombo.Location = new System.Drawing.Point(84, 7);
            this.instanceCombo.Size = new System.Drawing.Size(300, 21);
            this.instanceCombo.TabIndex = 0;
            // 
            // tagsComboBox
            // 
            this.tagsComboBox.Location = new System.Drawing.Point(481, 211);
            this.tagsComboBox.Visible = false;
            // 
            // reportInstructionsControl
            // 
            this.reportInstructionsControl.ReportInstructions = resources.GetString("reportInstructionsControl.ReportInstructions");
            this.reportInstructionsControl.Size = new System.Drawing.Size(752, 403);
            // 
            // rangeLabel
            // 
            this.rangeLabel.Location = new System.Drawing.Point(84, 60);
            this.rangeLabel.Size = new System.Drawing.Size(300, 21);
            // 
            // rangeInfoLabel
            // 
            this.rangeInfoLabel.Location = new System.Drawing.Point(10, 64);
            // 
            // sizeFilter
            // 
            this.sizeFilter.Location = new System.Drawing.Point(651, 55);
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
            this.chartType.Location = new System.Drawing.Point(84, 113);
            this.chartType.Name = "chartType";
            this.chartType.Size = new System.Drawing.Size(300, 21);
            this.chartType.TabIndex = 3;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(419, 59);
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
            this.sizeComparison.Location = new System.Drawing.Point(481, 55);
            this.sizeComparison.Name = "sizeComparison";
            this.sizeComparison.Size = new System.Drawing.Size(164, 21);
            this.sizeComparison.TabIndex = 6;
            // 
            // RowComparison
            // 
            this.RowComparison.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.RowComparison.FormattingEnabled = true;
            this.RowComparison.Items.AddRange(new object[] {
            "greater than or equal to...",
            "less than or equal to..."});
            this.RowComparison.Location = new System.Drawing.Point(481, 82);
            this.RowComparison.Name = "RowComparison";
            this.RowComparison.Size = new System.Drawing.Size(164, 21);
            this.RowComparison.TabIndex = 8;
            // 
            // rowFilter
            // 
            this.rowFilter.Location = new System.Drawing.Point(651, 82);
            this.rowFilter.Maximum = new decimal(new int[] {
            2147483647,
            0,
            0,
            0});
            this.rowFilter.Name = "rowFilter";
            this.rowFilter.Size = new System.Drawing.Size(67, 20);
            this.rowFilter.TabIndex = 9;
            this.rowFilter.Leave += new System.EventHandler(this.rowFilter_Leave);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(418, 85);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(59, 13);
            this.label3.TabIndex = 32;
            this.label3.Text = "Rowcount:";
            // 
            // groupBox1
            // 
            this.groupBox1.ForeColor = System.Drawing.SystemColors.ControlText;
            this.groupBox1.Location = new System.Drawing.Point(404, 7);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(325, 105);
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
            this.tableCount.Location = new System.Drawing.Point(84, 140);
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
            // TopTablesGrowth
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.Name = "TopTablesGrowth";
            this.panel1.ResumeLayout(false);
            this.filterPanel.ResumeLayout(false);
            this.filterPanel.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.sizeFilter)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.rowFilter)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.tableCount)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.ComboBox chartType;
        private System.Windows.Forms.NumericUpDown sizeFilter;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.ComboBox sizeComparison;
        private System.Windows.Forms.ComboBox RowComparison;
        private System.Windows.Forms.NumericUpDown rowFilter;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.NumericUpDown tableCount;
        private System.Windows.Forms.Label label5;
    }
}
