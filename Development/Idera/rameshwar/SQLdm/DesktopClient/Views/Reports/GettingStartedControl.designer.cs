using System.Windows.Forms;
using Idera.SQLdm.DesktopClient.Controls;

namespace Idera.SQLdm.DesktopClient.Views.Reports
{
    partial class GettingStartedControl
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
                if (analyzeLabelNormalFont != null) analyzeLabelNormalFont.Dispose();
                if (analyzeLabelMouseEnterFont != null) analyzeLabelMouseEnterFont.Dispose();

                if (components != null)
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
            this.panel2 = new System.Windows.Forms.Panel();
            this.label2 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.headerLabel = new System.Windows.Forms.Label();
            this.panel6 = new System.Windows.Forms.Panel();
            this.deployPanel = new Idera.SQLdm.DesktopClient.Controls.RoundedPanel();
            this.deployReportsLinkLabel = new System.Windows.Forms.LinkLabel();
            this.label3 = new System.Windows.Forms.Label();
            this.monitorPanel = new Idera.SQLdm.DesktopClient.Controls.RoundedPanel();
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.panel1 = new System.Windows.Forms.Panel();
            this.monitorSqldmActivityButtonPanel = new Idera.SQLdm.DesktopClient.Controls.RoundedPanel();
            this.sqldmActivityLabel = new System.Windows.Forms.Label();
            this.monitorVirtualizationButtonPanel = new Idera.SQLdm.DesktopClient.Controls.RoundedPanel();
            this.monitorVirtualizationLabel = new System.Windows.Forms.Label();
            this.monitorServersButtonPanel = new Idera.SQLdm.DesktopClient.Controls.RoundedPanel();
            this.monitorServersLabel = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.panel4 = new System.Windows.Forms.Panel();
            this.monitorServerReportsPanel = new System.Windows.Forms.TableLayoutPanel();
            this.alwaysOnTopologyButton = new Idera.SQLdm.DesktopClient.Controls.ReportSelectionButton();
            //SQLDM-28817
            this.deadLockReportButton = new Idera.SQLdm.DesktopClient.Controls.ReportSelectionButton();
            //SQLDM-28817
            this.detailedSessionReport = new Idera.SQLdm.DesktopClient.Controls.ReportSelectionButton();
            this.serverSummaryButton = new Idera.SQLdm.DesktopClient.Controls.ReportSelectionButton();
            this.enterpriseSummaryButton = new Idera.SQLdm.DesktopClient.Controls.ReportSelectionButton();
            this.activeAlertsButton = new Idera.SQLdm.DesktopClient.Controls.ReportSelectionButton();
            this.mirroringSummaryButton = new Idera.SQLdm.DesktopClient.Controls.ReportSelectionButton();
            this.metricThresholdButton = new Idera.SQLdm.DesktopClient.Controls.ReportSelectionButton();
            this.alertTemplatesButton = new Idera.SQLdm.DesktopClient.Controls.ReportSelectionButton();
            this.alertThresholdButton = new Idera.SQLdm.DesktopClient.Controls.ReportSelectionButton();
            this.templateComparisonButton = new Idera.SQLdm.DesktopClient.Controls.ReportSelectionButton();
            this.monitorVirtualizationReportsPanel = new System.Windows.Forms.TableLayoutPanel();
            this.virtualizationSummaryButton = new Idera.SQLdm.DesktopClient.Controls.ReportSelectionButton();
            this.vStatisticsReportButton = new Idera.SQLdm.DesktopClient.Controls.ReportSelectionButton();
            this.monitorSqldmActivityReportsPanel = new System.Windows.Forms.TableLayoutPanel();
            this.sqldmActivityReportButton = new Idera.SQLdm.DesktopClient.Controls.ReportSelectionButton();
            this.roundedPanel6 = new Idera.SQLdm.DesktopClient.Controls.RoundedPanel();
            this.label8 = new System.Windows.Forms.Label();
            this.discoverPanel = new Idera.SQLdm.DesktopClient.Controls.RoundedPanel();
            this.tableLayoutPanel3 = new System.Windows.Forms.TableLayoutPanel();
            this.panel3 = new System.Windows.Forms.Panel();
            this.analyzeDatabasesReportsPanel = new System.Windows.Forms.TableLayoutPanel();
            this.databaseAppsButton = new Idera.SQLdm.DesktopClient.Controls.ReportSelectionButton();
            this.topTablesGrowthButton = new Idera.SQLdm.DesktopClient.Controls.ReportSelectionButton();
            this.topDatabasesButton = new Idera.SQLdm.DesktopClient.Controls.ReportSelectionButton();
            this.topTablesFragmentedButton = new Idera.SQLdm.DesktopClient.Controls.ReportSelectionButton();
            this.databaseStatsButton = new Idera.SQLdm.DesktopClient.Controls.ReportSelectionButton();
            this.mirroringHistoryButton = new Idera.SQLdm.DesktopClient.Controls.ReportSelectionButton();
            this.tempdbButton = new Idera.SQLdm.DesktopClient.Controls.ReportSelectionButton();
            this.transactionLogStatisticsButton = new Idera.SQLdm.DesktopClient.Controls.ReportSelectionButton();
            this.alwaysOnStatisticsButton = new Idera.SQLdm.DesktopClient.Controls.ReportSelectionButton();
            this.analyzeServersReportsPanel = new System.Windows.Forms.TableLayoutPanel();
            this.topServersButton = new Idera.SQLdm.DesktopClient.Controls.ReportSelectionButton();
            this.serverInventoryButton = new Idera.SQLdm.DesktopClient.Controls.ReportSelectionButton();
            this.serverStatisticsButton = new Idera.SQLdm.DesktopClient.Controls.ReportSelectionButton();
            this.topQueriesButton = new Idera.SQLdm.DesktopClient.Controls.ReportSelectionButton();
            this.alertSummaryButton = new Idera.SQLdm.DesktopClient.Controls.ReportSelectionButton();
            this.serverUptimeButton = new Idera.SQLdm.DesktopClient.Controls.ReportSelectionButton();
            this.queryOverviewButton = new Idera.SQLdm.DesktopClient.Controls.ReportSelectionButton();
            this.baselineStatisticsButton = new Idera.SQLdm.DesktopClient.Controls.ReportSelectionButton();
            //Start: SQLdm 8.6 (Vineet Kumar): Added for Query Wait Stats Report
            this.queryWaitStatisticsButton = new Idera.SQLdm.DesktopClient.Controls.ReportSelectionButton();
            //End: SQLdm 8.6 (Vineet Kumar): Added for Query Wait Stats Report
            this.analyzeResourcesReportsPanel = new System.Windows.Forms.TableLayoutPanel();
            this.replicationStatisticsButton = new Idera.SQLdm.DesktopClient.Controls.ReportSelectionButton();
            this.diskStatisticsButton = new Idera.SQLdm.DesktopClient.Controls.ReportSelectionButton();
            this.cpuStatisticsButton = new Idera.SQLdm.DesktopClient.Controls.ReportSelectionButton();
            this.sessionStatisticsButton = new Idera.SQLdm.DesktopClient.Controls.ReportSelectionButton();
            this.memoryStatisticsButton = new Idera.SQLdm.DesktopClient.Controls.ReportSelectionButton();
            this.diskSpaceUsageButton = new Idera.SQLdm.DesktopClient.Controls.ReportSelectionButton();
            this.diskSpaceHistoryButton = new Idera.SQLdm.DesktopClient.Controls.ReportSelectionButton();
            this.diskDetailsButton = new Idera.SQLdm.DesktopClient.Controls.ReportSelectionButton();
            this.panel5 = new System.Windows.Forms.Panel();
            this.analyzeResourcesButtonPanel = new Idera.SQLdm.DesktopClient.Controls.RoundedPanel();
            this.analyzeResourcesLabel = new System.Windows.Forms.Label();
            this.analyzeDatabasesButtonPanel = new Idera.SQLdm.DesktopClient.Controls.RoundedPanel();
            this.analyzeDatabasesLabel = new System.Windows.Forms.Label();
            this.analyzeServersButtonPanel = new Idera.SQLdm.DesktopClient.Controls.RoundedPanel();
            this.analyzeServersLabel = new System.Windows.Forms.Label();
            this.label7 = new System.Windows.Forms.Label();
            this.roundedPanel5 = new Idera.SQLdm.DesktopClient.Controls.RoundedPanel();
            this.label5 = new System.Windows.Forms.Label();
            this.planPanel = new Idera.SQLdm.DesktopClient.Controls.RoundedPanel();
            this.tableLayoutPanel2 = new System.Windows.Forms.TableLayoutPanel();
            this.tableGrowthForecastButton = new Idera.SQLdm.DesktopClient.Controls.ReportSelectionButton();
            this.diskSpaceForecastButton = new Idera.SQLdm.DesktopClient.Controls.ReportSelectionButton();
            this.databaseGrowthForecastButton = new Idera.SQLdm.DesktopClient.Controls.ReportSelectionButton();
            this.roundedPanel2 = new Idera.SQLdm.DesktopClient.Controls.RoundedPanel();
            this.label6 = new System.Windows.Forms.Label();
            this.headerPanel = new System.Windows.Forms.Panel();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.panel2.SuspendLayout();
            this.panel6.SuspendLayout();
            this.deployPanel.SuspendLayout();
            this.monitorPanel.SuspendLayout();
            this.tableLayoutPanel1.SuspendLayout();
            this.panel1.SuspendLayout();
            this.monitorSqldmActivityButtonPanel.SuspendLayout();
            this.monitorVirtualizationButtonPanel.SuspendLayout();
            this.monitorServersButtonPanel.SuspendLayout();
            this.panel4.SuspendLayout();
            this.monitorServerReportsPanel.SuspendLayout();
            this.monitorVirtualizationReportsPanel.SuspendLayout();
            this.monitorSqldmActivityReportsPanel.SuspendLayout();
            this.roundedPanel6.SuspendLayout();
            this.discoverPanel.SuspendLayout();
            this.tableLayoutPanel3.SuspendLayout();
            this.panel3.SuspendLayout();
            this.analyzeDatabasesReportsPanel.SuspendLayout();
            this.analyzeServersReportsPanel.SuspendLayout();
            this.analyzeResourcesReportsPanel.SuspendLayout();
            this.panel5.SuspendLayout();
            this.analyzeResourcesButtonPanel.SuspendLayout();
            this.analyzeDatabasesButtonPanel.SuspendLayout();
            this.analyzeServersButtonPanel.SuspendLayout();
            this.roundedPanel5.SuspendLayout();
            this.planPanel.SuspendLayout();
            this.tableLayoutPanel2.SuspendLayout();
            this.roundedPanel2.SuspendLayout();
            this.headerPanel.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            this.SuspendLayout();
            // 
            // panel2
            //                                           FromArgb(((int)(((byte)(236)))), ((int)(((byte)(235)))), ((int)(((byte)(234)))));
            this.panel2.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(236)))), ((int)(((byte)(235)))), ((int)(((byte)(234)))));
            this.panel2.Controls.Add(this.label2);
            this.panel2.Controls.Add(this.label1);
            this.panel2.Dock = System.Windows.Forms.DockStyle.Top;
            this.panel2.Location = new System.Drawing.Point(0, 78);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(970, 60);
            this.panel2.TabIndex = 1;
            // 
            // label2
            // 
            this.label2.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.label2.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(236)))), ((int)(((byte)(235)))), ((int)(((byte)(234)))));
            this.label2.Font = new System.Drawing.Font("Verdana", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label2.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(119)))), ((int)(((byte)(111)))), ((int)(((byte)(101)))));
            this.label2.Location = new System.Drawing.Point(143, 29);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(687, 37);
            this.label2.TabIndex = 1;
            this.label2.Text = "SQL Diagnostic Manager Reports help you monitor the SQL Servers in your organizat" +
    "ion, analyze vital objects and trends, and plan for the future.";
            // 
            // label1
            // 
            this.label1.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.label1.AutoSize = true;
            this.label1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(236)))), ((int)(((byte)(235)))), ((int)(((byte)(234)))));
            this.label1.Font = new System.Drawing.Font("Verdana", 15.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(119)))), ((int)(((byte)(111)))), ((int)(((byte)(101)))));
            this.label1.Location = new System.Drawing.Point(140, 2);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(257, 25);
            this.label1.TabIndex = 0;
            this.label1.Text = "Monitor. Analyze. Plan.";
            // 
            // headerLabel
            // 
            this.headerLabel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.headerLabel.BackColor = System.Drawing.Color.Transparent;
            this.headerLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 15.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.headerLabel.ForeColor = System.Drawing.Color.White;
            this.headerLabel.ImageAlign = System.Drawing.ContentAlignment.TopLeft;
            this.headerLabel.Location = new System.Drawing.Point(591, 40);
            this.headerLabel.Name = "headerLabel";
            this.headerLabel.Size = new System.Drawing.Size(375, 37);
            this.headerLabel.TabIndex = 0;
            this.headerLabel.Text = "Getting Started with Reports";
            this.headerLabel.TextAlign = System.Drawing.ContentAlignment.BottomRight;
            // 
            // panel6
            // 
            this.panel6.AutoScroll = true;
            this.panel6.BackColor = System.Drawing.Color.White;
            this.panel6.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.panel6.Controls.Add(this.deployPanel);
            this.panel6.Controls.Add(this.monitorPanel);
            this.panel6.Controls.Add(this.discoverPanel);
            this.panel6.Controls.Add(this.planPanel);
            this.panel6.Dock = System.Windows.Forms.DockStyle.Top;
            this.panel6.Location = new System.Drawing.Point(0, 160);
            this.panel6.Name = "panel6";
            this.panel6.Size = new System.Drawing.Size(970, 531);
            this.panel6.TabIndex = 2;
            // 
            // deployPanel
            // 
            this.deployPanel.Anchor = System.Windows.Forms.AnchorStyles.Top;
            this.deployPanel.BorderColor = System.Drawing.Color.Gray;
            this.deployPanel.Controls.Add(this.deployReportsLinkLabel);
            this.deployPanel.Controls.Add(this.label3);
            this.deployPanel.FillColor = System.Drawing.Color.FromArgb(((int)(((byte)(228)))), ((int)(((byte)(228)))), ((int)(((byte)(228)))));
            this.deployPanel.FillColor2 = System.Drawing.Color.Empty;
            this.deployPanel.Location = new System.Drawing.Point(238, 488);
            this.deployPanel.Name = "deployPanel";
            this.deployPanel.Radius = 3F;
            this.deployPanel.Size = new System.Drawing.Size(503, 35);
            this.deployPanel.TabIndex = 17;
            // 
            // deployReportsLinkLabel
            // 
            this.deployReportsLinkLabel.AutoSize = true;
            this.deployReportsLinkLabel.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(228)))), ((int)(((byte)(228)))), ((int)(((byte)(228)))));
            this.deployReportsLinkLabel.Font = new System.Drawing.Font("Arial", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.deployReportsLinkLabel.LinkBehavior = System.Windows.Forms.LinkBehavior.HoverUnderline;
            this.deployReportsLinkLabel.LinkColor = System.Drawing.Color.Blue;
            this.deployReportsLinkLabel.Location = new System.Drawing.Point(364, 9);
            this.deployReportsLinkLabel.Name = "deployReportsLinkLabel";
            this.deployReportsLinkLabel.Size = new System.Drawing.Size(119, 16);
            this.deployReportsLinkLabel.TabIndex = 1;
            this.deployReportsLinkLabel.TabStop = true;
            this.deployReportsLinkLabel.Text = "Deploy them now";
            this.deployReportsLinkLabel.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.deployReportsLinkLabel_LinkClicked);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(228)))), ((int)(((byte)(228)))), ((int)(((byte)(228)))));
            this.label3.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label3.Location = new System.Drawing.Point(19, 10);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(346, 15);
            this.label3.TabIndex = 0;
            this.label3.Text = "Want to host these reports on your Reporting Services Server?";
            // 
            // monitorPanel
            // 
            this.monitorPanel.Anchor = System.Windows.Forms.AnchorStyles.Top;
            this.monitorPanel.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(215)))), ((int)(((byte)(215)))), ((int)(((byte)(215)))));
            this.monitorPanel.Controls.Add(this.tableLayoutPanel1);
            this.monitorPanel.Controls.Add(this.roundedPanel6);
            this.monitorPanel.FillColor = System.Drawing.Color.White;
            this.monitorPanel.FillColor2 = System.Drawing.Color.Empty;
            this.monitorPanel.Location = new System.Drawing.Point(89, 15);
            this.monitorPanel.Name = "monitorPanel";
            this.monitorPanel.Radius = 3F;
            this.monitorPanel.Size = new System.Drawing.Size(260, 467);
            this.monitorPanel.TabIndex = 14;
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.ColumnCount = 1;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel1.Controls.Add(this.panel1, 0, 0);
            this.tableLayoutPanel1.Controls.Add(this.panel4, 0, 1);
            this.tableLayoutPanel1.Location = new System.Drawing.Point(3, 41);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 2;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 25F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.tableLayoutPanel1.Size = new System.Drawing.Size(254, 383);
            this.tableLayoutPanel1.TabIndex = 6;
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.monitorSqldmActivityButtonPanel);
            this.panel1.Controls.Add(this.monitorVirtualizationButtonPanel);
            this.panel1.Controls.Add(this.monitorServersButtonPanel);
            this.panel1.Controls.Add(this.label4);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel1.Location = new System.Drawing.Point(0, 0);
            this.panel1.Margin = new System.Windows.Forms.Padding(0);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(254, 25);
            this.panel1.TabIndex = 0;
            // 
            // monitorSqldmActivityButtonPanel
            // 
            this.monitorSqldmActivityButtonPanel.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.monitorSqldmActivityButtonPanel.BackColor = System.Drawing.Color.White;
            this.monitorSqldmActivityButtonPanel.BorderColor = System.Drawing.Color.White;
            this.monitorSqldmActivityButtonPanel.Controls.Add(this.sqldmActivityLabel);
            this.monitorSqldmActivityButtonPanel.FillColor = System.Drawing.Color.White;
            this.monitorSqldmActivityButtonPanel.FillColor2 = System.Drawing.Color.Empty;
            this.monitorSqldmActivityButtonPanel.Location = new System.Drawing.Point(164, 0);
            this.monitorSqldmActivityButtonPanel.Name = "monitorSqldmActivityButtonPanel";
            this.monitorSqldmActivityButtonPanel.Radius = 3F;
            this.monitorSqldmActivityButtonPanel.Size = new System.Drawing.Size(65, 23);
            this.monitorSqldmActivityButtonPanel.TabIndex = 7;
            // 
            // sqldmActivityLabel
            // 
            this.sqldmActivityLabel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.sqldmActivityLabel.AutoSize = true;
            this.sqldmActivityLabel.BackColor = System.Drawing.Color.Transparent;
            this.sqldmActivityLabel.Cursor = System.Windows.Forms.Cursors.Hand;
            this.sqldmActivityLabel.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.sqldmActivityLabel.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(75)))), ((int)(((byte)(75)))), ((int)(((byte)(75)))));
            this.sqldmActivityLabel.Location = new System.Drawing.Point(8, 4);
            this.sqldmActivityLabel.Name = "sqldmActivityLabel";
            this.sqldmActivityLabel.Size = new System.Drawing.Size(48, 15);
            this.sqldmActivityLabel.TabIndex = 6;
            this.sqldmActivityLabel.Text = "Activity";
            this.sqldmActivityLabel.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.sqldmActivityLabel.MouseClick += new System.Windows.Forms.MouseEventHandler(this.monitorSqlActivityLabel_MouseClick);
            this.sqldmActivityLabel.MouseEnter += new System.EventHandler(this.analyzeLabel_MouseEnter);
            this.sqldmActivityLabel.MouseLeave += new System.EventHandler(this.analyzeLabel_MouseLeave);
            // 
            // monitorVirtualizationButtonPanel
            // 
            this.monitorVirtualizationButtonPanel.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.monitorVirtualizationButtonPanel.BackColor = System.Drawing.Color.White;
            this.monitorVirtualizationButtonPanel.BorderColor = System.Drawing.Color.White;
            this.monitorVirtualizationButtonPanel.Controls.Add(this.monitorVirtualizationLabel);
            this.monitorVirtualizationButtonPanel.FillColor = System.Drawing.Color.White;
            this.monitorVirtualizationButtonPanel.FillColor2 = System.Drawing.Color.Empty;
            this.monitorVirtualizationButtonPanel.Location = new System.Drawing.Point(71, 0);
            this.monitorVirtualizationButtonPanel.Name = "monitorVirtualizationButtonPanel";
            this.monitorVirtualizationButtonPanel.Radius = 3F;
            this.monitorVirtualizationButtonPanel.Size = new System.Drawing.Size(87, 23);
            this.monitorVirtualizationButtonPanel.TabIndex = 7;
            // 
            // monitorVirtualizationLabel
            // 
            this.monitorVirtualizationLabel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.monitorVirtualizationLabel.AutoSize = true;
            this.monitorVirtualizationLabel.BackColor = System.Drawing.Color.Transparent;
            this.monitorVirtualizationLabel.Cursor = System.Windows.Forms.Cursors.Hand;
            this.monitorVirtualizationLabel.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.monitorVirtualizationLabel.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(75)))), ((int)(((byte)(75)))), ((int)(((byte)(75)))));
            this.monitorVirtualizationLabel.Location = new System.Drawing.Point(5, 4);
            this.monitorVirtualizationLabel.Name = "monitorVirtualizationLabel";
            this.monitorVirtualizationLabel.Size = new System.Drawing.Size(81, 15);
            this.monitorVirtualizationLabel.TabIndex = 6;
            this.monitorVirtualizationLabel.Text = "Virtualization";
            this.monitorVirtualizationLabel.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.monitorVirtualizationLabel.MouseClick += new System.Windows.Forms.MouseEventHandler(this.monitorVirtualizationLabel_MouseClick);
            this.monitorVirtualizationLabel.MouseEnter += new System.EventHandler(this.analyzeLabel_MouseEnter);
            this.monitorVirtualizationLabel.MouseLeave += new System.EventHandler(this.analyzeLabel_MouseLeave);
            // 
            // monitorServersButtonPanel
            // 
            this.monitorServersButtonPanel.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.monitorServersButtonPanel.BackColor = System.Drawing.Color.White;
            this.monitorServersButtonPanel.BorderColor = System.Drawing.Color.White;
            this.monitorServersButtonPanel.Controls.Add(this.monitorServersLabel);
            this.monitorServersButtonPanel.FillColor = System.Drawing.Color.FromArgb(((int)(((byte)(228)))), ((int)(((byte)(228)))), ((int)(((byte)(228)))));
            this.monitorServersButtonPanel.FillColor2 = System.Drawing.Color.Empty;
            this.monitorServersButtonPanel.Location = new System.Drawing.Point(7, 0);
            this.monitorServersButtonPanel.Name = "monitorServersButtonPanel";
            this.monitorServersButtonPanel.Radius = 3F;
            this.monitorServersButtonPanel.Size = new System.Drawing.Size(58, 23);
            this.monitorServersButtonPanel.TabIndex = 7;
            // 
            // monitorServersLabel
            // 
            this.monitorServersLabel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.monitorServersLabel.AutoSize = true;
            this.monitorServersLabel.BackColor = System.Drawing.Color.Transparent;
            this.monitorServersLabel.Cursor = System.Windows.Forms.Cursors.Hand;
            this.monitorServersLabel.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.monitorServersLabel.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(75)))), ((int)(((byte)(75)))), ((int)(((byte)(75)))));
            this.monitorServersLabel.Location = new System.Drawing.Point(3, 4);
            this.monitorServersLabel.Name = "monitorServersLabel";
            this.monitorServersLabel.Size = new System.Drawing.Size(52, 15);
            this.monitorServersLabel.TabIndex = 6;
            this.monitorServersLabel.Text = "Servers";
            this.monitorServersLabel.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.monitorServersLabel.MouseClick += new System.Windows.Forms.MouseEventHandler(this.monitorServersLabel_MouseClick);
            this.monitorServersLabel.MouseEnter += new System.EventHandler(this.analyzeLabel_MouseEnter);
            this.monitorServersLabel.MouseLeave += new System.EventHandler(this.analyzeLabel_MouseLeave);
            // 
            // label4
            // 
            this.label4.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(215)))), ((int)(((byte)(215)))), ((int)(((byte)(215)))));
            this.label4.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.label4.Location = new System.Drawing.Point(0, 24);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(254, 1);
            this.label4.TabIndex = 4;
            // 
            // panel4
            // 
            this.panel4.Controls.Add(this.monitorServerReportsPanel);
            this.panel4.Controls.Add(this.monitorVirtualizationReportsPanel);
            this.panel4.Controls.Add(this.monitorSqldmActivityReportsPanel);
            this.panel4.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel4.Location = new System.Drawing.Point(0, 25);
            this.panel4.Margin = new System.Windows.Forms.Padding(0);
            this.panel4.Name = "panel4";
            this.panel4.Size = new System.Drawing.Size(254, 480);
            this.panel4.TabIndex = 1;
            // 
            // monitorServerReportsPanel
            // 
            this.monitorServerReportsPanel.AutoScroll = true;
            this.monitorServerReportsPanel.ColumnCount = 1;
            this.monitorServerReportsPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.monitorServerReportsPanel.Controls.Add(this.alwaysOnTopologyButton, 0, 5);
            //SQLDM-28817
            this.monitorServerReportsPanel.Controls.Add(this.deadLockReportButton, 0, 9);
            this.monitorServerReportsPanel.Controls.Add(this.serverSummaryButton, 0, 1);
            this.monitorServerReportsPanel.Controls.Add(this.enterpriseSummaryButton, 0, 0);
            this.monitorServerReportsPanel.Controls.Add(this.activeAlertsButton, 0, 2);
            this.monitorServerReportsPanel.Controls.Add(this.mirroringSummaryButton, 0, 3);
            this.monitorServerReportsPanel.Controls.Add(this.metricThresholdButton, 0, 4);
            this.monitorServerReportsPanel.Controls.Add(this.alertTemplatesButton, 0, 6);
            this.monitorServerReportsPanel.Controls.Add(this.alertThresholdButton, 0, 7);
            this.monitorServerReportsPanel.Controls.Add(this.templateComparisonButton, 0, 8);
            this.monitorServerReportsPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.monitorServerReportsPanel.Location = new System.Drawing.Point(0, 0);
            this.monitorServerReportsPanel.Margin = new System.Windows.Forms.Padding(0);
            this.monitorServerReportsPanel.Name = "monitorServerReportsPanel";
            this.monitorServerReportsPanel.RowCount = 9;
            this.monitorServerReportsPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 16.66667F));
            this.monitorServerReportsPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 16.66667F));
            this.monitorServerReportsPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 16.66667F));
            this.monitorServerReportsPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 16.66667F));
            this.monitorServerReportsPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 16.66667F));
            this.monitorServerReportsPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 16.66667F));
            this.monitorServerReportsPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 16.66667F));
            this.monitorServerReportsPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 16.66667F));
            this.monitorServerReportsPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 16.66667F));
			this.monitorServerReportsPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 16.66667F));
            this.monitorServerReportsPanel.Size = new System.Drawing.Size(254, 480);
            this.monitorServerReportsPanel.TabIndex = 7;
            // 
            // alwaysOnTopologyButton
            // 
            this.alwaysOnTopologyButton.Description = "View the current topology of your AlwaysOn Availability Groups configuration.";
            this.alwaysOnTopologyButton.Dock = System.Windows.Forms.DockStyle.Fill;
            this.alwaysOnTopologyButton.Location = new System.Drawing.Point(3, 298);
            this.alwaysOnTopologyButton.MinimumSize = new System.Drawing.Size(0, 40);
            this.alwaysOnTopologyButton.Name = "alwaysOnTopologyButton";
            this.alwaysOnTopologyButton.Size = new System.Drawing.Size(248, 57);
            this.alwaysOnTopologyButton.TabIndex = 47;
            this.alwaysOnTopologyButton.Tag = "AlwaysOnTopology";
            this.alwaysOnTopologyButton.Title = "AlwaysOn Topology";
            this.alwaysOnTopologyButton.TitleColor = System.Drawing.Color.FromArgb(((int)(((byte)(53)))), ((int)(((byte)(53)))), ((int)(((byte)(53)))));
            this.alwaysOnTopologyButton.Load += new System.EventHandler(this.reportButton_Load);
            this.alwaysOnTopologyButton.MouseClick += new System.Windows.Forms.MouseEventHandler(this.reportButton_MouseClick);
            // 
            // SQLDM-28817
            // deadLockReportButton
            // 
            this.deadLockReportButton.Description = "View the deadlock report of the monitored servers.";
            this.deadLockReportButton.Dock = System.Windows.Forms.DockStyle.Fill;
            this.deadLockReportButton.Location = new System.Drawing.Point(3, 340);
            this.deadLockReportButton.MinimumSize = new System.Drawing.Size(0, 28);
            this.deadLockReportButton.Name = "deadLockReportButton";
            this.deadLockReportButton.Size = new System.Drawing.Size(248, 35);
            this.deadLockReportButton.TabIndex = 48;
            this.deadLockReportButton.Tag = "DeadlockReport";
            this.deadLockReportButton.Title = "Dead Lock Report";
            this.deadLockReportButton.TitleColor = System.Drawing.Color.FromArgb(((int)(((byte)(53)))), ((int)(((byte)(53)))), ((int)(((byte)(53)))));
            this.deadLockReportButton.Load += new System.EventHandler(this.reportButton_Load);
            this.deadLockReportButton.MouseClick += new System.Windows.Forms.MouseEventHandler(this.reportButton_MouseClick);
            // 
            // serverSummaryButton
            // 
            this.serverSummaryButton.Description = "Monitor the health and view details for a specific SQL Server within your organiz" +
    "ation.";
            this.serverSummaryButton.Dock = System.Windows.Forms.DockStyle.Fill;
            this.serverSummaryButton.Location = new System.Drawing.Point(3, 62);
            this.serverSummaryButton.MinimumSize = new System.Drawing.Size(0, 40);
            this.serverSummaryButton.Name = "serverSummaryButton";
            this.serverSummaryButton.Size = new System.Drawing.Size(248, 53);
            this.serverSummaryButton.TabIndex = 43;
            this.serverSummaryButton.Tag = "ServerSummary";
            this.serverSummaryButton.Title = "Server Summary";
            this.serverSummaryButton.TitleColor = System.Drawing.Color.FromArgb(((int)(((byte)(53)))), ((int)(((byte)(53)))), ((int)(((byte)(53)))));
            this.serverSummaryButton.Load += new System.EventHandler(this.reportButton_Load);
            this.serverSummaryButton.MouseClick += new System.Windows.Forms.MouseEventHandler(this.reportButton_MouseClick);
            // 
            // enterpriseSummaryButton
            // 
            this.enterpriseSummaryButton.Description = "View the most recent health and availability of the SQL Servers within your organ" +
    "ization.";
            this.enterpriseSummaryButton.Dock = System.Windows.Forms.DockStyle.Fill;
            this.enterpriseSummaryButton.Location = new System.Drawing.Point(3, 3);
            this.enterpriseSummaryButton.MinimumSize = new System.Drawing.Size(0, 40);
            this.enterpriseSummaryButton.Name = "enterpriseSummaryButton";
            this.enterpriseSummaryButton.Size = new System.Drawing.Size(248, 53);
            this.enterpriseSummaryButton.TabIndex = 42;
            this.enterpriseSummaryButton.Tag = "EnterpriseSummary";
            this.enterpriseSummaryButton.Title = "Enterprise Summary";
            this.enterpriseSummaryButton.TitleColor = System.Drawing.Color.FromArgb(((int)(((byte)(53)))), ((int)(((byte)(53)))), ((int)(((byte)(53)))));
            this.enterpriseSummaryButton.Load += new System.EventHandler(this.reportButton_Load);
            this.enterpriseSummaryButton.MouseClick += new System.Windows.Forms.MouseEventHandler(this.reportButton_MouseClick);
            // 
            // activeAlertsButton
            // 
            this.activeAlertsButton.Description = "View the outstanding alerts for your SQL Servers and how long have they been acti" +
    "ve.";
            this.activeAlertsButton.Dock = System.Windows.Forms.DockStyle.Fill;
            this.activeAlertsButton.Location = new System.Drawing.Point(3, 121);
            this.activeAlertsButton.MinimumSize = new System.Drawing.Size(0, 40);
            this.activeAlertsButton.Name = "activeAlertsButton";
            this.activeAlertsButton.Size = new System.Drawing.Size(248, 53);
            this.activeAlertsButton.TabIndex = 44;
            this.activeAlertsButton.Tag = "ActiveAlerts";
            this.activeAlertsButton.Title = "Active Alerts";
            this.activeAlertsButton.TitleColor = System.Drawing.Color.FromArgb(((int)(((byte)(53)))), ((int)(((byte)(53)))), ((int)(((byte)(53)))));
            this.activeAlertsButton.Load += new System.EventHandler(this.reportButton_Load);
            this.activeAlertsButton.MouseClick += new System.Windows.Forms.MouseEventHandler(this.reportButton_MouseClick);
            // 
            // mirroringSummaryButton
            // 
            this.mirroringSummaryButton.Description = "View the most recent status of your mirrored databases.";
            this.mirroringSummaryButton.Dock = System.Windows.Forms.DockStyle.Fill;
            this.mirroringSummaryButton.Location = new System.Drawing.Point(3, 180);
            this.mirroringSummaryButton.MinimumSize = new System.Drawing.Size(0, 40);
            this.mirroringSummaryButton.Name = "mirroringSummaryButton";
            this.mirroringSummaryButton.Size = new System.Drawing.Size(248, 53);
            this.mirroringSummaryButton.TabIndex = 45;
            this.mirroringSummaryButton.Tag = "MirroringSummary";
            this.mirroringSummaryButton.Title = "Mirroring Summary";
            this.mirroringSummaryButton.TitleColor = System.Drawing.Color.FromArgb(((int)(((byte)(53)))), ((int)(((byte)(53)))), ((int)(((byte)(53)))));
            this.mirroringSummaryButton.Load += new System.EventHandler(this.reportButton_Load);
            this.mirroringSummaryButton.MouseClick += new System.Windows.Forms.MouseEventHandler(this.reportButton_MouseClick);
            //
            //TemplateComparisonButton
            //
            this.templateComparisonButton.Description = "Template Comparison for Source and Tartget Metrics";
            this.templateComparisonButton.Dock = System.Windows.Forms.DockStyle.Fill;
            this.templateComparisonButton.Location = new System.Drawing.Point(3, 490);
            this.templateComparisonButton.MinimumSize = new System.Drawing.Size(0, 40);
            this.templateComparisonButton.Name = "templateComparisonButton";
            this.templateComparisonButton.Size = new System.Drawing.Size(248, 53);
            this.templateComparisonButton.TabIndex = 49;
            this.templateComparisonButton.Tag = "TemplateComparison";
            this.templateComparisonButton.Title = "Template Comparison";
            this.templateComparisonButton.TitleColor = System.Drawing.Color.FromArgb(((int)(((byte)(53)))), ((int)(((byte)(53)))), ((int)(((byte)(53)))));
            this.templateComparisonButton.Load += new System.EventHandler(this.reportButton_Load);
            this.templateComparisonButton.MouseClick += new System.Windows.Forms.MouseEventHandler(this.reportButton_MouseClick);
            //
            //AlertThresholdButton
            //
            this.alertThresholdButton.Description = "View all the alert Threshold for a server";
            this.alertThresholdButton.Dock = System.Windows.Forms.DockStyle.Fill;
            this.alertThresholdButton.Location = new System.Drawing.Point(3, 426);
            this.alertThresholdButton.MinimumSize = new System.Drawing.Size(0, 40);
            this.alertThresholdButton.Name = "alertThresholdButton";
            this.alertThresholdButton.Size = new System.Drawing.Size(248, 53);
            this.alertThresholdButton.TabIndex = 48;
            this.alertThresholdButton.Tag = "AlertThreshold";
            this.alertThresholdButton.Title = "Alert Threshold";
            this.alertThresholdButton.TitleColor = System.Drawing.Color.FromArgb(((int)(((byte)(53)))), ((int)(((byte)(53)))), ((int)(((byte)(53)))));
            this.alertThresholdButton.Load += new System.EventHandler(this.reportButton_Load);
            this.alertThresholdButton.MouseClick += new System.Windows.Forms.MouseEventHandler(this.reportButton_MouseClick);

            //
            //AlertTemplateButton
            //
            this.alertTemplatesButton.Description = "List of Instances with Alert Template Assigned per instance";
            this.alertTemplatesButton.Dock = System.Windows.Forms.DockStyle.Fill;
            this.alertTemplatesButton.Location = new System.Drawing.Point(3, 362);
            this.alertTemplatesButton.MinimumSize = new System.Drawing.Size(0, 40);
            this.alertTemplatesButton.Name = "alertTemplatesButton";
            this.alertTemplatesButton.Size = new System.Drawing.Size(248, 53);
            this.alertTemplatesButton.TabIndex = 47;
            this.alertTemplatesButton.Tag = "AlertTemplateReport";
            this.alertTemplatesButton.Title = "Alert Templates";
            this.alertTemplatesButton.TitleColor = System.Drawing.Color.FromArgb(((int)(((byte)(53)))), ((int)(((byte)(53)))), ((int)(((byte)(53)))));
            this.alertTemplatesButton.Load += new System.EventHandler(this.reportButton_Load);
            this.alertTemplatesButton.MouseClick += new System.Windows.Forms.MouseEventHandler(this.reportButton_MouseClick);
            // 
            // metricThresholdButton
            // 
            this.metricThresholdButton.Description = "View all the metric thresholds for a server";
            this.metricThresholdButton.Dock = System.Windows.Forms.DockStyle.Fill;
            this.metricThresholdButton.Location = new System.Drawing.Point(3, 239);
            this.metricThresholdButton.MinimumSize = new System.Drawing.Size(0, 40);
            this.metricThresholdButton.Name = "metricThresholdButton";
            this.metricThresholdButton.Size = new System.Drawing.Size(248, 53);
            this.metricThresholdButton.TabIndex = 46;
            this.metricThresholdButton.Tag = "MetricThresholds";
            this.metricThresholdButton.Title = "Metric Thresholds";
            this.metricThresholdButton.TitleColor = System.Drawing.Color.FromArgb(((int)(((byte)(53)))), ((int)(((byte)(53)))), ((int)(((byte)(53)))));
            this.metricThresholdButton.Load += new System.EventHandler(this.reportButton_Load);
            this.metricThresholdButton.MouseClick += new System.Windows.Forms.MouseEventHandler(this.reportButton_MouseClick);
            // 
            // monitorVirtualizationReportsPanel
            // 
            this.monitorVirtualizationReportsPanel.ColumnCount = 1;
            this.monitorVirtualizationReportsPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.monitorVirtualizationReportsPanel.Controls.Add(this.virtualizationSummaryButton, 0, 0);
            this.monitorVirtualizationReportsPanel.Controls.Add(this.vStatisticsReportButton, 0, 1);
            this.monitorVirtualizationReportsPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.monitorVirtualizationReportsPanel.Location = new System.Drawing.Point(0, 0);
            this.monitorVirtualizationReportsPanel.Margin = new System.Windows.Forms.Padding(0);
            this.monitorVirtualizationReportsPanel.Name = "monitorVirtualizationReportsPanel";
            this.monitorVirtualizationReportsPanel.RowCount = 6;
            this.monitorVirtualizationReportsPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 16.66667F));
            this.monitorVirtualizationReportsPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 16.66667F));
            this.monitorVirtualizationReportsPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 16.66667F));
            this.monitorVirtualizationReportsPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 16.66667F));
            this.monitorVirtualizationReportsPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 16.66667F));
            this.monitorVirtualizationReportsPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 16.66667F));
            this.monitorVirtualizationReportsPanel.Size = new System.Drawing.Size(254, 358);
            this.monitorVirtualizationReportsPanel.TabIndex = 47;
            this.monitorVirtualizationReportsPanel.Visible = false;
            // 
            // virtualizationSummaryButton
            // 
            this.virtualizationSummaryButton.Description = "View the overall health and availability of your virtualizaed servers.";
            this.virtualizationSummaryButton.Dock = System.Windows.Forms.DockStyle.Fill;
            this.virtualizationSummaryButton.Location = new System.Drawing.Point(3, 3);
            this.virtualizationSummaryButton.MinimumSize = new System.Drawing.Size(0, 40);
            this.virtualizationSummaryButton.Name = "virtualizationSummaryButton";
            this.virtualizationSummaryButton.Size = new System.Drawing.Size(248, 53);
            this.virtualizationSummaryButton.TabIndex = 42;
            this.virtualizationSummaryButton.Tag = "VirtualizationSummary";
            this.virtualizationSummaryButton.Title = "Virtualization Summary";
            this.virtualizationSummaryButton.TitleColor = System.Drawing.Color.FromArgb(((int)(((byte)(53)))), ((int)(((byte)(53)))), ((int)(((byte)(53)))));
            this.virtualizationSummaryButton.Load += new System.EventHandler(this.reportButton_Load);
            this.virtualizationSummaryButton.MouseClick += new System.Windows.Forms.MouseEventHandler(this.reportButton_MouseClick);
            // 
            // vStatisticsReportButton
            // 
            this.vStatisticsReportButton.Description = "Shows Virtualization Statistics.";
            this.vStatisticsReportButton.Dock = System.Windows.Forms.DockStyle.Fill;
            this.vStatisticsReportButton.Location = new System.Drawing.Point(3, 62);
            this.vStatisticsReportButton.MinimumSize = new System.Drawing.Size(0, 40);
            this.vStatisticsReportButton.Name = "vStatisticsReportButton";
            this.vStatisticsReportButton.Size = new System.Drawing.Size(248, 53);
            this.vStatisticsReportButton.TabIndex = 44;
            this.vStatisticsReportButton.Tag = "VirtualizationStatistics";
            this.vStatisticsReportButton.Title = "Virtualization Statistics Report";
            this.vStatisticsReportButton.TitleColor = System.Drawing.Color.FromArgb(((int)(((byte)(53)))), ((int)(((byte)(53)))), ((int)(((byte)(53)))));
            this.vStatisticsReportButton.Load += new System.EventHandler(this.reportButton_Load);
            this.vStatisticsReportButton.MouseClick += new System.Windows.Forms.MouseEventHandler(this.reportButton_MouseClick);
            // 
            // monitorSqldmActivityReportsPanel
            // 
            this.monitorSqldmActivityReportsPanel.ColumnCount = 1;
            this.monitorSqldmActivityReportsPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.monitorSqldmActivityReportsPanel.Controls.Add(this.sqldmActivityReportButton, 0, 0);
            this.monitorSqldmActivityReportsPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.monitorSqldmActivityReportsPanel.Location = new System.Drawing.Point(0, 0);
            this.monitorSqldmActivityReportsPanel.Margin = new System.Windows.Forms.Padding(0);
            this.monitorSqldmActivityReportsPanel.Name = "monitorSqldmActivityReportsPanel";
            this.monitorSqldmActivityReportsPanel.RowCount = 6;
            this.monitorSqldmActivityReportsPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 16.66667F));
            this.monitorSqldmActivityReportsPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 16.66667F));
            this.monitorSqldmActivityReportsPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 16.66667F));
            this.monitorSqldmActivityReportsPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 16.66667F));
            this.monitorSqldmActivityReportsPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 16.66667F));
            this.monitorSqldmActivityReportsPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 16.66667F));
            this.monitorSqldmActivityReportsPanel.Size = new System.Drawing.Size(254, 358);
            this.monitorSqldmActivityReportsPanel.TabIndex = 48;
            this.monitorSqldmActivityReportsPanel.Visible = false;
            // 
            // sqldmActivityReportButton
            // 
            this.sqldmActivityReportButton.Description = "Summary of all the key actions performed in SQLdm.";
            this.sqldmActivityReportButton.Dock = System.Windows.Forms.DockStyle.Fill;
            this.sqldmActivityReportButton.Location = new System.Drawing.Point(3, 3);
            this.sqldmActivityReportButton.MinimumSize = new System.Drawing.Size(0, 40);
            this.sqldmActivityReportButton.Name = "sqldmActivityReportButton";
            this.sqldmActivityReportButton.Size = new System.Drawing.Size(248, 53);
            this.sqldmActivityReportButton.TabIndex = 42;
            this.sqldmActivityReportButton.Tag = "ChangeLogSummary";
            this.sqldmActivityReportButton.Title = "Change Log Summary";
            this.sqldmActivityReportButton.TitleColor = System.Drawing.Color.FromArgb(((int)(((byte)(53)))), ((int)(((byte)(53)))), ((int)(((byte)(53)))));
            this.sqldmActivityReportButton.Load += new System.EventHandler(this.reportButton_Load);
            this.sqldmActivityReportButton.MouseClick += new System.Windows.Forms.MouseEventHandler(this.reportButton_MouseClick);
            // 
            // roundedPanel6
            // 
            this.roundedPanel6.BackColor = System.Drawing.Color.White;
            this.roundedPanel6.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(215)))), ((int)(((byte)(215)))), ((int)(((byte)(215)))));
            this.roundedPanel6.Controls.Add(this.label8);
            this.roundedPanel6.Dock = System.Windows.Forms.DockStyle.Top;
            this.roundedPanel6.FillColor = System.Drawing.Color.FromArgb(((int)(((byte)(228)))), ((int)(((byte)(228)))), ((int)(((byte)(228)))));
            this.roundedPanel6.FillColor2 = System.Drawing.Color.Empty;
            this.roundedPanel6.Location = new System.Drawing.Point(0, 0);
            this.roundedPanel6.Name = "roundedPanel6";
            this.roundedPanel6.Radius = 3F;
            this.roundedPanel6.Size = new System.Drawing.Size(260, 40);
            this.roundedPanel6.TabIndex = 5;
            // 
            // label8
            // 
            this.label8.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.label8.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(228)))), ((int)(((byte)(228)))), ((int)(((byte)(228)))));
            this.label8.Font = new System.Drawing.Font("Arial", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label8.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(75)))), ((int)(((byte)(75)))), ((int)(((byte)(75)))));
            this.label8.Location = new System.Drawing.Point(7, 11);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(245, 19);
            this.label8.TabIndex = 5;
            this.label8.Text = "Monitor";
            this.label8.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // discoverPanel
            // 
            this.discoverPanel.Anchor = System.Windows.Forms.AnchorStyles.Top;
            this.discoverPanel.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(215)))), ((int)(((byte)(215)))), ((int)(((byte)(215)))));
            this.discoverPanel.Controls.Add(this.tableLayoutPanel3);
            this.discoverPanel.Controls.Add(this.roundedPanel5);
            this.discoverPanel.FillColor = System.Drawing.Color.White;
            this.discoverPanel.FillColor2 = System.Drawing.Color.Empty;
            this.discoverPanel.Location = new System.Drawing.Point(355, 15);
            this.discoverPanel.Name = "discoverPanel";
            this.discoverPanel.Radius = 3F;
            this.discoverPanel.Size = new System.Drawing.Size(260, 467);
            this.discoverPanel.TabIndex = 15;
            // 
            // tableLayoutPanel3
            // 
            this.tableLayoutPanel3.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tableLayoutPanel3.ColumnCount = 1;
            this.tableLayoutPanel3.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel3.Controls.Add(this.panel3, 0, 1);
            this.tableLayoutPanel3.Controls.Add(this.panel5, 0, 0);
            this.tableLayoutPanel3.Location = new System.Drawing.Point(3, 41);
            this.tableLayoutPanel3.Name = "tableLayoutPanel3";
            this.tableLayoutPanel3.RowCount = 2;
            this.tableLayoutPanel3.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 25F));
            this.tableLayoutPanel3.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel3.Size = new System.Drawing.Size(254, 423);
            this.tableLayoutPanel3.TabIndex = 6;
            // 
            // panel3
            // 
            this.panel3.Controls.Add(this.analyzeDatabasesReportsPanel);
            this.panel3.Controls.Add(this.analyzeServersReportsPanel);
            this.panel3.Controls.Add(this.analyzeResourcesReportsPanel);
            this.panel3.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel3.Location = new System.Drawing.Point(0, 25);
            this.panel3.Margin = new System.Windows.Forms.Padding(0);
            this.panel3.Name = "panel3";
            this.panel3.Size = new System.Drawing.Size(254, 398);
            this.panel3.TabIndex = 0;
            // 
            // analyzeDatabasesReportsPanel
            // 
            this.analyzeDatabasesReportsPanel.AutoScroll = true;
            this.analyzeDatabasesReportsPanel.ColumnCount = 1;
            this.analyzeDatabasesReportsPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.analyzeDatabasesReportsPanel.Controls.Add(this.databaseAppsButton, 0, 2);
            this.analyzeDatabasesReportsPanel.Controls.Add(this.topTablesGrowthButton, 0, 4);
            this.analyzeDatabasesReportsPanel.Controls.Add(this.topDatabasesButton, 0, 0);
            this.analyzeDatabasesReportsPanel.Controls.Add(this.topTablesFragmentedButton, 0, 5);
            this.analyzeDatabasesReportsPanel.Controls.Add(this.databaseStatsButton, 0, 1);
            this.analyzeDatabasesReportsPanel.Controls.Add(this.mirroringHistoryButton, 0, 3);
            this.analyzeDatabasesReportsPanel.Controls.Add(this.tempdbButton, 0, 6);
            this.analyzeDatabasesReportsPanel.Controls.Add(this.transactionLogStatisticsButton, 0, 7);
            this.analyzeDatabasesReportsPanel.Controls.Add(this.alwaysOnStatisticsButton, 0, 8);
            this.analyzeDatabasesReportsPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.analyzeDatabasesReportsPanel.Location = new System.Drawing.Point(0, 0);
            this.analyzeDatabasesReportsPanel.Margin = new System.Windows.Forms.Padding(0);
            this.analyzeDatabasesReportsPanel.Name = "analyzeDatabasesReportsPanel";
            this.analyzeDatabasesReportsPanel.RowCount = 10;
            this.analyzeDatabasesReportsPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 0F));
            this.analyzeDatabasesReportsPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 0F));
            this.analyzeDatabasesReportsPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 0F));
            this.analyzeDatabasesReportsPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 0F));
            this.analyzeDatabasesReportsPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 0F));
            this.analyzeDatabasesReportsPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 0F));
            this.analyzeDatabasesReportsPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 0F));
            this.analyzeDatabasesReportsPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 0F));
            this.analyzeDatabasesReportsPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 60F));
            this.analyzeDatabasesReportsPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 24F));
            this.analyzeDatabasesReportsPanel.Size = new System.Drawing.Size(254, 398);
            this.analyzeDatabasesReportsPanel.TabIndex = 1;
            this.analyzeDatabasesReportsPanel.Visible = false;
            // 
            // databaseAppsButton
            // 
            this.databaseAppsButton.Description = "Find database applications consuming the most SQL Server resources.";
            this.databaseAppsButton.Dock = System.Windows.Forms.DockStyle.Fill;
            this.databaseAppsButton.Location = new System.Drawing.Point(3, 101);
            this.databaseAppsButton.MinimumSize = new System.Drawing.Size(0, 40);
            this.databaseAppsButton.Name = "databaseAppsButton";
            this.databaseAppsButton.Size = new System.Drawing.Size(248, 43);
            this.databaseAppsButton.TabIndex = 40;
            this.databaseAppsButton.Tag = "TopDatabaseApps";
            this.databaseAppsButton.Title = "Top Database Applications";
            this.databaseAppsButton.TitleColor = System.Drawing.Color.FromArgb(((int)(((byte)(53)))), ((int)(((byte)(53)))), ((int)(((byte)(53)))));
            this.databaseAppsButton.Load += new System.EventHandler(this.reportButton_Load);
            this.databaseAppsButton.MouseClick += new System.Windows.Forms.MouseEventHandler(this.reportButton_MouseClick);
            // 
            // topTablesGrowthButton
            // 
            this.topTablesGrowthButton.Description = "Find the tables that are growing the fastest in your SQL Server databases.";
            this.topTablesGrowthButton.Dock = System.Windows.Forms.DockStyle.Fill;
            this.topTablesGrowthButton.Location = new System.Drawing.Point(3, 199);
            this.topTablesGrowthButton.MinimumSize = new System.Drawing.Size(0, 40);
            this.topTablesGrowthButton.Name = "topTablesGrowthButton";
            this.topTablesGrowthButton.Size = new System.Drawing.Size(248, 43);
            this.topTablesGrowthButton.TabIndex = 38;
            this.topTablesGrowthButton.Tag = "TopTablesGrowth";
            this.topTablesGrowthButton.Title = "Fastest Growing Tables";
            this.topTablesGrowthButton.TitleColor = System.Drawing.Color.FromArgb(((int)(((byte)(53)))), ((int)(((byte)(53)))), ((int)(((byte)(53)))));
            this.topTablesGrowthButton.Load += new System.EventHandler(this.reportButton_Load);
            this.topTablesGrowthButton.MouseClick += new System.Windows.Forms.MouseEventHandler(this.reportButton_MouseClick);
            // 
            // topDatabasesButton
            // 
            this.topDatabasesButton.Description = "Identify your worst performing databases.";
            this.topDatabasesButton.Dock = System.Windows.Forms.DockStyle.Fill;
            this.topDatabasesButton.Location = new System.Drawing.Point(3, 3);
            this.topDatabasesButton.MinimumSize = new System.Drawing.Size(0, 40);
            this.topDatabasesButton.Name = "topDatabasesButton";
            this.topDatabasesButton.Size = new System.Drawing.Size(248, 43);
            this.topDatabasesButton.TabIndex = 41;
            this.topDatabasesButton.Tag = "TopDatabases";
            this.topDatabasesButton.Title = "Top Databases";
            this.topDatabasesButton.TitleColor = System.Drawing.Color.FromArgb(((int)(((byte)(53)))), ((int)(((byte)(53)))), ((int)(((byte)(53)))));
            this.topDatabasesButton.Load += new System.EventHandler(this.reportButton_Load);
            this.topDatabasesButton.MouseClick += new System.Windows.Forms.MouseEventHandler(this.reportButton_MouseClick);
            // 
            // topTablesFragmentedButton
            // 
            this.topTablesFragmentedButton.Description = "Find the tables that are most fragmented in your SQL Server databases.";
            this.topTablesFragmentedButton.Dock = System.Windows.Forms.DockStyle.Fill;
            this.topTablesFragmentedButton.Location = new System.Drawing.Point(3, 248);
            this.topTablesFragmentedButton.MinimumSize = new System.Drawing.Size(0, 40);
            this.topTablesFragmentedButton.Name = "topTablesFragmentedButton";
            this.topTablesFragmentedButton.Size = new System.Drawing.Size(248, 43);
            this.topTablesFragmentedButton.TabIndex = 39;
            this.topTablesFragmentedButton.Tag = "TopTableFrag";
            this.topTablesFragmentedButton.Title = "Most Fragmented Tables";
            this.topTablesFragmentedButton.TitleColor = System.Drawing.Color.FromArgb(((int)(((byte)(53)))), ((int)(((byte)(53)))), ((int)(((byte)(53)))));
            this.topTablesFragmentedButton.Load += new System.EventHandler(this.reportButton_Load);
            this.topTablesFragmentedButton.MouseClick += new System.Windows.Forms.MouseEventHandler(this.reportButton_MouseClick);
            // 
            // databaseStatsButton
            // 
            this.databaseStatsButton.Description = "Analyze key performance metrics of your databases.";
            this.databaseStatsButton.Dock = System.Windows.Forms.DockStyle.Fill;
            this.databaseStatsButton.Location = new System.Drawing.Point(3, 52);
            this.databaseStatsButton.MinimumSize = new System.Drawing.Size(0, 40);
            this.databaseStatsButton.Name = "databaseStatsButton";
            this.databaseStatsButton.Size = new System.Drawing.Size(248, 43);
            this.databaseStatsButton.TabIndex = 42;
            this.databaseStatsButton.Tag = "DatabaseStatistics";
            this.databaseStatsButton.Title = "Database Statistics";
            this.databaseStatsButton.TitleColor = System.Drawing.Color.FromArgb(((int)(((byte)(53)))), ((int)(((byte)(53)))), ((int)(((byte)(53)))));
            this.databaseStatsButton.Load += new System.EventHandler(this.reportButton_Load);
            this.databaseStatsButton.MouseClick += new System.Windows.Forms.MouseEventHandler(this.reportButton_MouseClick);
            // 
            // mirroringHistoryButton
            // 
            this.mirroringHistoryButton.Description = "Search the event history for your mirrored databases.";
            this.mirroringHistoryButton.Dock = System.Windows.Forms.DockStyle.Fill;
            this.mirroringHistoryButton.Location = new System.Drawing.Point(3, 150);
            this.mirroringHistoryButton.MinimumSize = new System.Drawing.Size(0, 40);
            this.mirroringHistoryButton.Name = "mirroringHistoryButton";
            this.mirroringHistoryButton.Size = new System.Drawing.Size(248, 43);
            this.mirroringHistoryButton.TabIndex = 43;
            this.mirroringHistoryButton.Tag = "MirroringHistory";
            this.mirroringHistoryButton.Title = "Mirroring History";
            this.mirroringHistoryButton.TitleColor = System.Drawing.Color.FromArgb(((int)(((byte)(53)))), ((int)(((byte)(53)))), ((int)(((byte)(53)))));
            this.mirroringHistoryButton.Load += new System.EventHandler(this.reportButton_Load);
            this.mirroringHistoryButton.MouseClick += new System.Windows.Forms.MouseEventHandler(this.reportButton_MouseClick);
            // 
            // tempdbButton
            // 
            this.tempdbButton.Description = "Analyze the performance and space utilization of tempdb over time.";
            this.tempdbButton.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tempdbButton.Location = new System.Drawing.Point(3, 297);
            this.tempdbButton.MinimumSize = new System.Drawing.Size(0, 40);
            this.tempdbButton.Name = "tempdbButton";
            this.tempdbButton.Size = new System.Drawing.Size(248, 43);
            this.tempdbButton.TabIndex = 51;
            this.tempdbButton.Tag = "TempdbStatistics";
            this.tempdbButton.Title = "Tempdb Statistcs";
            this.tempdbButton.TitleColor = System.Drawing.Color.FromArgb(((int)(((byte)(53)))), ((int)(((byte)(53)))), ((int)(((byte)(53)))));
            this.tempdbButton.Load += new System.EventHandler(this.reportButton_Load);
            this.tempdbButton.MouseClick += new System.Windows.Forms.MouseEventHandler(this.reportButton_MouseClick);
            // 
            // transactionLogStatisticsButton
            // 
            this.transactionLogStatisticsButton.Description = "Analyze the database transaction log.";
            this.transactionLogStatisticsButton.Dock = System.Windows.Forms.DockStyle.Fill;
            this.transactionLogStatisticsButton.Location = new System.Drawing.Point(3, 346);
            this.transactionLogStatisticsButton.MinimumSize = new System.Drawing.Size(0, 40);
            this.transactionLogStatisticsButton.Name = "transactionLogStatisticsButton";
            this.transactionLogStatisticsButton.Size = new System.Drawing.Size(248, 43);
            this.transactionLogStatisticsButton.TabIndex = 51;
            this.transactionLogStatisticsButton.Tag = "TransactionLogStatistics";
            this.transactionLogStatisticsButton.Title = "TransactionLog Statistics";
            this.transactionLogStatisticsButton.TitleColor = System.Drawing.Color.FromArgb(((int)(((byte)(53)))), ((int)(((byte)(53)))), ((int)(((byte)(53)))));
            this.transactionLogStatisticsButton.Load += new System.EventHandler(this.reportButton_Load);
            this.transactionLogStatisticsButton.MouseClick += new System.Windows.Forms.MouseEventHandler(this.reportButton_MouseClick);
            // 
            // alwaysOnStatisticsButton
            // 
            this.alwaysOnStatisticsButton.Description = "View the historical health of your availability groups, availability replicas, an" +
    "d availability databases.";
            this.alwaysOnStatisticsButton.Dock = System.Windows.Forms.DockStyle.Fill;
            this.alwaysOnStatisticsButton.Location = new System.Drawing.Point(3, 395);
            this.alwaysOnStatisticsButton.MinimumSize = new System.Drawing.Size(0, 40);
            this.alwaysOnStatisticsButton.Name = "alwaysOnStatisticsButton";
            this.alwaysOnStatisticsButton.Size = new System.Drawing.Size(248, 54);
            this.alwaysOnStatisticsButton.TabIndex = 43;
            this.alwaysOnStatisticsButton.Tag = "AlwaysOnStatistics";
            this.alwaysOnStatisticsButton.Title = "AlwaysOn Statistics";
            this.alwaysOnStatisticsButton.TitleColor = System.Drawing.Color.FromArgb(((int)(((byte)(53)))), ((int)(((byte)(53)))), ((int)(((byte)(53)))));
            this.alwaysOnStatisticsButton.Load += new System.EventHandler(this.reportButton_Load);
            this.alwaysOnStatisticsButton.MouseClick += new System.Windows.Forms.MouseEventHandler(this.reportButton_MouseClick);
            // 
            // analyzeServersReportsPanel
            // 
            //Start: SQLdm 8.6 (Vineet Kumar): Added for Query Wait Stats Report. After addition of this report button, panel was overflowing. Hence set its scroller to true.
            this.analyzeServersReportsPanel.AutoScroll = true;
            //End: SQLdm 8.6 (Vineet Kumar): Added for Query Wait Stats Report. After addition of this report button, panel was overflowing. Hence set its scroller to true.
            this.analyzeServersReportsPanel.ColumnCount = 1;
            this.analyzeServersReportsPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.analyzeServersReportsPanel.Controls.Add(this.topServersButton, 0, 0);
            this.analyzeServersReportsPanel.Controls.Add(this.serverInventoryButton, 0, 2);
            this.analyzeServersReportsPanel.Controls.Add(this.serverStatisticsButton, 0, 1);
            this.analyzeServersReportsPanel.Controls.Add(this.topQueriesButton, 0, 4);
            this.analyzeServersReportsPanel.Controls.Add(this.alertSummaryButton, 0, 5);
            this.analyzeServersReportsPanel.Controls.Add(this.serverUptimeButton, 0, 8);
            this.analyzeServersReportsPanel.Controls.Add(this.queryOverviewButton, 0, 3);
            this.analyzeServersReportsPanel.Controls.Add(this.baselineStatisticsButton, 0, 6);
            //Start: SQLdm 8.6 (Vineet Kumar): Added for New Query Wait Stats Report. Adding it to the Analyze Servers Panel
            this.analyzeServersReportsPanel.Controls.Add(this.queryWaitStatisticsButton, 0, 7);
            //End: SQLdm 8.6 (Vineet Kumar): Added for New Query Wait Stats Report. Adding it to the Analyze Servers Panel
            this.analyzeServersReportsPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.analyzeServersReportsPanel.Location = new System.Drawing.Point(0, 0);
            this.analyzeServersReportsPanel.Margin = new System.Windows.Forms.Padding(0);
            this.analyzeServersReportsPanel.Name = "analyzeServersReportsPanel";
            //Start: SQLdm 8.6 (Vineet Kumar): Added for Query Wait Stats Report. Increased the row count by 1 to accomodate the new report
            this.analyzeServersReportsPanel.RowCount = 9;
            //End: SQLdm 8.6 (Vineet Kumar): Added for Query Wait Stats Report. Increased the row count by 1 to accomodate the new report
            this.analyzeServersReportsPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 0F));
            this.analyzeServersReportsPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 0F));
            this.analyzeServersReportsPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 0F));
            this.analyzeServersReportsPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 0F));
            this.analyzeServersReportsPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 0F));
            this.analyzeServersReportsPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 0F));
            this.analyzeServersReportsPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 0F));
            this.analyzeServersReportsPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 0F));
            this.analyzeServersReportsPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 0F));
            this.analyzeServersReportsPanel.Size = new System.Drawing.Size(254, 398);
            this.analyzeServersReportsPanel.TabIndex = 0;
            // 
            // topServersButton
            // 
            this.topServersButton.Description = "Identify your worst performing servers.";
            this.topServersButton.Dock = System.Windows.Forms.DockStyle.Fill;
            this.topServersButton.Location = new System.Drawing.Point(3, 3);
            this.topServersButton.MinimumSize = new System.Drawing.Size(0, 40);
            this.topServersButton.Name = "topServersButton";
            this.topServersButton.Size = new System.Drawing.Size(248, 51);
            this.topServersButton.TabIndex = 46;
            this.topServersButton.Tag = "TopServers";
            this.topServersButton.Title = "Top Servers";
            this.topServersButton.TitleColor = System.Drawing.Color.FromArgb(((int)(((byte)(53)))), ((int)(((byte)(53)))), ((int)(((byte)(53)))));
            this.topServersButton.Load += new System.EventHandler(this.reportButton_Load);
            this.topServersButton.MouseClick += new System.Windows.Forms.MouseEventHandler(this.reportButton_MouseClick);
            // 
            // serverInventoryButton
            // 
            this.serverInventoryButton.Description = "Search your server inventory for SQL Servers with specific properties.";
            this.serverInventoryButton.Dock = System.Windows.Forms.DockStyle.Fill;
            this.serverInventoryButton.Location = new System.Drawing.Point(3, 117);
            this.serverInventoryButton.MinimumSize = new System.Drawing.Size(0, 40);
            this.serverInventoryButton.Name = "serverInventoryButton";
            this.serverInventoryButton.Size = new System.Drawing.Size(248, 51);
            this.serverInventoryButton.TabIndex = 35;
            this.serverInventoryButton.Tag = "ServerInventory";
            this.serverInventoryButton.Title = "Server Inventory";
            this.serverInventoryButton.TitleColor = System.Drawing.Color.FromArgb(((int)(((byte)(53)))), ((int)(((byte)(53)))), ((int)(((byte)(53)))));
            this.serverInventoryButton.Load += new System.EventHandler(this.reportButton_Load);
            this.serverInventoryButton.MouseClick += new System.Windows.Forms.MouseEventHandler(this.reportButton_MouseClick);
            // 
            // serverStatisticsButton
            // 
            this.serverStatisticsButton.Description = "Analyze and compare performance trends for your SQL Servers.";
            this.serverStatisticsButton.Dock = System.Windows.Forms.DockStyle.Fill;
            this.serverStatisticsButton.Location = new System.Drawing.Point(3, 60);
            this.serverStatisticsButton.MinimumSize = new System.Drawing.Size(0, 40);
            this.serverStatisticsButton.Name = "serverStatisticsButton";
            this.serverStatisticsButton.Size = new System.Drawing.Size(248, 51);
            this.serverStatisticsButton.TabIndex = 36;
            this.serverStatisticsButton.Tag = "ServerStatistics";
            this.serverStatisticsButton.Title = "Server Statistics";
            this.serverStatisticsButton.TitleColor = System.Drawing.Color.FromArgb(((int)(((byte)(53)))), ((int)(((byte)(53)))), ((int)(((byte)(53)))));
            this.serverStatisticsButton.Load += new System.EventHandler(this.reportButton_Load);
            this.serverStatisticsButton.MouseClick += new System.Windows.Forms.MouseEventHandler(this.reportButton_MouseClick);
            // 
            // topQueriesButton
            // 
            this.topQueriesButton.Description = "Find queries that perform poorly or execute frequently in your SQL Server environ" +
    "ment.";
            this.topQueriesButton.Dock = System.Windows.Forms.DockStyle.Fill;
            this.topQueriesButton.Location = new System.Drawing.Point(3, 231);
            this.topQueriesButton.MinimumSize = new System.Drawing.Size(0, 40);
            this.topQueriesButton.Name = "topQueriesButton";
            this.topQueriesButton.Size = new System.Drawing.Size(248, 51);
            this.topQueriesButton.TabIndex = 37;
            this.topQueriesButton.Tag = "TopQueries";
            this.topQueriesButton.Title = "Top Queries";
            this.topQueriesButton.TitleColor = System.Drawing.Color.FromArgb(((int)(((byte)(53)))), ((int)(((byte)(53)))), ((int)(((byte)(53)))));
            this.topQueriesButton.Load += new System.EventHandler(this.reportButton_Load);
            this.topQueriesButton.MouseClick += new System.Windows.Forms.MouseEventHandler(this.reportButton_MouseClick);
            // 
            // alertSummaryButton
            // 
            this.alertSummaryButton.Description = "Search the alert history for your servers.";
            this.alertSummaryButton.Dock = System.Windows.Forms.DockStyle.Fill;
            this.alertSummaryButton.Location = new System.Drawing.Point(3, 288);
            this.alertSummaryButton.MinimumSize = new System.Drawing.Size(0, 40);
            this.alertSummaryButton.Name = "alertSummaryButton";
            this.alertSummaryButton.Size = new System.Drawing.Size(248, 51);
            this.alertSummaryButton.TabIndex = 45;
            this.alertSummaryButton.Tag = "AlertHistory";
            this.alertSummaryButton.Title = "Alert History";
            this.alertSummaryButton.TitleColor = System.Drawing.Color.FromArgb(((int)(((byte)(53)))), ((int)(((byte)(53)))), ((int)(((byte)(53)))));
            this.alertSummaryButton.Load += new System.EventHandler(this.reportButton_Load);
            this.alertSummaryButton.MouseClick += new System.Windows.Forms.MouseEventHandler(this.reportButton_MouseClick);
            // 
            // serverUptimeButton
            // 
            this.serverUptimeButton.Description = "View the uptime for your servers.";
            this.serverUptimeButton.Dock = System.Windows.Forms.DockStyle.Fill;
            this.serverUptimeButton.Location = new System.Drawing.Point(3, 300);
            this.serverUptimeButton.MinimumSize = new System.Drawing.Size(0, 40);
            this.serverUptimeButton.Name = "serverUptimeButton";
            this.serverUptimeButton.Size = new System.Drawing.Size(248, 25);
            this.serverUptimeButton.TabIndex = 49;
            this.serverUptimeButton.Tag = "ServerUptime";
            this.serverUptimeButton.Title = "Server Uptime";
            this.serverUptimeButton.TitleColor = System.Drawing.Color.FromArgb(((int)(((byte)(53)))), ((int)(((byte)(53)))), ((int)(((byte)(53)))));
            this.serverUptimeButton.Load += new System.EventHandler(this.reportButton_Load);
            this.serverUptimeButton.MouseClick += new System.Windows.Forms.MouseEventHandler(this.reportButton_MouseClick);
            // 
            // queryOverviewButton
            // 
            this.queryOverviewButton.Description = "View most frequently executed queries over time.";
            this.queryOverviewButton.Dock = System.Windows.Forms.DockStyle.Fill;
            this.queryOverviewButton.Location = new System.Drawing.Point(3, 174);
            this.queryOverviewButton.MinimumSize = new System.Drawing.Size(0, 40);
            this.queryOverviewButton.Name = "queryOverviewButton";
            this.queryOverviewButton.Size = new System.Drawing.Size(248, 51);
            this.queryOverviewButton.TabIndex = 47;
            this.queryOverviewButton.Tag = "QueryOverview";
            this.queryOverviewButton.Title = "Query Overview";
            this.queryOverviewButton.TitleColor = System.Drawing.Color.FromArgb(((int)(((byte)(53)))), ((int)(((byte)(53)))), ((int)(((byte)(53)))));
            this.queryOverviewButton.Load += new System.EventHandler(this.reportButton_Load);
            this.queryOverviewButton.MouseClick += new System.Windows.Forms.MouseEventHandler(this.reportButton_MouseClick);
            // 
            // baselineStatisticsButton
            // 
            this.baselineStatisticsButton.Description = "Analyze and compare performance baselines across two SQL Server instances.";
            this.baselineStatisticsButton.Dock = System.Windows.Forms.DockStyle.Fill;
            this.baselineStatisticsButton.Location = new System.Drawing.Point(3, 345);
            this.baselineStatisticsButton.MinimumSize = new System.Drawing.Size(0, 40);
            this.baselineStatisticsButton.Name = "baselineStatisticsButton";
            this.baselineStatisticsButton.Size = new System.Drawing.Size(248, 51);
            this.baselineStatisticsButton.TabIndex = 42;
            this.baselineStatisticsButton.Tag = "BaselineStatistics";
            this.baselineStatisticsButton.Title = "Baseline Statistics";
            this.baselineStatisticsButton.TitleColor = System.Drawing.Color.FromArgb(((int)(((byte)(53)))), ((int)(((byte)(53)))), ((int)(((byte)(53)))));
            this.baselineStatisticsButton.Load += new System.EventHandler(this.reportButton_Load);
            this.baselineStatisticsButton.MouseClick += new System.Windows.Forms.MouseEventHandler(this.reportButton_MouseClick);
            //Start: SQLdm 8.6 (Vineet Kumar): Added for New Query Wait Stats Report. Initialising the properties of new report button (Query Wait Statistics Report)
            // queryWaitStatisticsButton
            // 
            this.queryWaitStatisticsButton.Description = "Provides wait stats for a single instance over a selected time period";
            this.queryWaitStatisticsButton.Dock = System.Windows.Forms.DockStyle.Fill;
            this.queryWaitStatisticsButton.Location = new System.Drawing.Point(3, 345);
            this.queryWaitStatisticsButton.MinimumSize = new System.Drawing.Size(0, 40);
            this.queryWaitStatisticsButton.Name = "queryWaitStatisticsButton";
            this.queryWaitStatisticsButton.Size = new System.Drawing.Size(248, 51);
            this.queryWaitStatisticsButton.TabIndex = 42;
            this.queryWaitStatisticsButton.Tag = "QueryWaitStatistics";
            this.queryWaitStatisticsButton.Title = "Query Wait Statistics";
            this.queryWaitStatisticsButton.TitleColor = System.Drawing.Color.FromArgb(((int)(((byte)(53)))), ((int)(((byte)(53)))), ((int)(((byte)(53)))));
            this.queryWaitStatisticsButton.Load += new System.EventHandler(this.reportButton_Load);
            this.queryWaitStatisticsButton.MouseClick += new System.Windows.Forms.MouseEventHandler(this.reportButton_MouseClick);
            //End: SQLdm 8.6 (Vineet Kumar): Added for New Query Wait Stats Report. Initialising the properties of new report button (Query Wait Statistics Report)               
            // 
            // analyzeResourcesReportsPanel
            // 
            this.analyzeResourcesReportsPanel.AutoScroll = true;
            this.analyzeResourcesReportsPanel.ColumnCount = 1;
            this.analyzeResourcesReportsPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.analyzeResourcesReportsPanel.Controls.Add(this.replicationStatisticsButton, 0, 5);
            this.analyzeResourcesReportsPanel.Controls.Add(this.diskStatisticsButton, 0, 4);
            this.analyzeResourcesReportsPanel.Controls.Add(this.cpuStatisticsButton, 0, 2);
            this.analyzeResourcesReportsPanel.Controls.Add(this.sessionStatisticsButton, 0, 0);
            //SQLDM-28817
            this.analyzeResourcesReportsPanel.Controls.Add(this.detailedSessionReport, 0, 1);
            this.analyzeResourcesReportsPanel.Controls.Add(this.memoryStatisticsButton, 0, 6);
            this.analyzeResourcesReportsPanel.Controls.Add(this.diskSpaceUsageButton, 0, 7);
            this.analyzeResourcesReportsPanel.Controls.Add(this.diskSpaceHistoryButton, 0, 8);
            this.analyzeResourcesReportsPanel.Controls.Add(this.diskDetailsButton, 0, 3);
            this.analyzeResourcesReportsPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.analyzeResourcesReportsPanel.Location = new System.Drawing.Point(0, 0);
            this.analyzeResourcesReportsPanel.Margin = new System.Windows.Forms.Padding(0);
            this.analyzeResourcesReportsPanel.Name = "analyzeResourcesReportsPanel";
            this.analyzeResourcesReportsPanel.RowCount = 10;
            this.analyzeResourcesReportsPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 0F));
            this.analyzeResourcesReportsPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 0F));
            this.analyzeResourcesReportsPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 0F));
            this.analyzeResourcesReportsPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 0F));
            this.analyzeResourcesReportsPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 0F));
            this.analyzeResourcesReportsPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 0F));
            this.analyzeResourcesReportsPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 0F));
            this.analyzeResourcesReportsPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 0F));
            this.analyzeResourcesReportsPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 0F));
            this.analyzeResourcesReportsPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 0F));
            this.analyzeResourcesReportsPanel.Size = new System.Drawing.Size(254, 398);
            this.analyzeResourcesReportsPanel.TabIndex = 2;
            this.analyzeResourcesReportsPanel.Visible = false;
            // 
            // replicationStatisticsButton
            // 
            this.replicationStatisticsButton.Description = "View trends for key replication performance metrics.";
            this.replicationStatisticsButton.Dock = System.Windows.Forms.DockStyle.Fill;
            this.replicationStatisticsButton.Location = new System.Drawing.Point(3, 251);
            this.replicationStatisticsButton.MinimumSize = new System.Drawing.Size(0, 40);
            this.replicationStatisticsButton.Name = "replicationStatisticsButton";
            this.replicationStatisticsButton.Size = new System.Drawing.Size(248, 56);
            this.replicationStatisticsButton.TabIndex = 51;
            this.replicationStatisticsButton.Tag = "ReplicationSummary";
            this.replicationStatisticsButton.Title = "Replication Statistics";
            this.replicationStatisticsButton.TitleColor = System.Drawing.Color.FromArgb(((int)(((byte)(53)))), ((int)(((byte)(53)))), ((int)(((byte)(53)))));
            this.replicationStatisticsButton.Load += new System.EventHandler(this.reportButton_Load);
            this.replicationStatisticsButton.MouseClick += new System.Windows.Forms.MouseEventHandler(this.reportButton_MouseClick);
            // 
            // diskStatisticsButton
            // 
            this.diskStatisticsButton.Description = "View trends for key disk performance metrics.";
            this.diskStatisticsButton.Dock = System.Windows.Forms.DockStyle.Fill;
            this.diskStatisticsButton.Location = new System.Drawing.Point(3, 189);
            this.diskStatisticsButton.MinimumSize = new System.Drawing.Size(0, 40);
            this.diskStatisticsButton.Name = "diskStatisticsButton";
            this.diskStatisticsButton.Size = new System.Drawing.Size(248, 56);
            this.diskStatisticsButton.TabIndex = 50;
            this.diskStatisticsButton.Tag = "DiskSummary";
            this.diskStatisticsButton.Title = "Disk Statistics";
            this.diskStatisticsButton.TitleColor = System.Drawing.Color.FromArgb(((int)(((byte)(53)))), ((int)(((byte)(53)))), ((int)(((byte)(53)))));
            this.diskStatisticsButton.Load += new System.EventHandler(this.reportButton_Load);
            this.diskStatisticsButton.MouseClick += new System.Windows.Forms.MouseEventHandler(this.reportButton_MouseClick);
            // 
            // cpuStatisticsButton
            // 
            this.cpuStatisticsButton.Description = "View trends for key CPU performance metrics.";
            this.cpuStatisticsButton.Dock = System.Windows.Forms.DockStyle.Fill;
            this.cpuStatisticsButton.Location = new System.Drawing.Point(3, 65);
            this.cpuStatisticsButton.MinimumSize = new System.Drawing.Size(0, 40);
            this.cpuStatisticsButton.Name = "cpuStatisticsButton";
            this.cpuStatisticsButton.Size = new System.Drawing.Size(248, 56);
            this.cpuStatisticsButton.TabIndex = 47;
            this.cpuStatisticsButton.Tag = "CPUSummary";
            this.cpuStatisticsButton.Title = "CPU Statistics";
            this.cpuStatisticsButton.TitleColor = System.Drawing.Color.FromArgb(((int)(((byte)(53)))), ((int)(((byte)(53)))), ((int)(((byte)(53)))));
            this.cpuStatisticsButton.Load += new System.EventHandler(this.reportButton_Load);
            this.cpuStatisticsButton.MouseClick += new System.Windows.Forms.MouseEventHandler(this.reportButton_MouseClick);
            // 
            // sessionStatisticsButton
            // 
            this.sessionStatisticsButton.Description = "View trends for key session and network related performance metrics.";
            this.sessionStatisticsButton.Dock = System.Windows.Forms.DockStyle.Fill;
            this.sessionStatisticsButton.Location = new System.Drawing.Point(3, 3);
            this.sessionStatisticsButton.MinimumSize = new System.Drawing.Size(0, 40);
            this.sessionStatisticsButton.Name = "sessionStatisticsButton";
            this.sessionStatisticsButton.Size = new System.Drawing.Size(248, 56);
            this.sessionStatisticsButton.TabIndex = 46;
            this.sessionStatisticsButton.Tag = "SessionsSummary";
            this.sessionStatisticsButton.Title = "Session Statistics";
            this.sessionStatisticsButton.TitleColor = System.Drawing.Color.FromArgb(((int)(((byte)(53)))), ((int)(((byte)(53)))), ((int)(((byte)(53)))));
            this.sessionStatisticsButton.Load += new System.EventHandler(this.reportButton_Load);
            this.sessionStatisticsButton.MouseClick += new System.Windows.Forms.MouseEventHandler(this.reportButton_MouseClick);
            // 
            // SQLDM-28817
            // DetailedSessionsReport
            // 
            this.detailedSessionReport.Description = "View Detailed Session Report for Monitored Servers.";
            this.detailedSessionReport.Dock = System.Windows.Forms.DockStyle.Fill;
            this.detailedSessionReport.Location = new System.Drawing.Point(3, 3);
            this.detailedSessionReport.MinimumSize = new System.Drawing.Size(0, 40);
            this.detailedSessionReport.Name = "DetailedSessionReport";
            this.detailedSessionReport.Size = new System.Drawing.Size(248, 56);
            this.detailedSessionReport.TabIndex = 47;
            this.detailedSessionReport.Tag = "DetailedSessionReport";
            this.detailedSessionReport.Title = "Detailed Session Report";
            this.detailedSessionReport.TitleColor = System.Drawing.Color.FromArgb(((int)(((byte)(53)))), ((int)(((byte)(53)))), ((int)(((byte)(53)))));
            this.detailedSessionReport.Load += new System.EventHandler(this.reportButton_Load);
            this.detailedSessionReport.MouseClick += new System.Windows.Forms.MouseEventHandler(this.reportButton_MouseClick);
            // 
            // memoryStatisticsButton
            // 
            this.memoryStatisticsButton.Description = "View trends for key memory performance metrics.";
            this.memoryStatisticsButton.Dock = System.Windows.Forms.DockStyle.Fill;
            this.memoryStatisticsButton.Location = new System.Drawing.Point(3, 313);
            this.memoryStatisticsButton.MinimumSize = new System.Drawing.Size(0, 40);
            this.memoryStatisticsButton.Name = "memoryStatisticsButton";
            this.memoryStatisticsButton.Size = new System.Drawing.Size(248, 56);
            this.memoryStatisticsButton.TabIndex = 48;
            this.memoryStatisticsButton.Tag = "MemorySummary";
            this.memoryStatisticsButton.Title = "Memory Statistics";
            this.memoryStatisticsButton.TitleColor = System.Drawing.Color.FromArgb(((int)(((byte)(53)))), ((int)(((byte)(53)))), ((int)(((byte)(53)))));
            this.memoryStatisticsButton.Load += new System.EventHandler(this.reportButton_Load);
            this.memoryStatisticsButton.MouseClick += new System.Windows.Forms.MouseEventHandler(this.reportButton_MouseClick);
            // 
            // diskSpaceUsageButton
            // 
            this.diskSpaceUsageButton.Description = "View disk space usage data.";
            this.diskSpaceUsageButton.Dock = System.Windows.Forms.DockStyle.Fill;
            this.diskSpaceUsageButton.Location = new System.Drawing.Point(3, 313);
            this.diskSpaceUsageButton.MinimumSize = new System.Drawing.Size(0, 40);
            this.diskSpaceUsageButton.Name = "diskSpaceUsageButton";
            this.diskSpaceUsageButton.Size = new System.Drawing.Size(248, 56);
            this.diskSpaceUsageButton.TabIndex = 49;
            this.diskSpaceUsageButton.Tag = "DiskSpaceUsage";
            this.diskSpaceUsageButton.Title = "Disk Space Usage";
            this.diskSpaceUsageButton.TitleColor = System.Drawing.Color.FromArgb(((int)(((byte)(53)))), ((int)(((byte)(53)))), ((int)(((byte)(53)))));
            this.diskSpaceUsageButton.Load += new System.EventHandler(this.reportButton_Load);
            this.diskSpaceUsageButton.MouseClick += new System.Windows.Forms.MouseEventHandler(this.reportButton_MouseClick);
            // 
            // diskSpaceHistoryButton
            // 
            this.diskSpaceHistoryButton.Description = "View disk space history data.";
            this.diskSpaceHistoryButton.Dock = System.Windows.Forms.DockStyle.Fill;
            this.diskSpaceHistoryButton.Location = new System.Drawing.Point(3, 313);
            this.diskSpaceHistoryButton.MinimumSize = new System.Drawing.Size(0, 40);
            this.diskSpaceHistoryButton.Name = "diskSpaceHistoryButton";
            this.diskSpaceHistoryButton.Size = new System.Drawing.Size(248, 56);
            this.diskSpaceHistoryButton.TabIndex = 50;
            this.diskSpaceHistoryButton.Tag = "DiskSpaceHistory";
            this.diskSpaceHistoryButton.Title = "Disk Space History";
            this.diskSpaceHistoryButton.TitleColor = System.Drawing.Color.FromArgb(((int)(((byte)(53)))), ((int)(((byte)(53)))), ((int)(((byte)(53)))));
            this.diskSpaceHistoryButton.Load += new System.EventHandler(this.reportButton_Load);
            this.diskSpaceHistoryButton.MouseClick += new System.Windows.Forms.MouseEventHandler(this.reportButton_MouseClick);
            // 
            // diskDetailsButton
            // 
            this.diskDetailsButton.Description = "View key disk metrics for a server.";
            this.diskDetailsButton.Dock = System.Windows.Forms.DockStyle.Fill;
            this.diskDetailsButton.Location = new System.Drawing.Point(3, 127);
            this.diskDetailsButton.MinimumSize = new System.Drawing.Size(0, 40);
            this.diskDetailsButton.Name = "diskDetailsButton";
            this.diskDetailsButton.Size = new System.Drawing.Size(248, 56);
            this.diskDetailsButton.TabIndex = 51;
            this.diskDetailsButton.Tag = "DiskDetails";
            this.diskDetailsButton.Title = "Disk Details";
            this.diskDetailsButton.TitleColor = System.Drawing.Color.FromArgb(((int)(((byte)(53)))), ((int)(((byte)(53)))), ((int)(((byte)(53)))));
            this.diskDetailsButton.Load += new System.EventHandler(this.reportButton_Load);
            this.diskDetailsButton.MouseClick += new System.Windows.Forms.MouseEventHandler(this.reportButton_MouseClick);
            // 
            // panel5
            // 
            this.panel5.Controls.Add(this.analyzeResourcesButtonPanel);
            this.panel5.Controls.Add(this.analyzeDatabasesButtonPanel);
            this.panel5.Controls.Add(this.analyzeServersButtonPanel);
            this.panel5.Controls.Add(this.label7);
            this.panel5.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel5.Location = new System.Drawing.Point(0, 0);
            this.panel5.Margin = new System.Windows.Forms.Padding(0);
            this.panel5.Name = "panel5";
            this.panel5.Size = new System.Drawing.Size(254, 25);
            this.panel5.TabIndex = 1;
            // 
            // analyzeResourcesButtonPanel
            // 
            this.analyzeResourcesButtonPanel.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.analyzeResourcesButtonPanel.BackColor = System.Drawing.Color.White;
            this.analyzeResourcesButtonPanel.BorderColor = System.Drawing.Color.White;
            this.analyzeResourcesButtonPanel.Controls.Add(this.analyzeResourcesLabel);
            this.analyzeResourcesButtonPanel.FillColor = System.Drawing.Color.White;
            this.analyzeResourcesButtonPanel.FillColor2 = System.Drawing.Color.Empty;
            this.analyzeResourcesButtonPanel.Location = new System.Drawing.Point(171, 0);
            this.analyzeResourcesButtonPanel.Name = "analyzeResourcesButtonPanel";
            this.analyzeResourcesButtonPanel.Radius = 3F;
            this.analyzeResourcesButtonPanel.Size = new System.Drawing.Size(75, 23);
            this.analyzeResourcesButtonPanel.TabIndex = 2;
            // 
            // analyzeResourcesLabel
            // 
            this.analyzeResourcesLabel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.analyzeResourcesLabel.AutoSize = true;
            this.analyzeResourcesLabel.BackColor = System.Drawing.Color.Transparent;
            this.analyzeResourcesLabel.Cursor = System.Windows.Forms.Cursors.Hand;
            this.analyzeResourcesLabel.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.analyzeResourcesLabel.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(75)))), ((int)(((byte)(75)))), ((int)(((byte)(75)))));
            this.analyzeResourcesLabel.Location = new System.Drawing.Point(3, 4);
            this.analyzeResourcesLabel.Name = "analyzeResourcesLabel";
            this.analyzeResourcesLabel.Size = new System.Drawing.Size(69, 15);
            this.analyzeResourcesLabel.TabIndex = 6;
            this.analyzeResourcesLabel.Text = "Resources";
            this.analyzeResourcesLabel.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.analyzeResourcesLabel.MouseClick += new System.Windows.Forms.MouseEventHandler(this.analyzeTablesLabel_MouseClick);
            this.analyzeResourcesLabel.MouseEnter += new System.EventHandler(this.analyzeLabel_MouseEnter);
            this.analyzeResourcesLabel.MouseLeave += new System.EventHandler(this.analyzeLabel_MouseLeave);
            // 
            // analyzeDatabasesButtonPanel
            // 
            this.analyzeDatabasesButtonPanel.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.analyzeDatabasesButtonPanel.BackColor = System.Drawing.Color.White;
            this.analyzeDatabasesButtonPanel.BorderColor = System.Drawing.Color.White;
            this.analyzeDatabasesButtonPanel.Controls.Add(this.analyzeDatabasesLabel);
            this.analyzeDatabasesButtonPanel.FillColor = System.Drawing.Color.White;
            this.analyzeDatabasesButtonPanel.FillColor2 = System.Drawing.Color.Empty;
            this.analyzeDatabasesButtonPanel.Location = new System.Drawing.Point(90, 0);
            this.analyzeDatabasesButtonPanel.Name = "analyzeDatabasesButtonPanel";
            this.analyzeDatabasesButtonPanel.Radius = 3F;
            this.analyzeDatabasesButtonPanel.Size = new System.Drawing.Size(75, 23);
            this.analyzeDatabasesButtonPanel.TabIndex = 1;
            // 
            // analyzeDatabasesLabel
            // 
            this.analyzeDatabasesLabel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.analyzeDatabasesLabel.AutoSize = true;
            this.analyzeDatabasesLabel.BackColor = System.Drawing.Color.Transparent;
            this.analyzeDatabasesLabel.Cursor = System.Windows.Forms.Cursors.Hand;
            this.analyzeDatabasesLabel.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.analyzeDatabasesLabel.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(75)))), ((int)(((byte)(75)))), ((int)(((byte)(75)))));
            this.analyzeDatabasesLabel.Location = new System.Drawing.Point(3, 4);
            this.analyzeDatabasesLabel.Name = "analyzeDatabasesLabel";
            this.analyzeDatabasesLabel.Size = new System.Drawing.Size(68, 15);
            this.analyzeDatabasesLabel.TabIndex = 6;
            this.analyzeDatabasesLabel.Text = "Databases";
            this.analyzeDatabasesLabel.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.analyzeDatabasesLabel.MouseClick += new System.Windows.Forms.MouseEventHandler(this.analyzeDatabasesLabel_MouseClick);
            this.analyzeDatabasesLabel.MouseEnter += new System.EventHandler(this.analyzeLabel_MouseEnter);
            this.analyzeDatabasesLabel.MouseLeave += new System.EventHandler(this.analyzeLabel_MouseLeave);
            // 
            // analyzeServersButtonPanel
            // 
            this.analyzeServersButtonPanel.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.analyzeServersButtonPanel.BackColor = System.Drawing.Color.White;
            this.analyzeServersButtonPanel.BorderColor = System.Drawing.Color.White;
            this.analyzeServersButtonPanel.Controls.Add(this.analyzeServersLabel);
            this.analyzeServersButtonPanel.FillColor = System.Drawing.Color.FromArgb(((int)(((byte)(228)))), ((int)(((byte)(228)))), ((int)(((byte)(228)))));
            this.analyzeServersButtonPanel.FillColor2 = System.Drawing.Color.Empty;
            this.analyzeServersButtonPanel.Location = new System.Drawing.Point(9, 0);
            this.analyzeServersButtonPanel.Name = "analyzeServersButtonPanel";
            this.analyzeServersButtonPanel.Radius = 3F;
            this.analyzeServersButtonPanel.Size = new System.Drawing.Size(75, 23);
            this.analyzeServersButtonPanel.TabIndex = 0;
            // 
            // analyzeServersLabel
            // 
            this.analyzeServersLabel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.analyzeServersLabel.AutoSize = true;
            this.analyzeServersLabel.BackColor = System.Drawing.Color.Transparent;
            this.analyzeServersLabel.Cursor = System.Windows.Forms.Cursors.Hand;
            this.analyzeServersLabel.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.analyzeServersLabel.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(75)))), ((int)(((byte)(75)))), ((int)(((byte)(75)))));
            this.analyzeServersLabel.Location = new System.Drawing.Point(11, 4);
            this.analyzeServersLabel.Name = "analyzeServersLabel";
            this.analyzeServersLabel.Size = new System.Drawing.Size(52, 15);
            this.analyzeServersLabel.TabIndex = 6;
            this.analyzeServersLabel.Text = "Servers";
            this.analyzeServersLabel.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.analyzeServersLabel.MouseClick += new System.Windows.Forms.MouseEventHandler(this.analyzeServersLabel_MouseClick);
            this.analyzeServersLabel.MouseEnter += new System.EventHandler(this.analyzeLabel_MouseEnter);
            this.analyzeServersLabel.MouseLeave += new System.EventHandler(this.analyzeLabel_MouseLeave);
            // 
            // label7
            // 
            this.label7.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(215)))), ((int)(((byte)(215)))), ((int)(((byte)(215)))));
            this.label7.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.label7.Location = new System.Drawing.Point(0, 24);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(254, 1);
            this.label7.TabIndex = 3;
            // 
            // roundedPanel5
            // 
            this.roundedPanel5.BackColor = System.Drawing.Color.White;
            this.roundedPanel5.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(215)))), ((int)(((byte)(215)))), ((int)(((byte)(215)))));
            this.roundedPanel5.Controls.Add(this.label5);
            this.roundedPanel5.Dock = System.Windows.Forms.DockStyle.Top;
            this.roundedPanel5.FillColor = System.Drawing.Color.FromArgb(((int)(((byte)(228)))), ((int)(((byte)(228)))), ((int)(((byte)(228)))));
            this.roundedPanel5.FillColor2 = System.Drawing.Color.Empty;
            this.roundedPanel5.Location = new System.Drawing.Point(0, 0);
            this.roundedPanel5.Name = "roundedPanel5";
            this.roundedPanel5.Radius = 3F;
            this.roundedPanel5.Size = new System.Drawing.Size(260, 40);
            this.roundedPanel5.TabIndex = 5;
            // 
            // label5
            // 
            this.label5.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.label5.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(228)))), ((int)(((byte)(228)))), ((int)(((byte)(228)))));
            this.label5.Font = new System.Drawing.Font("Arial", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label5.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(75)))), ((int)(((byte)(75)))), ((int)(((byte)(75)))));
            this.label5.Location = new System.Drawing.Point(9, 11);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(242, 19);
            this.label5.TabIndex = 5;
            this.label5.Text = "Analyze";
            this.label5.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // planPanel
            // 
            this.planPanel.Anchor = System.Windows.Forms.AnchorStyles.Top;
            this.planPanel.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(215)))), ((int)(((byte)(215)))), ((int)(((byte)(215)))));
            this.planPanel.Controls.Add(this.tableLayoutPanel2);
            this.planPanel.Controls.Add(this.roundedPanel2);
            this.planPanel.FillColor = System.Drawing.Color.White;
            this.planPanel.FillColor2 = System.Drawing.Color.Empty;
            this.planPanel.Location = new System.Drawing.Point(621, 15);
            this.planPanel.Name = "planPanel";
            this.planPanel.Radius = 3F;
            this.planPanel.Size = new System.Drawing.Size(260, 467);
            this.planPanel.TabIndex = 16;
            // 
            // tableLayoutPanel2
            // 
            this.tableLayoutPanel2.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tableLayoutPanel2.ColumnCount = 1;
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel2.Controls.Add(this.tableGrowthForecastButton, 0, 3);
            this.tableLayoutPanel2.Controls.Add(this.diskSpaceForecastButton, 0, 1);
            this.tableLayoutPanel2.Controls.Add(this.databaseGrowthForecastButton, 0, 2);
            this.tableLayoutPanel2.Location = new System.Drawing.Point(3, 41);
            this.tableLayoutPanel2.Name = "tableLayoutPanel2";
            this.tableLayoutPanel2.RowCount = 7;
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 0F));
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 16.66667F));
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 16.66667F));
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 16.66667F));
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 16.66667F));
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 16.66667F));
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 16.66667F));
            this.tableLayoutPanel2.Size = new System.Drawing.Size(254, 423);
            this.tableLayoutPanel2.TabIndex = 7;
            // 
            // tableGrowthForecastButton
            // 
            this.tableGrowthForecastButton.Description = "Forecast future table growth based on recent trends.";
            this.tableGrowthForecastButton.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableGrowthForecastButton.Location = new System.Drawing.Point(3, 143);
            this.tableGrowthForecastButton.MinimumSize = new System.Drawing.Size(0, 40);
            this.tableGrowthForecastButton.Name = "tableGrowthForecastButton";
            this.tableGrowthForecastButton.Size = new System.Drawing.Size(248, 64);
            this.tableGrowthForecastButton.TabIndex = 44;
            this.tableGrowthForecastButton.Tag = "TableGrowthForecast";
            this.tableGrowthForecastButton.Title = "Table Growth Forecast";
            this.tableGrowthForecastButton.TitleColor = System.Drawing.Color.FromArgb(((int)(((byte)(53)))), ((int)(((byte)(53)))), ((int)(((byte)(53)))));
            this.tableGrowthForecastButton.Load += new System.EventHandler(this.reportButton_Load);
            this.tableGrowthForecastButton.MouseClick += new System.Windows.Forms.MouseEventHandler(this.reportButton_MouseClick);
            // 
            // diskSpaceForecastButton
            // 
            this.diskSpaceForecastButton.Description = "Forecast disk space usage based on average growth rates and see when you\'ll run o" +
    "ut.";
            this.diskSpaceForecastButton.Dock = System.Windows.Forms.DockStyle.Fill;
            this.diskSpaceForecastButton.Location = new System.Drawing.Point(3, 3);
            this.diskSpaceForecastButton.MinimumSize = new System.Drawing.Size(0, 40);
            this.diskSpaceForecastButton.Name = "diskSpaceForecastButton";
            this.diskSpaceForecastButton.Size = new System.Drawing.Size(248, 64);
            this.diskSpaceForecastButton.TabIndex = 42;
            this.diskSpaceForecastButton.Tag = "DiskSpaceForecast";
            this.diskSpaceForecastButton.Title = "Disk Space Usage Forecast";
            this.diskSpaceForecastButton.TitleColor = System.Drawing.Color.FromArgb(((int)(((byte)(53)))), ((int)(((byte)(53)))), ((int)(((byte)(53)))));
            this.diskSpaceForecastButton.Load += new System.EventHandler(this.reportButton_Load);
            this.diskSpaceForecastButton.MouseClick += new System.Windows.Forms.MouseEventHandler(this.reportButton_MouseClick);
            // 
            // databaseGrowthForecastButton
            // 
            this.databaseGrowthForecastButton.Description = "Forecast future database growth based on recent trends.";
            this.databaseGrowthForecastButton.Dock = System.Windows.Forms.DockStyle.Fill;
            this.databaseGrowthForecastButton.Location = new System.Drawing.Point(3, 73);
            this.databaseGrowthForecastButton.MinimumSize = new System.Drawing.Size(0, 40);
            this.databaseGrowthForecastButton.Name = "databaseGrowthForecastButton";
            this.databaseGrowthForecastButton.Size = new System.Drawing.Size(248, 64);
            this.databaseGrowthForecastButton.TabIndex = 43;
            this.databaseGrowthForecastButton.Tag = "DatabaseGrowthForecast";
            this.databaseGrowthForecastButton.Title = "Database Growth Forecast";
            this.databaseGrowthForecastButton.TitleColor = System.Drawing.Color.FromArgb(((int)(((byte)(53)))), ((int)(((byte)(53)))), ((int)(((byte)(53)))));
            this.databaseGrowthForecastButton.Load += new System.EventHandler(this.reportButton_Load);
            this.databaseGrowthForecastButton.MouseClick += new System.Windows.Forms.MouseEventHandler(this.reportButton_MouseClick);
            // 
            // roundedPanel2
            // 
            this.roundedPanel2.BackColor = System.Drawing.Color.White;
            this.roundedPanel2.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(215)))), ((int)(((byte)(215)))), ((int)(((byte)(215)))));
            this.roundedPanel2.Controls.Add(this.label6);
            this.roundedPanel2.Dock = System.Windows.Forms.DockStyle.Top;
            this.roundedPanel2.FillColor = System.Drawing.Color.FromArgb(((int)(((byte)(228)))), ((int)(((byte)(228)))), ((int)(((byte)(228)))));
            this.roundedPanel2.FillColor2 = System.Drawing.Color.Empty;
            this.roundedPanel2.Location = new System.Drawing.Point(0, 0);
            this.roundedPanel2.Name = "roundedPanel2";
            this.roundedPanel2.Radius = 3F;
            this.roundedPanel2.Size = new System.Drawing.Size(260, 40);
            this.roundedPanel2.TabIndex = 5;
            // 
            // label6
            // 
            this.label6.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.label6.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(228)))), ((int)(((byte)(228)))), ((int)(((byte)(228)))));
            this.label6.Font = new System.Drawing.Font("Arial", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label6.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(75)))), ((int)(((byte)(75)))), ((int)(((byte)(75)))));
            this.label6.Location = new System.Drawing.Point(9, 11);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(242, 19);
            this.label6.TabIndex = 5;
            this.label6.Text = "Plan";
            this.label6.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // headerPanel
            // 
            this.headerPanel.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(236)))), ((int)(((byte)(235)))), ((int)(((byte)(234)))));
            this.headerPanel.Controls.Add(this.pictureBox1);
            this.headerPanel.Dock = System.Windows.Forms.DockStyle.Top;
            this.headerPanel.Location = new System.Drawing.Point(0, 0);
            this.headerPanel.Name = "headerPanel";
            this.headerPanel.Padding = new System.Windows.Forms.Padding(1);
            this.headerPanel.Size = new System.Drawing.Size(970, 45);
            this.headerPanel.TabIndex = 18;
            // 
            // pictureBox1
            // 
            this.pictureBox1.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.pictureBox1.Image = global::Idera.SQLdm.DesktopClient.Properties.Resources.TodayPageHeader;
            this.pictureBox1.Location = new System.Drawing.Point(0, 0);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(532, 45);
            this.pictureBox1.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.pictureBox1.TabIndex = 19;
            this.pictureBox1.TabStop = false;
            // 
            // GettingStartedControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoScroll = true;
            this.BackColor = System.Drawing.Color.White;
            this.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.Controls.Add(this.panel6);
            this.Controls.Add(this.panel2);
            this.Controls.Add(this.headerPanel);
            this.Name = "GettingStartedControl";
            this.Size = new System.Drawing.Size(970, 691);
            this.panel2.ResumeLayout(false);
            this.panel2.PerformLayout();
            this.panel6.ResumeLayout(false);
            this.deployPanel.ResumeLayout(false);
            this.deployPanel.PerformLayout();
            this.monitorPanel.ResumeLayout(false);
            this.tableLayoutPanel1.ResumeLayout(false);
            this.panel1.ResumeLayout(false);
            this.monitorSqldmActivityButtonPanel.ResumeLayout(false);
            this.monitorSqldmActivityButtonPanel.PerformLayout();
            this.monitorVirtualizationButtonPanel.ResumeLayout(false);
            this.monitorVirtualizationButtonPanel.PerformLayout();
            this.monitorServersButtonPanel.ResumeLayout(false);
            this.monitorServersButtonPanel.PerformLayout();
            this.panel4.ResumeLayout(false);
            this.monitorServerReportsPanel.ResumeLayout(false);
            this.monitorVirtualizationReportsPanel.ResumeLayout(false);
            this.monitorSqldmActivityReportsPanel.ResumeLayout(false);
            this.roundedPanel6.ResumeLayout(false);
            this.discoverPanel.ResumeLayout(false);
            this.tableLayoutPanel3.ResumeLayout(false);
            this.panel3.ResumeLayout(false);
            this.analyzeDatabasesReportsPanel.ResumeLayout(false);
            this.analyzeServersReportsPanel.ResumeLayout(false);
            this.analyzeResourcesReportsPanel.ResumeLayout(false);
            this.panel5.ResumeLayout(false);
            this.analyzeResourcesButtonPanel.ResumeLayout(false);
            this.analyzeResourcesButtonPanel.PerformLayout();
            this.analyzeDatabasesButtonPanel.ResumeLayout(false);
            this.analyzeDatabasesButtonPanel.PerformLayout();
            this.analyzeServersButtonPanel.ResumeLayout(false);
            this.analyzeServersButtonPanel.PerformLayout();
            this.roundedPanel5.ResumeLayout(false);
            this.planPanel.ResumeLayout(false);
            this.tableLayoutPanel2.ResumeLayout(false);
            this.roundedPanel2.ResumeLayout(false);
            this.headerPanel.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private Idera.SQLdm.DesktopClient.Controls.ReportSelectionButton activeAlertsButton;
        private Idera.SQLdm.DesktopClient.Controls.ReportSelectionButton serverSummaryButton;
        private Idera.SQLdm.DesktopClient.Controls.ReportSelectionButton enterpriseSummaryButton;
        private Idera.SQLdm.DesktopClient.Controls.ReportSelectionButton databaseGrowthForecastButton;
        private Idera.SQLdm.DesktopClient.Controls.ReportSelectionButton diskSpaceForecastButton;
        private Idera.SQLdm.DesktopClient.Controls.ReportSelectionButton baselineStatisticsButton;
        private Idera.SQLdm.DesktopClient.Controls.RoundedPanel monitorPanel;
        private Idera.SQLdm.DesktopClient.Controls.RoundedPanel roundedPanel6;
        private System.Windows.Forms.Label label8;
        private Idera.SQLdm.DesktopClient.Controls.RoundedPanel discoverPanel;
        private Idera.SQLdm.DesktopClient.Controls.RoundedPanel roundedPanel5;
        private System.Windows.Forms.Label label5;
        private Idera.SQLdm.DesktopClient.Controls.RoundedPanel planPanel;
        private Idera.SQLdm.DesktopClient.Controls.RoundedPanel roundedPanel2;
        private System.Windows.Forms.Label label6;
        private Idera.SQLdm.DesktopClient.Controls.ReportSelectionButton topTablesFragmentedButton;
        private Idera.SQLdm.DesktopClient.Controls.ReportSelectionButton topTablesGrowthButton;
        private Idera.SQLdm.DesktopClient.Controls.ReportSelectionButton topQueriesButton;
        private Idera.SQLdm.DesktopClient.Controls.ReportSelectionButton serverStatisticsButton;
        private Idera.SQLdm.DesktopClient.Controls.ReportSelectionButton serverInventoryButton;
        private Idera.SQLdm.DesktopClient.Controls.ReportSelectionButton tableGrowthForecastButton;
        private Idera.SQLdm.DesktopClient.Controls.ReportSelectionButton databaseAppsButton;
        private Idera.SQLdm.DesktopClient.Controls.ReportSelectionButton alertSummaryButton;
        private Idera.SQLdm.DesktopClient.Controls.ReportSelectionButton serverUptimeButton;
        private Idera.SQLdm.DesktopClient.Controls.ReportSelectionButton topDatabasesButton;
        private Idera.SQLdm.DesktopClient.Controls.ReportSelectionButton databaseStatsButton;
        private Idera.SQLdm.DesktopClient.Controls.ReportSelectionButton topServersButton;
        private Idera.SQLdm.DesktopClient.Controls.ReportSelectionButton queryOverviewButton;
        //Start: SQLdm 8.6 (Vineet Kumar): Added for Query Wait Stats Report
        private Idera.SQLdm.DesktopClient.Controls.ReportSelectionButton queryWaitStatisticsButton;
        //End: SQLdm 8.6 (Vineet Kumar): Added for Query Wait Stats Report
        private Idera.SQLdm.DesktopClient.Controls.RoundedPanel deployPanel;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.LinkLabel deployReportsLinkLabel;
        private System.Windows.Forms.TableLayoutPanel monitorServerReportsPanel;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel2;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel3;
        private System.Windows.Forms.Panel panel3;
        private System.Windows.Forms.Panel panel5;
        private Idera.SQLdm.DesktopClient.Controls.RoundedPanel analyzeServersButtonPanel;
        private System.Windows.Forms.Label analyzeServersLabel;
        private Idera.SQLdm.DesktopClient.Controls.RoundedPanel analyzeDatabasesButtonPanel;
        private System.Windows.Forms.Label analyzeDatabasesLabel;
        private Idera.SQLdm.DesktopClient.Controls.RoundedPanel analyzeResourcesButtonPanel;
        private System.Windows.Forms.Label analyzeResourcesLabel;
        private System.Windows.Forms.TableLayoutPanel analyzeServersReportsPanel;
        private System.Windows.Forms.TableLayoutPanel analyzeResourcesReportsPanel;
        private System.Windows.Forms.TableLayoutPanel analyzeDatabasesReportsPanel;
        private System.Windows.Forms.Label label7;
        private Idera.SQLdm.DesktopClient.Controls.ReportSelectionButton mirroringSummaryButton;
        private Idera.SQLdm.DesktopClient.Controls.ReportSelectionButton mirroringHistoryButton;
        private Idera.SQLdm.DesktopClient.Controls.ReportSelectionButton diskStatisticsButton;
        private Idera.SQLdm.DesktopClient.Controls.ReportSelectionButton memoryStatisticsButton;
        private Idera.SQLdm.DesktopClient.Controls.ReportSelectionButton diskSpaceUsageButton;
        private Idera.SQLdm.DesktopClient.Controls.ReportSelectionButton diskSpaceHistoryButton;
        private Idera.SQLdm.DesktopClient.Controls.ReportSelectionButton cpuStatisticsButton;
        private Idera.SQLdm.DesktopClient.Controls.ReportSelectionButton sessionStatisticsButton;
        //SQLDM-28817
        private Idera.SQLdm.DesktopClient.Controls.ReportSelectionButton detailedSessionReport;
        private Idera.SQLdm.DesktopClient.Controls.ReportSelectionButton replicationStatisticsButton;
        private Idera.SQLdm.DesktopClient.Controls.ReportSelectionButton metricThresholdButton;
        private Idera.SQLdm.DesktopClient.Controls.ReportSelectionButton alertTemplatesButton;
        private Idera.SQLdm.DesktopClient.Controls.ReportSelectionButton alertThresholdButton;
        private Idera.SQLdm.DesktopClient.Controls.ReportSelectionButton templateComparisonButton;
        private Idera.SQLdm.DesktopClient.Controls.ReportSelectionButton diskDetailsButton;
        private Idera.SQLdm.DesktopClient.Controls.ReportSelectionButton tempdbButton;
        private Idera.SQLdm.DesktopClient.Controls.ReportSelectionButton transactionLogStatisticsButton;
        private System.Windows.Forms.PictureBox pictureBox1;
        private System.Windows.Forms.Label headerLabel;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Panel panel4;
        private Controls.RoundedPanel monitorSqldmActivityButtonPanel;
        private System.Windows.Forms.Label sqldmActivityLabel;
        private Controls.RoundedPanel monitorVirtualizationButtonPanel;
        private System.Windows.Forms.Label monitorVirtualizationLabel;
        private Controls.RoundedPanel monitorServersButtonPanel;
        private System.Windows.Forms.Label monitorServersLabel;
        private System.Windows.Forms.TableLayoutPanel monitorVirtualizationReportsPanel;
        private Controls.ReportSelectionButton virtualizationSummaryButton;
        private Controls.ReportSelectionButton vStatisticsReportButton;
        private System.Windows.Forms.TableLayoutPanel monitorSqldmActivityReportsPanel;
        private Controls.ReportSelectionButton sqldmActivityReportButton;
        private System.Windows.Forms.Label label4;
        private Panel panel6;
        private Panel headerPanel;
        private Idera.SQLdm.DesktopClient.Controls.ReportSelectionButton alwaysOnTopologyButton;
        //SQLDM-28817
        private Idera.SQLdm.DesktopClient.Controls.ReportSelectionButton deadLockReportButton;
        private Idera.SQLdm.DesktopClient.Controls.ReportSelectionButton alwaysOnStatisticsButton;
    }
}
