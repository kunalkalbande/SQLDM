using System.Windows.Forms;

namespace Idera.SQLdm.DesktopClient.Dialogs
{
    using Idera.SQLdm.Common.Data;

    partial class AlertRecommendationsDialog
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(AlertRecommendationsDialog));
            Infragistics.Win.Appearance appearance35 = new Infragistics.Win.Appearance();
            Infragistics.Win.UltraWinGrid.UltraGridBand ultraGridBand1 = new Infragistics.Win.UltraWinGrid.UltraGridBand("AlertBaselineEntry", -1);
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn1 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("Selected");
            Infragistics.Win.Appearance appearance1 = new Infragistics.Win.Appearance();
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn2 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("Tweaked");
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn3 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("Name", -1, null, 0, Infragistics.Win.UltraWinGrid.SortIndicator.Ascending, false);
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn4 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("Category");
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn5 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("ReferenceRange");
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn6 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("WarningThreshold");
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn7 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("CriticalThreshold");
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn8 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("GaugeWarningValue");
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn9 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("GaugeCriticalValue");
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn10 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("SuggestedWarning");
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn11 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("SuggestedCritical");
            Infragistics.Win.Appearance appearance36 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance49 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance50 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance51 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance52 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance53 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance54 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance55 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance56 = new Infragistics.Win.Appearance();
            Infragistics.Win.ValueList valueList1 = new Controls.CustomControls.CustomValueList(426658501);
            Infragistics.Win.ValueListItem valueListItem1 = new Infragistics.Win.ValueListItem();
            Infragistics.Win.ValueListItem valueListItem2 = new Infragistics.Win.ValueListItem();
            Infragistics.Win.ValueListItem valueListItem3 = new Infragistics.Win.ValueListItem();
             linearScale1 = new ChartFX.WinForms.Gauge.LinearScale();
            ChartFX.WinForms.Gauge.Marker marker1 = new ChartFX.WinForms.Gauge.Marker();
            ChartFX.WinForms.Gauge.Marker marker2 = new ChartFX.WinForms.Gauge.Marker();
            ChartFX.WinForms.Gauge.Section section1 = new ChartFX.WinForms.Gauge.Section();
            ChartFX.WinForms.Gauge.Section section2 = new ChartFX.WinForms.Gauge.Section();
            ChartFX.WinForms.Gauge.Section section3 = new ChartFX.WinForms.Gauge.Section();
            ChartFX.WinForms.Gauge.LinearStrip linearStrip1 = new ChartFX.WinForms.Gauge.LinearStrip();
            this.cancelButton = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomButton();
            this.okButton = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomButton();
            this.gridBindingSource = new System.Windows.Forms.BindingSource(this.components);
            this.refreshSuggestionsWorker = new System.ComponentModel.BackgroundWorker();
            this.office2007PropertyPage1 = new Idera.SQLdm.DesktopClient.Controls.Office2007PropertyPage(isDarkThemeSelected);
            this.dividerProgressBar2 = new Idera.SQLdm.DesktopClient.Controls.InfiniteProgressBar();
            this.label1 = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomLabel();
            this.recommendationsGrid = new Infragistics.Win.UltraWinGrid.UltraGrid();
            this.applyAllCheckBox = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomCheckBox();
            this.alertConfigurationGroupBox = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomGroupBox();
            this.alertConfigurationGauge = new ChartFX.WinForms.Gauge.HorizontalGauge();
            this.flowLayoutPanel1 = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomFlowLayoutPanel();
            this.alertRecommendationOptionsButton = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomButton();
            this.configureBaselineButton = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomButton();
            this.mainContainerPanel = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomTableLayoutPanel();
            ((System.ComponentModel.ISupportInitialize)(this.gridBindingSource)).BeginInit();
            this.office2007PropertyPage1.ContentPanel.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.recommendationsGrid)).BeginInit();
            this.alertConfigurationGroupBox.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.alertConfigurationGauge)).BeginInit();
            this.flowLayoutPanel1.SuspendLayout();
            this.mainContainerPanel.SuspendLayout();
            this.SuspendLayout();
            // 
            // cancelButton
            // 
            this.cancelButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.cancelButton.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.cancelButton.Location = new System.Drawing.Point(462, 446);
            this.cancelButton.Name = "cancelButton";
            this.cancelButton.Size = new System.Drawing.Size(75, 23);
            this.cancelButton.TabIndex = 4;
            this.cancelButton.Text = "Cancel";
            this.cancelButton.UseVisualStyleBackColor = true;
            // 
            // okButton
            // 
            this.okButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.okButton.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.okButton.Location = new System.Drawing.Point(381, 446);
            this.okButton.Name = "okButton";
            this.okButton.Size = new System.Drawing.Size(75, 23);
            this.okButton.TabIndex = 3;
            this.okButton.Text = "OK";
            this.okButton.UseVisualStyleBackColor = true;
            this.okButton.Click += new System.EventHandler(this.okButton_Click);
            // 
            // gridBindingSource
            // 
            this.gridBindingSource.AllowNew = false;
            this.gridBindingSource.DataSource = typeof(Idera.SQLdm.DesktopClient.Dialogs.AlertRecommendationsDialog.AlertBaselineEntry);
            this.gridBindingSource.CurrentChanged += new System.EventHandler(this.gridBindingSource_CurrentChanged);
            // 
            // refreshSuggestionsWorker
            // 
            this.refreshSuggestionsWorker.DoWork += new System.ComponentModel.DoWorkEventHandler(this.refreshSuggestionsWorker_DoWork);
            this.refreshSuggestionsWorker.RunWorkerCompleted += new System.ComponentModel.RunWorkerCompletedEventHandler(this.refreshSuggestionsWorker_RunWorkerCompleted);
            // 
            // office2007PropertyPage1
            // 
            this.office2007PropertyPage1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.office2007PropertyPage1.BackColor = System.Drawing.Color.White;
            this.office2007PropertyPage1.BorderWidth = 1;
            // 
            // office2007PropertyPage1.ContentPanel
            // 
            this.office2007PropertyPage1.ContentPanel.BackColor2 = System.Drawing.Color.White;
            this.office2007PropertyPage1.ContentPanel.BorderColor = System.Drawing.SystemColors.ControlDark;
            this.office2007PropertyPage1.ContentPanel.Controls.Add(this.dividerProgressBar2);
            this.office2007PropertyPage1.ContentPanel.Controls.Add(this.mainContainerPanel);
            this.office2007PropertyPage1.ContentPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.office2007PropertyPage1.ContentPanel.FillStyle = Idera.SQLdm.DesktopClient.Controls.GradientPanelFillStyle.Solid;
            this.office2007PropertyPage1.ContentPanel.Location = new System.Drawing.Point(1, 56);
            this.office2007PropertyPage1.ContentPanel.Name = "ContentPanel";
            this.office2007PropertyPage1.ContentPanel.Padding = new System.Windows.Forms.Padding(1);
            this.office2007PropertyPage1.ContentPanel.ShowBorder = false;
            this.office2007PropertyPage1.ContentPanel.Size = new System.Drawing.Size(523, 371);
            this.office2007PropertyPage1.ContentPanel.TabIndex = 1;
            this.office2007PropertyPage1.HeaderImage = global::Idera.SQLdm.DesktopClient.Properties.Resources.AlertsFeature1;
            this.office2007PropertyPage1.Location = new System.Drawing.Point(12, 12);
            this.office2007PropertyPage1.Name = "office2007PropertyPage1";
            this.office2007PropertyPage1.Size = new System.Drawing.Size(525, 428);
            this.office2007PropertyPage1.TabIndex = 5;
            this.office2007PropertyPage1.Text = "Select the alert recommendation you would like to apply for this server.";
            // 
            // dividerProgressBar2
            // 
            this.dividerProgressBar2.Anchor = System.Windows.Forms.AnchorStyles.Top;
            this.dividerProgressBar2.Color1 = System.Drawing.Color.DarkGray;
            this.dividerProgressBar2.Color2 = System.Drawing.Color.White;
            this.dividerProgressBar2.Location = new System.Drawing.Point(0, 0);
            this.dividerProgressBar2.Name = "dividerProgressBar2";
            this.dividerProgressBar2.Size = new System.Drawing.Size(475, 2);
            this.dividerProgressBar2.Speed = 15D;
            this.dividerProgressBar2.Step = 5F;
            this.dividerProgressBar2.TabIndex = 11;
            // 
            // label1
            // 
            this.label1.Dock = DockStyle.Fill;
            this.label1.AutoEllipsis = true;
            this.label1.Location = new System.Drawing.Point(10, 7);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(504, 64);
            this.label1.TabIndex = 10;
            this.label1.Text = resources.GetString("label1.Text");
            // 
            // recommendationsGrid
            // 
            this.recommendationsGrid.Dock = DockStyle.Fill;
            this.recommendationsGrid.DataSource = this.gridBindingSource;
            appearance35.BackColor = System.Drawing.SystemColors.Window;
            appearance35.BorderColor = System.Drawing.Color.Silver;
            this.recommendationsGrid.DisplayLayout.Appearance = appearance35;
            this.recommendationsGrid.DisplayLayout.AutoFitStyle = Infragistics.Win.UltraWinGrid.AutoFitStyle.ResizeAllColumns;
            appearance1.TextHAlignAsString = "Center";
            ultraGridColumn1.Header.Appearance = appearance1;
            ultraGridColumn1.Header.Caption = "Apply";
            ultraGridColumn1.Header.VisiblePosition = 0;
            ultraGridColumn2.Header.VisiblePosition = 1;
            ultraGridColumn2.Hidden = true;
            ultraGridColumn3.Header.VisiblePosition = 2;
            ultraGridColumn3.Width = 182;
            ultraGridColumn4.Header.VisiblePosition = 6;
            ultraGridColumn4.Hidden = true;
            ultraGridColumn5.Header.VisiblePosition = 5;
            ultraGridColumn5.Hidden = true;
            ultraGridColumn6.Header.VisiblePosition = 7;
            ultraGridColumn6.Hidden = true;
            ultraGridColumn7.Header.VisiblePosition = 9;
            ultraGridColumn7.Hidden = true;
            ultraGridColumn8.Header.VisiblePosition = 8;
            ultraGridColumn8.Hidden = true;
            ultraGridColumn9.Header.VisiblePosition = 10;
            ultraGridColumn9.Hidden = true;
            ultraGridColumn10.Header.Caption = "Recommended Warning";
            ultraGridColumn10.Header.VisiblePosition = 3;
            ultraGridColumn10.Width = 133;
            ultraGridColumn11.Header.Caption = "Recommended Critical";
            ultraGridColumn11.Header.VisiblePosition = 4;
            ultraGridBand1.Columns.AddRange(new object[] {
            ultraGridColumn1,
            ultraGridColumn2,
            ultraGridColumn3,
            ultraGridColumn4,
            ultraGridColumn5,
            ultraGridColumn6,
            ultraGridColumn7,
            ultraGridColumn8,
            ultraGridColumn9,
            ultraGridColumn10,
            ultraGridColumn11});
            this.recommendationsGrid.DisplayLayout.BandsSerializer.Add(ultraGridBand1);
            this.recommendationsGrid.DisplayLayout.BorderStyle = Infragistics.Win.UIElementBorderStyle.Solid;
            this.recommendationsGrid.DisplayLayout.CaptionVisible = Infragistics.Win.DefaultableBoolean.False;
            appearance36.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(227)))), ((int)(((byte)(239)))), ((int)(((byte)(255)))));
            appearance36.BackColor2 = System.Drawing.SystemColors.ControlDark;
            appearance36.BackGradientStyle = Infragistics.Win.GradientStyle.None;
            appearance36.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(101)))), ((int)(((byte)(147)))), ((int)(((byte)(207)))));
            this.recommendationsGrid.DisplayLayout.GroupByBox.Appearance = appearance36;
            appearance49.ForeColor = System.Drawing.SystemColors.GrayText;
            this.recommendationsGrid.DisplayLayout.GroupByBox.BandLabelAppearance = appearance49;
            this.recommendationsGrid.DisplayLayout.GroupByBox.BorderStyle = Infragistics.Win.UIElementBorderStyle.Solid;
            this.recommendationsGrid.DisplayLayout.GroupByBox.Hidden = true;
            appearance50.BackColor = System.Drawing.SystemColors.ControlLightLight;
            appearance50.BackColor2 = System.Drawing.SystemColors.Control;
            appearance50.BackGradientStyle = Infragistics.Win.GradientStyle.Horizontal;
            appearance50.ForeColor = System.Drawing.SystemColors.GrayText;
            this.recommendationsGrid.DisplayLayout.GroupByBox.PromptAppearance = appearance50;
            this.recommendationsGrid.DisplayLayout.MaxColScrollRegions = 1;
            this.recommendationsGrid.DisplayLayout.MaxRowScrollRegions = 1;
            this.recommendationsGrid.DisplayLayout.Override.AllowAddNew = Infragistics.Win.UltraWinGrid.AllowAddNew.No;
            this.recommendationsGrid.DisplayLayout.Override.AllowDelete = Infragistics.Win.DefaultableBoolean.False;
            this.recommendationsGrid.DisplayLayout.Override.AllowUpdate = Infragistics.Win.DefaultableBoolean.False;
            this.recommendationsGrid.DisplayLayout.Override.BorderStyleCell = Infragistics.Win.UIElementBorderStyle.Solid;
            this.recommendationsGrid.DisplayLayout.Override.BorderStyleRow = Infragistics.Win.UIElementBorderStyle.Solid;
            appearance51.BackColor = System.Drawing.SystemColors.Window;
            this.recommendationsGrid.DisplayLayout.Override.CardAreaAppearance = appearance51;
            appearance52.BorderColor = System.Drawing.Color.Silver;
            appearance52.TextTrimming = Infragistics.Win.TextTrimming.EllipsisCharacter;
            this.recommendationsGrid.DisplayLayout.Override.CellAppearance = appearance52;
            this.recommendationsGrid.DisplayLayout.Override.CellClickAction = Infragistics.Win.UltraWinGrid.CellClickAction.RowSelect;
            this.recommendationsGrid.DisplayLayout.Override.CellPadding = 0;
            this.recommendationsGrid.DisplayLayout.Override.FixedHeaderIndicator = Infragistics.Win.UltraWinGrid.FixedHeaderIndicator.None;
            this.recommendationsGrid.DisplayLayout.Override.GroupByColumnsHidden = Infragistics.Win.DefaultableBoolean.False;
            appearance53.BackColor = System.Drawing.SystemColors.Control;
            appearance53.BackColor2 = System.Drawing.SystemColors.ControlDark;
            appearance53.BackGradientAlignment = Infragistics.Win.GradientAlignment.Element;
            appearance53.BackGradientStyle = Infragistics.Win.GradientStyle.Horizontal;
            appearance53.BorderColor = System.Drawing.SystemColors.Window;
            this.recommendationsGrid.DisplayLayout.Override.GroupByRowAppearance = appearance53;
            this.recommendationsGrid.DisplayLayout.Override.GroupByRowInitialExpansionState = Infragistics.Win.UltraWinGrid.GroupByRowInitialExpansionState.Expanded;
            this.recommendationsGrid.DisplayLayout.Override.GroupBySummaryDisplayStyle = Infragistics.Win.UltraWinGrid.GroupBySummaryDisplayStyle.SummaryCells;
            appearance54.TextHAlignAsString = "Left";
            this.recommendationsGrid.DisplayLayout.Override.HeaderAppearance = appearance54;
            this.recommendationsGrid.DisplayLayout.Override.HeaderClickAction = Infragistics.Win.UltraWinGrid.HeaderClickAction.SortMulti;
            appearance55.BackColor = System.Drawing.SystemColors.Window;
            appearance55.BorderColor = System.Drawing.Color.Silver;
            this.recommendationsGrid.DisplayLayout.Override.RowAppearance = appearance55;
            this.recommendationsGrid.DisplayLayout.Override.RowSelectors = Infragistics.Win.DefaultableBoolean.False;
            this.recommendationsGrid.DisplayLayout.Override.SelectTypeCell = Infragistics.Win.UltraWinGrid.SelectType.None;
            this.recommendationsGrid.DisplayLayout.Override.SelectTypeCol = Infragistics.Win.UltraWinGrid.SelectType.None;
            this.recommendationsGrid.DisplayLayout.Override.SelectTypeGroupByRow = Infragistics.Win.UltraWinGrid.SelectType.None;
            this.recommendationsGrid.DisplayLayout.Override.SelectTypeRow = Infragistics.Win.UltraWinGrid.SelectType.Single;
            this.recommendationsGrid.DisplayLayout.Override.SummaryDisplayArea = Infragistics.Win.UltraWinGrid.SummaryDisplayAreas.InGroupByRows;
            appearance56.BackColor = System.Drawing.SystemColors.ControlLight;
            this.recommendationsGrid.DisplayLayout.Override.TemplateAddRowAppearance = appearance56;
            this.recommendationsGrid.DisplayLayout.Override.TipStyleCell = Infragistics.Win.UltraWinGrid.TipStyle.Hide;
            this.recommendationsGrid.DisplayLayout.ScrollBounds = Infragistics.Win.UltraWinGrid.ScrollBounds.ScrollToFill;
            this.recommendationsGrid.DisplayLayout.ScrollStyle = Infragistics.Win.UltraWinGrid.ScrollStyle.Immediate;
            this.recommendationsGrid.DisplayLayout.UseFixedHeaders = true;
            valueList1.Key = "EventType";
            valueListItem1.DataValue = 0;
            valueListItem1.DisplayText = "Stored Procedure";
            valueListItem2.DataValue = 1;
            valueListItem2.DisplayText = "SQL Statement";
            valueListItem3.DataValue = 2;
            valueListItem3.DisplayText = "SQL Batch";
            valueList1.ValueListItems.AddRange(new Infragistics.Win.ValueListItem[] {
            valueListItem1,
            valueListItem2,
            valueListItem3});
            this.recommendationsGrid.DisplayLayout.ValueLists.AddRange(new Infragistics.Win.ValueList[] {
            valueList1});
            this.recommendationsGrid.DisplayLayout.ViewStyle = Infragistics.Win.UltraWinGrid.ViewStyle.SingleBand;
            this.recommendationsGrid.DisplayLayout.ViewStyleBand = Infragistics.Win.UltraWinGrid.ViewStyleBand.OutlookGroupBy;
            this.recommendationsGrid.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.recommendationsGrid.Location = new System.Drawing.Point(10, 96);
            this.recommendationsGrid.Name = "recommendationsGrid";
            this.recommendationsGrid.Size = new System.Drawing.Size(504, 116);
            this.recommendationsGrid.TabIndex = 7;
            this.recommendationsGrid.InitializeRow += new Infragistics.Win.UltraWinGrid.InitializeRowEventHandler(this.recommendationsGrid_InitializeRow);
            this.recommendationsGrid.MouseClick += new System.Windows.Forms.MouseEventHandler(this.recommendationsGrid_MouseClick);
            // 
            // applyAllCheckBox
            // 
            this.applyAllCheckBox.AutoSize = true;
            this.applyAllCheckBox.Location = new System.Drawing.Point(13, 74);
            this.applyAllCheckBox.Name = "applyAllCheckBox";
            this.applyAllCheckBox.Size = new System.Drawing.Size(66, 17);
            this.applyAllCheckBox.TabIndex = 6;
            this.applyAllCheckBox.Text = "Apply All";
            this.applyAllCheckBox.UseVisualStyleBackColor = true;
            this.applyAllCheckBox.CheckedChanged += new System.EventHandler(this.applyAllCheckBox_CheckedChanged);
            // 
            // alertConfigurationGroupBox
            // 
            this.alertConfigurationGroupBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.alertConfigurationGroupBox.Controls.Add(this.alertConfigurationGauge);
            this.alertConfigurationGroupBox.Location = new System.Drawing.Point(10, 217);
            this.alertConfigurationGroupBox.Name = "alertConfigurationGroupBox";
            this.alertConfigurationGroupBox.Size = new System.Drawing.Size(504, 85);
            this.alertConfigurationGroupBox.TabIndex = 5;
            this.alertConfigurationGroupBox.TabStop = false;
            this.alertConfigurationGroupBox.Text = "Alert Configuration for {0}";
            // 
            // alertConfigurationGauge
            // 
            this.alertConfigurationGauge.Border.Visible = false;
            this.alertConfigurationGauge.Dock = System.Windows.Forms.DockStyle.Fill;
            this.alertConfigurationGauge.Location = new System.Drawing.Point(3, 16);
            this.alertConfigurationGauge.Name = "alertConfigurationGauge";
            linearScale1.AutoScaleInterval = null;
            linearScale1.Bar.Color = System.Drawing.Color.FromArgb(((int)(((byte)(215)))), ((int)(((byte)(225)))), ((int)(((byte)(243)))));
            linearScale1.Color = System.Drawing.Color.FromArgb(((int)(((byte)(215)))), ((int)(((byte)(225)))), ((int)(((byte)(243)))));
            marker1.Color = System.Drawing.Color.Gold;
            marker1.Draggable = true;
            marker1.Format.Decimals = 0;
            marker1.Format.FormatCustom = "F0";
            marker1.Format.FormatType = ChartFX.WinForms.Gauge.ValueFormatType.Custom;
            marker1.Label.Visible = true;
            marker1.Position = ChartFX.WinForms.Gauge.Position.Top;
            marker1.Size = 1.4F;
            marker1.ToolTip = "";
            marker1.Value = 60;
            marker2.Color = System.Drawing.Color.Red;
            marker2.Draggable = true;
            marker2.Format.FormatCustom = "F0";
            marker2.Format.FormatType = ChartFX.WinForms.Gauge.ValueFormatType.Custom;
            marker2.Label.Position = ChartFX.WinForms.Gauge.Position.Bottom;
            marker2.Label.Visible = true;
            marker2.Main = false;
            marker2.Position = ChartFX.WinForms.Gauge.Position.Bottom;
            marker2.PositionCustom = -0.1F;
            marker2.Size = 1.4F;
            marker2.ToolTip = "";
            marker2.Value = 80;
            linearScale1.Indicators.AddRange(new ChartFX.WinForms.Gauge.Indicator[] {
            marker1,
            marker2});
            linearScale1.Max = 100D;
            linearScale1.MaxAlwaysDisplayed = true;
            linearScale1.MinAlwaysDisplayed = true;
            section1.Bar.Color = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(192)))), ((int)(((byte)(0)))));
            section1.Color = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(192)))), ((int)(((byte)(0)))));
            section1.Max = 60D;
            section2.Bar.Color = System.Drawing.Color.Yellow;
            section2.Color = System.Drawing.Color.Yellow;
            section2.Max = 80D;
            section2.Min = 60D;
            section3.Bar.Color = System.Drawing.Color.Red;
            section3.Color = System.Drawing.Color.Red;
            section3.Min = 80D;
            linearScale1.Sections.AddRange(new ChartFX.WinForms.Gauge.Section[] {
            section1,
            section2,
            section3});
            linearStrip1.Color = System.Drawing.Color.Blue;
            linearStrip1.Max = 50D;
            linearStrip1.Min = 20D;
            linearStrip1.Offset = 0.17F;
            linearScale1.Stripes.AddRange(new ChartFX.WinForms.Gauge.LinearStrip[] {
            linearStrip1});
            linearScale1.Thickness = 0.17F;
            linearScale1.Tickmarks.Major.Label.Position = ChartFX.WinForms.Gauge.Position.Bottom;
            linearScale1.Tickmarks.Medium.Visible = false;
            linearScale1.Tickmarks.Visible = false;
            this.alertConfigurationGauge.Scales.AddRange(new ChartFX.WinForms.Gauge.LinearScale[] {
            linearScale1});
            this.alertConfigurationGauge.Size = new System.Drawing.Size(498, 66);
            this.alertConfigurationGauge.TabIndex = 4;
            this.alertConfigurationGauge.Text = "horizontalGauge1";
            this.alertConfigurationGauge.ValueChanged += new ChartFX.WinForms.Gauge.IndicatorEventHandler(this.alertConfigurationGauge_ValueChanged);
            this.alertConfigurationGauge.GetTip += new ChartFX.WinForms.Gauge.GetTipEventHandler(this.alertConfigurationGauge_GetTip);
            this.alertConfigurationGauge.MouseDown += new System.Windows.Forms.MouseEventHandler(this.alertConfigurationGauge_MouseDown);
            // 
            // flowLayoutPanel1
            // 
            this.flowLayoutPanel1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.flowLayoutPanel1.AutoSize = true;
            this.flowLayoutPanel1.Controls.Add(this.alertRecommendationOptionsButton);
            this.flowLayoutPanel1.Controls.Add(this.configureBaselineButton);
            this.flowLayoutPanel1.FlowDirection = System.Windows.Forms.FlowDirection.RightToLeft;
            this.flowLayoutPanel1.Location = new System.Drawing.Point(12, 405);
            this.flowLayoutPanel1.Name = "flowLayoutPanel1";
            this.flowLayoutPanel1.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.flowLayoutPanel1.Size = new System.Drawing.Size(525, 33);
            this.flowLayoutPanel1.TabIndex = 6;
            // 
            // alertRecommendationOptionsButton
            // 
            this.alertRecommendationOptionsButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.alertRecommendationOptionsButton.AutoSize = true;
            this.alertRecommendationOptionsButton.Location = new System.Drawing.Point(447, 3);
            this.alertRecommendationOptionsButton.Name = "alertRecommendationOptionsButton";
            this.alertRecommendationOptionsButton.Size = new System.Drawing.Size(75, 23);
            this.alertRecommendationOptionsButton.TabIndex = 0;
            this.alertRecommendationOptionsButton.Text = "Options...";
            this.alertRecommendationOptionsButton.UseVisualStyleBackColor = true;
            this.alertRecommendationOptionsButton.Click += new System.EventHandler(this.alertRecommendationOptionsButton_Click);
            // 
            // configureBaselineButton
            // 
            this.configureBaselineButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.configureBaselineButton.AutoSize = true;
            this.configureBaselineButton.Location = new System.Drawing.Point(327, 3);
            this.configureBaselineButton.Name = "configureBaselineButton";
            this.configureBaselineButton.Size = new System.Drawing.Size(114, 23);
            this.configureBaselineButton.TabIndex = 2;
            this.configureBaselineButton.Text = "Configure Baseline...";
            this.configureBaselineButton.UseVisualStyleBackColor = true;
            this.configureBaselineButton.Click += new System.EventHandler(this.configureBaselineButton_Click);
            // 
            // mainContainerPanel
            // 
            this.mainContainerPanel.Dock = DockStyle.Fill;
            this.mainContainerPanel.BackColor = System.Drawing.Color.White;
            this.mainContainerPanel.ColumnCount = 1;
            this.mainContainerPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.AutoSize));
            this.mainContainerPanel.Controls.Add(this.dividerProgressBar2, 0, 0);
            this.mainContainerPanel.Controls.Add(this.label1, 0, 1);
            this.mainContainerPanel.Controls.Add(this.applyAllCheckBox, 0, 2);
            this.mainContainerPanel.Controls.Add(this.recommendationsGrid, 0, 3);
            this.mainContainerPanel.Controls.Add(this.alertConfigurationGroupBox, 0, 4);
            this.mainContainerPanel.Controls.Add(this.flowLayoutPanel1, 0, 5);
            this.mainContainerPanel.Location = new System.Drawing.Point(0, 0);
            this.mainContainerPanel.Name = "mainContainerPanel";
            this.mainContainerPanel.RowCount = 6;
            this.mainContainerPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(SizeType.Absolute, 10.0F));
            this.mainContainerPanel.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.mainContainerPanel.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.mainContainerPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(SizeType.Percent, 100F));
            this.mainContainerPanel.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.mainContainerPanel.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.mainContainerPanel.Size = new System.Drawing.Size(495, 480);
            this.mainContainerPanel.TabIndex = 4;

            // 
            // AlertRecommendationsDialog
            // 
            this.AcceptButton = this.okButton;
            this.AutoSize = true;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.cancelButton;
            this.ClientSize = new System.Drawing.Size(549, 502);
            this.Controls.Add(this.office2007PropertyPage1);
            this.Controls.Add(this.cancelButton);
            this.Controls.Add(this.okButton);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.HelpButton = true;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "AlertRecommendationsDialog";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Alert Recommendations for {0}";
            this.HelpButtonClicked += new System.ComponentModel.CancelEventHandler(this.AlertRecommendationsDialog_HelpButtonClicked);
            this.Load += new System.EventHandler(this.AlertRecommendationsDialog_Load);
            this.HelpRequested += new System.Windows.Forms.HelpEventHandler(this.AlertRecommendationsDialog_HelpRequested);
            ((System.ComponentModel.ISupportInitialize)(this.gridBindingSource)).EndInit();
            this.office2007PropertyPage1.ContentPanel.ResumeLayout(false);
            this.office2007PropertyPage1.ContentPanel.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.recommendationsGrid)).EndInit();
            this.alertConfigurationGroupBox.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.alertConfigurationGauge)).EndInit();
            this.flowLayoutPanel1.ResumeLayout(false);
            this.flowLayoutPanel1.PerformLayout();
            this.mainContainerPanel.ResumeLayout(false);
            this.mainContainerPanel.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private Idera.SQLdm.DesktopClient.Controls.Office2007PropertyPage office2007PropertyPage1;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomButton cancelButton;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomButton okButton;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomButton alertRecommendationOptionsButton;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomButton configureBaselineButton;
        private ChartFX.WinForms.Gauge.HorizontalGauge alertConfigurationGauge;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomGroupBox alertConfigurationGroupBox;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomCheckBox applyAllCheckBox;
        private Infragistics.Win.UltraWinGrid.UltraGrid recommendationsGrid;
        private System.Windows.Forms.BindingSource gridBindingSource;
        private System.ComponentModel.BackgroundWorker refreshSuggestionsWorker;
        private Idera.SQLdm.DesktopClient.Controls.InfiniteProgressBar dividerProgressBar2;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomLabel label1;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomFlowLayoutPanel flowLayoutPanel1;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomTableLayoutPanel mainContainerPanel;
        private ChartFX.WinForms.Gauge.LinearScale linearScale1;
    }
}
