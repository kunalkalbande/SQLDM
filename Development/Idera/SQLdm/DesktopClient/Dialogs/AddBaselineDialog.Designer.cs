using Idera.SQLdm.DesktopClient.Controls;
using System.Drawing;

namespace Idera.SQLdm.DesktopClient.Dialogs
{
    partial class AddBaselineDialog
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
            Infragistics.Win.UltraWinEditors.DropDownEditorButton dropDownEditorButton5 = new Infragistics.Win.UltraWinEditors.DropDownEditorButton("DropDownList");
            Infragistics.Win.UltraWinEditors.DropDownEditorButton dropDownEditorButton6 = new Infragistics.Win.UltraWinEditors.DropDownEditorButton("DropDownList");
            Infragistics.Win.UltraWinEditors.DropDownEditorButton dropDownEditorButtonBegin = new Infragistics.Win.UltraWinEditors.DropDownEditorButton("DropDownList");
            Infragistics.Win.UltraWinEditors.DropDownEditorButton dropDownEditorButtonEnd = new Infragistics.Win.UltraWinEditors.DropDownEditorButton("DropDownList");

            //START : Revise Multiple Baseline for Independent Scheduling
            //SQLdm 10.1 (Srishti Purohit)
            Infragistics.Win.UltraWinSchedule.CalendarCombo.DateButton dateButton1 = new Infragistics.Win.UltraWinSchedule.CalendarCombo.DateButton();
            Infragistics.Win.UltraWinSchedule.CalendarCombo.DateButton dateButton2 = new Infragistics.Win.UltraWinSchedule.CalendarCombo.DateButton();

            this.automaticBaselineRadioButton = new System.Windows.Forms.RadioButton();
            this.customDateMainContainer = new System.Windows.Forms.FlowLayoutPanel();
            this.customDateFromLabel = new System.Windows.Forms.Label();
            this.customDateFromCombo = new Infragistics.Win.UltraWinSchedule.UltraCalendarCombo();
            this.customDateToLabel = new System.Windows.Forms.Label();
            this.customDateToCombo = new Infragistics.Win.UltraWinSchedule.UltraCalendarCombo();
            this.customBaselineRadioButton = new System.Windows.Forms.RadioButton();
            //this.editButton = new System.Windows.Forms.Button();
            this.deleteButton = new System.Windows.Forms.Button();

            this.panelDefault = new System.Windows.Forms.Panel();
            //this.headerLblDefault = new System.Windows.Forms.Label();
            //this.propertiesHeaderStripDefault = new Idera.SQLdm.DesktopClient.Controls.PropertiesHeaderStrip();
            this.timeDateContainer = new System.Windows.Forms.FlowLayoutPanel();
            this.lblBeginTime = new System.Windows.Forms.Label();
            this.lblEndTime = new System.Windows.Forms.Label();
            this.beginScheduleTimeCombo = new Idera.SQLdm.Common.UI.Controls.TimeComboEditor();
            this.endScheduleTimeCombo = new Idera.SQLdm.Common.UI.Controls.TimeComboEditor();
            this.daysChkContainer = new System.Windows.Forms.TableLayoutPanel();
            this.sundayCheckbox = new System.Windows.Forms.CheckBox();
            this.mondayCheckBox = new System.Windows.Forms.CheckBox();
            this.tuesdayCheckBox = new System.Windows.Forms.CheckBox();
            this.wednesdayCheckBox = new System.Windows.Forms.CheckBox();
            this.thursdayCheckBox = new System.Windows.Forms.CheckBox();
            this.fridayCheckBox = new System.Windows.Forms.CheckBox();
            this.saturdayCheckBox = new System.Windows.Forms.CheckBox();
            //END : Revise Multiple Baseline for Independent Scheduling
            //SQLdm 10.1 (Srishti Purohit)

            this.addButton = new System.Windows.Forms.Button();
            this.saveButton = new System.Windows.Forms.Button();
            this.cancelButton = new System.Windows.Forms.Button();
            this.label5 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.panelDaysCollectedData = new System.Windows.Forms.Panel();
            this.headerLblDaysCollectedData = new System.Windows.Forms.Label();
            this.headerStripDaysCollectedData = new Idera.SQLdm.DesktopClient.Controls.PropertiesHeaderStrip();
            this.panel1 = new System.Windows.Forms.Panel();
            this.header1label = new System.Windows.Forms.Label();
            this.headerStrip1 = new Idera.SQLdm.DesktopClient.Controls.PropertiesHeaderStrip();
            this.sundayCheckbox1 = new System.Windows.Forms.CheckBox();
            this.mondayCheckBox1 = new System.Windows.Forms.CheckBox();
            this.tuesdayCheckBox1 = new System.Windows.Forms.CheckBox();
            this.wednesdayCheckBox1 = new System.Windows.Forms.CheckBox();
            this.thursdayCheckBox1 = new System.Windows.Forms.CheckBox();
            this.fridayCheckBox1 = new System.Windows.Forms.CheckBox();
            this.saturdayCheckBox1 = new System.Windows.Forms.CheckBox();
            this.daysChkContainer1 = new System.Windows.Forms.TableLayoutPanel();
            this.baselineMainContainer = new System.Windows.Forms.TableLayoutPanel();
            //this.textBox1 = new System.Windows.Forms.TextBox();
            this.comboBaselineName = new System.Windows.Forms.ComboBox();
            this.timeDateContainer1 = new System.Windows.Forms.FlowLayoutPanel();
            this.beginCalculationTimeCombo = new Idera.SQLdm.Common.UI.Controls.TimeComboEditor();
            this.endCalculationTimeCombo = new Idera.SQLdm.Common.UI.Controls.TimeComboEditor();
            this.panelDaysCollectedData.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.customDateFromCombo)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.customDateToCombo)).BeginInit();
            this.panelDefault.SuspendLayout();
            this.panel1.SuspendLayout();
            this.daysChkContainer.SuspendLayout();
            this.daysChkContainer1.SuspendLayout();
            this.baselineMainContainer.SuspendLayout();
            this.timeDateContainer1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.beginCalculationTimeCombo)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.endCalculationTimeCombo)).BeginInit();

            ((System.ComponentModel.ISupportInitialize)(this.beginScheduleTimeCombo)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.endScheduleTimeCombo)).BeginInit();
            this.SuspendLayout();
            // 
            // panelDaysCollectedData
            // 
            this.baselineMainContainer.SetColumnSpan(this.panelDaysCollectedData, 4);
            this.panelDaysCollectedData.Controls.Add(this.headerLblDaysCollectedData);
            this.panelDaysCollectedData.Controls.Add(this.headerStripDaysCollectedData);
            this.panelDaysCollectedData.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panelDaysCollectedData.Location = new System.Drawing.Point(3, 3);
            this.panelDaysCollectedData.Name = "panel2";
            this.panelDaysCollectedData.Size = new System.Drawing.Size(489, 40);
            this.panelDaysCollectedData.TabIndex = 5;
            // 
            // headerLblDaysCollectedData
            // 
            this.headerLblDaysCollectedData.AutoSize = true;
            this.headerStripDaysCollectedData.BackColor = System.Drawing.SystemColors.Control;
            this.headerLblDaysCollectedData.Location = new System.Drawing.Point(0, 5);
            this.headerLblDaysCollectedData.Name = "header1label";
            this.headerLblDaysCollectedData.Size = new System.Drawing.Size(414, 30);
            this.headerLblDaysCollectedData.TabIndex = 20;
            this.headerLblDaysCollectedData.Text = "What date range, days and time period within those days should be used to calculate the performance baseline for this ser" +
    "ver?";
            this.headerLblDaysCollectedData.Visible = false;
            this.headerLblDaysCollectedData.AutoSize = false;
            this.headerLblDaysCollectedData.MaximumSize = new Size(100, 0);
            // 
            // headerStripDaysCollectedData
            // 
            this.headerStripDaysCollectedData.Dock = System.Windows.Forms.DockStyle.Fill;
            this.headerStripDaysCollectedData.ForeColor = System.Drawing.Color.Black;
            this.headerStripDaysCollectedData.Location = new System.Drawing.Point(0, 0);
            this.headerStripDaysCollectedData.Name = "headerStripDaysCollectedData";
            this.headerStripDaysCollectedData.Size = new System.Drawing.Size(489, 40);
            this.headerStripDaysCollectedData.TabIndex = 19;
            this.headerStripDaysCollectedData.Text = "What date range, days and time period within those days should be used to calculate the performance baseline for this ser" +
    "ver?";
            this.headerStripDaysCollectedData.WordWrap = true;
            // 
            // automaticBaselineRadioButton
            // 
            this.automaticBaselineRadioButton.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.automaticBaselineRadioButton.AutoSize = true;
            this.automaticBaselineRadioButton.Checked = true;
            this.baselineMainContainer.SetColumnSpan(this.automaticBaselineRadioButton, 4);
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
            // customBaselineRadioButton
            // 
            this.customBaselineRadioButton.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.customBaselineRadioButton.AutoSize = true;
            this.baselineMainContainer.SetColumnSpan(this.customBaselineRadioButton, 4);
            this.customBaselineRadioButton.Location = new System.Drawing.Point(33, 108);
            this.customBaselineRadioButton.Name = "customBaselineRadioButton";
            this.customBaselineRadioButton.Size = new System.Drawing.Size(60, 17);
            this.customBaselineRadioButton.TabIndex = 41;
            this.customBaselineRadioButton.Text = "Custom";
            this.customBaselineRadioButton.UseVisualStyleBackColor = true;
            this.customBaselineRadioButton.CheckedChanged += new System.EventHandler(this.customBaselineRadioButton_CheckedChanged);
            // 
            // customDateMainContainer
            // 
            this.baselineMainContainer.SetColumnSpan(this.customDateMainContainer, 4);
            this.customDateMainContainer.Controls.Add(this.customDateFromLabel);
            this.customDateMainContainer.Controls.Add(this.customDateFromCombo);
            this.customDateMainContainer.Controls.Add(this.customDateToLabel);
            this.customDateMainContainer.Controls.Add(this.customDateToCombo);
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
            // lblBeginTime
            // 
            this.lblBeginTime.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.lblBeginTime.AutoSize = true;
            this.lblBeginTime.Location = new System.Drawing.Point(3, 9);
            this.lblBeginTime.Margin = new System.Windows.Forms.Padding(3, 5, 3, 5);
            this.lblBeginTime.Name = "lblBeginTime";
            this.lblBeginTime.Size = new System.Drawing.Size(33, 13);
            this.lblBeginTime.TabIndex = 55;
            this.lblBeginTime.Text = "From:";
            // 
            // beginTimeCombo
            // 
            this.beginScheduleTimeCombo.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.beginScheduleTimeCombo.AutoFillTime = Infragistics.Win.UltraWinMaskedEdit.AutoFillTime.Midnight;
            dropDownEditorButtonBegin.Key = "DropDownList";
            dropDownEditorButtonBegin.PreferredDropDownSize = new System.Drawing.Size(0, 0);
            this.beginScheduleTimeCombo.ButtonsRight.Add(dropDownEditorButtonBegin);
            this.beginScheduleTimeCombo.DateTime = new System.DateTime(2008, 8, 1, 8, 0, 0, 0);
            this.beginScheduleTimeCombo.DropDownButtonDisplayStyle = Infragistics.Win.ButtonDisplayStyle.Never;
            this.beginScheduleTimeCombo.ListInterval = System.TimeSpan.Parse("00:30:00");
            this.beginScheduleTimeCombo.Location = new System.Drawing.Point(42, 5);
            this.beginScheduleTimeCombo.Margin = new System.Windows.Forms.Padding(3, 5, 3, 5);
            this.beginScheduleTimeCombo.MaskInput = "{time}";
            this.beginScheduleTimeCombo.Name = "beginTimeCombo";
            this.beginScheduleTimeCombo.Nullable = false;
            this.beginScheduleTimeCombo.Size = new System.Drawing.Size(88, 21);
            this.beginScheduleTimeCombo.TabIndex = 53;
            this.beginScheduleTimeCombo.Time = System.TimeSpan.Parse("08:00:00");
            this.beginScheduleTimeCombo.Value = new System.DateTime(2008, 8, 1, 8, 0, 0, 0);
            this.beginScheduleTimeCombo.ValueChanged += new System.EventHandler(this.Update);
            this.beginScheduleTimeCombo.Enabled = false;
            // 
            // lblEndTime
            // 
            this.lblEndTime.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.lblEndTime.AutoSize = true;
            this.lblEndTime.Location = new System.Drawing.Point(136, 9);
            this.lblEndTime.Margin = new System.Windows.Forms.Padding(3, 5, 3, 5);
            this.lblEndTime.Name = "lblEndTime";
            this.lblEndTime.Size = new System.Drawing.Size(23, 13);
            this.lblEndTime.TabIndex = 56;
            this.lblEndTime.Text = "To:";
            // 
            // endTimeCombo
            // 
            this.endScheduleTimeCombo.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.endScheduleTimeCombo.AutoFillTime = Infragistics.Win.UltraWinMaskedEdit.AutoFillTime.Midnight;
            dropDownEditorButtonEnd.Key = "DropDownList";
            dropDownEditorButtonEnd.PreferredDropDownSize = new System.Drawing.Size(0, 0);
            this.endScheduleTimeCombo.ButtonsRight.Add(dropDownEditorButtonEnd);
            this.endScheduleTimeCombo.DateTime = new System.DateTime(2008, 8, 1, 17, 0, 0, 0);
            this.endScheduleTimeCombo.DropDownButtonDisplayStyle = Infragistics.Win.ButtonDisplayStyle.Never;
            this.endScheduleTimeCombo.ListInterval = System.TimeSpan.Parse("00:30:00");
            this.endScheduleTimeCombo.Location = new System.Drawing.Point(165, 5);
            this.endScheduleTimeCombo.Margin = new System.Windows.Forms.Padding(3, 5, 3, 5);
            this.endScheduleTimeCombo.MaskInput = "{time}";
            this.endScheduleTimeCombo.Name = "endTimeCombo";
            this.endScheduleTimeCombo.Nullable = false;
            this.endScheduleTimeCombo.Size = new System.Drawing.Size(88, 21);
            this.endScheduleTimeCombo.TabIndex = 54;
            this.endScheduleTimeCombo.Time = System.TimeSpan.Parse("17:00:00");
            this.endScheduleTimeCombo.Value = new System.DateTime(2008, 8, 1, 17, 0, 0, 0);
            this.endScheduleTimeCombo.ValueChanged += new System.EventHandler(this.Update);
            this.endScheduleTimeCombo.Enabled = false;
            // 
            // timeDateContainer
            // 
            this.baselineMainContainer.SetColumnSpan(this.timeDateContainer, 4);
            this.timeDateContainer.Controls.Add(this.lblBeginTime);
            this.timeDateContainer.Controls.Add(this.beginScheduleTimeCombo);
            this.timeDateContainer.Controls.Add(this.lblEndTime);
            this.timeDateContainer.Controls.Add(this.endScheduleTimeCombo);
            this.timeDateContainer.Dock = System.Windows.Forms.DockStyle.Fill;
            this.timeDateContainer.Location = new System.Drawing.Point(83, 150);
            this.timeDateContainer.Name = "timeDateContainer";
            this.timeDateContainer.Size = new System.Drawing.Size(409, 40);
            this.timeDateContainer.TabIndex = 64;

            // 
            // daysChkContainer
            // 
            this.daysChkContainer.ColumnCount = 7;
            this.baselineMainContainer.SetColumnSpan(this.daysChkContainer, 4);
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
            this.daysChkContainer.Location = new System.Drawing.Point(33, 33);
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
            //this.mondayCheckBox.CheckState = System.Windows.Forms.CheckState.Checked;
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
            //this.tuesdayCheckBox.CheckState = System.Windows.Forms.CheckState.Checked;
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
            //this.wednesdayCheckBox.CheckState = System.Windows.Forms.CheckState.Checked;
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
            //this.thursdayCheckBox.CheckState = System.Windows.Forms.CheckState.Checked;
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
            //this.fridayCheckBox.CheckState = System.Windows.Forms.CheckState.Checked;
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
            // addButton
            // 
            this.addButton.AutoSize = true;
            this.addButton.BackColor = System.Drawing.SystemColors.Control;
            this.addButton.DialogResult = System.Windows.Forms.DialogResult.None;
            this.addButton.Location = new System.Drawing.Point(25, 550);
            this.addButton.Name = "addButton";
            this.addButton.Size = new System.Drawing.Size(75, 25);
            this.addButton.TabIndex = 1;
            this.addButton.Text = "Add";
            this.addButton.UseVisualStyleBackColor = false;
            this.addButton.Click += new System.EventHandler(this.addButton_Click);//SQLdm 10.0 (Tarun Sapra)- Added an event handler to check time range
            //// 
            //// editButton
            //// 
            //this.editButton.AutoSize = true;
            //this.editButton.BackColor = System.Drawing.SystemColors.Control;
            //this.editButton.DialogResult = System.Windows.Forms.DialogResult.None;
            //this.editButton.Location = new System.Drawing.Point(100, 550);
            //this.editButton.Name = "editButton";
            //this.editButton.Size = new System.Drawing.Size(75, 25);
            //this.editButton.TabIndex = 1;
            //this.editButton.Text = "Edit";
            //this.editButton.UseVisualStyleBackColor = false;
            //this.editButton.Click += new System.EventHandler(this.editButton_Click);
            // 
            // deleteButton
            // 
            this.deleteButton.DialogResult = System.Windows.Forms.DialogResult.None;
            this.deleteButton.Location = new System.Drawing.Point(100, 550);
            this.deleteButton.Name = "deleteButton";
            this.deleteButton.Size = new System.Drawing.Size(75, 25);
            this.deleteButton.TabIndex = 1;
            this.deleteButton.Text = "Delete";
            this.deleteButton.UseVisualStyleBackColor = false;
            this.deleteButton.Click += new System.EventHandler(this.deleteButton_Click);
            // 
            // SaveButton
            // 
            this.saveButton.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.saveButton.Location = new System.Drawing.Point(356, 550);
            this.saveButton.Name = "saveButton";
            this.saveButton.Size = new System.Drawing.Size(75, 25);
            this.saveButton.TabIndex = 2;
            this.saveButton.Text = "Save";
            this.saveButton.UseVisualStyleBackColor = true;
            // 
            // cancelButton
            // 
            this.cancelButton.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.cancelButton.Location = new System.Drawing.Point(430, 550);
            this.cancelButton.Name = "cancelButton";
            this.cancelButton.Size = new System.Drawing.Size(75, 25);
            this.cancelButton.TabIndex = 3;
            this.cancelButton.Text = "Cancel";
            this.cancelButton.UseVisualStyleBackColor = true;
            // 
            // label5
            // 
            this.label5.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(3, 9);
            this.label5.Margin = new System.Windows.Forms.Padding(3, 5, 3, 5);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(33, 13);
            this.label5.TabIndex = 15;
            this.label5.Text = "From:";
            // 
            // label6
            // 
            this.label6.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(136, 9);
            this.label6.Margin = new System.Windows.Forms.Padding(3, 5, 3, 5);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(23, 13);
            this.label6.TabIndex = 16;
            this.label6.Text = "To:";
            // 
            // label1
            // 
            this.label1.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.label1.AutoSize = true;
            this.baselineMainContainer.SetColumnSpan(this.label1, 3);
            this.label1.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.label1.Location = new System.Drawing.Point(3, 37);
            this.label1.Margin = new System.Windows.Forms.Padding(3, 5, 3, 5);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(74, 13);
            this.label1.TabIndex = 85;
            this.label1.Text = "Baseline Name";
            
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
            this.panel1.TabIndex = 5;
            // 
            // header1label
            // 
            this.header1label.AutoSize = true;
            this.header1label.BackColor = System.Drawing.SystemColors.Control;
            this.header1label.Location = new System.Drawing.Point(0, 5);
            this.header1label.Name = "header1label";
            this.header1label.Size = new System.Drawing.Size(414, 13);
            this.header1label.TabIndex = 20;
            this.header1label.Text = "What date range should be used to apply baseline for this ser" +
    "ver?";
            this.header1label.Visible = false;
            // 
            // headerStrip1
            // 
            this.headerStrip1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.headerStrip1.ForeColor = System.Drawing.Color.Black;
            this.headerStrip1.Location = new System.Drawing.Point(0, 0);
            this.headerStrip1.Name = "headerStrip1";
            this.headerStrip1.Size = new System.Drawing.Size(489, 25);
            this.headerStrip1.TabIndex = 19;
            this.headerStrip1.Text = "What date range should be used to apply baseline for this ser" +
    "ver?";
            this.headerStrip1.WordWrap = false;
            // 
            // sundayCheckbox1
            // 
            this.sundayCheckbox1.AutoSize = true;
            this.sundayCheckbox1.Location = new System.Drawing.Point(3, 3);
            this.sundayCheckbox1.Name = "sundayCheckbox1";
            this.sundayCheckbox1.Size = new System.Drawing.Size(45, 17);
            this.sundayCheckbox1.TabIndex = 7;
            this.sundayCheckbox1.Text = "Sun";
            this.sundayCheckbox1.UseVisualStyleBackColor = true;
            this.sundayCheckbox1.CheckedChanged += new System.EventHandler(this.Update);
            // 
            // mondayCheckBox1
            // 
            this.mondayCheckBox1.AutoSize = true;
            this.mondayCheckBox1.Checked = true;
            this.mondayCheckBox1.CheckState = System.Windows.Forms.CheckState.Checked;
            this.mondayCheckBox1.Location = new System.Drawing.Point(54, 3);
            this.mondayCheckBox1.Name = "mondayCheckBox1";
            this.mondayCheckBox1.Size = new System.Drawing.Size(47, 17);
            this.mondayCheckBox1.TabIndex = 8;
            this.mondayCheckBox1.Text = "Mon";
            this.mondayCheckBox1.UseVisualStyleBackColor = true;
            this.mondayCheckBox1.CheckedChanged += new System.EventHandler(this.Update);
            // 
            // tuesdayCheckBox1
            // 
            this.tuesdayCheckBox1.AutoSize = true;
            this.tuesdayCheckBox1.Checked = true;
            this.tuesdayCheckBox1.CheckState = System.Windows.Forms.CheckState.Checked;
            this.tuesdayCheckBox1.Location = new System.Drawing.Point(107, 3);
            this.tuesdayCheckBox1.Name = "tuesdayCheckBox1";
            this.tuesdayCheckBox1.Size = new System.Drawing.Size(45, 17);
            this.tuesdayCheckBox1.TabIndex = 9;
            this.tuesdayCheckBox1.Text = "Tue";
            this.tuesdayCheckBox1.UseVisualStyleBackColor = true;
            this.tuesdayCheckBox1.CheckedChanged += new System.EventHandler(this.Update);
            // 
            // wednesdayCheckBox1
            // 
            this.wednesdayCheckBox1.AutoSize = true;
            this.wednesdayCheckBox1.Checked = true;
            this.wednesdayCheckBox1.CheckState = System.Windows.Forms.CheckState.Checked;
            this.wednesdayCheckBox1.Location = new System.Drawing.Point(158, 3);
            this.wednesdayCheckBox1.Name = "wednesdayCheckBox1";
            this.wednesdayCheckBox1.Size = new System.Drawing.Size(49, 17);
            this.wednesdayCheckBox1.TabIndex = 10;
            this.wednesdayCheckBox1.Text = "Wed";
            this.wednesdayCheckBox1.UseVisualStyleBackColor = true;
            this.wednesdayCheckBox1.CheckedChanged += new System.EventHandler(this.Update);
            // 
            // thursdayCheckBox1
            // 
            this.thursdayCheckBox1.AutoSize = true;
            this.thursdayCheckBox1.Checked = true;
            this.thursdayCheckBox1.CheckState = System.Windows.Forms.CheckState.Checked;
            this.thursdayCheckBox1.Location = new System.Drawing.Point(213, 3);
            this.thursdayCheckBox1.Name = "thursdayCheckBox1";
            this.thursdayCheckBox1.Size = new System.Drawing.Size(45, 17);
            this.thursdayCheckBox1.TabIndex = 11;
            this.thursdayCheckBox1.Text = "Thu";
            this.thursdayCheckBox1.UseVisualStyleBackColor = true;
            this.thursdayCheckBox1.CheckedChanged += new System.EventHandler(this.Update);
            // 
            // fridayCheckBox1
            // 
            this.fridayCheckBox1.AutoSize = true;
            this.fridayCheckBox1.Checked = true;
            this.fridayCheckBox1.CheckState = System.Windows.Forms.CheckState.Checked;
            this.fridayCheckBox1.Location = new System.Drawing.Point(264, 3);
            this.fridayCheckBox1.Name = "fridayCheckBox1";
            this.fridayCheckBox1.Size = new System.Drawing.Size(37, 17);
            this.fridayCheckBox1.TabIndex = 12;
            this.fridayCheckBox1.Text = "Fri";
            this.fridayCheckBox1.UseVisualStyleBackColor = true;
            this.fridayCheckBox1.CheckedChanged += new System.EventHandler(this.Update);
            // 
            // saturdayCheckBox1
            // 
            this.saturdayCheckBox1.AutoSize = true;
            this.saturdayCheckBox1.Location = new System.Drawing.Point(307, 3);
            this.saturdayCheckBox1.Name = "saturdayCheckBox1";
            this.saturdayCheckBox1.Size = new System.Drawing.Size(42, 17);
            this.saturdayCheckBox1.TabIndex = 13;
            this.saturdayCheckBox1.Text = "Sat";
            this.saturdayCheckBox1.UseVisualStyleBackColor = true;
            this.saturdayCheckBox1.CheckedChanged += new System.EventHandler(this.Update);
            // 
            // daysChkContainer1
            // 
            this.daysChkContainer1.ColumnCount = 7;
            this.baselineMainContainer.SetColumnSpan(this.daysChkContainer1, 4);
            this.daysChkContainer1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.daysChkContainer1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.daysChkContainer1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.daysChkContainer1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.daysChkContainer1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.daysChkContainer1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.daysChkContainer1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.daysChkContainer1.Controls.Add(this.sundayCheckbox1, 0, 0);
            this.daysChkContainer1.Controls.Add(this.mondayCheckBox1, 1, 0);
            this.daysChkContainer1.Controls.Add(this.tuesdayCheckBox1, 2, 0);
            this.daysChkContainer1.Controls.Add(this.wednesdayCheckBox1, 3, 0);
            this.daysChkContainer1.Controls.Add(this.thursdayCheckBox1, 4, 0);
            this.daysChkContainer1.Controls.Add(this.fridayCheckBox1, 5, 0);
            this.daysChkContainer1.Controls.Add(this.saturdayCheckBox1, 6, 0);
            this.daysChkContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.daysChkContainer1.Location = new System.Drawing.Point(33, 60);
            this.daysChkContainer1.Margin = new System.Windows.Forms.Padding(3, 3, 3, 5);
            this.daysChkContainer1.Name = "daysChkContainer1";
            this.daysChkContainer1.RowCount = 1;
            this.daysChkContainer1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.daysChkContainer1.Size = new System.Drawing.Size(459, 26);
            this.daysChkContainer1.TabIndex = 6;
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

            this.baselineMainContainer.Controls.Add(this.label1, 0, 1);
            this.baselineMainContainer.Controls.Add(this.comboBaselineName, 3, 1);
            this.baselineMainContainer.Controls.Add(this.panelDaysCollectedData, 0, 2);
            this.baselineMainContainer.Controls.Add(this.automaticBaselineRadioButton, 2, 2);
            this.baselineMainContainer.Controls.Add(this.customBaselineRadioButton, 2, 4);
            this.baselineMainContainer.Controls.Add(this.customDateMainContainer, 3, 5);
            //this.baselineMainContainer.Controls.Add(this.panelDefault, 0, 6);
            this.baselineMainContainer.Controls.Add(this.daysChkContainer1, 2, 6);
            this.baselineMainContainer.Controls.Add(this.timeDateContainer1, 3, 7);
            this.baselineMainContainer.Controls.Add(this.panel1, 0, 9);
            this.baselineMainContainer.Controls.Add(this.daysChkContainer, 2, 10);
            this.baselineMainContainer.Controls.Add(this.timeDateContainer, 3, 11);
            this.baselineMainContainer.Location = new System.Drawing.Point(5, 10);
            this.baselineMainContainer.Name = "baselineMainContainer";
            this.baselineMainContainer.RowCount = 11;
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
            this.baselineMainContainer.Size = new System.Drawing.Size(495, 532);
            this.baselineMainContainer.TabIndex = 0;
            
            // 
            // databasesBox
            // 
            this.comboBaselineName.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.comboBaselineName.FormattingEnabled = true;
            this.comboBaselineName.Location = new System.Drawing.Point(83, 34);
            this.comboBaselineName.Name = "comboBaselineName";
            this.comboBaselineName.Size = new System.Drawing.Size(275, 20);
            this.comboBaselineName.TabIndex = 53;
            this.comboBaselineName.Text = CHOOSE_DROPDOWN_OPTION;
            this.comboBaselineName.SelectedIndexChanged += new System.EventHandler(this.comboBaselineName_SelectedIndexChanged);
            this.comboBaselineName.Leave += new System.EventHandler(this.Update);//SQLDM-25914 fix (Pulkit Puri)--EDITING BASELINE NAME
            //
            // timeDateContainer1
            // 
            this.baselineMainContainer.SetColumnSpan(this.timeDateContainer1, 4);
            this.timeDateContainer1.Controls.Add(this.label5);
            this.timeDateContainer1.Controls.Add(this.beginCalculationTimeCombo);
            this.timeDateContainer1.Controls.Add(this.label6);
            this.timeDateContainer1.Controls.Add(this.endCalculationTimeCombo);
            this.timeDateContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.timeDateContainer1.Location = new System.Drawing.Point(83, 94);
            this.timeDateContainer1.Name = "timeDateContainer1";
            this.timeDateContainer1.Size = new System.Drawing.Size(409, 40);
            this.timeDateContainer1.TabIndex = 14;
            // 
            // beginTimeCombo1
            // 
            this.beginCalculationTimeCombo.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.beginCalculationTimeCombo.AutoFillTime = Infragistics.Win.UltraWinMaskedEdit.AutoFillTime.Midnight;
            dropDownEditorButton5.Key = "DropDownList";
            dropDownEditorButton5.PreferredDropDownSize = new System.Drawing.Size(0, 0);
            this.beginCalculationTimeCombo.ButtonsRight.Add(dropDownEditorButton5);
            this.beginCalculationTimeCombo.DateTime = new System.DateTime(2008, 8, 1, 8, 0, 0, 0);
            this.beginCalculationTimeCombo.DropDownButtonDisplayStyle = Infragistics.Win.ButtonDisplayStyle.Never;
            this.beginCalculationTimeCombo.ListInterval = System.TimeSpan.Parse("00:30:00");
            this.beginCalculationTimeCombo.Location = new System.Drawing.Point(42, 5);
            this.beginCalculationTimeCombo.Margin = new System.Windows.Forms.Padding(3, 5, 3, 5);
            this.beginCalculationTimeCombo.MaskInput = "{time}";
            this.beginCalculationTimeCombo.Name = "beginTimeCombo1";
            this.beginCalculationTimeCombo.Nullable = false;
            this.beginCalculationTimeCombo.Size = new System.Drawing.Size(88, 21);
            this.beginCalculationTimeCombo.TabIndex = 17;
            this.beginCalculationTimeCombo.Time = System.TimeSpan.Parse("08:00:00");
            this.beginCalculationTimeCombo.Value = new System.DateTime(2008, 8, 1, 8, 0, 0, 0);
            this.beginCalculationTimeCombo.ValueChanged += new System.EventHandler(this.Update);
            // 
            // endTimeCombo1
            // 
            this.endCalculationTimeCombo.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.endCalculationTimeCombo.AutoFillTime = Infragistics.Win.UltraWinMaskedEdit.AutoFillTime.Midnight;
            dropDownEditorButton6.Key = "DropDownList";
            dropDownEditorButton6.PreferredDropDownSize = new System.Drawing.Size(0, 0);
            this.endCalculationTimeCombo.ButtonsRight.Add(dropDownEditorButton6);
            this.endCalculationTimeCombo.DateTime = new System.DateTime(2008, 8, 1, 17, 0, 0, 0);
            this.endCalculationTimeCombo.DropDownButtonDisplayStyle = Infragistics.Win.ButtonDisplayStyle.Never;
            this.endCalculationTimeCombo.ListInterval = System.TimeSpan.Parse("00:30:00");
            this.endCalculationTimeCombo.Location = new System.Drawing.Point(165, 5);
            this.endCalculationTimeCombo.Margin = new System.Windows.Forms.Padding(3, 5, 3, 5);
            this.endCalculationTimeCombo.MaskInput = "{time}";
            this.endCalculationTimeCombo.Name = "endTimeCombo1";
            this.endCalculationTimeCombo.Nullable = false;
            this.endCalculationTimeCombo.Size = new System.Drawing.Size(88, 21);
            this.endCalculationTimeCombo.TabIndex = 18;
            this.endCalculationTimeCombo.Time = System.TimeSpan.Parse("17:00:00");
            this.endCalculationTimeCombo.Value = new System.DateTime(2008, 8, 1, 17, 0, 0, 0);
            this.endCalculationTimeCombo.ValueChanged += new System.EventHandler(this.Update);
            // 
            // panelDefault
            // 
            this.baselineMainContainer.SetColumnSpan(this.panelDefault, 4);
            //this.panelDefault.Controls.Add(this.headerLblDefault);
            //this.panelDefault.Controls.Add(this.propertiesHeaderStripDefault);
            this.panelDefault.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panelDefault.Location = new System.Drawing.Point(3, 177);
            this.panelDefault.Name = "panel2";
            this.panelDefault.Size = new System.Drawing.Size(489, 25);
            this.panelDefault.TabIndex = 63;
    //        // 
    //        // headerLblDefault
    //        // 
    //        this.headerLblDefault.AutoSize = true;
    //        this.headerLblDefault.BackColor = System.Drawing.SystemColors.Control;
    //        this.headerLblDefault.Location = new System.Drawing.Point(0, 6);
    //        this.headerLblDefault.Name = "headerLblDefault";
    //        this.headerLblDefault.Size = new System.Drawing.Size(489, 13);
    //        this.headerLblDefault.TabIndex = 60;
    //        this.headerLblDefault.Text = "Which days and time period within those days should be used to calculate the base" +
    //"line?";
    //        this.headerLblDefault.Visible = false;
    //        // 
    //        // propertiesHeaderStripDefault
    //        // 
    //        this.propertiesHeaderStripDefault.Dock = System.Windows.Forms.DockStyle.Fill;
    //        this.propertiesHeaderStripDefault.ForeColor = System.Drawing.Color.Black;
    //        this.propertiesHeaderStripDefault.Location = new System.Drawing.Point(0, 0);
    //        this.propertiesHeaderStripDefault.Name = "propertiesHeaderStripDefault";
    //        this.propertiesHeaderStripDefault.Size = new System.Drawing.Size(489, 25);
    //        this.propertiesHeaderStripDefault.TabIndex = 38;
    //        this.propertiesHeaderStripDefault.Text = "Which days and time period within those days should be used to calculate the base" +
    //"line?";
    //        this.propertiesHeaderStripDefault.WordWrap = false;
            // 
            // AddBaselineDialog
            //             
            this.AcceptButton = this.addButton;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.saveButton;
            this.ClientSize = new System.Drawing.Size(505, 589);

            this.Controls.Add(this.saveButton);
            this.Controls.Add(this.cancelButton);
            this.Controls.Add(this.addButton);
            //this.Controls.Add(this.editButton);
            this.Controls.Add(this.deleteButton);
            this.Controls.Add(this.baselineMainContainer);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            //SQLDM 10.1 (Pulkit Puri)- SQLDM-25971 
            this.HelpButton = true;
            this.HelpButtonClicked += new System.ComponentModel.CancelEventHandler(this.ManageBaseline_HelpButtonClicked);
            //SQLDM 10.1 (Pulkit Puri)
            this.Name = "AddBaselineDialog";
            this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Manage Baseline";

            this.customDateMainContainer.ResumeLayout(false);
            this.customDateMainContainer.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.customDateFromCombo)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.customDateToCombo)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.beginScheduleTimeCombo)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.endScheduleTimeCombo)).EndInit();
            
            this.panelDaysCollectedData.ResumeLayout(false);
            this.panelDaysCollectedData.PerformLayout();
            this.panelDefault.ResumeLayout(false);
            this.panelDefault.PerformLayout();
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.daysChkContainer1.ResumeLayout(false);
            this.daysChkContainer1.PerformLayout();
            this.daysChkContainer.ResumeLayout(false);
            this.daysChkContainer.PerformLayout();
            this.baselineMainContainer.ResumeLayout(false);
            this.baselineMainContainer.PerformLayout();
            this.timeDateContainer1.ResumeLayout(false);
            this.timeDateContainer1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.beginCalculationTimeCombo)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.endCalculationTimeCombo)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button addButton;
        //private System.Windows.Forms.Button editButton;
        private System.Windows.Forms.Button deleteButton;
        private System.Windows.Forms.Button saveButton;
        private System.Windows.Forms.Button cancelButton;
        private System.Windows.Forms.Label label1;
        // Revise Multiple Baseline for Independent Scheduling
        //SQLdm 10.1 (Srishti Purohit)
        private System.Windows.Forms.Panel panelDaysCollectedData;
        private System.Windows.Forms.Panel panel1;
        public System.Windows.Forms.CheckBox saturdayCheckBox1;
        public System.Windows.Forms.CheckBox fridayCheckBox1;
        public System.Windows.Forms.CheckBox thursdayCheckBox1;
        public System.Windows.Forms.CheckBox wednesdayCheckBox1;
        public System.Windows.Forms.CheckBox tuesdayCheckBox1;
        public System.Windows.Forms.CheckBox sundayCheckbox1;
        public System.Windows.Forms.CheckBox mondayCheckBox1;
        private System.Windows.Forms.TableLayoutPanel daysChkContainer1;
        private System.Windows.Forms.TableLayoutPanel baselineMainContainer;
        public System.Windows.Forms.ComboBox comboBaselineName;
        private System.Windows.Forms.FlowLayoutPanel timeDateContainer1;
        public Common.UI.Controls.TimeComboEditor endCalculationTimeCombo;
        public Common.UI.Controls.TimeComboEditor beginCalculationTimeCombo;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Label headerLblDaysCollectedData;
        private PropertiesHeaderStrip headerStripDaysCollectedData;
        private System.Windows.Forms.Label header1label;
        private PropertiesHeaderStrip headerStrip1;

        private System.Windows.Forms.Panel panelDefault;
        //private PropertiesHeaderStrip propertiesHeaderStripDefault;
        //private System.Windows.Forms.Label headerLblDefault;
        private System.Windows.Forms.Label lblEndTime;
        private System.Windows.Forms.Label lblBeginTime;
        //SQLdm 10.1.1 (srishti purohit)Not mandatory field as per 10.1.1
        private Common.UI.Controls.TimeComboEditor endScheduleTimeCombo;
        private Common.UI.Controls.TimeComboEditor beginScheduleTimeCombo;
        //Not madatory field as per 10.1.1
        private System.Windows.Forms.FlowLayoutPanel timeDateContainer;
        private System.Windows.Forms.TableLayoutPanel daysChkContainer;
        private System.Windows.Forms.CheckBox saturdayCheckBox;
        private System.Windows.Forms.CheckBox fridayCheckBox;
        private System.Windows.Forms.CheckBox thursdayCheckBox;
        private System.Windows.Forms.CheckBox wednesdayCheckBox;
        private System.Windows.Forms.CheckBox tuesdayCheckBox;
        private System.Windows.Forms.CheckBox sundayCheckbox;
        private System.Windows.Forms.CheckBox mondayCheckBox;

        // Revise Multiple Baseline for Independent Scheduling
        //SQLdm 10.1 (Srishti Purohit)
        private System.Windows.Forms.RadioButton customBaselineRadioButton;
        private System.Windows.Forms.RadioButton automaticBaselineRadioButton;
        private System.Windows.Forms.FlowLayoutPanel customDateMainContainer;
        private System.Windows.Forms.Label customDateToLabel;
        private Infragistics.Win.UltraWinSchedule.UltraCalendarCombo customDateToCombo;
        private System.Windows.Forms.Label customDateFromLabel;
        private Infragistics.Win.UltraWinSchedule.UltraCalendarCombo customDateFromCombo;
    }
}