namespace Idera.SQLdm.DesktopClient.Views.Administration
{
    partial class AdministrationView
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(AdministrationView));
            this.headerStrip = new Idera.SQLdm.DesktopClient.Controls.HeaderStrip();
            this.titleLabel = new System.Windows.Forms.ToolStripLabel();
            this.contentPanel = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomPanel ();
            this.adminPanel = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomPanel ();
            this.featureButtonAuditedActions = new Idera.SQLdm.DesktopClient.Controls.FeatureButton();
            this.featureButtonCustomCounter = new Idera.SQLdm.DesktopClient.Controls.FeatureButton();
            this.featureButtonImportExport = new Idera.SQLdm.DesktopClient.Controls.FeatureButton();
            this.dividerProgressBar2 = new Idera.SQLdm.DesktopClient.Controls.InfiniteProgressBar();
            this.mainDdescriptionLabel = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomLabel();
            this.headerPanel = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomPanel ();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.headerLabel = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomLabel();
            this.featureButtonAppSecurity = new Idera.SQLdm.DesktopClient.Controls.FeatureButton();
            this.headerStrip.SuspendLayout();
            this.adminPanel.SuspendLayout();
            this.headerPanel.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            this.SuspendLayout();
            // 
            // headerStrip
            // 
            this.headerStrip.AutoSize = false;
            this.headerStrip.Font = new System.Drawing.Font("Arial", 12F, System.Drawing.FontStyle.Bold);
            this.headerStrip.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(21)))), ((int)(((byte)(66)))), ((int)(((byte)(139)))));
            this.headerStrip.GripStyle = System.Windows.Forms.ToolStripGripStyle.Hidden;
            this.headerStrip.HeaderImage = global::Idera.SQLdm.DesktopClient.Properties.Resources.AppSecurity16x16;
            this.headerStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.titleLabel});
            this.headerStrip.Location = new System.Drawing.Point(0, 0);
            this.headerStrip.Name = "headerStrip";
            this.headerStrip.Padding = new System.Windows.Forms.Padding(20, 2, 0, 0);
            this.headerStrip.Size = new System.Drawing.Size(670, 25);
            this.headerStrip.TabIndex = 0;
            // 
            // titleLabel
            // 
            this.titleLabel.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(75)))), ((int)(((byte)(75)))), ((int)(((byte)(75)))));
            this.titleLabel.Name = "titleLabel";
            this.titleLabel.Size = new System.Drawing.Size(120, 20);
            this.titleLabel.Text = "Administration";
            // 
            // contentPanel
            // 
            this.contentPanel.AutoScroll = true;
            this.contentPanel.BackColor = System.Drawing.SystemColors.Control;
            this.contentPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.contentPanel.Location = new System.Drawing.Point(0, 25);
            this.contentPanel.Name = "contentPanel";
            this.contentPanel.Size = new System.Drawing.Size(670, 438);
            this.contentPanel.TabIndex = 1;
            // 
            // adminPanel
            // 
            this.adminPanel.AutoScroll = true;
            this.adminPanel.BackColor = System.Drawing.SystemColors.ControlLightLight;
            this.adminPanel.Controls.Add(this.featureButtonAuditedActions);
            this.adminPanel.Controls.Add(this.featureButtonCustomCounter);
            this.adminPanel.Controls.Add(this.featureButtonImportExport);
            this.adminPanel.Controls.Add(this.dividerProgressBar2);
            this.adminPanel.Controls.Add(this.mainDdescriptionLabel);
            this.adminPanel.Controls.Add(this.headerPanel);
            this.adminPanel.Controls.Add(this.featureButtonAppSecurity);
            this.adminPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.adminPanel.Location = new System.Drawing.Point(0, 25);
            this.adminPanel.Name = "adminPanel";
            this.adminPanel.Size = new System.Drawing.Size(670, 438);
            this.adminPanel.TabIndex = 2;
            // 
            // featureButtonAuditedActions
            // 
            this.featureButtonAuditedActions.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.featureButtonAuditedActions.DescriptionFont = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.featureButtonAuditedActions.DescriptionText = resources.GetString("featureButtonAuditedActions.DescriptionText");
            this.featureButtonAuditedActions.HeaderColor = System.Drawing.Color.Red;
            this.featureButtonAuditedActions.HeaderFont = new System.Drawing.Font("Arial", 11F, System.Drawing.FontStyle.Bold);
            this.featureButtonAuditedActions.HeaderText = "Change Log";
            this.featureButtonAuditedActions.Image = global::Idera.SQLdm.DesktopClient.Properties.Resources.ChangeLog32x32;
            this.featureButtonAuditedActions.Location = new System.Drawing.Point(13, 306);
            this.featureButtonAuditedActions.MinimumSize = new System.Drawing.Size(0, 40);
            this.featureButtonAuditedActions.Name = "featureButtonAuditedActions";
            this.featureButtonAuditedActions.Size = new System.Drawing.Size(644, 76);
            this.featureButtonAuditedActions.TabIndex = 9;
            this.featureButtonAuditedActions.MouseClick += new System.Windows.Forms.MouseEventHandler(this.featureButtonAuditedActions_MouseClick);
            // 
            // featureButtonCustomCounter
            // 
            this.featureButtonCustomCounter.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.featureButtonCustomCounter.DescriptionFont = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.featureButtonCustomCounter.DescriptionText = resources.GetString("featureButtonCustomCounter.DescriptionText");
            this.featureButtonCustomCounter.HeaderColor = System.Drawing.Color.Red;
            this.featureButtonCustomCounter.HeaderFont = new System.Drawing.Font("Arial", 11F, System.Drawing.FontStyle.Bold);
            this.featureButtonCustomCounter.HeaderText = "Custom Counters";
            this.featureButtonCustomCounter.Image = global::Idera.SQLdm.DesktopClient.Properties.Resources.CustomCounter32x32;
            this.featureButtonCustomCounter.Location = new System.Drawing.Point(13, 225);
            this.featureButtonCustomCounter.MinimumSize = new System.Drawing.Size(0, 40);
            this.featureButtonCustomCounter.Name = "featureButtonCustomCounter";
            this.featureButtonCustomCounter.Size = new System.Drawing.Size(644, 76);
            this.featureButtonCustomCounter.TabIndex = 8;
            this.featureButtonCustomCounter.MouseClick += new System.Windows.Forms.MouseEventHandler(this.featureButtonCustomCounter_MouseClick);

            //START SQLdm 10.0 (Swati Gogia):Import/Export wizard
            // 
            // featureButtonImportExport
            // 
            this.featureButtonImportExport.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.featureButtonImportExport.DescriptionFont = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.featureButtonImportExport.DescriptionText = resources.GetString("featureButtonImportExport.DescriptionText");
            this.featureButtonImportExport.HeaderColor = System.Drawing.Color.Red;
            this.featureButtonImportExport.HeaderFont = new System.Drawing.Font("Arial", 11F, System.Drawing.FontStyle.Bold);
            this.featureButtonImportExport.HeaderText = "Import/Export";
            this.featureButtonImportExport.Image = global::Idera.SQLdm.DesktopClient.Properties.Resources.ImportExport32x32;
           // this.featureButtonImportExport.Image = global::Idera.SQLdm.DesktopClient.Properties.Resources.ChangeLog32x32;
            this.featureButtonImportExport.Location = new System.Drawing.Point(13, 388);
            this.featureButtonImportExport.MinimumSize = new System.Drawing.Size(0, 40);
            this.featureButtonImportExport.Name = "featureButtonImportExport";
            this.featureButtonImportExport.Size = new System.Drawing.Size(644, 76);
            this.featureButtonImportExport.TabIndex = 10;
            this.featureButtonImportExport.MouseClick += new System.Windows.Forms.MouseEventHandler(this.featureButtonImportExport_MouseClick);

            // 
            // dividerProgressBar2
            // 
            this.dividerProgressBar2.Anchor = System.Windows.Forms.AnchorStyles.Top;
            this.dividerProgressBar2.Color1 = System.Drawing.Color.FromArgb(((int)(((byte)(149)))), ((int)(((byte)(201)))), ((int)(((byte)(67)))));
            this.dividerProgressBar2.Color2 = System.Drawing.Color.White;
            this.dividerProgressBar2.Location = new System.Drawing.Point(185, 125);
            this.dividerProgressBar2.Name = "dividerProgressBar2";
            this.dividerProgressBar2.Size = new System.Drawing.Size(300, 2);
            this.dividerProgressBar2.Speed = 15D;
            this.dividerProgressBar2.Step = 5F;
            this.dividerProgressBar2.TabIndex = 7;
            // 
            // mainDdescriptionLabel
            // 
            this.mainDdescriptionLabel.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.mainDdescriptionLabel.AutoEllipsis = true;
            this.mainDdescriptionLabel.BackColor = System.Drawing.Color.Transparent;
            this.mainDdescriptionLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.mainDdescriptionLabel.Location = new System.Drawing.Point(14, 55);
            this.mainDdescriptionLabel.Name = "mainDdescriptionLabel";
            this.mainDdescriptionLabel.Size = new System.Drawing.Size(643, 48);
            this.mainDdescriptionLabel.TabIndex = 6;
            this.mainDdescriptionLabel.Text = resources.GetString("mainDdescriptionLabel.Text");
            // 
            // headerPanel
            // 
            this.headerPanel.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(236)))), ((int)(((byte)(235)))), ((int)(((byte)(234)))));
            this.headerPanel.Controls.Add(this.pictureBox1);
            this.headerPanel.Controls.Add(this.headerLabel);
            this.headerPanel.Dock = System.Windows.Forms.DockStyle.Top;
            this.headerPanel.Location = new System.Drawing.Point(0, 0);
            this.headerPanel.Name = "headerPanel";
            this.headerPanel.Padding = new System.Windows.Forms.Padding(1);
            this.headerPanel.Size = new System.Drawing.Size(670, 45);
            this.headerPanel.TabIndex = 3;
            // 
            // pictureBox1
            // 
            this.pictureBox1.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.pictureBox1.Image = global::Idera.SQLdm.DesktopClient.Properties.Resources.TodayPageHeader;
            this.pictureBox1.Location = new System.Drawing.Point(0, 0);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(532, 45);
            this.pictureBox1.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.pictureBox1.TabIndex = 11;
            this.pictureBox1.TabStop = false;
            // 
            // headerLabel
            // 
            this.headerLabel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.headerLabel.BackColor = System.Drawing.Color.Transparent;
            this.headerLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 15.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.headerLabel.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(119)))), ((int)(((byte)(111)))), ((int)(((byte)(101)))));
            this.headerLabel.ImageAlign = System.Drawing.ContentAlignment.TopLeft;
            this.headerLabel.Location = new System.Drawing.Point(274, 0);
            this.headerLabel.Name = "headerLabel";
            this.headerLabel.Size = new System.Drawing.Size(375, 37);
            this.headerLabel.TabIndex = 0;
            this.headerLabel.Text = "Administer SQLdm";
            this.headerLabel.TextAlign = System.Drawing.ContentAlignment.BottomRight;
            // 
            // featureButtonAppSecurity
            // 
            this.featureButtonAppSecurity.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.featureButtonAppSecurity.DescriptionFont = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.featureButtonAppSecurity.DescriptionText = resources.GetString("featureButtonAppSecurity.DescriptionText");
            this.featureButtonAppSecurity.HeaderColor = System.Drawing.Color.Red;
            this.featureButtonAppSecurity.HeaderFont = new System.Drawing.Font("Arial", 11F, System.Drawing.FontStyle.Bold);
            this.featureButtonAppSecurity.HeaderText = "Application Security";
            this.featureButtonAppSecurity.Image = global::Idera.SQLdm.DesktopClient.Properties.Resources.AppSecurity32x32;
            this.featureButtonAppSecurity.Location = new System.Drawing.Point(13, 143);
            this.featureButtonAppSecurity.MinimumSize = new System.Drawing.Size(0, 40);
            this.featureButtonAppSecurity.Name = "featureButtonAppSecurity";
            this.featureButtonAppSecurity.Size = new System.Drawing.Size(644, 76);
            this.featureButtonAppSecurity.TabIndex = 0;
            this.featureButtonAppSecurity.MouseClick += new System.Windows.Forms.MouseEventHandler(this.featureButtonAppSecurity_MouseClick);
            // 
            // AdministrationView
            // 
            this.Controls.Add(this.adminPanel);
            this.Controls.Add(this.contentPanel);
            this.Controls.Add(this.headerStrip);
            this.Name = "AdministrationView";
            this.Size = new System.Drawing.Size(670, 463);
            this.headerStrip.ResumeLayout(false);
            this.headerStrip.PerformLayout();
            this.adminPanel.ResumeLayout(false);
            this.headerPanel.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private Idera.SQLdm.DesktopClient.Controls.HeaderStrip headerStrip;
        private System.Windows.Forms.ToolStripLabel titleLabel;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomPanel  contentPanel;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomPanel  adminPanel;
        private Idera.SQLdm.DesktopClient.Controls.FeatureButton featureButtonAppSecurity;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomPanel  headerPanel;
        private System.Windows.Forms.PictureBox pictureBox1;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomLabel headerLabel;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomLabel mainDdescriptionLabel;
        private Idera.SQLdm.DesktopClient.Controls.FeatureButton featureButtonCustomCounter;
        private Idera.SQLdm.DesktopClient.Controls.FeatureButton featureButtonImportExport;
        private Idera.SQLdm.DesktopClient.Controls.InfiniteProgressBar dividerProgressBar2;
        private Controls.FeatureButton featureButtonAuditedActions;
    }
}
