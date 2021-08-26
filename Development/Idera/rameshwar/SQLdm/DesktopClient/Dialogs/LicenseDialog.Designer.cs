using System.Diagnostics;
using System.IO;
using System.Reflection;
namespace Idera.SQLdm.DesktopClient.Dialogs {
    partial class LicenseDialog {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing) {
            if (disposing && (components != null)) {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent() {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(LicenseDialog));
            this.listView1 = new System.Windows.Forms.ListView();
            this.columnType = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnServers = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnExpires = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnError = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnKey = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.buttonClose = new System.Windows.Forms.Button();
            this.buttonRemove = new System.Windows.Forms.Button();
            this.textBox1 = new System.Windows.Forms.TextBox();
            this.buttonAdd = new System.Windows.Forms.Button();
            this.labelRepository = new System.Windows.Forms.Label();
            this.labelLicensed = new System.Windows.Forms.Label();
            this.labelMonitored = new System.Windows.Forms.Label();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.buttonRefresh = new System.Windows.Forms.Button();
            this.labelRtvLicInfo = new System.Windows.Forms.Label();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.progressControl = new MRG.Controls.UI.LoadingCircle();
            this.bgWorker = new System.ComponentModel.BackgroundWorker();
            this.linkLabelManageServers = new System.Windows.Forms.LinkLabel();
            this.linkLabelCustomerPortal = new System.Windows.Forms.LinkLabel();
            this.buyNow = new System.Windows.Forms.Button();
            this.linkLabel1 = new System.Windows.Forms.LinkLabel();
            this.groupBox1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.SuspendLayout();
            // 
            // listView1
            // 
            this.listView1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.listView1.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnType,
            this.columnServers,
            this.columnExpires,
            this.columnError,
            this.columnKey});
            this.listView1.FullRowSelect = true;
            this.listView1.HideSelection = false;
            this.listView1.Location = new System.Drawing.Point(9, 19);
            this.listView1.Name = "listView1";
            this.listView1.Size = new System.Drawing.Size(497, 85);
            this.listView1.TabIndex = 0;
            this.listView1.UseCompatibleStateImageBehavior = false;
            this.listView1.View = System.Windows.Forms.View.Details;
            this.listView1.SelectedIndexChanged += new System.EventHandler(this.listView1_SelectedIndexChanged);
            // 
            // columnType
            // 
            this.columnType.Text = "Type";
            this.columnType.Width = 71;
            // 
            // columnServers
            // 
            this.columnServers.Text = "Servers";
            // 
            // columnExpires
            // 
            this.columnExpires.Text = "Expires";
            this.columnExpires.Width = 101;
            // 
            // columnError
            // 
            this.columnError.Text = "Error";
            this.columnError.Width = 39;
            // 
            // columnKey
            // 
            this.columnKey.Text = "Key";
            this.columnKey.Width = 220;
            // 
            // buttonClose
            // 
            this.buttonClose.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonClose.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.buttonClose.Location = new System.Drawing.Point(445, 281);
            this.buttonClose.Name = "buttonClose";
            this.buttonClose.Size = new System.Drawing.Size(75, 23);
            this.buttonClose.TabIndex = 1;
            this.buttonClose.Text = "Close";
            this.buttonClose.UseVisualStyleBackColor = true;
            // 
            // buttonRemove
            // 
            this.buttonRemove.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.buttonRemove.Enabled = false;
            this.buttonRemove.Location = new System.Drawing.Point(9, 112);
            this.buttonRemove.Name = "buttonRemove";
            this.buttonRemove.Size = new System.Drawing.Size(75, 23);
            this.buttonRemove.TabIndex = 2;
            this.buttonRemove.Text = "Remove";
            this.buttonRemove.UseVisualStyleBackColor = true;
            this.buttonRemove.Click += new System.EventHandler(this.buttonRemove_Click);
            // 
            // textBox1
            // 
            this.textBox1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.textBox1.Location = new System.Drawing.Point(9, 48);
            this.textBox1.Name = "textBox1";
            this.textBox1.Size = new System.Drawing.Size(440, 20);
            this.textBox1.TabIndex = 4;
            this.textBox1.TextChanged += new System.EventHandler(this.textBox1_TextChanged);
            // 
            // buttonAdd
            // 
            this.buttonAdd.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonAdd.Enabled = false;
            this.buttonAdd.Location = new System.Drawing.Point(454, 48);
            this.buttonAdd.Name = "buttonAdd";
            this.buttonAdd.Size = new System.Drawing.Size(52, 20);
            this.buttonAdd.TabIndex = 5;
            this.buttonAdd.Text = "Enter";
            this.buttonAdd.UseVisualStyleBackColor = true;
            this.buttonAdd.Click += new System.EventHandler(this.buttonAdd_Click);
            // 
            // labelRepository
            // 
            this.labelRepository.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.labelRepository.Location = new System.Drawing.Point(6, 17);
            this.labelRepository.Name = "labelRepository";
            this.labelRepository.Size = new System.Drawing.Size(500, 26);
            this.labelRepository.TabIndex = 6;
            this.labelRepository.Text = "When requesting new keys for this installation, specify \"{0}\" as the license scop" +
    "e.";
            this.labelRepository.Visible = false;
            // 
            // labelLicensed
            // 
            this.labelLicensed.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.labelLicensed.AutoSize = true;
            this.labelLicensed.Location = new System.Drawing.Point(6, 143);
            this.labelLicensed.Name = "labelLicensed";
            this.labelLicensed.Size = new System.Drawing.Size(139, 13);
            this.labelLicensed.TabIndex = 7;
            this.labelLicensed.Text = "Total licensed servers = {0}.";
            this.labelLicensed.Visible = false;
            // 
            // labelMonitored
            // 
            this.labelMonitored.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.labelMonitored.AutoSize = true;
            this.labelMonitored.Location = new System.Drawing.Point(6, 156);
            this.labelMonitored.Name = "labelMonitored";
            this.labelMonitored.Size = new System.Drawing.Size(163, 13);
            this.labelMonitored.TabIndex = 8;
            this.labelMonitored.Text = "Currently monitored servers = {0}.";
            this.labelMonitored.Visible = false;
            // 
            // groupBox1
            // 
            this.groupBox1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBox1.Controls.Add(this.buttonRefresh);
            this.groupBox1.Controls.Add(this.labelRtvLicInfo);
            this.groupBox1.Controls.Add(this.listView1);
            this.groupBox1.Controls.Add(this.labelMonitored);
            this.groupBox1.Controls.Add(this.labelLicensed);
            this.groupBox1.Controls.Add(this.buttonRemove);
            this.groupBox1.Location = new System.Drawing.Point(8, 11);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(512, 183);
            this.groupBox1.TabIndex = 9;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Current keys";
            // 
            // buttonRefresh
            // 
            this.buttonRefresh.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.buttonRefresh.Location = new System.Drawing.Point(103, 112);
            this.buttonRefresh.Name = "buttonRefresh";
            this.buttonRefresh.Size = new System.Drawing.Size(75, 23);
            this.buttonRefresh.TabIndex = 10;
            this.buttonRefresh.Text = "Refresh";
            this.buttonRefresh.UseVisualStyleBackColor = true;
            this.buttonRefresh.Click += new System.EventHandler(this.buttonRefresh_Click);
            // 
            // labelRtvLicInfo
            // 
            this.labelRtvLicInfo.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.labelRtvLicInfo.AutoSize = true;
            this.labelRtvLicInfo.Location = new System.Drawing.Point(6, 143);
            this.labelRtvLicInfo.Name = "labelRtvLicInfo";
            this.labelRtvLicInfo.Size = new System.Drawing.Size(154, 13);
            this.labelRtvLicInfo.TabIndex = 9;
            this.labelRtvLicInfo.Text = "Retrieving license information...";
            // 
            // groupBox2
            // 
            this.groupBox2.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBox2.Controls.Add(this.labelRepository);
            this.groupBox2.Controls.Add(this.textBox1);
            this.groupBox2.Controls.Add(this.buttonAdd);
            this.groupBox2.Location = new System.Drawing.Point(8, 189);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(512, 80);
            this.groupBox2.TabIndex = 10;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "New key";
            // 
            // progressControl
            // 
            this.progressControl.Active = false;
            this.progressControl.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.progressControl.BackColor = System.Drawing.Color.White;
            this.progressControl.Color = System.Drawing.Color.DarkGray;
            this.progressControl.InnerCircleRadius = 5;
            this.progressControl.Location = new System.Drawing.Point(8, 11);
            this.progressControl.Name = "progressControl";
            this.progressControl.NumberSpoke = 12;
            this.progressControl.OuterCircleRadius = 11;
            this.progressControl.RotationSpeed = 80;
            this.progressControl.Size = new System.Drawing.Size(512, 258);
            this.progressControl.SpokeThickness = 2;
            this.progressControl.StylePreset = MRG.Controls.UI.LoadingCircle.StylePresets.MacOSX;
            this.progressControl.TabIndex = 11;
            // 
            // bgWorker
            // 
            this.bgWorker.DoWork += new System.ComponentModel.DoWorkEventHandler(this.bgWorker_DoWork);
            this.bgWorker.RunWorkerCompleted += new System.ComponentModel.RunWorkerCompletedEventHandler(this.bgWorker_RunWorkerCompleted);
            // 
            // linkLabelManageServers
            // 
            this.linkLabelManageServers.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.linkLabelManageServers.AutoSize = true;
            this.linkLabelManageServers.Location = new System.Drawing.Point(8, 293);
            this.linkLabelManageServers.Name = "linkLabelManageServers";
            this.linkLabelManageServers.Size = new System.Drawing.Size(219, 13);
            this.linkLabelManageServers.TabIndex = 12;
            this.linkLabelManageServers.TabStop = true;
            this.linkLabelManageServers.Text = "Click here to manage monitored SQL Servers";
            this.linkLabelManageServers.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.linkLabelManageServers_LinkClicked);
            // 
            // linkLabelCustomerPortal
            // 
            this.linkLabelCustomerPortal.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.linkLabelCustomerPortal.AutoSize = true;
            this.linkLabelCustomerPortal.Location = new System.Drawing.Point(8, 273);
            this.linkLabelCustomerPortal.Name = "linkLabelCustomerPortal";
            this.linkLabelCustomerPortal.Size = new System.Drawing.Size(117, 13);
            this.linkLabelCustomerPortal.TabIndex = 16;
            this.linkLabelCustomerPortal.TabStop = true;
            this.linkLabelCustomerPortal.Text = "Generate License Keys";
            this.linkLabelCustomerPortal.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.linkLabelCustomerPortal_LinkClicked);
            // 
            // buyNow
            // 
            this.buyNow.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.buyNow.Location = new System.Drawing.Point(364, 281);
            this.buyNow.Name = "buyNow";
            this.buyNow.Size = new System.Drawing.Size(75, 23);
            this.buyNow.TabIndex = 13;
            this.buyNow.Text = "Buy Now";
            this.buyNow.UseVisualStyleBackColor = true;
            this.buyNow.Click += new System.EventHandler(this.buyNow_Click);
            // 
            // linkLabel1
            // 
            this.linkLabel1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.linkLabel1.AutoSize = true;
            this.linkLabel1.Location = new System.Drawing.Point(8, 314);
            this.linkLabel1.Name = "linkLabel1";
            this.linkLabel1.Size = new System.Drawing.Size(108, 13);
            this.linkLabel1.TabIndex = 17;
            this.linkLabel1.TabStop = true;
            this.linkLabel1.Text = "Idera Customer Portal";
            this.linkLabel1.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.linkLabel1_LinkClicked);
            // 
            // LicenseDialog
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(528, 332);
            this.Controls.Add(this.linkLabel1);
            this.Controls.Add(this.buyNow);
            this.Controls.Add(this.linkLabelManageServers);
            this.Controls.Add(this.linkLabelCustomerPortal);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.buttonClose);
            this.Controls.Add(this.progressControl);
            this.HelpButton = true;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.MinimumSize = new System.Drawing.Size(411, 359);
            this.Name = "LicenseDialog";
            this.Padding = new System.Windows.Forms.Padding(8);
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Show;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "License Keys";
            this.HelpButtonClicked += new System.ComponentModel.CancelEventHandler(this.LicenseDialog_HelpButtonClicked);
            this.HelpRequested += new System.Windows.Forms.HelpEventHandler(this.LicenseDialog_HelpRequested);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        //Start : SQL DM 9.0 (Vineet Kumar) (License Changes) -- Adding a link to customer portal on manage license dialogue
        void linkLabelCustomerPortal_LinkClicked(object sender, System.Windows.Forms.LinkLabelLinkClickedEventArgs e)
        {
            // Process.Start(Common.Constants.CustomerPortalLink);
            //[START] SQLdm 10.0 (Rajesh Gupta) : LM 2.0 Integration-Adding a link to License Manager Interface
            string baseDirectory = Path.GetDirectoryName(Assembly.GetExecutingAssembly().GetModules()[0].FullyQualifiedName);
            Process.Start(baseDirectory+Common.Constants.LICENSE_MANAGER_PATH);
            //[END] SQLdm 10.0 (Rajesh Gupta) : LM 2.0 Integration-Adding a link to License Manager Interface
        }
        //End : SQL DM 9.0 (Vineet Kumar) (License Changes) -- Adding a link to customer portal on manage license dialogue

        #endregion

        private System.Windows.Forms.ListView listView1;
        private System.Windows.Forms.ColumnHeader columnType;
        private System.Windows.Forms.ColumnHeader columnServers;
        private System.Windows.Forms.ColumnHeader columnExpires;
        private System.Windows.Forms.Button buttonClose;
        private System.Windows.Forms.Button buttonRemove;
        private System.Windows.Forms.TextBox textBox1;
        private System.Windows.Forms.Button buttonAdd;
        private System.Windows.Forms.Label labelRepository;
        private System.Windows.Forms.Label labelLicensed;
        private System.Windows.Forms.Label labelMonitored;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.ColumnHeader columnError;
        private System.Windows.Forms.ColumnHeader columnKey;
        private MRG.Controls.UI.LoadingCircle progressControl;
        private System.ComponentModel.BackgroundWorker bgWorker;
        private System.Windows.Forms.LinkLabel linkLabelManageServers;
        private System.Windows.Forms.LinkLabel linkLabelCustomerPortal; //SQL DM 9.0 (Vineet Kumar) (License Changes) -- Adding new link for customer portal
        private System.Windows.Forms.Label labelRtvLicInfo;
        private System.Windows.Forms.Button buyNow;
        private System.Windows.Forms.Button buttonRefresh;
        private System.Windows.Forms.LinkLabel linkLabel1;
    }
}