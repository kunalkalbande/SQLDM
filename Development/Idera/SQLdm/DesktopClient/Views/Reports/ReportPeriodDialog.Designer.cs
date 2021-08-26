namespace Idera.SQLdm.DesktopClient.Views.Reports {
    partial class ReportPeriodDialog {
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

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent() {
            this.ultraMonthViewSingle1 = new Infragistics.Win.UltraWinSchedule.UltraMonthViewSingle();
            this.daysRadio = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomRadioButton();
            this.monthsRadio = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomRadioButton();
            this.label1 = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomLabel();
            this.monthsListBox = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomListBox();
            this.button1 = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomButton();
            this.button2 = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomButton();
            this.yearsRadio = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomRadioButton();
            this.yearsListBox = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomListBox();
            this.rangeRadio = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomRadioButton();
            this.rangePanel = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomPanel ();
            this.toPicker = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomDateTimePicker();
            this.toLabel = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomLabel();
            this.fromlabel = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomLabel();
            this.fromPicker = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomDateTimePicker();
            this.daysTipLabel = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomLabel();
            ((System.ComponentModel.ISupportInitialize)(this.ultraMonthViewSingle1)).BeginInit();
            this.rangePanel.SuspendLayout();
            this.SuspendLayout();
            // 
            // ultraMonthViewSingle1
            // 
            this.ultraMonthViewSingle1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.ultraMonthViewSingle1.AutoAppointmentDialog = false;
            this.ultraMonthViewSingle1.Location = new System.Drawing.Point(12, 48);
            this.ultraMonthViewSingle1.Name = "ultraMonthViewSingle1";
            this.ultraMonthViewSingle1.OwnerNavigationStyle = Infragistics.Win.UltraWinSchedule.OwnerNavigationStyle.None;
            this.ultraMonthViewSingle1.ShowClickToAddIndicator = Infragistics.Win.DefaultableBoolean.False;
            this.ultraMonthViewSingle1.ShowOwnerHeader = Infragistics.Win.DefaultableBoolean.False;
            this.ultraMonthViewSingle1.Size = new System.Drawing.Size(302, 271);
            this.ultraMonthViewSingle1.TabIndex = 1;
            this.ultraMonthViewSingle1.WeekHeaderDisplayStyle = Infragistics.Win.UltraWinSchedule.WeekHeaderDisplayStyle.DateRange;
            // 
            // daysRadio
            // 
            this.daysRadio.AutoSize = true;
            this.daysRadio.Checked = true;
            this.daysRadio.Location = new System.Drawing.Point(12, 25);
            this.daysRadio.Name = "daysRadio";
            this.daysRadio.Size = new System.Drawing.Size(49, 17);
            this.daysRadio.TabIndex = 2;
            this.daysRadio.TabStop = true;
            this.daysRadio.Text = "Days";
            this.daysRadio.UseVisualStyleBackColor = true;
            this.daysRadio.CheckedChanged += new System.EventHandler(this.radioButton1_CheckedChanged);
            // 
            // monthsRadio
            // 
            this.monthsRadio.AutoSize = true;
            this.monthsRadio.Location = new System.Drawing.Point(67, 25);
            this.monthsRadio.Name = "monthsRadio";
            this.monthsRadio.Size = new System.Drawing.Size(60, 17);
            this.monthsRadio.TabIndex = 3;
            this.monthsRadio.Text = "Months";
            this.monthsRadio.UseVisualStyleBackColor = true;
            this.monthsRadio.CheckedChanged += new System.EventHandler(this.radioButton1_CheckedChanged);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(9, 9);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(195, 13);
            this.label1.TabIndex = 5;
            this.label1.Text = "Select the days or months for the report.";
            // 
            // monthsListBox
            // 
            this.monthsListBox.FormattingEnabled = true;
            this.monthsListBox.Location = new System.Drawing.Point(12, 48);
            this.monthsListBox.Name = "monthsListBox";
            this.monthsListBox.SelectionMode = System.Windows.Forms.SelectionMode.MultiExtended;
            this.monthsListBox.Size = new System.Drawing.Size(143, 186);
            this.monthsListBox.TabIndex = 6;
            this.monthsListBox.SelectedIndexChanged += new System.EventHandler(this.AnyListBox_SelectedIndexChanged);
            // 
            // button1
            // 
            this.button1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.button1.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.button1.Location = new System.Drawing.Point(158, 347);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(75, 23);
            this.button1.TabIndex = 7;
            this.button1.Text = "OK";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // button2
            // 
            this.button2.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.button2.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.button2.Location = new System.Drawing.Point(239, 347);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(75, 23);
            this.button2.TabIndex = 8;
            this.button2.Text = "Cancel";
            this.button2.UseVisualStyleBackColor = true;
            // 
            // yearsRadio
            // 
            this.yearsRadio.AutoSize = true;
            this.yearsRadio.Location = new System.Drawing.Point(133, 25);
            this.yearsRadio.Name = "yearsRadio";
            this.yearsRadio.Size = new System.Drawing.Size(52, 17);
            this.yearsRadio.TabIndex = 9;
            this.yearsRadio.Text = "Years";
            this.yearsRadio.UseVisualStyleBackColor = true;
            this.yearsRadio.CheckedChanged += new System.EventHandler(this.radioButton1_CheckedChanged);
            // 
            // yearsListBox
            // 
            this.yearsListBox.FormattingEnabled = true;
            this.yearsListBox.Location = new System.Drawing.Point(12, 48);
            this.yearsListBox.Name = "yearsListBox";
            this.yearsListBox.SelectionMode = System.Windows.Forms.SelectionMode.MultiExtended;
            this.yearsListBox.Size = new System.Drawing.Size(143, 186);
            this.yearsListBox.TabIndex = 10;
            this.yearsListBox.SelectedIndexChanged += new System.EventHandler(this.AnyListBox_SelectedIndexChanged);
            // 
            // rangeRadio
            // 
            this.rangeRadio.AutoSize = true;
            this.rangeRadio.Location = new System.Drawing.Point(191, 25);
            this.rangeRadio.Name = "rangeRadio";
            this.rangeRadio.Size = new System.Drawing.Size(78, 17);
            this.rangeRadio.TabIndex = 11;
            this.rangeRadio.Text = "Date range";
            this.rangeRadio.UseVisualStyleBackColor = true;
            this.rangeRadio.Visible = false;
            this.rangeRadio.CheckedChanged += new System.EventHandler(this.radioButton1_CheckedChanged);
            // 
            // rangePanel
            // 
            this.rangePanel.Controls.Add(this.toPicker);
            this.rangePanel.Controls.Add(this.toLabel);
            this.rangePanel.Controls.Add(this.fromlabel);
            this.rangePanel.Controls.Add(this.fromPicker);
            this.rangePanel.Location = new System.Drawing.Point(12, 48);
            this.rangePanel.Name = "rangePanel";
            this.rangePanel.Size = new System.Drawing.Size(300, 225);
            this.rangePanel.TabIndex = 12;
            // 
            // toPicker
            // 
            this.toPicker.Location = new System.Drawing.Point(36, 27);
            this.toPicker.Name = "toPicker";
            this.toPicker.Size = new System.Drawing.Size(200, 20);
            this.toPicker.TabIndex = 3;
            // 
            // toLabel
            // 
            this.toLabel.AutoSize = true;
            this.toLabel.Location = new System.Drawing.Point(0, 31);
            this.toLabel.Name = "toLabel";
            this.toLabel.Size = new System.Drawing.Size(23, 13);
            this.toLabel.TabIndex = 2;
            this.toLabel.Text = "To:";
            // 
            // fromlabel
            // 
            this.fromlabel.AutoSize = true;
            this.fromlabel.Location = new System.Drawing.Point(0, 4);
            this.fromlabel.Name = "fromlabel";
            this.fromlabel.Size = new System.Drawing.Size(33, 13);
            this.fromlabel.TabIndex = 1;
            this.fromlabel.Text = "From:";
            // 
            // fromPicker
            // 
            this.fromPicker.Location = new System.Drawing.Point(36, 0);
            this.fromPicker.Name = "fromPicker";
            this.fromPicker.Size = new System.Drawing.Size(200, 20);
            this.fromPicker.TabIndex = 0;
            // 
            // daysTipLabel
            // 
            this.daysTipLabel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.daysTipLabel.AutoSize = true;
            this.daysTipLabel.Location = new System.Drawing.Point(12, 326);
            this.daysTipLabel.Name = "daysTipLabel";
            this.daysTipLabel.Size = new System.Drawing.Size(265, 13);
            this.daysTipLabel.TabIndex = 13;
            this.daysTipLabel.Text = "Press Shift or Ctrl when clicking to select multiple dates";
            // 
            // ReportPeriodDialog
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(326, 383);
            this.Controls.Add(this.daysTipLabel);
            this.Controls.Add(this.ultraMonthViewSingle1);
            this.Controls.Add(this.rangePanel);
            this.Controls.Add(this.rangeRadio);
            this.Controls.Add(this.yearsListBox);
            this.Controls.Add(this.yearsRadio);
            this.Controls.Add(this.button2);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.monthsListBox);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.monthsRadio);
            this.Controls.Add(this.daysRadio);
            this.MinimumSize = new System.Drawing.Size(332, 356);
            this.Name = "ReportPeriodDialog";
            this.Text = "Specify Custom Report Period";
            ((System.ComponentModel.ISupportInitialize)(this.ultraMonthViewSingle1)).EndInit();
            this.rangePanel.ResumeLayout(false);
            this.rangePanel.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private Infragistics.Win.UltraWinSchedule.UltraMonthViewSingle ultraMonthViewSingle1;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomRadioButton daysRadio;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomRadioButton monthsRadio;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomLabel label1;
        private System.Windows.Forms.ListBox monthsListBox;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomButton button1;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomButton button2;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomRadioButton yearsRadio;
        private System.Windows.Forms.ListBox yearsListBox;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomRadioButton rangeRadio;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomPanel  rangePanel;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomLabel fromlabel;
        private System.Windows.Forms.DateTimePicker fromPicker;
        private System.Windows.Forms.DateTimePicker toPicker;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomLabel toLabel;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomLabel daysTipLabel;

    }
}