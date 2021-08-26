using Idera.SQLdm.DesktopClient.Properties;
using Idera.SQLdm.DesktopClient.Objects;
using Infragistics.Win.UltraWinToolbars;
using System.Drawing.Text;
using Idera.SQLdm.DesktopClient.Controls.Renderers;
using Idera.SQLdm.DesktopClient.Helpers;
using System;

namespace Idera.SQLdm.DesktopClient.Controls.NavigationPane
{
    partial class ServersNavigationPane
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
                if (userViewTreeViewToolTip != null) userViewTreeViewToolTip.Dispose();

                ApplicationModel.Default.ActiveInstances.Changed -= ActiveInstances_Changed;
                ApplicationModel.Default.Tags.Changed -= Tags_Changed;
                ApplicationModel.Default.FocusObjectChanged -= ApplicationModel_FocusObjectChanged;
                ApplicationController.Default.ActiveViewChanging -= ApplicationController_ActiveViewChanging;
                ApplicationController.Default.ActiveViewChanged -= ApplicationController_ActiveViewChanged;
                ApplicationController.Default.BackgroundRefreshCompleted -= BackgroundRefreshCompleted;
                Settings.Default.ActiveRepositoryConnectionChanging -= Settings_ActiveRepositoryConnectionChanging;
                Settings.Default.ActiveRepositoryConnectionChanged -= Settings_ActiveRepositoryConnectionChanged;

                var instances = ApplicationModel.Default.ActiveInstances;
                if (instances != null && instances.Count > 0)
                {
                    foreach (MonitoredSqlServerWrapper wrapper in instances)
                    {
                        wrapper.Changed -= instance_Changed;
                    }
                }

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
            this.components = new System.ComponentModel.Container();
            Infragistics.Win.UltraWinToolbars.ButtonTool buttonTool1 = new Infragistics.Win.UltraWinToolbars.ButtonTool("refreshButton");
            Infragistics.Win.Appearance appearance1 = new Infragistics.Win.Appearance();
            Infragistics.Win.UltraWinToolbars.PopupMenuTool popupMenuTool1 = new Infragistics.Win.UltraWinToolbars.PopupMenuTool("serverPopupMenu");
            Infragistics.Win.UltraWinToolbars.ButtonTool buttonTool2 = new Infragistics.Win.UltraWinToolbars.ButtonTool("openButton");
            Infragistics.Win.UltraWinToolbars.ButtonTool buttonTool34 = new Infragistics.Win.UltraWinToolbars.ButtonTool("refreshAlertsButton");
            Infragistics.Win.UltraWinToolbars.ButtonTool buttonTool4 = new Infragistics.Win.UltraWinToolbars.ButtonTool("deleteButton");
            Infragistics.Win.Appearance appearance2 = new Infragistics.Win.Appearance();
            Infragistics.Win.UltraWinToolbars.ButtonTool buttonTool5 = new Infragistics.Win.UltraWinToolbars.ButtonTool("removeInstanceFromViewButton");
            Infragistics.Win.UltraWinToolbars.StateButtonTool stateButtonTool1 = new Infragistics.Win.UltraWinToolbars.StateButtonTool("maintenanceModeStateButton", "");
            Infragistics.Win.UltraWinToolbars.ButtonTool buttonTool30 = new Infragistics.Win.UltraWinToolbars.ButtonTool("changeMaintenanceModeButton");
            Infragistics.Win.UltraWinToolbars.ButtonTool buttonTool33 = new Infragistics.Win.UltraWinToolbars.ButtonTool("configureBaselineButton");
            Infragistics.Win.UltraWinToolbars.ButtonTool buttonTool56 = new Infragistics.Win.UltraWinToolbars.ButtonTool("applyAlertTemplateButton");
            Infragistics.Win.UltraWinToolbars.ButtonTool buttonTool6 = new Infragistics.Win.UltraWinToolbars.ButtonTool("configureAlertsButton");
            Infragistics.Win.UltraWinToolbars.ButtonTool buttonTool36 = new Infragistics.Win.UltraWinToolbars.ButtonTool("snoozeAlertsButton");
            Infragistics.Win.UltraWinToolbars.ButtonTool buttonTool37 = new Infragistics.Win.UltraWinToolbars.ButtonTool("unsnoozeAlertsButton");
            Infragistics.Win.UltraWinToolbars.ButtonTool buttonTool53 = new Infragistics.Win.UltraWinToolbars.ButtonTool("pulseProfileButton");
            Infragistics.Win.UltraWinToolbars.ButtonTool buttonTool55 = new Infragistics.Win.UltraWinToolbars.ButtonTool("pulseFollowButton");
            Infragistics.Win.UltraWinToolbars.ButtonTool buttonTool7 = new Infragistics.Win.UltraWinToolbars.ButtonTool("propertiesButton");
            Infragistics.Win.UltraWinToolbars.ButtonTool buttonTool8 = new Infragistics.Win.UltraWinToolbars.ButtonTool("addServersButton");
            Infragistics.Win.UltraWinToolbars.ButtonTool buttonTool9 = new Infragistics.Win.UltraWinToolbars.ButtonTool("deleteButton");
            Infragistics.Win.Appearance appearance3 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance4 = new Infragistics.Win.Appearance();
            Infragistics.Win.UltraWinToolbars.ButtonTool buttonTool10 = new Infragistics.Win.UltraWinToolbars.ButtonTool("propertiesButton");
            Infragistics.Win.Appearance appearance5 = new Infragistics.Win.Appearance();
            Infragistics.Win.UltraWinToolbars.PopupMenuTool popupMenuTool2 = new Infragistics.Win.UltraWinToolbars.PopupMenuTool("userViewPopupMenu");
            Infragistics.Win.UltraWinToolbars.ButtonTool buttonTool11 = new Infragistics.Win.UltraWinToolbars.ButtonTool("openButton");
            Infragistics.Win.UltraWinToolbars.ButtonTool buttonTool60 = new Infragistics.Win.UltraWinToolbars.ButtonTool("viewAllServerProperties");
            Infragistics.Win.UltraWinToolbars.ButtonTool buttonTool66 = new Infragistics.Win.UltraWinToolbars.ButtonTool("snoozeAllAlertsServerButton");
            Infragistics.Win.UltraWinToolbars.ButtonTool buttonTool68 = new Infragistics.Win.UltraWinToolbars.ButtonTool("resumeAllAlertsServerButton");
            Infragistics.Win.UltraWinToolbars.ButtonTool buttonTool12 = new Infragistics.Win.UltraWinToolbars.ButtonTool("refreshButton");
            Infragistics.Win.UltraWinToolbars.ButtonTool buttonTool13 = new Infragistics.Win.UltraWinToolbars.ButtonTool("addUserViewButton");
            Infragistics.Win.UltraWinToolbars.ButtonTool buttonTool14 = new Infragistics.Win.UltraWinToolbars.ButtonTool("manageServersButton");
            Infragistics.Win.UltraWinToolbars.ButtonTool buttonTool15 = new Infragistics.Win.UltraWinToolbars.ButtonTool("deleteButton");
            Infragistics.Win.UltraWinToolbars.ButtonTool buttonTool16 = new Infragistics.Win.UltraWinToolbars.ButtonTool("renameUserViewButton");
            Infragistics.Win.UltraWinToolbars.ButtonTool buttonTool17 = new Infragistics.Win.UltraWinToolbars.ButtonTool("moveUserViewUpInListButton");
            Infragistics.Win.UltraWinToolbars.ButtonTool buttonTool18 = new Infragistics.Win.UltraWinToolbars.ButtonTool("moveUserViewDownInListButton");
            Infragistics.Win.UltraWinToolbars.ButtonTool buttonTool19 = new Infragistics.Win.UltraWinToolbars.ButtonTool("propertiesButton");
            Infragistics.Win.UltraWinToolbars.ButtonTool buttonTool20 = new Infragistics.Win.UltraWinToolbars.ButtonTool("manageServersButton");
            Infragistics.Win.UltraWinToolbars.ButtonTool buttonTool21 = new Infragistics.Win.UltraWinToolbars.ButtonTool("addUserViewButton");
            Infragistics.Win.UltraWinToolbars.ButtonTool buttonTool22 = new Infragistics.Win.UltraWinToolbars.ButtonTool("removeInstanceFromViewButton");
            Infragistics.Win.UltraWinToolbars.ButtonTool buttonTool23 = new Infragistics.Win.UltraWinToolbars.ButtonTool("openButton");
            Infragistics.Win.UltraWinToolbars.ButtonTool buttonTool24 = new Infragistics.Win.UltraWinToolbars.ButtonTool("renameUserViewButton");
            Infragistics.Win.UltraWinToolbars.ButtonTool buttonTool25 = new Infragistics.Win.UltraWinToolbars.ButtonTool("moveUserViewUpInListButton");
            Infragistics.Win.Appearance appearance6 = new Infragistics.Win.Appearance();
            Infragistics.Win.UltraWinToolbars.ButtonTool buttonTool26 = new Infragistics.Win.UltraWinToolbars.ButtonTool("moveUserViewDownInListButton");
            Infragistics.Win.Appearance appearance7 = new Infragistics.Win.Appearance();
            Infragistics.Win.UltraWinToolbars.ButtonTool buttonTool27 = new Infragistics.Win.UltraWinToolbars.ButtonTool("configureAlertsButton");
            Infragistics.Win.UltraWinToolbars.StateButtonTool stateButtonTool2 = new Infragistics.Win.UltraWinToolbars.StateButtonTool("maintenanceModeStateButton", "");
            Infragistics.Win.UltraWinToolbars.PopupMenuTool popupMenuTool3 = new Infragistics.Win.UltraWinToolbars.PopupMenuTool("LazyLoadingNodePopupMenu");
            Infragistics.Win.UltraWinToolbars.ButtonTool buttonTool28 = new Infragistics.Win.UltraWinToolbars.ButtonTool("refreshButton");
            Infragistics.Win.UltraWinToolbars.ButtonTool buttonTool29 = new Infragistics.Win.UltraWinToolbars.ButtonTool("changeMaintenanceModeButton");
            Infragistics.Win.UltraWinToolbars.ButtonTool buttonTool31 = new Infragistics.Win.UltraWinToolbars.ButtonTool("configureBaselineButton");
            Infragistics.Win.UltraWinToolbars.ButtonTool buttonTool32 = new Infragistics.Win.UltraWinToolbars.ButtonTool("refreshAlertsButton");
            Infragistics.Win.Appearance appearance18 = new Infragistics.Win.Appearance();
            Infragistics.Win.UltraWinToolbars.ButtonTool buttonTool3 = new Infragistics.Win.UltraWinToolbars.ButtonTool("snoozeAlertsButton");
            Infragistics.Win.UltraWinToolbars.ButtonTool buttonTool35 = new Infragistics.Win.UltraWinToolbars.ButtonTool("unsnoozeAlertsButton");
            Infragistics.Win.UltraWinToolbars.PopupMenuTool popupMenuTool4 = new Infragistics.Win.UltraWinToolbars.PopupMenuTool("tagsPopupMenu");
            Infragistics.Win.UltraWinToolbars.ButtonTool buttonTool46 = new Infragistics.Win.UltraWinToolbars.ButtonTool("openButton");
            Infragistics.Win.UltraWinToolbars.ButtonTool buttonTool40 = new Infragistics.Win.UltraWinToolbars.ButtonTool("addTagButton");
            Infragistics.Win.UltraWinToolbars.ButtonTool buttonTool43 = new Infragistics.Win.UltraWinToolbars.ButtonTool("deleteTagButton");
            Infragistics.Win.UltraWinToolbars.ButtonTool buttonTool58 = new Infragistics.Win.UltraWinToolbars.ButtonTool(TextConstants.SnoozeServersAlertsButtonButtonKey);
            Infragistics.Win.UltraWinToolbars.ButtonTool buttonTool62 = new Infragistics.Win.UltraWinToolbars.ButtonTool(TextConstants.ResumeServersAlertsButtonButtonKey);
            Infragistics.Win.UltraWinToolbars.ButtonTool buttonTool45 = new Infragistics.Win.UltraWinToolbars.ButtonTool("tagPropertiesButton");
            Infragistics.Win.UltraWinToolbars.ButtonTool buttonTool41 = new Infragistics.Win.UltraWinToolbars.ButtonTool("manageTagsButton");
            Infragistics.Win.UltraWinToolbars.ButtonTool buttonTool38 = new Infragistics.Win.UltraWinToolbars.ButtonTool("manageTagsButton");
            Infragistics.Win.Appearance appearance20 = new Infragistics.Win.Appearance();
            Infragistics.Win.UltraWinToolbars.ButtonTool buttonTool39 = new Infragistics.Win.UltraWinToolbars.ButtonTool("addTagButton");
            Infragistics.Win.UltraWinToolbars.ButtonTool buttonTool59 = new Infragistics.Win.UltraWinToolbars.ButtonTool(TextConstants.SnoozeServersAlertsButtonButtonKey);
            Infragistics.Win.UltraWinToolbars.ButtonTool buttonTool63 = new Infragistics.Win.UltraWinToolbars.ButtonTool(TextConstants.ResumeServersAlertsButtonButtonKey);
            Infragistics.Win.UltraWinToolbars.ButtonTool buttonTool42 = new Infragistics.Win.UltraWinToolbars.ButtonTool("deleteTagButton");
            Infragistics.Win.Appearance appearance19 = new Infragistics.Win.Appearance();
            Infragistics.Win.UltraWinToolbars.ButtonTool buttonTool44 = new Infragistics.Win.UltraWinToolbars.ButtonTool("tagPropertiesButton");
            Infragistics.Win.Appearance appearance21 = new Infragistics.Win.Appearance();
            Infragistics.Win.UltraWinToolbars.PopupMenuTool popupMenuTool5 = new Infragistics.Win.UltraWinToolbars.PopupMenuTool("tagPopupMenu");
            Infragistics.Win.UltraWinToolbars.ButtonTool buttonTool47 = new Infragistics.Win.UltraWinToolbars.ButtonTool("openButton");
            Infragistics.Win.UltraWinToolbars.ButtonTool buttonTool49 = new Infragistics.Win.UltraWinToolbars.ButtonTool("refreshButton");
            Infragistics.Win.UltraWinToolbars.ButtonTool buttonTool48 = new Infragistics.Win.UltraWinToolbars.ButtonTool("deleteTagButton");
            Infragistics.Win.UltraWinToolbars.ButtonTool buttonTool61 = new Infragistics.Win.UltraWinToolbars.ButtonTool(TextConstants.SnoozeServersAlertsButtonButtonKey);
			Infragistics.Win.UltraWinToolbars.ButtonTool buttonTool64 = new Infragistics.Win.UltraWinToolbars.ButtonTool(TextConstants.ResumeServersAlertsButtonButtonKey);
            Infragistics.Win.UltraWinToolbars.ButtonTool buttonTool50 = new Infragistics.Win.UltraWinToolbars.ButtonTool("tagPropertiesButton");
            Infragistics.Win.UltraWinToolbars.ButtonTool buttonTool51 = new Infragistics.Win.UltraWinToolbars.ButtonTool("pulseProfileButton");
            Infragistics.Win.UltraWinToolbars.ButtonTool buttonTool54 = new Infragistics.Win.UltraWinToolbars.ButtonTool("pulseFollowButton");
            Infragistics.Win.UltraWinToolbars.ButtonTool buttonTool52 = new Infragistics.Win.UltraWinToolbars.ButtonTool("applyAlertTemplateButton");
            Infragistics.Win.UltraWinToolbars.ButtonTool buttonTool57 = new Infragistics.Win.UltraWinToolbars.ButtonTool("viewAllServerProperties");
            Infragistics.Win.UltraWinToolbars.ButtonTool buttonTool65 = new Infragistics.Win.UltraWinToolbars.ButtonTool("snoozeAllAlertsServerButton");
            Infragistics.Win.UltraWinToolbars.ButtonTool buttonTool67 = new Infragistics.Win.UltraWinToolbars.ButtonTool("resumeAllAlertsServerButton");

            Infragistics.Win.UltraWinToolbars.PopupMenuTool maintenanceModeGroup = new Infragistics.Win.UltraWinToolbars.PopupMenuTool(TextConstants.MaintenanceModeButtonKey);
            Infragistics.Win.UltraWinToolbars.ButtonTool maintenanceEnabledButton = new Infragistics.Win.UltraWinToolbars.ButtonTool(TextConstants.MaintenanceModeEnableButtonKey);
            Infragistics.Win.UltraWinToolbars.ButtonTool maintenanceDisabledButton = new Infragistics.Win.UltraWinToolbars.ButtonTool(TextConstants.MaintenanceModeDisableButtonKey);
            Infragistics.Win.UltraWinToolbars.ButtonTool maintenanceScheduleButton = new Infragistics.Win.UltraWinToolbars.ButtonTool(TextConstants.MaintenanceModeScheduleButtonKey);
            Infragistics.Win.UltraWinToolbars.ButtonTool applyAlertTemplateTagButton = new Infragistics.Win.UltraWinToolbars.ButtonTool(TextConstants.ApplyAlertTemplateTagButtonKey);
            
            this.userViewTreeView = new System.Windows.Forms.TreeView();
            this.images = new System.Windows.Forms.ImageList(this.components);
            this.imagesDarkTheme = new System.Windows.Forms.ImageList(this.components);
            this.serverGroupPanel = new System.Windows.Forms.Panel();
            this.userViewTreeStatusLabel = new System.Windows.Forms.Label();
            this.userViewsPanel = new System.Windows.Forms.Panel();
            this.userViewsTreeView = new System.Windows.Forms.TreeView();
            this.toolTipManager = new Infragistics.Win.UltraWinToolTip.UltraToolTipManager(this.components);
            this.tagsPanel = new System.Windows.Forms.Panel();
            this.manageTagsLabel = new System.Windows.Forms.Label();
            this.tagsTreeView = new System.Windows.Forms.TreeView();
            this._ServersNavigationPane_Toolbars_Dock_Area_Left = new Infragistics.Win.UltraWinToolbars.UltraToolbarsDockArea();
            this._ServersNavigationPane_Toolbars_Dock_Area_Right = new Infragistics.Win.UltraWinToolbars.UltraToolbarsDockArea();
            this._ServersNavigationPane_Toolbars_Dock_Area_Top = new Infragistics.Win.UltraWinToolbars.UltraToolbarsDockArea();
            this._ServersNavigationPane_Toolbars_Dock_Area_Bottom = new Infragistics.Win.UltraWinToolbars.UltraToolbarsDockArea();
            this.userViewHeaderStrip = new System.Windows.Forms.ToolStrip();
            this.serverGroupHeaderStripLabel = new System.Windows.Forms.ToolStripLabel();
            this.searchPopHeaderStripLabel= new System.Windows.Forms.ToolStripLabel();
            this.tagsHeaderStrip = new System.Windows.Forms.ToolStrip();
            this.searchPopHeaderStrip= new System.Windows.Forms.ToolStrip(); 
            this.toolStripLabel1 = new System.Windows.Forms.ToolStripLabel();
            this.toggleTagsPanelButton = new System.Windows.Forms.ToolStripButton();
            this.userViewsHeaderStrip = new System.Windows.Forms.ToolStrip();
            this.userViewsHeaderStripLabel = new System.Windows.Forms.ToolStripLabel();
            this.toggleUserViewsButton = new System.Windows.Forms.ToolStripButton();
            this.toolbarsManager = new ContextMenuManager(this.components);
            this.serverGroupPanel.SuspendLayout();
            this.userViewsPanel.SuspendLayout();
            this.tagsPanel.SuspendLayout();
            this.userViewHeaderStrip.SuspendLayout();
            this.tagsHeaderStrip.SuspendLayout();
            this.userViewsHeaderStrip.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.toolbarsManager)).BeginInit();
            this.SuspendLayout();
            // 
            // userViewTreeView
            // 
            this.userViewTreeView.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.userViewTreeView.Dock = System.Windows.Forms.DockStyle.Fill;
            this.userViewTreeView.FullRowSelect = true;
            this.userViewTreeView.HideSelection = false;
            this.userViewTreeView.ImageIndex = 0;
            this.userViewTreeView.ImageList = this.images;
            this.userViewTreeView.Location = new System.Drawing.Point(0, 19);
            this.userViewTreeView.Name = "userViewTreeView";
            this.userViewTreeView.SelectedImageIndex = 0;
            this.userViewTreeView.ShowLines = false;
            this.userViewTreeView.Size = new System.Drawing.Size(290, 222);
            this.userViewTreeView.TabIndex = 1;
            this.userViewTreeView.BeforeCollapse += new System.Windows.Forms.TreeViewCancelEventHandler(this.userViewTreeView_BeforeCollapse);
            this.userViewTreeView.BeforeExpand += new System.Windows.Forms.TreeViewCancelEventHandler(this.userViewTreeView_BeforeExpand);
            this.userViewTreeView.BeforeSelect += new System.Windows.Forms.TreeViewCancelEventHandler(this.userViewTreeView_BeforeSelect);
            this.userViewTreeView.AfterSelect += new System.Windows.Forms.TreeViewEventHandler(this.userViewTreeView_AfterSelect);
            this.userViewTreeView.NodeMouseClick += new System.Windows.Forms.TreeNodeMouseClickEventHandler(this.userViewTreeView_NodeMouseClick);
            this.userViewTreeView.MouseDown += new System.Windows.Forms.MouseEventHandler(this.userViewTreeView_MouseDown);
            this.userViewTreeView.MouseMove += new System.Windows.Forms.MouseEventHandler(this.userViewTreeView_MouseMove);
            // 
            // images
            // 
            this.images.ColorDepth = System.Windows.Forms.ColorDepth.Depth32Bit;
            this.images.ImageSize = new System.Drawing.Size(16, 16);
            this.images.TransparentColor = System.Drawing.Color.Transparent;
            this.imagesDarkTheme.ColorDepth = this.images.ColorDepth;
            this.imagesDarkTheme.ImageSize = this.images.ImageSize;
            this.imagesDarkTheme.TransparentColor = this.images.TransparentColor;
            // 
            // serverGroupPanel
            // 
            this.serverGroupPanel.Controls.Add(this.userViewTreeStatusLabel);
            this.serverGroupPanel.Controls.Add(this.userViewTreeView);
            this.serverGroupPanel.Controls.Add(this.userViewHeaderStrip);
            this.serverGroupPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.serverGroupPanel.Location = new System.Drawing.Point(0, 240);
            this.serverGroupPanel.Name = "serverGroupPanel";
            this.serverGroupPanel.Size = new System.Drawing.Size(290, 241);
            this.serverGroupPanel.TabIndex = 10;
            // 
            // userViewTreeStatusLabel
            // 
            this.userViewTreeStatusLabel.BackColor = System.Drawing.Color.White;
            this.userViewTreeStatusLabel.Dock = System.Windows.Forms.DockStyle.Top;
            this.userViewTreeStatusLabel.Location = new System.Drawing.Point(0, 19);
            this.userViewTreeStatusLabel.Name = "userViewTreeStatusLabel";
            this.userViewTreeStatusLabel.Size = new System.Drawing.Size(290, 23);
            this.userViewTreeStatusLabel.TabIndex = 2;
            this.userViewTreeStatusLabel.Text = Idera.SQLdm.Common.Constants.LOADING;
            this.userViewTreeStatusLabel.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // userViewsPanel
            // 
            this.userViewsPanel.Controls.Add(this.userViewsTreeView);
            this.userViewsPanel.Controls.Add(this.userViewsHeaderStrip);
            this.userViewsPanel.Dock = System.Windows.Forms.DockStyle.Top;
            this.userViewsPanel.Location = new System.Drawing.Point(0, 0);
            this.userViewsPanel.Name = "userViewsPanel";
            this.userViewsPanel.Size = new System.Drawing.Size(290, 120);
            this.userViewsPanel.TabIndex = 11;
            // 
            // userViewsTreeView
            // 
            this.userViewsTreeView.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.userViewsTreeView.Dock = System.Windows.Forms.DockStyle.Fill;
            this.userViewsTreeView.FullRowSelect = true;
            this.userViewsTreeView.HideSelection = false;
            this.userViewsTreeView.ImageIndex = 0;
            this.userViewsTreeView.ImageList = this.images;
            this.userViewsTreeView.Location = new System.Drawing.Point(0, 19);
            this.userViewsTreeView.Name = "userViewsTreeView";
            this.userViewsTreeView.SelectedImageIndex = 0;
            this.userViewsTreeView.ShowLines = false;
            this.userViewsTreeView.Size = new System.Drawing.Size(290, 101);
            this.userViewsTreeView.TabIndex = 1;
            this.userViewsTreeView.BeforeLabelEdit += new System.Windows.Forms.NodeLabelEditEventHandler(this.userViewsTreeView_BeforeLabelEdit);
            this.userViewsTreeView.AfterLabelEdit += new System.Windows.Forms.NodeLabelEditEventHandler(this.userViewsTreeView_AfterLabelEdit);
            this.userViewsTreeView.AfterSelect += new System.Windows.Forms.TreeViewEventHandler(this.userViewsTreeView_AfterSelect);
            this.userViewsTreeView.NodeMouseClick += new System.Windows.Forms.TreeNodeMouseClickEventHandler(this.userViewsTreeView_NodeMouseClick);
            this.userViewsTreeView.MouseClick += new System.Windows.Forms.MouseEventHandler(this.userViewsTreeView_MouseClick);
            this.userViewsTreeView.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler(this.userViewsTreeView_MouseDoubleClick);
            // 
            // toolTipManager
            // 
            this.toolTipManager.AutoPopDelay = 0;
            this.toolTipManager.ContainingControl = this;
            this.toolTipManager.DisplayStyle = Infragistics.Win.ToolTipDisplayStyle.Office2007;
            // 
            // tagsPanel
            // 
            this.tagsPanel.Controls.Add(this.manageTagsLabel);
            this.tagsPanel.Controls.Add(this.tagsTreeView);
            this.tagsPanel.Controls.Add(this.tagsHeaderStrip);
            this.tagsPanel.Dock = System.Windows.Forms.DockStyle.Top;
            this.tagsPanel.Location = new System.Drawing.Point(0, 120);
            this.tagsPanel.Name = "tagsPanel";
            this.tagsPanel.Size = new System.Drawing.Size(290, 120);
            this.tagsPanel.TabIndex = 16;
            // 
            // manageTagsLabel
            // 
            this.manageTagsLabel.BackColor = System.Drawing.Color.White;
            this.manageTagsLabel.Dock = System.Windows.Forms.DockStyle.Top;
            this.manageTagsLabel.ForeColor = System.Drawing.SystemColors.GrayText;
            this.manageTagsLabel.Location = new System.Drawing.Point(0, 19);
            this.manageTagsLabel.Name = "manageTagsLabel";
            this.manageTagsLabel.Size = new System.Drawing.Size(290, 23);
            this.manageTagsLabel.TabIndex = 3;
            this.manageTagsLabel.Text = "< Click here to manage tags >";
            this.manageTagsLabel.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.manageTagsLabel.MouseClick += new System.Windows.Forms.MouseEventHandler(this.manageTagsLabel_MouseClick);
            // 
            // tagsTreeView
            // 
            this.tagsTreeView.BackColor = System.Drawing.SystemColors.Window;
            this.tagsTreeView.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.tagsTreeView.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tagsTreeView.FullRowSelect = true;
            this.tagsTreeView.HideSelection = false;
            this.tagsTreeView.ImageIndex = 0;
            this.tagsTreeView.ImageList = this.images;
            this.tagsTreeView.Location = new System.Drawing.Point(0, 19);
            this.tagsTreeView.Name = "tagsTreeView";
            this.tagsTreeView.SelectedImageIndex = 0;
            this.tagsTreeView.ShowLines = false;
            this.tagsTreeView.Size = new System.Drawing.Size(290, 101);
            this.tagsTreeView.TabIndex = 2;
            this.tagsTreeView.AfterSelect += new System.Windows.Forms.TreeViewEventHandler(this.tagsTreeView_AfterSelect);
            this.tagsTreeView.NodeMouseClick += new System.Windows.Forms.TreeNodeMouseClickEventHandler(this.tagsTreeView_NodeMouseClick);
            this.tagsTreeView.MouseClick += new System.Windows.Forms.MouseEventHandler(this.tagsTreeView_MouseClick);
            this.tagsTreeView.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler(this.tagsTreeView_MouseDoubleClick);
            // 
            // _ServersNavigationPane_Toolbars_Dock_Area_Left
            // 
            this._ServersNavigationPane_Toolbars_Dock_Area_Left.AccessibleRole = System.Windows.Forms.AccessibleRole.Grouping;
            this._ServersNavigationPane_Toolbars_Dock_Area_Left.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(191)))), ((int)(((byte)(219)))), ((int)(((byte)(255)))));
            this._ServersNavigationPane_Toolbars_Dock_Area_Left.DockedPosition = Infragistics.Win.UltraWinToolbars.DockedPosition.Left;
            this._ServersNavigationPane_Toolbars_Dock_Area_Left.ForeColor = System.Drawing.SystemColors.ControlText;
            this._ServersNavigationPane_Toolbars_Dock_Area_Left.Location = new System.Drawing.Point(0, 0);
            this._ServersNavigationPane_Toolbars_Dock_Area_Left.Name = "_ServersNavigationPane_Toolbars_Dock_Area_Left";
            this._ServersNavigationPane_Toolbars_Dock_Area_Left.Size = new System.Drawing.Size(0, 481);
            this._ServersNavigationPane_Toolbars_Dock_Area_Left.ToolbarsManager = this.toolbarsManager;
            // 
            // _ServersNavigationPane_Toolbars_Dock_Area_Right
            // 
            this._ServersNavigationPane_Toolbars_Dock_Area_Right.AccessibleRole = System.Windows.Forms.AccessibleRole.Grouping;
            this._ServersNavigationPane_Toolbars_Dock_Area_Right.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(191)))), ((int)(((byte)(219)))), ((int)(((byte)(255)))));
            this._ServersNavigationPane_Toolbars_Dock_Area_Right.DockedPosition = Infragistics.Win.UltraWinToolbars.DockedPosition.Right;
            this._ServersNavigationPane_Toolbars_Dock_Area_Right.ForeColor = System.Drawing.SystemColors.ControlText;
            this._ServersNavigationPane_Toolbars_Dock_Area_Right.Location = new System.Drawing.Point(290, 0);
            this._ServersNavigationPane_Toolbars_Dock_Area_Right.Name = "_ServersNavigationPane_Toolbars_Dock_Area_Right";
            this._ServersNavigationPane_Toolbars_Dock_Area_Right.Size = new System.Drawing.Size(0, 481);
            this._ServersNavigationPane_Toolbars_Dock_Area_Right.ToolbarsManager = this.toolbarsManager;
            // 
            // _ServersNavigationPane_Toolbars_Dock_Area_Top
            // 
            this._ServersNavigationPane_Toolbars_Dock_Area_Top.AccessibleRole = System.Windows.Forms.AccessibleRole.Grouping;
            this._ServersNavigationPane_Toolbars_Dock_Area_Top.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(191)))), ((int)(((byte)(219)))), ((int)(((byte)(255)))));
            this._ServersNavigationPane_Toolbars_Dock_Area_Top.DockedPosition = Infragistics.Win.UltraWinToolbars.DockedPosition.Top;
            this._ServersNavigationPane_Toolbars_Dock_Area_Top.ForeColor = System.Drawing.SystemColors.ControlText;
            this._ServersNavigationPane_Toolbars_Dock_Area_Top.Location = new System.Drawing.Point(0, 0);
            this._ServersNavigationPane_Toolbars_Dock_Area_Top.Name = "_ServersNavigationPane_Toolbars_Dock_Area_Top";
            this._ServersNavigationPane_Toolbars_Dock_Area_Top.Size = new System.Drawing.Size(290, 0);
            this._ServersNavigationPane_Toolbars_Dock_Area_Top.ToolbarsManager = this.toolbarsManager;
            // 
            // _ServersNavigationPane_Toolbars_Dock_Area_Bottom
            // 
            this._ServersNavigationPane_Toolbars_Dock_Area_Bottom.AccessibleRole = System.Windows.Forms.AccessibleRole.Grouping;
            this._ServersNavigationPane_Toolbars_Dock_Area_Bottom.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(191)))), ((int)(((byte)(219)))), ((int)(((byte)(255)))));
            this._ServersNavigationPane_Toolbars_Dock_Area_Bottom.DockedPosition = Infragistics.Win.UltraWinToolbars.DockedPosition.Bottom;
            this._ServersNavigationPane_Toolbars_Dock_Area_Bottom.ForeColor = System.Drawing.SystemColors.ControlText;
            this._ServersNavigationPane_Toolbars_Dock_Area_Bottom.Location = new System.Drawing.Point(0, 481);
            this._ServersNavigationPane_Toolbars_Dock_Area_Bottom.Name = "_ServersNavigationPane_Toolbars_Dock_Area_Bottom";
            this._ServersNavigationPane_Toolbars_Dock_Area_Bottom.Size = new System.Drawing.Size(290, 0);
            this._ServersNavigationPane_Toolbars_Dock_Area_Bottom.ToolbarsManager = this.toolbarsManager;
            //
            // searchPopHeaderStrip
            //
            this.searchPopHeaderStrip.AutoSize =true;
            this.searchPopHeaderStrip.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.searchPopHeaderStrip.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(75)))), ((int)(((byte)(75)))), ((int)(((byte)(75)))));
            this.searchPopHeaderStrip.GripMargin = new System.Windows.Forms.Padding(0);
            this.searchPopHeaderStrip.GripStyle = System.Windows.Forms.ToolStripGripStyle.Hidden;
            this.searchPopHeaderStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.searchPopHeaderStripLabel});
            this.searchPopHeaderStrip.Location = new System.Drawing.Point(0, 0);
            this.searchPopHeaderStrip.Name = "searchPopHeaderStrip";
            this.searchPopHeaderStrip.Padding = new System.Windows.Forms.Padding(0, 3, 0, 3);
            this.searchPopHeaderStrip.Size = new System.Drawing.Size(290, 33);
            this.searchPopHeaderStrip.TabIndex = 0;
            this.searchPopHeaderStrip.Renderer = new FlatToolStripRenderer();
            // 
            // searchPopHeaderStripLabel
            // 
            this.searchPopHeaderStripLabel.Font = new System.Drawing.Font("Tahoma", 8.25f, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Pixel, ((byte)(0)));
            this.searchPopHeaderStripLabel.Name = "searchPopHeaderStripLabel";
            this.searchPopHeaderStripLabel.Size = new System.Drawing.Size(51, 25);
            this.searchPopHeaderStripLabel.Text = "SEARCH";
            this.searchPopHeaderStripLabel.Margin = new System.Windows.Forms.Padding(12, 0, 0, 0);
            // 
            // userViewHeaderStrip
            // 
            this.userViewHeaderStrip.AutoSize = false;
            this.userViewHeaderStrip.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.userViewHeaderStrip.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(75)))), ((int)(((byte)(75)))), ((int)(((byte)(75)))));
            this.userViewHeaderStrip.GripMargin = new System.Windows.Forms.Padding(0);
            this.userViewHeaderStrip.GripStyle = System.Windows.Forms.ToolStripGripStyle.Hidden;
            this.userViewHeaderStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.serverGroupHeaderStripLabel});
            this.userViewHeaderStrip.Location = new System.Drawing.Point(0, 0);
            this.userViewHeaderStrip.Name = "userViewHeaderStrip";
            this.userViewHeaderStrip.Padding = new System.Windows.Forms.Padding(0,3,0,3);
            this.userViewHeaderStrip.Size = new System.Drawing.Size(290, 33);
            this.userViewHeaderStrip.TabIndex = 0;
            this.userViewHeaderStrip.Renderer = new FlatToolStripRenderer();
            // 
            // serverGroupHeaderStripLabel
            // 
            this.serverGroupHeaderStripLabel.Font = new System.Drawing.Font("Tahoma", 8.25f, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Pixel, ((byte)(0)));
            this.serverGroupHeaderStripLabel.Name = "serverGroupHeaderStripLabel";
            this.serverGroupHeaderStripLabel.Size = new System.Drawing.Size(51, 25);
            this.serverGroupHeaderStripLabel.Text = "SERVERS";
            this.serverGroupHeaderStripLabel.Margin = new System.Windows.Forms.Padding(12, 0, 0, 0);
            this.serverGroupHeaderStripLabel.Image = global::Idera.SQLdm.DesktopClient.Properties.Resources.Server;
            // 
            // tagsHeaderStrip
            // 
            this.tagsHeaderStrip.AutoSize = false;
            this.tagsHeaderStrip.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.tagsHeaderStrip.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(75)))), ((int)(((byte)(75)))), ((int)(((byte)(75)))));
            this.tagsHeaderStrip.GripMargin = new System.Windows.Forms.Padding(0);
            this.tagsHeaderStrip.GripStyle = System.Windows.Forms.ToolStripGripStyle.Hidden;
            this.tagsHeaderStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripLabel1,
            this.toggleTagsPanelButton});
            this.tagsHeaderStrip.Location = new System.Drawing.Point(0, 0);
            this.tagsHeaderStrip.Name = "tagsHeaderStrip";
            this.tagsHeaderStrip.Padding = new System.Windows.Forms.Padding(0,3,0,3);
            this.tagsHeaderStrip.Size = new System.Drawing.Size(290, 33);
            this.tagsHeaderStrip.TabIndex = 1;
            this.tagsHeaderStrip.MouseClick += new System.Windows.Forms.MouseEventHandler(this.tagsHeaderStrip_MouseClick);
            this.tagsHeaderStrip.Renderer = new FlatToolStripRenderer();
            // 
            // toolStripLabel1
            // 
            this.toolStripLabel1.Font = new System.Drawing.Font("Tahoma", 8.25f, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Pixel, ((byte)(0)));
            this.toolStripLabel1.Name = "toolStripLabel1";
            this.toolStripLabel1.Size = new System.Drawing.Size(34, 25);
            this.toolStripLabel1.Text = "TAGS";
            this.toolStripLabel1.Margin = new System.Windows.Forms.Padding(12, 0, 0, 0);
            this.toolStripLabel1.Image = global::Idera.SQLdm.DesktopClient.Properties.Resources.Tag16x16; 
            // 
            // toggleTagsPanelButton
            // 
            this.toggleTagsPanelButton.Alignment = System.Windows.Forms.ToolStripItemAlignment.Right;
            this.toggleTagsPanelButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.toggleTagsPanelButton.Enabled = false;
            this.toggleTagsPanelButton.Image = global::Idera.SQLdm.DesktopClient.Properties.Resources.HeaderStripSmallCollapse;
            this.toggleTagsPanelButton.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None;
            this.toggleTagsPanelButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toggleTagsPanelButton.Name = "toggleTagsPanelButton";
            this.toggleTagsPanelButton.Size = new System.Drawing.Size(23, 16);
            // 
            // userViewsHeaderStrip
            // 
            this.userViewsHeaderStrip.AutoSize = false;
            this.userViewsHeaderStrip.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.userViewsHeaderStrip.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(75)))), ((int)(((byte)(75)))), ((int)(((byte)(75)))));
            this.userViewsHeaderStrip.GripMargin = new System.Windows.Forms.Padding(0);
            this.userViewsHeaderStrip.GripStyle = System.Windows.Forms.ToolStripGripStyle.Hidden;
            this.userViewsHeaderStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.userViewsHeaderStripLabel,
            this.toggleUserViewsButton});
            this.userViewsHeaderStrip.Location = new System.Drawing.Point(0, 0);
            this.userViewsHeaderStrip.Name = "userViewsHeaderStrip";
            this.userViewsHeaderStrip.Padding = new System.Windows.Forms.Padding(0,3,0,3);
            this.userViewsHeaderStrip.Size = new System.Drawing.Size(290, 33);
            this.userViewsHeaderStrip.TabIndex = 0;
            this.userViewsHeaderStrip.MouseClick += new System.Windows.Forms.MouseEventHandler(this.userViewsHeaderStrip_MouseClick);
            this.userViewsHeaderStrip.Renderer = new FlatToolStripRenderer();
            // 
            // userViewsHeaderStripLabel
            // 
            this.userViewsHeaderStripLabel.Font = new System.Drawing.Font("Tahoma", 8.25f, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Pixel, ((byte)(0)));
            this.userViewsHeaderStripLabel.Name = "userViewsHeaderStripLabel";
            this.userViewsHeaderStripLabel.Size = new System.Drawing.Size(59, 25);
            this.userViewsHeaderStripLabel.Text = "MY VIEWS";
            this.userViewsHeaderStripLabel.Margin = new System.Windows.Forms.Padding(12, 0, 0, 0);
            this.userViewsHeaderStripLabel.Image = global::Idera.SQLdm.DesktopClient.Properties.Resources.Views16x16;
            // 
            // toggleUserViewsButton
            // 
            this.toggleUserViewsButton.Alignment = System.Windows.Forms.ToolStripItemAlignment.Right;
            this.toggleUserViewsButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.toggleUserViewsButton.Enabled = false;
            this.toggleUserViewsButton.Image = global::Idera.SQLdm.DesktopClient.Properties.Resources.HeaderStripSmallCollapse;
            this.toggleUserViewsButton.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None;
            this.toggleUserViewsButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toggleUserViewsButton.Name = "toggleUserViewsButton";
            this.toggleUserViewsButton.Size = new System.Drawing.Size(23, 16);
            // 
            // toolbarsManager
            // 
            this.toolbarsManager.DesignerFlags = 1;
            this.toolbarsManager.DockWithinContainer = this;
            this.toolbarsManager.HideToolbars = true;
            this.toolbarsManager.Office2007UICompatibility = false;
            this.toolbarsManager.ShowFullMenusDelay = 500;
            this.toolbarsManager.Style = Infragistics.Win.UltraWinToolbars.ToolbarStyle.Office2007;
            appearance1.Image = global::Idera.SQLdm.DesktopClient.Properties.Resources.ToolbarRefresh;
            buttonTool1.SharedPropsInternal.AppearancesSmall.Appearance = appearance1;
            buttonTool1.SharedPropsInternal.Caption = "Refresh";
            popupMenuTool1.SharedPropsInternal.Caption = "Server Popup Menu";
            buttonTool34.InstanceProps.IsFirstInGroup = true;
            appearance2.Image = global::Idera.SQLdm.DesktopClient.Properties.Resources.Delete;
            buttonTool4.InstanceProps.AppearancesSmall.Appearance = appearance2;
            buttonTool4.InstanceProps.Caption = "Delete";
            buttonTool4.InstanceProps.IsFirstInGroup = true;
            buttonTool5.InstanceProps.IsFirstInGroup = true;
            stateButtonTool1.InstanceProps.IsFirstInGroup = true;
            stateButtonTool1.MenuDisplayStyle = Infragistics.Win.UltraWinToolbars.StateButtonMenuDisplayStyle.DisplayCheckmark;
            buttonTool33.InstanceProps.IsFirstInGroup = true;
            buttonTool36.InstanceProps.IsFirstInGroup = true;
            buttonTool53.InstanceProps.IsFirstInGroup = true;
            buttonTool7.InstanceProps.IsFirstInGroup = true;
            buttonTool8.SharedPropsInternal.Caption = "Add Servers...";
            appearance3.Image = global::Idera.SQLdm.DesktopClient.Properties.Resources.Delete;
            buttonTool9.SharedPropsInternal.AppearancesLarge.Appearance = appearance3;
            appearance4.Image = global::Idera.SQLdm.DesktopClient.Properties.Resources.Delete;
            buttonTool9.SharedPropsInternal.AppearancesSmall.Appearance = appearance4;
            buttonTool9.SharedPropsInternal.Caption = "Delete";
            buttonTool9.SharedPropsInternal.Shortcut = System.Windows.Forms.Shortcut.Del;
            appearance5.Image = global::Idera.SQLdm.DesktopClient.Properties.Resources.Properties;
            buttonTool10.SharedPropsInternal.AppearancesSmall.Appearance = appearance5;
            buttonTool10.SharedPropsInternal.Caption = "Properties...";
            popupMenuTool2.SharedPropsInternal.Caption = "User View Popup Menu";
            buttonTool12.InstanceProps.IsFirstInGroup = true;
            buttonTool13.InstanceProps.IsFirstInGroup = true;
            buttonTool14.InstanceProps.IsFirstInGroup = true;
            buttonTool15.InstanceProps.IsFirstInGroup = true;
            buttonTool17.InstanceProps.IsFirstInGroup = true;
            buttonTool19.InstanceProps.IsFirstInGroup = true;
            popupMenuTool2.Tools.AddRange(new Infragistics.Win.UltraWinToolbars.ToolBase[] {
            buttonTool11,
            buttonTool60,
            buttonTool12,
            buttonTool13,
            buttonTool14,
            buttonTool15,
            buttonTool16,
            buttonTool17,
            buttonTool18,
            buttonTool19,
            buttonTool66,
            buttonTool68
            });
            buttonTool20.SharedPropsInternal.Caption = "Manage Servers...";
            buttonTool21.SharedPropsInternal.Caption = "Create View...";
            buttonTool22.SharedPropsInternal.Caption = "Remove From View";
            buttonTool23.SharedPropsInternal.Caption = "Open";
            buttonTool24.SharedPropsInternal.Caption = "Rename...";
            buttonTool24.SharedPropsInternal.Shortcut = System.Windows.Forms.Shortcut.F2;
            appearance6.Image = global::Idera.SQLdm.DesktopClient.Properties.Resources.MoveUp;
            buttonTool25.SharedPropsInternal.AppearancesSmall.Appearance = appearance6;
            buttonTool25.SharedPropsInternal.Caption = "Move Up in List";
            appearance7.Image = global::Idera.SQLdm.DesktopClient.Properties.Resources.MoveDown;
            buttonTool26.SharedPropsInternal.AppearancesSmall.Appearance = appearance7;
            buttonTool26.SharedPropsInternal.Caption = "Move Down in List";
            buttonTool27.SharedPropsInternal.Caption = "Configure Alerts...";
            stateButtonTool2.MenuDisplayStyle = Infragistics.Win.UltraWinToolbars.StateButtonMenuDisplayStyle.DisplayCheckmark;
            stateButtonTool2.SharedPropsInternal.Caption = "Maintenance Mode";
            popupMenuTool3.SharedPropsInternal.Caption = "LazyLoading Node Popup Menu";
            popupMenuTool3.Tools.AddRange(new Infragistics.Win.UltraWinToolbars.ToolBase[] {
            buttonTool28});
            buttonTool29.SharedPropsInternal.Caption = "Disable Maintenance Mode";
            buttonTool29.SharedPropsInternal.Visible = false;
            buttonTool31.SharedPropsInternal.Caption = "Configure Baseline...";
            appearance18.Image = global::Idera.SQLdm.DesktopClient.Properties.Resources.ToolbarRefresh;
            buttonTool32.SharedPropsInternal.AppearancesSmall.Appearance = appearance18;
            buttonTool32.SharedPropsInternal.Caption = "Refresh Alerts";
            buttonTool3.SharedPropsInternal.Caption = "Snooze Alerts...";
            buttonTool35.SharedPropsInternal.Caption = "Resume Alerts...";
            popupMenuTool4.SharedPropsInternal.Caption = "Tags Popup Menu";
            applyAlertTemplateTagButton.SharedPropsInternal.Caption = "Apply Alert Template...";
            buttonTool40.InstanceProps.IsFirstInGroup = true;
            buttonTool41.InstanceProps.IsFirstInGroup = true;
            buttonTool58.InstanceProps.IsFirstInGroup = true;
            applyAlertTemplateTagButton.InstanceProps.IsFirstInGroup = true;
            popupMenuTool4.Tools.AddRange(new Infragistics.Win.UltraWinToolbars.ToolBase[] {
            buttonTool46,
            buttonTool40,
            buttonTool43,
            buttonTool45,
            buttonTool41,
            buttonTool58,
            buttonTool62
            });
            appearance20.Image = global::Idera.SQLdm.DesktopClient.Properties.Resources.Tag16x16;
            buttonTool38.SharedPropsInternal.AppearancesSmall.Appearance = appearance20;
            buttonTool38.SharedPropsInternal.Caption = "Manage Tags...";
            buttonTool39.SharedPropsInternal.Caption = "Add Tag...";
            appearance19.Image = global::Idera.SQLdm.DesktopClient.Properties.Resources.Delete;
            buttonTool42.SharedPropsInternal.AppearancesSmall.Appearance = appearance19;
            buttonTool42.SharedPropsInternal.Caption = "Delete Tag";
            appearance21.Image = global::Idera.SQLdm.DesktopClient.Properties.Resources.Properties;
            buttonTool44.SharedPropsInternal.AppearancesSmall.Appearance = appearance21;
            buttonTool44.SharedPropsInternal.Caption = "Tag Properties...";
            buttonTool59.SharedPropsInternal.Caption = "Snooze Alerts...";
            buttonTool63.SharedPropsInternal.Caption = "Resume Alerts...";
            popupMenuTool5.SharedPropsInternal.Caption = "Tag Popup Menu";
            buttonTool49.InstanceProps.IsFirstInGroup = true;
            buttonTool48.InstanceProps.IsFirstInGroup = true;
            buttonTool50.InstanceProps.IsFirstInGroup = true;
            popupMenuTool5.Tools.AddRange(new Infragistics.Win.UltraWinToolbars.ToolBase[] {
            buttonTool47,
            buttonTool49,
            buttonTool48,
            buttonTool61,
            buttonTool64,
            buttonTool50});
            buttonTool51.SharedPropsInternal.Caption = "View Profile in Newsfeed";
            buttonTool54.SharedPropsInternal.Caption = "Follow updates in Newsfeed";
            buttonTool52.SharedPropsInternal.Caption = "Apply Alert Template... ";
            buttonTool57.SharedPropsInternal.Caption = "View All Server Properties";
            buttonTool65.SharedPropsInternal.Caption = "Snooze Alerts...";
            buttonTool67.SharedPropsInternal.Caption = "Resume Alerts...";
            this.toolbarsManager.Tools.AddRange(new Infragistics.Win.UltraWinToolbars.ToolBase[]
                {
                    buttonTool1,
                    popupMenuTool1,
                    buttonTool8,
                    buttonTool9,
                    buttonTool10,
                    popupMenuTool2,
                    buttonTool20,
                    buttonTool21,
                    buttonTool22,
                    buttonTool23,
                    buttonTool24,
                    buttonTool25,
                    buttonTool26,
                    buttonTool27,
                    stateButtonTool2,
                    popupMenuTool3,
                    buttonTool29,
                    buttonTool31,
                    buttonTool32,
                    buttonTool3,
                    buttonTool35,
                    popupMenuTool4,
                    buttonTool38,
                    buttonTool39,
                    buttonTool59,
                    applyAlertTemplateTagButton,
                    buttonTool63,
                    buttonTool42,
                    buttonTool44,
                    popupMenuTool5,
                    buttonTool51,
                    buttonTool54,
                    buttonTool52,
                    buttonTool57,
                    buttonTool65,
                    buttonTool67,
                    maintenanceModeGroup,
                    maintenanceEnabledButton,
                    maintenanceDisabledButton,
                    maintenanceScheduleButton
                });

            maintenanceModeGroup.SharedPropsInternal.Caption = "Maintenance Mode";
            maintenanceModeGroup.InstanceProps.IsFirstInGroup = true;
            maintenanceEnabledButton.SharedPropsInternal.Caption = "Enable";
            maintenanceDisabledButton.SharedPropsInternal.Caption = "Disable";
            maintenanceScheduleButton.SharedPropsInternal.Caption = "Schedule...";
            
            maintenanceModeGroup.Tools.AddToolRange(new string[]
                {
                    TextConstants.MaintenanceModeEnableButtonKey,
                    TextConstants.MaintenanceModeDisableButtonKey,
                    TextConstants.MaintenanceModeScheduleButtonKey
                });

            popupMenuTool1.Tools.AddRange(new Infragistics.Win.UltraWinToolbars.ToolBase[]
                {
                    buttonTool2,
                    buttonTool34,
                    buttonTool4,
                    buttonTool5
                });

            popupMenuTool1.Tools.AddTool(TextConstants.MaintenanceModeButtonKey);
            
            popupMenuTool1.Tools.AddRange(new Infragistics.Win.UltraWinToolbars.ToolBase[]
                {
                    buttonTool33,
                    buttonTool56,
                    buttonTool6,
                    buttonTool36,
                    buttonTool37,
                    buttonTool53,
                    buttonTool55,
                    buttonTool7
                });

            popupMenuTool2.Tools.AddTool(TextConstants.MaintenanceModeButtonKey);

            popupMenuTool4.Tools.AddTool(TextConstants.ApplyAlertTemplateTagButtonKey);
            popupMenuTool4.Tools.AddTool(TextConstants.MaintenanceModeButtonKey);
           
            popupMenuTool5.Tools.AddTool(TextConstants.MaintenanceModeButtonKey);

            this.toolbarsManager.AfterToolCloseup += new Infragistics.Win.UltraWinToolbars.ToolDropdownEventHandler(this.toolbarsManager_AfterToolCloseup);
            this.toolbarsManager.BeforeShortcutKeyProcessed += new Infragistics.Win.UltraWinToolbars.BeforeShortcutKeyProcessedEventHandler(this.toolbarsManager_BeforeShortcutKeyProcessed);
            this.toolbarsManager.BeforeToolDropdown += new Infragistics.Win.UltraWinToolbars.BeforeToolDropdownEventHandler(this.toolbarsManager_BeforeToolDropdown);
            this.toolbarsManager.ToolClick += new Infragistics.Win.UltraWinToolbars.ToolClickEventHandler(this.toolbarsManager_ToolClick);
            // 
            // ServersNavigationPane
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.Controls.Add(this.serverGroupPanel);
            this.Controls.Add(this.tagsPanel);
            this.Controls.Add(this.userViewsPanel);
            this.Controls.Add(this._ServersNavigationPane_Toolbars_Dock_Area_Left);
            this.Controls.Add(this._ServersNavigationPane_Toolbars_Dock_Area_Right);
            this.Controls.Add(this._ServersNavigationPane_Toolbars_Dock_Area_Top);
            this.Controls.Add(this._ServersNavigationPane_Toolbars_Dock_Area_Bottom);
            this.Margin = new System.Windows.Forms.Padding(0);
            this.Name = "ServersNavigationPane";
            this.Size = new System.Drawing.Size(290, 481);
            this.Load += new System.EventHandler(this.ServersNavigationPane_Load);
            this.serverGroupPanel.ResumeLayout(false);
            this.userViewsPanel.ResumeLayout(false);
            this.tagsPanel.ResumeLayout(false);
            this.userViewHeaderStrip.ResumeLayout(false);
            this.userViewHeaderStrip.PerformLayout();
            this.tagsHeaderStrip.ResumeLayout(false);
            this.tagsHeaderStrip.PerformLayout();
            this.userViewsHeaderStrip.ResumeLayout(false);
            this.userViewsHeaderStrip.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.toolbarsManager)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion
        
        private System.Windows.Forms.TreeView userViewTreeView;
        private System.Windows.Forms.ImageList images;
        private System.Windows.Forms.ImageList imagesDarkTheme;
        private ContextMenuManager toolbarsManager;
        private System.Windows.Forms.ToolStrip userViewsHeaderStrip;
        private System.Windows.Forms.ToolStripLabel userViewsHeaderStripLabel;
        private System.Windows.Forms.ToolStripButton toggleUserViewsButton;
        private System.Windows.Forms.Panel serverGroupPanel;
        private System.Windows.Forms.ToolStrip userViewHeaderStrip;
        private System.Windows.Forms.ToolStrip searchPopHeaderStrip;
        private System.Windows.Forms.ToolStripLabel serverGroupHeaderStripLabel;
        private System.Windows.Forms.ToolStripLabel searchPopHeaderStripLabel;
        private System.Windows.Forms.Panel userViewsPanel;
        private System.Windows.Forms.TreeView userViewsTreeView;
        private System.Windows.Forms.Label userViewTreeStatusLabel;
        private Infragistics.Win.UltraWinToolTip.UltraToolTipManager toolTipManager;
        private System.Windows.Forms.Panel tagsPanel;
        private System.Windows.Forms.Label manageTagsLabel;
        private System.Windows.Forms.TreeView tagsTreeView;
        private System.Windows.Forms.ToolStrip tagsHeaderStrip;
        private System.Windows.Forms.ToolStripLabel toolStripLabel1;
        private System.Windows.Forms.ToolStripButton toggleTagsPanelButton;
        private Infragistics.Win.UltraWinToolbars.UltraToolbarsDockArea _ServersNavigationPane_Toolbars_Dock_Area_Left;
        private Infragistics.Win.UltraWinToolbars.UltraToolbarsDockArea _ServersNavigationPane_Toolbars_Dock_Area_Right;
        private Infragistics.Win.UltraWinToolbars.UltraToolbarsDockArea _ServersNavigationPane_Toolbars_Dock_Area_Top;
        private Infragistics.Win.UltraWinToolbars.UltraToolbarsDockArea _ServersNavigationPane_Toolbars_Dock_Area_Bottom;
    }
}
