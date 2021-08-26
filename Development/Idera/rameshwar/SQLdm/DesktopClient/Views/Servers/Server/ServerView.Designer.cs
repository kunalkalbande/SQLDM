namespace Idera.SQLdm.DesktopClient.Views.Servers.Server
{
    partial class ServerView
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
            this.borderPanel = new Infragistics.Win.Misc.UltraPanel();
            this.ribbonControl = new Idera.SQLdm.DesktopClient.Views.Servers.Server.ServerViewRibbonControl();
            this.headerStrip = new Idera.SQLdm.DesktopClient.Controls.HeaderStrip();
            this.titleLabel = new System.Windows.Forms.ToolStripLabel();
            this.historyTimestampLabel = new System.Windows.Forms.ToolStripLabel();
            this.splitContainer = new System.Windows.Forms.SplitContainer();
            this.historyBrowserControl = new Idera.SQLdm.DesktopClient.Controls.HistoryBrowserControl();
            this.historyBrowserBackgroundWorker = new System.ComponentModel.BackgroundWorker();
            this.borderPanel.ClientArea.SuspendLayout();
            this.borderPanel.SuspendLayout();
            this.headerStrip.SuspendLayout();
            this.splitContainer.Panel1.SuspendLayout();
            this.splitContainer.Panel2.SuspendLayout();
            this.splitContainer.SuspendLayout();
            this.SuspendLayout();
            // 
            // borderPanel
            // 
            // 
            // borderPanel.ClientArea
            // 
            this.borderPanel.ClientArea.Controls.Add(this.ribbonControl);
            this.borderPanel.ClientArea.Controls.Add(this.headerStrip);
            this.borderPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.borderPanel.Location = new System.Drawing.Point(0, 0);
            this.borderPanel.Name = "borderPanel";
            this.borderPanel.Padding = new System.Windows.Forms.Padding(1);
            this.borderPanel.Size = new System.Drawing.Size(700, 690);
            this.borderPanel.TabIndex = 2;
            // 
            // ribbonControl
            // 
            this.ribbonControl.BackColor = System.Drawing.Color.Transparent;
            this.ribbonControl.Dock = System.Windows.Forms.DockStyle.Fill;
            this.ribbonControl.Location = new System.Drawing.Point(0, 25);
            this.ribbonControl.Name = "ribbonControl";
            this.ribbonControl.SelectedTab = Idera.SQLdm.DesktopClient.Views.Servers.Server.ServerViewTabs.Overview;
            this.ribbonControl.Size = new System.Drawing.Size(700, 665);
            this.ribbonControl.TabIndex = 1;
            this.ribbonControl.SubViewChanged += new System.EventHandler<Idera.SQLdm.DesktopClient.Views.Servers.Server.SubViewChangedEventArgs>(this.ribbonControl_SubViewChanged);
            // 
            // headerStrip
            // 
            this.headerStrip.AutoSize = false;
            this.headerStrip.Font = new System.Drawing.Font("Arial", 12F, System.Drawing.FontStyle.Bold);
            this.headerStrip.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(21)))), ((int)(((byte)(66)))), ((int)(((byte)(139)))));
            this.headerStrip.GripStyle = System.Windows.Forms.ToolStripGripStyle.Hidden;
            this.headerStrip.HeaderImage = global::Idera.SQLdm.DesktopClient.Properties.Resources.Server;
            this.headerStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.titleLabel,
            this.historyTimestampLabel});
            this.headerStrip.Location = new System.Drawing.Point(0, 0);
            this.headerStrip.Name = "headerStrip";
            this.headerStrip.Padding = new System.Windows.Forms.Padding(20, 2, 0, 0);
            this.headerStrip.Size = new System.Drawing.Size(700, 25);
            this.headerStrip.TabIndex = 0;
            // 
            // titleLabel
            // 
            this.titleLabel.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(75)))), ((int)(((byte)(75)))), ((int)(((byte)(75)))));
            this.titleLabel.Name = "titleLabel";
            this.titleLabel.Size = new System.Drawing.Size(107, 20);
            this.titleLabel.Text = "Server Name";
            this.titleLabel.TextAlign = System.Drawing.ContentAlignment.BottomCenter;
            // 
            // historyTimestampLabel
            // 
            this.historyTimestampLabel.Alignment = System.Windows.Forms.ToolStripItemAlignment.Right;
            this.historyTimestampLabel.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(75)))), ((int)(((byte)(75)))), ((int)(((byte)(75)))));
            this.historyTimestampLabel.Name = "historyTimestampLabel";
            this.historyTimestampLabel.Size = new System.Drawing.Size(152, 20);
            this.historyTimestampLabel.Text = "History Timestamp";
            this.historyTimestampLabel.TextAlign = System.Drawing.ContentAlignment.BottomCenter;
            // 
            // splitContainer
            // 
            this.splitContainer.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer.FixedPanel = System.Windows.Forms.FixedPanel.Panel2;
            this.splitContainer.Location = new System.Drawing.Point(0, 0);
            this.splitContainer.Name = "splitContainer";
            // 
            // splitContainer.Panel1
            // 
            this.splitContainer.Panel1.BackColor = System.Drawing.Color.White;
            this.splitContainer.Panel1.Controls.Add(this.borderPanel);
            // 
            // splitContainer.Panel2
            // 
            this.splitContainer.Panel2.Controls.Add(this.historyBrowserControl);
            this.splitContainer.Panel2MinSize = 33;
            this.splitContainer.Size = new System.Drawing.Size(930, 690);
            this.splitContainer.SplitterDistance = 700;
            this.splitContainer.TabIndex = 3;
            this.splitContainer.SplitterMoved += new System.Windows.Forms.SplitterEventHandler(this.splitContainer_SplitterMoved);
            this.splitContainer.MouseDown += new System.Windows.Forms.MouseEventHandler(this.splitContainer_MouseDown);
            this.splitContainer.MouseUp += new System.Windows.Forms.MouseEventHandler(this.splitContainer_MouseUp);
            // 
            // historyBrowserControl
            // 
            this.historyBrowserControl.Dock = System.Windows.Forms.DockStyle.Fill;
            this.historyBrowserControl.ExpandedWidth = 150;
            this.historyBrowserControl.Location = new System.Drawing.Point(0, 0);
            this.historyBrowserControl.Minimized = false;
            this.historyBrowserControl.Name = "historyBrowserControl";
            this.historyBrowserControl.Size = new System.Drawing.Size(226, 690);
            this.historyBrowserControl.TabIndex = 0;
            this.historyBrowserControl.CloseButtonClicked += new System.EventHandler(this.historyBrowserControl_CloseButtonClicked);
            this.historyBrowserControl.MinimizedChanged += new System.EventHandler(this.historyBrowserControl_MinimizedChanged);
            this.historyBrowserControl.HistoricalSnapshotSelected += new System.EventHandler<Idera.SQLdm.DesktopClient.Controls.HistoricalSnapshotSelectedEventArgs>(this.historyBrowserControl_HistoricalSnapshotSelected);
            // 
            // historyBrowserBackgroundWorker
            // 
            this.historyBrowserBackgroundWorker.DoWork += new System.ComponentModel.DoWorkEventHandler(this.historyBrowserBackgroundWorker_DoWork);
            this.historyBrowserBackgroundWorker.RunWorkerCompleted += new System.ComponentModel.RunWorkerCompletedEventHandler(this.historyBrowserBackgroundWorker_RunWorkerCompleted);
            // 
            // ServerView
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(235)))), ((int)(((byte)(235)))), ((int)(((byte)(235)))));
            this.Controls.Add(this.splitContainer);
            this.Name = "ServerView";
            this.Size = new System.Drawing.Size(930, 690);
            this.borderPanel.ClientArea.ResumeLayout(false);
            this.borderPanel.ResumeLayout(false);
            this.headerStrip.ResumeLayout(false);
            this.headerStrip.PerformLayout();
            this.splitContainer.Panel1.ResumeLayout(false);
            this.splitContainer.Panel2.ResumeLayout(false);
            this.splitContainer.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private Infragistics.Win.Misc.UltraPanel borderPanel;
        private Idera.SQLdm.DesktopClient.Controls.HeaderStrip headerStrip;
        private System.Windows.Forms.ToolStripLabel titleLabel;
        private ServerViewRibbonControl ribbonControl;
        private System.Windows.Forms.SplitContainer splitContainer;
        private System.Windows.Forms.ToolStripLabel historyTimestampLabel;
        private Idera.SQLdm.DesktopClient.Controls.HistoryBrowserControl historyBrowserControl;
        private System.ComponentModel.BackgroundWorker historyBrowserBackgroundWorker;
    }
}
