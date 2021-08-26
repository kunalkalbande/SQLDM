namespace Idera.SQLdm.DesktopClient.Controls.ServerSummary.Dashboard
{
    partial class DashboardPanelDetails
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
            this.mainGradientPanel = new Idera.SQLdm.DesktopClient.Controls.GradientPanel();
            this.mainTableLayoutPanel = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomTableLayoutPanel();
            this.panelNameLabel = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomLabel();
            this.helpLinkLabel = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomLinkLabel();
            this.descriptionLabel = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomLabel();
            this.mainGradientPanel.SuspendLayout();
            this.mainTableLayoutPanel.SuspendLayout();
            this.SuspendLayout();
            // 
            // mainGradientPanel
            // 
            this.mainGradientPanel.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(189)))), ((int)(((byte)(105)))));
            this.mainGradientPanel.BackColor2 = System.Drawing.Color.White;
            this.mainGradientPanel.BorderColor = System.Drawing.SystemColors.ControlDark;
            this.mainGradientPanel.Controls.Add(this.mainTableLayoutPanel);
            this.mainGradientPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.mainGradientPanel.Location = new System.Drawing.Point(0, 0);
            this.mainGradientPanel.Name = "mainGradientPanel";
            this.mainGradientPanel.Padding = new System.Windows.Forms.Padding(1);
            this.mainGradientPanel.Size = new System.Drawing.Size(520, 264);
            this.mainGradientPanel.TabIndex = 3;
            this.mainGradientPanel.MouseClick += new System.Windows.Forms.MouseEventHandler(this.Control_MouseClick);
            // 
            // mainTableLayoutPanel
            // 
            this.mainTableLayoutPanel.AutoSize = true;
            this.mainTableLayoutPanel.BackColor = System.Drawing.Color.Transparent;
            this.mainTableLayoutPanel.ColumnCount = 2;
            this.mainTableLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.mainTableLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 72F));
            this.mainTableLayoutPanel.Controls.Add(this.panelNameLabel, 0, 0);
            this.mainTableLayoutPanel.Controls.Add(this.helpLinkLabel, 1, 0);
            this.mainTableLayoutPanel.Controls.Add(this.descriptionLabel, 0, 1);
            this.mainTableLayoutPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.mainTableLayoutPanel.Location = new System.Drawing.Point(1, 1);
            this.mainTableLayoutPanel.Name = "mainTableLayoutPanel";
            this.mainTableLayoutPanel.RowCount = 4;
            this.mainTableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 24F));
            this.mainTableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.mainTableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 10F));
            this.mainTableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.mainTableLayoutPanel.Size = new System.Drawing.Size(518, 262);
            this.mainTableLayoutPanel.TabIndex = 8;
            this.mainTableLayoutPanel.MouseClick += new System.Windows.Forms.MouseEventHandler(this.Control_MouseClick);
            // 
            // panelNameLabel
            // 
            this.panelNameLabel.BackColor = System.Drawing.Color.Transparent;
            this.panelNameLabel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panelNameLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.panelNameLabel.Location = new System.Drawing.Point(1, 0);
            this.panelNameLabel.Margin = new System.Windows.Forms.Padding(1, 0, 3, 0);
            this.panelNameLabel.Name = "panelNameLabel";
            this.panelNameLabel.Size = new System.Drawing.Size(442, 24);
            this.panelNameLabel.TabIndex = 4;
            this.panelNameLabel.Text = "Panel Name";
            this.panelNameLabel.MouseClick += new System.Windows.Forms.MouseEventHandler(this.Control_MouseClick);
            // 
            // helpLinkLabel
            // 
            this.helpLinkLabel.AutoSize = true;
            this.helpLinkLabel.BackColor = System.Drawing.Color.Transparent;
            this.helpLinkLabel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.helpLinkLabel.Location = new System.Drawing.Point(449, 0);
            this.helpLinkLabel.Name = "helpLinkLabel";
            this.helpLinkLabel.Size = new System.Drawing.Size(66, 24);
            this.helpLinkLabel.TabIndex = 6;
            this.helpLinkLabel.TabStop = true;
            this.helpLinkLabel.Text = "More info ...";
            this.helpLinkLabel.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.helpLinkLabel.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.helpLinkLabel_LinkClicked);
            // 
            // descriptionLabel
            // 
            this.descriptionLabel.AutoEllipsis = true;
            this.descriptionLabel.AutoSize = true;
            this.descriptionLabel.BackColor = System.Drawing.Color.Transparent;
            this.mainTableLayoutPanel.SetColumnSpan(this.descriptionLabel, 2);
            this.descriptionLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.descriptionLabel.Location = new System.Drawing.Point(3, 24);
            this.descriptionLabel.Margin = new System.Windows.Forms.Padding(3, 0, 6, 0);
            this.descriptionLabel.Name = "descriptionLabel";
            this.descriptionLabel.Size = new System.Drawing.Size(90, 13);
            this.descriptionLabel.TabIndex = 5;
            this.descriptionLabel.Text = "Panel Description";
            this.descriptionLabel.MouseClick += new System.Windows.Forms.MouseEventHandler(this.Control_MouseClick);
            // 
            // DashboardPanelDetails
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoScroll = true;
            this.AutoSize = true;
            this.BackColor = System.Drawing.Color.White;
            this.Controls.Add(this.mainGradientPanel);
            this.Name = "DashboardPanelDetails";
            this.Size = new System.Drawing.Size(520, 264);
            this.MouseClick += new System.Windows.Forms.MouseEventHandler(this.Control_MouseClick);
            this.MouseLeave += new System.EventHandler(this.DashboardPanelDetails_MouseLeave);
            this.mainGradientPanel.ResumeLayout(false);
            this.mainGradientPanel.PerformLayout();
            this.mainTableLayoutPanel.ResumeLayout(false);
            this.mainTableLayoutPanel.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private Controls.GradientPanel mainGradientPanel;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomLabel panelNameLabel;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomLinkLabel helpLinkLabel;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomLabel descriptionLabel;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomTableLayoutPanel mainTableLayoutPanel;

    }
}