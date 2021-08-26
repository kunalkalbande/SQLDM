namespace Idera.SQLdm.DesktopClient.Dialogs.Notification
{
    partial class SnapshotTimeDialog
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
            this.cboStartTime = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomComboBox();
            this.cboEndTime = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomComboBox();
            this.btnOK = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomButton();
            this.btnCancel = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomButton();
            this.label1 = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomLabel();
            this.label2 = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomLabel();
            this.chkUseDOW = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomCheckBox();
            this.chkSunday = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomCheckBox();
            this.chkMonday = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomCheckBox();
            this.chkTuesday = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomCheckBox();
            this.chkWednesday = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomCheckBox();
            this.chkSaturday = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomCheckBox();
            this.chkFriday = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomCheckBox();
            this.chkThursday = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomCheckBox();
            this.dowPanel = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomTableLayoutPanel();
            this.errorProvider = new System.Windows.Forms.ErrorProvider(this.components);
            this.dowPanel.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.errorProvider)).BeginInit();
            this.SuspendLayout();
            // 
            // cboStartTime
            // 
            this.cboStartTime.Location = new System.Drawing.Point(93, 12);
            this.cboStartTime.Name = "cboStartTime";
            this.cboStartTime.Size = new System.Drawing.Size(104, 21);
            this.cboStartTime.TabIndex = 1;
            this.cboStartTime.TextChanged += new System.EventHandler(this.OnTextChanged);
            // 
            // cboEndTime
            // 
            this.cboEndTime.Location = new System.Drawing.Point(93, 39);
            this.cboEndTime.Name = "cboEndTime";
            this.cboEndTime.Size = new System.Drawing.Size(104, 21);
            this.cboEndTime.TabIndex = 3;
            this.cboEndTime.TextChanged += new System.EventHandler(this.OnTextChanged);
            // 
            // btnOK
            // 
            this.btnOK.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnOK.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.btnOK.Location = new System.Drawing.Point(176, 161);
            this.btnOK.Name = "btnOK";
            this.btnOK.Size = new System.Drawing.Size(75, 23);
            this.btnOK.TabIndex = 5;
            this.btnOK.Text = "OK";
            this.btnOK.UseVisualStyleBackColor = true;
            this.btnOK.Click += new System.EventHandler(this.btnOK_Click);
            // 
            // btnCancel
            // 
            this.btnCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnCancel.Location = new System.Drawing.Point(259, 161);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(75, 23);
            this.btnCancel.TabIndex = 6;
            this.btnCancel.Text = "Cancel";
            this.btnCancel.UseVisualStyleBackColor = true;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(14, 16);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(58, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "Start Time:";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(14, 39);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(55, 13);
            this.label2.TabIndex = 2;
            this.label2.Text = "End Time:";
            // 
            // chkUseDOW
            // 
            this.chkUseDOW.Location = new System.Drawing.Point(17, 71);
            this.chkUseDOW.Name = "chkUseDOW";
            this.chkUseDOW.Size = new System.Drawing.Size(120, 20);
            this.chkUseDOW.TabIndex = 4;
            this.chkUseDOW.Text = "On these days:";
            this.chkUseDOW.CheckStateChanged += new System.EventHandler(this.OnCheckStateChanged);
            // 
            // chkSunday
            // 
            this.chkSunday.Location = new System.Drawing.Point(3, 3);
            this.chkSunday.Name = "chkSunday";
            this.chkSunday.Size = new System.Drawing.Size(70, 20);
            this.chkSunday.TabIndex = 0;
            this.chkSunday.Text = "Sunday";
            this.chkSunday.CheckStateChanged += new System.EventHandler(this.OnCheckStateChanged);
            // 
            // chkMonday
            // 
            this.chkMonday.Location = new System.Drawing.Point(79, 3);
            this.chkMonday.Name = "chkMonday";
            this.chkMonday.Size = new System.Drawing.Size(73, 20);
            this.chkMonday.TabIndex = 1;
            this.chkMonday.Text = "Monday";
            this.chkMonday.CheckStateChanged += new System.EventHandler(this.OnCheckStateChanged);
            // 
            // chkTuesday
            // 
            this.chkTuesday.Location = new System.Drawing.Point(158, 3);
            this.chkTuesday.Name = "chkTuesday";
            this.chkTuesday.Size = new System.Drawing.Size(69, 20);
            this.chkTuesday.TabIndex = 2;
            this.chkTuesday.Text = "Tuesday";
            this.chkTuesday.CheckStateChanged += new System.EventHandler(this.OnCheckStateChanged);
            // 
            // chkWednesday
            // 
            this.chkWednesday.Location = new System.Drawing.Point(233, 3);
            this.chkWednesday.Name = "chkWednesday";
            this.chkWednesday.Size = new System.Drawing.Size(85, 20);
            this.chkWednesday.TabIndex = 3;
            this.chkWednesday.Text = "Wednesday";
            this.chkWednesday.CheckStateChanged += new System.EventHandler(this.OnCheckStateChanged);
            // 
            // chkSaturday
            // 
            this.chkSaturday.Location = new System.Drawing.Point(158, 29);
            this.chkSaturday.Name = "chkSaturday";
            this.chkSaturday.Size = new System.Drawing.Size(69, 20);
            this.chkSaturday.TabIndex = 6;
            this.chkSaturday.Text = "Saturday";
            this.chkSaturday.CheckStateChanged += new System.EventHandler(this.OnCheckStateChanged);
            // 
            // chkFriday
            // 
            this.chkFriday.Location = new System.Drawing.Point(79, 29);
            this.chkFriday.Name = "chkFriday";
            this.chkFriday.Size = new System.Drawing.Size(73, 20);
            this.chkFriday.TabIndex = 5;
            this.chkFriday.Text = "Friday";
            this.chkFriday.CheckStateChanged += new System.EventHandler(this.OnCheckStateChanged);
            // 
            // chkThursday
            // 
            this.chkThursday.Location = new System.Drawing.Point(3, 29);
            this.chkThursday.Name = "chkThursday";
            this.chkThursday.Size = new System.Drawing.Size(70, 20);
            this.chkThursday.TabIndex = 4;
            this.chkThursday.Text = "Thursday";
            this.chkThursday.CheckStateChanged += new System.EventHandler(this.OnCheckStateChanged);
            // 
            // dowPanel
            // 
            this.dowPanel.ColumnCount = 4;
            this.dowPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 23.19277F));
            this.dowPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 24.09639F));
            this.dowPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 22.89157F));
            this.dowPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 30.42169F));
            this.dowPanel.Controls.Add(this.chkTuesday, 2, 0);
            this.dowPanel.Controls.Add(this.chkSaturday, 2, 1);
            this.dowPanel.Controls.Add(this.chkSunday, 0, 0);
            this.dowPanel.Controls.Add(this.chkFriday, 1, 1);
            this.dowPanel.Controls.Add(this.chkMonday, 1, 0);
            this.dowPanel.Controls.Add(this.chkThursday, 0, 1);
            this.dowPanel.Controls.Add(this.chkWednesday, 3, 0);
            this.dowPanel.Location = new System.Drawing.Point(14, 97);
            this.dowPanel.Name = "dowPanel";
            this.dowPanel.RowCount = 2;
            this.dowPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.dowPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.dowPanel.Size = new System.Drawing.Size(332, 52);
            this.dowPanel.TabIndex = 15;
            // 
            // errorProvider
            // 
            this.errorProvider.ContainerControl = this;
            // 
            // SnapshotTimeDialog
            // 
            this.AcceptButton = this.btnOK;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.btnCancel;
            this.ClientSize = new System.Drawing.Size(346, 196);
            this.Controls.Add(this.dowPanel);
            this.Controls.Add(this.chkUseDOW);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnOK);
            this.Controls.Add(this.cboEndTime);
            this.Controls.Add(this.cboStartTime);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.HelpButton = true;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "SnapshotTimeDialog";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Alert Response Time Period";
            this.Load += new System.EventHandler(this.SnapshotTimeDialog_Load);
            this.dowPanel.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.errorProvider)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomComboBox cboStartTime;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomComboBox cboEndTime;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomButton btnOK;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomButton btnCancel;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomLabel label1;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomLabel label2;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomCheckBox chkUseDOW;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomCheckBox chkSunday;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomCheckBox chkMonday;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomCheckBox chkTuesday;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomCheckBox chkWednesday;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomCheckBox chkSaturday;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomCheckBox chkFriday;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomCheckBox chkThursday;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomTableLayoutPanel dowPanel;
        private System.Windows.Forms.ErrorProvider errorProvider;

    }
}