using Idera.SQLdm.DesktopClient.Properties;
using System.Drawing;

namespace Idera.SQLdm.DesktopClient.Dialogs
{
    partial class AlertTemplateDialog
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
            bool isDarkThemeSelected = Properties.Settings.Default.ColorScheme == "Dark";
            this.components = new System.ComponentModel.Container();
            Color backColor = Settings.Default.ColorScheme == "Dark" ? ColorTranslator.FromHtml(DarkThemeColorConstants.UltraGridBackColor) : Color.White;
            Color foreColor = Settings.Default.ColorScheme == "Dark" ? ColorTranslator.FromHtml(DarkThemeColorConstants.UltraGridForeColor) : Color.Black;
            Color activeBackColor = Settings.Default.ColorScheme == "Dark" ? ColorTranslator.FromHtml(DarkThemeColorConstants.UltraGridActiveBackColor) : Color.White;
            Color hoverBackColor = Settings.Default.ColorScheme == "Dark" ? ColorTranslator.FromHtml(DarkThemeColorConstants.UltraGridHoverBackColor) : Color.White;
            Infragistics.Win.Appearance appearance1 = new Infragistics.Win.Appearance();
            Infragistics.Win.UltraWinGrid.UltraGridBand ultraGridBand1 = new Infragistics.Win.UltraWinGrid.UltraGridBand("AlertTemplate", -1);
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn1 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("Name");
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn2 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("Description");
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn3 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("TemplateID");
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn4 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("DefaultIndicator");
            Infragistics.Win.Appearance appearance2 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance3 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance4 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance5 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance7 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance8 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance9 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance10 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance11 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance13 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance12 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance14 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance15 = new Infragistics.Win.Appearance();

            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(AlertTemplateDialog));
            this.cancelButton = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomButton();
            this.okButton = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomButton();
            this.office2007PropertyPage1 = new Idera.SQLdm.DesktopClient.Controls.Office2007PropertyPage(isDarkThemeSelected);
            this.tableLayoutPanel1 = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomTableLayoutPanel();
            this.alertTemplatesGrid = new Infragistics.Win.UltraWinGrid.UltraGrid();
            this.alertTemplatesBindingSource = new System.Windows.Forms.BindingSource(this.components);
            this.label1 = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomLabel();
            this.informationBox1 = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomInformationBox();
            this.propertiesHeaderStrip3 = new Idera.SQLdm.DesktopClient.Controls.PropertiesHeaderStrip();
            this.btnAddTemplate = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomButton();
            this.btnSetDefault = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomButton();
            this.btnEditTemplate = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomButton();
            this.btnImport = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomButton();
            this.btnExport = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomButton();
            this.btnDeleteTemplate = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomButton();
            this.btnApplyTemplate = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomButton();
            this.lnkCommunitySite = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomLinkLabel();
            this.tableLayoutPanel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.alertTemplatesGrid)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.alertTemplatesBindingSource)).BeginInit();
            this.SuspendLayout();
            // 
            // cancelButton
            // 
            this.cancelButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.cancelButton.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.cancelButton.Location = new System.Drawing.Point(706, 441);
            this.cancelButton.Name = "cancelButton";
            this.cancelButton.Size = new System.Drawing.Size(75, 23);
            this.cancelButton.TabIndex = 2;
            this.cancelButton.Text = "Cancel";
            this.cancelButton.UseVisualStyleBackColor = true;
            // 
            // okButton
            // 
            this.okButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.okButton.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.okButton.Location = new System.Drawing.Point(623, 441);
            this.okButton.Name = "okButton";
            this.okButton.Size = new System.Drawing.Size(75, 23);
            this.okButton.TabIndex = 1;
            this.okButton.Text = "OK";
            this.okButton.UseVisualStyleBackColor = true;
            // 
            // office2007PropertyPage1
            // 
            this.office2007PropertyPage1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.office2007PropertyPage1.BackColor = System.Drawing.Color.White;
            this.office2007PropertyPage1.BorderWidth = 1;
            // 
            // 
            // 
            this.office2007PropertyPage1.ContentPanel.BackColor2 = System.Drawing.Color.White;
            this.office2007PropertyPage1.ContentPanel.BorderColor = System.Drawing.SystemColors.ControlDark;
            this.office2007PropertyPage1.ContentPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.office2007PropertyPage1.ContentPanel.FillStyle = Idera.SQLdm.DesktopClient.Controls.GradientPanelFillStyle.Solid;
            this.office2007PropertyPage1.ContentPanel.Location = new System.Drawing.Point(1, 56);
            this.office2007PropertyPage1.ContentPanel.Name = "ContentPanel";
            this.office2007PropertyPage1.ContentPanel.Padding = new System.Windows.Forms.Padding(1);
            this.office2007PropertyPage1.ContentPanel.ShowBorder = false;
            this.office2007PropertyPage1.ContentPanel.Size = new System.Drawing.Size(767, 363);
            this.office2007PropertyPage1.ContentPanel.TabIndex = 1;
            this.office2007PropertyPage1.HeaderImage = global::Idera.SQLdm.DesktopClient.Properties.Resources.GeneralProperties;
            this.office2007PropertyPage1.Location = new System.Drawing.Point(12, 12);
            this.office2007PropertyPage1.Name = "office2007PropertyPage1";
            this.office2007PropertyPage1.Size = new System.Drawing.Size(769, 420);
            this.office2007PropertyPage1.TabIndex = 0;
            this.office2007PropertyPage1.Text = "Configure Alert Templates";
            this.office2007PropertyPage1.BackColor = backColor;
            //this.office2007PropertyPage1.ContentPanel.
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.BackColor = System.Drawing.Color.White;
            this.tableLayoutPanel1.ColumnCount = 9;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 14F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 14F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 14F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 30F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 14F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 14F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 30F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 14F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 14F));
            this.tableLayoutPanel1.Controls.Add(this.alertTemplatesGrid, 0, 3);
            this.tableLayoutPanel1.Controls.Add(this.label1, 0, 2);
            this.tableLayoutPanel1.Controls.Add(this.informationBox1, 0, 1);
            this.tableLayoutPanel1.Controls.Add(this.propertiesHeaderStrip3, 0, 0);
            this.tableLayoutPanel1.Controls.Add(this.btnAddTemplate, 0, 4);
            this.tableLayoutPanel1.Controls.Add(this.btnSetDefault, 7, 4);
            this.tableLayoutPanel1.Controls.Add(this.btnEditTemplate, 1, 4);
            this.tableLayoutPanel1.Controls.Add(this.btnImport, 4, 4);
            this.tableLayoutPanel1.Controls.Add(this.btnExport, 5, 4);
            this.tableLayoutPanel1.Controls.Add(this.btnDeleteTemplate, 2, 4);
            this.tableLayoutPanel1.Controls.Add(this.btnApplyTemplate, 8, 4);
            this.tableLayoutPanel1.Location = new System.Drawing.Point(27, 69);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 5;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.tableLayoutPanel1.Size = new System.Drawing.Size(740, 357);
            this.tableLayoutPanel1.TabIndex = 3;
            // 
            // alertTemplatesGrid
            // 
            this.tableLayoutPanel1.SetColumnSpan(this.alertTemplatesGrid, 9);
            this.alertTemplatesGrid.DataSource = this.alertTemplatesBindingSource;
            appearance1.BackColor = backcolor; //System.Drawing.SystemColors.Window;
            appearance1.BorderColor = System.Drawing.SystemColors.InactiveCaption;
         
            appearance14.BackColor = backColor;
            appearance14.ForeColor = foreColor;
            this.alertTemplatesGrid.DisplayLayout.Appearance = appearance1;
            this.alertTemplatesGrid.DisplayLayout.AutoFitStyle = Infragistics.Win.UltraWinGrid.AutoFitStyle.ExtendLastColumn;
            ultraGridColumn1.AutoCompleteMode = Infragistics.Win.AutoCompleteMode.Append;
            ultraGridColumn1.Header.VisiblePosition = 0;
            ultraGridColumn1.Width = 179;
            ultraGridColumn1.Header.Appearance = appearance14;
            ultraGridColumn1.CellAppearance = appearance14;
            ultraGridColumn2.AutoCompleteMode = Infragistics.Win.AutoCompleteMode.Append;
            ultraGridColumn2.Header.VisiblePosition = 1;
            ultraGridColumn2.Width = 323;
            ultraGridColumn2.Header.Appearance = appearance14;
            ultraGridColumn2.CellAppearance = appearance14;
            ultraGridColumn3.AutoCompleteMode = Infragistics.Win.AutoCompleteMode.Append;
            ultraGridColumn3.Header.VisiblePosition = 2;
            ultraGridColumn3.Hidden = true;
            ultraGridColumn4.AutoCompleteMode = Infragistics.Win.AutoCompleteMode.Append;
            ultraGridColumn4.Header.VisiblePosition = 3;
            ultraGridColumn4.Hidden = true;
            ultraGridBand1.Columns.AddRange(new object[] {
            ultraGridColumn1,
            ultraGridColumn2,
            ultraGridColumn3,
            ultraGridColumn4});
            this.alertTemplatesGrid.DisplayLayout.BandsSerializer.Add(ultraGridBand1);
            this.alertTemplatesGrid.DisplayLayout.BorderStyle = Infragistics.Win.UIElementBorderStyle.Solid;
            this.alertTemplatesGrid.DisplayLayout.CaptionVisible = Infragistics.Win.DefaultableBoolean.False;
            appearance2.BackColor = System.Drawing.SystemColors.ActiveBorder;
            appearance2.BackColor2 = System.Drawing.SystemColors.ControlDark;
            appearance2.BackGradientStyle = Infragistics.Win.GradientStyle.Vertical;
            appearance2.BorderColor = System.Drawing.SystemColors.Window;
            this.alertTemplatesGrid.DisplayLayout.GroupByBox.Appearance = appearance2;
            appearance3.ForeColor = System.Drawing.SystemColors.GrayText;
            this.alertTemplatesGrid.DisplayLayout.GroupByBox.BandLabelAppearance = appearance3;
            this.alertTemplatesGrid.DisplayLayout.GroupByBox.BorderStyle = Infragistics.Win.UIElementBorderStyle.Solid;
            this.alertTemplatesGrid.DisplayLayout.GroupByBox.Hidden = true;
            appearance4.BackColor = System.Drawing.SystemColors.ControlLightLight;
            appearance4.BackColor2 = System.Drawing.SystemColors.Control;
            appearance4.BackGradientStyle = Infragistics.Win.GradientStyle.Horizontal;
            appearance4.ForeColor = System.Drawing.SystemColors.GrayText;
            this.alertTemplatesGrid.DisplayLayout.GroupByBox.PromptAppearance = appearance4;
            this.alertTemplatesGrid.DisplayLayout.MaxColScrollRegions = 1;
            this.alertTemplatesGrid.DisplayLayout.MaxRowScrollRegions = 1;
            appearance5.BackColor = System.Drawing.SystemColors.Window;
            appearance5.ForeColor = System.Drawing.SystemColors.ControlText;
            this.alertTemplatesGrid.DisplayLayout.Override.ActiveCellAppearance = appearance5;
            this.alertTemplatesGrid.DisplayLayout.Override.BorderStyleCell = Infragistics.Win.UIElementBorderStyle.Solid;
            this.alertTemplatesGrid.DisplayLayout.Override.BorderStyleRow = Infragistics.Win.UIElementBorderStyle.Solid;
            appearance7.BackColor = System.Drawing.SystemColors.Window;
            this.alertTemplatesGrid.DisplayLayout.Override.CardAreaAppearance = appearance7;
            appearance8.BorderColor = System.Drawing.Color.Silver;
            appearance8.TextTrimming = Infragistics.Win.TextTrimming.EllipsisCharacter;
            this.alertTemplatesGrid.DisplayLayout.Override.CellAppearance = appearance8;
            this.alertTemplatesGrid.DisplayLayout.Override.CellClickAction = Infragistics.Win.UltraWinGrid.CellClickAction.RowSelect;
            this.alertTemplatesGrid.DisplayLayout.Override.CellPadding = 0;
            appearance9.BackColor = System.Drawing.SystemColors.Control;
            appearance9.BackColor2 = System.Drawing.SystemColors.ControlDark;
            appearance9.BackGradientAlignment = Infragistics.Win.GradientAlignment.Element;
            appearance9.BackGradientStyle = Infragistics.Win.GradientStyle.Horizontal;
            appearance9.BorderColor = System.Drawing.SystemColors.Window;
            this.alertTemplatesGrid.DisplayLayout.Override.GroupByRowAppearance = appearance9;
            appearance10.TextHAlignAsString = "Left";
            this.alertTemplatesGrid.DisplayLayout.Override.HeaderAppearance = appearance10;
            this.alertTemplatesGrid.DisplayLayout.Override.HeaderClickAction = Infragistics.Win.UltraWinGrid.HeaderClickAction.SortMulti;
            this.alertTemplatesGrid.DisplayLayout.Override.HeaderStyle = Infragistics.Win.HeaderStyle.WindowsXPCommand;
            appearance11.BackColor = System.Drawing.SystemColors.Window;
            appearance11.BorderColor = System.Drawing.Color.Silver;
            this.alertTemplatesGrid.DisplayLayout.Override.RowAppearance = appearance11;
            this.alertTemplatesGrid.DisplayLayout.Override.RowSelectors = Infragistics.Win.DefaultableBoolean.False;
            appearance13.BackColor = System.Drawing.SystemColors.Highlight;
            appearance13.ForeColor = System.Drawing.SystemColors.HighlightText;
            this.alertTemplatesGrid.DisplayLayout.Override.SelectedRowAppearance = appearance13;
            this.alertTemplatesGrid.DisplayLayout.Override.SelectTypeCol = Infragistics.Win.UltraWinGrid.SelectType.None;
            this.alertTemplatesGrid.DisplayLayout.Override.SelectTypeRow = Infragistics.Win.UltraWinGrid.SelectType.Single;
            appearance12.BackColor = System.Drawing.SystemColors.ControlLight;
            this.alertTemplatesGrid.DisplayLayout.Override.TemplateAddRowAppearance = appearance12;
            this.alertTemplatesGrid.DisplayLayout.ScrollBounds = Infragistics.Win.UltraWinGrid.ScrollBounds.ScrollToFill;
            this.alertTemplatesGrid.DisplayLayout.ScrollStyle = Infragistics.Win.UltraWinGrid.ScrollStyle.Immediate;
            this.alertTemplatesGrid.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.alertTemplatesGrid.Location = new System.Drawing.Point(3, 96);
            this.alertTemplatesGrid.Name = "alertTemplatesGrid";
            this.alertTemplatesGrid.Size = new System.Drawing.Size(726, 218);
            this.alertTemplatesGrid.TabIndex = 4;
            this.alertTemplatesGrid.Text = "ultraGrid1";
            this.alertTemplatesGrid.AfterSelectChange += new Infragistics.Win.UltraWinGrid.AfterSelectChangeEventHandler(this.alertTemplatesGrid_AfterSelectChange);
            // 
            // alertTemplatesBindingSource
            // 
            this.alertTemplatesBindingSource.DataSource = typeof(Idera.SQLdm.DesktopClient.Objects.AlertTemplate);
            // 
            // label1
            // 
            this.label1.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.label1.AutoSize = true;
            this.tableLayoutPanel1.SetColumnSpan(this.label1, 9);
            this.label1.Location = new System.Drawing.Point(3, 80);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(409, 13);
            this.label1.TabIndex = 3;
            this.label1.Text = "Note:  The template shown in bold text in the list below is the current default t" +
    "emplate.";
            // 
            // informationBox1
            // 
            this.informationBox1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.tableLayoutPanel1.SetColumnSpan(this.informationBox1, 9);
            this.informationBox1.Location = new System.Drawing.Point(3, 34);
            this.informationBox1.Name = "informationBox1";
            this.informationBox1.Size = new System.Drawing.Size(734, 43);
            this.informationBox1.TabIndex = 2;
            this.informationBox1.Text = resources.GetString("informationBox1.Text");
            // 
            // propertiesHeaderStrip3
            // 
            this.propertiesHeaderStrip3.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.tableLayoutPanel1.SetColumnSpan(this.propertiesHeaderStrip3, 9);
            this.propertiesHeaderStrip3.ForeColor = System.Drawing.Color.Black;
            this.propertiesHeaderStrip3.Location = new System.Drawing.Point(3, 3);
            this.propertiesHeaderStrip3.Name = "propertiesHeaderStrip3";
            this.propertiesHeaderStrip3.Size = new System.Drawing.Size(734, 25);
            this.propertiesHeaderStrip3.TabIndex = 1;
            this.propertiesHeaderStrip3.TabStop = false;
            this.propertiesHeaderStrip3.Text = "Define Alert Templates";
            this.propertiesHeaderStrip3.WordWrap = false;
            // 
            // btnAddTemplate
            // 
            this.btnAddTemplate.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.btnAddTemplate.BackColor = System.Drawing.SystemColors.Control;
            this.btnAddTemplate.Location = new System.Drawing.Point(3, 325);
            this.btnAddTemplate.Name = "btnAddTemplate";
            this.btnAddTemplate.Size = new System.Drawing.Size(84, 23);
            this.btnAddTemplate.TabIndex = 5;
            this.btnAddTemplate.Text = "New";
            this.btnAddTemplate.UseVisualStyleBackColor = false;
            this.btnAddTemplate.Click += new System.EventHandler(this.btnAddTemplate_Click);
            // 
            // btnSetDefault
            // 
            this.btnSetDefault.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.btnSetDefault.BackColor = System.Drawing.SystemColors.Control;
            this.btnSetDefault.Location = new System.Drawing.Point(548, 325);
            this.btnSetDefault.Name = "btnSetDefault";
            this.btnSetDefault.Size = new System.Drawing.Size(84, 23);
            this.btnSetDefault.TabIndex = 8;
            this.btnSetDefault.Text = "Set Default";
            this.btnSetDefault.UseVisualStyleBackColor = false;
            this.btnSetDefault.Click += new System.EventHandler(this.btnSetDefault_Click);
            // 
            // btnEditTemplate
            // 
            this.btnEditTemplate.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.btnEditTemplate.BackColor = System.Drawing.SystemColors.Control;
            this.btnEditTemplate.Location = new System.Drawing.Point(100, 325);
            this.btnEditTemplate.Name = "btnEditTemplate";
            this.btnEditTemplate.Size = new System.Drawing.Size(84, 23);
            this.btnEditTemplate.TabIndex = 6;
            this.btnEditTemplate.Text = "View/Edit";
            this.btnEditTemplate.UseVisualStyleBackColor = false;
            this.btnEditTemplate.Click += new System.EventHandler(this.btnEditTemplate_Click);
            // 
            // btnImport
            // 
            this.btnImport.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.btnImport.BackColor = System.Drawing.SystemColors.Control;
            this.btnImport.Location = new System.Drawing.Point(324, 325);
            this.btnImport.Name = "btnImport";
            this.btnImport.Size = new System.Drawing.Size(84, 23);
            this.btnImport.TabIndex = 6;
            this.btnImport.Text = "Import";
            this.btnImport.UseVisualStyleBackColor = false;
            this.btnImport.Click += new System.EventHandler(this.btnImport_Click);
            // 
            // btnExport
            // 
            this.btnExport.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.btnExport.BackColor = System.Drawing.SystemColors.Control;
            this.btnExport.Location = new System.Drawing.Point(421, 325);
            this.btnExport.Name = "btnExport";
            this.btnExport.Size = new System.Drawing.Size(84, 23);
            this.btnExport.TabIndex = 6;
            this.btnExport.Text = "Export";
            this.btnExport.UseVisualStyleBackColor = false;
            this.btnExport.Click += new System.EventHandler(this.btnExport_Click);
            // 
            // btnDeleteTemplate
            // 
            this.btnDeleteTemplate.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.btnDeleteTemplate.BackColor = System.Drawing.SystemColors.Control;
            this.btnDeleteTemplate.Location = new System.Drawing.Point(197, 325);
            this.btnDeleteTemplate.Name = "btnDeleteTemplate";
            this.btnDeleteTemplate.Size = new System.Drawing.Size(84, 23);
            this.btnDeleteTemplate.TabIndex = 7;
            this.btnDeleteTemplate.Text = "Delete";
            this.btnDeleteTemplate.UseVisualStyleBackColor = false;
            this.btnDeleteTemplate.Click += new System.EventHandler(this.btnDeleteTemplate_Click);
            // 
            // btnApplyTemplate
            // 
            this.btnApplyTemplate.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.btnApplyTemplate.BackColor = System.Drawing.SystemColors.Control;
            this.btnApplyTemplate.Location = new System.Drawing.Point(645, 325);
            this.btnApplyTemplate.Name = "btnApplyTemplate";
            this.btnApplyTemplate.Size = new System.Drawing.Size(84, 23);
            this.btnApplyTemplate.TabIndex = 9;
            this.btnApplyTemplate.Text = "Apply To...";
            this.btnApplyTemplate.UseVisualStyleBackColor = false;
            this.btnApplyTemplate.Click += new System.EventHandler(this.btnApplyTemplate_Click);
            // 
            // lnkCommunitySite
            // 
            this.lnkCommunitySite.AutoSize = true;
            this.lnkCommunitySite.Location = new System.Drawing.Point(33, 450);
            this.lnkCommunitySite.Name = "lnkCommunitySite";
            this.lnkCommunitySite.Size = new System.Drawing.Size(340, 13);
            this.lnkCommunitySite.TabIndex = 4;
            this.lnkCommunitySite.TabStop = true;
            this.lnkCommunitySite.Text = "Visit the IDERA Community Site to share Alert Templates with other users";
            this.lnkCommunitySite.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.lnkCommunitySite_LinkClicked);
            // 
            // AlertTemplateDialog
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(803, 486);
            this.Controls.Add(this.lnkCommunitySite);
            this.Controls.Add(this.tableLayoutPanel1);
            this.Controls.Add(this.cancelButton);
            this.Controls.Add(this.okButton);
            this.Controls.Add(this.office2007PropertyPage1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.HelpButton = true;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.MinimumSize = this.Size;
            this.Name = "AlertTemplateDialog";
            this.ShowIcon = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Alert Templates";
            this.HelpButtonClicked += new System.ComponentModel.CancelEventHandler(this.AlertTemplateDialog_HelpButtonClicked);
            this.Load += new System.EventHandler(this.AlertTemplateDialog_Load);
            this.HelpRequested += new System.Windows.Forms.HelpEventHandler(this.AlertTemplateDialog_HelpRequested);
            this.tableLayoutPanel1.ResumeLayout(false);
            this.tableLayoutPanel1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.alertTemplatesGrid)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.alertTemplatesBindingSource)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

      
        #endregion

        private Idera.SQLdm.DesktopClient.Controls.Office2007PropertyPage office2007PropertyPage1;
        private System.Windows.Forms.BindingSource alertTemplatesBindingSource;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomButton cancelButton;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomButton okButton;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomTableLayoutPanel tableLayoutPanel1;
        private Infragistics.Win.UltraWinGrid.UltraGrid alertTemplatesGrid;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomLabel label1;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomInformationBox informationBox1;
        private Controls.PropertiesHeaderStrip propertiesHeaderStrip3;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomButton btnAddTemplate;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomButton btnSetDefault;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomButton btnEditTemplate;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomButton btnDeleteTemplate;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomButton btnApplyTemplate;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomButton btnImport;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomButton btnExport;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomLinkLabel lnkCommunitySite;

        
    }
}
