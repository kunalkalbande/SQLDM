using System;
using System.Configuration;
using Idera.SQLdm.DesktopClient.Properties;

namespace Idera.SQLdm.DesktopClient.Controls.NavigationPane {
    partial class TasksNavigationPane {
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
                ApplicationController.Default.ActiveViewChanged += new EventHandler(ApplicationController_ActiveViewChanged);
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
        private void InitializeComponent() {
            this.activeTasksRadioButton = new System.Windows.Forms.RadioButton();
            this.dividerLabel1 = new System.Windows.Forms.Label();
            this.completedTasksRadioButton = new System.Windows.Forms.RadioButton();
            this.byOwnerRadioButton = new System.Windows.Forms.RadioButton();
            this.byStatusRadioButton = new System.Windows.Forms.RadioButton();
            this.toggleFilterOptionsLinkLabel = new System.Windows.Forms.LinkLabel();
            this.currentViewPanel = new System.Windows.Forms.Panel();
            this.customRadioButton = new System.Windows.Forms.RadioButton();
            this.currentViewHeaderStrip = new Idera.SQLdm.DesktopClient.Controls.HeaderStrip();
            this.currentViewHeaderStripLabel = new System.Windows.Forms.ToolStripLabel();
            this.toggleCurrentViewPanelButton = new System.Windows.Forms.ToolStripButton();
            this.actionsPanel = new System.Windows.Forms.Panel();
            this.label1 = new System.Windows.Forms.Label();
            this.notificationRulesLabel = new System.Windows.Forms.LinkLabel();
            this.currentViewPanel.SuspendLayout();
            this.currentViewHeaderStrip.SuspendLayout();
            this.actionsPanel.SuspendLayout();
            this.SuspendLayout();
            // 
            // activeTasksRadioButton
            // 
            this.activeTasksRadioButton.AutoSize = true;
            this.activeTasksRadioButton.Checked = true;
            this.activeTasksRadioButton.Location = new System.Drawing.Point(14, 22);
            this.activeTasksRadioButton.Name = "activeTasksRadioButton";
            this.activeTasksRadioButton.Size = new System.Drawing.Size(55, 17);
            this.activeTasksRadioButton.TabIndex = 1;
            this.activeTasksRadioButton.TabStop = true;
            this.activeTasksRadioButton.Text = "Active";
            this.activeTasksRadioButton.UseVisualStyleBackColor = true;
            this.activeTasksRadioButton.CheckedChanged += new System.EventHandler(this.activeTasksRadioButton_CheckedChanged);
            // 
            // dividerLabel1
            // 
            this.dividerLabel1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(203)))), ((int)(((byte)(203)))), ((int)(((byte)(203)))));
            this.dividerLabel1.Dock = System.Windows.Forms.DockStyle.Top;
            this.dividerLabel1.Location = new System.Drawing.Point(0, 0);
            this.dividerLabel1.Name = "dividerLabel1";
            this.dividerLabel1.Size = new System.Drawing.Size(238, 3);
            this.dividerLabel1.TabIndex = 0;
            // 
            // completedTasksRadioButton
            // 
            this.completedTasksRadioButton.AutoSize = true;
            this.completedTasksRadioButton.Location = new System.Drawing.Point(14, 43);
            this.completedTasksRadioButton.Name = "completedTasksRadioButton";
            this.completedTasksRadioButton.Size = new System.Drawing.Size(75, 17);
            this.completedTasksRadioButton.TabIndex = 2;
            this.completedTasksRadioButton.Text = "Completed";
            this.completedTasksRadioButton.UseVisualStyleBackColor = true;
            this.completedTasksRadioButton.CheckedChanged += new System.EventHandler(this.completedTasksRadioButton_CheckedChanged);
            // 
            // byOwnerRadioButton
            // 
            this.byOwnerRadioButton.AutoSize = true;
            this.byOwnerRadioButton.Location = new System.Drawing.Point(14, 64);
            this.byOwnerRadioButton.Name = "byOwnerRadioButton";
            this.byOwnerRadioButton.Size = new System.Drawing.Size(71, 17);
            this.byOwnerRadioButton.TabIndex = 3;
            this.byOwnerRadioButton.Text = "By Owner";
            this.byOwnerRadioButton.UseVisualStyleBackColor = true;
            this.byOwnerRadioButton.CheckedChanged += new System.EventHandler(this.byOwnerRadioButton_CheckedChanged);
            // 
            // byStatusRadioButton
            // 
            this.byStatusRadioButton.AutoSize = true;
            this.byStatusRadioButton.Location = new System.Drawing.Point(14, 85);
            this.byStatusRadioButton.Name = "byStatusRadioButton";
            this.byStatusRadioButton.Size = new System.Drawing.Size(70, 17);
            this.byStatusRadioButton.TabIndex = 4;
            this.byStatusRadioButton.Text = "By Status";
            this.byStatusRadioButton.UseVisualStyleBackColor = true;
            this.byStatusRadioButton.CheckedChanged += new System.EventHandler(this.byStatusRadioButton_CheckedChanged);
            // 
            // toggleFilterOptionsLinkLabel
            // 
            this.toggleFilterOptionsLinkLabel.AutoSize = true;
            this.toggleFilterOptionsLinkLabel.LinkBehavior = System.Windows.Forms.LinkBehavior.HoverUnderline;
            this.toggleFilterOptionsLinkLabel.LinkColor = System.Drawing.Color.Blue;
            this.toggleFilterOptionsLinkLabel.Location = new System.Drawing.Point(11, 11);
            this.toggleFilterOptionsLinkLabel.Name = "toggleFilterOptionsLinkLabel";
            this.toggleFilterOptionsLinkLabel.Size = new System.Drawing.Size(98, 13);
            this.toggleFilterOptionsLinkLabel.TabIndex = 1;
            this.toggleFilterOptionsLinkLabel.TabStop = true;
            this.toggleFilterOptionsLinkLabel.Text = "Show Filter Options";
            this.toggleFilterOptionsLinkLabel.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.toggleFilterOptionsLinkLabel_LinkClicked);
            // 
            // currentViewPanel
            // 
            this.currentViewPanel.Controls.Add(this.customRadioButton);
            this.currentViewPanel.Controls.Add(this.currentViewHeaderStrip);
            this.currentViewPanel.Controls.Add(this.activeTasksRadioButton);
            this.currentViewPanel.Controls.Add(this.completedTasksRadioButton);
            this.currentViewPanel.Controls.Add(this.byStatusRadioButton);
            this.currentViewPanel.Controls.Add(this.byOwnerRadioButton);
            this.currentViewPanel.Dock = System.Windows.Forms.DockStyle.Top;
            this.currentViewPanel.Location = new System.Drawing.Point(0, 0);
            this.currentViewPanel.Name = "currentViewPanel";
            this.currentViewPanel.Size = new System.Drawing.Size(238, 130);
            this.currentViewPanel.TabIndex = 8;
            // 
            // customRadioButton
            // 
            this.customRadioButton.AutoSize = true;
            this.customRadioButton.Location = new System.Drawing.Point(14, 106);
            this.customRadioButton.Name = "customRadioButton";
            this.customRadioButton.Size = new System.Drawing.Size(60, 17);
            this.customRadioButton.TabIndex = 5;
            this.customRadioButton.Text = "Custom";
            this.customRadioButton.UseVisualStyleBackColor = true;
            this.customRadioButton.CheckedChanged += new System.EventHandler(this.customRadioButton_CheckedChanged);
            // 
            // currentViewHeaderStrip
            // 
            this.currentViewHeaderStrip.AutoSize = false;
            this.currentViewHeaderStrip.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.currentViewHeaderStrip.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(75)))), ((int)(((byte)(75)))), ((int)(((byte)(75)))));
            this.currentViewHeaderStrip.GripMargin = new System.Windows.Forms.Padding(0);
            this.currentViewHeaderStrip.GripStyle = System.Windows.Forms.ToolStripGripStyle.Hidden;
            this.currentViewHeaderStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.currentViewHeaderStripLabel,
            this.toggleCurrentViewPanelButton});
            this.currentViewHeaderStrip.Location = new System.Drawing.Point(0, 0);
            this.currentViewHeaderStrip.Name = "currentViewHeaderStrip";
            this.currentViewHeaderStrip.Padding = new System.Windows.Forms.Padding(0);
            this.currentViewHeaderStrip.Size = new System.Drawing.Size(238, 19);
            this.currentViewHeaderStrip.Style = Idera.SQLdm.DesktopClient.Controls.HeaderStripStyle.SmallSingle;
            this.currentViewHeaderStrip.TabIndex = 0;
            this.currentViewHeaderStrip.MouseClick += new System.Windows.Forms.MouseEventHandler(this.currentViewHeaderStrip_MouseClick);
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
            this.toggleCurrentViewPanelButton.Alignment = System.Windows.Forms.ToolStripItemAlignment.Right;
            this.toggleCurrentViewPanelButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.toggleCurrentViewPanelButton.Enabled = false;
            this.toggleCurrentViewPanelButton.Image = global::Idera.SQLdm.DesktopClient.Properties.Resources.HeaderStripSmallCollapse;
            this.toggleCurrentViewPanelButton.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None;
            this.toggleCurrentViewPanelButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toggleCurrentViewPanelButton.Name = "toggleCurrentViewPanelButton";
            this.toggleCurrentViewPanelButton.Size = new System.Drawing.Size(23, 16);
            // 
            // actionsPanel
            // 
            this.actionsPanel.Controls.Add(this.label1);
            this.actionsPanel.Controls.Add(this.notificationRulesLabel);
            this.actionsPanel.Controls.Add(this.dividerLabel1);
            this.actionsPanel.Controls.Add(this.toggleFilterOptionsLinkLabel);
            this.actionsPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.actionsPanel.Location = new System.Drawing.Point(0, 130);
            this.actionsPanel.Name = "actionsPanel";
            this.actionsPanel.Size = new System.Drawing.Size(238, 256);
            this.actionsPanel.TabIndex = 9;
            // 
            // label1
            // 
            this.label1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.label1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(203)))), ((int)(((byte)(203)))), ((int)(((byte)(203)))));
            this.label1.Location = new System.Drawing.Point(0, 33);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(238, 1);
            this.label1.TabIndex = 2;
            // 
            // notificationRulesLabel
            // 
            this.notificationRulesLabel.AutoSize = true;
            this.notificationRulesLabel.LinkBehavior = System.Windows.Forms.LinkBehavior.HoverUnderline;
            this.notificationRulesLabel.LinkColor = System.Drawing.Color.Blue;
            this.notificationRulesLabel.Location = new System.Drawing.Point(11, 45);
            this.notificationRulesLabel.Name = "notificationRulesLabel";
            this.notificationRulesLabel.Size = new System.Drawing.Size(152, 13);
            this.notificationRulesLabel.TabIndex = 3;
            this.notificationRulesLabel.TabStop = true;
            this.notificationRulesLabel.Text = "Alert Actions and Responses...";
            this.notificationRulesLabel.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.notificationRulesLabel_LinkClicked);
            // 
            // TasksNavigationPane
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.White;
            this.Controls.Add(this.actionsPanel);
            this.Controls.Add(this.currentViewPanel);
            this.Name = "TasksNavigationPane";
            this.Size = new System.Drawing.Size(238, 386);
            this.Load += new System.EventHandler(this.TasksNavigationPane_Load);
            this.currentViewPanel.ResumeLayout(false);
            this.currentViewPanel.PerformLayout();
            this.currentViewHeaderStrip.ResumeLayout(false);
            this.currentViewHeaderStrip.PerformLayout();
            this.actionsPanel.ResumeLayout(false);
            this.actionsPanel.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.RadioButton activeTasksRadioButton;
        private System.Windows.Forms.Label dividerLabel1;
        private System.Windows.Forms.RadioButton completedTasksRadioButton;
        private System.Windows.Forms.RadioButton byOwnerRadioButton;
        private System.Windows.Forms.RadioButton byStatusRadioButton;
        private System.Windows.Forms.LinkLabel toggleFilterOptionsLinkLabel;
        private System.Windows.Forms.Panel currentViewPanel;
        private HeaderStrip currentViewHeaderStrip;
        private System.Windows.Forms.ToolStripLabel currentViewHeaderStripLabel;
        private System.Windows.Forms.ToolStripButton toggleCurrentViewPanelButton;
        private System.Windows.Forms.Panel actionsPanel;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.LinkLabel notificationRulesLabel;
        private System.Windows.Forms.RadioButton customRadioButton;
    }
}
