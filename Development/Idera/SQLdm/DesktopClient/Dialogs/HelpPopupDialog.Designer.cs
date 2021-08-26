namespace Idera.SQLdm.DesktopClient.Dialogs
{
    partial class HelpPopupDialog
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(HelpPopupDialog));
            this.panel1 = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomPanel ();
            this.webBrowser = new System.Windows.Forms.WebBrowser();
            this.footerPanel = new Idera.SQLdm.DesktopClient.Controls.GradientPanel();
            this.showHelpContentsLinkLabel = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomLinkLabel();
            this.footerDividerLabel = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomLabel();
            this.configureAlertsLinkLabel = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomLinkLabel();
            this.showDetailsLinkLabel = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomLinkLabel();
            this.headerPanel = new Idera.SQLdm.DesktopClient.Controls.GradientPanel();
            this.closeButton = new System.Windows.Forms.PictureBox();
            this.titleDividerLabel = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomLabel();
            this.titleLabel = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomLabel();
            this.dialogIcon = new System.Windows.Forms.PictureBox();
            this.panel1.SuspendLayout();
            this.footerPanel.SuspendLayout();
            this.headerPanel.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.closeButton)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dialogIcon)).BeginInit();
            this.SuspendLayout();
            // 
            // panel1
            // 
            this.panel1.BackColor = System.Drawing.Color.White;
            this.panel1.Controls.Add(this.webBrowser);
            this.panel1.Controls.Add(this.footerPanel);
            this.panel1.Controls.Add(this.headerPanel);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel1.Location = new System.Drawing.Point(1, 1);
            this.panel1.Name = "panel1";
            this.panel1.Padding = new System.Windows.Forms.Padding(1);
            this.panel1.Size = new System.Drawing.Size(278, 323);
            this.panel1.TabIndex = 0;
            // 
            // webBrowser
            // 
            this.webBrowser.Dock = System.Windows.Forms.DockStyle.Fill;
            this.webBrowser.Location = new System.Drawing.Point(1, 24);
            this.webBrowser.MinimumSize = new System.Drawing.Size(20, 20);
            this.webBrowser.Name = "webBrowser";
            this.webBrowser.Size = new System.Drawing.Size(276, 238);
            this.webBrowser.TabIndex = 2;
            this.webBrowser.Url = new System.Uri("", System.UriKind.Relative);
            // 
            // footerPanel
            // 
            this.footerPanel.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(224)))), ((int)(((byte)(224)))), ((int)(((byte)(224)))));
            this.footerPanel.BackColor2 = System.Drawing.Color.FromArgb(((int)(((byte)(238)))), ((int)(((byte)(238)))), ((int)(((byte)(238)))));
            this.footerPanel.BorderColor = System.Drawing.SystemColors.ControlDark;
            this.footerPanel.BorderWidth = 0;
            this.footerPanel.Controls.Add(this.showHelpContentsLinkLabel);
            this.footerPanel.Controls.Add(this.footerDividerLabel);
            this.footerPanel.Controls.Add(this.configureAlertsLinkLabel);
            this.footerPanel.Controls.Add(this.showDetailsLinkLabel);
            this.footerPanel.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.footerPanel.GradientAngle = 90;
            this.footerPanel.Location = new System.Drawing.Point(1, 262);
            this.footerPanel.Name = "footerPanel";
            this.footerPanel.ShowBorder = false;
            this.footerPanel.Size = new System.Drawing.Size(276, 60);
            this.footerPanel.TabIndex = 3;
            // 
            // showHelpContentsLinkLabel
            // 
            this.showHelpContentsLinkLabel.AutoSize = true;
            this.showHelpContentsLinkLabel.BackColor = System.Drawing.Color.Transparent;
            this.showHelpContentsLinkLabel.LinkBehavior = System.Windows.Forms.LinkBehavior.HoverUnderline;
            this.showHelpContentsLinkLabel.LinkColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(136)))), ((int)(((byte)(228)))));
            this.showHelpContentsLinkLabel.Location = new System.Drawing.Point(3, 6);
            this.showHelpContentsLinkLabel.Name = "showHelpContentsLinkLabel";
            this.showHelpContentsLinkLabel.Size = new System.Drawing.Size(88, 13);
            this.showHelpContentsLinkLabel.TabIndex = 3;
            this.showHelpContentsLinkLabel.TabStop = true;
            this.showHelpContentsLinkLabel.Text = "Need more help?";
            this.showHelpContentsLinkLabel.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.showHelpContentsLinkLabel_LinkClicked);
            // 
            // footerDividerLabel
            // 
            this.footerDividerLabel.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(190)))), ((int)(((byte)(214)))), ((int)(((byte)(224)))));
            this.footerDividerLabel.Dock = System.Windows.Forms.DockStyle.Top;
            this.footerDividerLabel.Location = new System.Drawing.Point(0, 0);
            this.footerDividerLabel.Name = "footerDividerLabel";
            this.footerDividerLabel.Size = new System.Drawing.Size(276, 1);
            this.footerDividerLabel.TabIndex = 3;
            // 
            // configureAlertsLinkLabel
            // 
            this.configureAlertsLinkLabel.AutoSize = true;
            this.configureAlertsLinkLabel.BackColor = System.Drawing.Color.Transparent;
            this.configureAlertsLinkLabel.LinkBehavior = System.Windows.Forms.LinkBehavior.HoverUnderline;
            this.configureAlertsLinkLabel.LinkColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(136)))), ((int)(((byte)(228)))));
            this.configureAlertsLinkLabel.Location = new System.Drawing.Point(3, 40);
            this.configureAlertsLinkLabel.Name = "configureAlertsLinkLabel";
            this.configureAlertsLinkLabel.Size = new System.Drawing.Size(90, 13);
            this.configureAlertsLinkLabel.TabIndex = 2;
            this.configureAlertsLinkLabel.TabStop = true;
            this.configureAlertsLinkLabel.Text = "Configure Alerts...";
            this.configureAlertsLinkLabel.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.configureAlertsLinkLabel_LinkClicked);
            // 
            // showDetailsLinkLabel
            // 
            this.showDetailsLinkLabel.AutoSize = true;
            this.showDetailsLinkLabel.BackColor = System.Drawing.Color.Transparent;
            this.showDetailsLinkLabel.LinkBehavior = System.Windows.Forms.LinkBehavior.HoverUnderline;
            this.showDetailsLinkLabel.LinkColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(136)))), ((int)(((byte)(228)))));
            this.showDetailsLinkLabel.Location = new System.Drawing.Point(3, 23);
            this.showDetailsLinkLabel.Name = "showDetailsLinkLabel";
            this.showDetailsLinkLabel.Size = new System.Drawing.Size(69, 13);
            this.showDetailsLinkLabel.TabIndex = 1;
            this.showDetailsLinkLabel.TabStop = true;
            this.showDetailsLinkLabel.Text = "Show Details";
            this.showDetailsLinkLabel.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.showDetailsLinkLabel_LinkClicked);
            // 
            // headerPanel
            // 
            this.headerPanel.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(224)))), ((int)(((byte)(224)))), ((int)(((byte)(224)))));
            this.headerPanel.BackColor2 = System.Drawing.Color.FromArgb(((int)(((byte)(238)))), ((int)(((byte)(238)))), ((int)(((byte)(238)))));
            this.headerPanel.BorderColor = System.Drawing.SystemColors.ControlDark;
            this.headerPanel.BorderWidth = 0;
            this.headerPanel.Controls.Add(this.closeButton);
            this.headerPanel.Controls.Add(this.titleDividerLabel);
            this.headerPanel.Controls.Add(this.titleLabel);
            this.headerPanel.Controls.Add(this.dialogIcon);
            this.headerPanel.Dock = System.Windows.Forms.DockStyle.Top;
            this.headerPanel.GradientAngle = 90;
            this.headerPanel.Location = new System.Drawing.Point(1, 1);
            this.headerPanel.Name = "headerPanel";
            this.headerPanel.ShowBorder = false;
            this.headerPanel.Size = new System.Drawing.Size(276, 23);
            this.headerPanel.TabIndex = 0;
            this.headerPanel.MouseDown += new System.Windows.Forms.MouseEventHandler(this.headerPanel_MouseDown);
            this.headerPanel.MouseMove += new System.Windows.Forms.MouseEventHandler(this.headerPanel_MouseMove);
            this.headerPanel.MouseUp += new System.Windows.Forms.MouseEventHandler(this.headerPanel_MouseUp);
            // 
            // closeButton
            // 
            this.closeButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.closeButton.BackColor = System.Drawing.Color.Transparent;
            this.closeButton.Image = ((System.Drawing.Image)(resources.GetObject("closeButton.Image")));
            this.closeButton.Location = new System.Drawing.Point(257, 3);
            this.closeButton.Name = "closeButton";
            this.closeButton.Size = new System.Drawing.Size(16, 16);
            this.closeButton.SizeMode = System.Windows.Forms.PictureBoxSizeMode.CenterImage;
            this.closeButton.TabIndex = 3;
            this.closeButton.TabStop = false;
            this.closeButton.MouseClick += new System.Windows.Forms.MouseEventHandler(this.closeButton_MouseClick);
            this.closeButton.MouseEnter += new System.EventHandler(this.closeButton_MouseEnter);
            this.closeButton.MouseLeave += new System.EventHandler(this.closeButton_MouseLeave);
            // 
            // titleDividerLabel
            // 
            this.titleDividerLabel.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(207)))), ((int)(((byte)(207)))), ((int)(((byte)(207)))));
            this.titleDividerLabel.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.titleDividerLabel.Location = new System.Drawing.Point(0, 22);
            this.titleDividerLabel.Name = "titleDividerLabel";
            this.titleDividerLabel.Size = new System.Drawing.Size(276, 1);
            this.titleDividerLabel.TabIndex = 2;
            // 
            // titleLabel
            // 
            this.titleLabel.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.titleLabel.AutoEllipsis = true;
            this.titleLabel.BackColor = System.Drawing.Color.Transparent;
            this.titleLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.titleLabel.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(68)))), ((int)(((byte)(68)))), ((int)(((byte)(68)))));
            this.titleLabel.Location = new System.Drawing.Point(20, 3);
            this.titleLabel.Name = "titleLabel";
            this.titleLabel.Size = new System.Drawing.Size(231, 16);
            this.titleLabel.TabIndex = 1;
            this.titleLabel.Text = "SQL Diagnostic Manager Help";
            this.titleLabel.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.titleLabel.MouseDown += new System.Windows.Forms.MouseEventHandler(this.titleLabel_MouseDown);
            this.titleLabel.MouseMove += new System.Windows.Forms.MouseEventHandler(this.titleLabel_MouseMove);
            this.titleLabel.MouseUp += new System.Windows.Forms.MouseEventHandler(this.titleLabel_MouseUp);
            // 
            // dialogIcon
            // 
            this.dialogIcon.BackColor = System.Drawing.Color.Transparent;
            this.dialogIcon.Image = global::Idera.SQLdm.DesktopClient.Properties.Resources.ToolbarHelp;
            this.dialogIcon.Location = new System.Drawing.Point(3, 3);
            this.dialogIcon.Name = "dialogIcon";
            this.dialogIcon.Size = new System.Drawing.Size(16, 16);
            this.dialogIcon.SizeMode = System.Windows.Forms.PictureBoxSizeMode.AutoSize;
            this.dialogIcon.TabIndex = 0;
            this.dialogIcon.TabStop = false;
            this.dialogIcon.MouseDown += new System.Windows.Forms.MouseEventHandler(this.dialogIcon_MouseDown);
            this.dialogIcon.MouseMove += new System.Windows.Forms.MouseEventHandler(this.dialogIcon_MouseMove);
            this.dialogIcon.MouseUp += new System.Windows.Forms.MouseEventHandler(this.dialogIcon_MouseUp);
            // 
            // HelpPopupDialog
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(101)))), ((int)(((byte)(136)))), ((int)(((byte)(186)))));
            this.ClientSize = new System.Drawing.Size(280, 325);
            this.Controls.Add(this.panel1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Name = "HelpPopupDialog";
            this.Padding = new System.Windows.Forms.Padding(1);
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.Manual;
            this.Text = "HelpPopupDialog";
            this.panel1.ResumeLayout(false);
            this.footerPanel.ResumeLayout(false);
            this.footerPanel.PerformLayout();
            this.headerPanel.ResumeLayout(false);
            this.headerPanel.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.closeButton)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dialogIcon)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomPanel  panel1;
        private Idera.SQLdm.DesktopClient.Controls.GradientPanel headerPanel;
        private System.Windows.Forms.PictureBox dialogIcon;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomLabel titleLabel;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomLabel titleDividerLabel;
        private System.Windows.Forms.PictureBox closeButton;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomLinkLabel showDetailsLinkLabel;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomLinkLabel configureAlertsLinkLabel;
        private System.Windows.Forms.WebBrowser webBrowser;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomLinkLabel showHelpContentsLinkLabel;
        private Idera.SQLdm.DesktopClient.Controls.GradientPanel footerPanel;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomLabel footerDividerLabel;
    }
}