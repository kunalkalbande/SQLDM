namespace Idera.SQLdm.DesktopClient.Controls.Reporting {
    partial class TimeParameters {
        /// <summary> 
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary> 
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing) {
            if (disposing && (components != null)) {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent() {
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.periodBox = new System.Windows.Forms.ComboBox();
            this.years = new System.Windows.Forms.RadioButton();
            this.days = new System.Windows.Forms.RadioButton();
            this.months = new System.Windows.Forms.RadioButton();
            this.intervalBox = new System.Windows.Forms.ComboBox();
            this.labelInterval = new System.Windows.Forms.Label();
            this.groupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.periodBox);
            this.groupBox1.Controls.Add(this.years);
            this.groupBox1.Controls.Add(this.days);
            this.groupBox1.Controls.Add(this.months);
            this.groupBox1.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.groupBox1.Location = new System.Drawing.Point(0, 26);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(409, 55);
            this.groupBox1.TabIndex = 39;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Period ";
            // 
            // periodBox
            // 
            this.periodBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.periodBox.FormattingEnabled = true;
            this.periodBox.Location = new System.Drawing.Point(9, 28);
            this.periodBox.Name = "periodBox";
            this.periodBox.Size = new System.Drawing.Size(392, 21);
            this.periodBox.TabIndex = 44;
            this.periodBox.DropDown += new System.EventHandler(this.periodList_DropDown);
            // 
            // years
            // 
            this.years.AutoSize = true;
            this.years.Location = new System.Drawing.Point(130, 11);
            this.years.Name = "years";
            this.years.Size = new System.Drawing.Size(91, 17);
            this.years.TabIndex = 2;
            this.years.TabStop = true;
            this.years.Text = "Last 365 days";
            this.years.UseVisualStyleBackColor = true;
            this.years.CheckedChanged += new System.EventHandler(this.PeriodCheckedChanged);
            // 
            // days
            // 
            this.days.AutoSize = true;
            this.days.Location = new System.Drawing.Point(9, 11);
            this.days.Name = "days";
            this.days.Size = new System.Drawing.Size(49, 17);
            this.days.TabIndex = 0;
            this.days.TabStop = true;
            this.days.Text = "Days";
            this.days.UseVisualStyleBackColor = true;
            this.days.CheckedChanged += new System.EventHandler(this.PeriodCheckedChanged);
            // 
            // months
            // 
            this.months.AutoSize = true;
            this.months.Location = new System.Drawing.Point(64, 11);
            this.months.Name = "months";
            this.months.Size = new System.Drawing.Size(60, 17);
            this.months.TabIndex = 1;
            this.months.TabStop = true;
            this.months.Text = "Months";
            this.months.UseVisualStyleBackColor = true;
            this.months.CheckedChanged += new System.EventHandler(this.PeriodCheckedChanged);
            // 
            // intervalBox
            // 
            this.intervalBox.FormattingEnabled = true;
            this.intervalBox.Items.AddRange(new object[] {
            "Minutes",
            "Hours",
            "Days",
            "Months",
            "Year"});
            this.intervalBox.Location = new System.Drawing.Point(45, 0);
            this.intervalBox.Name = "intervalBox";
            this.intervalBox.Size = new System.Drawing.Size(86, 21);
            this.intervalBox.TabIndex = 38;
            this.intervalBox.SelectedIndexChanged += new System.EventHandler(this.intervalBox_SelectedIndexChanged);
            // 
            // labelInterval
            // 
            this.labelInterval.AutoSize = true;
            this.labelInterval.Location = new System.Drawing.Point(0, 3);
            this.labelInterval.Name = "labelInterval";
            this.labelInterval.Size = new System.Drawing.Size(45, 13);
            this.labelInterval.TabIndex = 37;
            this.labelInterval.Text = "Interval:";
            // 
            // TimeParameters
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.intervalBox);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.labelInterval);
            this.Name = "TimeParameters";
            this.Padding = new System.Windows.Forms.Padding(0, 0, 0, 6);
            this.Size = new System.Drawing.Size(409, 87);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.ComboBox periodBox;
        private System.Windows.Forms.RadioButton years;
        private System.Windows.Forms.RadioButton days;
        private System.Windows.Forms.RadioButton months;
        private System.Windows.Forms.ComboBox intervalBox;
        private System.Windows.Forms.Label labelInterval;
    }
}
