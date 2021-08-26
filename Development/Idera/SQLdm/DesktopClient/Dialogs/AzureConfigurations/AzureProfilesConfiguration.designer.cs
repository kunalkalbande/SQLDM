
namespace Idera.SQLdm.DesktopClient.Dialogs.AzureConfigurations
{
    partial class AzureProfilesConfiguration
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
            Infragistics.Win.UltraWinGrid.UltraGridBand ultraGridBand1 = new Infragistics.Win.UltraWinGrid.UltraGridBand("IAzureApplicationProfile", -1);
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn1 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("Id");
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn2 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("Name");
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn3 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("Subscription");
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn4 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("Application");
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn5 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("Description");
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn6 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("Resources");
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn7 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("TestStatus", -1, 35772059);
            Infragistics.Win.UltraWinGrid.UltraGridBand ultraGridBand2 = new Infragistics.Win.UltraWinGrid.UltraGridBand("Resources", 0);
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn8 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("Uri");
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn9 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("Name");
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn10 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("Type");
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn11 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("Profile");
            Infragistics.Win.Appearance appearance2 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance3 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance4 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance5 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance6 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance7 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance8 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance9 = new Infragistics.Win.Appearance();
            Infragistics.Win.ValueList valueList1 = new Infragistics.Win.ValueList(35772059);
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(AzureProfilesConfiguration));
            Infragistics.Win.Appearance appearance10 = new Infragistics.Win.Appearance();
            Infragistics.Win.UltraWinGrid.UltraGridBand ultraGridBand3 = new Infragistics.Win.UltraWinGrid.UltraGridBand("IAzureProfile", -1);
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn12 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("Id");
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn13 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("SqlServerId");
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn14 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("ApplicationProfile");
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn15 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("Description");
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn16 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("azureServer", 0);
            Infragistics.Win.Appearance appearance11 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance12 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance13 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance14 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance15 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance16 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance17 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance18 = new Infragistics.Win.Appearance();
            Infragistics.Win.ValueList valueList2 = new Infragistics.Win.ValueList(35772059);
            this.btnClose = new System.Windows.Forms.Button();
            this.azureProfileWorker = new System.ComponentModel.BackgroundWorker();
            this.appProfileGrid = new Infragistics.Win.UltraWinGrid.UltraGrid();
            this.iAzureApplicationProfileBindingSource = new System.Windows.Forms.BindingSource(this.components);
            this.btnDeleteAppProfile = new System.Windows.Forms.Button();
            this.btnEditAppProfile = new System.Windows.Forms.Button();
            this.btnAddAppProfile = new System.Windows.Forms.Button();
            this.btnTestAppProfile = new System.Windows.Forms.Button();
            this.btnViewResources = new System.Windows.Forms.Button();
            this.headerPanel = new System.Windows.Forms.Panel();
            this.descriptionLabel = new System.Windows.Forms.Label();
            this.lblAzureAppProfileNoInstances = new System.Windows.Forms.Label();
            this.lblNoLinkedAzureProfile = new System.Windows.Forms.Label();
            this.buttonDeleteProfile = new System.Windows.Forms.Button();
            this.buttonEditProfile = new System.Windows.Forms.Button();
            this.buttonAddProfile = new System.Windows.Forms.Button();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.azureProfileGrid = new Infragistics.Win.UltraWinGrid.UltraGrid();
            this.iAzureProfileBindingSource = new System.Windows.Forms.BindingSource(this.components);
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.azureTestStatusLabel = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.appProfileGrid)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.iAzureApplicationProfileBindingSource)).BeginInit();
            this.headerPanel.SuspendLayout();
            this.groupBox1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.azureProfileGrid)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.iAzureProfileBindingSource)).BeginInit();
            this.groupBox2.SuspendLayout();
            this.SuspendLayout();
            // 
            // btnClose
            // 
            this.btnClose.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnClose.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnClose.Location = new System.Drawing.Point(441, 498);
            this.btnClose.Name = "btnClose";
            this.btnClose.Size = new System.Drawing.Size(75, 23);
            this.btnClose.TabIndex = 11;
            this.btnClose.Text = "Close";
            this.btnClose.UseVisualStyleBackColor = true;
            this.btnClose.Click += new System.EventHandler(this.btnClose_Click);
            // 
            // azureProfileWorker
            // 
            this.azureProfileWorker.WorkerSupportsCancellation = true;
            this.azureProfileWorker.DoWork += new System.ComponentModel.DoWorkEventHandler(this.azureProfileWorker_DoWork);
            this.azureProfileWorker.RunWorkerCompleted += new System.ComponentModel.RunWorkerCompletedEventHandler(this.azureProfileWorker_RunWorkerCompleted);
            // 
            // appProfileGrid
            // 
            this.appProfileGrid.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.appProfileGrid.DataSource = this.iAzureApplicationProfileBindingSource;
            appearance1.BackColor = System.Drawing.SystemColors.Window;
            appearance1.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(224)))), ((int)(((byte)(224)))), ((int)(((byte)(224)))));
            this.appProfileGrid.DisplayLayout.Appearance = appearance1;
            this.appProfileGrid.DisplayLayout.AutoFitStyle = Infragistics.Win.UltraWinGrid.AutoFitStyle.ResizeAllColumns;
            ultraGridColumn1.Header.VisiblePosition = 0;
            ultraGridColumn1.Hidden = true;
            ultraGridColumn1.Width = 74;
            ultraGridColumn2.Header.VisiblePosition = 2;
            ultraGridColumn2.Width = 97;
            ultraGridColumn3.Header.VisiblePosition = 3;
            ultraGridColumn3.Width = 155;
            ultraGridColumn4.Header.VisiblePosition = 4;
            ultraGridColumn4.Width = 90;
            ultraGridColumn5.Header.VisiblePosition = 5;
            ultraGridColumn5.Width = 126;
            ultraGridColumn6.Header.VisiblePosition = 6;
            ultraGridColumn7.Header.Caption = "";
            ultraGridColumn7.Header.VisiblePosition = 1;
            ultraGridColumn7.LockedWidth = true;
            ultraGridColumn7.MaxWidth = 28;
            ultraGridColumn7.MinWidth = 28;
            ultraGridColumn7.Width = 28;
            ultraGridBand1.Columns.AddRange(new object[] {
            ultraGridColumn1,
            ultraGridColumn2,
            ultraGridColumn3,
            ultraGridColumn4,
            ultraGridColumn5,
            ultraGridColumn6,
            ultraGridColumn7});
            ultraGridColumn8.Header.VisiblePosition = 0;
            ultraGridColumn8.Width = 133;
            ultraGridColumn9.Header.VisiblePosition = 1;
            ultraGridColumn9.Width = 131;
            ultraGridColumn10.Header.VisiblePosition = 2;
            ultraGridColumn10.Width = 131;
            ultraGridColumn11.Header.VisiblePosition = 3;
            ultraGridColumn11.Width = 63;
            ultraGridBand2.Columns.AddRange(new object[] {
            ultraGridColumn8,
            ultraGridColumn9,
            ultraGridColumn10,
            ultraGridColumn11});
            this.appProfileGrid.DisplayLayout.BandsSerializer.Add(ultraGridBand1);
            this.appProfileGrid.DisplayLayout.BandsSerializer.Add(ultraGridBand2);
            this.appProfileGrid.DisplayLayout.BorderStyle = Infragistics.Win.UIElementBorderStyle.Solid;
            this.appProfileGrid.DisplayLayout.CaptionVisible = Infragistics.Win.DefaultableBoolean.False;
            appearance2.BackColor = System.Drawing.SystemColors.ActiveBorder;
            appearance2.BackColor2 = System.Drawing.SystemColors.ControlDark;
            appearance2.BackGradientStyle = Infragistics.Win.GradientStyle.Vertical;
            appearance2.BorderColor = System.Drawing.SystemColors.Window;
            this.appProfileGrid.DisplayLayout.GroupByBox.Appearance = appearance2;
            appearance3.ForeColor = System.Drawing.SystemColors.GrayText;
            this.appProfileGrid.DisplayLayout.GroupByBox.BandLabelAppearance = appearance3;
            this.appProfileGrid.DisplayLayout.GroupByBox.BorderStyle = Infragistics.Win.UIElementBorderStyle.Solid;
            appearance4.BackColor = System.Drawing.SystemColors.ControlLightLight;
            appearance4.BackColor2 = System.Drawing.SystemColors.Control;
            appearance4.BackGradientStyle = Infragistics.Win.GradientStyle.Horizontal;
            appearance4.ForeColor = System.Drawing.SystemColors.GrayText;
            this.appProfileGrid.DisplayLayout.GroupByBox.PromptAppearance = appearance4;
            this.appProfileGrid.DisplayLayout.MaxColScrollRegions = 1;
            this.appProfileGrid.DisplayLayout.MaxRowScrollRegions = 1;
            this.appProfileGrid.DisplayLayout.Override.AllowAddNew = Infragistics.Win.UltraWinGrid.AllowAddNew.No;
            this.appProfileGrid.DisplayLayout.Override.AllowColSizing = Infragistics.Win.UltraWinGrid.AllowColSizing.Free;
            this.appProfileGrid.DisplayLayout.Override.AllowColSwapping = Infragistics.Win.UltraWinGrid.AllowColSwapping.NotAllowed;
            this.appProfileGrid.DisplayLayout.Override.AllowDelete = Infragistics.Win.DefaultableBoolean.False;
            this.appProfileGrid.DisplayLayout.Override.AllowRowFiltering = Infragistics.Win.DefaultableBoolean.False;
            this.appProfileGrid.DisplayLayout.Override.AllowRowSummaries = Infragistics.Win.UltraWinGrid.AllowRowSummaries.False;
            this.appProfileGrid.DisplayLayout.Override.AllowUpdate = Infragistics.Win.DefaultableBoolean.False;
            this.appProfileGrid.DisplayLayout.Override.BorderStyleCell = Infragistics.Win.UIElementBorderStyle.Solid;
            this.appProfileGrid.DisplayLayout.Override.BorderStyleRow = Infragistics.Win.UIElementBorderStyle.Solid;
            appearance5.BorderColor = System.Drawing.Color.Silver;
            appearance5.TextTrimming = Infragistics.Win.TextTrimming.EllipsisCharacter;
            this.appProfileGrid.DisplayLayout.Override.CellAppearance = appearance5;
            this.appProfileGrid.DisplayLayout.Override.CellClickAction = Infragistics.Win.UltraWinGrid.CellClickAction.RowSelect;
            this.appProfileGrid.DisplayLayout.Override.FixedHeaderIndicator = Infragistics.Win.UltraWinGrid.FixedHeaderIndicator.None;
            appearance6.BackColor = System.Drawing.SystemColors.Control;
            appearance6.BackColor2 = System.Drawing.SystemColors.ControlDark;
            appearance6.BackGradientAlignment = Infragistics.Win.GradientAlignment.Element;
            appearance6.BackGradientStyle = Infragistics.Win.GradientStyle.Horizontal;
            appearance6.BorderColor = System.Drawing.SystemColors.Window;
            this.appProfileGrid.DisplayLayout.Override.GroupByRowAppearance = appearance6;
            appearance7.TextHAlignAsString = "Left";
            this.appProfileGrid.DisplayLayout.Override.HeaderAppearance = appearance7;
            this.appProfileGrid.DisplayLayout.Override.HeaderClickAction = Infragistics.Win.UltraWinGrid.HeaderClickAction.SortMulti;
            appearance8.BackColor = System.Drawing.SystemColors.Window;
            appearance8.BorderColor = System.Drawing.Color.Silver;
            this.appProfileGrid.DisplayLayout.Override.RowAppearance = appearance8;
            this.appProfileGrid.DisplayLayout.Override.RowSelectors = Infragistics.Win.DefaultableBoolean.False;
            this.appProfileGrid.DisplayLayout.Override.RowSizing = Infragistics.Win.UltraWinGrid.RowSizing.Fixed;
            this.appProfileGrid.DisplayLayout.Override.SelectTypeCell = Infragistics.Win.UltraWinGrid.SelectType.None;
            this.appProfileGrid.DisplayLayout.Override.SelectTypeCol = Infragistics.Win.UltraWinGrid.SelectType.None;
            this.appProfileGrid.DisplayLayout.Override.SelectTypeGroupByRow = Infragistics.Win.UltraWinGrid.SelectType.None;
            this.appProfileGrid.DisplayLayout.Override.SelectTypeRow = Infragistics.Win.UltraWinGrid.SelectType.Single;
            appearance9.BackColor = System.Drawing.SystemColors.ControlLight;
            this.appProfileGrid.DisplayLayout.Override.TemplateAddRowAppearance = appearance9;
            this.appProfileGrid.DisplayLayout.ScrollBounds = Infragistics.Win.UltraWinGrid.ScrollBounds.ScrollToFill;
            this.appProfileGrid.DisplayLayout.ScrollStyle = Infragistics.Win.UltraWinGrid.ScrollStyle.Immediate;
            this.appProfileGrid.DisplayLayout.TabNavigation = Infragistics.Win.UltraWinGrid.TabNavigation.NextControl;
            valueList1.DisplayStyle = Infragistics.Win.ValueListDisplayStyle.Picture;
            valueList1.Key = "statusValueList";
            valueList1.PreferredDropDownSize = new System.Drawing.Size(0, 0);
            this.appProfileGrid.DisplayLayout.ValueLists.AddRange(new Infragistics.Win.ValueList[] {
            valueList1});
            this.appProfileGrid.DisplayLayout.ViewStyle = Infragistics.Win.UltraWinGrid.ViewStyle.SingleBand;
            this.appProfileGrid.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.appProfileGrid.Location = new System.Drawing.Point(9, 19);
            this.appProfileGrid.Name = "appProfileGrid";
            this.appProfileGrid.Size = new System.Drawing.Size(498, 134);
            this.appProfileGrid.TabIndex = 0;
            this.appProfileGrid.AfterSelectChange += new Infragistics.Win.UltraWinGrid.AfterSelectChangeEventHandler(this.appProfileGrid_AfterSelectChange);
            this.appProfileGrid.ClickCell += new Infragistics.Win.UltraWinGrid.ClickCellEventHandler(this.appProfileGrid_ClickCell);
            this.appProfileGrid.DoubleClickRow += new Infragistics.Win.UltraWinGrid.DoubleClickRowEventHandler(this.appProfileGrid_DoubleClickRow);
            // 
            // iAzureApplicationProfileBindingSource
            // 
            this.iAzureApplicationProfileBindingSource.DataSource = typeof(Idera.SQLdm.Common.Events.AzureMonitor.Interfaces.IAzureApplicationProfile);
            // 
            // btnDeleteAppProfile
            // 
            this.btnDeleteAppProfile.BackColor = System.Drawing.SystemColors.Control;
            this.btnDeleteAppProfile.Location = new System.Drawing.Point(189, 187);
            this.btnDeleteAppProfile.Name = "btnDeleteAppProfile";
            this.btnDeleteAppProfile.Size = new System.Drawing.Size(84, 23);
            this.btnDeleteAppProfile.TabIndex = 3;
            this.btnDeleteAppProfile.Text = "Delete";
            this.btnDeleteAppProfile.UseVisualStyleBackColor = true;
            this.btnDeleteAppProfile.Click += new System.EventHandler(this.btnDeleteAppProfile_Click);
            // 
            // btnEditAppProfile
            // 
            this.btnEditAppProfile.BackColor = System.Drawing.SystemColors.Control;
            this.btnEditAppProfile.Location = new System.Drawing.Point(99, 187);
            this.btnEditAppProfile.Name = "btnEditAppProfile";
            this.btnEditAppProfile.Size = new System.Drawing.Size(84, 23);
            this.btnEditAppProfile.TabIndex = 2;
            this.btnEditAppProfile.Text = "View/Edit";
            this.btnEditAppProfile.UseVisualStyleBackColor = true;
            this.btnEditAppProfile.Click += new System.EventHandler(this.btnEditAppProfile_Click);
            // 
            // btnAddAppProfile
            // 
            this.btnAddAppProfile.BackColor = System.Drawing.SystemColors.Control;
            this.btnAddAppProfile.Location = new System.Drawing.Point(9, 187);
            this.btnAddAppProfile.Name = "btnAddAppProfile";
            this.btnAddAppProfile.Size = new System.Drawing.Size(84, 23);
            this.btnAddAppProfile.TabIndex = 1;
            this.btnAddAppProfile.Text = "New";
            this.btnAddAppProfile.UseVisualStyleBackColor = true;
            this.btnAddAppProfile.Click += new System.EventHandler(this.btnAddAppProfile_Click);
            // 
            // btnTestAppProfile
            // 
            this.btnTestAppProfile.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnTestAppProfile.BackColor = System.Drawing.SystemColors.Control;
            this.btnTestAppProfile.Location = new System.Drawing.Point(389, 187);
            this.btnTestAppProfile.Name = "btnTestAppProfile";
            this.btnTestAppProfile.Size = new System.Drawing.Size(118, 23);
            this.btnTestAppProfile.TabIndex = 4;
            this.btnTestAppProfile.Text = "Test";
            this.btnTestAppProfile.UseVisualStyleBackColor = true;
            this.btnTestAppProfile.Click += new System.EventHandler(this.btnTestAppProfile_Click);
            // 
            // btnViewResources
            // 
            this.btnViewResources.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnViewResources.Location = new System.Drawing.Point(389, 179);
            this.btnViewResources.Name = "btnViewResources";
            this.btnViewResources.Size = new System.Drawing.Size(118, 23);
            this.btnViewResources.TabIndex = 9;
            this.btnViewResources.Text = "Available Resources";
            this.btnViewResources.UseVisualStyleBackColor = true;
            this.btnViewResources.Click += new System.EventHandler(this.btnViewResources_Click);
            // 
            // headerPanel
            // 
            this.headerPanel.BackColor = System.Drawing.Color.White;
            this.headerPanel.BackgroundImage = global::Idera.SQLdm.DesktopClient.Properties.Resources.CloudDialogHeader;
            this.headerPanel.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.headerPanel.Controls.Add(this.descriptionLabel);
            this.headerPanel.Dock = System.Windows.Forms.DockStyle.Top;
            this.headerPanel.Location = new System.Drawing.Point(0, 0);
            this.headerPanel.Name = "headerPanel";
            this.headerPanel.Size = new System.Drawing.Size(532, 60);
            this.headerPanel.TabIndex = 24;
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
            this.descriptionLabel.Size = new System.Drawing.Size(465, 47);
            this.descriptionLabel.TabIndex = 0;
            this.descriptionLabel.Text = resources.GetString("descriptionLabel.Text");
            this.descriptionLabel.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // lblAzureAppProfileNoInstances
            // 
            this.lblAzureAppProfileNoInstances.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.lblAzureAppProfileNoInstances.BackColor = System.Drawing.Color.White;
            this.lblAzureAppProfileNoInstances.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lblAzureAppProfileNoInstances.ForeColor = System.Drawing.SystemColors.GrayText;
            this.lblAzureAppProfileNoInstances.Location = new System.Drawing.Point(9, 19);
            this.lblAzureAppProfileNoInstances.Name = "lblAzureAppProfileNoInstances";
            this.lblAzureAppProfileNoInstances.Size = new System.Drawing.Size(498, 134);
            this.lblAzureAppProfileNoInstances.TabIndex = 14;
            this.lblAzureAppProfileNoInstances.Text = "No Azure application profile currently added. Click New to add new application pr" +
    "ofiles.";
            this.lblAzureAppProfileNoInstances.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lblNoLinkedAzureProfile
            // 
            this.lblNoLinkedAzureProfile.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.lblNoLinkedAzureProfile.BackColor = System.Drawing.Color.White;
            this.lblNoLinkedAzureProfile.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lblNoLinkedAzureProfile.ForeColor = System.Drawing.SystemColors.GrayText;
            this.lblNoLinkedAzureProfile.Location = new System.Drawing.Point(9, 16);
            this.lblNoLinkedAzureProfile.Name = "lblNoLinkedAzureProfile";
            this.lblNoLinkedAzureProfile.Size = new System.Drawing.Size(498, 152);
            this.lblNoLinkedAzureProfile.TabIndex = 28;
            this.lblNoLinkedAzureProfile.Text = "No azure profile linked with the azure instances. Click new to link Azure Applica" +
    "tion Profile to a Azure Server.";
            this.lblNoLinkedAzureProfile.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // buttonDeleteProfile
            // 
            this.buttonDeleteProfile.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.buttonDeleteProfile.BackColor = System.Drawing.SystemColors.Control;
            this.buttonDeleteProfile.Location = new System.Drawing.Point(189, 179);
            this.buttonDeleteProfile.Name = "buttonDeleteProfile";
            this.buttonDeleteProfile.Size = new System.Drawing.Size(84, 23);
            this.buttonDeleteProfile.TabIndex = 8;
            this.buttonDeleteProfile.Text = "Delete";
            this.buttonDeleteProfile.UseVisualStyleBackColor = true;
            this.buttonDeleteProfile.Click += new System.EventHandler(this.buttonDeleteProfile_Click);
            // 
            // buttonEditProfile
            // 
            this.buttonEditProfile.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.buttonEditProfile.BackColor = System.Drawing.SystemColors.Control;
            this.buttonEditProfile.Location = new System.Drawing.Point(99, 179);
            this.buttonEditProfile.Name = "buttonEditProfile";
            this.buttonEditProfile.Size = new System.Drawing.Size(84, 23);
            this.buttonEditProfile.TabIndex = 7;
            this.buttonEditProfile.Text = "View/Edit";
            this.buttonEditProfile.UseVisualStyleBackColor = true;
            this.buttonEditProfile.Click += new System.EventHandler(this.buttonEditProfile_Click);
            // 
            // buttonAddProfile
            // 
            this.buttonAddProfile.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.buttonAddProfile.BackColor = System.Drawing.SystemColors.Control;
            this.buttonAddProfile.Location = new System.Drawing.Point(9, 179);
            this.buttonAddProfile.Name = "buttonAddProfile";
            this.buttonAddProfile.Size = new System.Drawing.Size(84, 23);
            this.buttonAddProfile.TabIndex = 6;
            this.buttonAddProfile.Text = "New";
            this.buttonAddProfile.UseVisualStyleBackColor = true;
            this.buttonAddProfile.Click += new System.EventHandler(this.buttonAddProfile_Click);
            // 
            // groupBox1
            // 
            this.groupBox1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBox1.Controls.Add(this.azureProfileGrid);
            this.groupBox1.Controls.Add(this.buttonDeleteProfile);
            this.groupBox1.Controls.Add(this.btnViewResources);
            this.groupBox1.Controls.Add(this.buttonEditProfile);
            this.groupBox1.Controls.Add(this.buttonAddProfile);
            this.groupBox1.Controls.Add(this.lblNoLinkedAzureProfile);
            this.groupBox1.Location = new System.Drawing.Point(9, 284);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(513, 208);
            this.groupBox1.TabIndex = 32;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Azure Linked Profiles";
            // 
            // azureProfileGrid
            // 
            this.azureProfileGrid.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.azureProfileGrid.DataSource = this.iAzureProfileBindingSource;
            appearance10.BackColor = System.Drawing.SystemColors.Window;
            appearance10.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(224)))), ((int)(((byte)(224)))), ((int)(((byte)(224)))));
            this.azureProfileGrid.DisplayLayout.Appearance = appearance10;
            this.azureProfileGrid.DisplayLayout.AutoFitStyle = Infragistics.Win.UltraWinGrid.AutoFitStyle.ResizeAllColumns;
            ultraGridColumn12.Header.VisiblePosition = 0;
            ultraGridColumn12.Hidden = true;
            ultraGridColumn12.Width = 74;
            ultraGridColumn13.Header.VisiblePosition = 1;
            ultraGridColumn13.Hidden = true;
            ultraGridColumn13.Width = 124;
            ultraGridColumn14.Header.Caption = "Application Profile";
            ultraGridColumn14.Header.VisiblePosition = 3;
            ultraGridColumn14.Width = 203;
            ultraGridColumn15.Header.VisiblePosition = 4;
            ultraGridColumn15.Width = 223;
            ultraGridColumn16.Header.Caption = "Azure Server";
            ultraGridColumn16.Header.VisiblePosition = 2;
            ultraGridColumn16.Width = 70;
            ultraGridBand3.Columns.AddRange(new object[] {
            ultraGridColumn12,
            ultraGridColumn13,
            ultraGridColumn14,
            ultraGridColumn15,
            ultraGridColumn16});
            this.azureProfileGrid.DisplayLayout.BandsSerializer.Add(ultraGridBand3);
            this.azureProfileGrid.DisplayLayout.BorderStyle = Infragistics.Win.UIElementBorderStyle.Solid;
            this.azureProfileGrid.DisplayLayout.CaptionVisible = Infragistics.Win.DefaultableBoolean.False;
            appearance11.BackColor = System.Drawing.SystemColors.ActiveBorder;
            appearance11.BackColor2 = System.Drawing.SystemColors.ControlDark;
            appearance11.BackGradientStyle = Infragistics.Win.GradientStyle.Vertical;
            appearance11.BorderColor = System.Drawing.SystemColors.Window;
            this.azureProfileGrid.DisplayLayout.GroupByBox.Appearance = appearance11;
            appearance12.ForeColor = System.Drawing.SystemColors.GrayText;
            this.azureProfileGrid.DisplayLayout.GroupByBox.BandLabelAppearance = appearance12;
            this.azureProfileGrid.DisplayLayout.GroupByBox.BorderStyle = Infragistics.Win.UIElementBorderStyle.Solid;
            appearance13.BackColor = System.Drawing.SystemColors.ControlLightLight;
            appearance13.BackColor2 = System.Drawing.SystemColors.Control;
            appearance13.BackGradientStyle = Infragistics.Win.GradientStyle.Horizontal;
            appearance13.ForeColor = System.Drawing.SystemColors.GrayText;
            this.azureProfileGrid.DisplayLayout.GroupByBox.PromptAppearance = appearance13;
            this.azureProfileGrid.DisplayLayout.MaxColScrollRegions = 1;
            this.azureProfileGrid.DisplayLayout.MaxRowScrollRegions = 1;
            this.azureProfileGrid.DisplayLayout.Override.AllowAddNew = Infragistics.Win.UltraWinGrid.AllowAddNew.No;
            this.azureProfileGrid.DisplayLayout.Override.AllowColSizing = Infragistics.Win.UltraWinGrid.AllowColSizing.Free;
            this.azureProfileGrid.DisplayLayout.Override.AllowColSwapping = Infragistics.Win.UltraWinGrid.AllowColSwapping.NotAllowed;
            this.azureProfileGrid.DisplayLayout.Override.AllowDelete = Infragistics.Win.DefaultableBoolean.False;
            this.azureProfileGrid.DisplayLayout.Override.AllowRowFiltering = Infragistics.Win.DefaultableBoolean.False;
            this.azureProfileGrid.DisplayLayout.Override.AllowRowSummaries = Infragistics.Win.UltraWinGrid.AllowRowSummaries.False;
            this.azureProfileGrid.DisplayLayout.Override.AllowUpdate = Infragistics.Win.DefaultableBoolean.False;
            this.azureProfileGrid.DisplayLayout.Override.BorderStyleCell = Infragistics.Win.UIElementBorderStyle.Solid;
            this.azureProfileGrid.DisplayLayout.Override.BorderStyleRow = Infragistics.Win.UIElementBorderStyle.Solid;
            appearance14.BorderColor = System.Drawing.Color.Silver;
            appearance14.TextTrimming = Infragistics.Win.TextTrimming.EllipsisCharacter;
            this.azureProfileGrid.DisplayLayout.Override.CellAppearance = appearance14;
            this.azureProfileGrid.DisplayLayout.Override.CellClickAction = Infragistics.Win.UltraWinGrid.CellClickAction.RowSelect;
            this.azureProfileGrid.DisplayLayout.Override.FixedHeaderIndicator = Infragistics.Win.UltraWinGrid.FixedHeaderIndicator.None;
            appearance15.BackColor = System.Drawing.SystemColors.Control;
            appearance15.BackColor2 = System.Drawing.SystemColors.ControlDark;
            appearance15.BackGradientAlignment = Infragistics.Win.GradientAlignment.Element;
            appearance15.BackGradientStyle = Infragistics.Win.GradientStyle.Horizontal;
            appearance15.BorderColor = System.Drawing.SystemColors.Window;
            this.azureProfileGrid.DisplayLayout.Override.GroupByRowAppearance = appearance15;
            appearance16.TextHAlignAsString = "Left";
            this.azureProfileGrid.DisplayLayout.Override.HeaderAppearance = appearance16;
            this.azureProfileGrid.DisplayLayout.Override.HeaderClickAction = Infragistics.Win.UltraWinGrid.HeaderClickAction.SortMulti;
            appearance17.BackColor = System.Drawing.SystemColors.Window;
            appearance17.BorderColor = System.Drawing.Color.Silver;
            this.azureProfileGrid.DisplayLayout.Override.RowAppearance = appearance17;
            this.azureProfileGrid.DisplayLayout.Override.RowSelectors = Infragistics.Win.DefaultableBoolean.False;
            this.azureProfileGrid.DisplayLayout.Override.RowSizing = Infragistics.Win.UltraWinGrid.RowSizing.Fixed;
            this.azureProfileGrid.DisplayLayout.Override.SelectTypeCell = Infragistics.Win.UltraWinGrid.SelectType.None;
            this.azureProfileGrid.DisplayLayout.Override.SelectTypeCol = Infragistics.Win.UltraWinGrid.SelectType.None;
            this.azureProfileGrid.DisplayLayout.Override.SelectTypeGroupByRow = Infragistics.Win.UltraWinGrid.SelectType.None;
            this.azureProfileGrid.DisplayLayout.Override.SelectTypeRow = Infragistics.Win.UltraWinGrid.SelectType.Single;
            appearance18.BackColor = System.Drawing.SystemColors.ControlLight;
            this.azureProfileGrid.DisplayLayout.Override.TemplateAddRowAppearance = appearance18;
            this.azureProfileGrid.DisplayLayout.ScrollBounds = Infragistics.Win.UltraWinGrid.ScrollBounds.ScrollToFill;
            this.azureProfileGrid.DisplayLayout.ScrollStyle = Infragistics.Win.UltraWinGrid.ScrollStyle.Immediate;
            this.azureProfileGrid.DisplayLayout.TabNavigation = Infragistics.Win.UltraWinGrid.TabNavigation.NextControl;
            valueList2.DisplayStyle = Infragistics.Win.ValueListDisplayStyle.Picture;
            valueList2.Key = "statusValueList";
            valueList2.PreferredDropDownSize = new System.Drawing.Size(0, 0);
            this.azureProfileGrid.DisplayLayout.ValueLists.AddRange(new Infragistics.Win.ValueList[] {
            valueList2});
            this.azureProfileGrid.DisplayLayout.ViewStyle = Infragistics.Win.UltraWinGrid.ViewStyle.SingleBand;
            this.azureProfileGrid.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.azureProfileGrid.Location = new System.Drawing.Point(9, 16);
            this.azureProfileGrid.Name = "azureProfileGrid";
            this.azureProfileGrid.Size = new System.Drawing.Size(498, 152);
            this.azureProfileGrid.TabIndex = 5;
            this.azureProfileGrid.InitializeRow += new Infragistics.Win.UltraWinGrid.InitializeRowEventHandler(this.azureProfileGrid_InitializeRow);
            this.azureProfileGrid.AfterSelectChange += new Infragistics.Win.UltraWinGrid.AfterSelectChangeEventHandler(this.azureProfileGrid_AfterSelectChange);
            this.azureProfileGrid.ClickCell += new Infragistics.Win.UltraWinGrid.ClickCellEventHandler(this.azureProfileGrid_ClickCell);
            this.azureProfileGrid.DoubleClickRow += new Infragistics.Win.UltraWinGrid.DoubleClickRowEventHandler(this.azureProfileGrid_DoubleClickRow);
            // 
            // iAzureProfileBindingSource
            // 
            this.iAzureProfileBindingSource.DataSource = typeof(Idera.SQLdm.Common.Events.AzureMonitor.Interfaces.IAzureProfile);
            // 
            // groupBox2
            // 
            this.groupBox2.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBox2.Controls.Add(this.azureTestStatusLabel);
            this.groupBox2.Controls.Add(this.btnEditAppProfile);
            this.groupBox2.Controls.Add(this.appProfileGrid);
            this.groupBox2.Controls.Add(this.btnAddAppProfile);
            this.groupBox2.Controls.Add(this.btnTestAppProfile);
            this.groupBox2.Controls.Add(this.btnDeleteAppProfile);
            this.groupBox2.Controls.Add(this.lblAzureAppProfileNoInstances);
            this.groupBox2.Location = new System.Drawing.Point(9, 66);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(513, 216);
            this.groupBox2.TabIndex = 33;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Application Profiles";
            // 
            // azureTestStatusLabel
            // 
            this.azureTestStatusLabel.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.azureTestStatusLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold);
            this.azureTestStatusLabel.Location = new System.Drawing.Point(9, 153);
            this.azureTestStatusLabel.Margin = new System.Windows.Forms.Padding(3, 0, 0, 0);
            this.azureTestStatusLabel.Name = "azureTestStatusLabel";
            this.azureTestStatusLabel.Size = new System.Drawing.Size(501, 31);
            this.azureTestStatusLabel.TabIndex = 15;
            this.azureTestStatusLabel.Text = "Testing access for the Azure Application...";
            this.azureTestStatusLabel.TextAlign = System.Drawing.ContentAlignment.TopRight;
            // 
            // AzureProfilesConfiguration
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.btnClose;
            this.ClientSize = new System.Drawing.Size(532, 524);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.headerPanel);
            this.Controls.Add(this.btnClose);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.MinimumSize = new System.Drawing.Size(436, 501);
            this.Name = "AzureProfilesConfiguration";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Azure Profiles Configuration";
            this.Load += new System.EventHandler(this.AzureProfilesConfig_Load);
            ((System.ComponentModel.ISupportInitialize)(this.appProfileGrid)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.iAzureApplicationProfileBindingSource)).EndInit();
            this.headerPanel.ResumeLayout(false);
            this.groupBox1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.azureProfileGrid)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.iAzureProfileBindingSource)).EndInit();
            this.groupBox2.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion
        //private System.Windows.Forms.Button btnOK;
        private System.Windows.Forms.Button btnClose;
        private System.ComponentModel.BackgroundWorker azureProfileWorker;
        private Infragistics.Win.UltraWinGrid.UltraGrid appProfileGrid;
        private System.Windows.Forms.Button btnDeleteAppProfile;
        private System.Windows.Forms.Button btnEditAppProfile;
        private System.Windows.Forms.Button btnAddAppProfile;
        private System.Windows.Forms.Button btnTestAppProfile;
        private System.Windows.Forms.Button btnViewResources;
        private Controls.InfiniteProgressBar statusProgressBar;
        private System.Windows.Forms.Panel headerPanel;
        private System.Windows.Forms.Label descriptionLabel;
        private System.Windows.Forms.Label lblAzureAppProfileNoInstances;
        private System.Windows.Forms.Label lblNoLinkedAzureProfile;
        private System.Windows.Forms.Button buttonDeleteProfile;
        private System.Windows.Forms.Button buttonEditProfile;
        private System.Windows.Forms.Button buttonAddProfile;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.BindingSource iAzureApplicationProfileBindingSource;
        private System.Windows.Forms.BindingSource iAzureProfileBindingSource;
        private System.Windows.Forms.Label azureTestStatusLabel;
        private Infragistics.Win.UltraWinGrid.UltraGrid azureProfileGrid;
    }
}
