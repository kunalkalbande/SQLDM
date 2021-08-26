namespace Idera.SQLdm.DesktopClient.Dialogs
{
    partial class ConfigureBaselineDialog
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
            Infragistics.Win.UltraWinSchedule.CalendarCombo.DateButton dateButton3 = new Infragistics.Win.UltraWinSchedule.CalendarCombo.DateButton();
            Infragistics.Win.UltraWinSchedule.CalendarCombo.DateButton dateButton4 = new Infragistics.Win.UltraWinSchedule.CalendarCombo.DateButton();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ConfigureBaselineDialog));
            this.okButton = new System.Windows.Forms.Button();
            this.cancelButton = new System.Windows.Forms.Button();
            this.office2007PropertyPage1 = new Idera.SQLdm.DesktopClient.Controls.Office2007PropertyPage();
            this.label4 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.endTimeCombo = new Idera.SQLdm.Common.UI.Controls.TimeComboEditor();
            this.beginTimeCombo = new Idera.SQLdm.Common.UI.Controls.TimeComboEditor();
            this.saturdayCheckBox = new System.Windows.Forms.CheckBox();
            this.fridayCheckBox = new System.Windows.Forms.CheckBox();
            this.thursdayCheckBox = new System.Windows.Forms.CheckBox();
            this.wednesdayCheckBox = new System.Windows.Forms.CheckBox();
            this.tuesdayCheckBox = new System.Windows.Forms.CheckBox();
            this.sundayCheckbox = new System.Windows.Forms.CheckBox();
            this.mondayCheckBox = new System.Windows.Forms.CheckBox();
            this.propertiesHeaderStrip1 = new Idera.SQLdm.DesktopClient.Controls.PropertiesHeaderStrip();
            this.customDateToLabel = new System.Windows.Forms.Label();
            this.customDateToCombo = new Infragistics.Win.UltraWinSchedule.UltraCalendarCombo();
            this.customDateFromLabel = new System.Windows.Forms.Label();
            this.customDateFromCombo = new Infragistics.Win.UltraWinSchedule.UltraCalendarCombo();
            this.customBaselineRadioButton = new System.Windows.Forms.RadioButton();
            this.informationBox1 = new Divelements.WizardFramework.InformationBox();
            this.automaticBaselineRadioButton = new System.Windows.Forms.RadioButton();
            this.headerStrip1 = new Idera.SQLdm.DesktopClient.Controls.PropertiesHeaderStrip();
            this.office2007PropertyPage1.ContentPanel.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.endTimeCombo)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.beginTimeCombo)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.customDateToCombo)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.customDateFromCombo)).BeginInit();
            this.SuspendLayout();
            // 
            // okButton
            // 
            this.okButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.okButton.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.okButton.Location = new System.Drawing.Point(375, 378);
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
            this.cancelButton.Location = new System.Drawing.Point(456, 378);
            this.cancelButton.Name = "cancelButton";
            this.cancelButton.Size = new System.Drawing.Size(75, 23);
            this.cancelButton.TabIndex = 1;
            this.cancelButton.Text = "Cancel";
            this.cancelButton.UseVisualStyleBackColor = true;
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
            this.office2007PropertyPage1.ContentPanel.Controls.Add(this.label4);
            this.office2007PropertyPage1.ContentPanel.Controls.Add(this.label3);
            this.office2007PropertyPage1.ContentPanel.Controls.Add(this.endTimeCombo);
            this.office2007PropertyPage1.ContentPanel.Controls.Add(this.beginTimeCombo);
            this.office2007PropertyPage1.ContentPanel.Controls.Add(this.saturdayCheckBox);
            this.office2007PropertyPage1.ContentPanel.Controls.Add(this.fridayCheckBox);
            this.office2007PropertyPage1.ContentPanel.Controls.Add(this.thursdayCheckBox);
            this.office2007PropertyPage1.ContentPanel.Controls.Add(this.wednesdayCheckBox);
            this.office2007PropertyPage1.ContentPanel.Controls.Add(this.tuesdayCheckBox);
            this.office2007PropertyPage1.ContentPanel.Controls.Add(this.sundayCheckbox);
            this.office2007PropertyPage1.ContentPanel.Controls.Add(this.mondayCheckBox);
            this.office2007PropertyPage1.ContentPanel.Controls.Add(this.propertiesHeaderStrip1);
            this.office2007PropertyPage1.ContentPanel.Controls.Add(this.customDateToLabel);
            this.office2007PropertyPage1.ContentPanel.Controls.Add(this.customDateToCombo);
            this.office2007PropertyPage1.ContentPanel.Controls.Add(this.customDateFromLabel);
            this.office2007PropertyPage1.ContentPanel.Controls.Add(this.customDateFromCombo);
            this.office2007PropertyPage1.ContentPanel.Controls.Add(this.customBaselineRadioButton);
            this.office2007PropertyPage1.ContentPanel.Controls.Add(this.informationBox1);
            this.office2007PropertyPage1.ContentPanel.Controls.Add(this.automaticBaselineRadioButton);
            this.office2007PropertyPage1.ContentPanel.Controls.Add(this.headerStrip1);
            this.office2007PropertyPage1.ContentPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.office2007PropertyPage1.ContentPanel.FillStyle = Idera.SQLdm.DesktopClient.Controls.GradientPanelFillStyle.Solid;
            this.office2007PropertyPage1.ContentPanel.Location = new System.Drawing.Point(1, 56);
            this.office2007PropertyPage1.ContentPanel.Name = "ContentPanel";
            this.office2007PropertyPage1.ContentPanel.Padding = new System.Windows.Forms.Padding(1);
            this.office2007PropertyPage1.ContentPanel.ShowBorder = false;
            this.office2007PropertyPage1.ContentPanel.Size = new System.Drawing.Size(517, 303);
            this.office2007PropertyPage1.ContentPanel.TabIndex = 1;
            this.office2007PropertyPage1.HeaderImage = global::Idera.SQLdm.DesktopClient.Properties.Resources.Chart32x32;
            this.office2007PropertyPage1.Location = new System.Drawing.Point(12, 12);
            this.office2007PropertyPage1.Name = "office2007PropertyPage1";
            this.office2007PropertyPage1.Size = new System.Drawing.Size(519, 360);
            this.office2007PropertyPage1.TabIndex = 2;
            this.office2007PropertyPage1.Text = "Configure a performance baseline for your monitored SQL Server.";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(253, 256);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(23, 13);
            this.label4.TabIndex = 22;
            this.label4.Text = "To:";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(120, 256);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(33, 13);
            this.label3.TabIndex = 21;
            this.label3.Text = "From:";
            // 
            // endTimeCombo
            // 
            this.endTimeCombo.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            dropDownEditorButton3.Key = "DropDownList";
            this.endTimeCombo.ButtonsRight.Add(dropDownEditorButton3);
            this.endTimeCombo.DateTime = new System.DateTime(2008, 8, 1, 17, 0, 0, 0);
            this.endTimeCombo.DropDownButtonDisplayStyle = Infragistics.Win.ButtonDisplayStyle.Never;
            this.endTimeCombo.ListInterval = System.TimeSpan.Parse("00:30:00");
            this.endTimeCombo.Location = new System.Drawing.Point(282, 252);
            this.endTimeCombo.Name = "endTimeCombo";
            this.endTimeCombo.Size = new System.Drawing.Size(88, 21);
            this.endTimeCombo.TabIndex = 20;
            this.endTimeCombo.Time = System.TimeSpan.Parse("17:00:00");
            this.endTimeCombo.Value = new System.DateTime(2008, 8, 1, 17, 0, 0, 0);
            // 
            // beginTimeCombo
            // 
            this.beginTimeCombo.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            dropDownEditorButton4.Key = "DropDownList";
            this.beginTimeCombo.ButtonsRight.Add(dropDownEditorButton4);
            this.beginTimeCombo.DateTime = new System.DateTime(2008, 8, 1, 8, 0, 0, 0);
            this.beginTimeCombo.DropDownButtonDisplayStyle = Infragistics.Win.ButtonDisplayStyle.Never;
            this.beginTimeCombo.ListInterval = System.TimeSpan.Parse("00:30:00");
            this.beginTimeCombo.Location = new System.Drawing.Point(159, 252);
            this.beginTimeCombo.Name = "beginTimeCombo";
            this.beginTimeCombo.Size = new System.Drawing.Size(88, 21);
            this.beginTimeCombo.TabIndex = 19;
            this.beginTimeCombo.Time = System.TimeSpan.Parse("08:00:00");
            this.beginTimeCombo.Value = new System.DateTime(2008, 8, 1, 8, 0, 0, 0);
            // 
            // saturdayCheckBox
            // 
            this.saturdayCheckBox.AutoSize = true;
            this.saturdayCheckBox.Location = new System.Drawing.Point(395, 220);
            this.saturdayCheckBox.Name = "saturdayCheckBox";
            this.saturdayCheckBox.Size = new System.Drawing.Size(42, 17);
            this.saturdayCheckBox.TabIndex = 18;
            this.saturdayCheckBox.Text = "Sat";
            this.saturdayCheckBox.UseVisualStyleBackColor = true;
            // 
            // fridayCheckBox
            // 
            this.fridayCheckBox.AutoSize = true;
            this.fridayCheckBox.Checked = true;
            this.fridayCheckBox.CheckState = System.Windows.Forms.CheckState.Checked;
            this.fridayCheckBox.Location = new System.Drawing.Point(352, 220);
            this.fridayCheckBox.Name = "fridayCheckBox";
            this.fridayCheckBox.Size = new System.Drawing.Size(37, 17);
            this.fridayCheckBox.TabIndex = 17;
            this.fridayCheckBox.Text = "Fri";
            this.fridayCheckBox.UseVisualStyleBackColor = true;
            // 
            // thursdayCheckBox
            // 
            this.thursdayCheckBox.AutoSize = true;
            this.thursdayCheckBox.Checked = true;
            this.thursdayCheckBox.CheckState = System.Windows.Forms.CheckState.Checked;
            this.thursdayCheckBox.Location = new System.Drawing.Point(301, 220);
            this.thursdayCheckBox.Name = "thursdayCheckBox";
            this.thursdayCheckBox.Size = new System.Drawing.Size(45, 17);
            this.thursdayCheckBox.TabIndex = 16;
            this.thursdayCheckBox.Text = "Thu";
            this.thursdayCheckBox.UseVisualStyleBackColor = true;
            // 
            // wednesdayCheckBox
            // 
            this.wednesdayCheckBox.AutoSize = true;
            this.wednesdayCheckBox.Checked = true;
            this.wednesdayCheckBox.CheckState = System.Windows.Forms.CheckState.Checked;
            this.wednesdayCheckBox.Location = new System.Drawing.Point(246, 220);
            this.wednesdayCheckBox.Name = "wednesdayCheckBox";
            this.wednesdayCheckBox.Size = new System.Drawing.Size(49, 17);
            this.wednesdayCheckBox.TabIndex = 15;
            this.wednesdayCheckBox.Text = "Wed";
            this.wednesdayCheckBox.UseVisualStyleBackColor = true;
            // 
            // tuesdayCheckBox
            // 
            this.tuesdayCheckBox.AutoSize = true;
            this.tuesdayCheckBox.Checked = true;
            this.tuesdayCheckBox.CheckState = System.Windows.Forms.CheckState.Checked;
            this.tuesdayCheckBox.Location = new System.Drawing.Point(195, 220);
            this.tuesdayCheckBox.Name = "tuesdayCheckBox";
            this.tuesdayCheckBox.Size = new System.Drawing.Size(45, 17);
            this.tuesdayCheckBox.TabIndex = 14;
            this.tuesdayCheckBox.Text = "Tue";
            this.tuesdayCheckBox.UseVisualStyleBackColor = true;
            // 
            // sundayCheckbox
            // 
            this.sundayCheckbox.AutoSize = true;
            this.sundayCheckbox.Location = new System.Drawing.Point(91, 220);
            this.sundayCheckbox.Name = "sundayCheckbox";
            this.sundayCheckbox.Size = new System.Drawing.Size(45, 17);
            this.sundayCheckbox.TabIndex = 13;
            this.sundayCheckbox.Text = "Sun";
            this.sundayCheckbox.UseVisualStyleBackColor = true;
            // 
            // mondayCheckBox
            // 
            this.mondayCheckBox.AutoSize = true;
            this.mondayCheckBox.Checked = true;
            this.mondayCheckBox.CheckState = System.Windows.Forms.CheckState.Checked;
            this.mondayCheckBox.Location = new System.Drawing.Point(142, 220);
            this.mondayCheckBox.Name = "mondayCheckBox";
            this.mondayCheckBox.Size = new System.Drawing.Size(47, 17);
            this.mondayCheckBox.TabIndex = 12;
            this.mondayCheckBox.Text = "Mon";
            this.mondayCheckBox.UseVisualStyleBackColor = true;
            // 
            // propertiesHeaderStrip1
            // 
            this.propertiesHeaderStrip1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.propertiesHeaderStrip1.ForeColor = System.Drawing.Color.Black;
            this.propertiesHeaderStrip1.Location = new System.Drawing.Point(11, 183);
            this.propertiesHeaderStrip1.Name = "propertiesHeaderStrip1";
            this.propertiesHeaderStrip1.Size = new System.Drawing.Size(498, 25);
            this.propertiesHeaderStrip1.TabIndex = 11;
            this.propertiesHeaderStrip1.Text = "Which days and time period within those days should be used to calculate the base" +
                "line?";
            // 
            // customDateToLabel
            // 
            this.customDateToLabel.AutoSize = true;
            this.customDateToLabel.Enabled = false;
            this.customDateToLabel.Location = new System.Drawing.Point(253, 145);
            this.customDateToLabel.Name = "customDateToLabel";
            this.customDateToLabel.Size = new System.Drawing.Size(23, 13);
            this.customDateToLabel.TabIndex = 10;
            this.customDateToLabel.Text = "To:";
            // 
            // customDateToCombo
            // 
            this.customDateToCombo.BackColor = System.Drawing.SystemColors.Window;
            this.customDateToCombo.DateButtons.Add(dateButton3);
            this.customDateToCombo.Enabled = false;
            this.customDateToCombo.Location = new System.Drawing.Point(282, 142);
            this.customDateToCombo.Name = "customDateToCombo";
            this.customDateToCombo.NonAutoSizeHeight = 21;
            this.customDateToCombo.Size = new System.Drawing.Size(88, 21);
            this.customDateToCombo.TabIndex = 9;
            this.customDateToCombo.Value = new System.DateTime(2008, 8, 13, 0, 0, 0, 0);
            // 
            // customDateFromLabel
            // 
            this.customDateFromLabel.AutoSize = true;
            this.customDateFromLabel.Enabled = false;
            this.customDateFromLabel.Location = new System.Drawing.Point(120, 145);
            this.customDateFromLabel.Name = "customDateFromLabel";
            this.customDateFromLabel.Size = new System.Drawing.Size(33, 13);
            this.customDateFromLabel.TabIndex = 8;
            this.customDateFromLabel.Text = "From:";
            // 
            // customDateFromCombo
            // 
            this.customDateFromCombo.BackColor = System.Drawing.SystemColors.Window;
            this.customDateFromCombo.DateButtons.Add(dateButton4);
            this.customDateFromCombo.Enabled = false;
            this.customDateFromCombo.Location = new System.Drawing.Point(159, 142);
            this.customDateFromCombo.Name = "customDateFromCombo";
            this.customDateFromCombo.NonAutoSizeHeight = 21;
            this.customDateFromCombo.Size = new System.Drawing.Size(88, 21);
            this.customDateFromCombo.TabIndex = 7;
            this.customDateFromCombo.Value = new System.DateTime(2008, 8, 13, 0, 0, 0, 0);
            // 
            // customBaselineRadioButton
            // 
            this.customBaselineRadioButton.AutoSize = true;
            this.customBaselineRadioButton.Location = new System.Drawing.Point(61, 119);
            this.customBaselineRadioButton.Name = "customBaselineRadioButton";
            this.customBaselineRadioButton.Size = new System.Drawing.Size(60, 17);
            this.customBaselineRadioButton.TabIndex = 6;
            this.customBaselineRadioButton.Text = "Custom";
            this.customBaselineRadioButton.UseVisualStyleBackColor = true;
            this.customBaselineRadioButton.CheckedChanged += new System.EventHandler(this.customBaselineRadioButton_CheckedChanged);
            // 
            // informationBox1
            // 
            this.informationBox1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.informationBox1.Location = new System.Drawing.Point(27, 43);
            this.informationBox1.Name = "informationBox1";
            this.informationBox1.Size = new System.Drawing.Size(465, 47);
            this.informationBox1.TabIndex = 5;
            this.informationBox1.Text = resources.GetString("informationBox1.Text");
            // 
            // automaticBaselineRadioButton
            // 
            this.automaticBaselineRadioButton.AutoSize = true;
            this.automaticBaselineRadioButton.Checked = true;
            this.automaticBaselineRadioButton.Location = new System.Drawing.Point(61, 96);
            this.automaticBaselineRadioButton.Name = "automaticBaselineRadioButton";
            this.automaticBaselineRadioButton.Size = new System.Drawing.Size(422, 17);
            this.automaticBaselineRadioButton.TabIndex = 2;
            this.automaticBaselineRadioButton.TabStop = true;
            this.automaticBaselineRadioButton.Text = "Automatic (calculates a dynamic baseline based on the last 7 days of collected da" +
                "ta)";
            this.automaticBaselineRadioButton.TextAlign = System.Drawing.ContentAlignment.TopLeft;
            this.automaticBaselineRadioButton.UseVisualStyleBackColor = true;
            // 
            // headerStrip1
            // 
            this.headerStrip1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.headerStrip1.ForeColor = System.Drawing.Color.Black;
            this.headerStrip1.Location = new System.Drawing.Point(11, 12);
            this.headerStrip1.Name = "headerStrip1";
            this.headerStrip1.Size = new System.Drawing.Size(498, 25);
            this.headerStrip1.TabIndex = 1;
            this.headerStrip1.Text = "What date range should be used to calculate the performance baseline for this ser" +
                "ver?";
            // 
            // ConfigureBaselineDialog
            // 
            this.AcceptButton = this.okButton;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.cancelButton;
            this.ClientSize = new System.Drawing.Size(543, 613);
            this.Controls.Add(this.office2007PropertyPage1);
            this.Controls.Add(this.cancelButton);
            this.Controls.Add(this.okButton);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.HelpButton = true;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "ConfigureBaselineDialog";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Configure Baseline for {0}";
            this.Load += new System.EventHandler(this.ConfigureBaselineDialog_Load);
            this.HelpButtonClicked += new System.ComponentModel.CancelEventHandler(this.ConfigureBaselineDialog_HelpButtonClicked);
            this.HelpRequested += new System.Windows.Forms.HelpEventHandler(this.ConfigureBaselineDialog_HelpRequested);
            this.office2007PropertyPage1.ContentPanel.ResumeLayout(false);
            this.office2007PropertyPage1.ContentPanel.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.endTimeCombo)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.beginTimeCombo)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.customDateToCombo)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.customDateFromCombo)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button okButton;
        private System.Windows.Forms.Button cancelButton;
        private Idera.SQLdm.DesktopClient.Controls.Office2007PropertyPage office2007PropertyPage1;
        private System.Windows.Forms.RadioButton automaticBaselineRadioButton;
        private Idera.SQLdm.DesktopClient.Controls.PropertiesHeaderStrip headerStrip1;
        private Divelements.WizardFramework.InformationBox informationBox1;
        private System.Windows.Forms.RadioButton customBaselineRadioButton;
        private Idera.SQLdm.DesktopClient.Controls.PropertiesHeaderStrip propertiesHeaderStrip1;
        private System.Windows.Forms.Label customDateToLabel;
        private Infragistics.Win.UltraWinSchedule.UltraCalendarCombo customDateToCombo;
        private System.Windows.Forms.Label customDateFromLabel;
        private Infragistics.Win.UltraWinSchedule.UltraCalendarCombo customDateFromCombo;
        private System.Windows.Forms.CheckBox saturdayCheckBox;
        private System.Windows.Forms.CheckBox fridayCheckBox;
        private System.Windows.Forms.CheckBox thursdayCheckBox;
        private System.Windows.Forms.CheckBox wednesdayCheckBox;
        private System.Windows.Forms.CheckBox tuesdayCheckBox;
        private System.Windows.Forms.CheckBox sundayCheckbox;
        private System.Windows.Forms.CheckBox mondayCheckBox;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label3;
        private Idera.SQLdm.Common.UI.Controls.TimeComboEditor endTimeCombo;
        private Idera.SQLdm.Common.UI.Controls.TimeComboEditor beginTimeCombo;
    }
}