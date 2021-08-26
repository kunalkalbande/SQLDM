namespace Idera.SQLdm.DesktopClient
{
    partial class SqlServerForm
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
            Infragistics.Win.UltraWinToolbars.ButtonTool buttonTool1 = new Infragistics.Win.UltraWinToolbars.ButtonTool("buttonRefreshView");
            Infragistics.Win.UltraWinToolbars.ButtonTool buttonTool2 = new Infragistics.Win.UltraWinToolbars.ButtonTool("buttonPauseViewRefresh");
            Infragistics.Win.UltraWinToolbars.RibbonTab ribbonTab1 = new Infragistics.Win.UltraWinToolbars.RibbonTab("ribbonHome", "Overview");
            Infragistics.Win.UltraWinToolbars.RibbonTab ribbonTab2 = new Infragistics.Win.UltraWinToolbars.RibbonTab("ribbonSessions", "Sessions");
            Infragistics.Win.UltraWinToolbars.RibbonTab ribbonTab3 = new Infragistics.Win.UltraWinToolbars.RibbonTab("ribbonQueries", "Queries");
            Infragistics.Win.UltraWinToolbars.RibbonTab ribbonTab4 = new Infragistics.Win.UltraWinToolbars.RibbonTab("ribbonResources", "Resources");
            Infragistics.Win.UltraWinToolbars.RibbonTab ribbonTab5 = new Infragistics.Win.UltraWinToolbars.RibbonTab("ribbonDatabases", "Databases");
            Infragistics.Win.UltraWinToolbars.RibbonTab ribbonTab6 = new Infragistics.Win.UltraWinToolbars.RibbonTab("ribbonServices", "Services");
            Infragistics.Win.UltraWinToolbars.RibbonTab ribbonTab7 = new Infragistics.Win.UltraWinToolbars.RibbonTab("ribbonLogs", "Logs");
            Infragistics.Win.UltraWinToolbars.ButtonTool buttonTool3 = new Infragistics.Win.UltraWinToolbars.ButtonTool("buttonRefreshView");
            Infragistics.Win.Appearance appearance1 = new Infragistics.Win.Appearance();
            Infragistics.Win.UltraWinToolbars.ButtonTool buttonTool4 = new Infragistics.Win.UltraWinToolbars.ButtonTool("buttonPauseViewRefresh");
            Infragistics.Win.Appearance appearance2 = new Infragistics.Win.Appearance();
            this.toolbarsManager = new Infragistics.Win.UltraWinToolbars.UltraToolbarsManager(this.components);
            this.viewPanel = new System.Windows.Forms.Panel();
            this._SqlServerForm_Toolbars_Dock_Area_Left = new Infragistics.Win.UltraWinToolbars.UltraToolbarsDockArea();
            this._SqlServerForm_Toolbars_Dock_Area_Right = new Infragistics.Win.UltraWinToolbars.UltraToolbarsDockArea();
            this._SqlServerForm_Toolbars_Dock_Area_Top = new Infragistics.Win.UltraWinToolbars.UltraToolbarsDockArea();
            this._SqlServerForm_Toolbars_Dock_Area_Bottom = new Infragistics.Win.UltraWinToolbars.UltraToolbarsDockArea();
            ((System.ComponentModel.ISupportInitialize)(this.toolbarsManager)).BeginInit();
            this.SuspendLayout();
            // 
            // toolbarsManager
            // 
            this.toolbarsManager.DesignerFlags = 1;
            this.toolbarsManager.DockWithinContainer = this;
            this.toolbarsManager.Ribbon.ApplicationMenuButtonImage = global::Idera.SQLdm.DesktopClient.Properties.Resources.ApplicationMenu;
            this.toolbarsManager.Ribbon.QuickAccessToolbar.Tools.AddRange(new Infragistics.Win.UltraWinToolbars.ToolBase[] {
            buttonTool1,
            buttonTool2});
            ribbonTab1.Key = "ribbonHome";
            ribbonTab2.Key = "ribbonSessions";
            ribbonTab3.Key = "ribbonQueries";
            ribbonTab4.Key = "ribbonResources";
            ribbonTab5.Key = "ribbonDatabases";
            ribbonTab6.Key = "ribbonServices";
            ribbonTab7.Key = "ribbonLogs";
            this.toolbarsManager.Ribbon.Tabs.AddRange(new Infragistics.Win.UltraWinToolbars.RibbonTab[] {
            ribbonTab1,
            ribbonTab2,
            ribbonTab3,
            ribbonTab4,
            ribbonTab5,
            ribbonTab6,
            ribbonTab7});
            this.toolbarsManager.Ribbon.Visible = true;
            appearance1.Image = global::Idera.SQLdm.DesktopClient.Properties.Resources.ToolbarRefresh;
            buttonTool3.SharedProps.AppearancesSmall.Appearance = appearance1;
            buttonTool3.SharedProps.Caption = "Refresh";
            appearance2.Image = global::Idera.SQLdm.DesktopClient.Properties.Resources.ToolbarPause;
            buttonTool4.SharedProps.AppearancesSmall.Appearance = appearance2;
            buttonTool4.SharedProps.Caption = "Pause";
            this.toolbarsManager.Tools.AddRange(new Infragistics.Win.UltraWinToolbars.ToolBase[] {
            buttonTool3,
            buttonTool4});
            // 
            // viewPanel
            // 
            this.viewPanel.Cursor = System.Windows.Forms.Cursors.Default;
            this.viewPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.viewPanel.Location = new System.Drawing.Point(4, 147);
            this.viewPanel.Name = "viewPanel";
            this.viewPanel.Size = new System.Drawing.Size(725, 353);
            this.viewPanel.TabIndex = 0;
            // 
            // _SqlServerForm_Toolbars_Dock_Area_Left
            // 
            this._SqlServerForm_Toolbars_Dock_Area_Left.AccessibleRole = System.Windows.Forms.AccessibleRole.Grouping;
            this._SqlServerForm_Toolbars_Dock_Area_Left.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(191)))), ((int)(((byte)(219)))), ((int)(((byte)(255)))));
            this._SqlServerForm_Toolbars_Dock_Area_Left.DockedPosition = Infragistics.Win.UltraWinToolbars.DockedPosition.Left;
            this._SqlServerForm_Toolbars_Dock_Area_Left.ForeColor = System.Drawing.SystemColors.ControlText;
            this._SqlServerForm_Toolbars_Dock_Area_Left.InitialResizeAreaExtent = 4;
            this._SqlServerForm_Toolbars_Dock_Area_Left.Location = new System.Drawing.Point(0, 147);
            this._SqlServerForm_Toolbars_Dock_Area_Left.Name = "_SqlServerForm_Toolbars_Dock_Area_Left";
            this._SqlServerForm_Toolbars_Dock_Area_Left.Size = new System.Drawing.Size(4, 353);
            this._SqlServerForm_Toolbars_Dock_Area_Left.ToolbarsManager = this.toolbarsManager;
            // 
            // _SqlServerForm_Toolbars_Dock_Area_Right
            // 
            this._SqlServerForm_Toolbars_Dock_Area_Right.AccessibleRole = System.Windows.Forms.AccessibleRole.Grouping;
            this._SqlServerForm_Toolbars_Dock_Area_Right.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(191)))), ((int)(((byte)(219)))), ((int)(((byte)(255)))));
            this._SqlServerForm_Toolbars_Dock_Area_Right.DockedPosition = Infragistics.Win.UltraWinToolbars.DockedPosition.Right;
            this._SqlServerForm_Toolbars_Dock_Area_Right.ForeColor = System.Drawing.SystemColors.ControlText;
            this._SqlServerForm_Toolbars_Dock_Area_Right.InitialResizeAreaExtent = 4;
            this._SqlServerForm_Toolbars_Dock_Area_Right.Location = new System.Drawing.Point(729, 147);
            this._SqlServerForm_Toolbars_Dock_Area_Right.Name = "_SqlServerForm_Toolbars_Dock_Area_Right";
            this._SqlServerForm_Toolbars_Dock_Area_Right.Size = new System.Drawing.Size(4, 353);
            this._SqlServerForm_Toolbars_Dock_Area_Right.ToolbarsManager = this.toolbarsManager;
            // 
            // _SqlServerForm_Toolbars_Dock_Area_Top
            // 
            this._SqlServerForm_Toolbars_Dock_Area_Top.AccessibleRole = System.Windows.Forms.AccessibleRole.Grouping;
            this._SqlServerForm_Toolbars_Dock_Area_Top.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(191)))), ((int)(((byte)(219)))), ((int)(((byte)(255)))));
            this._SqlServerForm_Toolbars_Dock_Area_Top.DockedPosition = Infragistics.Win.UltraWinToolbars.DockedPosition.Top;
            this._SqlServerForm_Toolbars_Dock_Area_Top.ForeColor = System.Drawing.SystemColors.ControlText;
            this._SqlServerForm_Toolbars_Dock_Area_Top.Location = new System.Drawing.Point(0, 0);
            this._SqlServerForm_Toolbars_Dock_Area_Top.Name = "_SqlServerForm_Toolbars_Dock_Area_Top";
            this._SqlServerForm_Toolbars_Dock_Area_Top.Size = new System.Drawing.Size(733, 147);
            this._SqlServerForm_Toolbars_Dock_Area_Top.ToolbarsManager = this.toolbarsManager;
            // 
            // _SqlServerForm_Toolbars_Dock_Area_Bottom
            // 
            this._SqlServerForm_Toolbars_Dock_Area_Bottom.AccessibleRole = System.Windows.Forms.AccessibleRole.Grouping;
            this._SqlServerForm_Toolbars_Dock_Area_Bottom.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(191)))), ((int)(((byte)(219)))), ((int)(((byte)(255)))));
            this._SqlServerForm_Toolbars_Dock_Area_Bottom.DockedPosition = Infragistics.Win.UltraWinToolbars.DockedPosition.Bottom;
            this._SqlServerForm_Toolbars_Dock_Area_Bottom.ForeColor = System.Drawing.SystemColors.ControlText;
            this._SqlServerForm_Toolbars_Dock_Area_Bottom.InitialResizeAreaExtent = 4;
            this._SqlServerForm_Toolbars_Dock_Area_Bottom.Location = new System.Drawing.Point(0, 500);
            this._SqlServerForm_Toolbars_Dock_Area_Bottom.Name = "_SqlServerForm_Toolbars_Dock_Area_Bottom";
            this._SqlServerForm_Toolbars_Dock_Area_Bottom.Size = new System.Drawing.Size(733, 4);
            this._SqlServerForm_Toolbars_Dock_Area_Bottom.ToolbarsManager = this.toolbarsManager;
            // 
            // SqlServerForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(733, 504);
            this.Controls.Add(this.viewPanel);
            this.Controls.Add(this._SqlServerForm_Toolbars_Dock_Area_Left);
            this.Controls.Add(this._SqlServerForm_Toolbars_Dock_Area_Right);
            this.Controls.Add(this._SqlServerForm_Toolbars_Dock_Area_Top);
            this.Controls.Add(this._SqlServerForm_Toolbars_Dock_Area_Bottom);
            this.Name = "SqlServerForm";
            this.Text = "* Instance Name *";
            ((System.ComponentModel.ISupportInitialize)(this.toolbarsManager)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private Infragistics.Win.UltraWinToolbars.UltraToolbarsManager toolbarsManager;
        private System.Windows.Forms.Panel viewPanel;
        private Infragistics.Win.UltraWinToolbars.UltraToolbarsDockArea _SqlServerForm_Toolbars_Dock_Area_Left;
        private Infragistics.Win.UltraWinToolbars.UltraToolbarsDockArea _SqlServerForm_Toolbars_Dock_Area_Right;
        private Infragistics.Win.UltraWinToolbars.UltraToolbarsDockArea _SqlServerForm_Toolbars_Dock_Area_Top;
        private Infragistics.Win.UltraWinToolbars.UltraToolbarsDockArea _SqlServerForm_Toolbars_Dock_Area_Bottom;
    }
}