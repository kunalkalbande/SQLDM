
namespace Idera.SQLdm.DesktopClient.Views.Servers.ServerGroup
{
    partial class ServerGroupView
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
            System.Diagnostics.Stopwatch stopWatch = new System.Diagnostics.Stopwatch();
            stopWatch.Start();
            this.components = new System.ComponentModel.Container();
            Infragistics.Win.UltraWinToolbars.OptionSet optionSet1 = new Infragistics.Win.UltraWinToolbars.OptionSet("viewOptionSet");
            Infragistics.Win.UltraWinToolbars.PopupMenuTool popupMenuTool1 = new Infragistics.Win.UltraWinToolbars.PopupMenuTool("contextMenu");
            Infragistics.Win.UltraWinToolbars.PopupMenuTool popupMenuTool2 = new Infragistics.Win.UltraWinToolbars.PopupMenuTool("currentViewMenu");
            Infragistics.Win.UltraWinToolbars.PopupMenuTool popupMenuTool3 = new Infragistics.Win.UltraWinToolbars.PopupMenuTool("currentViewMenu");
            Infragistics.Win.UltraWinToolbars.StateButtonTool stateButtonTool1 = new Infragistics.Win.UltraWinToolbars.StateButtonTool("showThumbnailsButton", "viewOptionSet");
            Infragistics.Win.UltraWinToolbars.StateButtonTool stateButtonTool2 = new Infragistics.Win.UltraWinToolbars.StateButtonTool("showDetailsButton", "viewOptionSet");
            Infragistics.Win.UltraWinToolbars.StateButtonTool stateButtonTool3 = new Infragistics.Win.UltraWinToolbars.StateButtonTool("showHeatmapButton", "viewOptionSet"); //Saurabh UX-DM
            Infragistics.Win.UltraWinToolbars.StateButtonTool stateButtonTool4 = new Infragistics.Win.UltraWinToolbars.StateButtonTool("showDetailsButton", "viewOptionSet");
            Infragistics.Win.UltraWinToolbars.StateButtonTool stateButtonTool5 = new Infragistics.Win.UltraWinToolbars.StateButtonTool("showPropertiesButton", "");
            this.borderPanel = new Idera.SQLdm.DesktopClient.Controls.GradientPanel();
            this.contentPanel = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomPanel ();
            this.thumbnailView = new Idera.SQLdm.DesktopClient.Views.Servers.ServerGroup.ServerGroupThumbnailView();
            this.detailsView = new Idera.SQLdm.DesktopClient.Views.Servers.ServerGroup.ServerGroupDetailsView();
            this.propertiesView = new Idera.SQLdm.DesktopClient.Views.Servers.ServerGroup.ServerGroupPropertiesView();
            this.heatmapView = new Idera.SQLdm.DesktopClient.Views.Servers.ServerGroup.ServerGroupHeatMapView(); //Saurabh UX-DM
            //this.heatmapView_v2 = new Idera.SQLdm.DesktopClient.Views.Servers.ServerGroup.ServerGroupHeatMapView_V2();
            this.headerStrip = new Idera.SQLdm.DesktopClient.Controls.HeaderStrip(false, false, true);
            this.titleLabel = new System.Windows.Forms.ToolStripLabel();
            this.showPropertiesViewButton = new System.Windows.Forms.ToolStripButton();
            this.showDetailsViewButton = new System.Windows.Forms.ToolStripButton();
            this.showThumbnailsViewButton = new System.Windows.Forms.ToolStripButton();
            this.showHeatMapViewButton = new System.Windows.Forms.ToolStripButton(); //Saurabh UX-DM
            this.searchInstances = new System.Windows.Forms.ToolStripTextBox();//Saurabh UX-DM
            this.toolbarsManager = new Idera.SQLdm.DesktopClient.Controls.ContextMenuManager(this.components);
            this.borderPanel.SuspendLayout();
            this.contentPanel.SuspendLayout();
            this.headerStrip.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.toolbarsManager)).BeginInit();
            this.SuspendLayout();
            // 
            // borderPanel
            // 
            this.borderPanel.BackColor2 = System.Drawing.Color.White;
            this.borderPanel.BorderColor = System.Drawing.Color.Black;
            this.borderPanel.Controls.Add(this.contentPanel);
            this.borderPanel.Controls.Add(this.headerStrip);
            this.borderPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.borderPanel.FillStyle = Idera.SQLdm.DesktopClient.Controls.GradientPanelFillStyle.Solid;
            this.borderPanel.Location = new System.Drawing.Point(0, 0);
            this.borderPanel.Name = "borderPanel";
            this.borderPanel.Padding = new System.Windows.Forms.Padding(1);
            if (Idera.SQLdm.DesktopClient.Properties.Settings.Default.ColorScheme == "Dark")
                this.borderPanel.Padding = new System.Windows.Forms.Padding(0, 0, 0, 0);
            this.borderPanel.Size = new System.Drawing.Size(557, 495);
            this.borderPanel.TabIndex = 2;
            // 
            // contentPanel
            // 
            this.contentPanel.Controls.Add(this.thumbnailView);
            this.contentPanel.Controls.Add(this.detailsView);
            this.contentPanel.Controls.Add(this.propertiesView);
            //this.contentPanel.Controls.Add(this.heatmapView); // Saurabh UX-DM
            this.contentPanel.Controls.Add(this.heatmapView);
            this.contentPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.contentPanel.Location = new System.Drawing.Point(1, 26);
            this.contentPanel.Name = "contentPanel";
            this.contentPanel.Size = new System.Drawing.Size(555, 468);
            this.contentPanel.TabIndex = 1;
            // 
            // thumbnailView
            // 
            this.thumbnailView.AutoScroll = true;
            this.thumbnailView.BackColor = System.Drawing.Color.Transparent;
            this.thumbnailView.Dock = System.Windows.Forms.DockStyle.Fill;
            this.thumbnailView.Location = new System.Drawing.Point(0, 0);
            this.thumbnailView.Name = "thumbnailView";
            this.thumbnailView.Size = new System.Drawing.Size(555, 468);
            this.thumbnailView.TabIndex = 0;
            // 
            // detailsView
            // 
            this.detailsView.Dock = System.Windows.Forms.DockStyle.Fill;
            this.detailsView.Location = new System.Drawing.Point(0, 0);
            this.detailsView.Name = "detailsView";
            this.detailsView.Size = new System.Drawing.Size(555, 468);
            this.detailsView.TabIndex = 1;
            // 
            // heatmapView
            // 
            this.heatmapView.Dock = System.Windows.Forms.DockStyle.Fill;
            this.heatmapView.Location = new System.Drawing.Point(0, 0);
            this.heatmapView.Name = "heatmapView";
            this.heatmapView.Size = new System.Drawing.Size(555, 468);
            this.heatmapView.TabIndex = 1;
            // 
            // propertiesView
            // 
            this.propertiesView.BackColor = System.Drawing.Color.Transparent;
            this.propertiesView.Dock = System.Windows.Forms.DockStyle.Fill;
            this.propertiesView.Location = new System.Drawing.Point(0, 0);
            this.propertiesView.Name = "propertiesView";
            this.propertiesView.Size = new System.Drawing.Size(555, 468);
            this.propertiesView.TabIndex = 2;
            // 
            // headerStrip
            // 
            this.headerStrip.AutoSize = false;
            this.headerStrip.Font = new System.Drawing.Font("Arial", 12F, System.Drawing.FontStyle.Bold);
            this.headerStrip.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(21)))), ((int)(((byte)(66)))), ((int)(((byte)(139)))));
            this.headerStrip.GripStyle = System.Windows.Forms.ToolStripGripStyle.Hidden;
            this.headerStrip.HeaderImage = global::Idera.SQLdm.DesktopClient.Properties.Resources.NavigationPaneServersSmall;
            this.headerStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.titleLabel,
            this.showPropertiesViewButton,
            this.showDetailsViewButton,
            this.showThumbnailsViewButton,
            this.showHeatMapViewButton,
            this.searchInstances}); //Saurabh UX-DM
            this.headerStrip.Location = new System.Drawing.Point(1, 1);
            this.headerStrip.Name = "headerStrip";
            this.headerStrip.Padding = new System.Windows.Forms.Padding(20, 2, 0, 0);
            this.headerStrip.Size = new System.Drawing.Size(555, 25);//Saurabh UX-DM
            this.headerStrip.TabIndex = 0;
            // 
            // titleLabel
            // 
            this.titleLabel.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(75)))), ((int)(((byte)(75)))), ((int)(((byte)(75)))));
            this.titleLabel.Name = "titleLabel";
            this.titleLabel.Size = new System.Drawing.Size(105, 20);
            this.titleLabel.Text = "Group Name";

            //Saurabh UX-DM
            // Instance Search text box
            //
            this.searchInstances.Name = "txtSearchInstances";
            this.searchInstances.Size = new System.Drawing.Size(105, 20);
            this.searchInstances.Text = "";
            this.searchInstances.Alignment = System.Windows.Forms.ToolStripItemAlignment.Right;
            this.searchInstances.Text = "Search Instances..";
            this.searchInstances.Font = new System.Drawing.Font(DefaultFont, System.Drawing.FontStyle.Italic);
            this.searchInstances.Visible = false;
            this.searchInstances.GotFocus += new System.EventHandler(this.focusGained);
            this.searchInstances.LostFocus += new System.EventHandler(this.focusLost);
            this.searchInstances.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.UpdateHeatMapInstancesOnSearch);
            // 
            // showPropertiesViewButton
            // 
            this.showPropertiesViewButton.Alignment = System.Windows.Forms.ToolStripItemAlignment.Right;
            this.showPropertiesViewButton.CheckOnClick = true;
            this.showPropertiesViewButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.showPropertiesViewButton.Image = global::Idera.SQLdm.DesktopClient.Properties.Resources.Properties;
            this.showPropertiesViewButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.showPropertiesViewButton.Name = "showPropertiesViewButton";
            this.showPropertiesViewButton.Size = new System.Drawing.Size(23, 20);
            this.showPropertiesViewButton.Text = "Show Server Configuration Properties";
            this.showPropertiesViewButton.Click += new System.EventHandler(this.showPropertiesViewButton_Click);
            //
            // showDetailsViewButton
            // 
            this.showDetailsViewButton.Alignment = System.Windows.Forms.ToolStripItemAlignment.Right;
            this.showDetailsViewButton.CheckOnClick = true;
            this.showDetailsViewButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.showDetailsViewButton.Image = global::Idera.SQLdm.DesktopClient.Properties.Resources.ServerGroupDetailsView;
            this.showDetailsViewButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.showDetailsViewButton.Name = "showDetailsViewButton";
            this.showDetailsViewButton.Size = new System.Drawing.Size(23, 20);
            this.showDetailsViewButton.Text = "Show Details";
            this.showDetailsViewButton.Click += new System.EventHandler(this.showDetailsViewButton_Click);
            // 
            // showThumbnailsViewButton
            // 
            this.showThumbnailsViewButton.Alignment = System.Windows.Forms.ToolStripItemAlignment.Right;
            this.showThumbnailsViewButton.Checked = true;
            this.showThumbnailsViewButton.CheckOnClick = true;
            this.showThumbnailsViewButton.CheckState = System.Windows.Forms.CheckState.Checked;
            this.showThumbnailsViewButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.showThumbnailsViewButton.Image = global::Idera.SQLdm.DesktopClient.Properties.Resources.ServerGroupThumbnailView;
            this.showThumbnailsViewButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.showThumbnailsViewButton.Name = "showThumbnailsViewButton";
            this.showThumbnailsViewButton.Size = new System.Drawing.Size(23, 20);
            this.showThumbnailsViewButton.Text = "Show Thumbnails";
            this.showThumbnailsViewButton.Click += new System.EventHandler(this.showThumbnailsViewButton_Click);

            // showHeatmapViewButton - Saurabh UX DM
            this.showHeatMapViewButton.Alignment = System.Windows.Forms.ToolStripItemAlignment.Right;
            this.showHeatMapViewButton.CheckOnClick = true;
            this.showHeatMapViewButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.showHeatMapViewButton.Image = global::Idera.SQLdm.DesktopClient.Properties.Resources.ServerGroupHeatMapView;
            this.showHeatMapViewButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.showHeatMapViewButton.Name = "showHeatmapViewButton";
            this.showHeatMapViewButton.Size = new System.Drawing.Size(23, 20);
            this.showHeatMapViewButton.Text = "Show HeatMap";
            this.showHeatMapViewButton.Click += new System.EventHandler(this.showHeatmapButton_Click);
            // 
            // 
            // toolbarsManager
            // 
            this.toolbarsManager.DesignerFlags = 1;
            this.toolbarsManager.OptionSets.Add(optionSet1);
            this.toolbarsManager.ShowFullMenusDelay = 500;
            this.toolbarsManager.Style = Infragistics.Win.UltraWinToolbars.ToolbarStyle.Office2007;
            popupMenuTool1.SharedPropsInternal.Caption = "contextMenu";
            popupMenuTool1.Tools.AddRange(new Infragistics.Win.UltraWinToolbars.ToolBase[] {
            popupMenuTool2});
            popupMenuTool3.SharedPropsInternal.Caption = "Current View";
            stateButtonTool1.Checked = true;
            stateButtonTool1.MenuDisplayStyle = Infragistics.Win.UltraWinToolbars.StateButtonMenuDisplayStyle.DisplayCheckmark;
            stateButtonTool2.MenuDisplayStyle = Infragistics.Win.UltraWinToolbars.StateButtonMenuDisplayStyle.DisplayCheckmark;
            popupMenuTool3.Tools.AddRange(new Infragistics.Win.UltraWinToolbars.ToolBase[] {
            stateButtonTool1,
            stateButtonTool2});
            stateButtonTool3.Checked = true;
            stateButtonTool3.MenuDisplayStyle = Infragistics.Win.UltraWinToolbars.StateButtonMenuDisplayStyle.DisplayCheckmark;
            stateButtonTool3.OptionSetKey = "viewOptionSet";
            stateButtonTool3.SharedPropsInternal.Caption = "Thumbnail";
            stateButtonTool4.MenuDisplayStyle = Infragistics.Win.UltraWinToolbars.StateButtonMenuDisplayStyle.DisplayCheckmark;
            stateButtonTool4.OptionSetKey = "viewOptionSet";
            stateButtonTool4.SharedPropsInternal.Caption = "Details";
            stateButtonTool5.SharedPropsInternal.Caption = "Properties";
            this.toolbarsManager.Tools.AddRange(new Infragistics.Win.UltraWinToolbars.ToolBase[] {
            popupMenuTool1,
            popupMenuTool3,
            stateButtonTool3,
            stateButtonTool4,
            stateButtonTool5});
            this.toolbarsManager.ToolClick += new Infragistics.Win.UltraWinToolbars.ToolClickEventHandler(this.toolbarsManager_ToolClick);
            // 
            // ServerGroupView
            // 
            this.Controls.Add(this.borderPanel);
            this.Name = "ServerGroupView";
            this.Size = new System.Drawing.Size(557, 495);
            this.borderPanel.ResumeLayout(false);
            this.contentPanel.ResumeLayout(false);
            this.headerStrip.ResumeLayout(false);
            this.headerStrip.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.toolbarsManager)).EndInit();
            this.ResumeLayout(false);
            stopWatch.Stop();
            StartUpTimeLog.DebugFormat("Time taken by ServerGroupView.InitializeComponent : {0}", stopWatch.ElapsedMilliseconds);
        }

        #endregion

        private Idera.SQLdm.DesktopClient.Controls.GradientPanel borderPanel;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomPanel  contentPanel;
        private Idera.SQLdm.DesktopClient.Controls.HeaderStrip headerStrip;
        private System.Windows.Forms.ToolStripLabel titleLabel;
        private Idera.SQLdm.DesktopClient.Controls.ContextMenuManager toolbarsManager;
        private ServerGroupThumbnailView thumbnailView;
        private ServerGroupDetailsView detailsView;
        private ServerGroupHeatMapView heatmapView; //Saurabh UX-DM
        //private ServerGroupHeatMapView_V2 heatmapView_v2;
        private System.Windows.Forms.ToolStripButton showDetailsViewButton;
        private System.Windows.Forms.ToolStripButton showThumbnailsViewButton;
        private System.Windows.Forms.ToolStripButton showPropertiesViewButton;
        private System.Windows.Forms.ToolStripButton showHeatMapViewButton; //Saurabh UX-DM
        private System.Windows.Forms.ToolStripTextBox searchInstances; // Saurabh DM_UX
        private ServerGroupPropertiesView propertiesView;
    }
}
