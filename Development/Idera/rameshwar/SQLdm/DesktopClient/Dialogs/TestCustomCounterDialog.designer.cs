namespace Idera.SQLdm.DesktopClient.Dialogs
{
    partial class TestCustomCounterDialog
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
            Infragistics.Win.UltraWinDataSource.UltraDataColumn ultraDataColumn1 = new Infragistics.Win.UltraWinDataSource.UltraDataColumn("InstanceObject");
            Infragistics.Win.UltraWinDataSource.UltraDataColumn ultraDataColumn2 = new Infragistics.Win.UltraWinDataSource.UltraDataColumn("Test");
            Infragistics.Win.UltraWinDataSource.UltraDataColumn ultraDataColumn3 = new Infragistics.Win.UltraWinDataSource.UltraDataColumn("Instance");
            Infragistics.Win.UltraWinDataSource.UltraDataColumn ultraDataColumn4 = new Infragistics.Win.UltraWinDataSource.UltraDataColumn("Value");
            Infragistics.Win.UltraWinDataSource.UltraDataColumn ultraDataColumn5 = new Infragistics.Win.UltraWinDataSource.UltraDataColumn("ScaledValue");
            Infragistics.Win.UltraWinDataSource.UltraDataColumn ultraDataColumn6 = new Infragistics.Win.UltraWinDataSource.UltraDataColumn("Result");
            Infragistics.Win.UltraWinDataSource.UltraDataColumn ultraDataColumn7 = new Infragistics.Win.UltraWinDataSource.UltraDataColumn("Exception");
            Infragistics.Win.UltraWinDataSource.UltraDataColumn ultraDataColumn8 = new Infragistics.Win.UltraWinDataSource.UltraDataColumn("Duration");
            Infragistics.Win.Appearance appearance1 = new Infragistics.Win.Appearance();
            Infragistics.Win.UltraWinGrid.UltraGridBand ultraGridBand1 = new Infragistics.Win.UltraWinGrid.UltraGridBand("Band 0", -1);
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn1 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("InstanceObject");
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn2 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("Test");
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn3 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("Instance", -1, null, 0, Infragistics.Win.UltraWinGrid.SortIndicator.Ascending, false);
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn4 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("Value");
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn5 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("ScaledValue");
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn6 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("Result");
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn7 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("Exception");
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn8 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("Duration");
            Infragistics.Win.Appearance appearance3 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance4 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance5 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance6 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance7 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance8 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance9 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance10 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance11 = new Infragistics.Win.Appearance();
            Infragistics.Win.ValueList valueList1 = new Infragistics.Win.ValueList(84474157);
            Infragistics.Win.ValueListItem valueListItem1 = new Infragistics.Win.ValueListItem();
            Infragistics.Win.ValueListItem valueListItem2 = new Infragistics.Win.ValueListItem();
            this.testButton = new System.Windows.Forms.Button();
            this.doneButton = new System.Windows.Forms.Button();
            this.serverGridDataSource = new Infragistics.Win.UltraWinDataSource.UltraDataSource(this.components);
            this.descriptionLabel = new System.Windows.Forms.Label();
            this.headerPanel = new System.Windows.Forms.Panel();
            this.backgroundWorker = new System.ComponentModel.BackgroundWorker();
            this.sqlBatchContentPanel = new System.Windows.Forms.Panel();
            this.sqlBatchLabel = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.counterNameContentPanel = new System.Windows.Forms.Panel();
            this.metricNameLabel = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.counterDefinitionContentPanel = new System.Windows.Forms.Panel();
            this.instanceNameLabel = new System.Windows.Forms.Label();
            this.counterNameLabel = new System.Windows.Forms.Label();
            this.objectNameLabel = new System.Windows.Forms.Label();
            this.objNameLabel = new System.Windows.Forms.Label();
            this.counterDescriptionLabelLabel = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.contentStackLayoutPanel = new Idera.SQLdm.Common.UI.Controls.StackLayoutPanel();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.ultraGrid1 = new Infragistics.Win.UltraWinGrid.UltraGrid();
            this.selectAllCheckBox = new System.Windows.Forms.CheckBox();
            this.cancelButton = new System.Windows.Forms.Button();
            this.connectionProgressBar = new Idera.SQLdm.DesktopClient.Controls.InfiniteProgressBar();
            ((System.ComponentModel.ISupportInitialize)(this.serverGridDataSource)).BeginInit();
            this.headerPanel.SuspendLayout();
            this.sqlBatchContentPanel.SuspendLayout();
            this.counterNameContentPanel.SuspendLayout();
            this.counterDefinitionContentPanel.SuspendLayout();
            this.contentStackLayoutPanel.SuspendLayout();
            this.groupBox1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.ultraGrid1)).BeginInit();
            this.SuspendLayout();
            // 
            // testButton
            // 
            this.testButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.testButton.Location = new System.Drawing.Point(453, 468);
            this.testButton.Name = "testButton";
            this.testButton.Size = new System.Drawing.Size(75, 23);
            this.testButton.TabIndex = 1;
            this.testButton.Text = "Test";
            this.testButton.UseVisualStyleBackColor = true;
            this.testButton.Click += new System.EventHandler(this.testButton_Click);
            // 
            // doneButton
            // 
            this.doneButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.doneButton.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.doneButton.Location = new System.Drawing.Point(534, 468);
            this.doneButton.Name = "doneButton";
            this.doneButton.Size = new System.Drawing.Size(75, 23);
            this.doneButton.TabIndex = 2;
            this.doneButton.Text = "Done";
            this.doneButton.UseVisualStyleBackColor = true;
            // 
            // serverGridDataSource
            // 
            ultraDataColumn1.DataType = typeof(object);
            ultraDataColumn2.DataType = typeof(bool);
            ultraDataColumn2.DefaultValue = false;
            ultraDataColumn4.DataType = typeof(decimal);
            ultraDataColumn5.DataType = typeof(decimal);
            ultraDataColumn7.DataType = typeof(object);
            ultraDataColumn8.DataType = typeof(System.DateTime);
            this.serverGridDataSource.Band.Columns.AddRange(new object[] {
            ultraDataColumn1,
            ultraDataColumn2,
            ultraDataColumn3,
            ultraDataColumn4,
            ultraDataColumn5,
            ultraDataColumn6,
            ultraDataColumn7,
            ultraDataColumn8});
            // 
            // descriptionLabel
            // 
            this.descriptionLabel.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.descriptionLabel.BackColor = System.Drawing.Color.Transparent;
            this.descriptionLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.descriptionLabel.Location = new System.Drawing.Point(64, 6);
            this.descriptionLabel.Name = "descriptionLabel";
            this.descriptionLabel.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.descriptionLabel.Size = new System.Drawing.Size(554, 47);
            this.descriptionLabel.TabIndex = 0;
            this.descriptionLabel.Text = "Test the custom counter against one or more SQL Server instances monitored by SQL" +
                " diagnostic manager.";
            this.descriptionLabel.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // headerPanel
            // 
            this.headerPanel.BackColor = System.Drawing.Color.White;
            this.headerPanel.BackgroundImage = global::Idera.SQLdm.DesktopClient.Properties.Resources.AddServersManagerDialogHeader;
            this.headerPanel.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.headerPanel.Controls.Add(this.descriptionLabel);
            this.headerPanel.Dock = System.Windows.Forms.DockStyle.Top;
            this.headerPanel.Location = new System.Drawing.Point(0, 0);
            this.headerPanel.Name = "headerPanel";
            this.headerPanel.Size = new System.Drawing.Size(621, 60);
            this.headerPanel.TabIndex = 8;
            // 
            // backgroundWorker
            // 
            this.backgroundWorker.WorkerReportsProgress = true;
            this.backgroundWorker.WorkerSupportsCancellation = true;
            this.backgroundWorker.DoWork += new System.ComponentModel.DoWorkEventHandler(this.backgroundWorker_DoWork);
            this.backgroundWorker.RunWorkerCompleted += new System.ComponentModel.RunWorkerCompletedEventHandler(this.backgroundWorker_RunWorkerCompleted);
            this.backgroundWorker.ProgressChanged += new System.ComponentModel.ProgressChangedEventHandler(this.backgroundWorker_ProgressChanged);
            // 
            // sqlBatchContentPanel
            // 
            this.sqlBatchContentPanel.Controls.Add(this.sqlBatchLabel);
            this.sqlBatchContentPanel.Controls.Add(this.label3);
            this.sqlBatchContentPanel.Location = new System.Drawing.Point(15, 0);
            this.sqlBatchContentPanel.Name = "sqlBatchContentPanel";
            this.sqlBatchContentPanel.Size = new System.Drawing.Size(613, 92);
            this.sqlBatchContentPanel.TabIndex = 0;
            // 
            // sqlBatchLabel
            // 
            this.sqlBatchLabel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.sqlBatchLabel.Location = new System.Drawing.Point(0, 13);
            this.sqlBatchLabel.Multiline = true;
            this.sqlBatchLabel.Name = "sqlBatchLabel";
            this.sqlBatchLabel.ReadOnly = true;
            this.sqlBatchLabel.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.sqlBatchLabel.Size = new System.Drawing.Size(613, 79);
            this.sqlBatchLabel.TabIndex = 1;
            this.sqlBatchLabel.Text = "{0}";
            // 
            // label3
            // 
            this.label3.Dock = System.Windows.Forms.DockStyle.Top;
            this.label3.Location = new System.Drawing.Point(0, 0);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(613, 13);
            this.label3.TabIndex = 0;
            this.label3.Text = "SQL Script:";
            // 
            // counterNameContentPanel
            // 
            this.counterNameContentPanel.Controls.Add(this.metricNameLabel);
            this.counterNameContentPanel.Controls.Add(this.label2);
            this.counterNameContentPanel.Dock = System.Windows.Forms.DockStyle.Top;
            this.counterNameContentPanel.Location = new System.Drawing.Point(3, 16);
            this.counterNameContentPanel.Name = "counterNameContentPanel";
            this.counterNameContentPanel.Padding = new System.Windows.Forms.Padding(10, 0, 10, 0);
            this.counterNameContentPanel.Size = new System.Drawing.Size(593, 19);
            this.counterNameContentPanel.TabIndex = 0;
            // 
            // metricNameLabel
            // 
            this.metricNameLabel.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.metricNameLabel.Location = new System.Drawing.Point(106, 4);
            this.metricNameLabel.Name = "metricNameLabel";
            this.metricNameLabel.Size = new System.Drawing.Size(465, 13);
            this.metricNameLabel.TabIndex = 1;
            this.metricNameLabel.Text = "{0}";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(10, 3);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(78, 13);
            this.label2.TabIndex = 0;
            this.label2.Text = "Counter Name:";
            // 
            // counterDefinitionContentPanel
            // 
            this.counterDefinitionContentPanel.Controls.Add(this.instanceNameLabel);
            this.counterDefinitionContentPanel.Controls.Add(this.counterNameLabel);
            this.counterDefinitionContentPanel.Controls.Add(this.objectNameLabel);
            this.counterDefinitionContentPanel.Controls.Add(this.objNameLabel);
            this.counterDefinitionContentPanel.Controls.Add(this.counterDescriptionLabelLabel);
            this.counterDefinitionContentPanel.Controls.Add(this.label6);
            this.counterDefinitionContentPanel.Location = new System.Drawing.Point(10, 3);
            this.counterDefinitionContentPanel.Name = "counterDefinitionContentPanel";
            this.counterDefinitionContentPanel.Size = new System.Drawing.Size(573, 71);
            this.counterDefinitionContentPanel.TabIndex = 9;
            // 
            // instanceNameLabel
            // 
            this.instanceNameLabel.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.instanceNameLabel.AutoEllipsis = true;
            this.instanceNameLabel.Location = new System.Drawing.Point(96, 38);
            this.instanceNameLabel.Name = "instanceNameLabel";
            this.instanceNameLabel.Size = new System.Drawing.Size(484, 35);
            this.instanceNameLabel.TabIndex = 5;
            this.instanceNameLabel.Text = "N/A";
            // 
            // counterNameLabel
            // 
            this.counterNameLabel.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.counterNameLabel.AutoEllipsis = true;
            this.counterNameLabel.Location = new System.Drawing.Point(96, 19);
            this.counterNameLabel.Name = "counterNameLabel";
            this.counterNameLabel.Size = new System.Drawing.Size(493, 13);
            this.counterNameLabel.TabIndex = 3;
            this.counterNameLabel.Text = "{0}";
            // 
            // objectNameLabel
            // 
            this.objectNameLabel.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.objectNameLabel.AutoEllipsis = true;
            this.objectNameLabel.Location = new System.Drawing.Point(96, 0);
            this.objectNameLabel.Name = "objectNameLabel";
            this.objectNameLabel.Size = new System.Drawing.Size(493, 13);
            this.objectNameLabel.TabIndex = 1;
            this.objectNameLabel.Text = "{0}";
            // 
            // objNameLabel
            // 
            this.objNameLabel.AutoSize = true;
            this.objNameLabel.Location = new System.Drawing.Point(0, 0);
            this.objNameLabel.Name = "objNameLabel";
            this.objNameLabel.Size = new System.Drawing.Size(41, 13);
            this.objNameLabel.TabIndex = 0;
            this.objNameLabel.Text = "Object:";
            // 
            // counterDescriptionLabelLabel
            // 
            this.counterDescriptionLabelLabel.AutoSize = true;
            this.counterDescriptionLabelLabel.Location = new System.Drawing.Point(0, 19);
            this.counterDescriptionLabelLabel.Name = "counterDescriptionLabelLabel";
            this.counterDescriptionLabelLabel.Size = new System.Drawing.Size(47, 13);
            this.counterDescriptionLabelLabel.TabIndex = 2;
            this.counterDescriptionLabelLabel.Text = "Counter:";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(0, 38);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(51, 13);
            this.label6.TabIndex = 4;
            this.label6.Text = "Instance:";
            // 
            // contentStackLayoutPanel
            // 
            this.contentStackLayoutPanel.ActiveControl = this.counterDefinitionContentPanel;
            this.contentStackLayoutPanel.Controls.Add(this.counterDefinitionContentPanel);
            this.contentStackLayoutPanel.Controls.Add(this.sqlBatchContentPanel);
            this.contentStackLayoutPanel.Dock = System.Windows.Forms.DockStyle.Top;
            this.contentStackLayoutPanel.Location = new System.Drawing.Point(3, 35);
            this.contentStackLayoutPanel.Name = "contentStackLayoutPanel";
            this.contentStackLayoutPanel.Padding = new System.Windows.Forms.Padding(10, 3, 10, 0);
            this.contentStackLayoutPanel.Size = new System.Drawing.Size(593, 74);
            this.contentStackLayoutPanel.TabIndex = 11;
            // 
            // groupBox1
            // 
            this.groupBox1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBox1.Controls.Add(this.contentStackLayoutPanel);
            this.groupBox1.Controls.Add(this.counterNameContentPanel);
            this.groupBox1.Location = new System.Drawing.Point(12, 69);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(599, 114);
            this.groupBox1.TabIndex = 17;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Counter Information";
            // 
            // groupBox2
            // 
            this.groupBox2.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBox2.Controls.Add(this.ultraGrid1);
            this.groupBox2.Controls.Add(this.selectAllCheckBox);
            this.groupBox2.Location = new System.Drawing.Point(12, 189);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Padding = new System.Windows.Forms.Padding(10);
            this.groupBox2.Size = new System.Drawing.Size(599, 273);
            this.groupBox2.TabIndex = 18;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Select instance(s) to test the custom counter against:";
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
            ultraGridColumn1.Header.VisiblePosition = 0;
            ultraGridColumn1.Hidden = true;
            ultraGridColumn2.Header.VisiblePosition = 1;
            ultraGridColumn3.Header.VisiblePosition = 2;
            ultraGridColumn3.Width = 169;
            ultraGridColumn4.Format = "N2";
            ultraGridColumn4.Header.VisiblePosition = 3;
            ultraGridColumn5.Format = "N2";
            ultraGridColumn5.Header.Caption = "Scaled Value";
            ultraGridColumn5.Header.VisiblePosition = 4;
            ultraGridColumn6.Header.VisiblePosition = 6;
            ultraGridColumn7.Header.VisiblePosition = 7;
            ultraGridColumn7.Hidden = true;
            ultraGridColumn8.Format = "HH:mm:ss.FF";
            ultraGridColumn8.Header.VisiblePosition = 5;
            ultraGridBand1.Columns.AddRange(new object[] {
            ultraGridColumn1,
            ultraGridColumn2,
            ultraGridColumn3,
            ultraGridColumn4,
            ultraGridColumn5,
            ultraGridColumn6,
            ultraGridColumn7,
            ultraGridColumn8});
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
            this.ultraGrid1.Size = new System.Drawing.Size(573, 221);
            this.ultraGrid1.TabIndex = 15;
            this.ultraGrid1.InitializeLayout += new Infragistics.Win.UltraWinGrid.InitializeLayoutEventHandler(this.ultraGrid1_InitializeLayout);
            this.ultraGrid1.DoubleClickCell += new Infragistics.Win.UltraWinGrid.DoubleClickCellEventHandler(this.ultraGrid1_DoubleClickCell);
            this.ultraGrid1.MouseClick += new System.Windows.Forms.MouseEventHandler(this.ultraGrid1_MouseClick);
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
            // cancelButton
            // 
            this.cancelButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.cancelButton.Location = new System.Drawing.Point(453, 468);
            this.cancelButton.Name = "cancelButton";
            this.cancelButton.Size = new System.Drawing.Size(75, 23);
            this.cancelButton.TabIndex = 19;
            this.cancelButton.Text = "Cancel Test";
            this.cancelButton.UseVisualStyleBackColor = true;
            this.cancelButton.Click += new System.EventHandler(this.cancelButton_Click);
            // 
            // connectionProgressBar
            // 
            this.connectionProgressBar.Color1 = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(135)))), ((int)(((byte)(45)))));
            this.connectionProgressBar.Color2 = System.Drawing.Color.White;
            this.connectionProgressBar.Dock = System.Windows.Forms.DockStyle.Top;
            this.connectionProgressBar.Location = new System.Drawing.Point(0, 60);
            this.connectionProgressBar.Name = "connectionProgressBar";
            this.connectionProgressBar.Size = new System.Drawing.Size(621, 3);
            this.connectionProgressBar.Speed = 15;
            this.connectionProgressBar.Step = 5F;
            this.connectionProgressBar.TabIndex = 16;
            // 
            // TestCustomCounterDialog
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(621, 501);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.connectionProgressBar);
            this.Controls.Add(this.headerPanel);
            this.Controls.Add(this.doneButton);
            this.Controls.Add(this.testButton);
            this.Controls.Add(this.cancelButton);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.HelpButton = true;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "TestCustomCounterDialog";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Test Custom Counter";
            this.HelpButtonClicked += new System.ComponentModel.CancelEventHandler(this.TestCustomCounterDialog_HelpButtonClicked);
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.TestCustomCounterDialog_FormClosing);
            this.HelpRequested += new System.Windows.Forms.HelpEventHandler(this.TestCustomCounterDialog_HelpRequested);
            this.Load += new System.EventHandler(this.TestCustomCounterDialog_Load);
            ((System.ComponentModel.ISupportInitialize)(this.serverGridDataSource)).EndInit();
            this.headerPanel.ResumeLayout(false);
            this.sqlBatchContentPanel.ResumeLayout(false);
            this.sqlBatchContentPanel.PerformLayout();
            this.counterNameContentPanel.ResumeLayout(false);
            this.counterNameContentPanel.PerformLayout();
            this.counterDefinitionContentPanel.ResumeLayout(false);
            this.counterDefinitionContentPanel.PerformLayout();
            this.contentStackLayoutPanel.ResumeLayout(false);
            this.groupBox1.ResumeLayout(false);
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.ultraGrid1)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button testButton;
        private System.Windows.Forms.Button doneButton;
        private Infragistics.Win.UltraWinDataSource.UltraDataSource serverGridDataSource;
        private System.Windows.Forms.Label descriptionLabel;
        private System.Windows.Forms.Panel headerPanel;
        private System.ComponentModel.BackgroundWorker backgroundWorker;
        private System.Windows.Forms.Panel sqlBatchContentPanel;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox sqlBatchLabel;
        private System.Windows.Forms.Panel counterNameContentPanel;
        private System.Windows.Forms.Label metricNameLabel;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Panel counterDefinitionContentPanel;
        private System.Windows.Forms.Label instanceNameLabel;
        private System.Windows.Forms.Label counterNameLabel;
        private System.Windows.Forms.Label objectNameLabel;
        private System.Windows.Forms.Label objNameLabel;
        private System.Windows.Forms.Label counterDescriptionLabelLabel;
        private System.Windows.Forms.Label label6;
        private Idera.SQLdm.Common.UI.Controls.StackLayoutPanel contentStackLayoutPanel;
        private Idera.SQLdm.DesktopClient.Controls.InfiniteProgressBar connectionProgressBar;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.CheckBox selectAllCheckBox;
        private Infragistics.Win.UltraWinGrid.UltraGrid ultraGrid1;
        private System.Windows.Forms.Button cancelButton;
    }
}