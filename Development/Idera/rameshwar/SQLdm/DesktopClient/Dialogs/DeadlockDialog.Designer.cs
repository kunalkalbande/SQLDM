namespace Idera.SQLdm.DesktopClient.Dialogs
{
    partial class DeadlockDialog
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
            Infragistics.Win.UltraWinGrid.UltraGridBand ultraGridBand1 = new Infragistics.Win.UltraWinGrid.UltraGridBand("Resource", -1);
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn1 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("Lock Type");
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn2 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("Own/Wait");
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn3 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("Mode");
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn4 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("SPID");
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn5 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("ECID");
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn6 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("Frame");
            Infragistics.Win.UltraWinGrid.UltraGridBand ultraGridBand2 = new Infragistics.Win.UltraWinGrid.UltraGridBand("Frame", 0);
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn7 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("Procedure");
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn8 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("Line");
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn9 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("Sql");
            Infragistics.Win.UltraWinDataSource.UltraDataBand ultraDataBand1 = new Infragistics.Win.UltraWinDataSource.UltraDataBand("Frame");
            Infragistics.Win.UltraWinDataSource.UltraDataColumn ultraDataColumn1 = new Infragistics.Win.UltraWinDataSource.UltraDataColumn("Procedure");
            Infragistics.Win.UltraWinDataSource.UltraDataColumn ultraDataColumn2 = new Infragistics.Win.UltraWinDataSource.UltraDataColumn("Line");
            Infragistics.Win.UltraWinDataSource.UltraDataColumn ultraDataColumn3 = new Infragistics.Win.UltraWinDataSource.UltraDataColumn("Sql");
            Infragistics.Win.UltraWinDataSource.UltraDataColumn ultraDataColumn4 = new Infragistics.Win.UltraWinDataSource.UltraDataColumn("Lock Type");
            Infragistics.Win.UltraWinDataSource.UltraDataColumn ultraDataColumn5 = new Infragistics.Win.UltraWinDataSource.UltraDataColumn("Own/Wait");
            Infragistics.Win.UltraWinDataSource.UltraDataColumn ultraDataColumn6 = new Infragistics.Win.UltraWinDataSource.UltraDataColumn("Mode");
            Infragistics.Win.UltraWinDataSource.UltraDataColumn ultraDataColumn7 = new Infragistics.Win.UltraWinDataSource.UltraDataColumn("SPID");
            Infragistics.Win.UltraWinDataSource.UltraDataColumn ultraDataColumn8 = new Infragistics.Win.UltraWinDataSource.UltraDataColumn("ECID");
            Infragistics.Win.UltraWinToolbars.ButtonTool buttonTool41 = new Infragistics.Win.UltraWinToolbars.ButtonTool("showColumnChooserButton");
            Infragistics.Win.UltraWinToolbars.ButtonTool buttonTool42 = new Infragistics.Win.UltraWinToolbars.ButtonTool("toggleGroupByBoxButton");
            Infragistics.Win.UltraWinToolbars.ButtonTool buttonTool43 = new Infragistics.Win.UltraWinToolbars.ButtonTool("sortAscendingButton");
            Infragistics.Win.UltraWinToolbars.ButtonTool buttonTool44 = new Infragistics.Win.UltraWinToolbars.ButtonTool("sortDescendingButton");
            Infragistics.Win.UltraWinToolbars.ButtonTool buttonTool45 = new Infragistics.Win.UltraWinToolbars.ButtonTool("removeThisColumnButton");
            Infragistics.Win.UltraWinToolbars.ButtonTool buttonTool46 = new Infragistics.Win.UltraWinToolbars.ButtonTool("toggleColumnTextButton");
            Infragistics.Win.UltraWinToolbars.StateButtonTool stateButtonTool7 = new Infragistics.Win.UltraWinToolbars.StateButtonTool("groupByThisColumnButton", "");
            Infragistics.Win.UltraWinToolbars.ButtonTool buttonTool47 = new Infragistics.Win.UltraWinToolbars.ButtonTool("printGridButton");
            Infragistics.Win.UltraWinToolbars.ButtonTool buttonTool48 = new Infragistics.Win.UltraWinToolbars.ButtonTool("exportGridButton");
            Infragistics.Win.UltraWinToolbars.ButtonTool buttonTool49 = new Infragistics.Win.UltraWinToolbars.ButtonTool("collapseAllGroupsButton");
            Infragistics.Win.UltraWinToolbars.ButtonTool buttonTool50 = new Infragistics.Win.UltraWinToolbars.ButtonTool("expandAllGroupsButton");
            Infragistics.Win.UltraWinToolbars.ButtonTool buttonTool51 = new Infragistics.Win.UltraWinToolbars.ButtonTool("copyToClipboardButton");
            Infragistics.Win.UltraWinToolbars.PopupMenuTool popupMenuTool4 = new Infragistics.Win.UltraWinToolbars.PopupMenuTool("columnContextMenu");
            Infragistics.Win.UltraWinToolbars.ButtonTool buttonTool52 = new Infragistics.Win.UltraWinToolbars.ButtonTool("sortAscendingButton");
            Infragistics.Win.UltraWinToolbars.ButtonTool buttonTool53 = new Infragistics.Win.UltraWinToolbars.ButtonTool("sortDescendingButton");
            Infragistics.Win.UltraWinToolbars.StateButtonTool stateButtonTool8 = new Infragistics.Win.UltraWinToolbars.StateButtonTool("groupByThisColumnButton", "");
            Infragistics.Win.UltraWinToolbars.ButtonTool buttonTool54 = new Infragistics.Win.UltraWinToolbars.ButtonTool("toggleGroupByBoxButton");
            Infragistics.Win.UltraWinToolbars.ButtonTool buttonTool55 = new Infragistics.Win.UltraWinToolbars.ButtonTool("removeThisColumnButton");
            Infragistics.Win.UltraWinToolbars.ButtonTool buttonTool56 = new Infragistics.Win.UltraWinToolbars.ButtonTool("showColumnChooserButton");
            Infragistics.Win.UltraWinToolbars.PopupMenuTool popupMenuTool5 = new Infragistics.Win.UltraWinToolbars.PopupMenuTool("gridContextMenu");
            Infragistics.Win.UltraWinToolbars.ButtonTool buttonTool2 = new Infragistics.Win.UltraWinToolbars.ButtonTool("showDetailsButton");
            Infragistics.Win.UltraWinToolbars.ButtonTool buttonTool58 = new Infragistics.Win.UltraWinToolbars.ButtonTool("collapseAllGroupsButton");
            Infragistics.Win.UltraWinToolbars.ButtonTool buttonTool59 = new Infragistics.Win.UltraWinToolbars.ButtonTool("expandAllGroupsButton");
            Infragistics.Win.UltraWinToolbars.ButtonTool buttonTool60 = new Infragistics.Win.UltraWinToolbars.ButtonTool("printGridButton");
            Infragistics.Win.UltraWinToolbars.ButtonTool buttonTool61 = new Infragistics.Win.UltraWinToolbars.ButtonTool("exportGridButton");
            Infragistics.Win.UltraWinToolbars.ButtonTool buttonTool62 = new Infragistics.Win.UltraWinToolbars.ButtonTool("copyToClipboardButton");
            Infragistics.Win.UltraWinToolbars.StateButtonTool stateButtonTool13 = new Infragistics.Win.UltraWinToolbars.StateButtonTool("exchangeEventsStateButton", "");
            Infragistics.Win.UltraWinToolbars.StateButtonTool stateButtonTool12 = new Infragistics.Win.UltraWinToolbars.StateButtonTool("threadPoolStateButton", "");
            Infragistics.Win.UltraWinToolbars.StateButtonTool stateButtonTool9 = new Infragistics.Win.UltraWinToolbars.StateButtonTool("exchangeEventsStateButton", "");
            Infragistics.Win.UltraWinToolbars.StateButtonTool stateButtonTool10 = new Infragistics.Win.UltraWinToolbars.StateButtonTool("threadPoolStateButton", "");
            Infragistics.Win.UltraWinToolbars.ButtonTool buttonTool1 = new Infragistics.Win.UltraWinToolbars.ButtonTool("showDetailsButton");
            this.panel1 = new System.Windows.Forms.Panel();
            this.okButton = new System.Windows.Forms.Button();
            this.exportButton = new System.Windows.Forms.Button();
            this.flatGrid = new Infragistics.Win.UltraWinGrid.UltraGrid();
            this.ultraDataSource1 = new Infragistics.Win.UltraWinDataSource.UltraDataSource(this.components);
            this.toolbarManager = new Idera.SQLdm.DesktopClient.Controls.ContextMenuManager(this.components);
            this._DeadlockDialog_Toolbars_Dock_Area_Left = new Infragistics.Win.UltraWinToolbars.UltraToolbarsDockArea();
            this._DeadlockDialog_Toolbars_Dock_Area_Right = new Infragistics.Win.UltraWinToolbars.UltraToolbarsDockArea();
            this._DeadlockDialog_Toolbars_Dock_Area_Top = new Infragistics.Win.UltraWinToolbars.UltraToolbarsDockArea();
            this._DeadlockDialog_Toolbars_Dock_Area_Bottom = new Infragistics.Win.UltraWinToolbars.UltraToolbarsDockArea();
            this.detailsPanel = new System.Windows.Forms.Panel();
            this.detailsTableLayoutPanel = new System.Windows.Forms.TableLayoutPanel();
            this.boundLastCommandTextBox = new System.Windows.Forms.TextBox();
            this.lastCommandLabel = new System.Windows.Forms.Label();
            this.connGroupBox = new System.Windows.Forms.GroupBox();
            this.hostLabel = new System.Windows.Forms.Label();
            this.boundSpidLabel = new System.Windows.Forms.Label();
            this.statusLabel = new System.Windows.Forms.Label();
            this.boundStatusLabel = new System.Windows.Forms.Label();
            this.spidLabel = new System.Windows.Forms.Label();
            this.boundHostLabel = new System.Windows.Forms.Label();
            this.boundDatabaseLabel = new System.Windows.Forms.Label();
            this.databaseLabel = new System.Windows.Forms.Label();
            this.boundApplicationLabel = new System.Windows.Forms.Label();
            this.applicationLabel = new System.Windows.Forms.Label();
            this.userLabel = new System.Windows.Forms.Label();
            this.boundUserLabel = new System.Windows.Forms.Label();
            this.ExecutionContextLabel = new System.Windows.Forms.Label();
            this.boundExecutionContextLabel = new System.Windows.Forms.Label();
            this.waitGroupBox = new System.Windows.Forms.GroupBox();
            this.waitTimeLabel = new System.Windows.Forms.Label();
            this.waitResourceLabel = new System.Windows.Forms.Label();
            this.waitTypeLabel = new System.Windows.Forms.Label();
            this.boundWaitTimeLabel = new System.Windows.Forms.Label();
            this.boundWaitResourceLabel = new System.Windows.Forms.Label();
            this.boundWaitTypeLabel = new System.Windows.Forms.Label();
            this.usageGroupBox = new System.Windows.Forms.GroupBox();
            this.physicalIoLabel = new System.Windows.Forms.Label();
            this.boundLastTransLabel = new System.Windows.Forms.Label();
            this.cpuLabel = new System.Windows.Forms.Label();
            this.boundXactIdLabel = new System.Windows.Forms.Label();
            this.memoryLabel = new System.Windows.Forms.Label();
            this.boundTransNameLabel = new System.Windows.Forms.Label();
            this.openTransactionsLabel = new System.Windows.Forms.Label();
            this.boundOpenTransactionsLabel = new System.Windows.Forms.Label();
            this.boundBatchCompleteLabel = new System.Windows.Forms.Label();
            this.lastActivityLabel = new System.Windows.Forms.Label();
            this.loggedInSinceLabel = new System.Windows.Forms.Label();
            this.boundBatchStartLabel = new System.Windows.Forms.Label();
            this.headerStrip1 = new Idera.SQLdm.DesktopClient.Controls.HeaderStrip();
            this.toggleDetailsPanelButton = new System.Windows.Forms.ToolStripButton();
            this.detailsHeaderStripLabel = new System.Windows.Forms.ToolStripLabel();
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.noDataLabel = new System.Windows.Forms.Label();
            this.ultraGridPrintDocument = new Infragistics.Win.UltraWinGrid.UltraGridPrintDocument(this.components);
            this.ultraGridExcelExporter = new Infragistics.Win.UltraWinGrid.ExcelExport.UltraGridExcelExporter(this.components);
            this.ultraPrintPreviewDialog = new Infragistics.Win.Printing.UltraPrintPreviewDialog(this.components);
            this.saveFileDialog = new System.Windows.Forms.SaveFileDialog();
            this.panel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.flatGrid)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.ultraDataSource1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.toolbarManager)).BeginInit();
            this.detailsPanel.SuspendLayout();
            this.detailsTableLayoutPanel.SuspendLayout();
            this.connGroupBox.SuspendLayout();
            this.waitGroupBox.SuspendLayout();
            this.usageGroupBox.SuspendLayout();
            this.headerStrip1.SuspendLayout();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            this.SuspendLayout();
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.okButton);
            this.panel1.Controls.Add(this.exportButton);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.panel1.Location = new System.Drawing.Point(0, 563);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(1000, 37);
            this.panel1.TabIndex = 0;
            // 
            // okButton
            // 
            this.okButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.okButton.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.okButton.Location = new System.Drawing.Point(913, 11);
            this.okButton.Name = "okButton";
            this.okButton.Size = new System.Drawing.Size(75, 23);
            this.okButton.TabIndex = 0;
            this.okButton.Text = "Done";
            this.okButton.UseVisualStyleBackColor = true;
            this.okButton.Click += new System.EventHandler(this.okButton_Click);
            // 
            // exportButton
            // 
            this.exportButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.exportButton.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.exportButton.Location = new System.Drawing.Point(12, 11);
            this.exportButton.Name = "exportButton";
            this.exportButton.Size = new System.Drawing.Size(83, 23);
            this.exportButton.TabIndex = 1;
            this.exportButton.Text = "Export XDL...";
            this.exportButton.UseVisualStyleBackColor = true;
            this.exportButton.Click += new System.EventHandler(this.exportButton_Click);
            // 
            // flatGrid
            // 
            this.flatGrid.DataSource = this.ultraDataSource1;
            this.flatGrid.DisplayLayout.AutoFitStyle = Infragistics.Win.UltraWinGrid.AutoFitStyle.ExtendLastColumn;
            ultraGridColumn1.AutoCompleteMode = Infragistics.Win.AutoCompleteMode.Append;
            ultraGridColumn1.Header.VisiblePosition = 0;
            ultraGridColumn2.AutoCompleteMode = Infragistics.Win.AutoCompleteMode.Append;
            ultraGridColumn2.Header.VisiblePosition = 1;
            ultraGridColumn3.AutoCompleteMode = Infragistics.Win.AutoCompleteMode.Append;
            ultraGridColumn3.Header.VisiblePosition = 2;
            ultraGridColumn4.AutoCompleteMode = Infragistics.Win.AutoCompleteMode.Append;
            ultraGridColumn4.Header.VisiblePosition = 3;
            ultraGridColumn5.AutoCompleteMode = Infragistics.Win.AutoCompleteMode.Append;
            ultraGridColumn5.Header.VisiblePosition = 4;
            ultraGridColumn6.AutoCompleteMode = Infragistics.Win.AutoCompleteMode.Append;
            ultraGridColumn6.Header.VisiblePosition = 5;
            ultraGridBand1.Columns.AddRange(new object[] {
            ultraGridColumn1,
            ultraGridColumn2,
            ultraGridColumn3,
            ultraGridColumn4,
            ultraGridColumn5,
            ultraGridColumn6});
            ultraGridBand2.CardSettings.AutoFit = true;
            ultraGridBand2.CardSettings.ShowCaption = false;
            ultraGridBand2.CardSettings.Style = Infragistics.Win.UltraWinGrid.CardStyle.VariableHeight;
            ultraGridBand2.CardSettings.Width = 748;
            ultraGridColumn7.AutoCompleteMode = Infragistics.Win.AutoCompleteMode.Append;
            ultraGridColumn7.Header.VisiblePosition = 0;
            ultraGridColumn7.SortIndicator = Infragistics.Win.UltraWinGrid.SortIndicator.Disabled;
            ultraGridColumn8.AutoCompleteMode = Infragistics.Win.AutoCompleteMode.Append;
            ultraGridColumn8.Header.VisiblePosition = 1;
            ultraGridColumn8.SortIndicator = Infragistics.Win.UltraWinGrid.SortIndicator.Disabled;
            ultraGridColumn9.AutoCompleteMode = Infragistics.Win.AutoCompleteMode.Append;
            ultraGridColumn9.Header.TextOrientation = new Infragistics.Win.TextOrientationInfo(0, Infragistics.Win.TextFlowDirection.Horizontal);
            ultraGridColumn9.Header.VisiblePosition = 2;
            ultraGridColumn9.SortIndicator = Infragistics.Win.UltraWinGrid.SortIndicator.Disabled;
            ultraGridBand2.Columns.AddRange(new object[] {
            ultraGridColumn7,
            ultraGridColumn8,
            ultraGridColumn9});
            ultraGridBand2.GroupHeadersVisible = false;
            ultraGridBand2.UseRowLayout = true;
            this.flatGrid.DisplayLayout.BandsSerializer.Add(ultraGridBand1);
            this.flatGrid.DisplayLayout.BandsSerializer.Add(ultraGridBand2);
            this.flatGrid.DisplayLayout.CaptionVisible = Infragistics.Win.DefaultableBoolean.False;
            this.flatGrid.DisplayLayout.Override.AllowAddNew = Infragistics.Win.UltraWinGrid.AllowAddNew.No;
            this.flatGrid.DisplayLayout.Override.AllowDelete = Infragistics.Win.DefaultableBoolean.False;
            this.flatGrid.DisplayLayout.Override.AllowRowFiltering = Infragistics.Win.DefaultableBoolean.False;
            this.flatGrid.DisplayLayout.Override.AllowUpdate = Infragistics.Win.DefaultableBoolean.False;
            this.flatGrid.DisplayLayout.Override.CellClickAction = Infragistics.Win.UltraWinGrid.CellClickAction.RowSelect;
            this.flatGrid.DisplayLayout.Override.RowFilterAction = Infragistics.Win.UltraWinGrid.RowFilterAction.HideFilteredOutRows;
            this.flatGrid.DisplayLayout.Override.RowSelectors = Infragistics.Win.DefaultableBoolean.False;
            this.flatGrid.DisplayLayout.Override.SelectTypeCell = Infragistics.Win.UltraWinGrid.SelectType.None;
            this.flatGrid.DisplayLayout.Override.SelectTypeCol = Infragistics.Win.UltraWinGrid.SelectType.None;
            this.flatGrid.DisplayLayout.Override.SelectTypeRow = Infragistics.Win.UltraWinGrid.SelectType.Single;
            this.flatGrid.DisplayLayout.ViewStyleBand = Infragistics.Win.UltraWinGrid.ViewStyleBand.OutlookGroupBy;
            this.flatGrid.Dock = System.Windows.Forms.DockStyle.Fill;
            this.flatGrid.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.flatGrid.Location = new System.Drawing.Point(0, 0);
            this.flatGrid.Name = "flatGrid";
            this.flatGrid.Size = new System.Drawing.Size(1000, 338);
            this.flatGrid.TabIndex = 1;
            this.flatGrid.Text = "Ca-ca";
            this.flatGrid.MouseDown += new System.Windows.Forms.MouseEventHandler(this.flatGrid_MouseDown);
            this.flatGrid.InitializeLayout += new Infragistics.Win.UltraWinGrid.InitializeLayoutEventHandler(this.flatGrid_InitializeLayout);
            this.flatGrid.AfterSelectChange += new Infragistics.Win.UltraWinGrid.AfterSelectChangeEventHandler(this.flatGrid_AfterSelectChange);
            this.flatGrid.InitializeRow += new Infragistics.Win.UltraWinGrid.InitializeRowEventHandler(this.flatGrid_InitializeRow);
            // 
            // ultraDataSource1
            // 
            ultraDataColumn2.DataType = typeof(int);
            ultraDataBand1.Columns.AddRange(new object[] {
            ultraDataColumn1,
            ultraDataColumn2,
            ultraDataColumn3});
            this.ultraDataSource1.Band.ChildBands.AddRange(new object[] {
            ultraDataBand1});
            ultraDataColumn8.DataType = typeof(int);
            this.ultraDataSource1.Band.Columns.AddRange(new object[] {
            ultraDataColumn4,
            ultraDataColumn5,
            ultraDataColumn6,
            ultraDataColumn7,
            ultraDataColumn8});
            this.ultraDataSource1.Band.Key = "Resource";
            // 
            // toolbarManager
            // 
            this.toolbarManager.DesignerFlags = 0;
            this.toolbarManager.DockWithinContainer = this;
            this.toolbarManager.DockWithinContainerBaseType = typeof(System.Windows.Forms.Form);
            this.toolbarManager.ShowFullMenusDelay = 500;
            buttonTool41.SharedProps.Caption = "Column Chooser";
            buttonTool42.SharedProps.Caption = "Group By Box";
            buttonTool43.SharedProps.Caption = "Sort Ascending";
            buttonTool44.SharedProps.Caption = "Sort Descending";
            buttonTool45.SharedProps.Caption = "Remove This Column";
            buttonTool46.SharedProps.Caption = "Show Column Text";
            stateButtonTool7.SharedProps.Caption = "Group By This Column";
            buttonTool47.SharedProps.Caption = "Print";
            buttonTool48.SharedProps.Caption = "Export To Excel";
            buttonTool49.SharedProps.Caption = "Collapse All Groups";
            buttonTool50.SharedProps.Caption = "Expand All Groups";
            buttonTool51.SharedProps.Caption = "Copy To Clipboard";
            popupMenuTool4.SharedProps.Caption = "columnContextMenu";
            buttonTool55.InstanceProps.IsFirstInGroup = true;
            popupMenuTool4.Tools.AddRange(new Infragistics.Win.UltraWinToolbars.ToolBase[] {
            buttonTool52,
            buttonTool53,
            stateButtonTool8,
            buttonTool54,
            buttonTool55,
            buttonTool56});
            popupMenuTool5.SharedProps.Caption = "gridContextMenu";
            buttonTool58.InstanceProps.IsFirstInGroup = true;
            buttonTool60.InstanceProps.IsFirstInGroup = true;
            stateButtonTool13.InstanceProps.IsFirstInGroup = true;
            stateButtonTool13.MenuDisplayStyle = Infragistics.Win.UltraWinToolbars.StateButtonMenuDisplayStyle.DisplayCheckmark;
            stateButtonTool12.MenuDisplayStyle = Infragistics.Win.UltraWinToolbars.StateButtonMenuDisplayStyle.DisplayCheckmark;
            popupMenuTool5.Tools.AddRange(new Infragistics.Win.UltraWinToolbars.ToolBase[] {
            buttonTool2,
            buttonTool58,
            buttonTool59,
            buttonTool60,
            buttonTool61,
            buttonTool62,
            stateButtonTool13,
            stateButtonTool12});
            stateButtonTool9.MenuDisplayStyle = Infragistics.Win.UltraWinToolbars.StateButtonMenuDisplayStyle.DisplayCheckmark;
            stateButtonTool9.SharedProps.Caption = "Exchange Events";
            stateButtonTool9.SharedProps.Visible = false;
            stateButtonTool10.MenuDisplayStyle = Infragistics.Win.UltraWinToolbars.StateButtonMenuDisplayStyle.DisplayCheckmark;
            stateButtonTool10.SharedProps.Caption = "Thread Pool Locks";
            stateButtonTool10.SharedProps.Visible = false;
            buttonTool1.SharedProps.Caption = "Show Details";
            this.toolbarManager.Tools.AddRange(new Infragistics.Win.UltraWinToolbars.ToolBase[] {
            buttonTool41,
            buttonTool42,
            buttonTool43,
            buttonTool44,
            buttonTool45,
            buttonTool46,
            stateButtonTool7,
            buttonTool47,
            buttonTool48,
            buttonTool49,
            buttonTool50,
            buttonTool51,
            popupMenuTool4,
            popupMenuTool5,
            stateButtonTool9,
            stateButtonTool10,
            buttonTool1});
            this.toolbarManager.ToolClick += new Infragistics.Win.UltraWinToolbars.ToolClickEventHandler(this.toolbarManager_ToolClick);
            this.toolbarManager.BeforeToolDropdown += new Infragistics.Win.UltraWinToolbars.BeforeToolDropdownEventHandler(this.toolbarManager_BeforeToolDropdown);
            // 
            // _DeadlockDialog_Toolbars_Dock_Area_Left
            // 
            this._DeadlockDialog_Toolbars_Dock_Area_Left.AccessibleRole = System.Windows.Forms.AccessibleRole.Grouping;
            this._DeadlockDialog_Toolbars_Dock_Area_Left.BackColor = System.Drawing.SystemColors.Control;
            this._DeadlockDialog_Toolbars_Dock_Area_Left.DockedPosition = Infragistics.Win.UltraWinToolbars.DockedPosition.Left;
            this._DeadlockDialog_Toolbars_Dock_Area_Left.ForeColor = System.Drawing.SystemColors.ControlText;
            this._DeadlockDialog_Toolbars_Dock_Area_Left.Location = new System.Drawing.Point(0, 0);
            this._DeadlockDialog_Toolbars_Dock_Area_Left.Name = "_DeadlockDialog_Toolbars_Dock_Area_Left";
            this._DeadlockDialog_Toolbars_Dock_Area_Left.Size = new System.Drawing.Size(0, 600);
            this._DeadlockDialog_Toolbars_Dock_Area_Left.ToolbarsManager = this.toolbarManager;
            // 
            // _DeadlockDialog_Toolbars_Dock_Area_Right
            // 
            this._DeadlockDialog_Toolbars_Dock_Area_Right.AccessibleRole = System.Windows.Forms.AccessibleRole.Grouping;
            this._DeadlockDialog_Toolbars_Dock_Area_Right.BackColor = System.Drawing.SystemColors.Control;
            this._DeadlockDialog_Toolbars_Dock_Area_Right.DockedPosition = Infragistics.Win.UltraWinToolbars.DockedPosition.Right;
            this._DeadlockDialog_Toolbars_Dock_Area_Right.ForeColor = System.Drawing.SystemColors.ControlText;
            this._DeadlockDialog_Toolbars_Dock_Area_Right.Location = new System.Drawing.Point(1000, 0);
            this._DeadlockDialog_Toolbars_Dock_Area_Right.Name = "_DeadlockDialog_Toolbars_Dock_Area_Right";
            this._DeadlockDialog_Toolbars_Dock_Area_Right.Size = new System.Drawing.Size(0, 600);
            this._DeadlockDialog_Toolbars_Dock_Area_Right.ToolbarsManager = this.toolbarManager;
            // 
            // _DeadlockDialog_Toolbars_Dock_Area_Top
            // 
            this._DeadlockDialog_Toolbars_Dock_Area_Top.AccessibleRole = System.Windows.Forms.AccessibleRole.Grouping;
            this._DeadlockDialog_Toolbars_Dock_Area_Top.BackColor = System.Drawing.SystemColors.Control;
            this._DeadlockDialog_Toolbars_Dock_Area_Top.DockedPosition = Infragistics.Win.UltraWinToolbars.DockedPosition.Top;
            this._DeadlockDialog_Toolbars_Dock_Area_Top.ForeColor = System.Drawing.SystemColors.ControlText;
            this._DeadlockDialog_Toolbars_Dock_Area_Top.Location = new System.Drawing.Point(0, 0);
            this._DeadlockDialog_Toolbars_Dock_Area_Top.Name = "_DeadlockDialog_Toolbars_Dock_Area_Top";
            this._DeadlockDialog_Toolbars_Dock_Area_Top.Size = new System.Drawing.Size(1000, 0);
            this._DeadlockDialog_Toolbars_Dock_Area_Top.ToolbarsManager = this.toolbarManager;
            // 
            // _DeadlockDialog_Toolbars_Dock_Area_Bottom
            // 
            this._DeadlockDialog_Toolbars_Dock_Area_Bottom.AccessibleRole = System.Windows.Forms.AccessibleRole.Grouping;
            this._DeadlockDialog_Toolbars_Dock_Area_Bottom.BackColor = System.Drawing.SystemColors.Control;
            this._DeadlockDialog_Toolbars_Dock_Area_Bottom.DockedPosition = Infragistics.Win.UltraWinToolbars.DockedPosition.Bottom;
            this._DeadlockDialog_Toolbars_Dock_Area_Bottom.ForeColor = System.Drawing.SystemColors.ControlText;
            this._DeadlockDialog_Toolbars_Dock_Area_Bottom.Location = new System.Drawing.Point(0, 600);
            this._DeadlockDialog_Toolbars_Dock_Area_Bottom.Name = "_DeadlockDialog_Toolbars_Dock_Area_Bottom";
            this._DeadlockDialog_Toolbars_Dock_Area_Bottom.Size = new System.Drawing.Size(1000, 0);
            this._DeadlockDialog_Toolbars_Dock_Area_Bottom.ToolbarsManager = this.toolbarManager;
            // 
            // detailsPanel
            // 
            this.detailsPanel.Controls.Add(this.detailsTableLayoutPanel);
            this.detailsPanel.Controls.Add(this.headerStrip1);
            this.detailsPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.detailsPanel.Location = new System.Drawing.Point(0, 0);
            this.detailsPanel.Name = "detailsPanel";
            this.detailsPanel.Size = new System.Drawing.Size(1000, 221);
            this.detailsPanel.TabIndex = 10;
            // 
            // detailsTableLayoutPanel
            // 
            this.detailsTableLayoutPanel.ColumnCount = 3;
            this.detailsTableLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 32.95019F));
            this.detailsTableLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 33.71647F));
            this.detailsTableLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 33.33333F));
            this.detailsTableLayoutPanel.Controls.Add(this.boundLastCommandTextBox, 1, 3);
            this.detailsTableLayoutPanel.Controls.Add(this.lastCommandLabel, 1, 2);
            this.detailsTableLayoutPanel.Controls.Add(this.connGroupBox, 0, 0);
            this.detailsTableLayoutPanel.Controls.Add(this.waitGroupBox, 2, 0);
            this.detailsTableLayoutPanel.Controls.Add(this.usageGroupBox, 1, 0);
            this.detailsTableLayoutPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.detailsTableLayoutPanel.Location = new System.Drawing.Point(0, 19);
            this.detailsTableLayoutPanel.Name = "detailsTableLayoutPanel";
            this.detailsTableLayoutPanel.RowCount = 4;
            this.detailsTableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 77F));
            this.detailsTableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 60F));
            this.detailsTableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 15F));
            this.detailsTableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.detailsTableLayoutPanel.Size = new System.Drawing.Size(1000, 202);
            this.detailsTableLayoutPanel.TabIndex = 7;
            // 
            // boundLastCommandTextBox
            // 
            this.boundLastCommandTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.boundLastCommandTextBox.BackColor = System.Drawing.Color.White;
            this.detailsTableLayoutPanel.SetColumnSpan(this.boundLastCommandTextBox, 2);
            this.boundLastCommandTextBox.Location = new System.Drawing.Point(332, 152);
            this.boundLastCommandTextBox.Margin = new System.Windows.Forms.Padding(3, 0, 3, 3);
            this.boundLastCommandTextBox.MinimumSize = new System.Drawing.Size(4, 20);
            this.boundLastCommandTextBox.Multiline = true;
            this.boundLastCommandTextBox.Name = "boundLastCommandTextBox";
            this.boundLastCommandTextBox.ReadOnly = true;
            this.boundLastCommandTextBox.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.boundLastCommandTextBox.Size = new System.Drawing.Size(665, 47);
            this.boundLastCommandTextBox.TabIndex = 19;
            // 
            // lastCommandLabel
            // 
            this.lastCommandLabel.AutoSize = true;
            this.detailsTableLayoutPanel.SetColumnSpan(this.lastCommandLabel, 2);
            this.lastCommandLabel.Location = new System.Drawing.Point(332, 137);
            this.lastCommandLabel.Name = "lastCommandLabel";
            this.lastCommandLabel.Size = new System.Drawing.Size(80, 13);
            this.lastCommandLabel.TabIndex = 17;
            this.lastCommandLabel.Text = "Last Command:";
            // 
            // connGroupBox
            // 
            this.connGroupBox.Controls.Add(this.hostLabel);
            this.connGroupBox.Controls.Add(this.boundSpidLabel);
            this.connGroupBox.Controls.Add(this.statusLabel);
            this.connGroupBox.Controls.Add(this.boundStatusLabel);
            this.connGroupBox.Controls.Add(this.spidLabel);
            this.connGroupBox.Controls.Add(this.boundHostLabel);
            this.connGroupBox.Controls.Add(this.boundDatabaseLabel);
            this.connGroupBox.Controls.Add(this.databaseLabel);
            this.connGroupBox.Controls.Add(this.boundApplicationLabel);
            this.connGroupBox.Controls.Add(this.applicationLabel);
            this.connGroupBox.Controls.Add(this.userLabel);
            this.connGroupBox.Controls.Add(this.boundUserLabel);
            this.connGroupBox.Controls.Add(this.ExecutionContextLabel);
            this.connGroupBox.Controls.Add(this.boundExecutionContextLabel);
            this.connGroupBox.Dock = System.Windows.Forms.DockStyle.Fill;
            this.connGroupBox.ForeColor = System.Drawing.Color.DimGray;
            this.connGroupBox.Location = new System.Drawing.Point(3, 3);
            this.connGroupBox.Name = "connGroupBox";
            this.detailsTableLayoutPanel.SetRowSpan(this.connGroupBox, 4);
            this.connGroupBox.Size = new System.Drawing.Size(323, 196);
            this.connGroupBox.TabIndex = 21;
            this.connGroupBox.TabStop = false;
            this.connGroupBox.Text = "Connection";
            // 
            // hostLabel
            // 
            this.hostLabel.AutoSize = true;
            this.hostLabel.ForeColor = System.Drawing.SystemColors.ControlText;
            this.hostLabel.Location = new System.Drawing.Point(3, 50);
            this.hostLabel.Name = "hostLabel";
            this.hostLabel.Size = new System.Drawing.Size(32, 13);
            this.hostLabel.TabIndex = 2;
            this.hostLabel.Text = "Host:";
            // 
            // boundSpidLabel
            // 
            this.boundSpidLabel.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.boundSpidLabel.AutoEllipsis = true;
            this.boundSpidLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.boundSpidLabel.ForeColor = System.Drawing.SystemColors.ControlText;
            this.boundSpidLabel.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.boundSpidLabel.Location = new System.Drawing.Point(95, 14);
            this.boundSpidLabel.Name = "boundSpidLabel";
            this.boundSpidLabel.Size = new System.Drawing.Size(225, 17);
            this.boundSpidLabel.TabIndex = 4;
            this.boundSpidLabel.Text = "?";
            this.boundSpidLabel.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // statusLabel
            // 
            this.statusLabel.AutoSize = true;
            this.statusLabel.ForeColor = System.Drawing.SystemColors.ControlText;
            this.statusLabel.Location = new System.Drawing.Point(3, 35);
            this.statusLabel.Name = "statusLabel";
            this.statusLabel.Size = new System.Drawing.Size(40, 13);
            this.statusLabel.TabIndex = 3;
            this.statusLabel.Text = "Status:";
            // 
            // boundStatusLabel
            // 
            this.boundStatusLabel.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.boundStatusLabel.AutoEllipsis = true;
            this.boundStatusLabel.ForeColor = System.Drawing.SystemColors.ControlText;
            this.boundStatusLabel.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.boundStatusLabel.Location = new System.Drawing.Point(95, 33);
            this.boundStatusLabel.Name = "boundStatusLabel";
            this.boundStatusLabel.Size = new System.Drawing.Size(225, 17);
            this.boundStatusLabel.TabIndex = 7;
            this.boundStatusLabel.Text = "?";
            this.boundStatusLabel.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // spidLabel
            // 
            this.spidLabel.AutoSize = true;
            this.spidLabel.ForeColor = System.Drawing.SystemColors.ControlText;
            this.spidLabel.Location = new System.Drawing.Point(3, 18);
            this.spidLabel.Name = "spidLabel";
            this.spidLabel.Size = new System.Drawing.Size(47, 13);
            this.spidLabel.TabIndex = 0;
            this.spidLabel.Text = "Session:";
            // 
            // boundHostLabel
            // 
            this.boundHostLabel.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.boundHostLabel.AutoEllipsis = true;
            this.boundHostLabel.ForeColor = System.Drawing.SystemColors.ControlText;
            this.boundHostLabel.Location = new System.Drawing.Point(95, 48);
            this.boundHostLabel.Name = "boundHostLabel";
            this.boundHostLabel.Size = new System.Drawing.Size(225, 17);
            this.boundHostLabel.TabIndex = 18;
            this.boundHostLabel.Text = "?";
            this.boundHostLabel.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // boundDatabaseLabel
            // 
            this.boundDatabaseLabel.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.boundDatabaseLabel.AutoEllipsis = true;
            this.boundDatabaseLabel.ForeColor = System.Drawing.SystemColors.ControlText;
            this.boundDatabaseLabel.Location = new System.Drawing.Point(95, 114);
            this.boundDatabaseLabel.Name = "boundDatabaseLabel";
            this.boundDatabaseLabel.Size = new System.Drawing.Size(225, 17);
            this.boundDatabaseLabel.TabIndex = 9;
            this.boundDatabaseLabel.Text = "?";
            this.boundDatabaseLabel.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // databaseLabel
            // 
            this.databaseLabel.AutoSize = true;
            this.databaseLabel.ForeColor = System.Drawing.SystemColors.ControlText;
            this.databaseLabel.Location = new System.Drawing.Point(4, 116);
            this.databaseLabel.Name = "databaseLabel";
            this.databaseLabel.Size = new System.Drawing.Size(56, 13);
            this.databaseLabel.TabIndex = 8;
            this.databaseLabel.Text = "Database:";
            // 
            // boundApplicationLabel
            // 
            this.boundApplicationLabel.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.boundApplicationLabel.AutoEllipsis = true;
            this.boundApplicationLabel.ForeColor = System.Drawing.SystemColors.ControlText;
            this.boundApplicationLabel.Location = new System.Drawing.Point(95, 97);
            this.boundApplicationLabel.Name = "boundApplicationLabel";
            this.boundApplicationLabel.Size = new System.Drawing.Size(225, 17);
            this.boundApplicationLabel.TabIndex = 21;
            this.boundApplicationLabel.Text = "?";
            this.boundApplicationLabel.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // applicationLabel
            // 
            this.applicationLabel.AutoSize = true;
            this.applicationLabel.ForeColor = System.Drawing.SystemColors.ControlText;
            this.applicationLabel.Location = new System.Drawing.Point(4, 99);
            this.applicationLabel.Name = "applicationLabel";
            this.applicationLabel.Size = new System.Drawing.Size(62, 13);
            this.applicationLabel.TabIndex = 20;
            this.applicationLabel.Text = "Application:";
            // 
            // userLabel
            // 
            this.userLabel.AutoSize = true;
            this.userLabel.ForeColor = System.Drawing.SystemColors.ControlText;
            this.userLabel.Location = new System.Drawing.Point(3, 65);
            this.userLabel.Name = "userLabel";
            this.userLabel.Size = new System.Drawing.Size(32, 13);
            this.userLabel.TabIndex = 1;
            this.userLabel.Text = "User:";
            // 
            // boundUserLabel
            // 
            this.boundUserLabel.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.boundUserLabel.AutoEllipsis = true;
            this.boundUserLabel.ForeColor = System.Drawing.SystemColors.ControlText;
            this.boundUserLabel.Location = new System.Drawing.Point(95, 63);
            this.boundUserLabel.Name = "boundUserLabel";
            this.boundUserLabel.Size = new System.Drawing.Size(225, 17);
            this.boundUserLabel.TabIndex = 5;
            this.boundUserLabel.Text = "?";
            this.boundUserLabel.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // ExecutionContextLabel
            // 
            this.ExecutionContextLabel.AutoSize = true;
            this.ExecutionContextLabel.ForeColor = System.Drawing.SystemColors.ControlText;
            this.ExecutionContextLabel.Location = new System.Drawing.Point(3, 82);
            this.ExecutionContextLabel.Name = "ExecutionContextLabel";
            this.ExecutionContextLabel.Size = new System.Drawing.Size(96, 13);
            this.ExecutionContextLabel.TabIndex = 22;
            this.ExecutionContextLabel.Text = "Execution Context:";
            // 
            // boundExecutionContextLabel
            // 
            this.boundExecutionContextLabel.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.boundExecutionContextLabel.AutoEllipsis = true;
            this.boundExecutionContextLabel.ForeColor = System.Drawing.SystemColors.ControlText;
            this.boundExecutionContextLabel.Location = new System.Drawing.Point(95, 80);
            this.boundExecutionContextLabel.Name = "boundExecutionContextLabel";
            this.boundExecutionContextLabel.Size = new System.Drawing.Size(225, 17);
            this.boundExecutionContextLabel.TabIndex = 23;
            this.boundExecutionContextLabel.Text = "?";
            this.boundExecutionContextLabel.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // waitGroupBox
            // 
            this.waitGroupBox.Controls.Add(this.waitTimeLabel);
            this.waitGroupBox.Controls.Add(this.waitResourceLabel);
            this.waitGroupBox.Controls.Add(this.waitTypeLabel);
            this.waitGroupBox.Controls.Add(this.boundWaitTimeLabel);
            this.waitGroupBox.Controls.Add(this.boundWaitResourceLabel);
            this.waitGroupBox.Controls.Add(this.boundWaitTypeLabel);
            this.waitGroupBox.Dock = System.Windows.Forms.DockStyle.Fill;
            this.waitGroupBox.ForeColor = System.Drawing.Color.DimGray;
            this.waitGroupBox.Location = new System.Drawing.Point(669, 3);
            this.waitGroupBox.Name = "waitGroupBox";
            this.detailsTableLayoutPanel.SetRowSpan(this.waitGroupBox, 2);
            this.waitGroupBox.Size = new System.Drawing.Size(328, 131);
            this.waitGroupBox.TabIndex = 22;
            this.waitGroupBox.TabStop = false;
            this.waitGroupBox.Text = "Lock Information";
            // 
            // waitTimeLabel
            // 
            this.waitTimeLabel.AutoSize = true;
            this.waitTimeLabel.ForeColor = System.Drawing.SystemColors.ControlText;
            this.waitTimeLabel.Location = new System.Drawing.Point(3, 18);
            this.waitTimeLabel.Name = "waitTimeLabel";
            this.waitTimeLabel.Size = new System.Drawing.Size(80, 13);
            this.waitTimeLabel.TabIndex = 24;
            this.waitTimeLabel.Text = "Wait Time (ms):";
            // 
            // waitResourceLabel
            // 
            this.waitResourceLabel.AutoSize = true;
            this.waitResourceLabel.ForeColor = System.Drawing.SystemColors.ControlText;
            this.waitResourceLabel.Location = new System.Drawing.Point(3, 52);
            this.waitResourceLabel.Name = "waitResourceLabel";
            this.waitResourceLabel.Size = new System.Drawing.Size(81, 13);
            this.waitResourceLabel.TabIndex = 26;
            this.waitResourceLabel.Text = "Wait Resource:";
            // 
            // waitTypeLabel
            // 
            this.waitTypeLabel.AutoSize = true;
            this.waitTypeLabel.ForeColor = System.Drawing.SystemColors.ControlText;
            this.waitTypeLabel.Location = new System.Drawing.Point(3, 35);
            this.waitTypeLabel.Name = "waitTypeLabel";
            this.waitTypeLabel.Size = new System.Drawing.Size(59, 13);
            this.waitTypeLabel.TabIndex = 25;
            this.waitTypeLabel.Text = "Wait Type:";
            // 
            // boundWaitTimeLabel
            // 
            this.boundWaitTimeLabel.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.boundWaitTimeLabel.AutoEllipsis = true;
            this.boundWaitTimeLabel.ForeColor = System.Drawing.SystemColors.ControlText;
            this.boundWaitTimeLabel.Location = new System.Drawing.Point(89, 16);
            this.boundWaitTimeLabel.Name = "boundWaitTimeLabel";
            this.boundWaitTimeLabel.Size = new System.Drawing.Size(235, 17);
            this.boundWaitTimeLabel.TabIndex = 29;
            this.boundWaitTimeLabel.Text = "?";
            this.boundWaitTimeLabel.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // boundWaitResourceLabel
            // 
            this.boundWaitResourceLabel.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.boundWaitResourceLabel.AutoEllipsis = true;
            this.boundWaitResourceLabel.ForeColor = System.Drawing.SystemColors.ControlText;
            this.boundWaitResourceLabel.Location = new System.Drawing.Point(56, 50);
            this.boundWaitResourceLabel.Name = "boundWaitResourceLabel";
            this.boundWaitResourceLabel.Size = new System.Drawing.Size(268, 17);
            this.boundWaitResourceLabel.TabIndex = 31;
            this.boundWaitResourceLabel.Text = "?";
            this.boundWaitResourceLabel.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // boundWaitTypeLabel
            // 
            this.boundWaitTypeLabel.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.boundWaitTypeLabel.AutoEllipsis = true;
            this.boundWaitTypeLabel.ForeColor = System.Drawing.SystemColors.ControlText;
            this.boundWaitTypeLabel.Location = new System.Drawing.Point(59, 33);
            this.boundWaitTypeLabel.Name = "boundWaitTypeLabel";
            this.boundWaitTypeLabel.Size = new System.Drawing.Size(265, 17);
            this.boundWaitTypeLabel.TabIndex = 30;
            this.boundWaitTypeLabel.Text = "?";
            this.boundWaitTypeLabel.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // usageGroupBox
            // 
            this.usageGroupBox.Controls.Add(this.physicalIoLabel);
            this.usageGroupBox.Controls.Add(this.boundLastTransLabel);
            this.usageGroupBox.Controls.Add(this.cpuLabel);
            this.usageGroupBox.Controls.Add(this.boundXactIdLabel);
            this.usageGroupBox.Controls.Add(this.memoryLabel);
            this.usageGroupBox.Controls.Add(this.boundTransNameLabel);
            this.usageGroupBox.Controls.Add(this.openTransactionsLabel);
            this.usageGroupBox.Controls.Add(this.boundOpenTransactionsLabel);
            this.usageGroupBox.Controls.Add(this.boundBatchCompleteLabel);
            this.usageGroupBox.Controls.Add(this.lastActivityLabel);
            this.usageGroupBox.Controls.Add(this.loggedInSinceLabel);
            this.usageGroupBox.Controls.Add(this.boundBatchStartLabel);
            this.usageGroupBox.Dock = System.Windows.Forms.DockStyle.Fill;
            this.usageGroupBox.ForeColor = System.Drawing.Color.DimGray;
            this.usageGroupBox.Location = new System.Drawing.Point(332, 3);
            this.usageGroupBox.Name = "usageGroupBox";
            this.detailsTableLayoutPanel.SetRowSpan(this.usageGroupBox, 2);
            this.usageGroupBox.Size = new System.Drawing.Size(331, 131);
            this.usageGroupBox.TabIndex = 20;
            this.usageGroupBox.TabStop = false;
            this.usageGroupBox.Text = "Usage";
            // 
            // physicalIoLabel
            // 
            this.physicalIoLabel.AutoSize = true;
            this.physicalIoLabel.ForeColor = System.Drawing.SystemColors.ControlText;
            this.physicalIoLabel.Location = new System.Drawing.Point(4, 52);
            this.physicalIoLabel.Name = "physicalIoLabel";
            this.physicalIoLabel.Size = new System.Drawing.Size(126, 13);
            this.physicalIoLabel.TabIndex = 10;
            this.physicalIoLabel.Text = "Last Transaction Started:";
            // 
            // boundLastTransLabel
            // 
            this.boundLastTransLabel.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.boundLastTransLabel.AutoEllipsis = true;
            this.boundLastTransLabel.ForeColor = System.Drawing.SystemColors.ControlText;
            this.boundLastTransLabel.Location = new System.Drawing.Point(72, 50);
            this.boundLastTransLabel.Name = "boundLastTransLabel";
            this.boundLastTransLabel.Size = new System.Drawing.Size(255, 17);
            this.boundLastTransLabel.TabIndex = 11;
            this.boundLastTransLabel.Text = "?";
            this.boundLastTransLabel.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // cpuLabel
            // 
            this.cpuLabel.AutoSize = true;
            this.cpuLabel.ForeColor = System.Drawing.SystemColors.ControlText;
            this.cpuLabel.Location = new System.Drawing.Point(4, 69);
            this.cpuLabel.Name = "cpuLabel";
            this.cpuLabel.Size = new System.Drawing.Size(78, 13);
            this.cpuLabel.TabIndex = 15;
            this.cpuLabel.Text = "Transaction Id:";
            // 
            // boundXactIdLabel
            // 
            this.boundXactIdLabel.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.boundXactIdLabel.AutoEllipsis = true;
            this.boundXactIdLabel.ForeColor = System.Drawing.SystemColors.ControlText;
            this.boundXactIdLabel.Location = new System.Drawing.Point(88, 67);
            this.boundXactIdLabel.Name = "boundXactIdLabel";
            this.boundXactIdLabel.Size = new System.Drawing.Size(239, 17);
            this.boundXactIdLabel.TabIndex = 16;
            this.boundXactIdLabel.Text = "?";
            this.boundXactIdLabel.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // memoryLabel
            // 
            this.memoryLabel.AutoSize = true;
            this.memoryLabel.ForeColor = System.Drawing.SystemColors.ControlText;
            this.memoryLabel.Location = new System.Drawing.Point(4, 86);
            this.memoryLabel.Name = "memoryLabel";
            this.memoryLabel.Size = new System.Drawing.Size(97, 13);
            this.memoryLabel.TabIndex = 12;
            this.memoryLabel.Text = "Transaction Name:";
            // 
            // boundTransNameLabel
            // 
            this.boundTransNameLabel.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.boundTransNameLabel.AutoEllipsis = true;
            this.boundTransNameLabel.ForeColor = System.Drawing.SystemColors.ControlText;
            this.boundTransNameLabel.Location = new System.Drawing.Point(72, 84);
            this.boundTransNameLabel.Name = "boundTransNameLabel";
            this.boundTransNameLabel.Size = new System.Drawing.Size(255, 17);
            this.boundTransNameLabel.TabIndex = 13;
            this.boundTransNameLabel.Text = "?";
            this.boundTransNameLabel.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // openTransactionsLabel
            // 
            this.openTransactionsLabel.AutoSize = true;
            this.openTransactionsLabel.ForeColor = System.Drawing.SystemColors.ControlText;
            this.openTransactionsLabel.Location = new System.Drawing.Point(4, 103);
            this.openTransactionsLabel.Name = "openTransactionsLabel";
            this.openTransactionsLabel.Size = new System.Drawing.Size(100, 13);
            this.openTransactionsLabel.TabIndex = 36;
            this.openTransactionsLabel.Text = "Open Transactions:";
            // 
            // boundOpenTransactionsLabel
            // 
            this.boundOpenTransactionsLabel.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.boundOpenTransactionsLabel.AutoEllipsis = true;
            this.boundOpenTransactionsLabel.ForeColor = System.Drawing.SystemColors.ControlText;
            this.boundOpenTransactionsLabel.Location = new System.Drawing.Point(100, 101);
            this.boundOpenTransactionsLabel.Name = "boundOpenTransactionsLabel";
            this.boundOpenTransactionsLabel.Size = new System.Drawing.Size(227, 17);
            this.boundOpenTransactionsLabel.TabIndex = 37;
            this.boundOpenTransactionsLabel.Text = "?";
            this.boundOpenTransactionsLabel.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // boundBatchCompleteLabel
            // 
            this.boundBatchCompleteLabel.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.boundBatchCompleteLabel.AutoEllipsis = true;
            this.boundBatchCompleteLabel.ForeColor = System.Drawing.SystemColors.ControlText;
            this.boundBatchCompleteLabel.Location = new System.Drawing.Point(124, 33);
            this.boundBatchCompleteLabel.Name = "boundBatchCompleteLabel";
            this.boundBatchCompleteLabel.Size = new System.Drawing.Size(203, 17);
            this.boundBatchCompleteLabel.TabIndex = 42;
            this.boundBatchCompleteLabel.Text = "?";
            this.boundBatchCompleteLabel.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // lastActivityLabel
            // 
            this.lastActivityLabel.AutoSize = true;
            this.lastActivityLabel.ForeColor = System.Drawing.SystemColors.ControlText;
            this.lastActivityLabel.Location = new System.Drawing.Point(4, 35);
            this.lastActivityLabel.Name = "lastActivityLabel";
            this.lastActivityLabel.Size = new System.Drawing.Size(114, 13);
            this.lastActivityLabel.TabIndex = 43;
            this.lastActivityLabel.Text = "Last Batch Completed:";
            // 
            // loggedInSinceLabel
            // 
            this.loggedInSinceLabel.AutoSize = true;
            this.loggedInSinceLabel.ForeColor = System.Drawing.SystemColors.ControlText;
            this.loggedInSinceLabel.Location = new System.Drawing.Point(4, 18);
            this.loggedInSinceLabel.Name = "loggedInSinceLabel";
            this.loggedInSinceLabel.Size = new System.Drawing.Size(98, 13);
            this.loggedInSinceLabel.TabIndex = 40;
            this.loggedInSinceLabel.Text = "Last Batch Started:";
            // 
            // boundBatchStartLabel
            // 
            this.boundBatchStartLabel.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.boundBatchStartLabel.AutoEllipsis = true;
            this.boundBatchStartLabel.ForeColor = System.Drawing.SystemColors.ControlText;
            this.boundBatchStartLabel.Location = new System.Drawing.Point(72, 16);
            this.boundBatchStartLabel.Name = "boundBatchStartLabel";
            this.boundBatchStartLabel.Size = new System.Drawing.Size(255, 17);
            this.boundBatchStartLabel.TabIndex = 41;
            this.boundBatchStartLabel.Text = "?";
            this.boundBatchStartLabel.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // headerStrip1
            // 
            this.headerStrip1.AutoSize = false;
            this.headerStrip1.Font = new System.Drawing.Font("Arial", 12F, System.Drawing.FontStyle.Bold);
            this.headerStrip1.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(21)))), ((int)(((byte)(66)))), ((int)(((byte)(139)))));
            this.headerStrip1.GripStyle = System.Windows.Forms.ToolStripGripStyle.Hidden;
            this.headerStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toggleDetailsPanelButton,
            this.detailsHeaderStripLabel});
            this.headerStrip1.Location = new System.Drawing.Point(0, 0);
            this.headerStrip1.Name = "headerStrip1";
            this.headerStrip1.Padding = new System.Windows.Forms.Padding(0, 2, 0, 0);
            this.headerStrip1.Size = new System.Drawing.Size(1000, 19);
            this.headerStrip1.TabIndex = 1;
            // 
            // toggleDetailsPanelButton
            // 
            this.toggleDetailsPanelButton.Alignment = System.Windows.Forms.ToolStripItemAlignment.Right;
            this.toggleDetailsPanelButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.toggleDetailsPanelButton.Image = global::Idera.SQLdm.DesktopClient.Properties.Resources.Hide;
            this.toggleDetailsPanelButton.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None;
            this.toggleDetailsPanelButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toggleDetailsPanelButton.Name = "toggleDetailsPanelButton";
            this.toggleDetailsPanelButton.Size = new System.Drawing.Size(23, 14);
            this.toggleDetailsPanelButton.Click += new System.EventHandler(this.toggleDetailsPanelButton_Click);
            // 
            // detailsHeaderStripLabel
            // 
            this.detailsHeaderStripLabel.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.detailsHeaderStripLabel.ForeColor = System.Drawing.Color.Black;
            this.detailsHeaderStripLabel.Name = "detailsHeaderStripLabel";
            this.detailsHeaderStripLabel.Size = new System.Drawing.Size(93, 14);
            this.detailsHeaderStripLabel.Text = "Process Details";
            // 
            // splitContainer1
            // 
            this.splitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer1.Location = new System.Drawing.Point(0, 0);
            this.splitContainer1.Name = "splitContainer1";
            this.splitContainer1.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // splitContainer1.Panel1
            // 
            this.splitContainer1.Panel1.Controls.Add(this.flatGrid);
            this.splitContainer1.Panel1.Controls.Add(this.noDataLabel);
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.Controls.Add(this.detailsPanel);
            this.splitContainer1.Size = new System.Drawing.Size(1000, 563);
            this.splitContainer1.SplitterDistance = 338;
            this.splitContainer1.TabIndex = 19;
            this.splitContainer1.MouseDown += new System.Windows.Forms.MouseEventHandler(this.splitContainer_MouseDown);
            this.splitContainer1.MouseUp += new System.Windows.Forms.MouseEventHandler(this.splitContainer_MouseUp);
            // 
            // noDataLabel
            // 
            this.noDataLabel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.noDataLabel.Location = new System.Drawing.Point(0, 0);
            this.noDataLabel.Name = "noDataLabel";
            this.noDataLabel.Size = new System.Drawing.Size(1000, 338);
            this.noDataLabel.TabIndex = 2;
            this.noDataLabel.Text = "No data";
            this.noDataLabel.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // ultraGridPrintDocument
            // 
            this.ultraGridPrintDocument.DocumentName = "Deadlocks";
            this.ultraGridPrintDocument.Grid = this.flatGrid;
            // 
            // ultraPrintPreviewDialog
            // 
            this.ultraPrintPreviewDialog.Document = this.ultraGridPrintDocument;
            this.ultraPrintPreviewDialog.Name = "ultraPrintPreviewDialog";
            // 
            // DeadlockDialog
            // 
            this.AcceptButton = this.okButton;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.okButton;
            this.ClientSize = new System.Drawing.Size(1000, 600);
            this.Controls.Add(this.splitContainer1);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this._DeadlockDialog_Toolbars_Dock_Area_Left);
            this.Controls.Add(this._DeadlockDialog_Toolbars_Dock_Area_Right);
            this.Controls.Add(this._DeadlockDialog_Toolbars_Dock_Area_Top);
            this.Controls.Add(this._DeadlockDialog_Toolbars_Dock_Area_Bottom);
            this.HelpButton = true;
            this.Name = "DeadlockDialog";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "DeadlockDialog";
            this.Load += new System.EventHandler(this.DeadlockDialog_Load);
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.DeadlockDialog_FormClosing);
            this.panel1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.flatGrid)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.ultraDataSource1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.toolbarManager)).EndInit();
            this.detailsPanel.ResumeLayout(false);
            this.detailsTableLayoutPanel.ResumeLayout(false);
            this.detailsTableLayoutPanel.PerformLayout();
            this.connGroupBox.ResumeLayout(false);
            this.connGroupBox.PerformLayout();
            this.waitGroupBox.ResumeLayout(false);
            this.waitGroupBox.PerformLayout();
            this.usageGroupBox.ResumeLayout(false);
            this.usageGroupBox.PerformLayout();
            this.headerStrip1.ResumeLayout(false);
            this.headerStrip1.PerformLayout();
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel2.ResumeLayout(false);
            this.splitContainer1.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Button okButton;
        private Infragistics.Win.UltraWinGrid.UltraGrid flatGrid;
        private Infragistics.Win.UltraWinDataSource.UltraDataSource ultraDataSource1;
        private Idera.SQLdm.DesktopClient.Controls.ContextMenuManager toolbarManager;
        private Infragistics.Win.UltraWinToolbars.UltraToolbarsDockArea _DeadlockDialog_Toolbars_Dock_Area_Left;
        private Infragistics.Win.UltraWinToolbars.UltraToolbarsDockArea _DeadlockDialog_Toolbars_Dock_Area_Right;
        private Infragistics.Win.UltraWinToolbars.UltraToolbarsDockArea _DeadlockDialog_Toolbars_Dock_Area_Top;
        private Infragistics.Win.UltraWinToolbars.UltraToolbarsDockArea _DeadlockDialog_Toolbars_Dock_Area_Bottom;
        private System.Windows.Forms.Button exportButton;
        private System.Windows.Forms.Panel detailsPanel;
        private Idera.SQLdm.DesktopClient.Controls.HeaderStrip headerStrip1;
        private System.Windows.Forms.ToolStripButton toggleDetailsPanelButton;
        private System.Windows.Forms.ToolStripLabel detailsHeaderStripLabel;
        private System.Windows.Forms.TableLayoutPanel detailsTableLayoutPanel;
        private System.Windows.Forms.TextBox boundLastCommandTextBox;
        private System.Windows.Forms.Label lastCommandLabel;
        private System.Windows.Forms.GroupBox connGroupBox;
        private System.Windows.Forms.Label hostLabel;
        private System.Windows.Forms.Label boundSpidLabel;
        private System.Windows.Forms.Label statusLabel;
        private System.Windows.Forms.Label boundStatusLabel;
        private System.Windows.Forms.Label spidLabel;
        private System.Windows.Forms.Label boundHostLabel;
        private System.Windows.Forms.Label boundDatabaseLabel;
        private System.Windows.Forms.Label databaseLabel;
        private System.Windows.Forms.Label boundApplicationLabel;
        private System.Windows.Forms.Label applicationLabel;
        private System.Windows.Forms.Label userLabel;
        private System.Windows.Forms.Label boundUserLabel;
        private System.Windows.Forms.Label ExecutionContextLabel;
        private System.Windows.Forms.Label boundExecutionContextLabel;
        private System.Windows.Forms.GroupBox waitGroupBox;
        private System.Windows.Forms.Label waitTimeLabel;
        private System.Windows.Forms.Label waitResourceLabel;
        private System.Windows.Forms.Label waitTypeLabel;
        private System.Windows.Forms.Label boundWaitTimeLabel;
        private System.Windows.Forms.Label boundWaitResourceLabel;
        private System.Windows.Forms.Label boundWaitTypeLabel;
        private System.Windows.Forms.GroupBox usageGroupBox;
        private System.Windows.Forms.Label physicalIoLabel;
        private System.Windows.Forms.Label boundLastTransLabel;
        private System.Windows.Forms.Label cpuLabel;
        private System.Windows.Forms.Label boundXactIdLabel;
        private System.Windows.Forms.Label memoryLabel;
        private System.Windows.Forms.Label boundTransNameLabel;
        private System.Windows.Forms.Label openTransactionsLabel;
        private System.Windows.Forms.Label boundOpenTransactionsLabel;
        private System.Windows.Forms.Label boundBatchCompleteLabel;
        private System.Windows.Forms.Label lastActivityLabel;
        private System.Windows.Forms.Label loggedInSinceLabel;
        private System.Windows.Forms.Label boundBatchStartLabel;
        private System.Windows.Forms.SplitContainer splitContainer1;
        private Infragistics.Win.UltraWinGrid.UltraGridPrintDocument ultraGridPrintDocument;
        private Infragistics.Win.UltraWinGrid.ExcelExport.UltraGridExcelExporter ultraGridExcelExporter;
        private Infragistics.Win.Printing.UltraPrintPreviewDialog ultraPrintPreviewDialog;
        private System.Windows.Forms.SaveFileDialog saveFileDialog;
        private System.Windows.Forms.Label noDataLabel;
    }
}