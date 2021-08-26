namespace Idera.SQLdm.DesktopClient.Views.Reports.ReportControls
{
    partial class QueryStatistics
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
                if (storedProcedure != null) storedProcedure.Dispose();
                if (batch           != null) batch.Dispose();
                if (sqlStatement    != null) sqlStatement.Dispose();

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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(QueryStatistics));
            this.appName = new Idera.SQLdm.DesktopClient.Controls.AllowDeleteTextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.eventTypeCombo = new System.Windows.Forms.ComboBox();
            this.label3 = new System.Windows.Forms.Label();
            this.dbName = new Idera.SQLdm.DesktopClient.Controls.AllowDeleteTextBox();
            this.sigMode = new System.Windows.Forms.CheckBox();
            this.label4 = new System.Windows.Forms.Label();
            this.queryText = new Idera.SQLdm.DesktopClient.Controls.AllowDeleteTextBox();
            this.label5 = new System.Windows.Forms.Label();
            this.caseInsensitive = new System.Windows.Forms.CheckBox();
            this.panel1.SuspendLayout();
            this.filterPanel.SuspendLayout();
            this.SuspendLayout();
            // 
            // panel1
            // 
            this.panel1.Size = new System.Drawing.Size(807, 202);
            // 
            // reportViewer
            // 
            this.reportViewer.Size = new System.Drawing.Size(807, 402);
            this.reportViewer.ReportExport += new Microsoft.Reporting.WinForms.ExportEventHandler(reportViewer_ReportExport);
            // 
            // filterPanel
            // 
            this.filterPanel.Controls.Add(this.caseInsensitive);
            this.filterPanel.Controls.Add(this.label5);
            this.filterPanel.Controls.Add(this.label4);
            this.filterPanel.Controls.Add(this.queryText);
            this.filterPanel.Controls.Add(this.sigMode);
            this.filterPanel.Controls.Add(this.label3);
            this.filterPanel.Controls.Add(this.dbName);
            this.filterPanel.Controls.Add(this.eventTypeCombo);
            this.filterPanel.Controls.Add(this.label2);
            this.filterPanel.Controls.Add(this.label1);
            this.filterPanel.Controls.Add(this.appName);
            this.filterPanel.Size = new System.Drawing.Size(807, 202);
            this.filterPanel.Controls.SetChildIndex(this.rangeInfoLabel, 0);
            this.filterPanel.Controls.SetChildIndex(this.rangeLabel, 0);
            this.filterPanel.Controls.SetChildIndex(this.appName, 0);
            this.filterPanel.Controls.SetChildIndex(this.label1, 0);
            this.filterPanel.Controls.SetChildIndex(this.periodLabel, 0);
            this.filterPanel.Controls.SetChildIndex(this.periodCombo, 0);
            this.filterPanel.Controls.SetChildIndex(this.sampleSizeCombo, 0);
            this.filterPanel.Controls.SetChildIndex(this.sampleLabel, 0);
            this.filterPanel.Controls.SetChildIndex(this.instanceCombo, 0);
            this.filterPanel.Controls.SetChildIndex(this.instanceLabel, 0);
            this.filterPanel.Controls.SetChildIndex(this.tagsComboBox, 0);
            this.filterPanel.Controls.SetChildIndex(this.tagsLabel, 0);
            this.filterPanel.Controls.SetChildIndex(this.label2, 0);
            this.filterPanel.Controls.SetChildIndex(this.eventTypeCombo, 0);
            this.filterPanel.Controls.SetChildIndex(this.dbName, 0);
            this.filterPanel.Controls.SetChildIndex(this.label3, 0);
            this.filterPanel.Controls.SetChildIndex(this.sigMode, 0);
            this.filterPanel.Controls.SetChildIndex(this.queryText, 0);
            this.filterPanel.Controls.SetChildIndex(this.label4, 0);
            this.filterPanel.Controls.SetChildIndex(this.label5, 0);
            this.filterPanel.Controls.SetChildIndex(this.caseInsensitive, 0);
            // 
            // tagsLabel
            // 
            this.tagsLabel.Location = new System.Drawing.Point(460, 241);
            this.tagsLabel.Visible = false;
            // 
            // periodLabel
            // 
            this.periodLabel.Location = new System.Drawing.Point(10, 37);
            // 
            // sampleLabel
            // 
            this.sampleLabel.Location = new System.Drawing.Point(449, 214);
            this.sampleLabel.Visible = false;
            // 
            // instanceLabel
            // 
            this.instanceLabel.Location = new System.Drawing.Point(10, 10);
            // 
            // periodCombo
            // 
            this.periodCombo.Location = new System.Drawing.Point(79, 33);
            this.periodCombo.Size = new System.Drawing.Size(300, 21);
            this.periodCombo.TabIndex = 1;
            // 
            // sampleSizeCombo
            // 
            this.sampleSizeCombo.Location = new System.Drawing.Point(500, 211);
            this.sampleSizeCombo.Visible = false;
            // 
            // instanceCombo
            // 
            this.instanceCombo.Location = new System.Drawing.Point(78, 6);
            this.instanceCombo.Size = new System.Drawing.Size(300, 21);
            this.instanceCombo.TabIndex = 0;
            // 
            // tagsComboBox
            // 
            this.tagsComboBox.Location = new System.Drawing.Point(500, 238);
            this.tagsComboBox.Visible = false;
            // 
            // reportInstructionsControl
            //
            this.reportInstructionsControl.ReportInstructions = resources.GetString("reportInstructionsControl.ReportInstructions");
            this.reportInstructionsControl.Size = new System.Drawing.Size(807, 402);
            // 
            // rangeLabel
            // 
            this.rangeLabel.Location = new System.Drawing.Point(79, 59);
            this.rangeLabel.Size = new System.Drawing.Size(300, 21);
            // 
            // rangeInfoLabel
            // 
            this.rangeInfoLabel.Location = new System.Drawing.Point(10, 64);
            // 
            // appName
            // 
            this.appName.Location = new System.Drawing.Point(496, 7);
            this.appName.Name = "appName";
            this.appName.Size = new System.Drawing.Size(300, 20);
            this.appName.TabIndex = 2;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(396, 10);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(93, 13);
            this.label1.TabIndex = 26;
            this.label1.Text = "Application Name:";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(396, 37);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(65, 13);
            this.label2.TabIndex = 28;
            this.label2.Text = "Event Type:";
            // 
            // eventTypeCombo
            // 
            this.eventTypeCombo.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.eventTypeCombo.FormattingEnabled = true;
            this.eventTypeCombo.Location = new System.Drawing.Point(496, 33);
            this.eventTypeCombo.Name = "eventTypeCombo";
            this.eventTypeCombo.Size = new System.Drawing.Size(300, 21);
            this.eventTypeCombo.TabIndex = 3;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(396, 64);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(87, 13);
            this.label3.TabIndex = 31;
            this.label3.Text = "Database Name:";
            // 
            // dbName
            // 
            this.dbName.Location = new System.Drawing.Point(496, 60);
            this.dbName.Name = "dbName";
            this.dbName.Size = new System.Drawing.Size(300, 20);
            this.dbName.TabIndex = 4;
            // 
            // sigMode
            // 
            this.sigMode.AutoSize = true;
            this.sigMode.Checked = true;
            this.sigMode.CheckState = System.Windows.Forms.CheckState.Checked;
            this.sigMode.Location = new System.Drawing.Point(79, 147);
            this.sigMode.Name = "sigMode";
            this.sigMode.Size = new System.Drawing.Size(101, 17);
            this.sigMode.TabIndex = 6;
            this.sigMode.Text = "Signature Mode";
            this.sigMode.UseVisualStyleBackColor = true;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(10, 89);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(62, 13);
            this.label4.TabIndex = 47;
            this.label4.Text = "Query Text:";
            // 
            // queryText
            // 
            this.queryText.Location = new System.Drawing.Point(78, 86);
            this.queryText.Multiline = true;
            this.queryText.Name = "queryText";
            this.queryText.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.queryText.Size = new System.Drawing.Size(718, 55);
            this.queryText.TabIndex = 5;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Italic, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label5.Location = new System.Drawing.Point(547, 144);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(249, 13);
            this.label5.TabIndex = 48;
            this.label5.Text = "use % as wildcard in Application or Database Name";
            // 
            // caseInsensitive
            // 
            this.caseInsensitive.AutoSize = true;
            this.caseInsensitive.Checked = true;
            this.caseInsensitive.CheckState = System.Windows.Forms.CheckState.Checked;
            this.caseInsensitive.Location = new System.Drawing.Point(186, 147);
            this.caseInsensitive.Name = "caseInsensitive";
            this.caseInsensitive.Size = new System.Drawing.Size(103, 17);
            this.caseInsensitive.TabIndex = 42;
            this.caseInsensitive.Text = "Case Insensitive";
            this.caseInsensitive.UseVisualStyleBackColor = true;
            // 
            // QueryStatistics
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.Name = "QueryStatistics";
            this.Size = new System.Drawing.Size(807, 607);
            this.panel1.ResumeLayout(false);
            this.filterPanel.ResumeLayout(false);
            this.filterPanel.PerformLayout();
            this.ResumeLayout(false);

        }
        

        #endregion

        private System.Windows.Forms.Label label1;
        private Controls.AllowDeleteTextBox appName;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.ComboBox eventTypeCombo;
        private System.Windows.Forms.Label label3;
        private Controls.AllowDeleteTextBox dbName;
        private System.Windows.Forms.Label label4;
        private Controls.AllowDeleteTextBox queryText;
        private System.Windows.Forms.CheckBox sigMode;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.CheckBox caseInsensitive;
    }
}
