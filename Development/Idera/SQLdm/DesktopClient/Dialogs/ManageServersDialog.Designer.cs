using Idera.SQLdm.DesktopClient.Controls.CustomControls;
using Idera.SQLdm.DesktopClient.Properties;
using System;

namespace Idera.SQLdm.DesktopClient.Dialogs
{
    partial class ManageServersDialog
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ManageServersDialog));
            this.cancelButton = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomButton();
            this.okButton = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomButton();
            this.headerPanel = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomPanel ();
            this.descriptionLabel = new Controls.CustomControls.CustomLabel();
            this.dividerLabel1 = new Controls.CustomControls.CustomLabel();
            this.instancesListView = new CustomListView();
            this.testStatusHeader = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.instanceNameHeader = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.authenticationModeHeader = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.vCenterHeader = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.vmNameHeader = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.connectionTestStatusImages = new System.Windows.Forms.ImageList(this.components);
            this.addButton = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomButton();
            this.removeButton = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomButton();
            this.testButton = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomButton();
            this.editButton = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomButton();
            this.applyButton = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomButton();
            this.instanceCountLabel = new Controls.CustomControls.CustomLabel(); ;
            this.licenseInformationPictureBox = new System.Windows.Forms.PictureBox();
            this.licenseInformationLabel = new Controls.CustomControls.CustomLabel(); ;
            this.gettingStartedLabel = new Controls.CustomControls.CustomLabel(); ;
            this.applyChangesBackgroundWorker = new System.ComponentModel.BackgroundWorker();
            this.btnVMConfig = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomButton();
            this.statusProgressBar = new Idera.SQLdm.DesktopClient.Controls.InfiniteProgressBar();
            this.headerPanel.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.licenseInformationPictureBox)).BeginInit();
            this.SuspendLayout();
            // 
            // cancelButton
            // 
            this.cancelButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.cancelButton.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.cancelButton.Location = new System.Drawing.Point(478, 419);
            this.cancelButton.Name = "cancelButton";
            this.cancelButton.Size = new System.Drawing.Size(75, 23);
            this.cancelButton.TabIndex = 6;
            this.cancelButton.Text = "Cancel";
            this.cancelButton.UseVisualStyleBackColor = true;
            // 
            // okButton
            // 
            this.okButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.okButton.Location = new System.Drawing.Point(316, 419);
            this.okButton.Name = "okButton";
            this.okButton.Size = new System.Drawing.Size(75, 23);
            this.okButton.TabIndex = 4;
            this.okButton.Text = "OK";
            this.okButton.UseVisualStyleBackColor = true;
            this.okButton.Click += new System.EventHandler(this.okButton_Click);
            // 
            // headerPanel
            // 
            this.headerPanel.BackColor = System.Drawing.Color.White;
            if (Settings.Default.ColorScheme != "Dark")
                this.headerPanel.BackgroundImage = global::Idera.SQLdm.DesktopClient.Properties.Resources.AddServersManagerDialogHeader;
            this.headerPanel.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.headerPanel.Controls.Add(this.descriptionLabel);
            this.headerPanel.Dock = System.Windows.Forms.DockStyle.Top;
            this.headerPanel.Location = new System.Drawing.Point(0, 0);
            this.headerPanel.Name = "headerPanel";
            this.headerPanel.Size = new System.Drawing.Size(500, 60);
            this.headerPanel.TabIndex = 7;
            //this.headerPanel.BackColor = System.Drawing.Color.Yellow;
            // 
            // descriptionLabel
            // 
            this.descriptionLabel.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.descriptionLabel.BackColor = System.Drawing.Color.Transparent;
            this.descriptionLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            if (Settings.Default.ColorScheme == "Dark")
                this.descriptionLabel.Location = new System.Drawing.Point(10, 6);
            else
                this.descriptionLabel.Location = new System.Drawing.Point(60, 6);
            this.descriptionLabel.Name = "descriptionLabel";
            this.descriptionLabel.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.descriptionLabel.Size = new System.Drawing.Size(430, 47);
            this.descriptionLabel.TabIndex = 0;
            this.descriptionLabel.Text = resources.GetString("descriptionLabel.Text");
            this.descriptionLabel.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            
            // 
            // dividerLabel1
            // 
            this.dividerLabel1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.dividerLabel1.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.dividerLabel1.Location = new System.Drawing.Point(12, 408);
            this.dividerLabel1.Name = "dividerLabel1";
            this.dividerLabel1.Size = new System.Drawing.Size(541, 2);
            this.dividerLabel1.TabIndex = 7;
            // 
            // instancesListView
            // 
            this.instancesListView.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.instancesListView.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.testStatusHeader,
            this.instanceNameHeader,
            this.authenticationModeHeader,
            this.vCenterHeader,
            this.vmNameHeader});
            //this.instancesListView.BackColor = backcolor;
            this.instancesListView.FullRowSelect = true;
            this.instancesListView.HideSelection = false;
            this.instancesListView.Location = new System.Drawing.Point(12, 112);
            this.instancesListView.Name = "instancesListView";
            this.instancesListView.ShowItemToolTips = true;
            this.instancesListView.Size = new System.Drawing.Size(541, 255);
            this.instancesListView.SmallImageList = this.connectionTestStatusImages;
            this.instancesListView.Sorting = System.Windows.Forms.SortOrder.Ascending;
            this.instancesListView.TabIndex = 7;
            this.instancesListView.UseCompatibleStateImageBehavior = false;
            this.instancesListView.View = System.Windows.Forms.View.Details;
            this.instancesListView.ColumnClick += new System.Windows.Forms.ColumnClickEventHandler(this.instancesListView_ColumnClick);
            this.instancesListView.SelectedIndexChanged += new System.EventHandler(this.instancesListView_SelectedIndexChanged);
            this.instancesListView.SizeChanged += new System.EventHandler(this.instancesListView_SizeChanged);
            this.instancesListView.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler(this.instancesListView_MouseDoubleClick);
            this.instancesListView.Resize += new System.EventHandler(this.instancesListView_Resize);
            // 
            // testStatusHeader
            // 
            this.testStatusHeader.Text = "";
            this.testStatusHeader.Width = 25;
            // 
            // instanceNameHeader
            // 
            this.instanceNameHeader.Text = "Instance";
            this.instanceNameHeader.Width = 200;
            // 
            // authenticationModeHeader
            // 
            this.authenticationModeHeader.Text = "Authentication Mode";
            this.authenticationModeHeader.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.authenticationModeHeader.Width = 115;
            // 
            // vCenterHeader
            // 
            this.vCenterHeader.Text = "Virtualization Host";
            this.vCenterHeader.Width = 119;
            // 
            // vmNameHeader
            // 
            this.vmNameHeader.Text = "VM Name";
            this.vmNameHeader.Width = 119;
            // 
            // connectionTestStatusImages
            // 
            this.connectionTestStatusImages.ColorDepth = System.Windows.Forms.ColorDepth.Depth32Bit;
            this.connectionTestStatusImages.ImageSize = new System.Drawing.Size(16, 16);
            this.connectionTestStatusImages.TransparentColor = System.Drawing.Color.Transparent;
            // 
            // addButton
            // 
            this.addButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.addButton.Location = new System.Drawing.Point(12, 373);
            this.addButton.Name = "addButton";
            this.addButton.Size = new System.Drawing.Size(75, 23);
            this.addButton.TabIndex = 0;
            this.addButton.Text = "Add";
            this.addButton.UseVisualStyleBackColor = true;
            this.addButton.Click += new System.EventHandler(this.addButton_Click);
            // 
            // removeButton
            // 
            this.removeButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.removeButton.Enabled = false;
            this.removeButton.Location = new System.Drawing.Point(174, 373);
            this.removeButton.Name = "removeButton";
            this.removeButton.Size = new System.Drawing.Size(75, 23);
            this.removeButton.TabIndex = 2;
            this.removeButton.Text = "Remove";
            this.removeButton.UseVisualStyleBackColor = true;
            this.removeButton.Click += new System.EventHandler(this.removeButton_Click);
            // 
            // testButton
            // 
            this.testButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.testButton.Enabled = false;
            this.testButton.Location = new System.Drawing.Point(478, 373);
            this.testButton.Name = "testButton";
            this.testButton.Size = new System.Drawing.Size(75, 23);
            this.testButton.TabIndex = 3;
            this.testButton.Text = "Test";
            this.testButton.UseVisualStyleBackColor = true;
            this.testButton.Click += new System.EventHandler(this.testButton_Click);
            // 
            // editButton
            // 
            this.editButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.editButton.Enabled = false;
            this.editButton.Location = new System.Drawing.Point(93, 373);
            this.editButton.Name = "editButton";
            this.editButton.Size = new System.Drawing.Size(75, 23);
            this.editButton.TabIndex = 1;
            this.editButton.Text = "Edit";
            this.editButton.UseVisualStyleBackColor = true;
            this.editButton.Click += new System.EventHandler(this.editButton_Click);
            // 
            // applyButton
            // 
            this.applyButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.applyButton.Enabled = false;
            this.applyButton.Location = new System.Drawing.Point(397, 419);
            this.applyButton.Name = "applyButton";
            this.applyButton.Size = new System.Drawing.Size(75, 23);
            this.applyButton.TabIndex = 5;
            this.applyButton.Text = "Apply";
            this.applyButton.UseVisualStyleBackColor = true;
            this.applyButton.Click += new System.EventHandler(this.applyButton_Click);
            // 
            // instanceCountLabel
            // 
            this.instanceCountLabel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.instanceCountLabel.Location = new System.Drawing.Point(397, 92);
            this.instanceCountLabel.Name = "instanceCountLabel";
            this.instanceCountLabel.Size = new System.Drawing.Size(156, 17);
            this.instanceCountLabel.TabIndex = 11;
            this.instanceCountLabel.Text = "{0} instances(s)";
            this.instanceCountLabel.TextAlign = System.Drawing.ContentAlignment.BottomRight;
            // 
            // licenseInformationPictureBox
            // 
            this.licenseInformationPictureBox.Location = new System.Drawing.Point(12, 70);
            this.licenseInformationPictureBox.Name = "licenseInformationPictureBox";
            this.licenseInformationPictureBox.Size = new System.Drawing.Size(16, 16);
            this.licenseInformationPictureBox.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.licenseInformationPictureBox.TabIndex = 12;
            this.licenseInformationPictureBox.TabStop = false;
            // 
            // licenseInformationLabel
            // 
            this.licenseInformationLabel.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.licenseInformationLabel.Location = new System.Drawing.Point(34, 69);
            this.licenseInformationLabel.Name = "licenseInformationLabel";
            this.licenseInformationLabel.Size = new System.Drawing.Size(519, 14);
            this.licenseInformationLabel.TabIndex = 13;
            this.licenseInformationLabel.Text = "SQL Diagnostic Manager is currently licensed to monitor {0} SQL Server instances." +
                "";
            this.licenseInformationLabel.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // gettingStartedLabel
            // 
            this.gettingStartedLabel.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.gettingStartedLabel.BackColor = System.Drawing.Color.White;
            this.gettingStartedLabel.ForeColor = System.Drawing.SystemColors.GrayText;
            this.gettingStartedLabel.Location = new System.Drawing.Point(13, 113);
            this.gettingStartedLabel.Name = "gettingStartedLabel";
            this.gettingStartedLabel.Size = new System.Drawing.Size(537, 253);
            this.gettingStartedLabel.TabIndex = 14;
            this.gettingStartedLabel.Text = "No SQL servers are currently monitored. Click Add to monitor new instances.";
            this.gettingStartedLabel.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // applyChangesBackgroundWorker
            // 
            this.applyChangesBackgroundWorker.DoWork += new System.ComponentModel.DoWorkEventHandler(this.applyChangesBackgroundWorker_DoWork);
            this.applyChangesBackgroundWorker.RunWorkerCompleted += new System.ComponentModel.RunWorkerCompletedEventHandler(this.applyChangesBackgroundWorker_RunWorkerCompleted);
            // 
            // btnVMConfig
            // 
            this.btnVMConfig.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnVMConfig.Location = new System.Drawing.Point(294, 373);
            this.btnVMConfig.Name = "btnVMConfig";
            //this.btnVMConfig.Size = new System.Drawing.Size(97, 23);
            this.btnVMConfig.TabIndex = 15;
            this.btnVMConfig.Text = "VM Configuration";
            this.btnVMConfig.UseVisualStyleBackColor = true;
            this.btnVMConfig.Click += new System.EventHandler(this.btnVMConfig_Click);
            // 
            // statusProgressBar
            // 
            this.statusProgressBar.Color1 = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(135)))), ((int)(((byte)(45)))));
            this.statusProgressBar.Color2 = System.Drawing.Color.White;
            this.statusProgressBar.Dock = System.Windows.Forms.DockStyle.Top;
            this.statusProgressBar.Location = new System.Drawing.Point(0, 60);
            this.statusProgressBar.Name = "statusProgressBar";
            this.statusProgressBar.Size = new System.Drawing.Size(565, 3);
            this.statusProgressBar.Speed = 15D;
            this.statusProgressBar.Step = 5F;
            this.statusProgressBar.TabIndex = 9;
            // 
            // ManageServersDialog
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(565, 454);
            this.Controls.Add(this.instancesListView);
            this.Controls.Add(this.btnVMConfig);
            this.Controls.Add(this.gettingStartedLabel);
            this.Controls.Add(this.licenseInformationLabel);
            this.Controls.Add(this.licenseInformationPictureBox);
            this.Controls.Add(this.instanceCountLabel);
            this.Controls.Add(this.applyButton);
            this.Controls.Add(this.editButton);
            this.Controls.Add(this.statusProgressBar);
            this.Controls.Add(this.testButton);
            this.Controls.Add(this.removeButton);
            this.Controls.Add(this.addButton);
            this.Controls.Add(this.dividerLabel1);
            this.Controls.Add(this.headerPanel);
            this.Controls.Add(this.okButton);
            this.Controls.Add(this.cancelButton);
            this.HelpButton = true;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.MinimumSize = new System.Drawing.Size(480, 400);
            Console.WriteLine("Current color in designer after redraw" + this.backcolor);
         //  this.BackColor = this.backcolor;// System.Drawing.Color.Red;
            this.Name = "ManageServersDialog";
            this.ShowInTaskbar = false;
            this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Show;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Manage Servers";
            this.HelpButtonClicked += new System.ComponentModel.CancelEventHandler(this.ManageServersDialog_HelpButtonClicked);
            this.HelpRequested += new System.Windows.Forms.HelpEventHandler(this.ManageServersDialog_HelpRequested);
            this.headerPanel.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.licenseInformationPictureBox)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomButton cancelButton;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomButton okButton;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomPanel  headerPanel;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomLabel dividerLabel1;
        private CustomListView instancesListView;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomButton addButton;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomButton removeButton;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomButton testButton;
        private Idera.SQLdm.DesktopClient.Controls.InfiniteProgressBar statusProgressBar;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomButton editButton;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomLabel descriptionLabel;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomButton applyButton;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomLabel instanceCountLabel;
        private System.Windows.Forms.PictureBox licenseInformationPictureBox;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomLabel licenseInformationLabel;
        private System.Windows.Forms.ColumnHeader testStatusHeader;
        private System.Windows.Forms.ColumnHeader instanceNameHeader;
        private System.Windows.Forms.ColumnHeader authenticationModeHeader;
        private System.Windows.Forms.ImageList connectionTestStatusImages;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomLabel gettingStartedLabel;
        private System.ComponentModel.BackgroundWorker applyChangesBackgroundWorker;
        private System.Windows.Forms.ColumnHeader vCenterHeader;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomButton btnVMConfig;
        private System.Windows.Forms.ColumnHeader vmNameHeader;


    }
}