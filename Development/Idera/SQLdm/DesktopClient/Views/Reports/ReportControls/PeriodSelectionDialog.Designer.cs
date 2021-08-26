namespace Idera.SQLdm.DesktopClient.Views.Reports.ReportControls
{
    partial class PeriodSelectionDialog
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
            this.components = new System.ComponentModel.Container();
            Infragistics.Win.UltraWinSchedule.CalendarCombo.DateButton dateButton1 = new Infragistics.Win.UltraWinSchedule.CalendarCombo.DateButton();
            Infragistics.Win.Appearance appearance2 = new Infragistics.Win.Appearance();
            Infragistics.Win.UltraWinSchedule.CalendarCombo.DateButton dateButton2 = new Infragistics.Win.UltraWinSchedule.CalendarCombo.DateButton();
            this.label1 = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomLabel();
            this.label2 = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomLabel();
            this.panel1 = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomPanel ();
            this.endRangePicker = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomUltraCalendarCombo();
            this.ultraCalendarLook = new Infragistics.Win.UltraWinSchedule.UltraCalendarLook(this.components);
            this.startRangePicker = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomUltraCalendarCombo();
            this.pictureBox2 = new System.Windows.Forms.PictureBox();
            this.label4 = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomLabel();
            this.label3 = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomLabel();
            this.okBtn = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomButton();
            this.cancelBtn = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomButton();
            this.panel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.endRangePicker)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.startRangePicker)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox2)).BeginInit();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(231)))), ((int)(((byte)(231)))), ((int)(((byte)(238)))));
            this.label1.Dock = System.Windows.Forms.DockStyle.Top;
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(75)))), ((int)(((byte)(75)))), ((int)(((byte)(75)))));
            this.label1.Location = new System.Drawing.Point(1, 1);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(301, 18);
            this.label1.TabIndex = 0;
            this.label1.Text = "Select Custom Range";
            this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // label2
            // 
            this.label2.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(144, 47);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(13, 13);
            this.label2.TabIndex = 5;
            this.label2.Text = "--";
            // 
            // panel1
            // 
            this.panel1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(250)))), ((int)(((byte)(250)))), ((int)(((byte)(250)))));
            this.panel1.Controls.Add(this.cancelBtn);
            this.panel1.Controls.Add(this.okBtn);
            this.panel1.Controls.Add(this.endRangePicker);
            this.panel1.Controls.Add(this.startRangePicker);
            this.panel1.Controls.Add(this.pictureBox2);
            this.panel1.Controls.Add(this.label4);
            this.panel1.Controls.Add(this.label2);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel1.Location = new System.Drawing.Point(1, 20);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(301, 99);
            this.panel1.TabIndex = 6;
            // 
            // endRangePicker
            // 
            this.endRangePicker.AllowNull = false;
            this.endRangePicker.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.endRangePicker.CalendarLook = this.ultraCalendarLook;
            this.endRangePicker.DateButtons.Add(dateButton1);
            this.endRangePicker.Location = new System.Drawing.Point(163, 44);
            this.endRangePicker.Name = "endRangePicker";
            this.endRangePicker.NonAutoSizeHeight = 21;
            this.endRangePicker.NullDateLabel = "";
            this.endRangePicker.Size = new System.Drawing.Size(88, 21);
            this.endRangePicker.TabIndex = 18;
            this.endRangePicker.Value = new System.DateTime(2007, 2, 20, 0, 0, 0, 0);
            this.endRangePicker.ValueChanged += new System.EventHandler(this.endRangePicker_ValueChanged);
            // 
            // ultraCalendarLook
            // 
            appearance2.BackColor = System.Drawing.Color.Transparent;
            this.ultraCalendarLook.MonthHeaderAppearance = appearance2;
            this.ultraCalendarLook.ViewStyle = Infragistics.Win.UltraWinSchedule.ViewStyle.Office2007;
            // 
            // startRangePicker
            // 
            this.startRangePicker.AllowNull = false;
            this.startRangePicker.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.startRangePicker.CalendarLook = this.ultraCalendarLook;
            this.startRangePicker.DateButtons.Add(dateButton2);
            this.startRangePicker.Location = new System.Drawing.Point(50, 44);
            this.startRangePicker.Name = "startRangePicker";
            this.startRangePicker.NonAutoSizeHeight = 21;
            this.startRangePicker.NullDateLabel = "";
            this.startRangePicker.Size = new System.Drawing.Size(88, 21);
            this.startRangePicker.TabIndex = 17;
            this.startRangePicker.Value = new System.DateTime(2007, 2, 20, 0, 0, 0, 0);
            this.startRangePicker.ValueChanged += new System.EventHandler(this.startRangePicker_ValueChanged);
            // 
            // pictureBox2
            // 
            this.pictureBox2.Image = global::Idera.SQLdm.DesktopClient.Properties.Resources.Information16x16;
            this.pictureBox2.Location = new System.Drawing.Point(8, 8);
            this.pictureBox2.Name = "pictureBox2";
            this.pictureBox2.Size = new System.Drawing.Size(16, 16);
            this.pictureBox2.SizeMode = System.Windows.Forms.PictureBoxSizeMode.AutoSize;
            this.pictureBox2.TabIndex = 16;
            this.pictureBox2.TabStop = false;
            // 
            // label4
            // 
            this.label4.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.label4.Location = new System.Drawing.Point(30, 8);
            this.label4.AutoEllipsis = true;
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(268, 28);
            this.label4.TabIndex = 6;
            this.label4.Text = "Note that the date range is limited to the range for which data was collected for" +
                " your monitored servers.";
            // 
            // label3
            // 
            this.label3.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(197)))), ((int)(((byte)(197)))), ((int)(((byte)(197)))));
            this.label3.Dock = System.Windows.Forms.DockStyle.Top;
            this.label3.Location = new System.Drawing.Point(1, 19);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(301, 1);
            this.label3.TabIndex = 7;
            // 
            // okBtn
            // 
            this.okBtn.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.okBtn.Location = new System.Drawing.Point(171, 76);
            this.okBtn.Name = "okBtn";
            this.okBtn.Size = new System.Drawing.Size(52, 20);
            this.okBtn.TabIndex = 19;
            this.okBtn.Text = "OK";
            this.okBtn.UseVisualStyleBackColor = true;
            this.okBtn.Click += new System.EventHandler(this.okBtn_Click);
            // 
            // cancelBtn
            // 
            this.cancelBtn.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.cancelBtn.Location = new System.Drawing.Point(245, 76);
            this.cancelBtn.Name = "cancelBtn";
            this.cancelBtn.Size = new System.Drawing.Size(52, 20);
            this.cancelBtn.TabIndex = 20;
            this.cancelBtn.Text = "Cancel";
            this.cancelBtn.UseVisualStyleBackColor = true;
            this.cancelBtn.Click += new System.EventHandler(this.cancelBtn_Click);
            // 
            // PeriodSelectionDialog
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(134)))), ((int)(((byte)(134)))), ((int)(((byte)(134)))));
            this.ClientSize = new System.Drawing.Size(303, 120);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Name = "PeriodSelectionDialog";
            this.Padding = new System.Windows.Forms.Padding(1);
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.Manual;
            this.Text = "PeriodSelectionDialog";
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.endRangePicker)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.startRangePicker)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox2)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomLabel label1;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomLabel label2;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomPanel  panel1;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomLabel label3;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomLabel label4;
        private System.Windows.Forms.PictureBox pictureBox2;
        private Infragistics.Win.UltraWinSchedule.UltraCalendarCombo startRangePicker;
        private Infragistics.Win.UltraWinSchedule.UltraCalendarCombo endRangePicker;
        private Infragistics.Win.UltraWinSchedule.UltraCalendarLook ultraCalendarLook;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomButton cancelBtn;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomButton okBtn;
    }
}
