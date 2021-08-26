namespace Idera.SQLdm.DesktopClient.Controls
{
    partial class Office2007PropertyPage
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
            this.backgroundPanel = new Idera.SQLdm.DesktopClient.Controls.GradientPanel();
            this.contentPanel = new Idera.SQLdm.DesktopClient.Controls.GradientPanel();
            this.headerPanel = new Idera.SQLdm.DesktopClient.Controls.GradientPanel();
            this.headerImagePanel = new System.Windows.Forms.Panel();
            this.descriptionLabel = new System.Windows.Forms.Label();
            this._picBoxImageHeader = new System.Windows.Forms.PictureBox();
            this.backgroundPanel.SuspendLayout();
            this.headerPanel.SuspendLayout();
            this.headerImagePanel.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this._picBoxImageHeader)).BeginInit();
            this.SuspendLayout();
            // 
            // backgroundPanel
            // 
            this.backgroundPanel.BackColor2 = System.Drawing.Color.White;
            this.backgroundPanel.BorderColor = System.Drawing.SystemColors.ControlDark;
            this.backgroundPanel.Controls.Add(this.contentPanel);
            this.backgroundPanel.Controls.Add(this.headerPanel);
            this.backgroundPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.backgroundPanel.FillStyle = Idera.SQLdm.DesktopClient.Controls.GradientPanelFillStyle.Solid;
            this.backgroundPanel.Location = new System.Drawing.Point(0, 0);
            this.backgroundPanel.Margin = new System.Windows.Forms.Padding(4);
            this.backgroundPanel.Name = "backgroundPanel";
            this.backgroundPanel.Padding = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.backgroundPanel.Size = new System.Drawing.Size(659, 528);
            this.backgroundPanel.TabIndex = 0;
            // 
            // contentPanel
            // 
            this.contentPanel.BackColor2 = System.Drawing.Color.White;
            this.contentPanel.BorderColor = System.Drawing.SystemColors.ControlDark;
            this.contentPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.contentPanel.FillStyle = Idera.SQLdm.DesktopClient.Controls.GradientPanelFillStyle.Solid;
            this.contentPanel.Location = new System.Drawing.Point(3, 70);
            this.contentPanel.Margin = new System.Windows.Forms.Padding(4);
            this.contentPanel.Name = "contentPanel";
            this.contentPanel.Padding = new System.Windows.Forms.Padding(1);
            this.contentPanel.ShowBorder = false;
            this.contentPanel.Size = new System.Drawing.Size(653, 456);
            this.contentPanel.TabIndex = 1;
            // 
            // headerPanel
            // 
            this.headerPanel.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(210)))), ((int)(((byte)(210)))), ((int)(((byte)(210)))));
            this.headerPanel.BackColor2 = System.Drawing.Color.White;
            this.headerPanel.BorderColor = System.Drawing.SystemColors.ControlDark;
            this.headerPanel.Controls.Add(this.headerImagePanel);
            this.headerPanel.Controls.Add(this.descriptionLabel);
            this.headerPanel.Dock = System.Windows.Forms.DockStyle.Top;
            this.headerPanel.GradientAngle = 90;
            this.headerPanel.Location = new System.Drawing.Point(3, 2);
            this.headerPanel.Margin = new System.Windows.Forms.Padding(4);
            this.headerPanel.Name = "headerPanel";
            this.headerPanel.Padding = new System.Windows.Forms.Padding(1);
            this.headerPanel.ShowBorder = false;
            this.headerPanel.Size = new System.Drawing.Size(653, 68);
            this.headerPanel.TabIndex = 0;
            // 
            // headerImagePanel
            // 
            this.headerImagePanel.BackColor = System.Drawing.Color.Transparent;
            this.headerImagePanel.Controls.Add(this._picBoxImageHeader);
            this.headerImagePanel.Location = new System.Drawing.Point(17, 14);
            this.headerImagePanel.Margin = new System.Windows.Forms.Padding(4);
            this.headerImagePanel.Name = "headerImagePanel";
            this.headerImagePanel.Size = new System.Drawing.Size(43, 39);
            this.headerImagePanel.TabIndex = 0;
            // 
            // descriptionLabel
            // 
            this.descriptionLabel.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.descriptionLabel.BackColor = System.Drawing.Color.Transparent;
            this.descriptionLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.descriptionLabel.ForeColor = System.Drawing.SystemColors.WindowText;
            this.descriptionLabel.Location = new System.Drawing.Point(68, 18);
            this.descriptionLabel.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.descriptionLabel.Name = "descriptionLabel";
            this.descriptionLabel.Size = new System.Drawing.Size(581, 44);
            this.descriptionLabel.TabIndex = 0;
            this.descriptionLabel.Text = "Property page description.";
            // 
            // _picBoxImageHeader
            // 
            this._picBoxImageHeader.Dock = System.Windows.Forms.DockStyle.Fill;
            this._picBoxImageHeader.Location = new System.Drawing.Point(0, 0);
            this._picBoxImageHeader.Name = "_picBoxImageHeader";
            this._picBoxImageHeader.Size = new System.Drawing.Size(43, 39);
            this._picBoxImageHeader.TabIndex = 0;
            this._picBoxImageHeader.TabStop = false;
            // 
            // Office2007PropertyPage
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.White;
            this.Controls.Add(this.backgroundPanel);
            this.Margin = new System.Windows.Forms.Padding(4);
            this.Name = "Office2007PropertyPage";
            this.Size = new System.Drawing.Size(659, 528);
            this.backgroundPanel.ResumeLayout(false);
            this.headerPanel.ResumeLayout(false);
            this.headerImagePanel.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this._picBoxImageHeader)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private Idera.SQLdm.DesktopClient.Controls.GradientPanel backgroundPanel;
        private Idera.SQLdm.DesktopClient.Controls.GradientPanel headerPanel;
        private Idera.SQLdm.DesktopClient.Controls.GradientPanel contentPanel;
        private System.Windows.Forms.Label descriptionLabel;
        private System.Windows.Forms.Panel headerImagePanel;
        private System.Windows.Forms.PictureBox _picBoxImageHeader;
    }
}
