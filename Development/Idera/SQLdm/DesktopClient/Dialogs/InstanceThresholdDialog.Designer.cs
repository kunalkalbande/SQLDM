using Idera.SQLdm.DesktopClient.Helpers;

namespace Idera.SQLdm.DesktopClient.Dialogs
{
    partial class instanceThresholdDialog
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
            if (disposing)
            {
                if (textEditor != null)
                    textEditor.Dispose();

                if(components != null)
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
            Infragistics.Win.Appearance appearance18 = new Infragistics.Win.Appearance();
            Infragistics.Win.UltraWinGrid.UltraGridBand ultraGridBand1 = new Infragistics.Win.UltraWinGrid.UltraGridBand("Band 0", -1);
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn12 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("Column1");
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn13 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("Column2");
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn14 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("Column3");
            Infragistics.Win.Appearance appearance3 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance19 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance20 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance21 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance22 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance23 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance24 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance25 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance26 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance27 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance28 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance29 = new Infragistics.Win.Appearance();
             linearScale1 = new ChartFX.WinForms.Gauge.LinearScale();
            ChartFX.WinForms.Gauge.Marker marker1 = new ChartFX.WinForms.Gauge.Marker();
            ChartFX.WinForms.Gauge.Marker marker2 = new ChartFX.WinForms.Gauge.Marker();
            ChartFX.WinForms.Gauge.Marker marker3 = new ChartFX.WinForms.Gauge.Marker();
            ChartFX.WinForms.Gauge.Section section1 = new ChartFX.WinForms.Gauge.Section();
            ChartFX.WinForms.Gauge.Section section2 = new ChartFX.WinForms.Gauge.Section();
            ChartFX.WinForms.Gauge.Section section3 = new ChartFX.WinForms.Gauge.Section();
            ChartFX.WinForms.Gauge.Section section4 = new ChartFX.WinForms.Gauge.Section();
            ChartFX.WinForms.Gauge.LinearStrip linearStrip1 = new ChartFX.WinForms.Gauge.LinearStrip();
            Infragistics.Win.Appearance appearance1 = new Infragistics.Win.Appearance();
            Infragistics.Win.UltraWinGrid.UltraGridBand ultraGridBand2 = new Infragistics.Win.UltraWinGrid.UltraGridBand("FlattenedThresholds", -1);
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn1 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("ThresholdItemType");
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn2 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("IsEditable");
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn3 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("IsNumeric");
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn4 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("IsMultiValued");
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn5 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("IsMutuallyExclusive");
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn6 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("IsContained");
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn7 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("Enabled", -1, 5686891);
            Infragistics.Win.Appearance appearance2 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance30 = new Infragistics.Win.Appearance();
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn8 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("RangeStart");
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn9 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("RangeEnd");
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn10 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("Value");
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn11 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("SuggestedValue");
            //10.0 SQLdm Srishti Purohit -- to hide property added to differentiat alert type (normal or baseline)
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumnIsBaselineTypeAlert = new Infragistics.Win.UltraWinGrid.UltraGridColumn("IsBaselineTypeAlert");
            Infragistics.Win.Appearance appearance31 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance4 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance5 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance6 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance7 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance8 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance9 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance10 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance11 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance12 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance13 = new Infragistics.Win.Appearance();
            Infragistics.Win.ValueList valueList1 = new Controls.CustomControls.CustomValueList(10018657);
            Infragistics.Win.ValueListItem valueListItem1 = new Infragistics.Win.ValueListItem();
            Infragistics.Win.ValueListItem valueListItem2 = new Infragistics.Win.ValueListItem();
            Infragistics.Win.ValueListItem valueListItem3 = new Infragistics.Win.ValueListItem();
            Infragistics.Win.ValueList valueList2 = new Controls.CustomControls.CustomValueList(5632719);
            Infragistics.Win.ValueListItem valueListItem4 = new Infragistics.Win.ValueListItem();
            Infragistics.Win.Appearance appearance14 = new Infragistics.Win.Appearance();
            Infragistics.Win.ValueListItem valueListItem5 = new Infragistics.Win.ValueListItem();
            Infragistics.Win.Appearance appearance15 = new Infragistics.Win.Appearance();
            Infragistics.Win.ValueList valueList3 = new Controls.CustomControls.CustomValueList(5686891);
            Infragistics.Win.ValueListItem valueListItem6 = new Infragistics.Win.ValueListItem();
            Infragistics.Win.Appearance appearance16 = new Infragistics.Win.Appearance();
            Infragistics.Win.ValueListItem valueListItem7 = new Infragistics.Win.ValueListItem();
            Infragistics.Win.Appearance appearance17 = new Infragistics.Win.Appearance();
            this.btnOK = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomButton();
            this.btnCancel = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomButton();
            this.office2007PropertyPage1 = new Idera.SQLdm.DesktopClient.Controls.Office2007PropertyPage(isDarkThemeSelected);
            this.label1 = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomLabel();
            this.instanceSelector = new Infragistics.Win.UltraWinGrid.UltraCombo();
            this.label2 = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomLabel(); //SQLdm 8.6 (Ankit Srivastava) -Preferred Node feature -Added new label for Replica Instance Name 
            this.replicaNameTextBox = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomTextBox(); //SQLdm 8.6 (Ankit Srivastava) -Preferred Node feature -Added new textbox for Replica Instance Name
            this.dividerProgressBar2 = new Idera.SQLdm.DesktopClient.Controls.InfiniteProgressBar();
            this.alertConfigurationGroupBox = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomGroupBox();
            this.alertConfigurationGauge = new ChartFX.WinForms.Gauge.HorizontalGauge();
            this.instanceConfigurationGrid = new Infragistics.Win.UltraWinGrid.UltraGrid();
            this.configBindSource = new System.Windows.Forms.BindingSource(this.components);
            this.btnAdvanced = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomButton();
            this.retrieveInstancesWorker = new System.ComponentModel.BackgroundWorker();
            this.ultraValidator1 = new Infragistics.Win.Misc.UltraValidator(this.components);
            this.office2007PropertyPage1.ContentPanel.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.instanceSelector)).BeginInit();
            this.replicaNameTextBox.SuspendLayout(); //SQLdm 8.6 (Ankit Srivastava) -Preferred Node feature -Added new textbox for Replica Instance Name
            this.alertConfigurationGroupBox.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.alertConfigurationGauge)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.instanceConfigurationGrid)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.configBindSource)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.ultraValidator1)).BeginInit();
            this.SuspendLayout();
            // 
            // btnOK
            // 
            this.btnOK.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnOK.Location = new System.Drawing.Point(191, 367);
            this.btnOK.Name = "btnOK";
            this.btnOK.Size = new System.Drawing.Size(75, 23);
            this.btnOK.TabIndex = 0;
            this.btnOK.Text = "OK";
            this.btnOK.UseVisualStyleBackColor = true;
            this.btnOK.Click += new System.EventHandler(this.btnOK_Click);
            // 
            // btnCancel
            // 
            this.btnCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnCancel.Location = new System.Drawing.Point(272, 367);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(75, 23);
            this.btnCancel.TabIndex = 1;
            this.btnCancel.Text = "Cancel";
            this.btnCancel.UseVisualStyleBackColor = true;
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
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
            this.office2007PropertyPage1.ContentPanel.Controls.Add(this.label1);
            this.office2007PropertyPage1.ContentPanel.Controls.Add(this.instanceSelector);
            this.office2007PropertyPage1.ContentPanel.Controls.Add(this.label2); //SQLdm 8.6 (Ankit Srivastava) -Preferred Node feature -Added the label to the content panel
            this.office2007PropertyPage1.ContentPanel.Controls.Add(this.replicaNameTextBox); //SQLdm 8.6 (Ankit Srivastava) -Preferred Node feature -Added the textbox to the content panel
            this.office2007PropertyPage1.ContentPanel.Controls.Add(this.dividerProgressBar2);
            this.office2007PropertyPage1.ContentPanel.Controls.Add(this.alertConfigurationGroupBox);
            this.office2007PropertyPage1.ContentPanel.Controls.Add(this.instanceConfigurationGrid);
            this.office2007PropertyPage1.ContentPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.office2007PropertyPage1.ContentPanel.FillStyle = Idera.SQLdm.DesktopClient.Controls.GradientPanelFillStyle.Solid;
            this.office2007PropertyPage1.ContentPanel.Location = new System.Drawing.Point(1, 56);
            this.office2007PropertyPage1.ContentPanel.Name = "ContentPanel";
            this.office2007PropertyPage1.ContentPanel.Padding = new System.Windows.Forms.Padding(1);
            this.office2007PropertyPage1.ContentPanel.ShowBorder = false;
            this.office2007PropertyPage1.ContentPanel.Size = new System.Drawing.Size(333, 292);
            this.office2007PropertyPage1.ContentPanel.TabIndex = 1;
            this.office2007PropertyPage1.HeaderImage = global::Idera.SQLdm.DesktopClient.Properties.Resources.AlertsFeature1;
            this.office2007PropertyPage1.Location = new System.Drawing.Point(12, 12);
            this.office2007PropertyPage1.Name = "office2007PropertyPage1";
            this.office2007PropertyPage1.Size = new System.Drawing.Size(335, 349);
            this.office2007PropertyPage1.TabIndex = 6;
            this.office2007PropertyPage1.Text = "Set the thresholds for this instance of the {0} metric.";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = AutoScaleSizeHelper.isScalingRequired ? new System.Drawing.Point(12, 7) : new System.Drawing.Point(12, 15);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(53, 13);
            this.label1.TabIndex = 19;
            this.label1.Text = "Database";
            this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // instanceSelector
            // 
            this.instanceSelector.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.instanceSelector.AutoCompleteMode = Infragistics.Win.AutoCompleteMode.SuggestAppend;
            this.instanceSelector.CheckedListSettings.CheckStateMember = "";
            appearance18.BackColor = System.Drawing.SystemColors.Window;
            appearance18.BorderColor = System.Drawing.SystemColors.InactiveCaption;
            this.instanceSelector.DisplayLayout.Appearance = appearance18;
            ultraGridBand1.ColHeadersVisible = false;
            ultraGridColumn12.Header.Enabled = false;
            ultraGridColumn12.Header.VisiblePosition = 0;
            ultraGridColumn13.Header.Enabled = false;
            ultraGridColumn13.Header.VisiblePosition = 1;
            ultraGridColumn14.Header.Enabled = false;
            ultraGridColumn14.Header.VisiblePosition = 2;
            ultraGridBand1.Columns.AddRange(new object[] {
            ultraGridColumn12,
            ultraGridColumn13,
            ultraGridColumn14});
            appearance3.ForeColor = System.Drawing.Color.Black;
            ultraGridBand1.Override.ActiveRowAppearance = appearance3;
            this.instanceSelector.DisplayLayout.BandsSerializer.Add(ultraGridBand1);
            this.instanceSelector.DisplayLayout.BorderStyle = Infragistics.Win.UIElementBorderStyle.Solid;
            this.instanceSelector.DisplayLayout.CaptionVisible = Infragistics.Win.DefaultableBoolean.False;
            appearance19.BackColor = System.Drawing.SystemColors.ActiveBorder;
            appearance19.BackColor2 = System.Drawing.SystemColors.ControlDark;
            appearance19.BackGradientStyle = Infragistics.Win.GradientStyle.Vertical;
            appearance19.BorderColor = System.Drawing.SystemColors.Window;
            this.instanceSelector.DisplayLayout.GroupByBox.Appearance = appearance19;
            appearance20.ForeColor = System.Drawing.SystemColors.GrayText;
            this.instanceSelector.DisplayLayout.GroupByBox.BandLabelAppearance = appearance20;
            this.instanceSelector.DisplayLayout.GroupByBox.BorderStyle = Infragistics.Win.UIElementBorderStyle.Solid;
            appearance21.BackColor = System.Drawing.SystemColors.ControlLightLight;
            appearance21.BackColor2 = System.Drawing.SystemColors.Control;
            appearance21.BackGradientStyle = Infragistics.Win.GradientStyle.Horizontal;
            appearance21.ForeColor = System.Drawing.SystemColors.GrayText;
            this.instanceSelector.DisplayLayout.GroupByBox.PromptAppearance = appearance21;
            this.instanceSelector.DisplayLayout.MaxColScrollRegions = 1;
            this.instanceSelector.DisplayLayout.MaxRowScrollRegions = 1;
            appearance22.BackColor = System.Drawing.SystemColors.Window;
            appearance22.ForeColor = System.Drawing.SystemColors.ControlText;
            this.instanceSelector.DisplayLayout.Override.ActiveCellAppearance = appearance22;
            appearance23.BackColor = System.Drawing.SystemColors.Highlight;
            appearance23.ForeColor = System.Drawing.SystemColors.HighlightText;
            this.instanceSelector.DisplayLayout.Override.ActiveRowAppearance = appearance23;
            this.instanceSelector.DisplayLayout.Override.BorderStyleCell = Infragistics.Win.UIElementBorderStyle.Dotted;
            this.instanceSelector.DisplayLayout.Override.BorderStyleRow = Infragistics.Win.UIElementBorderStyle.Dotted;
            appearance24.BackColor = System.Drawing.SystemColors.Window;
            this.instanceSelector.DisplayLayout.Override.CardAreaAppearance = appearance24;
            appearance25.BorderColor = System.Drawing.Color.Silver;
            appearance25.TextTrimming = Infragistics.Win.TextTrimming.EllipsisCharacter;
            this.instanceSelector.DisplayLayout.Override.CellAppearance = appearance25;
            this.instanceSelector.DisplayLayout.Override.CellClickAction = Infragistics.Win.UltraWinGrid.CellClickAction.EditAndSelectText;
            this.instanceSelector.DisplayLayout.Override.CellPadding = 0;
            appearance26.BackColor = System.Drawing.SystemColors.Control;
            appearance26.BackColor2 = System.Drawing.SystemColors.ControlDark;
            appearance26.BackGradientAlignment = Infragistics.Win.GradientAlignment.Element;
            appearance26.BackGradientStyle = Infragistics.Win.GradientStyle.Horizontal;
            appearance26.BorderColor = System.Drawing.SystemColors.Window;
            this.instanceSelector.DisplayLayout.Override.GroupByRowAppearance = appearance26;
            appearance27.TextHAlignAsString = "Left";
            this.instanceSelector.DisplayLayout.Override.HeaderAppearance = appearance27;
            this.instanceSelector.DisplayLayout.Override.HeaderClickAction = Infragistics.Win.UltraWinGrid.HeaderClickAction.SortMulti;
            this.instanceSelector.DisplayLayout.Override.HeaderStyle = Infragistics.Win.HeaderStyle.WindowsXPCommand;
            appearance28.BackColor = System.Drawing.SystemColors.Window;
            appearance28.BorderColor = System.Drawing.Color.Silver;
            this.instanceSelector.DisplayLayout.Override.RowAppearance = appearance28;
            this.instanceSelector.DisplayLayout.Override.RowSelectors = Infragistics.Win.DefaultableBoolean.False;
            appearance29.BackColor = System.Drawing.SystemColors.ControlLight;
            this.instanceSelector.DisplayLayout.Override.TemplateAddRowAppearance = appearance29;
            this.instanceSelector.DisplayLayout.ScrollBounds = Infragistics.Win.UltraWinGrid.ScrollBounds.ScrollToFill;
            this.instanceSelector.DisplayLayout.ScrollStyle = Infragistics.Win.UltraWinGrid.ScrollStyle.Immediate;
            this.instanceSelector.DisplayLayout.ViewStyleBand = Infragistics.Win.UltraWinGrid.ViewStyleBand.OutlookGroupBy;
            this.instanceSelector.DisplayStyle = Infragistics.Win.EmbeddableElementDisplayStyle.Office2010;
            this.instanceSelector.DropDownResizeHandleStyle = Infragistics.Win.DropDownResizeHandleStyle.None;
            this.instanceSelector.DropDownWidth = 0;
            this.instanceSelector.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.instanceSelector.Location = new System.Drawing.Point(15, 31);
            this.instanceSelector.MinDropDownItems = 0;
            this.instanceSelector.Name = "instanceSelector";
            this.instanceSelector.PreferredDropDownSize = new System.Drawing.Size(0, 0);
            this.instanceSelector.Size = new System.Drawing.Size(177, 22);
            this.instanceSelector.TabIndex = 17;
            this.ultraValidator1.GetValidationSettings(this.instanceSelector).NotificationSettings.Action = Infragistics.Win.Misc.NotificationAction.Image;
            this.ultraValidator1.GetValidationSettings(this.instanceSelector).NotificationSettings.Caption = "Specify Unique Name";
            this.ultraValidator1.GetValidationSettings(this.instanceSelector).NotificationSettings.Image = global::Idera.SQLdm.DesktopClient.Properties.Resources.RefreshError;
            this.ultraValidator1.GetValidationSettings(this.instanceSelector).NotificationSettings.Text = "You must select or specify a unique instance name.";
            this.instanceSelector.AfterDropDown += new System.EventHandler(this.instanceSelector_AfterDropDown);
            this.instanceSelector.BeforeDropDown += new System.ComponentModel.CancelEventHandler(this.instanceSelector_BeforeDropDown);
            this.instanceSelector.TextChanged += new System.EventHandler(this.instanceSelector_TextChanged);
            this.instanceSelector.Enter += new System.EventHandler(this.instanceSelector_Enter);
            // 
            // label2 //SQLdm 8.6 (Ankit Srivastava) -Preferred Node feature
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(15, 75);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(53, 13);
            this.label2.TabIndex = 20;
            this.label2.Text = "Preferred Replica Name";
            this.label2.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // replicaNameTextBox //SQLdm 8.6 (Ankit Srivastava) -Preferred Node feature
            // 
            this.replicaNameTextBox.AutoSize = true;
            this.replicaNameTextBox.Name = "replicaNameTextBox";
            this.replicaNameTextBox.Location = new System.Drawing.Point(15,90);
            this.replicaNameTextBox.Size = new System.Drawing.Size(177, 20);
            this.replicaNameTextBox.MaxLength = 512;
            this.replicaNameTextBox.TabIndex = 21;
            this.replicaNameTextBox.TextAlign = System.Windows.Forms.HorizontalAlignment.Left;
            this.replicaNameTextBox.TextChanged += new System.EventHandler(this.replicaNameTextBox_TextChanged);
            //
            // dividerProgressBar2
            // 
            this.dividerProgressBar2.Anchor = System.Windows.Forms.AnchorStyles.Top;
            this.dividerProgressBar2.Color1 = System.Drawing.Color.DarkGray;
            this.dividerProgressBar2.Color2 = System.Drawing.Color.White;
            this.dividerProgressBar2.Location = new System.Drawing.Point(-72, 3);
            this.dividerProgressBar2.Name = "dividerProgressBar2";
            this.dividerProgressBar2.Size = new System.Drawing.Size(475, 2);
            this.dividerProgressBar2.Speed = 15D;
            this.dividerProgressBar2.Step = 5F;
            this.dividerProgressBar2.TabIndex = 16;
            // 
            // alertConfigurationGroupBox
            // 
            this.alertConfigurationGroupBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.alertConfigurationGroupBox.Controls.Add(this.alertConfigurationGauge);
            this.alertConfigurationGroupBox.Location = new System.Drawing.Point(12, 61);
            this.alertConfigurationGroupBox.Name = "alertConfigurationGroupBox";
            this.alertConfigurationGroupBox.Size = new System.Drawing.Size(313, 103);
            this.alertConfigurationGroupBox.TabIndex = 12;
            this.alertConfigurationGroupBox.TabStop = false;
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
            marker1.Color = System.Drawing.Color.Blue;
            marker1.Draggable = true;
            marker1.Format.Decimals = 0;
            marker1.Format.FormatCustom = "F0";
            marker1.Format.FormatType = ChartFX.WinForms.Gauge.ValueFormatType.Custom;
            marker1.Label.Position = ChartFX.WinForms.Gauge.Position.Bottom;
            marker1.Label.Visible = true;
            marker1.Position = ChartFX.WinForms.Gauge.Position.Bottom;
            marker1.Size = 1.4F;
            marker1.ToolTip = "";
            marker1.Value = 40;
            marker2.Color = System.Drawing.Color.Gold;
            marker2.Draggable = true;
            marker2.Format.FormatCustom = "F0";
            marker2.Format.FormatType = ChartFX.WinForms.Gauge.ValueFormatType.Custom;
            marker2.Label.Visible = true;
            marker2.Main = false;
            marker2.Position = ChartFX.WinForms.Gauge.Position.Top;
            marker2.PositionCustom = -0.1F;
            marker2.Size = 1.4F;
            marker2.ToolTip = "";
            marker2.Value = 60;
            marker3.Color = System.Drawing.Color.Red;
            marker3.Draggable = true;
            marker3.Format.FormatCustom = "F0";
            marker3.Format.FormatType = ChartFX.WinForms.Gauge.ValueFormatType.Custom;
            marker3.Label.Position = ChartFX.WinForms.Gauge.Position.Bottom;
            marker3.Label.Visible = true;
            marker3.Main = false;
            marker3.Position = ChartFX.WinForms.Gauge.Position.Bottom;
            marker3.Size = 1.4F;
            marker3.ToolTip = "";
            marker3.Value = 80;
            linearScale1.Indicators.AddRange(new ChartFX.WinForms.Gauge.Indicator[] {
            marker1,
            marker2,
            marker3});
            linearScale1.Max = 100D;
            linearScale1.MaxAlwaysDisplayed = true;
            linearScale1.MinAlwaysDisplayed = true;
            section1.Bar.Color = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(192)))), ((int)(((byte)(0)))));
            section1.Color = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(192)))), ((int)(((byte)(0)))));
            section1.Max = 40D;
            section2.Bar.Color = System.Drawing.Color.Blue;
            section2.Color = System.Drawing.Color.Blue;
            section2.Max = 60D;
            section2.Min = 40D;
            section3.Bar.Color = System.Drawing.Color.Yellow;
            section3.Color = System.Drawing.Color.Yellow;
            section3.Max = 80D;
            section3.Min = 60D;
            section4.Bar.Color = System.Drawing.Color.Red;
            section4.Color = System.Drawing.Color.Red;
            section4.Min = 80D;
            linearScale1.Sections.AddRange(new ChartFX.WinForms.Gauge.Section[] {
            section1,
            section2,
            section3,
            section4});
            linearStrip1.Color = System.Drawing.Color.Blue;
            linearStrip1.FillType = ChartFX.WinForms.Gauge.StripFillType.Pattern;
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
            this.alertConfigurationGauge.Size = new System.Drawing.Size(307, 84);
            this.alertConfigurationGauge.TabIndex = 4;
            this.alertConfigurationGauge.Text = "horizontalGauge1";
            this.alertConfigurationGauge.ValueChanged += new ChartFX.WinForms.Gauge.IndicatorEventHandler(this.alertConfigurationGauge_ValueChanged);
            // 
            // instanceConfigurationGrid
            // 
            this.instanceConfigurationGrid.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.instanceConfigurationGrid.DataMember = "FlattenedThresholds";
            this.instanceConfigurationGrid.DataSource = this.configBindSource;
            appearance1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(251)))), ((int)(((byte)(252)))), ((int)(((byte)(255)))));
            appearance1.BorderColor = System.Drawing.SystemColors.InactiveCaption;
            this.instanceConfigurationGrid.DisplayLayout.Appearance = appearance1;
            this.instanceConfigurationGrid.DisplayLayout.AutoFitStyle = Infragistics.Win.UltraWinGrid.AutoFitStyle.ExtendLastColumn;
            ultraGridColumn1.Header.Caption = "State";
            ultraGridColumn1.Header.VisiblePosition = 1;
            ultraGridColumn1.InvalidValueBehavior = Infragistics.Win.UltraWinGrid.InvalidValueBehavior.RevertValueAndRetainFocus;
            ultraGridColumn1.PromptChar = ' ';
            ultraGridColumn1.Width = 152;
            ultraGridColumn2.Header.VisiblePosition = 2;
            ultraGridColumn2.Hidden = true;
            ultraGridColumn3.Header.VisiblePosition = 3;
            ultraGridColumn3.Hidden = true;
            ultraGridColumn4.Header.VisiblePosition = 4;
            ultraGridColumn4.Hidden = true;
            ultraGridColumn5.Header.VisiblePosition = 5;
            ultraGridColumn5.Hidden = true;
            ultraGridColumn6.Header.VisiblePosition = 6;
            ultraGridColumn6.Hidden = true;
            ultraGridColumn7.AutoCompleteMode = Infragistics.Win.AutoCompleteMode.Append;
            ultraGridColumn7.CellActivation = Infragistics.Win.UltraWinGrid.Activation.NoEdit;
            appearance2.TextHAlignAsString = "Center";
            ultraGridColumn7.CellAppearance = appearance2;
            appearance30.ImageHAlign = Infragistics.Win.HAlign.Center;
            ultraGridColumn7.Header.Appearance = appearance30;
            ultraGridColumn7.Header.Caption = "";
            ultraGridColumn7.Header.ToolTipText = "Enabled";
            ultraGridColumn7.Header.VisiblePosition = 0;
            ultraGridColumn7.MaxWidth = 24;
            ultraGridColumn7.MinWidth = 24;
            ultraGridColumn7.SortIndicator = Infragistics.Win.UltraWinGrid.SortIndicator.Disabled;
            ultraGridColumn7.Width = 24;
            ultraGridColumn8.AutoCompleteMode = Infragistics.Win.AutoCompleteMode.Append;
            ultraGridColumn8.Format = "F0";
            ultraGridColumn8.Header.Caption = "Start";
            ultraGridColumn8.Header.VisiblePosition = 7;
            ultraGridColumn8.PromptChar = ' ';
            ultraGridColumn8.SortIndicator = Infragistics.Win.UltraWinGrid.SortIndicator.Disabled;
            ultraGridColumn8.Width = 60;
            ultraGridColumn9.CellActivation = Infragistics.Win.UltraWinGrid.Activation.NoEdit;
            ultraGridColumn9.Header.Caption = "End";
            ultraGridColumn9.Header.VisiblePosition = 8;
            ultraGridColumn9.SortIndicator = Infragistics.Win.UltraWinGrid.SortIndicator.Disabled;
            ultraGridColumn9.Width = 60;
            ultraGridColumn10.Header.VisiblePosition = 9;
            ultraGridColumn10.Hidden = true;
            ultraGridColumn11.Header.VisiblePosition = 10;
            ultraGridColumn11.Hidden = true;

            ultraGridColumnIsBaselineTypeAlert.Header.VisiblePosition = 11;
            ultraGridColumnIsBaselineTypeAlert.Hidden = true;
            ultraGridBand2.Columns.AddRange(new object[] {
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
            ultraGridColumn11,
            ultraGridColumnIsBaselineTypeAlert});
            appearance31.TextHAlignAsString = "Center";
            ultraGridBand2.Override.EditCellAppearance = appearance31;
            this.instanceConfigurationGrid.DisplayLayout.BandsSerializer.Add(ultraGridBand2);
            this.instanceConfigurationGrid.DisplayLayout.CaptionVisible = Infragistics.Win.DefaultableBoolean.False;
            appearance4.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(227)))), ((int)(((byte)(239)))), ((int)(((byte)(255)))));
            appearance4.BackColor2 = System.Drawing.SystemColors.ControlDark;
            appearance4.BackGradientStyle = Infragistics.Win.GradientStyle.None;
            appearance4.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(101)))), ((int)(((byte)(147)))), ((int)(((byte)(207)))));
            this.instanceConfigurationGrid.DisplayLayout.GroupByBox.Appearance = appearance4;
            appearance5.ForeColor = System.Drawing.SystemColors.GrayText;
            this.instanceConfigurationGrid.DisplayLayout.GroupByBox.BandLabelAppearance = appearance5;
            this.instanceConfigurationGrid.DisplayLayout.GroupByBox.BorderStyle = Infragistics.Win.UIElementBorderStyle.Solid;
            this.instanceConfigurationGrid.DisplayLayout.GroupByBox.Hidden = true;
            appearance6.BackColor = System.Drawing.SystemColors.ControlLightLight;
            appearance6.BackColor2 = System.Drawing.SystemColors.Control;
            appearance6.BackGradientStyle = Infragistics.Win.GradientStyle.Horizontal;
            appearance6.ForeColor = System.Drawing.SystemColors.GrayText;
            this.instanceConfigurationGrid.DisplayLayout.GroupByBox.PromptAppearance = appearance6;
            this.instanceConfigurationGrid.DisplayLayout.MaxColScrollRegions = 1;
            this.instanceConfigurationGrid.DisplayLayout.MaxRowScrollRegions = 1;
            this.instanceConfigurationGrid.DisplayLayout.Override.AllowAddNew = Infragistics.Win.UltraWinGrid.AllowAddNew.No;
            this.instanceConfigurationGrid.DisplayLayout.Override.AllowDelete = Infragistics.Win.DefaultableBoolean.False;
            this.instanceConfigurationGrid.DisplayLayout.Override.AllowUpdate = Infragistics.Win.DefaultableBoolean.True;
            this.instanceConfigurationGrid.DisplayLayout.Override.BorderStyleCell = Infragistics.Win.UIElementBorderStyle.None;
            this.instanceConfigurationGrid.DisplayLayout.Override.BorderStyleRow = Infragistics.Win.UIElementBorderStyle.None;
            appearance7.BackColor = System.Drawing.SystemColors.Window;
            this.instanceConfigurationGrid.DisplayLayout.Override.CardAreaAppearance = appearance7;
            appearance8.BorderColor = System.Drawing.Color.Silver;
            appearance8.TextTrimming = Infragistics.Win.TextTrimming.EllipsisCharacter;
            this.instanceConfigurationGrid.DisplayLayout.Override.CellAppearance = appearance8;
            appearance9.ImageHAlign = Infragistics.Win.HAlign.Center;
            appearance9.ImageVAlign = Infragistics.Win.VAlign.Middle;
            this.instanceConfigurationGrid.DisplayLayout.Override.CellButtonAppearance = appearance9;
            this.instanceConfigurationGrid.DisplayLayout.Override.CellPadding = 0;
            this.instanceConfigurationGrid.DisplayLayout.Override.FixedHeaderIndicator = Infragistics.Win.UltraWinGrid.FixedHeaderIndicator.None;
            this.instanceConfigurationGrid.DisplayLayout.Override.GroupByColumnsHidden = Infragistics.Win.DefaultableBoolean.False;
            appearance10.BackColor = System.Drawing.SystemColors.Control;
            appearance10.BackColor2 = System.Drawing.SystemColors.ControlDark;
            appearance10.BackGradientAlignment = Infragistics.Win.GradientAlignment.Element;
            appearance10.BackGradientStyle = Infragistics.Win.GradientStyle.Horizontal;
            appearance10.BorderColor = System.Drawing.SystemColors.Window;
            this.instanceConfigurationGrid.DisplayLayout.Override.GroupByRowAppearance = appearance10;
            this.instanceConfigurationGrid.DisplayLayout.Override.GroupByRowInitialExpansionState = Infragistics.Win.UltraWinGrid.GroupByRowInitialExpansionState.Expanded;
            appearance11.TextHAlignAsString = "Left";
            this.instanceConfigurationGrid.DisplayLayout.Override.HeaderAppearance = appearance11;
            this.instanceConfigurationGrid.DisplayLayout.Override.HeaderClickAction = Infragistics.Win.UltraWinGrid.HeaderClickAction.SortSingle;
            appearance12.BackColor = System.Drawing.SystemColors.Window;
            appearance12.BorderColor = System.Drawing.Color.Silver;
            this.instanceConfigurationGrid.DisplayLayout.Override.RowAppearance = appearance12;
            this.instanceConfigurationGrid.DisplayLayout.Override.RowSelectors = Infragistics.Win.DefaultableBoolean.False;
            this.instanceConfigurationGrid.DisplayLayout.Override.SelectTypeCell = Infragistics.Win.UltraWinGrid.SelectType.Single;
            this.instanceConfigurationGrid.DisplayLayout.Override.SelectTypeCol = Infragistics.Win.UltraWinGrid.SelectType.None;
            this.instanceConfigurationGrid.DisplayLayout.Override.SelectTypeRow = Infragistics.Win.UltraWinGrid.SelectType.Single;
            appearance13.BackColor = System.Drawing.SystemColors.ControlLight;
            this.instanceConfigurationGrid.DisplayLayout.Override.TemplateAddRowAppearance = appearance13;
            this.instanceConfigurationGrid.DisplayLayout.ScrollBounds = Infragistics.Win.UltraWinGrid.ScrollBounds.ScrollToFill;
            this.instanceConfigurationGrid.DisplayLayout.ScrollStyle = Infragistics.Win.UltraWinGrid.ScrollStyle.Immediate;
            this.instanceConfigurationGrid.DisplayLayout.UseFixedHeaders = true;
            valueList1.DisplayStyle = Infragistics.Win.ValueListDisplayStyle.DisplayTextAndPicture;
            valueList1.Key = "ThresholdTypeItems";
            valueList1.PreferredDropDownSize = new System.Drawing.Size(0, 0);
            valueListItem1.DataValue = 0;
            valueListItem1.DisplayText = "OK";
            valueListItem2.DataValue = 1;
            valueListItem2.DisplayText = "Warning";
            valueListItem3.DataValue = 2;
            valueListItem3.DisplayText = "Critical";
            valueList1.ValueListItems.AddRange(new Infragistics.Win.ValueListItem[] {
            valueListItem1,
            valueListItem2,
            valueListItem3});
            valueList2.DisplayStyle = Infragistics.Win.ValueListDisplayStyle.Picture;
            valueList2.Key = "RadioButtons";
            valueList2.PreferredDropDownSize = new System.Drawing.Size(0, 0);
            valueList2.ScaleItemImage = Infragistics.Win.ScaleImage.Never;
            appearance14.Image = global::Idera.SQLdm.DesktopClient.Properties.Resources.RibbonRadioButtonUnchecked;
            appearance14.ImageHAlign = Infragistics.Win.HAlign.Center;
            valueListItem4.Appearance = appearance14;
            valueListItem4.DataValue = false;
            appearance15.Image = global::Idera.SQLdm.DesktopClient.Properties.Resources.RibbonRadioButtonChecked;
            appearance15.ImageHAlign = Infragistics.Win.HAlign.Center;
            valueListItem5.Appearance = appearance15;
            valueListItem5.DataValue = true;
            valueList2.ValueListItems.AddRange(new Infragistics.Win.ValueListItem[] {
            valueListItem4,
            valueListItem5});
            valueList3.DisplayStyle = Infragistics.Win.ValueListDisplayStyle.Picture;
            valueList3.Key = "CheckBoxes";
            valueList3.PreferredDropDownSize = new System.Drawing.Size(0, 0);
            valueList3.ScaleItemImage = Infragistics.Win.ScaleImage.Never;
            appearance16.Image = global::Idera.SQLdm.DesktopClient.Properties.Resources.RibbonCheckboxUnchecked;
            appearance16.ImageHAlign = Infragistics.Win.HAlign.Center;
            valueListItem6.Appearance = appearance16;
            valueListItem6.DataValue = false;
            appearance17.Image = global::Idera.SQLdm.DesktopClient.Properties.Resources.RibbonCheckboxChecked;
            appearance17.ImageHAlign = Infragistics.Win.HAlign.Center;
            valueListItem7.Appearance = appearance17;
            valueListItem7.DataValue = true;
            valueList3.ValueListItems.AddRange(new Infragistics.Win.ValueListItem[] {
            valueListItem6,
            valueListItem7});
            this.instanceConfigurationGrid.DisplayLayout.ValueLists.AddRange(new Infragistics.Win.ValueList[] {
            valueList1,
            valueList2,
            valueList3});
            this.instanceConfigurationGrid.DisplayLayout.ViewStyle = Infragistics.Win.UltraWinGrid.ViewStyle.SingleBand;
            this.instanceConfigurationGrid.DisplayLayout.ViewStyleBand = Infragistics.Win.UltraWinGrid.ViewStyleBand.OutlookGroupBy;
            this.instanceConfigurationGrid.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.instanceConfigurationGrid.Location = new System.Drawing.Point(10, 170);
            this.instanceConfigurationGrid.Name = "instanceConfigurationGrid";
            this.instanceConfigurationGrid.Size = new System.Drawing.Size(315, 113);
            this.instanceConfigurationGrid.TabIndex = 18;
            this.instanceConfigurationGrid.UpdateMode = Infragistics.Win.UltraWinGrid.UpdateMode.OnCellChangeOrLostFocus;
            this.instanceConfigurationGrid.AfterCellUpdate += new Infragistics.Win.UltraWinGrid.CellEventHandler(this.instanceConfigurationGrid_AfterCellUpdate);
            this.instanceConfigurationGrid.InitializeRow += new Infragistics.Win.UltraWinGrid.InitializeRowEventHandler(this.instanceConfigurationGrid_InitializeRow);
            this.instanceConfigurationGrid.BeforeCellActivate += new Infragistics.Win.UltraWinGrid.CancelableCellEventHandler(this.instanceConfigurationGrid_BeforeCellActivate);
            this.instanceConfigurationGrid.ClickCell += new Infragistics.Win.UltraWinGrid.ClickCellEventHandler(this.instanceConfigurationGrid_ClickCell);
            // 
            // configBindSource
            // 
            this.configBindSource.DataMember = "ItemList";
            this.configBindSource.DataSource = typeof(Idera.SQLdm.Common.Configuration.AlertConfiguration);
            // 
            // btnAdvanced
            // 
            this.btnAdvanced.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnAdvanced.Location = new System.Drawing.Point(12, 367);
            this.btnAdvanced.Name = "btnAdvanced";
            this.btnAdvanced.Size = new System.Drawing.Size(75, 23);
            this.btnAdvanced.TabIndex = 0;
            this.btnAdvanced.Text = "Advanced";
            this.btnAdvanced.UseVisualStyleBackColor = true;
            this.btnAdvanced.Click += new System.EventHandler(this.btnAdvanced_Click);
            // 
            // retrieveInstancesWorker
            // 
            this.retrieveInstancesWorker.WorkerReportsProgress = true;
            this.retrieveInstancesWorker.WorkerSupportsCancellation = true;
            this.retrieveInstancesWorker.DoWork += new System.ComponentModel.DoWorkEventHandler(this.retrieveInstancesWorker_DoWork);
            this.retrieveInstancesWorker.RunWorkerCompleted += new System.ComponentModel.RunWorkerCompletedEventHandler(this.retrieveInstancesWorker_RunWorkerCompleted);
            // 
            // ultraValidator1
            // 
            this.ultraValidator1.NotificationSettings.Action = Infragistics.Win.Misc.NotificationAction.Image;
            this.ultraValidator1.NotificationSettings.Image = global::Idera.SQLdm.DesktopClient.Properties.Resources.RefreshError;
            this.ultraValidator1.ValidationTrigger = Infragistics.Win.Misc.ValidationTrigger.Programmatic;
            this.ultraValidator1.Validating += new Infragistics.Win.Misc.ValidatingHandler(this.ultraValidator1_Validating);
            // 
            // instanceThresholdDialog
            // 
            this.AcceptButton = this.btnOK;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.btnCancel;
            this.ClientSize = new System.Drawing.Size(359, 402);
            this.Controls.Add(this.btnAdvanced);
            this.Controls.Add(this.office2007PropertyPage1);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnOK);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.HelpButton = true;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "instanceThresholdDialog";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "{0} Threshold Configuration";
            this.HelpButtonClicked += new System.ComponentModel.CancelEventHandler(this.instanceThresholdDialog_HelpButtonClicked);
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.instanceThresholdDialog_FormClosing);
            this.Load += new System.EventHandler(this.instanceThresholdDialog_Load);
            this.HelpRequested += new System.Windows.Forms.HelpEventHandler(this.instanceThresholdDialog_HelpRequested);
            this.office2007PropertyPage1.ContentPanel.ResumeLayout(false);
            this.office2007PropertyPage1.ContentPanel.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.instanceSelector)).EndInit();
            this.alertConfigurationGroupBox.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.alertConfigurationGauge)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.instanceConfigurationGrid)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.configBindSource)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.ultraValidator1)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomButton btnOK;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomButton btnCancel;
        private Controls.Office2007PropertyPage office2007PropertyPage1;
        private Controls.InfiniteProgressBar dividerProgressBar2;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomGroupBox alertConfigurationGroupBox;
        private ChartFX.WinForms.Gauge.HorizontalGauge alertConfigurationGauge;
        private System.Windows.Forms.BindingSource configBindSource;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomButton btnAdvanced;
        private Infragistics.Win.UltraWinGrid.UltraCombo instanceSelector;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomTextBox replicaNameTextBox; //SQLdm 8.6 (Ankit Srivastava) -Preferred Node feature - declare the textbox for the replica instance name
        private System.ComponentModel.BackgroundWorker retrieveInstancesWorker;
        private Infragistics.Win.UltraWinGrid.UltraGrid instanceConfigurationGrid;
        private Infragistics.Win.Misc.UltraValidator ultraValidator1;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomLabel label1;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomLabel label2; //SQLdm 8.6 (Ankit Srivastava) -Preferred Node feature -declared the label for the replica instance name
        private ChartFX.WinForms.Gauge.LinearScale linearScale1;
    }
}
