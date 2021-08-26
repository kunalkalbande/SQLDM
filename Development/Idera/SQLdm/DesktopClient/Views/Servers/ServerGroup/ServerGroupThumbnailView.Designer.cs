namespace Idera.SQLdm.DesktopClient.Views.Servers.ServerGroup
{
    partial class ServerGroupThumbnailView
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
            // unhook event handlers to allow view to dispose
            if (disposing)
            {
                ApplicationModel.Default.ActiveInstances.Changed -= ActiveInstances_Changed;
                ApplicationController.Default.BackgroundRefreshCompleted -= BackgroundRefreshCompleted;
                if (view != null)
                {
                    if (view is Idera.SQLdm.DesktopClient.Objects.UserView)
                    {
                        ((Idera.SQLdm.DesktopClient.Objects.UserView)view).InstancesChanged -= UserViewInstances_Changed;
                    }
                    else if (view is int)
                    {
                        ApplicationModel.Default.Tags.Changed -= Tags_Changed;
                    }
                }

                var instances = ApplicationModel.Default.ActiveInstances;
                if (instances != null && instances.Count > 0)
                {
                    foreach (var id in thumbnails.Keys)
                    {
                        if (instances.Contains(id))
                        {
                            var instance = instances[id];
                            if (instance != null)
                            {
                                instance.Changed -= parentView.instance_Changed;
                            }
                        }
                    }
                }
            }

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
            Infragistics.Win.UltraWinToolbars.OptionSet optionSet1 = new Infragistics.Win.UltraWinToolbars.OptionSet("thumbnailOptionSet");
            Infragistics.Win.UltraWinToolbars.PopupMenuTool popupMenuTool1 = new Infragistics.Win.UltraWinToolbars.PopupMenuTool("instanceContextMenu");
            Infragistics.Win.UltraWinToolbars.ButtonTool buttonTool1 = new Controls.CustomControls.CustomButtonTool("openInstanceButton");
            Infragistics.Win.UltraWinToolbars.PopupMenuTool popupMenuTool2 = new Infragistics.Win.UltraWinToolbars.PopupMenuTool("thumbnailOptionsMenu");
            Infragistics.Win.UltraWinToolbars.ButtonTool buttonTool2 = new Controls.CustomControls.CustomButtonTool("refreshInstanceButton");
            Infragistics.Win.UltraWinToolbars.ButtonTool buttonTool3 = new Controls.CustomControls.CustomButtonTool("deleteInstanceButton");
            Infragistics.Win.UltraWinToolbars.ButtonTool buttonTool11 = new Controls.CustomControls.CustomButtonTool("pulseProfileButton");
            Infragistics.Win.UltraWinToolbars.ButtonTool buttonTool12 = new Controls.CustomControls.CustomButtonTool("pulseFollowButton");
            Infragistics.Win.UltraWinToolbars.ButtonTool buttonTool4 = new Controls.CustomControls.CustomButtonTool("showInstancePropertiesButton");
            Infragistics.Win.UltraWinToolbars.ButtonTool buttonTool5 = new Controls.CustomControls.CustomButtonTool("openInstanceButton");
            Infragistics.Win.UltraWinToolbars.ButtonTool buttonTool6 = new Controls.CustomControls.CustomButtonTool("refreshInstanceButton");
            Infragistics.Win.Appearance appearance1 = new Infragistics.Win.Appearance();
            Infragistics.Win.UltraWinToolbars.ButtonTool buttonTool7 = new Controls.CustomControls.CustomButtonTool("deleteInstanceButton");
            Infragistics.Win.Appearance appearance2 = new Infragistics.Win.Appearance();
            Infragistics.Win.UltraWinToolbars.ButtonTool buttonTool8 = new Controls.CustomControls.CustomButtonTool("showInstancePropertiesButton");
            Infragistics.Win.Appearance appearance3 = new Infragistics.Win.Appearance();
            Infragistics.Win.UltraWinToolbars.PopupMenuTool popupMenuTool3 = new Infragistics.Win.UltraWinToolbars.PopupMenuTool("thumbnailOptionsMenu");
            Infragistics.Win.UltraWinToolbars.StateButtonTool stateButtonTool1 = new Infragistics.Win.UltraWinToolbars.StateButtonTool("showThumbnailSummaryButton", "thumbnailOptionSet");
            Infragistics.Win.UltraWinToolbars.StateButtonTool stateButtonTool2 = new Infragistics.Win.UltraWinToolbars.StateButtonTool("showThumbnailResponseTimeButton", "thumbnailOptionSet");
            Infragistics.Win.UltraWinToolbars.StateButtonTool stateButtonTool3 = new Infragistics.Win.UltraWinToolbars.StateButtonTool("showThumbnailUserSessionsButton", "thumbnailOptionSet");
            Infragistics.Win.UltraWinToolbars.StateButtonTool stateButtonTool4 = new Infragistics.Win.UltraWinToolbars.StateButtonTool("showThumbnailCpuUsageButton", "thumbnailOptionSet");
            Infragistics.Win.UltraWinToolbars.StateButtonTool stateButtonTool5 = new Infragistics.Win.UltraWinToolbars.StateButtonTool("showThumbnailMemoryUsageButton", "thumbnailOptionSet");
            Infragistics.Win.UltraWinToolbars.StateButtonTool stateButtonTool6 = new Infragistics.Win.UltraWinToolbars.StateButtonTool("showThumbnailSqlReadsWritesButton", "thumbnailOptionSet");
            Infragistics.Win.UltraWinToolbars.StateButtonTool stateButtonTool7 = new Infragistics.Win.UltraWinToolbars.StateButtonTool("showThumbnailSummaryButton", "thumbnailOptionSet");
            Infragistics.Win.UltraWinToolbars.StateButtonTool stateButtonTool8 = new Infragistics.Win.UltraWinToolbars.StateButtonTool("showThumbnailResponseTimeButton", "thumbnailOptionSet");
            Infragistics.Win.UltraWinToolbars.StateButtonTool stateButtonTool9 = new Infragistics.Win.UltraWinToolbars.StateButtonTool("showThumbnailUserSessionsButton", "thumbnailOptionSet");
            Infragistics.Win.UltraWinToolbars.StateButtonTool stateButtonTool10 = new Infragistics.Win.UltraWinToolbars.StateButtonTool("showThumbnailCpuUsageButton", "thumbnailOptionSet");
            Infragistics.Win.UltraWinToolbars.StateButtonTool stateButtonTool11 = new Infragistics.Win.UltraWinToolbars.StateButtonTool("showThumbnailMemoryUsageButton", "thumbnailOptionSet");
            Infragistics.Win.UltraWinToolbars.StateButtonTool stateButtonTool12 = new Infragistics.Win.UltraWinToolbars.StateButtonTool("showThumbnailSqlReadsWritesButton", "thumbnailOptionSet");
            Infragistics.Win.UltraWinToolbars.ButtonTool buttonTool9 = new Controls.CustomControls.CustomButtonTool("pulseProfileButton");
            Infragistics.Win.UltraWinToolbars.ButtonTool buttonTool10 = new Controls.CustomControls.CustomButtonTool("pulseFollowButton");
            this.flowLayoutPanel = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomFlowLayoutPanel();
            this.ServerGroupThumbnailView_Fill_Panel = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomPanel ();
            this.shortcutPanel = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomPanel ();
            this.label1 = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomLabel();
            this.zButton = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomUltraButton();
            this.yButton = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomUltraButton();
            this.xButton = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomUltraButton();
            this.wButton = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomUltraButton();
            this.vButton = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomUltraButton();
            this.uButton = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomUltraButton();
            this.tButton = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomUltraButton();
            this.sButton = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomUltraButton();
            this.rButton = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomUltraButton();
            this.qButton = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomUltraButton();
            this.pButton = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomUltraButton();
            this.oButton = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomUltraButton();
            this.nButton = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomUltraButton();
            this.mButton = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomUltraButton();
            this.lButton = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomUltraButton();
            this.kButton = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomUltraButton();
            this.jButton = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomUltraButton();
            this.iButton = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomUltraButton();
            this.hButton = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomUltraButton();
            this.gButton = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomUltraButton();
            this.fButton = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomUltraButton();
            this.eButton = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomUltraButton();
            this.dButton = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomUltraButton();
            this.cButton = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomUltraButton();
            this.bButton = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomUltraButton();
            this.aButton = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomUltraButton();
            this.numericButton = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomUltraButton();
            this.viewStatusLabel = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomLabel();
            this.toolTipManager = new Infragistics.Win.UltraWinToolTip.UltraToolTipManager(this.components);
            this.toolbarsManager = new Idera.SQLdm.DesktopClient.Controls.ContextMenuManager(this.components);
            this.ServerGroupThumbnailView_Fill_Panel.SuspendLayout();
            this.shortcutPanel.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.toolbarsManager)).BeginInit();
            this.SuspendLayout();
            // 
            // flowLayoutPanel
            // 
            this.flowLayoutPanel.AutoScroll = true;
            this.flowLayoutPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.flowLayoutPanel.Location = new System.Drawing.Point(0, 0);
            this.flowLayoutPanel.Name = "flowLayoutPanel";
            this.flowLayoutPanel.Size = new System.Drawing.Size(622, 553);
            this.flowLayoutPanel.TabIndex = 0;
            this.flowLayoutPanel.Visible = false;
            this.flowLayoutPanel.MouseDown += new System.Windows.Forms.MouseEventHandler(this.flowLayoutPanel_MouseDown);
            // 
            // ServerGroupThumbnailView_Fill_Panel
            // 
            this.ServerGroupThumbnailView_Fill_Panel.AutoScroll = true;
            this.ServerGroupThumbnailView_Fill_Panel.Controls.Add(this.flowLayoutPanel);
            this.ServerGroupThumbnailView_Fill_Panel.Controls.Add(this.shortcutPanel);
            this.ServerGroupThumbnailView_Fill_Panel.Controls.Add(this.viewStatusLabel);
            this.ServerGroupThumbnailView_Fill_Panel.Cursor = System.Windows.Forms.Cursors.Default;
            this.ServerGroupThumbnailView_Fill_Panel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.ServerGroupThumbnailView_Fill_Panel.Location = new System.Drawing.Point(0, 0);
            this.ServerGroupThumbnailView_Fill_Panel.Name = "ServerGroupThumbnailView_Fill_Panel";
            this.ServerGroupThumbnailView_Fill_Panel.Size = new System.Drawing.Size(662, 553);
            this.ServerGroupThumbnailView_Fill_Panel.TabIndex = 0;
            // 
            // shortcutPanel
            // 
            this.shortcutPanel.Controls.Add(this.label1);
            this.shortcutPanel.Controls.Add(this.zButton);
            this.shortcutPanel.Controls.Add(this.yButton);
            this.shortcutPanel.Controls.Add(this.xButton);
            this.shortcutPanel.Controls.Add(this.wButton);
            this.shortcutPanel.Controls.Add(this.vButton);
            this.shortcutPanel.Controls.Add(this.uButton);
            this.shortcutPanel.Controls.Add(this.tButton);
            this.shortcutPanel.Controls.Add(this.sButton);
            this.shortcutPanel.Controls.Add(this.rButton);
            this.shortcutPanel.Controls.Add(this.qButton);
            this.shortcutPanel.Controls.Add(this.pButton);
            this.shortcutPanel.Controls.Add(this.oButton);
            this.shortcutPanel.Controls.Add(this.nButton);
            this.shortcutPanel.Controls.Add(this.mButton);
            this.shortcutPanel.Controls.Add(this.lButton);
            this.shortcutPanel.Controls.Add(this.kButton);
            this.shortcutPanel.Controls.Add(this.jButton);
            this.shortcutPanel.Controls.Add(this.iButton);
            this.shortcutPanel.Controls.Add(this.hButton);
            this.shortcutPanel.Controls.Add(this.gButton);
            this.shortcutPanel.Controls.Add(this.fButton);
            this.shortcutPanel.Controls.Add(this.eButton);
            this.shortcutPanel.Controls.Add(this.dButton);
            this.shortcutPanel.Controls.Add(this.cButton);
            this.shortcutPanel.Controls.Add(this.bButton);
            this.shortcutPanel.Controls.Add(this.aButton);
            this.shortcutPanel.Controls.Add(this.numericButton);
            this.shortcutPanel.Dock = System.Windows.Forms.DockStyle.Right;
            this.shortcutPanel.Location = new System.Drawing.Point(622, 0);
            this.shortcutPanel.Name = "shortcutPanel";
            this.shortcutPanel.Size = new System.Drawing.Size(40, 553);
            this.shortcutPanel.TabIndex = 1;
            this.shortcutPanel.Visible = false;
            // 
            // label1
            // 
            this.label1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(140)))), ((int)(((byte)(140)))), ((int)(((byte)(140)))));
            this.label1.Dock = System.Windows.Forms.DockStyle.Left;
            this.label1.Location = new System.Drawing.Point(0, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(1, 553);
            this.label1.TabIndex = 27;
            // 
            // zButton
            // 
            this.zButton.Location = new System.Drawing.Point(6, 523);
            this.zButton.Name = "zButton";
            this.zButton.ShowFocusRect = false;
            this.zButton.ShowOutline = false;
            this.zButton.Size = new System.Drawing.Size(29, 18);
            this.zButton.TabIndex = 26;
            this.zButton.Text = "z";
            this.zButton.Click += new System.EventHandler(this.findLetterButton_Click);
            // 
            // yButton
            // 
            this.yButton.Location = new System.Drawing.Point(6, 503);
            this.yButton.Name = "yButton";
            this.yButton.ShowFocusRect = false;
            this.yButton.ShowOutline = false;
            this.yButton.Size = new System.Drawing.Size(29, 18);
            this.yButton.TabIndex = 25;
            this.yButton.Text = "y";
            this.yButton.Click += new System.EventHandler(this.findLetterButton_Click);
            // 
            // xButton
            // 
            this.xButton.Location = new System.Drawing.Point(6, 483);
            this.xButton.Name = "xButton";
            this.xButton.ShowFocusRect = false;
            this.xButton.ShowOutline = false;
            this.xButton.Size = new System.Drawing.Size(29, 18);
            this.xButton.TabIndex = 24;
            this.xButton.Text = "x";
            this.xButton.Click += new System.EventHandler(this.findLetterButton_Click);
            // 
            // wButton
            // 
            this.wButton.Location = new System.Drawing.Point(6, 463);
            this.wButton.Name = "wButton";
            this.wButton.ShowFocusRect = false;
            this.wButton.ShowOutline = false;
            this.wButton.Size = new System.Drawing.Size(29, 18);
            this.wButton.TabIndex = 23;
            this.wButton.Text = "w";
            this.wButton.Click += new System.EventHandler(this.findLetterButton_Click);
            // 
            // vButton
            // 
            this.vButton.Location = new System.Drawing.Point(6, 443);
            this.vButton.Name = "vButton";
            this.vButton.ShowFocusRect = false;
            this.vButton.ShowOutline = false;
            this.vButton.Size = new System.Drawing.Size(29, 18);
            this.vButton.TabIndex = 22;
            this.vButton.Text = "v";
            this.vButton.Click += new System.EventHandler(this.findLetterButton_Click);
            // 
            // uButton
            // 
            this.uButton.Location = new System.Drawing.Point(6, 423);
            this.uButton.Name = "uButton";
            this.uButton.ShowFocusRect = false;
            this.uButton.ShowOutline = false;
            this.uButton.Size = new System.Drawing.Size(29, 18);
            this.uButton.TabIndex = 21;
            this.uButton.Text = "u";
            this.uButton.Click += new System.EventHandler(this.findLetterButton_Click);
            // 
            // tButton
            // 
            this.tButton.Location = new System.Drawing.Point(6, 403);
            this.tButton.Name = "tButton";
            this.tButton.ShowFocusRect = false;
            this.tButton.ShowOutline = false;
            this.tButton.Size = new System.Drawing.Size(29, 18);
            this.tButton.TabIndex = 20;
            this.tButton.Text = "t";
            this.tButton.Click += new System.EventHandler(this.findLetterButton_Click);
            // 
            // sButton
            // 
            this.sButton.Location = new System.Drawing.Point(6, 383);
            this.sButton.Name = "sButton";
            this.sButton.ShowFocusRect = false;
            this.sButton.ShowOutline = false;
            this.sButton.Size = new System.Drawing.Size(29, 18);
            this.sButton.TabIndex = 19;
            this.sButton.Text = "s";
            this.sButton.Click += new System.EventHandler(this.findLetterButton_Click);
            // 
            // rButton
            // 
            this.rButton.Location = new System.Drawing.Point(6, 363);
            this.rButton.Name = "rButton";
            this.rButton.ShowFocusRect = false;
            this.rButton.ShowOutline = false;
            this.rButton.Size = new System.Drawing.Size(29, 18);
            this.rButton.TabIndex = 18;
            this.rButton.Text = "r";
            this.rButton.Click += new System.EventHandler(this.findLetterButton_Click);
            // 
            // qButton
            // 
            this.qButton.Location = new System.Drawing.Point(6, 343);
            this.qButton.Name = "qButton";
            this.qButton.ShowFocusRect = false;
            this.qButton.ShowOutline = false;
            this.qButton.Size = new System.Drawing.Size(29, 18);
            this.qButton.TabIndex = 17;
            this.qButton.Text = "q";
            this.qButton.Click += new System.EventHandler(this.findLetterButton_Click);
            // 
            // pButton
            // 
            this.pButton.Location = new System.Drawing.Point(6, 323);
            this.pButton.Name = "pButton";
            this.pButton.ShowFocusRect = false;
            this.pButton.ShowOutline = false;
            this.pButton.Size = new System.Drawing.Size(29, 18);
            this.pButton.TabIndex = 16;
            this.pButton.Text = "p";
            this.pButton.Click += new System.EventHandler(this.findLetterButton_Click);
            // 
            // oButton
            // 
            this.oButton.Location = new System.Drawing.Point(6, 303);
            this.oButton.Name = "oButton";
            this.oButton.ShowFocusRect = false;
            this.oButton.ShowOutline = false;
            this.oButton.Size = new System.Drawing.Size(29, 18);
            this.oButton.TabIndex = 15;
            this.oButton.Text = "o";
            this.oButton.Click += new System.EventHandler(this.findLetterButton_Click);
            // 
            // nButton
            // 
            this.nButton.Location = new System.Drawing.Point(6, 283);
            this.nButton.Name = "nButton";
            this.nButton.ShowFocusRect = false;
            this.nButton.ShowOutline = false;
            this.nButton.Size = new System.Drawing.Size(29, 18);
            this.nButton.TabIndex = 14;
            this.nButton.Text = "n";
            this.nButton.Click += new System.EventHandler(this.findLetterButton_Click);
            // 
            // mButton
            // 
            this.mButton.Location = new System.Drawing.Point(6, 263);
            this.mButton.Name = "mButton";
            this.mButton.ShowFocusRect = false;
            this.mButton.ShowOutline = false;
            this.mButton.Size = new System.Drawing.Size(29, 18);
            this.mButton.TabIndex = 13;
            this.mButton.Text = "m";
            this.mButton.Click += new System.EventHandler(this.findLetterButton_Click);
            // 
            // lButton
            // 
            this.lButton.Location = new System.Drawing.Point(6, 243);
            this.lButton.Name = "lButton";
            this.lButton.ShowFocusRect = false;
            this.lButton.ShowOutline = false;
            this.lButton.Size = new System.Drawing.Size(29, 18);
            this.lButton.TabIndex = 12;
            this.lButton.Text = "l";
            this.lButton.Click += new System.EventHandler(this.findLetterButton_Click);
            // 
            // kButton
            // 
            this.kButton.Location = new System.Drawing.Point(6, 223);
            this.kButton.Name = "kButton";
            this.kButton.ShowFocusRect = false;
            this.kButton.ShowOutline = false;
            this.kButton.Size = new System.Drawing.Size(29, 18);
            this.kButton.TabIndex = 11;
            this.kButton.Text = "k";
            this.kButton.Click += new System.EventHandler(this.findLetterButton_Click);
            // 
            // jButton
            // 
            this.jButton.Location = new System.Drawing.Point(6, 203);
            this.jButton.Name = "jButton";
            this.jButton.ShowFocusRect = false;
            this.jButton.ShowOutline = false;
            this.jButton.Size = new System.Drawing.Size(29, 18);
            this.jButton.TabIndex = 10;
            this.jButton.Text = "j";
            this.jButton.Click += new System.EventHandler(this.findLetterButton_Click);
            // 
            // iButton
            // 
            this.iButton.Location = new System.Drawing.Point(6, 183);
            this.iButton.Name = "iButton";
            this.iButton.ShowFocusRect = false;
            this.iButton.ShowOutline = false;
            this.iButton.Size = new System.Drawing.Size(29, 18);
            this.iButton.TabIndex = 9;
            this.iButton.Text = "i";
            this.iButton.Click += new System.EventHandler(this.findLetterButton_Click);
            // 
            // hButton
            // 
            this.hButton.Location = new System.Drawing.Point(6, 163);
            this.hButton.Name = "hButton";
            this.hButton.ShowFocusRect = false;
            this.hButton.ShowOutline = false;
            this.hButton.Size = new System.Drawing.Size(29, 18);
            this.hButton.TabIndex = 8;
            this.hButton.Text = "h";
            this.hButton.Click += new System.EventHandler(this.findLetterButton_Click);
            // 
            // gButton
            // 
            this.gButton.Location = new System.Drawing.Point(6, 143);
            this.gButton.Name = "gButton";
            this.gButton.ShowFocusRect = false;
            this.gButton.ShowOutline = false;
            this.gButton.Size = new System.Drawing.Size(29, 18);
            this.gButton.TabIndex = 7;
            this.gButton.Text = "g";
            this.gButton.Click += new System.EventHandler(this.findLetterButton_Click);
            // 
            // fButton
            // 
            this.fButton.Location = new System.Drawing.Point(6, 123);
            this.fButton.Name = "fButton";
            this.fButton.ShowFocusRect = false;
            this.fButton.ShowOutline = false;
            this.fButton.Size = new System.Drawing.Size(29, 18);
            this.fButton.TabIndex = 6;
            this.fButton.Text = "f";
            this.fButton.Click += new System.EventHandler(this.findLetterButton_Click);
            // 
            // eButton
            // 
            this.eButton.Location = new System.Drawing.Point(6, 103);
            this.eButton.Name = "eButton";
            this.eButton.ShowFocusRect = false;
            this.eButton.ShowOutline = false;
            this.eButton.Size = new System.Drawing.Size(29, 18);
            this.eButton.TabIndex = 5;
            this.eButton.Text = "e";
            this.eButton.Click += new System.EventHandler(this.findLetterButton_Click);
            // 
            // dButton
            // 
            this.dButton.Location = new System.Drawing.Point(6, 83);
            this.dButton.Name = "dButton";
            this.dButton.ShowFocusRect = false;
            this.dButton.ShowOutline = false;
            this.dButton.Size = new System.Drawing.Size(29, 18);
            this.dButton.TabIndex = 4;
            this.dButton.Text = "d";
            this.dButton.Click += new System.EventHandler(this.findLetterButton_Click);
            // 
            // cButton
            // 
            this.cButton.Location = new System.Drawing.Point(6, 63);
            this.cButton.Name = "cButton";
            this.cButton.ShowFocusRect = false;
            this.cButton.ShowOutline = false;
            this.cButton.Size = new System.Drawing.Size(29, 18);
            this.cButton.TabIndex = 3;
            this.cButton.Text = "c";
            this.cButton.Click += new System.EventHandler(this.findLetterButton_Click);
            // 
            // bButton
            // 
            this.bButton.Location = new System.Drawing.Point(6, 43);
            this.bButton.Name = "bButton";
            this.bButton.ShowFocusRect = false;
            this.bButton.ShowOutline = false;
            this.bButton.Size = new System.Drawing.Size(29, 18);
            this.bButton.TabIndex = 2;
            this.bButton.Text = "b";
            this.bButton.Click += new System.EventHandler(this.findLetterButton_Click);
            // 
            // aButton
            // 
            this.aButton.Location = new System.Drawing.Point(6, 23);
            this.aButton.Name = "aButton";
            this.aButton.ShowFocusRect = false;
            this.aButton.ShowOutline = false;
            this.aButton.Size = new System.Drawing.Size(29, 18);
            this.aButton.TabIndex = 1;
            this.aButton.Text = "a";
            this.aButton.Click += new System.EventHandler(this.findLetterButton_Click);
            // 
            // numericButton
            // 
            this.numericButton.Location = new System.Drawing.Point(6, 3);
            this.numericButton.Name = "numericButton";
            this.numericButton.ShowFocusRect = false;
            this.numericButton.ShowOutline = false;
            this.numericButton.Size = new System.Drawing.Size(29, 18);
            this.numericButton.TabIndex = 0;
            this.numericButton.Text = "123";
            this.numericButton.Click += new System.EventHandler(this.findNumericButton_Click);
            // 
            // viewStatusLabel
            // 
            this.viewStatusLabel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.viewStatusLabel.Location = new System.Drawing.Point(0, 0);
            this.viewStatusLabel.Name = "viewStatusLabel";
            this.viewStatusLabel.Padding = new System.Windows.Forms.Padding(0, 5, 0, 0);
            this.viewStatusLabel.Size = new System.Drawing.Size(662, 553);
            this.viewStatusLabel.TabIndex = 2;
            this.viewStatusLabel.Text = "< status label >";
            this.viewStatusLabel.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            // 
            // toolTipManager
            // 
            this.toolTipManager.AutoPopDelay = 0;
            this.toolTipManager.ContainingControl = this;
            this.toolTipManager.DisplayStyle = Infragistics.Win.ToolTipDisplayStyle.Office2007;
            // 
            // toolbarsManager
            // 
            this.toolbarsManager.DesignerFlags = 1;
            this.toolbarsManager.OptionSets.Add(optionSet1);
            this.toolbarsManager.ShowFullMenusDelay = 500;
            popupMenuTool1.SharedPropsInternal.Caption = "instanceContextMenu";
            popupMenuTool2.InstanceProps.IsFirstInGroup = true;
            buttonTool2.InstanceProps.IsFirstInGroup = true;
            buttonTool3.InstanceProps.IsFirstInGroup = true;
            buttonTool11.InstanceProps.IsFirstInGroup = true;
            buttonTool4.InstanceProps.IsFirstInGroup = true;
            popupMenuTool1.Tools.AddRange(new Infragistics.Win.UltraWinToolbars.ToolBase[] {
            buttonTool1,
            popupMenuTool2,
            buttonTool2,
            buttonTool3,
            buttonTool11,
            buttonTool12,
            buttonTool4});
            buttonTool5.SharedPropsInternal.Caption = "Open";
            appearance1.Image = global::Idera.SQLdm.DesktopClient.Properties.Resources.ToolbarRefresh;
            buttonTool6.SharedPropsInternal.AppearancesSmall.Appearance = appearance1;
            buttonTool6.SharedPropsInternal.Caption = "Refresh Alerts";
            appearance2.Image = global::Idera.SQLdm.DesktopClient.Properties.Resources.Delete;
            buttonTool7.SharedPropsInternal.AppearancesSmall.Appearance = appearance2;
            buttonTool7.SharedPropsInternal.Caption = "Delete";
            buttonTool7.SharedPropsInternal.Shortcut = System.Windows.Forms.Shortcut.Del;
            appearance3.Image = global::Idera.SQLdm.DesktopClient.Properties.Resources.Properties;
            buttonTool8.SharedPropsInternal.AppearancesSmall.Appearance = appearance3;
            buttonTool8.SharedPropsInternal.Caption = "Properties...";
            popupMenuTool3.SharedPropsInternal.Caption = "Thumbnail";
            stateButtonTool1.Checked = true;
            stateButtonTool1.MenuDisplayStyle = Infragistics.Win.UltraWinToolbars.StateButtonMenuDisplayStyle.DisplayCheckmark;
            stateButtonTool2.MenuDisplayStyle = Infragistics.Win.UltraWinToolbars.StateButtonMenuDisplayStyle.DisplayCheckmark;
            stateButtonTool3.MenuDisplayStyle = Infragistics.Win.UltraWinToolbars.StateButtonMenuDisplayStyle.DisplayCheckmark;
            stateButtonTool4.MenuDisplayStyle = Infragistics.Win.UltraWinToolbars.StateButtonMenuDisplayStyle.DisplayCheckmark;
            stateButtonTool5.MenuDisplayStyle = Infragistics.Win.UltraWinToolbars.StateButtonMenuDisplayStyle.DisplayCheckmark;
            stateButtonTool6.MenuDisplayStyle = Infragistics.Win.UltraWinToolbars.StateButtonMenuDisplayStyle.DisplayCheckmark;
            popupMenuTool3.Tools.AddRange(new Infragistics.Win.UltraWinToolbars.ToolBase[] {
            stateButtonTool1,
            stateButtonTool2,
            stateButtonTool3,
            stateButtonTool4,
            stateButtonTool5,
            stateButtonTool6});
            stateButtonTool7.Checked = true;
            stateButtonTool7.MenuDisplayStyle = Infragistics.Win.UltraWinToolbars.StateButtonMenuDisplayStyle.DisplayCheckmark;
            stateButtonTool7.OptionSetKey = "thumbnailOptionSet";
            stateButtonTool7.SharedPropsInternal.Caption = "Summary";
            stateButtonTool8.MenuDisplayStyle = Infragistics.Win.UltraWinToolbars.StateButtonMenuDisplayStyle.DisplayCheckmark;
            stateButtonTool8.OptionSetKey = "thumbnailOptionSet";
            stateButtonTool8.SharedPropsInternal.Caption = "Response Time";
            stateButtonTool9.MenuDisplayStyle = Infragistics.Win.UltraWinToolbars.StateButtonMenuDisplayStyle.DisplayCheckmark;
            stateButtonTool9.OptionSetKey = "thumbnailOptionSet";
            stateButtonTool9.SharedPropsInternal.Caption = "User Sessions";
            stateButtonTool10.MenuDisplayStyle = Infragistics.Win.UltraWinToolbars.StateButtonMenuDisplayStyle.DisplayCheckmark;
            stateButtonTool10.OptionSetKey = "thumbnailOptionSet";
            stateButtonTool10.SharedPropsInternal.Caption = "SQL CPU Usage";
            stateButtonTool11.MenuDisplayStyle = Infragistics.Win.UltraWinToolbars.StateButtonMenuDisplayStyle.DisplayCheckmark;
            stateButtonTool11.OptionSetKey = "thumbnailOptionSet";
            stateButtonTool11.SharedPropsInternal.Caption = "SQL Memory Usage";
            stateButtonTool12.MenuDisplayStyle = Infragistics.Win.UltraWinToolbars.StateButtonMenuDisplayStyle.DisplayCheckmark;
            stateButtonTool12.OptionSetKey = "thumbnailOptionSet";
            stateButtonTool12.SharedPropsInternal.Caption = "SQL Disk I/O";
            buttonTool9.SharedPropsInternal.Caption = "View Profile in Newsfeed";
            buttonTool10.SharedPropsInternal.Caption = "Follow Updates in Newsfeed";
            this.toolbarsManager.Tools.AddRange(new Infragistics.Win.UltraWinToolbars.ToolBase[] {
            popupMenuTool1,
            buttonTool5,
            buttonTool6,
            buttonTool7,
            buttonTool8,
            popupMenuTool3,
            stateButtonTool7,
            stateButtonTool8,
            stateButtonTool9,
            stateButtonTool10,
            stateButtonTool11,
            stateButtonTool12,
            buttonTool9,
            buttonTool10});
            this.toolbarsManager.BeforeToolDropdown += new Infragistics.Win.UltraWinToolbars.BeforeToolDropdownEventHandler(this.toolbarsManager_BeforeToolDropdown);
            this.toolbarsManager.ToolClick += new Infragistics.Win.UltraWinToolbars.ToolClickEventHandler(this.toolbarsManager_ToolClick);
            // 
            // ServerGroupThumbnailView
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.AutoScroll = true;
            this.BackColor = System.Drawing.Color.White;
            this.Controls.Add(this.ServerGroupThumbnailView_Fill_Panel);
            this.Name = "ServerGroupThumbnailView";
            this.Size = new System.Drawing.Size(662, 553);
            this.ServerGroupThumbnailView_Fill_Panel.ResumeLayout(false);
            this.shortcutPanel.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.toolbarsManager)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomFlowLayoutPanel flowLayoutPanel;
        private Idera.SQLdm.DesktopClient.Controls.ContextMenuManager toolbarsManager;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomPanel  ServerGroupThumbnailView_Fill_Panel;
        private Infragistics.Win.UltraWinToolTip.UltraToolTipManager toolTipManager;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomPanel  shortcutPanel;
        private Infragistics.Win.Misc.UltraButton numericButton;
        private Infragistics.Win.Misc.UltraButton bButton;
        private Infragistics.Win.Misc.UltraButton aButton;
        private Infragistics.Win.Misc.UltraButton sButton;
        private Infragistics.Win.Misc.UltraButton rButton;
        private Infragistics.Win.Misc.UltraButton qButton;
        private Infragistics.Win.Misc.UltraButton pButton;
        private Infragistics.Win.Misc.UltraButton oButton;
        private Infragistics.Win.Misc.UltraButton nButton;
        private Infragistics.Win.Misc.UltraButton mButton;
        private Infragistics.Win.Misc.UltraButton lButton;
        private Infragistics.Win.Misc.UltraButton kButton;
        private Infragistics.Win.Misc.UltraButton jButton;
        private Infragistics.Win.Misc.UltraButton iButton;
        private Infragistics.Win.Misc.UltraButton hButton;
        private Infragistics.Win.Misc.UltraButton gButton;
        private Infragistics.Win.Misc.UltraButton fButton;
        private Infragistics.Win.Misc.UltraButton eButton;
        private Infragistics.Win.Misc.UltraButton dButton;
        private Infragistics.Win.Misc.UltraButton cButton;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomLabel label1;
        private Infragistics.Win.Misc.UltraButton zButton;
        private Infragistics.Win.Misc.UltraButton yButton;
        private Infragistics.Win.Misc.UltraButton xButton;
        private Infragistics.Win.Misc.UltraButton wButton;
        private Infragistics.Win.Misc.UltraButton vButton;
        private Infragistics.Win.Misc.UltraButton uButton;
        private Infragistics.Win.Misc.UltraButton tButton;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomLabel viewStatusLabel;


    }
}
