namespace Idera.SQLdm.Common.UI.Dialogs
{
    partial class DiskExcludeDialog
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(DiskExcludeDialog));
            this.cancelButton = new System.Windows.Forms.Button();
            this.okButton = new System.Windows.Forms.Button();
            this.descriptionLabel = new System.Windows.Forms.Label();
            this.diskTreeView = new System.Windows.Forms.TreeView();
            this.treeViewImages = new System.Windows.Forms.ImageList(this.components);
            this.backgroundWorker = new System.ComponentModel.BackgroundWorker();
            this.browseProgressControl = new MRG.Controls.UI.LoadingCircle();
            this.SuspendLayout();
            // 
            // cancelButton
            // 
            this.cancelButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.cancelButton.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.cancelButton.Location = new System.Drawing.Point(255, 306);
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
            this.okButton.Location = new System.Drawing.Point(174, 306);
            this.okButton.Name = "okButton";
            this.okButton.Size = new System.Drawing.Size(75, 23);
            this.okButton.TabIndex = 1;
            this.okButton.Text = "OK";
            this.okButton.UseVisualStyleBackColor = true;
            this.okButton.Click += new System.EventHandler(this.okButton_Click);
            // 
            // descriptionLabel
            // 
            this.descriptionLabel.AutoSize = true;
            this.descriptionLabel.Location = new System.Drawing.Point(12, 9);
            this.descriptionLabel.Name = "descriptionLabel";
            this.descriptionLabel.Size = new System.Drawing.Size(253, 13);
            this.descriptionLabel.TabIndex = 4;
            this.descriptionLabel.Text = "Browse the server and select the disk to connect to:";
            // 
            // diskTreeView
            // 
            this.diskTreeView.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.diskTreeView.ImageIndex = 0;
            this.diskTreeView.ImageList = this.treeViewImages;
            this.diskTreeView.Location = new System.Drawing.Point(12, 36);
            this.diskTreeView.Name = "diskTreeView";
            this.diskTreeView.SelectedImageIndex = 0;
            this.diskTreeView.Size = new System.Drawing.Size(318, 264);
            this.diskTreeView.TabIndex = 0;
            this.diskTreeView.AfterCheck += new System.Windows.Forms.TreeViewEventHandler(this.diskTreeView_AfterCheck);
            this.diskTreeView.AfterSelect += new System.Windows.Forms.TreeViewEventHandler(this.diskTreeView_AfterSelect);
            this.diskTreeView.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler(this.diskTreeView_MouseDoubleClick);
            // 
            // treeViewImages
            // 
            this.treeViewImages.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("treeViewImages.ImageStream")));
            this.treeViewImages.TransparentColor = System.Drawing.Color.Transparent;
            this.treeViewImages.Images.SetKeyName(0, "FolderClosed16x16.png");
            this.treeViewImages.Images.SetKeyName(1, "ResourcesDiskSizeView16x16.png");
            this.treeViewImages.Images.SetKeyName(2, "Server.png");
            // 
            // backgroundWorker
            // 
            this.backgroundWorker.DoWork += new System.ComponentModel.DoWorkEventHandler(this.backgroundWorker_DoWork);
            this.backgroundWorker.RunWorkerCompleted += new System.ComponentModel.RunWorkerCompletedEventHandler(this.backgroundWorker_RunWorkerCompleted);
            // 
            // browseProgressControl
            // 
            this.browseProgressControl.Active = false;
            this.browseProgressControl.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.browseProgressControl.BackColor = System.Drawing.Color.White;
            this.browseProgressControl.Color = System.Drawing.Color.DarkGray;
            this.browseProgressControl.InnerCircleRadius = 5;
            this.browseProgressControl.Location = new System.Drawing.Point(13, 37);
            this.browseProgressControl.Name = "browseProgressControl";
            this.browseProgressControl.NumberSpoke = 12;
            this.browseProgressControl.OuterCircleRadius = 11;
            this.browseProgressControl.RotationSpeed = 80;
            this.browseProgressControl.Size = new System.Drawing.Size(316, 262);
            this.browseProgressControl.SpokeThickness = 2;
            this.browseProgressControl.StylePreset = MRG.Controls.UI.LoadingCircle.StylePresets.MacOSX;
            this.browseProgressControl.TabIndex = 0;
            // 
            // DiskExcludeDialog
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(342, 341);
            this.Controls.Add(this.browseProgressControl);
            this.Controls.Add(this.diskTreeView);
            this.Controls.Add(this.descriptionLabel);
            this.Controls.Add(this.cancelButton);
            this.Controls.Add(this.okButton);
            this.HelpButton = true;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.MinimumSize = new System.Drawing.Size(350, 375);
            this.Name = "DiskExcludeDialog";
            this.ShowInTaskbar = false;
            this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Show;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Browse Server for Disk";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button cancelButton;
        private System.Windows.Forms.Button okButton;
        private System.Windows.Forms.Label descriptionLabel;
        private System.Windows.Forms.TreeView diskTreeView;
        private System.Windows.Forms.ImageList treeViewImages;
        private System.ComponentModel.BackgroundWorker backgroundWorker;
        private MRG.Controls.UI.LoadingCircle browseProgressControl;
    }
}