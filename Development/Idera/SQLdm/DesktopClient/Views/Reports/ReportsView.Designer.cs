namespace Idera.SQLdm.DesktopClient.Views.Reports
{
    partial class ReportsView
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
            this.contentPanel = new System.Windows.Forms.Panel();
            this.reportToolsPanel = new System.Windows.Forms.Panel();
            this.label1 = new System.Windows.Forms.Label();
            this.reportsToolStrip = new System.Windows.Forms.ToolStrip();
            this.backDropDownButton = new System.Windows.Forms.ToolStripSplitButton();
            this.forwardButton = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.refreshButton = new System.Windows.Forms.ToolStripButton();
            this.cancelButton = new System.Windows.Forms.ToolStripButton();
            this.gettingStartedButton = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
            this.toggleFilterPanelButton = new System.Windows.Forms.ToolStripButton();
            this.resetFiltersButton = new System.Windows.Forms.ToolStripButton();
            this.headerStrip = new Idera.SQLdm.DesktopClient.Controls.HeaderStrip();
            this.titleLabel = new System.Windows.Forms.ToolStripLabel();
            this.reportToolsPanel.SuspendLayout();
            this.reportsToolStrip.SuspendLayout();
            this.headerStrip.SuspendLayout();
            this.SuspendLayout();
            // 
            // contentPanel
            // 
            this.contentPanel.BackColor = System.Drawing.Color.Transparent;
            this.contentPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.contentPanel.Location = new System.Drawing.Point(0, 57);
            this.contentPanel.Name = "contentPanel";
            this.contentPanel.Size = new System.Drawing.Size(791, 475);
            this.contentPanel.TabIndex = 1;
            // 
            // reportToolsPanel
            // 
            this.reportToolsPanel.Controls.Add(this.label1);
            this.reportToolsPanel.Controls.Add(this.reportsToolStrip);
            this.reportToolsPanel.Dock = System.Windows.Forms.DockStyle.Top;
            this.reportToolsPanel.Location = new System.Drawing.Point(0, 25);
            this.reportToolsPanel.Name = "reportToolsPanel";
            this.reportToolsPanel.Size = new System.Drawing.Size(791, 32);
            this.reportToolsPanel.TabIndex = 4;
            // 
            // label1
            // 
            this.label1.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.label1.Dock = System.Windows.Forms.DockStyle.Top;
            this.label1.Location = new System.Drawing.Point(0, 30);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(791, 2);
            this.label1.TabIndex = 4;
            this.label1.Text = "label1";
            // 
            // reportsToolStrip
            // 
            this.reportsToolStrip.AutoSize = false;
            this.reportsToolStrip.GripStyle = System.Windows.Forms.ToolStripGripStyle.Hidden;
            this.reportsToolStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.backDropDownButton,
            this.forwardButton,
            this.toolStripSeparator1,
            this.refreshButton,
            this.cancelButton,
            this.gettingStartedButton,
            this.toolStripSeparator2,
            this.toggleFilterPanelButton,
            this.resetFiltersButton});
            this.reportsToolStrip.Location = new System.Drawing.Point(0, 0);
            this.reportsToolStrip.Name = "reportsToolStrip";
            this.reportsToolStrip.Size = new System.Drawing.Size(791, 30);
            this.reportsToolStrip.TabIndex = 3;
            // 
            // backDropDownButton
            // 
            this.backDropDownButton.AutoToolTip = false;
            this.backDropDownButton.Enabled = false;
            this.backDropDownButton.Image = global::Idera.SQLdm.DesktopClient.Properties.Resources.Back24x24;
            this.backDropDownButton.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None;
            this.backDropDownButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.backDropDownButton.Name = "backDropDownButton";
            this.backDropDownButton.Size = new System.Drawing.Size(72, 27);
            this.backDropDownButton.Text = "Back";
            this.backDropDownButton.ButtonClick += new System.EventHandler(this.backDropDownButton_ButtonClick);
            // 
            // forwardButton
            // 
            this.forwardButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.forwardButton.Enabled = false;
            this.forwardButton.Image = global::Idera.SQLdm.DesktopClient.Properties.Resources.Forward24x24;
            this.forwardButton.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None;
            this.forwardButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.forwardButton.Name = "forwardButton";
            this.forwardButton.Size = new System.Drawing.Size(28, 27);
            this.forwardButton.Text = "Forward";
            this.forwardButton.Click += new System.EventHandler(this.forwardButton_Click);
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(6, 30);
            // 
            // refreshButton
            // 
            this.refreshButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.refreshButton.Image = global::Idera.SQLdm.DesktopClient.Properties.Resources.Refresh24x24;
            this.refreshButton.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None;
            this.refreshButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.refreshButton.Name = "refreshButton";
            this.refreshButton.Size = new System.Drawing.Size(28, 27);
            this.refreshButton.Text = "Refresh";
            this.refreshButton.Click += new System.EventHandler(this.refreshButton_Click);
            // 
            // cancelButton
            // 
            this.cancelButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.cancelButton.Enabled = false;
            this.cancelButton.Image = global::Idera.SQLdm.DesktopClient.Properties.Resources.Stop24x24;
            this.cancelButton.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None;
            this.cancelButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.cancelButton.Name = "cancelButton";
            this.cancelButton.Size = new System.Drawing.Size(28, 27);
            this.cancelButton.Text = "Cancel";
            this.cancelButton.Click += new System.EventHandler(this.cancelButton_Click);
            // 
            // gettingStartedButton
            // 
            this.gettingStartedButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.gettingStartedButton.Image = global::Idera.SQLdm.DesktopClient.Properties.Resources.Home24x24;
            this.gettingStartedButton.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None;
            this.gettingStartedButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.gettingStartedButton.Name = "gettingStartedButton";
            this.gettingStartedButton.Size = new System.Drawing.Size(28, 27);
            this.gettingStartedButton.Text = "Getting Started";
            this.gettingStartedButton.Click += new System.EventHandler(this.gettingStartedButton_Click);
            // 
            // toolStripSeparator2
            // 
            this.toolStripSeparator2.Name = "toolStripSeparator2";
            this.toolStripSeparator2.Size = new System.Drawing.Size(6, 30);
            // 
            // toggleFilterPanelButton
            // 
            this.toggleFilterPanelButton.Alignment = System.Windows.Forms.ToolStripItemAlignment.Right;
            this.toggleFilterPanelButton.AutoToolTip = false;
            this.toggleFilterPanelButton.Image = global::Idera.SQLdm.DesktopClient.Properties.Resources.OptionsGlyphUp16x16;
            this.toggleFilterPanelButton.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None;
            this.toggleFilterPanelButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toggleFilterPanelButton.Name = "toggleFilterPanelButton";
            this.toggleFilterPanelButton.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
            this.toggleFilterPanelButton.Size = new System.Drawing.Size(79, 27);
            this.toggleFilterPanelButton.Text = "Hide Filters";
            this.toggleFilterPanelButton.Click += new System.EventHandler(this.toggleFilterPanelButton_Click);
            // 
            // resetFiltersButton
            // 
            this.resetFiltersButton.AutoToolTip = false;
            this.resetFiltersButton.Image = global::Idera.SQLdm.DesktopClient.Properties.Resources.NewFilter16x16;
            this.resetFiltersButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.resetFiltersButton.Name = "resetFiltersButton";
            this.resetFiltersButton.Size = new System.Drawing.Size(89, 27);
            this.resetFiltersButton.Text = "Reset Filters";
            this.resetFiltersButton.Click += new System.EventHandler(this.resetFiltersButton_Click);
            // 
            // headerStrip
            // 
            this.headerStrip.AutoSize = false;
            this.headerStrip.Font = new System.Drawing.Font("Arial", 12F, System.Drawing.FontStyle.Bold);
            // this.headerStrip.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(254)))), ((int)(((byte)(66)))), ((int)(((byte)(16)))));
            // 149,201,67 
            this.headerStrip.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(149)))), ((int)(((byte)(201)))), ((int)(((byte)(67)))));

            this.headerStrip.GripStyle = System.Windows.Forms.ToolStripGripStyle.Hidden;
            this.headerStrip.HeaderImage = global::Idera.SQLdm.DesktopClient.Properties.Resources.NavigationPaneReportsSmall;
            this.headerStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.titleLabel});
            this.headerStrip.Location = new System.Drawing.Point(0, 0);
            this.headerStrip.Name = "headerStrip";
            this.headerStrip.Padding = new System.Windows.Forms.Padding(20, 2, 0, 0);
            this.headerStrip.Size = new System.Drawing.Size(791, 25);
            this.headerStrip.TabIndex = 0;
            this.headerStrip.Text = "headerStrip1";
            // 
            // titleLabel
            // 
            this.titleLabel.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(75)))), ((int)(((byte)(75)))), ((int)(((byte)(75)))));
            this.titleLabel.Name = "titleLabel";
            this.titleLabel.Size = new System.Drawing.Size(70, 20);
            this.titleLabel.Text = "Reports";
            // 
            // ReportsView
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.contentPanel);
            this.Controls.Add(this.reportToolsPanel);
            this.Controls.Add(this.headerStrip);
            this.Name = "ReportsView";
            this.Size = new System.Drawing.Size(791, 532);
            this.reportToolsPanel.ResumeLayout(false);
            this.reportsToolStrip.ResumeLayout(false);
            this.reportsToolStrip.PerformLayout();
            this.headerStrip.ResumeLayout(false);
            this.headerStrip.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private Idera.SQLdm.DesktopClient.Controls.HeaderStrip headerStrip;
        private System.Windows.Forms.ToolStripLabel titleLabel;
        private System.Windows.Forms.Panel contentPanel;
        private System.Windows.Forms.ToolStrip reportsToolStrip;
        private System.Windows.Forms.ToolStripSplitButton backDropDownButton;
        private System.Windows.Forms.ToolStripButton forwardButton;
        private System.Windows.Forms.ToolStripButton cancelButton;
        private System.Windows.Forms.ToolStripButton gettingStartedButton;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.ToolStripButton toggleFilterPanelButton;
        private System.Windows.Forms.ToolStripButton refreshButton;
        private System.Windows.Forms.Panel reportToolsPanel;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator2;
        private System.Windows.Forms.ToolStripButton resetFiltersButton;
    }
}
