using Idera.SQLdm.DesktopClient.Controls;
namespace Idera.SQLdm.DesktopClient.Dialogs
{
    partial class SqlViewerDialog
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
            Infragistics.Win.UltraWinToolbars.UltraToolbar ultraToolbar1 = new Infragistics.Win.UltraWinToolbars.UltraToolbar("viewer");
            Infragistics.Win.UltraWinToolbars.ButtonTool buttonTool2 = new Controls.CustomControls.CustomButtonTool("PreviousButton");
            Infragistics.Win.UltraWinToolbars.ButtonTool buttonTool1 = new Controls.CustomControls.CustomButtonTool("NextButton");
            Infragistics.Win.UltraWinToolbars.ButtonTool buttonTool3 = new Controls.CustomControls.CustomButtonTool("NextButton");
            Infragistics.Win.Appearance appearance12 = new Infragistics.Win.Appearance();
            Infragistics.Win.UltraWinToolbars.ButtonTool buttonTool4 = new Controls.CustomControls.CustomButtonTool("PreviousButton");
            Infragistics.Win.Appearance appearance11 = new Infragistics.Win.Appearance();
            Infragistics.Win.UltraWinStatusBar.UltraStatusPanel ultraStatusPanel1 = new Infragistics.Win.UltraWinStatusBar.UltraStatusPanel();
            Infragistics.Win.UltraWinStatusBar.UltraStatusPanel ultraStatusPanel2 = new Infragistics.Win.UltraWinStatusBar.UltraStatusPanel();
            ActiproSoftware.SyntaxEditor.Document document1 = new ActiproSoftware.SyntaxEditor.Document();
            ActiproSoftware.SyntaxEditor.VisualStudio2005SyntaxEditorRenderer visualStudio2005SyntaxEditorRenderer1 = new ActiproSoftware.SyntaxEditor.VisualStudio2005SyntaxEditorRenderer();
            this.panel1 = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomPanel ();
            this.copyButton = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomButton();
            this.closeButton = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomButton();
            this._SqlViewerDialog_Toolbars_Dock_Area_Left = new Infragistics.Win.UltraWinToolbars.UltraToolbarsDockArea();
            this.toolbarManager = new Infragistics.Win.UltraWinToolbars.UltraToolbarsManager(this.components);
            this._SqlViewerDialog_Toolbars_Dock_Area_Right = new Infragistics.Win.UltraWinToolbars.UltraToolbarsDockArea();
            this._SqlViewerDialog_Toolbars_Dock_Area_Top = new Infragistics.Win.UltraWinToolbars.UltraToolbarsDockArea();
            this._SqlViewerDialog_Toolbars_Dock_Area_Bottom = new Infragistics.Win.UltraWinToolbars.UltraToolbarsDockArea();
            this.statusBar = new Infragistics.Win.UltraWinStatusBar.UltraStatusBar();
            this.gradientPanel1 = new GradientPanel();
            this.splitContainer1 = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomSplitContainer();
            this.syntaxEditor1 = new ActiproSoftware.SyntaxEditor.SyntaxEditor();
            this.lvRecommendations = new Infragistics.Win.UltraWinListView.UltraListView();
            this.panel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.toolbarManager)).BeginInit();
            this.gradientPanel1.SuspendLayout();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.lvRecommendations)).BeginInit();
            this.SuspendLayout();
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.copyButton);
            this.panel1.Controls.Add(this.closeButton);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.panel1.Location = new System.Drawing.Point(5, 415);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(565, 38);
            this.panel1.TabIndex = 2;
            // 
            // copyButton
            // 
            this.copyButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.copyButton.Location = new System.Drawing.Point(3, 11);
            this.copyButton.Name = "copyButton";
            this.copyButton.Size = new System.Drawing.Size(75, 23);
            this.copyButton.TabIndex = 1;
            this.copyButton.Text = "Copy";
            this.copyButton.UseVisualStyleBackColor = true;
            this.copyButton.Click += new System.EventHandler(this.copyButton_Click);
            // 
            // closeButton
            // 
            this.closeButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.closeButton.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.closeButton.Location = new System.Drawing.Point(487, 11);
            this.closeButton.Name = "closeButton";
            this.closeButton.Size = new System.Drawing.Size(75, 23);
            this.closeButton.TabIndex = 0;
            this.closeButton.Text = "Close";
            this.closeButton.UseVisualStyleBackColor = true;
            this.closeButton.Click += new System.EventHandler(this.closeButton_Click);
            // 
            // _SqlViewerDialog_Toolbars_Dock_Area_Left
            // 
            this._SqlViewerDialog_Toolbars_Dock_Area_Left.AccessibleRole = System.Windows.Forms.AccessibleRole.Grouping;
            this._SqlViewerDialog_Toolbars_Dock_Area_Left.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(158)))), ((int)(((byte)(190)))), ((int)(((byte)(245)))));
            this._SqlViewerDialog_Toolbars_Dock_Area_Left.DockedPosition = Infragistics.Win.UltraWinToolbars.DockedPosition.Left;
            this._SqlViewerDialog_Toolbars_Dock_Area_Left.ForeColor = System.Drawing.SystemColors.ControlText;
            this._SqlViewerDialog_Toolbars_Dock_Area_Left.Location = new System.Drawing.Point(5, 33);
            this._SqlViewerDialog_Toolbars_Dock_Area_Left.Name = "_SqlViewerDialog_Toolbars_Dock_Area_Left";
            this._SqlViewerDialog_Toolbars_Dock_Area_Left.Size = new System.Drawing.Size(0, 420);
            this._SqlViewerDialog_Toolbars_Dock_Area_Left.ToolbarsManager = this.toolbarManager;
            // 
            // toolbarManager
            // 
            this.toolbarManager.DesignerFlags = 1;
            this.toolbarManager.DockWithinContainer = this;
            this.toolbarManager.DockWithinContainerBaseType = typeof(System.Windows.Forms.Form);
            this.toolbarManager.FormDisplayStyle = Infragistics.Win.UltraWinToolbars.FormDisplayStyle.Standard;
            this.toolbarManager.ShowQuickCustomizeButton = false;
            this.toolbarManager.Style = Infragistics.Win.UltraWinToolbars.ToolbarStyle.Office2003;
            ultraToolbar1.DockedColumn = 0;
            ultraToolbar1.DockedRow = 0;
            ultraToolbar1.NonInheritedTools.AddRange(new Infragistics.Win.UltraWinToolbars.ToolBase[] {
            buttonTool2,
            buttonTool1});
            ultraToolbar1.Text = "viewer";
            this.toolbarManager.Toolbars.AddRange(new Infragistics.Win.UltraWinToolbars.UltraToolbar[] {
            ultraToolbar1});
            this.toolbarManager.ToolbarSettings.AllowCustomize = Infragistics.Win.DefaultableBoolean.False;
            this.toolbarManager.ToolbarSettings.FillEntireRow = Infragistics.Win.DefaultableBoolean.True;
            this.toolbarManager.ToolbarSettings.GrabHandleStyle = Infragistics.Win.UltraWinToolbars.GrabHandleStyle.None;
            this.toolbarManager.ToolbarSettings.UseLargeImages = Infragistics.Win.DefaultableBoolean.False;
            appearance12.Image = global::Idera.SQLdm.DesktopClient.Properties.Resources.GotoNextError;
            buttonTool3.SharedProps.AppearancesSmall.Appearance = appearance12;
            buttonTool3.SharedProps.Caption = "Next";
            appearance11.Image = global::Idera.SQLdm.DesktopClient.Properties.Resources.GotoPreviousError;
            buttonTool4.SharedProps.AppearancesSmall.Appearance = appearance11;
            buttonTool4.SharedProps.Caption = "Previous";
            this.toolbarManager.Tools.AddRange(new Infragistics.Win.UltraWinToolbars.ToolBase[] {
            buttonTool3,
            buttonTool4});
            this.toolbarManager.ToolClick += new Infragistics.Win.UltraWinToolbars.ToolClickEventHandler(this.toolbarManager_ToolClick);
            // 
            // _SqlViewerDialog_Toolbars_Dock_Area_Right
            // 
            this._SqlViewerDialog_Toolbars_Dock_Area_Right.AccessibleRole = System.Windows.Forms.AccessibleRole.Grouping;
            this._SqlViewerDialog_Toolbars_Dock_Area_Right.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(158)))), ((int)(((byte)(190)))), ((int)(((byte)(245)))));
            this._SqlViewerDialog_Toolbars_Dock_Area_Right.DockedPosition = Infragistics.Win.UltraWinToolbars.DockedPosition.Right;
            this._SqlViewerDialog_Toolbars_Dock_Area_Right.ForeColor = System.Drawing.SystemColors.ControlText;
            this._SqlViewerDialog_Toolbars_Dock_Area_Right.Location = new System.Drawing.Point(570, 33);
            this._SqlViewerDialog_Toolbars_Dock_Area_Right.Name = "_SqlViewerDialog_Toolbars_Dock_Area_Right";
            this._SqlViewerDialog_Toolbars_Dock_Area_Right.Size = new System.Drawing.Size(0, 420);
            this._SqlViewerDialog_Toolbars_Dock_Area_Right.ToolbarsManager = this.toolbarManager;
            // 
            // _SqlViewerDialog_Toolbars_Dock_Area_Top
            // 
            this._SqlViewerDialog_Toolbars_Dock_Area_Top.AccessibleRole = System.Windows.Forms.AccessibleRole.Grouping;
            this._SqlViewerDialog_Toolbars_Dock_Area_Top.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(158)))), ((int)(((byte)(190)))), ((int)(((byte)(245)))));
            this._SqlViewerDialog_Toolbars_Dock_Area_Top.DockedPosition = Infragistics.Win.UltraWinToolbars.DockedPosition.Top;
            this._SqlViewerDialog_Toolbars_Dock_Area_Top.ForeColor = System.Drawing.SystemColors.ControlText;
            this._SqlViewerDialog_Toolbars_Dock_Area_Top.Location = new System.Drawing.Point(5, 5);
            this._SqlViewerDialog_Toolbars_Dock_Area_Top.Name = "_SqlViewerDialog_Toolbars_Dock_Area_Top";
            this._SqlViewerDialog_Toolbars_Dock_Area_Top.Size = new System.Drawing.Size(565, 28);
            this._SqlViewerDialog_Toolbars_Dock_Area_Top.ToolbarsManager = this.toolbarManager;
            // 
            // _SqlViewerDialog_Toolbars_Dock_Area_Bottom
            // 
            this._SqlViewerDialog_Toolbars_Dock_Area_Bottom.AccessibleRole = System.Windows.Forms.AccessibleRole.Grouping;
            this._SqlViewerDialog_Toolbars_Dock_Area_Bottom.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(158)))), ((int)(((byte)(190)))), ((int)(((byte)(245)))));
            this._SqlViewerDialog_Toolbars_Dock_Area_Bottom.DockedPosition = Infragistics.Win.UltraWinToolbars.DockedPosition.Bottom;
            this._SqlViewerDialog_Toolbars_Dock_Area_Bottom.ForeColor = System.Drawing.SystemColors.ControlText;
            this._SqlViewerDialog_Toolbars_Dock_Area_Bottom.Location = new System.Drawing.Point(5, 453);
            this._SqlViewerDialog_Toolbars_Dock_Area_Bottom.Name = "_SqlViewerDialog_Toolbars_Dock_Area_Bottom";
            this._SqlViewerDialog_Toolbars_Dock_Area_Bottom.Size = new System.Drawing.Size(565, 0);
            this._SqlViewerDialog_Toolbars_Dock_Area_Bottom.ToolbarsManager = this.toolbarManager;
            // 
            // statusBar
            // 
            this.statusBar.Location = new System.Drawing.Point(5, 453);
            this.statusBar.Name = "statusBar";
            ultraStatusPanel1.BorderStyle = Infragistics.Win.UIElementBorderStyle.None;
            ultraStatusPanel1.SizingMode = Infragistics.Win.UltraWinStatusBar.PanelSizingMode.Spring;
            ultraStatusPanel2.BorderStyle = Infragistics.Win.UIElementBorderStyle.None;
            ultraStatusPanel2.Key = "CursorPos";
            ultraStatusPanel2.SizingMode = Infragistics.Win.UltraWinStatusBar.PanelSizingMode.Automatic;
            this.statusBar.Panels.AddRange(new Infragistics.Win.UltraWinStatusBar.UltraStatusPanel[] {
            ultraStatusPanel1,
            ultraStatusPanel2});
            this.statusBar.Size = new System.Drawing.Size(565, 23);
            this.statusBar.TabIndex = 7;
            this.statusBar.Text = "ultraStatusBar1";
            this.statusBar.UseAppStyling = false;
            this.statusBar.UseFlatMode = Infragistics.Win.DefaultableBoolean.True;
            this.statusBar.UseOsThemes = Infragistics.Win.DefaultableBoolean.False;
            this.statusBar.ViewStyle = Infragistics.Win.UltraWinStatusBar.ViewStyle.Standard;
            // 
            // gradientPanel1
            // 
            this.gradientPanel1.BackColor = System.Drawing.Color.White;
            this.gradientPanel1.BackColor2 = System.Drawing.Color.White;
            this.gradientPanel1.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(207)))), ((int)(((byte)(207)))), ((int)(((byte)(207)))));
            this.gradientPanel1.Controls.Add(this.splitContainer1);
            this.gradientPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.gradientPanel1.Location = new System.Drawing.Point(5, 33);
            this.gradientPanel1.Name = "gradientPanel1";
            this.gradientPanel1.Padding = new System.Windows.Forms.Padding(3);
            this.gradientPanel1.Size = new System.Drawing.Size(565, 382);
            this.gradientPanel1.TabIndex = 1;
            // 
            // splitContainer1
            // 
            this.splitContainer1.BackColor = System.Drawing.Color.Transparent;
            this.splitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer1.Location = new System.Drawing.Point(3, 3);
            this.splitContainer1.Margin = new System.Windows.Forms.Padding(0);
            this.splitContainer1.Name = "splitContainer1";
            this.splitContainer1.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // splitContainer1.Panel1
            // 
            this.splitContainer1.Panel1.Controls.Add(this.syntaxEditor1);
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.Controls.Add(this.lvRecommendations);
            this.splitContainer1.Size = new System.Drawing.Size(559, 376);
            this.splitContainer1.SplitterDistance = 251;
            this.splitContainer1.TabIndex = 1;
            // 
            // syntaxEditor1
            // 
            this.syntaxEditor1.Dock = System.Windows.Forms.DockStyle.Fill;
            document1.ReadOnly = true;
            document1.Text = "Select * from Table_1";
            this.syntaxEditor1.Document = document1;
            this.syntaxEditor1.Location = new System.Drawing.Point(0, 0);
            this.syntaxEditor1.Name = "syntaxEditor1";
            visualStudio2005SyntaxEditorRenderer1.ResetAllPropertiesOnSystemColorChange = false;
            this.syntaxEditor1.Renderer = visualStudio2005SyntaxEditorRenderer1;
            this.syntaxEditor1.Size = new System.Drawing.Size(559, 251);
            this.syntaxEditor1.TabIndex = 0;
            this.syntaxEditor1.SelectionChanged += new ActiproSoftware.SyntaxEditor.SelectionEventHandler(this.syntaxEditor1_SelectionChanged);
            // 
            // lvRecommendations
            // 
            this.lvRecommendations.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lvRecommendations.ItemSettings.HideSelection = false;
            this.lvRecommendations.ItemSettings.SelectionType = Infragistics.Win.UltraWinListView.SelectionType.Single;
            this.lvRecommendations.Location = new System.Drawing.Point(0, 0);
            this.lvRecommendations.MainColumn.DataType = typeof(string);
            this.lvRecommendations.MainColumn.Text = "Recommendations";
            this.lvRecommendations.Name = "lvRecommendations";
            this.lvRecommendations.Size = new System.Drawing.Size(559, 121);
            this.lvRecommendations.TabIndex = 0;
            this.lvRecommendations.Text = "ultraListView1";
            this.lvRecommendations.View = Infragistics.Win.UltraWinListView.UltraListViewStyle.Details;
            this.lvRecommendations.ViewSettingsDetails.AutoFitColumns = Infragistics.Win.UltraWinListView.AutoFitColumns.ResizeAllColumns;
            this.lvRecommendations.ViewSettingsDetails.ColumnAutoSizeMode = ((Infragistics.Win.UltraWinListView.ColumnAutoSizeMode)((Infragistics.Win.UltraWinListView.ColumnAutoSizeMode.Header | Infragistics.Win.UltraWinListView.ColumnAutoSizeMode.VisibleItems)));
            this.lvRecommendations.ViewSettingsDetails.FullRowSelect = true;
            this.lvRecommendations.ItemSelectionChanged += new Infragistics.Win.UltraWinListView.ItemSelectionChangedEventHandler(this.lvRecommendations_ItemSelectionChanged);
            // 
            // SqlViewerDialog
            // 
            this.AcceptButton = this.closeButton;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(575, 481);
            this.Controls.Add(this.gradientPanel1);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this._SqlViewerDialog_Toolbars_Dock_Area_Left);
            this.Controls.Add(this._SqlViewerDialog_Toolbars_Dock_Area_Right);
            this.Controls.Add(this._SqlViewerDialog_Toolbars_Dock_Area_Top);
            this.Controls.Add(this._SqlViewerDialog_Toolbars_Dock_Area_Bottom);
            this.Controls.Add(this.statusBar);
            this.MinimumSize = new System.Drawing.Size(591, 519);
            this.Name = "SqlViewerDialog";
            this.Padding = new System.Windows.Forms.Padding(5);
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Sql Viewer";
            this.Load += new System.EventHandler(this.SqlViewerDialog_Load);
            this.Shown += new System.EventHandler(this.SqlViewerDialog_Shown);
            this.HelpRequested += new System.Windows.Forms.HelpEventHandler(this.SqlViewerDialog_HelpRequested);
            this.panel1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.toolbarManager)).EndInit();
            this.gradientPanel1.ResumeLayout(false);
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel2.ResumeLayout(false);
            this.splitContainer1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.lvRecommendations)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private ActiproSoftware.SyntaxEditor.SyntaxEditor syntaxEditor1;
        private GradientPanel gradientPanel1;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomPanel  panel1;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomButton closeButton;
        private Infragistics.Win.UltraWinToolbars.UltraToolbarsManager toolbarManager;
        private Infragistics.Win.UltraWinToolbars.UltraToolbarsDockArea _SqlViewerDialog_Toolbars_Dock_Area_Left;
        private Infragistics.Win.UltraWinToolbars.UltraToolbarsDockArea _SqlViewerDialog_Toolbars_Dock_Area_Right;
        private Infragistics.Win.UltraWinToolbars.UltraToolbarsDockArea _SqlViewerDialog_Toolbars_Dock_Area_Top;
        private Infragistics.Win.UltraWinToolbars.UltraToolbarsDockArea _SqlViewerDialog_Toolbars_Dock_Area_Bottom;
        private Infragistics.Win.UltraWinStatusBar.UltraStatusBar statusBar;
        private System.Windows.Forms.SplitContainer splitContainer1;
        private Infragistics.Win.UltraWinListView.UltraListView lvRecommendations;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomButton copyButton;
    }
}