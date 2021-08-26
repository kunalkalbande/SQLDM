using Idera.SQLdm.DesktopClient.Controls.CustomControls;
using Idera.SQLdm.DesktopClient.Properties;
using System;
using System.Drawing;
using System.Windows.Forms;

namespace Idera.SQLdm.DesktopClient.Dialogs
{
    partial class GroomingOptionsDialog
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
            bool isDarkThemeSelected = Properties.Settings.Default.ColorScheme == "Dark";
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(GroomingOptionsDialog));
            Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomDropDownEditorButton dropDownEditorButton1 = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomDropDownEditorButton("DropDownList");
            Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomDropDownEditorButton dropDownEditorButton2 = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomDropDownEditorButton("DropDownList");
            this.cancelButton = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomButton();
            this.okButton = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomButton();
            this.apply = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomButton();
            this.timer1 = new System.Windows.Forms.Timer(this.components);
            this.groomNow = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomButton();
            this.aggregateNow = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomButton();
            this.groomingPropertiesContentPage = new Idera.SQLdm.DesktopClient.Controls.Office2007PropertyPage();
            this.containerPanel = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomPanel ();
            this.tableLayoutPanel3 = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomTableLayoutPanel();
            this.tableLayoutPanel4 = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomTableLayoutPanel();
            this.aggregationThresholdNumericUpDown = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomNumericUpDown();
            this.alertsGroomingThresholdNumericUpDown = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomNumericUpDown();
            this.activityGroomingThresholdNumericUpDown = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomNumericUpDown();
            this.label17 = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomLabel();
            this.label16 = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomLabel();
            this.label6 = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomLabel();
            this.label11 = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomLabel();
            this.label3 = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomLabel();
            this.label5 = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomLabel();
            this.metricsGroomingThresholdNumericUpDown = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomNumericUpDown();
            this.label12 = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomLabel();
            this.label2 = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomLabel();
            this.lblAudit = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomLabel();
            this.auditGroomingThresholdNumericUpDown = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomNumericUpDown();
            this.label4 = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomLabel();
            this.lblPrescriptiveAnalysisGroom = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomLabel();
            this.PAGroomingThresholdNumericUpDown = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomNumericUpDown();
            this.lblPADays = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomLabel();
            this.refresh = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomButton();
            this.groupBox4 = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomGroupBox();
            this.tableLayoutPanel5 = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomTableLayoutPanel();
            this.label19 = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomLabel();
            this.label20 = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomLabel();
            this.label18 = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomLabel();
            this.aggregationCurrentlyRunning = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomTextBox();
            this.aggregationLastRun = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomTextBox();
            this.aggregationCompletionStatus = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomTextBox();
            this.groupBox3 = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomGroupBox();
            this.tableLayoutPanel2 = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomTableLayoutPanel();
            this.label8 = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomLabel();
            this.currentlyRunning = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomTextBox();
            this.label10 = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomLabel();
            this.lastRun = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomTextBox();
            this.completionStatus = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomTextBox();
            this.label9 = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomLabel();
            this.propertiesHeaderStrip2 = new Idera.SQLdm.DesktopClient.Controls.PropertiesHeaderStrip();
            this.informationBox2 = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomInformationBox();
            this.propertiesHeaderStrip1 = new Idera.SQLdm.DesktopClient.Controls.PropertiesHeaderStrip();
            this.groupBox2 = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomGroupBox();
            this.tableLayoutPanel6 = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomTableLayoutPanel();
            this.label13 = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomLabel();
            this.aggregationTimeIntervalCombo = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomComboBox();
            this.aggregationHourlyButton = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomRadioButton();
            this.aggregationOnceDailyButton = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomRadioButton();
            this.aggregationTimeEditor = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomTimeComboEditor();
            this.label15 = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomLabel();
            this.groupBox1 = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomGroupBox();
            this.tableLayoutPanel1 = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomTableLayoutPanel();
            this.hourlyButton = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomRadioButton();
            this.label1 = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomLabel();
            this.onceDailyButton = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomRadioButton();
            this.groomTimeIntervalCombo = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomComboBox();
            this.groomingTimeEditor = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomTimeComboEditor();
            this.label14 = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomLabel();
            this.informationBox1 = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomInformationBox();
            this.propertiesHeaderStrip3 = new Idera.SQLdm.DesktopClient.Controls.PropertiesHeaderStrip();
            this.currentTimeLabel = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomLabel();
            this.containerPanel.SuspendLayout();
            this.tableLayoutPanel3.SuspendLayout();
            this.tableLayoutPanel4.SuspendLayout();
            this.lblForecastingDataAggregation = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomLabel();
            this.ForecastingAggregationThresholdNumericUpDown = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomNumericUpDown();
            this.lblFADays = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomLabel();
            this.lblGroomForecast = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomLabel();
            this.lblGroomForecastDays = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomLabel();
            this.GroomForecastNumericUpDown = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomNumericUpDown();
            ((System.ComponentModel.ISupportInitialize)(this.aggregationThresholdNumericUpDown)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.alertsGroomingThresholdNumericUpDown)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.activityGroomingThresholdNumericUpDown)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.metricsGroomingThresholdNumericUpDown)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.auditGroomingThresholdNumericUpDown)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.ForecastingAggregationThresholdNumericUpDown)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.GroomForecastNumericUpDown)).BeginInit();
            this.groupBox4.SuspendLayout();
            this.tableLayoutPanel5.SuspendLayout();
            this.groupBox3.SuspendLayout();
            this.tableLayoutPanel2.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.tableLayoutPanel6.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.aggregationTimeEditor)).BeginInit();
            this.groupBox1.SuspendLayout();
            this.tableLayoutPanel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.groomingTimeEditor)).BeginInit();
            this.SuspendLayout();
            // 
            // cancelButton
            // 
            this.cancelButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.cancelButton.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.cancelButton.Location = new System.Drawing.Point(478, 685);
            this.cancelButton.Name = "cancelButton";
            this.cancelButton.Size = new System.Drawing.Size(75, 23);
            this.cancelButton.TabIndex = 2;
            this.cancelButton.Text = "Cancel";
            this.cancelButton.UseVisualStyleBackColor = true;
            // 
            // okButton
            // 
            this.okButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.okButton.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.okButton.Location = new System.Drawing.Point(316, 685);
            this.okButton.Name = "okButton";
            this.okButton.Size = new System.Drawing.Size(75, 23);
            this.okButton.TabIndex = 0;
            this.okButton.Text = "OK";
            this.okButton.UseVisualStyleBackColor = true;
            this.okButton.Click += new System.EventHandler(this.okButton_Click);
            // 
            // apply
            // 
            this.apply.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.apply.Enabled = false;
            this.apply.Location = new System.Drawing.Point(397, 685);
            this.apply.Name = "apply";
            this.apply.Size = new System.Drawing.Size(75, 23);
            this.apply.TabIndex = 1;
            this.apply.Text = "Apply";
            this.apply.UseVisualStyleBackColor = true;
            this.apply.Click += new System.EventHandler(this.apply_Click);
            // 
            // timer1
            // 
            this.timer1.Interval = 1000;
            this.timer1.Tick += new System.EventHandler(this.timer1_Tick);
            // 
            // groomNow
            // 
            this.groomNow.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.groomNow.BackColor = System.Drawing.Color.White;
            this.groomNow.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.groomNow.Location = new System.Drawing.Point(12, 685);
            this.groomNow.Name = "groomNow";
            
            this.groomNow.Size = new System.Drawing.Size(75, 23);
            this.groomNow.TabIndex = 4;
            this.groomNow.Text = "Groom Now";
            this.groomNow.UseVisualStyleBackColor = false;
            this.groomNow.Click += new System.EventHandler(this.groomNow_Click);
            // 
            // aggregateNow
            // 
            this.aggregateNow.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.aggregateNow.BackColor = System.Drawing.Color.White;
            this.aggregateNow.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.aggregateNow.Location = new System.Drawing.Point(108, 685);
            this.aggregateNow.Name = "aggregateNow";
            this.aggregateNow.Size = new System.Drawing.Size(91, 23);
            this.aggregateNow.TabIndex = 5;
            this.aggregateNow.Text = "Aggregate Now";
            this.aggregateNow.UseVisualStyleBackColor = false;
            this.aggregateNow.Click += new System.EventHandler(this.aggregateNow_Click);
            // 
            // groomingPropertiesContentPage
            // 
            this.groomingPropertiesContentPage.BackColor = System.Drawing.Color.White;
            this.groomingPropertiesContentPage.BorderWidth = 1;
            // 
            // 
            // 
            this.groomingPropertiesContentPage.ContentPanel.BackColor2 = System.Drawing.Color.White;
            this.groomingPropertiesContentPage.ContentPanel.BorderColor = System.Drawing.SystemColors.ControlDark;
            this.groomingPropertiesContentPage.ContentPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.groomingPropertiesContentPage.ContentPanel.FillStyle = Idera.SQLdm.DesktopClient.Controls.GradientPanelFillStyle.Solid;
            this.groomingPropertiesContentPage.ContentPanel.Location = new System.Drawing.Point(1, 56);
            this.groomingPropertiesContentPage.ContentPanel.Name = "ContentPanel";
            this.groomingPropertiesContentPage.ContentPanel.Padding = new System.Windows.Forms.Padding(1);
            this.groomingPropertiesContentPage.ContentPanel.ShowBorder = false;
            this.groomingPropertiesContentPage.ContentPanel.Size = new System.Drawing.Size(548, 618);
            this.groomingPropertiesContentPage.ContentPanel.TabIndex = 1;
            this.groomingPropertiesContentPage.Dock = System.Windows.Forms.DockStyle.Fill;
            this.groomingPropertiesContentPage.HeaderImage = global::Idera.SQLdm.DesktopClient.Properties.Resources.GroomingOptions;
            this.groomingPropertiesContentPage.Location = new System.Drawing.Point(0, 0);
            this.groomingPropertiesContentPage.Name = "groomingPropertiesContentPage";
            this.groomingPropertiesContentPage.Size = new System.Drawing.Size(550, 675);
            this.groomingPropertiesContentPage.TabIndex = 3;
            this.groomingPropertiesContentPage.Text = "Schedule and configure SQLDM Repository grooming.";
            // 
            // containerPanel
            // 
            this.containerPanel.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.containerPanel.Controls.Add(this.tableLayoutPanel3);
            this.containerPanel.Controls.Add(this.groomingPropertiesContentPage);
            this.containerPanel.Location = new System.Drawing.Point(11, 12);
            this.containerPanel.Name = "containerPanel";
            this.containerPanel.Size = new System.Drawing.Size(550, 665);
            this.containerPanel.TabIndex = 6;
            // 
            // tableLayoutPanel3
            // 
            this.tableLayoutPanel3.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tableLayoutPanel3.AutoSize = true;
            this.tableLayoutPanel3.BackColor = System.Drawing.Color.White;
            this.tableLayoutPanel3.ColumnCount = 2;
            this.tableLayoutPanel3.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel3.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel3.Controls.Add(this.tableLayoutPanel4, 0, 6);
            this.tableLayoutPanel3.Controls.Add(this.refresh, 1, 9);
            this.tableLayoutPanel3.Controls.Add(this.groupBox4, 1, 8);
            this.tableLayoutPanel3.Controls.Add(this.groupBox3, 0, 8);
            this.tableLayoutPanel3.Controls.Add(this.propertiesHeaderStrip2, 0, 7);
            this.tableLayoutPanel3.Controls.Add(this.informationBox2, 0, 5);
            this.tableLayoutPanel3.Controls.Add(this.propertiesHeaderStrip1, 0, 4);
            this.tableLayoutPanel3.Controls.Add(this.groupBox2, 1, 3);
            this.tableLayoutPanel3.Controls.Add(this.groupBox1, 0, 3);
            this.tableLayoutPanel3.Controls.Add(this.informationBox1, 0, 1);
            this.tableLayoutPanel3.Controls.Add(this.propertiesHeaderStrip3, 0, 0);
            this.tableLayoutPanel3.Controls.Add(this.currentTimeLabel, 0, 2);
            this.tableLayoutPanel3.Location = new System.Drawing.Point(2, 44);
            this.tableLayoutPanel3.Name = "tableLayoutPanel3";
            this.tableLayoutPanel3.RowCount = 10;
            this.tableLayoutPanel3.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel3.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel3.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel3.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel3.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel3.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel3.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel3.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel3.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel3.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel3.Size = new System.Drawing.Size(545, 619);
            this.tableLayoutPanel3.TabIndex = 4;
            // 
            // tableLayoutPanel4
            // 
            this.tableLayoutPanel4.AutoSize = true;
            this.tableLayoutPanel4.ColumnCount = 5;
            this.tableLayoutPanel3.SetColumnSpan(this.tableLayoutPanel4, 2);
            this.tableLayoutPanel4.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 42F));
            this.tableLayoutPanel4.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel4.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 43F));
            this.tableLayoutPanel4.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel4.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel4.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel4.Controls.Add(this.aggregationThresholdNumericUpDown, 3, 3);
            this.tableLayoutPanel4.Controls.Add(this.alertsGroomingThresholdNumericUpDown, 3, 2);
            this.tableLayoutPanel4.Controls.Add(this.activityGroomingThresholdNumericUpDown, 3, 1);
            this.tableLayoutPanel4.Controls.Add(this.label17, 1, 3);
            this.tableLayoutPanel4.Controls.Add(this.label16, 4, 3);
            this.tableLayoutPanel4.Controls.Add(this.label6, 4, 2);
            this.tableLayoutPanel4.Controls.Add(this.label11, 4, 1);
            this.tableLayoutPanel4.Controls.Add(this.label3, 1, 2);
            this.tableLayoutPanel4.Controls.Add(this.label5, 4, 0);
            this.tableLayoutPanel4.Controls.Add(this.metricsGroomingThresholdNumericUpDown, 3, 0);
            this.tableLayoutPanel4.Controls.Add(this.label12, 1, 1);
            this.tableLayoutPanel4.Controls.Add(this.label2, 1, 0);
            this.tableLayoutPanel4.Controls.Add(this.lblAudit, 1, 4);
            this.tableLayoutPanel4.Controls.Add(this.auditGroomingThresholdNumericUpDown, 3, 4);
            this.tableLayoutPanel4.Controls.Add(this.label4, 4, 4);
            this.tableLayoutPanel4.Controls.Add(this.lblPrescriptiveAnalysisGroom, 1, 5);
            this.tableLayoutPanel4.Controls.Add(this.PAGroomingThresholdNumericUpDown, 3, 5);
            this.tableLayoutPanel4.Controls.Add(this.lblPADays, 4, 5);
            this.tableLayoutPanel4.Controls.Add(this.lblForecastingDataAggregation, 1, 6);
            this.tableLayoutPanel4.Controls.Add(this.ForecastingAggregationThresholdNumericUpDown, 3, 6);
            this.tableLayoutPanel4.Controls.Add(this.lblFADays, 4, 6);
            this.tableLayoutPanel4.Controls.Add(this.lblGroomForecast, 1, 7);
            this.tableLayoutPanel4.Controls.Add(this.GroomForecastNumericUpDown, 3, 7);
            this.tableLayoutPanel4.Controls.Add(this.lblGroomForecastDays, 4, 7);
            this.tableLayoutPanel4.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel4.Location = new System.Drawing.Point(3, 318);
            this.tableLayoutPanel4.Name = "tableLayoutPanel4";
            this.tableLayoutPanel4.RowCount = 5;
            this.tableLayoutPanel4.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel4.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel4.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel4.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel4.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel4.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel4.Size = new System.Drawing.Size(539, 130);
            this.tableLayoutPanel4.TabIndex = 73;
            // 
            // aggregationThresholdNumericUpDown
            // 
            this.aggregationThresholdNumericUpDown.Anchor = System.Windows.Forms.AnchorStyles.Left;
            //this.aggregationThresholdNumericUpDown.AutoSize = true;
            this.aggregationThresholdNumericUpDown.Location = new System.Drawing.Point(330, 81);
            this.aggregationThresholdNumericUpDown.Maximum = new decimal(new int[] {
            999,
            0,
            0,
            0});
            this.aggregationThresholdNumericUpDown.Minimum = new decimal(new int[] {
            2,
            0,
            0,
            0});
            this.aggregationThresholdNumericUpDown.Name = "aggregationThresholdNumericUpDown";
            this.aggregationThresholdNumericUpDown.Size = new System.Drawing.Size(47, 20);
            this.aggregationThresholdNumericUpDown.TabIndex = 72;
            this.aggregationThresholdNumericUpDown.Value = new decimal(new int[] {
            14,
            0,
            0,
            0});
            this.aggregationThresholdNumericUpDown.ValueChanged += new System.EventHandler(this.ConfigValueChanged);
            this.aggregationThresholdNumericUpDown.Validating += new System.ComponentModel.CancelEventHandler(this.numericUpDown_Validating);
            // 
            // alertsGroomingThresholdNumericUpDown
            // 
            this.alertsGroomingThresholdNumericUpDown.Anchor = System.Windows.Forms.AnchorStyles.Left;
            //this.alertsGroomingThresholdNumericUpDown.AutoSize = true;
            this.alertsGroomingThresholdNumericUpDown.Location = new System.Drawing.Point(330, 55);
            this.alertsGroomingThresholdNumericUpDown.Maximum = new decimal(new int[] {
            999,
            0,
            0,
            0});
            this.alertsGroomingThresholdNumericUpDown.Minimum = new decimal(new int[] {
            2,
            0,
            0,
            0});
            this.alertsGroomingThresholdNumericUpDown.Name = "alertsGroomingThresholdNumericUpDown";
            this.alertsGroomingThresholdNumericUpDown.Size = new System.Drawing.Size(47, 20);
            this.alertsGroomingThresholdNumericUpDown.TabIndex = 71;
            this.alertsGroomingThresholdNumericUpDown.Value = new decimal(new int[] {
            31,
            0,
            0,
            0});
            this.alertsGroomingThresholdNumericUpDown.ValueChanged += new System.EventHandler(this.ConfigValueChanged);
            this.alertsGroomingThresholdNumericUpDown.Validating += new System.ComponentModel.CancelEventHandler(this.numericUpDown_Validating);
            // 
            // activityGroomingThresholdNumericUpDown
            // 
            this.activityGroomingThresholdNumericUpDown.Anchor = System.Windows.Forms.AnchorStyles.Left;
            //this.activityGroomingThresholdNumericUpDown.AutoSize = true;
            this.activityGroomingThresholdNumericUpDown.Location = new System.Drawing.Point(330, 29);
            this.activityGroomingThresholdNumericUpDown.Maximum = new decimal(new int[] {
            999,
            0,
            0,
            0});
            this.activityGroomingThresholdNumericUpDown.Minimum = new decimal(new int[] {
            2,
            0,
            0,
            0});
            this.activityGroomingThresholdNumericUpDown.Name = "activityGroomingThresholdNumericUpDown";
            this.activityGroomingThresholdNumericUpDown.Size = new System.Drawing.Size(47, 20);
            this.activityGroomingThresholdNumericUpDown.TabIndex = 70;
            this.activityGroomingThresholdNumericUpDown.Value = new decimal(new int[] {
            31,
            0,
            0,
            0});
            this.activityGroomingThresholdNumericUpDown.ValueChanged += new System.EventHandler(this.ConfigValueChanged);
            this.activityGroomingThresholdNumericUpDown.Validating += new System.ComponentModel.CancelEventHandler(this.numericUpDown_Validating);
            // 
            // label17
            // 
            this.label17.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.label17.AutoSize = true;
            this.label17.Location = new System.Drawing.Point(45, 84);
            this.label17.Name = "label17";
            this.label17.Size = new System.Drawing.Size(215, 13);
            this.label17.TabIndex = 69;
            this.label17.Text = "Aggregate query data into daily records after";
            // 
            // label16
            // 
            this.label16.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.label16.AutoSize = true;
            this.label16.Location = new System.Drawing.Point(377, 84);
            this.label16.Name = "label16";
            this.label16.Size = new System.Drawing.Size(29, 13);
            this.label16.TabIndex = 68;
            this.label16.Text = "days";
            // 
            // label6
            // 
            this.label6.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(377, 58);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(29, 13);
            this.label6.TabIndex = 67;
            this.label6.Text = "days";
            // 
            // label11
            // 
            this.label11.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.label11.AutoSize = true;
            this.label11.Location = new System.Drawing.Point(377, 32);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(29, 13);
            this.label11.TabIndex = 66;
            this.label11.Text = "days";
            // 
            // label3
            // 
            this.label3.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(45, 58);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(157, 13);
            this.label3.TabIndex = 65;
            this.label3.Text = "Groom inactive Alerts older than";
            // 
            // label5
            // 
            this.label5.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(377, 6);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(29, 13);
            this.label5.TabIndex = 64;
            this.label5.Text = "days";
            // 
            // metricsGroomingThresholdNumericUpDown
            // 
            this.metricsGroomingThresholdNumericUpDown.Anchor = System.Windows.Forms.AnchorStyles.Left;
            //this.metricsGroomingThresholdNumericUpDown.AutoSize = true;
            this.metricsGroomingThresholdNumericUpDown.Location = new System.Drawing.Point(330, 3);
            this.metricsGroomingThresholdNumericUpDown.Maximum = new decimal(new int[] {
            1100,
            0,
            0,
            0});
            this.metricsGroomingThresholdNumericUpDown.Minimum = new decimal(new int[] {
            2,
            0,
            0,
            0});
            this.metricsGroomingThresholdNumericUpDown.Name = "metricsGroomingThresholdNumericUpDown";
            this.metricsGroomingThresholdNumericUpDown.Size = new System.Drawing.Size(47, 20);
            this.metricsGroomingThresholdNumericUpDown.TabIndex = 63;
            this.metricsGroomingThresholdNumericUpDown.Value = new decimal(new int[] {
           90,
            0,
            0,
            0});
            this.metricsGroomingThresholdNumericUpDown.ValueChanged += new System.EventHandler(this.ConfigValueChanged);
            this.metricsGroomingThresholdNumericUpDown.Validating += new System.ComponentModel.CancelEventHandler(this.numericUpDown_Validating);
            // 
            // label12
            // 
            this.label12.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.label12.AutoSize = true;
            this.label12.Location = new System.Drawing.Point(45, 26);
            this.label12.Name = "label12";
            this.label12.Size = new System.Drawing.Size(218, 26);
            this.label12.TabIndex = 62;
            this.label12.Text = "Groom Sessions, Queries, Deadlocks, Waits,\r\nand History Browser data older than";
            // 
            // label2
            // 
            this.label2.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(45, 6);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(236, 13);
            this.label2.TabIndex = 61;
            this.label2.Text = "Groom standard metrics and baselines older than";
            // 
            // lblAudit
            // 
            this.lblAudit.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.lblAudit.AutoSize = true;
            this.lblAudit.Location = new System.Drawing.Point(45, 110);
            this.lblAudit.Name = "lblAudit";
            this.lblAudit.Size = new System.Drawing.Size(173, 13);
            this.lblAudit.TabIndex = 73;
            this.lblAudit.Text = "Groom Change Log data older than";
            // 
            // auditGroomingThresholdNumericUpDown
            // 
            this.auditGroomingThresholdNumericUpDown.Anchor = System.Windows.Forms.AnchorStyles.Left;
            //this.auditGroomingThresholdNumericUpDown.AutoSize = true;
            this.auditGroomingThresholdNumericUpDown.Location = new System.Drawing.Point(330, 107);
            this.auditGroomingThresholdNumericUpDown.Maximum = new decimal(new int[] {
            365,
            0,
            0,
            0});
            this.auditGroomingThresholdNumericUpDown.Minimum = new decimal(new int[] {
            2,
            0,
            0,
            0});
            this.auditGroomingThresholdNumericUpDown.Name = "auditGroomingThresholdNumericUpDown";
            this.auditGroomingThresholdNumericUpDown.Size = new System.Drawing.Size(47, 20);
            this.auditGroomingThresholdNumericUpDown.TabIndex = 74;
            this.auditGroomingThresholdNumericUpDown.Value = new decimal(new int[] {
            14,
            0,
            0,
            0});
            // 
            // label4
            // 
            this.label4.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(377, 110);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(29, 13);
            this.label4.TabIndex = 75;
            this.label4.Text = "days";
            // 
            // lblPrescriptiveAnalysisGroom
            // 
            this.lblPrescriptiveAnalysisGroom.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.lblPrescriptiveAnalysisGroom.AutoSize = true;
            this.lblPrescriptiveAnalysisGroom.Location = new System.Drawing.Point(45, 110);
            this.lblPrescriptiveAnalysisGroom.Name = "lblPrescriptiveAnalysisGroom";
            this.lblPrescriptiveAnalysisGroom.Size = new System.Drawing.Size(173, 13);
            this.lblPrescriptiveAnalysisGroom.TabIndex = 73;
            this.lblPrescriptiveAnalysisGroom.Text = "Groom Prescriptive Analysis data older than";
            // 
            // PAGroomingThresholdNumericUpDown
            // 
            this.PAGroomingThresholdNumericUpDown.Anchor = System.Windows.Forms.AnchorStyles.Left;
            //this.PAGroomingThresholdNumericUpDown.AutoSize = true;
            this.PAGroomingThresholdNumericUpDown.Location = new System.Drawing.Point(330, 107);
            this.PAGroomingThresholdNumericUpDown.Maximum = new decimal(new int[] {
            365,
            0,
            0,
            0});
            this.PAGroomingThresholdNumericUpDown.Minimum = new decimal(new int[] {
            2,
            0,
            0,
            0});
            this.PAGroomingThresholdNumericUpDown.Name = "auditGroomingThresholdNumericUpDown";
            this.PAGroomingThresholdNumericUpDown.Size = new System.Drawing.Size(47, 20);
            this.PAGroomingThresholdNumericUpDown.TabIndex = 74;
            this.PAGroomingThresholdNumericUpDown.Value = new decimal(new int[] {
            14,
            0,
            0,
            0});
            // 
            // lblPADays
            // 
            this.lblPADays.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.lblPADays.AutoSize = true;
            this.lblPADays.Location = new System.Drawing.Point(377, 110);
            this.lblPADays.Name = "label4";
            this.lblPADays.Size = new System.Drawing.Size(29, 13);
            this.lblPADays.TabIndex = 75;
            this.lblPADays.Text = "days";

            // lblForecastingDataAggregation








            // 
            this.lblForecastingDataAggregation.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.lblForecastingDataAggregation.AutoSize = true;
            this.lblForecastingDataAggregation.Location = new System.Drawing.Point(45, 110);
            this.lblForecastingDataAggregation.Name = "lblForecastingDataAggregation";
            this.lblForecastingDataAggregation.Size = new System.Drawing.Size(173, 13);
            this.lblForecastingDataAggregation.TabIndex = 73;
            this.lblForecastingDataAggregation.Text = "Aggregate forecasting data to daily records after";
            // 
            // ForecastingAggregationThresholdNumericUpDown
            // 
            this.ForecastingAggregationThresholdNumericUpDown.Anchor = System.Windows.Forms.AnchorStyles.Left;
            //this.ForecastingAggregationThresholdNumericUpDown.AutoSize = true;
            this.ForecastingAggregationThresholdNumericUpDown.Location = new System.Drawing.Point(330, 107);
            this.ForecastingAggregationThresholdNumericUpDown.Maximum = new decimal(new int[] {
            1095,
            0,
            0,
            0});
            this.ForecastingAggregationThresholdNumericUpDown.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.ForecastingAggregationThresholdNumericUpDown.Name = "auditGroomingThresholdNumericUpDown";
            this.ForecastingAggregationThresholdNumericUpDown.Size = new System.Drawing.Size(47, 20);
            this.ForecastingAggregationThresholdNumericUpDown.TabIndex = 74;
            this.ForecastingAggregationThresholdNumericUpDown.Value = new decimal(new int[] {
            14,
            0,
            0,
            0});
            // 
            // lblDays
            // 
            this.lblFADays.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.lblFADays.AutoSize = true;
            this.lblFADays.Location = new System.Drawing.Point(377, 110);
            this.lblFADays.Name = "label4";
            this.lblFADays.Size = new System.Drawing.Size(29, 13);
            this.lblFADays.TabIndex = 75;
            this.lblFADays.Text = "days";
            // 
            //lblGroomForecastingData
            //
            this.lblGroomForecast.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.lblGroomForecast.AutoSize = true;
            this.lblGroomForecast.Location = new System.Drawing.Point(45, 110);
            this.lblGroomForecast.Name = "lblGroomForecast";
            this.lblGroomForecast.Size = new System.Drawing.Size(173, 13);
            this.lblGroomForecast.TabIndex = 73;
            this.lblGroomForecast.Text = "Groom forecasting data older than";
            // 
            // GroomForecastNumericUpDown






            // 


            this.GroomForecastNumericUpDown.Anchor = System.Windows.Forms.AnchorStyles.Left;
            //this.GroomForecastNumericUpDown.AutoSize = true;
            this.GroomForecastNumericUpDown.Location = new System.Drawing.Point(330, 107);
            this.GroomForecastNumericUpDown.Maximum = new decimal(new int[] {
            1095,
            0,
            0,
            0});
            this.GroomForecastNumericUpDown.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.GroomForecastNumericUpDown.Name = "GroomForecastNumericUpDown";
            this.GroomForecastNumericUpDown.Size = new System.Drawing.Size(47, 20);
            this.GroomForecastNumericUpDown.TabIndex = 74;
            this.GroomForecastNumericUpDown.Value = new decimal(new int[] {
            3,
            0,
            0,
            0});
            // 
            // lblGroomForecastDays
            // 
            this.lblGroomForecastDays.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.lblGroomForecastDays.AutoSize = true;
            this.lblGroomForecastDays.Location = new System.Drawing.Point(377, 110);
            this.lblGroomForecastDays.Name = "lblGroomForecastDays";
            this.lblGroomForecastDays.Size = new System.Drawing.Size(29, 13);
            this.lblGroomForecastDays.TabIndex = 75;
            this.lblGroomForecastDays.Text = "days";
            // 
            // refresh
            // 
            this.refresh.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this.refresh.BackColor = System.Drawing.Color.White;
            this.refresh.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.refresh.Location = new System.Drawing.Point(467, 594);
            this.refresh.Name = "refresh";
            this.refresh.Size = new System.Drawing.Size(75, 22);
            this.refresh.TabIndex = 71;
            this.refresh.Text = "Refresh";
            this.refresh.UseVisualStyleBackColor = false;
            this.refresh.Click += new System.EventHandler(this.refresh_Click);
            // 
            // groupBox4
            // 
            this.groupBox4.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.groupBox4.Controls.Add(this.tableLayoutPanel5);
            this.groupBox4.Location = new System.Drawing.Point(275, 485);
            this.groupBox4.Name = "groupBox4";
            this.groupBox4.Size = new System.Drawing.Size(249, 103);
            this.groupBox4.TabIndex = 70;
            this.groupBox4.TabStop = false;
            this.groupBox4.Text = "Aggregation";
            // 
            // tableLayoutPanel5
            // 
            this.tableLayoutPanel5.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.tableLayoutPanel5.ColumnCount = 2;
            this.tableLayoutPanel5.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel5.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel5.Controls.Add(this.label19, 0, 2);
            this.tableLayoutPanel5.Controls.Add(this.label20, 0, 1);
            this.tableLayoutPanel5.Controls.Add(this.label18, 0, 0);
            this.tableLayoutPanel5.Controls.Add(this.aggregationCurrentlyRunning, 1, 0);
            this.tableLayoutPanel5.Controls.Add(this.aggregationLastRun, 1, 1);
            this.tableLayoutPanel5.Controls.Add(this.aggregationCompletionStatus, 1, 2);
            this.tableLayoutPanel5.Location = new System.Drawing.Point(6, 17);
            this.tableLayoutPanel5.Name = "tableLayoutPanel5";
            this.tableLayoutPanel5.RowCount = 3;
            this.tableLayoutPanel5.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel5.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel5.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel5.Size = new System.Drawing.Size(232, 77);
            this.tableLayoutPanel5.TabIndex = 68;
            // 
            // label19
            // 
            this.label19.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.label19.AutoSize = true;
            this.label19.Location = new System.Drawing.Point(3, 58);
            this.label19.Name = "label19";
            this.label19.Size = new System.Drawing.Size(90, 13);
            this.label19.TabIndex = 70;
            this.label19.Text = "Completion status";
            // 
            // label20
            // 
            this.label20.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.label20.AutoSize = true;
            this.label20.Location = new System.Drawing.Point(3, 32);
            this.label20.Name = "label20";
            this.label20.Size = new System.Drawing.Size(91, 13);
            this.label20.TabIndex = 69;
            this.label20.Text = "Last completed at";
            // 
            // label18
            // 
            this.label18.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.label18.AutoSize = true;
            this.label18.Location = new System.Drawing.Point(3, 6);
            this.label18.Name = "label18";
            this.label18.Size = new System.Drawing.Size(86, 13);
            this.label18.TabIndex = 68;
            this.label18.Text = "Currently running";
            // 
            // aggregationCurrentlyRunning
            // 
            this.aggregationCurrentlyRunning.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.aggregationCurrentlyRunning.Location = new System.Drawing.Point(100, 3);
            this.aggregationCurrentlyRunning.Name = "aggregationCurrentlyRunning";
            this.aggregationCurrentlyRunning.ReadOnly = true;
            this.aggregationCurrentlyRunning.Size = new System.Drawing.Size(129, 20);
            this.aggregationCurrentlyRunning.TabIndex = 60;
            this.aggregationCurrentlyRunning.TabStop = false;
            // 
            // aggregationLastRun
            // 
            this.aggregationLastRun.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.aggregationLastRun.Location = new System.Drawing.Point(100, 29);
            this.aggregationLastRun.Name = "aggregationLastRun";
            this.aggregationLastRun.ReadOnly = true;
            this.aggregationLastRun.Size = new System.Drawing.Size(129, 20);
            this.aggregationLastRun.TabIndex = 62;
            this.aggregationLastRun.TabStop = false;
            // 
            // aggregationCompletionStatus
            // 
            this.aggregationCompletionStatus.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.aggregationCompletionStatus.Location = new System.Drawing.Point(100, 55);
            this.aggregationCompletionStatus.Name = "aggregationCompletionStatus";
            this.aggregationCompletionStatus.ReadOnly = true;
            this.aggregationCompletionStatus.Size = new System.Drawing.Size(129, 20);
            this.aggregationCompletionStatus.TabIndex = 63;
            this.aggregationCompletionStatus.TabStop = false;
            // 
            // groupBox3
            // 
            this.groupBox3.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this.groupBox3.Controls.Add(this.tableLayoutPanel2);
            this.groupBox3.Location = new System.Drawing.Point(20, 485);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Size = new System.Drawing.Size(249, 103);
            this.groupBox3.TabIndex = 69;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = "Data Grooming";
            // 
            // tableLayoutPanel2
            // 
            this.tableLayoutPanel2.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.tableLayoutPanel2.ColumnCount = 2;
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel2.Controls.Add(this.label8, 0, 1);
            this.tableLayoutPanel2.Controls.Add(this.currentlyRunning, 1, 0);
            this.tableLayoutPanel2.Controls.Add(this.label10, 0, 0);
            this.tableLayoutPanel2.Controls.Add(this.lastRun, 1, 1);
            this.tableLayoutPanel2.Controls.Add(this.completionStatus, 1, 2);
            this.tableLayoutPanel2.Controls.Add(this.label9, 0, 2);
            this.tableLayoutPanel2.Location = new System.Drawing.Point(5, 18);
            this.tableLayoutPanel2.Name = "tableLayoutPanel2";
            this.tableLayoutPanel2.RowCount = 1;
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel2.Size = new System.Drawing.Size(232, 77);
            this.tableLayoutPanel2.TabIndex = 41;
            // 
            // label8
            // 
            this.label8.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(3, 32);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(91, 13);
            this.label8.TabIndex = 35;
            this.label8.Text = "Last completed at";
            // 
            // currentlyRunning
            // 
            this.currentlyRunning.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this.currentlyRunning.Location = new System.Drawing.Point(100, 3);
            this.currentlyRunning.Name = "currentlyRunning";
            this.currentlyRunning.ReadOnly = true;
            this.currentlyRunning.Size = new System.Drawing.Size(129, 20);
            this.currentlyRunning.TabIndex = 8;
            this.currentlyRunning.TabStop = false;
            // 
            // label10
            // 
            this.label10.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.label10.AutoSize = true;
            this.label10.Location = new System.Drawing.Point(3, 6);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(86, 13);
            this.label10.TabIndex = 40;
            this.label10.Text = "Currently running";
            // 
            // lastRun
            // 
            this.lastRun.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this.lastRun.Location = new System.Drawing.Point(100, 29);
            this.lastRun.Name = "lastRun";
            this.lastRun.ReadOnly = true;
            this.lastRun.Size = new System.Drawing.Size(129, 20);
            this.lastRun.TabIndex = 10;
            this.lastRun.TabStop = false;
            // 
            // completionStatus
            // 
            this.completionStatus.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this.completionStatus.Location = new System.Drawing.Point(100, 55);
            this.completionStatus.Name = "completionStatus";
            this.completionStatus.ReadOnly = true;
            this.completionStatus.Size = new System.Drawing.Size(129, 20);
            this.completionStatus.TabIndex = 11;
            this.completionStatus.TabStop = false;
            // 
            // label9
            // 
            this.label9.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.label9.AutoSize = true;
            this.label9.Location = new System.Drawing.Point(3, 58);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(90, 13);
            this.label9.TabIndex = 36;
            this.label9.Text = "Completion status";
            // 
            // propertiesHeaderStrip2
            // 
            this.propertiesHeaderStrip2.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tableLayoutPanel3.SetColumnSpan(this.propertiesHeaderStrip2, 2);
            this.propertiesHeaderStrip2.ForeColor = System.Drawing.Color.Black;
            this.propertiesHeaderStrip2.Location = new System.Drawing.Point(3, 454);
            this.propertiesHeaderStrip2.Name = "propertiesHeaderStrip2";
            this.propertiesHeaderStrip2.Size = new System.Drawing.Size(539, 25);
            this.propertiesHeaderStrip2.TabIndex = 61;
            this.propertiesHeaderStrip2.TabStop = false;
            this.propertiesHeaderStrip2.Text = "What is the current grooming status?";
            // 
            // informationBox2
            // 
            this.informationBox2.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tableLayoutPanel3.SetColumnSpan(this.informationBox2, 2);
            this.informationBox2.Location = new System.Drawing.Point(3, 267);
            this.informationBox2.Name = "informationBox2";
            this.informationBox2.Size = new System.Drawing.Size(539, 45);
            this.informationBox2.TabIndex = 59;
            this.informationBox2.TabStop = false;
            this.informationBox2.Text = resources.GetString("informationBox2.Text");
            // 
            // propertiesHeaderStrip1
            // 
            this.tableLayoutPanel3.SetColumnSpan(this.propertiesHeaderStrip1, 2);
            this.propertiesHeaderStrip1.ForeColor = System.Drawing.Color.Black;
            this.propertiesHeaderStrip1.Location = new System.Drawing.Point(3, 236);
            this.propertiesHeaderStrip1.Name = "propertiesHeaderStrip1";
            this.propertiesHeaderStrip1.Size = new System.Drawing.Size(539, 25);
            this.propertiesHeaderStrip1.TabIndex = 58;
            this.propertiesHeaderStrip1.TabStop = false;
            this.propertiesHeaderStrip1.Text = "When should data be groomed to save repository space?";
            // 
            // groupBox2
            // 
            this.groupBox2.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left)));
            this.groupBox2.Controls.Add(this.tableLayoutPanel6);
            this.groupBox2.Controls.Add(this.label15);
            this.groupBox2.Location = new System.Drawing.Point(275, 105);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(246, 125);
            this.groupBox2.TabIndex = 57;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Aggregation";
            // 
            // tableLayoutPanel6
            // 
            this.tableLayoutPanel6.AutoSize = true;
            this.tableLayoutPanel6.ColumnCount = 3;
            this.tableLayoutPanel6.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel6.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel6.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel6.Controls.Add(this.label13, 2, 1);
            this.tableLayoutPanel6.Controls.Add(this.aggregationTimeIntervalCombo, 1, 1);
            this.tableLayoutPanel6.Controls.Add(this.aggregationHourlyButton, 0, 1);
            this.tableLayoutPanel6.Controls.Add(this.aggregationOnceDailyButton, 0, 0);
            this.tableLayoutPanel6.Controls.Add(this.aggregationTimeEditor, 1, 0);
            this.tableLayoutPanel6.Location = new System.Drawing.Point(6, 52);
            this.tableLayoutPanel6.Name = "tableLayoutPanel6";
            this.tableLayoutPanel6.RowCount = 2;
            this.tableLayoutPanel6.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel6.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel6.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.tableLayoutPanel6.Size = new System.Drawing.Size(234, 54);
            this.tableLayoutPanel6.TabIndex = 52;
            // 
            // label13
            // 
            this.label13.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.label13.AutoSize = true;
            this.label13.Location = new System.Drawing.Point(179, 34);
            this.label13.Name = "label13";
            this.label13.Size = new System.Drawing.Size(33, 13);
            this.label13.TabIndex = 49;
            this.label13.Text = "hours";
            // 
            // aggregationTimeIntervalCombo
            // 
            this.aggregationTimeIntervalCombo.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.aggregationTimeIntervalCombo.AutoCompleteMode = System.Windows.Forms.AutoCompleteMode.SuggestAppend;
            this.aggregationTimeIntervalCombo.AutoCompleteSource = System.Windows.Forms.AutoCompleteSource.ListItems;
            this.aggregationTimeIntervalCombo.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.aggregationTimeIntervalCombo.Enabled = false;
            this.aggregationTimeIntervalCombo.FormattingEnabled = true;
            this.aggregationTimeIntervalCombo.Items.AddRange(new object[] {
            "1",
            "2",
            "3",
            "4",
            "5",
            "6",
            "7",
            "8",
            "9",
            "10",
            "11",
            "12",
            "13",
            "14",
            "15",
            "16",
            "17",
            "18",
            "19",
            "20",
            "21",
            "22",
            "23"});
            this.aggregationTimeIntervalCombo.Location = new System.Drawing.Point(96, 30);
            this.aggregationTimeIntervalCombo.Name = "aggregationTimeIntervalCombo";
            this.aggregationTimeIntervalCombo.Size = new System.Drawing.Size(77, 21);
            this.aggregationTimeIntervalCombo.TabIndex = 3;
            // 
            // aggregationHourlyButton
            // 
            this.aggregationHourlyButton.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.aggregationHourlyButton.AutoSize = true;
            this.aggregationHourlyButton.Location = new System.Drawing.Point(3, 32);
            this.aggregationHourlyButton.Name = "aggregationHourlyButton";
            this.aggregationHourlyButton.Size = new System.Drawing.Size(55, 17);
            this.aggregationHourlyButton.TabIndex = 1;
            this.aggregationHourlyButton.Text = "Every ";
            this.aggregationHourlyButton.UseVisualStyleBackColor = true;
            this.aggregationHourlyButton.CheckedChanged += new System.EventHandler(this.aggregationHourlyButton_CheckedChanged);
            // 
            // aggregationOnceDailyButton
            // 
            this.aggregationOnceDailyButton.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.aggregationOnceDailyButton.AutoSize = true;
            this.aggregationOnceDailyButton.Checked = true;
            this.aggregationOnceDailyButton.Location = new System.Drawing.Point(3, 5);
            this.aggregationOnceDailyButton.Name = "aggregationOnceDailyButton";
            this.aggregationOnceDailyButton.Size = new System.Drawing.Size(87, 17);
            this.aggregationOnceDailyButton.TabIndex = 0;
            this.aggregationOnceDailyButton.TabStop = true;
            this.aggregationOnceDailyButton.Text = "Once daily at";
            this.aggregationOnceDailyButton.UseVisualStyleBackColor = true;
            this.aggregationOnceDailyButton.CheckedChanged += new System.EventHandler(this.aggregationOnceDailyButton_CheckedChanged);
            // 
            // aggregationTimeEditor
            // 
            this.aggregationTimeEditor.Anchor = System.Windows.Forms.AnchorStyles.Left;
            dropDownEditorButton1.Key = "DropDownList";
            dropDownEditorButton1.PreferredDropDownSize = new System.Drawing.Size(0, 0);
            this.aggregationTimeEditor.ButtonsRight.Add(dropDownEditorButton1);
            this.aggregationTimeEditor.DateTime = new System.DateTime(2007, 6, 7, 0, 0, 0, 0);
            this.aggregationTimeEditor.DropDownButtonDisplayStyle = Infragistics.Win.ButtonDisplayStyle.Never;
            this.aggregationTimeEditor.FormatString = "hh:mm tt";
            this.aggregationTimeEditor.ListInterval = System.TimeSpan.Parse("00:30:00");
            this.aggregationTimeEditor.Location = new System.Drawing.Point(96, 3);
            this.aggregationTimeEditor.MaskInput = "hh:mm tt";
            this.aggregationTimeEditor.Name = "aggregationTimeEditor";
            this.aggregationTimeEditor.Nullable = false;
            this.aggregationTimeEditor.Size = new System.Drawing.Size(77, 21);
            this.aggregationTimeEditor.TabIndex = 2;
            this.aggregationTimeEditor.Time = System.TimeSpan.Parse("00:00:00");
            this.aggregationTimeEditor.Value = new System.DateTime(2007, 6, 7, 0, 0, 0, 0);
            this.aggregationTimeEditor.ValueChanged += new System.EventHandler(this.ConfigValueChanged);
            // 
            // label15
            // 
            this.label15.Location = new System.Drawing.Point(9, 22);
            this.label15.Name = "label15";
            this.label15.Size = new System.Drawing.Size(206, 35);
            this.label15.TabIndex = 51;
            this.label15.Text = "Combine older data into daily roll-up records to save space.";
            // 
            // groupBox1
            // 
            this.groupBox1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBox1.Controls.Add(this.tableLayoutPanel1);
            this.groupBox1.Controls.Add(this.label14);
            this.groupBox1.Location = new System.Drawing.Point(23, 105);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(246, 125);
            this.groupBox1.TabIndex = 56;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Data Grooming";
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.tableLayoutPanel1.AutoSize = true;
            this.tableLayoutPanel1.ColumnCount = 3;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel1.Controls.Add(this.hourlyButton, 0, 1);
            this.tableLayoutPanel1.Controls.Add(this.label1, 2, 1);
            this.tableLayoutPanel1.Controls.Add(this.onceDailyButton, 0, 0);
            this.tableLayoutPanel1.Controls.Add(this.groomTimeIntervalCombo, 1, 1);
            this.tableLayoutPanel1.Controls.Add(this.groomingTimeEditor, 1, 0);
            this.tableLayoutPanel1.Location = new System.Drawing.Point(5, 52);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 2;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.tableLayoutPanel1.Size = new System.Drawing.Size(239, 54);
            this.tableLayoutPanel1.TabIndex = 51;
            // 
            // hourlyButton
            // 
            this.hourlyButton.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.hourlyButton.AutoSize = true;
            this.hourlyButton.Location = new System.Drawing.Point(3, 32);
            this.hourlyButton.Name = "hourlyButton";
            this.hourlyButton.Size = new System.Drawing.Size(55, 17);
            this.hourlyButton.TabIndex = 1;
            this.hourlyButton.Text = "Every ";
            this.hourlyButton.UseVisualStyleBackColor = true;
            this.hourlyButton.CheckedChanged += new System.EventHandler(this.hourlyButton_CheckedChanged);
            // 
            // label1
            // 
            this.label1.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(179, 34);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(33, 13);
            this.label1.TabIndex = 49;
            this.label1.Text = "hours";
            // 
            // onceDailyButton
            // 
            this.onceDailyButton.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.onceDailyButton.AutoSize = true;
            this.onceDailyButton.Checked = true;
            this.onceDailyButton.Location = new System.Drawing.Point(3, 5);
            this.onceDailyButton.Name = "onceDailyButton";
            this.onceDailyButton.Size = new System.Drawing.Size(87, 17);
            this.onceDailyButton.TabIndex = 0;
            this.onceDailyButton.TabStop = true;
            this.onceDailyButton.Text = "Once daily at";
            this.onceDailyButton.UseVisualStyleBackColor = true;
            this.onceDailyButton.CheckedChanged += new System.EventHandler(this.onceDailyButton_CheckedChanged);
            // 
            // groomTimeIntervalCombo
            // 
            this.groomTimeIntervalCombo.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.groomTimeIntervalCombo.AutoCompleteMode = System.Windows.Forms.AutoCompleteMode.SuggestAppend;
            this.groomTimeIntervalCombo.AutoCompleteSource = System.Windows.Forms.AutoCompleteSource.ListItems;
            this.groomTimeIntervalCombo.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.groomTimeIntervalCombo.Enabled = false;
            this.groomTimeIntervalCombo.FormattingEnabled = true;
            this.groomTimeIntervalCombo.Items.AddRange(new object[] {
            "1",
            "2",
            "3",
            "4",
            "5",
            "6",
            "7",
            "8",
            "9",
            "10",
            "11",
            "12",
            "13",
            "14",
            "15",
            "16",
            "17",
            "18",
            "19",
            "20",
            "21",
            "22",
            "23"});
            this.groomTimeIntervalCombo.Location = new System.Drawing.Point(96, 30);
            this.groomTimeIntervalCombo.Name = "groomTimeIntervalCombo";
            this.groomTimeIntervalCombo.Size = new System.Drawing.Size(77, 21);
            this.groomTimeIntervalCombo.TabIndex = 3;
            this.groomTimeIntervalCombo.SelectedValueChanged += new System.EventHandler(this.ConfigValueChanged);
            this.groomTimeIntervalCombo.TextChanged += new System.EventHandler(this.ConfigValueChanged);
            // 
            // groomingTimeEditor
            // 
            this.groomingTimeEditor.Anchor = System.Windows.Forms.AnchorStyles.Left;
            dropDownEditorButton2.Key = "DropDownList";
            dropDownEditorButton2.PreferredDropDownSize = new System.Drawing.Size(0, 0);
            this.groomingTimeEditor.ButtonsRight.Add(dropDownEditorButton2);
            this.groomingTimeEditor.DateTime = new System.DateTime(2007, 6, 7, 0, 0, 0, 0);
            this.groomingTimeEditor.DropDownButtonDisplayStyle = Infragistics.Win.ButtonDisplayStyle.Never;
            this.groomingTimeEditor.FormatString = "hh:mm tt";
            this.groomingTimeEditor.ListInterval = System.TimeSpan.Parse("00:30:00");
            this.groomingTimeEditor.Location = new System.Drawing.Point(96, 3);
            this.groomingTimeEditor.MaskInput = "hh:mm tt";
            this.groomingTimeEditor.Name = "groomingTimeEditor";
            this.groomingTimeEditor.Nullable = false;
            this.groomingTimeEditor.Size = new System.Drawing.Size(77, 21);
            this.groomingTimeEditor.TabIndex = 2;
            this.groomingTimeEditor.Time = System.TimeSpan.Parse("00:00:00");
            this.groomingTimeEditor.Value = new System.DateTime(2007, 6, 7, 0, 0, 0, 0);
            this.groomingTimeEditor.ValueChanged += new System.EventHandler(this.ConfigValueChanged);
            // 
            // label14
            // 
            this.label14.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.label14.Location = new System.Drawing.Point(14, 22);
            this.label14.Name = "label14";
            this.label14.Size = new System.Drawing.Size(225, 35);
            this.label14.TabIndex = 50;
            this.label14.Text = "Delete old data and perform table and index maintenance.";
            // 
            // informationBox1
            // 
            this.informationBox1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tableLayoutPanel3.SetColumnSpan(this.informationBox1, 2);
            this.informationBox1.Location = new System.Drawing.Point(3, 34);
            this.informationBox1.Name = "informationBox1";
            this.informationBox1.Size = new System.Drawing.Size(539, 52);
            this.informationBox1.TabIndex = 11;
            this.informationBox1.TabStop = false;
            this.informationBox1.Text = resources.GetString("informationBox1.Text");
            // 
            // propertiesHeaderStrip3
            // 
            this.propertiesHeaderStrip3.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tableLayoutPanel3.SetColumnSpan(this.propertiesHeaderStrip3, 2);
            this.propertiesHeaderStrip3.ForeColor = System.Drawing.Color.Black;
            this.propertiesHeaderStrip3.Location = new System.Drawing.Point(3, 3);
            this.propertiesHeaderStrip3.Name = "propertiesHeaderStrip3";
            this.propertiesHeaderStrip3.Size = new System.Drawing.Size(539, 25);
            this.propertiesHeaderStrip3.TabIndex = 9;
            this.propertiesHeaderStrip3.TabStop = false;
            this.propertiesHeaderStrip3.Text = "What time of the day should grooming occur?";
            // 
            // currentTimeLabel
            // 
            this.currentTimeLabel.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.currentTimeLabel.AutoSize = true;
            this.tableLayoutPanel3.SetColumnSpan(this.currentTimeLabel, 2);
            this.currentTimeLabel.Location = new System.Drawing.Point(3, 89);
            this.currentTimeLabel.Name = "currentTimeLabel";
            this.currentTimeLabel.Size = new System.Drawing.Size(195, 13);
            this.currentTimeLabel.TabIndex = 42;
            this.currentTimeLabel.Text = "The current time on the repository is {0}.";
            // 
            // GroomingOptionsDialog
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoScroll = true;
            //this.AutoScrollMinSize = new Size(565, 810);
            this.ClientSize = new System.Drawing.Size(565, 710);
            this.Controls.Add(this.aggregateNow);
            this.Controls.Add(this.apply);
            this.Controls.Add(this.containerPanel);
            this.Controls.Add(this.cancelButton);
            this.Controls.Add(this.okButton);
            this.Controls.Add(this.groomNow);
            //this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.HelpButton = true;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "GroomingOptionsDialog";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Grooming Options";
            this.Load += new System.EventHandler(this.GroomingOptionsDialog_Load);
            this.containerPanel.ResumeLayout(false);
            this.containerPanel.PerformLayout();
            this.tableLayoutPanel3.ResumeLayout(false);
            this.tableLayoutPanel3.PerformLayout();
            this.tableLayoutPanel4.ResumeLayout(false);
            this.tableLayoutPanel4.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.aggregationThresholdNumericUpDown)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.alertsGroomingThresholdNumericUpDown)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.activityGroomingThresholdNumericUpDown)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.metricsGroomingThresholdNumericUpDown)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.auditGroomingThresholdNumericUpDown)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.ForecastingAggregationThresholdNumericUpDown)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.GroomForecastNumericUpDown)).EndInit();
            this.groupBox4.ResumeLayout(false);
            this.tableLayoutPanel5.ResumeLayout(false);
            this.tableLayoutPanel5.PerformLayout();
            this.groupBox3.ResumeLayout(false);
            this.tableLayoutPanel2.ResumeLayout(false);
            this.tableLayoutPanel2.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.tableLayoutPanel6.ResumeLayout(false);
            this.tableLayoutPanel6.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.aggregationTimeEditor)).EndInit();
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.tableLayoutPanel1.ResumeLayout(false);
            this.tableLayoutPanel1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.groomingTimeEditor)).EndInit();
            updateButtonStyle();
            this.ResumeLayout(false);
        }

        void OnCurrentThemeChanged(object sender, EventArgs e)
        {
            SetPropertiesTheme();
            updateButtonStyle();
        }

        void SetPropertiesTheme()
        {
            var propertiesThemeManager = new Controls.PropertiesThemeManager();
            propertiesThemeManager.UpdatePropertyTheme(groomingPropertiesContentPage);
        }

        void updateButtonStyle()
        {
            if (Settings.Default.ColorScheme == "Dark")
            {
                this.groomNow.FlatStyle = FlatStyle.Standard;
                this.groomNow.UseVisualStyleBackColor = true;
                this.aggregateNow.FlatStyle = FlatStyle.Standard;
                this.aggregateNow.UseVisualStyleBackColor = true;
                this.refresh.FlatStyle = FlatStyle.Standard;
                this.refresh.UseVisualStyleBackColor = true;
            }
            else
            {
                this.groomNow.FlatStyle = FlatStyle.System;
                this.groomNow.UseVisualStyleBackColor = false;
                this.refresh.FlatStyle = FlatStyle.System;
                this.refresh.UseVisualStyleBackColor = false;
                this.aggregateNow.FlatStyle = FlatStyle.System;
                this.aggregateNow.UseVisualStyleBackColor = false;
            }
        }

        #endregion

        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomButton cancelButton;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomButton okButton;
        private Idera.SQLdm.DesktopClient.Controls.Office2007PropertyPage groomingPropertiesContentPage;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomButton groomNow;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomButton apply;
        private System.Windows.Forms.Timer timer1;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomButton aggregateNow;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomPanel  containerPanel;
        private TableLayoutPanel tableLayoutPanel3;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomInformationBox informationBox1;
        private Controls.PropertiesHeaderStrip propertiesHeaderStrip3;
        private Controls.PropertiesHeaderStrip propertiesHeaderStrip1;
        private GroupBox groupBox2;
        private Label label15;
        private Label label13;
        private RadioButton aggregationHourlyButton;
        private RadioButton aggregationOnceDailyButton;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomTimeComboEditor aggregationTimeEditor;
        private GroupBox groupBox1;
        private TableLayoutPanel tableLayoutPanel1;
        private RadioButton hourlyButton;
        private Label label1;
        private RadioButton onceDailyButton;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomComboBox groomTimeIntervalCombo;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomTimeComboEditor groomingTimeEditor;
        private Label label14;
        private Label currentTimeLabel;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomInformationBox informationBox2;
        private Controls.PropertiesHeaderStrip propertiesHeaderStrip2;
        private GroupBox groupBox4;
        private TableLayoutPanel tableLayoutPanel5;
        private Label label19;
        private Label label20;
        private Label label18;
        private TextBox aggregationCurrentlyRunning;
        private TextBox aggregationLastRun;
        private TextBox aggregationCompletionStatus;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomGroupBox groupBox3;
        private TableLayoutPanel tableLayoutPanel2;
        private Label label8;
        private TextBox currentlyRunning;
        private Label label10;
        private TextBox lastRun;
        private TextBox completionStatus;
        private Label label9;
        private Button refresh;
        private TableLayoutPanel tableLayoutPanel4;
        private CustomNumericUpDown aggregationThresholdNumericUpDown;
        private CustomNumericUpDown alertsGroomingThresholdNumericUpDown;
        private CustomNumericUpDown activityGroomingThresholdNumericUpDown;
        private Label label17;
        private Label label16;
        private Label label6;
        private Label label11;
        private Label label3;
        private Label label5;
        private CustomNumericUpDown metricsGroomingThresholdNumericUpDown;
        private Label label12;
        private Label label2;
        private TableLayoutPanel tableLayoutPanel6;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomComboBox aggregationTimeIntervalCombo;
        private Label lblAudit;
        private CustomNumericUpDown auditGroomingThresholdNumericUpDown;
        private Label label4;
        //10.0 SQLdm srishti purohit
        //START - Prescriptive analysis old data grooming implementation
        private Label lblPrescriptiveAnalysisGroom;
        private CustomNumericUpDown PAGroomingThresholdNumericUpDown;
        private Label lblPADays;
        //END - Prescriptive analysis old data grooming implementation
        //START - Forecasting Aggregation old data implementation

        private Label lblForecastingDataAggregation;
        private CustomNumericUpDown ForecastingAggregationThresholdNumericUpDown;

        private Label lblFADays;
        //END - Forecasting Aggregation old data implementation
        private Label lblGroomForecast;
        private CustomNumericUpDown GroomForecastNumericUpDown;

        private Label lblGroomForecastDays;
    }
}
