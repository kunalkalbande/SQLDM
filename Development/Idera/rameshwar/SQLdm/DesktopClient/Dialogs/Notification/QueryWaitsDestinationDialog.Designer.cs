namespace Idera.SQLdm.DesktopClient.Dialogs.Notification
{
    //Query Waits Alert Action Response
    //10.1 Srishti Purohit SQLdm
    partial class QueryWaitsDestinationDialog
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
            this.okButton = new System.Windows.Forms.Button();
            this.cancelButton = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.durationInMunitesSpinner = new System.Windows.Forms.NumericUpDown();
            this.enableForeverButton = new System.Windows.Forms.RadioButton();
            this.enableLimitedButton = new System.Windows.Forms.RadioButton();
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.lblDurationRange = new System.Windows.Forms.Label();
            this.panel1 = new System.Windows.Forms.Panel();
            ((System.ComponentModel.ISupportInitialize)(this.durationInMunitesSpinner)).BeginInit();
            this.tableLayoutPanel1.SuspendLayout();
            this.panel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // okButton
            // 
            this.okButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.okButton.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.okButton.Location = new System.Drawing.Point(109, 8);
            this.okButton.Name = "okButton";
            this.okButton.Size = new System.Drawing.Size(75, 23);
            this.okButton.TabIndex = 0;
            this.okButton.Text = "OK";
            this.okButton.UseVisualStyleBackColor = true;
            this.okButton.Click += new System.EventHandler(this.okButton_Click);
            // 
            // cancelButton
            // 
            this.cancelButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.cancelButton.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.cancelButton.Location = new System.Drawing.Point(190, 8);
            this.cancelButton.Name = "cancelButton";
            this.cancelButton.Size = new System.Drawing.Size(75, 23);
            this.cancelButton.TabIndex = 1;
            this.cancelButton.Text = "Cancel";
            this.cancelButton.UseVisualStyleBackColor = true;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.label1.Enabled = false;
            this.label1.Location = new System.Drawing.Point(33, 58);
            this.label1.MinimumSize = new System.Drawing.Size(0, 26);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(142, 26);
            this.label1.TabIndex = 2;
            this.label1.Text = "Enable the query waits for ";
            this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // durationInMunitesSpinner
            // 
            this.durationInMunitesSpinner.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.durationInMunitesSpinner.Enabled = false;
            this.durationInMunitesSpinner.Location = new System.Drawing.Point(181, 61);
            this.durationInMunitesSpinner.Maximum = new decimal(new int[] {
            1439,
            0,
            0,
            0});
            this.durationInMunitesSpinner.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.durationInMunitesSpinner.Name = "durationInMunitesSpinner";
            this.durationInMunitesSpinner.Size = new System.Drawing.Size(48, 20);
            this.durationInMunitesSpinner.TabIndex = 3;
            this.durationInMunitesSpinner.Value = new decimal(new int[] {
            15,
            0,
            0,
            0});
            // 
            // enableForeverButton
            // 
            this.enableForeverButton.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.enableForeverButton.Checked = true;
            this.enableForeverButton.Location = new System.Drawing.Point(13, 13);
            this.enableForeverButton.Name = "enableForeverButton";
            this.enableForeverButton.Size = new System.Drawing.Size(14, 18);
            this.enableForeverButton.TabIndex = 4;
            this.enableForeverButton.TabStop = true;
            this.enableForeverButton.UseVisualStyleBackColor = true;
            // 
            // enableLimitedButton
            // 
            this.enableLimitedButton.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.enableLimitedButton.AutoSize = true;
            this.enableLimitedButton.Location = new System.Drawing.Point(13, 39);
            this.enableLimitedButton.Name = "enableLimitedButton";
            this.enableLimitedButton.Size = new System.Drawing.Size(14, 13);
            this.enableLimitedButton.TabIndex = 5;
            this.enableLimitedButton.UseVisualStyleBackColor = true;
            this.enableLimitedButton.CheckedChanged += new System.EventHandler(this.enableLimitedButton_CheckedChanged);
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.ColumnCount = 5;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 10F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel1.Controls.Add(this.panel1, 1, 5);
            this.tableLayoutPanel1.Controls.Add(this.label4, 4, 3);
            this.tableLayoutPanel1.Controls.Add(this.durationInMunitesSpinner, 3, 3);
            this.tableLayoutPanel1.Controls.Add(this.enableForeverButton, 1, 1);
            this.tableLayoutPanel1.Controls.Add(this.enableLimitedButton, 1, 2);
            this.tableLayoutPanel1.Controls.Add(this.label2, 2, 1);
            this.tableLayoutPanel1.Controls.Add(this.label1, 2, 3);
            this.tableLayoutPanel1.Controls.Add(this.label3, 2, 2);
            this.tableLayoutPanel1.Controls.Add(this.lblDurationRange, 2, 4);
            this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel1.Location = new System.Drawing.Point(0, 0);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 6;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 10F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel1.Size = new System.Drawing.Size(294, 177);
            this.tableLayoutPanel1.TabIndex = 6;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.tableLayoutPanel1.SetColumnSpan(this.label2, 3);
            this.label2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.label2.Location = new System.Drawing.Point(33, 10);
            this.label2.MinimumSize = new System.Drawing.Size(0, 24);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(248, 24);
            this.label2.TabIndex = 6;
            this.label2.Text = "Enable query waits";
            this.label2.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.tableLayoutPanel1.SetColumnSpan(this.label3, 3);
            this.label3.Dock = System.Windows.Forms.DockStyle.Fill;
            this.label3.Location = new System.Drawing.Point(33, 34);
            this.label3.MinimumSize = new System.Drawing.Size(0, 24);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(248, 24);
            this.label3.TabIndex = 7;
            this.label3.Text = "Enable query waits for a limited time";
            this.label3.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Dock = System.Windows.Forms.DockStyle.Fill;
            this.label4.Enabled = false;
            this.label4.Location = new System.Drawing.Point(235, 58);
            this.label4.MinimumSize = new System.Drawing.Size(0, 26);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(46, 26);
            this.label4.TabIndex = 7;
            this.label4.Text = "minutes.";
            this.label4.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // lblDurationRange
            // 
            this.tableLayoutPanel1.SetColumnSpan(this.lblDurationRange, 3);
            this.lblDurationRange.AutoSize = true;
            this.lblDurationRange.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lblDurationRange.Enabled = false;
            this.lblDurationRange.Location = new System.Drawing.Point(34, 87);
            this.lblDurationRange.MinimumSize = new System.Drawing.Size(0, 26);
            this.lblDurationRange.Name = "lblDurationRange";
            this.lblDurationRange.Size = new System.Drawing.Size(56, 26);
            this.lblDurationRange.TabIndex = 7;
            this.lblDurationRange.Text = "Duration can be minimum 1 and maximum 1439 mins.";
            this.lblDurationRange.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // panel1
            // 
            this.tableLayoutPanel1.SetColumnSpan(this.panel1, 4);
            this.panel1.Controls.Add(this.cancelButton);
            this.panel1.Controls.Add(this.okButton);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel1.Location = new System.Drawing.Point(13, 87);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(268, 42);
            this.panel1.TabIndex = 7;
            // 
            // QueryWaitsDestinationDialog
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(294, 177);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Controls.Add(this.tableLayoutPanel1);
            this.HelpButton = true;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "QueryWaitsDestinationDialog";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Query Waits Settings";
            this.Load += new System.EventHandler(this.QueryWaitsDestinationDialog_Load);
            ((System.ComponentModel.ISupportInitialize)(this.durationInMunitesSpinner)).EndInit();
            this.tableLayoutPanel1.ResumeLayout(false);
            this.tableLayoutPanel1.PerformLayout();
            this.panel1.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button okButton;
        private System.Windows.Forms.Button cancelButton;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.NumericUpDown durationInMunitesSpinner;
        private System.Windows.Forms.RadioButton enableForeverButton;
        private System.Windows.Forms.RadioButton enableLimitedButton;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label lblDurationRange;
        private System.Windows.Forms.Panel panel1;
    }
}