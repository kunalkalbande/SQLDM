namespace Idera.SQLdm.DesktopClient.Dialogs
{
    partial class GettingStartedWizard
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(GettingStartedWizard));
            this.cancelButton = new System.Windows.Forms.Button();
            this.headerLabel = new System.Windows.Forms.Label();
            this.headerPanel = new System.Windows.Forms.Panel();
            this.dividerLabel1 = new System.Windows.Forms.Label();
            this.productDescriptionLabel = new System.Windows.Forms.Label();
            this.footerPanel = new System.Windows.Forms.Panel();
            this.descriptionLabel = new System.Windows.Forms.Label();
            this.visitTrialCenterFeatureButton = new Idera.SQLdm.DesktopClient.Controls.FeatureButton();
            this.configureAlertsFeatureButton = new Idera.SQLdm.DesktopClient.Controls.FeatureButton();
            this.addNewServersFeatureButton = new Idera.SQLdm.DesktopClient.Controls.FeatureButton();
            this.headerPanel.SuspendLayout();
            this.footerPanel.SuspendLayout();
            this.SuspendLayout();
            // 
            // cancelButton
            // 
            this.cancelButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.cancelButton.BackColor = System.Drawing.SystemColors.ButtonFace;
            this.cancelButton.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.cancelButton.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.cancelButton.Location = new System.Drawing.Point(501, 12);
            this.cancelButton.Name = "cancelButton";
            this.cancelButton.Size = new System.Drawing.Size(90, 23);
            this.cancelButton.TabIndex = 0;
            this.cancelButton.Text = "Start Console";
            this.cancelButton.UseVisualStyleBackColor = false;
            this.cancelButton.Click += new System.EventHandler(this.cancelButton_Click);
            // 
            // headerLabel
            // 
            this.headerLabel.AutoSize = true;
            this.headerLabel.BackColor = System.Drawing.Color.Transparent;
            this.headerLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.headerLabel.ForeColor = System.Drawing.SystemColors.ControlText;
            this.headerLabel.Location = new System.Drawing.Point(116, 14);
            this.headerLabel.Name = "headerLabel";
            this.headerLabel.Size = new System.Drawing.Size(370, 24);
            this.headerLabel.TabIndex = 0;
            this.headerLabel.Text = "Welcome to IDERA SQL diagnostic manager";
            // 
            // headerPanel
            // 
            this.headerPanel.BackgroundImage = global::Idera.SQLdm.DesktopClient.Properties.Resources.GettingStartedWizardHeader;
            this.headerPanel.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.headerPanel.Controls.Add(this.dividerLabel1);
            this.headerPanel.Controls.Add(this.productDescriptionLabel);
            this.headerPanel.Controls.Add(this.headerLabel);
            this.headerPanel.Dock = System.Windows.Forms.DockStyle.Top;
            this.headerPanel.Location = new System.Drawing.Point(0, 0);
            this.headerPanel.Name = "headerPanel";
            this.headerPanel.Size = new System.Drawing.Size(599, 119);
            this.headerPanel.TabIndex = 1;
            // 
            // dividerLabel1
            // 
            this.dividerLabel1.BackColor = System.Drawing.Color.Transparent;
            this.dividerLabel1.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.dividerLabel1.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.dividerLabel1.Location = new System.Drawing.Point(120, 45);
            this.dividerLabel1.Name = "dividerLabel1";
            this.dividerLabel1.Size = new System.Drawing.Size(440, 2);
            this.dividerLabel1.TabIndex = 9;
            // 
            // productDescriptionLabel
            // 
            this.productDescriptionLabel.BackColor = System.Drawing.Color.Transparent;
            this.productDescriptionLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.productDescriptionLabel.Location = new System.Drawing.Point(117, 54);
            this.productDescriptionLabel.Name = "productDescriptionLabel";
            this.productDescriptionLabel.Size = new System.Drawing.Size(470, 54);
            this.productDescriptionLabel.TabIndex = 1;
            this.productDescriptionLabel.Text = "SQL Diagnostic Manager minimizes costly server downtime by proactively alerting a" +
                "dministrators to problems and enabling rapid diagnosis and resolution.";
            // 
            // footerPanel
            // 
            this.footerPanel.BackgroundImage = global::Idera.SQLdm.DesktopClient.Properties.Resources.GettingStartedWizardFooter;
            this.footerPanel.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.footerPanel.Controls.Add(this.cancelButton);
            this.footerPanel.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.footerPanel.Location = new System.Drawing.Point(0, 392);
            this.footerPanel.Name = "footerPanel";
            this.footerPanel.Size = new System.Drawing.Size(599, 42);
            this.footerPanel.TabIndex = 0;
            // 
            // descriptionLabel
            // 
            this.descriptionLabel.Location = new System.Drawing.Point(12, 132);
            this.descriptionLabel.Name = "descriptionLabel";
            this.descriptionLabel.Size = new System.Drawing.Size(575, 31);
            this.descriptionLabel.TabIndex = 1;
            this.descriptionLabel.Text = "Choose an option to configure SQL Diagnostic Manager now. These options are also " +
                "available from the application menu if you would like to configure later.";
            // 
            // visitTrialCenterFeatureButton
            // 
            this.visitTrialCenterFeatureButton.DescriptionFont = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.visitTrialCenterFeatureButton.DescriptionText = resources.GetString("visitTrialCenterFeatureButton.DescriptionText");
            this.visitTrialCenterFeatureButton.HeaderColor = System.Drawing.Color.Black;
            this.visitTrialCenterFeatureButton.HeaderFont = new System.Drawing.Font("Arial", 11F, System.Drawing.FontStyle.Bold);
            this.visitTrialCenterFeatureButton.HeaderText = "Visit the IDERA Trial Center";
            this.visitTrialCenterFeatureButton.Image = global::Idera.SQLdm.DesktopClient.Properties.Resources.Download32x32;
            this.visitTrialCenterFeatureButton.Location = new System.Drawing.Point(35, 169);
            this.visitTrialCenterFeatureButton.MinimumSize = new System.Drawing.Size(0, 40);
            this.visitTrialCenterFeatureButton.Name = "visitTrialCenterFeatureButton";
            this.visitTrialCenterFeatureButton.Size = new System.Drawing.Size(533, 75);
            this.visitTrialCenterFeatureButton.TabIndex = 5;
            this.visitTrialCenterFeatureButton.MouseClick += new System.Windows.Forms.MouseEventHandler(this.visitTrialCenterFeatureButton_MouseClick);
            // 
            // configureAlertsFeatureButton
            // 
            this.configureAlertsFeatureButton.DescriptionFont = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.configureAlertsFeatureButton.DescriptionText = resources.GetString("configureAlertsFeatureButton.DescriptionText");
            this.configureAlertsFeatureButton.HeaderColor = System.Drawing.SystemColors.ControlText;
            this.configureAlertsFeatureButton.HeaderFont = new System.Drawing.Font("Arial", 11F, System.Drawing.FontStyle.Bold);
            this.configureAlertsFeatureButton.HeaderText = "Configure Default Alert Thresholds";
            this.configureAlertsFeatureButton.Image = global::Idera.SQLdm.DesktopClient.Properties.Resources.AlertsFeature1;
            this.configureAlertsFeatureButton.Location = new System.Drawing.Point(35, 304);
            this.configureAlertsFeatureButton.MinimumSize = new System.Drawing.Size(0, 40);
            this.configureAlertsFeatureButton.Name = "configureAlertsFeatureButton";
            this.configureAlertsFeatureButton.Size = new System.Drawing.Size(533, 76);
            this.configureAlertsFeatureButton.TabIndex = 3;
            this.configureAlertsFeatureButton.MouseClick += new System.Windows.Forms.MouseEventHandler(this.configureAlertsFeatureButton_MouseClick);
            // 
            // addNewServersFeatureButton
            // 
            this.addNewServersFeatureButton.DescriptionFont = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.addNewServersFeatureButton.DescriptionText = "Add new SQL Servers that should be monitored by SQL Diagnostic Manager. ";
            this.addNewServersFeatureButton.HeaderColor = System.Drawing.Color.Red;
            this.addNewServersFeatureButton.HeaderFont = new System.Drawing.Font("Arial", 11F, System.Drawing.FontStyle.Bold);
            this.addNewServersFeatureButton.HeaderText = "Add New Servers (Recommended)";
            this.addNewServersFeatureButton.Image = global::Idera.SQLdm.DesktopClient.Properties.Resources.AddServersFeature;
            this.addNewServersFeatureButton.Location = new System.Drawing.Point(35, 242);
            this.addNewServersFeatureButton.MinimumSize = new System.Drawing.Size(0, 40);
            this.addNewServersFeatureButton.Name = "addNewServersFeatureButton";
            this.addNewServersFeatureButton.Size = new System.Drawing.Size(533, 55);
            this.addNewServersFeatureButton.TabIndex = 4;
            this.addNewServersFeatureButton.MouseClick += new System.Windows.Forms.MouseEventHandler(this.addNewServersFeatureButton_MouseClick);
            // 
            // GettingStartedWizard
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.White;
            this.CancelButton = this.cancelButton;
            this.ClientSize = new System.Drawing.Size(599, 434);
            this.Controls.Add(this.visitTrialCenterFeatureButton);
            this.Controls.Add(this.configureAlertsFeatureButton);
            this.Controls.Add(this.addNewServersFeatureButton);
            this.Controls.Add(this.descriptionLabel);
            this.Controls.Add(this.footerPanel);
            this.Controls.Add(this.headerPanel);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.HelpButton = true;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "GettingStartedWizard";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "IDERA SQL Diagnostic Manager";
            this.HelpButtonClicked += new System.ComponentModel.CancelEventHandler(this.GettingStartedWizard_HelpButtonClicked);
            this.HelpRequested += new System.Windows.Forms.HelpEventHandler(this.GettingStartedWizard_HelpRequested);
            this.headerPanel.ResumeLayout(false);
            this.headerPanel.PerformLayout();
            this.footerPanel.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button cancelButton;
        private System.Windows.Forms.Label headerLabel;
        private System.Windows.Forms.Panel headerPanel;
        private System.Windows.Forms.Panel footerPanel;
        private System.Windows.Forms.Label descriptionLabel;
        private System.Windows.Forms.Label dividerLabel1;
        private System.Windows.Forms.Label productDescriptionLabel;
        private Idera.SQLdm.DesktopClient.Controls.FeatureButton addNewServersFeatureButton;
        private Idera.SQLdm.DesktopClient.Controls.FeatureButton configureAlertsFeatureButton;
        private Idera.SQLdm.DesktopClient.Controls.FeatureButton visitTrialCenterFeatureButton;
    }
}