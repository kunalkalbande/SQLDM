namespace Idera.SQLdm.DesktopClient.Controls.Analysis
{
    partial class BlockedRecommendationsTab
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
            Infragistics.Win.Appearance appearance15 = new Infragistics.Win.Appearance();
            Infragistics.Win.UltraWinGrid.UltraGridBand ultraGridBand1 = new Infragistics.Win.UltraWinGrid.UltraGridBand("Band 0", -1);
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn1 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("Blocked", -1, 276968329);
            Infragistics.Win.Appearance appearance16 = new Infragistics.Win.Appearance();
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn2 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("ID");
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn3 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("Category");
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn4 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("Description");
            Infragistics.Win.Appearance appearance17 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance18 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance19 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance20 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance21 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance22 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance23 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance24 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance26 = new Infragistics.Win.Appearance();
            Infragistics.Win.ValueList valueList1 = new Infragistics.Win.ValueList(276968329);
            Infragistics.Win.ValueListItem valueListItem3 = new Infragistics.Win.ValueListItem();
            Infragistics.Win.Appearance appearance27 = new Infragistics.Win.Appearance();
            Infragistics.Win.ValueListItem valueListItem4 = new Infragistics.Win.ValueListItem();
            Infragistics.Win.Appearance appearance28 = new Infragistics.Win.Appearance();
            this._blockedGrid = new Infragistics.Win.UltraWinGrid.UltraGrid();
            ((System.ComponentModel.ISupportInitialize)(this._blockedGrid)).BeginInit();
            this.SuspendLayout();
            // 
            // _blockedGrid
            // 
            appearance15.BackColor = System.Drawing.SystemColors.Window;
            appearance15.BorderColor = System.Drawing.SystemColors.ControlDark;
            this._blockedGrid.DisplayLayout.Appearance = appearance15;
            this._blockedGrid.DisplayLayout.AutoFitStyle = Infragistics.Win.UltraWinGrid.AutoFitStyle.ExtendLastColumn;
            appearance16.Image = global::Idera.SQLdm.DesktopClient.Properties.Resources.block32;
            ultraGridColumn1.Header.Appearance = appearance16;
            ultraGridColumn1.Header.Caption = "";
            ultraGridColumn1.Header.VisiblePosition = 0;
            ultraGridColumn1.MaxWidth = 37;
            ultraGridColumn1.MinWidth = 37;
            ultraGridColumn1.Width = 37;
            ultraGridColumn2.Header.VisiblePosition = 1;
            ultraGridColumn2.Width = 76;
            ultraGridColumn3.Header.VisiblePosition = 2;
            ultraGridColumn3.Width = 131;
            ultraGridColumn4.Header.VisiblePosition = 3;
            ultraGridBand1.Columns.AddRange(new object[] {
            ultraGridColumn1,
            ultraGridColumn2,
            ultraGridColumn3,
            ultraGridColumn4});
            this._blockedGrid.DisplayLayout.BandsSerializer.Add(ultraGridBand1);
            this._blockedGrid.DisplayLayout.BorderStyle = Infragistics.Win.UIElementBorderStyle.Solid;
            this._blockedGrid.DisplayLayout.CaptionVisible = Infragistics.Win.DefaultableBoolean.False;
            appearance17.BackColor = System.Drawing.SystemColors.ActiveBorder;
            appearance17.BackColor2 = System.Drawing.SystemColors.ControlDark;
            appearance17.BackGradientStyle = Infragistics.Win.GradientStyle.Vertical;
            appearance17.BorderColor = System.Drawing.SystemColors.Window;
            this._blockedGrid.DisplayLayout.GroupByBox.Appearance = appearance17;
            appearance18.ForeColor = System.Drawing.SystemColors.GrayText;
            this._blockedGrid.DisplayLayout.GroupByBox.BandLabelAppearance = appearance18;
            this._blockedGrid.DisplayLayout.GroupByBox.BorderStyle = Infragistics.Win.UIElementBorderStyle.Solid;
            this._blockedGrid.DisplayLayout.GroupByBox.Hidden = true;
            appearance19.BackColor = System.Drawing.SystemColors.ControlLightLight;
            appearance19.BackColor2 = System.Drawing.SystemColors.Control;
            appearance19.BackGradientStyle = Infragistics.Win.GradientStyle.Horizontal;
            appearance19.ForeColor = System.Drawing.SystemColors.GrayText;
            this._blockedGrid.DisplayLayout.GroupByBox.PromptAppearance = appearance19;
            this._blockedGrid.DisplayLayout.MaxColScrollRegions = 1;
            this._blockedGrid.DisplayLayout.MaxRowScrollRegions = 1;
            this._blockedGrid.DisplayLayout.Override.AllowAddNew = Infragistics.Win.UltraWinGrid.AllowAddNew.No;
            this._blockedGrid.DisplayLayout.Override.AllowColMoving = Infragistics.Win.UltraWinGrid.AllowColMoving.NotAllowed;
            this._blockedGrid.DisplayLayout.Override.AllowColSwapping = Infragistics.Win.UltraWinGrid.AllowColSwapping.NotAllowed;
            this._blockedGrid.DisplayLayout.Override.AllowDelete = Infragistics.Win.DefaultableBoolean.False;
            this._blockedGrid.DisplayLayout.Override.AllowUpdate = Infragistics.Win.DefaultableBoolean.False;
            this._blockedGrid.DisplayLayout.Override.BorderStyleCell = Infragistics.Win.UIElementBorderStyle.Solid;
            this._blockedGrid.DisplayLayout.Override.BorderStyleRow = Infragistics.Win.UIElementBorderStyle.None;
            appearance20.BackColor = System.Drawing.SystemColors.Window;
            this._blockedGrid.DisplayLayout.Override.CardAreaAppearance = appearance20;
            appearance21.BorderColor = System.Drawing.Color.Silver;
            appearance21.TextTrimming = Infragistics.Win.TextTrimming.EllipsisCharacter;
            appearance21.TextVAlignAsString = "Middle";
            this._blockedGrid.DisplayLayout.Override.CellAppearance = appearance21;
            this._blockedGrid.DisplayLayout.Override.CellClickAction = Infragistics.Win.UltraWinGrid.CellClickAction.RowSelect;
            this._blockedGrid.DisplayLayout.Override.CellMultiLine = Infragistics.Win.DefaultableBoolean.False;
            this._blockedGrid.DisplayLayout.Override.CellPadding = 0;
            this._blockedGrid.DisplayLayout.Override.ColumnAutoSizeMode = Infragistics.Win.UltraWinGrid.ColumnAutoSizeMode.AllRowsInBand;
            this._blockedGrid.DisplayLayout.Override.ColumnHeaderTextOrientation = new Infragistics.Win.TextOrientationInfo(0, Infragistics.Win.TextFlowDirection.Horizontal);
            this._blockedGrid.DisplayLayout.Override.GroupByColumnsHidden = Infragistics.Win.DefaultableBoolean.False;
            appearance22.BackColor = System.Drawing.SystemColors.Control;
            appearance22.BackColor2 = System.Drawing.SystemColors.ControlDark;
            appearance22.BackGradientAlignment = Infragistics.Win.GradientAlignment.Element;
            appearance22.BackGradientStyle = Infragistics.Win.GradientStyle.Horizontal;
            appearance22.BorderColor = System.Drawing.SystemColors.Window;
            this._blockedGrid.DisplayLayout.Override.GroupByRowAppearance = appearance22;
            this._blockedGrid.DisplayLayout.Override.GroupByRowDescriptionMask = "[value] ([count] [count,items,item,items])";
            appearance23.TextHAlignAsString = "Left";
            this._blockedGrid.DisplayLayout.Override.HeaderAppearance = appearance23;
            this._blockedGrid.DisplayLayout.Override.HeaderClickAction = Infragistics.Win.UltraWinGrid.HeaderClickAction.SortMulti;
            this._blockedGrid.DisplayLayout.Override.HeaderStyle = Infragistics.Win.HeaderStyle.Standard;
            this._blockedGrid.DisplayLayout.Override.MinRowHeight = 22;
            appearance24.BackColor = System.Drawing.SystemColors.Window;
            appearance24.BorderColor = System.Drawing.Color.Silver;
            this._blockedGrid.DisplayLayout.Override.RowAppearance = appearance24;
            this._blockedGrid.DisplayLayout.Override.RowSelectors = Infragistics.Win.DefaultableBoolean.False;
            this._blockedGrid.DisplayLayout.Override.SelectTypeCell = Infragistics.Win.UltraWinGrid.SelectType.None;
            this._blockedGrid.DisplayLayout.Override.SelectTypeCol = Infragistics.Win.UltraWinGrid.SelectType.None;
            this._blockedGrid.DisplayLayout.Override.SelectTypeRow = Infragistics.Win.UltraWinGrid.SelectType.Single;
            this._blockedGrid.DisplayLayout.Override.SummaryDisplayArea = Infragistics.Win.UltraWinGrid.SummaryDisplayAreas.BottomFixed;
            appearance26.BackColor = System.Drawing.SystemColors.ControlLight;
            this._blockedGrid.DisplayLayout.Override.TemplateAddRowAppearance = appearance26;
            this._blockedGrid.DisplayLayout.ScrollBounds = Infragistics.Win.UltraWinGrid.ScrollBounds.ScrollToFill;
            this._blockedGrid.DisplayLayout.ScrollStyle = Infragistics.Win.UltraWinGrid.ScrollStyle.Immediate;
            valueList1.DisplayStyle = Infragistics.Win.ValueListDisplayStyle.Picture;
            valueList1.Key = "Checkboxes";
            appearance27.Image = global::Idera.SQLdm.DesktopClient.Properties.Resources.CheckBoxBackground;
            appearance27.ImageHAlign = Infragistics.Win.HAlign.Left;
            appearance27.ImageVAlign = Infragistics.Win.VAlign.Middle;
            valueListItem3.Appearance = appearance27;
            valueListItem3.DataValue = false;
            valueListItem3.DisplayText = "False";
            appearance28.Image = global::Idera.SQLdm.DesktopClient.Properties.Resources.block32;
            appearance28.ImageHAlign = Infragistics.Win.HAlign.Left;
            appearance28.ImageVAlign = Infragistics.Win.VAlign.Middle;
            valueListItem4.Appearance = appearance28;
            valueListItem4.DataValue = true;
            valueListItem4.DisplayText = "True";
            valueList1.ValueListItems.AddRange(new Infragistics.Win.ValueListItem[] {
            valueListItem3,
            valueListItem4});
            this._blockedGrid.DisplayLayout.ValueLists.AddRange(new Infragistics.Win.ValueList[] {
            valueList1});
            this._blockedGrid.DisplayLayout.ViewStyleBand = Infragistics.Win.UltraWinGrid.ViewStyleBand.OutlookGroupBy;
            this._blockedGrid.Dock = System.Windows.Forms.DockStyle.Fill;
            this._blockedGrid.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this._blockedGrid.Location = new System.Drawing.Point(0, 0);
            this._blockedGrid.Name = "_blockedGrid";
            this._blockedGrid.Size = new System.Drawing.Size(320, 271);
            this._blockedGrid.TabIndex = 3;
            this._blockedGrid.MouseClick += new System.Windows.Forms.MouseEventHandler(this._blockedGrid_MouseClick);
            // 
            // BlockedRecommendationsTab
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this._blockedGrid);
            this.Name = "BlockedRecommendationsTab";
            this.Size = new System.Drawing.Size(320, 271);
            ((System.ComponentModel.ISupportInitialize)(this._blockedGrid)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private Infragistics.Win.UltraWinGrid.UltraGrid _blockedGrid;
    }
}
