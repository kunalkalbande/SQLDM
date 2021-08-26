namespace Idera.SQLdm.DesktopClient.Views.Servers.Server.Analysis
{
    partial class AnalysisResultsList
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
            Infragistics.Win.Appearance appearance19 = new Infragistics.Win.Appearance();
            Infragistics.Win.UltraWinGrid.UltraGridBand ultraGridBand1 = new Infragistics.Win.UltraWinGrid.UltraGridBand("Band 0", -1);
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn1 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("SQLServerID");
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn2 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("AnalysisID");
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn3 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("AnalysisStartTime");
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn4 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("AnalysisDuration");
            Infragistics.Win.Appearance appearance39 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance1 = new Infragistics.Win.Appearance();
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn5 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("Type");
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn6 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("TotalRecommendationCount");
            Infragistics.Win.Appearance appearance2 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance3 = new Infragistics.Win.Appearance();
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn7 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("ComputedRankFactor");
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn8 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("Priority");
            Infragistics.Win.Appearance appearance21 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance22 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance23 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance24 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance25 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance26 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance27 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance28 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance30 = new Infragistics.Win.Appearance();
            Infragistics.Win.ValueList valueList1 = new Controls.CustomControls.CustomValueList(805615918);
            Infragistics.Win.ValueListItem valueListItem14 = new Infragistics.Win.ValueListItem();
            Infragistics.Win.Appearance appearance31 = new Infragistics.Win.Appearance();
            Infragistics.Win.ValueListItem valueListItem15 = new Infragistics.Win.ValueListItem();
            Infragistics.Win.Appearance appearance32 = new Infragistics.Win.Appearance();
            Infragistics.Win.ValueListItem valueListItem16 = new Infragistics.Win.ValueListItem();
            Infragistics.Win.Appearance appearance33 = new Infragistics.Win.Appearance();
            Infragistics.Win.ValueListItem valueListItem17 = new Infragistics.Win.ValueListItem();
            Infragistics.Win.Appearance appearance34 = new Infragistics.Win.Appearance();
            Infragistics.Win.ValueListItem valueListItem18 = new Infragistics.Win.ValueListItem();
            Infragistics.Win.Appearance appearance35 = new Infragistics.Win.Appearance();
            Infragistics.Win.ValueListItem valueListItem19 = new Infragistics.Win.ValueListItem();
            Infragistics.Win.Appearance appearance36 = new Infragistics.Win.Appearance();
            Infragistics.Win.ValueList valueList2 = new Controls.CustomControls.CustomValueList(276968329);
            Infragistics.Win.ValueListItem valueListItem20 = new Infragistics.Win.ValueListItem();
            Infragistics.Win.Appearance appearance37 = new Infragistics.Win.Appearance();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(AnalysisResultsList));
            Infragistics.Win.ValueListItem valueListItem21 = new Infragistics.Win.ValueListItem();
            Infragistics.Win.Appearance appearance38 = new Infragistics.Win.Appearance();
            Infragistics.Win.ValueList valueList3 = new Controls.CustomControls.CustomValueList(1560433853);
            Infragistics.Win.ValueListItem valueListItem22 = new Infragistics.Win.ValueListItem();
            Infragistics.Win.ValueListItem valueListItem23 = new Infragistics.Win.ValueListItem();
            Infragistics.Win.ValueListItem valueListItem24 = new Infragistics.Win.ValueListItem();
            Infragistics.Win.ValueList valueList4 = new Controls.CustomControls.CustomValueList(1560566189);
            Infragistics.Win.ValueListItem valueListItem25 = new Infragistics.Win.ValueListItem();
            Infragistics.Win.ValueListItem valueListItem26 = new Infragistics.Win.ValueListItem();

            this._historyGrid = new Infragistics.Win.UltraWinGrid.UltraGrid();
            this.worker = new System.ComponentModel.BackgroundWorker();
            this._statusLabel = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomLabel();
            this._loadingPanel = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomPanel ();
            this.label3 = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomLabel();
            this._loadingProgressControl = new MRG.Controls.UI.LoadingCircle();
            ((System.ComponentModel.ISupportInitialize)(this._historyGrid)).BeginInit();
            this._loadingPanel.SuspendLayout();
            this.SuspendLayout();

            // 
            // _historyGrid
            // 
            appearance19.BackColor = System.Drawing.SystemColors.Window;
            appearance19.BorderColor = System.Drawing.SystemColors.ControlDark;
            this._historyGrid.DisplayLayout.Appearance = appearance19;
            this._historyGrid.DisplayLayout.AutoFitStyle = Infragistics.Win.UltraWinGrid.AutoFitStyle.ExtendLastColumn;
            ultraGridColumn1.Header.VisiblePosition = 0;
            ultraGridColumn1.Width = 92;
            ultraGridColumn2.Header.VisiblePosition = 1;
            ultraGridColumn2.Width = 92;
            ultraGridColumn2.Format = "G";
            ultraGridColumn3.Header.VisiblePosition = 2;
            ultraGridColumn3.Width = 322;
            appearance39.TextHAlignAsString = "Left";
            ultraGridColumn4.CellAppearance = appearance39;
            ultraGridColumn4.Format = "";
            appearance1.TextHAlignAsString = "Left";
            ultraGridColumn4.Header.Appearance = appearance1;
            ultraGridColumn4.Header.VisiblePosition = 3;
            ultraGridColumn4.Width = 122;
            ultraGridColumn5.Header.VisiblePosition = 4;
            ultraGridColumn5.Width = 238;
            appearance2.TextHAlignAsString = "Left";
            ultraGridColumn6.CellAppearance = appearance2;
            appearance3.TextHAlignAsString = "Left";
            ultraGridColumn6.Header.Appearance = appearance3;
            ultraGridColumn6.Header.VisiblePosition = 5;
            ultraGridColumn6.Width = 218;
            ultraGridColumn7.Header.VisiblePosition = 6;
            ultraGridColumn7.Style = Infragistics.Win.UltraWinGrid.ColumnStyle.Image;
            ultraGridColumn7.Width = 176;
            ultraGridColumn8.Header.VisiblePosition = 7;
            ultraGridColumn8.Style = Infragistics.Win.UltraWinGrid.ColumnStyle.Image;
            ultraGridColumn8.Width = 176;
            ultraGridBand1.Columns.AddRange(new object[] {
            ultraGridColumn1,
            ultraGridColumn2,
            ultraGridColumn3,
            ultraGridColumn4,
            ultraGridColumn5,
            ultraGridColumn6,
            ultraGridColumn7,
            ultraGridColumn8});
            this._historyGrid.DisplayLayout.BandsSerializer.Add(ultraGridBand1);
            this._historyGrid.DisplayLayout.BorderStyle = Infragistics.Win.UIElementBorderStyle.Solid;
            this._historyGrid.DisplayLayout.CaptionVisible = Infragistics.Win.DefaultableBoolean.False;
            appearance21.BackColor = System.Drawing.SystemColors.ActiveBorder;
            appearance21.BackColor2 = System.Drawing.SystemColors.ControlDark;
            appearance21.BackGradientStyle = Infragistics.Win.GradientStyle.Vertical;
            appearance21.BorderColor = System.Drawing.SystemColors.Window;
            this._historyGrid.DisplayLayout.GroupByBox.Appearance = appearance21;
            appearance22.ForeColor = System.Drawing.SystemColors.GrayText;
            this._historyGrid.DisplayLayout.GroupByBox.BandLabelAppearance = appearance22;
            this._historyGrid.DisplayLayout.GroupByBox.BorderStyle = Infragistics.Win.UIElementBorderStyle.Solid;
            this._historyGrid.DisplayLayout.GroupByBox.Hidden = true;
            appearance23.BackColor = System.Drawing.SystemColors.ControlLightLight;
            appearance23.BackColor2 = System.Drawing.SystemColors.Control;
            appearance23.BackGradientStyle = Infragistics.Win.GradientStyle.Horizontal;
            appearance23.ForeColor = System.Drawing.SystemColors.GrayText;
            this._historyGrid.DisplayLayout.GroupByBox.PromptAppearance = appearance23;
            this._historyGrid.DisplayLayout.MaxColScrollRegions = 1;
            this._historyGrid.DisplayLayout.MaxRowScrollRegions = 1;
            this._historyGrid.DisplayLayout.Override.AllowAddNew = Infragistics.Win.UltraWinGrid.AllowAddNew.No;
            this._historyGrid.DisplayLayout.Override.AllowColMoving = Infragistics.Win.UltraWinGrid.AllowColMoving.NotAllowed;
            this._historyGrid.DisplayLayout.Override.AllowColSwapping = Infragistics.Win.UltraWinGrid.AllowColSwapping.NotAllowed;
            this._historyGrid.DisplayLayout.Override.AllowDelete = Infragistics.Win.DefaultableBoolean.False;
            this._historyGrid.DisplayLayout.Override.AllowUpdate = Infragistics.Win.DefaultableBoolean.False;
            this._historyGrid.DisplayLayout.Override.BorderStyleCell = Infragistics.Win.UIElementBorderStyle.Solid;
            this._historyGrid.DisplayLayout.Override.BorderStyleRow = Infragistics.Win.UIElementBorderStyle.None;
            appearance24.BackColor = System.Drawing.SystemColors.Window;
            this._historyGrid.DisplayLayout.Override.CardAreaAppearance = appearance24;
            appearance25.BorderColor = System.Drawing.Color.Silver;
            appearance25.TextTrimming = Infragistics.Win.TextTrimming.EllipsisCharacter;
            this._historyGrid.DisplayLayout.Override.CellAppearance = appearance25;
            this._historyGrid.DisplayLayout.Override.CellClickAction = Infragistics.Win.UltraWinGrid.CellClickAction.RowSelect;
            this._historyGrid.DisplayLayout.Override.CellMultiLine = Infragistics.Win.DefaultableBoolean.False;
            this._historyGrid.DisplayLayout.Override.CellPadding = 0;
            this._historyGrid.DisplayLayout.Override.ColumnAutoSizeMode = Infragistics.Win.UltraWinGrid.ColumnAutoSizeMode.AllRowsInBand;
            this._historyGrid.DisplayLayout.Override.ColumnHeaderTextOrientation = new Infragistics.Win.TextOrientationInfo(0, Infragistics.Win.TextFlowDirection.Horizontal);
            this._historyGrid.DisplayLayout.Override.GroupByColumnsHidden = Infragistics.Win.DefaultableBoolean.False;
            appearance26.BackColor = System.Drawing.SystemColors.Control;
            appearance26.BackColor2 = System.Drawing.SystemColors.ControlDark;
            appearance26.BackGradientAlignment = Infragistics.Win.GradientAlignment.Element;
            appearance26.BackGradientStyle = Infragistics.Win.GradientStyle.Horizontal;
            appearance26.BorderColor = System.Drawing.SystemColors.Window;
            this._historyGrid.DisplayLayout.Override.GroupByRowAppearance = appearance26;
            this._historyGrid.DisplayLayout.Override.GroupByRowDescriptionMask = "[value] ([count] [count,items,item,items])";
            appearance27.TextHAlignAsString = "Left";
            this._historyGrid.DisplayLayout.Override.HeaderAppearance = appearance27;
            this._historyGrid.DisplayLayout.Override.HeaderClickAction = Infragistics.Win.UltraWinGrid.HeaderClickAction.SortMulti;
            this._historyGrid.DisplayLayout.Override.HeaderStyle = Infragistics.Win.HeaderStyle.Standard;
            appearance28.BackColor = System.Drawing.SystemColors.Window;
            appearance28.BorderColor = System.Drawing.Color.Silver;
            this._historyGrid.DisplayLayout.Override.RowAppearance = appearance28;
            this._historyGrid.DisplayLayout.Override.RowSelectors = Infragistics.Win.DefaultableBoolean.False;
            this._historyGrid.DisplayLayout.Override.SelectTypeCell = Infragistics.Win.UltraWinGrid.SelectType.None;
            this._historyGrid.DisplayLayout.Override.SelectTypeCol = Infragistics.Win.UltraWinGrid.SelectType.None;
            this._historyGrid.DisplayLayout.Override.SelectTypeRow = Infragistics.Win.UltraWinGrid.SelectType.Extended;
            this._historyGrid.DisplayLayout.Override.SummaryDisplayArea = Infragistics.Win.UltraWinGrid.SummaryDisplayAreas.BottomFixed;
            appearance30.BackColor = System.Drawing.SystemColors.ControlLight;
            this._historyGrid.DisplayLayout.Override.TemplateAddRowAppearance = appearance30;
            this._historyGrid.DisplayLayout.ScrollBounds = Infragistics.Win.UltraWinGrid.ScrollBounds.ScrollToFill;
            this._historyGrid.DisplayLayout.ScrollStyle = Infragistics.Win.UltraWinGrid.ScrollStyle.Immediate;
            valueList1.Key = "ServerHealthValueList";
            appearance31.BackColor = System.Drawing.Color.Red;
            appearance31.ForeColor = System.Drawing.Color.White;
            valueListItem14.Appearance = appearance31;
            valueListItem14.DataValue = 4;
            valueListItem14.DisplayText = "Critical";
            appearance32.BackColor = System.Drawing.Color.DarkOrange;
            appearance32.ForeColor = System.Drawing.Color.White;
            valueListItem15.Appearance = appearance32;
            valueListItem15.DataValue = 3;
            valueListItem15.DisplayText = "Severe";
            appearance33.BackColor = System.Drawing.Color.Blue;
            appearance33.ForeColor = System.Drawing.Color.White;
            valueListItem16.Appearance = appearance33;
            valueListItem16.DataValue = 2;
            valueListItem16.DisplayText = "Fair";
            appearance34.BackColor = System.Drawing.Color.Green;
            appearance34.ForeColor = System.Drawing.Color.White;
            valueListItem17.Appearance = appearance34;
            valueListItem17.DataValue = 1;
            valueListItem17.DisplayText = "Good";
            appearance35.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(135)))), ((int)(((byte)(27)))), ((int)(((byte)(126)))));
            appearance35.ForeColor = System.Drawing.Color.White;
            valueListItem18.Appearance = appearance35;
            valueListItem18.DataValue = 0;
            valueListItem18.DisplayText = "Undetermined";
            appearance36.BackColor = System.Drawing.Color.White;
            appearance36.ForeColor = System.Drawing.Color.Black;
            valueListItem19.Appearance = appearance36;
            valueListItem19.DataValue = 5;
            valueListItem19.DisplayText = "Not Applicable";
            valueList1.ValueListItems.AddRange(new Infragistics.Win.ValueListItem[] {
            valueListItem14,
            valueListItem15,
            valueListItem16,
            valueListItem17,
            valueListItem18,
            valueListItem19});
            valueList2.DisplayStyle = Infragistics.Win.ValueListDisplayStyle.Picture;
            valueList2.Key = "Checkboxes";
            appearance37.Image = ((object)(resources.GetObject("appearance37.Image")));
            appearance37.ImageHAlign = Infragistics.Win.HAlign.Center;
            valueListItem20.Appearance = appearance37;
            valueListItem20.DataValue = false;
            valueListItem20.DisplayText = "False";
            appearance38.Image = ((object)(resources.GetObject("appearance38.Image")));
            appearance38.ImageHAlign = Infragistics.Win.HAlign.Center;
            valueListItem21.Appearance = appearance38;
            valueListItem21.DataValue = true;
            valueListItem21.DisplayText = "True";
            valueList2.ValueListItems.AddRange(new Infragistics.Win.ValueListItem[] {
            valueListItem20,
            valueListItem21});
            valueList3.DisplayStyle = Infragistics.Win.ValueListDisplayStyle.DisplayText;
            valueList3.Key = "TaskTypeList";
            valueListItem22.DataValue = 0;
            valueListItem22.DisplayText = "Analysis";
            valueListItem23.DataValue = 1;
            valueListItem23.DisplayText = "Scheduled";
            valueListItem24.DataValue = 2;
            valueListItem24.DisplayText = "Benchmark";
            valueList3.ValueListItems.AddRange(new Infragistics.Win.ValueListItem[] {
            valueListItem22,
            valueListItem23,
            valueListItem24});
            valueList4.DisplayStyle = Infragistics.Win.ValueListDisplayStyle.DisplayText;
            valueList4.Key = "ScheduledList";
            valueListItem25.DataValue = false;
            valueListItem25.DisplayText = "User";
            valueListItem26.DataValue = true;
            valueListItem26.DisplayText = "Scheduler";
            valueList4.ValueListItems.AddRange(new Infragistics.Win.ValueListItem[] {
            valueListItem25,
            valueListItem26});
            this._historyGrid.DisplayLayout.ValueLists.AddRange(new Infragistics.Win.ValueList[] {
            valueList1,
            valueList2,
            valueList3,
            valueList4});
            this._historyGrid.DisplayLayout.ViewStyleBand = Infragistics.Win.UltraWinGrid.ViewStyleBand.OutlookGroupBy;
            this._historyGrid.Dock = System.Windows.Forms.DockStyle.Fill;
            this._historyGrid.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this._historyGrid.Location = new System.Drawing.Point(35, 41);
            this._historyGrid.Name = "_historyGrid";
            this._historyGrid.Size = new System.Drawing.Size(632, 298);
            this._historyGrid.TabIndex = 1;
            this._historyGrid.Visible = true;
            this._historyGrid.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler(this._historyGrid_MouseDoubleClick);
            this._historyGrid.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this._historyGrid_KeyPress);
            // 
            // worker
            // 
            this.worker.DoWork += new System.ComponentModel.DoWorkEventHandler(this.worker_DoWork);
            this.worker.RunWorkerCompleted += new System.ComponentModel.RunWorkerCompletedEventHandler(this.worker_RunWorkerCompleted);
            // 
            // _statusLabel
            // 
            this._statusLabel.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
            | System.Windows.Forms.AnchorStyles.Right)));
            this._statusLabel.BackColor = System.Drawing.Color.White;
            this._statusLabel.Font = new System.Drawing.Font("Arial", 12F, System.Drawing.FontStyle.Bold);
            this._statusLabel.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(96)))), ((int)(((byte)(96)))), ((int)(((byte)(96)))));
            this._statusLabel.Location = new System.Drawing.Point(96, 52);
            this._statusLabel.Name = "_statusLabel";
            this._statusLabel.Size = new System.Drawing.Size(440, 19);
            this._statusLabel.TabIndex = 3;
            this._statusLabel.Text = "< Status Label >";
            this._statusLabel.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this._statusLabel.Visible = false;
            // 
            // _loadingPanel
            // 
            this._loadingPanel.BackColor = System.Drawing.Color.White;
            this._loadingPanel.Controls.Add(this.label3);
            this._loadingPanel.Controls.Add(this._loadingProgressControl);
            this._loadingPanel.Location = new System.Drawing.Point(28, 41);
            this._loadingPanel.Name = "_loadingPanel";
            this._loadingPanel.Size = new System.Drawing.Size(128, 40);
            this._loadingPanel.TabIndex = 40;
            this._loadingPanel.Visible = false;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.ForeColor = System.Drawing.Color.DimGray;
            this.label3.Location = new System.Drawing.Point(57, 14);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(54, 13);
            this.label3.TabIndex = 2;
            this.label3.Text = "Loading...";
            // 
            // _loadingProgressControl
            // 
            this._loadingProgressControl.Active = false;
            this._loadingProgressControl.Color = System.Drawing.Color.DarkGray;
            this._loadingProgressControl.InnerCircleRadius = 5;
            this._loadingProgressControl.Location = new System.Drawing.Point(17, 3);
            this._loadingProgressControl.Name = "_loadingProgressControl";
            this._loadingProgressControl.NumberSpoke = 12;
            this._loadingProgressControl.OuterCircleRadius = 11;
            this._loadingProgressControl.RotationSpeed = 100;
            this._loadingProgressControl.Size = new System.Drawing.Size(34, 34);
            this._loadingProgressControl.SpokeThickness = 2;
            this._loadingProgressControl.StylePreset = MRG.Controls.UI.LoadingCircle.StylePresets.MacOSX;
            this._loadingProgressControl.TabIndex = 1;
            // 
            // AnalysisResultsList
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.BackColor = System.Drawing.Color.Transparent;
            this.Controls.Add(this._loadingPanel);
            this.Controls.Add(this._statusLabel);
            this.Controls.Add(this._historyGrid);
            this.Name = "AnalysisResultsList";
            this.Size = new System.Drawing.Size(632, 337);
            ((System.ComponentModel.ISupportInitialize)(this._historyGrid)).EndInit();
            this._loadingPanel.ResumeLayout(false);
            this._loadingPanel.PerformLayout();
            this.ResumeLayout(false);
        }

        #endregion
        private Infragistics.Win.UltraWinGrid.UltraGrid _historyGrid;
        private System.ComponentModel.BackgroundWorker worker;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomLabel _statusLabel;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomPanel  _loadingPanel;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomLabel label3;
        private MRG.Controls.UI.LoadingCircle _loadingProgressControl;
    }
}