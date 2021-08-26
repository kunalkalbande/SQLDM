namespace Idera.SQLdm.DesktopClient.Dialogs.Notification
{
    using Idera.SQLdm.Common.Objects;

    partial class TestSqlActionDialog
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
            Infragistics.Win.UltraWinGrid.UltraGridBand ultraGridBand1 = new Infragistics.Win.UltraWinGrid.UltraGridBand("Band 0", -1);
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn1 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("InstanceObject");
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn2 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("Test");
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn3 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("Instance", -1, null, 0, Infragistics.Win.UltraWinGrid.SortIndicator.Ascending, false);
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn4 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("Result");
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn5 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("Exception");
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn6 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("Duration");
            Infragistics.Win.Appearance appearance3 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance4 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance5 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance6 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance7 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance8 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance9 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance10 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance11 = new Infragistics.Win.Appearance();
            Infragistics.Win.ValueList valueList1 = new Controls.CustomControls.CustomValueList(84474157);
            Infragistics.Win.ValueListItem valueListItem1 = new Infragistics.Win.ValueListItem();
            Infragistics.Win.ValueListItem valueListItem2 = new Infragistics.Win.ValueListItem();
            Infragistics.Win.UltraWinDataSource.UltraDataColumn ultraDataColumn7 = new Infragistics.Win.UltraWinDataSource.UltraDataColumn("InstanceObject");
            Infragistics.Win.UltraWinDataSource.UltraDataColumn ultraDataColumn8 = new Infragistics.Win.UltraWinDataSource.UltraDataColumn("Test");
            Infragistics.Win.UltraWinDataSource.UltraDataColumn ultraDataColumn9 = new Infragistics.Win.UltraWinDataSource.UltraDataColumn("Instance");
            Infragistics.Win.UltraWinDataSource.UltraDataColumn ultraDataColumn10 = new Infragistics.Win.UltraWinDataSource.UltraDataColumn("Result");
            Infragistics.Win.UltraWinDataSource.UltraDataColumn ultraDataColumn11 = new Infragistics.Win.UltraWinDataSource.UltraDataColumn("Exception");
            Infragistics.Win.UltraWinDataSource.UltraDataColumn ultraDataColumn12 = new Infragistics.Win.UltraWinDataSource.UltraDataColumn("Duration");
            this.testButton = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomButton();
            this.doneButton = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomButton();
            this.groupBox2 = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomGroupBox();
            this.ultraGrid1 = new Infragistics.Win.UltraWinGrid.UltraGrid();
            this.serverGridDataSource = new Infragistics.Win.UltraWinDataSource.UltraDataSource(this.components);
            this.selectAllCheckBox = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomCheckBox();
            this.groupBox1 = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomGroupBox();
            this.contentStackLayoutPanel = new Idera.SQLdm.Common.UI.Controls.StackLayoutPanel();
            this.sqlBatchContentPanel = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomPanel ();
            this.panel1 = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomPanel ();
            this.label4 = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomLabel();
            this.descriptionLabel = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomLabel();
            this.label3 = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomLabel();
            this.counterDefinitionContentPanel = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomPanel ();
            this.counterNameLabel = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomLabel();
            this.objectNameLabel = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomLabel();
            this.jobNameLabel = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomLabel();
            this.counterDescriptionLabelLabel = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomLabel();
            this.counterNameContentPanel = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomPanel ();
            this.actionTypeLabel = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomLabel();
            this.label2 = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomLabel();
            this.backgroundWorker = new System.ComponentModel.BackgroundWorker();
            this.sqlBatchLabel = new Infragistics.Win.UltraWinEditors.UltraTextEditor();
            this.groupBox2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.ultraGrid1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.serverGridDataSource)).BeginInit();
            this.groupBox1.SuspendLayout();
            this.contentStackLayoutPanel.SuspendLayout();
            this.sqlBatchContentPanel.SuspendLayout();
            this.panel1.SuspendLayout();
            this.counterDefinitionContentPanel.SuspendLayout();
            this.counterNameContentPanel.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.sqlBatchLabel)).BeginInit();
            this.SuspendLayout();
            // 
            // testButton
            // 
            this.testButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.testButton.Location = new System.Drawing.Point(397, 352);
            this.testButton.Name = "testButton";
            this.testButton.Size = new System.Drawing.Size(75, 23);
            this.testButton.TabIndex = 2;
            this.testButton.Text = "Test";
            this.testButton.UseVisualStyleBackColor = true;
            this.testButton.Click += new System.EventHandler(this.testButton_Click);
            // 
            // doneButton
            // 
            this.doneButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.doneButton.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.doneButton.Location = new System.Drawing.Point(478, 352);
            this.doneButton.Name = "doneButton";
            this.doneButton.Size = new System.Drawing.Size(75, 23);
            this.doneButton.TabIndex = 3;
            this.doneButton.Text = "Done";
            this.doneButton.UseVisualStyleBackColor = true;
            this.doneButton.Click += new System.EventHandler(this.doneButton_Click);
            // 
            // groupBox2
            // 
            this.groupBox2.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBox2.Controls.Add(this.ultraGrid1);
            this.groupBox2.Controls.Add(this.selectAllCheckBox);
            this.groupBox2.Location = new System.Drawing.Point(9, 142);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Padding = new System.Windows.Forms.Padding(10);
            this.groupBox2.Size = new System.Drawing.Size(547, 201);
            this.groupBox2.TabIndex = 20;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Select instance(s) to test the action against:";
            // 
            // ultraGrid1
            // 
            this.ultraGrid1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.ultraGrid1.DataSource = this.serverGridDataSource;
            appearance1.BackColor = System.Drawing.SystemColors.Window;
            appearance1.BorderColor = System.Drawing.SystemColors.InactiveCaption;
            this.ultraGrid1.DisplayLayout.Appearance = appearance1;
            this.ultraGrid1.DisplayLayout.AutoFitStyle = Infragistics.Win.UltraWinGrid.AutoFitStyle.ExtendLastColumn;
            ultraGridColumn1.AutoCompleteMode = Infragistics.Win.AutoCompleteMode.Append;
            ultraGridColumn1.Header.VisiblePosition = 0;
            ultraGridColumn1.Hidden = true;
            ultraGridColumn2.AutoCompleteMode = Infragistics.Win.AutoCompleteMode.Append;
            ultraGridColumn2.Header.VisiblePosition = 1;
            ultraGridColumn3.AutoCompleteMode = Infragistics.Win.AutoCompleteMode.Append;
            ultraGridColumn3.Header.VisiblePosition = 2;
            ultraGridColumn3.Width = 169;
            ultraGridColumn4.AutoCompleteMode = Infragistics.Win.AutoCompleteMode.Append;
            ultraGridColumn4.Header.VisiblePosition = 4;
            ultraGridColumn5.AutoCompleteMode = Infragistics.Win.AutoCompleteMode.Append;
            ultraGridColumn5.Header.VisiblePosition = 5;
            ultraGridColumn5.Hidden = true;
            ultraGridColumn6.AutoCompleteMode = Infragistics.Win.AutoCompleteMode.Append;
            ultraGridColumn6.Format = "HH:mm:ss.FF";
            ultraGridColumn6.Header.VisiblePosition = 3;
            ultraGridBand1.Columns.AddRange(new object[] {
            ultraGridColumn1,
            ultraGridColumn2,
            ultraGridColumn3,
            ultraGridColumn4,
            ultraGridColumn5,
            ultraGridColumn6});
            this.ultraGrid1.DisplayLayout.BandsSerializer.Add(ultraGridBand1);
            this.ultraGrid1.DisplayLayout.BorderStyle = Infragistics.Win.UIElementBorderStyle.Solid;
            this.ultraGrid1.DisplayLayout.CaptionVisible = Infragistics.Win.DefaultableBoolean.False;
            appearance3.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(227)))), ((int)(((byte)(239)))), ((int)(((byte)(255)))));
            appearance3.BackColor2 = System.Drawing.SystemColors.ControlDark;
            appearance3.BackGradientStyle = Infragistics.Win.GradientStyle.None;
            appearance3.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(101)))), ((int)(((byte)(147)))), ((int)(((byte)(207)))));
            this.ultraGrid1.DisplayLayout.GroupByBox.Appearance = appearance3;
            appearance4.ForeColor = System.Drawing.SystemColors.GrayText;
            this.ultraGrid1.DisplayLayout.GroupByBox.BandLabelAppearance = appearance4;
            this.ultraGrid1.DisplayLayout.GroupByBox.BorderStyle = Infragistics.Win.UIElementBorderStyle.Solid;
            this.ultraGrid1.DisplayLayout.GroupByBox.Hidden = true;
            appearance5.BackColor = System.Drawing.SystemColors.ControlLightLight;
            appearance5.BackColor2 = System.Drawing.SystemColors.Control;
            appearance5.BackGradientStyle = Infragistics.Win.GradientStyle.Horizontal;
            appearance5.ForeColor = System.Drawing.SystemColors.GrayText;
            this.ultraGrid1.DisplayLayout.GroupByBox.PromptAppearance = appearance5;
            this.ultraGrid1.DisplayLayout.LoadStyle = Infragistics.Win.UltraWinGrid.LoadStyle.LoadOnDemand;
            this.ultraGrid1.DisplayLayout.MaxColScrollRegions = 1;
            this.ultraGrid1.DisplayLayout.MaxRowScrollRegions = 1;
            this.ultraGrid1.DisplayLayout.Override.AllowAddNew = Infragistics.Win.UltraWinGrid.AllowAddNew.No;
            this.ultraGrid1.DisplayLayout.Override.AllowColSizing = Infragistics.Win.UltraWinGrid.AllowColSizing.Free;
            this.ultraGrid1.DisplayLayout.Override.AllowDelete = Infragistics.Win.DefaultableBoolean.False;
            this.ultraGrid1.DisplayLayout.Override.AllowUpdate = Infragistics.Win.DefaultableBoolean.False;
            this.ultraGrid1.DisplayLayout.Override.BorderStyleCell = Infragistics.Win.UIElementBorderStyle.Solid;
            this.ultraGrid1.DisplayLayout.Override.BorderStyleRow = Infragistics.Win.UIElementBorderStyle.Solid;
            appearance6.BackColor = System.Drawing.SystemColors.Window;
            this.ultraGrid1.DisplayLayout.Override.CardAreaAppearance = appearance6;
            appearance7.BorderColor = System.Drawing.Color.Silver;
            appearance7.TextTrimming = Infragistics.Win.TextTrimming.EllipsisCharacter;
            this.ultraGrid1.DisplayLayout.Override.CellAppearance = appearance7;
            this.ultraGrid1.DisplayLayout.Override.CellClickAction = Infragistics.Win.UltraWinGrid.CellClickAction.RowSelect;
            this.ultraGrid1.DisplayLayout.Override.CellPadding = 0;
            this.ultraGrid1.DisplayLayout.Override.DefaultRowHeight = 20;
            this.ultraGrid1.DisplayLayout.Override.FixedHeaderIndicator = Infragistics.Win.UltraWinGrid.FixedHeaderIndicator.None;
            this.ultraGrid1.DisplayLayout.Override.GroupByColumnsHidden = Infragistics.Win.DefaultableBoolean.False;
            appearance8.BackColor = System.Drawing.SystemColors.Control;
            appearance8.BackColor2 = System.Drawing.SystemColors.ControlDark;
            appearance8.BackGradientAlignment = Infragistics.Win.GradientAlignment.Element;
            appearance8.BackGradientStyle = Infragistics.Win.GradientStyle.Horizontal;
            appearance8.BorderColor = System.Drawing.SystemColors.Window;
            this.ultraGrid1.DisplayLayout.Override.GroupByRowAppearance = appearance8;
            this.ultraGrid1.DisplayLayout.Override.GroupByRowInitialExpansionState = Infragistics.Win.UltraWinGrid.GroupByRowInitialExpansionState.Collapsed;
            appearance9.TextHAlignAsString = "Left";
            this.ultraGrid1.DisplayLayout.Override.HeaderAppearance = appearance9;
            this.ultraGrid1.DisplayLayout.Override.HeaderClickAction = Infragistics.Win.UltraWinGrid.HeaderClickAction.SortMulti;
            this.ultraGrid1.DisplayLayout.Override.MaxSelectedRows = 1;
            appearance10.BackColor = System.Drawing.SystemColors.Window;
            appearance10.BorderColor = System.Drawing.Color.Silver;
            this.ultraGrid1.DisplayLayout.Override.RowAppearance = appearance10;
            this.ultraGrid1.DisplayLayout.Override.RowSelectors = Infragistics.Win.DefaultableBoolean.False;
            this.ultraGrid1.DisplayLayout.Override.SelectTypeCell = Infragistics.Win.UltraWinGrid.SelectType.None;
            this.ultraGrid1.DisplayLayout.Override.SelectTypeCol = Infragistics.Win.UltraWinGrid.SelectType.None;
            this.ultraGrid1.DisplayLayout.Override.SelectTypeGroupByRow = Infragistics.Win.UltraWinGrid.SelectType.None;
            this.ultraGrid1.DisplayLayout.Override.SelectTypeRow = Infragistics.Win.UltraWinGrid.SelectType.Single;
            appearance11.BackColor = System.Drawing.SystemColors.ControlLight;
            this.ultraGrid1.DisplayLayout.Override.TemplateAddRowAppearance = appearance11;
            this.ultraGrid1.DisplayLayout.Override.WrapHeaderText = Infragistics.Win.DefaultableBoolean.False;
            this.ultraGrid1.DisplayLayout.ScrollBounds = Infragistics.Win.UltraWinGrid.ScrollBounds.ScrollToFill;
            this.ultraGrid1.DisplayLayout.ScrollStyle = Infragistics.Win.UltraWinGrid.ScrollStyle.Immediate;
            this.ultraGrid1.DisplayLayout.UseFixedHeaders = true;
            valueList1.DisplayStyle = Infragistics.Win.ValueListDisplayStyle.DisplayText;
            valueList1.Key = "BooleanYesNo";
            valueListItem1.DataValue = false;
            valueListItem1.DisplayText = "No";
            valueListItem2.DataValue = true;
            valueListItem2.DisplayText = "Yes";
            valueList1.ValueListItems.AddRange(new Infragistics.Win.ValueListItem[] {
            valueListItem1,
            valueListItem2});
            this.ultraGrid1.DisplayLayout.ValueLists.AddRange(new Infragistics.Win.ValueList[] {
            valueList1});
            this.ultraGrid1.DisplayLayout.ViewStyle = Infragistics.Win.UltraWinGrid.ViewStyle.SingleBand;
            this.ultraGrid1.DisplayLayout.ViewStyleBand = Infragistics.Win.UltraWinGrid.ViewStyleBand.OutlookGroupBy;
            this.ultraGrid1.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.ultraGrid1.Location = new System.Drawing.Point(13, 39);
            this.ultraGrid1.Name = "ultraGrid1";
            this.ultraGrid1.Size = new System.Drawing.Size(521, 149);
            this.ultraGrid1.TabIndex = 15;
            this.ultraGrid1.DoubleClickCell += new Infragistics.Win.UltraWinGrid.DoubleClickCellEventHandler(this.ultraGrid1_DoubleClickCell);
            this.ultraGrid1.MouseClick += new System.Windows.Forms.MouseEventHandler(this.ultraGrid1_MouseClick);
            // 
            // serverGridDataSource
            // 
            ultraDataColumn7.DataType = typeof(object);
            ultraDataColumn8.DataType = typeof(bool);
            ultraDataColumn8.DefaultValue = false;
            ultraDataColumn11.DataType = typeof(object);
            ultraDataColumn12.DataType = typeof(System.DateTime);
            this.serverGridDataSource.Band.Columns.AddRange(new object[] {
            ultraDataColumn7,
            ultraDataColumn8,
            ultraDataColumn9,
            ultraDataColumn10,
            ultraDataColumn11,
            ultraDataColumn12});
            // 
            // selectAllCheckBox
            // 
            this.selectAllCheckBox.AutoSize = true;
            this.selectAllCheckBox.Location = new System.Drawing.Point(13, 20);
            this.selectAllCheckBox.Name = "selectAllCheckBox";
            this.selectAllCheckBox.Size = new System.Drawing.Size(70, 17);
            this.selectAllCheckBox.TabIndex = 14;
            this.selectAllCheckBox.Text = "Select All";
            this.selectAllCheckBox.UseVisualStyleBackColor = true;
            this.selectAllCheckBox.CheckedChanged += new System.EventHandler(this.selectAllCheckBox_CheckedChanged);
            // 
            // groupBox1
            // 
            this.groupBox1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBox1.Controls.Add(this.contentStackLayoutPanel);
            this.groupBox1.Controls.Add(this.counterNameContentPanel);
            this.groupBox1.Location = new System.Drawing.Point(9, 12);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(547, 127);
            this.groupBox1.TabIndex = 19;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Action Information";
            // 
            // contentStackLayoutPanel
            // 
            this.contentStackLayoutPanel.ActiveControl = this.sqlBatchContentPanel;
            this.contentStackLayoutPanel.Controls.Add(this.sqlBatchContentPanel);
            this.contentStackLayoutPanel.Controls.Add(this.counterDefinitionContentPanel);
            this.contentStackLayoutPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.contentStackLayoutPanel.Location = new System.Drawing.Point(3, 35);
            this.contentStackLayoutPanel.Name = "contentStackLayoutPanel";
            this.contentStackLayoutPanel.Padding = new System.Windows.Forms.Padding(10, 3, 10, 0);
            this.contentStackLayoutPanel.Size = new System.Drawing.Size(541, 89);
            this.contentStackLayoutPanel.TabIndex = 11;
            // 
            // sqlBatchContentPanel
            // 
            this.sqlBatchContentPanel.Controls.Add(this.sqlBatchLabel);
            this.sqlBatchContentPanel.Controls.Add(this.panel1);
            this.sqlBatchContentPanel.Location = new System.Drawing.Point(10, 3);
            this.sqlBatchContentPanel.Name = "sqlBatchContentPanel";
            this.sqlBatchContentPanel.Size = new System.Drawing.Size(521, 86);
            this.sqlBatchContentPanel.TabIndex = 0;
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.label4);
            this.panel1.Controls.Add(this.descriptionLabel);
            this.panel1.Controls.Add(this.label3);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Top;
            this.panel1.Location = new System.Drawing.Point(0, 0);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(521, 29);
            this.panel1.TabIndex = 8;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(0, 0);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(63, 13);
            this.label4.TabIndex = 6;
            this.label4.Text = "Description:";
            // 
            // descriptionLabel
            // 
            this.descriptionLabel.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.descriptionLabel.AutoEllipsis = true;
            this.descriptionLabel.Location = new System.Drawing.Point(76, 0);
            this.descriptionLabel.Name = "descriptionLabel";
            this.descriptionLabel.Size = new System.Drawing.Size(442, 13);
            this.descriptionLabel.TabIndex = 7;
            this.descriptionLabel.Text = "{0}";
            // 
            // label3
            // 
            this.label3.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.label3.Location = new System.Drawing.Point(0, 16);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(506, 13);
            this.label3.TabIndex = 0;
            this.label3.Text = "SQL Script:";
            // 
            // counterDefinitionContentPanel
            // 
            this.counterDefinitionContentPanel.Controls.Add(this.counterNameLabel);
            this.counterDefinitionContentPanel.Controls.Add(this.objectNameLabel);
            this.counterDefinitionContentPanel.Controls.Add(this.jobNameLabel);
            this.counterDefinitionContentPanel.Controls.Add(this.counterDescriptionLabelLabel);
            this.counterDefinitionContentPanel.Location = new System.Drawing.Point(10, 3);
            this.counterDefinitionContentPanel.Name = "counterDefinitionContentPanel";
            this.counterDefinitionContentPanel.Size = new System.Drawing.Size(521, 86);
            this.counterDefinitionContentPanel.TabIndex = 9;
            // 
            // counterNameLabel
            // 
            this.counterNameLabel.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.counterNameLabel.AutoEllipsis = true;
            this.counterNameLabel.Location = new System.Drawing.Point(76, 21);
            this.counterNameLabel.Name = "counterNameLabel";
            this.counterNameLabel.Size = new System.Drawing.Size(455, 13);
            this.counterNameLabel.TabIndex = 3;
            this.counterNameLabel.Text = "{0}";
            // 
            // objectNameLabel
            // 
            this.objectNameLabel.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.objectNameLabel.AutoEllipsis = true;
            this.objectNameLabel.Location = new System.Drawing.Point(76, 2);
            this.objectNameLabel.Name = "objectNameLabel";
            this.objectNameLabel.Size = new System.Drawing.Size(455, 13);
            this.objectNameLabel.TabIndex = 1;
            this.objectNameLabel.Text = "{0}";
            // 
            // jobNameLabel
            // 
            this.jobNameLabel.AutoSize = true;
            this.jobNameLabel.Location = new System.Drawing.Point(0, 2);
            this.jobNameLabel.Name = "jobNameLabel";
            this.jobNameLabel.Size = new System.Drawing.Size(58, 13);
            this.jobNameLabel.TabIndex = 0;
            this.jobNameLabel.Text = "Job Name:";
            // 
            // counterDescriptionLabelLabel
            // 
            this.counterDescriptionLabelLabel.AutoSize = true;
            this.counterDescriptionLabelLabel.Location = new System.Drawing.Point(0, 21);
            this.counterDescriptionLabelLabel.Name = "counterDescriptionLabelLabel";
            this.counterDescriptionLabelLabel.Size = new System.Drawing.Size(52, 13);
            this.counterDescriptionLabelLabel.TabIndex = 2;
            this.counterDescriptionLabelLabel.Text = "Job Step:";
            // 
            // counterNameContentPanel
            // 
            this.counterNameContentPanel.Controls.Add(this.actionTypeLabel);
            this.counterNameContentPanel.Controls.Add(this.label2);
            this.counterNameContentPanel.Dock = System.Windows.Forms.DockStyle.Top;
            this.counterNameContentPanel.Location = new System.Drawing.Point(3, 16);
            this.counterNameContentPanel.Name = "counterNameContentPanel";
            this.counterNameContentPanel.Padding = new System.Windows.Forms.Padding(10, 0, 10, 0);
            this.counterNameContentPanel.Size = new System.Drawing.Size(541, 19);
            this.counterNameContentPanel.TabIndex = 0;
            // 
            // actionTypeLabel
            // 
            this.actionTypeLabel.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.actionTypeLabel.Location = new System.Drawing.Point(86, 4);
            this.actionTypeLabel.Name = "actionTypeLabel";
            this.actionTypeLabel.Size = new System.Drawing.Size(430, 13);
            this.actionTypeLabel.TabIndex = 1;
            this.actionTypeLabel.Text = "{0}";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(10, 3);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(67, 13);
            this.label2.TabIndex = 0;
            this.label2.Text = "Action Type:";
            // 
            // backgroundWorker
            // 
            this.backgroundWorker.WorkerReportsProgress = true;
            this.backgroundWorker.WorkerSupportsCancellation = true;
            this.backgroundWorker.DoWork += new System.ComponentModel.DoWorkEventHandler(this.backgroundWorker_DoWork);
            this.backgroundWorker.RunWorkerCompleted += new System.ComponentModel.RunWorkerCompletedEventHandler(this.backgroundWorker_RunWorkerCompleted);
            this.backgroundWorker.ProgressChanged += new System.ComponentModel.ProgressChangedEventHandler(this.backgroundWorker_ProgressChanged);
            // 
            // sqlBatchLabel
            // 
            this.sqlBatchLabel.AcceptsReturn = true;
            this.sqlBatchLabel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.sqlBatchLabel.Enabled = false;
            this.sqlBatchLabel.Location = new System.Drawing.Point(0, 29);
            this.sqlBatchLabel.Multiline = true;
            this.sqlBatchLabel.Name = "sqlBatchLabel";
            this.sqlBatchLabel.ReadOnly = true;
            this.sqlBatchLabel.ShowOverflowIndicator = true;
            this.sqlBatchLabel.Size = new System.Drawing.Size(521, 57);
            this.sqlBatchLabel.TabIndex = 9;
            this.sqlBatchLabel.Text = "{0}";
            // 
            // TestSqlActionDialog
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.doneButton;
            this.ClientSize = new System.Drawing.Size(567, 383);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.doneButton);
            this.Controls.Add(this.testButton);
            this.Name = "TestSqlActionDialog";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Test Action";
            this.Load += new System.EventHandler(this.TestSqlActionDialog_Load);
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.TestSqlActionDialog_FormClosing);
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.ultraGrid1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.serverGridDataSource)).EndInit();
            this.groupBox1.ResumeLayout(false);
            this.contentStackLayoutPanel.ResumeLayout(false);
            this.sqlBatchContentPanel.ResumeLayout(false);
            this.sqlBatchContentPanel.PerformLayout();
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.counterDefinitionContentPanel.ResumeLayout(false);
            this.counterDefinitionContentPanel.PerformLayout();
            this.counterNameContentPanel.ResumeLayout(false);
            this.counterNameContentPanel.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.sqlBatchLabel)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomButton testButton;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomButton doneButton;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomGroupBox groupBox2;
        private Infragistics.Win.UltraWinGrid.UltraGrid ultraGrid1;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomCheckBox selectAllCheckBox;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomGroupBox groupBox1;
        private Idera.SQLdm.Common.UI.Controls.StackLayoutPanel contentStackLayoutPanel;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomPanel  counterDefinitionContentPanel;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomLabel counterNameLabel;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomLabel objectNameLabel;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomLabel jobNameLabel;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomLabel counterDescriptionLabelLabel;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomPanel  sqlBatchContentPanel;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomLabel label3;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomPanel  counterNameContentPanel;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomLabel actionTypeLabel;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomLabel label2;
        private System.ComponentModel.BackgroundWorker backgroundWorker;
        private Infragistics.Win.UltraWinDataSource.UltraDataSource serverGridDataSource;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomLabel descriptionLabel;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomLabel label4;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomPanel  panel1;
        private Infragistics.Win.UltraWinEditors.UltraTextEditor sqlBatchLabel;
    }
}