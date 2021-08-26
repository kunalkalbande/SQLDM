namespace Idera.SQLdm.Common.UI.Dialogs
{
    partial class SqlServerBrowserDialog
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(SqlServerBrowserDialog));
            this.searchScopeTabControl = new System.Windows.Forms.TabControl();
            this.localInstancesTab = new System.Windows.Forms.TabPage();
            this.localInstancesStatusLabel = new System.Windows.Forms.Label();
            this.localInstancesProgressControl = new MRG.Controls.UI.LoadingCircle();
            this.localServersLabel = new System.Windows.Forms.Label();
            this.localInstancesTreeView = new System.Windows.Forms.TreeView();
            this.instanceImages = new System.Windows.Forms.ImageList(this.components);
            this.networkInstancesTab = new System.Windows.Forms.TabPage();
            this.networkInstancesStatusLabel = new System.Windows.Forms.Label();
            this.networkInstancesProgressControl = new MRG.Controls.UI.LoadingCircle();
            this.networkServersLabel = new System.Windows.Forms.Label();
            this.networkInstancesTreeView = new System.Windows.Forms.TreeView();
            this.okButton = new System.Windows.Forms.Button();
            this.cancelButton = new System.Windows.Forms.Button();
            this.localInstancesWorker = new System.ComponentModel.BackgroundWorker();
            this.networkInstancesWorker = new System.ComponentModel.BackgroundWorker();
            this.searchScopeTabControl.SuspendLayout();
            this.localInstancesTab.SuspendLayout();
            this.networkInstancesTab.SuspendLayout();
            this.SuspendLayout();
            // 
            // searchScopeTabControl
            // 
            this.searchScopeTabControl.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.searchScopeTabControl.Controls.Add(this.localInstancesTab);
            this.searchScopeTabControl.Controls.Add(this.networkInstancesTab);
            this.searchScopeTabControl.Location = new System.Drawing.Point(12, 12);
            this.searchScopeTabControl.Name = "searchScopeTabControl";
            this.searchScopeTabControl.SelectedIndex = 0;
            this.searchScopeTabControl.Size = new System.Drawing.Size(352, 329);
            this.searchScopeTabControl.TabIndex = 0;
            this.searchScopeTabControl.SelectedIndexChanged += new System.EventHandler(this.searchScopeTabControl_SelectedIndexChanged);
            // 
            // localInstancesTab
            // 
            this.localInstancesTab.Controls.Add(this.localInstancesStatusLabel);
            this.localInstancesTab.Controls.Add(this.localInstancesProgressControl);
            this.localInstancesTab.Controls.Add(this.localServersLabel);
            this.localInstancesTab.Controls.Add(this.localInstancesTreeView);
            this.localInstancesTab.Location = new System.Drawing.Point(4, 22);
            this.localInstancesTab.Name = "localInstancesTab";
            this.localInstancesTab.Padding = new System.Windows.Forms.Padding(3);
            this.localInstancesTab.Size = new System.Drawing.Size(344, 303);
            this.localInstancesTab.TabIndex = 0;
            this.localInstancesTab.Text = "Local Instances";
            this.localInstancesTab.UseVisualStyleBackColor = true;
            // 
            // localInstancesStatusLabel
            // 
            this.localInstancesStatusLabel.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.localInstancesStatusLabel.BackColor = System.Drawing.Color.White;
            this.localInstancesStatusLabel.ForeColor = System.Drawing.SystemColors.ControlText;
            this.localInstancesStatusLabel.Location = new System.Drawing.Point(9, 27);
            this.localInstancesStatusLabel.Name = "localInstancesStatusLabel";
            this.localInstancesStatusLabel.Size = new System.Drawing.Size(326, 18);
            this.localInstancesStatusLabel.TabIndex = 2;
            this.localInstancesStatusLabel.Text = "There are no local instances available on this machine.";
            this.localInstancesStatusLabel.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.localInstancesStatusLabel.Visible = false;
            // 
            // localInstancesProgressControl
            // 
            this.localInstancesProgressControl.Active = false;
            this.localInstancesProgressControl.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.localInstancesProgressControl.BackColor = System.Drawing.Color.White;
            this.localInstancesProgressControl.Color = System.Drawing.Color.DarkGray;
            this.localInstancesProgressControl.InnerCircleRadius = 8;
            this.localInstancesProgressControl.Location = new System.Drawing.Point(7, 26);
            this.localInstancesProgressControl.Name = "localInstancesProgressControl";
            this.localInstancesProgressControl.NumberSpoke = 10;
            this.localInstancesProgressControl.OuterCircleRadius = 12;
            this.localInstancesProgressControl.RotationSpeed = 80;
            this.localInstancesProgressControl.Size = new System.Drawing.Size(330, 270);
            this.localInstancesProgressControl.SpokeThickness = 4;
            this.localInstancesProgressControl.TabIndex = 3;
            // 
            // localServersLabel
            // 
            this.localServersLabel.AutoSize = true;
            this.localServersLabel.Location = new System.Drawing.Point(6, 9);
            this.localServersLabel.Name = "localServersLabel";
            this.localServersLabel.Size = new System.Drawing.Size(241, 13);
            this.localServersLabel.TabIndex = 0;
            this.localServersLabel.Text = "Select a local SQL Server instance to connect to:";
            // 
            // localInstancesTreeView
            // 
            this.localInstancesTreeView.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.localInstancesTreeView.FullRowSelect = true;
            this.localInstancesTreeView.HideSelection = false;
            this.localInstancesTreeView.ImageIndex = 0;
            this.localInstancesTreeView.ImageList = this.instanceImages;
            this.localInstancesTreeView.Location = new System.Drawing.Point(6, 25);
            this.localInstancesTreeView.Name = "localInstancesTreeView";
            this.localInstancesTreeView.SelectedImageIndex = 0;
            this.localInstancesTreeView.ShowRootLines = false;
            this.localInstancesTreeView.Size = new System.Drawing.Size(332, 272);
            this.localInstancesTreeView.TabIndex = 4;
            this.localInstancesTreeView.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler(this.localInstancesTreeView_MouseDoubleClick);
            this.localInstancesTreeView.AfterSelect += new System.Windows.Forms.TreeViewEventHandler(this.localInstancesTreeView_AfterSelect);
            // 
            // instanceImages
            // 
            this.instanceImages.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("instanceImages.ImageStream")));
            this.instanceImages.TransparentColor = System.Drawing.Color.Transparent;
            this.instanceImages.Images.SetKeyName(0, "SmallServer.png");
            // 
            // networkInstancesTab
            // 
            this.networkInstancesTab.Controls.Add(this.networkInstancesStatusLabel);
            this.networkInstancesTab.Controls.Add(this.networkInstancesProgressControl);
            this.networkInstancesTab.Controls.Add(this.networkServersLabel);
            this.networkInstancesTab.Controls.Add(this.networkInstancesTreeView);
            this.networkInstancesTab.Location = new System.Drawing.Point(4, 22);
            this.networkInstancesTab.Name = "networkInstancesTab";
            this.networkInstancesTab.Padding = new System.Windows.Forms.Padding(3);
            this.networkInstancesTab.Size = new System.Drawing.Size(344, 303);
            this.networkInstancesTab.TabIndex = 1;
            this.networkInstancesTab.Text = "Network Instances";
            this.networkInstancesTab.UseVisualStyleBackColor = true;
            // 
            // networkInstancesStatusLabel
            // 
            this.networkInstancesStatusLabel.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.networkInstancesStatusLabel.BackColor = System.Drawing.Color.White;
            this.networkInstancesStatusLabel.ForeColor = System.Drawing.SystemColors.ControlText;
            this.networkInstancesStatusLabel.Location = new System.Drawing.Point(9, 27);
            this.networkInstancesStatusLabel.Name = "networkInstancesStatusLabel";
            this.networkInstancesStatusLabel.Size = new System.Drawing.Size(326, 18);
            this.networkInstancesStatusLabel.TabIndex = 6;
            this.networkInstancesStatusLabel.Text = "Unable to retrieve any available network instances.";
            this.networkInstancesStatusLabel.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.networkInstancesStatusLabel.Visible = false;
            // 
            // networkInstancesProgressControl
            // 
            this.networkInstancesProgressControl.Active = false;
            this.networkInstancesProgressControl.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.networkInstancesProgressControl.BackColor = System.Drawing.Color.White;
            this.networkInstancesProgressControl.Color = System.Drawing.Color.DarkGray;
            this.networkInstancesProgressControl.InnerCircleRadius = 8;
            this.networkInstancesProgressControl.Location = new System.Drawing.Point(7, 26);
            this.networkInstancesProgressControl.Name = "networkInstancesProgressControl";
            this.networkInstancesProgressControl.NumberSpoke = 10;
            this.networkInstancesProgressControl.OuterCircleRadius = 12;
            this.networkInstancesProgressControl.RotationSpeed = 80;
            this.networkInstancesProgressControl.Size = new System.Drawing.Size(330, 270);
            this.networkInstancesProgressControl.SpokeThickness = 4;
            this.networkInstancesProgressControl.TabIndex = 7;
            // 
            // networkServersLabel
            // 
            this.networkServersLabel.AutoSize = true;
            this.networkServersLabel.Location = new System.Drawing.Point(6, 9);
            this.networkServersLabel.Name = "networkServersLabel";
            this.networkServersLabel.Size = new System.Drawing.Size(290, 13);
            this.networkServersLabel.TabIndex = 4;
            this.networkServersLabel.Text = "Select a SQL Server instance on the network to connect to:";
            // 
            // networkInstancesTreeView
            // 
            this.networkInstancesTreeView.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.networkInstancesTreeView.FullRowSelect = true;
            this.networkInstancesTreeView.HideSelection = false;
            this.networkInstancesTreeView.ImageIndex = 0;
            this.networkInstancesTreeView.ImageList = this.instanceImages;
            this.networkInstancesTreeView.Location = new System.Drawing.Point(6, 25);
            this.networkInstancesTreeView.Name = "networkInstancesTreeView";
            this.networkInstancesTreeView.SelectedImageIndex = 0;
            this.networkInstancesTreeView.ShowRootLines = false;
            this.networkInstancesTreeView.Size = new System.Drawing.Size(332, 272);
            this.networkInstancesTreeView.TabIndex = 8;
            this.networkInstancesTreeView.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler(this.networkInstancesTreeView_MouseDoubleClick);
            this.networkInstancesTreeView.AfterSelect += new System.Windows.Forms.TreeViewEventHandler(this.networkInstancesTreeView_AfterSelect);
            // 
            // okButton
            // 
            this.okButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.okButton.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.okButton.Enabled = false;
            this.okButton.Location = new System.Drawing.Point(208, 347);
            this.okButton.Name = "okButton";
            this.okButton.Size = new System.Drawing.Size(75, 23);
            this.okButton.TabIndex = 1;
            this.okButton.Text = "OK";
            this.okButton.UseVisualStyleBackColor = true;
            // 
            // cancelButton
            // 
            this.cancelButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.cancelButton.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.cancelButton.Location = new System.Drawing.Point(289, 347);
            this.cancelButton.Name = "cancelButton";
            this.cancelButton.Size = new System.Drawing.Size(75, 23);
            this.cancelButton.TabIndex = 2;
            this.cancelButton.Text = "Cancel";
            this.cancelButton.UseVisualStyleBackColor = true;
            // 
            // localInstancesWorker
            // 
            this.localInstancesWorker.DoWork += new System.ComponentModel.DoWorkEventHandler(this.localInstancesWorker_DoWork);
            this.localInstancesWorker.RunWorkerCompleted += new System.ComponentModel.RunWorkerCompletedEventHandler(this.localInstancesWorker_RunWorkerCompleted);
            // 
            // networkInstancesWorker
            // 
            this.networkInstancesWorker.DoWork += new System.ComponentModel.DoWorkEventHandler(this.networkInstancesWorker_DoWork);
            this.networkInstancesWorker.RunWorkerCompleted += new System.ComponentModel.RunWorkerCompletedEventHandler(this.networkInstancesWorker_RunWorkerCompleted);
            // 
            // SqlServerBrowserDialog
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(376, 382);
            this.Controls.Add(this.cancelButton);
            this.Controls.Add(this.okButton);
            this.Controls.Add(this.searchScopeTabControl);
            this.HelpButton = true;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.MinimumSize = new System.Drawing.Size(350, 375);
            this.Name = "SqlServerBrowserDialog";
            this.ShowInTaskbar = false;
            this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Show;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Browse for SQL Servers";
            this.searchScopeTabControl.ResumeLayout(false);
            this.localInstancesTab.ResumeLayout(false);
            this.localInstancesTab.PerformLayout();
            this.networkInstancesTab.ResumeLayout(false);
            this.networkInstancesTab.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TabControl searchScopeTabControl;
        private System.Windows.Forms.TabPage localInstancesTab;
        private System.Windows.Forms.TabPage networkInstancesTab;
        private System.Windows.Forms.Button okButton;
        private System.Windows.Forms.Button cancelButton;
        private System.Windows.Forms.Label localServersLabel;
        private System.ComponentModel.BackgroundWorker localInstancesWorker;
        private System.Windows.Forms.Label localInstancesStatusLabel;
        private MRG.Controls.UI.LoadingCircle localInstancesProgressControl;
        private System.Windows.Forms.ImageList instanceImages;
        private System.Windows.Forms.Label networkInstancesStatusLabel;
        private MRG.Controls.UI.LoadingCircle networkInstancesProgressControl;
        private System.Windows.Forms.Label networkServersLabel;
        private System.ComponentModel.BackgroundWorker networkInstancesWorker;
        private System.Windows.Forms.TreeView localInstancesTreeView;
        private System.Windows.Forms.TreeView networkInstancesTreeView;
    }
}