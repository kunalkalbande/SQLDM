using System;

namespace Idera.SQLdm.DesktopClient.Controls
{
    partial class HistoryBrowserControl
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
            this.components = new System.ComponentModel.Container();
            Infragistics.Win.UltraWinToolbars.StateButtonTool stateButtonTool1 = new Infragistics.Win.UltraWinToolbars.StateButtonTool("showMaximizedButton", "");
            Infragistics.Win.UltraWinToolbars.StateButtonTool stateButtonTool2 = new Infragistics.Win.UltraWinToolbars.StateButtonTool("showMinimizedButton", "");
            Infragistics.Win.UltraWinToolbars.StateButtonTool stateButtonTool3 = new Infragistics.Win.UltraWinToolbars.StateButtonTool("closeButton", "");
            Infragistics.Win.UltraWinToolbars.PopupMenuTool popupMenuTool1 = new Infragistics.Win.UltraWinToolbars.PopupMenuTool("optionsContextMenu");
            Infragistics.Win.UltraWinToolbars.StateButtonTool stateButtonTool4 = new Infragistics.Win.UltraWinToolbars.StateButtonTool("showMaximizedButton", "");
            Infragistics.Win.UltraWinToolbars.StateButtonTool stateButtonTool5 = new Infragistics.Win.UltraWinToolbars.StateButtonTool("showMinimizedButton", "");
            Infragistics.Win.UltraWinToolbars.StateButtonTool stateButtonTool6 = new Infragistics.Win.UltraWinToolbars.StateButtonTool("closeButton", "");
            this.historyTreeImages = new System.Windows.Forms.ImageList(this.components);
            this.borderPanel = new Idera.SQLdm.DesktopClient.Controls.GradientPanel();
            this.historyBrowserPane = new Idera.SQLdm.DesktopClient.Controls.HistoryBrowserPane();
            this.historyBrowserHotTrackLabel = new Idera.SQLdm.DesktopClient.Controls.HistoryBrowserHotTrackLabel();
            this.headerStrip = new Idera.SQLdm.DesktopClient.Controls.HeaderStrip(true);
            this.titleLabel = new System.Windows.Forms.ToolStripLabel();
            this.closeButton = new System.Windows.Forms.ToolStripButton();
            this.toggleMinimizedButton = new System.Windows.Forms.ToolStripButton();
            this.toolbarsManager = new Idera.SQLdm.DesktopClient.Controls.ContextMenuManager(this.components);
            this._HistoryBrowserControl_Toolbars_Dock_Area_Left = new Infragistics.Win.UltraWinToolbars.UltraToolbarsDockArea();
            this._HistoryBrowserControl_Toolbars_Dock_Area_Right = new Infragistics.Win.UltraWinToolbars.UltraToolbarsDockArea();
            this._HistoryBrowserControl_Toolbars_Dock_Area_Top = new Infragistics.Win.UltraWinToolbars.UltraToolbarsDockArea();
            this._HistoryBrowserControl_Toolbars_Dock_Area_Bottom = new Infragistics.Win.UltraWinToolbars.UltraToolbarsDockArea();
            this.borderPanel.SuspendLayout();
            this.headerStrip.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.toolbarsManager)).BeginInit();
            this.SuspendLayout();
            // 
            // historyTreeImages
            // 
            this.historyTreeImages.ColorDepth = System.Windows.Forms.ColorDepth.Depth32Bit;
            this.historyTreeImages.ImageSize = new System.Drawing.Size(16, 16);
            this.historyTreeImages.TransparentColor = System.Drawing.Color.Transparent;
            // 
            // borderPanel
            // 
            this.borderPanel.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(235)))), ((int)(((byte)(235)))), ((int)(((byte)(235)))));
            this.borderPanel.BackColor2 = System.Drawing.Color.White;
            this.borderPanel.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(140)))), ((int)(((byte)(140)))), ((int)(((byte)(140)))));
            this.borderPanel.Controls.Add(this.historyBrowserPane);
            this.borderPanel.Controls.Add(this.historyBrowserHotTrackLabel);
            this.borderPanel.Controls.Add(this.headerStrip);
            this.borderPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.borderPanel.FillStyle = Idera.SQLdm.DesktopClient.Controls.GradientPanelFillStyle.Solid;
            this.borderPanel.Location = new System.Drawing.Point(0, 0);
            this.borderPanel.Name = "borderPanel";
            this.borderPanel.Padding = new System.Windows.Forms.Padding(1);
            this.borderPanel.Size = new System.Drawing.Size(238, 510);
            this.borderPanel.TabIndex = 0;
            // 
            // historyBrowserPane
            // 
            this.historyBrowserPane.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(235)))), ((int)(((byte)(235)))), ((int)(((byte)(235)))));
            this.historyBrowserPane.Dock = System.Windows.Forms.DockStyle.Fill;
            this.historyBrowserPane.Location = new System.Drawing.Point(1, 26);
            this.historyBrowserPane.Name = "historyBrowserPane";
            this.historyBrowserPane.Size = new System.Drawing.Size(236, 483);
            this.historyBrowserPane.TabIndex = 12;
            this.historyBrowserPane.HistoricalSnapshotSelected += new System.EventHandler<Idera.SQLdm.DesktopClient.Controls.HistoricalSnapshotSelectedEventArgs>(this.historyBrowserPane_HistoricalSnapshotSelected);
            this.historyBrowserPane.HistoricalCustomRangeSelected += new System.EventHandler<EventArgs>(this.historyBrowserPane_HistoricalCustomRangeSelected);
            // 
            // historyBrowserHotTrackLabel
            // 
            this.toolbarsManager.SetContextMenuUltra(this.historyBrowserHotTrackLabel, "optionsContextMenu");
            this.historyBrowserHotTrackLabel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.historyBrowserHotTrackLabel.Font = new System.Drawing.Font("Arial", 12F, System.Drawing.FontStyle.Bold);
            this.historyBrowserHotTrackLabel.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(75)))), ((int)(((byte)(75)))), ((int)(((byte)(75)))));
            this.historyBrowserHotTrackLabel.HotTrackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(231)))), ((int)(((byte)(162)))));
            this.historyBrowserHotTrackLabel.Location = new System.Drawing.Point(1, 26);
            this.historyBrowserHotTrackLabel.MouseDownColor = System.Drawing.Color.FromArgb(((int)(((byte)(251)))), ((int)(((byte)(140)))), ((int)(((byte)(60)))));
            this.historyBrowserHotTrackLabel.Name = "historyBrowserHotTrackLabel";
            this.historyBrowserHotTrackLabel.Size = new System.Drawing.Size(236, 483);
            this.historyBrowserHotTrackLabel.TabIndex = 11;
            this.historyBrowserHotTrackLabel.Text = "History Browser";
            this.historyBrowserHotTrackLabel.MouseClick += new System.Windows.Forms.MouseEventHandler(this.historyBrowserHotTrackLabel_MouseClick);
            // 
            // headerStrip
            // 
            this.headerStrip.AutoSize = false;
            this.toolbarsManager.SetContextMenuUltra(this.headerStrip, "optionsContextMenu");
            this.headerStrip.Font = new System.Drawing.Font("Arial", 12F, System.Drawing.FontStyle.Bold);
            this.headerStrip.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(21)))), ((int)(((byte)(66)))), ((int)(((byte)(139)))));
            this.headerStrip.GripStyle = System.Windows.Forms.ToolStripGripStyle.Hidden;
            this.headerStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.titleLabel,
            this.closeButton,
            this.toggleMinimizedButton});
            this.headerStrip.Location = new System.Drawing.Point(1, 1);
            this.headerStrip.Name = "headerStrip";
            this.headerStrip.Padding = new System.Windows.Forms.Padding(0, 2, 0, 0);
            this.headerStrip.Size = new System.Drawing.Size(236, 25);
            this.headerStrip.TabIndex = 1;
            // 
            // titleLabel
            // 
            this.titleLabel.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(75)))), ((int)(((byte)(75)))), ((int)(((byte)(75)))));
            this.titleLabel.Name = "titleLabel";
            this.titleLabel.Size = new System.Drawing.Size(133, 20);
            this.titleLabel.Text = "History Browser";
            // 
            // closeButton
            // 
            this.closeButton.Alignment = System.Windows.Forms.ToolStripItemAlignment.Right;
            this.closeButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.closeButton.Image = global::Idera.SQLdm.DesktopClient.Properties.Resources.Hide;
            this.closeButton.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None;
            this.closeButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.closeButton.Name = "closeButton";
            this.closeButton.Size = new System.Drawing.Size(23, 20);
            this.closeButton.ToolTipText = "Close History Browser";
            this.closeButton.Click += new System.EventHandler(this.closeButton_Click);
            // 
            // toggleMinimizedButton
            // 
            this.toggleMinimizedButton.Alignment = System.Windows.Forms.ToolStripItemAlignment.Right;
            this.toggleMinimizedButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.toggleMinimizedButton.Image = global::Idera.SQLdm.DesktopClient.Properties.Resources.RightArrows;
            this.toggleMinimizedButton.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None;
            this.toggleMinimizedButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toggleMinimizedButton.Name = "toggleMinimizedButton";
            this.toggleMinimizedButton.Size = new System.Drawing.Size(23, 20);
            this.toggleMinimizedButton.ToolTipText = "Minimize History Browser";
            this.toggleMinimizedButton.Visible = false;
            this.toggleMinimizedButton.Click += new System.EventHandler(this.toggleMinimizedButton_Click);
            // 
            // toolbarsManager
            // 
            this.toolbarsManager.DesignerFlags = 0;
            this.toolbarsManager.DockWithinContainer = this;
            this.toolbarsManager.ShowFullMenusDelay = 500;
            stateButtonTool1.MenuDisplayStyle = Infragistics.Win.UltraWinToolbars.StateButtonMenuDisplayStyle.DisplayCheckmark;
            stateButtonTool1.SharedPropsInternal.Caption = "Normal";
            stateButtonTool2.MenuDisplayStyle = Infragistics.Win.UltraWinToolbars.StateButtonMenuDisplayStyle.DisplayCheckmark;
            stateButtonTool2.SharedPropsInternal.Caption = "Minimized";
            stateButtonTool3.SharedPropsInternal.Caption = "Close";
            popupMenuTool1.SharedPropsInternal.Caption = "optionsContextMenu";
            stateButtonTool4.MenuDisplayStyle = Infragistics.Win.UltraWinToolbars.StateButtonMenuDisplayStyle.DisplayCheckmark;
            stateButtonTool5.MenuDisplayStyle = Infragistics.Win.UltraWinToolbars.StateButtonMenuDisplayStyle.DisplayCheckmark;
            popupMenuTool1.Tools.AddRange(new Infragistics.Win.UltraWinToolbars.ToolBase[] {
            stateButtonTool4,
            stateButtonTool5,
            stateButtonTool6});
            this.toolbarsManager.Tools.AddRange(new Infragistics.Win.UltraWinToolbars.ToolBase[] {
            stateButtonTool1,
            stateButtonTool2,
            stateButtonTool3,
            popupMenuTool1});
            this.toolbarsManager.BeforeToolDropdown += new Infragistics.Win.UltraWinToolbars.BeforeToolDropdownEventHandler(this.toolbarsManager_BeforeToolDropdown);
            this.toolbarsManager.ToolClick += new Infragistics.Win.UltraWinToolbars.ToolClickEventHandler(this.toolbarsManager_ToolClick);
            // 
            // _HistoryBrowserControl_Toolbars_Dock_Area_Left
            // 
            this._HistoryBrowserControl_Toolbars_Dock_Area_Left.AccessibleRole = System.Windows.Forms.AccessibleRole.Grouping;
            this._HistoryBrowserControl_Toolbars_Dock_Area_Left.BackColor = System.Drawing.SystemColors.Control;
            this._HistoryBrowserControl_Toolbars_Dock_Area_Left.DockedPosition = Infragistics.Win.UltraWinToolbars.DockedPosition.Left;
            this._HistoryBrowserControl_Toolbars_Dock_Area_Left.ForeColor = System.Drawing.SystemColors.ControlText;
            this._HistoryBrowserControl_Toolbars_Dock_Area_Left.Location = new System.Drawing.Point(0, 0);
            this._HistoryBrowserControl_Toolbars_Dock_Area_Left.Name = "_HistoryBrowserControl_Toolbars_Dock_Area_Left";
            this._HistoryBrowserControl_Toolbars_Dock_Area_Left.Size = new System.Drawing.Size(0, 510);
            this._HistoryBrowserControl_Toolbars_Dock_Area_Left.ToolbarsManager = this.toolbarsManager;
            // 
            // _HistoryBrowserControl_Toolbars_Dock_Area_Right
            // 
            this._HistoryBrowserControl_Toolbars_Dock_Area_Right.AccessibleRole = System.Windows.Forms.AccessibleRole.Grouping;
            this._HistoryBrowserControl_Toolbars_Dock_Area_Right.BackColor = System.Drawing.SystemColors.Control;
            this._HistoryBrowserControl_Toolbars_Dock_Area_Right.DockedPosition = Infragistics.Win.UltraWinToolbars.DockedPosition.Right;
            this._HistoryBrowserControl_Toolbars_Dock_Area_Right.ForeColor = System.Drawing.SystemColors.ControlText;
            this._HistoryBrowserControl_Toolbars_Dock_Area_Right.Location = new System.Drawing.Point(238, 0);
            this._HistoryBrowserControl_Toolbars_Dock_Area_Right.Name = "_HistoryBrowserControl_Toolbars_Dock_Area_Right";
            this._HistoryBrowserControl_Toolbars_Dock_Area_Right.Size = new System.Drawing.Size(0, 510);
            this._HistoryBrowserControl_Toolbars_Dock_Area_Right.ToolbarsManager = this.toolbarsManager;
            // 
            // _HistoryBrowserControl_Toolbars_Dock_Area_Top
            // 
            this._HistoryBrowserControl_Toolbars_Dock_Area_Top.AccessibleRole = System.Windows.Forms.AccessibleRole.Grouping;
            this._HistoryBrowserControl_Toolbars_Dock_Area_Top.BackColor = System.Drawing.SystemColors.Control;
            this._HistoryBrowserControl_Toolbars_Dock_Area_Top.DockedPosition = Infragistics.Win.UltraWinToolbars.DockedPosition.Top;
            this._HistoryBrowserControl_Toolbars_Dock_Area_Top.ForeColor = System.Drawing.SystemColors.ControlText;
            this._HistoryBrowserControl_Toolbars_Dock_Area_Top.Location = new System.Drawing.Point(0, 0);
            this._HistoryBrowserControl_Toolbars_Dock_Area_Top.Name = "_HistoryBrowserControl_Toolbars_Dock_Area_Top";
            this._HistoryBrowserControl_Toolbars_Dock_Area_Top.Size = new System.Drawing.Size(238, 0);
            this._HistoryBrowserControl_Toolbars_Dock_Area_Top.ToolbarsManager = this.toolbarsManager;
            // 
            // _HistoryBrowserControl_Toolbars_Dock_Area_Bottom
            // 
            this._HistoryBrowserControl_Toolbars_Dock_Area_Bottom.AccessibleRole = System.Windows.Forms.AccessibleRole.Grouping;
            this._HistoryBrowserControl_Toolbars_Dock_Area_Bottom.BackColor = System.Drawing.SystemColors.Control;
            this._HistoryBrowserControl_Toolbars_Dock_Area_Bottom.DockedPosition = Infragistics.Win.UltraWinToolbars.DockedPosition.Bottom;
            this._HistoryBrowserControl_Toolbars_Dock_Area_Bottom.ForeColor = System.Drawing.SystemColors.ControlText;
            this._HistoryBrowserControl_Toolbars_Dock_Area_Bottom.Location = new System.Drawing.Point(0, 510);
            this._HistoryBrowserControl_Toolbars_Dock_Area_Bottom.Name = "_HistoryBrowserControl_Toolbars_Dock_Area_Bottom";
            this._HistoryBrowserControl_Toolbars_Dock_Area_Bottom.Size = new System.Drawing.Size(238, 0);
            this._HistoryBrowserControl_Toolbars_Dock_Area_Bottom.ToolbarsManager = this.toolbarsManager;
            // 
            // HistoryBrowserControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.borderPanel);
            this.Controls.Add(this._HistoryBrowserControl_Toolbars_Dock_Area_Left);
            this.Controls.Add(this._HistoryBrowserControl_Toolbars_Dock_Area_Right);
            this.Controls.Add(this._HistoryBrowserControl_Toolbars_Dock_Area_Top);
            this.Controls.Add(this._HistoryBrowserControl_Toolbars_Dock_Area_Bottom);
            this.Name = "HistoryBrowserControl";
            this.Size = new System.Drawing.Size(238, 510);
            this.borderPanel.ResumeLayout(false);
            this.headerStrip.ResumeLayout(false);
            this.headerStrip.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.toolbarsManager)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private GradientPanel borderPanel;
        private HeaderStrip headerStrip;
        private System.Windows.Forms.ToolStripLabel titleLabel;
        private System.Windows.Forms.ToolStripButton closeButton;
        private System.Windows.Forms.ImageList historyTreeImages;
        private System.Windows.Forms.ToolStripButton toggleMinimizedButton;
        private HistoryBrowserHotTrackLabel historyBrowserHotTrackLabel;
        private HistoryBrowserPane historyBrowserPane;
        private Idera.SQLdm.DesktopClient.Controls.ContextMenuManager toolbarsManager;
        private Infragistics.Win.UltraWinToolbars.UltraToolbarsDockArea _HistoryBrowserControl_Toolbars_Dock_Area_Left;
        private Infragistics.Win.UltraWinToolbars.UltraToolbarsDockArea _HistoryBrowserControl_Toolbars_Dock_Area_Right;
        private Infragistics.Win.UltraWinToolbars.UltraToolbarsDockArea _HistoryBrowserControl_Toolbars_Dock_Area_Top;
        private Infragistics.Win.UltraWinToolbars.UltraToolbarsDockArea _HistoryBrowserControl_Toolbars_Dock_Area_Bottom;

    }
}
