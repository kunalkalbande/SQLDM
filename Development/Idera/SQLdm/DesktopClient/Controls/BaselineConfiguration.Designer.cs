using Idera.SQLdm.DesktopClient.Properties;
using System.Drawing;
using System.Windows.Forms;

namespace Idera.SQLdm.DesktopClient.Controls
{
    partial class BaselineConfigurationPage
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
            Color backColor = Settings.Default.ColorScheme == "Dark" ? ColorTranslator.FromHtml(DarkThemeColorConstants.UltraGridBackColor) : Color.White;
            Color foreColor = Settings.Default.ColorScheme == "Dark" ? ColorTranslator.FromHtml(DarkThemeColorConstants.UltraGridForeColor) : Color.Black;
            Color activeBackColor = Settings.Default.ColorScheme == "Dark" ? ColorTranslator.FromHtml(DarkThemeColorConstants.UltraGridActiveBackColor) : Color.White;
            Color hoverBackColor = Settings.Default.ColorScheme == "Dark" ? ColorTranslator.FromHtml(DarkThemeColorConstants.UltraGridHoverBackColor) : Color.White;
            Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomDropDownEditorButton dropDownEditorButton3 = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomDropDownEditorButton("DropDownList");
            Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomDropDownEditorButton dropDownEditorButton4 = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomDropDownEditorButton("DropDownList");
            Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomDropDownEditorButton dropDownEditorButton1 = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomDropDownEditorButton("DropDownList");
            Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomDropDownEditorButton dropDownEditorButton2 = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomDropDownEditorButton("DropDownList");
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(BaselineConfigurationPage));
            Infragistics.Win.UltraWinSchedule.CalendarCombo.DateButton dateButton1 = new Infragistics.Win.UltraWinSchedule.CalendarCombo.DateButton();
            Infragistics.Win.UltraWinSchedule.CalendarCombo.DateButton dateButton2 = new Infragistics.Win.UltraWinSchedule.CalendarCombo.DateButton();
            //this.label5 = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomLabel();
            //this.label6 = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomLabel();
            bool isDarkThemeSelected = Settings.Default.ColorScheme == "Dark";
            this.label7 = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomLabel();
            this.label8 = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomLabel();
            this.saveButton = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomButton();
            this.addButton = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomButton();
            this.cancelButton = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomButton();
            this.beginTimeCombo1 = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomTimeComboEditor();
            this.endTimeCombo1 = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomTimeComboEditor();
            this.timeDateContainer1 = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomFlowLayoutPanel();
            this.daysChkContainer1 = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomTableLayoutPanel();
            //this.sundayCheckbox1 = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomCheckBox();
            //this.mondayCheckBox1 = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomCheckBox();
            //this.tuesdayCheckBox1 = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomCheckBox();
            //this.wednesdayCheckBox1 = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomCheckBox();
            //this.thursdayCheckBox1 = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomCheckBox();
            //this.fridayCheckBox1 = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomCheckBox();
            //this.saturdayCheckBox1 = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomCheckBox();
            //this.comboBox1 = new System.Windows.Forms.ComboBox();
            this.office2007PropertyPage1 = new Idera.SQLdm.DesktopClient.Controls.Office2007PropertyPage(isDarkThemeSelected);
            this.baselineMainContainer = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomTableLayoutPanel();
            //this.informationBox2 = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomInformationBox();
            this.panel1 = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomPanel ();
            this.header1label = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomLabel();
            this.btnSelectOtherServers = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomButton();
            //this.textBox1 = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomTextBox();
            //this.label1 = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomLabel();
            this.timeDateContainer = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomFlowLayoutPanel();
            this.label3 = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomLabel();
            this.beginTimeCombo = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomTimeComboEditor();
            this.label4 = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomLabel();
            this.endTimeCombo = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomTimeComboEditor();
            this.informationBox1 = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomInformationBox();
            this.daysChkContainer = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomTableLayoutPanel();
            this.sundayCheckbox = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomCheckBox();
            this.mondayCheckBox = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomCheckBox();
            this.tuesdayCheckBox = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomCheckBox();
            this.wednesdayCheckBox = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomCheckBox();
            this.thursdayCheckBox = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomCheckBox();
            this.fridayCheckBox = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomCheckBox();
            this.saturdayCheckBox = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomCheckBox();
            this.automaticBaselineRadioButton = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomRadioButton();
            this.customDateMainContainer = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomFlowLayoutPanel();
            this.customDateFromLabel = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomLabel();
            this.customDateFromCombo = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomUltraCalendarCombo();
            this.customDateToLabel = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomLabel();
            this.customDateToCombo = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomUltraCalendarCombo();
           // this.btnBaseLineAssistant = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomButton(); // SQLdm 10.1 (Pulkit Puri)
            this.customBaselineRadioButton = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomRadioButton();
            this.panel2 = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomPanel ();
            this.header2label = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomLabel();
            //this.panel4 = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomPanel ();
            //this.header4label = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomLabel();
            this.panel3 = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomPanel ();
            this.header3label = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomLabel();
            this.headerStrip1 = new Idera.SQLdm.DesktopClient.Controls.PropertiesHeaderStrip();
            this.propertiesHeaderStrip1 = new Idera.SQLdm.DesktopClient.Controls.PropertiesHeaderStrip();
            //this.propertiesHeaderStrip2 = new Idera.SQLdm.DesktopClient.Controls.PropertiesHeaderStrip();
            this.propertiesHeaderStripSelectServers = new Idera.SQLdm.DesktopClient.Controls.PropertiesHeaderStrip();
            ((System.ComponentModel.ISupportInitialize)(this.beginTimeCombo1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.endTimeCombo1)).BeginInit();
            this.timeDateContainer1.SuspendLayout();
            this.daysChkContainer1.SuspendLayout();
            this.baselineMainContainer.SuspendLayout();
            this.panel1.SuspendLayout();
            this.timeDateContainer.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.beginTimeCombo)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.endTimeCombo)).BeginInit();
            this.daysChkContainer.SuspendLayout();
            this.customDateMainContainer.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.customDateFromCombo)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.customDateToCombo)).BeginInit();
            this.panel2.SuspendLayout();
            //this.panel4.SuspendLayout();
            this.panel3.SuspendLayout();
            this.SuspendLayout();
            //// 
            //// label5
            //// 
            //this.label5.Anchor = System.Windows.Forms.AnchorStyles.Left;
            //this.label5.AutoSize = true;
            //this.label5.Location = new System.Drawing.Point(3, 9);
            //this.label5.Margin = new System.Windows.Forms.Padding(3, 5, 3, 5);
            //this.label5.Name = "label5";
            //this.label5.Size = new System.Drawing.Size(33, 13);
            //this.label5.TabIndex = 82;
            //this.label5.Text = "From:";
            //// 
            //// label6
            //// 
            //this.label6.Anchor = System.Windows.Forms.AnchorStyles.Left;
            //this.label6.AutoSize = true;
            //this.label6.Location = new System.Drawing.Point(136, 9);
            //this.label6.Margin = new System.Windows.Forms.Padding(3, 5, 3, 5);
            //this.label6.Name = "label6";
            //this.label6.Size = new System.Drawing.Size(23, 13);
            //this.label6.TabIndex = 81;
            //this.label6.Text = "To:";
            // 
            // label7
            // 
            this.label7.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.label7.AutoSize = true;
            this.baselineMainContainer.SetColumnSpan(this.label7, 3);
            this.label7.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.label7.Location = new System.Drawing.Point(3, 210);
            this.label7.Margin = new System.Windows.Forms.Padding(3, 5, 3, 5);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(74, 13);
            this.label7.TabIndex = 85;
            this.label7.Text = "Baseline Name";
            // 
            // label8
            // 
            this.label8.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.label8.AutoSize = true;
            this.label8.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.label8.Location = new System.Drawing.Point(83, 210);
            this.label8.Margin = new System.Windows.Forms.Padding(3, 5, 3, 5);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(41, 13);
            this.label8.TabIndex = 85;
            this.label8.Text = Common.Constants.DEFAULT_BASELINE_NAME;
            // 
            // saveButton
            // 
            this.saveButton.AutoSize = true;
            this.saveButton.BackColor = System.Drawing.SystemColors.Control;
            this.saveButton.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.saveButton.Location = new System.Drawing.Point(33, 200);
            this.saveButton.Margin = new System.Windows.Forms.Padding(3, 15, 3, 3);
            this.saveButton.Name = "saveButton";
            this.saveButton.Size = new System.Drawing.Size(75, 25);
            this.saveButton.TabIndex = 66;
            this.saveButton.Text = "Save";
            this.saveButton.UseVisualStyleBackColor = false;
            // 
            // addButton
            // 
			
			 this.addButton.AutoSize = true;
            //this.addButton.BackColor = System.Drawing.SystemColors.Control;
            this.baselineMainContainer.SetColumnSpan(this.addButton, 2);
            //this.addButton.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.addButton.Location = new System.Drawing.Point(259,4); // SQLDM 10.1 (Pulkit Puri)--changing coordinates
            this.addButton.Name = "addButton";
            this.addButton.Size = new System.Drawing.Size(108,23);// SQLDM 10.1 (Pulkit Puri)--changing size
            this.addButton.TabIndex = 86;
            this.addButton.Text = "Manage Baseline";
            this.addButton.UseVisualStyleBackColor = true;
            this.addButton.Click += new System.EventHandler(this.addButton_Click);
            // 
            // cancelButton
            // 
            this.cancelButton.AutoSize = true;
            this.cancelButton.BackColor = System.Drawing.SystemColors.Control;
            this.cancelButton.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.cancelButton.Location = new System.Drawing.Point(133, 200);
            this.cancelButton.Margin = new System.Windows.Forms.Padding(3, 15, 3, 3);
            this.cancelButton.Name = "cancelButton";
            this.cancelButton.Size = new System.Drawing.Size(75, 25);
            this.cancelButton.TabIndex = 83;
            this.cancelButton.Text = "Cancel";
            this.cancelButton.UseVisualStyleBackColor = false;
            // 
            // beginTimeCombo1
            // 
            this.beginTimeCombo1.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.beginTimeCombo1.AutoFillTime = Infragistics.Win.UltraWinMaskedEdit.AutoFillTime.Midnight;
            dropDownEditorButton3.Key = "DropDownList";
            dropDownEditorButton3.PreferredDropDownSize = new System.Drawing.Size(0, 0);
            this.beginTimeCombo1.ButtonsRight.Add(dropDownEditorButton3);
            this.beginTimeCombo1.DateTime = new System.DateTime(2008, 8, 1, 8, 0, 0, 0);
            this.beginTimeCombo1.DropDownButtonDisplayStyle = Infragistics.Win.ButtonDisplayStyle.Never;
            this.beginTimeCombo1.ListInterval = System.TimeSpan.Parse("00:30:00");
            this.beginTimeCombo1.Location = new System.Drawing.Point(42, 5);
            this.beginTimeCombo1.Margin = new System.Windows.Forms.Padding(3, 5, 3, 5);
            this.beginTimeCombo1.MaskInput = "{time}";
            this.beginTimeCombo1.Name = "beginTimeCombo1";
            this.beginTimeCombo1.Nullable = false;
            this.beginTimeCombo1.Size = new System.Drawing.Size(88, 21);
            this.beginTimeCombo1.TabIndex = 79;
            this.beginTimeCombo1.Time = System.TimeSpan.Parse("08:00:00");
            this.beginTimeCombo1.Value = new System.DateTime(2008, 8, 1, 8, 0, 0, 0);
            this.beginTimeCombo1.ValueChanged += new System.EventHandler(this.Update);
            // 
            // endTimeCombo1
            // 
            this.endTimeCombo1.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.endTimeCombo1.AutoFillTime = Infragistics.Win.UltraWinMaskedEdit.AutoFillTime.Midnight;
            dropDownEditorButton4.Key = "DropDownList";
            dropDownEditorButton4.PreferredDropDownSize = new System.Drawing.Size(0, 0);
            this.endTimeCombo1.ButtonsRight.Add(dropDownEditorButton4);
            this.endTimeCombo1.DateTime = new System.DateTime(2008, 8, 1, 17, 0, 0, 0);
            this.endTimeCombo1.DropDownButtonDisplayStyle = Infragistics.Win.ButtonDisplayStyle.Never;
            this.endTimeCombo1.ListInterval = System.TimeSpan.Parse("00:30:00");
            this.endTimeCombo1.Location = new System.Drawing.Point(165, 5);
            this.endTimeCombo1.Margin = new System.Windows.Forms.Padding(3, 5, 3, 5);
            this.endTimeCombo1.MaskInput = "{time}";
            this.endTimeCombo1.Name = "endTimeCombo1";
            this.endTimeCombo1.Nullable = false;
            this.endTimeCombo1.Size = new System.Drawing.Size(88, 21);
            this.endTimeCombo1.TabIndex = 80;
            this.endTimeCombo1.Time = System.TimeSpan.Parse("17:00:00");
            this.endTimeCombo1.Value = new System.DateTime(2008, 8, 1, 17, 0, 0, 0);
            this.endTimeCombo1.ValueChanged += new System.EventHandler(this.Update);
            //// 
            //// timeDateContainer1
            //// 
            //this.timeDateContainer1.Controls.Add(this.label5);
            //this.timeDateContainer1.Controls.Add(this.beginTimeCombo1);
            //this.timeDateContainer1.Controls.Add(this.label6);
            //this.timeDateContainer1.Controls.Add(this.endTimeCombo1);
            //this.timeDateContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
            //this.timeDateContainer1.Location = new System.Drawing.Point(83, 441);
            //this.timeDateContainer1.Name = "timeDateContainer1";
            //this.timeDateContainer1.Size = new System.Drawing.Size(409, 40);
            //this.timeDateContainer1.TabIndex = 78;
            // 
            // daysChkContainer1
            // 
            this.daysChkContainer1.ColumnCount = 7;
            this.baselineMainContainer.SetColumnSpan(this.daysChkContainer1, 2);
            this.daysChkContainer1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.daysChkContainer1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.daysChkContainer1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.daysChkContainer1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.daysChkContainer1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.daysChkContainer1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.daysChkContainer1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            //this.daysChkContainer1.Controls.Add(this.sundayCheckbox1, 0, 0);
            //this.daysChkContainer1.Controls.Add(this.mondayCheckBox1, 1, 0);
            //this.daysChkContainer1.Controls.Add(this.tuesdayCheckBox1, 2, 0);
            //this.daysChkContainer1.Controls.Add(this.wednesdayCheckBox1, 3, 0);
            //this.daysChkContainer1.Controls.Add(this.thursdayCheckBox1, 4, 0);
            //this.daysChkContainer1.Controls.Add(this.fridayCheckBox1, 5, 0);
            //this.daysChkContainer1.Controls.Add(this.saturdayCheckBox1, 6, 0);
            this.daysChkContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.daysChkContainer1.Location = new System.Drawing.Point(33, 407);
            this.daysChkContainer1.Margin = new System.Windows.Forms.Padding(3, 3, 3, 5);
            this.daysChkContainer1.Name = "daysChkContainer1";
            this.daysChkContainer1.RowCount = 1;
            this.daysChkContainer1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.daysChkContainer1.Size = new System.Drawing.Size(459, 26);
            this.daysChkContainer1.TabIndex = 70;
            //// 
            //// sundayCheckbox1
            //// 
            //this.sundayCheckbox1.AutoSize = true;
            //this.sundayCheckbox1.Location = new System.Drawing.Point(3, 3);
            //this.sundayCheckbox1.Name = "sundayCheckbox1";
            //this.sundayCheckbox1.Size = new System.Drawing.Size(45, 17);
            //this.sundayCheckbox1.TabIndex = 71;
            //this.sundayCheckbox1.Text = "Sun";
            //this.sundayCheckbox1.UseVisualStyleBackColor = true;
            //this.sundayCheckbox1.CheckedChanged += new System.EventHandler(this.Update);
            //// 
            //// mondayCheckBox1
            //// 
            //this.mondayCheckBox1.AutoSize = true;
            //this.mondayCheckBox1.Checked = true;
            //this.mondayCheckBox1.CheckState = System.Windows.Forms.CheckState.Checked;
            //this.mondayCheckBox1.Location = new System.Drawing.Point(54, 3);
            //this.mondayCheckBox1.Name = "mondayCheckBox1";
            //this.mondayCheckBox1.Size = new System.Drawing.Size(47, 17);
            //this.mondayCheckBox1.TabIndex = 72;
            //this.mondayCheckBox1.Text = "Mon";
            //this.mondayCheckBox1.UseVisualStyleBackColor = true;
            //this.mondayCheckBox1.CheckedChanged += new System.EventHandler(this.Update);
            //// 
            //// tuesdayCheckBox1
            //// 
            //this.tuesdayCheckBox1.AutoSize = true;
            //this.tuesdayCheckBox1.Checked = true;
            //this.tuesdayCheckBox1.CheckState = System.Windows.Forms.CheckState.Checked;
            //this.tuesdayCheckBox1.Location = new System.Drawing.Point(107, 3);
            //this.tuesdayCheckBox1.Name = "tuesdayCheckBox1";
            //this.tuesdayCheckBox1.Size = new System.Drawing.Size(45, 17);
            //this.tuesdayCheckBox1.TabIndex = 73;
            //this.tuesdayCheckBox1.Text = "Tue";
            //this.tuesdayCheckBox1.UseVisualStyleBackColor = true;
            //this.tuesdayCheckBox1.CheckedChanged += new System.EventHandler(this.Update);
            //// 
            //// wednesdayCheckBox1
            //// 
            //this.wednesdayCheckBox1.AutoSize = true;
            //this.wednesdayCheckBox1.Checked = true;
            //this.wednesdayCheckBox1.CheckState = System.Windows.Forms.CheckState.Checked;
            //this.wednesdayCheckBox1.Location = new System.Drawing.Point(158, 3);
            //this.wednesdayCheckBox1.Name = "wednesdayCheckBox1";
            //this.wednesdayCheckBox1.Size = new System.Drawing.Size(49, 17);
            //this.wednesdayCheckBox1.TabIndex = 74;
            //this.wednesdayCheckBox1.Text = "Wed";
            //this.wednesdayCheckBox1.UseVisualStyleBackColor = true;
            //this.wednesdayCheckBox1.CheckedChanged += new System.EventHandler(this.Update);
            //// 
            //// thursdayCheckBox1
            //// 
            //this.thursdayCheckBox1.AutoSize = true;
            //this.thursdayCheckBox1.Checked = true;
            //this.thursdayCheckBox1.CheckState = System.Windows.Forms.CheckState.Checked;
            //this.thursdayCheckBox1.Location = new System.Drawing.Point(213, 3);
            //this.thursdayCheckBox1.Name = "thursdayCheckBox1";
            //this.thursdayCheckBox1.Size = new System.Drawing.Size(45, 17);
            //this.thursdayCheckBox1.TabIndex = 75;
            //this.thursdayCheckBox1.Text = "Thu";
            //this.thursdayCheckBox1.UseVisualStyleBackColor = true;
            //this.thursdayCheckBox1.CheckedChanged += new System.EventHandler(this.Update);
            //// 
            //// fridayCheckBox1
            //// 
            //this.fridayCheckBox1.AutoSize = true;
            //this.fridayCheckBox1.Checked = true;
            //this.fridayCheckBox1.CheckState = System.Windows.Forms.CheckState.Checked;
            //this.fridayCheckBox1.Location = new System.Drawing.Point(264, 3);
            //this.fridayCheckBox1.Name = "fridayCheckBox1";
            //this.fridayCheckBox1.Size = new System.Drawing.Size(37, 17);
            //this.fridayCheckBox1.TabIndex = 76;
            //this.fridayCheckBox1.Text = "Fri";
            //this.fridayCheckBox1.UseVisualStyleBackColor = true;
            //this.fridayCheckBox1.CheckedChanged += new System.EventHandler(this.Update);
            //// 
            //// saturdayCheckBox1
            //// 
            //this.saturdayCheckBox1.AutoSize = true;
            //this.saturdayCheckBox1.Location = new System.Drawing.Point(307, 3);
            //this.saturdayCheckBox1.Name = "saturdayCheckBox1";
            //this.saturdayCheckBox1.Size = new System.Drawing.Size(42, 17);
            //this.saturdayCheckBox1.TabIndex = 77;
            //this.saturdayCheckBox1.Text = "Sat";
            //this.saturdayCheckBox1.UseVisualStyleBackColor = true;
            //this.saturdayCheckBox1.CheckedChanged += new System.EventHandler(this.Update);
            //// 
            //// comboBox1
            //// 
            //this.comboBox1.Anchor = System.Windows.Forms.AnchorStyles.Left;
            //this.comboBox1.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            //this.comboBox1.FormattingEnabled = true;
            //this.comboBox1.IntegralHeight = false;
            //this.comboBox1.Location = new System.Drawing.Point(83, 501);
            //this.comboBox1.Margin = new System.Windows.Forms.Padding(3, 15, 3, 3);
            //this.comboBox1.MaxDropDownItems = 5;
            //this.comboBox1.Name = "comboBox1";
            //this.comboBox1.Size = new System.Drawing.Size(121, 21);
            //this.comboBox1.TabIndex = 87;
            //this.comboBox1.SelectedIndexChanged += new System.EventHandler(this.comboBox1_SelectedIndexChanged);
            // 
            // office2007PropertyPage1
            // 
            this.office2007PropertyPage1.BackColor = System.Drawing.Color.White;
            this.office2007PropertyPage1.BorderWidth = 1;
            // 
            // 
            // 
            this.office2007PropertyPage1.ContentPanel.BackColor2 = System.Drawing.Color.White;
            this.office2007PropertyPage1.ContentPanel.BorderColor = System.Drawing.SystemColors.ControlDark;
            this.office2007PropertyPage1.ContentPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.office2007PropertyPage1.ContentPanel.FillStyle = Idera.SQLdm.DesktopClient.Controls.GradientPanelFillStyle.Solid;
            this.office2007PropertyPage1.ContentPanel.Location = new System.Drawing.Point(1, 56);
            this.office2007PropertyPage1.ContentPanel.Name = "ContentPanel";
            this.office2007PropertyPage1.ContentPanel.Padding = new System.Windows.Forms.Padding(1);
            this.office2007PropertyPage1.ContentPanel.ShowBorder = false;
            this.office2007PropertyPage1.ContentPanel.Size = new System.Drawing.Size(503, 632);
            this.office2007PropertyPage1.ContentPanel.TabIndex = 2;
            this.office2007PropertyPage1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.office2007PropertyPage1.HeaderImage = global::Idera.SQLdm.DesktopClient.Properties.Resources.Chart32x32;
            this.office2007PropertyPage1.Location = new System.Drawing.Point(0, 0);
            this.office2007PropertyPage1.Name = "office2007PropertyPage1";
            this.office2007PropertyPage1.Size = new System.Drawing.Size(505, 689);
            this.office2007PropertyPage1.TabIndex = 0;
            this.office2007PropertyPage1.Text = "Configure a performance baseline for your monitored SQL Server.";
            // 
            // baselineMainContainer
            // 
            this.baselineMainContainer.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.baselineMainContainer.BackColor = System.Drawing.Color.White;
            this.baselineMainContainer.ColumnCount = 4;
            this.baselineMainContainer.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 0F));
            this.baselineMainContainer.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 30F));
            this.baselineMainContainer.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 50F));
            this.baselineMainContainer.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            //this.baselineMainContainer.Controls.Add(this.informationBox2, 1, 10);
            this.baselineMainContainer.Controls.Add(this.panel1, 0, 0);
            this.baselineMainContainer.Controls.Add(this.btnSelectOtherServers, 2, 12);
            //this.baselineMainContainer.Controls.Add(this.comboBox1, 3, 14);
            //this.baselineMainContainer.Controls.Add(this.addButton, 1, 16);
            //this.baselineMainContainer.Controls.Add(this.textBox1, 3, 11);
            //this.baselineMainContainer.Controls.Add(this.label1, 0, 11);
            this.baselineMainContainer.Controls.Add(this.label7, 0, 6);
            this.baselineMainContainer.Controls.Add(this.label8, 3, 6);
            this.baselineMainContainer.Controls.Add(this.timeDateContainer, 3, 8);
            this.baselineMainContainer.Controls.Add(this.timeDateContainer1, 3, 13);
            this.baselineMainContainer.Controls.Add(this.informationBox1, 1, 1);
            this.baselineMainContainer.Controls.Add(this.daysChkContainer, 2, 7);
            this.baselineMainContainer.Controls.Add(this.daysChkContainer1, 2, 12);
            this.baselineMainContainer.Controls.Add(this.automaticBaselineRadioButton, 2, 2);
            this.baselineMainContainer.Controls.Add(this.customDateMainContainer, 3, 4);
            this.baselineMainContainer.Controls.Add(this.customBaselineRadioButton, 2, 3);
            this.baselineMainContainer.Controls.Add(this.panel2, 0, 5);
            //this.baselineMainContainer.Controls.Add(this.panel4, 0, 9);
            this.baselineMainContainer.Controls.Add(this.panel3, 0, 11);
            this.baselineMainContainer.Location = new System.Drawing.Point(5, 54);
            this.baselineMainContainer.Name = "baselineMainContainer";
            this.baselineMainContainer.RowCount = 17;
            this.baselineMainContainer.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.baselineMainContainer.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.baselineMainContainer.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.baselineMainContainer.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.baselineMainContainer.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.baselineMainContainer.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.baselineMainContainer.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.baselineMainContainer.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.baselineMainContainer.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.baselineMainContainer.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.baselineMainContainer.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.baselineMainContainer.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.baselineMainContainer.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.baselineMainContainer.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.baselineMainContainer.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.baselineMainContainer.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.baselineMainContainer.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.baselineMainContainer.Size = new System.Drawing.Size(495, 632);
            this.baselineMainContainer.TabIndex = 65;
    //        // 
    //        // informationBox2
    //        // 
    //        this.baselineMainContainer.SetColumnSpan(this.informationBox2, 3);
    //        this.informationBox2.Dock = System.Windows.Forms.DockStyle.Fill;
    //        this.informationBox2.Location = new System.Drawing.Point(3, 342);
    //        this.informationBox2.Name = "informationBox2";
    //        this.informationBox2.Size = new System.Drawing.Size(489, 33);
    //        this.informationBox2.TabIndex = 88;
    //        this.informationBox2.Text = "Additional named baselines can be added/edited and will be used in place of the d" +
    //"efault baseline for the specified timeframe.";
            // 
            // panel1
            // 
            this.baselineMainContainer.SetColumnSpan(this.panel1, 4);
            this.panel1.Controls.Add(this.header1label);
            this.panel1.Controls.Add(this.headerStrip1);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel1.Location = new System.Drawing.Point(3, 3);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(489, 25);
            this.panel1.TabIndex = 0;
            // 
            // header1label
            // 
            this.header1label.AutoSize = true;
            this.header1label.BackColor = System.Drawing.SystemColors.Control;
            this.header1label.Location = new System.Drawing.Point(0, 5);
            this.header1label.Name = "header1label";
            this.header1label.Size = new System.Drawing.Size(414, 13);
            this.header1label.TabIndex = 59;
            this.header1label.Text = "What date range should be used to calculate the performance baseline for this ser" +
    "ver?";
            this.header1label.Visible = false;
            // 
            // btnSelectOtherServers
            // 
            this.btnSelectOtherServers.AutoSize = true;
            //this.btnSelectOtherServers.BackColor = System.Drawing.SystemColors.Control;
            this.baselineMainContainer.SetColumnSpan(this.btnSelectOtherServers, 4);//SQLdm 10.1 (pulkit Puri) -- for adjustment
            //this.btnSelectOtherServers.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.btnSelectOtherServers.Location = new System.Drawing.Point(33, 400);
            this.btnSelectOtherServers.Margin = new System.Windows.Forms.Padding(3, 15, 3, 3);
            this.btnSelectOtherServers.Name = "btnSelectOtherServers";
            this.btnSelectOtherServers.Size = new System.Drawing.Size(155, 22);
            this.btnSelectOtherServers.TabIndex = 57;
            this.btnSelectOtherServers.Text = "Apply Baseline Configuration";
            this.btnSelectOtherServers.UseVisualStyleBackColor = true;
            this.btnSelectOtherServers.Click += new System.EventHandler(this.btnSelectOtherServers_Click);
            //// 
            //// textBox1
            //// 
            //this.textBox1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            //this.textBox1.Location = new System.Drawing.Point(83, 381);
            //this.textBox1.Name = "textBox1";
            //this.textBox1.Size = new System.Drawing.Size(275, 20);
            //this.textBox1.TabIndex = 84;
            //this.textBox1.Leave += new System.EventHandler(this.Update);
            //// 
            //// label1
            //// 
            //this.label1.Anchor = System.Windows.Forms.AnchorStyles.Left;
            //this.label1.AutoSize = true;
            //this.baselineMainContainer.SetColumnSpan(this.label1, 3);
            //this.label1.FlatStyle = System.Windows.Forms.FlatStyle.System;
            //this.label1.Location = new System.Drawing.Point(3, 384);
            //this.label1.Margin = new System.Windows.Forms.Padding(3, 5, 3, 5);
            //this.label1.Name = "label1";
            //this.label1.Size = new System.Drawing.Size(74, 13);
            //this.label1.TabIndex = 85;
            //this.label1.Text = "Baseline Name";
            // 
            // timeDateContainer
            // 
            this.timeDateContainer.Controls.Add(this.label3);
            this.timeDateContainer.Controls.Add(this.beginTimeCombo);
            this.timeDateContainer.Controls.Add(this.label4);
            this.timeDateContainer.Controls.Add(this.endTimeCombo);
            this.timeDateContainer.Dock = System.Windows.Forms.DockStyle.Fill;
            this.timeDateContainer.Location = new System.Drawing.Point(83, 265);
            this.timeDateContainer.Name = "timeDateContainer";
            this.timeDateContainer.Size = new System.Drawing.Size(409, 40);
            this.timeDateContainer.TabIndex = 64;
            // 
            // label3
            // 
            this.label3.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(3, 9);
            this.label3.Margin = new System.Windows.Forms.Padding(3, 5, 3, 5);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(33, 13);
            this.label3.TabIndex = 55;
            this.label3.Text = "From:";
            // 
            // beginTimeCombo
            // 
            this.beginTimeCombo.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.beginTimeCombo.AutoFillTime = Infragistics.Win.UltraWinMaskedEdit.AutoFillTime.Midnight;
            dropDownEditorButton1.Key = "DropDownList";
            dropDownEditorButton1.PreferredDropDownSize = new System.Drawing.Size(0, 0);
            this.beginTimeCombo.ButtonsRight.Add(dropDownEditorButton1);
            this.beginTimeCombo.DateTime = new System.DateTime(2008, 8, 1, 8, 0, 0, 0);
            this.beginTimeCombo.DropDownButtonDisplayStyle = Infragistics.Win.ButtonDisplayStyle.Never;
            this.beginTimeCombo.ListInterval = System.TimeSpan.Parse("00:30:00");
            this.beginTimeCombo.Location = new System.Drawing.Point(42, 5);
            this.beginTimeCombo.Margin = new System.Windows.Forms.Padding(3, 5, 3, 5);
            this.beginTimeCombo.MaskInput = "{time}";
            this.beginTimeCombo.Name = "beginTimeCombo";
            this.beginTimeCombo.Nullable = false;
            this.beginTimeCombo.Size = new System.Drawing.Size(88, 21);
            this.beginTimeCombo.TabIndex = 53;
            this.beginTimeCombo.Time = System.TimeSpan.Parse("08:00:00");
            this.beginTimeCombo.Value = new System.DateTime(2008, 8, 1, 8, 0, 0, 0);
            this.beginTimeCombo.ValueChanged += new System.EventHandler(this.Update);
            // 
            // label4
            // 
            this.label4.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(136, 9);
            this.label4.Margin = new System.Windows.Forms.Padding(3, 5, 3, 5);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(23, 13);
            this.label4.TabIndex = 56;
            this.label4.Text = "To:";
            // 
            // endTimeCombo
            // 
            this.endTimeCombo.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.endTimeCombo.AutoFillTime = Infragistics.Win.UltraWinMaskedEdit.AutoFillTime.Midnight;
            dropDownEditorButton2.Key = "DropDownList";
            dropDownEditorButton2.PreferredDropDownSize = new System.Drawing.Size(0, 0);
            this.endTimeCombo.ButtonsRight.Add(dropDownEditorButton2);
            this.endTimeCombo.DateTime = new System.DateTime(2008, 8, 1, 17, 0, 0, 0);
            this.endTimeCombo.DropDownButtonDisplayStyle = Infragistics.Win.ButtonDisplayStyle.Never;
            this.endTimeCombo.ListInterval = System.TimeSpan.Parse("00:30:00");
            this.endTimeCombo.Location = new System.Drawing.Point(165, 5);
            this.endTimeCombo.Margin = new System.Windows.Forms.Padding(3, 5, 3, 5);
            this.endTimeCombo.MaskInput = "{time}";
            this.endTimeCombo.Name = "endTimeCombo";
            this.endTimeCombo.Nullable = false;
            this.endTimeCombo.Size = new System.Drawing.Size(88, 21);
            this.endTimeCombo.TabIndex = 54;
            this.endTimeCombo.Time = System.TimeSpan.Parse("17:00:00");
            this.endTimeCombo.Value = new System.DateTime(2008, 8, 1, 17, 0, 0, 0);
            this.endTimeCombo.ValueChanged += new System.EventHandler(this.Update);
            // 
            // informationBox1
            // 
            this.baselineMainContainer.SetColumnSpan(this.informationBox1, 3);
            this.informationBox1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.informationBox1.Location = new System.Drawing.Point(3, 34);
            this.informationBox1.Name = "informationBox1";
            this.informationBox1.Size = new System.Drawing.Size(489, 45);
            this.informationBox1.TabIndex = 40;
            this.informationBox1.Text = resources.GetString("informationBox1.Text");
            // 
            // daysChkContainer
            // 
            this.daysChkContainer.ColumnCount = 7;
            this.baselineMainContainer.SetColumnSpan(this.daysChkContainer, 2);
            this.daysChkContainer.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.daysChkContainer.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.daysChkContainer.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.daysChkContainer.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.daysChkContainer.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.daysChkContainer.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.daysChkContainer.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.daysChkContainer.Controls.Add(this.sundayCheckbox, 0, 0);
            this.daysChkContainer.Controls.Add(this.mondayCheckBox, 1, 0);
            this.daysChkContainer.Controls.Add(this.tuesdayCheckBox, 2, 0);
            this.daysChkContainer.Controls.Add(this.wednesdayCheckBox, 3, 0);
            this.daysChkContainer.Controls.Add(this.thursdayCheckBox, 4, 0);
            this.daysChkContainer.Controls.Add(this.fridayCheckBox, 5, 0);
            this.daysChkContainer.Controls.Add(this.saturdayCheckBox, 6, 0);
            this.daysChkContainer.Dock = System.Windows.Forms.DockStyle.Fill;
            this.daysChkContainer.Location = new System.Drawing.Point(33, 231);
            this.daysChkContainer.Margin = new System.Windows.Forms.Padding(3, 3, 3, 5);
            this.daysChkContainer.Name = "daysChkContainer";
            this.daysChkContainer.RowCount = 1;
            this.daysChkContainer.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.daysChkContainer.Size = new System.Drawing.Size(459, 26);
            this.daysChkContainer.TabIndex = 63;
            // 
            // sundayCheckbox
            // 
            this.sundayCheckbox.AutoSize = true;
            this.sundayCheckbox.Location = new System.Drawing.Point(3, 3);
            this.sundayCheckbox.Name = "sundayCheckbox";
            this.sundayCheckbox.Size = new System.Drawing.Size(45, 17);
            this.sundayCheckbox.TabIndex = 47;
            this.sundayCheckbox.Text = "Sun";
            this.sundayCheckbox.UseVisualStyleBackColor = true;
            this.sundayCheckbox.CheckedChanged += new System.EventHandler(this.Update);
            // 
            // mondayCheckBox
            // 
            this.mondayCheckBox.AutoSize = true;
            this.mondayCheckBox.Checked = true;
            this.mondayCheckBox.CheckState = System.Windows.Forms.CheckState.Checked;
            this.mondayCheckBox.Location = new System.Drawing.Point(54, 3);
            this.mondayCheckBox.Name = "mondayCheckBox";
            this.mondayCheckBox.Size = new System.Drawing.Size(47, 17);
            this.mondayCheckBox.TabIndex = 46;
            this.mondayCheckBox.Text = "Mon";
            this.mondayCheckBox.UseVisualStyleBackColor = true;
            this.mondayCheckBox.CheckedChanged += new System.EventHandler(this.Update);
            // 
            // tuesdayCheckBox
            // 
            this.tuesdayCheckBox.AutoSize = true;
            this.tuesdayCheckBox.Checked = true;
            this.tuesdayCheckBox.CheckState = System.Windows.Forms.CheckState.Checked;
            this.tuesdayCheckBox.Location = new System.Drawing.Point(107, 3);
            this.tuesdayCheckBox.Name = "tuesdayCheckBox";
            this.tuesdayCheckBox.Size = new System.Drawing.Size(45, 17);
            this.tuesdayCheckBox.TabIndex = 48;
            this.tuesdayCheckBox.Text = "Tue";
            this.tuesdayCheckBox.UseVisualStyleBackColor = true;
            this.tuesdayCheckBox.CheckedChanged += new System.EventHandler(this.Update);
            // 
            // wednesdayCheckBox
            // 
            this.wednesdayCheckBox.AutoSize = true;
            this.wednesdayCheckBox.Checked = true;
            this.wednesdayCheckBox.CheckState = System.Windows.Forms.CheckState.Checked;
            this.wednesdayCheckBox.Location = new System.Drawing.Point(158, 3);
            this.wednesdayCheckBox.Name = "wednesdayCheckBox";
            this.wednesdayCheckBox.Size = new System.Drawing.Size(49, 17);
            this.wednesdayCheckBox.TabIndex = 49;
            this.wednesdayCheckBox.Text = "Wed";
            this.wednesdayCheckBox.UseVisualStyleBackColor = true;
            this.wednesdayCheckBox.CheckedChanged += new System.EventHandler(this.Update);
            // 
            // thursdayCheckBox
            // 
            this.thursdayCheckBox.AutoSize = true;
            this.thursdayCheckBox.Checked = true;
            this.thursdayCheckBox.CheckState = System.Windows.Forms.CheckState.Checked;
            this.thursdayCheckBox.Location = new System.Drawing.Point(213, 3);
            this.thursdayCheckBox.Name = "thursdayCheckBox";
            this.thursdayCheckBox.Size = new System.Drawing.Size(45, 17);
            this.thursdayCheckBox.TabIndex = 50;
            this.thursdayCheckBox.Text = "Thu";
            this.thursdayCheckBox.UseVisualStyleBackColor = true;
            this.thursdayCheckBox.CheckedChanged += new System.EventHandler(this.Update);
            // 
            // fridayCheckBox
            // 
            this.fridayCheckBox.AutoSize = true;
            this.fridayCheckBox.Checked = true;
            this.fridayCheckBox.CheckState = System.Windows.Forms.CheckState.Checked;
            this.fridayCheckBox.Location = new System.Drawing.Point(264, 3);
            this.fridayCheckBox.Name = "fridayCheckBox";
            this.fridayCheckBox.Size = new System.Drawing.Size(37, 17);
            this.fridayCheckBox.TabIndex = 51;
            this.fridayCheckBox.Text = "Fri";
            this.fridayCheckBox.UseVisualStyleBackColor = true;
            this.fridayCheckBox.CheckedChanged += new System.EventHandler(this.Update);
            // 
            // saturdayCheckBox
            // 
            this.saturdayCheckBox.AutoSize = true;
            this.saturdayCheckBox.Location = new System.Drawing.Point(307, 3);
            this.saturdayCheckBox.Name = "saturdayCheckBox";
            this.saturdayCheckBox.Size = new System.Drawing.Size(42, 17);
            this.saturdayCheckBox.TabIndex = 52;
            this.saturdayCheckBox.Text = "Sat";
            this.saturdayCheckBox.UseVisualStyleBackColor = true;
            this.saturdayCheckBox.CheckedChanged += new System.EventHandler(this.Update);
            // 
            // automaticBaselineRadioButton
            // 
            this.automaticBaselineRadioButton.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.automaticBaselineRadioButton.AutoSize = true;
            this.automaticBaselineRadioButton.Checked = true;
            this.baselineMainContainer.SetColumnSpan(this.automaticBaselineRadioButton, 2);
            this.automaticBaselineRadioButton.Location = new System.Drawing.Point(33, 85);
            this.automaticBaselineRadioButton.Name = "automaticBaselineRadioButton";
            this.automaticBaselineRadioButton.Size = new System.Drawing.Size(422, 17);
            this.automaticBaselineRadioButton.TabIndex = 39;
            this.automaticBaselineRadioButton.TabStop = true;
            this.automaticBaselineRadioButton.Text = "Dynamic (calculates a dynamic baseline based on the last 7 days of collected da" +
    "ta)";
            this.automaticBaselineRadioButton.TextAlign = System.Drawing.ContentAlignment.TopLeft;
            this.automaticBaselineRadioButton.UseVisualStyleBackColor = true;
            this.automaticBaselineRadioButton.CheckedChanged += new System.EventHandler(this.automaticBaselineRadioButton_CheckedChanged);
            // 
            // customDateMainContainer
            // 
            this.customDateMainContainer.Controls.Add(this.customDateFromLabel);
            this.customDateMainContainer.Controls.Add(this.customDateFromCombo);
            this.customDateMainContainer.Controls.Add(this.customDateToLabel);
            this.customDateMainContainer.Controls.Add(this.customDateToCombo);
            this.customDateMainContainer.Controls.Add(this.addButton);// SQLdm 10.1 (Pulkit Puri)Relocating Add baseline button
            this.customDateMainContainer.Dock = System.Windows.Forms.DockStyle.Fill;
            this.customDateMainContainer.Location = new System.Drawing.Point(83, 131);
            this.customDateMainContainer.Name = "customDateMainContainer";
            this.customDateMainContainer.Size = new System.Drawing.Size(409, 40);
            this.customDateMainContainer.TabIndex = 62;
            // 
            // customDateFromLabel
            // 
            this.customDateFromLabel.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.customDateFromLabel.AutoSize = true;
            this.customDateFromLabel.Enabled = false;
            this.customDateFromLabel.Location = new System.Drawing.Point(3, 9);
            this.customDateFromLabel.Margin = new System.Windows.Forms.Padding(3, 5, 3, 5);
            this.customDateFromLabel.Name = "customDateFromLabel";
            this.customDateFromLabel.Size = new System.Drawing.Size(33, 13);
            this.customDateFromLabel.TabIndex = 43;
            this.customDateFromLabel.Text = "From:";
            // 
            // customDateFromCombo
            // 
            this.customDateFromCombo.AllowNull = false;
            this.customDateFromCombo.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.customDateFromCombo.DateButtons.Add(dateButton1);
            this.customDateFromCombo.Enabled = false;
            this.customDateFromCombo.Location = new System.Drawing.Point(42, 5);
            this.customDateFromCombo.Margin = new System.Windows.Forms.Padding(3, 5, 3, 5);
            this.customDateFromCombo.Name = "customDateFromCombo";
            this.customDateFromCombo.NonAutoSizeHeight = 21;
            this.customDateFromCombo.Size = new System.Drawing.Size(88, 21);
            this.customDateFromCombo.TabIndex = 42;
            this.customDateFromCombo.Value = new System.DateTime(2008, 8, 13, 0, 0, 0, 0);
            this.customDateFromCombo.ValueChanged += new System.EventHandler(this.Update);
            // 
            // customDateToLabel
            // 
            this.customDateToLabel.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.customDateToLabel.AutoSize = true;
            this.customDateToLabel.Enabled = false;
            this.customDateToLabel.Location = new System.Drawing.Point(136, 9);
            this.customDateToLabel.Margin = new System.Windows.Forms.Padding(3, 5, 3, 5);
            this.customDateToLabel.Name = "customDateToLabel";
            this.customDateToLabel.Size = new System.Drawing.Size(23, 13);
            this.customDateToLabel.TabIndex = 45;
            this.customDateToLabel.Text = "To:";
            // 
            // customDateToCombo
            // 
            this.customDateToCombo.AllowNull = false;
            this.customDateToCombo.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.customDateToCombo.DateButtons.Add(dateButton2);
            this.customDateToCombo.Enabled = false;
            this.customDateToCombo.Location = new System.Drawing.Point(165, 5);
            this.customDateToCombo.Margin = new System.Windows.Forms.Padding(3, 5, 3, 5);
            this.customDateToCombo.Name = "customDateToCombo";
            this.customDateToCombo.NonAutoSizeHeight = 21;
            this.customDateToCombo.Size = new System.Drawing.Size(88, 21);
            this.customDateToCombo.TabIndex = 44;
            this.customDateToCombo.Value = new System.DateTime(2008, 8, 13, 0, 0, 0, 0);
            this.customDateToCombo.ValueChanged += new System.EventHandler(this.Update);
            // 
            // btnBaseLineAssistant
            // 
			
			
           /* this.btnBaseLineAssistant.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this.btnBaseLineAssistant.AutoSize = true;
            this.btnBaseLineAssistant.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.btnBaseLineAssistant.Location = new System.Drawing.Point(259, 4);
            this.btnBaseLineAssistant.Name = "btnBaseLineAssistant";
            this.btnBaseLineAssistant.Size = new System.Drawing.Size(108, 23);
            this.btnBaseLineAssistant.TabIndex = 65;
            this.btnBaseLineAssistant.Text = "Baseline Visualizer";
            this.btnBaseLineAssistant.UseVisualStyleBackColor = false;
            this.btnBaseLineAssistant.Click += new System.EventHandler(this.btnBaseLineAssistant_Click);*/
			// commented to remove this button
			
            // 
            // customBaselineRadioButton
            // 
            this.customBaselineRadioButton.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.customBaselineRadioButton.AutoSize = true;
            this.baselineMainContainer.SetColumnSpan(this.customBaselineRadioButton, 2);
            this.customBaselineRadioButton.Location = new System.Drawing.Point(33, 108);
            this.customBaselineRadioButton.Name = "customBaselineRadioButton";
            this.customBaselineRadioButton.Size = new System.Drawing.Size(60, 17);
            this.customBaselineRadioButton.TabIndex = 41;
            this.customBaselineRadioButton.Text = "Custom";
            this.customBaselineRadioButton.UseVisualStyleBackColor = true;
            this.customBaselineRadioButton.CheckedChanged += new System.EventHandler(this.customBaselineRadioButton_CheckedChanged_1);
            // 
            // panel2
            // 
            this.baselineMainContainer.SetColumnSpan(this.panel2, 4);
            this.panel2.Controls.Add(this.header2label);
            this.panel2.Controls.Add(this.propertiesHeaderStrip1);
            this.panel2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel2.Location = new System.Drawing.Point(3, 177);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(489, 25);
            this.panel2.TabIndex = 63;
            // 
            // header2label
            // 
            this.header2label.AutoSize = true;
            this.header2label.BackColor = System.Drawing.SystemColors.Control;
            this.header2label.Location = new System.Drawing.Point(0, 6);
            this.header2label.Name = "header2label";
            this.header2label.Size = new System.Drawing.Size(421, 13);
            this.header2label.TabIndex = 60;
            this.header2label.Text = "Which days and time period within those days should be used to calculate the base" +
    "line?";
            this.header2label.Visible = false;
            //// 
            //// panel4
            //// 
            //this.baselineMainContainer.SetColumnSpan(this.panel4, 4);
            ////this.panel4.Controls.Add(this.header4label);
            //this.panel4.Controls.Add(this.propertiesHeaderStrip2);
            //this.panel4.Dock = System.Windows.Forms.DockStyle.Fill;
            //this.panel4.Location = new System.Drawing.Point(3, 311);
            //this.panel4.Name = "panel4";
            //this.panel4.Size = new System.Drawing.Size(489, 25);
            //this.panel4.TabIndex = 67;
            //// 
            //// header4label
            //// 
            //this.header4label.AutoSize = true;
            //this.header4label.BackColor = System.Drawing.SystemColors.Control;
            //this.header4label.Location = new System.Drawing.Point(0, 6);
            //this.header4label.Name = "header4label";
            //this.header4label.Size = new System.Drawing.Size(223, 13);
            //this.header4label.TabIndex = 68;
            //this.header4label.Text = "Would you like to create additional baselines?\r\n";
            //this.header4label.Visible = false;
            // 
            // panel3
            // 
            this.baselineMainContainer.SetColumnSpan(this.panel3, 4);
            this.panel3.Controls.Add(this.header3label);
            this.panel3.Controls.Add(this.propertiesHeaderStripSelectServers);
            this.panel3.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel3.Location = new System.Drawing.Point(3, 311);
            this.panel3.Name = "panel3";
            this.panel3.Size = new System.Drawing.Size(489, 25);
            this.panel3.TabIndex = 67;
            // 
            // header3label
            // 
            this.header3label.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.header3label.AutoSize = true;
            this.header3label.BackColor = System.Drawing.SystemColors.Control;
            this.header3label.Location = new System.Drawing.Point(0, 6);
            this.header3label.Name = "header3label";
            this.header3label.Size = new System.Drawing.Size(350, 13);
            this.header3label.TabIndex = 61;
            this.header3label.Text = "Would you like to apply this baseline configuration to other SQL Servers?";
            this.header3label.Visible = false;
            // 
            // headerStrip1
            // 
            this.headerStrip1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.headerStrip1.ForeColor = System.Drawing.Color.Black;
            this.headerStrip1.Location = new System.Drawing.Point(0, 0);
            this.headerStrip1.Name = "headerStrip1";
            this.headerStrip1.Size = new System.Drawing.Size(489, 25);
            this.headerStrip1.TabIndex = 37;
            this.headerStrip1.Text = "What date range should be used to calculate the performance baseline for this ser" +
    "ver?";
            this.headerStrip1.WordWrap = false;
            // 
            // propertiesHeaderStrip1
            // 
            this.propertiesHeaderStrip1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.propertiesHeaderStrip1.ForeColor = System.Drawing.Color.Black;
            this.propertiesHeaderStrip1.Location = new System.Drawing.Point(0, 0);
            this.propertiesHeaderStrip1.Name = "propertiesHeaderStrip1";
            this.propertiesHeaderStrip1.Size = new System.Drawing.Size(489, 25);
            this.propertiesHeaderStrip1.TabIndex = 38;
            this.propertiesHeaderStrip1.Text = "Which days and time period within those days should be used to calculate the base" +
    "line?";
            this.propertiesHeaderStrip1.WordWrap = false;
            //// 
            //// propertiesHeaderStrip2
            //// 
            //this.propertiesHeaderStrip2.Dock = System.Windows.Forms.DockStyle.Fill;
            //this.propertiesHeaderStrip2.ForeColor = System.Drawing.Color.Black;
            //this.propertiesHeaderStrip2.Location = new System.Drawing.Point(0, 0);
            //this.propertiesHeaderStrip2.Name = "propertiesHeaderStrip2";
            //this.propertiesHeaderStrip2.Size = new System.Drawing.Size(489, 25);
            //this.propertiesHeaderStrip2.TabIndex = 69;
            //this.propertiesHeaderStrip2.Text = "Would you like to create additional baselines?";
            //this.propertiesHeaderStrip2.WordWrap = false;
            // 
            // propertiesHeaderStripSelectServers
            // 
            this.propertiesHeaderStripSelectServers.Dock = System.Windows.Forms.DockStyle.Fill;
            this.propertiesHeaderStripSelectServers.ForeColor = System.Drawing.Color.Black;
            this.propertiesHeaderStripSelectServers.Location = new System.Drawing.Point(0, 0);
            this.propertiesHeaderStripSelectServers.Name = "propertiesHeaderStripSelectServers";
            this.propertiesHeaderStripSelectServers.Size = new System.Drawing.Size(489, 25);
            this.propertiesHeaderStripSelectServers.TabIndex = 58;
            this.propertiesHeaderStripSelectServers.Text = "Would you like to apply this baseline configuration to other SQL Servers?";
            this.propertiesHeaderStripSelectServers.WordWrap = false;
            // 
            // BaselineConfigurationPage
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.baselineMainContainer);
            this.Controls.Add(this.office2007PropertyPage1);
            this.Name = "BaselineConfigurationPage";
            this.Size = new System.Drawing.Size(505, 689);
            ((System.ComponentModel.ISupportInitialize)(this.beginTimeCombo1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.endTimeCombo1)).EndInit();
            this.timeDateContainer1.ResumeLayout(false);
            this.timeDateContainer1.PerformLayout();
            this.daysChkContainer1.ResumeLayout(false);
            this.daysChkContainer1.PerformLayout();
            this.baselineMainContainer.ResumeLayout(false);
            this.baselineMainContainer.PerformLayout();
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.timeDateContainer.ResumeLayout(false);
            this.timeDateContainer.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.beginTimeCombo)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.endTimeCombo)).EndInit();
            this.daysChkContainer.ResumeLayout(false);
            this.daysChkContainer.PerformLayout();
            this.customDateMainContainer.ResumeLayout(false);
            this.customDateMainContainer.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.customDateFromCombo)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.customDateToCombo)).EndInit();
            this.panel2.ResumeLayout(false);
            this.panel2.PerformLayout();
            //this.panel4.ResumeLayout(false);
            //this.panel4.PerformLayout();
            this.panel3.ResumeLayout(false);
            this.panel3.PerformLayout();
            this.ResumeLayout(false);

        }

        void OnCurrentThemeChanged(object sender, System.EventArgs e)
        {
            SetPropertiesTheme();
        }

        void SetPropertiesTheme()
        {
            var propertiesThemeManager = new Controls.PropertiesThemeManager();
            propertiesThemeManager.UpdatePropertyTheme(office2007PropertyPage1);
        }

        #endregion

        private Office2007PropertyPage office2007PropertyPage1;
        private PropertiesHeaderStrip propertiesHeaderStripSelectServers;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomButton btnSelectOtherServers;
       // private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomButton btnBaseLineAssistant;// SQLdm 10.1 (Pulkit Puri)--for deleting the button
	   //SQLdm 10.0 (Tarun Sapra) : Adding button for baseline assistant dialog 
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomButton saveButton;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomButton addButton;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomButton cancelButton;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomLabel label4;
        //private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomTextBox textBox1;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomLabel label3;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomLabel label7;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomLabel label8;
        //private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomLabel label5;
        //private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomLabel label6;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomTimeComboEditor endTimeCombo;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomTimeComboEditor beginTimeCombo;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomTimeComboEditor endTimeCombo1;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomTimeComboEditor beginTimeCombo1;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomCheckBox saturdayCheckBox;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomCheckBox fridayCheckBox;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomCheckBox thursdayCheckBox;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomCheckBox wednesdayCheckBox;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomCheckBox tuesdayCheckBox;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomCheckBox sundayCheckbox;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomCheckBox mondayCheckBox;
        //private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomCheckBox saturdayCheckBox1;
        //private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomCheckBox fridayCheckBox1;
        //private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomCheckBox thursdayCheckBox1;
        //private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomCheckBox wednesdayCheckBox1;
        //private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomCheckBox tuesdayCheckBox1;
        //private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomCheckBox sundayCheckbox1;
        //private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomCheckBox mondayCheckBox1;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomLabel customDateToLabel;
        private Infragistics.Win.UltraWinSchedule.UltraCalendarCombo customDateToCombo;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomLabel customDateFromLabel;
        private Infragistics.Win.UltraWinSchedule.UltraCalendarCombo customDateFromCombo;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomRadioButton customBaselineRadioButton;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomInformationBox informationBox1;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomRadioButton automaticBaselineRadioButton;
        private PropertiesHeaderStrip propertiesHeaderStrip1;
        //private PropertiesHeaderStrip propertiesHeaderStrip2;
        private PropertiesHeaderStrip headerStrip1;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomLabel header3label;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomLabel header2label;
        //private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomLabel header4label;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomLabel header1label;
        //private System.Windows.Forms.ComboBox comboBox1;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomFlowLayoutPanel customDateMainContainer;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomTableLayoutPanel daysChkContainer;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomTableLayoutPanel daysChkContainer1;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomFlowLayoutPanel timeDateContainer;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomFlowLayoutPanel timeDateContainer1;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomTableLayoutPanel baselineMainContainer;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomPanel  panel1;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomPanel  panel2;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomPanel  panel3;
        //private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomPanel  panel4;
        //private Label label1;
        //private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomInformationBox informationBox2;
    }
}
