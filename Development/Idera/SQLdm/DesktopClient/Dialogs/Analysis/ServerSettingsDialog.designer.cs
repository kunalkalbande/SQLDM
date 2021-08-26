using Idera.SQLdm.DesktopClient.Controls.Analysis;
namespace Idera.SQLdm.DesktopClient.Dialogs.Analysis
{
    partial class ServerSettingsDialog
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ServerSettingsDialog));
            this.tabSettings = new System.Windows.Forms.TabControl();
            this.tabFilters = new System.Windows.Forms.TabPage();
            this.filtersSettings = new FiltersSettingsTab(instnceID, includeDatabase, filterApplicationText);
            this.tabBlockRecommendations = new System.Windows.Forms.TabPage();
            this.blockedRecommendations = new BlockedRecommendationsTab(blockedRecommendation);
            this.tabBlockDatabases = new System.Windows.Forms.TabPage();
            this.blockedDatabases = new BlockedDatabasesTab(instnceID, blockedDatabase);
            this.instanceImages = new System.Windows.Forms.ImageList(this.components);
            this.okButton = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomButton();
            this.cancelButton = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomButton();
            this.localInstancesWorker = new System.ComponentModel.BackgroundWorker();
            this.networkInstancesWorker = new System.ComponentModel.BackgroundWorker();
            this.tabSettings.SuspendLayout();
            this.tabFilters.SuspendLayout();
            this.tabBlockRecommendations.SuspendLayout();
            this.tabBlockDatabases.SuspendLayout();
            this.SuspendLayout();
            // 
            // tabSettings
            // 
            this.tabSettings.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.tabSettings.Controls.Add(this.tabFilters);
            this.tabSettings.Controls.Add(this.tabBlockRecommendations);
            this.tabSettings.Controls.Add(this.tabBlockDatabases);
            this.tabSettings.Location = new System.Drawing.Point(12, 39);
            this.tabSettings.Name = "tabSettings";
            this.tabSettings.SelectedIndex = 0;
            this.tabSettings.Size = new System.Drawing.Size(560, 442);
            this.tabSettings.TabIndex = 0;
            // 
            // tabFilters
            // 
            this.tabFilters.Controls.Add(this.filtersSettings);
            this.tabFilters.Location = new System.Drawing.Point(4, 22);
            this.tabFilters.Name = "tabFilters";
            this.tabFilters.Padding = new System.Windows.Forms.Padding(3);
            this.tabFilters.Size = new System.Drawing.Size(552, 416);
            this.tabFilters.TabIndex = 0;
            this.tabFilters.Text = "Filters Settings";
            this.tabFilters.UseVisualStyleBackColor = true;
            // 
            // filtersSettings
            // 
            this.filtersSettings.Dock = System.Windows.Forms.DockStyle.Fill;
            this.filtersSettings.Location = new System.Drawing.Point(3, 3);
            this.filtersSettings.Name = "filtersSettings";
            this.filtersSettings.Size = new System.Drawing.Size(546, 410);
            this.filtersSettings.TabIndex = 0;
            this.filtersSettings.SettingsChanged += new System.EventHandler(this.filtersSettings_SettingsChanged);
            // 
            // tabBlockRecommendations
            // 
            this.tabBlockRecommendations.Controls.Add(this.blockedRecommendations);
            this.tabBlockRecommendations.Location = new System.Drawing.Point(4, 22);
            this.tabBlockRecommendations.Name = "tabBlockRecommendations";
            this.tabBlockRecommendations.Padding = new System.Windows.Forms.Padding(3);
            this.tabBlockRecommendations.Size = new System.Drawing.Size(542, 406);
            this.tabBlockRecommendations.TabIndex = 1;
            this.tabBlockRecommendations.Text = "Block Recommendations";
            this.tabBlockRecommendations.UseVisualStyleBackColor = true;
            // 
            // blockedRecommendations
            // 
            this.blockedRecommendations.Dock = System.Windows.Forms.DockStyle.Fill;
            this.blockedRecommendations.Location = new System.Drawing.Point(3, 3);
            this.blockedRecommendations.Name = "blockedRecommendations";
            this.blockedRecommendations.Size = new System.Drawing.Size(536, 400);
            this.blockedRecommendations.TabIndex = 0;
            this.blockedRecommendations.SettingsChanged += new System.EventHandler(this.blockedRecommendations_SettingsChanged);
            // 
            // tabBlockDatabases
            // 
            this.tabBlockDatabases.Controls.Add(this.blockedDatabases);
            this.tabBlockDatabases.Location = new System.Drawing.Point(4, 22);
            this.tabBlockDatabases.Name = "tabBlockDatabases";
            this.tabBlockDatabases.Size = new System.Drawing.Size(542, 406);
            this.tabBlockDatabases.TabIndex = 2;
            this.tabBlockDatabases.Text = "Block Databases";
            this.tabBlockDatabases.UseVisualStyleBackColor = true;
            // 
            // blockedDatabases
            // 
            this.blockedDatabases.Dock = System.Windows.Forms.DockStyle.Fill;
            this.blockedDatabases.Location = new System.Drawing.Point(0, 0);
            this.blockedDatabases.Name = "blockedDatabases";
            this.blockedDatabases.Size = new System.Drawing.Size(542, 406);
            this.blockedDatabases.TabIndex = 0;
            this.blockedDatabases.SettingsChanged += new System.EventHandler(this.blockedDatabases_SettingsChanged);
            // 
            // instanceImages
            // 
            this.instanceImages.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("instanceImages.ImageStream")));
            this.instanceImages.TransparentColor = System.Drawing.Color.Transparent;
            this.instanceImages.Images.SetKeyName(0, "SmallServer.png");
            // 
            // okButton
            // 
            this.okButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.okButton.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.okButton.Enabled = false;
            this.okButton.Location = new System.Drawing.Point(416, 487);
            this.okButton.Name = "okButton";
            this.okButton.Size = new System.Drawing.Size(75, 23);
            this.okButton.TabIndex = 1;
            this.okButton.Text = "OK";
            this.okButton.UseVisualStyleBackColor = true;
            this.okButton.Click += new System.EventHandler(this.okButton_Click);
            // 
            // cancelButton
            // 
            this.cancelButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.cancelButton.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.cancelButton.Location = new System.Drawing.Point(497, 487);
            this.cancelButton.Name = "cancelButton";
            this.cancelButton.Size = new System.Drawing.Size(75, 23);
            this.cancelButton.TabIndex = 2;
            this.cancelButton.Text = "Cancel";
            this.cancelButton.UseVisualStyleBackColor = true;
            this.cancelButton.Click += new System.EventHandler(this.cancelButton_Click);
            // 
            // ServerSettingsDialog
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.cancelButton;
            this.ClientSize = new System.Drawing.Size(574, 512);
            this.Controls.Add(this.cancelButton);
            this.Controls.Add(this.okButton);
            this.Controls.Add(this.tabSettings);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.HelpButton = true;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.MinimumSize = new System.Drawing.Size(590, 550);
            this.Name = "ServerSettingsDialog";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Show;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Server Settings";
            this.HelpButtonClicked += new System.ComponentModel.CancelEventHandler(this.ServerSettingsDialog_HelpButtonClicked);
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.ServerSettingsDialog_FormClosing);
            this.HelpRequested += new System.Windows.Forms.HelpEventHandler(this.ServerSettingsDialog_HelpRequested);
            this.tabSettings.ResumeLayout(false);
            this.tabFilters.ResumeLayout(false);
            this.tabBlockRecommendations.ResumeLayout(false);
            this.tabBlockDatabases.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TabControl tabSettings;
        private System.Windows.Forms.TabPage tabFilters;
        private System.Windows.Forms.TabPage tabBlockRecommendations;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomButton okButton;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomButton cancelButton;
        private System.ComponentModel.BackgroundWorker localInstancesWorker;
        private System.Windows.Forms.ImageList instanceImages;
        private System.ComponentModel.BackgroundWorker networkInstancesWorker;
        private System.Windows.Forms.TabPage tabBlockDatabases;
        public BlockedRecommendationsTab blockedRecommendations;
        public FiltersSettingsTab filtersSettings;
        public BlockedDatabasesTab blockedDatabases;
    }
}