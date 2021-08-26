using Idera.SQLdm.Common.UI.Controls;
namespace Idera.SQLdm.DesktopClient.Dialogs.Notification
{
    using Idera.SQLdm.Common.Notification;
    using Idera.SQLdm.DesktopClient.Properties;
    using System.Drawing;

    partial class NotificationRulesViewPanel
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
            Color backColor = Settings.Default.ColorScheme == "Dark" ? ColorTranslator.FromHtml(DarkThemeColorConstants.UltraGridBackColor) : Color.White;
            Color foreColor = Settings.Default.ColorScheme == "Dark" ? ColorTranslator.FromHtml(DarkThemeColorConstants.UltraGridForeColor) : Color.Black;
            Color activeBackColor = Settings.Default.ColorScheme == "Dark" ? ColorTranslator.FromHtml(DarkThemeColorConstants.UltraGridActiveBackColor) : Color.White;
            Color hoverBackColor = Settings.Default.ColorScheme == "Dark" ? ColorTranslator.FromHtml(DarkThemeColorConstants.UltraGridHoverBackColor) : Color.White;
            this.components = new System.ComponentModel.Container();
            Infragistics.Win.UltraWinGrid.UltraGridBand ultraGridBand1 = new Infragistics.Win.UltraWinGrid.UltraGridBand("NotificationRuleWrapper", -1);
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn1 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("Name", -1, null, 0, Infragistics.Win.UltraWinGrid.SortIndicator.Ascending, false);
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn2 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("Enabled");
            Infragistics.Win.Appearance appearance1 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance12 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance2 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance3 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance4 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance5 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance6 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance7 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance8 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance9 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance10 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance11 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance13 = new Infragistics.Win.Appearance();
            this.splitContainer1 = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomSplitContainer();
            this.rulesGroupBox = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomUltraGroupBox();
            this.stackLayoutPanel1 = new Idera.SQLdm.Common.UI.Controls.StackLayoutPanel();
            this.rulesListView = new Infragistics.Win.UltraWinGrid.UltraGrid();
            this.noRulesLabel = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomLabel();
            this.previewGroupBox = new Infragistics.Win.Misc.UltraExpandableGroupBox();
            this.previewGroupBoxPanel = new Infragistics.Win.Misc.UltraExpandableGroupBoxPanel();
            this.rulePreview = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomFilledWebBrowser();
            this.removeButton = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomButton();
            this.editButton = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomButton();
            this.addButton = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomButton();
            this.copyButton = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomButton();
            this.exportButton = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomButton();
            this.importButton = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomButton();
            this.panel1 = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomPanel ();
            this.tableLayoutPanel1 = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomTableLayoutPanel();
            this.rulesBindingSource = new System.Windows.Forms.BindingSource(this.components);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.rulesGroupBox)).BeginInit();
            this.rulesGroupBox.SuspendLayout();
            this.stackLayoutPanel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.rulesListView)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.previewGroupBox)).BeginInit();
            this.previewGroupBox.SuspendLayout();
            this.previewGroupBoxPanel.SuspendLayout();
            this.panel1.SuspendLayout();
            this.tableLayoutPanel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.rulesBindingSource)).BeginInit();
            this.SuspendLayout();
            // 
            // splitContainer1
            // 
            this.splitContainer1.BackColor = backColor; //System.Drawing.Color.Transparent;
            this.splitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer1.Location = new System.Drawing.Point(3, 3);
            this.splitContainer1.Name = "splitContainer1";
            this.splitContainer1.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // splitContainer1.Panel1
            // 
            this.splitContainer1.Panel1.Controls.Add(this.rulesGroupBox);
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.Controls.Add(this.previewGroupBox);
            this.splitContainer1.Size = new System.Drawing.Size(444, 441);
            this.splitContainer1.SplitterDistance = 232;
            this.splitContainer1.TabIndex = 3;
            this.splitContainer1.SplitterMoving += new System.Windows.Forms.SplitterCancelEventHandler(this.splitContainer1_SplitterMoving);
            // 
            // rulesGroupBox
            // 
            this.rulesGroupBox.BorderStyle = Infragistics.Win.Misc.GroupBoxBorderStyle.RectangularSolid;
            appearance1.BackColor = backColor; //System.Drawing.Color.White;
            appearance1.BackColor2 = backColor;
            appearance1.ForeColor = foreColor;
            this.rulesGroupBox.ContentAreaAppearance = appearance1;
            this.rulesGroupBox.HeaderAppearance = appearance1;
            this.rulesGroupBox.Appearance = appearance1;
            this.rulesGroupBox.Controls.Add(this.stackLayoutPanel1);
            this.rulesGroupBox.Dock = System.Windows.Forms.DockStyle.Fill;
            appearance12.BackColor = System.Drawing.Color.Transparent;
            //this.rulesGroupBox.HeaderAppearance = appearance12;
            this.rulesGroupBox.HeaderPosition = Infragistics.Win.Misc.GroupBoxHeaderPosition.TopOutsideBorder;
            this.rulesGroupBox.Location = new System.Drawing.Point(0, 0);
            this.rulesGroupBox.Name = "rulesGroupBox";
            this.rulesGroupBox.Size = new System.Drawing.Size(444, 232);
            this.rulesGroupBox.TabIndex = 0;
            this.rulesGroupBox.Text = "Rules";
            //this.rulesGroupBox.UseAppStyling = false;
            // 
            // stackLayoutPanel1
            // 
            this.stackLayoutPanel1.ActiveControl = null;
            this.stackLayoutPanel1.BackColor = backColor; //System.Drawing.Color.Transparent;
            this.stackLayoutPanel1.Controls.Add(this.rulesListView);
            this.stackLayoutPanel1.Controls.Add(this.noRulesLabel);
            this.stackLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.stackLayoutPanel1.Location = new System.Drawing.Point(2, 20);
            this.stackLayoutPanel1.Margin = new System.Windows.Forms.Padding(0);
            this.stackLayoutPanel1.Name = "stackLayoutPanel1";
            this.stackLayoutPanel1.Size = new System.Drawing.Size(440, 210);
            this.stackLayoutPanel1.TabIndex = 9;
            // 
            // rulesListView
            // 
            this.rulesListView.DataSource = this.rulesBindingSource;
            appearance2.BackColor = backColor; //ColorTranslator.FromHtml(DarkThemeColorConstants.UltraGridBackColor); //System.Drawing.SystemColors.Window;
            appearance2.BorderColor = System.Drawing.SystemColors.InactiveCaption;
            appearance2.ForeColor = foreColor;
            this.rulesListView.DisplayLayout.Appearance = appearance2;
            this.rulesListView.DisplayLayout.AutoFitStyle = Infragistics.Win.UltraWinGrid.AutoFitStyle.ExtendLastColumn;
            ultraGridColumn1.AutoCompleteMode = Infragistics.Win.AutoCompleteMode.Append;
            ultraGridColumn1.Header.VisiblePosition = 1;
            ultraGridColumn1.Header.Appearance = appearance2;
            ultraGridColumn1.CellAppearance = appearance2;
            ultraGridColumn1.CellButtonAppearance = appearance2;
            ultraGridColumn2.AutoCompleteMode = Infragistics.Win.AutoCompleteMode.Append;
            ultraGridColumn2.Header.VisiblePosition = 0;
            ultraGridColumn2.Header.Appearance = appearance2;
            ultraGridColumn2.CellAppearance = appearance2;
            ultraGridColumn2.CellButtonAppearance = appearance2;
            ultraGridBand1.Columns.AddRange(new object[] {
            ultraGridColumn1,
            ultraGridColumn2});
            this.rulesListView.DisplayLayout.BandsSerializer.Add(ultraGridBand1);
            this.rulesListView.DisplayLayout.CaptionVisible = Infragistics.Win.DefaultableBoolean.False;
            appearance3.BackColor = backColor; //System.Drawing.Color.FromArgb(((int)(((byte)(227)))), ((int)(((byte)(239)))), ((int)(((byte)(255)))));
            appearance3.BackColor2 = backColor;// System.Drawing.SystemColors.ControlDark;
            appearance3.BackGradientStyle = Infragistics.Win.GradientStyle.None;
            appearance3.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(101)))), ((int)(((byte)(147)))), ((int)(((byte)(207)))));
            this.rulesListView.DisplayLayout.GroupByBox.Appearance = appearance3;
            appearance4.ForeColor = foreColor; //System.Drawing.SystemColors.GrayText;
            this.rulesListView.DisplayLayout.GroupByBox.BandLabelAppearance = appearance4;
            this.rulesListView.DisplayLayout.GroupByBox.BorderStyle = Infragistics.Win.UIElementBorderStyle.Solid;
            this.rulesListView.DisplayLayout.GroupByBox.Hidden = true;
            appearance5.BackColor = backColor;  // System.Drawing.SystemColors.ControlLightLight;
            appearance5.BackColor2 = backColor;  // System.Drawing.SystemColors.Control;
            appearance5.BackGradientStyle = Infragistics.Win.GradientStyle.Horizontal;
            appearance5.ForeColor = foreColor; //System.Drawing.SystemColors.GrayText;
            this.rulesListView.DisplayLayout.GroupByBox.PromptAppearance = appearance5;
            this.rulesListView.DisplayLayout.MaxColScrollRegions = 1;
            this.rulesListView.DisplayLayout.MaxRowScrollRegions = 1;
            this.rulesListView.DisplayLayout.Override.AllowAddNew = Infragistics.Win.UltraWinGrid.AllowAddNew.No;
            this.rulesListView.DisplayLayout.Override.AllowDelete = Infragistics.Win.DefaultableBoolean.False;
            this.rulesListView.DisplayLayout.Override.AllowUpdate = Infragistics.Win.DefaultableBoolean.False;
            this.rulesListView.DisplayLayout.Override.BorderStyleCell = Infragistics.Win.UIElementBorderStyle.None;
            this.rulesListView.DisplayLayout.Override.BorderStyleRow = Infragistics.Win.UIElementBorderStyle.None;
            appearance6.BackColor = backColor;  // System.Drawing.SystemColors.Window;
            this.rulesListView.DisplayLayout.Override.CardAreaAppearance = appearance6;
            appearance7.BorderColor = System.Drawing.Color.Silver;
            appearance7.TextTrimming = Infragistics.Win.TextTrimming.EllipsisCharacter;
            this.rulesListView.DisplayLayout.Override.CellAppearance = appearance7;
            this.rulesListView.DisplayLayout.Override.CellClickAction = Infragistics.Win.UltraWinGrid.CellClickAction.RowSelect;
            this.rulesListView.DisplayLayout.Override.CellPadding = 0;
            this.rulesListView.DisplayLayout.Override.FixedHeaderIndicator = Infragistics.Win.UltraWinGrid.FixedHeaderIndicator.None;
            this.rulesListView.DisplayLayout.Override.GroupByColumnsHidden = Infragistics.Win.DefaultableBoolean.False;
            appearance8.BackColor = backColor;  // System.Drawing.SystemColors.Control;
            appearance8.BackColor2 = backColor;  // System.Drawing.SystemColors.ControlDark;
            appearance8.BackGradientAlignment = Infragistics.Win.GradientAlignment.Element;
            appearance8.BackGradientStyle = Infragistics.Win.GradientStyle.Horizontal;
            appearance8.BorderColor = System.Drawing.SystemColors.Window;
            this.rulesListView.DisplayLayout.Override.GroupByRowAppearance = appearance8;
            this.rulesListView.DisplayLayout.Override.GroupByRowInitialExpansionState = Infragistics.Win.UltraWinGrid.GroupByRowInitialExpansionState.Expanded;
            appearance9.TextHAlignAsString = "Left";
            this.rulesListView.DisplayLayout.Override.HeaderAppearance = appearance9;
            this.rulesListView.DisplayLayout.Override.HeaderClickAction = Infragistics.Win.UltraWinGrid.HeaderClickAction.SortSingle;
            appearance10.BackColor = backColor;  // System.Drawing.SystemColors.Window;
            appearance10.BorderColor = System.Drawing.Color.Silver;
            this.rulesListView.DisplayLayout.Override.RowAppearance = appearance10;
            this.rulesListView.DisplayLayout.Override.RowSelectors = Infragistics.Win.DefaultableBoolean.False;
            this.rulesListView.DisplayLayout.Override.SelectTypeCell = Infragistics.Win.UltraWinGrid.SelectType.None;
            this.rulesListView.DisplayLayout.Override.SelectTypeCol = Infragistics.Win.UltraWinGrid.SelectType.None;
            this.rulesListView.DisplayLayout.Override.SelectTypeRow = Infragistics.Win.UltraWinGrid.SelectType.Extended;
            appearance11.BackColor = backColor; //System.Drawing.SystemColors.ControlLight;
            this.rulesListView.DisplayLayout.Override.TemplateAddRowAppearance = appearance11;
            this.rulesListView.DisplayLayout.ScrollBounds = Infragistics.Win.UltraWinGrid.ScrollBounds.ScrollToFill;
            this.rulesListView.DisplayLayout.ScrollStyle = Infragistics.Win.UltraWinGrid.ScrollStyle.Immediate;
            this.rulesListView.DisplayLayout.UseFixedHeaders = true;
            this.rulesListView.DisplayLayout.ViewStyle = Infragistics.Win.UltraWinGrid.ViewStyle.SingleBand;
            this.rulesListView.DisplayLayout.ViewStyleBand = Infragistics.Win.UltraWinGrid.ViewStyleBand.OutlookGroupBy;
            this.rulesListView.Dock = System.Windows.Forms.DockStyle.Fill;
            this.rulesListView.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.rulesListView.Location = new System.Drawing.Point(0, 0);
            this.rulesListView.Name = "rulesListView";
            this.rulesListView.Size = new System.Drawing.Size(440, 210);
            this.rulesListView.TabIndex = 5;
            this.rulesListView.UpdateMode = Infragistics.Win.UltraWinGrid.UpdateMode.OnCellChangeOrLostFocus;
            this.rulesListView.AfterSelectChange += new Infragistics.Win.UltraWinGrid.AfterSelectChangeEventHandler(this.rulesListView_AfterSelectChange);
            this.rulesListView.DoubleClickRow += new Infragistics.Win.UltraWinGrid.DoubleClickRowEventHandler(this.rulesListView_DoubleClickRow);
            this.rulesListView.MouseClick += new System.Windows.Forms.MouseEventHandler(this.rulesListView_MouseClick);
            // 
            // noRulesLabel
            // 
            this.noRulesLabel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.noRulesLabel.BackColor = backColor;  
            this.noRulesLabel.ForeColor = foreColor; //System.Drawing.SystemColors.GrayText;
            this.noRulesLabel.Location = new System.Drawing.Point(0, 0);
            this.noRulesLabel.Name = "noRulesLabel";
            this.noRulesLabel.Size = new System.Drawing.Size(440, 210);
            this.noRulesLabel.TabIndex = 2;
            this.noRulesLabel.Text = "There are no action rules defined.  Click the Add button to create a new instance" +
                ".";
            this.noRulesLabel.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // previewGroupBox
            // 
            this.previewGroupBox.BorderStyle = Infragistics.Win.Misc.GroupBoxBorderStyle.RectangularSolid;
            appearance13.BackColor = backColor;  // System.Drawing.Color.White;
            appearance13.BackColor2 = backColor;
            appearance13.ForeColor = foreColor;
            this.previewGroupBox.ContentAreaAppearance = appearance13;
            this.previewGroupBox.Appearance = appearance13;
            this.previewGroupBox.HeaderAppearance = appearance13;
            this.previewGroupBox.Controls.Add(this.previewGroupBoxPanel);
            this.previewGroupBox.Dock = System.Windows.Forms.DockStyle.Fill;
            this.previewGroupBox.ExpandedSize = new System.Drawing.Size(444, 205);
            this.previewGroupBox.ExpansionIndicator = Infragistics.Win.Misc.GroupBoxExpansionIndicator.Far;
            this.previewGroupBox.HeaderPosition = Infragistics.Win.Misc.GroupBoxHeaderPosition.TopOutsideBorder;
            this.previewGroupBox.Location = new System.Drawing.Point(0, 0);
            this.previewGroupBox.Name = "previewGroupBox";
            this.previewGroupBox.Size = new System.Drawing.Size(444, 205);
            this.previewGroupBox.TabIndex = 0;
            this.previewGroupBox.Text = "Rule Description";
            //this.previewGroupBox.UseAppStyling = false;
            this.previewGroupBox.ExpandedStateChanged += new System.EventHandler(this.ultraExpandableGroupBox1_ExpandedStateChanged);
            // 
            // previewGroupBoxPanel
            // 
            this.previewGroupBoxPanel.Controls.Add(this.rulePreview);
            this.previewGroupBoxPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.previewGroupBoxPanel.Location = new System.Drawing.Point(2, 23);
            this.previewGroupBoxPanel.Name = "previewGroupBoxPanel";
            this.previewGroupBoxPanel.Size = new System.Drawing.Size(440, 180);
            this.previewGroupBoxPanel.TabIndex = 0;
            // 
            // rulePreview
            // 
            this.rulePreview.AllowWebBrowserDrop = false;
            this.rulePreview.CausesValidation = false;
            this.rulePreview.Dock = System.Windows.Forms.DockStyle.Fill;
            this.rulePreview.IsWebBrowserContextMenuEnabled = false;
            this.rulePreview.Location = new System.Drawing.Point(0, 0);
            this.rulePreview.MinimumSize = new System.Drawing.Size(20, 20);
            this.rulePreview.Name = "rulePreview";
            this.rulePreview.ScriptErrorsSuppressed = true;
            this.rulePreview.Size = new System.Drawing.Size(440, 180);
            this.rulePreview.TabIndex = 0;
            this.rulePreview.TabStop = false;
            this.rulePreview.WebBrowserShortcutsEnabled = false;
            this.rulePreview.Navigating += new System.Windows.Forms.WebBrowserNavigatingEventHandler(this.rulePreview_Navigating);
            // 
            // removeButton
            // 
            this.removeButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.removeButton.Location = new System.Drawing.Point(249, 8);
            this.removeButton.Name = "removeButton";
            this.removeButton.Size = new System.Drawing.Size(75, 23);
            this.removeButton.TabIndex = 9;
            this.removeButton.Text = "Remove";
            this.removeButton.UseVisualStyleBackColor = true;
            this.removeButton.Click += new System.EventHandler(this.removeButton_Click);
            // 
            // editButton
            // 
            this.editButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.editButton.Location = new System.Drawing.Point(85, 8);
            this.editButton.Name = "editButton";
            this.editButton.Size = new System.Drawing.Size(75, 23);
            this.editButton.TabIndex = 7;
            this.editButton.Text = "Edit";
            this.editButton.UseVisualStyleBackColor = true;
            this.editButton.Click += new System.EventHandler(this.editButton_Click);
            // 
            // addButton
            // 
            this.addButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.addButton.Location = new System.Drawing.Point(3, 8);
            this.addButton.Name = "addButton";
            this.addButton.Size = new System.Drawing.Size(75, 23);
            this.addButton.TabIndex = 6;
            this.addButton.Text = "Add";
            this.addButton.UseVisualStyleBackColor = true;
            this.addButton.Click += new System.EventHandler(this.addButton_Click);
            // 
            // copyButton
            // 
            this.copyButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.copyButton.Location = new System.Drawing.Point(167, 8);
            this.copyButton.Name = "copyButton";
            this.copyButton.Size = new System.Drawing.Size(75, 23);
            this.copyButton.TabIndex = 8;
            this.copyButton.Text = "Copy";
            this.copyButton.UseVisualStyleBackColor = true;
            this.copyButton.Click += new System.EventHandler(this.copyButton_Click);
            // 
            // exportButton SQLdm 10.0 Swati Gogia
            // 
            this.exportButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.exportButton.Location = new System.Drawing.Point(332, 8);
            this.exportButton.Name = "exportButton";
            this.exportButton.Size = new System.Drawing.Size(75, 23);
            this.exportButton.TabIndex = 8;
            this.exportButton.Text = "Export";
            this.exportButton.UseVisualStyleBackColor = true;
            this.exportButton.Click += new System.EventHandler(this.exportButton_Click);
            // 
            // importButton SQLdm 10.0 (Swati Gogia)
            // 
            this.importButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.importButton.Location = new System.Drawing.Point(412, 8);
            this.importButton.Name = "importButton";
            this.importButton.Size = new System.Drawing.Size(75, 23);
            this.importButton.TabIndex = 8;
            this.importButton.Text = "Import";
            this.importButton.UseVisualStyleBackColor = true;
            this.importButton.Click += new System.EventHandler(this.importButton_Click);
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.removeButton);
            this.panel1.Controls.Add(this.copyButton);
            this.panel1.Controls.Add(this.exportButton);
            this.panel1.Controls.Add(this.importButton);
            this.panel1.Controls.Add(this.addButton);
            this.panel1.Controls.Add(this.editButton);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel1.Location = new System.Drawing.Point(3, 450);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(444, 34);
            this.panel1.TabIndex = 10;
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.ColumnCount = 1;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel1.Controls.Add(this.panel1, 0, 1);
            this.tableLayoutPanel1.Controls.Add(this.splitContainer1, 0, 0);
            this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel1.Location = new System.Drawing.Point(0, 0);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 2;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel1.Size = new System.Drawing.Size(450, 487);
            this.tableLayoutPanel1.TabIndex = 11;
            // 
            // rulesBindingSource
            // 
            this.rulesBindingSource.DataSource = typeof(Idera.SQLdm.DesktopClient.Dialogs.Notification.NotificationRuleWrapper);
            // 
            // NotificationRulesViewPanel
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = backColor; //System.Drawing.SystemColors.Control;
            this.ForeColor = foreColor;
            this.Controls.Add(this.tableLayoutPanel1);
            this.Name = "NotificationRulesViewPanel";
            this.Size = new System.Drawing.Size(450, 487);
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).EndInit();
            this.splitContainer1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.rulesGroupBox)).EndInit();
            this.rulesGroupBox.ResumeLayout(false);
            this.stackLayoutPanel1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.rulesListView)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.previewGroupBox)).EndInit();
            this.previewGroupBox.ResumeLayout(false);
            this.previewGroupBoxPanel.ResumeLayout(false);
            this.panel1.ResumeLayout(false);
            this.tableLayoutPanel1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.rulesBindingSource)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private Idera.SQLdm.Common.UI.Controls.StackLayoutPanel stackLayoutPanel1;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomButton removeButton;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomButton editButton;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomButton addButton;
        private System.Windows.Forms.SplitContainer splitContainer1;
        private Infragistics.Win.Misc.UltraGroupBox rulesGroupBox;
        private Infragistics.Win.Misc.UltraExpandableGroupBox previewGroupBox;
        private Infragistics.Win.Misc.UltraExpandableGroupBoxPanel previewGroupBoxPanel;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomFilledWebBrowser rulePreview;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomButton copyButton;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomButton exportButton;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomButton importButton;
        private System.Windows.Forms.BindingSource rulesBindingSource;
        private Infragistics.Win.UltraWinGrid.UltraGrid rulesListView;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomPanel  panel1;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomTableLayoutPanel tableLayoutPanel1;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomLabel noRulesLabel;
    }
}
