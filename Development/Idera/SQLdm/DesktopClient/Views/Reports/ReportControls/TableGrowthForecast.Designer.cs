namespace Idera.SQLdm.DesktopClient.Views.Reports.ReportControls
{
    partial class TableGrowthForecast
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
                if (forecastParabola != null) forecastParabola.Dispose();
                if (forecastQuartic  != null) forecastQuartic.Dispose();
                if (forecastCubic    != null) forecastCubic.Dispose();
                if (forecastLinear   != null) forecastLinear.Dispose();

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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(TableGrowthForecast));
            this.label3 = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomLabel();
            this.tablesTextbox = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomTextBox();
            this.tableBrowseButton = new Infragistics.Win.Misc.UltraButton();
            this.forecastUnits = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomNumericUpDown();
            this.label4 = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomLabel();
            this.lblForecastType = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomLabel();
            this.forecastTypeCombo = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomComboBox();
            this.panel1.SuspendLayout();
            this.filterPanel.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.forecastUnits)).BeginInit();
            this.showTablesCheckbox = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomCheckBox();
            this.SuspendLayout();
            // 
            // databaseTextbox
            // 
            this.databaseTextbox.Location = new System.Drawing.Point(72, 34);
            this.databaseTextbox.Size = new System.Drawing.Size(264, 20);
            this.databaseTextbox.TabIndex = 3;
            this.databaseTextbox.TextChanged += new System.EventHandler(this.databaseTextbox_TextChanged);
            // 
            // databaseBrowseButton
            // 
            this.databaseBrowseButton.Location = new System.Drawing.Point(342, 34);
            this.databaseBrowseButton.ShowFocusRect = false;
            this.databaseBrowseButton.ShowOutline = false;
            this.databaseBrowseButton.TabIndex = 3;
            // 
            // label1
            // 
            this.label1.ForeColor = System.Drawing.SystemColors.ControlText;
            this.label1.Location = new System.Drawing.Point(10, 37);
            // 
            // panel1
            // 
            this.panel1.Size = new System.Drawing.Size(752, 204);
            // 
            // reportViewer
            // 
            this.reportViewer.Size = new System.Drawing.Size(752, 400);
            // 
            // filterPanel
            // 
            this.filterPanel.Controls.Add(this.forecastTypeCombo);
            this.filterPanel.Controls.Add(this.showTablesCheckbox);
            this.filterPanel.Controls.Add(this.lblForecastType);
            this.filterPanel.Controls.Add(this.forecastUnits);
            this.filterPanel.Controls.Add(this.label4);
            this.filterPanel.Controls.Add(this.label3);
            this.filterPanel.Controls.Add(this.tablesTextbox);
            this.filterPanel.Controls.Add(this.tableBrowseButton);
            this.filterPanel.Size = new System.Drawing.Size(752, 204);
            this.filterPanel.Controls.SetChildIndex(this.rangeInfoLabel, 0);
            this.filterPanel.Controls.SetChildIndex(this.rangeLabel, 0);
            this.filterPanel.Controls.SetChildIndex(this.tagsLabel, 0);
            this.filterPanel.Controls.SetChildIndex(this.tagsComboBox, 0);
            this.filterPanel.Controls.SetChildIndex(this.showTablesCheckbox, 0);
            this.filterPanel.Controls.SetChildIndex(this.tableBrowseButton, 0);
            this.filterPanel.Controls.SetChildIndex(this.tablesTextbox, 0);
            this.filterPanel.Controls.SetChildIndex(this.label3, 0);
            this.filterPanel.Controls.SetChildIndex(this.databaseBrowseButton, 0);
            this.filterPanel.Controls.SetChildIndex(this.label1, 0);
            this.filterPanel.Controls.SetChildIndex(this.databaseTextbox, 0);
            this.filterPanel.Controls.SetChildIndex(this.instanceLabel, 0);
            this.filterPanel.Controls.SetChildIndex(this.instanceCombo, 0);
            this.filterPanel.Controls.SetChildIndex(this.periodLabel, 0);
            this.filterPanel.Controls.SetChildIndex(this.periodCombo, 0);
            this.filterPanel.Controls.SetChildIndex(this.sampleLabel, 0);
            this.filterPanel.Controls.SetChildIndex(this.sampleSizeCombo, 0);
            this.filterPanel.Controls.SetChildIndex(this.label4, 0);
            this.filterPanel.Controls.SetChildIndex(this.forecastUnits, 0);
            this.filterPanel.Controls.SetChildIndex(this.lblForecastType, 0);
            this.filterPanel.Controls.SetChildIndex(this.forecastTypeCombo, 0);
            // 
            // tagsLabel
            // 
            this.tagsLabel.Location = new System.Drawing.Point(459, 251);
            this.tagsLabel.Visible = false;
            // 
            // periodLabel
            // 
            this.periodLabel.Location = new System.Drawing.Point(10, 89);
            // 
            // sampleLabel
            // 
            this.sampleLabel.Location = new System.Drawing.Point(10, 142);
            // 
            // instanceLabel
            // 
            this.instanceLabel.Location = new System.Drawing.Point(10, 10);
            // 
            // periodCombo
            // 
            this.periodCombo.Location = new System.Drawing.Point(72, 86);
            this.periodCombo.Size = new System.Drawing.Size(300, 21);
            this.periodCombo.TabIndex = 0;
            // 
            // sampleSizeCombo
            // 
            this.sampleSizeCombo.Location = new System.Drawing.Point(72, 139);
            this.sampleSizeCombo.Size = new System.Drawing.Size(300, 21);
            this.sampleSizeCombo.TabIndex = 1;
            // 
            // instanceCombo
            // 
            this.instanceCombo.Location = new System.Drawing.Point(72, 7);
            this.instanceCombo.Size = new System.Drawing.Size(300, 21);
            this.instanceCombo.TabIndex = 2;
            // 
            // tagsComboBox
            // 
            this.tagsComboBox.Location = new System.Drawing.Point(500, 247);
            this.tagsComboBox.Visible = false;
            // 
            // reportInstructionsControl
            // 
            this.reportInstructionsControl.ReportInstructions = resources.GetString("reportInstructionsControl.ReportInstructions");
            this.reportInstructionsControl.Size = new System.Drawing.Size(752, 400);
            // 
            // rangeLabel
            // 
            this.rangeLabel.Location = new System.Drawing.Point(72, 112);
            this.rangeLabel.Size = new System.Drawing.Size(300, 21);
            // 
            // rangeInfoLabel
            // 
            this.rangeInfoLabel.Location = new System.Drawing.Point(10, 116);

            // 
            // showTablesCheckbox
            // 
            this.showTablesCheckbox.AutoSize = true;
            this.showTablesCheckbox.Location = new System.Drawing.Point(393, 60);  // Change (61,144) to (393, 60).  KT 12/11/2017
            this.showTablesCheckbox.Name = "showTablesCheckbox";
            this.showTablesCheckbox.Size = new System.Drawing.Size(118, 17);
            this.showTablesCheckbox.TabIndex = 3;
            this.showTablesCheckbox.Text = "Show Tabular Data";
            this.showTablesCheckbox.UseVisualStyleBackColor = true;

            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.ForeColor = System.Drawing.SystemColors.ControlText;
            this.label3.Location = new System.Drawing.Point(10, 63);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(48, 13);
            this.label3.TabIndex = 22;
            this.label3.Text = "Table(s):";
            // 
            // tablesTextbox
            // 
            this.tablesTextbox.Location = new System.Drawing.Point(72, 60);
            this.tablesTextbox.Name = "tablesTextbox";
            this.tablesTextbox.ReadOnly = true;
            this.tablesTextbox.Size = new System.Drawing.Size(264, 20);
            this.tablesTextbox.TabIndex = 4;
            // 
            // tableBrowseButton
            // 
            this.tableBrowseButton.Location = new System.Drawing.Point(342, 60);
            this.tableBrowseButton.Name = "tableBrowseButton";
            this.tableBrowseButton.ShowFocusRect = false;
            this.tableBrowseButton.ShowOutline = false;
            this.tableBrowseButton.Size = new System.Drawing.Size(30, 20);
            this.tableBrowseButton.TabIndex = 4;
            this.tableBrowseButton.Text = "...";
            this.tableBrowseButton.Click += new System.EventHandler(this.tableBrowseButton_Click);
            // 
            // forecastUnits
            // 
            this.forecastUnits.Location = new System.Drawing.Point(477, 34);
            this.forecastUnits.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.forecastUnits.Name = "forecastUnits";
            this.forecastUnits.Size = new System.Drawing.Size(40, 20);
            this.forecastUnits.TabIndex = 5;
            this.forecastUnits.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.forecastUnits.Value = new decimal(new int[] {
            10,
            0,
            0,
            0});
            this.forecastUnits.Leave += new System.EventHandler(this.forecastUnits_Leave);
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.ForeColor = System.Drawing.SystemColors.ControlText;
            this.label4.Location = new System.Drawing.Point(393, 36);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(78, 13);
            this.label4.TabIndex = 26;
            this.label4.Text = "Forecast Units:";
            // 
            // lblForecastType
            // 
            this.lblForecastType.AutoSize = true;
            this.lblForecastType.ForeColor = System.Drawing.Color.Black;
            this.lblForecastType.Location = new System.Drawing.Point(393, 10);
            this.lblForecastType.Name = "lblForecastType";
            this.lblForecastType.Size = new System.Drawing.Size(78, 13);
            this.lblForecastType.TabIndex = 27;
            this.lblForecastType.Text = "Forecast Type:";
            // 
            // forecastTypeCombo
            // 
            this.forecastTypeCombo.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.forecastTypeCombo.FormattingEnabled = true;
            this.forecastTypeCombo.Location = new System.Drawing.Point(477, 7);
            this.forecastTypeCombo.Name = "forecastTypeCombo";
            this.forecastTypeCombo.Size = new System.Drawing.Size(200, 21);
            this.forecastTypeCombo.TabIndex = 6;
            // 
            // TableGrowthForecast
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.Name = "TableGrowthForecast";
            this.panel1.ResumeLayout(false);
            this.filterPanel.ResumeLayout(false);
            this.filterPanel.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.forecastUnits)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomLabel label3;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomTextBox tablesTextbox;
        private Infragistics.Win.Misc.UltraButton tableBrowseButton;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomNumericUpDown forecastUnits;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomLabel label4;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomLabel lblForecastType;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomComboBox forecastTypeCombo;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomCheckBox showTablesCheckbox;
    }
}
