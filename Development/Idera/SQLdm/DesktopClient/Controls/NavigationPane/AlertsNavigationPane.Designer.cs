using System;
using System.Configuration;
using System.Drawing;
using Idera.SQLdm.DesktopClient.Helpers;
using Idera.SQLdm.DesktopClient.Properties;

namespace Idera.SQLdm.DesktopClient.Controls.NavigationPane
{
    partial class AlertsNavigationPane
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
            if (disposing)
            {
                ApplicationController.Default.ActiveViewChanged -= new EventHandler(ApplicationController_ActiveViewChanged);
                Settings.Default.SettingChanging -= new SettingChangingEventHandler(Settings_SettingChanging);

                if (components != null)
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
            this.actionsPanel = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomPanel ();
            this.defaultAlertConfigurationLink = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomLinkLabel();
            this.label1 = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomLabel();
            this.notificationRulesLabel = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomLinkLabel();
            this.toggleDetailsLinkLabel = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomLinkLabel();
            this.toggleFilterOptionsLinkLabel = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomLinkLabel();
            this.dividerLabel1 = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomLabel();
            this.currentViewPanel = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomPanel ();
            this.currentViewHeaderStrip = new Idera.SQLdm.DesktopClient.Controls.HeaderStrip();
            this.currentViewHeaderStripLabel = new System.Windows.Forms.ToolStripLabel();
            this.toggleCurrentViewPanelButton = new System.Windows.Forms.ToolStripButton();
            this.currentViewOptionPanel = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomPanel ();
            this.customFilterRadioButton = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomRadioButton();
            this.tableReorgRadioButton = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomRadioButton();
            this.worstPerformingRadioButton = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomRadioButton();
            this.oldestOpenRadioButton = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomRadioButton();
            this.bombedJobsRadioButton = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomRadioButton();
            this.blokcedProcessesRadioButton = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomRadioButton();
            this.activeAlertsRadioButton = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomRadioButton();
            this.bySeverityRadioButton = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomRadioButton();
            this.byMetricRadioButton = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomRadioButton();
            this.byInstanceRadioButton = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomRadioButton();
            this.actionsPanel.SuspendLayout();
            this.currentViewPanel.SuspendLayout();
            this.currentViewHeaderStrip.SuspendLayout();
            this.currentViewOptionPanel.SuspendLayout();
            this.SuspendLayout();
            // 
            // actionsPanel
            // 
            this.actionsPanel.Controls.Add(this.defaultAlertConfigurationLink);
            this.actionsPanel.Controls.Add(this.label1);
            this.actionsPanel.Controls.Add(this.notificationRulesLabel);
            this.actionsPanel.Controls.Add(this.toggleDetailsLinkLabel);
            this.actionsPanel.Controls.Add(this.toggleFilterOptionsLinkLabel);
            this.actionsPanel.Controls.Add(this.dividerLabel1);
            this.actionsPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.actionsPanel.Location = new System.Drawing.Point(0, 253);
            this.actionsPanel.Name = "actionsPanel";
            this.actionsPanel.Size = new System.Drawing.Size(267, 253);
            this.actionsPanel.TabIndex = 11;
            // 
            // defaultAlertConfigurationLink
            // 
            this.defaultAlertConfigurationLink.AutoSize = true;
            this.defaultAlertConfigurationLink.LinkBehavior = System.Windows.Forms.LinkBehavior.HoverUnderline;
            this.defaultAlertConfigurationLink.LinkColor = System.Drawing.Color.Blue;
            this.defaultAlertConfigurationLink.Location = new System.Drawing.Point(11, 67);
            this.defaultAlertConfigurationLink.Name = "defaultAlertConfigurationLink";
            this.defaultAlertConfigurationLink.Size = new System.Drawing.Size(154, 13);
            this.defaultAlertConfigurationLink.TabIndex = 4;
            this.defaultAlertConfigurationLink.TabStop = true;
            this.defaultAlertConfigurationLink.Text = "Alert Configuration Templates...";
            this.defaultAlertConfigurationLink.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.defaultAlertConfigurationLink_LinkClicked);
            // 
            // label1
            // 
            this.label1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.label1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(203)))), ((int)(((byte)(203)))), ((int)(((byte)(203)))));
            this.label1.Location = new System.Drawing.Point(0, 57);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(267, 1);
            this.label1.TabIndex = 3;
            // 
            // notificationRulesLabel
            // 
            this.notificationRulesLabel.AutoSize = true;
            this.notificationRulesLabel.LinkBehavior = System.Windows.Forms.LinkBehavior.HoverUnderline;
            this.notificationRulesLabel.BackColor = backColor;
            this.notificationRulesLabel.LinkColor = System.Drawing.Color.Blue;
            this.notificationRulesLabel.Location = new System.Drawing.Point(11, 88);
            this.notificationRulesLabel.Name = "notificationRulesLabel";
            this.notificationRulesLabel.Size = new System.Drawing.Size(152, 13);
            this.notificationRulesLabel.TabIndex = 5;
            this.notificationRulesLabel.TabStop = true;
            this.notificationRulesLabel.Text = "Alert Actions and Responses...";
            this.notificationRulesLabel.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.notificationRulesLabel_LinkClicked);
            // 
            // toggleDetailsLinkLabel
            // 
            this.toggleDetailsLinkLabel.AutoSize = true;
            this.toggleDetailsLinkLabel.LinkBehavior = System.Windows.Forms.LinkBehavior.HoverUnderline;
            this.toggleDetailsLinkLabel.LinkColor = System.Drawing.Color.Blue;
            this.toggleDetailsLinkLabel.BackColor = backColor;
            this.toggleDetailsLinkLabel.Location = new System.Drawing.Point(11, 35);
            this.toggleDetailsLinkLabel.Name = "toggleDetailsLinkLabel";
            this.toggleDetailsLinkLabel.Size = new System.Drawing.Size(69, 13);
            this.toggleDetailsLinkLabel.TabIndex = 2;
            this.toggleDetailsLinkLabel.TabStop = true;
            this.toggleDetailsLinkLabel.Text = "Show Details";
            this.toggleDetailsLinkLabel.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.toggleDetailsLinkLabel_LinkClicked);
            // 
            // toggleFilterOptionsLinkLabel
            // 
            this.toggleFilterOptionsLinkLabel.AutoSize = true;
            this.toggleFilterOptionsLinkLabel.LinkBehavior = System.Windows.Forms.LinkBehavior.HoverUnderline;
            this.toggleFilterOptionsLinkLabel.LinkColor = System.Drawing.Color.Blue;
            this.toggleFilterOptionsLinkLabel.Location = new System.Drawing.Point(11, 13);
            this.toggleFilterOptionsLinkLabel.Name = "toggleFilterOptionsLinkLabel";
            this.toggleFilterOptionsLinkLabel.Size = new System.Drawing.Size(98, 13);
            this.toggleFilterOptionsLinkLabel.TabIndex = 1;
            this.toggleFilterOptionsLinkLabel.TabStop = true;
            this.toggleFilterOptionsLinkLabel.Text = "Show Filter Options";
            this.toggleFilterOptionsLinkLabel.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.toggleFilterOptionsLinkLabel_LinkClicked);
            // 
            // dividerLabel1
            // 
            this.dividerLabel1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(203)))), ((int)(((byte)(203)))), ((int)(((byte)(203)))));
            this.dividerLabel1.Dock = System.Windows.Forms.DockStyle.Top;
            this.dividerLabel1.Location = new System.Drawing.Point(0, 0);
            this.dividerLabel1.Name = "dividerLabel1";
            this.dividerLabel1.Size = new System.Drawing.Size(267, 3);
            this.dividerLabel1.TabIndex = 0;
            // 
            // currentViewPanel
            // 
            this.currentViewPanel.Controls.Add(this.currentViewOptionPanel);
            this.currentViewPanel.Controls.Add(this.currentViewHeaderStrip);
            this.currentViewPanel.Dock = System.Windows.Forms.DockStyle.Top;
            this.currentViewPanel.Location = new System.Drawing.Point(0, 0);
            this.currentViewPanel.Name = "currentViewPanel";
            this.currentViewPanel.Size = new System.Drawing.Size(267, 253);
            this.currentViewPanel.TabIndex = 10;
            // 
            // currentViewHeaderStrip
            // 
            this.currentViewHeaderStrip.AutoSize = false;
            this.currentViewHeaderStrip.Font = new System.Drawing.Font("Tahoma", 8.25F);
            this.currentViewHeaderStrip.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(75)))), ((int)(((byte)(75)))), ((int)(((byte)(75)))));
            this.currentViewHeaderStrip.GripMargin = new System.Windows.Forms.Padding(0);
            this.currentViewHeaderStrip.GripStyle = System.Windows.Forms.ToolStripGripStyle.Hidden;
            this.currentViewHeaderStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.currentViewHeaderStripLabel,
            this.toggleCurrentViewPanelButton});
            this.currentViewHeaderStrip.Location = new System.Drawing.Point(0, 0);
            this.currentViewHeaderStrip.Name = "currentViewHeaderStrip";
            this.currentViewHeaderStrip.Padding = new System.Windows.Forms.Padding(0);
            this.currentViewHeaderStrip.Size = new System.Drawing.Size(267, 19);
            this.currentViewHeaderStrip.Style = Idera.SQLdm.DesktopClient.Controls.HeaderStripStyle.SmallSingle;
            this.currentViewHeaderStrip.TabIndex = 0;
            this.currentViewHeaderStrip.MouseClick += new System.Windows.Forms.MouseEventHandler(this.currentViewHeaderStrip_MouseClick);
            // 
            // currentViewHeaderStripLabel
            // 
            this.currentViewHeaderStripLabel.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.currentViewHeaderStripLabel.Name = "currentViewHeaderStripLabel";
            this.currentViewHeaderStripLabel.Size = new System.Drawing.Size(79, 16);
            this.currentViewHeaderStripLabel.Margin = new System.Windows.Forms.Padding(8, 0, 0, 0);
            this.currentViewHeaderStripLabel.Text = "Current View";
            // 
            // toggleCurrentViewPanelButton
            // 
            this.toggleCurrentViewPanelButton.Alignment = System.Windows.Forms.ToolStripItemAlignment.Right;
            this.toggleCurrentViewPanelButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.toggleCurrentViewPanelButton.Enabled = false;
            this.toggleCurrentViewPanelButton.Image = global::Idera.SQLdm.DesktopClient.Properties.Resources.HeaderStripSmallCollapse;
            this.toggleCurrentViewPanelButton.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None;
            this.toggleCurrentViewPanelButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toggleCurrentViewPanelButton.Name = "toggleCurrentViewPanelButton";
            this.toggleCurrentViewPanelButton.Size = new System.Drawing.Size(23, 16);
            // 
            // currentViewOptionPanel
            // 
            this.currentViewOptionPanel.AutoScroll = true;
            this.currentViewOptionPanel.Controls.Add(this.customFilterRadioButton);
            this.currentViewOptionPanel.Controls.Add(this.tableReorgRadioButton);
            this.currentViewOptionPanel.Controls.Add(this.worstPerformingRadioButton);
            this.currentViewOptionPanel.Controls.Add(this.oldestOpenRadioButton);
            this.currentViewOptionPanel.Controls.Add(this.bombedJobsRadioButton);
            this.currentViewOptionPanel.Controls.Add(this.blokcedProcessesRadioButton);
            this.currentViewOptionPanel.Controls.Add(this.activeAlertsRadioButton);
            this.currentViewOptionPanel.Controls.Add(this.bySeverityRadioButton);
            this.currentViewOptionPanel.Controls.Add(this.byMetricRadioButton);
            this.currentViewOptionPanel.Controls.Add(this.byInstanceRadioButton);
            this.currentViewOptionPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.currentViewOptionPanel.Location = new System.Drawing.Point(0, 19);
            this.currentViewOptionPanel.Name = "currentViewOptionPanel";
            this.currentViewOptionPanel.Size = new System.Drawing.Size(267, 234);
            this.currentViewOptionPanel.TabIndex = 1;
            // 4K Screen resolution scaling
            int radioButtonYLocation = 23;
            if (AutoScaleSizeHelper.isScalingRequired)
            {
                if (AutoScaleSizeHelper.isLargeSize)
                    radioButtonYLocation = 24;
                if (AutoScaleSizeHelper.isXLargeSize)
                    radioButtonYLocation = 27;
                if (AutoScaleSizeHelper.isXXLargeSize)
                    radioButtonYLocation = 30;
            }
            else
                radioButtonYLocation = 23;
            // 
            // customFilterRadioButton
            // 
            this.customFilterRadioButton.AutoSize = true;
            this.customFilterRadioButton.Location = new System.Drawing.Point(20, 4 + (9 * radioButtonYLocation));
            this.customFilterRadioButton.Name = "customFilterRadioButton";
            this.customFilterRadioButton.Size = new System.Drawing.Size(60, 17);
            this.customFilterRadioButton.TabIndex = 20;
            this.customFilterRadioButton.Text = "Custom";
            this.customFilterRadioButton.UseVisualStyleBackColor = true;
            this.customFilterRadioButton.CheckedChanged += new System.EventHandler(this.customFilterRadioButton_CheckedChanged);
            // 
            // tableReorgRadioButton
            // 
            this.tableReorgRadioButton.AutoSize = true;
            this.tableReorgRadioButton.Location = new System.Drawing.Point(20, 4 + (8 * radioButtonYLocation));
            this.tableReorgRadioButton.Name = "tableReorgRadioButton";
            this.tableReorgRadioButton.Size = new System.Drawing.Size(122, 17);
            this.tableReorgRadioButton.TabIndex = 19;
            this.tableReorgRadioButton.Text = "Table Fragmentation";
            this.tableReorgRadioButton.UseVisualStyleBackColor = true;
            this.tableReorgRadioButton.CheckedChanged += new System.EventHandler(this.tableReorgRadioButton_CheckedChanged);
            // 
            // worstPerformingRadioButton
            // 
            this.worstPerformingRadioButton.AutoSize = true;
            this.worstPerformingRadioButton.Location = new System.Drawing.Point(20, 4 + (7 * radioButtonYLocation));
            this.worstPerformingRadioButton.Name = "worstPerformingRadioButton";
            this.worstPerformingRadioButton.Size = new System.Drawing.Size(127, 17);
            this.worstPerformingRadioButton.TabIndex = 18;
            this.worstPerformingRadioButton.Text = "Query Monitor Events";
            this.worstPerformingRadioButton.UseVisualStyleBackColor = true;
            this.worstPerformingRadioButton.CheckedChanged += new System.EventHandler(this.worstPerformingRadioButton_CheckedChanged);
            // 
            // oldestOpenRadioButton
            // 
            this.oldestOpenRadioButton.AutoSize = true;
            this.oldestOpenRadioButton.Location = new System.Drawing.Point(20, 4 + (6 * radioButtonYLocation));
            this.oldestOpenRadioButton.Name = "oldestOpenRadioButton";
            this.oldestOpenRadioButton.Size = new System.Drawing.Size(148, 17);
            this.oldestOpenRadioButton.TabIndex = 17;
            this.oldestOpenRadioButton.Text = "Oldest Open Transactions";
            this.oldestOpenRadioButton.UseVisualStyleBackColor = true;
            this.oldestOpenRadioButton.CheckedChanged += new System.EventHandler(this.oldestOpenRadioButton_CheckedChanged);
            // 
            // bombedJobsRadioButton
            // 
            this.bombedJobsRadioButton.AutoSize = true;
            this.bombedJobsRadioButton.Location = new System.Drawing.Point(20, 4 + (4 * radioButtonYLocation));
            this.bombedJobsRadioButton.Name = "bombedJobsRadioButton";
            this.bombedJobsRadioButton.Size = new System.Drawing.Size(136, 17);
            this.bombedJobsRadioButton.TabIndex = 15;
            this.bombedJobsRadioButton.Text = "SQL Agent Job Failures";
            this.bombedJobsRadioButton.UseVisualStyleBackColor = true;
            this.bombedJobsRadioButton.CheckedChanged += new System.EventHandler(this.bombedJobsRadioButton_CheckedChanged);
            // 
            // blokcedProcessesRadioButton
            // 
            this.blokcedProcessesRadioButton.AutoSize = true;
            this.blokcedProcessesRadioButton.Location = new System.Drawing.Point(20, 4 + (5 * radioButtonYLocation));
            this.blokcedProcessesRadioButton.Name = "blokcedProcessesRadioButton";
            this.blokcedProcessesRadioButton.Size = new System.Drawing.Size(109, 17);
            this.blokcedProcessesRadioButton.TabIndex = 16;
            this.blokcedProcessesRadioButton.Text = "Blocked Sessions";
            this.blokcedProcessesRadioButton.UseVisualStyleBackColor = true;
            this.blokcedProcessesRadioButton.CheckedChanged += new System.EventHandler(this.blokcedProcessesRadioButton_CheckedChanged);
            // 
            // currentViewHeaderStripLabel
            // 
            this.currentViewHeaderStripLabel.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.currentViewHeaderStripLabel.Name = "currentViewHeaderStripLabel";
            this.currentViewHeaderStripLabel.Size = new System.Drawing.Size(79, 16);
            this.currentViewHeaderStripLabel.Text = "Current View";
            // 
            // toggleCurrentViewPanelButton
            // 
            // activeAlertsRadioButton
            // 
            this.activeAlertsRadioButton.AutoSize = true;
            this.activeAlertsRadioButton.Checked = true;
            this.activeAlertsRadioButton.Location = new System.Drawing.Point(20, 4 + (0* radioButtonYLocation));
            this.activeAlertsRadioButton.Name = "activeAlertsRadioButton";
            this.activeAlertsRadioButton.Size = new System.Drawing.Size(55, 17);
            this.activeAlertsRadioButton.TabIndex = 11;
            this.activeAlertsRadioButton.TabStop = true;
            this.activeAlertsRadioButton.Text = "Active";
            this.activeAlertsRadioButton.UseVisualStyleBackColor = true;
            this.activeAlertsRadioButton.CheckedChanged += new System.EventHandler(this.activeAlertsRadioButton_CheckedChanged);
            // 
            // bySeverityRadioButton
            // 
            this.bySeverityRadioButton.AutoSize = true;
            this.bySeverityRadioButton.Location = new System.Drawing.Point(20, 4 + (1 * radioButtonYLocation));
            this.bySeverityRadioButton.Name = "bySeverityRadioButton";
            this.bySeverityRadioButton.Size = new System.Drawing.Size(78, 17);
            this.bySeverityRadioButton.TabIndex = 12;
            this.bySeverityRadioButton.Text = "By Severity";
            this.bySeverityRadioButton.UseVisualStyleBackColor = true;
            this.bySeverityRadioButton.CheckedChanged += new System.EventHandler(this.bySeverityRadioButton_CheckedChanged);
            // 
            // byMetricRadioButton
            // 
            this.byMetricRadioButton.AutoSize = true;
            this.byMetricRadioButton.Location = new System.Drawing.Point(20, 4 + (3 * radioButtonYLocation));
            this.byMetricRadioButton.Name = "byMetricRadioButton";
            this.byMetricRadioButton.Size = new System.Drawing.Size(69, 17);
            this.byMetricRadioButton.TabIndex = 14;
            this.byMetricRadioButton.Text = "By Metric";
            this.byMetricRadioButton.UseVisualStyleBackColor = true;
            this.byMetricRadioButton.CheckedChanged += new System.EventHandler(this.byMetricRadioButton_CheckedChanged);
            // 
            // byInstanceRadioButton
            // 
            this.byInstanceRadioButton.AutoSize = true;
            this.byInstanceRadioButton.Location = new System.Drawing.Point(20, 4 + (2 * radioButtonYLocation));
            this.byInstanceRadioButton.Name = "byInstanceRadioButton";
            this.byInstanceRadioButton.Size = new System.Drawing.Size(71, 17);
            this.byInstanceRadioButton.TabIndex = 13;
            this.byInstanceRadioButton.Text = "By Server";
            this.byInstanceRadioButton.UseVisualStyleBackColor = true;
            this.byInstanceRadioButton.CheckedChanged += new System.EventHandler(this.byInstanceRadioButton_CheckedChanged);
            // 
            // AlertsNavigationPane
            // 
            this.BackColor = System.Drawing.Color.White;
            this.Controls.Add(this.actionsPanel);
            this.Controls.Add(this.currentViewPanel);
            this.Name = "AlertsNavigationPane";
            this.Size = new System.Drawing.Size(267, 506);
            this.Load += new System.EventHandler(this.AlertsNavigationPane_Load);
            this.actionsPanel.ResumeLayout(false);
            this.actionsPanel.PerformLayout();
            this.currentViewPanel.ResumeLayout(false);
            this.currentViewHeaderStrip.ResumeLayout(false);
            this.currentViewHeaderStrip.PerformLayout();
            this.currentViewOptionPanel.ResumeLayout(false);
            this.currentViewOptionPanel.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomPanel  actionsPanel;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomLabel dividerLabel1;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomPanel  currentViewPanel;
        private HeaderStrip currentViewHeaderStrip;
        private System.Windows.Forms.ToolStripLabel currentViewHeaderStripLabel;
        private System.Windows.Forms.ToolStripButton toggleCurrentViewPanelButton;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomLinkLabel toggleDetailsLinkLabel;
        // private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomLinkLabel toggleFilterOptionsLinkLabel;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomLinkLabel toggleFilterOptionsLinkLabel;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomLinkLabel notificationRulesLabel;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomLabel label1;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomLinkLabel defaultAlertConfigurationLink;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomPanel  currentViewOptionPanel;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomRadioButton customFilterRadioButton;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomRadioButton tableReorgRadioButton;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomRadioButton worstPerformingRadioButton;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomRadioButton oldestOpenRadioButton;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomRadioButton bombedJobsRadioButton;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomRadioButton blokcedProcessesRadioButton;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomRadioButton activeAlertsRadioButton;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomRadioButton bySeverityRadioButton;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomRadioButton byMetricRadioButton;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomRadioButton byInstanceRadioButton;
    }
}
