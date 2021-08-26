namespace Idera.SQLdm.DesktopClient.Dialogs
{
    partial class BaselineWizard
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
            Infragistics.Win.Appearance appearance1 = new Infragistics.Win.Appearance();
            Infragistics.Win.UltraWinGrid.UltraGridBand ultraGridBand1 = new Infragistics.Win.UltraWinGrid.UltraGridBand("BaselineItemData", -1);
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn1 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("Name", -1, null, 0, Infragistics.Win.UltraWinGrid.SortIndicator.Ascending, false);
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn2 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("Description");
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn3 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("Category");
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn4 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("Unit");
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn5 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("CollectionDateTime");
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn6 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("Average");
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn7 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("Deviation");
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn8 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("Count");
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn9 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("ReferenceRangeStart");
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn10 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("ReferenceRangeEnd");
            Infragistics.Win.Appearance appearance2 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance3 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance4 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance5 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance6 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance7 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance8 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance9 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance10 = new Infragistics.Win.Appearance();
            Infragistics.Win.ValueList valueList1 = new Controls.CustomControls.CustomValueList(8767172);
            Infragistics.Win.ValueListItem valueListItem1 = new Infragistics.Win.ValueListItem();
            Infragistics.Win.Appearance appearance11 = new Infragistics.Win.Appearance();
            Infragistics.Win.ValueListItem valueListItem2 = new Infragistics.Win.ValueListItem();
            Infragistics.Win.Appearance appearance12 = new Infragistics.Win.Appearance();
            Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomDropDownEditorButton dropDownEditorButton1 = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomDropDownEditorButton("DropDownList");
            Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomDropDownEditorButton dropDownEditorButton2 = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomDropDownEditorButton("DropDownList");
            this.wizard1 = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CusotmWizard();

            this.introductionPage1 = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomIntroductionPage();
            this.configPage = new Divelements.WizardFramework.WizardPage();
            this.endDateEditor = new Infragistics.Win.UltraWinEditors.UltraDateTimeEditor();
            this.label2 = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomLabel();
            this.label3 = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomLabel();
            this.informationBox1 = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomInformationBox();
            this.startDateEditor = new Infragistics.Win.UltraWinEditors.UltraDateTimeEditor();
            this.waitPage = new Divelements.WizardFramework.WizardPage();
            this.loadingCircle1 = new MRG.Controls.UI.LoadingCircle();
            this.previewPage = new Divelements.WizardFramework.WizardPage();
            this.alertsGrid = new Infragistics.Win.UltraWinGrid.UltraGrid();
            this.finishPage = new Divelements.WizardFramework.FinishPage();
            this.backgroundWorker = new System.ComponentModel.BackgroundWorker();
            this.endTimeEditor = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomTimeComboEditor();
            this.startTimeEditor = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomTimeComboEditor();
            this.bindingSource = new System.Windows.Forms.BindingSource(this.components);
            this.wizard1.SuspendLayout();
            this.configPage.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.endDateEditor)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.startDateEditor)).BeginInit();
            this.waitPage.SuspendLayout();
            this.previewPage.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.alertsGrid)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.endTimeEditor)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.startTimeEditor)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.bindingSource)).BeginInit();
            this.SuspendLayout();
            // 
            // wizard1
            // 
            this.wizard1.AnimatePageTransitions = false;
            this.wizard1.Controls.Add(this.introductionPage1);
            this.wizard1.Controls.Add(this.waitPage);
            this.wizard1.Controls.Add(this.previewPage);
            this.wizard1.Controls.Add(this.configPage);
            this.wizard1.Controls.Add(this.finishPage);
            this.wizard1.Location = new System.Drawing.Point(0, 0);
            this.wizard1.Name = "wizard1";
            this.wizard1.OwnerForm = this;
            this.wizard1.Size = new System.Drawing.Size(634, 448);
            this.wizard1.TabIndex = 0;
            this.wizard1.UserExperienceType = Divelements.WizardFramework.WizardUserExperienceType.Wizard97;
            this.wizard1.Finish += new System.EventHandler(this.wizard1_Finish);
            this.wizard1.Cancel += new System.EventHandler(this.wizard1_Cancel);
            // 
            // introductionPage1
            // 
            this.introductionPage1.BackColor = System.Drawing.SystemColors.Window;
            this.introductionPage1.IntroductionText = "This wizard will walk you through setting a baseline for a monitored instance.";
            this.introductionPage1.Location = new System.Drawing.Point(175, 71);
            this.introductionPage1.Name = "introductionPage1";
            this.introductionPage1.NextPage = this.configPage;
            this.introductionPage1.Size = new System.Drawing.Size(437, 319);
            this.introductionPage1.TabIndex = 1004;
            this.introductionPage1.Text = "Welcome to the Baseline Wizard";
            this.introductionPage1.BeforeDisplay += new System.EventHandler(this.introductionPage1_BeforeDisplay);
            // 
            // configPage
            // 
            this.configPage.Controls.Add(this.endTimeEditor);
            this.configPage.Controls.Add(this.endDateEditor);
            this.configPage.Controls.Add(this.label2);
            this.configPage.Controls.Add(this.label3);
            this.configPage.Controls.Add(this.informationBox1);
            this.configPage.Controls.Add(this.startTimeEditor);
            this.configPage.Controls.Add(this.startDateEditor);
            this.configPage.Description = "Set the start and end range to use for calculating the baseline.";
            this.configPage.Location = new System.Drawing.Point(11, 71);
            this.configPage.Name = "configPage";
            this.configPage.NextPage = this.waitPage;
            this.configPage.PreviousPage = this.introductionPage1;
            this.configPage.Size = new System.Drawing.Size(612, 319);
            this.configPage.TabIndex = 1005;
            this.configPage.Text = "Configure Baseline";
            this.configPage.BeforeDisplay += new System.EventHandler(this.configPage_BeforeDisplay);
            // 
            // endDateEditor
            // 
            this.endDateEditor.Location = new System.Drawing.Point(113, 90);
            this.endDateEditor.Name = "endDateEditor";
            this.endDateEditor.Size = new System.Drawing.Size(86, 21);
            this.endDateEditor.TabIndex = 12;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(53, 94);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(29, 13);
            this.label2.TabIndex = 11;
            this.label2.Text = "End:";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(53, 62);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(32, 13);
            this.label3.TabIndex = 10;
            this.label3.Text = "Start:";
            // 
            // informationBox1
            // 
            this.informationBox1.Location = new System.Drawing.Point(11, 16);
            this.informationBox1.Name = "informationBox1";
            this.informationBox1.Size = new System.Drawing.Size(376, 40);
            this.informationBox1.TabIndex = 9;
            this.informationBox1.Text = "Enter the time period to use for calculate the baseline.  You should select a per" +
                "iod of time that your server was running well with a normal workload.";
            // 
            // startDateEditor
            // 
            this.startDateEditor.Location = new System.Drawing.Point(113, 63);
            this.startDateEditor.Name = "startDateEditor";
            this.startDateEditor.Size = new System.Drawing.Size(86, 21);
            this.startDateEditor.TabIndex = 7;
            // 
            // waitPage
            // 
            this.waitPage.AllowMoveNext = false;
            this.waitPage.Controls.Add(this.loadingCircle1);
            this.waitPage.Description = "Please wait for the baseline calculations to finish.";
            this.waitPage.Location = new System.Drawing.Point(11, 71);
            this.waitPage.Name = "waitPage";
            this.waitPage.NextPage = this.previewPage;
            this.waitPage.PreviousPage = this.configPage;
            this.waitPage.Size = new System.Drawing.Size(612, 319);
            this.waitPage.TabIndex = 1007;
            this.waitPage.Text = "Calculating Baseline";
            this.waitPage.BeforeMoveNext += new Divelements.WizardFramework.WizardPageEventHandler(this.waitPage_BeforeMoveNext);
            this.waitPage.BeforeMoveBack += new Divelements.WizardFramework.WizardPageEventHandler(this.waitPage_BeforeMoveBack);
            this.waitPage.BeforeDisplay += new System.EventHandler(this.waitPage_BeforeDisplay);
            // 
            // loadingCircle1
            // 
            this.loadingCircle1.Active = false;
            this.loadingCircle1.Color = System.Drawing.Color.DarkGray;
            this.loadingCircle1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.loadingCircle1.InnerCircleRadius = 8;
            this.loadingCircle1.Location = new System.Drawing.Point(0, 0);
            this.loadingCircle1.Name = "loadingCircle1";
            this.loadingCircle1.NumberSpoke = 36;
            this.loadingCircle1.OuterCircleRadius = 9;
            this.loadingCircle1.RotationSpeed = 20;
            this.loadingCircle1.Size = new System.Drawing.Size(612, 319);
            this.loadingCircle1.SpokeThickness = 4;
            this.loadingCircle1.TabIndex = 0;
            this.loadingCircle1.Text = "loadingCircle1";
            // 
            // previewPage
            // 
            this.previewPage.Controls.Add(this.alertsGrid);
            this.previewPage.Description = "";
            this.previewPage.Location = new System.Drawing.Point(11, 71);
            this.previewPage.Name = "previewPage";
            this.previewPage.NextPage = this.finishPage;
            this.previewPage.PreviousPage = this.configPage;
            this.previewPage.Size = new System.Drawing.Size(612, 319);
            this.previewPage.TabIndex = 1008;
            this.previewPage.Text = "Review Baseline";
            // 
            // alertsGrid
            // 
            this.alertsGrid.DataSource = this.bindingSource;
            appearance1.BackColor = System.Drawing.SystemColors.Window;
            appearance1.BorderColor = System.Drawing.SystemColors.InactiveCaption;
            this.alertsGrid.DisplayLayout.Appearance = appearance1;
            this.alertsGrid.DisplayLayout.AutoFitStyle = Infragistics.Win.UltraWinGrid.AutoFitStyle.ExtendLastColumn;
            ultraGridBand1.CardSettings.Style = Infragistics.Win.UltraWinGrid.CardStyle.StandardLabels;
            ultraGridBand1.ColHeaderLines = 2;
            ultraGridColumn1.Header.VisiblePosition = 0;
            ultraGridColumn1.Width = 190;
            ultraGridColumn2.Header.VisiblePosition = 2;
            ultraGridColumn2.Hidden = true;
            ultraGridColumn3.Header.VisiblePosition = 1;
            ultraGridColumn3.Hidden = true;
            ultraGridColumn4.Header.VisiblePosition = 9;
            ultraGridColumn4.Hidden = true;
            ultraGridColumn5.ExcludeFromColumnChooser = Infragistics.Win.UltraWinGrid.ExcludeFromColumnChooser.True;
            ultraGridColumn5.Header.VisiblePosition = 8;
            ultraGridColumn5.Hidden = true;
            ultraGridColumn6.Header.VisiblePosition = 5;
            ultraGridColumn6.Width = 76;
            ultraGridColumn7.Header.Caption = "Standard Deviation";
            ultraGridColumn7.Header.VisiblePosition = 6;
            ultraGridColumn7.Width = 83;
            ultraGridColumn8.Header.Caption = "Record Count";
            ultraGridColumn8.Header.VisiblePosition = 7;
            ultraGridColumn8.Width = 65;
            ultraGridColumn9.Header.Caption = "Reference Range Start";
            ultraGridColumn9.Header.VisiblePosition = 3;
            ultraGridColumn9.Width = 91;
            ultraGridColumn10.Header.Caption = "Reference Range End";
            ultraGridColumn10.Header.VisiblePosition = 4;
            ultraGridColumn10.Width = 89;
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
            ultraGridColumn10});
            ultraGridBand1.Indentation = 0;
            ultraGridBand1.IndentationGroupByRow = 0;
            ultraGridBand1.IndentationGroupByRowExpansionIndicator = 0;
            ultraGridBand1.Override.AllowUpdate = Infragistics.Win.DefaultableBoolean.True;
            this.alertsGrid.DisplayLayout.BandsSerializer.Add(ultraGridBand1);
            appearance2.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(227)))), ((int)(((byte)(239)))), ((int)(((byte)(255)))));
            appearance2.BackColor2 = System.Drawing.SystemColors.ControlDark;
            appearance2.BackGradientStyle = Infragistics.Win.GradientStyle.None;
            appearance2.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(101)))), ((int)(((byte)(147)))), ((int)(((byte)(207)))));
            this.alertsGrid.DisplayLayout.GroupByBox.Appearance = appearance2;
            appearance3.ForeColor = System.Drawing.SystemColors.GrayText;
            this.alertsGrid.DisplayLayout.GroupByBox.BandLabelAppearance = appearance3;
            this.alertsGrid.DisplayLayout.GroupByBox.BorderStyle = Infragistics.Win.UIElementBorderStyle.Solid;
            this.alertsGrid.DisplayLayout.GroupByBox.Hidden = true;
            appearance4.BackColor = System.Drawing.SystemColors.ControlLightLight;
            appearance4.BackColor2 = System.Drawing.SystemColors.Control;
            appearance4.BackGradientStyle = Infragistics.Win.GradientStyle.Horizontal;
            appearance4.ForeColor = System.Drawing.SystemColors.GrayText;
            this.alertsGrid.DisplayLayout.GroupByBox.PromptAppearance = appearance4;
            this.alertsGrid.DisplayLayout.MaxColScrollRegions = 1;
            this.alertsGrid.DisplayLayout.MaxRowScrollRegions = 1;
            this.alertsGrid.DisplayLayout.Override.AllowAddNew = Infragistics.Win.UltraWinGrid.AllowAddNew.No;
            this.alertsGrid.DisplayLayout.Override.AllowDelete = Infragistics.Win.DefaultableBoolean.False;
            this.alertsGrid.DisplayLayout.Override.AllowUpdate = Infragistics.Win.DefaultableBoolean.True;
            this.alertsGrid.DisplayLayout.Override.BorderStyleCell = Infragistics.Win.UIElementBorderStyle.None;
            this.alertsGrid.DisplayLayout.Override.BorderStyleRow = Infragistics.Win.UIElementBorderStyle.None;
            appearance5.BackColor = System.Drawing.SystemColors.Window;
            this.alertsGrid.DisplayLayout.Override.CardAreaAppearance = appearance5;
            appearance6.BorderColor = System.Drawing.Color.Silver;
            appearance6.TextTrimming = Infragistics.Win.TextTrimming.EllipsisCharacter;
            this.alertsGrid.DisplayLayout.Override.CellAppearance = appearance6;
            this.alertsGrid.DisplayLayout.Override.CellClickAction = Infragistics.Win.UltraWinGrid.CellClickAction.RowSelect;
            this.alertsGrid.DisplayLayout.Override.CellPadding = 0;
            this.alertsGrid.DisplayLayout.Override.FixedHeaderIndicator = Infragistics.Win.UltraWinGrid.FixedHeaderIndicator.None;
            this.alertsGrid.DisplayLayout.Override.GroupByColumnsHidden = Infragistics.Win.DefaultableBoolean.False;
            appearance7.BackColor = System.Drawing.SystemColors.Control;
            appearance7.BackColor2 = System.Drawing.SystemColors.ControlDark;
            appearance7.BackGradientAlignment = Infragistics.Win.GradientAlignment.Element;
            appearance7.BackGradientStyle = Infragistics.Win.GradientStyle.Horizontal;
            appearance7.BorderColor = System.Drawing.SystemColors.Window;
            this.alertsGrid.DisplayLayout.Override.GroupByRowAppearance = appearance7;
            this.alertsGrid.DisplayLayout.Override.GroupByRowInitialExpansionState = Infragistics.Win.UltraWinGrid.GroupByRowInitialExpansionState.Expanded;
            appearance8.TextHAlignAsString = "Center";
            this.alertsGrid.DisplayLayout.Override.HeaderAppearance = appearance8;
            this.alertsGrid.DisplayLayout.Override.HeaderClickAction = Infragistics.Win.UltraWinGrid.HeaderClickAction.SortSingle;
            appearance9.BackColor = System.Drawing.SystemColors.Window;
            appearance9.BorderColor = System.Drawing.Color.Silver;
            this.alertsGrid.DisplayLayout.Override.RowAppearance = appearance9;
            this.alertsGrid.DisplayLayout.Override.RowSelectors = Infragistics.Win.DefaultableBoolean.False;
            this.alertsGrid.DisplayLayout.Override.SelectTypeCell = Infragistics.Win.UltraWinGrid.SelectType.Single;
            this.alertsGrid.DisplayLayout.Override.SelectTypeCol = Infragistics.Win.UltraWinGrid.SelectType.None;
            this.alertsGrid.DisplayLayout.Override.SelectTypeGroupByRow = Infragistics.Win.UltraWinGrid.SelectType.None;
            this.alertsGrid.DisplayLayout.Override.SelectTypeRow = Infragistics.Win.UltraWinGrid.SelectType.ExtendedAutoDrag;
            appearance10.BackColor = System.Drawing.SystemColors.ControlLight;
            this.alertsGrid.DisplayLayout.Override.TemplateAddRowAppearance = appearance10;
            this.alertsGrid.DisplayLayout.Override.WrapHeaderText = Infragistics.Win.DefaultableBoolean.True;
            this.alertsGrid.DisplayLayout.ScrollBounds = Infragistics.Win.UltraWinGrid.ScrollBounds.ScrollToFill;
            this.alertsGrid.DisplayLayout.ScrollStyle = Infragistics.Win.UltraWinGrid.ScrollStyle.Immediate;
            this.alertsGrid.DisplayLayout.UseFixedHeaders = true;
            valueList1.DisplayStyle = Infragistics.Win.ValueListDisplayStyle.Picture;
            valueList1.Key = "CheckBoxes";
            appearance11.ImageHAlign = Infragistics.Win.HAlign.Center;
            valueListItem1.Appearance = appearance11;
            valueListItem1.DataValue = false;
            appearance12.ImageHAlign = Infragistics.Win.HAlign.Center;
            valueListItem2.Appearance = appearance12;
            valueListItem2.DataValue = true;
            valueListItem2.DisplayText = "";
            valueList1.ValueListItems.AddRange(new Infragistics.Win.ValueListItem[] {
            valueListItem1,
            valueListItem2});
            this.alertsGrid.DisplayLayout.ValueLists.AddRange(new Infragistics.Win.ValueList[] {
            valueList1});
            this.alertsGrid.DisplayLayout.ViewStyle = Infragistics.Win.UltraWinGrid.ViewStyle.SingleBand;
            this.alertsGrid.DisplayLayout.ViewStyleBand = Infragistics.Win.UltraWinGrid.ViewStyleBand.OutlookGroupBy;
            this.alertsGrid.Dock = System.Windows.Forms.DockStyle.Fill;
            this.alertsGrid.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.alertsGrid.Location = new System.Drawing.Point(0, 0);
            this.alertsGrid.Name = "alertsGrid";
            this.alertsGrid.Size = new System.Drawing.Size(612, 319);
            this.alertsGrid.TabIndex = 6;
            this.alertsGrid.UpdateMode = Infragistics.Win.UltraWinGrid.UpdateMode.OnCellChangeOrLostFocus;
            // 
            // finishPage
            // 
            this.finishPage.BackColor = System.Drawing.SystemColors.Window;
            this.finishPage.FinishText = "When you press Finish the baseline will be saved to the SQLDM Repository.";
            this.finishPage.Location = new System.Drawing.Point(175, 71);
            this.finishPage.Name = "finishPage";
            this.finishPage.PreviousPage = this.previewPage;
            this.finishPage.Size = new System.Drawing.Size(437, 319);
            this.finishPage.TabIndex = 1006;
            this.finishPage.Text = "Completing the Baseline Wizard";
            // 
            // backgroundWorker
            // 
            this.backgroundWorker.WorkerSupportsCancellation = true;
            this.backgroundWorker.DoWork += new System.ComponentModel.DoWorkEventHandler(this.backgroundWorker_DoWork);
            this.backgroundWorker.RunWorkerCompleted += new System.ComponentModel.RunWorkerCompletedEventHandler(this.backgroundWorker_RunWorkerCompleted);
            // 
            // endTimeEditor
            // 
            dropDownEditorButton1.Key = "DropDownList";
            this.endTimeEditor.ButtonsRight.Add(dropDownEditorButton1);
            this.endTimeEditor.DropDownButtonDisplayStyle = Infragistics.Win.ButtonDisplayStyle.Never;
            this.endTimeEditor.ListInterval = System.TimeSpan.Parse("00:30:00");
            this.endTimeEditor.Location = new System.Drawing.Point(205, 90);
            this.endTimeEditor.MaskInput = "hh:mm tt";
            this.endTimeEditor.Name = "endTimeEditor";
            this.endTimeEditor.Size = new System.Drawing.Size(75, 21);
            this.endTimeEditor.TabIndex = 13;
            this.endTimeEditor.Time = System.TimeSpan.Parse("00:00:00");
            // 
            // startTimeEditor
            // 
            dropDownEditorButton2.Key = "DropDownList";
            this.startTimeEditor.ButtonsRight.Add(dropDownEditorButton2);
            this.startTimeEditor.DropDownButtonDisplayStyle = Infragistics.Win.ButtonDisplayStyle.Never;
            this.startTimeEditor.ListInterval = System.TimeSpan.Parse("00:30:00");
            this.startTimeEditor.Location = new System.Drawing.Point(205, 62);
            this.startTimeEditor.MaskInput = "hh:mm tt";
            this.startTimeEditor.Name = "startTimeEditor";
            this.startTimeEditor.Size = new System.Drawing.Size(75, 21);
            this.startTimeEditor.TabIndex = 8;
            this.startTimeEditor.Time = System.TimeSpan.Parse("00:00:00");
            // 
            // bindingSource
            // 
            this.bindingSource.AllowNew = false;
            this.bindingSource.DataSource = typeof(Idera.SQLdm.Common.Data.BaselineItemData);
            // 
            // BaselineWizard
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(634, 448);
            this.Controls.Add(this.wizard1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.HelpButton = true;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "BaselineWizard";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Set Baseline - {0}";
            this.Load += new System.EventHandler(this.BaselineWizard_Load);
            this.wizard1.ResumeLayout(false);
            this.configPage.ResumeLayout(false);
            this.configPage.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.endDateEditor)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.startDateEditor)).EndInit();
            this.waitPage.ResumeLayout(false);
            this.previewPage.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.alertsGrid)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.endTimeEditor)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.startTimeEditor)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.bindingSource)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private Divelements.WizardFramework.Wizard wizard1;
        private Divelements.WizardFramework.IntroductionPage introductionPage1;
        private Divelements.WizardFramework.WizardPage configPage;
        private Divelements.WizardFramework.WizardPage waitPage;
        private Divelements.WizardFramework.FinishPage finishPage;
        private Divelements.WizardFramework.WizardPage previewPage;
        private MRG.Controls.UI.LoadingCircle loadingCircle1;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomTimeComboEditor endTimeEditor;
        private Infragistics.Win.UltraWinEditors.UltraDateTimeEditor endDateEditor;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomLabel label2;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomLabel label3;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomInformationBox informationBox1;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomTimeComboEditor startTimeEditor;
        private Infragistics.Win.UltraWinEditors.UltraDateTimeEditor startDateEditor;
        private System.ComponentModel.BackgroundWorker backgroundWorker;
        private Infragistics.Win.UltraWinGrid.UltraGrid alertsGrid;
        private System.Windows.Forms.BindingSource bindingSource;

    }
}