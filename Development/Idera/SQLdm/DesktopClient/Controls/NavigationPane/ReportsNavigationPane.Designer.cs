using Idera.SQLdm.DesktopClient.Properties;

namespace Idera.SQLdm.DesktopClient.Controls.NavigationPane
{
    partial class ReportsNavigationPane
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
            if (disposing)
            {
                ApplicationModel.Default.CustomReports.Changed -= CustomReports_Changed;
                ApplicationController.Default.ReportsViewChanged -= ApplicationController_ReportsViewChanged;
                Settings.Default.ActiveRepositoryConnectionChanged -= Settings_ActiveRepositoryConnectionChanged;

                if (childNodeFont != null) childNodeFont.Dispose();
                if (rootNodeFont  != null) rootNodeFont.Dispose();                

                if(components != null)
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
            this.reportOptionsPanel = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomPanel ();
            this.panel3 = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomPanel ();
            this.helpPanel = new Idera.SQLdm.DesktopClient.Controls.RoundedPanel();
            this.tableLayoutPanel3 = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomTableLayoutPanel();
            this.pictureBox8 = new System.Windows.Forms.PictureBox();
            this.label1 = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomLabel();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.reportAbout = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomLinkLabel();
            this.lblCommunityCentre = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomLinkLabel();
            this.customReportOptionsContainerPanel = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomPanel ();
            this.customReportsPanel = new Idera.SQLdm.DesktopClient.Controls.RoundedPanel();
            this.tableLayoutPanel2 = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomTableLayoutPanel();
            this.pictureBox5 = new System.Windows.Forms.PictureBox();
            this.label3 = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomLabel();
            this.lblDeleteCustomReport = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomLinkLabel();
            this.lblImportCustomReport = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomLinkLabel();
            this.lblExportCustomReport = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomLinkLabel();
            this.lblNewCustomReport = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomLinkLabel();
            this.lblEditCustomReport = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomLinkLabel();
            this.pictureBox7 = new System.Windows.Forms.PictureBox();
            this.pictureBox6 = new System.Windows.Forms.PictureBox();
            this.pictureBoxImport = new System.Windows.Forms.PictureBox();
            this.pictureBox4 = new System.Windows.Forms.PictureBox();
            this.deploymentOptionsContainerPanel = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomPanel ();
            this.deploymentOptionsPanel = new Idera.SQLdm.DesktopClient.Controls.RoundedPanel();
            this.tableLayoutPanel1 = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomTableLayoutPanel();
            this.pictureBox2 = new System.Windows.Forms.PictureBox();
            this.label2 = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomLabel();
            this.scheduleEmail = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomLinkLabel();
            this.reportDeploy = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomLinkLabel();
            this.pictureBox3 = new System.Windows.Forms.PictureBox();
            this.reportsNavigationTreePanel = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomPanel ();
            this.reportsNavigationTreeView = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomTreeView();
            this.reportOptionsPanel.SuspendLayout();
            this.panel3.SuspendLayout();
            this.helpPanel.SuspendLayout();
            this.tableLayoutPanel3.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox8)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            this.customReportOptionsContainerPanel.SuspendLayout();
            this.customReportsPanel.SuspendLayout();
            this.tableLayoutPanel2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox5)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox7)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox6)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxImport)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox4)).BeginInit();
            this.deploymentOptionsContainerPanel.SuspendLayout();
            this.deploymentOptionsPanel.SuspendLayout();
            this.tableLayoutPanel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox2)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox3)).BeginInit();
            this.reportsNavigationTreePanel.SuspendLayout();
            this.SuspendLayout();
            // 
            // reportOptionsPanel
            // 
            this.reportOptionsPanel.AutoScroll = true;
            this.reportOptionsPanel.AutoSize = true;
            this.reportOptionsPanel.Controls.Add(this.panel3);
            this.reportOptionsPanel.Controls.Add(this.customReportOptionsContainerPanel);
            this.reportOptionsPanel.Controls.Add(this.deploymentOptionsContainerPanel);
            this.reportOptionsPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.reportOptionsPanel.Location = new System.Drawing.Point(0, 143);
            this.reportOptionsPanel.Name = "reportOptionsPanel";
            this.reportOptionsPanel.Size = new System.Drawing.Size(357, 462);
            this.reportOptionsPanel.TabIndex = 6;
            this.reportOptionsPanel.Visible = false;
            // 
            // panel3
            // 
            this.panel3.AutoSize = true;
            this.panel3.Controls.Add(this.helpPanel);
            this.panel3.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel3.Location = new System.Drawing.Point(0, 229);
            this.panel3.Name = "panel3";
            this.panel3.Size = new System.Drawing.Size(357, 233);
            this.panel3.TabIndex = 7;
            // 
            // helpPanel
            // 
            this.helpPanel.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.helpPanel.AutoScroll = true;
            this.helpPanel.AutoSize = true;
            this.helpPanel.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(237)))), ((int)(((byte)(235)))), ((int)(((byte)(205)))));
            this.helpPanel.Controls.Add(this.tableLayoutPanel3);
            this.helpPanel.FillColor = System.Drawing.Color.FromArgb(((int)(((byte)(254)))), ((int)(((byte)(253)))), ((int)(((byte)(236)))));
            this.helpPanel.FillColor2 = System.Drawing.Color.Empty;
            this.helpPanel.Location = new System.Drawing.Point(5, 5);
            this.helpPanel.Name = "helpPanel";
            this.helpPanel.Radius = 3F;
            this.helpPanel.Size = new System.Drawing.Size(348, 79);
            this.helpPanel.TabIndex = 5;
            // 
            // tableLayoutPanel3
            // 
            this.tableLayoutPanel3.AutoSize = true;
            this.tableLayoutPanel3.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(254)))), ((int)(((byte)(253)))), ((int)(((byte)(236)))));
            this.tableLayoutPanel3.ColumnCount = 2;
            this.tableLayoutPanel3.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel3.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel3.Controls.Add(this.pictureBox8, 0, 2);
            this.tableLayoutPanel3.Controls.Add(this.label1, 0, 0);
            this.tableLayoutPanel3.Controls.Add(this.pictureBox1, 0, 1);
            this.tableLayoutPanel3.Controls.Add(this.reportAbout, 1, 1);
            this.tableLayoutPanel3.Controls.Add(this.lblCommunityCentre, 1, 2);
            this.tableLayoutPanel3.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel3.Location = new System.Drawing.Point(0, 0);
            this.tableLayoutPanel3.Name = "tableLayoutPanel3";
            this.tableLayoutPanel3.RowCount = 3;
            this.tableLayoutPanel3.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 34F));
            this.tableLayoutPanel3.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 33F));
            this.tableLayoutPanel3.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 33F));
            this.tableLayoutPanel3.Size = new System.Drawing.Size(348, 79);
            this.tableLayoutPanel3.TabIndex = 3;
            // 
            // pictureBox8
            // 
            this.pictureBox8.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.pictureBox8.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(254)))), ((int)(((byte)(253)))), ((int)(((byte)(236)))));
            this.pictureBox8.Image = global::Idera.SQLdm.DesktopClient.Properties.Resources.communitysite;
            this.pictureBox8.Location = new System.Drawing.Point(3, 57);
            this.pictureBox8.Name = "pictureBox8";
            this.pictureBox8.Padding = new System.Windows.Forms.Padding(10, 0, 0, 0);
            this.pictureBox8.Size = new System.Drawing.Size(26, 16);
            this.pictureBox8.SizeMode = System.Windows.Forms.PictureBoxSizeMode.AutoSize;
            this.pictureBox8.TabIndex = 3;
            this.pictureBox8.TabStop = false;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(254)))), ((int)(((byte)(253)))), ((int)(((byte)(236)))));
            this.tableLayoutPanel3.SetColumnSpan(this.label1, 2);
            this.label1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.label1.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(3, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(342, 26);
            this.label1.TabIndex = 0;
            this.label1.Text = "Help Resources";
            // 
            // pictureBox1
            // 
            this.pictureBox1.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.pictureBox1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(254)))), ((int)(((byte)(253)))), ((int)(((byte)(236)))));
            this.pictureBox1.Image = global::Idera.SQLdm.DesktopClient.Properties.Resources.Help_16x16;
            this.pictureBox1.Location = new System.Drawing.Point(3, 31);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Padding = new System.Windows.Forms.Padding(10, 0, 0, 0);
            this.pictureBox1.Size = new System.Drawing.Size(26, 16);
            this.pictureBox1.SizeMode = System.Windows.Forms.PictureBoxSizeMode.AutoSize;
            this.pictureBox1.TabIndex = 1;
            this.pictureBox1.TabStop = false;
            // 
            // reportAbout
            // 
            this.reportAbout.AutoSize = true;
            this.reportAbout.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(254)))), ((int)(((byte)(253)))), ((int)(((byte)(236)))));
            this.reportAbout.Dock = System.Windows.Forms.DockStyle.Fill;
            this.reportAbout.LinkColor = System.Drawing.Color.Black;
            this.reportAbout.Location = new System.Drawing.Point(35, 26);
            this.reportAbout.Name = "reportAbout";
            this.reportAbout.Size = new System.Drawing.Size(310, 26);
            this.reportAbout.TabIndex = 2;
            this.reportAbout.TabStop = true;
            this.reportAbout.Text = "About this Report";
            this.reportAbout.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.reportAbout.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.reportAbout_LinkClicked);
            // 
            // lblCommunityCentre
            // 
            this.lblCommunityCentre.AutoSize = true;
            this.lblCommunityCentre.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(254)))), ((int)(((byte)(253)))), ((int)(((byte)(236)))));
            this.lblCommunityCentre.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lblCommunityCentre.LinkColor = System.Drawing.Color.Black;
            this.lblCommunityCentre.Location = new System.Drawing.Point(35, 52);
            this.lblCommunityCentre.Name = "lblCommunityCentre";
            this.lblCommunityCentre.Size = new System.Drawing.Size(310, 27);
            this.lblCommunityCentre.TabIndex = 2;
            this.lblCommunityCentre.TabStop = true;
            this.lblCommunityCentre.Text = "Visit the IDERA community site\r\n to share custom reports";
            this.lblCommunityCentre.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.lblCommunityCentre.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.lblCommunityCentre_LinkClicked);
            // 
            // customReportOptionsContainerPanel
            // 
            this.customReportOptionsContainerPanel.Controls.Add(this.customReportsPanel);
            this.customReportOptionsContainerPanel.Dock = System.Windows.Forms.DockStyle.Top;
            this.customReportOptionsContainerPanel.Location = new System.Drawing.Point(0, 86);
            this.customReportOptionsContainerPanel.Name = "customReportOptionsContainerPanel";
            this.customReportOptionsContainerPanel.Padding = new System.Windows.Forms.Padding(5);
            this.customReportOptionsContainerPanel.Size = new System.Drawing.Size(357, 143);
            this.customReportOptionsContainerPanel.TabIndex = 7;
            // 
            // customReportsPanel
            // 
            this.customReportsPanel.AutoSize = true;
            this.customReportsPanel.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(237)))), ((int)(((byte)(235)))), ((int)(((byte)(205)))));
            this.customReportsPanel.Controls.Add(this.tableLayoutPanel2);
            this.customReportsPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.customReportsPanel.FillColor = System.Drawing.Color.FromArgb(((int)(((byte)(254)))), ((int)(((byte)(253)))), ((int)(((byte)(236)))));
            this.customReportsPanel.FillColor2 = System.Drawing.Color.Empty;
            this.customReportsPanel.Location = new System.Drawing.Point(5, 5);
            this.customReportsPanel.Name = "customReportsPanel";
            this.customReportsPanel.Radius = 3F;
            this.customReportsPanel.Size = new System.Drawing.Size(347, 133);
            this.customReportsPanel.TabIndex = 4;
            // 
            // tableLayoutPanel2
            // 
            this.tableLayoutPanel2.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(254)))), ((int)(((byte)(253)))), ((int)(((byte)(236)))));
            this.tableLayoutPanel2.ColumnCount = 2;
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel2.Controls.Add(this.pictureBox5, 0, 5);
            this.tableLayoutPanel2.Controls.Add(this.label3, 0, 0);
            this.tableLayoutPanel2.Controls.Add(this.lblDeleteCustomReport, 1, 3);
            this.tableLayoutPanel2.Controls.Add(this.lblImportCustomReport, 1, 4);
            this.tableLayoutPanel2.Controls.Add(this.lblExportCustomReport, 1, 5);
            this.tableLayoutPanel2.Controls.Add(this.lblNewCustomReport, 1, 1);
            this.tableLayoutPanel2.Controls.Add(this.lblEditCustomReport, 1, 2);
            this.tableLayoutPanel2.Controls.Add(this.pictureBox7, 0, 1);
            this.tableLayoutPanel2.Controls.Add(this.pictureBox6, 0, 3);
            this.tableLayoutPanel2.Controls.Add(this.pictureBoxImport, 0, 4);
            this.tableLayoutPanel2.Controls.Add(this.pictureBox4, 0, 2);
            this.tableLayoutPanel2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel2.Location = new System.Drawing.Point(0, 0);
            this.tableLayoutPanel2.Name = "tableLayoutPanel2";
            this.tableLayoutPanel2.RowCount = 6;
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 25F));
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 25F));
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 25F));
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 25F));
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.tableLayoutPanel2.Size = new System.Drawing.Size(347, 133);
            this.tableLayoutPanel2.TabIndex = 10;
            // 
            // pictureBox5
            // 
            this.pictureBox5.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.pictureBox5.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(254)))), ((int)(((byte)(253)))), ((int)(((byte)(236)))));
            this.pictureBox5.Image = global::Idera.SQLdm.DesktopClient.Properties.Resources.arrow_down_green_16x16_plain;
            this.pictureBox5.Location = new System.Drawing.Point(3, 115);
            this.pictureBox5.Name = "pictureBox5";
            this.pictureBox5.Padding = new System.Windows.Forms.Padding(10, 0, 0, 0);
            this.pictureBox5.Size = new System.Drawing.Size(26, 15);
            this.pictureBox5.SizeMode = System.Windows.Forms.PictureBoxSizeMode.AutoSize;
            this.pictureBox5.TabIndex = 10;
            this.pictureBox5.TabStop = false;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(254)))), ((int)(((byte)(253)))), ((int)(((byte)(236)))));
            this.tableLayoutPanel2.SetColumnSpan(this.label3, 2);
            this.label3.Dock = System.Windows.Forms.DockStyle.Fill;
            this.label3.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label3.Location = new System.Drawing.Point(3, 0);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(341, 23);
            this.label3.TabIndex = 1;
            this.label3.Text = "Custom Report Options";
            this.label3.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // lblDeleteCustomReport
            // 
            this.lblDeleteCustomReport.AutoSize = true;
            this.lblDeleteCustomReport.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(254)))), ((int)(((byte)(253)))), ((int)(((byte)(236)))));
            this.lblDeleteCustomReport.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lblDeleteCustomReport.LinkColor = System.Drawing.Color.Black;
            this.lblDeleteCustomReport.Location = new System.Drawing.Point(35, 69);
            this.lblDeleteCustomReport.Name = "lblDeleteCustomReport";
            this.lblDeleteCustomReport.Size = new System.Drawing.Size(309, 23);
            this.lblDeleteCustomReport.TabIndex = 5;
            this.lblDeleteCustomReport.TabStop = true;
            this.lblDeleteCustomReport.Text = "Delete";
            this.lblDeleteCustomReport.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.lblDeleteCustomReport.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.lblDeleteCustomReport_LinkClicked);
            // 
            // lblImportCustomReport
            // 
            this.lblImportCustomReport.AutoSize = true;
            this.lblImportCustomReport.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(254)))), ((int)(((byte)(253)))), ((int)(((byte)(236)))));
            this.lblImportCustomReport.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lblImportCustomReport.LinkColor = System.Drawing.Color.Black;
            this.lblImportCustomReport.Location = new System.Drawing.Point(35, 92);
            this.lblImportCustomReport.Name = "lblImportCustomReport";
            this.lblImportCustomReport.Size = new System.Drawing.Size(309, 20);
            this.lblImportCustomReport.TabIndex = 5;
            this.lblImportCustomReport.TabStop = true;
            this.lblImportCustomReport.Text = "Import";
            this.lblImportCustomReport.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.lblImportCustomReport.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.lblImportCustomReport_LinkClicked);
            // 
            // lblExportCustomReport
            // 
            this.lblExportCustomReport.AutoSize = true;
            this.lblExportCustomReport.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(254)))), ((int)(((byte)(253)))), ((int)(((byte)(236)))));
            this.lblExportCustomReport.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lblExportCustomReport.LinkColor = System.Drawing.Color.Black;
            this.lblExportCustomReport.Location = new System.Drawing.Point(35, 112);
            this.lblExportCustomReport.Name = "lblExportCustomReport";
            this.lblExportCustomReport.Size = new System.Drawing.Size(309, 21);
            this.lblExportCustomReport.TabIndex = 5;
            this.lblExportCustomReport.TabStop = true;
            this.lblExportCustomReport.Text = "Export";
            this.lblExportCustomReport.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.lblExportCustomReport.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.lblExportCustomReport_LinkClicked);
            // 
            // lblNewCustomReport
            // 
            this.lblNewCustomReport.AutoSize = true;
            this.lblNewCustomReport.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(254)))), ((int)(((byte)(253)))), ((int)(((byte)(236)))));
            this.lblNewCustomReport.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lblNewCustomReport.LinkColor = System.Drawing.Color.Black;
            this.lblNewCustomReport.Location = new System.Drawing.Point(35, 23);
            this.lblNewCustomReport.Name = "lblNewCustomReport";
            this.lblNewCustomReport.Size = new System.Drawing.Size(309, 23);
            this.lblNewCustomReport.TabIndex = 8;
            this.lblNewCustomReport.TabStop = true;
            this.lblNewCustomReport.Text = "New";
            this.lblNewCustomReport.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.lblNewCustomReport.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.lblNewCustomReport_LinkClicked);
            // 
            // lblEditCustomReport
            // 
            this.lblEditCustomReport.AutoSize = true;
            this.lblEditCustomReport.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(254)))), ((int)(((byte)(253)))), ((int)(((byte)(236)))));
            this.lblEditCustomReport.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lblEditCustomReport.Enabled = false;
            this.lblEditCustomReport.LinkColor = System.Drawing.Color.Black;
            this.lblEditCustomReport.Location = new System.Drawing.Point(35, 46);
            this.lblEditCustomReport.Name = "lblEditCustomReport";
            this.lblEditCustomReport.Size = new System.Drawing.Size(309, 23);
            this.lblEditCustomReport.TabIndex = 4;
            this.lblEditCustomReport.TabStop = true;
            this.lblEditCustomReport.Text = "Edit";
            this.lblEditCustomReport.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.lblEditCustomReport.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.lblEditCustomReport_LinkClicked);
            // 
            // pictureBox7
            // 
            this.pictureBox7.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.pictureBox7.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(254)))), ((int)(((byte)(253)))), ((int)(((byte)(236)))));
            this.pictureBox7.Image = global::Idera.SQLdm.DesktopClient.Properties.Resources.ChartGridHeader;
            this.pictureBox7.Location = new System.Drawing.Point(3, 26);
            this.pictureBox7.Name = "pictureBox7";
            this.pictureBox7.Padding = new System.Windows.Forms.Padding(10, 0, 0, 0);
            this.pictureBox7.Size = new System.Drawing.Size(26, 16);
            this.pictureBox7.SizeMode = System.Windows.Forms.PictureBoxSizeMode.AutoSize;
            this.pictureBox7.TabIndex = 9;
            this.pictureBox7.TabStop = false;
            // 
            // pictureBox6
            // 
            this.pictureBox6.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.pictureBox6.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(254)))), ((int)(((byte)(253)))), ((int)(((byte)(236)))));
            this.pictureBox6.Image = global::Idera.SQLdm.DesktopClient.Properties.Resources.DeleteSmall;
            this.pictureBox6.Location = new System.Drawing.Point(3, 72);
            this.pictureBox6.Name = "pictureBox6";
            this.pictureBox6.Padding = new System.Windows.Forms.Padding(10, 0, 0, 0);
            this.pictureBox6.Size = new System.Drawing.Size(26, 16);
            this.pictureBox6.SizeMode = System.Windows.Forms.PictureBoxSizeMode.AutoSize;
            this.pictureBox6.TabIndex = 7;
            this.pictureBox6.TabStop = false;
            // 
            // pictureBoxImport
            // 
            this.pictureBoxImport.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.pictureBoxImport.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(254)))), ((int)(((byte)(253)))), ((int)(((byte)(236)))));
            this.pictureBoxImport.Image = global::Idera.SQLdm.DesktopClient.Properties.Resources.arrow_up_green_16x16_plain;
            this.pictureBoxImport.Location = new System.Drawing.Point(3, 95);
            this.pictureBoxImport.Name = "pictureBoxImport";
            this.pictureBoxImport.Padding = new System.Windows.Forms.Padding(10, 0, 0, 0);
            this.pictureBoxImport.Size = new System.Drawing.Size(26, 14);
            this.pictureBoxImport.SizeMode = System.Windows.Forms.PictureBoxSizeMode.AutoSize;
            this.pictureBoxImport.TabIndex = 7;
            this.pictureBoxImport.TabStop = false;
            // 
            // pictureBox4
            // 
            this.pictureBox4.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.pictureBox4.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(254)))), ((int)(((byte)(253)))), ((int)(((byte)(236)))));
            this.pictureBox4.Image = global::Idera.SQLdm.DesktopClient.Properties.Resources.data_edit;
            this.pictureBox4.Location = new System.Drawing.Point(3, 49);
            this.pictureBox4.Name = "pictureBox4";
            this.pictureBox4.Padding = new System.Windows.Forms.Padding(10, 0, 0, 0);
            this.pictureBox4.Size = new System.Drawing.Size(26, 16);
            this.pictureBox4.SizeMode = System.Windows.Forms.PictureBoxSizeMode.AutoSize;
            this.pictureBox4.TabIndex = 6;
            this.pictureBox4.TabStop = false;
            // 
            // deploymentOptionsContainerPanel
            // 
            this.deploymentOptionsContainerPanel.Controls.Add(this.deploymentOptionsPanel);
            this.deploymentOptionsContainerPanel.Dock = System.Windows.Forms.DockStyle.Top;
            this.deploymentOptionsContainerPanel.Location = new System.Drawing.Point(0, 0);
            this.deploymentOptionsContainerPanel.Name = "deploymentOptionsContainerPanel";
            this.deploymentOptionsContainerPanel.Padding = new System.Windows.Forms.Padding(5);
            this.deploymentOptionsContainerPanel.Size = new System.Drawing.Size(357, 86);
            this.deploymentOptionsContainerPanel.TabIndex = 6;
            // 
            // deploymentOptionsPanel
            // 
            this.deploymentOptionsPanel.AutoSize = true;
            this.deploymentOptionsPanel.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(237)))), ((int)(((byte)(235)))), ((int)(((byte)(205)))));
            this.deploymentOptionsPanel.Controls.Add(this.tableLayoutPanel1);
            this.deploymentOptionsPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.deploymentOptionsPanel.FillColor = System.Drawing.Color.FromArgb(((int)(((byte)(254)))), ((int)(((byte)(253)))), ((int)(((byte)(236)))));
            this.deploymentOptionsPanel.FillColor2 = System.Drawing.Color.Empty;
            this.deploymentOptionsPanel.Location = new System.Drawing.Point(5, 5);
            this.deploymentOptionsPanel.Name = "deploymentOptionsPanel";
            this.deploymentOptionsPanel.Radius = 3F;
            this.deploymentOptionsPanel.Size = new System.Drawing.Size(347, 76);
            this.deploymentOptionsPanel.TabIndex = 4;
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(254)))), ((int)(((byte)(253)))), ((int)(((byte)(236)))));
            this.tableLayoutPanel1.ColumnCount = 2;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel1.Controls.Add(this.pictureBox2, 0, 1);
            this.tableLayoutPanel1.Controls.Add(this.label2, 0, 0);
            this.tableLayoutPanel1.Controls.Add(this.scheduleEmail, 1, 2);
            this.tableLayoutPanel1.Controls.Add(this.reportDeploy, 1, 1);
            this.tableLayoutPanel1.Controls.Add(this.pictureBox3, 0, 2);
            this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel1.Location = new System.Drawing.Point(0, 0);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 3;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 33.33333F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 33.33333F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 33.33333F));
            this.tableLayoutPanel1.Size = new System.Drawing.Size(347, 76);
            this.tableLayoutPanel1.TabIndex = 6;
            // 
            // pictureBox2
            // 
            this.pictureBox2.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.pictureBox2.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(254)))), ((int)(((byte)(253)))), ((int)(((byte)(236)))));
            this.pictureBox2.Image = global::Idera.SQLdm.DesktopClient.Properties.Resources.DeployReport_16x16;
            this.pictureBox2.Location = new System.Drawing.Point(3, 29);
            this.pictureBox2.Name = "pictureBox2";
            this.pictureBox2.Padding = new System.Windows.Forms.Padding(10, 0, 0, 0);
            this.pictureBox2.Size = new System.Drawing.Size(26, 16);
            this.pictureBox2.SizeMode = System.Windows.Forms.PictureBoxSizeMode.AutoSize;
            this.pictureBox2.TabIndex = 2;
            this.pictureBox2.TabStop = false;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(254)))), ((int)(((byte)(253)))), ((int)(((byte)(236)))));
            this.tableLayoutPanel1.SetColumnSpan(this.label2, 2);
            this.label2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.label2.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label2.Location = new System.Drawing.Point(3, 0);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(341, 25);
            this.label2.TabIndex = 1;
            this.label2.Text = "Options";
            this.label2.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // scheduleEmail
            // 
            this.scheduleEmail.AutoSize = true;
            this.scheduleEmail.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(254)))), ((int)(((byte)(253)))), ((int)(((byte)(236)))));
            this.scheduleEmail.Dock = System.Windows.Forms.DockStyle.Fill;
            this.scheduleEmail.LinkColor = System.Drawing.Color.Black;
            this.scheduleEmail.Location = new System.Drawing.Point(35, 50);
            this.scheduleEmail.Name = "scheduleEmail";
            this.scheduleEmail.Size = new System.Drawing.Size(309, 26);
            this.scheduleEmail.TabIndex = 5;
            this.scheduleEmail.TabStop = true;
            this.scheduleEmail.Text = "Schedule Email";
            this.scheduleEmail.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.scheduleEmail.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.scheduleEmail_LinkClicked);
            // 
            // reportDeploy
            // 
            this.reportDeploy.AutoSize = true;
            this.reportDeploy.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(254)))), ((int)(((byte)(253)))), ((int)(((byte)(236)))));
            this.reportDeploy.Dock = System.Windows.Forms.DockStyle.Fill;
            this.reportDeploy.LinkColor = System.Drawing.Color.Black;
            this.reportDeploy.Location = new System.Drawing.Point(35, 25);
            this.reportDeploy.Name = "reportDeploy";
            this.reportDeploy.Size = new System.Drawing.Size(309, 25);
            this.reportDeploy.TabIndex = 3;
            this.reportDeploy.TabStop = true;
            this.reportDeploy.Text = "Deploy Report";
            this.reportDeploy.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.reportDeploy.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.reportDeploy_LinkClicked);
            // 
            // pictureBox3
            // 
            this.pictureBox3.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.pictureBox3.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(254)))), ((int)(((byte)(253)))), ((int)(((byte)(236)))));
            this.pictureBox3.Image = global::Idera.SQLdm.DesktopClient.Properties.Resources.Mail_16x16;
            this.pictureBox3.Location = new System.Drawing.Point(3, 55);
            this.pictureBox3.Name = "pictureBox3";
            this.pictureBox3.Padding = new System.Windows.Forms.Padding(10, 0, 0, 0);
            this.pictureBox3.Size = new System.Drawing.Size(26, 16);
            this.pictureBox3.SizeMode = System.Windows.Forms.PictureBoxSizeMode.AutoSize;
            this.pictureBox3.TabIndex = 4;
            this.pictureBox3.TabStop = false;
            // 
            // reportsNavigationTreePanel
            // 
            this.reportsNavigationTreePanel.Controls.Add(this.reportsNavigationTreeView);
            this.reportsNavigationTreePanel.Dock = System.Windows.Forms.DockStyle.Top;
            this.reportsNavigationTreePanel.Location = new System.Drawing.Point(0, 0);
            this.reportsNavigationTreePanel.Name = "reportsNavigationTreePanel";
            this.reportsNavigationTreePanel.Padding = new System.Windows.Forms.Padding(0, 1, 0, 1);
            this.reportsNavigationTreePanel.Size = new System.Drawing.Size(357, 143);
            this.reportsNavigationTreePanel.TabIndex = 0;
            // 
            // reportsNavigationTreeView
            // 
            this.reportsNavigationTreeView.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.reportsNavigationTreeView.Cursor = System.Windows.Forms.Cursors.Hand;
            this.reportsNavigationTreeView.Dock = System.Windows.Forms.DockStyle.Fill;
            this.reportsNavigationTreeView.DrawMode = System.Windows.Forms.TreeViewDrawMode.OwnerDrawAll;
            this.reportsNavigationTreeView.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.reportsNavigationTreeView.FullRowSelect = true;
            this.reportsNavigationTreeView.HideSelection = false;
            this.reportsNavigationTreeView.HotTracking = true;
            this.reportsNavigationTreeView.ItemHeight = 24;
            this.reportsNavigationTreeView.Location = new System.Drawing.Point(0, 1);
            this.reportsNavigationTreeView.Name = "reportsNavigationTreeView";
            this.reportsNavigationTreeView.Scrollable = false;
            this.reportsNavigationTreeView.ShowLines = false;
            this.reportsNavigationTreeView.ShowPlusMinus = false;
            this.reportsNavigationTreeView.ShowRootLines = false;
            this.reportsNavigationTreeView.Size = new System.Drawing.Size(357, 141);
            this.reportsNavigationTreeView.TabIndex = 0;
            this.reportsNavigationTreeView.BeforeCollapse += new System.Windows.Forms.TreeViewCancelEventHandler(this.reportsNavigationTreeView_BeforeCollapse);
            this.reportsNavigationTreeView.AfterCollapse += new System.Windows.Forms.TreeViewEventHandler(this.reportsNavigationTreeView_AfterCollapse);
            this.reportsNavigationTreeView.AfterExpand += new System.Windows.Forms.TreeViewEventHandler(this.reportsNavigationTreeView_AfterExpand);
            this.reportsNavigationTreeView.DrawNode += new System.Windows.Forms.DrawTreeNodeEventHandler(this.reportsNavigationTreeView_DrawNode);
            this.reportsNavigationTreeView.BeforeSelect += new System.Windows.Forms.TreeViewCancelEventHandler(this.reportsNavigationTreeView_BeforeSelect);
            this.reportsNavigationTreeView.AfterSelect += new System.Windows.Forms.TreeViewEventHandler(this.reportsNavigationTreeView_AfterSelect);
            this.reportsNavigationTreeView.NodeMouseClick += new System.Windows.Forms.TreeNodeMouseClickEventHandler(this.reportsNavigationTreeView_NodeMouseClick);
            // 
            // ReportsNavigationPane
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoScroll = true;
            this.AutoSize = true;
            this.BackColor = System.Drawing.Color.White;
            this.Controls.Add(this.reportOptionsPanel);
            this.Controls.Add(this.reportsNavigationTreePanel);
            this.Name = "ReportsNavigationPane";
            this.Size = new System.Drawing.Size(357, 605);
            this.reportOptionsPanel.ResumeLayout(false);
            this.reportOptionsPanel.PerformLayout();
            this.panel3.ResumeLayout(false);
            this.panel3.PerformLayout();
            this.helpPanel.ResumeLayout(false);
            this.helpPanel.PerformLayout();
            this.tableLayoutPanel3.ResumeLayout(false);
            this.tableLayoutPanel3.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox8)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            this.customReportOptionsContainerPanel.ResumeLayout(false);
            this.customReportOptionsContainerPanel.PerformLayout();
            this.customReportsPanel.ResumeLayout(false);
            this.tableLayoutPanel2.ResumeLayout(false);
            this.tableLayoutPanel2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox5)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox7)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox6)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxImport)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox4)).EndInit();
            this.deploymentOptionsContainerPanel.ResumeLayout(false);
            this.deploymentOptionsContainerPanel.PerformLayout();
            this.deploymentOptionsPanel.ResumeLayout(false);
            this.tableLayoutPanel1.ResumeLayout(false);
            this.tableLayoutPanel1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox2)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox3)).EndInit();
            this.reportsNavigationTreePanel.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }



        #endregion

        private RoundedPanel deploymentOptionsPanel;
        private RoundedPanel helpPanel;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomLinkLabel reportAbout;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomLinkLabel lblCommunityCentre;
        private System.Windows.Forms.PictureBox pictureBox1;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomLabel label1;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomLabel label2;
        private System.Windows.Forms.PictureBox pictureBox2;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomLinkLabel reportDeploy;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomPanel  reportOptionsPanel;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomLinkLabel scheduleEmail;
        private System.Windows.Forms.PictureBox pictureBox3;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomPanel  reportsNavigationTreePanel;
        private System.Windows.Forms.TreeView reportsNavigationTreeView;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomPanel  panel3;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomPanel  deploymentOptionsContainerPanel;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomPanel  customReportOptionsContainerPanel;
        private RoundedPanel customReportsPanel;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomLabel label3;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomLinkLabel lblDeleteCustomReport;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomLinkLabel lblImportCustomReport;//SQLdm 9.1 (Vineet Kumar)  (Community Integration) -- Adding import/export icons
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomLinkLabel lblExportCustomReport;//SQLdm 9.1 (Vineet Kumar)  (Community Integration) -- Adding import/export icons
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomLinkLabel lblEditCustomReport;
        private System.Windows.Forms.PictureBox pictureBox6;
        private System.Windows.Forms.PictureBox pictureBoxImport;
        private System.Windows.Forms.PictureBox pictureBox4;
        private System.Windows.Forms.PictureBox pictureBox7;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomLinkLabel lblNewCustomReport;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomTableLayoutPanel tableLayoutPanel1;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomTableLayoutPanel tableLayoutPanel2;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomTableLayoutPanel tableLayoutPanel3;
        private System.Windows.Forms.PictureBox pictureBox5;
        private System.Windows.Forms.PictureBox pictureBox8;
    }
}
