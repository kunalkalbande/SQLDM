using System;
using System.Windows.Forms;
using Idera.SQLdm.Common;

namespace Idera.SQLdm.DesktopClient.Dialogs
{
    partial class MassMaintenanceModeDialog
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
            Infragistics.Win.UltraWinEditors.DropDownEditorButton dropDownEditorButton1 = new Infragistics.Win.UltraWinEditors.DropDownEditorButton("DropDownList");
            Infragistics.Win.UltraWinEditors.DropDownEditorButton dropDownEditorButton2 = new Infragistics.Win.UltraWinEditors.DropDownEditorButton("DropDownList");
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MassMaintenanceModeDialog));
            Infragistics.Win.UltraWinEditors.DropDownEditorButton dropDownEditorButton3 = new Infragistics.Win.UltraWinEditors.DropDownEditorButton("DropDownList");
            Infragistics.Win.UltraWinEditors.DropDownEditorButton dropDownEditorButton4 = new Infragistics.Win.UltraWinEditors.DropDownEditorButton("DropDownList");
            Infragistics.Win.UltraWinEditors.DropDownEditorButton dropDownEditorButton5 = new Infragistics.Win.UltraWinEditors.DropDownEditorButton("DropDownList");
            Infragistics.Win.UltraWinEditors.DropDownEditorButton dropDownEditorButton6 = new Infragistics.Win.UltraWinEditors.DropDownEditorButton("DropDownList");
            this.inputDayLimiter = new System.Windows.Forms.NumericUpDown();
            this.inputOfEveryMonthLimiter = new System.Windows.Forms.NumericUpDown();
            this.inputOfEveryTheMonthLimiter = new System.Windows.Forms.NumericUpDown();
            this.propertiesHeaderStrip27 = new Idera.SQLdm.DesktopClient.Controls.PropertiesHeaderStrip();
            this.informationBox15 = new Divelements.WizardFramework.InformationBox();
            this.mmMonthlyRecurringRadio = new System.Windows.Forms.RadioButton();
            this.mmMonthlyDayRadio = new System.Windows.Forms.RadioButton();
            this.mmMonthlyTheRadio = new System.Windows.Forms.RadioButton();
            this.mmMonthlyRecurringPanel = new System.Windows.Forms.Panel();
            this.flowLayoutPanel8 = new System.Windows.Forms.FlowLayoutPanel();
            this.label29 = new System.Windows.Forms.Label();
            this.mmMonthRecurringDuration = new Idera.SQLdm.Common.UI.Controls.TimeComboEditor();
            this.label30 = new System.Windows.Forms.Label();
            this.mmMonthlyOfEveryLabel = new System.Windows.Forms.Label();
            this.mmMonthlyLabel1 = new System.Windows.Forms.Label();
            this.WeekcomboBox = new System.Windows.Forms.ComboBox();
            this.DaycomboBox = new System.Windows.Forms.ComboBox();
            this.mmMonthlyOfTheEveryLabel = new System.Windows.Forms.Label();
            this.mmMonthlyLabel2 = new System.Windows.Forms.Label();
            this.mmMonthlyAtLabel = new System.Windows.Forms.Label();
            this.mmMonthRecurringBegin = new Idera.SQLdm.Common.UI.Controls.TimeComboEditor();
            this.serverDateTimeVersionBackgroundWorker = new System.ComponentModel.BackgroundWorker();
            this.okButton = new System.Windows.Forms.Button();
            this.cancelButton = new System.Windows.Forms.Button();
            this.verifyButton = new System.Windows.Forms.Button();
            this.buttonsPanel = new System.Windows.Forms.FlowLayoutPanel();
            this.treeView = new System.Windows.Forms.TreeView();
            this.maintenancePropertyPage = new Idera.SQLdm.DesktopClient.Controls.PropertyPage();
            this.splitContainer2 = new System.Windows.Forms.SplitContainer();
            this.maintenanceModeControlsContainer = new System.Windows.Forms.TableLayoutPanel();
            this.informationBox6 = new Divelements.WizardFramework.InformationBox();
            this.propertiesHeaderStrip13 = new Idera.SQLdm.DesktopClient.Controls.PropertiesHeaderStrip();
            this.mmNeverRadio = new System.Windows.Forms.RadioButton();
            this.mmAlwaysRadio = new System.Windows.Forms.RadioButton();
            this.mmRecurringRadio = new System.Windows.Forms.RadioButton();
            this.mmOnceRadio = new System.Windows.Forms.RadioButton();
            this.panel2 = new System.Windows.Forms.Panel();
            this.mmOncePanel = new System.Windows.Forms.Panel();
            this.tableLayoutPanel14 = new System.Windows.Forms.TableLayoutPanel();
            this.label21 = new System.Windows.Forms.Label();
            this.mmServerDateTime = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.mmOnceBeginTime = new Idera.SQLdm.Common.UI.Controls.TimeComboEditor();
            this.mmOnceBeginDate = new Infragistics.Win.UltraWinEditors.UltraDateTimeEditor();
            this.label7 = new System.Windows.Forms.Label();
            this.mmOnceStopTime = new Idera.SQLdm.Common.UI.Controls.TimeComboEditor();
            this.mmOnceStopDate = new Infragistics.Win.UltraWinEditors.UltraDateTimeEditor();
            this.propertiesHeaderStrip16 = new Idera.SQLdm.DesktopClient.Controls.PropertiesHeaderStrip();
            this.informationBox5 = new Divelements.WizardFramework.InformationBox();
            this.mmRecurringPanel = new System.Windows.Forms.Panel();
            this.flowLayoutPanel4 = new System.Windows.Forms.FlowLayoutPanel();
            this.label5 = new System.Windows.Forms.Label();
            this.mmRecurringDuration = new Idera.SQLdm.Common.UI.Controls.TimeComboEditor();
            this.flowLayoutPanel3 = new System.Windows.Forms.FlowLayoutPanel();
            this.label4 = new System.Windows.Forms.Label();
            this.mmRecurringBegin = new Idera.SQLdm.Common.UI.Controls.TimeComboEditor();
            this.informationBox4 = new Divelements.WizardFramework.InformationBox();
            this.mmBeginSatCheckbox = new System.Windows.Forms.CheckBox();
            this.mmBeginFriCheckbox = new System.Windows.Forms.CheckBox();
            this.mmBeginThurCheckbox = new System.Windows.Forms.CheckBox();
            this.mmBeginWedCheckbox = new System.Windows.Forms.CheckBox();
            this.mmBeginTueCheckbox = new System.Windows.Forms.CheckBox();
            this.mmBeginMonCheckbox = new System.Windows.Forms.CheckBox();
            this.mmBeginSunCheckbox = new System.Windows.Forms.CheckBox();
            this.propertiesHeaderStrip15 = new Idera.SQLdm.DesktopClient.Controls.PropertiesHeaderStrip();
            this.propertiesHeaderStrip14 = new Idera.SQLdm.DesktopClient.Controls.PropertiesHeaderStrip();
            this.maintenanceModeContentPage = new Idera.SQLdm.DesktopClient.Controls.Office2007PropertyPage();
            ((System.ComponentModel.ISupportInitialize)(this.inputDayLimiter)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.inputOfEveryMonthLimiter)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.inputOfEveryTheMonthLimiter)).BeginInit();
            this.mmMonthlyRecurringPanel.SuspendLayout();
            this.flowLayoutPanel8.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.mmMonthRecurringDuration)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.mmMonthRecurringBegin)).BeginInit();
            this.buttonsPanel.SuspendLayout();
            this.maintenancePropertyPage.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer2)).BeginInit();
            this.splitContainer2.Panel1.SuspendLayout();
            this.splitContainer2.Panel2.SuspendLayout();
            this.splitContainer2.SuspendLayout();
            this.maintenanceModeControlsContainer.SuspendLayout();
            this.panel2.SuspendLayout();
            this.mmOncePanel.SuspendLayout();
            this.tableLayoutPanel14.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.mmOnceBeginTime)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.mmOnceBeginDate)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.mmOnceStopTime)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.mmOnceStopDate)).BeginInit();
            this.mmRecurringPanel.SuspendLayout();
            this.flowLayoutPanel4.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.mmRecurringDuration)).BeginInit();
            this.flowLayoutPanel3.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.mmRecurringBegin)).BeginInit();
            this.SuspendLayout();
            // 
            // inputDayLimiter
            // 
            this.inputDayLimiter.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.inputDayLimiter.Location = new System.Drawing.Point(105, 86);
            this.inputDayLimiter.Maximum = new decimal(new int[] {
            30,
            0,
            0,
            0});
            this.inputDayLimiter.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.inputDayLimiter.Name = "inputDayLimiter";
            this.inputDayLimiter.Size = new System.Drawing.Size(40, 20);
            this.inputDayLimiter.TabIndex = 32;
            this.inputDayLimiter.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
            // 
            // inputOfEveryMonthLimiter
            // 
            this.inputOfEveryMonthLimiter.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.inputOfEveryMonthLimiter.Location = new System.Drawing.Point(205, 86);
            this.inputOfEveryMonthLimiter.Maximum = new decimal(new int[] {
            12,
            0,
            0,
            0});
            this.inputOfEveryMonthLimiter.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.inputOfEveryMonthLimiter.Name = "inputOfEveryMonthLimiter";
            this.inputOfEveryMonthLimiter.Size = new System.Drawing.Size(40, 20);
            this.inputOfEveryMonthLimiter.TabIndex = 32;
            this.inputOfEveryMonthLimiter.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
            // 
            // inputOfEveryTheMonthLimiter
            // 
            this.inputOfEveryTheMonthLimiter.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.inputOfEveryTheMonthLimiter.Location = new System.Drawing.Point(335, 126);
            this.inputOfEveryTheMonthLimiter.Maximum = new decimal(new int[] {
            12,
            0,
            0,
            0});
            this.inputOfEveryTheMonthLimiter.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.inputOfEveryTheMonthLimiter.Name = "inputOfEveryTheMonthLimiter";
            this.inputOfEveryTheMonthLimiter.Size = new System.Drawing.Size(40, 20);
            this.inputOfEveryTheMonthLimiter.TabIndex = 32;
            this.inputOfEveryTheMonthLimiter.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
            // 
            // propertiesHeaderStrip27
            // 
            this.propertiesHeaderStrip27.Dock = System.Windows.Forms.DockStyle.Top;
            this.propertiesHeaderStrip27.ForeColor = System.Drawing.Color.Black;
            this.propertiesHeaderStrip27.Location = new System.Drawing.Point(0, 0);
            this.propertiesHeaderStrip27.Name = "propertiesHeaderStrip27";
            this.propertiesHeaderStrip27.Size = new System.Drawing.Size(491, 25);
            this.propertiesHeaderStrip27.TabIndex = 55;
            this.propertiesHeaderStrip27.Text = "What time should this server be in maintenance mode?";
            this.propertiesHeaderStrip27.WordWrap = false;
            // 
            // informationBox15
            // 
            this.informationBox15.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.informationBox15.Location = new System.Drawing.Point(10, 31);
            this.informationBox15.Name = "informationBox15";
            this.informationBox15.Size = new System.Drawing.Size(491, 47);
            this.informationBox15.TabIndex = 54;
            this.informationBox15.Text = "Schedule times should be specified in the collection service time zone.";
            // 
            // mmMonthlyRecurringRadio
            // 
            this.mmMonthlyRecurringRadio.AutoSize = true;
            this.mmMonthlyRecurringRadio.Location = new System.Drawing.Point(63, 204);
            this.mmMonthlyRecurringRadio.Name = "mmMonthlyRecurringRadio";
            this.mmMonthlyRecurringRadio.Size = new System.Drawing.Size(195, 17);
            this.mmMonthlyRecurringRadio.TabIndex = 24;
            this.mmMonthlyRecurringRadio.Tag = "4";
            this.mmMonthlyRecurringRadio.Text = "Recurring every month at the specified time";
            this.mmMonthlyRecurringRadio.UseVisualStyleBackColor = true;
            this.mmMonthlyRecurringRadio.CheckedChanged += new System.EventHandler(this.mmMonthlyRecurringRadio_CheckedChanged);
            // 
            // mmMonthlyDayRadio
            // 
            this.mmMonthlyDayRadio.AutoSize = true;
            this.mmMonthlyDayRadio.Location = new System.Drawing.Point(55, 84);
            this.mmMonthlyDayRadio.Name = "mmMonthlyDayRadio";
            this.mmMonthlyDayRadio.Size = new System.Drawing.Size(47, 17);
            this.mmMonthlyDayRadio.TabIndex = 59;
            this.mmMonthlyDayRadio.Text = "Day ";
            this.mmMonthlyDayRadio.UseVisualStyleBackColor = true;
            this.mmMonthlyDayRadio.CheckedChanged += new System.EventHandler(this.mmMonthlyDayRadio_CheckedChanged);
            // 
            // mmMonthlyTheRadio
            // 
            this.mmMonthlyTheRadio.AutoSize = true;
            this.mmMonthlyTheRadio.Location = new System.Drawing.Point(55, 124);
            this.mmMonthlyTheRadio.Name = "mmMonthlyTheRadio";
            this.mmMonthlyTheRadio.Size = new System.Drawing.Size(47, 17);
            this.mmMonthlyTheRadio.TabIndex = 59;
            this.mmMonthlyTheRadio.Text = "The ";
            this.mmMonthlyTheRadio.UseVisualStyleBackColor = true;
            this.mmMonthlyTheRadio.CheckedChanged += new System.EventHandler(this.mmMonthlyTheRadio_CheckedChanged);
            // 
            // mmMonthlyRecurringPanel
            // 
            this.mmMonthlyRecurringPanel.BackColor = System.Drawing.Color.White;
            this.mmMonthlyRecurringPanel.Controls.Add(this.flowLayoutPanel8);
            this.mmMonthlyRecurringPanel.Controls.Add(this.propertiesHeaderStrip27);
            this.mmMonthlyRecurringPanel.Controls.Add(this.informationBox15);
            this.mmMonthlyRecurringPanel.Controls.Add(this.mmMonthlyDayRadio);
            this.mmMonthlyRecurringPanel.Controls.Add(this.inputDayLimiter);
            this.mmMonthlyRecurringPanel.Controls.Add(this.mmMonthlyOfEveryLabel);
            this.mmMonthlyRecurringPanel.Controls.Add(this.inputOfEveryMonthLimiter);
            this.mmMonthlyRecurringPanel.Controls.Add(this.mmMonthlyLabel1);
            this.mmMonthlyRecurringPanel.Controls.Add(this.mmMonthlyTheRadio);
            this.mmMonthlyRecurringPanel.Controls.Add(this.WeekcomboBox);
            this.mmMonthlyRecurringPanel.Controls.Add(this.DaycomboBox);
            this.mmMonthlyRecurringPanel.Controls.Add(this.mmMonthlyOfTheEveryLabel);
            this.mmMonthlyRecurringPanel.Controls.Add(this.inputOfEveryTheMonthLimiter);
            this.mmMonthlyRecurringPanel.Controls.Add(this.mmMonthlyLabel2);
            this.mmMonthlyRecurringPanel.Controls.Add(this.mmMonthlyAtLabel);
            this.mmMonthlyRecurringPanel.Controls.Add(this.mmMonthRecurringBegin);
            this.mmMonthlyRecurringPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.mmMonthlyRecurringPanel.Location = new System.Drawing.Point(0, 0);
            this.mmMonthlyRecurringPanel.Name = "mmMonthlyRecurringPanel";
            this.mmMonthlyRecurringPanel.Size = new System.Drawing.Size(491, 233);
            this.mmMonthlyRecurringPanel.TabIndex = 42;
            this.mmMonthlyRecurringPanel.Visible = false;
            // 
            // flowLayoutPanel8
            // 
            this.flowLayoutPanel8.Controls.Add(this.label29);
            this.flowLayoutPanel8.Controls.Add(this.mmMonthRecurringDuration);
            this.flowLayoutPanel8.Controls.Add(this.label30);
            this.flowLayoutPanel8.Location = new System.Drawing.Point(55, 180);
            this.flowLayoutPanel8.Name = "flowLayoutPanel8";
            this.flowLayoutPanel8.Size = new System.Drawing.Size(221, 31);
            this.flowLayoutPanel8.TabIndex = 57;
            // 
            // label29
            // 
            this.label29.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.label29.AutoSize = true;
            this.label29.Location = new System.Drawing.Point(5, 7);
            this.label29.Name = "label29";
            this.label29.Size = new System.Drawing.Size(19, 13);
            this.label29.TabIndex = 57;
            this.label29.Text = "for";
            // 
            // mmMonthRecurringDuration
            // 
            dropDownEditorButton1.Key = "DropDownList";
            dropDownEditorButton1.PreferredDropDownSize = new System.Drawing.Size(0, 0);
            this.mmMonthRecurringDuration.ButtonsRight.Add(dropDownEditorButton1);
            this.mmMonthRecurringDuration.DateTime = new System.DateTime(1900, 1, 1, 0, 0, 0, 0);
            this.mmMonthRecurringDuration.DropDownButtonDisplayStyle = Infragistics.Win.ButtonDisplayStyle.Never;
            this.mmMonthRecurringDuration.FormatString = "HH:mm";
            this.mmMonthRecurringDuration.ListInterval = System.TimeSpan.Parse("00:30:00");
            this.mmMonthRecurringDuration.Location = new System.Drawing.Point(30, 3);
            this.mmMonthRecurringDuration.MaskInput = "hh:mm";
            this.mmMonthRecurringDuration.Name = "mmMonthRecurringDuration";
            this.mmMonthRecurringDuration.Size = new System.Drawing.Size(69, 21);
            this.mmMonthRecurringDuration.TabIndex = 56;
            this.mmMonthRecurringDuration.Time = System.TimeSpan.Parse("00:00:00");
            this.mmMonthRecurringDuration.Value = new System.DateTime(1900, 1, 1, 0, 0, 0, 0);
            // 
            // label30
            // 
            this.label30.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.label30.AutoSize = true;
            this.label30.Location = new System.Drawing.Point(105, 7);
            this.label30.Name = "label30";
            this.label30.Size = new System.Drawing.Size(21, 13);
            this.label30.TabIndex = 57;
            this.label30.Text = "hrs";
            // 
            // mmMonthlyOfEveryLabel
            // 
            this.mmMonthlyOfEveryLabel.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.mmMonthlyOfEveryLabel.AutoSize = true;
            this.mmMonthlyOfEveryLabel.Location = new System.Drawing.Point(155, 87);
            this.mmMonthlyOfEveryLabel.Name = "mmMonthlyOfEveryLabel";
            this.mmMonthlyOfEveryLabel.Size = new System.Drawing.Size(45, 13);
            this.mmMonthlyOfEveryLabel.TabIndex = 97;
            this.mmMonthlyOfEveryLabel.Text = "of every";
            this.mmMonthlyOfEveryLabel.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // mmMonthlyLabel1
            // 
            this.mmMonthlyLabel1.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.mmMonthlyLabel1.AutoSize = true;
            this.mmMonthlyLabel1.Location = new System.Drawing.Point(250, 87);
            this.mmMonthlyLabel1.Margin = new System.Windows.Forms.Padding(3, 5, 3, 5);
            this.mmMonthlyLabel1.Name = "mmMonthlyLabel1";
            this.mmMonthlyLabel1.Size = new System.Drawing.Size(50, 13);
            this.mmMonthlyLabel1.TabIndex = 98;
            this.mmMonthlyLabel1.Text = "month(s) ";
            this.mmMonthlyLabel1.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // WeekcomboBox
            // 
            this.WeekcomboBox.FormattingEnabled = true;
            this.WeekcomboBox.Location = new System.Drawing.Point(105, 124);
            this.WeekcomboBox.Name = "WeekcomboBox";
            this.WeekcomboBox.Size = new System.Drawing.Size(81, 21);
            this.WeekcomboBox.TabIndex = 99;
            // 
            // DaycomboBox
            // 
            this.DaycomboBox.FormattingEnabled = true;
            this.DaycomboBox.Location = new System.Drawing.Point(195, 124);
            this.DaycomboBox.Name = "DaycomboBox";
            this.DaycomboBox.Size = new System.Drawing.Size(81, 21);
            this.DaycomboBox.TabIndex = 100;
            // 
            // mmMonthlyOfTheEveryLabel
            // 
            this.mmMonthlyOfTheEveryLabel.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.mmMonthlyOfTheEveryLabel.AutoSize = true;
            this.mmMonthlyOfTheEveryLabel.Location = new System.Drawing.Point(285, 127);
            this.mmMonthlyOfTheEveryLabel.Margin = new System.Windows.Forms.Padding(3, 5, 3, 5);
            this.mmMonthlyOfTheEveryLabel.Name = "mmMonthlyOfTheEveryLabel";
            this.mmMonthlyOfTheEveryLabel.Size = new System.Drawing.Size(45, 13);
            this.mmMonthlyOfTheEveryLabel.TabIndex = 97;
            this.mmMonthlyOfTheEveryLabel.Text = "of every";
            this.mmMonthlyOfTheEveryLabel.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // mmMonthlyLabel2
            // 
            this.mmMonthlyLabel2.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.mmMonthlyLabel2.AutoSize = true;
            this.mmMonthlyLabel2.Location = new System.Drawing.Point(380, 127);
            this.mmMonthlyLabel2.Margin = new System.Windows.Forms.Padding(3, 5, 3, 5);
            this.mmMonthlyLabel2.Name = "mmMonthlyLabel2";
            this.mmMonthlyLabel2.Size = new System.Drawing.Size(50, 13);
            this.mmMonthlyLabel2.TabIndex = 98;
            this.mmMonthlyLabel2.Text = "month(s) ";
            this.mmMonthlyLabel2.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // mmMonthlyAtLabel
            // 
            this.mmMonthlyAtLabel.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.mmMonthlyAtLabel.AutoSize = true;
            this.mmMonthlyAtLabel.Location = new System.Drawing.Point(60, 156);
            this.mmMonthlyAtLabel.Name = "mmMonthlyAtLabel";
            this.mmMonthlyAtLabel.Size = new System.Drawing.Size(19, 13);
            this.mmMonthlyAtLabel.TabIndex = 99;
            this.mmMonthlyAtLabel.Text = "at ";
            this.mmMonthlyAtLabel.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // mmMonthRecurringBegin
            // 
            dropDownEditorButton2.Key = "DropDownList";
            dropDownEditorButton2.PreferredDropDownSize = new System.Drawing.Size(0, 0);
            this.mmMonthRecurringBegin.ButtonsRight.Add(dropDownEditorButton2);
            this.mmMonthRecurringBegin.DateTime = new System.DateTime(1900, 1, 1, 2, 0, 0, 0);
            this.mmMonthRecurringBegin.DropDownButtonDisplayStyle = Infragistics.Win.ButtonDisplayStyle.Never;
            this.mmMonthRecurringBegin.ListInterval = System.TimeSpan.Parse("00:30:00");
            this.mmMonthRecurringBegin.Location = new System.Drawing.Point(82, 150);
            this.mmMonthRecurringBegin.MaskInput = "{time}";
            this.mmMonthRecurringBegin.Name = "mmMonthRecurringBegin";
            this.mmMonthRecurringBegin.Size = new System.Drawing.Size(90, 21);
            this.mmMonthRecurringBegin.TabIndex = 53;
            this.mmMonthRecurringBegin.Time = System.TimeSpan.Parse("02:00:00");
            this.mmMonthRecurringBegin.Value = new System.DateTime(1900, 1, 1, 2, 0, 0, 0);
            // 
            // serverDateTimeVersionBackgroundWorker
            // 
            this.serverDateTimeVersionBackgroundWorker.WorkerSupportsCancellation = true;
            this.serverDateTimeVersionBackgroundWorker.DoWork += new System.ComponentModel.DoWorkEventHandler(this.serverDateTimeVersionBackgroundWorker_DoWork);
            this.serverDateTimeVersionBackgroundWorker.RunWorkerCompleted += new System.ComponentModel.RunWorkerCompletedEventHandler(this.serverDateTimeVersionBackgroundWorker_RunWorkerCompleted);
            // 
            // okButton
            // 
            this.okButton.Location = new System.Drawing.Point(549, 3);
            this.okButton.Name = "okButton";
            this.okButton.Size = new System.Drawing.Size(75, 23);
            this.okButton.TabIndex = 0;
            this.okButton.Text = "OK";
            this.okButton.Click += new System.EventHandler(this.okButton_Click);
            // 
            // cancelButton
            // 
            this.cancelButton.Location = new System.Drawing.Point(711, 3);
            this.cancelButton.Name = "cancelButton";
            this.cancelButton.Size = new System.Drawing.Size(75, 23);
            this.cancelButton.TabIndex = 1;
            this.cancelButton.Text = "Cancel";
            this.cancelButton.Click += new System.EventHandler(this.cancelButton_Click);
            // 
            // verifyButton
            // 
            this.verifyButton.Location = new System.Drawing.Point(630, 3);
            this.verifyButton.Name = "verifyButton";
            this.verifyButton.Size = new System.Drawing.Size(75, 23);
            this.verifyButton.TabIndex = 2;
            this.verifyButton.Text = "Verify";
            this.verifyButton.Visible = false;
            // 
            // buttonsPanel
            // 
            this.buttonsPanel.AutoSize = true;
            this.buttonsPanel.Controls.Add(this.cancelButton);
            this.buttonsPanel.Controls.Add(this.verifyButton);
            this.buttonsPanel.Controls.Add(this.okButton);
            this.buttonsPanel.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.buttonsPanel.Location = new System.Drawing.Point(0, 525);
            this.buttonsPanel.Name = "buttonsPanel";
            this.buttonsPanel.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
            this.buttonsPanel.Size = new System.Drawing.Size(789, 29);
            this.buttonsPanel.TabIndex = 0;
            // 
            // treeView
            // 
            this.treeView.Dock = System.Windows.Forms.DockStyle.Fill;
            this.treeView.Location = new System.Drawing.Point(0, 0);
            this.treeView.Name = "treeView";
            this.treeView.Size = new System.Drawing.Size(263, 463);
            this.treeView.TabIndex = 0;
            // 
            // maintenancePropertyPage
            // 
            this.maintenancePropertyPage.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.maintenancePropertyPage.Controls.Add(this.splitContainer2);
            this.maintenancePropertyPage.Controls.Add(this.maintenanceModeContentPage);
            this.maintenancePropertyPage.ForeColor = System.Drawing.SystemColors.ControlText;
            this.maintenancePropertyPage.Location = new System.Drawing.Point(0, 0);
            this.maintenancePropertyPage.Name = "maintenancePropertyPage";
            this.maintenancePropertyPage.Size = new System.Drawing.Size(789, 550);
            this.maintenancePropertyPage.TabIndex = 1;
            this.maintenancePropertyPage.Text = "Maintenance Mode";
            // 
            // splitContainer2
            // 
            this.splitContainer2.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.splitContainer2.Location = new System.Drawing.Point(12, 52);
            this.splitContainer2.Name = "splitContainer2";
            // 
            // splitContainer2.Panel1
            // 
            this.splitContainer2.Panel1.Controls.Add(this.treeView);
            this.splitContainer2.Panel1MinSize = 150;
            // 
            // splitContainer2.Panel2
            // 
            this.splitContainer2.Panel2.Controls.Add(this.maintenanceModeControlsContainer);
            this.splitContainer2.Panel2MinSize = 250;
            this.splitContainer2.Size = new System.Drawing.Size(765, 463);
            this.splitContainer2.SplitterDistance = 263;
            this.splitContainer2.SplitterWidth = 5;
            this.splitContainer2.TabIndex = 3;
            // 
            // maintenanceModeControlsContainer
            // 
            this.maintenanceModeControlsContainer.BackColor = System.Drawing.Color.White;
            this.maintenanceModeControlsContainer.ColumnCount = 2;
            this.maintenanceModeControlsContainer.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 60F));
            this.maintenanceModeControlsContainer.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.maintenanceModeControlsContainer.Controls.Add(this.informationBox6, 0, 0);
            this.maintenanceModeControlsContainer.Controls.Add(this.propertiesHeaderStrip13, 0, 1);
            this.maintenanceModeControlsContainer.Controls.Add(this.mmNeverRadio, 1, 2);
            this.maintenanceModeControlsContainer.Controls.Add(this.mmAlwaysRadio, 1, 3);
            this.maintenanceModeControlsContainer.Controls.Add(this.mmRecurringRadio, 1, 4);
            this.maintenanceModeControlsContainer.Controls.Add(this.mmOnceRadio, 1, 6);
            this.maintenanceModeControlsContainer.Controls.Add(this.mmMonthlyRecurringRadio, 1, 5);
            this.maintenanceModeControlsContainer.Controls.Add(this.panel2, 0, 7);
            this.maintenanceModeControlsContainer.Dock = System.Windows.Forms.DockStyle.Fill;
            this.maintenanceModeControlsContainer.Location = new System.Drawing.Point(0, 0);
            this.maintenanceModeControlsContainer.Name = "maintenanceModeControlsContainer";
            this.maintenanceModeControlsContainer.RowCount = 8;
            this.maintenanceModeControlsContainer.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.maintenanceModeControlsContainer.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.maintenanceModeControlsContainer.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.maintenanceModeControlsContainer.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.maintenanceModeControlsContainer.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.maintenanceModeControlsContainer.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.maintenanceModeControlsContainer.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.maintenanceModeControlsContainer.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.maintenanceModeControlsContainer.Size = new System.Drawing.Size(497, 463);
            this.maintenanceModeControlsContainer.TabIndex = 2;
            // 
            // informationBox6
            // 
            this.informationBox6.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.maintenanceModeControlsContainer.SetColumnSpan(this.informationBox6, 2);
            this.informationBox6.Location = new System.Drawing.Point(3, 3);
            this.informationBox6.Name = "informationBox6";
            this.informationBox6.Size = new System.Drawing.Size(491, 72);
            this.informationBox6.TabIndex = 44;
            this.informationBox6.Text = resources.GetString("informationBox6.Text");
            // 
            // propertiesHeaderStrip13
            // 
            this.maintenanceModeControlsContainer.SetColumnSpan(this.propertiesHeaderStrip13, 2);
            this.propertiesHeaderStrip13.Dock = System.Windows.Forms.DockStyle.Fill;
            this.propertiesHeaderStrip13.Location = new System.Drawing.Point(3, 81);
            this.propertiesHeaderStrip13.Name = "propertiesHeaderStrip13";
            this.propertiesHeaderStrip13.Size = new System.Drawing.Size(491, 25);
            this.propertiesHeaderStrip13.TabIndex = 17;
            this.propertiesHeaderStrip13.Text = "When should this server be in maintenance mode?";
            this.propertiesHeaderStrip13.WordWrap = false;
            // 
            // mmNeverRadio
            // 
            this.mmNeverRadio.AutoSize = true;
            this.mmNeverRadio.Checked = true;
            this.mmNeverRadio.Location = new System.Drawing.Point(63, 112);
            this.mmNeverRadio.Name = "mmNeverRadio";
            this.mmNeverRadio.Size = new System.Drawing.Size(54, 17);
            this.mmNeverRadio.TabIndex = 20;
            this.mmNeverRadio.TabStop = true;
            this.mmNeverRadio.Tag = "0";
            this.mmNeverRadio.Text = "Never";
            this.mmNeverRadio.UseVisualStyleBackColor = true;
            this.mmNeverRadio.CheckedChanged += new System.EventHandler(this.mmNeverRadio_CheckedChanged);
            // 
            // mmAlwaysRadio
            // 
            this.mmAlwaysRadio.AutoSize = true;
            this.mmAlwaysRadio.Location = new System.Drawing.Point(63, 135);
            this.mmAlwaysRadio.Name = "mmAlwaysRadio";
            this.mmAlwaysRadio.Size = new System.Drawing.Size(111, 17);
            this.mmAlwaysRadio.TabIndex = 21;
            this.mmAlwaysRadio.Tag = "1";
            this.mmAlwaysRadio.Text = "Until further notice";
            this.mmAlwaysRadio.UseVisualStyleBackColor = true;
            this.mmAlwaysRadio.CheckedChanged += new System.EventHandler(this.mmAlwaysRadio_CheckedChanged);
            // 
            // mmRecurringRadio
            // 
            this.mmRecurringRadio.AutoSize = true;
            this.mmRecurringRadio.Location = new System.Drawing.Point(63, 158);
            this.mmRecurringRadio.Name = "mmRecurringRadio";
            this.mmRecurringRadio.Size = new System.Drawing.Size(226, 17);
            this.mmRecurringRadio.TabIndex = 22;
            this.mmRecurringRadio.Tag = "2";
            this.mmRecurringRadio.Text = "Recurring every week at the specified time";
            this.mmRecurringRadio.UseVisualStyleBackColor = true;
            this.mmRecurringRadio.CheckedChanged += new System.EventHandler(this.mmWeeklyRadio_CheckedChanged);
            // 
            // mmOnceRadio
            // 
            this.mmOnceRadio.AutoSize = true;
            this.mmOnceRadio.Location = new System.Drawing.Point(63, 181);
            this.mmOnceRadio.Name = "mmOnceRadio";
            this.mmOnceRadio.Size = new System.Drawing.Size(62, 17);
            this.mmOnceRadio.TabIndex = 23;
            this.mmOnceRadio.Tag = "3";
            this.mmOnceRadio.Text = "Occurring once at the specified time";
            this.mmOnceRadio.UseVisualStyleBackColor = true;
            this.mmOnceRadio.CheckedChanged += new System.EventHandler(this.mmOnceRadio_CheckedChanged);
            // 
            // panel2
            // 
            this.maintenanceModeControlsContainer.SetColumnSpan(this.panel2, 2);
            this.panel2.Controls.Add(this.mmOncePanel);
            this.panel2.Controls.Add(this.mmRecurringPanel);
            this.panel2.Controls.Add(this.mmMonthlyRecurringPanel);
            this.panel2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel2.Location = new System.Drawing.Point(3, 227);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(491, 233);
            this.panel2.TabIndex = 1;
            // 
            // mmOncePanel
            // 
            this.mmOncePanel.BackColor = System.Drawing.Color.White;
            this.mmOncePanel.Controls.Add(this.tableLayoutPanel14);
            this.mmOncePanel.Controls.Add(this.propertiesHeaderStrip16);
            this.mmOncePanel.Controls.Add(this.informationBox5);
            this.mmOncePanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.mmOncePanel.Location = new System.Drawing.Point(0, 0);
            this.mmOncePanel.Name = "mmOncePanel";
            this.mmOncePanel.Size = new System.Drawing.Size(491, 233);
            this.mmOncePanel.TabIndex = 43;
            this.mmOncePanel.Visible = false;
            // 
            // tableLayoutPanel14
            // 
            this.tableLayoutPanel14.AutoSize = true;
            this.tableLayoutPanel14.ColumnCount = 3;
            this.tableLayoutPanel14.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel14.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel14.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel14.Controls.Add(this.label21, 0, 0);
            this.tableLayoutPanel14.Controls.Add(this.mmServerDateTime, 1, 0);
            this.tableLayoutPanel14.Controls.Add(this.label6, 0, 1);
            this.tableLayoutPanel14.Controls.Add(this.mmOnceBeginTime, 2, 1);
            this.tableLayoutPanel14.Controls.Add(this.mmOnceBeginDate, 1, 1);
            this.tableLayoutPanel14.Controls.Add(this.label7, 0, 2);
            this.tableLayoutPanel14.Controls.Add(this.mmOnceStopTime, 2, 2);
            this.tableLayoutPanel14.Controls.Add(this.mmOnceStopDate, 1, 2);
            this.tableLayoutPanel14.Location = new System.Drawing.Point(45, 85);
            this.tableLayoutPanel14.Name = "tableLayoutPanel14";
            this.tableLayoutPanel14.RowCount = 3;
            this.tableLayoutPanel14.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel14.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel14.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel14.Size = new System.Drawing.Size(363, 71);
            this.tableLayoutPanel14.TabIndex = 78;
            // 
            // label21
            // 
            this.label21.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.label21.AutoSize = true;
            this.label21.Location = new System.Drawing.Point(3, 0);
            this.label21.Name = "label21";
            this.label21.Size = new System.Drawing.Size(72, 13);
            this.label21.TabIndex = 77;
            this.label21.Text = "Service Time:";
            // 
            // mmServerDateTime
            // 
            this.mmServerDateTime.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.mmServerDateTime.AutoSize = true;
            this.mmServerDateTime.Location = new System.Drawing.Point(81, 0);
            this.mmServerDateTime.Name = "mmServerDateTime";
            this.mmServerDateTime.Size = new System.Drawing.Size(67, 13);
            this.mmServerDateTime.TabIndex = 78;
            this.mmServerDateTime.Text = "Refreshing...";
            // 
            // label6
            // 
            this.label6.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(3, 20);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(54, 13);
            this.label6.TabIndex = 79;
            this.label6.Text = "Start time:";
            // 
            // mmOnceBeginTime
            // 
            this.mmOnceBeginTime.Anchor = System.Windows.Forms.AnchorStyles.Left;
            dropDownEditorButton3.Key = "DropDownList";
            dropDownEditorButton3.PreferredDropDownSize = new System.Drawing.Size(0, 0);
            this.mmOnceBeginTime.ButtonsRight.Add(dropDownEditorButton3);
            this.mmOnceBeginTime.DateTime = new System.DateTime(2009, 2, 11, 0, 0, 0, 0);
            this.mmOnceBeginTime.DropDownButtonDisplayStyle = Infragistics.Win.ButtonDisplayStyle.Never;
            this.mmOnceBeginTime.FormatString = "hh:mm tt";
            this.mmOnceBeginTime.ListInterval = System.TimeSpan.Parse("00:30:00");
            this.mmOnceBeginTime.Location = new System.Drawing.Point(183, 16);
            this.mmOnceBeginTime.MaskInput = "hh:mm tt";
            this.mmOnceBeginTime.Name = "mmOnceBeginTime";
            this.mmOnceBeginTime.Size = new System.Drawing.Size(90, 21);
            this.mmOnceBeginTime.TabIndex = 81;
            this.mmOnceBeginTime.Time = System.TimeSpan.Parse("00:00:00");
            this.mmOnceBeginTime.Value = new System.DateTime(2009, 2, 11, 0, 0, 0, 0);
            // 
            // mmOnceBeginDate
            // 
            this.mmOnceBeginDate.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.mmOnceBeginDate.DateTime = new System.DateTime(2009, 4, 6, 0, 0, 0, 0);
            this.mmOnceBeginDate.Location = new System.Drawing.Point(81, 16);
            this.mmOnceBeginDate.Name = "mmOnceBeginDate";
            this.mmOnceBeginDate.Size = new System.Drawing.Size(96, 21);
            this.mmOnceBeginDate.TabIndex = 80;
            this.mmOnceBeginDate.Value = new System.DateTime(2009, 4, 6, 0, 0, 0, 0);
            // 
            // label7
            // 
            this.label7.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(3, 49);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(54, 13);
            this.label7.TabIndex = 82;
            this.label7.Text = "Stop time:";
            // 
            // mmOnceStopTime
            // 
            this.mmOnceStopTime.Anchor = System.Windows.Forms.AnchorStyles.Left;
            dropDownEditorButton4.Key = "DropDownList";
            dropDownEditorButton4.PreferredDropDownSize = new System.Drawing.Size(0, 0);
            this.mmOnceStopTime.ButtonsRight.Add(dropDownEditorButton4);
            this.mmOnceStopTime.DateTime = new System.DateTime(2009, 2, 11, 0, 0, 0, 0);
            this.mmOnceStopTime.DropDownButtonDisplayStyle = Infragistics.Win.ButtonDisplayStyle.Never;
            this.mmOnceStopTime.FormatString = "hh:mm tt";
            this.mmOnceStopTime.ListInterval = System.TimeSpan.Parse("00:30:00");
            this.mmOnceStopTime.Location = new System.Drawing.Point(183, 45);
            this.mmOnceStopTime.MaskInput = "hh:mm tt";
            this.mmOnceStopTime.Name = "mmOnceStopTime";
            this.mmOnceStopTime.Size = new System.Drawing.Size(90, 21);
            this.mmOnceStopTime.TabIndex = 84;
            this.mmOnceStopTime.Time = System.TimeSpan.Parse("00:00:00");
            this.mmOnceStopTime.Value = new System.DateTime(2009, 2, 11, 0, 0, 0, 0);
            // 
            // mmOnceStopDate
            // 
            this.mmOnceStopDate.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.mmOnceStopDate.DateTime = new System.DateTime(2009, 4, 6, 0, 0, 0, 0);
            this.mmOnceStopDate.Location = new System.Drawing.Point(81, 45);
            this.mmOnceStopDate.Name = "mmOnceStopDate";
            this.mmOnceStopDate.Size = new System.Drawing.Size(96, 21);
            this.mmOnceStopDate.TabIndex = 83;
            this.mmOnceStopDate.Value = new System.DateTime(2009, 4, 6, 0, 0, 0, 0);
            // 
            // propertiesHeaderStrip16
            // 
            this.propertiesHeaderStrip16.Dock = System.Windows.Forms.DockStyle.Top;
            this.propertiesHeaderStrip16.ForeColor = System.Drawing.Color.Black;
            this.propertiesHeaderStrip16.Location = new System.Drawing.Point(0, 0);
            this.propertiesHeaderStrip16.Name = "propertiesHeaderStrip16";
            this.propertiesHeaderStrip16.Size = new System.Drawing.Size(491, 25);
            this.propertiesHeaderStrip16.TabIndex = 55;
            this.propertiesHeaderStrip16.Text = "What time should this server be in maintenance mode?";
            this.propertiesHeaderStrip16.WordWrap = false;
            // 
            // informationBox5
            // 
            this.informationBox5.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.informationBox5.Location = new System.Drawing.Point(10, 31);
            this.informationBox5.Name = "informationBox5";
            this.informationBox5.Size = new System.Drawing.Size(491, 39);
            this.informationBox5.TabIndex = 54;
            this.informationBox5.Text = "Schedule times should be specified in the collection service time zone.";
            // 
            // mmRecurringPanel
            // 
            this.mmRecurringPanel.BackColor = System.Drawing.Color.White;
            this.mmRecurringPanel.Controls.Add(this.flowLayoutPanel4);
            this.mmRecurringPanel.Controls.Add(this.flowLayoutPanel3);
            this.mmRecurringPanel.Controls.Add(this.informationBox4);
            this.mmRecurringPanel.Controls.Add(this.mmBeginSatCheckbox);
            this.mmRecurringPanel.Controls.Add(this.mmBeginFriCheckbox);
            this.mmRecurringPanel.Controls.Add(this.mmBeginThurCheckbox);
            this.mmRecurringPanel.Controls.Add(this.mmBeginWedCheckbox);
            this.mmRecurringPanel.Controls.Add(this.mmBeginTueCheckbox);
            this.mmRecurringPanel.Controls.Add(this.mmBeginMonCheckbox);
            this.mmRecurringPanel.Controls.Add(this.mmBeginSunCheckbox);
            this.mmRecurringPanel.Controls.Add(this.propertiesHeaderStrip15);
            this.mmRecurringPanel.Controls.Add(this.propertiesHeaderStrip14);
            this.mmRecurringPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.mmRecurringPanel.Location = new System.Drawing.Point(0, 0);
            this.mmRecurringPanel.Name = "mmRecurringPanel";
            this.mmRecurringPanel.Size = new System.Drawing.Size(491, 233);
            this.mmRecurringPanel.TabIndex = 42;
            this.mmRecurringPanel.Visible = false;
            // 
            // flowLayoutPanel4
            // 
            this.flowLayoutPanel4.Controls.Add(this.label5);
            this.flowLayoutPanel4.Controls.Add(this.mmRecurringDuration);
            this.flowLayoutPanel4.Location = new System.Drawing.Point(43, 181);
            this.flowLayoutPanel4.Name = "flowLayoutPanel4";
            this.flowLayoutPanel4.Size = new System.Drawing.Size(221, 31);
            this.flowLayoutPanel4.TabIndex = 57;
            // 
            // label5
            // 
            this.label5.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(3, 7);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(68, 13);
            this.label5.TabIndex = 57;
            this.label5.Text = "Duration (hr):";
            // 
            // mmRecurringDuration
            // 
            dropDownEditorButton5.Key = "DropDownList";
            dropDownEditorButton5.PreferredDropDownSize = new System.Drawing.Size(0, 0);
            this.mmRecurringDuration.ButtonsRight.Add(dropDownEditorButton5);
            this.mmRecurringDuration.DateTime = new System.DateTime(2009, 2, 11, 0, 0, 0, 0);
            this.mmRecurringDuration.DropDownButtonDisplayStyle = Infragistics.Win.ButtonDisplayStyle.Never;
            this.mmRecurringDuration.FormatString = "HH:mm";
            this.mmRecurringDuration.ListInterval = System.TimeSpan.Parse("00:30:00");
            this.mmRecurringDuration.Location = new System.Drawing.Point(77, 3);
            this.mmRecurringDuration.MaskInput = "hh:mm";
            this.mmRecurringDuration.Name = "mmRecurringDuration";
            this.mmRecurringDuration.Size = new System.Drawing.Size(69, 21);
            this.mmRecurringDuration.TabIndex = 56;
            this.mmRecurringDuration.Time = System.TimeSpan.Parse("00:00:00");
            this.mmRecurringDuration.Value = new System.DateTime(2009, 2, 11, 0, 0, 0, 0);
            // 
            // flowLayoutPanel3
            // 
            this.flowLayoutPanel3.Controls.Add(this.label4);
            this.flowLayoutPanel3.Controls.Add(this.mmRecurringBegin);
            this.flowLayoutPanel3.Location = new System.Drawing.Point(43, 106);
            this.flowLayoutPanel3.Name = "flowLayoutPanel3";
            this.flowLayoutPanel3.Size = new System.Drawing.Size(221, 30);
            this.flowLayoutPanel3.TabIndex = 56;
            // 
            // label4
            // 
            this.label4.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(3, 7);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(54, 13);
            this.label4.TabIndex = 54;
            this.label4.Text = "Start time:";
            // 
            // mmRecurringBegin
            // 
            dropDownEditorButton6.Key = "DropDownList";
            dropDownEditorButton6.PreferredDropDownSize = new System.Drawing.Size(0, 0);
            this.mmRecurringBegin.ButtonsRight.Add(dropDownEditorButton6);
            this.mmRecurringBegin.DateTime = new System.DateTime(2008, 6, 30, 2, 0, 0, 0);
            this.mmRecurringBegin.DropDownButtonDisplayStyle = Infragistics.Win.ButtonDisplayStyle.Never;
            this.mmRecurringBegin.ListInterval = System.TimeSpan.Parse("00:30:00");
            this.mmRecurringBegin.Location = new System.Drawing.Point(63, 3);
            this.mmRecurringBegin.MaskInput = "{time}";
            this.mmRecurringBegin.Name = "mmRecurringBegin";
            this.mmRecurringBegin.Size = new System.Drawing.Size(90, 21);
            this.mmRecurringBegin.TabIndex = 53;
            this.mmRecurringBegin.Time = System.TimeSpan.Parse("02:00:00");
            this.mmRecurringBegin.Value = new System.DateTime(2008, 6, 30, 2, 0, 0, 0);
            // 
            // informationBox4
            // 
            this.informationBox4.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.informationBox4.Location = new System.Drawing.Point(10, 31);
            this.informationBox4.Name = "informationBox4";
            this.informationBox4.Size = new System.Drawing.Size(478, 47);
            this.informationBox4.TabIndex = 53;
            this.informationBox4.Text = "Schedule times should be specified in the collection service time zone.";
            // 
            // mmBeginSatCheckbox
            // 
            this.mmBeginSatCheckbox.AutoSize = true;
            this.mmBeginSatCheckbox.Location = new System.Drawing.Point(364, 84);
            this.mmBeginSatCheckbox.Name = "mmBeginSatCheckbox";
            this.mmBeginSatCheckbox.Size = new System.Drawing.Size(42, 17);
            this.mmBeginSatCheckbox.TabIndex = 50;
            this.mmBeginSatCheckbox.Text = "Sat";
            this.mmBeginSatCheckbox.UseVisualStyleBackColor = true;
            // 
            // mmBeginFriCheckbox
            // 
            this.mmBeginFriCheckbox.AutoSize = true;
            this.mmBeginFriCheckbox.Checked = true;
            this.mmBeginFriCheckbox.CheckState = System.Windows.Forms.CheckState.Checked;
            this.mmBeginFriCheckbox.Location = new System.Drawing.Point(320, 84);
            this.mmBeginFriCheckbox.Name = "mmBeginFriCheckbox";
            this.mmBeginFriCheckbox.Size = new System.Drawing.Size(37, 17);
            this.mmBeginFriCheckbox.TabIndex = 49;
            this.mmBeginFriCheckbox.Text = "Fri";
            this.mmBeginFriCheckbox.UseVisualStyleBackColor = true;
            // 
            // mmBeginThurCheckbox
            // 
            this.mmBeginThurCheckbox.AutoSize = true;
            this.mmBeginThurCheckbox.Checked = true;
            this.mmBeginThurCheckbox.CheckState = System.Windows.Forms.CheckState.Checked;
            this.mmBeginThurCheckbox.Location = new System.Drawing.Point(268, 84);
            this.mmBeginThurCheckbox.Name = "mmBeginThurCheckbox";
            this.mmBeginThurCheckbox.Size = new System.Drawing.Size(45, 17);
            this.mmBeginThurCheckbox.TabIndex = 48;
            this.mmBeginThurCheckbox.Text = "Thu";
            this.mmBeginThurCheckbox.UseVisualStyleBackColor = true;
            // 
            // mmBeginWedCheckbox
            // 
            this.mmBeginWedCheckbox.AutoSize = true;
            this.mmBeginWedCheckbox.Checked = true;
            this.mmBeginWedCheckbox.CheckState = System.Windows.Forms.CheckState.Checked;
            this.mmBeginWedCheckbox.Location = new System.Drawing.Point(212, 84);
            this.mmBeginWedCheckbox.Name = "mmBeginWedCheckbox";
            this.mmBeginWedCheckbox.Size = new System.Drawing.Size(49, 17);
            this.mmBeginWedCheckbox.TabIndex = 47;
            this.mmBeginWedCheckbox.Text = "Wed";
            this.mmBeginWedCheckbox.UseVisualStyleBackColor = true;
            // 
            // mmBeginTueCheckbox
            // 
            this.mmBeginTueCheckbox.AutoSize = true;
            this.mmBeginTueCheckbox.Checked = true;
            this.mmBeginTueCheckbox.CheckState = System.Windows.Forms.CheckState.Checked;
            this.mmBeginTueCheckbox.Location = new System.Drawing.Point(160, 84);
            this.mmBeginTueCheckbox.Name = "mmBeginTueCheckbox";
            this.mmBeginTueCheckbox.Size = new System.Drawing.Size(45, 17);
            this.mmBeginTueCheckbox.TabIndex = 46;
            this.mmBeginTueCheckbox.Text = "Tue";
            this.mmBeginTueCheckbox.UseVisualStyleBackColor = true;
            // 
            // mmBeginMonCheckbox
            // 
            this.mmBeginMonCheckbox.AutoSize = true;
            this.mmBeginMonCheckbox.Checked = true;
            this.mmBeginMonCheckbox.CheckState = System.Windows.Forms.CheckState.Checked;
            this.mmBeginMonCheckbox.Location = new System.Drawing.Point(106, 84);
            this.mmBeginMonCheckbox.Name = "mmBeginMonCheckbox";
            this.mmBeginMonCheckbox.Size = new System.Drawing.Size(47, 17);
            this.mmBeginMonCheckbox.TabIndex = 45;
            this.mmBeginMonCheckbox.Text = "Mon";
            this.mmBeginMonCheckbox.UseVisualStyleBackColor = true;
            // 
            // mmBeginSunCheckbox
            // 
            this.mmBeginSunCheckbox.AutoSize = true;
            this.mmBeginSunCheckbox.Location = new System.Drawing.Point(54, 84);
            this.mmBeginSunCheckbox.Name = "mmBeginSunCheckbox";
            this.mmBeginSunCheckbox.Size = new System.Drawing.Size(45, 17);
            this.mmBeginSunCheckbox.TabIndex = 44;
            this.mmBeginSunCheckbox.Text = "Sun";
            this.mmBeginSunCheckbox.UseVisualStyleBackColor = true;
            // 
            // propertiesHeaderStrip15
            // 
            this.propertiesHeaderStrip15.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.propertiesHeaderStrip15.Location = new System.Drawing.Point(3, 148);
            this.propertiesHeaderStrip15.Name = "propertiesHeaderStrip15";
            this.propertiesHeaderStrip15.Size = new System.Drawing.Size(491, 25);
            this.propertiesHeaderStrip15.TabIndex = 43;
            this.propertiesHeaderStrip15.Text = "How long should the server stay in maintenance mode?";
            this.propertiesHeaderStrip15.WordWrap = false;
            // 
            // propertiesHeaderStrip14
            // 
            this.propertiesHeaderStrip14.Dock = System.Windows.Forms.DockStyle.Top;
            this.propertiesHeaderStrip14.Location = new System.Drawing.Point(0, 0);
            this.propertiesHeaderStrip14.Name = "propertiesHeaderStrip14";
            this.propertiesHeaderStrip14.Size = new System.Drawing.Size(491, 25);
            this.propertiesHeaderStrip14.TabIndex = 42;
            this.propertiesHeaderStrip14.Text = "What time should maintenance mode begin?";
            this.propertiesHeaderStrip14.WordWrap = false;
            // 
            // maintenanceModeContentPage
            // 
            this.maintenanceModeContentPage.BackColor = System.Drawing.Color.White;
            this.maintenanceModeContentPage.BorderWidth = 1;
            // 
            // 
            // 
            this.maintenanceModeContentPage.ContentPanel.BackColor2 = System.Drawing.Color.White;
            this.maintenanceModeContentPage.ContentPanel.BorderColor = System.Drawing.SystemColors.ControlDark;
            this.maintenanceModeContentPage.ContentPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.maintenanceModeContentPage.ContentPanel.FillStyle = Idera.SQLdm.DesktopClient.Controls.GradientPanelFillStyle.Solid;
            this.maintenanceModeContentPage.ContentPanel.Location = new System.Drawing.Point(1, 56);
            this.maintenanceModeContentPage.ContentPanel.Name = "ContentPanel";
            this.maintenanceModeContentPage.ContentPanel.Padding = new System.Windows.Forms.Padding(1);
            this.maintenanceModeContentPage.ContentPanel.ShowBorder = false;
            this.maintenanceModeContentPage.ContentPanel.Size = new System.Drawing.Size(787, 493);
            this.maintenanceModeContentPage.ContentPanel.TabIndex = 1;
            this.maintenanceModeContentPage.Dock = System.Windows.Forms.DockStyle.Fill;
            this.maintenanceModeContentPage.HeaderImage = global::Idera.SQLdm.DesktopClient.Properties.Resources.MaintenanceModeLarge;
            this.maintenanceModeContentPage.Location = new System.Drawing.Point(0, 0);
            this.maintenanceModeContentPage.Name = "maintenanceModeContentPage";
            this.maintenanceModeContentPage.Size = new System.Drawing.Size(789, 550);
            this.maintenanceModeContentPage.TabIndex = 0;
            this.maintenanceModeContentPage.Text = "Customize maintenance mode settings.";
            // 
            // MassMaintenanceModeDialog
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(789, 554);
            this.Controls.Add(this.buttonsPanel);
            this.Controls.Add(this.maintenancePropertyPage);
            this.Name = "MassMaintenanceModeDialog";
            this.ShowIcon = false;
            this.Text = "Maintenance Mode";
            ((System.ComponentModel.ISupportInitialize)(this.inputDayLimiter)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.inputOfEveryMonthLimiter)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.inputOfEveryTheMonthLimiter)).EndInit();
            this.mmMonthlyRecurringPanel.ResumeLayout(false);
            this.mmMonthlyRecurringPanel.PerformLayout();
            this.flowLayoutPanel8.ResumeLayout(false);
            this.flowLayoutPanel8.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.mmMonthRecurringDuration)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.mmMonthRecurringBegin)).EndInit();
            this.buttonsPanel.ResumeLayout(false);
            this.maintenancePropertyPage.ResumeLayout(false);
            this.splitContainer2.Panel1.ResumeLayout(false);
            this.splitContainer2.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer2)).EndInit();
            this.splitContainer2.ResumeLayout(false);
            this.maintenanceModeControlsContainer.ResumeLayout(false);
            this.maintenanceModeControlsContainer.PerformLayout();
            this.panel2.ResumeLayout(false);
            this.mmOncePanel.ResumeLayout(false);
            this.mmOncePanel.PerformLayout();
            this.tableLayoutPanel14.ResumeLayout(false);
            this.tableLayoutPanel14.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.mmOnceBeginTime)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.mmOnceBeginDate)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.mmOnceStopTime)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.mmOnceStopDate)).EndInit();
            this.mmRecurringPanel.ResumeLayout(false);
            this.mmRecurringPanel.PerformLayout();
            this.flowLayoutPanel4.ResumeLayout(false);
            this.flowLayoutPanel4.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.mmRecurringDuration)).EndInit();
            this.flowLayoutPanel3.ResumeLayout(false);
            this.flowLayoutPanel3.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.mmRecurringBegin)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }



        #endregion

        private Button okButton;
        private Button cancelButton;
        private Button verifyButton;
        private FlowLayoutPanel buttonsPanel;
        private TreeView treeView;
        private Controls.PropertyPage maintenancePropertyPage;
        private TableLayoutPanel maintenanceModeControlsContainer;
        private Divelements.WizardFramework.InformationBox informationBox6;
        private Controls.PropertiesHeaderStrip propertiesHeaderStrip13;
        private RadioButton mmNeverRadio;
        private RadioButton mmAlwaysRadio;
        private RadioButton mmRecurringRadio;
        private RadioButton mmOnceRadio;
        private Panel panel2;
        private Panel mmOncePanel;
        private TableLayoutPanel tableLayoutPanel14;
        private Label label21;
        private Label mmServerDateTime;
        private Label label6;
        private Common.UI.Controls.TimeComboEditor mmOnceBeginTime;
        private Infragistics.Win.UltraWinEditors.UltraDateTimeEditor mmOnceBeginDate;
        private Label label7;
        private Common.UI.Controls.TimeComboEditor mmOnceStopTime;
        private Infragistics.Win.UltraWinEditors.UltraDateTimeEditor mmOnceStopDate;
        private Controls.PropertiesHeaderStrip propertiesHeaderStrip16;
        private Divelements.WizardFramework.InformationBox informationBox5;
        private Panel mmRecurringPanel;
        private FlowLayoutPanel flowLayoutPanel4;
        private Label label5;
        private Common.UI.Controls.TimeComboEditor mmRecurringDuration;
        private FlowLayoutPanel flowLayoutPanel3;
        private Label label4;
        private Common.UI.Controls.TimeComboEditor mmRecurringBegin;
        private Divelements.WizardFramework.InformationBox informationBox4;
        private CheckBox mmBeginSatCheckbox;
        private CheckBox mmBeginFriCheckbox;
        private CheckBox mmBeginThurCheckbox;
        private CheckBox mmBeginWedCheckbox;
        private CheckBox mmBeginTueCheckbox;
        private CheckBox mmBeginMonCheckbox;
        private CheckBox mmBeginSunCheckbox;
        private Controls.PropertiesHeaderStrip propertiesHeaderStrip15;
        private Controls.PropertiesHeaderStrip propertiesHeaderStrip14;
        private Controls.Office2007PropertyPage maintenanceModeContentPage;
        private SplitContainer splitContainer2;
        private System.ComponentModel.BackgroundWorker serverDateTimeVersionBackgroundWorker;
        private System.Windows.Forms.RadioButton mmMonthlyRecurringRadio;
        private System.Windows.Forms.Panel mmMonthlyRecurringPanel;
        private System.Windows.Forms.RadioButton mmMonthlyDayRadio;
        private System.Windows.Forms.RadioButton mmMonthlyTheRadio;
        private System.Windows.Forms.NumericUpDown inputDayLimiter;
        private System.Windows.Forms.NumericUpDown inputOfEveryMonthLimiter;
        private System.Windows.Forms.NumericUpDown inputOfEveryTheMonthLimiter;
        private System.Windows.Forms.Label mmMonthlyLabel1;
        private System.Windows.Forms.Label mmMonthlyLabel2;
        private System.Windows.Forms.ComboBox WeekcomboBox;
        private System.Windows.Forms.ComboBox DaycomboBox;
        private System.Windows.Forms.Label mmMonthlyOfEveryLabel;
        private System.Windows.Forms.Label mmMonthlyOfTheEveryLabel;
        private System.Windows.Forms.Label mmMonthlyAtLabel;
        private Common.UI.Controls.TimeComboEditor mmMonthRecurringBegin;
        private Idera.SQLdm.DesktopClient.Controls.PropertiesHeaderStrip propertiesHeaderStrip27;
        private Divelements.WizardFramework.InformationBox informationBox15;
        private Common.UI.Controls.TimeComboEditor mmMonthRecurringDuration;
        private System.Windows.Forms.FlowLayoutPanel flowLayoutPanel8;
        private System.Windows.Forms.Label label29;
        private System.Windows.Forms.Label label30;

    }
}
