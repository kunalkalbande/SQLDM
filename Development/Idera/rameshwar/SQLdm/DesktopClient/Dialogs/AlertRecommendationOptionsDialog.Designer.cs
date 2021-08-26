namespace Idera.SQLdm.DesktopClient.Dialogs
{
    partial class AlertRecommendationOptionsDialog
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(AlertRecommendationOptionsDialog));
            this.okButton = new System.Windows.Forms.Button();
            this.cancelButton = new System.Windows.Forms.Button();
            this.restoreDefaultOptionsButton = new System.Windows.Forms.Button();
            this.office2007PropertyPage1 = new Idera.SQLdm.DesktopClient.Controls.Office2007PropertyPage();
            this.headerStrip1 = new Idera.SQLdm.DesktopClient.Controls.PropertiesHeaderStrip();
            this.informationBox1 = new Divelements.WizardFramework.InformationBox();
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.label5 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.warningPercentageOptionControl = new System.Windows.Forms.NumericUpDown();
            this.label2 = new System.Windows.Forms.Label();
            this.criticalPercentageOptionControl = new System.Windows.Forms.NumericUpDown();
            this.label3 = new System.Windows.Forms.Label();
            this.office2007PropertyPage1.ContentPanel.SuspendLayout();
            this.tableLayoutPanel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.warningPercentageOptionControl)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.criticalPercentageOptionControl)).BeginInit();
            this.SuspendLayout();
            // 
            // okButton
            // 
            this.okButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.okButton.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.okButton.Location = new System.Drawing.Point(333, 228);
            this.okButton.Name = "okButton";
            this.okButton.Size = new System.Drawing.Size(75, 23);
            this.okButton.TabIndex = 1;
            this.okButton.Text = "OK";
            this.okButton.UseVisualStyleBackColor = true;
            this.okButton.Click += new System.EventHandler(this.okButton_Click);
            // 
            // cancelButton
            // 
            this.cancelButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.cancelButton.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.cancelButton.Location = new System.Drawing.Point(414, 228);
            this.cancelButton.Name = "cancelButton";
            this.cancelButton.Size = new System.Drawing.Size(75, 23);
            this.cancelButton.TabIndex = 2;
            this.cancelButton.Text = "Cancel";
            this.cancelButton.UseVisualStyleBackColor = true;
            // 
            // restoreDefaultOptionsButton
            // 
            this.restoreDefaultOptionsButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.restoreDefaultOptionsButton.Location = new System.Drawing.Point(12, 228);
            this.restoreDefaultOptionsButton.Name = "restoreDefaultOptionsButton";
            this.restoreDefaultOptionsButton.Size = new System.Drawing.Size(98, 23);
            this.restoreDefaultOptionsButton.TabIndex = 3;
            this.restoreDefaultOptionsButton.Text = "Restore Defaults";
            this.restoreDefaultOptionsButton.UseVisualStyleBackColor = true;
            this.restoreDefaultOptionsButton.Click += new System.EventHandler(this.restoreDefaultOptionsButton_Click);
            // 
            // office2007PropertyPage1
            // 
            this.office2007PropertyPage1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.office2007PropertyPage1.BackColor = System.Drawing.Color.White;
            this.office2007PropertyPage1.BorderWidth = 1;
            // 
            // office2007PropertyPage1.ContentPanel
            // 
            this.office2007PropertyPage1.ContentPanel.BackColor2 = System.Drawing.Color.White;
            this.office2007PropertyPage1.ContentPanel.BorderColor = System.Drawing.SystemColors.ControlDark;
            this.office2007PropertyPage1.ContentPanel.Controls.Add(this.headerStrip1);
            this.office2007PropertyPage1.ContentPanel.Controls.Add(this.informationBox1);
            this.office2007PropertyPage1.ContentPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.office2007PropertyPage1.ContentPanel.FillStyle = Idera.SQLdm.DesktopClient.Controls.GradientPanelFillStyle.Solid;
            this.office2007PropertyPage1.ContentPanel.Location = new System.Drawing.Point(1, 56);
            this.office2007PropertyPage1.ContentPanel.Name = "ContentPanel";
            this.office2007PropertyPage1.ContentPanel.Padding = new System.Windows.Forms.Padding(1);
            this.office2007PropertyPage1.ContentPanel.ShowBorder = false;
            this.office2007PropertyPage1.ContentPanel.Size = new System.Drawing.Size(475, 153);
            this.office2007PropertyPage1.ContentPanel.TabIndex = 1;
            this.office2007PropertyPage1.HeaderImage = global::Idera.SQLdm.DesktopClient.Properties.Resources.ConfigureLogs32x32;
            this.office2007PropertyPage1.Location = new System.Drawing.Point(12, 12);
            this.office2007PropertyPage1.Name = "office2007PropertyPage1";
            this.office2007PropertyPage1.Size = new System.Drawing.Size(477, 210);
            this.office2007PropertyPage1.TabIndex = 0;
            this.office2007PropertyPage1.Text = "Configure alert recommendation options.";
            // 
            // headerStrip1
            // 
            this.headerStrip1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.headerStrip1.ForeColor = System.Drawing.Color.Black;
            this.headerStrip1.Location = new System.Drawing.Point(9, 3);
            this.headerStrip1.Name = "headerStrip1";
            this.headerStrip1.Size = new System.Drawing.Size(455, 25);
            this.headerStrip1.TabIndex = 3;
            this.headerStrip1.Text = "What values should be used when offering alert threshold recommendations?";
            // 
            // informationBox1
            // 
            this.informationBox1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.informationBox1.Location = new System.Drawing.Point(26, 34);
            this.informationBox1.Name = "informationBox1";
            this.informationBox1.Size = new System.Drawing.Size(423, 51);
            this.informationBox1.TabIndex = 2;
            this.informationBox1.Text = resources.GetString("informationBox1.Text");
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.BackColor = System.Drawing.Color.White;
            this.tableLayoutPanel1.ColumnCount = 3;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel1.Controls.Add(this.label5, 0, 1);
            this.tableLayoutPanel1.Controls.Add(this.label4, 0, 0);
            this.tableLayoutPanel1.Controls.Add(this.warningPercentageOptionControl, 1, 0);
            this.tableLayoutPanel1.Controls.Add(this.label2, 2, 0);
            this.tableLayoutPanel1.Controls.Add(this.criticalPercentageOptionControl, 1, 1);
            this.tableLayoutPanel1.Controls.Add(this.label3, 2, 1);
            this.tableLayoutPanel1.Location = new System.Drawing.Point(120, 154);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 2;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel1.Size = new System.Drawing.Size(305, 58);
            this.tableLayoutPanel1.TabIndex = 4;
            // 
            // label5
            // 
            this.label5.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(3, 35);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(26, 13);
            this.label5.TabIndex = 12;
            this.label5.Text = "Use";
            // 
            // label4
            // 
            this.label4.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(3, 6);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(26, 13);
            this.label4.TabIndex = 9;
            this.label4.Text = "Use";
            // 
            // warningPercentageOptionControl
            // 
            this.warningPercentageOptionControl.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.warningPercentageOptionControl.AutoSize = true;
            this.warningPercentageOptionControl.Location = new System.Drawing.Point(35, 3);
            this.warningPercentageOptionControl.Maximum = new decimal(new int[] {
            99,
            0,
            0,
            0});
            this.warningPercentageOptionControl.Name = "warningPercentageOptionControl";
            this.warningPercentageOptionControl.Size = new System.Drawing.Size(39, 20);
            this.warningPercentageOptionControl.TabIndex = 10;
            this.warningPercentageOptionControl.Value = new decimal(new int[] {
            20,
            0,
            0,
            0});
            // 
            // label2
            // 
            this.label2.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(80, 6);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(148, 13);
            this.label2.TabIndex = 11;
            this.label2.Text = "% above baseline for Warning";
            // 
            // criticalPercentageOptionControl
            // 
            this.criticalPercentageOptionControl.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.criticalPercentageOptionControl.AutoSize = true;
            this.criticalPercentageOptionControl.Location = new System.Drawing.Point(35, 32);
            this.criticalPercentageOptionControl.Maximum = new decimal(new int[] {
            99,
            0,
            0,
            0});
            this.criticalPercentageOptionControl.Name = "criticalPercentageOptionControl";
            this.criticalPercentageOptionControl.Size = new System.Drawing.Size(39, 20);
            this.criticalPercentageOptionControl.TabIndex = 13;
            this.criticalPercentageOptionControl.Value = new decimal(new int[] {
            30,
            0,
            0,
            0});
            // 
            // label3
            // 
            this.label3.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(80, 35);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(139, 13);
            this.label3.TabIndex = 14;
            this.label3.Text = "% above baseline for Critical";
            // 
            // AlertRecommendationOptionsDialog
            // 
            this.AcceptButton = this.okButton;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.cancelButton;
            this.ClientSize = new System.Drawing.Size(501, 263);
            this.Controls.Add(this.tableLayoutPanel1);
            this.Controls.Add(this.office2007PropertyPage1);
            this.Controls.Add(this.cancelButton);
            this.Controls.Add(this.restoreDefaultOptionsButton);
            this.Controls.Add(this.okButton);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.HelpButton = true;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "AlertRecommendationOptionsDialog";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Alert Recommendation Options";
            this.HelpButtonClicked += new System.ComponentModel.CancelEventHandler(this.AlertRecommendationOptionsDialog_HelpButtonClicked);
            this.HelpRequested += new System.Windows.Forms.HelpEventHandler(this.AlertRecommendationOptionsDialog_HelpRequested);
            this.office2007PropertyPage1.ContentPanel.ResumeLayout(false);
            this.tableLayoutPanel1.ResumeLayout(false);
            this.tableLayoutPanel1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.warningPercentageOptionControl)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.criticalPercentageOptionControl)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button okButton;
        private System.Windows.Forms.Button cancelButton;
        private Divelements.WizardFramework.InformationBox informationBox1;
        private System.Windows.Forms.Button restoreDefaultOptionsButton;
        private Idera.SQLdm.DesktopClient.Controls.Office2007PropertyPage office2007PropertyPage1;
        private Idera.SQLdm.DesktopClient.Controls.PropertiesHeaderStrip headerStrip1;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.NumericUpDown warningPercentageOptionControl;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.NumericUpDown criticalPercentageOptionControl;
        private System.Windows.Forms.Label label3;
    }
}
