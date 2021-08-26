namespace Idera.SQLdm.DesktopClient.Dialogs.Notification
{
    partial class NotificationPopupWindow
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
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(NotificationPopupWindow));
            this.panel1 = new System.Windows.Forms.Panel();
            this.panel2 = new System.Windows.Forms.Panel();
            this.renderTimer = new System.Windows.Forms.Timer(this.components);
            this.contentPanel = new Idera.SQLdm.DesktopClient.Controls.GradientPanel();
            this.linksLayoutPanel = new System.Windows.Forms.TableLayoutPanel();
            this.linkLabel5 = new System.Windows.Forms.LinkLabel();
            this.linkLabel4 = new System.Windows.Forms.LinkLabel();
            this.linkLabel3 = new System.Windows.Forms.LinkLabel();
            this.linkLabel2 = new System.Windows.Forms.LinkLabel();
            this.linkLabel1 = new System.Windows.Forms.LinkLabel();
            this.showActiveAlertsLinkLabel = new System.Windows.Forms.LinkLabel();
            this.messageLabel = new System.Windows.Forms.Label();
            this.headerPanel = new Idera.SQLdm.DesktopClient.Controls.GradientPanel();
            this.closeButton = new System.Windows.Forms.PictureBox();
            this.titleDividerLabel = new System.Windows.Forms.Label();
            this.titleLabel = new System.Windows.Forms.Label();
            this.dialogIcon = new System.Windows.Forms.PictureBox();
            this.panel1.SuspendLayout();
            this.panel2.SuspendLayout();
            this.contentPanel.SuspendLayout();
            this.linksLayoutPanel.SuspendLayout();
            this.headerPanel.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.closeButton)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dialogIcon)).BeginInit();
            this.SuspendLayout();
            // 
            // panel1
            // 
            this.panel1.BackColor = System.Drawing.Color.White;
            this.panel1.Controls.Add(this.panel2);
            this.panel1.Controls.Add(this.headerPanel);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel1.Location = new System.Drawing.Point(1, 1);
            this.panel1.Name = "panel1";
            this.panel1.Padding = new System.Windows.Forms.Padding(1);
            this.panel1.Size = new System.Drawing.Size(421, 153);
            this.panel1.TabIndex = 1;
            // 
            // panel2
            // 
            this.panel2.Controls.Add(this.contentPanel);
            this.panel2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel2.Location = new System.Drawing.Point(1, 24);
            this.panel2.Name = "panel2";
            this.panel2.Padding = new System.Windows.Forms.Padding(0, 1, 0, 0);
            this.panel2.Size = new System.Drawing.Size(419, 128);
            this.panel2.TabIndex = 1;
            // 
            // renderTimer
            // 
            this.renderTimer.Tick += new System.EventHandler(this.renderTimer_Tick);
            // 
            // contentPanel
            // 
            this.contentPanel.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(248)))), ((int)(((byte)(245)))), ((int)(((byte)(250)))));
            this.contentPanel.BackColor2 = System.Drawing.Color.FromArgb(((int)(((byte)(236)))), ((int)(((byte)(243)))), ((int)(((byte)(246)))));
            this.contentPanel.BorderColor = System.Drawing.SystemColors.ControlDark;
            this.contentPanel.BorderWidth = 0;
            this.contentPanel.Controls.Add(this.linksLayoutPanel);
            this.contentPanel.Controls.Add(this.showActiveAlertsLinkLabel);
            this.contentPanel.Controls.Add(this.messageLabel);
            this.contentPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.contentPanel.GradientAngle = 90;
            this.contentPanel.Location = new System.Drawing.Point(0, 1);
            this.contentPanel.Name = "contentPanel";
            this.contentPanel.ShowBorder = false;
            this.contentPanel.Size = new System.Drawing.Size(419, 127);
            this.contentPanel.TabIndex = 0;
            this.contentPanel.MouseLeave += new System.EventHandler(this.contentPanel_MouseLeave);
            this.contentPanel.MouseEnter += new System.EventHandler(this.contentPanel_MouseEnter);
            // 
            // linksLayoutPanel
            // 
            this.linksLayoutPanel.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.linksLayoutPanel.BackColor = System.Drawing.Color.Transparent;
            this.linksLayoutPanel.ColumnCount = 1;
            this.linksLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.linksLayoutPanel.Controls.Add(this.linkLabel5, 0, 4);
            this.linksLayoutPanel.Controls.Add(this.linkLabel4, 0, 3);
            this.linksLayoutPanel.Controls.Add(this.linkLabel3, 0, 2);
            this.linksLayoutPanel.Controls.Add(this.linkLabel2, 0, 1);
            this.linksLayoutPanel.Controls.Add(this.linkLabel1, 0, 0);
            this.linksLayoutPanel.Location = new System.Drawing.Point(10, 7);
            this.linksLayoutPanel.Name = "linksLayoutPanel";
            this.linksLayoutPanel.RowCount = 5;
            this.linksLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 20F));
            this.linksLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 20F));
            this.linksLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 20F));
            this.linksLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 20F));
            this.linksLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 20F));
            this.linksLayoutPanel.Size = new System.Drawing.Size(399, 95);
            this.linksLayoutPanel.TabIndex = 4;
            // 
            // linkLabel5
            // 
            this.linkLabel5.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.linkLabel5.AutoEllipsis = true;
            this.linkLabel5.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.linkLabel5.LinkBehavior = System.Windows.Forms.LinkBehavior.HoverUnderline;
            this.linkLabel5.LinkColor = System.Drawing.Color.Black;
            this.linkLabel5.Location = new System.Drawing.Point(3, 76);
            this.linkLabel5.Name = "linkLabel5";
            this.linkLabel5.Padding = new System.Windows.Forms.Padding(20, 0, 0, 0);
            this.linkLabel5.Size = new System.Drawing.Size(393, 19);
            this.linkLabel5.TabIndex = 4;
            this.linkLabel5.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.linkLabel5.VisitedLinkColor = System.Drawing.Color.Blue;
            this.linkLabel5.MouseLeave += new System.EventHandler(this.OnLinkMouseLeave);
            this.linkLabel5.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.OnLinkClicked);
            this.linkLabel5.MouseEnter += new System.EventHandler(this.OnLinkMouseEnter);
            // 
            // linkLabel4
            // 
            this.linkLabel4.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.linkLabel4.AutoEllipsis = true;
            this.linkLabel4.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.linkLabel4.LinkBehavior = System.Windows.Forms.LinkBehavior.HoverUnderline;
            this.linkLabel4.LinkColor = System.Drawing.Color.Black;
            this.linkLabel4.Location = new System.Drawing.Point(3, 57);
            this.linkLabel4.Name = "linkLabel4";
            this.linkLabel4.Padding = new System.Windows.Forms.Padding(20, 0, 0, 0);
            this.linkLabel4.Size = new System.Drawing.Size(393, 19);
            this.linkLabel4.TabIndex = 3;
            this.linkLabel4.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.linkLabel4.VisitedLinkColor = System.Drawing.Color.Blue;
            this.linkLabel4.MouseLeave += new System.EventHandler(this.OnLinkMouseLeave);
            this.linkLabel4.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.OnLinkClicked);
            this.linkLabel4.MouseEnter += new System.EventHandler(this.OnLinkMouseEnter);
            // 
            // linkLabel3
            // 
            this.linkLabel3.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.linkLabel3.AutoEllipsis = true;
            this.linkLabel3.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.linkLabel3.LinkBehavior = System.Windows.Forms.LinkBehavior.HoverUnderline;
            this.linkLabel3.LinkColor = System.Drawing.Color.Black;
            this.linkLabel3.Location = new System.Drawing.Point(3, 38);
            this.linkLabel3.Name = "linkLabel3";
            this.linkLabel3.Padding = new System.Windows.Forms.Padding(20, 0, 0, 0);
            this.linkLabel3.Size = new System.Drawing.Size(393, 19);
            this.linkLabel3.TabIndex = 2;
            this.linkLabel3.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.linkLabel3.VisitedLinkColor = System.Drawing.Color.Blue;
            this.linkLabel3.MouseLeave += new System.EventHandler(this.OnLinkMouseLeave);
            this.linkLabel3.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.OnLinkClicked);
            this.linkLabel3.MouseEnter += new System.EventHandler(this.OnLinkMouseEnter);
            // 
            // linkLabel2
            // 
            this.linkLabel2.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.linkLabel2.AutoEllipsis = true;
            this.linkLabel2.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.linkLabel2.LinkBehavior = System.Windows.Forms.LinkBehavior.HoverUnderline;
            this.linkLabel2.LinkColor = System.Drawing.Color.Black;
            this.linkLabel2.Location = new System.Drawing.Point(3, 19);
            this.linkLabel2.Name = "linkLabel2";
            this.linkLabel2.Padding = new System.Windows.Forms.Padding(20, 0, 0, 0);
            this.linkLabel2.Size = new System.Drawing.Size(393, 19);
            this.linkLabel2.TabIndex = 1;
            this.linkLabel2.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.linkLabel2.VisitedLinkColor = System.Drawing.Color.Blue;
            this.linkLabel2.MouseLeave += new System.EventHandler(this.OnLinkMouseLeave);
            this.linkLabel2.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.OnLinkClicked);
            this.linkLabel2.MouseEnter += new System.EventHandler(this.OnLinkMouseEnter);
            // 
            // linkLabel1
            // 
            this.linkLabel1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.linkLabel1.AutoEllipsis = true;
            this.linkLabel1.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.linkLabel1.LinkBehavior = System.Windows.Forms.LinkBehavior.HoverUnderline;
            this.linkLabel1.LinkColor = System.Drawing.Color.Black;
            this.linkLabel1.Location = new System.Drawing.Point(3, 0);
            this.linkLabel1.Name = "linkLabel1";
            this.linkLabel1.Padding = new System.Windows.Forms.Padding(20, 0, 0, 0);
            this.linkLabel1.Size = new System.Drawing.Size(393, 19);
            this.linkLabel1.TabIndex = 0;
            this.linkLabel1.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.linkLabel1.VisitedLinkColor = System.Drawing.Color.Blue;
            this.linkLabel1.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.OnLinkClicked);
            this.linkLabel1.MouseEnter += new System.EventHandler(this.OnLinkMouseEnter);
            this.linkLabel1.MouseLeave += new System.EventHandler(this.OnLinkMouseLeave);
            // 
            // showActiveAlertsLinkLabel
            // 
            this.showActiveAlertsLinkLabel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.showActiveAlertsLinkLabel.AutoSize = true;
            this.showActiveAlertsLinkLabel.BackColor = System.Drawing.Color.Transparent;
            this.showActiveAlertsLinkLabel.LinkBehavior = System.Windows.Forms.LinkBehavior.HoverUnderline;
            this.showActiveAlertsLinkLabel.LinkColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(136)))), ((int)(((byte)(228)))));
            this.showActiveAlertsLinkLabel.Location = new System.Drawing.Point(317, 107);
            this.showActiveAlertsLinkLabel.Name = "showActiveAlertsLinkLabel";
            this.showActiveAlertsLinkLabel.Size = new System.Drawing.Size(96, 13);
            this.showActiveAlertsLinkLabel.TabIndex = 2;
            this.showActiveAlertsLinkLabel.TabStop = true;
            this.showActiveAlertsLinkLabel.Text = "Show Active Alerts";
            this.showActiveAlertsLinkLabel.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.showActiveAlertsLinkLabel_LinkClicked);
            this.showActiveAlertsLinkLabel.MouseEnter += new System.EventHandler(this.OnLinkMouseEnter);
            this.showActiveAlertsLinkLabel.MouseLeave += new System.EventHandler(this.OnLinkMouseLeave);
            // 
            // messageLabel
            // 
            this.messageLabel.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.messageLabel.BackColor = System.Drawing.Color.Transparent;
            this.messageLabel.Cursor = System.Windows.Forms.Cursors.Hand;
            this.messageLabel.Location = new System.Drawing.Point(10, 7);
            this.messageLabel.Name = "messageLabel";
            this.messageLabel.Size = new System.Drawing.Size(399, 95);
            this.messageLabel.TabIndex = 5;
            this.messageLabel.Text = "Message";
            this.messageLabel.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.messageLabel.MouseLeave += new System.EventHandler(this.OnLinkMouseLeave);
            this.messageLabel.MouseClick += new System.Windows.Forms.MouseEventHandler(this.messageLabel_MouseClick);
            this.messageLabel.MouseEnter += new System.EventHandler(this.OnLinkMouseEnter);
            // 
            // headerPanel
            // 
            this.headerPanel.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(224)))), ((int)(((byte)(224)))), ((int)(((byte)(224)))));
            this.headerPanel.BackColor2 = System.Drawing.Color.FromArgb(((int)(((byte)(236)))), ((int)(((byte)(236)))), ((int)(((byte)(236)))));
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
            this.headerPanel.Size = new System.Drawing.Size(419, 23);
            this.headerPanel.TabIndex = 0;
            // 
            // closeButton
            // 
            this.closeButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.closeButton.BackColor = System.Drawing.Color.Transparent;
            this.closeButton.Image = ((System.Drawing.Image)(resources.GetObject("closeButton.Image")));
            this.closeButton.Location = new System.Drawing.Point(400, 3);
            this.closeButton.Name = "closeButton";
            this.closeButton.Size = new System.Drawing.Size(16, 16);
            this.closeButton.SizeMode = System.Windows.Forms.PictureBoxSizeMode.CenterImage;
            this.closeButton.TabIndex = 3;
            this.closeButton.TabStop = false;
            this.closeButton.MouseLeave += new System.EventHandler(this.closeButton_MouseLeave);
            this.closeButton.MouseClick += new System.Windows.Forms.MouseEventHandler(this.closeButton_MouseClick);
            this.closeButton.MouseEnter += new System.EventHandler(this.closeButton_MouseEnter);
            // 
            // titleDividerLabel
            // 
            this.titleDividerLabel.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(190)))), ((int)(((byte)(214)))), ((int)(((byte)(224)))));
            this.titleDividerLabel.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.titleDividerLabel.Location = new System.Drawing.Point(0, 22);
            this.titleDividerLabel.Name = "titleDividerLabel";
            this.titleDividerLabel.Size = new System.Drawing.Size(419, 1);
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
            this.titleLabel.Size = new System.Drawing.Size(374, 16);
            this.titleLabel.TabIndex = 1;
            this.titleLabel.Text = "SQL Diagnostic Manager";
            this.titleLabel.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // dialogIcon
            // 
            this.dialogIcon.BackColor = System.Drawing.Color.Transparent;
            this.dialogIcon.Image = global::Idera.SQLdm.DesktopClient.Properties.Resources.AppImage16x16;
            this.dialogIcon.Location = new System.Drawing.Point(3, 3);
            this.dialogIcon.Name = "dialogIcon";
            this.dialogIcon.Size = new System.Drawing.Size(16, 16);
            this.dialogIcon.SizeMode = System.Windows.Forms.PictureBoxSizeMode.AutoSize;
            this.dialogIcon.TabIndex = 0;
            this.dialogIcon.TabStop = false;
            // 
            // NotificationPopupWindow
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(101)))), ((int)(((byte)(136)))), ((int)(((byte)(186)))));
            this.ClientSize = new System.Drawing.Size(423, 155);
            this.Controls.Add(this.panel1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Name = "NotificationPopupWindow";
            this.Padding = new System.Windows.Forms.Padding(1);
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.Manual;
            this.Text = "NotificationPopupWindow";
            this.panel1.ResumeLayout(false);
            this.panel2.ResumeLayout(false);
            this.contentPanel.ResumeLayout(false);
            this.contentPanel.PerformLayout();
            this.linksLayoutPanel.ResumeLayout(false);
            this.headerPanel.ResumeLayout(false);
            this.headerPanel.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.closeButton)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dialogIcon)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel panel1;
        private Idera.SQLdm.DesktopClient.Controls.GradientPanel headerPanel;
        private System.Windows.Forms.PictureBox closeButton;
        private System.Windows.Forms.Label titleDividerLabel;
        private System.Windows.Forms.Label titleLabel;
        private System.Windows.Forms.PictureBox dialogIcon;
        private System.Windows.Forms.Panel panel2;
        private Idera.SQLdm.DesktopClient.Controls.GradientPanel contentPanel;
        private System.Windows.Forms.LinkLabel showActiveAlertsLinkLabel;
        private System.Windows.Forms.Timer renderTimer;
        private System.Windows.Forms.TableLayoutPanel linksLayoutPanel;
        private System.Windows.Forms.LinkLabel linkLabel5;
        private System.Windows.Forms.LinkLabel linkLabel4;
        private System.Windows.Forms.LinkLabel linkLabel3;
        private System.Windows.Forms.LinkLabel linkLabel2;
        private System.Windows.Forms.LinkLabel linkLabel1;
        private System.Windows.Forms.Label messageLabel;
    }
}
