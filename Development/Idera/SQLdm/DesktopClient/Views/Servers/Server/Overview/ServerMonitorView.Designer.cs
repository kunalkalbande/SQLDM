namespace Idera.SQLdm.DesktopClient.Views.Servers.Server.Overview
{
    partial class ServerMonitorView
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
            this.refreshBackgroundWorker = new System.ComponentModel.BackgroundWorker();
            this.areaLayoutPanel = new System.Windows.Forms.TableLayoutPanel();
            this.sessionsGroupPanel = new Idera.SQLdm.DesktopClient.Controls.GroupPanel();
            this.sessionsGroupLayoutPanel = new System.Windows.Forms.TableLayoutPanel();
            this.activeUserProcessesPanel = new System.Windows.Forms.Panel();
            this.activeUserProcessesLabel = new System.Windows.Forms.Label();
            this.activeUserProcessesButton = new Infragistics.Win.Misc.UltraButton();
            this.userProcessesPanel = new System.Windows.Forms.Panel();
            this.userProcessesButton = new Infragistics.Win.Misc.UltraButton();
            this.userProcessesLabel = new System.Windows.Forms.Label();
            this.clientComputersPanel = new System.Windows.Forms.Panel();
            this.clientComputersButton = new Infragistics.Win.Misc.UltraButton();
            this.clientComputersLabel = new System.Windows.Forms.Label();
            this.responseTimePanel = new System.Windows.Forms.Panel();
            this.responseTimeButton = new Infragistics.Win.Misc.UltraButton();
            this.responseTimeLabel = new System.Windows.Forms.Label();
            this.sessionsGroupLabel = new System.Windows.Forms.Label();
            this.diskGroupPanel = new Idera.SQLdm.DesktopClient.Controls.GroupPanel();
            this.diskGroupLayoutPanel = new System.Windows.Forms.TableLayoutPanel();
            this.diskQueueLengthPanel = new System.Windows.Forms.Panel();
            this.databaseGroupDividerLabel1 = new System.Windows.Forms.Label();
            this.diskQueueLengthMaximumLabel = new System.Windows.Forms.Label();
            this.diskQueueLengthMinimumLabel = new System.Windows.Forms.Label();
            this.diskQueueLengthStatusBar = new Infragistics.Win.UltraWinProgressBar.UltraProgressBar();
            this.diskQueueLengthLabel = new System.Windows.Forms.Label();
            this.logFilesPanel = new System.Windows.Forms.Panel();
            this.databaseGroupDividerLabel2 = new System.Windows.Forms.Label();
            this.logFilesCountValueLabel = new System.Windows.Forms.Label();
            this.logFilesSizeValueLabel = new System.Windows.Forms.Label();
            this.logFilesSizeLabel = new System.Windows.Forms.Label();
            this.logFilesCountLabel = new System.Windows.Forms.Label();
            this.logFilesLabel = new System.Windows.Forms.Label();
            this.logFilesPercentFullStatusBar = new Infragistics.Win.UltraWinProgressBar.UltraProgressBar();
            this.dataFilesPanel = new System.Windows.Forms.Panel();
            this.databaseGroupDividerLabel3 = new System.Windows.Forms.Label();
            this.dataFilesCountValueLabel = new System.Windows.Forms.Label();
            this.dataFilesSizeValueLabel = new System.Windows.Forms.Label();
            this.dataFilesSizeLabel = new System.Windows.Forms.Label();
            this.dataFilesCountLabel = new System.Windows.Forms.Label();
            this.dataFilesLabel = new System.Windows.Forms.Label();
            this.dataFilesPercentFullStatusBar = new Infragistics.Win.UltraWinProgressBar.UltraProgressBar();
            this.databasesPanel = new System.Windows.Forms.Panel();
            this.databasesButton = new Infragistics.Win.Misc.UltraButton();
            this.databasesLabel = new System.Windows.Forms.Label();
            this.databaseGroupLabel = new System.Windows.Forms.Label();
            this.memoryGroupPanel = new Idera.SQLdm.DesktopClient.Controls.GroupPanel();
            this.memoryGroupLayoutPanel = new System.Windows.Forms.TableLayoutPanel();
            this.pagingPanel = new System.Windows.Forms.Panel();
            this.memoryGroupDividerLabel1 = new System.Windows.Forms.Label();
            this.pagingValueLabel = new System.Windows.Forms.Label();
            this.pagingLabel = new System.Windows.Forms.Label();
            this.pagingFlowControl = new Idera.SQLdm.DesktopClient.Controls.InfiniteProgressCircle();
            this.procedureCachePanel = new System.Windows.Forms.Panel();
            this.memoryGroupDividerLabel2 = new System.Windows.Forms.Label();
            this.procedureCacheSizeLabel = new System.Windows.Forms.Label();
            this.procedureCacheHitRateLabel = new System.Windows.Forms.Label();
            this.procedureCacheHitRateStatusBar = new Infragistics.Win.UltraWinProgressBar.UltraProgressBar();
            this.procedureCacheLabel = new System.Windows.Forms.Label();
            this.bufferCachePanel = new System.Windows.Forms.Panel();
            this.memoryGroupDividerLabel3 = new System.Windows.Forms.Label();
            this.bufferCacheSizeLabel = new System.Windows.Forms.Label();
            this.bufferCacheHitRateLabel = new System.Windows.Forms.Label();
            this.bufferCacheHitRateStatusBar = new Infragistics.Win.UltraWinProgressBar.UltraProgressBar();
            this.bufferCacheLabel = new System.Windows.Forms.Label();
            this.sqlMemoryUsagePanel = new System.Windows.Forms.Panel();
            this.sqlMemoryAllocatedLabel = new System.Windows.Forms.Label();
            this.sqlMemoryUsageMinimumLabel = new System.Windows.Forms.Label();
            this.sqlMemoryUsageStatusBar = new Infragistics.Win.UltraWinProgressBar.UltraProgressBar();
            this.sqlMemoryUsageLabel = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.processorGroupPanel = new Idera.SQLdm.DesktopClient.Controls.GroupPanel();
            this.processorGroupLayoutPanel = new System.Windows.Forms.TableLayoutPanel();
            this.processorUsagePanel = new System.Windows.Forms.Panel();
            this.processorUsageLabel = new System.Windows.Forms.Label();
            this.processorUsageValueLabel = new System.Windows.Forms.Label();
            this.processorUsageFlowControl = new Idera.SQLdm.DesktopClient.Controls.InfiniteProgressCircle();
            this.processorQueueLengthPanel = new System.Windows.Forms.Panel();
            this.processorGroupDividerLabel1 = new System.Windows.Forms.Label();
            this.processorQueueLengthMaximumLabel = new System.Windows.Forms.Label();
            this.processorQueueLengthMinimumLabel = new System.Windows.Forms.Label();
            this.processorQueueLengthStatusBar = new Infragistics.Win.UltraWinProgressBar.UltraProgressBar();
            this.processorQueueLengthLabel = new System.Windows.Forms.Label();
            this.blockedProcessesPanel = new System.Windows.Forms.Panel();
            this.blockedProcessesButton = new Infragistics.Win.Misc.UltraButton();
            this.blockedProcessesLabel = new System.Windows.Forms.Label();
            this.sqlProcessesPanel = new System.Windows.Forms.Panel();
            this.sqlProcessesButton = new Infragistics.Win.Misc.UltraButton();
            this.sqlProcessesLabel = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.sessionsDataFlowPanel = new System.Windows.Forms.Panel();
            this.transactionsPerSecondFlowControl = new Idera.SQLdm.DesktopClient.Controls.InfiniteProgressFlowBar();
            this.packetsSentPerSecondFlowControl = new Idera.SQLdm.DesktopClient.Controls.InfiniteProgressFlowBar();
            this.packetsReceivedPerSecondFlowControl = new Idera.SQLdm.DesktopClient.Controls.InfiniteProgressFlowBar();
            this.transactionsPerSecondLabel = new System.Windows.Forms.Label();
            this.packetsSentPerSecondLabel = new System.Windows.Forms.Label();
            this.packetsReceivedPerSecondLabel = new System.Windows.Forms.Label();
            this.processorDataFlowPanel = new System.Windows.Forms.Panel();
            this.pageWritesPerSecondFlowControl = new Idera.SQLdm.DesktopClient.Controls.InfiniteProgressFlowBar();
            this.pageReadsPerSecondFlowControl = new Idera.SQLdm.DesktopClient.Controls.InfiniteProgressFlowBar();
            this.sqlCompilationsPerSecondFlowControl = new Idera.SQLdm.DesktopClient.Controls.InfiniteProgressFlowBar();
            this.sqlCompilationsPerSecondLabel = new System.Windows.Forms.Label();
            this.pageReadsPerSecondLabel = new System.Windows.Forms.Label();
            this.pageWritesPerSecondLabel = new System.Windows.Forms.Label();
            this.memoryDataFlowPanel = new System.Windows.Forms.Panel();
            this.logFlushesPerSecondFlowControl = new Idera.SQLdm.DesktopClient.Controls.InfiniteProgressFlowBar();
            this.pageReadsPerSecondFlowControl2 = new Idera.SQLdm.DesktopClient.Controls.InfiniteProgressFlowBar();
            this.pageWritesPerSecondFlowControl2 = new Idera.SQLdm.DesktopClient.Controls.InfiniteProgressFlowBar();
            this.logFlushesPerSecondLabel = new System.Windows.Forms.Label();
            this.pageReadsPerSecondLabel2 = new System.Windows.Forms.Label();
            this.pageWritesPerSecondLabel2 = new System.Windows.Forms.Label();
            this.headerPanel = new Idera.SQLdm.DesktopClient.Controls.PulseHeaderPanel();
            this.versionInformationLabel = new System.Windows.Forms.Label();
            this.areaLayoutPanel.SuspendLayout();
            this.sessionsGroupPanel.SuspendLayout();
            this.sessionsGroupLayoutPanel.SuspendLayout();
            this.activeUserProcessesPanel.SuspendLayout();
            this.userProcessesPanel.SuspendLayout();
            this.clientComputersPanel.SuspendLayout();
            this.responseTimePanel.SuspendLayout();
            this.diskGroupPanel.SuspendLayout();
            this.diskGroupLayoutPanel.SuspendLayout();
            this.diskQueueLengthPanel.SuspendLayout();
            this.logFilesPanel.SuspendLayout();
            this.dataFilesPanel.SuspendLayout();
            this.databasesPanel.SuspendLayout();
            this.memoryGroupPanel.SuspendLayout();
            this.memoryGroupLayoutPanel.SuspendLayout();
            this.pagingPanel.SuspendLayout();
            this.procedureCachePanel.SuspendLayout();
            this.bufferCachePanel.SuspendLayout();
            this.sqlMemoryUsagePanel.SuspendLayout();
            this.processorGroupPanel.SuspendLayout();
            this.processorGroupLayoutPanel.SuspendLayout();
            this.processorUsagePanel.SuspendLayout();
            this.processorQueueLengthPanel.SuspendLayout();
            this.blockedProcessesPanel.SuspendLayout();
            this.sqlProcessesPanel.SuspendLayout();
            this.sessionsDataFlowPanel.SuspendLayout();
            this.processorDataFlowPanel.SuspendLayout();
            this.memoryDataFlowPanel.SuspendLayout();
            this.headerPanel.SuspendLayout();
            this.SuspendLayout();
            // 
            // refreshBackgroundWorker
            // 
            this.refreshBackgroundWorker.WorkerSupportsCancellation = true;
            this.refreshBackgroundWorker.DoWork += new System.ComponentModel.DoWorkEventHandler(this.refreshBackgroundWorker_DoWork);
            this.refreshBackgroundWorker.RunWorkerCompleted += new System.ComponentModel.RunWorkerCompletedEventHandler(this.refreshBackgroundWorker_RunWorkerCompleted);
            // 
            // areaLayoutPanel
            // 
            this.areaLayoutPanel.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.areaLayoutPanel.ColumnCount = 7;
            this.areaLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 150F));
            this.areaLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 33.33333F));
            this.areaLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 150F));
            this.areaLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 33.33333F));
            this.areaLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 150F));
            this.areaLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 33.33333F));
            this.areaLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 155F));
            this.areaLayoutPanel.Controls.Add(this.sessionsGroupPanel, 0, 0);
            this.areaLayoutPanel.Controls.Add(this.diskGroupPanel, 6, 0);
            this.areaLayoutPanel.Controls.Add(this.memoryGroupPanel, 4, 0);
            this.areaLayoutPanel.Controls.Add(this.processorGroupPanel, 2, 0);
            this.areaLayoutPanel.Controls.Add(this.sessionsDataFlowPanel, 1, 0);
            this.areaLayoutPanel.Controls.Add(this.processorDataFlowPanel, 3, 0);
            this.areaLayoutPanel.Controls.Add(this.memoryDataFlowPanel, 5, 0);
            this.areaLayoutPanel.Location = new System.Drawing.Point(6, 43);
            this.areaLayoutPanel.Name = "areaLayoutPanel";
            this.areaLayoutPanel.RowCount = 1;
            this.areaLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.areaLayoutPanel.Size = new System.Drawing.Size(764, 501);
            this.areaLayoutPanel.TabIndex = 6;
            // 
            // sessionsGroupPanel
            // 
            this.sessionsGroupPanel.BackColor = System.Drawing.Color.Transparent;
            this.sessionsGroupPanel.Controls.Add(this.sessionsGroupLayoutPanel);
            this.sessionsGroupPanel.Controls.Add(this.sessionsGroupLabel);
            this.sessionsGroupPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.sessionsGroupPanel.GroupBoxBackColor = System.Drawing.Color.White;
            this.sessionsGroupPanel.Location = new System.Drawing.Point(0, 0);
            this.sessionsGroupPanel.Margin = new System.Windows.Forms.Padding(0);
            this.sessionsGroupPanel.Name = "sessionsGroupPanel";
            this.sessionsGroupPanel.Size = new System.Drawing.Size(150, 501);
            this.sessionsGroupPanel.Style = Idera.SQLdm.DesktopClient.Controls.GroupPanelStyle.Space;
            this.sessionsGroupPanel.TabIndex = 1;
            // 
            // sessionsGroupLayoutPanel
            // 
            this.sessionsGroupLayoutPanel.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.sessionsGroupLayoutPanel.BackColor = System.Drawing.Color.Transparent;
            this.sessionsGroupLayoutPanel.ColumnCount = 1;
            this.sessionsGroupLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.sessionsGroupLayoutPanel.Controls.Add(this.activeUserProcessesPanel, 0, 3);
            this.sessionsGroupLayoutPanel.Controls.Add(this.userProcessesPanel, 0, 2);
            this.sessionsGroupLayoutPanel.Controls.Add(this.clientComputersPanel, 0, 1);
            this.sessionsGroupLayoutPanel.Controls.Add(this.responseTimePanel, 0, 0);
            this.sessionsGroupLayoutPanel.Location = new System.Drawing.Point(3, 25);
            this.sessionsGroupLayoutPanel.Name = "sessionsGroupLayoutPanel";
            this.sessionsGroupLayoutPanel.RowCount = 4;
            this.sessionsGroupLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 25F));
            this.sessionsGroupLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 25F));
            this.sessionsGroupLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 25F));
            this.sessionsGroupLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 25F));
            this.sessionsGroupLayoutPanel.Size = new System.Drawing.Size(144, 473);
            this.sessionsGroupLayoutPanel.TabIndex = 13;
            // 
            // activeUserProcessesPanel
            // 
            this.activeUserProcessesPanel.Controls.Add(this.activeUserProcessesLabel);
            this.activeUserProcessesPanel.Controls.Add(this.activeUserProcessesButton);
            this.activeUserProcessesPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.activeUserProcessesPanel.Location = new System.Drawing.Point(3, 357);
            this.activeUserProcessesPanel.Name = "activeUserProcessesPanel";
            this.activeUserProcessesPanel.Size = new System.Drawing.Size(138, 113);
            this.activeUserProcessesPanel.TabIndex = 16;
            // 
            // activeUserProcessesLabel
            // 
            this.activeUserProcessesLabel.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.activeUserProcessesLabel.AutoEllipsis = true;
            this.activeUserProcessesLabel.AutoSize = true;
            this.activeUserProcessesLabel.BackColor = System.Drawing.Color.Transparent;
            this.activeUserProcessesLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.activeUserProcessesLabel.Location = new System.Drawing.Point(2, 27);
            this.activeUserProcessesLabel.Name = "activeUserProcessesLabel";
            this.activeUserProcessesLabel.Size = new System.Drawing.Size(135, 13);
            this.activeUserProcessesLabel.TabIndex = 7;
            this.activeUserProcessesLabel.Text = "Active User Processes";
            this.activeUserProcessesLabel.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            // 
            // activeUserProcessesButton
            // 
            this.activeUserProcessesButton.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.activeUserProcessesButton.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.activeUserProcessesButton.Location = new System.Drawing.Point(23, 50);
            this.activeUserProcessesButton.MaximumSize = new System.Drawing.Size(100, 35);
            this.activeUserProcessesButton.Name = "activeUserProcessesButton";
            this.activeUserProcessesButton.ShowFocusRect = false;
            this.activeUserProcessesButton.ShowOutline = false;
            this.activeUserProcessesButton.Size = new System.Drawing.Size(92, 35);
            this.activeUserProcessesButton.TabIndex = 8;
            this.activeUserProcessesButton.Text = "?";
            // 
            // userProcessesPanel
            // 
            this.userProcessesPanel.Controls.Add(this.userProcessesButton);
            this.userProcessesPanel.Controls.Add(this.userProcessesLabel);
            this.userProcessesPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.userProcessesPanel.Location = new System.Drawing.Point(3, 239);
            this.userProcessesPanel.Name = "userProcessesPanel";
            this.userProcessesPanel.Size = new System.Drawing.Size(138, 112);
            this.userProcessesPanel.TabIndex = 15;
            // 
            // userProcessesButton
            // 
            this.userProcessesButton.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.userProcessesButton.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.userProcessesButton.Location = new System.Drawing.Point(23, 50);
            this.userProcessesButton.MaximumSize = new System.Drawing.Size(100, 35);
            this.userProcessesButton.Name = "userProcessesButton";
            this.userProcessesButton.ShowFocusRect = false;
            this.userProcessesButton.ShowOutline = false;
            this.userProcessesButton.Size = new System.Drawing.Size(92, 35);
            this.userProcessesButton.TabIndex = 6;
            this.userProcessesButton.Text = "?";
            // 
            // userProcessesLabel
            // 
            this.userProcessesLabel.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.userProcessesLabel.AutoEllipsis = true;
            this.userProcessesLabel.AutoSize = true;
            this.userProcessesLabel.BackColor = System.Drawing.Color.Transparent;
            this.userProcessesLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.userProcessesLabel.Location = new System.Drawing.Point(21, 27);
            this.userProcessesLabel.Name = "userProcessesLabel";
            this.userProcessesLabel.Size = new System.Drawing.Size(95, 13);
            this.userProcessesLabel.TabIndex = 5;
            this.userProcessesLabel.Text = "User Processes";
            this.userProcessesLabel.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            // 
            // clientComputersPanel
            // 
            this.clientComputersPanel.Controls.Add(this.clientComputersButton);
            this.clientComputersPanel.Controls.Add(this.clientComputersLabel);
            this.clientComputersPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.clientComputersPanel.Location = new System.Drawing.Point(3, 121);
            this.clientComputersPanel.Name = "clientComputersPanel";
            this.clientComputersPanel.Size = new System.Drawing.Size(138, 112);
            this.clientComputersPanel.TabIndex = 14;
            // 
            // clientComputersButton
            // 
            this.clientComputersButton.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.clientComputersButton.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.clientComputersButton.Location = new System.Drawing.Point(23, 50);
            this.clientComputersButton.MaximumSize = new System.Drawing.Size(100, 35);
            this.clientComputersButton.Name = "clientComputersButton";
            this.clientComputersButton.ShowFocusRect = false;
            this.clientComputersButton.ShowOutline = false;
            this.clientComputersButton.Size = new System.Drawing.Size(92, 35);
            this.clientComputersButton.TabIndex = 4;
            this.clientComputersButton.Text = "?";
            // 
            // clientComputersLabel
            // 
            this.clientComputersLabel.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.clientComputersLabel.AutoEllipsis = true;
            this.clientComputersLabel.AutoSize = true;
            this.clientComputersLabel.BackColor = System.Drawing.Color.Transparent;
            this.clientComputersLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.clientComputersLabel.Location = new System.Drawing.Point(15, 27);
            this.clientComputersLabel.Name = "clientComputersLabel";
            this.clientComputersLabel.Size = new System.Drawing.Size(102, 13);
            this.clientComputersLabel.TabIndex = 3;
            this.clientComputersLabel.Text = "Client Computers";
            this.clientComputersLabel.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            // 
            // responseTimePanel
            // 
            this.responseTimePanel.Controls.Add(this.responseTimeButton);
            this.responseTimePanel.Controls.Add(this.responseTimeLabel);
            this.responseTimePanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.responseTimePanel.Location = new System.Drawing.Point(3, 3);
            this.responseTimePanel.Name = "responseTimePanel";
            this.responseTimePanel.Size = new System.Drawing.Size(138, 112);
            this.responseTimePanel.TabIndex = 13;
            // 
            // responseTimeButton
            // 
            this.responseTimeButton.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.responseTimeButton.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.responseTimeButton.Location = new System.Drawing.Point(23, 51);
            this.responseTimeButton.MaximumSize = new System.Drawing.Size(100, 35);
            this.responseTimeButton.Name = "responseTimeButton";
            this.responseTimeButton.ShowFocusRect = false;
            this.responseTimeButton.ShowOutline = false;
            this.responseTimeButton.Size = new System.Drawing.Size(92, 35);
            this.responseTimeButton.TabIndex = 2;
            this.responseTimeButton.Text = "?";
            // 
            // responseTimeLabel
            // 
            this.responseTimeLabel.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.responseTimeLabel.AutoEllipsis = true;
            this.responseTimeLabel.AutoSize = true;
            this.responseTimeLabel.BackColor = System.Drawing.Color.Transparent;
            this.responseTimeLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.responseTimeLabel.Location = new System.Drawing.Point(7, 27);
            this.responseTimeLabel.Name = "responseTimeLabel";
            this.responseTimeLabel.Size = new System.Drawing.Size(121, 13);
            this.responseTimeLabel.TabIndex = 1;
            this.responseTimeLabel.Text = "Response Time (ms)";
            this.responseTimeLabel.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            // 
            // sessionsGroupLabel
            // 
            this.sessionsGroupLabel.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.sessionsGroupLabel.AutoSize = true;
            this.sessionsGroupLabel.BackColor = System.Drawing.Color.Transparent;
            this.sessionsGroupLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.sessionsGroupLabel.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(32)))), ((int)(((byte)(77)))), ((int)(((byte)(137)))));
            this.sessionsGroupLabel.Location = new System.Drawing.Point(39, 5);
            this.sessionsGroupLabel.Name = "sessionsGroupLabel";
            this.sessionsGroupLabel.Size = new System.Drawing.Size(72, 16);
            this.sessionsGroupLabel.TabIndex = 0;
            this.sessionsGroupLabel.Text = "Sessions";
            // 
            // diskGroupPanel
            // 
            this.diskGroupPanel.BackColor = System.Drawing.Color.Transparent;
            this.diskGroupPanel.Controls.Add(this.diskGroupLayoutPanel);
            this.diskGroupPanel.Controls.Add(this.databaseGroupLabel);
            this.diskGroupPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.diskGroupPanel.GroupBoxBackColor = System.Drawing.Color.White;
            this.diskGroupPanel.Location = new System.Drawing.Point(609, 0);
            this.diskGroupPanel.Margin = new System.Windows.Forms.Padding(0);
            this.diskGroupPanel.Name = "diskGroupPanel";
            this.diskGroupPanel.Size = new System.Drawing.Size(155, 501);
            this.diskGroupPanel.Style = Idera.SQLdm.DesktopClient.Controls.GroupPanelStyle.Space;
            this.diskGroupPanel.TabIndex = 4;
            // 
            // diskGroupLayoutPanel
            // 
            this.diskGroupLayoutPanel.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.diskGroupLayoutPanel.BackColor = System.Drawing.Color.Transparent;
            this.diskGroupLayoutPanel.ColumnCount = 1;
            this.diskGroupLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.diskGroupLayoutPanel.Controls.Add(this.diskQueueLengthPanel, 0, 3);
            this.diskGroupLayoutPanel.Controls.Add(this.logFilesPanel, 0, 2);
            this.diskGroupLayoutPanel.Controls.Add(this.dataFilesPanel, 0, 1);
            this.diskGroupLayoutPanel.Controls.Add(this.databasesPanel, 0, 0);
            this.diskGroupLayoutPanel.Location = new System.Drawing.Point(3, 25);
            this.diskGroupLayoutPanel.Name = "diskGroupLayoutPanel";
            this.diskGroupLayoutPanel.RowCount = 4;
            this.diskGroupLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 25F));
            this.diskGroupLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 25F));
            this.diskGroupLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 25F));
            this.diskGroupLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 25F));
            this.diskGroupLayoutPanel.Size = new System.Drawing.Size(149, 473);
            this.diskGroupLayoutPanel.TabIndex = 15;
            // 
            // diskQueueLengthPanel
            // 
            this.diskQueueLengthPanel.Controls.Add(this.databaseGroupDividerLabel1);
            this.diskQueueLengthPanel.Controls.Add(this.diskQueueLengthMaximumLabel);
            this.diskQueueLengthPanel.Controls.Add(this.diskQueueLengthMinimumLabel);
            this.diskQueueLengthPanel.Controls.Add(this.diskQueueLengthStatusBar);
            this.diskQueueLengthPanel.Controls.Add(this.diskQueueLengthLabel);
            this.diskQueueLengthPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.diskQueueLengthPanel.Location = new System.Drawing.Point(3, 357);
            this.diskQueueLengthPanel.Name = "diskQueueLengthPanel";
            this.diskQueueLengthPanel.Size = new System.Drawing.Size(143, 113);
            this.diskQueueLengthPanel.TabIndex = 16;
            // 
            // databaseGroupDividerLabel1
            // 
            this.databaseGroupDividerLabel1.BackColor = System.Drawing.Color.Silver;
            this.databaseGroupDividerLabel1.Dock = System.Windows.Forms.DockStyle.Top;
            this.databaseGroupDividerLabel1.Location = new System.Drawing.Point(0, 0);
            this.databaseGroupDividerLabel1.Name = "databaseGroupDividerLabel1";
            this.databaseGroupDividerLabel1.Size = new System.Drawing.Size(143, 1);
            this.databaseGroupDividerLabel1.TabIndex = 21;
            // 
            // diskQueueLengthMaximumLabel
            // 
            this.diskQueueLengthMaximumLabel.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.diskQueueLengthMaximumLabel.AutoSize = true;
            this.diskQueueLengthMaximumLabel.Location = new System.Drawing.Point(117, 74);
            this.diskQueueLengthMaximumLabel.Name = "diskQueueLengthMaximumLabel";
            this.diskQueueLengthMaximumLabel.Size = new System.Drawing.Size(13, 13);
            this.diskQueueLengthMaximumLabel.TabIndex = 12;
            this.diskQueueLengthMaximumLabel.Text = "5";
            // 
            // diskQueueLengthMinimumLabel
            // 
            this.diskQueueLengthMinimumLabel.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.diskQueueLengthMinimumLabel.AutoSize = true;
            this.diskQueueLengthMinimumLabel.Location = new System.Drawing.Point(12, 74);
            this.diskQueueLengthMinimumLabel.Name = "diskQueueLengthMinimumLabel";
            this.diskQueueLengthMinimumLabel.Size = new System.Drawing.Size(13, 13);
            this.diskQueueLengthMinimumLabel.TabIndex = 11;
            this.diskQueueLengthMinimumLabel.Text = "0";
            // 
            // diskQueueLengthStatusBar
            // 
            this.diskQueueLengthStatusBar.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.diskQueueLengthStatusBar.Location = new System.Drawing.Point(13, 48);
            this.diskQueueLengthStatusBar.Maximum = 5;
            this.diskQueueLengthStatusBar.Name = "diskQueueLengthStatusBar";
            this.diskQueueLengthStatusBar.Size = new System.Drawing.Size(115, 23);
            this.diskQueueLengthStatusBar.TabIndex = 10;
            this.diskQueueLengthStatusBar.Text = "[Value]";
            // 
            // diskQueueLengthLabel
            // 
            this.diskQueueLengthLabel.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.diskQueueLengthLabel.AutoEllipsis = true;
            this.diskQueueLengthLabel.AutoSize = true;
            this.diskQueueLengthLabel.BackColor = System.Drawing.Color.Transparent;
            this.diskQueueLengthLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.diskQueueLengthLabel.Location = new System.Drawing.Point(33, 25);
            this.diskQueueLengthLabel.Name = "diskQueueLengthLabel";
            this.diskQueueLengthLabel.Size = new System.Drawing.Size(73, 13);
            this.diskQueueLengthLabel.TabIndex = 9;
            this.diskQueueLengthLabel.Text = "Disk Queue";
            this.diskQueueLengthLabel.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            // 
            // logFilesPanel
            // 
            this.logFilesPanel.Controls.Add(this.databaseGroupDividerLabel2);
            this.logFilesPanel.Controls.Add(this.logFilesCountValueLabel);
            this.logFilesPanel.Controls.Add(this.logFilesSizeValueLabel);
            this.logFilesPanel.Controls.Add(this.logFilesSizeLabel);
            this.logFilesPanel.Controls.Add(this.logFilesCountLabel);
            this.logFilesPanel.Controls.Add(this.logFilesLabel);
            this.logFilesPanel.Controls.Add(this.logFilesPercentFullStatusBar);
            this.logFilesPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.logFilesPanel.Location = new System.Drawing.Point(3, 239);
            this.logFilesPanel.Name = "logFilesPanel";
            this.logFilesPanel.Size = new System.Drawing.Size(143, 112);
            this.logFilesPanel.TabIndex = 15;
            // 
            // databaseGroupDividerLabel2
            // 
            this.databaseGroupDividerLabel2.BackColor = System.Drawing.Color.Silver;
            this.databaseGroupDividerLabel2.Dock = System.Windows.Forms.DockStyle.Top;
            this.databaseGroupDividerLabel2.Location = new System.Drawing.Point(0, 0);
            this.databaseGroupDividerLabel2.Name = "databaseGroupDividerLabel2";
            this.databaseGroupDividerLabel2.Size = new System.Drawing.Size(143, 1);
            this.databaseGroupDividerLabel2.TabIndex = 26;
            // 
            // logFilesCountValueLabel
            // 
            this.logFilesCountValueLabel.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.logFilesCountValueLabel.AutoEllipsis = true;
            this.logFilesCountValueLabel.BackColor = System.Drawing.Color.Transparent;
            this.logFilesCountValueLabel.Location = new System.Drawing.Point(66, 52);
            this.logFilesCountValueLabel.Name = "logFilesCountValueLabel";
            this.logFilesCountValueLabel.Size = new System.Drawing.Size(57, 13);
            this.logFilesCountValueLabel.TabIndex = 25;
            this.logFilesCountValueLabel.Text = "?";
            this.logFilesCountValueLabel.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // logFilesSizeValueLabel
            // 
            this.logFilesSizeValueLabel.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.logFilesSizeValueLabel.AutoEllipsis = true;
            this.logFilesSizeValueLabel.BackColor = System.Drawing.Color.Transparent;
            this.logFilesSizeValueLabel.Location = new System.Drawing.Point(66, 84);
            this.logFilesSizeValueLabel.Name = "logFilesSizeValueLabel";
            this.logFilesSizeValueLabel.Size = new System.Drawing.Size(57, 13);
            this.logFilesSizeValueLabel.TabIndex = 24;
            this.logFilesSizeValueLabel.Text = "?";
            this.logFilesSizeValueLabel.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // logFilesSizeLabel
            // 
            this.logFilesSizeLabel.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.logFilesSizeLabel.AutoEllipsis = true;
            this.logFilesSizeLabel.AutoSize = true;
            this.logFilesSizeLabel.BackColor = System.Drawing.Color.Transparent;
            this.logFilesSizeLabel.Location = new System.Drawing.Point(66, 71);
            this.logFilesSizeLabel.Name = "logFilesSizeLabel";
            this.logFilesSizeLabel.Size = new System.Drawing.Size(30, 13);
            this.logFilesSizeLabel.TabIndex = 23;
            this.logFilesSizeLabel.Text = "Size:";
            this.logFilesSizeLabel.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // logFilesCountLabel
            // 
            this.logFilesCountLabel.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.logFilesCountLabel.AutoEllipsis = true;
            this.logFilesCountLabel.AutoSize = true;
            this.logFilesCountLabel.BackColor = System.Drawing.Color.Transparent;
            this.logFilesCountLabel.Location = new System.Drawing.Point(66, 39);
            this.logFilesCountLabel.Name = "logFilesCountLabel";
            this.logFilesCountLabel.Size = new System.Drawing.Size(31, 13);
            this.logFilesCountLabel.TabIndex = 22;
            this.logFilesCountLabel.Text = "Files:";
            this.logFilesCountLabel.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // logFilesLabel
            // 
            this.logFilesLabel.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.logFilesLabel.AutoEllipsis = true;
            this.logFilesLabel.AutoSize = true;
            this.logFilesLabel.BackColor = System.Drawing.Color.Transparent;
            this.logFilesLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.logFilesLabel.Location = new System.Drawing.Point(41, 14);
            this.logFilesLabel.Name = "logFilesLabel";
            this.logFilesLabel.Size = new System.Drawing.Size(58, 13);
            this.logFilesLabel.TabIndex = 21;
            this.logFilesLabel.Text = "Log Files";
            this.logFilesLabel.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            // 
            // logFilesPercentFullStatusBar
            // 
            this.logFilesPercentFullStatusBar.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.logFilesPercentFullStatusBar.Location = new System.Drawing.Point(20, 35);
            this.logFilesPercentFullStatusBar.Name = "logFilesPercentFullStatusBar";
            this.logFilesPercentFullStatusBar.Orientation = System.Windows.Forms.Orientation.Vertical;
            this.logFilesPercentFullStatusBar.Size = new System.Drawing.Size(35, 64);
            this.logFilesPercentFullStatusBar.TabIndex = 20;
            this.logFilesPercentFullStatusBar.Text = "[Formatted]";
            // 
            // dataFilesPanel
            // 
            this.dataFilesPanel.Controls.Add(this.databaseGroupDividerLabel3);
            this.dataFilesPanel.Controls.Add(this.dataFilesCountValueLabel);
            this.dataFilesPanel.Controls.Add(this.dataFilesSizeValueLabel);
            this.dataFilesPanel.Controls.Add(this.dataFilesSizeLabel);
            this.dataFilesPanel.Controls.Add(this.dataFilesCountLabel);
            this.dataFilesPanel.Controls.Add(this.dataFilesLabel);
            this.dataFilesPanel.Controls.Add(this.dataFilesPercentFullStatusBar);
            this.dataFilesPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dataFilesPanel.Location = new System.Drawing.Point(3, 121);
            this.dataFilesPanel.Name = "dataFilesPanel";
            this.dataFilesPanel.Size = new System.Drawing.Size(143, 112);
            this.dataFilesPanel.TabIndex = 14;
            // 
            // databaseGroupDividerLabel3
            // 
            this.databaseGroupDividerLabel3.BackColor = System.Drawing.Color.Silver;
            this.databaseGroupDividerLabel3.Dock = System.Windows.Forms.DockStyle.Top;
            this.databaseGroupDividerLabel3.Location = new System.Drawing.Point(0, 0);
            this.databaseGroupDividerLabel3.Name = "databaseGroupDividerLabel3";
            this.databaseGroupDividerLabel3.Size = new System.Drawing.Size(143, 1);
            this.databaseGroupDividerLabel3.TabIndex = 20;
            // 
            // dataFilesCountValueLabel
            // 
            this.dataFilesCountValueLabel.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.dataFilesCountValueLabel.AutoEllipsis = true;
            this.dataFilesCountValueLabel.BackColor = System.Drawing.Color.Transparent;
            this.dataFilesCountValueLabel.Location = new System.Drawing.Point(66, 52);
            this.dataFilesCountValueLabel.Name = "dataFilesCountValueLabel";
            this.dataFilesCountValueLabel.Size = new System.Drawing.Size(57, 13);
            this.dataFilesCountValueLabel.TabIndex = 19;
            this.dataFilesCountValueLabel.Text = "?";
            this.dataFilesCountValueLabel.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // dataFilesSizeValueLabel
            // 
            this.dataFilesSizeValueLabel.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.dataFilesSizeValueLabel.AutoEllipsis = true;
            this.dataFilesSizeValueLabel.BackColor = System.Drawing.Color.Transparent;
            this.dataFilesSizeValueLabel.Location = new System.Drawing.Point(66, 84);
            this.dataFilesSizeValueLabel.Name = "dataFilesSizeValueLabel";
            this.dataFilesSizeValueLabel.Size = new System.Drawing.Size(57, 13);
            this.dataFilesSizeValueLabel.TabIndex = 18;
            this.dataFilesSizeValueLabel.Text = "?";
            this.dataFilesSizeValueLabel.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // dataFilesSizeLabel
            // 
            this.dataFilesSizeLabel.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.dataFilesSizeLabel.AutoEllipsis = true;
            this.dataFilesSizeLabel.AutoSize = true;
            this.dataFilesSizeLabel.BackColor = System.Drawing.Color.Transparent;
            this.dataFilesSizeLabel.Location = new System.Drawing.Point(66, 71);
            this.dataFilesSizeLabel.Name = "dataFilesSizeLabel";
            this.dataFilesSizeLabel.Size = new System.Drawing.Size(30, 13);
            this.dataFilesSizeLabel.TabIndex = 17;
            this.dataFilesSizeLabel.Text = "Size:";
            this.dataFilesSizeLabel.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // dataFilesCountLabel
            // 
            this.dataFilesCountLabel.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.dataFilesCountLabel.AutoEllipsis = true;
            this.dataFilesCountLabel.AutoSize = true;
            this.dataFilesCountLabel.BackColor = System.Drawing.Color.Transparent;
            this.dataFilesCountLabel.Location = new System.Drawing.Point(66, 39);
            this.dataFilesCountLabel.Name = "dataFilesCountLabel";
            this.dataFilesCountLabel.Size = new System.Drawing.Size(31, 13);
            this.dataFilesCountLabel.TabIndex = 16;
            this.dataFilesCountLabel.Text = "Files:";
            this.dataFilesCountLabel.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // dataFilesLabel
            // 
            this.dataFilesLabel.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.dataFilesLabel.AutoEllipsis = true;
            this.dataFilesLabel.AutoSize = true;
            this.dataFilesLabel.BackColor = System.Drawing.Color.Transparent;
            this.dataFilesLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.dataFilesLabel.Location = new System.Drawing.Point(38, 14);
            this.dataFilesLabel.Name = "dataFilesLabel";
            this.dataFilesLabel.Size = new System.Drawing.Size(64, 13);
            this.dataFilesLabel.TabIndex = 15;
            this.dataFilesLabel.Text = "Data Files";
            this.dataFilesLabel.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            // 
            // dataFilesPercentFullStatusBar
            // 
            this.dataFilesPercentFullStatusBar.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.dataFilesPercentFullStatusBar.Location = new System.Drawing.Point(20, 35);
            this.dataFilesPercentFullStatusBar.Name = "dataFilesPercentFullStatusBar";
            this.dataFilesPercentFullStatusBar.Orientation = System.Windows.Forms.Orientation.Vertical;
            this.dataFilesPercentFullStatusBar.Size = new System.Drawing.Size(35, 64);
            this.dataFilesPercentFullStatusBar.TabIndex = 14;
            this.dataFilesPercentFullStatusBar.Text = "[Formatted]";
            // 
            // databasesPanel
            // 
            this.databasesPanel.Controls.Add(this.databasesButton);
            this.databasesPanel.Controls.Add(this.databasesLabel);
            this.databasesPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.databasesPanel.Location = new System.Drawing.Point(3, 3);
            this.databasesPanel.Name = "databasesPanel";
            this.databasesPanel.Size = new System.Drawing.Size(143, 112);
            this.databasesPanel.TabIndex = 13;
            // 
            // databasesButton
            // 
            this.databasesButton.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.databasesButton.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.databasesButton.Location = new System.Drawing.Point(25, 51);
            this.databasesButton.MaximumSize = new System.Drawing.Size(100, 35);
            this.databasesButton.Name = "databasesButton";
            this.databasesButton.ShowFocusRect = false;
            this.databasesButton.ShowOutline = false;
            this.databasesButton.Size = new System.Drawing.Size(92, 35);
            this.databasesButton.TabIndex = 2;
            this.databasesButton.Text = "?";
            // 
            // databasesLabel
            // 
            this.databasesLabel.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.databasesLabel.AutoEllipsis = true;
            this.databasesLabel.AutoSize = true;
            this.databasesLabel.BackColor = System.Drawing.Color.Transparent;
            this.databasesLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.databasesLabel.Location = new System.Drawing.Point(37, 27);
            this.databasesLabel.Name = "databasesLabel";
            this.databasesLabel.Size = new System.Drawing.Size(67, 13);
            this.databasesLabel.TabIndex = 1;
            this.databasesLabel.Text = "Databases";
            this.databasesLabel.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            // 
            // databaseGroupLabel
            // 
            this.databaseGroupLabel.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.databaseGroupLabel.AutoSize = true;
            this.databaseGroupLabel.BackColor = System.Drawing.Color.Transparent;
            this.databaseGroupLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.databaseGroupLabel.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(32)))), ((int)(((byte)(77)))), ((int)(((byte)(137)))));
            this.databaseGroupLabel.Location = new System.Drawing.Point(55, 5);
            this.databaseGroupLabel.Name = "databaseGroupLabel";
            this.databaseGroupLabel.Size = new System.Drawing.Size(39, 16);
            this.databaseGroupLabel.TabIndex = 0;
            this.databaseGroupLabel.Text = "Disk";
            // 
            // memoryGroupPanel
            // 
            this.memoryGroupPanel.BackColor = System.Drawing.Color.Transparent;
            this.memoryGroupPanel.Controls.Add(this.memoryGroupLayoutPanel);
            this.memoryGroupPanel.Controls.Add(this.label4);
            this.memoryGroupPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.memoryGroupPanel.GroupBoxBackColor = System.Drawing.Color.White;
            this.memoryGroupPanel.Location = new System.Drawing.Point(406, 0);
            this.memoryGroupPanel.Margin = new System.Windows.Forms.Padding(0);
            this.memoryGroupPanel.Name = "memoryGroupPanel";
            this.memoryGroupPanel.Size = new System.Drawing.Size(150, 501);
            this.memoryGroupPanel.Style = Idera.SQLdm.DesktopClient.Controls.GroupPanelStyle.Space;
            this.memoryGroupPanel.TabIndex = 3;
            // 
            // memoryGroupLayoutPanel
            // 
            this.memoryGroupLayoutPanel.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.memoryGroupLayoutPanel.BackColor = System.Drawing.Color.Transparent;
            this.memoryGroupLayoutPanel.ColumnCount = 1;
            this.memoryGroupLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.memoryGroupLayoutPanel.Controls.Add(this.pagingPanel, 0, 3);
            this.memoryGroupLayoutPanel.Controls.Add(this.procedureCachePanel, 0, 2);
            this.memoryGroupLayoutPanel.Controls.Add(this.bufferCachePanel, 0, 1);
            this.memoryGroupLayoutPanel.Controls.Add(this.sqlMemoryUsagePanel, 0, 0);
            this.memoryGroupLayoutPanel.Location = new System.Drawing.Point(3, 25);
            this.memoryGroupLayoutPanel.Name = "memoryGroupLayoutPanel";
            this.memoryGroupLayoutPanel.RowCount = 4;
            this.memoryGroupLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 25F));
            this.memoryGroupLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 25F));
            this.memoryGroupLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 25F));
            this.memoryGroupLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 25F));
            this.memoryGroupLayoutPanel.Size = new System.Drawing.Size(144, 473);
            this.memoryGroupLayoutPanel.TabIndex = 15;
            // 
            // pagingPanel
            // 
            this.pagingPanel.Controls.Add(this.memoryGroupDividerLabel1);
            this.pagingPanel.Controls.Add(this.pagingValueLabel);
            this.pagingPanel.Controls.Add(this.pagingLabel);
            this.pagingPanel.Controls.Add(this.pagingFlowControl);
            this.pagingPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pagingPanel.Location = new System.Drawing.Point(3, 357);
            this.pagingPanel.Name = "pagingPanel";
            this.pagingPanel.Size = new System.Drawing.Size(138, 113);
            this.pagingPanel.TabIndex = 16;
            // 
            // memoryGroupDividerLabel1
            // 
            this.memoryGroupDividerLabel1.BackColor = System.Drawing.Color.Silver;
            this.memoryGroupDividerLabel1.Dock = System.Windows.Forms.DockStyle.Top;
            this.memoryGroupDividerLabel1.Location = new System.Drawing.Point(0, 0);
            this.memoryGroupDividerLabel1.Name = "memoryGroupDividerLabel1";
            this.memoryGroupDividerLabel1.Size = new System.Drawing.Size(138, 1);
            this.memoryGroupDividerLabel1.TabIndex = 21;
            // 
            // pagingValueLabel
            // 
            this.pagingValueLabel.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.pagingValueLabel.AutoEllipsis = true;
            this.pagingValueLabel.BackColor = System.Drawing.Color.Transparent;
            this.pagingValueLabel.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.pagingValueLabel.Location = new System.Drawing.Point(52, 61);
            this.pagingValueLabel.Name = "pagingValueLabel";
            this.pagingValueLabel.Size = new System.Drawing.Size(35, 15);
            this.pagingValueLabel.TabIndex = 8;
            this.pagingValueLabel.Text = "?";
            this.pagingValueLabel.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // pagingLabel
            // 
            this.pagingLabel.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.pagingLabel.AutoEllipsis = true;
            this.pagingLabel.AutoSize = true;
            this.pagingLabel.BackColor = System.Drawing.Color.Transparent;
            this.pagingLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.pagingLabel.Location = new System.Drawing.Point(44, 6);
            this.pagingLabel.Name = "pagingLabel";
            this.pagingLabel.Size = new System.Drawing.Size(46, 13);
            this.pagingLabel.TabIndex = 7;
            this.pagingLabel.Text = "Paging";
            this.pagingLabel.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            // 
            // pagingFlowControl
            // 
            this.pagingFlowControl.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.pagingFlowControl.ArrowDegrees = 10;
            this.pagingFlowControl.ColorEnd = System.Drawing.Color.FromArgb(((int)(((byte)(183)))), ((int)(((byte)(201)))), ((int)(((byte)(238)))));
            this.pagingFlowControl.ColorStart = System.Drawing.Color.FromArgb(((int)(((byte)(119)))), ((int)(((byte)(158)))), ((int)(((byte)(218)))));
            this.pagingFlowControl.Location = new System.Drawing.Point(27, 25);
            this.pagingFlowControl.MaximumValue = 100;
            this.pagingFlowControl.Name = "pagingFlowControl";
            this.pagingFlowControl.NumberSegments = 4;
            this.pagingFlowControl.Rotation = Idera.SQLdm.DesktopClient.Controls.InfiniteProgressCircle.Direction.Clockwise;
            this.pagingFlowControl.Size = new System.Drawing.Size(85, 85);
            this.pagingFlowControl.TabIndex = 22;
            this.pagingFlowControl.Text = "infiniteProgressCircle2";
            this.pagingFlowControl.Thickness = 15;
            this.pagingFlowControl.Value = 0;
            // 
            // procedureCachePanel
            // 
            this.procedureCachePanel.Controls.Add(this.memoryGroupDividerLabel2);
            this.procedureCachePanel.Controls.Add(this.procedureCacheSizeLabel);
            this.procedureCachePanel.Controls.Add(this.procedureCacheHitRateLabel);
            this.procedureCachePanel.Controls.Add(this.procedureCacheHitRateStatusBar);
            this.procedureCachePanel.Controls.Add(this.procedureCacheLabel);
            this.procedureCachePanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.procedureCachePanel.Location = new System.Drawing.Point(3, 239);
            this.procedureCachePanel.Name = "procedureCachePanel";
            this.procedureCachePanel.Size = new System.Drawing.Size(138, 112);
            this.procedureCachePanel.TabIndex = 15;
            // 
            // memoryGroupDividerLabel2
            // 
            this.memoryGroupDividerLabel2.BackColor = System.Drawing.Color.Silver;
            this.memoryGroupDividerLabel2.Dock = System.Windows.Forms.DockStyle.Top;
            this.memoryGroupDividerLabel2.Location = new System.Drawing.Point(0, 0);
            this.memoryGroupDividerLabel2.Name = "memoryGroupDividerLabel2";
            this.memoryGroupDividerLabel2.Size = new System.Drawing.Size(138, 1);
            this.memoryGroupDividerLabel2.TabIndex = 21;
            // 
            // procedureCacheSizeLabel
            // 
            this.procedureCacheSizeLabel.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.procedureCacheSizeLabel.AutoEllipsis = true;
            this.procedureCacheSizeLabel.BackColor = System.Drawing.Color.Transparent;
            this.procedureCacheSizeLabel.Location = new System.Drawing.Point(12, 37);
            this.procedureCacheSizeLabel.Name = "procedureCacheSizeLabel";
            this.procedureCacheSizeLabel.Size = new System.Drawing.Size(115, 13);
            this.procedureCacheSizeLabel.TabIndex = 19;
            this.procedureCacheSizeLabel.Text = "Size: ?";
            this.procedureCacheSizeLabel.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // procedureCacheHitRateLabel
            // 
            this.procedureCacheHitRateLabel.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.procedureCacheHitRateLabel.AutoEllipsis = true;
            this.procedureCacheHitRateLabel.AutoSize = true;
            this.procedureCacheHitRateLabel.BackColor = System.Drawing.Color.Transparent;
            this.procedureCacheHitRateLabel.Location = new System.Drawing.Point(46, 57);
            this.procedureCacheHitRateLabel.Name = "procedureCacheHitRateLabel";
            this.procedureCacheHitRateLabel.Size = new System.Drawing.Size(46, 13);
            this.procedureCacheHitRateLabel.TabIndex = 18;
            this.procedureCacheHitRateLabel.Text = "Hit Rate";
            this.procedureCacheHitRateLabel.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // procedureCacheHitRateStatusBar
            // 
            this.procedureCacheHitRateStatusBar.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.procedureCacheHitRateStatusBar.Location = new System.Drawing.Point(12, 73);
            this.procedureCacheHitRateStatusBar.Name = "procedureCacheHitRateStatusBar";
            this.procedureCacheHitRateStatusBar.Size = new System.Drawing.Size(115, 23);
            this.procedureCacheHitRateStatusBar.TabIndex = 17;
            this.procedureCacheHitRateStatusBar.Text = "[Formatted]";
            // 
            // procedureCacheLabel
            // 
            this.procedureCacheLabel.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.procedureCacheLabel.AutoEllipsis = true;
            this.procedureCacheLabel.AutoSize = true;
            this.procedureCacheLabel.BackColor = System.Drawing.Color.Transparent;
            this.procedureCacheLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.procedureCacheLabel.Location = new System.Drawing.Point(16, 16);
            this.procedureCacheLabel.Name = "procedureCacheLabel";
            this.procedureCacheLabel.Size = new System.Drawing.Size(105, 13);
            this.procedureCacheLabel.TabIndex = 16;
            this.procedureCacheLabel.Text = "Procedure Cache";
            this.procedureCacheLabel.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            // 
            // bufferCachePanel
            // 
            this.bufferCachePanel.Controls.Add(this.memoryGroupDividerLabel3);
            this.bufferCachePanel.Controls.Add(this.bufferCacheSizeLabel);
            this.bufferCachePanel.Controls.Add(this.bufferCacheHitRateLabel);
            this.bufferCachePanel.Controls.Add(this.bufferCacheHitRateStatusBar);
            this.bufferCachePanel.Controls.Add(this.bufferCacheLabel);
            this.bufferCachePanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.bufferCachePanel.Location = new System.Drawing.Point(3, 121);
            this.bufferCachePanel.Name = "bufferCachePanel";
            this.bufferCachePanel.Size = new System.Drawing.Size(138, 112);
            this.bufferCachePanel.TabIndex = 14;
            // 
            // memoryGroupDividerLabel3
            // 
            this.memoryGroupDividerLabel3.BackColor = System.Drawing.Color.Silver;
            this.memoryGroupDividerLabel3.Dock = System.Windows.Forms.DockStyle.Top;
            this.memoryGroupDividerLabel3.Location = new System.Drawing.Point(0, 0);
            this.memoryGroupDividerLabel3.Name = "memoryGroupDividerLabel3";
            this.memoryGroupDividerLabel3.Size = new System.Drawing.Size(138, 1);
            this.memoryGroupDividerLabel3.TabIndex = 21;
            // 
            // bufferCacheSizeLabel
            // 
            this.bufferCacheSizeLabel.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.bufferCacheSizeLabel.AutoEllipsis = true;
            this.bufferCacheSizeLabel.BackColor = System.Drawing.Color.Transparent;
            this.bufferCacheSizeLabel.Location = new System.Drawing.Point(12, 37);
            this.bufferCacheSizeLabel.Name = "bufferCacheSizeLabel";
            this.bufferCacheSizeLabel.Size = new System.Drawing.Size(115, 13);
            this.bufferCacheSizeLabel.TabIndex = 15;
            this.bufferCacheSizeLabel.Text = "Size: ?";
            this.bufferCacheSizeLabel.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // bufferCacheHitRateLabel
            // 
            this.bufferCacheHitRateLabel.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.bufferCacheHitRateLabel.AutoEllipsis = true;
            this.bufferCacheHitRateLabel.AutoSize = true;
            this.bufferCacheHitRateLabel.BackColor = System.Drawing.Color.Transparent;
            this.bufferCacheHitRateLabel.Location = new System.Drawing.Point(46, 57);
            this.bufferCacheHitRateLabel.Name = "bufferCacheHitRateLabel";
            this.bufferCacheHitRateLabel.Size = new System.Drawing.Size(46, 13);
            this.bufferCacheHitRateLabel.TabIndex = 14;
            this.bufferCacheHitRateLabel.Text = "Hit Rate";
            this.bufferCacheHitRateLabel.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // bufferCacheHitRateStatusBar
            // 
            this.bufferCacheHitRateStatusBar.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.bufferCacheHitRateStatusBar.Location = new System.Drawing.Point(12, 73);
            this.bufferCacheHitRateStatusBar.Name = "bufferCacheHitRateStatusBar";
            this.bufferCacheHitRateStatusBar.Size = new System.Drawing.Size(115, 23);
            this.bufferCacheHitRateStatusBar.TabIndex = 13;
            this.bufferCacheHitRateStatusBar.Text = "[Formatted]";
            // 
            // bufferCacheLabel
            // 
            this.bufferCacheLabel.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.bufferCacheLabel.AutoEllipsis = true;
            this.bufferCacheLabel.AutoSize = true;
            this.bufferCacheLabel.BackColor = System.Drawing.Color.Transparent;
            this.bufferCacheLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.bufferCacheLabel.Location = new System.Drawing.Point(26, 16);
            this.bufferCacheLabel.Name = "bufferCacheLabel";
            this.bufferCacheLabel.Size = new System.Drawing.Size(81, 13);
            this.bufferCacheLabel.TabIndex = 3;
            this.bufferCacheLabel.Text = "Buffer Cache";
            this.bufferCacheLabel.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            // 
            // sqlMemoryUsagePanel
            // 
            this.sqlMemoryUsagePanel.Controls.Add(this.sqlMemoryAllocatedLabel);
            this.sqlMemoryUsagePanel.Controls.Add(this.sqlMemoryUsageMinimumLabel);
            this.sqlMemoryUsagePanel.Controls.Add(this.sqlMemoryUsageStatusBar);
            this.sqlMemoryUsagePanel.Controls.Add(this.sqlMemoryUsageLabel);
            this.sqlMemoryUsagePanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.sqlMemoryUsagePanel.Location = new System.Drawing.Point(3, 3);
            this.sqlMemoryUsagePanel.Name = "sqlMemoryUsagePanel";
            this.sqlMemoryUsagePanel.Size = new System.Drawing.Size(138, 112);
            this.sqlMemoryUsagePanel.TabIndex = 13;
            // 
            // sqlMemoryAllocatedLabel
            // 
            this.sqlMemoryAllocatedLabel.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.sqlMemoryAllocatedLabel.Location = new System.Drawing.Point(40, 74);
            this.sqlMemoryAllocatedLabel.Name = "sqlMemoryAllocatedLabel";
            this.sqlMemoryAllocatedLabel.Size = new System.Drawing.Size(88, 13);
            this.sqlMemoryAllocatedLabel.TabIndex = 12;
            this.sqlMemoryAllocatedLabel.Text = "100 MB";
            this.sqlMemoryAllocatedLabel.TextAlign = System.Drawing.ContentAlignment.TopRight;
            // 
            // sqlMemoryUsageMinimumLabel
            // 
            this.sqlMemoryUsageMinimumLabel.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.sqlMemoryUsageMinimumLabel.AutoSize = true;
            this.sqlMemoryUsageMinimumLabel.Location = new System.Drawing.Point(10, 74);
            this.sqlMemoryUsageMinimumLabel.Name = "sqlMemoryUsageMinimumLabel";
            this.sqlMemoryUsageMinimumLabel.Size = new System.Drawing.Size(13, 13);
            this.sqlMemoryUsageMinimumLabel.TabIndex = 11;
            this.sqlMemoryUsageMinimumLabel.Text = "0";
            // 
            // sqlMemoryUsageStatusBar
            // 
            this.sqlMemoryUsageStatusBar.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.sqlMemoryUsageStatusBar.Location = new System.Drawing.Point(11, 48);
            this.sqlMemoryUsageStatusBar.Name = "sqlMemoryUsageStatusBar";
            this.sqlMemoryUsageStatusBar.Size = new System.Drawing.Size(115, 23);
            this.sqlMemoryUsageStatusBar.TabIndex = 10;
            this.sqlMemoryUsageStatusBar.Text = "[Value]";
            // 
            // sqlMemoryUsageLabel
            // 
            this.sqlMemoryUsageLabel.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.sqlMemoryUsageLabel.AutoEllipsis = true;
            this.sqlMemoryUsageLabel.AutoSize = true;
            this.sqlMemoryUsageLabel.BackColor = System.Drawing.Color.Transparent;
            this.sqlMemoryUsageLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.sqlMemoryUsageLabel.Location = new System.Drawing.Point(8, 25);
            this.sqlMemoryUsageLabel.Name = "sqlMemoryUsageLabel";
            this.sqlMemoryUsageLabel.Size = new System.Drawing.Size(118, 13);
            this.sqlMemoryUsageLabel.TabIndex = 9;
            this.sqlMemoryUsageLabel.Text = "SQL Memory Usage";
            this.sqlMemoryUsageLabel.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            // 
            // label4
            // 
            this.label4.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.label4.AutoSize = true;
            this.label4.BackColor = System.Drawing.Color.Transparent;
            this.label4.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label4.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(32)))), ((int)(((byte)(77)))), ((int)(((byte)(137)))));
            this.label4.Location = new System.Drawing.Point(40, 5);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(63, 16);
            this.label4.TabIndex = 0;
            this.label4.Text = "Memory";
            // 
            // processorGroupPanel
            // 
            this.processorGroupPanel.BackColor = System.Drawing.Color.Transparent;
            this.processorGroupPanel.Controls.Add(this.processorGroupLayoutPanel);
            this.processorGroupPanel.Controls.Add(this.label3);
            this.processorGroupPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.processorGroupPanel.GroupBoxBackColor = System.Drawing.Color.White;
            this.processorGroupPanel.Location = new System.Drawing.Point(203, 0);
            this.processorGroupPanel.Margin = new System.Windows.Forms.Padding(0);
            this.processorGroupPanel.Name = "processorGroupPanel";
            this.processorGroupPanel.Size = new System.Drawing.Size(150, 501);
            this.processorGroupPanel.Style = Idera.SQLdm.DesktopClient.Controls.GroupPanelStyle.Space;
            this.processorGroupPanel.TabIndex = 2;
            // 
            // processorGroupLayoutPanel
            // 
            this.processorGroupLayoutPanel.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.processorGroupLayoutPanel.BackColor = System.Drawing.Color.Transparent;
            this.processorGroupLayoutPanel.ColumnCount = 1;
            this.processorGroupLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.processorGroupLayoutPanel.Controls.Add(this.processorUsagePanel, 0, 3);
            this.processorGroupLayoutPanel.Controls.Add(this.processorQueueLengthPanel, 0, 2);
            this.processorGroupLayoutPanel.Controls.Add(this.blockedProcessesPanel, 0, 1);
            this.processorGroupLayoutPanel.Controls.Add(this.sqlProcessesPanel, 0, 0);
            this.processorGroupLayoutPanel.Location = new System.Drawing.Point(3, 25);
            this.processorGroupLayoutPanel.Name = "processorGroupLayoutPanel";
            this.processorGroupLayoutPanel.RowCount = 4;
            this.processorGroupLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 25F));
            this.processorGroupLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 25F));
            this.processorGroupLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 25F));
            this.processorGroupLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 25F));
            this.processorGroupLayoutPanel.Size = new System.Drawing.Size(144, 473);
            this.processorGroupLayoutPanel.TabIndex = 14;
            // 
            // processorUsagePanel
            // 
            this.processorUsagePanel.Controls.Add(this.processorUsageLabel);
            this.processorUsagePanel.Controls.Add(this.processorUsageValueLabel);
            this.processorUsagePanel.Controls.Add(this.processorUsageFlowControl);
            this.processorUsagePanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.processorUsagePanel.Location = new System.Drawing.Point(3, 357);
            this.processorUsagePanel.Name = "processorUsagePanel";
            this.processorUsagePanel.Size = new System.Drawing.Size(138, 113);
            this.processorUsagePanel.TabIndex = 16;
            // 
            // processorUsageLabel
            // 
            this.processorUsageLabel.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.processorUsageLabel.AutoEllipsis = true;
            this.processorUsageLabel.AutoSize = true;
            this.processorUsageLabel.BackColor = System.Drawing.Color.Transparent;
            this.processorUsageLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.processorUsageLabel.Location = new System.Drawing.Point(17, 6);
            this.processorUsageLabel.Name = "processorUsageLabel";
            this.processorUsageLabel.Size = new System.Drawing.Size(103, 13);
            this.processorUsageLabel.TabIndex = 7;
            this.processorUsageLabel.Text = "Processor Usage";
            this.processorUsageLabel.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            // 
            // processorUsageValueLabel
            // 
            this.processorUsageValueLabel.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.processorUsageValueLabel.AutoEllipsis = true;
            this.processorUsageValueLabel.BackColor = System.Drawing.Color.Transparent;
            this.processorUsageValueLabel.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.processorUsageValueLabel.Location = new System.Drawing.Point(51, 59);
            this.processorUsageValueLabel.Name = "processorUsageValueLabel";
            this.processorUsageValueLabel.Size = new System.Drawing.Size(36, 15);
            this.processorUsageValueLabel.TabIndex = 9;
            this.processorUsageValueLabel.Text = "?";
            this.processorUsageValueLabel.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // processorUsageFlowControl
            // 
            this.processorUsageFlowControl.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.processorUsageFlowControl.ArrowDegrees = 10;
            this.processorUsageFlowControl.ColorEnd = System.Drawing.Color.FromArgb(((int)(((byte)(183)))), ((int)(((byte)(201)))), ((int)(((byte)(238)))));
            this.processorUsageFlowControl.ColorStart = System.Drawing.Color.FromArgb(((int)(((byte)(119)))), ((int)(((byte)(158)))), ((int)(((byte)(218)))));
            this.processorUsageFlowControl.Location = new System.Drawing.Point(27, 25);
            this.processorUsageFlowControl.MaximumValue = 100;
            this.processorUsageFlowControl.Name = "processorUsageFlowControl";
            this.processorUsageFlowControl.NumberSegments = 4;
            this.processorUsageFlowControl.Rotation = Idera.SQLdm.DesktopClient.Controls.InfiniteProgressCircle.Direction.Clockwise;
            this.processorUsageFlowControl.Size = new System.Drawing.Size(85, 85);
            this.processorUsageFlowControl.TabIndex = 10;
            this.processorUsageFlowControl.Text = "infiniteProgressCircle1";
            this.processorUsageFlowControl.Thickness = 15;
            this.processorUsageFlowControl.Value = 0;
            // 
            // processorQueueLengthPanel
            // 
            this.processorQueueLengthPanel.Controls.Add(this.processorGroupDividerLabel1);
            this.processorQueueLengthPanel.Controls.Add(this.processorQueueLengthMaximumLabel);
            this.processorQueueLengthPanel.Controls.Add(this.processorQueueLengthMinimumLabel);
            this.processorQueueLengthPanel.Controls.Add(this.processorQueueLengthStatusBar);
            this.processorQueueLengthPanel.Controls.Add(this.processorQueueLengthLabel);
            this.processorQueueLengthPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.processorQueueLengthPanel.Location = new System.Drawing.Point(3, 239);
            this.processorQueueLengthPanel.Name = "processorQueueLengthPanel";
            this.processorQueueLengthPanel.Size = new System.Drawing.Size(138, 112);
            this.processorQueueLengthPanel.TabIndex = 15;
            // 
            // processorGroupDividerLabel1
            // 
            this.processorGroupDividerLabel1.BackColor = System.Drawing.Color.Silver;
            this.processorGroupDividerLabel1.Dock = System.Windows.Forms.DockStyle.Top;
            this.processorGroupDividerLabel1.Location = new System.Drawing.Point(0, 0);
            this.processorGroupDividerLabel1.Name = "processorGroupDividerLabel1";
            this.processorGroupDividerLabel1.Size = new System.Drawing.Size(138, 1);
            this.processorGroupDividerLabel1.TabIndex = 21;
            // 
            // processorQueueLengthMaximumLabel
            // 
            this.processorQueueLengthMaximumLabel.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.processorQueueLengthMaximumLabel.AutoSize = true;
            this.processorQueueLengthMaximumLabel.Location = new System.Drawing.Point(116, 74);
            this.processorQueueLengthMaximumLabel.Name = "processorQueueLengthMaximumLabel";
            this.processorQueueLengthMaximumLabel.Size = new System.Drawing.Size(13, 13);
            this.processorQueueLengthMaximumLabel.TabIndex = 8;
            this.processorQueueLengthMaximumLabel.Text = "5";
            // 
            // processorQueueLengthMinimumLabel
            // 
            this.processorQueueLengthMinimumLabel.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.processorQueueLengthMinimumLabel.AutoSize = true;
            this.processorQueueLengthMinimumLabel.Location = new System.Drawing.Point(11, 74);
            this.processorQueueLengthMinimumLabel.Name = "processorQueueLengthMinimumLabel";
            this.processorQueueLengthMinimumLabel.Size = new System.Drawing.Size(13, 13);
            this.processorQueueLengthMinimumLabel.TabIndex = 7;
            this.processorQueueLengthMinimumLabel.Text = "0";
            // 
            // processorQueueLengthStatusBar
            // 
            this.processorQueueLengthStatusBar.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.processorQueueLengthStatusBar.Location = new System.Drawing.Point(12, 48);
            this.processorQueueLengthStatusBar.Maximum = 5;
            this.processorQueueLengthStatusBar.Name = "processorQueueLengthStatusBar";
            this.processorQueueLengthStatusBar.Size = new System.Drawing.Size(115, 23);
            this.processorQueueLengthStatusBar.TabIndex = 6;
            this.processorQueueLengthStatusBar.Text = "[Value]";
            // 
            // processorQueueLengthLabel
            // 
            this.processorQueueLengthLabel.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.processorQueueLengthLabel.AutoEllipsis = true;
            this.processorQueueLengthLabel.AutoSize = true;
            this.processorQueueLengthLabel.BackColor = System.Drawing.Color.Transparent;
            this.processorQueueLengthLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.processorQueueLengthLabel.Location = new System.Drawing.Point(16, 25);
            this.processorQueueLengthLabel.Name = "processorQueueLengthLabel";
            this.processorQueueLengthLabel.Size = new System.Drawing.Size(104, 13);
            this.processorQueueLengthLabel.TabIndex = 5;
            this.processorQueueLengthLabel.Text = "Processor Queue";
            this.processorQueueLengthLabel.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            // 
            // blockedProcessesPanel
            // 
            this.blockedProcessesPanel.Controls.Add(this.blockedProcessesButton);
            this.blockedProcessesPanel.Controls.Add(this.blockedProcessesLabel);
            this.blockedProcessesPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.blockedProcessesPanel.Location = new System.Drawing.Point(3, 121);
            this.blockedProcessesPanel.Name = "blockedProcessesPanel";
            this.blockedProcessesPanel.Size = new System.Drawing.Size(138, 112);
            this.blockedProcessesPanel.TabIndex = 14;
            // 
            // blockedProcessesButton
            // 
            this.blockedProcessesButton.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.blockedProcessesButton.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.blockedProcessesButton.Location = new System.Drawing.Point(23, 50);
            this.blockedProcessesButton.MaximumSize = new System.Drawing.Size(100, 35);
            this.blockedProcessesButton.Name = "blockedProcessesButton";
            this.blockedProcessesButton.ShowFocusRect = false;
            this.blockedProcessesButton.ShowOutline = false;
            this.blockedProcessesButton.Size = new System.Drawing.Size(92, 35);
            this.blockedProcessesButton.TabIndex = 4;
            this.blockedProcessesButton.Text = "?";
            // 
            // blockedProcessesLabel
            // 
            this.blockedProcessesLabel.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.blockedProcessesLabel.AutoEllipsis = true;
            this.blockedProcessesLabel.AutoSize = true;
            this.blockedProcessesLabel.BackColor = System.Drawing.Color.Transparent;
            this.blockedProcessesLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.blockedProcessesLabel.Location = new System.Drawing.Point(12, 27);
            this.blockedProcessesLabel.Name = "blockedProcessesLabel";
            this.blockedProcessesLabel.Size = new System.Drawing.Size(115, 13);
            this.blockedProcessesLabel.TabIndex = 3;
            this.blockedProcessesLabel.Text = "Blocked Processes";
            this.blockedProcessesLabel.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            // 
            // sqlProcessesPanel
            // 
            this.sqlProcessesPanel.Controls.Add(this.sqlProcessesButton);
            this.sqlProcessesPanel.Controls.Add(this.sqlProcessesLabel);
            this.sqlProcessesPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.sqlProcessesPanel.Location = new System.Drawing.Point(3, 3);
            this.sqlProcessesPanel.Name = "sqlProcessesPanel";
            this.sqlProcessesPanel.Size = new System.Drawing.Size(138, 112);
            this.sqlProcessesPanel.TabIndex = 13;
            // 
            // sqlProcessesButton
            // 
            this.sqlProcessesButton.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.sqlProcessesButton.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.sqlProcessesButton.Location = new System.Drawing.Point(23, 51);
            this.sqlProcessesButton.MaximumSize = new System.Drawing.Size(100, 35);
            this.sqlProcessesButton.Name = "sqlProcessesButton";
            this.sqlProcessesButton.ShowFocusRect = false;
            this.sqlProcessesButton.ShowOutline = false;
            this.sqlProcessesButton.Size = new System.Drawing.Size(92, 35);
            this.sqlProcessesButton.TabIndex = 2;
            this.sqlProcessesButton.Text = "?";
            // 
            // sqlProcessesLabel
            // 
            this.sqlProcessesLabel.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.sqlProcessesLabel.AutoEllipsis = true;
            this.sqlProcessesLabel.AutoSize = true;
            this.sqlProcessesLabel.BackColor = System.Drawing.Color.Transparent;
            this.sqlProcessesLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.sqlProcessesLabel.Location = new System.Drawing.Point(23, 27);
            this.sqlProcessesLabel.Name = "sqlProcessesLabel";
            this.sqlProcessesLabel.Size = new System.Drawing.Size(93, 13);
            this.sqlProcessesLabel.TabIndex = 1;
            this.sqlProcessesLabel.Text = "SQL Processes";
            this.sqlProcessesLabel.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            // 
            // label3
            // 
            this.label3.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.label3.AutoSize = true;
            this.label3.BackColor = System.Drawing.Color.Transparent;
            this.label3.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label3.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(32)))), ((int)(((byte)(77)))), ((int)(((byte)(137)))));
            this.label3.Location = new System.Drawing.Point(36, 5);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(79, 16);
            this.label3.TabIndex = 0;
            this.label3.Text = "Processor";
            // 
            // sessionsDataFlowPanel
            // 
            this.sessionsDataFlowPanel.BackColor = System.Drawing.Color.Transparent;
            this.sessionsDataFlowPanel.Controls.Add(this.transactionsPerSecondFlowControl);
            this.sessionsDataFlowPanel.Controls.Add(this.packetsSentPerSecondFlowControl);
            this.sessionsDataFlowPanel.Controls.Add(this.packetsReceivedPerSecondFlowControl);
            this.sessionsDataFlowPanel.Controls.Add(this.transactionsPerSecondLabel);
            this.sessionsDataFlowPanel.Controls.Add(this.packetsSentPerSecondLabel);
            this.sessionsDataFlowPanel.Controls.Add(this.packetsReceivedPerSecondLabel);
            this.sessionsDataFlowPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.sessionsDataFlowPanel.Location = new System.Drawing.Point(150, 0);
            this.sessionsDataFlowPanel.Margin = new System.Windows.Forms.Padding(0);
            this.sessionsDataFlowPanel.Name = "sessionsDataFlowPanel";
            this.sessionsDataFlowPanel.Size = new System.Drawing.Size(53, 501);
            this.sessionsDataFlowPanel.TabIndex = 5;
            // 
            // transactionsPerSecondFlowControl
            // 
            this.transactionsPerSecondFlowControl.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.transactionsPerSecondFlowControl.BackColor = System.Drawing.Color.Transparent;
            this.transactionsPerSecondFlowControl.Color1 = System.Drawing.Color.FromArgb(((int)(((byte)(119)))), ((int)(((byte)(158)))), ((int)(((byte)(218)))));
            this.transactionsPerSecondFlowControl.Color2 = System.Drawing.Color.FromArgb(((int)(((byte)(183)))), ((int)(((byte)(201)))), ((int)(((byte)(238)))));
            this.transactionsPerSecondFlowControl.FlowDirection = Idera.SQLdm.DesktopClient.Controls.InfiniteProgressFlowBarDirection.Right;
            this.transactionsPerSecondFlowControl.Location = new System.Drawing.Point(0, 332);
            this.transactionsPerSecondFlowControl.MaximumValue = 100;
            this.transactionsPerSecondFlowControl.Name = "transactionsPerSecondFlowControl";
            this.transactionsPerSecondFlowControl.Size = new System.Drawing.Size(53, 23);
            this.transactionsPerSecondFlowControl.TabIndex = 10;
            this.transactionsPerSecondFlowControl.Value = 0;
            // 
            // packetsSentPerSecondFlowControl
            // 
            this.packetsSentPerSecondFlowControl.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.packetsSentPerSecondFlowControl.BackColor = System.Drawing.Color.Transparent;
            this.packetsSentPerSecondFlowControl.Color1 = System.Drawing.Color.FromArgb(((int)(((byte)(119)))), ((int)(((byte)(158)))), ((int)(((byte)(218)))));
            this.packetsSentPerSecondFlowControl.Color2 = System.Drawing.Color.FromArgb(((int)(((byte)(183)))), ((int)(((byte)(201)))), ((int)(((byte)(238)))));
            this.packetsSentPerSecondFlowControl.FlowDirection = Idera.SQLdm.DesktopClient.Controls.InfiniteProgressFlowBarDirection.Left;
            this.packetsSentPerSecondFlowControl.Location = new System.Drawing.Point(0, 216);
            this.packetsSentPerSecondFlowControl.MaximumValue = 100;
            this.packetsSentPerSecondFlowControl.Name = "packetsSentPerSecondFlowControl";
            this.packetsSentPerSecondFlowControl.Size = new System.Drawing.Size(53, 23);
            this.packetsSentPerSecondFlowControl.TabIndex = 9;
            this.packetsSentPerSecondFlowControl.Value = 0;
            // 
            // packetsReceivedPerSecondFlowControl
            // 
            this.packetsReceivedPerSecondFlowControl.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.packetsReceivedPerSecondFlowControl.BackColor = System.Drawing.Color.Transparent;
            this.packetsReceivedPerSecondFlowControl.Color1 = System.Drawing.Color.FromArgb(((int)(((byte)(119)))), ((int)(((byte)(158)))), ((int)(((byte)(218)))));
            this.packetsReceivedPerSecondFlowControl.Color2 = System.Drawing.Color.FromArgb(((int)(((byte)(183)))), ((int)(((byte)(201)))), ((int)(((byte)(238)))));
            this.packetsReceivedPerSecondFlowControl.FlowDirection = Idera.SQLdm.DesktopClient.Controls.InfiniteProgressFlowBarDirection.Right;
            this.packetsReceivedPerSecondFlowControl.Location = new System.Drawing.Point(0, 106);
            this.packetsReceivedPerSecondFlowControl.MaximumValue = 100;
            this.packetsReceivedPerSecondFlowControl.Name = "packetsReceivedPerSecondFlowControl";
            this.packetsReceivedPerSecondFlowControl.Size = new System.Drawing.Size(53, 23);
            this.packetsReceivedPerSecondFlowControl.TabIndex = 8;
            this.packetsReceivedPerSecondFlowControl.Value = 0;
            // 
            // transactionsPerSecondLabel
            // 
            this.transactionsPerSecondLabel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.transactionsPerSecondLabel.AutoEllipsis = true;
            this.transactionsPerSecondLabel.Location = new System.Drawing.Point(3, 358);
            this.transactionsPerSecondLabel.Name = "transactionsPerSecondLabel";
            this.transactionsPerSecondLabel.Size = new System.Drawing.Size(47, 28);
            this.transactionsPerSecondLabel.TabIndex = 2;
            this.transactionsPerSecondLabel.Text = "? Tran/s";
            this.transactionsPerSecondLabel.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            // 
            // packetsSentPerSecondLabel
            // 
            this.packetsSentPerSecondLabel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.packetsSentPerSecondLabel.AutoEllipsis = true;
            this.packetsSentPerSecondLabel.Location = new System.Drawing.Point(3, 242);
            this.packetsSentPerSecondLabel.Name = "packetsSentPerSecondLabel";
            this.packetsSentPerSecondLabel.Size = new System.Drawing.Size(47, 28);
            this.packetsSentPerSecondLabel.TabIndex = 1;
            this.packetsSentPerSecondLabel.Text = "? Pkt/s";
            this.packetsSentPerSecondLabel.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            // 
            // packetsReceivedPerSecondLabel
            // 
            this.packetsReceivedPerSecondLabel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.packetsReceivedPerSecondLabel.AutoEllipsis = true;
            this.packetsReceivedPerSecondLabel.Location = new System.Drawing.Point(3, 132);
            this.packetsReceivedPerSecondLabel.Name = "packetsReceivedPerSecondLabel";
            this.packetsReceivedPerSecondLabel.Size = new System.Drawing.Size(47, 28);
            this.packetsReceivedPerSecondLabel.TabIndex = 0;
            this.packetsReceivedPerSecondLabel.Text = "? Pkt/s";
            this.packetsReceivedPerSecondLabel.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            // 
            // processorDataFlowPanel
            // 
            this.processorDataFlowPanel.BackColor = System.Drawing.Color.Transparent;
            this.processorDataFlowPanel.Controls.Add(this.pageWritesPerSecondFlowControl);
            this.processorDataFlowPanel.Controls.Add(this.pageReadsPerSecondFlowControl);
            this.processorDataFlowPanel.Controls.Add(this.sqlCompilationsPerSecondFlowControl);
            this.processorDataFlowPanel.Controls.Add(this.sqlCompilationsPerSecondLabel);
            this.processorDataFlowPanel.Controls.Add(this.pageReadsPerSecondLabel);
            this.processorDataFlowPanel.Controls.Add(this.pageWritesPerSecondLabel);
            this.processorDataFlowPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.processorDataFlowPanel.Location = new System.Drawing.Point(353, 0);
            this.processorDataFlowPanel.Margin = new System.Windows.Forms.Padding(0);
            this.processorDataFlowPanel.Name = "processorDataFlowPanel";
            this.processorDataFlowPanel.Size = new System.Drawing.Size(53, 501);
            this.processorDataFlowPanel.TabIndex = 6;
            // 
            // pageWritesPerSecondFlowControl
            // 
            this.pageWritesPerSecondFlowControl.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.pageWritesPerSecondFlowControl.BackColor = System.Drawing.Color.Transparent;
            this.pageWritesPerSecondFlowControl.Color1 = System.Drawing.Color.FromArgb(((int)(((byte)(119)))), ((int)(((byte)(158)))), ((int)(((byte)(218)))));
            this.pageWritesPerSecondFlowControl.Color2 = System.Drawing.Color.FromArgb(((int)(((byte)(183)))), ((int)(((byte)(201)))), ((int)(((byte)(238)))));
            this.pageWritesPerSecondFlowControl.FlowDirection = Idera.SQLdm.DesktopClient.Controls.InfiniteProgressFlowBarDirection.Right;
            this.pageWritesPerSecondFlowControl.Location = new System.Drawing.Point(0, 106);
            this.pageWritesPerSecondFlowControl.MaximumValue = 100;
            this.pageWritesPerSecondFlowControl.Name = "pageWritesPerSecondFlowControl";
            this.pageWritesPerSecondFlowControl.Size = new System.Drawing.Size(53, 23);
            this.pageWritesPerSecondFlowControl.TabIndex = 8;
            this.pageWritesPerSecondFlowControl.Value = 0;
            // 
            // pageReadsPerSecondFlowControl
            // 
            this.pageReadsPerSecondFlowControl.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.pageReadsPerSecondFlowControl.BackColor = System.Drawing.Color.Transparent;
            this.pageReadsPerSecondFlowControl.Color1 = System.Drawing.Color.FromArgb(((int)(((byte)(119)))), ((int)(((byte)(158)))), ((int)(((byte)(218)))));
            this.pageReadsPerSecondFlowControl.Color2 = System.Drawing.Color.FromArgb(((int)(((byte)(183)))), ((int)(((byte)(201)))), ((int)(((byte)(238)))));
            this.pageReadsPerSecondFlowControl.FlowDirection = Idera.SQLdm.DesktopClient.Controls.InfiniteProgressFlowBarDirection.Left;
            this.pageReadsPerSecondFlowControl.Location = new System.Drawing.Point(0, 216);
            this.pageReadsPerSecondFlowControl.MaximumValue = 100;
            this.pageReadsPerSecondFlowControl.Name = "pageReadsPerSecondFlowControl";
            this.pageReadsPerSecondFlowControl.Size = new System.Drawing.Size(53, 23);
            this.pageReadsPerSecondFlowControl.TabIndex = 7;
            this.pageReadsPerSecondFlowControl.Value = 0;
            // 
            // sqlCompilationsPerSecondFlowControl
            // 
            this.sqlCompilationsPerSecondFlowControl.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.sqlCompilationsPerSecondFlowControl.BackColor = System.Drawing.Color.Transparent;
            this.sqlCompilationsPerSecondFlowControl.Color1 = System.Drawing.Color.FromArgb(((int)(((byte)(119)))), ((int)(((byte)(158)))), ((int)(((byte)(218)))));
            this.sqlCompilationsPerSecondFlowControl.Color2 = System.Drawing.Color.FromArgb(((int)(((byte)(183)))), ((int)(((byte)(201)))), ((int)(((byte)(238)))));
            this.sqlCompilationsPerSecondFlowControl.FlowDirection = Idera.SQLdm.DesktopClient.Controls.InfiniteProgressFlowBarDirection.Right;
            this.sqlCompilationsPerSecondFlowControl.Location = new System.Drawing.Point(0, 332);
            this.sqlCompilationsPerSecondFlowControl.MaximumValue = 100;
            this.sqlCompilationsPerSecondFlowControl.Name = "sqlCompilationsPerSecondFlowControl";
            this.sqlCompilationsPerSecondFlowControl.Size = new System.Drawing.Size(53, 23);
            this.sqlCompilationsPerSecondFlowControl.TabIndex = 6;
            this.sqlCompilationsPerSecondFlowControl.Value = 0;
            // 
            // sqlCompilationsPerSecondLabel
            // 
            this.sqlCompilationsPerSecondLabel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.sqlCompilationsPerSecondLabel.AutoEllipsis = true;
            this.sqlCompilationsPerSecondLabel.Location = new System.Drawing.Point(3, 358);
            this.sqlCompilationsPerSecondLabel.Name = "sqlCompilationsPerSecondLabel";
            this.sqlCompilationsPerSecondLabel.Size = new System.Drawing.Size(47, 28);
            this.sqlCompilationsPerSecondLabel.TabIndex = 5;
            this.sqlCompilationsPerSecondLabel.Text = "? Cmp/s";
            this.sqlCompilationsPerSecondLabel.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            // 
            // pageReadsPerSecondLabel
            // 
            this.pageReadsPerSecondLabel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.pageReadsPerSecondLabel.AutoEllipsis = true;
            this.pageReadsPerSecondLabel.Location = new System.Drawing.Point(3, 242);
            this.pageReadsPerSecondLabel.Name = "pageReadsPerSecondLabel";
            this.pageReadsPerSecondLabel.Size = new System.Drawing.Size(47, 28);
            this.pageReadsPerSecondLabel.TabIndex = 4;
            this.pageReadsPerSecondLabel.Text = "? R/s";
            this.pageReadsPerSecondLabel.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            // 
            // pageWritesPerSecondLabel
            // 
            this.pageWritesPerSecondLabel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.pageWritesPerSecondLabel.AutoEllipsis = true;
            this.pageWritesPerSecondLabel.Location = new System.Drawing.Point(3, 132);
            this.pageWritesPerSecondLabel.Name = "pageWritesPerSecondLabel";
            this.pageWritesPerSecondLabel.Size = new System.Drawing.Size(47, 28);
            this.pageWritesPerSecondLabel.TabIndex = 3;
            this.pageWritesPerSecondLabel.Text = "? W/s";
            this.pageWritesPerSecondLabel.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            // 
            // memoryDataFlowPanel
            // 
            this.memoryDataFlowPanel.BackColor = System.Drawing.Color.Transparent;
            this.memoryDataFlowPanel.Controls.Add(this.logFlushesPerSecondFlowControl);
            this.memoryDataFlowPanel.Controls.Add(this.pageReadsPerSecondFlowControl2);
            this.memoryDataFlowPanel.Controls.Add(this.pageWritesPerSecondFlowControl2);
            this.memoryDataFlowPanel.Controls.Add(this.logFlushesPerSecondLabel);
            this.memoryDataFlowPanel.Controls.Add(this.pageReadsPerSecondLabel2);
            this.memoryDataFlowPanel.Controls.Add(this.pageWritesPerSecondLabel2);
            this.memoryDataFlowPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.memoryDataFlowPanel.Location = new System.Drawing.Point(556, 0);
            this.memoryDataFlowPanel.Margin = new System.Windows.Forms.Padding(0);
            this.memoryDataFlowPanel.Name = "memoryDataFlowPanel";
            this.memoryDataFlowPanel.Size = new System.Drawing.Size(53, 501);
            this.memoryDataFlowPanel.TabIndex = 7;
            // 
            // logFlushesPerSecondFlowControl
            // 
            this.logFlushesPerSecondFlowControl.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.logFlushesPerSecondFlowControl.BackColor = System.Drawing.Color.Transparent;
            this.logFlushesPerSecondFlowControl.Color1 = System.Drawing.Color.FromArgb(((int)(((byte)(119)))), ((int)(((byte)(158)))), ((int)(((byte)(218)))));
            this.logFlushesPerSecondFlowControl.Color2 = System.Drawing.Color.FromArgb(((int)(((byte)(183)))), ((int)(((byte)(201)))), ((int)(((byte)(238)))));
            this.logFlushesPerSecondFlowControl.FlowDirection = Idera.SQLdm.DesktopClient.Controls.InfiniteProgressFlowBarDirection.Right;
            this.logFlushesPerSecondFlowControl.Location = new System.Drawing.Point(0, 332);
            this.logFlushesPerSecondFlowControl.MaximumValue = 100;
            this.logFlushesPerSecondFlowControl.Name = "logFlushesPerSecondFlowControl";
            this.logFlushesPerSecondFlowControl.Size = new System.Drawing.Size(53, 23);
            this.logFlushesPerSecondFlowControl.TabIndex = 12;
            this.logFlushesPerSecondFlowControl.Value = 0;
            // 
            // pageReadsPerSecondFlowControl2
            // 
            this.pageReadsPerSecondFlowControl2.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.pageReadsPerSecondFlowControl2.BackColor = System.Drawing.Color.Transparent;
            this.pageReadsPerSecondFlowControl2.Color1 = System.Drawing.Color.FromArgb(((int)(((byte)(119)))), ((int)(((byte)(158)))), ((int)(((byte)(218)))));
            this.pageReadsPerSecondFlowControl2.Color2 = System.Drawing.Color.FromArgb(((int)(((byte)(183)))), ((int)(((byte)(201)))), ((int)(((byte)(238)))));
            this.pageReadsPerSecondFlowControl2.FlowDirection = Idera.SQLdm.DesktopClient.Controls.InfiniteProgressFlowBarDirection.Left;
            this.pageReadsPerSecondFlowControl2.Location = new System.Drawing.Point(0, 216);
            this.pageReadsPerSecondFlowControl2.MaximumValue = 100;
            this.pageReadsPerSecondFlowControl2.Name = "pageReadsPerSecondFlowControl2";
            this.pageReadsPerSecondFlowControl2.Size = new System.Drawing.Size(53, 23);
            this.pageReadsPerSecondFlowControl2.TabIndex = 11;
            this.pageReadsPerSecondFlowControl2.Value = 0;
            // 
            // pageWritesPerSecondFlowControl2
            // 
            this.pageWritesPerSecondFlowControl2.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.pageWritesPerSecondFlowControl2.BackColor = System.Drawing.Color.Transparent;
            this.pageWritesPerSecondFlowControl2.Color1 = System.Drawing.Color.FromArgb(((int)(((byte)(119)))), ((int)(((byte)(158)))), ((int)(((byte)(218)))));
            this.pageWritesPerSecondFlowControl2.Color2 = System.Drawing.Color.FromArgb(((int)(((byte)(183)))), ((int)(((byte)(201)))), ((int)(((byte)(238)))));
            this.pageWritesPerSecondFlowControl2.FlowDirection = Idera.SQLdm.DesktopClient.Controls.InfiniteProgressFlowBarDirection.Right;
            this.pageWritesPerSecondFlowControl2.Location = new System.Drawing.Point(0, 106);
            this.pageWritesPerSecondFlowControl2.MaximumValue = 100;
            this.pageWritesPerSecondFlowControl2.Name = "pageWritesPerSecondFlowControl2";
            this.pageWritesPerSecondFlowControl2.Size = new System.Drawing.Size(53, 23);
            this.pageWritesPerSecondFlowControl2.TabIndex = 10;
            this.pageWritesPerSecondFlowControl2.Value = 0;
            // 
            // logFlushesPerSecondLabel
            // 
            this.logFlushesPerSecondLabel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.logFlushesPerSecondLabel.AutoEllipsis = true;
            this.logFlushesPerSecondLabel.Location = new System.Drawing.Point(3, 356);
            this.logFlushesPerSecondLabel.Name = "logFlushesPerSecondLabel";
            this.logFlushesPerSecondLabel.Size = new System.Drawing.Size(47, 28);
            this.logFlushesPerSecondLabel.TabIndex = 8;
            this.logFlushesPerSecondLabel.Text = "? Flush/s";
            this.logFlushesPerSecondLabel.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            // 
            // pageReadsPerSecondLabel2
            // 
            this.pageReadsPerSecondLabel2.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.pageReadsPerSecondLabel2.AutoEllipsis = true;
            this.pageReadsPerSecondLabel2.Location = new System.Drawing.Point(3, 242);
            this.pageReadsPerSecondLabel2.Name = "pageReadsPerSecondLabel2";
            this.pageReadsPerSecondLabel2.Size = new System.Drawing.Size(47, 28);
            this.pageReadsPerSecondLabel2.TabIndex = 7;
            this.pageReadsPerSecondLabel2.Text = "? R/s";
            this.pageReadsPerSecondLabel2.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            // 
            // pageWritesPerSecondLabel2
            // 
            this.pageWritesPerSecondLabel2.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.pageWritesPerSecondLabel2.AutoEllipsis = true;
            this.pageWritesPerSecondLabel2.Location = new System.Drawing.Point(3, 132);
            this.pageWritesPerSecondLabel2.Name = "pageWritesPerSecondLabel2";
            this.pageWritesPerSecondLabel2.Size = new System.Drawing.Size(47, 28);
            this.pageWritesPerSecondLabel2.TabIndex = 6;
            this.pageWritesPerSecondLabel2.Text = "? W/s";
            this.pageWritesPerSecondLabel2.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            // 
            // headerPanel
            // 
            this.headerPanel.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.headerPanel.Controls.Add(this.versionInformationLabel);
            this.headerPanel.FillColor1 = System.Drawing.Color.FromArgb(((int)(((byte)(252)))), ((int)(((byte)(253)))), ((int)(((byte)(254)))));
            this.headerPanel.FillColor2 = System.Drawing.Color.FromArgb(((int)(((byte)(194)))), ((int)(((byte)(212)))), ((int)(((byte)(235)))));
            this.headerPanel.Location = new System.Drawing.Point(6, 6);
            this.headerPanel.Name = "headerPanel";
            this.headerPanel.Size = new System.Drawing.Size(764, 31);
            this.headerPanel.TabIndex = 5;
            // 
            // versionInformationLabel
            // 
            this.versionInformationLabel.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.versionInformationLabel.AutoEllipsis = true;
            this.versionInformationLabel.BackColor = System.Drawing.Color.Transparent;
            this.versionInformationLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.versionInformationLabel.Location = new System.Drawing.Point(3, 3);
            this.versionInformationLabel.Name = "versionInformationLabel";
            this.versionInformationLabel.Size = new System.Drawing.Size(756, 23);
            this.versionInformationLabel.TabIndex = 0;
            this.versionInformationLabel.Text = "< Version Info >";
            this.versionInformationLabel.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // ServerMonitorView
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.Transparent;
            this.Controls.Add(this.areaLayoutPanel);
            this.Controls.Add(this.headerPanel);
            this.MinimumSize = new System.Drawing.Size(775, 550);
            this.Name = "ServerMonitorView";
            this.Size = new System.Drawing.Size(775, 550);
            this.areaLayoutPanel.ResumeLayout(false);
            this.sessionsGroupPanel.ResumeLayout(false);
            this.sessionsGroupPanel.PerformLayout();
            this.sessionsGroupLayoutPanel.ResumeLayout(false);
            this.activeUserProcessesPanel.ResumeLayout(false);
            this.activeUserProcessesPanel.PerformLayout();
            this.userProcessesPanel.ResumeLayout(false);
            this.userProcessesPanel.PerformLayout();
            this.clientComputersPanel.ResumeLayout(false);
            this.clientComputersPanel.PerformLayout();
            this.responseTimePanel.ResumeLayout(false);
            this.responseTimePanel.PerformLayout();
            this.diskGroupPanel.ResumeLayout(false);
            this.diskGroupPanel.PerformLayout();
            this.diskGroupLayoutPanel.ResumeLayout(false);
            this.diskQueueLengthPanel.ResumeLayout(false);
            this.diskQueueLengthPanel.PerformLayout();
            this.logFilesPanel.ResumeLayout(false);
            this.logFilesPanel.PerformLayout();
            this.dataFilesPanel.ResumeLayout(false);
            this.dataFilesPanel.PerformLayout();
            this.databasesPanel.ResumeLayout(false);
            this.databasesPanel.PerformLayout();
            this.memoryGroupPanel.ResumeLayout(false);
            this.memoryGroupPanel.PerformLayout();
            this.memoryGroupLayoutPanel.ResumeLayout(false);
            this.pagingPanel.ResumeLayout(false);
            this.pagingPanel.PerformLayout();
            this.procedureCachePanel.ResumeLayout(false);
            this.procedureCachePanel.PerformLayout();
            this.bufferCachePanel.ResumeLayout(false);
            this.bufferCachePanel.PerformLayout();
            this.sqlMemoryUsagePanel.ResumeLayout(false);
            this.sqlMemoryUsagePanel.PerformLayout();
            this.processorGroupPanel.ResumeLayout(false);
            this.processorGroupPanel.PerformLayout();
            this.processorGroupLayoutPanel.ResumeLayout(false);
            this.processorUsagePanel.ResumeLayout(false);
            this.processorUsagePanel.PerformLayout();
            this.processorQueueLengthPanel.ResumeLayout(false);
            this.processorQueueLengthPanel.PerformLayout();
            this.blockedProcessesPanel.ResumeLayout(false);
            this.blockedProcessesPanel.PerformLayout();
            this.sqlProcessesPanel.ResumeLayout(false);
            this.sqlProcessesPanel.PerformLayout();
            this.sessionsDataFlowPanel.ResumeLayout(false);
            this.processorDataFlowPanel.ResumeLayout(false);
            this.memoryDataFlowPanel.ResumeLayout(false);
            this.headerPanel.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private Idera.SQLdm.DesktopClient.Controls.PulseHeaderPanel headerPanel;
        private System.ComponentModel.BackgroundWorker refreshBackgroundWorker;
        private Idera.SQLdm.DesktopClient.Controls.GroupPanel processorGroupPanel;
        private System.Windows.Forms.Label label3;
        private Idera.SQLdm.DesktopClient.Controls.GroupPanel memoryGroupPanel;
        private System.Windows.Forms.Label label4;
        private Idera.SQLdm.DesktopClient.Controls.GroupPanel diskGroupPanel;
        private System.Windows.Forms.Label databaseGroupLabel;
        private Idera.SQLdm.DesktopClient.Controls.GroupPanel sessionsGroupPanel;
        private System.Windows.Forms.Label activeUserProcessesLabel;
        private Infragistics.Win.Misc.UltraButton activeUserProcessesButton;
        private System.Windows.Forms.Label userProcessesLabel;
        private Infragistics.Win.Misc.UltraButton userProcessesButton;
        private System.Windows.Forms.Label clientComputersLabel;
        private Infragistics.Win.Misc.UltraButton clientComputersButton;
        private System.Windows.Forms.Label responseTimeLabel;
        private Infragistics.Win.Misc.UltraButton responseTimeButton;
        private System.Windows.Forms.Label sessionsGroupLabel;
        private System.Windows.Forms.TableLayoutPanel areaLayoutPanel;
        private System.Windows.Forms.TableLayoutPanel sessionsGroupLayoutPanel;
        private System.Windows.Forms.Panel activeUserProcessesPanel;
        private System.Windows.Forms.Panel userProcessesPanel;
        private System.Windows.Forms.Panel clientComputersPanel;
        private System.Windows.Forms.Panel responseTimePanel;
        private System.Windows.Forms.TableLayoutPanel processorGroupLayoutPanel;
        private System.Windows.Forms.Panel processorUsagePanel;
        private System.Windows.Forms.Label processorUsageLabel;
        private System.Windows.Forms.Panel processorQueueLengthPanel;
        private System.Windows.Forms.Label processorQueueLengthLabel;
        private System.Windows.Forms.Panel blockedProcessesPanel;
        private Infragistics.Win.Misc.UltraButton blockedProcessesButton;
        private System.Windows.Forms.Label blockedProcessesLabel;
        private System.Windows.Forms.Panel sqlProcessesPanel;
        private Infragistics.Win.Misc.UltraButton sqlProcessesButton;
        private System.Windows.Forms.Label sqlProcessesLabel;
        private Infragistics.Win.UltraWinProgressBar.UltraProgressBar processorQueueLengthStatusBar;
        private System.Windows.Forms.Label processorQueueLengthMaximumLabel;
        private System.Windows.Forms.Label processorQueueLengthMinimumLabel;
        private System.Windows.Forms.TableLayoutPanel memoryGroupLayoutPanel;
        private System.Windows.Forms.Panel pagingPanel;
        private System.Windows.Forms.Label pagingLabel;
        private System.Windows.Forms.Panel procedureCachePanel;
        private System.Windows.Forms.Panel bufferCachePanel;
        private System.Windows.Forms.Label bufferCacheLabel;
        private System.Windows.Forms.Panel sqlMemoryUsagePanel;
        private System.Windows.Forms.Label sqlMemoryAllocatedLabel;
        private System.Windows.Forms.Label sqlMemoryUsageMinimumLabel;
        private Infragistics.Win.UltraWinProgressBar.UltraProgressBar sqlMemoryUsageStatusBar;
        private System.Windows.Forms.Label sqlMemoryUsageLabel;
        private Infragistics.Win.UltraWinProgressBar.UltraProgressBar bufferCacheHitRateStatusBar;
        private System.Windows.Forms.Label bufferCacheSizeLabel;
        private System.Windows.Forms.Label bufferCacheHitRateLabel;
        private System.Windows.Forms.Label procedureCacheSizeLabel;
        private System.Windows.Forms.Label procedureCacheHitRateLabel;
        private Infragistics.Win.UltraWinProgressBar.UltraProgressBar procedureCacheHitRateStatusBar;
        private System.Windows.Forms.Label procedureCacheLabel;
        private System.Windows.Forms.Label pagingValueLabel;
        private System.Windows.Forms.Label processorUsageValueLabel;
        private System.Windows.Forms.TableLayoutPanel diskGroupLayoutPanel;
        private System.Windows.Forms.Panel diskQueueLengthPanel;
        private System.Windows.Forms.Label diskQueueLengthMaximumLabel;
        private System.Windows.Forms.Label diskQueueLengthMinimumLabel;
        private Infragistics.Win.UltraWinProgressBar.UltraProgressBar diskQueueLengthStatusBar;
        private System.Windows.Forms.Label diskQueueLengthLabel;
        private System.Windows.Forms.Panel logFilesPanel;
        private System.Windows.Forms.Panel dataFilesPanel;
        private System.Windows.Forms.Panel databasesPanel;
        private Infragistics.Win.Misc.UltraButton databasesButton;
        private System.Windows.Forms.Label databasesLabel;
        private Infragistics.Win.UltraWinProgressBar.UltraProgressBar dataFilesPercentFullStatusBar;
        private System.Windows.Forms.Label dataFilesLabel;
        private System.Windows.Forms.Label dataFilesSizeLabel;
        private System.Windows.Forms.Label dataFilesCountLabel;
        private System.Windows.Forms.Label dataFilesCountValueLabel;
        private System.Windows.Forms.Label dataFilesSizeValueLabel;
        private System.Windows.Forms.Label logFilesCountValueLabel;
        private System.Windows.Forms.Label logFilesSizeValueLabel;
        private System.Windows.Forms.Label logFilesSizeLabel;
        private System.Windows.Forms.Label logFilesCountLabel;
        private System.Windows.Forms.Label logFilesLabel;
        private Infragistics.Win.UltraWinProgressBar.UltraProgressBar logFilesPercentFullStatusBar;
        private System.Windows.Forms.Label databaseGroupDividerLabel3;
        private System.Windows.Forms.Label databaseGroupDividerLabel1;
        private System.Windows.Forms.Label databaseGroupDividerLabel2;
        private System.Windows.Forms.Label memoryGroupDividerLabel1;
        private System.Windows.Forms.Label memoryGroupDividerLabel2;
        private System.Windows.Forms.Label memoryGroupDividerLabel3;
        private System.Windows.Forms.Label processorGroupDividerLabel1;
        private System.Windows.Forms.Panel sessionsDataFlowPanel;
        private System.Windows.Forms.Label transactionsPerSecondLabel;
        private System.Windows.Forms.Label packetsSentPerSecondLabel;
        private System.Windows.Forms.Label packetsReceivedPerSecondLabel;
        private System.Windows.Forms.Panel processorDataFlowPanel;
        private System.Windows.Forms.Label sqlCompilationsPerSecondLabel;
        private System.Windows.Forms.Label pageReadsPerSecondLabel;
        private System.Windows.Forms.Label pageWritesPerSecondLabel;
        private System.Windows.Forms.Panel memoryDataFlowPanel;
        private System.Windows.Forms.Label logFlushesPerSecondLabel;
        private System.Windows.Forms.Label pageReadsPerSecondLabel2;
        private System.Windows.Forms.Label pageWritesPerSecondLabel2;
        private System.Windows.Forms.Label versionInformationLabel;
        private Idera.SQLdm.DesktopClient.Controls.InfiniteProgressFlowBar sqlCompilationsPerSecondFlowControl;
        private Idera.SQLdm.DesktopClient.Controls.InfiniteProgressFlowBar packetsSentPerSecondFlowControl;
        private Idera.SQLdm.DesktopClient.Controls.InfiniteProgressFlowBar packetsReceivedPerSecondFlowControl;
        private Idera.SQLdm.DesktopClient.Controls.InfiniteProgressFlowBar pageWritesPerSecondFlowControl;
        private Idera.SQLdm.DesktopClient.Controls.InfiniteProgressFlowBar pageReadsPerSecondFlowControl;
        private Idera.SQLdm.DesktopClient.Controls.InfiniteProgressFlowBar transactionsPerSecondFlowControl;
        private Idera.SQLdm.DesktopClient.Controls.InfiniteProgressFlowBar logFlushesPerSecondFlowControl;
        private Idera.SQLdm.DesktopClient.Controls.InfiniteProgressFlowBar pageReadsPerSecondFlowControl2;
        private Idera.SQLdm.DesktopClient.Controls.InfiniteProgressFlowBar pageWritesPerSecondFlowControl2;
        private Idera.SQLdm.DesktopClient.Controls.InfiniteProgressCircle pagingFlowControl;
        private Idera.SQLdm.DesktopClient.Controls.InfiniteProgressCircle processorUsageFlowControl;
    }
}
