namespace Idera.SQLdm.DesktopClient.Controls
{
    partial class AlertsTimeFilter
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

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            Infragistics.Win.UltraWinEditors.DropDownEditorButton dropDownEditorButton5 = new Infragistics.Win.UltraWinEditors.DropDownEditorButton("DropDownList");
            Infragistics.Win.UltraWinSchedule.CalendarCombo.DateButton dateButton5 = new Infragistics.Win.UltraWinSchedule.CalendarCombo.DateButton();
            Infragistics.Win.UltraWinEditors.DropDownEditorButton dropDownEditorButton6 = new Infragistics.Win.UltraWinEditors.DropDownEditorButton("DropDownList");
            Infragistics.Win.UltraWinSchedule.CalendarCombo.DateButton dateButton6 = new Infragistics.Win.UltraWinSchedule.CalendarCombo.DateButton();
            this._lbl_Days = new System.Windows.Forms.Label();
            this._lastDaysInput = new System.Windows.Forms.NumericUpDown();
            this._lbl_Last = new System.Windows.Forms.Label();
            this._lbl_Begin = new System.Windows.Forms.Label();
            this._lbl_End = new System.Windows.Forms.Label();
            this._beginTimeCombo = new Idera.SQLdm.Common.UI.Controls.TimeComboEditor();
            this._endDateCombo = new Infragistics.Win.UltraWinSchedule.UltraCalendarCombo();
            this._endTimeCombo = new Idera.SQLdm.Common.UI.Controls.TimeComboEditor();
            this._beginDateCombo = new Infragistics.Win.UltraWinSchedule.UltraCalendarCombo();
            this._pnl_LastN = new System.Windows.Forms.Panel();
            this._pnl_TimeSpan = new System.Windows.Forms.Panel();
            this._lnklbl_Specification = new System.Windows.Forms.LinkLabel();
            ((System.ComponentModel.ISupportInitialize)(this._lastDaysInput)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this._beginTimeCombo)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this._endDateCombo)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this._endTimeCombo)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this._beginDateCombo)).BeginInit();
            this._pnl_LastN.SuspendLayout();
            this._pnl_TimeSpan.SuspendLayout();
            this.SuspendLayout();
            // 
            // _lbl_Days
            // 
            this._lbl_Days.Location = new System.Drawing.Point(99, 5);
            this._lbl_Days.Name = "_lbl_Days";
            this._lbl_Days.Size = new System.Drawing.Size(32, 18);
            this._lbl_Days.TabIndex = 31;
            this._lbl_Days.Text = "days";
            // 
            // _lastDaysInput
            // 
            this._lastDaysInput.Location = new System.Drawing.Point(48, 3);
            this._lastDaysInput.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this._lastDaysInput.Name = "_lastDaysInput";
            this._lastDaysInput.Size = new System.Drawing.Size(45, 20);
            this._lastDaysInput.TabIndex = 30;
            this._lastDaysInput.Value = new decimal(new int[] {
            7,
            0,
            0,
            0});
            // 
            // _lbl_Last
            // 
            this._lbl_Last.AutoSize = true;
            this._lbl_Last.Location = new System.Drawing.Point(15, 5);
            this._lbl_Last.Name = "_lbl_Last";
            this._lbl_Last.Size = new System.Drawing.Size(27, 13);
            this._lbl_Last.TabIndex = 29;
            this._lbl_Last.Text = "Last";
            // 
            // _lbl_Begin
            // 
            this._lbl_Begin.AutoSize = true;
            this._lbl_Begin.Location = new System.Drawing.Point(15, 6);
            this._lbl_Begin.Name = "_lbl_Begin";
            this._lbl_Begin.Size = new System.Drawing.Size(37, 13);
            this._lbl_Begin.TabIndex = 32;
            this._lbl_Begin.Text = "Begin:";
            // 
            // _lbl_End
            // 
            this._lbl_End.AutoSize = true;
            this._lbl_End.Location = new System.Drawing.Point(15, 33);
            this._lbl_End.Name = "_lbl_End";
            this._lbl_End.Size = new System.Drawing.Size(29, 13);
            this._lbl_End.TabIndex = 33;
            this._lbl_End.Text = "End:";
            // 
            // _beginTimeCombo
            // 
            dropDownEditorButton5.Key = "DropDownList";
            this._beginTimeCombo.ButtonsRight.Add(dropDownEditorButton5);
            this._beginTimeCombo.DropDownButtonDisplayStyle = Infragistics.Win.ButtonDisplayStyle.Never;
            this._beginTimeCombo.FormatString = "hh:mm tt";
            this._beginTimeCombo.ListInterval = System.TimeSpan.Parse("00:30:00");
            this._beginTimeCombo.Location = new System.Drawing.Point(158, 3);
            this._beginTimeCombo.MaskInput = "hh:mm tt";
            this._beginTimeCombo.Name = "_beginTimeCombo";
            this._beginTimeCombo.Size = new System.Drawing.Size(82, 21);
            this._beginTimeCombo.TabIndex = 34;
            this._beginTimeCombo.Time = System.TimeSpan.Parse("00:00:00");
            // 
            // _endDateCombo
            // 
            this._endDateCombo.BackColor = System.Drawing.SystemColors.Window;
            this._endDateCombo.DateButtons.Add(dateButton5);
            this._endDateCombo.Location = new System.Drawing.Point(58, 30);
            this._endDateCombo.Name = "_endDateCombo";
            this._endDateCombo.NonAutoSizeHeight = 21;
            this._endDateCombo.NullDateLabel = "";
            this._endDateCombo.Size = new System.Drawing.Size(94, 21);
            this._endDateCombo.TabIndex = 35;
            this._endDateCombo.Value = new System.DateTime(2007, 2, 20, 0, 0, 0, 0);
            // 
            // _endTimeCombo
            // 
            dropDownEditorButton6.Key = "DropDownList";
            this._endTimeCombo.ButtonsRight.Add(dropDownEditorButton6);
            this._endTimeCombo.DropDownButtonDisplayStyle = Infragistics.Win.ButtonDisplayStyle.Never;
            this._endTimeCombo.FormatString = "hh:mm tt";
            this._endTimeCombo.ListInterval = System.TimeSpan.Parse("00:30:00");
            this._endTimeCombo.Location = new System.Drawing.Point(158, 30);
            this._endTimeCombo.MaskInput = "hh:mm tt";
            this._endTimeCombo.Name = "_endTimeCombo";
            this._endTimeCombo.Size = new System.Drawing.Size(82, 21);
            this._endTimeCombo.TabIndex = 36;
            this._endTimeCombo.Time = System.TimeSpan.Parse("00:00:00");
            // 
            // _beginDateCombo
            // 
            this._beginDateCombo.BackColor = System.Drawing.SystemColors.Window;
            this._beginDateCombo.DateButtons.Add(dateButton6);
            this._beginDateCombo.Location = new System.Drawing.Point(58, 3);
            this._beginDateCombo.Name = "_beginDateCombo";
            this._beginDateCombo.NonAutoSizeHeight = 21;
            this._beginDateCombo.NullDateLabel = "";
            this._beginDateCombo.Size = new System.Drawing.Size(94, 21);
            this._beginDateCombo.TabIndex = 37;
            this._beginDateCombo.Value = new System.DateTime(2007, 2, 20, 0, 0, 0, 0);
            // 
            // _pnl_LastN
            // 
            this._pnl_LastN.Controls.Add(this._lbl_Last);
            this._pnl_LastN.Controls.Add(this._lastDaysInput);
            this._pnl_LastN.Controls.Add(this._lbl_Days);
            this._pnl_LastN.Location = new System.Drawing.Point(0, 0);
            this._pnl_LastN.Name = "_pnl_LastN";
            this._pnl_LastN.Size = new System.Drawing.Size(253, 56);
            this._pnl_LastN.TabIndex = 1;
            // 
            // _pnl_TimeSpan
            // 
            this._pnl_TimeSpan.Controls.Add(this._pnl_LastN);
            this._pnl_TimeSpan.Controls.Add(this._lbl_Begin);
            this._pnl_TimeSpan.Controls.Add(this._beginDateCombo);
            this._pnl_TimeSpan.Controls.Add(this._lbl_End);
            this._pnl_TimeSpan.Controls.Add(this._endTimeCombo);
            this._pnl_TimeSpan.Controls.Add(this._beginTimeCombo);
            this._pnl_TimeSpan.Controls.Add(this._endDateCombo);
            this._pnl_TimeSpan.Location = new System.Drawing.Point(3, 16);
            this._pnl_TimeSpan.Name = "_pnl_TimeSpan";
            this._pnl_TimeSpan.Size = new System.Drawing.Size(253, 56);
            this._pnl_TimeSpan.TabIndex = 2;
            // 
            // _lnklbl_Specification
            // 
            this._lnklbl_Specification.AutoSize = true;
            this._lnklbl_Specification.Location = new System.Drawing.Point(3, 0);
            this._lnklbl_Specification.Name = "_lnklbl_Specification";
            this._lnklbl_Specification.Size = new System.Drawing.Size(94, 13);
            this._lnklbl_Specification.TabIndex = 0;
            this._lnklbl_Specification.TabStop = true;
            this._lnklbl_Specification.Text = "Time Specification";
            this._lnklbl_Specification.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this._lnklbl_Specification_LinkClicked);
            // 
            // AlertsTimeFilter
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(227)))), ((int)(((byte)(239)))), ((int)(((byte)(255)))));
            this.Controls.Add(this._lnklbl_Specification);
            this.Controls.Add(this._pnl_TimeSpan);
            this.Name = "AlertsTimeFilter";
            this.Size = new System.Drawing.Size(259, 77);
            ((System.ComponentModel.ISupportInitialize)(this._lastDaysInput)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this._beginTimeCombo)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this._endDateCombo)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this._endTimeCombo)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this._beginDateCombo)).EndInit();
            this._pnl_LastN.ResumeLayout(false);
            this._pnl_LastN.PerformLayout();
            this._pnl_TimeSpan.ResumeLayout(false);
            this._pnl_TimeSpan.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label _lbl_Days;
        private System.Windows.Forms.NumericUpDown _lastDaysInput;
        private System.Windows.Forms.Label _lbl_Last;
        private System.Windows.Forms.Label _lbl_Begin;
        private System.Windows.Forms.Label _lbl_End;
        private Idera.SQLdm.Common.UI.Controls.TimeComboEditor _beginTimeCombo;
        private Infragistics.Win.UltraWinSchedule.UltraCalendarCombo _endDateCombo;
        private Idera.SQLdm.Common.UI.Controls.TimeComboEditor _endTimeCombo;
        private Infragistics.Win.UltraWinSchedule.UltraCalendarCombo _beginDateCombo;
        private System.Windows.Forms.Panel _pnl_LastN;
        private System.Windows.Forms.Panel _pnl_TimeSpan;
        private System.Windows.Forms.LinkLabel _lnklbl_Specification;
    }
}
