namespace Idera.SQLdm.DesktopClient.Dialogs
{
    partial class AboutDialog
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
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
            this.headerPanel = new System.Windows.Forms.Panel();
            this.listViewImages = new System.Windows.Forms.ImageList(this.components);
            this.gradientPanel1 = new Idera.SQLdm.DesktopClient.Controls.GradientPanel();
            this.copyrightLabel = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.copyInfoButton = new System.Windows.Forms.Button();
            this.componentListView = new System.Windows.Forms.ListView();
            this.componentHeader = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.versionHeader = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.okButton = new System.Windows.Forms.Button();
            this.systemInfoButton = new System.Windows.Forms.Button();
            this.connectionProgressBar = new Idera.SQLdm.DesktopClient.Controls.InfiniteProgressBar();
            this.gradientPanel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // headerPanel
            // 
            this.headerPanel.BackgroundImage = global::Idera.SQLdm.DesktopClient.Properties.Resources.RepositoryConnectionDialogHeader;
            this.headerPanel.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.headerPanel.Dock = System.Windows.Forms.DockStyle.Top;
            this.headerPanel.Location = new System.Drawing.Point(0, 0);
            this.headerPanel.Name = "headerPanel";
            this.headerPanel.Size = new System.Drawing.Size(476, 72);
            this.headerPanel.TabIndex = 1;
            // 
            // listViewImages
            // 
            this.listViewImages.ColorDepth = System.Windows.Forms.ColorDepth.Depth8Bit;
            this.listViewImages.ImageSize = new System.Drawing.Size(1, 20);
            this.listViewImages.TransparentColor = System.Drawing.Color.Transparent;
            // 
            // gradientPanel1
            // 
            this.gradientPanel1.BackColor = System.Drawing.SystemColors.Control;
            this.gradientPanel1.BackColor2 = System.Drawing.Color.FromArgb(((int)(((byte)(191)))), ((int)(((byte)(203)))), ((int)(((byte)(204)))));
            this.gradientPanel1.BorderColor = System.Drawing.SystemColors.ControlDark;
            this.gradientPanel1.Controls.Add(this.copyrightLabel);
            this.gradientPanel1.Controls.Add(this.label1);
            this.gradientPanel1.Controls.Add(this.copyInfoButton);
            this.gradientPanel1.Controls.Add(this.componentListView);
            this.gradientPanel1.Controls.Add(this.okButton);
            this.gradientPanel1.Controls.Add(this.systemInfoButton);
            this.gradientPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.gradientPanel1.FillStyle = Idera.SQLdm.DesktopClient.Controls.GradientPanelFillStyle.Solid;
            this.gradientPanel1.Location = new System.Drawing.Point(0, 75);
            this.gradientPanel1.Name = "gradientPanel1";
            this.gradientPanel1.Padding = new System.Windows.Forms.Padding(1);
            this.gradientPanel1.ShowBorder = false;
            this.gradientPanel1.Size = new System.Drawing.Size(476, 288);
            this.gradientPanel1.TabIndex = 27;
            // 
            // copyrightLabel
            // 
            this.copyrightLabel.ForeColor = System.Drawing.SystemColors.ControlDarkDark;
            this.copyrightLabel.Location = new System.Drawing.Point(12, 182);
            this.copyrightLabel.Name = "copyrightLabel";
            this.copyrightLabel.Size = new System.Drawing.Size(359, 26);
            this.copyrightLabel.TabIndex = 30;
            this.copyrightLabel.Text = assemblyInfo.GetCommonAssemblyCopyright();
            // 
            // label1
            // 
            this.label1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.label1.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.label1.Location = new System.Drawing.Point(12, 215);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(452, 2);
            this.label1.TabIndex = 29;
            // 
            // copyInfoButton
            // 
            this.copyInfoButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.copyInfoButton.Location = new System.Drawing.Point(377, 185);
            this.copyInfoButton.Name = "copyInfoButton";
            this.copyInfoButton.Size = new System.Drawing.Size(87, 23);
            this.copyInfoButton.TabIndex = 3;
            this.copyInfoButton.Text = "Copy Info";
            this.copyInfoButton.UseVisualStyleBackColor = true;
            this.copyInfoButton.Click += new System.EventHandler(this.copyInfoButton_Click);
            // 
            // componentListView
            // 
            this.componentListView.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.componentHeader,
            this.versionHeader});
            this.componentListView.FullRowSelect = true;
            this.componentListView.GridLines = true;
            this.componentListView.Location = new System.Drawing.Point(12, 6);
            this.componentListView.MultiSelect = false;
            this.componentListView.Name = "componentListView";
            this.componentListView.Size = new System.Drawing.Size(452, 173);
            this.componentListView.SmallImageList = this.listViewImages;
            this.componentListView.TabIndex = 2;
            this.componentListView.UseCompatibleStateImageBehavior = false;
            this.componentListView.View = System.Windows.Forms.View.Details;
            // 
            // componentHeader
            // 
            this.componentHeader.Text = "Component";
            this.componentHeader.Width = 298;
            // 
            // versionHeader
            // 
            this.versionHeader.Text = "Version";
            this.versionHeader.Width = 150;
            // 
            // okButton
            // 
            this.okButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.okButton.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.okButton.Location = new System.Drawing.Point(377, 223);
            this.okButton.Name = "okButton";
            this.okButton.Size = new System.Drawing.Size(87, 23);
            this.okButton.TabIndex = 0;
            this.okButton.Text = "&OK";
            // 
            // systemInfoButton
            // 
            this.systemInfoButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.systemInfoButton.Location = new System.Drawing.Point(377, 252);
            this.systemInfoButton.Name = "systemInfoButton";
            this.systemInfoButton.Size = new System.Drawing.Size(87, 23);
            this.systemInfoButton.TabIndex = 1;
            this.systemInfoButton.Text = "System Info...";
            this.systemInfoButton.UseVisualStyleBackColor = true;
            this.systemInfoButton.Click += new System.EventHandler(this.systemInfoButton_Click);
            // 
            // connectionProgressBar
            // 
            this.connectionProgressBar.Color1 = System.Drawing.Color.FromArgb(((int)(((byte)(149)))), ((int)(((byte)(201)))), ((int)(((byte)(67)))));
            this.connectionProgressBar.Color2 = System.Drawing.Color.White;
            this.connectionProgressBar.Dock = System.Windows.Forms.DockStyle.Top;
            this.connectionProgressBar.Location = new System.Drawing.Point(0, 72);
            this.connectionProgressBar.Name = "connectionProgressBar";
            this.connectionProgressBar.Size = new System.Drawing.Size(476, 3);
            this.connectionProgressBar.Speed = 15D;
            this.connectionProgressBar.Step = 5F;
            this.connectionProgressBar.TabIndex = 4;
            // 
            // AboutDialog
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(476, 363);
            this.Controls.Add(this.gradientPanel1);
            this.Controls.Add(this.connectionProgressBar);
            this.Controls.Add(this.headerPanel);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "AboutDialog";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "AboutBox";
            this.gradientPanel1.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button okButton;
        private System.Windows.Forms.Button systemInfoButton;
        private System.Windows.Forms.Panel headerPanel;
        private Idera.SQLdm.DesktopClient.Controls.InfiniteProgressBar connectionProgressBar;
        private Idera.SQLdm.DesktopClient.Controls.GradientPanel gradientPanel1;
        private System.Windows.Forms.Button copyInfoButton;
        private System.Windows.Forms.ListView componentListView;
        private System.Windows.Forms.ColumnHeader componentHeader;
        private System.Windows.Forms.ColumnHeader versionHeader;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label copyrightLabel;
        private System.Windows.Forms.ImageList listViewImages;
    }
}
