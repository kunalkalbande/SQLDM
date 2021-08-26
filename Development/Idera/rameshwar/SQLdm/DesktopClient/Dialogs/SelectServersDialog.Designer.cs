namespace Idera.SQLdm.DesktopClient.Dialogs
{
    partial class SelectServersDialog
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
            Infragistics.Win.Appearance appearance2 = new Infragistics.Win.Appearance();
            Infragistics.Win.UltraWinToolbars.PopupMenuTool popupMenuTool1 = new Infragistics.Win.UltraWinToolbars.PopupMenuTool("tagsPopupMenu");
            this.okButton = new System.Windows.Forms.Button();
            this.cancelButton = new System.Windows.Forms.Button();
            this.descriptionLabel = new System.Windows.Forms.Label();
            this.selectTagsRadioButton = new System.Windows.Forms.RadioButton();
            this.selectServerRadioButton = new System.Windows.Forms.RadioButton();
            this.panel1 = new System.Windows.Forms.Panel();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.selectTemplateCheckBox = new System.Windows.Forms.CheckBox();
            this.templateDropDownButton = new Infragistics.Win.UltraWinEditors.UltraComboEditor();
            this.selectAllServersCheckBox = new System.Windows.Forms.CheckBox();
            this.serversStatusLabel = new System.Windows.Forms.Label();
            this.serversListBox = new System.Windows.Forms.CheckedListBox();
            this.tagsDropDownButton = new Infragistics.Win.Misc.UltraDropDownButton();
            this.toolbarsManager = new Idera.SQLdm.DesktopClient.Controls.ContextMenuManager(this.components);
            this._SelectServersDialog_Toolbars_Dock_Area_Left = new Infragistics.Win.UltraWinToolbars.UltraToolbarsDockArea();
            this._SelectServersDialog_Toolbars_Dock_Area_Right = new Infragistics.Win.UltraWinToolbars.UltraToolbarsDockArea();
            this._SelectServersDialog_Toolbars_Dock_Area_Top = new Infragistics.Win.UltraWinToolbars.UltraToolbarsDockArea();
            this._SelectServersDialog_Toolbars_Dock_Area_Bottom = new Infragistics.Win.UltraWinToolbars.UltraToolbarsDockArea();
            this.groupBox1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.templateDropDownButton)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.toolbarsManager)).BeginInit();
            this.SuspendLayout();
            // 
            // okButton
            // 
            this.okButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.okButton.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.okButton.Enabled = false;
            this.okButton.Location = new System.Drawing.Point(297, 519);
            this.okButton.Name = "okButton";
            this.okButton.Size = new System.Drawing.Size(75, 23);
            this.okButton.TabIndex = 6;
            this.okButton.Text = "OK";
            this.okButton.UseVisualStyleBackColor = true;
            this.okButton.Click += new System.EventHandler(this.okButton_Click);
            // 
            // cancelButton
            // 
            this.cancelButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.cancelButton.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.cancelButton.Location = new System.Drawing.Point(378, 519);
            this.cancelButton.Name = "cancelButton";
            this.cancelButton.Size = new System.Drawing.Size(75, 23);
            this.cancelButton.TabIndex = 7;
            this.cancelButton.Text = "Cancel";
            this.cancelButton.UseVisualStyleBackColor = true;
            // 
            // descriptionLabel
            // 
            this.descriptionLabel.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.descriptionLabel.Location = new System.Drawing.Point(50, 12);
            this.descriptionLabel.Name = "descriptionLabel";
            this.descriptionLabel.Size = new System.Drawing.Size(403, 32);
            this.descriptionLabel.TabIndex = 2;
            this.descriptionLabel.Text = "< Description >";
            this.descriptionLabel.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // selectTagsRadioButton
            // 
            this.selectTagsRadioButton.AutoSize = true;
            this.selectTagsRadioButton.Checked = true;
            this.selectTagsRadioButton.Location = new System.Drawing.Point(20, 77);
            this.selectTagsRadioButton.Name = "selectTagsRadioButton";
            this.selectTagsRadioButton.Size = new System.Drawing.Size(82, 17);
            this.selectTagsRadioButton.TabIndex = 2;
            this.selectTagsRadioButton.TabStop = true;
            this.selectTagsRadioButton.Text = "Select Tags";
            this.selectTagsRadioButton.UseVisualStyleBackColor = true;
            // 
            // selectServerRadioButton
            // 
            this.selectServerRadioButton.AutoSize = true;
            this.selectServerRadioButton.Location = new System.Drawing.Point(20, 142);
            this.selectServerRadioButton.Name = "selectServerRadioButton";
            this.selectServerRadioButton.Size = new System.Drawing.Size(94, 17);
            this.selectServerRadioButton.TabIndex = 4;
            this.selectServerRadioButton.Text = "Select Servers";
            this.selectServerRadioButton.UseVisualStyleBackColor = true;
            this.selectServerRadioButton.CheckedChanged += new System.EventHandler(this.selectServerRadioButton_CheckedChanged);
            // 
            // panel1
            // 
            this.panel1.BackgroundImage = global::Idera.SQLdm.DesktopClient.Properties.Resources.NavigationPaneServersLarge;
            this.panel1.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.panel1.Location = new System.Drawing.Point(12, 12);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(32, 32);
            this.panel1.TabIndex = 5;
            // 
            // groupBox1
            // 
            this.groupBox1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBox1.Controls.Add(this.tagsDropDownButton);
            this.groupBox1.Controls.Add(this.selectTagsRadioButton);
            this.groupBox1.Controls.Add(this.selectAllServersCheckBox);
            this.groupBox1.Controls.Add(this.serversStatusLabel);
            this.groupBox1.Controls.Add(this.serversListBox);
            this.groupBox1.Controls.Add(this.selectServerRadioButton);
            this.groupBox1.Controls.Add(this.selectTemplateCheckBox);
            this.groupBox1.Controls.Add(this.templateDropDownButton);
            this.groupBox1.Location = new System.Drawing.Point(12, 50);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(441, 463);
            this.groupBox1.TabIndex = 0;
            this.groupBox1.TabStop = false;
            // 
            // selectTemplateCheckBox
            // 
            this.selectTemplateCheckBox.AutoSize = true;
            this.selectTemplateCheckBox.Location = new System.Drawing.Point(20, 12);
            this.selectTemplateCheckBox.Name = "selectTemplateCheckBox";
            this.selectTemplateCheckBox.Size = new System.Drawing.Size(103, 17);
            this.selectTemplateCheckBox.TabIndex = 7;
            this.selectTemplateCheckBox.Text = "Select Template";
            this.selectTemplateCheckBox.UseVisualStyleBackColor = true;
            this.selectTemplateCheckBox.CheckedChanged += new System.EventHandler(this.selectTemplateCheckBox_CheckedChanged);
            // 
            // templateDropDownButton
            // 
            this.templateDropDownButton.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.templateDropDownButton.DropDownStyle = Infragistics.Win.DropDownStyle.DropDownList;
            this.templateDropDownButton.Enabled = false;
            this.templateDropDownButton.Location = new System.Drawing.Point(39, 39);
            this.templateDropDownButton.Name = "templateDropDownButton";
            this.templateDropDownButton.Size = new System.Drawing.Size(383, 21);
            this.templateDropDownButton.TabIndex = 1;
            // 
            // selectAllServersCheckBox
            // 
            this.selectAllServersCheckBox.AutoSize = true;
            this.selectAllServersCheckBox.Enabled = false;
            this.selectAllServersCheckBox.Location = new System.Drawing.Point(41, 162);
            this.selectAllServersCheckBox.Name = "selectAllServersCheckBox";
            this.selectAllServersCheckBox.Size = new System.Drawing.Size(70, 17);
            this.selectAllServersCheckBox.TabIndex = 5;
            this.selectAllServersCheckBox.Text = "Select All";
            this.selectAllServersCheckBox.UseVisualStyleBackColor = true;
            this.selectAllServersCheckBox.CheckedChanged += new System.EventHandler(this.selectAllServersCheckBox_CheckedChanged);
            // 
            // serversStatusLabel
            // 
            this.serversStatusLabel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.serversStatusLabel.BackColor = System.Drawing.Color.White;
            this.serversStatusLabel.ForeColor = System.Drawing.SystemColors.ControlText;
            this.serversStatusLabel.Location = new System.Drawing.Point(45, 271);
            this.serversStatusLabel.Name = "serversStatusLabel";
            this.serversStatusLabel.Size = new System.Drawing.Size(369, 13);
            this.serversStatusLabel.TabIndex = 4;
            this.serversStatusLabel.Text = Idera.SQLdm.Common.Constants.LOADING;
            this.serversStatusLabel.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // serversListBox
            // 
            this.serversListBox.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.serversListBox.CheckOnClick = true;
            this.serversListBox.Enabled = false;
            this.serversListBox.FormattingEnabled = true;
            this.serversListBox.Location = new System.Drawing.Point(38, 180);
            this.serversListBox.Name = "serversListBox";
            this.serversListBox.Size = new System.Drawing.Size(383, 259);
            this.serversListBox.Sorted = true;
            this.serversListBox.TabIndex = 6;
            this.serversListBox.SelectedIndexChanged += new System.EventHandler(this.serversListBox_SelectedIndexChanged);
            // 
            // tagsDropDownButton
            // 
            this.tagsDropDownButton.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            appearance2.BackColor = System.Drawing.Color.White;
            appearance2.BorderColor = System.Drawing.SystemColors.ControlDark;
            appearance2.TextHAlignAsString = "Left";
            this.tagsDropDownButton.Appearance = appearance2;
            this.tagsDropDownButton.ButtonStyle = Infragistics.Win.UIElementButtonStyle.WindowsVistaButton;
            this.tagsDropDownButton.Location = new System.Drawing.Point(38, 101);
            this.tagsDropDownButton.Name = "tagsDropDownButton";
            this.tagsDropDownButton.PopupItemKey = "tagsPopupMenu";
            this.tagsDropDownButton.PopupItemProvider = this.toolbarsManager;
            this.tagsDropDownButton.ShowFocusRect = false;
            this.tagsDropDownButton.ShowOutline = false;
            this.tagsDropDownButton.Size = new System.Drawing.Size(383, 22);
            this.tagsDropDownButton.Style = Infragistics.Win.Misc.SplitButtonDisplayStyle.DropDownButtonOnly;
            this.tagsDropDownButton.TabIndex = 3;
            this.tagsDropDownButton.UseAppStyling = false;
            this.tagsDropDownButton.UseOsThemes = Infragistics.Win.DefaultableBoolean.False;
            this.tagsDropDownButton.WrapText = false;
            this.tagsDropDownButton.DroppingDown += new System.ComponentModel.CancelEventHandler(this.tagsDropDownButton_DroppingDown);
            this.tagsDropDownButton.TextChanged += new System.EventHandler(this.tagsDropDownButton_TextChanged);
            // 
            // toolbarsManager
            // 
            this.toolbarsManager.DesignerFlags = 0;
            this.toolbarsManager.DockWithinContainer = this;
            this.toolbarsManager.DockWithinContainerBaseType = typeof(System.Windows.Forms.Form);
            this.toolbarsManager.ShowFullMenusDelay = 500;
            popupMenuTool1.SharedPropsInternal.Caption = "Tags Popup Menu";
            this.toolbarsManager.Tools.AddRange(new Infragistics.Win.UltraWinToolbars.ToolBase[] {
            popupMenuTool1});
            this.toolbarsManager.ToolClick += new Infragistics.Win.UltraWinToolbars.ToolClickEventHandler(this.toolbarsManager_ToolClick);
            // 
            // _SelectServersDialog_Toolbars_Dock_Area_Left
            // 
            this._SelectServersDialog_Toolbars_Dock_Area_Left.AccessibleRole = System.Windows.Forms.AccessibleRole.Grouping;
            this._SelectServersDialog_Toolbars_Dock_Area_Left.BackColor = System.Drawing.SystemColors.Control;
            this._SelectServersDialog_Toolbars_Dock_Area_Left.DockedPosition = Infragistics.Win.UltraWinToolbars.DockedPosition.Left;
            this._SelectServersDialog_Toolbars_Dock_Area_Left.ForeColor = System.Drawing.SystemColors.ControlText;
            this._SelectServersDialog_Toolbars_Dock_Area_Left.Location = new System.Drawing.Point(0, 0);
            this._SelectServersDialog_Toolbars_Dock_Area_Left.Name = "_SelectServersDialog_Toolbars_Dock_Area_Left";
            this._SelectServersDialog_Toolbars_Dock_Area_Left.Size = new System.Drawing.Size(0, 554);
            this._SelectServersDialog_Toolbars_Dock_Area_Left.ToolbarsManager = this.toolbarsManager;
            // 
            // _SelectServersDialog_Toolbars_Dock_Area_Right
            // 
            this._SelectServersDialog_Toolbars_Dock_Area_Right.AccessibleRole = System.Windows.Forms.AccessibleRole.Grouping;
            this._SelectServersDialog_Toolbars_Dock_Area_Right.BackColor = System.Drawing.SystemColors.Control;
            this._SelectServersDialog_Toolbars_Dock_Area_Right.DockedPosition = Infragistics.Win.UltraWinToolbars.DockedPosition.Right;
            this._SelectServersDialog_Toolbars_Dock_Area_Right.ForeColor = System.Drawing.SystemColors.ControlText;
            this._SelectServersDialog_Toolbars_Dock_Area_Right.Location = new System.Drawing.Point(465, 0);
            this._SelectServersDialog_Toolbars_Dock_Area_Right.Name = "_SelectServersDialog_Toolbars_Dock_Area_Right";
            this._SelectServersDialog_Toolbars_Dock_Area_Right.Size = new System.Drawing.Size(0, 554);
            this._SelectServersDialog_Toolbars_Dock_Area_Right.ToolbarsManager = this.toolbarsManager;
            // 
            // _SelectServersDialog_Toolbars_Dock_Area_Top
            // 
            this._SelectServersDialog_Toolbars_Dock_Area_Top.AccessibleRole = System.Windows.Forms.AccessibleRole.Grouping;
            this._SelectServersDialog_Toolbars_Dock_Area_Top.BackColor = System.Drawing.SystemColors.Control;
            this._SelectServersDialog_Toolbars_Dock_Area_Top.DockedPosition = Infragistics.Win.UltraWinToolbars.DockedPosition.Top;
            this._SelectServersDialog_Toolbars_Dock_Area_Top.ForeColor = System.Drawing.SystemColors.ControlText;
            this._SelectServersDialog_Toolbars_Dock_Area_Top.Location = new System.Drawing.Point(0, 0);
            this._SelectServersDialog_Toolbars_Dock_Area_Top.Name = "_SelectServersDialog_Toolbars_Dock_Area_Top";
            this._SelectServersDialog_Toolbars_Dock_Area_Top.Size = new System.Drawing.Size(465, 0);
            this._SelectServersDialog_Toolbars_Dock_Area_Top.ToolbarsManager = this.toolbarsManager;
            // 
            // _SelectServersDialog_Toolbars_Dock_Area_Bottom
            // 
            this._SelectServersDialog_Toolbars_Dock_Area_Bottom.AccessibleRole = System.Windows.Forms.AccessibleRole.Grouping;
            this._SelectServersDialog_Toolbars_Dock_Area_Bottom.BackColor = System.Drawing.SystemColors.Control;
            this._SelectServersDialog_Toolbars_Dock_Area_Bottom.DockedPosition = Infragistics.Win.UltraWinToolbars.DockedPosition.Bottom;
            this._SelectServersDialog_Toolbars_Dock_Area_Bottom.ForeColor = System.Drawing.SystemColors.ControlText;
            this._SelectServersDialog_Toolbars_Dock_Area_Bottom.Location = new System.Drawing.Point(0, 554);
            this._SelectServersDialog_Toolbars_Dock_Area_Bottom.Name = "_SelectServersDialog_Toolbars_Dock_Area_Bottom";
            this._SelectServersDialog_Toolbars_Dock_Area_Bottom.Size = new System.Drawing.Size(465, 0);
            this._SelectServersDialog_Toolbars_Dock_Area_Bottom.ToolbarsManager = this.toolbarsManager;
            // 
            // SelectServersDialog
            // 
            this.AcceptButton = this.okButton;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.cancelButton;
            this.ClientSize = new System.Drawing.Size(465, 554);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.descriptionLabel);
            this.Controls.Add(this.cancelButton);
            this.Controls.Add(this.okButton);
            this.Controls.Add(this._SelectServersDialog_Toolbars_Dock_Area_Left);
            this.Controls.Add(this._SelectServersDialog_Toolbars_Dock_Area_Right);
            this.Controls.Add(this._SelectServersDialog_Toolbars_Dock_Area_Top);
            this.Controls.Add(this._SelectServersDialog_Toolbars_Dock_Area_Bottom);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.MinimumSize = new System.Drawing.Size(385, 400);
            this.Name = "SelectServersDialog";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Select Servers or Tags";
            this.Load += new System.EventHandler(this.SelectServersDialog_Load);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.templateDropDownButton)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.toolbarsManager)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button okButton;
        private System.Windows.Forms.Button cancelButton;
        private System.Windows.Forms.Label descriptionLabel;
        private System.Windows.Forms.RadioButton selectTagsRadioButton;
        private System.Windows.Forms.RadioButton selectServerRadioButton;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.GroupBox groupBox1;
        private Infragistics.Win.Misc.UltraDropDownButton tagsDropDownButton;
        private Idera.SQLdm.DesktopClient.Controls.ContextMenuManager toolbarsManager;
        private Infragistics.Win.UltraWinToolbars.UltraToolbarsDockArea _SelectServersDialog_Toolbars_Dock_Area_Left;
        private Infragistics.Win.UltraWinToolbars.UltraToolbarsDockArea _SelectServersDialog_Toolbars_Dock_Area_Right;
        private Infragistics.Win.UltraWinToolbars.UltraToolbarsDockArea _SelectServersDialog_Toolbars_Dock_Area_Top;
        private Infragistics.Win.UltraWinToolbars.UltraToolbarsDockArea _SelectServersDialog_Toolbars_Dock_Area_Bottom;
        private System.Windows.Forms.CheckedListBox serversListBox;
        private System.Windows.Forms.Label serversStatusLabel;
        private System.Windows.Forms.CheckBox selectAllServersCheckBox;
        private Infragistics.Win.UltraWinEditors.UltraComboEditor templateDropDownButton;
        private System.Windows.Forms.CheckBox selectTemplateCheckBox;
    }
}