namespace Idera.SQLdm.DesktopClient.Dialogs
{
    partial class BrowseReportServerFoldersDialog
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
            this.components = new System.ComponentModel.Container();
            this.label1 = new System.Windows.Forms.Label();
            this.reportServerFolderTreeView = new System.Windows.Forms.TreeView();
            this.cancelButton = new System.Windows.Forms.Button();
            this.okButton = new System.Windows.Forms.Button();
            this.loadingPogressControl = new MRG.Controls.UI.LoadingCircle();
            this.backgroundWorker = new System.ComponentModel.BackgroundWorker();
            this.treeImages = new System.Windows.Forms.ImageList(this.components);
            this.loadingLabel = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 9);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(230, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "Select a deployment folder on the report server:";
            // 
            // reportServerFolderTreeView
            // 
            this.reportServerFolderTreeView.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.reportServerFolderTreeView.FullRowSelect = true;
            this.reportServerFolderTreeView.HideSelection = false;
            this.reportServerFolderTreeView.ImageIndex = 0;
            this.reportServerFolderTreeView.ImageList = this.treeImages;
            this.reportServerFolderTreeView.Location = new System.Drawing.Point(12, 25);
            this.reportServerFolderTreeView.Name = "reportServerFolderTreeView";
            this.reportServerFolderTreeView.PathSeparator = "/";
            this.reportServerFolderTreeView.SelectedImageIndex = 0;
            this.reportServerFolderTreeView.ShowLines = false;
            this.reportServerFolderTreeView.ShowRootLines = false;
            this.reportServerFolderTreeView.Size = new System.Drawing.Size(320, 314);
            this.reportServerFolderTreeView.TabIndex = 1;
            this.reportServerFolderTreeView.AfterSelect += new System.Windows.Forms.TreeViewEventHandler(this.reportServerFolderTreeView_AfterSelect);
            // 
            // cancelButton
            // 
            this.cancelButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.cancelButton.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.cancelButton.Location = new System.Drawing.Point(257, 345);
            this.cancelButton.Name = "cancelButton";
            this.cancelButton.Size = new System.Drawing.Size(75, 23);
            this.cancelButton.TabIndex = 2;
            this.cancelButton.Text = "Cancel";
            this.cancelButton.UseVisualStyleBackColor = true;
            // 
            // okButton
            // 
            this.okButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.okButton.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.okButton.Enabled = false;
            this.okButton.Location = new System.Drawing.Point(176, 345);
            this.okButton.Name = "okButton";
            this.okButton.Size = new System.Drawing.Size(75, 23);
            this.okButton.TabIndex = 3;
            this.okButton.Text = "OK";
            this.okButton.UseVisualStyleBackColor = true;
            // 
            // loadingPogressControl
            // 
            this.loadingPogressControl.Active = false;
            this.loadingPogressControl.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.loadingPogressControl.BackColor = System.Drawing.Color.White;
            this.loadingPogressControl.Color = System.Drawing.Color.DarkGray;
            this.loadingPogressControl.InnerCircleRadius = 8;
            this.loadingPogressControl.Location = new System.Drawing.Point(135, 140);
            this.loadingPogressControl.Name = "loadingPogressControl";
            this.loadingPogressControl.NumberSpoke = 10;
            this.loadingPogressControl.OuterCircleRadius = 10;
            this.loadingPogressControl.RotationSpeed = 80;
            this.loadingPogressControl.Size = new System.Drawing.Size(75, 58);
            this.loadingPogressControl.SpokeThickness = 4;
            this.loadingPogressControl.TabIndex = 4;
            // 
            // backgroundWorker
            // 
            this.backgroundWorker.WorkerSupportsCancellation = true;
            this.backgroundWorker.DoWork += new System.ComponentModel.DoWorkEventHandler(this.backgroundWorker_DoWork);
            this.backgroundWorker.RunWorkerCompleted += new System.ComponentModel.RunWorkerCompletedEventHandler(this.backgroundWorker_RunWorkerCompleted);
            // 
            // treeImages
            // 
            this.treeImages.ColorDepth = System.Windows.Forms.ColorDepth.Depth32Bit;
            this.treeImages.ImageSize = new System.Drawing.Size(16, 16);
            this.treeImages.TransparentColor = System.Drawing.Color.Transparent;
            // 
            // loadingLabel
            // 
            this.loadingLabel.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.loadingLabel.BackColor = System.Drawing.Color.White;
            this.loadingLabel.ForeColor = System.Drawing.SystemColors.ControlDarkDark;
            this.loadingLabel.Location = new System.Drawing.Point(70, 190);
            this.loadingLabel.Name = "loadingLabel";
            this.loadingLabel.Size = new System.Drawing.Size(204, 37);
            this.loadingLabel.TabIndex = 5;
            this.loadingLabel.Text = "Loading report folders. Initialization can take a minute or two.";
            this.loadingLabel.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            // 
            // BrowseReportServerFoldersDialog
            // 
            this.AcceptButton = this.okButton;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.cancelButton;
            this.ClientSize = new System.Drawing.Size(344, 374);
            this.Controls.Add(this.loadingLabel);
            this.Controls.Add(this.loadingPogressControl);
            this.Controls.Add(this.okButton);
            this.Controls.Add(this.cancelButton);
            this.Controls.Add(this.reportServerFolderTreeView);
            this.Controls.Add(this.label1);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.MinimumSize = new System.Drawing.Size(260, 290);
            this.Name = "BrowseReportServerFoldersDialog";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Browse For Deployment Folder";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.BrowseReportServerFoldersDialog_FormClosing);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TreeView reportServerFolderTreeView;
        private System.Windows.Forms.Button cancelButton;
        private System.Windows.Forms.Button okButton;
        private MRG.Controls.UI.LoadingCircle loadingPogressControl;
        private System.ComponentModel.BackgroundWorker backgroundWorker;
        private System.Windows.Forms.ImageList treeImages;
        private System.Windows.Forms.Label loadingLabel;

    }
}
