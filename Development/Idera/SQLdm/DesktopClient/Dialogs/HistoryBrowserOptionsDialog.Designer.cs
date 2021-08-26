namespace Idera.SQLdm.DesktopClient.Dialogs
{
    partial class HistoryBrowserOptionsDialog
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
            Infragistics.Win.UltraWinEditors.DropDownEditorButton dropDownEditorButton3 = new Infragistics.Win.UltraWinEditors.DropDownEditorButton("DropDownList");
            Infragistics.Win.UltraWinEditors.DropDownEditorButton dropDownEditorButton4 = new Infragistics.Win.UltraWinEditors.DropDownEditorButton("DropDownList");
            this.cancelButton = new System.Windows.Forms.Button();
            this.okButton = new System.Windows.Forms.Button();
            this.panel = new System.Windows.Forms.Panel();
            this.beginTimeDateTimePicker = new System.Windows.Forms.DateTimePicker();
            this.endTimeDateTimePicker = new System.Windows.Forms.DateTimePicker();
            this.labelTo = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.labelScale = new System.Windows.Forms.Label();
            this.realtimeChartsHistoryLimitComboBox = new Infragistics.Win.UltraWinEditors.UltraComboEditor();
            this.beginDateDateTimePicker = new System.Windows.Forms.DateTimePicker();
            this.endDateDateTimePicker = new System.Windows.Forms.DateTimePicker();
            this.panel.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.realtimeChartsHistoryLimitComboBox)).BeginInit();
            this.SuspendLayout();
            // 
            // cancelButton
            // 
            this.cancelButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.cancelButton.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.cancelButton.Location = new System.Drawing.Point(714, 105);
            this.cancelButton.Name = "cancelButton";
            this.cancelButton.Size = new System.Drawing.Size(75, 23);
            this.cancelButton.TabIndex = 1;
            this.cancelButton.Text = "Cancel";
            this.cancelButton.UseVisualStyleBackColor = true;
            // 
            // okButton
            // 
            this.okButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.okButton.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.okButton.Location = new System.Drawing.Point(623, 105);
            this.okButton.Name = "okButton";
            this.okButton.Size = new System.Drawing.Size(75, 23);
            this.okButton.TabIndex = 0;
            this.okButton.Text = "OK";
            this.okButton.UseVisualStyleBackColor = true;
            this.okButton.Click += new System.EventHandler(this.okButton_Click);
            // 
            // panel
            // 
            this.panel.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.panel.Controls.Add(this.labelScale);
            this.panel.Controls.Add(this.beginDateDateTimePicker);
            this.panel.Controls.Add(this.endDateDateTimePicker);
            this.panel.Controls.Add(this.beginTimeDateTimePicker);
            this.panel.Controls.Add(this.endTimeDateTimePicker);
            this.panel.Controls.Add(this.labelTo);
            this.panel.Controls.Add(this.label1);
            this.panel.Controls.Add(this.realtimeChartsHistoryLimitComboBox);
            this.panel.Location = new System.Drawing.Point(12, 12);
            this.panel.Name = "panel";
            this.panel.Size = new System.Drawing.Size(830, 89);
            this.panel.TabIndex = 2;
            this.panel.TabStop = false;
            this.panel.Text = "Select History Range";

            // 
            // labelScale
            // 
            this.labelScale.AutoSize = true;
            this.labelScale.Location = new System.Drawing.Point(0, 33);
            this.labelScale.Name = "labelScale";
            this.labelScale.Size = new System.Drawing.Size(23, 53);
            this.labelScale.TabIndex = 7;
            this.labelScale.Text = "Range:";
            // 
            // realtimeChartsHistoryLimitComboBox
            // 
            this.realtimeChartsHistoryLimitComboBox.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.realtimeChartsHistoryLimitComboBox.DropDownStyle = Infragistics.Win.DropDownStyle.DropDownList;
            valueListItem1.DataValue = 40320;
            valueListItem1.DisplayText = "4 Weeks";
            valueListItem2.DataValue = 7200;
            valueListItem2.DisplayText = "5 Days";
            valueListItem3.DataValue = 1440;
            valueListItem3.DisplayText = "1 Day";
            valueListItem4.DataValue = 480;
            valueListItem4.DisplayText = "8 hours";
            valueListItem5.DataValue = 240;
            valueListItem5.DisplayText = "4 hours";
            valueListItem6.DataValue = 60;
            valueListItem6.DisplayText = "1 hour";
            valueListItem7.DataValue = 15;
            valueListItem7.DisplayText = "15 minutes";
            valueListItem8.DataValue = CUSTOM_OPTION_DATA_VALUE;
            valueListItem8.DisplayText = "Custom";
            //SQLDM - 28460 start
            valueListItem9.DataValue = 30;
            valueListItem9.DisplayText = "30 minutes";
            //SQLDM - 28460 end

            //SQLDM -28593 start
            valueListItem10.DataValue = 120;
            valueListItem10.DisplayText = "2 hours";
            valueListItem11.DataValue = 360;
            valueListItem11.DisplayText = "6 hours";
            //SQLDM-28593 end
            this.realtimeChartsHistoryLimitComboBox.Items.AddRange(new Infragistics.Win.ValueListItem[] {
            valueListItem1,
            valueListItem2,
            valueListItem3,
            valueListItem4,
            //SQLDM-28593 start
            valueListItem11,
            //SQLDM-28593 end
            valueListItem5,
            //SQLDM-28593 start
            valueListItem10,
            //SQLDM-28593 end
            valueListItem6,
            valueListItem9,
            valueListItem7,
            valueListItem8});
            this.realtimeChartsHistoryLimitComboBox.Location = new System.Drawing.Point(42, 27);
            this.realtimeChartsHistoryLimitComboBox.Name = "realtimeChartsHistoryLimitComboBox";
            this.realtimeChartsHistoryLimitComboBox.Size = new System.Drawing.Size(95, 15);
            this.realtimeChartsHistoryLimitComboBox.TabIndex = 8;
            this.realtimeChartsHistoryLimitComboBox.SelectionChanged += new System.EventHandler(this.realtimeChartsHistoryLimitComboBox_SelectionChanged);

            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(160, 33);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(33, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "Start Date:";

            // 
            // beginDateDateTimePicker
            // 
            this.beginDateDateTimePicker.Format = System.Windows.Forms.DateTimePickerFormat.Custom;
            this.beginDateDateTimePicker.CustomFormat = DATE_CUSTOM_FORMAT;
            this.beginDateDateTimePicker.Location = new System.Drawing.Point(222, 29);
            this.beginDateDateTimePicker.Name = "beginDateDateTimePicker";
            this.beginDateDateTimePicker.Size = new System.Drawing.Size(137, 20);
            this.beginDateDateTimePicker.TabIndex = 1;
            this.beginDateDateTimePicker.Value = new System.DateTime(2010, 9, 1, 0, 0, 0, 0);
            this.beginDateDateTimePicker.ValueChanged += new System.EventHandler(this.DateTimePicker_ValueChanged);
            // 
            // beginTimeDateTimePicker
            // 
            this.beginTimeDateTimePicker.CustomFormat = TIME_CUSTOM_FORMAT;
            this.beginTimeDateTimePicker.Format = System.Windows.Forms.DateTimePickerFormat.Custom;
            this.beginTimeDateTimePicker.Location = new System.Drawing.Point(363, 29);
            this.beginTimeDateTimePicker.Name = "beginTimeDateTimePicker";
            this.beginTimeDateTimePicker.ShowUpDown = true;
            this.beginTimeDateTimePicker.Size = new System.Drawing.Size(120, 20);
            this.beginTimeDateTimePicker.TabIndex = 3;
            this.beginTimeDateTimePicker.Value = new System.DateTime(2010, 9, 1, 0, 0, 0, 0);
            this.beginTimeDateTimePicker.ValueChanged += new System.EventHandler(this.DateTimePicker_ValueChanged);
            // 
            // label2
            // 
            this.labelTo.AutoSize = true;
            this.labelTo.Location = new System.Drawing.Point(500, 33);
            this.labelTo.Name = "labelTo";
            this.labelTo.Size = new System.Drawing.Size(23, 13);
            this.labelTo.TabIndex = 4;
            this.labelTo.Text = "End Date:";

            // 
            // endDateDateTimePicker
            // 
            this.endDateDateTimePicker.Format = System.Windows.Forms.DateTimePickerFormat.Custom;
            this.endDateDateTimePicker.CustomFormat = DATE_CUSTOM_FORMAT;
            this.endDateDateTimePicker.Location = new System.Drawing.Point(560, 29);
            this.endDateDateTimePicker.Name = "endDateDateTimePicker";
            this.endDateDateTimePicker.Size = new System.Drawing.Size(137, 20);
            this.endDateDateTimePicker.TabIndex = 5;
            this.endDateDateTimePicker.Value = new System.DateTime(2010, 9, 1, 0, 0, 0, 0);
            this.endDateDateTimePicker.ValueChanged += new System.EventHandler(this.DateTimePicker_ValueChanged);
            // 
            // endTimeDateTimePicker
            // 
            this.endTimeDateTimePicker.CustomFormat = TIME_CUSTOM_FORMAT;
            this.endTimeDateTimePicker.Format = System.Windows.Forms.DateTimePickerFormat.Custom;
            this.endTimeDateTimePicker.Location = new System.Drawing.Point(700, 29);
            this.endTimeDateTimePicker.Name = "endTimeDateTimePicker";
            this.endTimeDateTimePicker.ShowUpDown = true;
            this.endTimeDateTimePicker.Size = new System.Drawing.Size(120, 20);
            this.endTimeDateTimePicker.TabIndex = 6;
            this.endTimeDateTimePicker.Value = new System.DateTime(2010, 9, 1, 0, 0, 0, 0);
            this.endTimeDateTimePicker.ValueChanged += new System.EventHandler(this.DateTimePicker_ValueChanged);
            
            
            //Setting default values

            this.beginDateDateTimePicker.Enabled = false;
            this.beginTimeDateTimePicker.Enabled = false;
            this.endDateDateTimePicker.Enabled = false;
            this.endTimeDateTimePicker.Enabled = false;
            // 
            // HistoryBrowserOptionsDialog
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(850, 152);
            this.Controls.Add(this.panel);
            this.Controls.Add(this.okButton);
            this.Controls.Add(this.cancelButton);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.HelpButton = true;
            this.HelpButtonClicked += new System.ComponentModel.CancelEventHandler(this.HistoryBrowserOptionsDialog_HelpButtonClicked);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "HistoryBrowserOptionsDialog";
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "History Browser Range";
            ((System.ComponentModel.ISupportInitialize)(this.realtimeChartsHistoryLimitComboBox)).EndInit();
            this.panel.ResumeLayout(false);
            this.panel.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button cancelButton;
        private System.Windows.Forms.Button okButton;
        private System.Windows.Forms.Panel panel;
        private System.Windows.Forms.Label labelTo;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label labelScale;
        private Infragistics.Win.UltraWinEditors.UltraComboEditor realtimeChartsHistoryLimitComboBox;

        Infragistics.Win.ValueListItem valueListItem1 = new Infragistics.Win.ValueListItem();
        Infragistics.Win.ValueListItem valueListItem2 = new Infragistics.Win.ValueListItem();
        Infragistics.Win.ValueListItem valueListItem3 = new Infragistics.Win.ValueListItem();
        Infragistics.Win.ValueListItem valueListItem4 = new Infragistics.Win.ValueListItem();
        Infragistics.Win.ValueListItem valueListItem5 = new Infragistics.Win.ValueListItem();
        Infragistics.Win.ValueListItem valueListItem6 = new Infragistics.Win.ValueListItem();
        Infragistics.Win.ValueListItem valueListItem7 = new Infragistics.Win.ValueListItem();
        Infragistics.Win.ValueListItem valueListItem8 = new Infragistics.Win.ValueListItem();
        //SQLDM-28460 start
        Infragistics.Win.ValueListItem valueListItem9 = new Infragistics.Win.ValueListItem();
        //SQLDM-28460 end
        //SQLDM-28593 start
        Infragistics.Win.ValueListItem valueListItem10 = new Infragistics.Win.ValueListItem();
        Infragistics.Win.ValueListItem valueListItem11 = new Infragistics.Win.ValueListItem();
        //SQLDM-28593 end
        private System.Windows.Forms.DateTimePicker beginDateDateTimePicker;
        private System.Windows.Forms.DateTimePicker endDateDateTimePicker;
        private System.Windows.Forms.DateTimePicker beginTimeDateTimePicker;
        private System.Windows.Forms.DateTimePicker endTimeDateTimePicker;
    }
}