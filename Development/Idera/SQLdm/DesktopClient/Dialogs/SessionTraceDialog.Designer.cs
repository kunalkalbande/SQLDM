namespace Idera.SQLdm.DesktopClient.Dialogs
{
    partial class SessionTraceDialog
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
            Infragistics.Win.UltraWinDataSource.UltraDataColumn ultraDataColumn1 = new Infragistics.Win.UltraWinDataSource.UltraDataColumn("Time");
            Infragistics.Win.UltraWinDataSource.UltraDataColumn ultraDataColumn2 = new Infragistics.Win.UltraWinDataSource.UltraDataColumn("Status");
            Infragistics.Win.UltraWinDataSource.UltraDataColumn ultraDataColumn3 = new Infragistics.Win.UltraWinDataSource.UltraDataColumn("Duration");
            Infragistics.Win.UltraWinDataSource.UltraDataColumn ultraDataColumn4 = new Infragistics.Win.UltraWinDataSource.UltraDataColumn("Cpu Time");
            Infragistics.Win.UltraWinDataSource.UltraDataColumn ultraDataColumn5 = new Infragistics.Win.UltraWinDataSource.UltraDataColumn("Reads");
            Infragistics.Win.UltraWinDataSource.UltraDataColumn ultraDataColumn6 = new Infragistics.Win.UltraWinDataSource.UltraDataColumn("Writes");
            Infragistics.Win.UltraWinDataSource.UltraDataColumn ultraDataColumn7 = new Infragistics.Win.UltraWinDataSource.UltraDataColumn("Command");
            Infragistics.Win.UltraWinDataSource.UltraDataColumn ultraDataColumn8 = new Infragistics.Win.UltraWinDataSource.UltraDataColumn("SequenceNumber");
            Infragistics.Win.UltraWinStatusBar.UltraStatusPanel ultraStatusPanel1 = new Infragistics.Win.UltraWinStatusBar.UltraStatusPanel();
            Infragistics.Win.UltraWinStatusBar.UltraStatusPanel ultraStatusPanel2 = new Infragistics.Win.UltraWinStatusBar.UltraStatusPanel();
            Infragistics.Win.UltraWinStatusBar.UltraStatusPanel ultraStatusPanel3 = new Infragistics.Win.UltraWinStatusBar.UltraStatusPanel();
            Infragistics.Win.UltraWinStatusBar.UltraStatusPanel ultraStatusPanel4 = new Infragistics.Win.UltraWinStatusBar.UltraStatusPanel();
            Infragistics.Win.Appearance appearance1 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance2 = new Infragistics.Win.Appearance();
            Infragistics.Win.UltraWinGrid.UltraGridBand ultraGridBand1 = new Infragistics.Win.UltraWinGrid.UltraGridBand("Band 0", -1);
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn1 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("Time");
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn2 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("Status");
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn3 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("Duration");
            Infragistics.Win.Appearance appearance3 = new Infragistics.Win.Appearance();
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn4 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("Cpu Time");
            Infragistics.Win.Appearance appearance4 = new Infragistics.Win.Appearance();
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn5 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("Reads");
            Infragistics.Win.Appearance appearance5 = new Infragistics.Win.Appearance();
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn6 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("Writes");
            Infragistics.Win.Appearance appearance6 = new Infragistics.Win.Appearance();
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn7 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("Command");
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn8 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("SequenceNumber", -1, null, 0, Infragistics.Win.UltraWinGrid.SortIndicator.Ascending, false);
            Infragistics.Win.Appearance appearance7 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance8 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance9 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance10 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance11 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance12 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance13 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance14 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance15 = new Infragistics.Win.Appearance();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(SessionTraceDialog));
            Infragistics.Win.Appearance appearance16 = new Infragistics.Win.Appearance();
            Infragistics.Win.UltraWinToolbars.PopupMenuTool popupMenuTool1 = new Infragistics.Win.UltraWinToolbars.PopupMenuTool("columnContextMenu");
            Infragistics.Win.UltraWinToolbars.ButtonTool buttonTool1 = new Controls.CustomControls.CustomButtonTool("sortAscendingButton");
            Infragistics.Win.UltraWinToolbars.ButtonTool buttonTool2 = new Controls.CustomControls.CustomButtonTool("sortDescendingButton");
            Infragistics.Win.UltraWinToolbars.StateButtonTool stateButtonTool1 = new Infragistics.Win.UltraWinToolbars.StateButtonTool("groupByThisColumnButton", "");
            Infragistics.Win.UltraWinToolbars.ButtonTool buttonTool3 = new Controls.CustomControls.CustomButtonTool("toggleGroupByBoxButton");
            Infragistics.Win.UltraWinToolbars.ButtonTool buttonTool4 = new Controls.CustomControls.CustomButtonTool("removeThisColumnButton");
            Infragistics.Win.UltraWinToolbars.ButtonTool buttonTool5 = new Controls.CustomControls.CustomButtonTool("showColumnChooserButton");
            Infragistics.Win.UltraWinToolbars.ButtonTool buttonTool6 = new Controls.CustomControls.CustomButtonTool("showColumnChooserButton");
            Infragistics.Win.Appearance appearance17 = new Infragistics.Win.Appearance();
            Infragistics.Win.UltraWinToolbars.ButtonTool buttonTool7 = new Controls.CustomControls.CustomButtonTool("toggleGroupByBoxButton");
            Infragistics.Win.Appearance appearance18 = new Infragistics.Win.Appearance();
            Infragistics.Win.UltraWinToolbars.ButtonTool buttonTool8 = new Controls.CustomControls.CustomButtonTool("sortAscendingButton");
            Infragistics.Win.Appearance appearance19 = new Infragistics.Win.Appearance();
            Infragistics.Win.UltraWinToolbars.ButtonTool buttonTool9 = new Controls.CustomControls.CustomButtonTool("sortDescendingButton");
            Infragistics.Win.Appearance appearance20 = new Infragistics.Win.Appearance();
            Infragistics.Win.UltraWinToolbars.ButtonTool buttonTool10 = new Controls.CustomControls.CustomButtonTool("removeThisColumnButton");
            Infragistics.Win.UltraWinToolbars.StateButtonTool stateButtonTool2 = new Infragistics.Win.UltraWinToolbars.StateButtonTool("groupByThisColumnButton", "");
            Infragistics.Win.UltraWinToolbars.PopupMenuTool popupMenuTool2 = new Infragistics.Win.UltraWinToolbars.PopupMenuTool("gridContextMenu");
            Infragistics.Win.UltraWinToolbars.ButtonTool buttonTool11 = new Controls.CustomControls.CustomButtonTool("printGridButton");
            Infragistics.Win.UltraWinToolbars.ButtonTool buttonTool12 = new Controls.CustomControls.CustomButtonTool("exportGridButton");
            Infragistics.Win.UltraWinToolbars.ButtonTool buttonTool13 = new Controls.CustomControls.CustomButtonTool("printGridButton");
            Infragistics.Win.Appearance appearance21 = new Infragistics.Win.Appearance();
            Infragistics.Win.UltraWinToolbars.ButtonTool buttonTool14 = new Controls.CustomControls.CustomButtonTool("exportGridButton");
            Infragistics.Win.Appearance appearance22 = new Infragistics.Win.Appearance();
            this.traceGridDataSource = new Infragistics.Win.UltraWinDataSource.UltraDataSource(this.components);
            this.automatedRefreshTimer = new System.Windows.Forms.Timer(this.components);
            this.statusBar = new Infragistics.Win.UltraWinStatusBar.UltraStatusBar();
            this.ultraGridPrintDocument = new Infragistics.Win.UltraWinGrid.UltraGridPrintDocument(this.components);
            this.traceGrid = new Infragistics.Win.UltraWinGrid.UltraGrid();
            this.ultraPrintPreviewDialog = new Infragistics.Win.Printing.UltraPrintPreviewDialog(this.components);
            this.saveFileDialog = new System.Windows.Forms.SaveFileDialog();
            this.toolbarsManager = new Idera.SQLdm.DesktopClient.Controls.ContextMenuManager(this.components);
            this.ultraGridExcelExporter = new Infragistics.Win.UltraWinGrid.ExcelExport.UltraGridExcelExporter(this.components);
            this.autoRefreshSpinner = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomNumericUpDown();
            this.contentPanel = new Idera.SQLdm.DesktopClient.Controls.GradientPanel();
            this.durationSpinner = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomNumericUpDown();
            this.lastCommandTextBox = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomTextBox();
            this.optionsTextBox = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomTextBox();
            this.lastCommandLabel = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomLabel();
            this.statusPulseHeaderPanel = new Idera.SQLdm.DesktopClient.Controls.PulseHeaderPanel();
            this.statusValueLabel = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomLabel();
            this.durationLabel = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomLabel();
            this.gridLabel = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomLabel();
            this.summaryGroupBox = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomGroupBox();
            this.commandTextBox = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomTextBox();
            this.isolationLevelTextBox = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomTextBox();
            this.deadlockPriorityTextBox = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomTextBox();
            this.languageTextBox = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomTextBox();
            this.lockWaitTextBox = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomTextBox();
            this.nestingLevelTextBox = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomTextBox();
            this.textSizeTextBox = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomTextBox();
            this.lineNumberTextBox = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomTextBox();
            this.lastErrorNumberTextBox = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomTextBox();
            this.cursorSetRowsTextBox = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomTextBox();
            this.cursorFetchStatusTextBox = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomTextBox();
            this.openTransactionsTextBox = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomTextBox();
            this.writesTextBox = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomTextBox();
            this.readsTextBox = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomTextBox();
            this.rowCountTextBox = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomTextBox();
            this.cpuTimeTextBox = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomTextBox();
            this.cpuTimeLabel = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomLabel();
            this.rowCountLabel = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomLabel();
            this.textSizeLabel = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomLabel();
            this.readsLabel = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomLabel();
            this.lastErrorLabel = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomLabel();
            this.openTransactionslabel = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomLabel();
            this.cursorFetchStatusLabel = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomLabel();
            this.cursorSetRowsLabel = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomLabel();
            this.commandLabel = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomLabel();
            this.languagelabel = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomLabel();
            this.writesLabel = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomLabel();
            this.isoLevelLabel = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomLabel();
            this.lineNumberLabel = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomLabel();
            this.nestingLevelLabel = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomLabel();
            this.lockWaitLabel = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomLabel();
            this.deadlockPriorityLabel = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomLabel();
            this.optionsLabel = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomLabel();
            this._SessionTraceDialog_Toolbars_Dock_Area_Left = new Infragistics.Win.UltraWinToolbars.UltraToolbarsDockArea();
            this._SessionTraceDialog_Toolbars_Dock_Area_Right = new Infragistics.Win.UltraWinToolbars.UltraToolbarsDockArea();
            this.sessionTraceHeaderStrip = new Idera.SQLdm.DesktopClient.Controls.HeaderStrip();
            this.startTraceToolStripButton = new System.Windows.Forms.ToolStripButton();
            this.stopTraceToolStripButton = new System.Windows.Forms.ToolStripButton();
            this.refreshToolStripButton = new System.Windows.Forms.ToolStripButton();
            this.clearTraceToolStripButton = new System.Windows.Forms.ToolStripButton();
            this.killSessionToolStripButton = new System.Windows.Forms.ToolStripButton();
            this.refreshToolStripLabel = new System.Windows.Forms.ToolStripLabel();
            this.SYSAdminWarningLabel = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomLabel();
            this._SessionTraceDialog_Toolbars_Dock_Area_Top = new Infragistics.Win.UltraWinToolbars.UltraToolbarsDockArea();
            this._SessionTraceDialog_Toolbars_Dock_Area_Bottom = new Infragistics.Win.UltraWinToolbars.UltraToolbarsDockArea();
            ((System.ComponentModel.ISupportInitialize)(this.traceGridDataSource)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.statusBar)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.traceGrid)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.toolbarsManager)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.autoRefreshSpinner)).BeginInit();
            this.contentPanel.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.durationSpinner)).BeginInit();
            this.statusPulseHeaderPanel.SuspendLayout();
            this.summaryGroupBox.SuspendLayout();
            this.sessionTraceHeaderStrip.SuspendLayout();
            this.SuspendLayout();
            // 
            // traceGridDataSource
            // 
            ultraDataColumn1.DataType = typeof(System.DateTime);
            ultraDataColumn3.DataType = typeof(long);
            ultraDataColumn4.DataType = typeof(long);
            ultraDataColumn5.DataType = typeof(long);
            ultraDataColumn6.DataType = typeof(long);
            ultraDataColumn8.DataType = typeof(int);
            this.traceGridDataSource.Band.Columns.AddRange(new object[] {
            ultraDataColumn1,
            ultraDataColumn2,
            ultraDataColumn3,
            ultraDataColumn4,
            ultraDataColumn5,
            ultraDataColumn6,
            ultraDataColumn7,
            ultraDataColumn8});
            // 
            // automatedRefreshTimer
            // 
            this.automatedRefreshTimer.Enabled = true;
            this.automatedRefreshTimer.Interval = 30000;
            this.automatedRefreshTimer.Tick += new System.EventHandler(this.automatedRefreshTimer_Tick);
            // 
            // statusBar
            // 
            this.statusBar.Location = new System.Drawing.Point(0, 513);
            this.statusBar.Name = "statusBar";
            this.statusBar.Padding = new Infragistics.Win.UltraWinStatusBar.UIElementMargins(5, 2, 0, 0);
            ultraStatusPanel1.SizingMode = Infragistics.Win.UltraWinStatusBar.PanelSizingMode.Automatic;
            ultraStatusPanel1.Text = "< Instance & Session >";
            ultraStatusPanel2.SizingMode = Infragistics.Win.UltraWinStatusBar.PanelSizingMode.Automatic;
            ultraStatusPanel2.Text = "< Trace Status >";
            ultraStatusPanel3.SizingMode = Infragistics.Win.UltraWinStatusBar.PanelSizingMode.Spring;
            ultraStatusPanel3.Text = "< Items >";
            appearance1.Image = global::Idera.SQLdm.DesktopClient.Properties.Resources.ToolbarRefresh;
            appearance1.ImageHAlign = Infragistics.Win.HAlign.Right;
            ultraStatusPanel4.Appearance = appearance1;
            ultraStatusPanel4.SizingMode = Infragistics.Win.UltraWinStatusBar.PanelSizingMode.Automatic;
            ultraStatusPanel4.Text = "Refreshing...";
            this.statusBar.Panels.AddRange(new Infragistics.Win.UltraWinStatusBar.UltraStatusPanel[] {
            ultraStatusPanel1,
            ultraStatusPanel2,
            ultraStatusPanel3,
            ultraStatusPanel4});
            this.statusBar.Size = new System.Drawing.Size(691, 23);
            this.statusBar.TabIndex = 69;
            this.statusBar.ViewStyle = Infragistics.Win.UltraWinStatusBar.ViewStyle.Office2007;
            this.statusBar.WrapText = false;
            this.statusBar.PanelClick += new Infragistics.Win.UltraWinStatusBar.PanelClickEventHandler(this.statusBar_PanelClick);
            // 
            // ultraGridPrintDocument
            // 
            this.ultraGridPrintDocument.DocumentName = "Session Trace";
            this.ultraGridPrintDocument.Grid = this.traceGrid;
            // 
            // traceGrid
            // 
            this.traceGrid.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.traceGrid.DataSource = this.traceGridDataSource;
            appearance2.BackColor = System.Drawing.SystemColors.Window;
            appearance2.BorderColor = System.Drawing.SystemColors.InactiveCaption;
            this.traceGrid.DisplayLayout.Appearance = appearance2;
            this.traceGrid.DisplayLayout.AutoFitStyle = Infragistics.Win.UltraWinGrid.AutoFitStyle.ExtendLastColumn;
            ultraGridColumn1.Format = "T";
            ultraGridColumn1.Header.VisiblePosition = 1;
            ultraGridColumn2.Header.VisiblePosition = 2;
            ultraGridColumn2.Width = 86;
            appearance3.TextHAlignAsString = "Right";
            ultraGridColumn3.CellAppearance = appearance3;
            ultraGridColumn3.Header.Caption = "Duration (ms)";
            ultraGridColumn3.Header.VisiblePosition = 3;
            ultraGridColumn3.Width = 76;
            appearance4.TextHAlignAsString = "Right";
            ultraGridColumn4.CellAppearance = appearance4;
            ultraGridColumn4.Header.Caption = "CPU Time (ms)";
            ultraGridColumn4.Header.VisiblePosition = 4;
            ultraGridColumn4.Width = 84;
            appearance5.TextHAlignAsString = "Right";
            ultraGridColumn5.CellAppearance = appearance5;
            ultraGridColumn5.Header.VisiblePosition = 5;
            ultraGridColumn5.Width = 43;
            appearance6.TextHAlignAsString = "Right";
            ultraGridColumn6.CellAppearance = appearance6;
            ultraGridColumn6.Header.VisiblePosition = 6;
            ultraGridColumn6.Width = 40;
            ultraGridColumn7.Header.VisiblePosition = 7;
            ultraGridColumn8.Header.Caption = "Sequence Number";
            ultraGridColumn8.Header.VisiblePosition = 0;
            ultraGridBand1.Columns.AddRange(new object[] {
            ultraGridColumn1,
            ultraGridColumn2,
            ultraGridColumn3,
            ultraGridColumn4,
            ultraGridColumn5,
            ultraGridColumn6,
            ultraGridColumn7,
            ultraGridColumn8});
            this.traceGrid.DisplayLayout.BandsSerializer.Add(ultraGridBand1);
            this.traceGrid.DisplayLayout.BorderStyle = Infragistics.Win.UIElementBorderStyle.Solid;
            this.traceGrid.DisplayLayout.CaptionVisible = Infragistics.Win.DefaultableBoolean.False;
            appearance7.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(235)))), ((int)(((byte)(235)))), ((int)(((byte)(235)))));
            appearance7.BackColor2 = System.Drawing.SystemColors.ControlDark;
            appearance7.BackGradientStyle = Infragistics.Win.GradientStyle.None;
            appearance7.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(140)))), ((int)(((byte)(140)))), ((int)(((byte)(140)))));
            this.traceGrid.DisplayLayout.GroupByBox.Appearance = appearance7;
            appearance8.ForeColor = System.Drawing.SystemColors.GrayText;
            this.traceGrid.DisplayLayout.GroupByBox.BandLabelAppearance = appearance8;
            this.traceGrid.DisplayLayout.GroupByBox.BorderStyle = Infragistics.Win.UIElementBorderStyle.Solid;
            this.traceGrid.DisplayLayout.GroupByBox.Hidden = true;
            appearance9.BackColor = System.Drawing.SystemColors.ControlLightLight;
            appearance9.BackColor2 = System.Drawing.SystemColors.Control;
            appearance9.BackGradientStyle = Infragistics.Win.GradientStyle.Horizontal;
            appearance9.ForeColor = System.Drawing.SystemColors.GrayText;
            this.traceGrid.DisplayLayout.GroupByBox.PromptAppearance = appearance9;
            this.traceGrid.DisplayLayout.MaxColScrollRegions = 1;
            this.traceGrid.DisplayLayout.MaxRowScrollRegions = 1;
            this.traceGrid.DisplayLayout.Override.AllowAddNew = Infragistics.Win.UltraWinGrid.AllowAddNew.No;
            this.traceGrid.DisplayLayout.Override.AllowDelete = Infragistics.Win.DefaultableBoolean.False;
            this.traceGrid.DisplayLayout.Override.AllowUpdate = Infragistics.Win.DefaultableBoolean.False;
            this.traceGrid.DisplayLayout.Override.BorderStyleCell = Infragistics.Win.UIElementBorderStyle.Solid;
            this.traceGrid.DisplayLayout.Override.BorderStyleRow = Infragistics.Win.UIElementBorderStyle.Solid;
            appearance10.BackColor = System.Drawing.SystemColors.Window;
            this.traceGrid.DisplayLayout.Override.CardAreaAppearance = appearance10;
            appearance11.BorderColor = System.Drawing.Color.Silver;
            appearance11.TextTrimming = Infragistics.Win.TextTrimming.EllipsisCharacter;
            this.traceGrid.DisplayLayout.Override.CellAppearance = appearance11;
            this.traceGrid.DisplayLayout.Override.CellClickAction = Infragistics.Win.UltraWinGrid.CellClickAction.RowSelect;
            this.traceGrid.DisplayLayout.Override.CellPadding = 0;
            this.traceGrid.DisplayLayout.Override.FixedHeaderIndicator = Infragistics.Win.UltraWinGrid.FixedHeaderIndicator.None;
            this.traceGrid.DisplayLayout.Override.GroupByColumnsHidden = Infragistics.Win.DefaultableBoolean.False;
            appearance12.BackColor = System.Drawing.SystemColors.Control;
            appearance12.BackColor2 = System.Drawing.SystemColors.ControlDark;
            appearance12.BackGradientAlignment = Infragistics.Win.GradientAlignment.Element;
            appearance12.BackGradientStyle = Infragistics.Win.GradientStyle.Horizontal;
            appearance12.BorderColor = System.Drawing.SystemColors.Window;
            this.traceGrid.DisplayLayout.Override.GroupByRowAppearance = appearance12;
            this.traceGrid.DisplayLayout.Override.GroupByRowInitialExpansionState = Infragistics.Win.UltraWinGrid.GroupByRowInitialExpansionState.Expanded;
            appearance13.TextHAlignAsString = "Left";
            this.traceGrid.DisplayLayout.Override.HeaderAppearance = appearance13;
            this.traceGrid.DisplayLayout.Override.HeaderClickAction = Infragistics.Win.UltraWinGrid.HeaderClickAction.SortMulti;
            appearance14.BackColor = System.Drawing.SystemColors.Window;
            appearance14.BorderColor = System.Drawing.Color.Silver;
            this.traceGrid.DisplayLayout.Override.RowAppearance = appearance14;
            this.traceGrid.DisplayLayout.Override.RowSelectors = Infragistics.Win.DefaultableBoolean.False;
            appearance15.ForeColor = System.Drawing.Color.Black;
            appearance15.ImageBackground = ((System.Drawing.Image)(resources.GetObject("appearance15.ImageBackground")));
            this.traceGrid.DisplayLayout.Override.SelectedRowAppearance = appearance15;
            this.traceGrid.DisplayLayout.Override.SelectTypeCell = Infragistics.Win.UltraWinGrid.SelectType.None;
            this.traceGrid.DisplayLayout.Override.SelectTypeCol = Infragistics.Win.UltraWinGrid.SelectType.None;
            this.traceGrid.DisplayLayout.Override.SelectTypeRow = Infragistics.Win.UltraWinGrid.SelectType.Single;
            this.traceGrid.DisplayLayout.Override.SelectTypeGroupByRow = Infragistics.Win.UltraWinGrid.SelectType.None;//JSFIX
            appearance16.BackColor = System.Drawing.SystemColors.ControlLight;
            this.traceGrid.DisplayLayout.Override.TemplateAddRowAppearance = appearance16;
            this.traceGrid.DisplayLayout.ScrollBounds = Infragistics.Win.UltraWinGrid.ScrollBounds.ScrollToFill;
            this.traceGrid.DisplayLayout.ScrollStyle = Infragistics.Win.UltraWinGrid.ScrollStyle.Immediate;
            this.traceGrid.DisplayLayout.UseFixedHeaders = true;
            this.traceGrid.DisplayLayout.ViewStyle = Infragistics.Win.UltraWinGrid.ViewStyle.SingleBand;
            this.traceGrid.DisplayLayout.ViewStyleBand = Infragistics.Win.UltraWinGrid.ViewStyleBand.OutlookGroupBy;
            this.traceGrid.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.traceGrid.Location = new System.Drawing.Point(0, 320);
            this.traceGrid.Name = "traceGrid";
            this.traceGrid.Size = new System.Drawing.Size(691, 166);
            this.traceGrid.TabIndex = 23;
            this.traceGrid.MouseDown += new System.Windows.Forms.MouseEventHandler(this.traceGrid_MouseDown);
            // 
            // ultraPrintPreviewDialog
            // 
            this.ultraPrintPreviewDialog.Document = this.ultraGridPrintDocument;
            this.ultraPrintPreviewDialog.Name = "ultraPrintPreviewDialog";
            // 
            // saveFileDialog
            // 
            this.saveFileDialog.DefaultExt = "xls";
            this.saveFileDialog.FileName = "SessionTrace";
            this.saveFileDialog.Filter = "Excel Workbook (*.xls)|*.xls";
            this.saveFileDialog.Title = "Save as Excel Spreadsheet";
            // 
            // toolbarsManager
            // 
            this.toolbarsManager.DesignerFlags = 1;
            this.toolbarsManager.DockWithinContainer = this;
            this.toolbarsManager.DockWithinContainerBaseType = typeof(System.Windows.Forms.Form);
            this.toolbarsManager.ShowFullMenusDelay = 500;
            popupMenuTool1.SharedPropsInternal.Caption = "columnContextMenu";
            stateButtonTool1.MenuDisplayStyle = Infragistics.Win.UltraWinToolbars.StateButtonMenuDisplayStyle.DisplayCheckmark;
            buttonTool4.InstanceProps.IsFirstInGroup = true;
            popupMenuTool1.Tools.AddRange(new Infragistics.Win.UltraWinToolbars.ToolBase[] {
            buttonTool1,
            buttonTool2,
            stateButtonTool1,
            buttonTool3,
            buttonTool4,
            buttonTool5});
            appearance17.Image = global::Idera.SQLdm.DesktopClient.Properties.Resources.ColumnChooser;
            buttonTool6.SharedPropsInternal.AppearancesSmall.Appearance = appearance17;
            buttonTool6.SharedPropsInternal.Caption = "Column Chooser";
            appearance18.Image = global::Idera.SQLdm.DesktopClient.Properties.Resources.GroupByBox;
            buttonTool7.SharedPropsInternal.AppearancesSmall.Appearance = appearance18;
            buttonTool7.SharedPropsInternal.Caption = "Group By Box";
            appearance19.Image = global::Idera.SQLdm.DesktopClient.Properties.Resources.SortAscending;
            buttonTool8.SharedPropsInternal.AppearancesSmall.Appearance = appearance19;
            buttonTool8.SharedPropsInternal.Caption = "Sort Ascending";
            appearance20.Image = global::Idera.SQLdm.DesktopClient.Properties.Resources.SortDescending;
            buttonTool9.SharedPropsInternal.AppearancesSmall.Appearance = appearance20;
            buttonTool9.SharedPropsInternal.Caption = "Sort Descending";
            buttonTool10.SharedPropsInternal.Caption = "Remove This Column";
            stateButtonTool2.MenuDisplayStyle = Infragistics.Win.UltraWinToolbars.StateButtonMenuDisplayStyle.DisplayCheckmark;
            stateButtonTool2.SharedPropsInternal.Caption = "Group By This Column";
            popupMenuTool2.SharedPropsInternal.Caption = "gridContextMenu";
            popupMenuTool2.Tools.AddRange(new Infragistics.Win.UltraWinToolbars.ToolBase[] {
            buttonTool11,
            buttonTool12});
            appearance21.Image = global::Idera.SQLdm.DesktopClient.Properties.Resources.Print16x16;
            buttonTool13.SharedPropsInternal.AppearancesSmall.Appearance = appearance21;
            buttonTool13.SharedPropsInternal.Caption = "Print";
            appearance22.Image = global::Idera.SQLdm.DesktopClient.Properties.Resources.Export16x16;
            buttonTool14.SharedPropsInternal.AppearancesSmall.Appearance = appearance22;
            buttonTool14.SharedPropsInternal.Caption = "Export To Excel";
            this.toolbarsManager.Tools.AddRange(new Infragistics.Win.UltraWinToolbars.ToolBase[] {
            popupMenuTool1,
            buttonTool6,
            buttonTool7,
            buttonTool8,
            buttonTool9,
            buttonTool10,
            stateButtonTool2,
            popupMenuTool2,
            buttonTool13,
            buttonTool14});
            this.toolbarsManager.ToolClick += new Infragistics.Win.UltraWinToolbars.ToolClickEventHandler(this.toolbarsManager_ToolClick);
            this.toolbarsManager.BeforeToolDropdown += new Infragistics.Win.UltraWinToolbars.BeforeToolDropdownEventHandler(this.toolbarsManager_BeforeToolDropdown);
            // 
            // autoRefreshSpinner
            // 
            this.autoRefreshSpinner.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.autoRefreshSpinner.Location = new System.Drawing.Point(592, 2);
            this.autoRefreshSpinner.Maximum = new decimal(new int[] {
            1800,
            0,
            0,
            0});
            this.autoRefreshSpinner.Minimum = new decimal(new int[] {
            3,
            0,
            0,
            0});
            this.autoRefreshSpinner.Name = "autoRefreshSpinner";
            this.autoRefreshSpinner.Size = new System.Drawing.Size(48, 20);
            this.autoRefreshSpinner.TabIndex = 1;
            this.autoRefreshSpinner.Value = new decimal(new int[] {
            10,
            0,
            0,
            0});
            this.autoRefreshSpinner.ValueChanged += new System.EventHandler(this.autoRefreshSpinner_ValueChanged);
            // 
            // contentPanel
            // 
            this.contentPanel.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(213)))), ((int)(((byte)(213)))), ((int)(((byte)(213)))));
            this.contentPanel.BackColor2 = System.Drawing.Color.Empty;
            this.contentPanel.BorderColor = System.Drawing.SystemColors.ControlDark;
            this.contentPanel.Controls.Add(this.durationSpinner);
            this.contentPanel.Controls.Add(this.lastCommandTextBox);
            this.contentPanel.Controls.Add(this.optionsTextBox);
            this.contentPanel.Controls.Add(this.lastCommandLabel);
            this.contentPanel.Controls.Add(this.statusPulseHeaderPanel);
            this.contentPanel.Controls.Add(this.durationLabel);
            this.contentPanel.Controls.Add(this.gridLabel);
            this.contentPanel.Controls.Add(this.summaryGroupBox);
            this.contentPanel.Controls.Add(this.optionsLabel);
            this.contentPanel.Controls.Add(this.traceGrid);
            this.contentPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.contentPanel.GradientAngle = 90;
            this.contentPanel.Location = new System.Drawing.Point(0, 25);
            this.contentPanel.Name = "contentPanel";
            this.contentPanel.Padding = new System.Windows.Forms.Padding(0, 2, 0, 2);
            this.contentPanel.ShowBorder = false;
            this.contentPanel.Size = new System.Drawing.Size(691, 488);
            this.contentPanel.TabIndex = 81;
            // 
            // durationSpinner
            // 
            this.durationSpinner.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.durationSpinner.Location = new System.Drawing.Point(558, 298);
            this.durationSpinner.Maximum = new decimal(new int[] {
            100000000,
            0,
            0,
            0});
            this.durationSpinner.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.durationSpinner.Name = "durationSpinner";
            this.durationSpinner.Size = new System.Drawing.Size(67, 20);
            this.durationSpinner.TabIndex = 22;
            this.durationSpinner.Value = new decimal(new int[] {
            5000,
            0,
            0,
            0});
            // 
            // lastCommandTextBox
            // 
            this.lastCommandTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.lastCommandTextBox.BackColor = System.Drawing.Color.White;
            this.lastCommandTextBox.Location = new System.Drawing.Point(3, 235);
            this.lastCommandTextBox.Multiline = true;
            this.lastCommandTextBox.Name = "lastCommandTextBox";
            this.lastCommandTextBox.ReadOnly = true;
            this.lastCommandTextBox.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.lastCommandTextBox.Size = new System.Drawing.Size(685, 60);
            this.lastCommandTextBox.TabIndex = 21;
            // 
            // optionsTextBox
            // 
            this.optionsTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.optionsTextBox.BackColor = System.Drawing.Color.White;
            this.optionsTextBox.Location = new System.Drawing.Point(3, 183);
            this.optionsTextBox.Multiline = true;
            this.optionsTextBox.Name = "optionsTextBox";
            this.optionsTextBox.ReadOnly = true;
            this.optionsTextBox.Size = new System.Drawing.Size(685, 30);
            this.optionsTextBox.TabIndex = 20;
            // 
            // lastCommandLabel
            // 
            this.lastCommandLabel.AutoSize = true;
            this.lastCommandLabel.BackColor = System.Drawing.Color.Transparent;
            this.lastCommandLabel.Location = new System.Drawing.Point(0, 221);
            this.lastCommandLabel.Name = "lastCommandLabel";
            this.lastCommandLabel.Size = new System.Drawing.Size(77, 13);
            this.lastCommandLabel.TabIndex = 67;
            this.lastCommandLabel.Text = "Last Command";
            // 
            // statusPulseHeaderPanel
            // 
            this.statusPulseHeaderPanel.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.statusPulseHeaderPanel.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(93)))), ((int)(((byte)(140)))), ((int)(((byte)(201)))));
            this.statusPulseHeaderPanel.Controls.Add(this.statusValueLabel);
            this.statusPulseHeaderPanel.FillColor1 = System.Drawing.Color.FromArgb(((int)(((byte)(252)))), ((int)(((byte)(253)))), ((int)(((byte)(254)))));
            this.statusPulseHeaderPanel.FillColor2 = System.Drawing.Color.FromArgb(((int)(((byte)(194)))), ((int)(((byte)(212)))), ((int)(((byte)(235)))));
            this.statusPulseHeaderPanel.Location = new System.Drawing.Point(2, 2);
            this.statusPulseHeaderPanel.Name = "statusPulseHeaderPanel";
            this.statusPulseHeaderPanel.Size = new System.Drawing.Size(687, 26);
            this.statusPulseHeaderPanel.TabIndex = 68;
            // 
            // statusValueLabel
            // 
            this.statusValueLabel.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(213)))), ((int)(((byte)(213)))), ((int)(((byte)(213)))));
            this.statusValueLabel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.statusValueLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.statusValueLabel.Location = new System.Drawing.Point(0, 0);
            this.statusValueLabel.Name = "statusValueLabel";
            this.statusValueLabel.Size = new System.Drawing.Size(687, 26);
            this.statusValueLabel.TabIndex = 2;
            this.statusValueLabel.Text = "< status >";
            this.statusValueLabel.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // durationLabel
            // 
            this.durationLabel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.durationLabel.AutoSize = true;
            this.durationLabel.BackColor = System.Drawing.Color.Transparent;
            this.durationLabel.ForeColor = System.Drawing.Color.Black;
            this.durationLabel.Location = new System.Drawing.Point(400, 302);
            this.durationLabel.Name = "durationLabel";
            this.durationLabel.Size = new System.Drawing.Size(291, 13);
            this.durationLabel.TabIndex = 66;
            this.durationLabel.Text = "Highlight durations greater than                           milliseconds";
            // 
            // gridLabel
            // 
            this.gridLabel.AutoSize = true;
            this.gridLabel.BackColor = System.Drawing.Color.Transparent;
            this.gridLabel.Location = new System.Drawing.Point(0, 305);
            this.gridLabel.Name = "gridLabel";
            this.gridLabel.Size = new System.Drawing.Size(118, 13);
            this.gridLabel.TabIndex = 62;
            this.gridLabel.Text = "SQL Server Statements";
            // 
            // summaryGroupBox
            // 
            this.summaryGroupBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.summaryGroupBox.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(213)))), ((int)(((byte)(213)))), ((int)(((byte)(213)))));
            this.summaryGroupBox.Controls.Add(this.commandTextBox);
            this.summaryGroupBox.Controls.Add(this.isolationLevelTextBox);
            this.summaryGroupBox.Controls.Add(this.deadlockPriorityTextBox);
            this.summaryGroupBox.Controls.Add(this.languageTextBox);
            this.summaryGroupBox.Controls.Add(this.lockWaitTextBox);
            this.summaryGroupBox.Controls.Add(this.nestingLevelTextBox);
            this.summaryGroupBox.Controls.Add(this.textSizeTextBox);
            this.summaryGroupBox.Controls.Add(this.lineNumberTextBox);
            this.summaryGroupBox.Controls.Add(this.lastErrorNumberTextBox);
            this.summaryGroupBox.Controls.Add(this.cursorSetRowsTextBox);
            this.summaryGroupBox.Controls.Add(this.cursorFetchStatusTextBox);
            this.summaryGroupBox.Controls.Add(this.openTransactionsTextBox);
            this.summaryGroupBox.Controls.Add(this.writesTextBox);
            this.summaryGroupBox.Controls.Add(this.readsTextBox);
            this.summaryGroupBox.Controls.Add(this.rowCountTextBox);
            this.summaryGroupBox.Controls.Add(this.cpuTimeTextBox);
            this.summaryGroupBox.Controls.Add(this.cpuTimeLabel);
            this.summaryGroupBox.Controls.Add(this.rowCountLabel);
            this.summaryGroupBox.Controls.Add(this.textSizeLabel);
            this.summaryGroupBox.Controls.Add(this.readsLabel);
            this.summaryGroupBox.Controls.Add(this.lastErrorLabel);
            this.summaryGroupBox.Controls.Add(this.openTransactionslabel);
            this.summaryGroupBox.Controls.Add(this.cursorFetchStatusLabel);
            this.summaryGroupBox.Controls.Add(this.cursorSetRowsLabel);
            this.summaryGroupBox.Controls.Add(this.commandLabel);
            this.summaryGroupBox.Controls.Add(this.languagelabel);
            this.summaryGroupBox.Controls.Add(this.writesLabel);
            this.summaryGroupBox.Controls.Add(this.isoLevelLabel);
            this.summaryGroupBox.Controls.Add(this.lineNumberLabel);
            this.summaryGroupBox.Controls.Add(this.nestingLevelLabel);
            this.summaryGroupBox.Controls.Add(this.lockWaitLabel);
            this.summaryGroupBox.Controls.Add(this.deadlockPriorityLabel);
            this.summaryGroupBox.Location = new System.Drawing.Point(3, 30);
            this.summaryGroupBox.Name = "summaryGroupBox";
            this.summaryGroupBox.Size = new System.Drawing.Size(687, 136);
            this.summaryGroupBox.TabIndex = 3;
            this.summaryGroupBox.TabStop = false;
            this.summaryGroupBox.Text = "Trace Overview";
            // 
            // commandTextBox
            // 
            this.commandTextBox.BackColor = System.Drawing.Color.White;
            this.commandTextBox.Location = new System.Drawing.Point(288, 106);
            this.commandTextBox.Name = "commandTextBox";
            this.commandTextBox.ReadOnly = true;
            this.commandTextBox.Size = new System.Drawing.Size(393, 20);
            this.commandTextBox.TabIndex = 19;
            // 
            // isolationLevelTextBox
            // 
            this.isolationLevelTextBox.BackColor = System.Drawing.Color.White;
            this.isolationLevelTextBox.Location = new System.Drawing.Point(5, 106);
            this.isolationLevelTextBox.Name = "isolationLevelTextBox";
            this.isolationLevelTextBox.ReadOnly = true;
            this.isolationLevelTextBox.Size = new System.Drawing.Size(277, 20);
            this.isolationLevelTextBox.TabIndex = 18;
            // 
            // deadlockPriorityTextBox
            // 
            this.deadlockPriorityTextBox.BackColor = System.Drawing.Color.White;
            this.deadlockPriorityTextBox.Location = new System.Drawing.Point(595, 68);
            this.deadlockPriorityTextBox.Name = "deadlockPriorityTextBox";
            this.deadlockPriorityTextBox.ReadOnly = true;
            this.deadlockPriorityTextBox.Size = new System.Drawing.Size(86, 20);
            this.deadlockPriorityTextBox.TabIndex = 17;
            // 
            // languageTextBox
            // 
            this.languageTextBox.BackColor = System.Drawing.Color.White;
            this.languageTextBox.Location = new System.Drawing.Point(485, 68);
            this.languageTextBox.Name = "languageTextBox";
            this.languageTextBox.ReadOnly = true;
            this.languageTextBox.Size = new System.Drawing.Size(104, 20);
            this.languageTextBox.TabIndex = 16;
            // 
            // lockWaitTextBox
            // 
            this.lockWaitTextBox.BackColor = System.Drawing.Color.White;
            this.lockWaitTextBox.Location = new System.Drawing.Point(385, 68);
            this.lockWaitTextBox.Name = "lockWaitTextBox";
            this.lockWaitTextBox.ReadOnly = true;
            this.lockWaitTextBox.Size = new System.Drawing.Size(91, 20);
            this.lockWaitTextBox.TabIndex = 15;
            // 
            // nestingLevelTextBox
            // 
            this.nestingLevelTextBox.BackColor = System.Drawing.Color.White;
            this.nestingLevelTextBox.Location = new System.Drawing.Point(288, 68);
            this.nestingLevelTextBox.Name = "nestingLevelTextBox";
            this.nestingLevelTextBox.ReadOnly = true;
            this.nestingLevelTextBox.Size = new System.Drawing.Size(91, 20);
            this.nestingLevelTextBox.TabIndex = 14;
            // 
            // textSizeTextBox
            // 
            this.textSizeTextBox.BackColor = System.Drawing.Color.White;
            this.textSizeTextBox.Location = new System.Drawing.Point(196, 68);
            this.textSizeTextBox.Name = "textSizeTextBox";
            this.textSizeTextBox.ReadOnly = true;
            this.textSizeTextBox.Size = new System.Drawing.Size(86, 20);
            this.textSizeTextBox.TabIndex = 13;
            // 
            // lineNumberTextBox
            // 
            this.lineNumberTextBox.BackColor = System.Drawing.Color.White;
            this.lineNumberTextBox.Location = new System.Drawing.Point(104, 68);
            this.lineNumberTextBox.Name = "lineNumberTextBox";
            this.lineNumberTextBox.ReadOnly = true;
            this.lineNumberTextBox.Size = new System.Drawing.Size(86, 20);
            this.lineNumberTextBox.TabIndex = 12;
            // 
            // lastErrorNumberTextBox
            // 
            this.lastErrorNumberTextBox.BackColor = System.Drawing.Color.White;
            this.lastErrorNumberTextBox.Location = new System.Drawing.Point(6, 68);
            this.lastErrorNumberTextBox.Name = "lastErrorNumberTextBox";
            this.lastErrorNumberTextBox.ReadOnly = true;
            this.lastErrorNumberTextBox.Size = new System.Drawing.Size(92, 20);
            this.lastErrorNumberTextBox.TabIndex = 11;
            // 
            // cursorSetRowsTextBox
            // 
            this.cursorSetRowsTextBox.BackColor = System.Drawing.Color.White;
            this.cursorSetRowsTextBox.Location = new System.Drawing.Point(595, 30);
            this.cursorSetRowsTextBox.Name = "cursorSetRowsTextBox";
            this.cursorSetRowsTextBox.ReadOnly = true;
            this.cursorSetRowsTextBox.Size = new System.Drawing.Size(86, 20);
            this.cursorSetRowsTextBox.TabIndex = 10;
            // 
            // cursorFetchStatusTextBox
            // 
            this.cursorFetchStatusTextBox.BackColor = System.Drawing.Color.White;
            this.cursorFetchStatusTextBox.Location = new System.Drawing.Point(485, 30);
            this.cursorFetchStatusTextBox.Name = "cursorFetchStatusTextBox";
            this.cursorFetchStatusTextBox.ReadOnly = true;
            this.cursorFetchStatusTextBox.Size = new System.Drawing.Size(104, 20);
            this.cursorFetchStatusTextBox.TabIndex = 9;
            // 
            // openTransactionsTextBox
            // 
            this.openTransactionsTextBox.BackColor = System.Drawing.Color.White;
            this.openTransactionsTextBox.Location = new System.Drawing.Point(385, 30);
            this.openTransactionsTextBox.Name = "openTransactionsTextBox";
            this.openTransactionsTextBox.ReadOnly = true;
            this.openTransactionsTextBox.Size = new System.Drawing.Size(91, 20);
            this.openTransactionsTextBox.TabIndex = 8;
            // 
            // writesTextBox
            // 
            this.writesTextBox.BackColor = System.Drawing.Color.White;
            this.writesTextBox.Location = new System.Drawing.Point(288, 30);
            this.writesTextBox.Name = "writesTextBox";
            this.writesTextBox.ReadOnly = true;
            this.writesTextBox.Size = new System.Drawing.Size(91, 20);
            this.writesTextBox.TabIndex = 7;
            // 
            // readsTextBox
            // 
            this.readsTextBox.BackColor = System.Drawing.Color.White;
            this.readsTextBox.Location = new System.Drawing.Point(196, 30);
            this.readsTextBox.Name = "readsTextBox";
            this.readsTextBox.ReadOnly = true;
            this.readsTextBox.Size = new System.Drawing.Size(86, 20);
            this.readsTextBox.TabIndex = 6;
            // 
            // rowCountTextBox
            // 
            this.rowCountTextBox.BackColor = System.Drawing.Color.White;
            this.rowCountTextBox.Location = new System.Drawing.Point(104, 30);
            this.rowCountTextBox.Name = "rowCountTextBox";
            this.rowCountTextBox.ReadOnly = true;
            this.rowCountTextBox.Size = new System.Drawing.Size(86, 20);
            this.rowCountTextBox.TabIndex = 5;
            // 
            // cpuTimeTextBox
            // 
            this.cpuTimeTextBox.BackColor = System.Drawing.Color.White;
            this.cpuTimeTextBox.Location = new System.Drawing.Point(6, 30);
            this.cpuTimeTextBox.Name = "cpuTimeTextBox";
            this.cpuTimeTextBox.ReadOnly = true;
            this.cpuTimeTextBox.Size = new System.Drawing.Size(92, 20);
            this.cpuTimeTextBox.TabIndex = 4;
            // 
            // cpuTimeLabel
            // 
            this.cpuTimeLabel.AutoSize = true;
            this.cpuTimeLabel.Location = new System.Drawing.Point(3, 16);
            this.cpuTimeLabel.Name = "cpuTimeLabel";
            this.cpuTimeLabel.Size = new System.Drawing.Size(77, 13);
            this.cpuTimeLabel.TabIndex = 13;
            this.cpuTimeLabel.Text = "CPU Time (ms)";
            // 
            // rowCountLabel
            // 
            this.rowCountLabel.AutoSize = true;
            this.rowCountLabel.Location = new System.Drawing.Point(101, 16);
            this.rowCountLabel.Name = "rowCountLabel";
            this.rowCountLabel.Size = new System.Drawing.Size(60, 13);
            this.rowCountLabel.TabIndex = 16;
            this.rowCountLabel.Text = "Row Count";
            // 
            // textSizeLabel
            // 
            this.textSizeLabel.AutoSize = true;
            this.textSizeLabel.Location = new System.Drawing.Point(193, 54);
            this.textSizeLabel.Name = "textSizeLabel";
            this.textSizeLabel.Size = new System.Drawing.Size(51, 13);
            this.textSizeLabel.TabIndex = 44;
            this.textSizeLabel.Text = "Text Size";
            // 
            // readsLabel
            // 
            this.readsLabel.AutoSize = true;
            this.readsLabel.Location = new System.Drawing.Point(193, 16);
            this.readsLabel.Name = "readsLabel";
            this.readsLabel.Size = new System.Drawing.Size(80, 13);
            this.readsLabel.TabIndex = 18;
            this.readsLabel.Text = "Physical Reads";
            // 
            // lastErrorLabel
            // 
            this.lastErrorLabel.AutoSize = true;
            this.lastErrorLabel.Location = new System.Drawing.Point(3, 54);
            this.lastErrorLabel.Name = "lastErrorLabel";
            this.lastErrorLabel.Size = new System.Drawing.Size(92, 13);
            this.lastErrorLabel.TabIndex = 37;
            this.lastErrorLabel.Text = "Last Error Number";
            // 
            // openTransactionslabel
            // 
            this.openTransactionslabel.AutoSize = true;
            this.openTransactionslabel.Location = new System.Drawing.Point(382, 16);
            this.openTransactionslabel.Name = "openTransactionslabel";
            this.openTransactionslabel.Size = new System.Drawing.Size(97, 13);
            this.openTransactionslabel.TabIndex = 35;
            this.openTransactionslabel.Text = "Open Transactions";
            // 
            // cursorFetchStatusLabel
            // 
            this.cursorFetchStatusLabel.AutoSize = true;
            this.cursorFetchStatusLabel.Location = new System.Drawing.Point(481, 16);
            this.cursorFetchStatusLabel.Name = "cursorFetchStatusLabel";
            this.cursorFetchStatusLabel.Size = new System.Drawing.Size(100, 13);
            this.cursorFetchStatusLabel.TabIndex = 20;
            this.cursorFetchStatusLabel.Text = "Cursor Fetch Status";
            // 
            // cursorSetRowsLabel
            // 
            this.cursorSetRowsLabel.AutoSize = true;
            this.cursorSetRowsLabel.Location = new System.Drawing.Point(592, 16);
            this.cursorSetRowsLabel.Name = "cursorSetRowsLabel";
            this.cursorSetRowsLabel.Size = new System.Drawing.Size(86, 13);
            this.cursorSetRowsLabel.TabIndex = 22;
            this.cursorSetRowsLabel.Text = "Cursor Set Rows";
            // 
            // commandLabel
            // 
            this.commandLabel.AutoSize = true;
            this.commandLabel.Location = new System.Drawing.Point(285, 92);
            this.commandLabel.Name = "commandLabel";
            this.commandLabel.Size = new System.Drawing.Size(91, 13);
            this.commandLabel.TabIndex = 57;
            this.commandLabel.Text = "Current Command";
            // 
            // languagelabel
            // 
            this.languagelabel.AutoSize = true;
            this.languagelabel.Location = new System.Drawing.Point(482, 54);
            this.languagelabel.Name = "languagelabel";
            this.languagelabel.Size = new System.Drawing.Size(55, 13);
            this.languagelabel.TabIndex = 55;
            this.languagelabel.Text = "Language";
            // 
            // writesLabel
            // 
            this.writesLabel.AutoSize = true;
            this.writesLabel.Location = new System.Drawing.Point(285, 16);
            this.writesLabel.Name = "writesLabel";
            this.writesLabel.Size = new System.Drawing.Size(79, 13);
            this.writesLabel.TabIndex = 26;
            this.writesLabel.Text = "Physical Writes";
            // 
            // isoLevelLabel
            // 
            this.isoLevelLabel.AutoSize = true;
            this.isoLevelLabel.Location = new System.Drawing.Point(3, 92);
            this.isoLevelLabel.Name = "isoLevelLabel";
            this.isoLevelLabel.Size = new System.Drawing.Size(134, 13);
            this.isoLevelLabel.TabIndex = 53;
            this.isoLevelLabel.Text = "Transaction Isolation Level";
            // 
            // lineNumberLabel
            // 
            this.lineNumberLabel.AutoSize = true;
            this.lineNumberLabel.Location = new System.Drawing.Point(101, 54);
            this.lineNumberLabel.Name = "lineNumberLabel";
            this.lineNumberLabel.Size = new System.Drawing.Size(67, 13);
            this.lineNumberLabel.TabIndex = 39;
            this.lineNumberLabel.Text = "Line Number";
            // 
            // nestingLevelLabel
            // 
            this.nestingLevelLabel.AutoSize = true;
            this.nestingLevelLabel.Location = new System.Drawing.Point(284, 54);
            this.nestingLevelLabel.Name = "nestingLevelLabel";
            this.nestingLevelLabel.Size = new System.Drawing.Size(72, 13);
            this.nestingLevelLabel.TabIndex = 41;
            this.nestingLevelLabel.Text = "Nesting Level";
            // 
            // lockWaitLabel
            // 
            this.lockWaitLabel.AutoSize = true;
            this.lockWaitLabel.Location = new System.Drawing.Point(382, 54);
            this.lockWaitLabel.Name = "lockWaitLabel";
            this.lockWaitLabel.Size = new System.Drawing.Size(97, 13);
            this.lockWaitLabel.TabIndex = 46;
            this.lockWaitLabel.Text = "Lock Wait Timeout";
            // 
            // deadlockPriorityLabel
            // 
            this.deadlockPriorityLabel.AutoSize = true;
            this.deadlockPriorityLabel.Location = new System.Drawing.Point(592, 54);
            this.deadlockPriorityLabel.Name = "deadlockPriorityLabel";
            this.deadlockPriorityLabel.Size = new System.Drawing.Size(87, 13);
            this.deadlockPriorityLabel.TabIndex = 42;
            this.deadlockPriorityLabel.Text = "Deadlock Priority";
            // 
            // optionsLabel
            // 
            this.optionsLabel.AutoSize = true;
            this.optionsLabel.BackColor = System.Drawing.Color.Transparent;
            this.optionsLabel.Location = new System.Drawing.Point(0, 169);
            this.optionsLabel.Name = "optionsLabel";
            this.optionsLabel.Size = new System.Drawing.Size(86, 13);
            this.optionsLabel.TabIndex = 49;
            this.optionsLabel.Text = "Options In Effect";
            // 
            // _SessionTraceDialog_Toolbars_Dock_Area_Left
            // 
            this._SessionTraceDialog_Toolbars_Dock_Area_Left.AccessibleRole = System.Windows.Forms.AccessibleRole.Grouping;
            this._SessionTraceDialog_Toolbars_Dock_Area_Left.BackColor = System.Drawing.SystemColors.Control;
            this._SessionTraceDialog_Toolbars_Dock_Area_Left.DockedPosition = Infragistics.Win.UltraWinToolbars.DockedPosition.Left;
            this._SessionTraceDialog_Toolbars_Dock_Area_Left.ForeColor = System.Drawing.SystemColors.ControlText;
            this._SessionTraceDialog_Toolbars_Dock_Area_Left.Location = new System.Drawing.Point(0, 25);
            this._SessionTraceDialog_Toolbars_Dock_Area_Left.Name = "_SessionTraceDialog_Toolbars_Dock_Area_Left";
            this._SessionTraceDialog_Toolbars_Dock_Area_Left.Size = new System.Drawing.Size(0, 488);
            this._SessionTraceDialog_Toolbars_Dock_Area_Left.ToolbarsManager = this.toolbarsManager;
            // 
            // _SessionTraceDialog_Toolbars_Dock_Area_Right
            // 
            this._SessionTraceDialog_Toolbars_Dock_Area_Right.AccessibleRole = System.Windows.Forms.AccessibleRole.Grouping;
            this._SessionTraceDialog_Toolbars_Dock_Area_Right.BackColor = System.Drawing.SystemColors.Control;
            this._SessionTraceDialog_Toolbars_Dock_Area_Right.DockedPosition = Infragistics.Win.UltraWinToolbars.DockedPosition.Right;
            this._SessionTraceDialog_Toolbars_Dock_Area_Right.ForeColor = System.Drawing.SystemColors.ControlText;
            this._SessionTraceDialog_Toolbars_Dock_Area_Right.Location = new System.Drawing.Point(691, 25);
            this._SessionTraceDialog_Toolbars_Dock_Area_Right.Name = "_SessionTraceDialog_Toolbars_Dock_Area_Right";
            this._SessionTraceDialog_Toolbars_Dock_Area_Right.Size = new System.Drawing.Size(0, 488);
            this._SessionTraceDialog_Toolbars_Dock_Area_Right.ToolbarsManager = this.toolbarsManager;
            // 
            // sessionTraceHeaderStrip
            // 
            this.sessionTraceHeaderStrip.AutoSize = false;
            this.sessionTraceHeaderStrip.Font = new System.Drawing.Font("Arial", 12F, System.Drawing.FontStyle.Bold);
            this.sessionTraceHeaderStrip.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(21)))), ((int)(((byte)(66)))), ((int)(((byte)(139)))));
            this.sessionTraceHeaderStrip.GripStyle = System.Windows.Forms.ToolStripGripStyle.Hidden;
            this.sessionTraceHeaderStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.startTraceToolStripButton,
            this.stopTraceToolStripButton,
            this.refreshToolStripButton,
            this.clearTraceToolStripButton,
            this.killSessionToolStripButton,
            this.refreshToolStripLabel});
            this.sessionTraceHeaderStrip.Location = new System.Drawing.Point(0, 0);
            this.sessionTraceHeaderStrip.Name = "sessionTraceHeaderStrip";
            this.sessionTraceHeaderStrip.Padding = new System.Windows.Forms.Padding(0, 2, 0, 0);
            this.sessionTraceHeaderStrip.Size = new System.Drawing.Size(691, 25);
            this.sessionTraceHeaderStrip.TabIndex = 0;
            this.sessionTraceHeaderStrip.Text = "headerStrip1";
            // 
            // startTraceToolStripButton
            // 
            this.startTraceToolStripButton.Font = new System.Drawing.Font("Arial", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.startTraceToolStripButton.ForeColor = System.Drawing.Color.Black;
            this.startTraceToolStripButton.Image = global::Idera.SQLdm.DesktopClient.Properties.Resources.StartTrace;
            this.startTraceToolStripButton.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.startTraceToolStripButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.startTraceToolStripButton.Name = "startTraceToolStripButton";
            this.startTraceToolStripButton.Size = new System.Drawing.Size(86, 20);
            this.startTraceToolStripButton.Text = "Start Trace";
            this.startTraceToolStripButton.ToolTipText = "Start Trace";
            this.startTraceToolStripButton.Click += new System.EventHandler(this.startTraceToolStripButton_Click);
            // 
            // stopTraceToolStripButton
            // 
            this.stopTraceToolStripButton.Font = new System.Drawing.Font("Arial", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.stopTraceToolStripButton.ForeColor = System.Drawing.Color.Black;
            this.stopTraceToolStripButton.Image = global::Idera.SQLdm.DesktopClient.Properties.Resources.StopTrace;
            this.stopTraceToolStripButton.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.stopTraceToolStripButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.stopTraceToolStripButton.Name = "stopTraceToolStripButton";
            this.stopTraceToolStripButton.Size = new System.Drawing.Size(85, 20);
            this.stopTraceToolStripButton.Text = "Stop Trace";
            this.stopTraceToolStripButton.ToolTipText = "Stop Trace";
            this.stopTraceToolStripButton.Click += new System.EventHandler(this.stopTraceToolStripButton_Click);
            // 
            // refreshToolStripButton
            // 
            this.refreshToolStripButton.Font = new System.Drawing.Font("Arial", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.refreshToolStripButton.ForeColor = System.Drawing.Color.Black;
            this.refreshToolStripButton.Image = global::Idera.SQLdm.DesktopClient.Properties.Resources.ToolbarRefresh;
            this.refreshToolStripButton.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.refreshToolStripButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.refreshToolStripButton.Name = "refreshToolStripButton";
            this.refreshToolStripButton.Size = new System.Drawing.Size(71, 20);
            this.refreshToolStripButton.Text = "Refresh";
            this.refreshToolStripButton.ToolTipText = "Refresh Trace data now";
            this.refreshToolStripButton.Click += new System.EventHandler(this.refreshButton_Click);
            // 
            // clearTraceToolStripButton
            // 
            this.clearTraceToolStripButton.Font = new System.Drawing.Font("Arial", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.clearTraceToolStripButton.ForeColor = System.Drawing.Color.Black;
            this.clearTraceToolStripButton.Image = global::Idera.SQLdm.DesktopClient.Properties.Resources.ClearTrace;
            this.clearTraceToolStripButton.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.clearTraceToolStripButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.clearTraceToolStripButton.Name = "clearTraceToolStripButton";
            this.clearTraceToolStripButton.Size = new System.Drawing.Size(56, 20);
            this.clearTraceToolStripButton.Text = "Clear";
            this.clearTraceToolStripButton.ToolTipText = "Clear SQL Server Statements";
            this.clearTraceToolStripButton.Click += new System.EventHandler(this.clearTraceToolStripButton_Click);
            // 
            // killSessionToolStripButton
            // 
            this.killSessionToolStripButton.Enabled = false;
            this.killSessionToolStripButton.Font = new System.Drawing.Font("Arial", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.killSessionToolStripButton.ForeColor = System.Drawing.Color.Black;
            this.killSessionToolStripButton.Image = global::Idera.SQLdm.DesktopClient.Properties.Resources.DeleteSmall;
            this.killSessionToolStripButton.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.killSessionToolStripButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.killSessionToolStripButton.Name = "killSessionToolStripButton";
            this.killSessionToolStripButton.Size = new System.Drawing.Size(43, 20);
            this.killSessionToolStripButton.Text = "Kill";
            this.killSessionToolStripButton.ToolTipText = "Kill Session";
            this.killSessionToolStripButton.Click += new System.EventHandler(this.killSessionToolStripButton_Click);
            // 
            // SYSADMIN check
            //
            this.SYSAdminWarningLabel.BackColor = System.Drawing.Color.White;
            this.SYSAdminWarningLabel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.SYSAdminWarningLabel.Location = new System.Drawing.Point(0, 0);
            this.SYSAdminWarningLabel.Padding = new System.Windows.Forms.Padding(0, 5, 0, 0);
            this.SYSAdminWarningLabel.Name = "SYSAdminWarningLabel";
            this.SYSAdminWarningLabel.Size = new System.Drawing.Size(708, 233);
            this.SYSAdminWarningLabel.TabIndex = 15;
            this.SYSAdminWarningLabel.Text = "No data is available for this view.";
            this.SYSAdminWarningLabel.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            // 
            // refreshToolStripLabel
            // 
            this.refreshToolStripLabel.Alignment = System.Windows.Forms.ToolStripItemAlignment.Right;
            this.refreshToolStripLabel.Font = new System.Drawing.Font("Arial", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.refreshToolStripLabel.ForeColor = System.Drawing.Color.Black;
            this.refreshToolStripLabel.Name = "refreshToolStripLabel";
            this.refreshToolStripLabel.Size = new System.Drawing.Size(179, 20);
            this.refreshToolStripLabel.Text = "Refresh every                    seconds";
            // 
            // _SessionTraceDialog_Toolbars_Dock_Area_Top
            // 
            this._SessionTraceDialog_Toolbars_Dock_Area_Top.AccessibleRole = System.Windows.Forms.AccessibleRole.Grouping;
            this._SessionTraceDialog_Toolbars_Dock_Area_Top.BackColor = System.Drawing.SystemColors.Control;
            this._SessionTraceDialog_Toolbars_Dock_Area_Top.DockedPosition = Infragistics.Win.UltraWinToolbars.DockedPosition.Top;
            this._SessionTraceDialog_Toolbars_Dock_Area_Top.ForeColor = System.Drawing.SystemColors.ControlText;
            this._SessionTraceDialog_Toolbars_Dock_Area_Top.Location = new System.Drawing.Point(0, 0);
            this._SessionTraceDialog_Toolbars_Dock_Area_Top.Name = "_SessionTraceDialog_Toolbars_Dock_Area_Top";
            this._SessionTraceDialog_Toolbars_Dock_Area_Top.Size = new System.Drawing.Size(691, 0);
            this._SessionTraceDialog_Toolbars_Dock_Area_Top.ToolbarsManager = this.toolbarsManager;
            // 
            // _SessionTraceDialog_Toolbars_Dock_Area_Bottom
            // 
            this._SessionTraceDialog_Toolbars_Dock_Area_Bottom.AccessibleRole = System.Windows.Forms.AccessibleRole.Grouping;
            this._SessionTraceDialog_Toolbars_Dock_Area_Bottom.BackColor = System.Drawing.SystemColors.Control;
            this._SessionTraceDialog_Toolbars_Dock_Area_Bottom.DockedPosition = Infragistics.Win.UltraWinToolbars.DockedPosition.Bottom;
            this._SessionTraceDialog_Toolbars_Dock_Area_Bottom.ForeColor = System.Drawing.SystemColors.ControlText;
            this._SessionTraceDialog_Toolbars_Dock_Area_Bottom.Location = new System.Drawing.Point(0, 513);
            this._SessionTraceDialog_Toolbars_Dock_Area_Bottom.Name = "_SessionTraceDialog_Toolbars_Dock_Area_Bottom";
            this._SessionTraceDialog_Toolbars_Dock_Area_Bottom.Size = new System.Drawing.Size(691, 0);
            this._SessionTraceDialog_Toolbars_Dock_Area_Bottom.ToolbarsManager = this.toolbarsManager;
            // 
            // SessionTraceDialog
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoScroll = true;
            this.AutoScrollMinSize = new System.Drawing.Size(690, 500);
            this.BackColor = System.Drawing.SystemColors.Control;
            this.ClientSize = new System.Drawing.Size(691, 536);
            this.Controls.Add(this.autoRefreshSpinner);
            this.Controls.Add(this.contentPanel);
            this.Controls.Add(this._SessionTraceDialog_Toolbars_Dock_Area_Left);
            this.Controls.Add(this._SessionTraceDialog_Toolbars_Dock_Area_Right);
            this.Controls.Add(this.sessionTraceHeaderStrip);
            this.Controls.Add(this._SessionTraceDialog_Toolbars_Dock_Area_Top);
            this.Controls.Add(this._SessionTraceDialog_Toolbars_Dock_Area_Bottom);
            this.Controls.Add(this.statusBar);
            this.Controls.Add(this.SYSAdminWarningLabel);
            this.HelpButton = true;
            this.KeyPreview = true;
            this.MinimumSize = new System.Drawing.Size(300, 240);
            this.Name = "SessionTraceDialog";
            this.Text = "Session Trace";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.SessionTraceDialog_FormClosing);
            this.Load += new System.EventHandler(this.SessionTraceDialog_Load);
            this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.SessionTraceDialog_KeyDown);
            ((System.ComponentModel.ISupportInitialize)(this.traceGridDataSource)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.statusBar)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.traceGrid)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.toolbarsManager)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.autoRefreshSpinner)).EndInit();
            this.contentPanel.ResumeLayout(false);
            this.contentPanel.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.durationSpinner)).EndInit();
            this.statusPulseHeaderPanel.ResumeLayout(false);
            this.summaryGroupBox.ResumeLayout(false);
            this.summaryGroupBox.PerformLayout();
            this.sessionTraceHeaderStrip.ResumeLayout(false);
            this.sessionTraceHeaderStrip.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private Infragistics.Win.UltraWinDataSource.UltraDataSource traceGridDataSource;
        private Infragistics.Win.UltraWinGrid.UltraGrid traceGrid;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomLabel cpuTimeLabel;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomLabel rowCountLabel;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomLabel readsLabel;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomLabel cursorFetchStatusLabel;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomLabel cursorSetRowsLabel;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomLabel writesLabel;
        private Idera.SQLdm.DesktopClient.Controls.HeaderStrip sessionTraceHeaderStrip;
        private System.Windows.Forms.ToolStripButton refreshToolStripButton;
        private System.Windows.Forms.ToolStripButton killSessionToolStripButton;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomLabel lockWaitLabel;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomLabel textSizeLabel;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomLabel deadlockPriorityLabel;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomLabel nestingLevelLabel;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomLabel lineNumberLabel;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomLabel openTransactionslabel;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomLabel optionsLabel;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomLabel isoLevelLabel;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomLabel languagelabel;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomLabel commandLabel;
        private System.Windows.Forms.Timer automatedRefreshTimer;
        private System.Windows.Forms.ToolStripButton startTraceToolStripButton;
        private System.Windows.Forms.ToolStripButton stopTraceToolStripButton;
        private System.Windows.Forms.ToolStripButton clearTraceToolStripButton;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomGroupBox summaryGroupBox;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomLabel gridLabel;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomLabel durationLabel;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomLabel lastCommandLabel;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomLabel lastErrorLabel;
        private System.Windows.Forms.ToolStripLabel refreshToolStripLabel;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomLabel SYSAdminWarningLabel;
        private Idera.SQLdm.DesktopClient.Controls.PulseHeaderPanel statusPulseHeaderPanel;
        private Infragistics.Win.UltraWinStatusBar.UltraStatusBar statusBar;
        private Infragistics.Win.UltraWinGrid.UltraGridPrintDocument ultraGridPrintDocument;
        private Infragistics.Win.Printing.UltraPrintPreviewDialog ultraPrintPreviewDialog;
        private System.Windows.Forms.SaveFileDialog saveFileDialog;
        private Infragistics.Win.UltraWinGrid.ExcelExport.UltraGridExcelExporter ultraGridExcelExporter;
        private Idera.SQLdm.DesktopClient.Controls.ContextMenuManager toolbarsManager;
        private Infragistics.Win.UltraWinToolbars.UltraToolbarsDockArea _SessionTraceDialog_Toolbars_Dock_Area_Left;
        private Infragistics.Win.UltraWinToolbars.UltraToolbarsDockArea _SessionTraceDialog_Toolbars_Dock_Area_Right;
        private Infragistics.Win.UltraWinToolbars.UltraToolbarsDockArea _SessionTraceDialog_Toolbars_Dock_Area_Top;
        private Infragistics.Win.UltraWinToolbars.UltraToolbarsDockArea _SessionTraceDialog_Toolbars_Dock_Area_Bottom;
        private Idera.SQLdm.DesktopClient.Controls.GradientPanel contentPanel;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomTextBox cpuTimeTextBox;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomTextBox rowCountTextBox;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomTextBox writesTextBox;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomTextBox readsTextBox;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomTextBox openTransactionsTextBox;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomTextBox cursorSetRowsTextBox;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomTextBox cursorFetchStatusTextBox;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomTextBox deadlockPriorityTextBox;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomTextBox languageTextBox;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomTextBox lockWaitTextBox;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomTextBox nestingLevelTextBox;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomTextBox textSizeTextBox;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomTextBox lineNumberTextBox;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomTextBox lastErrorNumberTextBox;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomTextBox commandTextBox;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomTextBox isolationLevelTextBox;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomLabel statusValueLabel;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomTextBox optionsTextBox;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomTextBox lastCommandTextBox;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomNumericUpDown autoRefreshSpinner;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomNumericUpDown durationSpinner;
    }
}
