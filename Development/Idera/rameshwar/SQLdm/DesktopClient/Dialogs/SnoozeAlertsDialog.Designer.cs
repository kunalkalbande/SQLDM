namespace Idera.SQLdm.DesktopClient.Dialogs
{
    partial class SnoozeAlertsDialog
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
            Infragistics.Win.Appearance appearance13 = new Infragistics.Win.Appearance();
            Infragistics.Win.UltraWinGrid.UltraGridBand ultraGridBand1 = new Infragistics.Win.UltraWinGrid.UltraGridBand("SnoozedAlerts", -1);
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn1 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("Metric");
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn2 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("Name", -1, null, 0, Infragistics.Win.UltraWinGrid.SortIndicator.Ascending, false);
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn3 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("Selected");
            Infragistics.Win.Appearance appearance14 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance15 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance16 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance17 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance18 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance19 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance20 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance21 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance22 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance23 = new Infragistics.Win.Appearance();
            Infragistics.Win.ValueList valueList1 = new Infragistics.Win.ValueList(426658501);
            Infragistics.Win.ValueListItem valueListItem1 = new Infragistics.Win.ValueListItem();
            Infragistics.Win.ValueListItem valueListItem2 = new Infragistics.Win.ValueListItem();
            Infragistics.Win.ValueListItem valueListItem3 = new Infragistics.Win.ValueListItem();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(SnoozeAlertsDialog));
            this.panel1 = new System.Windows.Forms.Panel();
            this.okButton = new System.Windows.Forms.Button();
            this.cancelButton = new System.Windows.Forms.Button();
            this.contentStackPanel = new Idera.SQLdm.Common.UI.Controls.StackLayoutPanel();
            this.unsnoozeAlertContentPanel = new System.Windows.Forms.Panel();
            this.unsnoozeInfoBox = new System.Windows.Forms.Label();
            this.unsnoozeImage = new System.Windows.Forms.PictureBox();
            this.resumeToggleCheckBox = new System.Windows.Forms.CheckBox();
            this.snoozedAlertsGrid = new Infragistics.Win.UltraWinGrid.UltraGrid();
            this.snoozeAlertContentPanel = new System.Windows.Forms.Panel();
            this.configureAlertSnoozePanel = new System.Windows.Forms.Panel();
            this.snoozeMinutesSpinner = new System.Windows.Forms.NumericUpDown();
            this.label3 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.snoozeInfoBox = new System.Windows.Forms.Label();
            this.snoozeImage = new System.Windows.Forms.PictureBox();
            this.panel1.SuspendLayout();
            this.contentStackPanel.SuspendLayout();
            this.unsnoozeAlertContentPanel.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.unsnoozeImage)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.snoozedAlertsGrid)).BeginInit();
            this.snoozeAlertContentPanel.SuspendLayout();
            this.configureAlertSnoozePanel.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.snoozeMinutesSpinner)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.snoozeImage)).BeginInit();
            this.SuspendLayout();
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.okButton);
            this.panel1.Controls.Add(this.cancelButton);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.panel1.Location = new System.Drawing.Point(0, 289);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(568, 44);
            this.panel1.TabIndex = 0;
            // 
            // okButton
            // 
            this.okButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.okButton.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.okButton.Location = new System.Drawing.Point(400, 9);
            this.okButton.Name = "okButton";
            this.okButton.Size = new System.Drawing.Size(75, 23);
            this.okButton.TabIndex = 1;
            this.okButton.Text = "OK";
            this.okButton.UseVisualStyleBackColor = true;
            this.okButton.Click += new System.EventHandler(this.okButton_Click);
            // 
            // cancelButton
            // 
            this.cancelButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.cancelButton.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.cancelButton.Location = new System.Drawing.Point(481, 9);
            this.cancelButton.Name = "cancelButton";
            this.cancelButton.Size = new System.Drawing.Size(75, 23);
            this.cancelButton.TabIndex = 0;
            this.cancelButton.Text = "Cancel";
            this.cancelButton.UseVisualStyleBackColor = true;
            // 
            // contentStackPanel
            // 
            this.contentStackPanel.ActiveControl = this.unsnoozeAlertContentPanel;
            this.contentStackPanel.Controls.Add(this.unsnoozeAlertContentPanel);
            this.contentStackPanel.Controls.Add(this.snoozeAlertContentPanel);
            this.contentStackPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.contentStackPanel.Location = new System.Drawing.Point(0, 0);
            this.contentStackPanel.Name = "contentStackPanel";
            this.contentStackPanel.Size = new System.Drawing.Size(568, 289);
            this.contentStackPanel.TabIndex = 1;
            // 
            // unsnoozeAlertContentPanel
            // 
            this.unsnoozeAlertContentPanel.Controls.Add(this.unsnoozeInfoBox);
            this.unsnoozeAlertContentPanel.Controls.Add(this.unsnoozeImage);
            this.unsnoozeAlertContentPanel.Controls.Add(this.resumeToggleCheckBox);
            this.unsnoozeAlertContentPanel.Controls.Add(this.snoozedAlertsGrid);
            this.unsnoozeAlertContentPanel.Location = new System.Drawing.Point(0, 0);
            this.unsnoozeAlertContentPanel.Name = "unsnoozeAlertContentPanel";
            this.unsnoozeAlertContentPanel.Size = new System.Drawing.Size(568, 289);
            this.unsnoozeAlertContentPanel.TabIndex = 1;
            // 
            // unsnoozeInfoBox
            // 
            this.unsnoozeInfoBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.unsnoozeInfoBox.Location = new System.Drawing.Point(67, 22);
            this.unsnoozeInfoBox.Name = "unsnoozeInfoBox";
            this.unsnoozeInfoBox.Size = new System.Drawing.Size(473, 44);
            this.unsnoozeInfoBox.TabIndex = 11;
            this.unsnoozeInfoBox.Text = "You are about to resume alerts for {0}. If an alert is raised for this metric, it" +
                " will be reflected in the state of your monitored server. Would you like to cont" +
                "inue?";
            // 
            // unsnoozeImage
            // 
            this.unsnoozeImage.Image = global::Idera.SQLdm.DesktopClient.Properties.Resources.AlarmClockStart32x32;
            this.unsnoozeImage.Location = new System.Drawing.Point(29, 22);
            this.unsnoozeImage.Name = "unsnoozeImage";
            this.unsnoozeImage.Size = new System.Drawing.Size(32, 32);
            this.unsnoozeImage.SizeMode = System.Windows.Forms.PictureBoxSizeMode.AutoSize;
            this.unsnoozeImage.TabIndex = 10;
            this.unsnoozeImage.TabStop = false;
            // 
            // resumeToggleCheckBox
            // 
            this.resumeToggleCheckBox.AutoSize = true;
            this.resumeToggleCheckBox.Location = new System.Drawing.Point(51, 69);
            this.resumeToggleCheckBox.Name = "resumeToggleCheckBox";
            this.resumeToggleCheckBox.Size = new System.Drawing.Size(79, 17);
            this.resumeToggleCheckBox.TabIndex = 9;
            this.resumeToggleCheckBox.Text = "Resume All";
            this.resumeToggleCheckBox.UseVisualStyleBackColor = true;
            this.resumeToggleCheckBox.CheckedChanged += new System.EventHandler(this.resumeToggleCheckBox_CheckedChanged);
            // 
            // snoozedAlertsGrid
            // 
            this.snoozedAlertsGrid.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            appearance13.BackColor = System.Drawing.SystemColors.Window;
            appearance13.BorderColor = System.Drawing.Color.Silver;
            this.snoozedAlertsGrid.DisplayLayout.Appearance = appearance13;
            this.snoozedAlertsGrid.DisplayLayout.AutoFitStyle = Infragistics.Win.UltraWinGrid.AutoFitStyle.ExtendLastColumn;
            ultraGridColumn1.CellActivation = Infragistics.Win.UltraWinGrid.Activation.NoEdit;
            ultraGridColumn1.Header.VisiblePosition = 2;
            ultraGridColumn1.Hidden = true;
            ultraGridColumn2.CellActivation = Infragistics.Win.UltraWinGrid.Activation.NoEdit;
            ultraGridColumn2.CellClickAction = Infragistics.Win.UltraWinGrid.CellClickAction.RowSelect;
            ultraGridColumn2.Header.VisiblePosition = 1;
            ultraGridColumn2.Width = 182;
            ultraGridColumn3.CellActivation = Infragistics.Win.UltraWinGrid.Activation.NoEdit;
            ultraGridColumn3.CellClickAction = Infragistics.Win.UltraWinGrid.CellClickAction.RowSelect;
            appearance14.TextHAlignAsString = "Center";
            ultraGridColumn3.Header.Appearance = appearance14;
            ultraGridColumn3.Header.Caption = "Resume";
            ultraGridColumn3.Header.VisiblePosition = 0;
            ultraGridBand1.Columns.AddRange(new object[] {
            ultraGridColumn1,
            ultraGridColumn2,
            ultraGridColumn3});
            this.snoozedAlertsGrid.DisplayLayout.BandsSerializer.Add(ultraGridBand1);
            this.snoozedAlertsGrid.DisplayLayout.BorderStyle = Infragistics.Win.UIElementBorderStyle.Solid;
            this.snoozedAlertsGrid.DisplayLayout.CaptionVisible = Infragistics.Win.DefaultableBoolean.False;
            appearance15.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(227)))), ((int)(((byte)(239)))), ((int)(((byte)(255)))));
            appearance15.BackColor2 = System.Drawing.SystemColors.ControlDark;
            appearance15.BackGradientStyle = Infragistics.Win.GradientStyle.None;
            appearance15.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(101)))), ((int)(((byte)(147)))), ((int)(((byte)(207)))));
            this.snoozedAlertsGrid.DisplayLayout.GroupByBox.Appearance = appearance15;
            appearance16.ForeColor = System.Drawing.SystemColors.GrayText;
            this.snoozedAlertsGrid.DisplayLayout.GroupByBox.BandLabelAppearance = appearance16;
            this.snoozedAlertsGrid.DisplayLayout.GroupByBox.BorderStyle = Infragistics.Win.UIElementBorderStyle.Solid;
            this.snoozedAlertsGrid.DisplayLayout.GroupByBox.Hidden = true;
            appearance17.BackColor = System.Drawing.SystemColors.ControlLightLight;
            appearance17.BackColor2 = System.Drawing.SystemColors.Control;
            appearance17.BackGradientStyle = Infragistics.Win.GradientStyle.Horizontal;
            appearance17.ForeColor = System.Drawing.SystemColors.GrayText;
            this.snoozedAlertsGrid.DisplayLayout.GroupByBox.PromptAppearance = appearance17;
            this.snoozedAlertsGrid.DisplayLayout.MaxColScrollRegions = 1;
            this.snoozedAlertsGrid.DisplayLayout.MaxRowScrollRegions = 1;
            this.snoozedAlertsGrid.DisplayLayout.Override.AllowAddNew = Infragistics.Win.UltraWinGrid.AllowAddNew.No;
            this.snoozedAlertsGrid.DisplayLayout.Override.AllowDelete = Infragistics.Win.DefaultableBoolean.False;
            this.snoozedAlertsGrid.DisplayLayout.Override.AllowUpdate = Infragistics.Win.DefaultableBoolean.False;
            this.snoozedAlertsGrid.DisplayLayout.Override.BorderStyleCell = Infragistics.Win.UIElementBorderStyle.Solid;
            this.snoozedAlertsGrid.DisplayLayout.Override.BorderStyleRow = Infragistics.Win.UIElementBorderStyle.Solid;
            appearance18.BackColor = System.Drawing.SystemColors.Window;
            this.snoozedAlertsGrid.DisplayLayout.Override.CardAreaAppearance = appearance18;
            appearance19.BorderColor = System.Drawing.Color.Silver;
            appearance19.TextTrimming = Infragistics.Win.TextTrimming.EllipsisCharacter;
            this.snoozedAlertsGrid.DisplayLayout.Override.CellAppearance = appearance19;
            this.snoozedAlertsGrid.DisplayLayout.Override.CellClickAction = Infragistics.Win.UltraWinGrid.CellClickAction.RowSelect;
            this.snoozedAlertsGrid.DisplayLayout.Override.CellPadding = 0;
            this.snoozedAlertsGrid.DisplayLayout.Override.FixedHeaderIndicator = Infragistics.Win.UltraWinGrid.FixedHeaderIndicator.None;
            this.snoozedAlertsGrid.DisplayLayout.Override.GroupByColumnsHidden = Infragistics.Win.DefaultableBoolean.False;
            appearance20.BackColor = System.Drawing.SystemColors.Control;
            appearance20.BackColor2 = System.Drawing.SystemColors.ControlDark;
            appearance20.BackGradientAlignment = Infragistics.Win.GradientAlignment.Element;
            appearance20.BackGradientStyle = Infragistics.Win.GradientStyle.Horizontal;
            appearance20.BorderColor = System.Drawing.SystemColors.Window;
            this.snoozedAlertsGrid.DisplayLayout.Override.GroupByRowAppearance = appearance20;
            this.snoozedAlertsGrid.DisplayLayout.Override.GroupByRowInitialExpansionState = Infragistics.Win.UltraWinGrid.GroupByRowInitialExpansionState.Expanded;
            this.snoozedAlertsGrid.DisplayLayout.Override.GroupBySummaryDisplayStyle = Infragistics.Win.UltraWinGrid.GroupBySummaryDisplayStyle.SummaryCells;
            appearance21.TextHAlignAsString = "Left";
            this.snoozedAlertsGrid.DisplayLayout.Override.HeaderAppearance = appearance21;
            this.snoozedAlertsGrid.DisplayLayout.Override.HeaderClickAction = Infragistics.Win.UltraWinGrid.HeaderClickAction.SortMulti;
            appearance22.BackColor = System.Drawing.SystemColors.Window;
            appearance22.BorderColor = System.Drawing.Color.Silver;
            this.snoozedAlertsGrid.DisplayLayout.Override.RowAppearance = appearance22;
            this.snoozedAlertsGrid.DisplayLayout.Override.RowSelectors = Infragistics.Win.DefaultableBoolean.False;
            this.snoozedAlertsGrid.DisplayLayout.Override.SelectTypeCell = Infragistics.Win.UltraWinGrid.SelectType.None;
            this.snoozedAlertsGrid.DisplayLayout.Override.SelectTypeCol = Infragistics.Win.UltraWinGrid.SelectType.None;
            this.snoozedAlertsGrid.DisplayLayout.Override.SelectTypeGroupByRow = Infragistics.Win.UltraWinGrid.SelectType.None;
            this.snoozedAlertsGrid.DisplayLayout.Override.SelectTypeRow = Infragistics.Win.UltraWinGrid.SelectType.Single;
            this.snoozedAlertsGrid.DisplayLayout.Override.SummaryDisplayArea = Infragistics.Win.UltraWinGrid.SummaryDisplayAreas.InGroupByRows;
            appearance23.BackColor = System.Drawing.SystemColors.ControlLight;
            this.snoozedAlertsGrid.DisplayLayout.Override.TemplateAddRowAppearance = appearance23;
            this.snoozedAlertsGrid.DisplayLayout.Override.TipStyleCell = Infragistics.Win.UltraWinGrid.TipStyle.Hide;
            this.snoozedAlertsGrid.DisplayLayout.ScrollBounds = Infragistics.Win.UltraWinGrid.ScrollBounds.ScrollToFill;
            this.snoozedAlertsGrid.DisplayLayout.ScrollStyle = Infragistics.Win.UltraWinGrid.ScrollStyle.Immediate;
            this.snoozedAlertsGrid.DisplayLayout.UseFixedHeaders = true;
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
            this.snoozedAlertsGrid.DisplayLayout.ValueLists.AddRange(new Infragistics.Win.ValueList[] {
            valueList1});
            this.snoozedAlertsGrid.DisplayLayout.ViewStyle = Infragistics.Win.UltraWinGrid.ViewStyle.SingleBand;
            this.snoozedAlertsGrid.DisplayLayout.ViewStyleBand = Infragistics.Win.UltraWinGrid.ViewStyleBand.OutlookGroupBy;
            this.snoozedAlertsGrid.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.snoozedAlertsGrid.Location = new System.Drawing.Point(29, 88);
            this.snoozedAlertsGrid.Name = "snoozedAlertsGrid";
            this.snoozedAlertsGrid.Size = new System.Drawing.Size(511, 190);
            this.snoozedAlertsGrid.TabIndex = 8;
            this.snoozedAlertsGrid.UpdateMode = Infragistics.Win.UltraWinGrid.UpdateMode.OnCellChangeOrLostFocus;
            this.snoozedAlertsGrid.MouseClick += new System.Windows.Forms.MouseEventHandler(this.snoozedAlertsGrid_MouseClick);
            // 
            // snoozeAlertContentPanel
            // 
            this.snoozeAlertContentPanel.Controls.Add(this.configureAlertSnoozePanel);
            this.snoozeAlertContentPanel.Controls.Add(this.label4);
            this.snoozeAlertContentPanel.Controls.Add(this.snoozeInfoBox);
            this.snoozeAlertContentPanel.Controls.Add(this.snoozeImage);
            this.snoozeAlertContentPanel.Location = new System.Drawing.Point(0, 0);
            this.snoozeAlertContentPanel.Name = "snoozeAlertContentPanel";
            this.snoozeAlertContentPanel.Size = new System.Drawing.Size(568, 289);
            this.snoozeAlertContentPanel.TabIndex = 0;
            // 
            // configureAlertSnoozePanel
            // 
            this.configureAlertSnoozePanel.Controls.Add(this.snoozeMinutesSpinner);
            this.configureAlertSnoozePanel.Controls.Add(this.label3);
            this.configureAlertSnoozePanel.Controls.Add(this.label1);
            this.configureAlertSnoozePanel.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.configureAlertSnoozePanel.Location = new System.Drawing.Point(0, 254);
            this.configureAlertSnoozePanel.Name = "configureAlertSnoozePanel";
            this.configureAlertSnoozePanel.Size = new System.Drawing.Size(568, 33);
            this.configureAlertSnoozePanel.TabIndex = 2;
            // 
            // snoozeMinutesSpinner
            // 
            this.snoozeMinutesSpinner.Location = new System.Drawing.Point(275, 2);
            this.snoozeMinutesSpinner.Maximum = new decimal(new int[] {
            1440,
            0,
            0,
            0});
            this.snoozeMinutesSpinner.Minimum = new decimal(new int[] {
            10,
            0,
            0,
            0});
            this.snoozeMinutesSpinner.Name = "snoozeMinutesSpinner";
            this.snoozeMinutesSpinner.Size = new System.Drawing.Size(48, 20);
            this.snoozeMinutesSpinner.TabIndex = 2;
            this.snoozeMinutesSpinner.Value = new decimal(new int[] {
            60,
            0,
            0,
            0});
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(329, 4);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(43, 13);
            this.label3.TabIndex = 5;
            this.label3.Text = "minutes";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(177, 4);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(92, 13);
            this.label1.TabIndex = 1;
            this.label1.Text = "Snooze alert(s) for";
            // 
            // label4
            // 
            this.label4.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.label4.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.label4.Location = new System.Drawing.Point(0, 287);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(568, 2);
            this.label4.TabIndex = 6;
            // 
            // snoozeInfoBox
            // 
            this.snoozeInfoBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.snoozeInfoBox.Location = new System.Drawing.Point(67, 22);
            this.snoozeInfoBox.Name = "snoozeInfoBox";
            this.snoozeInfoBox.Size = new System.Drawing.Size(489, 32);
            this.snoozeInfoBox.TabIndex = 4;
            this.snoozeInfoBox.Text = resources.GetString("snoozeInfoBox.Text");
            this.snoozeInfoBox.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // snoozeImage
            // 
            this.snoozeImage.Image = global::Idera.SQLdm.DesktopClient.Properties.Resources.AlarmClockPause32x32;
            this.snoozeImage.Location = new System.Drawing.Point(29, 22);
            this.snoozeImage.Name = "snoozeImage";
            this.snoozeImage.Size = new System.Drawing.Size(32, 32);
            this.snoozeImage.SizeMode = System.Windows.Forms.PictureBoxSizeMode.AutoSize;
            this.snoozeImage.TabIndex = 3;
            this.snoozeImage.TabStop = false;
            // 
            // SnoozeAlertsDialog
            // 
            this.AcceptButton = this.okButton;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.cancelButton;
            this.ClientSize = new System.Drawing.Size(568, 333);
            this.Controls.Add(this.contentStackPanel);
            this.Controls.Add(this.panel1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.HelpButton = true;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "SnoozeAlertsDialog";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Snooze{0}Alerts - {1}";
            this.Load += new System.EventHandler(this.SnoozeAlertsDialog_Load);
            this.HelpButtonClicked += new System.ComponentModel.CancelEventHandler(this.SnoozeAlertsDialog_HelpButtonClicked);
            this.HelpRequested += new System.Windows.Forms.HelpEventHandler(this.SnoozeAlertsDialog_HelpRequested);
            this.panel1.ResumeLayout(false);
            this.contentStackPanel.ResumeLayout(false);
            this.unsnoozeAlertContentPanel.ResumeLayout(false);
            this.unsnoozeAlertContentPanel.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.unsnoozeImage)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.snoozedAlertsGrid)).EndInit();
            this.snoozeAlertContentPanel.ResumeLayout(false);
            this.snoozeAlertContentPanel.PerformLayout();
            this.configureAlertSnoozePanel.ResumeLayout(false);
            this.configureAlertSnoozePanel.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.snoozeMinutesSpinner)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.snoozeImage)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Button okButton;
        private System.Windows.Forms.Button cancelButton;
        private Idera.SQLdm.Common.UI.Controls.StackLayoutPanel contentStackPanel;
        private System.Windows.Forms.Panel unsnoozeAlertContentPanel;
        private System.Windows.Forms.Panel snoozeAlertContentPanel;
        private System.Windows.Forms.NumericUpDown snoozeMinutesSpinner;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.PictureBox snoozeImage;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label snoozeInfoBox;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Panel configureAlertSnoozePanel;
        private Infragistics.Win.UltraWinGrid.UltraGrid snoozedAlertsGrid;
        private System.Windows.Forms.CheckBox resumeToggleCheckBox;
        private System.Windows.Forms.Label unsnoozeInfoBox;
        private System.Windows.Forms.PictureBox unsnoozeImage;
    }
}
