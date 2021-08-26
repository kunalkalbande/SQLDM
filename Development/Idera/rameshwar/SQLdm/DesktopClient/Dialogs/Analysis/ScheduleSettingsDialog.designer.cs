namespace Idera.SQLdoctor.StandardClient.Dialogs
{
    partial class ScheduleSettingsDialog
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
            Infragistics.Win.Appearance appearance1 = new Infragistics.Win.Appearance();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ScheduleSettingsDialog));
            Infragistics.Win.Appearance appearance2 = new Infragistics.Win.Appearance();
            this._oneTimeAnalysisInfoPanel = new System.Windows.Forms.TableLayoutPanel();
            this._oneTimeAnalysisHeaderStrip = new Idera.SQLdoctor.CommonGUI.Controls.Headers.PropertiesHeaderStrip();
            this._oneTimeAnalysisLinkLabel = new Infragistics.Win.FormattedLinkLabel.UltraFormattedLinkLabel();
            this._scheduleSettingsPanel = new System.Windows.Forms.TableLayoutPanel();
            this._recurrenceComboBox = new System.Windows.Forms.ComboBox();
            this.panel1 = new System.Windows.Forms.Panel();
            this.label4 = new System.Windows.Forms.Label();
            this._startTimePicker = new System.Windows.Forms.DateTimePicker();
            this._weeklyRecurrencePanel = new System.Windows.Forms.Panel();
            this._sundayCheckBox = new System.Windows.Forms.CheckBox();
            this._saturdayCheckBox = new System.Windows.Forms.CheckBox();
            this._fridayCheckBox = new System.Windows.Forms.CheckBox();
            this._thursdayCheckBox = new System.Windows.Forms.CheckBox();
            this._wednesdayCheckBox = new System.Windows.Forms.CheckBox();
            this._tuesdayCheckBox = new System.Windows.Forms.CheckBox();
            this._mondayCheckBox = new System.Windows.Forms.CheckBox();
            this.label1 = new System.Windows.Forms.Label();
            this._serverNameComboBox = new System.Windows.Forms.ComboBox();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this._lastRunStatusLabel = new System.Windows.Forms.Label();
            this._scheduleEnabledCheckBox = new System.Windows.Forms.CheckBox();
            this.panelHeader = new Idera.SQLdoctor.CommonGUI.Controls.Panels.GradientPanelWithoutBorder();
            this.multiGradientPanel1 = new Idera.SQLdoctor.CommonGUI.Controls.Panels.MultiGradientPanel();
            this.label5 = new System.Windows.Forms.Label();
            this.lblSQLie = new System.Windows.Forms.Label();
            this.pictureHeader = new System.Windows.Forms.PictureBox();
            this.panel2 = new System.Windows.Forms.Panel();
            this.btnCancel = new System.Windows.Forms.Button();
            this.btnOK = new System.Windows.Forms.Button();
            this.panelButtons = new System.Windows.Forms.Panel();
            this.lblEtched = new System.Windows.Forms.Label();
            this.NumericAnalysisDuration = new System.Windows.Forms.NumericUpDown();
            this.label7 = new System.Windows.Forms.Label();
            this.panel3 = new System.Windows.Forms.Panel();
            this.label6 = new System.Windows.Forms.Label();
            this._oneTimeAnalysisInfoPanel.SuspendLayout();
            this._scheduleSettingsPanel.SuspendLayout();
            this.panel1.SuspendLayout();
            this._weeklyRecurrencePanel.SuspendLayout();
            this.panelHeader.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureHeader)).BeginInit();
            this.panel2.SuspendLayout();
            this.panelButtons.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.NumericAnalysisDuration)).BeginInit();
            this.panel3.SuspendLayout();
            this.SuspendLayout();
            // 
            // _oneTimeAnalysisInfoPanel
            // 
            this._oneTimeAnalysisInfoPanel.ColumnCount = 1;
            this._oneTimeAnalysisInfoPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this._oneTimeAnalysisInfoPanel.Controls.Add(this._oneTimeAnalysisHeaderStrip, 0, 0);
            this._oneTimeAnalysisInfoPanel.Controls.Add(this._oneTimeAnalysisLinkLabel, 0, 1);
            this._oneTimeAnalysisInfoPanel.Dock = System.Windows.Forms.DockStyle.Top;
            this._oneTimeAnalysisInfoPanel.Location = new System.Drawing.Point(0, 259);
            this._oneTimeAnalysisInfoPanel.Name = "_oneTimeAnalysisInfoPanel";
            this._oneTimeAnalysisInfoPanel.Padding = new System.Windows.Forms.Padding(0, 0, 10, 0);
            this._oneTimeAnalysisInfoPanel.RowCount = 3;
            this._oneTimeAnalysisInfoPanel.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this._oneTimeAnalysisInfoPanel.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this._oneTimeAnalysisInfoPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this._oneTimeAnalysisInfoPanel.Size = new System.Drawing.Size(536, 128);
            this._oneTimeAnalysisInfoPanel.TabIndex = 5;
            // 
            // _oneTimeAnalysisHeaderStrip
            // 
            this._oneTimeAnalysisHeaderStrip.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this._oneTimeAnalysisHeaderStrip.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this._oneTimeAnalysisHeaderStrip.ForeColor = System.Drawing.SystemColors.HotTrack;
            this._oneTimeAnalysisHeaderStrip.Location = new System.Drawing.Point(3, 3);
            this._oneTimeAnalysisHeaderStrip.Name = "_oneTimeAnalysisHeaderStrip";
            this._oneTimeAnalysisHeaderStrip.Size = new System.Drawing.Size(520, 25);
            this._oneTimeAnalysisHeaderStrip.TabIndex = 4;
            this._oneTimeAnalysisHeaderStrip.Text = "One time analysis is scheduled for server {0}";
            // 
            // _oneTimeAnalysisLinkLabel
            // 
            this._oneTimeAnalysisLinkLabel.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            appearance1.FontData.BoldAsString = "False";
            appearance1.FontData.ItalicAsString = "False";
            appearance1.FontData.SizeInPoints = 10F;
            appearance1.FontData.StrikeoutAsString = "False";
            appearance1.FontData.UnderlineAsString = "False";
            this._oneTimeAnalysisLinkLabel.Appearance = appearance1;
            this._oneTimeAnalysisLinkLabel.Location = new System.Drawing.Point(13, 34);
            this._oneTimeAnalysisLinkLabel.Margin = new System.Windows.Forms.Padding(13, 3, 3, 3);
            this._oneTimeAnalysisLinkLabel.Name = "_oneTimeAnalysisLinkLabel";
            this._oneTimeAnalysisLinkLabel.Size = new System.Drawing.Size(510, 89);
            this._oneTimeAnalysisLinkLabel.TabIndex = 0;
            this._oneTimeAnalysisLinkLabel.TabStop = true;
            this._oneTimeAnalysisLinkLabel.UseAppStyling = false;
            this._oneTimeAnalysisLinkLabel.UseOsThemes = Infragistics.Win.DefaultableBoolean.False;
            this._oneTimeAnalysisLinkLabel.Value = resources.GetString("_oneTimeAnalysisLinkLabel.Value");
            appearance2.ForeColor = System.Drawing.Color.Blue;
            this._oneTimeAnalysisLinkLabel.VisitedLinkAppearance = appearance2;
            this._oneTimeAnalysisLinkLabel.LinkClicked += new Infragistics.Win.FormattedLinkLabel.LinkClickedEventHandler(this._oneTimeAnalysisLinkLabel_LinkClicked);
            // 
            // _scheduleSettingsPanel
            // 
            this._scheduleSettingsPanel.ColumnCount = 2;
            this._scheduleSettingsPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this._scheduleSettingsPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this._scheduleSettingsPanel.Controls.Add(this._recurrenceComboBox, 1, 2);
            this._scheduleSettingsPanel.Controls.Add(this.panel1, 1, 4);
            this._scheduleSettingsPanel.Controls.Add(this._weeklyRecurrencePanel, 1, 3);
            this._scheduleSettingsPanel.Controls.Add(this.label1, 0, 0);
            this._scheduleSettingsPanel.Controls.Add(this.panel3, 1, 5);
            this._scheduleSettingsPanel.Controls.Add(this._serverNameComboBox, 1, 0);
            this._scheduleSettingsPanel.Controls.Add(this.label2, 0, 2);
            this._scheduleSettingsPanel.Controls.Add(this.label3, 0, 4);
            this._scheduleSettingsPanel.Controls.Add(this.label6, 0, 5);
            this._scheduleSettingsPanel.Dock = System.Windows.Forms.DockStyle.Top;
            this._scheduleSettingsPanel.Enabled = false;
            this._scheduleSettingsPanel.Location = new System.Drawing.Point(0, 101);
            this._scheduleSettingsPanel.Name = "_scheduleSettingsPanel";
            this._scheduleSettingsPanel.Padding = new System.Windows.Forms.Padding(15, 5, 5, 5);
            this._scheduleSettingsPanel.RowCount = 7;
            this._scheduleSettingsPanel.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this._scheduleSettingsPanel.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this._scheduleSettingsPanel.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this._scheduleSettingsPanel.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this._scheduleSettingsPanel.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this._scheduleSettingsPanel.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this._scheduleSettingsPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this._scheduleSettingsPanel.Size = new System.Drawing.Size(536, 158);
            this._scheduleSettingsPanel.TabIndex = 2;
            // 
            // _recurrenceComboBox
            // 
            this._recurrenceComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this._recurrenceComboBox.FormattingEnabled = true;
            this._recurrenceComboBox.Items.AddRange(new object[] {
            "Daily",
            "Weekly"});
            this._recurrenceComboBox.Location = new System.Drawing.Point(96, 35);
            this._recurrenceComboBox.Name = "_recurrenceComboBox";
            this._recurrenceComboBox.Size = new System.Drawing.Size(252, 21);
            this._recurrenceComboBox.TabIndex = 1;
            this._recurrenceComboBox.SelectedIndexChanged += new System.EventHandler(this._recurrenceComboBox_SelectedIndexChanged);
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.label4);
            this.panel1.Controls.Add(this._startTimePicker);
            this.panel1.Location = new System.Drawing.Point(95, 90);
            this.panel1.Margin = new System.Windows.Forms.Padding(2);
            this.panel1.Name = "panel1";
            this.panel1.Padding = new System.Windows.Forms.Padding(0, 4, 0, 0);
            this.panel1.Size = new System.Drawing.Size(389, 24);
            this.panel1.TabIndex = 12;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(100, 7);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(247, 13);
            this.label4.TabIndex = 6;
            this.label4.Text = "(per local time of the computer running SQL doctor)";
            this.label4.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // _startTimePicker
            // 
            this._startTimePicker.CustomFormat = "";
            this._startTimePicker.Dock = System.Windows.Forms.DockStyle.Left;
            this._startTimePicker.Format = System.Windows.Forms.DateTimePickerFormat.Time;
            this._startTimePicker.Location = new System.Drawing.Point(0, 4);
            this._startTimePicker.Name = "_startTimePicker";
            this._startTimePicker.ShowUpDown = true;
            this._startTimePicker.Size = new System.Drawing.Size(94, 20);
            this._startTimePicker.TabIndex = 0;
            this._startTimePicker.Value = new System.DateTime(2010, 4, 22, 8, 0, 0, 0);
            this._startTimePicker.ValueChanged += new System.EventHandler(this.SettingChanged);
            // 
            // _weeklyRecurrencePanel
            // 
            this._weeklyRecurrencePanel.Controls.Add(this._sundayCheckBox);
            this._weeklyRecurrencePanel.Controls.Add(this._saturdayCheckBox);
            this._weeklyRecurrencePanel.Controls.Add(this._fridayCheckBox);
            this._weeklyRecurrencePanel.Controls.Add(this._thursdayCheckBox);
            this._weeklyRecurrencePanel.Controls.Add(this._wednesdayCheckBox);
            this._weeklyRecurrencePanel.Controls.Add(this._tuesdayCheckBox);
            this._weeklyRecurrencePanel.Controls.Add(this._mondayCheckBox);
            this._weeklyRecurrencePanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this._weeklyRecurrencePanel.Location = new System.Drawing.Point(96, 62);
            this._weeklyRecurrencePanel.Name = "_weeklyRecurrencePanel";
            this._weeklyRecurrencePanel.Size = new System.Drawing.Size(563, 23);
            this._weeklyRecurrencePanel.TabIndex = 13;
            // 
            // _sundayCheckBox
            // 
            this._sundayCheckBox.AutoSize = true;
            this._sundayCheckBox.Dock = System.Windows.Forms.DockStyle.Left;
            this._sundayCheckBox.Location = new System.Drawing.Point(265, 0);
            this._sundayCheckBox.Name = "_sundayCheckBox";
            this._sundayCheckBox.Size = new System.Drawing.Size(45, 23);
            this._sundayCheckBox.TabIndex = 6;
            this._sundayCheckBox.Text = "Sun";
            this._sundayCheckBox.UseVisualStyleBackColor = true;
            this._sundayCheckBox.CheckedChanged += new System.EventHandler(this.SettingChanged);
            // 
            // _saturdayCheckBox
            // 
            this._saturdayCheckBox.AutoSize = true;
            this._saturdayCheckBox.Dock = System.Windows.Forms.DockStyle.Left;
            this._saturdayCheckBox.Location = new System.Drawing.Point(223, 0);
            this._saturdayCheckBox.Name = "_saturdayCheckBox";
            this._saturdayCheckBox.Size = new System.Drawing.Size(42, 23);
            this._saturdayCheckBox.TabIndex = 5;
            this._saturdayCheckBox.Text = "Sat";
            this._saturdayCheckBox.UseVisualStyleBackColor = true;
            this._saturdayCheckBox.CheckedChanged += new System.EventHandler(this.SettingChanged);
            // 
            // _fridayCheckBox
            // 
            this._fridayCheckBox.AutoSize = true;
            this._fridayCheckBox.Dock = System.Windows.Forms.DockStyle.Left;
            this._fridayCheckBox.Location = new System.Drawing.Point(186, 0);
            this._fridayCheckBox.Name = "_fridayCheckBox";
            this._fridayCheckBox.Size = new System.Drawing.Size(37, 23);
            this._fridayCheckBox.TabIndex = 4;
            this._fridayCheckBox.Text = "Fri";
            this._fridayCheckBox.UseVisualStyleBackColor = true;
            this._fridayCheckBox.CheckedChanged += new System.EventHandler(this.SettingChanged);
            // 
            // _thursdayCheckBox
            // 
            this._thursdayCheckBox.AutoSize = true;
            this._thursdayCheckBox.Dock = System.Windows.Forms.DockStyle.Left;
            this._thursdayCheckBox.Location = new System.Drawing.Point(141, 0);
            this._thursdayCheckBox.Name = "_thursdayCheckBox";
            this._thursdayCheckBox.Size = new System.Drawing.Size(45, 23);
            this._thursdayCheckBox.TabIndex = 3;
            this._thursdayCheckBox.Text = "Thu";
            this._thursdayCheckBox.UseVisualStyleBackColor = true;
            this._thursdayCheckBox.CheckedChanged += new System.EventHandler(this.SettingChanged);
            // 
            // _wednesdayCheckBox
            // 
            this._wednesdayCheckBox.AutoSize = true;
            this._wednesdayCheckBox.Dock = System.Windows.Forms.DockStyle.Left;
            this._wednesdayCheckBox.Location = new System.Drawing.Point(92, 0);
            this._wednesdayCheckBox.Name = "_wednesdayCheckBox";
            this._wednesdayCheckBox.Size = new System.Drawing.Size(49, 23);
            this._wednesdayCheckBox.TabIndex = 2;
            this._wednesdayCheckBox.Text = "Wed";
            this._wednesdayCheckBox.UseVisualStyleBackColor = true;
            this._wednesdayCheckBox.CheckedChanged += new System.EventHandler(this.SettingChanged);
            // 
            // _tuesdayCheckBox
            // 
            this._tuesdayCheckBox.AutoSize = true;
            this._tuesdayCheckBox.Dock = System.Windows.Forms.DockStyle.Left;
            this._tuesdayCheckBox.Location = new System.Drawing.Point(47, 0);
            this._tuesdayCheckBox.Name = "_tuesdayCheckBox";
            this._tuesdayCheckBox.Size = new System.Drawing.Size(45, 23);
            this._tuesdayCheckBox.TabIndex = 1;
            this._tuesdayCheckBox.Text = "Tue";
            this._tuesdayCheckBox.UseVisualStyleBackColor = true;
            this._tuesdayCheckBox.CheckedChanged += new System.EventHandler(this.SettingChanged);
            // 
            // _mondayCheckBox
            // 
            this._mondayCheckBox.AutoSize = true;
            this._mondayCheckBox.Dock = System.Windows.Forms.DockStyle.Left;
            this._mondayCheckBox.Location = new System.Drawing.Point(0, 0);
            this._mondayCheckBox.Name = "_mondayCheckBox";
            this._mondayCheckBox.Size = new System.Drawing.Size(47, 23);
            this._mondayCheckBox.TabIndex = 0;
            this._mondayCheckBox.Text = "Mon";
            this._mondayCheckBox.UseVisualStyleBackColor = true;
            this._mondayCheckBox.CheckedChanged += new System.EventHandler(this.SettingChanged);
            // 
            // label1
            // 
            this.label1.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(18, 12);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(72, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "Server Name:";
            // 
            // _serverNameComboBox
            // 
            this._serverNameComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this._serverNameComboBox.FormattingEnabled = true;
            this._serverNameComboBox.Location = new System.Drawing.Point(96, 8);
            this._serverNameComboBox.Name = "_serverNameComboBox";
            this._serverNameComboBox.Size = new System.Drawing.Size(252, 21);
            this._serverNameComboBox.TabIndex = 0;
            this._serverNameComboBox.SelectedIndexChanged += new System.EventHandler(this.SettingChanged);
            // 
            // label2
            // 
            this.label2.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(18, 39);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(66, 13);
            this.label2.TabIndex = 3;
            this.label2.Text = "Recurrence:";
            // 
            // label3
            // 
            this.label3.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(18, 95);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(58, 13);
            this.label3.TabIndex = 3;
            this.label3.Text = "Start Time:";
            // 
            // _lastRunStatusLabel
            // 
            this._lastRunStatusLabel.AutoSize = true;
            this._lastRunStatusLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this._lastRunStatusLabel.Location = new System.Drawing.Point(18, 19);
            this._lastRunStatusLabel.Margin = new System.Windows.Forms.Padding(3, 7, 3, 0);
            this._lastRunStatusLabel.MaximumSize = new System.Drawing.Size(0, 60);
            this._lastRunStatusLabel.MinimumSize = new System.Drawing.Size(0, 13);
            this._lastRunStatusLabel.Name = "_lastRunStatusLabel";
            this._lastRunStatusLabel.Size = new System.Drawing.Size(103, 13);
            this._lastRunStatusLabel.TabIndex = 5;
            this._lastRunStatusLabel.Text = "Last Run Status: {0}";
            // 
            // _scheduleEnabledCheckBox
            // 
            this._scheduleEnabledCheckBox.AutoSize = true;
            this._scheduleEnabledCheckBox.Location = new System.Drawing.Point(21, 15);
            this._scheduleEnabledCheckBox.Name = "_scheduleEnabledCheckBox";
            this._scheduleEnabledCheckBox.Size = new System.Drawing.Size(221, 17);
            this._scheduleEnabledCheckBox.TabIndex = 0;
            this._scheduleEnabledCheckBox.Text = "Run a routine checkup of my SQL Server";
            this._scheduleEnabledCheckBox.UseVisualStyleBackColor = false;
            this._scheduleEnabledCheckBox.CheckedChanged += new System.EventHandler(this._scheduleEnabledCheckBox_CheckedChanged);
            // 
            // panelHeader
            // 
            this.panelHeader.BackColor = System.Drawing.Color.White;
            this.panelHeader.Controls.Add(this.multiGradientPanel1);
            this.panelHeader.Controls.Add(this.label5);
            this.panelHeader.Controls.Add(this.lblSQLie);
            this.panelHeader.Controls.Add(this.pictureHeader);
            this.panelHeader.Dock = System.Windows.Forms.DockStyle.Top;
            this.panelHeader.GradientColor = System.Drawing.Color.White;
            this.panelHeader.Location = new System.Drawing.Point(0, 0);
            this.panelHeader.Name = "panelHeader";
            this.panelHeader.Rotation = 0F;
            this.panelHeader.Size = new System.Drawing.Size(536, 61);
            this.panelHeader.TabIndex = 6;
            // 
            // multiGradientPanel1
            // 
            this.multiGradientPanel1.BackColor = System.Drawing.SystemColors.ActiveCaptionText;
            this.multiGradientPanel1.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.multiGradientPanel1.GradientColors = new System.Drawing.Color[] {
        System.Drawing.Color.FromArgb(((int)(((byte)(221)))), ((int)(((byte)(226)))), ((int)(((byte)(236))))),
        System.Drawing.Color.FromArgb(((int)(((byte)(221)))), ((int)(((byte)(226)))), ((int)(((byte)(236))))),
        System.Drawing.Color.White};
            this.multiGradientPanel1.Location = new System.Drawing.Point(0, 58);
            this.multiGradientPanel1.Name = "multiGradientPanel1";
            this.multiGradientPanel1.Rotation = 0F;
            this.multiGradientPanel1.Size = new System.Drawing.Size(536, 3);
            this.multiGradientPanel1.TabIndex = 4;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.BackColor = System.Drawing.Color.Transparent;
            this.label5.Font = new System.Drawing.Font("Arial", 20.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label5.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(75)))), ((int)(((byte)(108)))), ((int)(((byte)(153)))));
            this.label5.Location = new System.Drawing.Point(60, 19);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(253, 32);
            this.label5.TabIndex = 3;
            this.label5.Text = "Schedule analysis";
            // 
            // lblSQLie
            // 
            this.lblSQLie.AutoSize = true;
            this.lblSQLie.BackColor = System.Drawing.Color.Transparent;
            this.lblSQLie.Font = new System.Drawing.Font("Arial", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblSQLie.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(75)))), ((int)(((byte)(108)))), ((int)(((byte)(153)))));
            this.lblSQLie.Location = new System.Drawing.Point(60, 5);
            this.lblSQLie.Name = "lblSQLie";
            this.lblSQLie.Size = new System.Drawing.Size(89, 14);
            this.lblSQLie.TabIndex = 2;
            this.lblSQLie.Text = "Idera SQL doctor";
            // 
            // pictureHeader
            // 
            this.pictureHeader.BackColor = System.Drawing.Color.Transparent;
            this.pictureHeader.Image = global::Idera.SQLdoctor.StandardClient.Properties.Resources.ScheduledCheckup48;
            this.pictureHeader.Location = new System.Drawing.Point(6, 5);
            this.pictureHeader.Name = "pictureHeader";
            this.pictureHeader.Size = new System.Drawing.Size(48, 48);
            this.pictureHeader.TabIndex = 0;
            this.pictureHeader.TabStop = false;
            // 
            // panel2
            // 
            this.panel2.Controls.Add(this._scheduleEnabledCheckBox);
            this.panel2.Dock = System.Windows.Forms.DockStyle.Top;
            this.panel2.Location = new System.Drawing.Point(0, 61);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(536, 40);
            this.panel2.TabIndex = 7;
            // 
            // btnCancel
            // 
            this.btnCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnCancel.Location = new System.Drawing.Point(449, 14);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(75, 23);
            this.btnCancel.TabIndex = 1;
            this.btnCancel.Text = "&Cancel";
            this.btnCancel.UseVisualStyleBackColor = true;
            // 
            // btnOK
            // 
            this.btnOK.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnOK.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.btnOK.Location = new System.Drawing.Point(368, 14);
            this.btnOK.Name = "btnOK";
            this.btnOK.Size = new System.Drawing.Size(75, 23);
            this.btnOK.TabIndex = 0;
            this.btnOK.Text = "&OK";
            this.btnOK.UseVisualStyleBackColor = true;
            this.btnOK.Click += new System.EventHandler(this.btnOK_Click);
            // 
            // panelButtons
            // 
            this.panelButtons.Controls.Add(this.lblEtched);
            this.panelButtons.Controls.Add(this.btnCancel);
            this.panelButtons.Controls.Add(this.btnOK);
            this.panelButtons.Controls.Add(this._lastRunStatusLabel);
            this.panelButtons.Dock = System.Windows.Forms.DockStyle.Top;
            this.panelButtons.Location = new System.Drawing.Point(0, 387);
            this.panelButtons.Name = "panelButtons";
            this.panelButtons.Size = new System.Drawing.Size(536, 48);
            this.panelButtons.TabIndex = 10;
            // 
            // lblEtched
            // 
            this.lblEtched.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.lblEtched.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.lblEtched.Location = new System.Drawing.Point(10, 3);
            this.lblEtched.Name = "lblEtched";
            this.lblEtched.Size = new System.Drawing.Size(513, 1);
            this.lblEtched.TabIndex = 10;
            // 
            // NumericAnalysisDuration
            // 
            this.NumericAnalysisDuration.Dock = System.Windows.Forms.DockStyle.Left;
            this.NumericAnalysisDuration.Location = new System.Drawing.Point(0, 4);
            this.NumericAnalysisDuration.Maximum = new decimal(new int[] {
            500,
            0,
            0,
            0});
            this.NumericAnalysisDuration.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.NumericAnalysisDuration.Name = "NumericAnalysisDuration";
            this.NumericAnalysisDuration.Size = new System.Drawing.Size(57, 20);
            this.NumericAnalysisDuration.TabIndex = 16;
            this.NumericAnalysisDuration.Value = new decimal(new int[] {
            5,
            0,
            0,
            0});
            this.NumericAnalysisDuration.ValueChanged += new System.EventHandler(this.NumericAnalysisDuration_ValueChanged);
            this.NumericAnalysisDuration.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.NumericAnalysisDuration_KeyPress);
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(61, 6);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(85, 13);
            this.label7.TabIndex = 17;
            this.label7.Text = "minutes (1 - 500)";
            this.label7.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // panel3
            // 
            this.panel3.Controls.Add(this.label7);
            this.panel3.Controls.Add(this.NumericAnalysisDuration);
            this.panel3.Location = new System.Drawing.Point(96, 119);
            this.panel3.Name = "panel3";
            this.panel3.Padding = new System.Windows.Forms.Padding(0, 4, 0, 0);
            this.panel3.Size = new System.Drawing.Size(252, 24);
            this.panel3.TabIndex = 15;
            // 
            // label6
            // 
            this.label6.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(18, 124);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(50, 13);
            this.label6.TabIndex = 14;
            this.label6.Text = "Duration:";
            // 
            // ScheduleSettingsDialog
            // 
            this.AcceptButton = this.btnOK;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.White;
            this.CancelButton = this.btnCancel;
            this.ClientSize = new System.Drawing.Size(536, 436);
            this.Controls.Add(this.panelButtons);
            this.Controls.Add(this._oneTimeAnalysisInfoPanel);
            this.Controls.Add(this._scheduleSettingsPanel);
            this.Controls.Add(this.panel2);
            this.Controls.Add(this.panelHeader);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.HelpButton = true;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "ScheduleSettingsDialog";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Schedule analysis";
            this.Load += new System.EventHandler(this.ScheduleSettingsDialog_Load);
            this.HelpButtonClicked += new System.ComponentModel.CancelEventHandler(this.ScheduleSettingsDialog_HelpButtonClicked);
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.ScheduleSettingsDialog_FormClosing);
            this.HelpRequested += new System.Windows.Forms.HelpEventHandler(this.ScheduleSettingsDialog_HelpRequested);
            this._oneTimeAnalysisInfoPanel.ResumeLayout(false);
            this._scheduleSettingsPanel.ResumeLayout(false);
            this._scheduleSettingsPanel.PerformLayout();
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this._weeklyRecurrencePanel.ResumeLayout(false);
            this._weeklyRecurrencePanel.PerformLayout();
            this.panelHeader.ResumeLayout(false);
            this.panelHeader.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureHeader)).EndInit();
            this.panel2.ResumeLayout(false);
            this.panel2.PerformLayout();
            this.panelButtons.ResumeLayout(false);
            this.panelButtons.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.NumericAnalysisDuration)).EndInit();
            this.panel3.ResumeLayout(false);
            this.panel3.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.CheckBox _scheduleEnabledCheckBox;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.ComboBox _serverNameComboBox;
        private System.Windows.Forms.ComboBox _recurrenceComboBox;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.CheckBox _saturdayCheckBox;
        private System.Windows.Forms.CheckBox _fridayCheckBox;
        private System.Windows.Forms.CheckBox _thursdayCheckBox;
        private System.Windows.Forms.CheckBox _wednesdayCheckBox;
        private System.Windows.Forms.CheckBox _tuesdayCheckBox;
        private System.Windows.Forms.CheckBox _mondayCheckBox;
        private System.Windows.Forms.CheckBox _sundayCheckBox;
        private System.Windows.Forms.DateTimePicker _startTimePicker;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Panel _weeklyRecurrencePanel;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.TableLayoutPanel _scheduleSettingsPanel;
        private Infragistics.Win.FormattedLinkLabel.UltraFormattedLinkLabel _oneTimeAnalysisLinkLabel;
        private Idera.SQLdoctor.CommonGUI.Controls.Headers.PropertiesHeaderStrip _oneTimeAnalysisHeaderStrip;
        private System.Windows.Forms.TableLayoutPanel _oneTimeAnalysisInfoPanel;
        private System.Windows.Forms.Label _lastRunStatusLabel;
        private Idera.SQLdoctor.CommonGUI.Controls.Panels.GradientPanelWithoutBorder panelHeader;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label lblSQLie;
        private System.Windows.Forms.PictureBox pictureHeader;
        private Idera.SQLdoctor.CommonGUI.Controls.Panels.MultiGradientPanel multiGradientPanel1;
        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.Button btnOK;
        private System.Windows.Forms.Panel panelButtons;
        private System.Windows.Forms.Label lblEtched;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Panel panel3;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.NumericUpDown NumericAnalysisDuration;
    }
}
