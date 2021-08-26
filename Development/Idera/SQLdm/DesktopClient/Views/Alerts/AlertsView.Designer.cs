namespace Idera.SQLdm.DesktopClient.Views.Alerts
{
    partial class AlertsView
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
            System.Diagnostics.Stopwatch stopWatch = new System.Diagnostics.Stopwatch();
            stopWatch.Start();
            this.components = new System.ComponentModel.Container();
            Infragistics.Win.Appearance appearance1 = new Infragistics.Win.Appearance();
            Infragistics.Win.UltraWinGrid.UltraGridBand ultraGridBand1 = new Infragistics.Win.UltraWinGrid.UltraGridBand("Band 0", -1);
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn1 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("AlertID");
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn2 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("UTCOccurrenceDateTime", -1, null, 0, Infragistics.Win.UltraWinGrid.SortIndicator.Descending, false);
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn3 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("ServerName");
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn4 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("DatabaseName");
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn5 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("TableName");
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn6 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("Active", -1, 84474157);
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn7 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("Metric", -1, 29722407);
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn8 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("Severity", -1, 29767626);
            Infragistics.Win.Appearance appearance30 = new Infragistics.Win.Appearance();
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn9 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("StateEvent", -1, 17251657);
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn10 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("Value");
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn11 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("Heading");
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn12 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("Message");
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn13 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("ServerType");
            Infragistics.Win.Appearance appearance3 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance4 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance5 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance6 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance7 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance8 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance9 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance10 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance11 = new Infragistics.Win.Appearance();
            Infragistics.Win.ValueList valueList1 = new Infragistics.Win.ValueList(29722407);
            Infragistics.Win.ValueList valueList2 = new Infragistics.Win.ValueList(29767626);
            Infragistics.Win.Appearance appearance12 = new Infragistics.Win.Appearance();
            Infragistics.Win.ValueList valueList3 = new Infragistics.Win.ValueList(17251657);
            Infragistics.Win.ValueList valueList4 = new Infragistics.Win.ValueList(84474157);
            Infragistics.Win.ValueListItem valueListItem1 = new Infragistics.Win.ValueListItem();
            Infragistics.Win.ValueListItem valueListItem2 = new Infragistics.Win.ValueListItem();
            Infragistics.Win.UltraWinDataSource.UltraDataColumn ultraDataColumn1 = new Infragistics.Win.UltraWinDataSource.UltraDataColumn("AlertID");
            Infragistics.Win.UltraWinDataSource.UltraDataColumn ultraDataColumn2 = new Infragistics.Win.UltraWinDataSource.UltraDataColumn("UTCOccurrenceDateTime");
            Infragistics.Win.UltraWinDataSource.UltraDataColumn ultraDataColumn3 = new Infragistics.Win.UltraWinDataSource.UltraDataColumn("ServerName");
            Infragistics.Win.UltraWinDataSource.UltraDataColumn ultraDataColumn4 = new Infragistics.Win.UltraWinDataSource.UltraDataColumn("DatabaseName");
            Infragistics.Win.UltraWinDataSource.UltraDataColumn ultraDataColumn5 = new Infragistics.Win.UltraWinDataSource.UltraDataColumn("TableName");
            Infragistics.Win.UltraWinDataSource.UltraDataColumn ultraDataColumn6 = new Infragistics.Win.UltraWinDataSource.UltraDataColumn("Active");
            Infragistics.Win.UltraWinDataSource.UltraDataColumn ultraDataColumn7 = new Infragistics.Win.UltraWinDataSource.UltraDataColumn("Metric");
            Infragistics.Win.UltraWinDataSource.UltraDataColumn ultraDataColumn8 = new Infragistics.Win.UltraWinDataSource.UltraDataColumn("Severity");
            Infragistics.Win.UltraWinDataSource.UltraDataColumn ultraDataColumn9 = new Infragistics.Win.UltraWinDataSource.UltraDataColumn("StateEvent");
            Infragistics.Win.UltraWinDataSource.UltraDataColumn ultraDataColumn10 = new Infragistics.Win.UltraWinDataSource.UltraDataColumn("Value");
            Infragistics.Win.UltraWinDataSource.UltraDataColumn ultraDataColumn11 = new Infragistics.Win.UltraWinDataSource.UltraDataColumn("Heading");
            Infragistics.Win.UltraWinDataSource.UltraDataColumn ultraDataColumn12 = new Infragistics.Win.UltraWinDataSource.UltraDataColumn("Message");
            Infragistics.Win.UltraWinDataSource.UltraDataColumn ultraDataColumn13 = new Infragistics.Win.UltraWinDataSource.UltraDataColumn("ServerType");
            Infragistics.Win.UltraWinEditors.DropDownEditorButton dropDownEditorButton1 = new Infragistics.Win.UltraWinEditors.DropDownEditorButton("DropDownList");
            Infragistics.Win.UltraWinEditors.DropDownEditorButton dropDownEditorButton2 = new Infragistics.Win.UltraWinEditors.DropDownEditorButton("DropDownList");
            Infragistics.Win.UltraWinSchedule.CalendarCombo.DateButton dateButton1 = new Infragistics.Win.UltraWinSchedule.CalendarCombo.DateButton();
            Infragistics.Win.UltraWinSchedule.CalendarCombo.DateButton dateButton2 = new Infragistics.Win.UltraWinSchedule.CalendarCombo.DateButton();
            Infragistics.Win.UltraWinToolbars.PopupMenuTool popupMenuTool1 = new Infragistics.Win.UltraWinToolbars.PopupMenuTool("columnContextMenu");
            Infragistics.Win.UltraWinToolbars.ButtonTool buttonTool1 = new Infragistics.Win.UltraWinToolbars.ButtonTool("sortAscendingButton");
            Infragistics.Win.UltraWinToolbars.ButtonTool buttonTool2 = new Infragistics.Win.UltraWinToolbars.ButtonTool("sortDescendingButton");
            Infragistics.Win.UltraWinToolbars.StateButtonTool stateButtonTool1 = new Infragistics.Win.UltraWinToolbars.StateButtonTool("groupByThisColumnButton", "");
            Infragistics.Win.UltraWinToolbars.ButtonTool buttonTool3 = new Infragistics.Win.UltraWinToolbars.ButtonTool("toggleGroupByBoxButton");
            Infragistics.Win.UltraWinToolbars.ButtonTool buttonTool4 = new Infragistics.Win.UltraWinToolbars.ButtonTool("removeThisColumnButton");
            Infragistics.Win.UltraWinToolbars.ButtonTool buttonTool5 = new Infragistics.Win.UltraWinToolbars.ButtonTool("showColumnChooserButton");
            Infragistics.Win.UltraWinToolbars.ButtonTool buttonTool6 = new Infragistics.Win.UltraWinToolbars.ButtonTool("toggleColumnTextButton");
            Infragistics.Win.UltraWinToolbars.ButtonTool buttonTool7 = new Infragistics.Win.UltraWinToolbars.ButtonTool("showColumnChooserButton");
            Infragistics.Win.Appearance appearance13 = new Infragistics.Win.Appearance();
            Infragistics.Win.UltraWinToolbars.ButtonTool buttonTool8 = new Infragistics.Win.UltraWinToolbars.ButtonTool("toggleGroupByBoxButton");
            Infragistics.Win.Appearance appearance14 = new Infragistics.Win.Appearance();
            Infragistics.Win.UltraWinToolbars.ButtonTool buttonTool9 = new Infragistics.Win.UltraWinToolbars.ButtonTool("sortAscendingButton");
            Infragistics.Win.Appearance appearance15 = new Infragistics.Win.Appearance();
            Infragistics.Win.UltraWinToolbars.ButtonTool buttonTool10 = new Infragistics.Win.UltraWinToolbars.ButtonTool("sortDescendingButton");
            Infragistics.Win.Appearance appearance16 = new Infragistics.Win.Appearance();
            Infragistics.Win.UltraWinToolbars.ButtonTool buttonTool11 = new Infragistics.Win.UltraWinToolbars.ButtonTool("removeThisColumnButton");
            Infragistics.Win.UltraWinToolbars.ButtonTool buttonTool12 = new Infragistics.Win.UltraWinToolbars.ButtonTool("toggleColumnTextButton");
            Infragistics.Win.UltraWinToolbars.StateButtonTool stateButtonTool2 = new Infragistics.Win.UltraWinToolbars.StateButtonTool("groupByThisColumnButton", "");
            Infragistics.Win.UltraWinToolbars.PopupMenuTool popupMenuTool2 = new Infragistics.Win.UltraWinToolbars.PopupMenuTool("gridContextMenu");
            Infragistics.Win.UltraWinToolbars.ButtonTool buttonTool40 = new Infragistics.Win.UltraWinToolbars.ButtonTool("viewDeadlockDetailsButton");
            Infragistics.Win.UltraWinToolbars.ButtonTool buttonTool42 = new Infragistics.Win.UltraWinToolbars.ButtonTool("viewBlockDetailsButton");
            Infragistics.Win.UltraWinToolbars.ButtonTool buttonTool33 = new Infragistics.Win.UltraWinToolbars.ButtonTool("viewRealTimeSnapshotButton");
            Infragistics.Win.UltraWinToolbars.ButtonTool buttonTool34 = new Infragistics.Win.UltraWinToolbars.ButtonTool("viewHistoricalSnapshotButton");
            Infragistics.Win.UltraWinToolbars.ButtonTool buttonTool38 = new Infragistics.Win.UltraWinToolbars.ButtonTool("viewAlertHelpButton");
            Infragistics.Win.UltraWinToolbars.ButtonTool buttonTool13 = new Infragistics.Win.UltraWinToolbars.ButtonTool("showDetailsButton");
            Infragistics.Win.UltraWinToolbars.ButtonTool buttonTool14 = new Infragistics.Win.UltraWinToolbars.ButtonTool("clearAlertButton");
            Infragistics.Win.UltraWinToolbars.ButtonTool buttonTool15 = new Infragistics.Win.UltraWinToolbars.ButtonTool("clearAllAlertsButton");
            Infragistics.Win.UltraWinToolbars.ButtonTool buttonTool16 = new Infragistics.Win.UltraWinToolbars.ButtonTool("editAlertConfigurationButton");
            Infragistics.Win.UltraWinToolbars.ButtonTool buttonTool36 = new Infragistics.Win.UltraWinToolbars.ButtonTool("snoozeAlertButton");
            Infragistics.Win.UltraWinToolbars.ButtonTool buttonTool17 = new Infragistics.Win.UltraWinToolbars.ButtonTool("collapseAllGroupsButton");
            Infragistics.Win.UltraWinToolbars.ButtonTool buttonTool18 = new Infragistics.Win.UltraWinToolbars.ButtonTool("expandAllGroupsButton");
            Infragistics.Win.UltraWinToolbars.ButtonTool buttonTool19 = new Infragistics.Win.UltraWinToolbars.ButtonTool("printGridButton");
            Infragistics.Win.UltraWinToolbars.ButtonTool buttonTool20 = new Infragistics.Win.UltraWinToolbars.ButtonTool("exportGridButton");
            Infragistics.Win.UltraWinToolbars.ButtonTool buttonTool21 = new Infragistics.Win.UltraWinToolbars.ButtonTool("copyToClipboardButton");
            Infragistics.Win.UltraWinToolbars.ButtonTool buttonTool22 = new Infragistics.Win.UltraWinToolbars.ButtonTool("printGridButton");
            Infragistics.Win.Appearance appearance17 = new Infragistics.Win.Appearance();
            Infragistics.Win.UltraWinToolbars.ButtonTool buttonTool23 = new Infragistics.Win.UltraWinToolbars.ButtonTool("exportGridButton");
            Infragistics.Win.Appearance appearance18 = new Infragistics.Win.Appearance();
            Infragistics.Win.UltraWinToolbars.ButtonTool buttonTool24 = new Infragistics.Win.UltraWinToolbars.ButtonTool("editAlertConfigurationButton");
            Infragistics.Win.UltraWinToolbars.ButtonTool buttonTool25 = new Infragistics.Win.UltraWinToolbars.ButtonTool("showDetailsButton");
            Infragistics.Win.UltraWinToolbars.ButtonTool buttonTool26 = new Infragistics.Win.UltraWinToolbars.ButtonTool("collapseAllGroupsButton");
            Infragistics.Win.UltraWinToolbars.ButtonTool buttonTool27 = new Infragistics.Win.UltraWinToolbars.ButtonTool("expandAllGroupsButton");
            Infragistics.Win.UltraWinToolbars.ButtonTool buttonTool28 = new Infragistics.Win.UltraWinToolbars.ButtonTool("copyToClipboardButton");
            Infragistics.Win.Appearance appearance19 = new Infragistics.Win.Appearance();
            Infragistics.Win.UltraWinToolbars.ButtonTool buttonTool29 = new Infragistics.Win.UltraWinToolbars.ButtonTool("clearAlertButton");
            Infragistics.Win.UltraWinToolbars.ButtonTool buttonTool30 = new Infragistics.Win.UltraWinToolbars.ButtonTool("clearAllAlertsButton");
            Infragistics.Win.UltraWinToolbars.ButtonTool buttonTool31 = new Infragistics.Win.UltraWinToolbars.ButtonTool("viewRealTimeSnapshotButton");
            Infragistics.Win.Appearance appearance31 = new Infragistics.Win.Appearance();
            Infragistics.Win.UltraWinToolbars.ButtonTool buttonTool32 = new Infragistics.Win.UltraWinToolbars.ButtonTool("viewHistoricalSnapshotButton");
            Infragistics.Win.UltraWinToolbars.ButtonTool buttonTool35 = new Infragistics.Win.UltraWinToolbars.ButtonTool("snoozeAlertButton");
            Infragistics.Win.UltraWinToolbars.ButtonTool buttonTool37 = new Infragistics.Win.UltraWinToolbars.ButtonTool("viewAlertHelpButton");
            Infragistics.Win.UltraWinToolbars.ButtonTool buttonTool39 = new Infragistics.Win.UltraWinToolbars.ButtonTool("viewDeadlockDetailsButton");
            Infragistics.Win.UltraWinToolbars.ButtonTool buttonTool41 = new Infragistics.Win.UltraWinToolbars.ButtonTool("viewBlockDetailsButton");
            this.contentPanel = new System.Windows.Forms.Panel();
            this.splitContainer = new System.Windows.Forms.SplitContainer();
            this.gridPanel = new System.Windows.Forms.Panel();
            this.alertsGrid = new Infragistics.Win.UltraWinGrid.UltraGrid();
            this.alertsViewDataSource = new Idera.SQLdm.DesktopClient.Views.Alerts.AlertsViewDataSource();
            this.alertsGridStatusLabel = new System.Windows.Forms.Label();
            this.detailsPanel = new System.Windows.Forms.Panel();
            this.detailsContentPanel = new System.Windows.Forms.TableLayoutPanel();
            this.showDetailsLinkLabel = new System.Windows.Forms.LinkLabel();
            this.helpHistoryLinkLabel1 = new System.Windows.Forms.LinkLabel();
            this.headerLabel = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.label8 = new System.Windows.Forms.Label();
            this.label10 = new System.Windows.Forms.Label();
            this.panel1 = new System.Windows.Forms.Panel();
            this.severityLabel = new System.Windows.Forms.Label();
            this.severityImage = new System.Windows.Forms.PictureBox();
            this.label7 = new System.Windows.Forms.Label();
            this.panel2 = new System.Windows.Forms.Panel();
            this.transitionLabel = new System.Windows.Forms.Label();
            this.transitionImage = new System.Windows.Forms.PictureBox();
            this.label11 = new System.Windows.Forms.Label();
            this.metricLabel = new System.Windows.Forms.Label();
            this.messageTextBox = new System.Windows.Forms.TextBox();
            this.showRealTimeViewLinkLabel = new System.Windows.Forms.LinkLabel();
            this.helpHistoryLinkLabel2 = new System.Windows.Forms.LinkLabel();
            this.label9 = new System.Windows.Forms.Label();
            this.detailsHeaderStrip = new Idera.SQLdm.DesktopClient.Controls.HeaderStrip();
            this.detailsHeaderStripLabel = new System.Windows.Forms.ToolStripLabel();
            this.toggleDetailsPanelButton = new System.Windows.Forms.ToolStripButton();
            this.noSelectionLabel = new System.Windows.Forms.Label();
            this.filterOptionsPanel = new System.Windows.Forms.Panel();
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.clearButton = new Infragistics.Win.Misc.UltraButton();
            this.applyButton = new Infragistics.Win.Misc.UltraButton();
            this.dividerVert = new System.Windows.Forms.Label();
            this.beginTimeCombo = new Idera.SQLdm.Common.UI.Controls.TimeComboEditor();
            this.lblTo = new System.Windows.Forms.Label();
            this.endTimeCombo = new Idera.SQLdm.Common.UI.Controls.TimeComboEditor();
            this.lblFrom = new System.Windows.Forms.Label();
            this.endDateCombo = new Infragistics.Win.UltraWinSchedule.UltraCalendarCombo();
            this.dividerLabel = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.tagCombo = new Infragistics.Win.UltraWinEditors.UltraComboEditor();
            this.beginDateCombo = new Infragistics.Win.UltraWinSchedule.UltraCalendarCombo();
            this.serverLabel = new System.Windows.Forms.Label();
            this.ServerTypelabel = new System.Windows.Forms.Label();
            this.ServerTypeText=new System.Windows.Forms.Label();
            this.serverCombo = new Infragistics.Win.UltraWinEditors.UltraComboEditor();
            this.metricFilterLabel = new System.Windows.Forms.Label();
            this.metricCombo = new Infragistics.Win.UltraWinEditors.UltraComboEditor();
            this.severityFilterLabel = new System.Windows.Forms.Label();
            this.severityCombo = new Infragistics.Win.UltraWinEditors.UltraComboEditor();
            this.rdbtnActiveOnly = new System.Windows.Forms.RadioButton();
            this.rdbtnTimeSpan = new System.Windows.Forms.RadioButton();
            this.dividerLabel1 = new System.Windows.Forms.Label();
            this.toolbarsManager = new Idera.SQLdm.DesktopClient.Controls.ContextMenuManager(this.components);
            this.headerStrip = new Idera.SQLdm.DesktopClient.Controls.HeaderStrip();
            this.titleLabel = new System.Windows.Forms.ToolStripLabel();
            this.toggleFilterOptionsPanelButton = new System.Windows.Forms.ToolStripButton();
            this.filterAppliedLabel = new System.Windows.Forms.ToolStripLabel();
            this.ultraGridExcelExporter = new Infragistics.Win.UltraWinGrid.ExcelExport.UltraGridExcelExporter(this.components);
            this.saveFileDialog = new System.Windows.Forms.SaveFileDialog();
            this.ultraGridPrintDocument = new Infragistics.Win.UltraWinGrid.UltraGridPrintDocument(this.components);
            this.ultraPrintPreviewDialog = new Infragistics.Win.Printing.UltraPrintPreviewDialog(this.components);
            this._AlertsView_Toolbars_Dock_Area_Left = new Infragistics.Win.UltraWinToolbars.UltraToolbarsDockArea();
            this._AlertsView_Toolbars_Dock_Area_Right = new Infragistics.Win.UltraWinToolbars.UltraToolbarsDockArea();
            this._AlertsView_Toolbars_Dock_Area_Top = new Infragistics.Win.UltraWinToolbars.UltraToolbarsDockArea();
            this._AlertsView_Toolbars_Dock_Area_Bottom = new Infragistics.Win.UltraWinToolbars.UltraToolbarsDockArea();
            this.contentPanel.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer)).BeginInit();
            this.splitContainer.Panel1.SuspendLayout();
            this.splitContainer.Panel2.SuspendLayout();
            this.splitContainer.SuspendLayout();
            this.gridPanel.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.alertsGrid)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.alertsViewDataSource)).BeginInit();
            this.detailsPanel.SuspendLayout();
            this.detailsContentPanel.SuspendLayout();
            this.panel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.severityImage)).BeginInit();
            this.panel2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.transitionImage)).BeginInit();
            this.detailsHeaderStrip.SuspendLayout();
            this.filterOptionsPanel.SuspendLayout();
            this.tableLayoutPanel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.beginTimeCombo)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.endTimeCombo)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.endDateCombo)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.tagCombo)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.beginDateCombo)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.serverCombo)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.metricCombo)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.severityCombo)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.toolbarsManager)).BeginInit();
            this.headerStrip.SuspendLayout();
            this.SuspendLayout();
            // 
            // contentPanel
            // 
            this.contentPanel.AutoScroll = true;
            this.contentPanel.Controls.Add(this.splitContainer);
            this.contentPanel.Controls.Add(this.filterOptionsPanel);
            this.contentPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.contentPanel.Location = new System.Drawing.Point(0, 25);
            this.contentPanel.Name = "contentPanel";
            this.contentPanel.Size = new System.Drawing.Size(761, 575);
            this.contentPanel.TabIndex = 1;
            // 
            // splitContainer
            // 
            this.splitContainer.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer.Location = new System.Drawing.Point(0, 162);
            this.splitContainer.MinimumSize = new System.Drawing.Size(0, 128);
            this.splitContainer.Name = "splitContainer";
            this.splitContainer.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // splitContainer.Panel1
            // 
            this.splitContainer.Panel1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(203)))), ((int)(((byte)(203)))), ((int)(((byte)(203)))));
            this.splitContainer.Panel1.Controls.Add(this.gridPanel);
            this.splitContainer.Panel1.Controls.Add(this.alertsGridStatusLabel);
            this.splitContainer.Panel1.Padding = new System.Windows.Forms.Padding(0, 0, 0, 1);
            // 
            // splitContainer.Panel2
            // 
            this.splitContainer.Panel2.Controls.Add(this.detailsPanel);
            this.splitContainer.Size = new System.Drawing.Size(761, 413);
            this.splitContainer.SplitterDistance = 200;
            this.splitContainer.TabIndex = 8;
            this.splitContainer.MouseDown += new System.Windows.Forms.MouseEventHandler(this.splitContainer1_MouseDown);
            this.splitContainer.MouseUp += new System.Windows.Forms.MouseEventHandler(this.splitContainer1_MouseUp);
            // 
            // gridPanel
            // 
            this.gridPanel.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(203)))), ((int)(((byte)(203)))), ((int)(((byte)(203)))));
            this.gridPanel.Controls.Add(this.alertsGrid);
            this.gridPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.gridPanel.Location = new System.Drawing.Point(0, 0);
            this.gridPanel.Name = "gridPanel";
            this.gridPanel.Size = new System.Drawing.Size(761, 279);
            this.gridPanel.TabIndex = 3;
            // 
            // alertsGrid
            // 
            this.toolbarsManager.SetContextMenuUltra(this.alertsGrid, "gridContextMenu");
            this.alertsGrid.DataMember = "Band 0";
            this.alertsGrid.DataSource = this.alertsViewDataSource;
            appearance1.BackColor = System.Drawing.SystemColors.Window;
            appearance1.BorderColor = System.Drawing.SystemColors.InactiveCaption;
            this.alertsGrid.DisplayLayout.Appearance = appearance1;
            this.alertsGrid.DisplayLayout.AutoFitStyle = Infragistics.Win.UltraWinGrid.AutoFitStyle.ExtendLastColumn;
            ultraGridColumn1.AutoCompleteMode = Infragistics.Win.AutoCompleteMode.Append;
            ultraGridColumn1.ExcludeFromColumnChooser = Infragistics.Win.UltraWinGrid.ExcludeFromColumnChooser.True;
            ultraGridColumn1.Header.VisiblePosition = 12;
            ultraGridColumn1.Hidden = true;
            ultraGridColumn1.Width = 75;
            ultraGridColumn2.AutoCompleteMode = Infragistics.Win.AutoCompleteMode.Append;
            ultraGridColumn2.AutoSizeMode = Infragistics.Win.UltraWinGrid.ColumnAutoSizeMode.None;
            ultraGridColumn2.Format = "G";
            ultraGridColumn2.GroupByMode = Infragistics.Win.UltraWinGrid.GroupByMode.OutlookDate;
            ultraGridColumn2.Header.Caption = "Time";
            ultraGridColumn2.Header.VisiblePosition = 3;
            ultraGridColumn2.Style = Infragistics.Win.UltraWinGrid.ColumnStyle.DateTime;
            ultraGridColumn2.Width = 131;
            ultraGridColumn3.AutoCompleteMode = Infragistics.Win.AutoCompleteMode.Append;
            ultraGridColumn3.AutoSizeMode = Infragistics.Win.UltraWinGrid.ColumnAutoSizeMode.None;
            ultraGridColumn3.Header.Caption = "Instance";
            ultraGridColumn3.Header.VisiblePosition = 4;
            ultraGridColumn3.Width = 179;
            ultraGridColumn13.AutoCompleteMode = Infragistics.Win.AutoCompleteMode.Append;
            ultraGridColumn13.AutoSizeMode = Infragistics.Win.UltraWinGrid.ColumnAutoSizeMode.None;
            ultraGridColumn13.Header.Caption = "Server Type";
            ultraGridColumn13.Header.VisiblePosition = 5;
            ultraGridColumn13.Width = 179;
            ultraGridColumn4.AutoCompleteMode = Infragistics.Win.AutoCompleteMode.Append;
            ultraGridColumn4.Header.Caption = "Database";
            ultraGridColumn4.Header.VisiblePosition = 7;
            ultraGridColumn4.Hidden = true;
            ultraGridColumn4.Width = 122;
            ultraGridColumn5.AutoCompleteMode = Infragistics.Win.AutoCompleteMode.Append;
            ultraGridColumn5.Header.VisiblePosition = 9;
            ultraGridColumn5.Hidden = true;
            ultraGridColumn5.Width = 131;
            ultraGridColumn6.AutoCompleteMode = Infragistics.Win.AutoCompleteMode.Append;
            ultraGridColumn6.ExcludeFromColumnChooser = Infragistics.Win.UltraWinGrid.ExcludeFromColumnChooser.False;
            ultraGridColumn6.Header.VisiblePosition = 2;
            ultraGridColumn6.Hidden = true;
            ultraGridColumn6.Width = 50;
            ultraGridColumn7.AutoCompleteMode = Infragistics.Win.AutoCompleteMode.Append;
            ultraGridColumn7.CellDisplayStyle = Infragistics.Win.UltraWinGrid.CellDisplayStyle.FormattedText;
            ultraGridColumn7.Header.VisiblePosition = 8;
            ultraGridColumn7.Hidden = true;
            ultraGridColumn7.Width = 52;
            ultraGridColumn8.AutoCompleteMode = Infragistics.Win.AutoCompleteMode.Append;
            ultraGridColumn8.ColumnChooserCaption = "Severity";
            ultraGridColumn8.ExcludeFromColumnChooser = Infragistics.Win.UltraWinGrid.ExcludeFromColumnChooser.False;
            appearance30.FontData.BoldAsString = "True";
            appearance30.ForeColor = System.Drawing.Color.Red;
            appearance30.TextHAlignAsString = "Center";
            ultraGridColumn8.Header.Appearance = appearance30;
            ultraGridColumn8.Header.Caption = "!";
            ultraGridColumn8.Header.VisiblePosition = 0;
            ultraGridColumn8.Width = 20;
            ultraGridColumn9.AutoCompleteMode = Infragistics.Win.AutoCompleteMode.Append;
            ultraGridColumn9.Header.Caption = "Change";
            ultraGridColumn9.Header.VisiblePosition = 1;
            ultraGridColumn9.Width = 134;
            ultraGridColumn10.AutoCompleteMode = Infragistics.Win.AutoCompleteMode.Append;
            ultraGridColumn10.ExcludeFromColumnChooser = Infragistics.Win.UltraWinGrid.ExcludeFromColumnChooser.True;
            ultraGridColumn10.Header.VisiblePosition = 10;
            ultraGridColumn10.Hidden = true;
            ultraGridColumn10.Width = 64;
            ultraGridColumn11.AutoCompleteMode = Infragistics.Win.AutoCompleteMode.Append;
            ultraGridColumn11.Header.Caption = "Summary";
            ultraGridColumn11.Header.VisiblePosition = 6;
            ultraGridColumn11.Width = 411;
            ultraGridColumn12.AutoCompleteMode = Infragistics.Win.AutoCompleteMode.Append;
            ultraGridColumn12.Header.Caption = "Details";
            ultraGridColumn12.Header.VisiblePosition = 11;
            ultraGridColumn12.Hidden = true;
            ultraGridColumn12.Width = 95;
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
            ultraGridColumn11,
            ultraGridColumn12,
            ultraGridColumn13});
            this.alertsGrid.DisplayLayout.BandsSerializer.Add(ultraGridBand1);
            this.alertsGrid.DisplayLayout.BorderStyle = Infragistics.Win.UIElementBorderStyle.None;
            this.alertsGrid.DisplayLayout.CaptionVisible = Infragistics.Win.DefaultableBoolean.False;
            appearance3.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(235)))), ((int)(((byte)(235)))), ((int)(((byte)(235)))));
            appearance3.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(140)))), ((int)(((byte)(140)))), ((int)(((byte)(140)))));
            this.alertsGrid.DisplayLayout.GroupByBox.Appearance = appearance3;
            appearance4.ForeColor = System.Drawing.SystemColors.GrayText;
            this.alertsGrid.DisplayLayout.GroupByBox.BandLabelAppearance = appearance4;
            this.alertsGrid.DisplayLayout.GroupByBox.BorderStyle = Infragistics.Win.UIElementBorderStyle.Solid;
            this.alertsGrid.DisplayLayout.GroupByBox.Hidden = true;
            appearance5.BackColor = System.Drawing.SystemColors.ControlLightLight;
            appearance5.BackColor2 = System.Drawing.SystemColors.Control;
            appearance5.BackGradientStyle = Infragistics.Win.GradientStyle.Horizontal;
            appearance5.ForeColor = System.Drawing.SystemColors.GrayText;
            this.alertsGrid.DisplayLayout.GroupByBox.PromptAppearance = appearance5;
            this.alertsGrid.DisplayLayout.LoadStyle = Infragistics.Win.UltraWinGrid.LoadStyle.LoadOnDemand;
            this.alertsGrid.DisplayLayout.MaxColScrollRegions = 1;
            this.alertsGrid.DisplayLayout.MaxRowScrollRegions = 1;
            this.alertsGrid.DisplayLayout.Override.AllowAddNew = Infragistics.Win.UltraWinGrid.AllowAddNew.No;
            this.alertsGrid.DisplayLayout.Override.AllowColSizing = Infragistics.Win.UltraWinGrid.AllowColSizing.Free;
            this.alertsGrid.DisplayLayout.Override.AllowDelete = Infragistics.Win.DefaultableBoolean.False;
            this.alertsGrid.DisplayLayout.Override.AllowUpdate = Infragistics.Win.DefaultableBoolean.False;
            this.alertsGrid.DisplayLayout.Override.BorderStyleCell = Infragistics.Win.UIElementBorderStyle.Solid;
            this.alertsGrid.DisplayLayout.Override.BorderStyleRow = Infragistics.Win.UIElementBorderStyle.Solid;
            appearance6.BackColor = System.Drawing.SystemColors.Window;
            this.alertsGrid.DisplayLayout.Override.CardAreaAppearance = appearance6;
            appearance7.BorderColor = System.Drawing.Color.Silver;
            appearance7.TextTrimming = Infragistics.Win.TextTrimming.EllipsisCharacter;
            this.alertsGrid.DisplayLayout.Override.CellAppearance = appearance7;
            this.alertsGrid.DisplayLayout.Override.CellClickAction = Infragistics.Win.UltraWinGrid.CellClickAction.RowSelect;
            this.alertsGrid.DisplayLayout.Override.CellPadding = 0;
            this.alertsGrid.DisplayLayout.Override.DefaultRowHeight = 20;
            this.alertsGrid.DisplayLayout.Override.FixedHeaderIndicator = Infragistics.Win.UltraWinGrid.FixedHeaderIndicator.None;
            this.alertsGrid.DisplayLayout.Override.GroupByColumnsHidden = Infragistics.Win.DefaultableBoolean.False;
            appearance8.BackColor = System.Drawing.SystemColors.Control;
            appearance8.BackColor2 = System.Drawing.SystemColors.ControlDark;
            appearance8.BackGradientAlignment = Infragistics.Win.GradientAlignment.Element;
            appearance8.BackGradientStyle = Infragistics.Win.GradientStyle.Horizontal;
            appearance8.BorderColor = System.Drawing.SystemColors.Window;
            this.alertsGrid.DisplayLayout.Override.GroupByRowAppearance = appearance8;
            this.alertsGrid.DisplayLayout.Override.GroupByRowInitialExpansionState = Infragistics.Win.UltraWinGrid.GroupByRowInitialExpansionState.Collapsed;
            appearance9.TextHAlignAsString = "Left";
            this.alertsGrid.DisplayLayout.Override.HeaderAppearance = appearance9;
            this.alertsGrid.DisplayLayout.Override.HeaderClickAction = Infragistics.Win.UltraWinGrid.HeaderClickAction.SortMulti;
            this.alertsGrid.DisplayLayout.Override.MaxSelectedRows = 1;
            appearance10.BackColor = System.Drawing.SystemColors.Window;
            appearance10.BorderColor = System.Drawing.Color.Silver;
            this.alertsGrid.DisplayLayout.Override.RowAppearance = appearance10;
            this.alertsGrid.DisplayLayout.Override.RowSelectors = Infragistics.Win.DefaultableBoolean.False;
            this.alertsGrid.DisplayLayout.Override.SelectTypeCell = Infragistics.Win.UltraWinGrid.SelectType.None;
            this.alertsGrid.DisplayLayout.Override.SelectTypeCol = Infragistics.Win.UltraWinGrid.SelectType.None;
            this.alertsGrid.DisplayLayout.Override.SelectTypeGroupByRow = Infragistics.Win.UltraWinGrid.SelectType.None;
            this.alertsGrid.DisplayLayout.Override.SelectTypeRow = Infragistics.Win.UltraWinGrid.SelectType.Single;
            appearance11.BackColor = System.Drawing.SystemColors.ControlLight;
            this.alertsGrid.DisplayLayout.Override.TemplateAddRowAppearance = appearance11;
            this.alertsGrid.DisplayLayout.ScrollBounds = Infragistics.Win.UltraWinGrid.ScrollBounds.ScrollToFill;
            this.alertsGrid.DisplayLayout.ScrollStyle = Infragistics.Win.UltraWinGrid.ScrollStyle.Immediate;
            this.alertsGrid.DisplayLayout.UseFixedHeaders = true;
            valueList1.DisplayStyle = Infragistics.Win.ValueListDisplayStyle.DisplayText;
            valueList1.Key = "Metrics";
            valueList1.PreferredDropDownSize = new System.Drawing.Size(0, 0);
            appearance12.ImageHAlign = Infragistics.Win.HAlign.Center;
            valueList2.Appearance = appearance12;
            valueList2.DisplayStyle = Infragistics.Win.ValueListDisplayStyle.Picture;
            valueList2.Key = "Severity";
            valueList2.PreferredDropDownSize = new System.Drawing.Size(0, 0);
            valueList2.SortStyle = Infragistics.Win.ValueListSortStyle.DescendingByValue;
            valueList3.DisplayStyle = Infragistics.Win.ValueListDisplayStyle.DisplayTextAndPicture;
            valueList3.Key = "Transitions";
            valueList3.PreferredDropDownSize = new System.Drawing.Size(0, 0);
            valueList3.SortStyle = Infragistics.Win.ValueListSortStyle.Ascending;
            valueList4.DisplayStyle = Infragistics.Win.ValueListDisplayStyle.DisplayText;
            valueList4.Key = "BooleanYesNo";
            valueList4.PreferredDropDownSize = new System.Drawing.Size(0, 0);
            valueListItem1.DataValue = false;
            valueListItem1.DisplayText = "No";
            valueListItem2.DataValue = true;
            valueListItem2.DisplayText = "Yes";
            valueList4.ValueListItems.AddRange(new Infragistics.Win.ValueListItem[] {
            valueListItem1,
            valueListItem2});
            this.alertsGrid.DisplayLayout.ValueLists.AddRange(new Infragistics.Win.ValueList[] {
            valueList1,
            valueList2,
            valueList3,
            valueList4});
            this.alertsGrid.DisplayLayout.ViewStyle = Infragistics.Win.UltraWinGrid.ViewStyle.SingleBand;
            this.alertsGrid.DisplayLayout.ViewStyleBand = Infragistics.Win.UltraWinGrid.ViewStyleBand.OutlookGroupBy;
            this.alertsGrid.Dock = System.Windows.Forms.DockStyle.Fill;
            this.alertsGrid.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.alertsGrid.Location = new System.Drawing.Point(0, 0);
            this.alertsGrid.Name = "alertsGrid";
            this.alertsGrid.Size = new System.Drawing.Size(761, 279);
            this.alertsGrid.TabIndex = 0;
            this.alertsGrid.InitializeGroupByRow += new Infragistics.Win.UltraWinGrid.InitializeGroupByRowEventHandler(this.alertsGrid_InitializeGroupByRow);
            this.alertsGrid.AfterRowRegionSize += new Infragistics.Win.UltraWinGrid.RowScrollRegionEventHandler(this.alertsGrid_AfterRowRegionSize);
            this.alertsGrid.AfterSelectChange += new Infragistics.Win.UltraWinGrid.AfterSelectChangeEventHandler(this.alertsGrid_AfterSelectChange);
            this.alertsGrid.SummaryValueChanged += new Infragistics.Win.UltraWinGrid.SummaryValueChangedEventHandler(this.alertsGrid_SummaryValueChanged);
            this.alertsGrid.DoubleClickRow += new Infragistics.Win.UltraWinGrid.DoubleClickRowEventHandler(this.alertsGrid_DoubleClickRow);
            this.alertsGrid.AfterSortChange += new Infragistics.Win.UltraWinGrid.BandEventHandler(this.alertsGrid_AfterSortChange);
            this.alertsGrid.MouseDown += new System.Windows.Forms.MouseEventHandler(this.alertsGrid_MouseDown);
            // 
            // alertsViewDataSource
            // 
            this.alertsViewDataSource.AllowAdd = false;
            this.alertsViewDataSource.AllowDelete = false;
            ultraDataColumn1.AllowDBNull = Infragistics.Win.DefaultableBoolean.False;
            ultraDataColumn1.DataType = typeof(long);
            ultraDataColumn2.AllowDBNull = Infragistics.Win.DefaultableBoolean.False;
            ultraDataColumn2.DataType = typeof(System.DateTime);
            ultraDataColumn2.DefaultValue = new System.DateTime(((long)(0)));
            ultraDataColumn3.AllowDBNull = Infragistics.Win.DefaultableBoolean.True;
            ultraDataColumn4.AllowDBNull = Infragistics.Win.DefaultableBoolean.True;
            ultraDataColumn5.AllowDBNull = Infragistics.Win.DefaultableBoolean.True;
            ultraDataColumn6.AllowDBNull = Infragistics.Win.DefaultableBoolean.True;
            ultraDataColumn6.DataType = typeof(bool);
            ultraDataColumn7.AllowDBNull = Infragistics.Win.DefaultableBoolean.True;
            ultraDataColumn7.DataType = typeof(int);
            ultraDataColumn8.AllowDBNull = Infragistics.Win.DefaultableBoolean.True;
            ultraDataColumn8.DataType = typeof(byte);
            ultraDataColumn9.AllowDBNull = Infragistics.Win.DefaultableBoolean.True;
            ultraDataColumn9.DataType = typeof(byte);
            ultraDataColumn10.AllowDBNull = Infragistics.Win.DefaultableBoolean.True;
            ultraDataColumn10.DataType = typeof(double);
            ultraDataColumn11.AllowDBNull = Infragistics.Win.DefaultableBoolean.True;
            ultraDataColumn12.AllowDBNull = Infragistics.Win.DefaultableBoolean.True;
            ultraDataColumn13.AllowDBNull = Infragistics.Win.DefaultableBoolean.True;
            this.alertsViewDataSource.Band.Columns.AddRange(new object[] {
            ultraDataColumn1,
            ultraDataColumn2,
            ultraDataColumn3,
            ultraDataColumn4,
            ultraDataColumn5,
            ultraDataColumn6,
            ultraDataColumn7,
            ultraDataColumn8,
            ultraDataColumn9,
            ultraDataColumn10,
            ultraDataColumn11,
            ultraDataColumn12,
            ultraDataColumn13});
            this.alertsViewDataSource.KeyIndex = 0;
            // 
            // alertsGridStatusLabel
            // 
            this.alertsGridStatusLabel.BackColor = System.Drawing.Color.White;
            this.alertsGridStatusLabel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.alertsGridStatusLabel.Location = new System.Drawing.Point(0, 0);
            this.alertsGridStatusLabel.Name = "alertsGridStatusLabel";
            this.alertsGridStatusLabel.Padding = new System.Windows.Forms.Padding(0, 5, 0, 0);
            this.alertsGridStatusLabel.Size = new System.Drawing.Size(761, 279);
            this.alertsGridStatusLabel.TabIndex = 7;
            this.alertsGridStatusLabel.Text = "< Status Message >";
            this.alertsGridStatusLabel.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            // 
            // detailsPanel
            // 
            this.detailsPanel.Controls.Add(this.detailsContentPanel);
            this.detailsPanel.Controls.Add(this.detailsHeaderStrip);
            this.detailsPanel.Controls.Add(this.noSelectionLabel);
            this.detailsPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.detailsPanel.Location = new System.Drawing.Point(0, 0);
            this.detailsPanel.Name = "detailsPanel";
            this.detailsPanel.Size = new System.Drawing.Size(761, 129);
            this.detailsPanel.TabIndex = 1;
            // 
            // detailsContentPanel
            // 
            this.detailsContentPanel.ColumnCount = 7;
            this.detailsContentPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 70F));
            this.detailsContentPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 180F));
            this.detailsContentPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 70F));
            this.detailsContentPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 180F));
            this.detailsContentPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 90F));
            this.detailsContentPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 230F));
            this.detailsContentPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 150F));
            this.detailsContentPanel.Controls.Add(this.showDetailsLinkLabel, 6, 3);
            this.detailsContentPanel.Controls.Add(this.helpHistoryLinkLabel1, 6, 1);
            this.detailsContentPanel.Controls.Add(this.headerLabel, 1, 3);
            this.detailsContentPanel.Controls.Add(this.label2, 0, 0);
            this.detailsContentPanel.Controls.Add(this.label3, 2, 0);
            this.detailsContentPanel.Controls.Add(this.label4, 0, 2);
            this.detailsContentPanel.Controls.Add(this.ServerTypelabel, 4, 0);
            this.detailsContentPanel.Controls.Add(this.ServerTypeText, 5, 0);
            this.detailsContentPanel.Controls.Add(this.label5, 2, 2);
            this.detailsContentPanel.Controls.Add(this.label6, 0, 3);
            this.detailsContentPanel.Controls.Add(this.label8, 3, 0);
            this.detailsContentPanel.Controls.Add(this.label10, 3, 2);
            this.detailsContentPanel.Controls.Add(this.panel1, 1, 0);
            this.detailsContentPanel.Controls.Add(this.label7, 0, 1);
            this.detailsContentPanel.Controls.Add(this.panel2, 1, 1);
            this.detailsContentPanel.Controls.Add(this.label11, 2, 1);
            this.detailsContentPanel.Controls.Add(this.metricLabel, 3, 1);
            this.detailsContentPanel.Controls.Add(this.messageTextBox, 0, 4);
            this.detailsContentPanel.Controls.Add(this.showRealTimeViewLinkLabel, 6, 0);
            this.detailsContentPanel.Controls.Add(this.helpHistoryLinkLabel2, 6, 2);
            this.detailsContentPanel.Controls.Add(this.label9, 1, 2);
            this.detailsContentPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.detailsContentPanel.Location = new System.Drawing.Point(0, 19);
            this.detailsContentPanel.Name = "detailsContentPanel";
            this.detailsContentPanel.RowCount = 5;
            this.detailsContentPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.detailsContentPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.detailsContentPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.detailsContentPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.detailsContentPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.detailsContentPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.detailsContentPanel.Size = new System.Drawing.Size(761, 110);
            this.detailsContentPanel.TabIndex = 8;
            // 
            // showDetailsLinkLabel
            // 
            this.showDetailsLinkLabel.AutoSize = true;
            this.showDetailsLinkLabel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.showDetailsLinkLabel.Location = new System.Drawing.Point(641, 60);
            this.showDetailsLinkLabel.Name = "showDetailsLinkLabel";
            this.showDetailsLinkLabel.Size = new System.Drawing.Size(117, 20);
            this.showDetailsLinkLabel.TabIndex = 21;
            this.showDetailsLinkLabel.TabStop = true;
            this.showDetailsLinkLabel.Text = "Show Block Details";
            this.showDetailsLinkLabel.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.showDetailsLinkLabel.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.showDetailsLinkLabel_LinkClicked);
            // 
            // helpHistoryLinkLabel1
            // 
            this.helpHistoryLinkLabel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.helpHistoryLinkLabel1.Location = new System.Drawing.Point(641, 0);
            this.helpHistoryLinkLabel1.Name = "helpHistoryLinkLabel1";
            this.helpHistoryLinkLabel1.Size = new System.Drawing.Size(117, 20);
            this.helpHistoryLinkLabel1.TabIndex = 19;
            this.helpHistoryLinkLabel1.TabStop = true;
            this.helpHistoryLinkLabel1.Text = "helpHistoryLinkLabel1";
            this.helpHistoryLinkLabel1.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.helpHistoryLinkLabel1.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.helpHistoryLinkLabel1_LinkClicked);
            // 
            // headerLabel
            // 
            this.headerLabel.AutoEllipsis = true;
            this.headerLabel.AutoSize = true;
            this.detailsContentPanel.SetColumnSpan(this.headerLabel, 3);
            this.headerLabel.DataBindings.Add(new System.Windows.Forms.Binding("Text", this.alertsViewDataSource, "Band 0.Heading", true));
            this.headerLabel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.headerLabel.Location = new System.Drawing.Point(59, 63);
            this.headerLabel.Margin = new System.Windows.Forms.Padding(0, 3, 3, 0);
            this.headerLabel.Name = "headerLabel";
            this.headerLabel.Size = new System.Drawing.Size(576, 17);
            this.headerLabel.TabIndex = 16;
            this.headerLabel.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            //
            //Server Type Text
            //
            this.ServerTypeText.AutoSize = true;
            this.ServerTypeText.Dock = System.Windows.Forms.DockStyle.Fill;
            this.ServerTypeText.Location = new System.Drawing.Point(310, 3);
            this.ServerTypeText.Margin = new System.Windows.Forms.Padding(3, 3, 3, 0);
            this.ServerTypeText.Name = "ServerTypeText";
            this.ServerTypeText.Size = new System.Drawing.Size(266, 17);
            this.ServerTypeText.TabIndex = 15;
            this.ServerTypeText.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;

            //
            //server type label
            //
            this.ServerTypelabel.AutoEllipsis = true;
            this.ServerTypelabel.AutoSize = true;
            this.ServerTypelabel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.ServerTypelabel.Location = new System.Drawing.Point(300, 3);
            this.ServerTypelabel.Margin = new System.Windows.Forms.Padding(3, 3, 3, 0);
            this.ServerTypelabel.Name = "ServerTypelabel";
            this.ServerTypelabel.Size = new System.Drawing.Size(40, 17);
            this.ServerTypelabel.TabIndex = 0;
            this.ServerTypelabel.Text = "Server Type:";
            this.ServerTypelabel.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // label2
            // 
            this.label2.AutoEllipsis = true;
            this.label2.AutoSize = true;
            this.label2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.label2.Location = new System.Drawing.Point(3, 3);
            this.label2.Margin = new System.Windows.Forms.Padding(3, 3, 3, 0);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(53, 17);
            this.label2.TabIndex = 0;
            this.label2.Text = "Severity:";
            this.label2.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // label3
            // 
            this.label3.AutoEllipsis = true;
            this.label3.AutoSize = true;
            this.label3.Dock = System.Windows.Forms.DockStyle.Fill;
            this.label3.Location = new System.Drawing.Point(307, 3);
            this.label3.Margin = new System.Windows.Forms.Padding(3, 3, 3, 0);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(56, 17);
            this.label3.TabIndex = 1;
            this.label3.Text = "Time:";
            this.label3.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // label4
            // 
            this.label4.AutoEllipsis = true;
            this.label4.AutoSize = true;
            this.label4.Dock = System.Windows.Forms.DockStyle.Fill;
            this.label4.Location = new System.Drawing.Point(3, 43);
            this.label4.Margin = new System.Windows.Forms.Padding(3, 3, 3, 0);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(53, 17);
            this.label4.TabIndex = 2;
            this.label4.Text = "Server:";
            this.label4.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // label5
            // 
            this.label5.AutoEllipsis = true;
            this.label5.AutoSize = true;
            this.label5.Dock = System.Windows.Forms.DockStyle.Fill;
            this.label5.Location = new System.Drawing.Point(307, 43);
            this.label5.Margin = new System.Windows.Forms.Padding(3, 3, 3, 0);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(56, 17);
            this.label5.TabIndex = 3;
            this.label5.Text = "Database:";
            this.label5.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // label6
            // 
            this.label6.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
            | System.Windows.Forms.AnchorStyles.Left)
            | System.Windows.Forms.AnchorStyles.Right)));
            this.label6.AutoEllipsis = true;
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(3, 63);
            this.label6.Margin = new System.Windows.Forms.Padding(3, 3, 3, 0);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(53, 17);
            this.label6.TabIndex = 4;
            this.label6.Text = "Summary:";
            this.label6.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.DataBindings.Add(new System.Windows.Forms.Binding("Text", this.alertsViewDataSource, "Band 0.UTCOccurrenceDateTime", true));
            this.label8.Dock = System.Windows.Forms.DockStyle.Fill;
            this.label8.Location = new System.Drawing.Point(369, 3);
            this.label8.Margin = new System.Windows.Forms.Padding(3, 3, 3, 0);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(266, 17);
            this.label8.TabIndex = 7;
            this.label8.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.DataBindings.Add(new System.Windows.Forms.Binding("Text", this.alertsViewDataSource, "Band 0.DatabaseName", true));
            this.label10.Dock = System.Windows.Forms.DockStyle.Fill;
            this.label10.Location = new System.Drawing.Point(369, 43);
            this.label10.Margin = new System.Windows.Forms.Padding(3, 3, 3, 0);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(266, 17);
            this.label10.TabIndex = 9;
            this.label10.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // panel1
            // 
            this.panel1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
            | System.Windows.Forms.AnchorStyles.Left)
            | System.Windows.Forms.AnchorStyles.Right)));
            this.panel1.Controls.Add(this.severityLabel);
            this.panel1.Controls.Add(this.severityImage);
            this.panel1.Location = new System.Drawing.Point(59, 0);
            this.panel1.Margin = new System.Windows.Forms.Padding(0);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(245, 20);
            this.panel1.TabIndex = 10;
            // 
            // severityLabel
            // 
            this.severityLabel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.severityLabel.Location = new System.Drawing.Point(16, 0);
            this.severityLabel.Name = "severityLabel";
            this.severityLabel.Size = new System.Drawing.Size(229, 20);
            this.severityLabel.TabIndex = 0;
            this.severityLabel.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // severityImage
            // 
            this.severityImage.Dock = System.Windows.Forms.DockStyle.Left;
            this.severityImage.Location = new System.Drawing.Point(0, 0);
            this.severityImage.Name = "severityImage";
            this.severityImage.Size = new System.Drawing.Size(16, 20);
            this.severityImage.SizeMode = System.Windows.Forms.PictureBoxSizeMode.CenterImage;
            this.severityImage.TabIndex = 0;
            this.severityImage.TabStop = false;
            // 
            // label7
            // 
            this.label7.AutoEllipsis = true;
            this.label7.AutoSize = true;
            this.label7.Dock = System.Windows.Forms.DockStyle.Fill;
            this.label7.Location = new System.Drawing.Point(3, 23);
            this.label7.Margin = new System.Windows.Forms.Padding(3, 3, 3, 0);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(53, 17);
            this.label7.TabIndex = 11;
            this.label7.Text = "Change:";
            this.label7.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // panel2
            // 
            this.panel2.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
            | System.Windows.Forms.AnchorStyles.Left)
            | System.Windows.Forms.AnchorStyles.Right)));
            this.panel2.Controls.Add(this.transitionLabel);
            this.panel2.Controls.Add(this.transitionImage);
            this.panel2.Location = new System.Drawing.Point(59, 20);
            this.panel2.Margin = new System.Windows.Forms.Padding(0);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(245, 20);
            this.panel2.TabIndex = 12;
            // 
            // transitionLabel
            // 
            this.transitionLabel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.transitionLabel.Location = new System.Drawing.Point(16, 0);
            this.transitionLabel.Name = "transitionLabel";
            this.transitionLabel.Size = new System.Drawing.Size(229, 20);
            this.transitionLabel.TabIndex = 1;
            this.transitionLabel.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // transitionImage
            // 
            this.transitionImage.Dock = System.Windows.Forms.DockStyle.Left;
            this.transitionImage.Location = new System.Drawing.Point(0, 0);
            this.transitionImage.Name = "transitionImage";
            this.transitionImage.Size = new System.Drawing.Size(16, 20);
            this.transitionImage.SizeMode = System.Windows.Forms.PictureBoxSizeMode.CenterImage;
            this.transitionImage.TabIndex = 0;
            this.transitionImage.TabStop = false;
            // 
            // label11
            // 
            this.label11.AutoEllipsis = true;
            this.label11.AutoSize = true;
            this.label11.Dock = System.Windows.Forms.DockStyle.Fill;
            this.label11.Location = new System.Drawing.Point(307, 23);
            this.label11.Margin = new System.Windows.Forms.Padding(3, 3, 3, 0);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(56, 17);
            this.label11.TabIndex = 13;
            this.label11.Text = "Metric:";
            this.label11.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // metricLabel
            // 
            this.metricLabel.AutoSize = true;
            this.metricLabel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.metricLabel.Location = new System.Drawing.Point(369, 23);
            this.metricLabel.Margin = new System.Windows.Forms.Padding(3, 3, 3, 0);
            this.metricLabel.Name = "metricLabel";
            this.metricLabel.Size = new System.Drawing.Size(266, 17);
            this.metricLabel.TabIndex = 15;
            this.metricLabel.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // messageTextBox
            // 
            this.messageTextBox.BackColor = System.Drawing.Color.White;
            this.messageTextBox.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.detailsContentPanel.SetColumnSpan(this.messageTextBox, 7);
            this.messageTextBox.DataBindings.Add(new System.Windows.Forms.Binding("Text", this.alertsViewDataSource, "Band 0.Message", true));
            this.messageTextBox.Dock = System.Windows.Forms.DockStyle.Fill;
            this.messageTextBox.Location = new System.Drawing.Point(3, 83);
            this.messageTextBox.Multiline = true;
            this.messageTextBox.Name = "messageTextBox";
            this.messageTextBox.ReadOnly = true;
            this.messageTextBox.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.messageTextBox.Size = new System.Drawing.Size(755, 24);
            this.messageTextBox.TabIndex = 17;
            // 
            // showRealTimeViewLinkLabel
            // 
            this.showRealTimeViewLinkLabel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.showRealTimeViewLinkLabel.Location = new System.Drawing.Point(641, 0);
            this.showRealTimeViewLinkLabel.Name = "showRealTimeViewLinkLabel";
            this.showRealTimeViewLinkLabel.Size = new System.Drawing.Size(117, 20);
            this.showRealTimeViewLinkLabel.TabIndex = 18;
            this.showRealTimeViewLinkLabel.TabStop = true;
            this.showRealTimeViewLinkLabel.Text = "Show Real Time View";
            this.showRealTimeViewLinkLabel.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.showRealTimeViewLinkLabel.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.showRealTimeViewLinkLabel_LinkClicked);
            // 
            // helpHistoryLinkLabel2
            // 
            this.helpHistoryLinkLabel2.AutoSize = true;
            this.helpHistoryLinkLabel2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.helpHistoryLinkLabel2.Location = new System.Drawing.Point(641, 40);
            this.helpHistoryLinkLabel2.Name = "helpHistoryLinkLabel2";
            this.helpHistoryLinkLabel2.Size = new System.Drawing.Size(117, 20);
            this.helpHistoryLinkLabel2.TabIndex = 20;
            this.helpHistoryLinkLabel2.TabStop = true;
            this.helpHistoryLinkLabel2.Text = "helpHistoryLinkLabel2";
            this.helpHistoryLinkLabel2.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.helpHistoryLinkLabel2.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.helpHistoryLinkLabel2_LinkClicked);
            // 
            // label9
            // 
            this.label9.AutoEllipsis = true;
            this.label9.DataBindings.Add(new System.Windows.Forms.Binding("Text", this.alertsViewDataSource, "Band 0.ServerName", true));
            this.label9.Dock = System.Windows.Forms.DockStyle.Fill;
            this.label9.Location = new System.Drawing.Point(59, 43);
            this.label9.Margin = new System.Windows.Forms.Padding(0, 3, 3, 0);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(242, 17);
            this.label9.TabIndex = 8;
            this.label9.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // detailsHeaderStrip
            // 
            this.detailsHeaderStrip.AutoSize = false;
            this.detailsHeaderStrip.Font = new System.Drawing.Font("Tahoma", 8.25F);
            this.detailsHeaderStrip.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(75)))), ((int)(((byte)(75)))), ((int)(((byte)(75)))));
            this.detailsHeaderStrip.GripMargin = new System.Windows.Forms.Padding(0);
            this.detailsHeaderStrip.GripStyle = System.Windows.Forms.ToolStripGripStyle.Hidden;
            this.detailsHeaderStrip.HotTrackEnabled = false;
            this.detailsHeaderStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.detailsHeaderStripLabel,
            this.toggleDetailsPanelButton});
            this.detailsHeaderStrip.Location = new System.Drawing.Point(0, 0);
            this.detailsHeaderStrip.Name = "detailsHeaderStrip";
            this.detailsHeaderStrip.Padding = new System.Windows.Forms.Padding(0);
            this.detailsHeaderStrip.Size = new System.Drawing.Size(761, 19);
            this.detailsHeaderStrip.Style = Idera.SQLdm.DesktopClient.Controls.HeaderStripStyle.Small;
            this.detailsHeaderStrip.TabIndex = 1;
            // 
            // detailsHeaderStripLabel
            // 
            this.detailsHeaderStripLabel.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.detailsHeaderStripLabel.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(245)))), ((int)(((byte)(245)))));
            this.detailsHeaderStripLabel.Name = "detailsHeaderStripLabel";
            this.detailsHeaderStripLabel.Size = new System.Drawing.Size(46, 16);
            this.detailsHeaderStripLabel.Text = "Details";
            // 
            // toggleDetailsPanelButton
            // 
            this.toggleDetailsPanelButton.Alignment = System.Windows.Forms.ToolStripItemAlignment.Right;
            this.toggleDetailsPanelButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.toggleDetailsPanelButton.Image = global::Idera.SQLdm.DesktopClient.Properties.Resources.Office2007Close;
            this.toggleDetailsPanelButton.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None;
            this.toggleDetailsPanelButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toggleDetailsPanelButton.Name = "toggleDetailsPanelButton";
            this.toggleDetailsPanelButton.Size = new System.Drawing.Size(23, 16);
            this.toggleDetailsPanelButton.Click += new System.EventHandler(this.toggleDetailsPanelButton_Click);
            // 
            // noSelectionLabel
            // 
            this.noSelectionLabel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.noSelectionLabel.ForeColor = System.Drawing.SystemColors.ControlText;
            this.noSelectionLabel.Location = new System.Drawing.Point(0, 0);
            this.noSelectionLabel.Name = "noSelectionLabel";
            this.noSelectionLabel.Size = new System.Drawing.Size(761, 129);
            this.noSelectionLabel.TabIndex = 9;
            this.noSelectionLabel.Text = "Select an alert to view its details.";
            this.noSelectionLabel.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            // 
            // filterOptionsPanel
            // 
            this.filterOptionsPanel.Controls.Add(this.tableLayoutPanel1);
            this.filterOptionsPanel.Controls.Add(this.dividerLabel1);
            this.filterOptionsPanel.Dock = System.Windows.Forms.DockStyle.Top;
            this.filterOptionsPanel.Location = new System.Drawing.Point(0, 0);
            this.filterOptionsPanel.Name = "filterOptionsPanel";
            this.filterOptionsPanel.Size = new System.Drawing.Size(761, 162);
            this.filterOptionsPanel.TabIndex = 0;
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.AutoScroll = true;
            this.tableLayoutPanel1.ColumnCount = 7;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 60F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 5F));
            this.tableLayoutPanel1.Controls.Add(this.clearButton, 4, 5);
            this.tableLayoutPanel1.Controls.Add(this.applyButton, 5, 5);
            this.tableLayoutPanel1.Controls.Add(this.dividerVert, 2, 0);
            this.tableLayoutPanel1.Controls.Add(this.beginTimeCombo, 5, 2);
            this.tableLayoutPanel1.Controls.Add(this.lblTo, 3, 3);
            this.tableLayoutPanel1.Controls.Add(this.endTimeCombo, 5, 3);
            this.tableLayoutPanel1.Controls.Add(this.lblFrom, 3, 2);
            this.tableLayoutPanel1.Controls.Add(this.endDateCombo, 4, 3);
            this.tableLayoutPanel1.Controls.Add(this.dividerLabel, 0, 4);
            this.tableLayoutPanel1.Controls.Add(this.label1, 0, 0);
            this.tableLayoutPanel1.Controls.Add(this.tagCombo, 1, 0);
            this.tableLayoutPanel1.Controls.Add(this.beginDateCombo, 4, 2);
            this.tableLayoutPanel1.Controls.Add(this.serverLabel, 0, 1);
            this.tableLayoutPanel1.Controls.Add(this.serverCombo, 1, 1);
            this.tableLayoutPanel1.Controls.Add(this.metricFilterLabel, 0, 2);
            this.tableLayoutPanel1.Controls.Add(this.metricCombo, 1, 2);
            this.tableLayoutPanel1.Controls.Add(this.severityFilterLabel, 0, 3);
            this.tableLayoutPanel1.Controls.Add(this.severityCombo, 1, 3);
            this.tableLayoutPanel1.Controls.Add(this.rdbtnActiveOnly, 3, 0);
            this.tableLayoutPanel1.Controls.Add(this.rdbtnTimeSpan, 3, 1);
            this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel1.Location = new System.Drawing.Point(0, 0);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 6;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel1.Size = new System.Drawing.Size(761, 161);
            this.tableLayoutPanel1.TabIndex = 2;
            // 
            // clearButton
            // 
            this.clearButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.clearButton.Location = new System.Drawing.Point(559, 131);
            this.clearButton.Name = "clearButton";
            this.clearButton.Size = new System.Drawing.Size(94, 21);
            this.clearButton.TabIndex = 18;
            this.clearButton.Text = "Clear Filter";
            this.clearButton.Click += new System.EventHandler(this.clearButton_Click);
            // 
            // applyButton
            // 
            this.applyButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.applyButton.Location = new System.Drawing.Point(659, 129);
            this.applyButton.Name = "applyButton";
            this.applyButton.Size = new System.Drawing.Size(94, 24);
            this.applyButton.TabIndex = 19;
            this.applyButton.Text = "Apply";
            this.applyButton.Click += new System.EventHandler(this.applyButton_Click);
            // 
            // dividerVert
            // 
            this.dividerVert.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.dividerVert.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.dividerVert.Location = new System.Drawing.Point(491, 0);
            this.dividerVert.Name = "dividerVert";
            this.tableLayoutPanel1.SetRowSpan(this.dividerVert, 4);
            this.dividerVert.Size = new System.Drawing.Size(2, 120);
            this.dividerVert.TabIndex = 8;
            // 
            // beginTimeCombo
            // 
            this.beginTimeCombo.Anchor = System.Windows.Forms.AnchorStyles.Left;
            dropDownEditorButton1.Key = "DropDownList";
            dropDownEditorButton1.PreferredDropDownSize = new System.Drawing.Size(0, 0);
            this.beginTimeCombo.ButtonsRight.Add(dropDownEditorButton1);
            this.beginTimeCombo.DateTime = new System.DateTime(2012, 11, 8, 0, 0, 0, 0);
            this.beginTimeCombo.DropDownButtonDisplayStyle = Infragistics.Win.ButtonDisplayStyle.Never;
            this.beginTimeCombo.Enabled = false;
            this.beginTimeCombo.ListInterval = System.TimeSpan.Parse("00:30:00");
            this.beginTimeCombo.Location = new System.Drawing.Point(659, 57);
            this.beginTimeCombo.MaskInput = "{time}";
            this.beginTimeCombo.Name = "beginTimeCombo";
            this.beginTimeCombo.Size = new System.Drawing.Size(94, 21);
            this.beginTimeCombo.TabIndex = 13;
            this.beginTimeCombo.Time = System.TimeSpan.Parse("00:00:00");
            this.beginTimeCombo.Value = new System.DateTime(2012, 11, 8, 0, 0, 0, 0);
            this.beginTimeCombo.ValueChanged += new System.EventHandler(this.FilterValueChanged);
            // 
            // lblTo
            // 
            this.lblTo.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this.lblTo.AutoSize = true;
            this.lblTo.Location = new System.Drawing.Point(530, 94);
            this.lblTo.Name = "lblTo";
            this.lblTo.Size = new System.Drawing.Size(23, 13);
            this.lblTo.TabIndex = 14;
            this.lblTo.Text = "To:";
            this.lblTo.UseMnemonic = false;
            // 
            // endTimeCombo
            // 
            this.endTimeCombo.Anchor = System.Windows.Forms.AnchorStyles.Left;
            dropDownEditorButton2.Key = "DropDownList";
            dropDownEditorButton2.PreferredDropDownSize = new System.Drawing.Size(0, 0);
            this.endTimeCombo.ButtonsRight.Add(dropDownEditorButton2);
            this.endTimeCombo.DateTime = new System.DateTime(2012, 11, 8, 0, 0, 0, 0);
            this.endTimeCombo.DropDownButtonDisplayStyle = Infragistics.Win.ButtonDisplayStyle.Never;
            this.endTimeCombo.Enabled = false;
            this.endTimeCombo.ListInterval = System.TimeSpan.Parse("00:30:00");
            this.endTimeCombo.Location = new System.Drawing.Point(659, 90);
            this.endTimeCombo.MaskInput = "{time}";
            this.endTimeCombo.Name = "endTimeCombo";
            this.endTimeCombo.Size = new System.Drawing.Size(94, 21);
            this.endTimeCombo.TabIndex = 16;
            this.endTimeCombo.Time = System.TimeSpan.Parse("00:00:00");
            this.endTimeCombo.Value = new System.DateTime(2012, 11, 8, 0, 0, 0, 0);
            this.endTimeCombo.ValueChanged += new System.EventHandler(this.FilterValueChanged);
            // 
            // lblFrom
            // 
            this.lblFrom.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this.lblFrom.AutoSize = true;
            this.lblFrom.Location = new System.Drawing.Point(520, 61);
            this.lblFrom.Name = "lblFrom";
            this.lblFrom.Size = new System.Drawing.Size(33, 13);
            this.lblFrom.TabIndex = 11;
            this.lblFrom.Text = "From:";
            // 
            // endDateCombo
            // 
            this.endDateCombo.AllowNull = false;
            this.endDateCombo.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.endDateCombo.DateButtons.Add(dateButton1);
            this.endDateCombo.Enabled = false;
            this.endDateCombo.Location = new System.Drawing.Point(559, 90);
            this.endDateCombo.Name = "endDateCombo";
            this.endDateCombo.NonAutoSizeHeight = 21;
            this.endDateCombo.NullDateLabel = "";
            this.endDateCombo.Size = new System.Drawing.Size(94, 21);
            this.endDateCombo.TabIndex = 15;
            this.endDateCombo.Value = new System.DateTime(2007, 2, 20, 0, 0, 0, 0);
            this.endDateCombo.ValueChanged += new System.EventHandler(this.FilterValueChanged);
            // 
            // dividerLabel
            // 
            this.dividerLabel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.dividerLabel.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.tableLayoutPanel1.SetColumnSpan(this.dividerLabel, 6);
            this.dividerLabel.Location = new System.Drawing.Point(3, 120);
            this.dividerLabel.Name = "dividerLabel";
            this.dividerLabel.Size = new System.Drawing.Size(750, 2);
            this.dividerLabel.TabIndex = 28;
            this.dividerLabel.Text = "label1";
            // 
            // label1
            // 
            this.label1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(3, 7);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(48, 13);
            this.label1.TabIndex = 21;
            this.label1.Text = "Tag:";
            // 
            // tagCombo
            // 
            this.tagCombo.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.tagCombo.DropDownStyle = Infragistics.Win.DropDownStyle.DropDownList;
            this.tagCombo.LimitToList = true;
            this.tagCombo.Location = new System.Drawing.Point(57, 3);
            this.tagCombo.Name = "tagCombo";
            this.tagCombo.Size = new System.Drawing.Size(428, 21);
            this.tagCombo.TabIndex = 22;
            this.tagCombo.BeforeDropDown += new System.ComponentModel.CancelEventHandler(this.tagCombo_BeforeDropDown);
            this.tagCombo.ValueChanged += new System.EventHandler(this.FilterValueChanged);
            // 
            // beginDateCombo
            // 
            this.beginDateCombo.AllowNull = false;
            this.beginDateCombo.DateButtons.Add(dateButton2);
            this.beginDateCombo.Enabled = false;
            this.beginDateCombo.Location = new System.Drawing.Point(559, 57);
            this.beginDateCombo.Name = "beginDateCombo";
            this.beginDateCombo.NonAutoSizeHeight = 21;
            this.beginDateCombo.NullDateLabel = "";
            this.beginDateCombo.Size = new System.Drawing.Size(94, 21);
            this.beginDateCombo.TabIndex = 12;
            this.beginDateCombo.Value = new System.DateTime(2007, 2, 20, 0, 0, 0, 0);
            this.beginDateCombo.ValueChanged += new System.EventHandler(this.FilterValueChanged);
            // 
            // serverLabel
            // 
            this.serverLabel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.serverLabel.AutoSize = true;
            this.serverLabel.Location = new System.Drawing.Point(3, 34);
            this.serverLabel.Name = "serverLabel";
            this.serverLabel.Size = new System.Drawing.Size(48, 13);
            this.serverLabel.TabIndex = 23;
            this.serverLabel.Text = "Server:";
            // 
            // serverCombo
            // 
            this.serverCombo.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.serverCombo.DropDownStyle = Infragistics.Win.DropDownStyle.DropDownList;
            this.serverCombo.LimitToList = true;
            this.serverCombo.Location = new System.Drawing.Point(57, 30);
            this.serverCombo.Name = "serverCombo";
            this.serverCombo.Size = new System.Drawing.Size(428, 21);
            this.serverCombo.TabIndex = 24;
            this.serverCombo.BeforeDropDown += new System.ComponentModel.CancelEventHandler(this.serverCombo_BeforeDropDown);
            this.serverCombo.ValueChanged += new System.EventHandler(this.FilterValueChanged);
            // 
            // metricFilterLabel
            // 
            this.metricFilterLabel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.metricFilterLabel.AutoSize = true;
            this.metricFilterLabel.Location = new System.Drawing.Point(3, 61);
            this.metricFilterLabel.Name = "metricFilterLabel";
            this.metricFilterLabel.Size = new System.Drawing.Size(48, 13);
            this.metricFilterLabel.TabIndex = 25;
            this.metricFilterLabel.Text = "Metric:";
            // 
            // metricCombo
            // 
            this.metricCombo.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.metricCombo.DropDownStyle = Infragistics.Win.DropDownStyle.DropDownList;
            this.metricCombo.LimitToList = true;
            this.metricCombo.Location = new System.Drawing.Point(57, 57);
            this.metricCombo.Name = "metricCombo";
            this.metricCombo.Size = new System.Drawing.Size(428, 21);
            this.metricCombo.TabIndex = 26;
            this.metricCombo.BeforeDropDown += new System.ComponentModel.CancelEventHandler(this.metricCombo_BeforeDropDown);
            this.metricCombo.ValueChanged += new System.EventHandler(this.FilterValueChanged);
            // 
            // severityFilterLabel
            // 
            this.severityFilterLabel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.severityFilterLabel.AutoSize = true;
            this.severityFilterLabel.Location = new System.Drawing.Point(3, 94);
            this.severityFilterLabel.Name = "severityFilterLabel";
            this.severityFilterLabel.Size = new System.Drawing.Size(48, 13);
            this.severityFilterLabel.TabIndex = 27;
            this.severityFilterLabel.Text = "Severity:";
            // 
            // severityCombo
            // 
            this.severityCombo.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.severityCombo.DropDownStyle = Infragistics.Win.DropDownStyle.DropDownList;
            this.severityCombo.LimitToList = true;
            this.severityCombo.Location = new System.Drawing.Point(57, 90);
            this.severityCombo.Name = "severityCombo";
            this.severityCombo.Size = new System.Drawing.Size(428, 21);
            this.severityCombo.TabIndex = 7;
            this.severityCombo.ValueChanged += new System.EventHandler(this.FilterValueChanged);
            // 
            // rdbtnActiveOnly
            // 
            this.rdbtnActiveOnly.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)
            | System.Windows.Forms.AnchorStyles.Right)));
            this.rdbtnActiveOnly.AutoSize = true;
            this.rdbtnActiveOnly.Checked = true;
            this.tableLayoutPanel1.SetColumnSpan(this.rdbtnActiveOnly, 3);
            this.rdbtnActiveOnly.Location = new System.Drawing.Point(499, 7);
            this.rdbtnActiveOnly.Name = "rdbtnActiveOnly";
            this.rdbtnActiveOnly.Size = new System.Drawing.Size(254, 17);
            this.rdbtnActiveOnly.TabIndex = 29;
            this.rdbtnActiveOnly.TabStop = true;
            this.rdbtnActiveOnly.Text = "Show active alerts";
            this.rdbtnActiveOnly.UseVisualStyleBackColor = true;
            this.rdbtnActiveOnly.CheckedChanged += new System.EventHandler(this.FilterValueChanged);
            // 
            // rdbtnTimeSpan
            // 
            this.rdbtnTimeSpan.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.rdbtnTimeSpan.AutoSize = true;
            this.tableLayoutPanel1.SetColumnSpan(this.rdbtnTimeSpan, 3);
            this.rdbtnTimeSpan.Location = new System.Drawing.Point(499, 32);
            this.rdbtnTimeSpan.Name = "rdbtnTimeSpan";
            this.rdbtnTimeSpan.Size = new System.Drawing.Size(254, 17);
            this.rdbtnTimeSpan.TabIndex = 30;
            this.rdbtnTimeSpan.Text = "Show all alerts for the time span";
            this.rdbtnTimeSpan.UseVisualStyleBackColor = true;
            // 
            // dividerLabel1
            // 
            this.dividerLabel1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(140)))), ((int)(((byte)(140)))), ((int)(((byte)(140)))));
            this.dividerLabel1.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.dividerLabel1.Location = new System.Drawing.Point(0, 161);
            this.dividerLabel1.Name = "dividerLabel1";
            this.dividerLabel1.Size = new System.Drawing.Size(761, 1);
            this.dividerLabel1.TabIndex = 1;
            // 
            // toolbarsManager
            // 
            this.toolbarsManager.DesignerFlags = 0;
            this.toolbarsManager.DockWithinContainer = this;
            this.toolbarsManager.ShowFullMenusDelay = 500;
            popupMenuTool1.SharedPropsInternal.Caption = "columnContextMenu";
            stateButtonTool1.MenuDisplayStyle = Infragistics.Win.UltraWinToolbars.StateButtonMenuDisplayStyle.DisplayCheckmark;
            buttonTool4.InstanceProps.IsFirstInGroup = true;
            buttonTool6.InstanceProps.IsFirstInGroup = true;
            popupMenuTool1.Tools.AddRange(new Infragistics.Win.UltraWinToolbars.ToolBase[] {
            buttonTool1,
            buttonTool2,
            stateButtonTool1,
            buttonTool3,
            buttonTool4,
            buttonTool5,
            buttonTool6});
            appearance13.Image = global::Idera.SQLdm.DesktopClient.Properties.Resources.ColumnChooser;
            buttonTool7.SharedPropsInternal.AppearancesSmall.Appearance = appearance13;
            buttonTool7.SharedPropsInternal.Caption = "Column Chooser";
            appearance14.Image = global::Idera.SQLdm.DesktopClient.Properties.Resources.GroupByBox;
            buttonTool8.SharedPropsInternal.AppearancesSmall.Appearance = appearance14;
            buttonTool8.SharedPropsInternal.Caption = "Group By Box";
            appearance15.Image = global::Idera.SQLdm.DesktopClient.Properties.Resources.SortAscending;
            buttonTool9.SharedPropsInternal.AppearancesSmall.Appearance = appearance15;
            buttonTool9.SharedPropsInternal.Caption = "Sort Ascending";
            appearance16.Image = global::Idera.SQLdm.DesktopClient.Properties.Resources.SortDescending;
            buttonTool10.SharedPropsInternal.AppearancesSmall.Appearance = appearance16;
            buttonTool10.SharedPropsInternal.Caption = "Sort Descending";
            buttonTool11.SharedPropsInternal.Caption = "Remove This Column";
            buttonTool12.SharedPropsInternal.Caption = "Show Column Text";
            stateButtonTool2.MenuDisplayStyle = Infragistics.Win.UltraWinToolbars.StateButtonMenuDisplayStyle.DisplayCheckmark;
            stateButtonTool2.SharedPropsInternal.Caption = "Group By This Column";
            popupMenuTool2.SharedPropsInternal.Caption = "gridContextMenu";
            buttonTool13.InstanceProps.IsFirstInGroup = true;
            buttonTool14.InstanceProps.IsFirstInGroup = true;
            buttonTool16.InstanceProps.IsFirstInGroup = true;
            buttonTool17.InstanceProps.IsFirstInGroup = true;
            buttonTool19.InstanceProps.IsFirstInGroup = true;
            popupMenuTool2.Tools.AddRange(new Infragistics.Win.UltraWinToolbars.ToolBase[] {
            buttonTool40,
            buttonTool42,
            buttonTool33,
            buttonTool34,
            buttonTool38,
            buttonTool13,
            buttonTool14,
            buttonTool15,
            buttonTool16,
            buttonTool36,
            buttonTool17,
            buttonTool18,
            buttonTool19,
            buttonTool20,
            buttonTool21});
            appearance17.Image = global::Idera.SQLdm.DesktopClient.Properties.Resources.Print16x16;
            buttonTool22.SharedPropsInternal.AppearancesSmall.Appearance = appearance17;
            buttonTool22.SharedPropsInternal.Caption = "Print";
            appearance18.Image = global::Idera.SQLdm.DesktopClient.Properties.Resources.Export16x16;
            buttonTool23.SharedPropsInternal.AppearancesSmall.Appearance = appearance18;
            buttonTool23.SharedPropsInternal.Caption = "Export To Excel";
            buttonTool24.SharedPropsInternal.Caption = "Configure Alerts...";
            buttonTool25.SharedPropsInternal.Caption = "Show Details";
            buttonTool26.SharedPropsInternal.Caption = "Collapse All Groups";
            buttonTool27.SharedPropsInternal.Caption = "Expand All Groups";
            appearance19.Image = global::Idera.SQLdm.DesktopClient.Properties.Resources.copy;
            buttonTool28.SharedPropsInternal.AppearancesSmall.Appearance = appearance19;
            buttonTool28.SharedPropsInternal.Caption = "Copy To Clipboard";
            buttonTool29.SharedPropsInternal.Caption = "Clear Alert";
            buttonTool30.SharedPropsInternal.Caption = "Clear All Alerts of this Type for this Instance";
            appearance31.FontData.BoldAsString = "True";
            buttonTool31.SharedPropsInternal.AppearancesSmall.Appearance = appearance31;
            buttonTool31.SharedPropsInternal.Caption = "Show Real Time View";
            buttonTool32.SharedPropsInternal.Caption = "Show Historical View";
            buttonTool35.SharedPropsInternal.Caption = "Snooze Alert...";
            buttonTool37.SharedPropsInternal.Caption = "Show Alert Help";
            buttonTool39.SharedPropsInternal.Caption = "Show Deadlock Details...";
            buttonTool39.SharedPropsInternal.Visible = false;
            buttonTool41.SharedPropsInternal.Caption = "Show Block Details...";
            buttonTool41.SharedPropsInternal.Visible = false;
            this.toolbarsManager.Tools.AddRange(new Infragistics.Win.UltraWinToolbars.ToolBase[] {
            popupMenuTool1,
            buttonTool7,
            buttonTool8,
            buttonTool9,
            buttonTool10,
            buttonTool11,
            buttonTool12,
            stateButtonTool2,
            popupMenuTool2,
            buttonTool22,
            buttonTool23,
            buttonTool24,
            buttonTool25,
            buttonTool26,
            buttonTool27,
            buttonTool28,
            buttonTool29,
            buttonTool30,
            buttonTool31,
            buttonTool32,
            buttonTool35,
            buttonTool37,
            buttonTool39,
            buttonTool41});
            this.toolbarsManager.BeforeToolDropdown += new Infragistics.Win.UltraWinToolbars.BeforeToolDropdownEventHandler(this.toolbarsManager_BeforeToolDropdown);
            this.toolbarsManager.ToolClick += new Infragistics.Win.UltraWinToolbars.ToolClickEventHandler(this.toolbarsManager_ToolClick);
            // 
            // headerStrip
            // 
            this.headerStrip.AutoSize = false;
            this.headerStrip.Font = new System.Drawing.Font("Arial", 12F, System.Drawing.FontStyle.Bold);
            this.headerStrip.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(75)))), ((int)(((byte)(75)))), ((int)(((byte)(75)))));
            this.headerStrip.GripStyle = System.Windows.Forms.ToolStripGripStyle.Hidden;
            this.headerStrip.HeaderImage = global::Idera.SQLdm.DesktopClient.Properties.Resources.NavigationPaneAlertsSmall;
            this.headerStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.titleLabel,
            this.toggleFilterOptionsPanelButton,
            this.filterAppliedLabel});
            this.headerStrip.Location = new System.Drawing.Point(0, 0);
            this.headerStrip.Name = "headerStrip";
            this.headerStrip.Padding = new System.Windows.Forms.Padding(20, 2, 0, 0);
            this.headerStrip.Size = new System.Drawing.Size(761, 25);
            this.headerStrip.TabIndex = 0;
            // 
            // titleLabel
            // 
            this.titleLabel.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(75)))), ((int)(((byte)(75)))), ((int)(((byte)(75)))));
            this.titleLabel.Name = "titleLabel";
            this.titleLabel.Size = new System.Drawing.Size(53, 20);
            this.titleLabel.Text = "Alerts";
            // 
            // toggleFilterOptionsPanelButton
            // 
            this.toggleFilterOptionsPanelButton.Alignment = System.Windows.Forms.ToolStripItemAlignment.Right;
            this.toggleFilterOptionsPanelButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.toggleFilterOptionsPanelButton.ForeColor = System.Drawing.Color.Black;
            this.toggleFilterOptionsPanelButton.Image = global::Idera.SQLdm.DesktopClient.Properties.Resources.UpArrows;
            this.toggleFilterOptionsPanelButton.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None;
            this.toggleFilterOptionsPanelButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toggleFilterOptionsPanelButton.Name = "toggleFilterOptionsPanelButton";
            this.toggleFilterOptionsPanelButton.Size = new System.Drawing.Size(23, 20);
            this.toggleFilterOptionsPanelButton.Text = "Toggle Filter Options";
            this.toggleFilterOptionsPanelButton.Click += new System.EventHandler(this.toggleFilterOptionsPanelButton_Click);
            // 
            // filterAppliedLabel
            // 
            this.filterAppliedLabel.Alignment = System.Windows.Forms.ToolStripItemAlignment.Right;
            this.filterAppliedLabel.Font = new System.Drawing.Font("Arial", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.filterAppliedLabel.ForeColor = System.Drawing.Color.Black;
            this.filterAppliedLabel.LinkBehavior = System.Windows.Forms.LinkBehavior.HoverUnderline;
            this.filterAppliedLabel.LinkColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(255)))));
            this.filterAppliedLabel.Name = "filterAppliedLabel";
            this.filterAppliedLabel.Size = new System.Drawing.Size(91, 20);
            this.filterAppliedLabel.Text = "(Filter Applied)";
            // 
            // ultraGridPrintDocument
            // 
            this.ultraGridPrintDocument.DocumentName = "Alerts";
            this.ultraGridPrintDocument.Grid = this.alertsGrid;
            // 
            // ultraPrintPreviewDialog
            // 
            this.ultraPrintPreviewDialog.Document = this.ultraGridPrintDocument;
            this.ultraPrintPreviewDialog.Name = "ultraPrintPreviewDialog";
            // 
            // _AlertsView_Toolbars_Dock_Area_Left
            // 
            this._AlertsView_Toolbars_Dock_Area_Left.AccessibleRole = System.Windows.Forms.AccessibleRole.Grouping;
            this._AlertsView_Toolbars_Dock_Area_Left.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(235)))), ((int)(((byte)(235)))), ((int)(((byte)(235)))));
            this._AlertsView_Toolbars_Dock_Area_Left.DockedPosition = Infragistics.Win.UltraWinToolbars.DockedPosition.Left;
            this._AlertsView_Toolbars_Dock_Area_Left.ForeColor = System.Drawing.SystemColors.ControlText;
            this._AlertsView_Toolbars_Dock_Area_Left.Location = new System.Drawing.Point(0, 25);
            this._AlertsView_Toolbars_Dock_Area_Left.Name = "_AlertsView_Toolbars_Dock_Area_Left";
            this._AlertsView_Toolbars_Dock_Area_Left.Size = new System.Drawing.Size(0, 575);
            this._AlertsView_Toolbars_Dock_Area_Left.ToolbarsManager = this.toolbarsManager;
            // 
            // _AlertsView_Toolbars_Dock_Area_Right
            // 
            this._AlertsView_Toolbars_Dock_Area_Right.AccessibleRole = System.Windows.Forms.AccessibleRole.Grouping;
            this._AlertsView_Toolbars_Dock_Area_Right.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(235)))), ((int)(((byte)(235)))), ((int)(((byte)(235)))));
            this._AlertsView_Toolbars_Dock_Area_Right.DockedPosition = Infragistics.Win.UltraWinToolbars.DockedPosition.Right;
            this._AlertsView_Toolbars_Dock_Area_Right.ForeColor = System.Drawing.SystemColors.ControlText;
            this._AlertsView_Toolbars_Dock_Area_Right.Location = new System.Drawing.Point(761, 25);
            this._AlertsView_Toolbars_Dock_Area_Right.Name = "_AlertsView_Toolbars_Dock_Area_Right";
            this._AlertsView_Toolbars_Dock_Area_Right.Size = new System.Drawing.Size(0, 575);
            this._AlertsView_Toolbars_Dock_Area_Right.ToolbarsManager = this.toolbarsManager;
            // 
            // _AlertsView_Toolbars_Dock_Area_Top
            // 
            this._AlertsView_Toolbars_Dock_Area_Top.AccessibleRole = System.Windows.Forms.AccessibleRole.Grouping;
            this._AlertsView_Toolbars_Dock_Area_Top.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(235)))), ((int)(((byte)(235)))), ((int)(((byte)(235)))));
            this._AlertsView_Toolbars_Dock_Area_Top.DockedPosition = Infragistics.Win.UltraWinToolbars.DockedPosition.Top;
            this._AlertsView_Toolbars_Dock_Area_Top.ForeColor = System.Drawing.SystemColors.ControlText;
            this._AlertsView_Toolbars_Dock_Area_Top.Location = new System.Drawing.Point(0, 0);
            this._AlertsView_Toolbars_Dock_Area_Top.Name = "_AlertsView_Toolbars_Dock_Area_Top";
            this._AlertsView_Toolbars_Dock_Area_Top.Size = new System.Drawing.Size(761, 0);
            this._AlertsView_Toolbars_Dock_Area_Top.ToolbarsManager = this.toolbarsManager;
            // 
            // _AlertsView_Toolbars_Dock_Area_Bottom
            // 
            this._AlertsView_Toolbars_Dock_Area_Bottom.AccessibleRole = System.Windows.Forms.AccessibleRole.Grouping;
            this._AlertsView_Toolbars_Dock_Area_Bottom.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(235)))), ((int)(((byte)(235)))), ((int)(((byte)(235)))));
            this._AlertsView_Toolbars_Dock_Area_Bottom.DockedPosition = Infragistics.Win.UltraWinToolbars.DockedPosition.Bottom;
            this._AlertsView_Toolbars_Dock_Area_Bottom.ForeColor = System.Drawing.SystemColors.ControlText;
            this._AlertsView_Toolbars_Dock_Area_Bottom.Location = new System.Drawing.Point(0, 600);
            this._AlertsView_Toolbars_Dock_Area_Bottom.Name = "_AlertsView_Toolbars_Dock_Area_Bottom";
            this._AlertsView_Toolbars_Dock_Area_Bottom.Size = new System.Drawing.Size(761, 0);
            this._AlertsView_Toolbars_Dock_Area_Bottom.ToolbarsManager = this.toolbarsManager;
            // 
            // AlertsView
            // 
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(235)))), ((int)(((byte)(235)))), ((int)(((byte)(235)))));
            this.Controls.Add(this.contentPanel);
            this.Controls.Add(this._AlertsView_Toolbars_Dock_Area_Left);
            this.Controls.Add(this._AlertsView_Toolbars_Dock_Area_Right);
            this.Controls.Add(this.headerStrip);
            this.Controls.Add(this._AlertsView_Toolbars_Dock_Area_Top);
            this.Controls.Add(this._AlertsView_Toolbars_Dock_Area_Bottom);
            this.Name = "AlertsView";
            this.Size = new System.Drawing.Size(761, 600);
            this.Load += new System.EventHandler(this.AlertsView_Load);
            this.contentPanel.ResumeLayout(false);
            this.splitContainer.Panel1.ResumeLayout(false);
            this.splitContainer.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer)).EndInit();
            this.splitContainer.ResumeLayout(false);
            this.gridPanel.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.alertsGrid)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.alertsViewDataSource)).EndInit();
            this.detailsPanel.ResumeLayout(false);
            this.detailsContentPanel.ResumeLayout(false);
            this.detailsContentPanel.PerformLayout();
            this.panel1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.severityImage)).EndInit();
            this.panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.transitionImage)).EndInit();
            this.detailsHeaderStrip.ResumeLayout(false);
            this.detailsHeaderStrip.PerformLayout();
            this.filterOptionsPanel.ResumeLayout(false);
            this.tableLayoutPanel1.ResumeLayout(false);
            this.tableLayoutPanel1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.beginTimeCombo)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.endTimeCombo)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.endDateCombo)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.tagCombo)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.beginDateCombo)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.serverCombo)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.metricCombo)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.severityCombo)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.toolbarsManager)).EndInit();
            this.headerStrip.ResumeLayout(false);
            this.headerStrip.PerformLayout();
            this.ResumeLayout(false);
            stopWatch.Stop();
            StartUpTimeLog.DebugFormat("Time taken by AlertView.InitializeComponent : {0}", stopWatch.ElapsedMilliseconds);
        }

        #endregion

        private System.Windows.Forms.Panel contentPanel;
        private Idera.SQLdm.DesktopClient.Controls.HeaderStrip headerStrip;
        private System.Windows.Forms.ToolStripLabel titleLabel;
        private System.Windows.Forms.ToolStripButton toggleFilterOptionsPanelButton;
        private System.Windows.Forms.Panel detailsPanel;
        private Idera.SQLdm.DesktopClient.Controls.HeaderStrip detailsHeaderStrip;
        private System.Windows.Forms.ToolStripLabel detailsHeaderStripLabel;
        private System.Windows.Forms.ToolStripButton toggleDetailsPanelButton;
        private System.Windows.Forms.Panel filterOptionsPanel;
        private System.Windows.Forms.Panel gridPanel;
        private Infragistics.Win.UltraWinGrid.UltraGrid alertsGrid;
        private System.Windows.Forms.Label dividerLabel1;
        private Idera.SQLdm.DesktopClient.Controls.ContextMenuManager toolbarsManager;
        private System.Windows.Forms.ToolStripLabel filterAppliedLabel;
        private AlertsViewDataSource alertsViewDataSource;
        private System.Windows.Forms.Label noSelectionLabel;
        private Infragistics.Win.UltraWinGrid.ExcelExport.UltraGridExcelExporter ultraGridExcelExporter;
        private System.Windows.Forms.SaveFileDialog saveFileDialog;
        private Infragistics.Win.UltraWinGrid.UltraGridPrintDocument ultraGridPrintDocument;
        private Infragistics.Win.Printing.UltraPrintPreviewDialog ultraPrintPreviewDialog;
        private System.Windows.Forms.TableLayoutPanel detailsContentPanel;
        private System.Windows.Forms.Label headerLabel;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.Label label10;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Label severityLabel;
        private System.Windows.Forms.PictureBox severityImage;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.Label transitionLabel;
        private System.Windows.Forms.PictureBox transitionImage;
        private System.Windows.Forms.Label label11;
        private System.Windows.Forms.Label metricLabel;
        private System.Windows.Forms.Label alertsGridStatusLabel;
        private System.Windows.Forms.TextBox messageTextBox;
        private System.Windows.Forms.LinkLabel showRealTimeViewLinkLabel;
        private System.Windows.Forms.LinkLabel helpHistoryLinkLabel1;
        private System.Windows.Forms.LinkLabel helpHistoryLinkLabel2;
        private System.Windows.Forms.SplitContainer splitContainer;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private Infragistics.Win.Misc.UltraButton clearButton;
        private Infragistics.Win.Misc.UltraButton applyButton;
        private System.Windows.Forms.Label dividerVert;
        private Common.UI.Controls.TimeComboEditor beginTimeCombo;
        private System.Windows.Forms.Label lblTo;
        private Common.UI.Controls.TimeComboEditor endTimeCombo;
        private System.Windows.Forms.Label lblFrom;
        private Infragistics.Win.UltraWinSchedule.UltraCalendarCombo endDateCombo;
        private System.Windows.Forms.Label dividerLabel;
        private System.Windows.Forms.Label label1;
        private Infragistics.Win.UltraWinEditors.UltraComboEditor tagCombo;
        private Infragistics.Win.UltraWinSchedule.UltraCalendarCombo beginDateCombo;
        private System.Windows.Forms.Label serverLabel;
        private System.Windows.Forms.Label ServerTypelabel;
        private System.Windows.Forms.Label ServerTypeText;
        private Infragistics.Win.UltraWinEditors.UltraComboEditor serverCombo;
        private System.Windows.Forms.Label metricFilterLabel;
        private Infragistics.Win.UltraWinEditors.UltraComboEditor metricCombo;
        private System.Windows.Forms.Label severityFilterLabel;
        private Infragistics.Win.UltraWinEditors.UltraComboEditor severityCombo;
        private System.Windows.Forms.RadioButton rdbtnActiveOnly;
        private System.Windows.Forms.RadioButton rdbtnTimeSpan;
        private System.Windows.Forms.LinkLabel showDetailsLinkLabel;
        private Infragistics.Win.UltraWinToolbars.UltraToolbarsDockArea _AlertsView_Toolbars_Dock_Area_Left;
        private Infragistics.Win.UltraWinToolbars.UltraToolbarsDockArea _AlertsView_Toolbars_Dock_Area_Right;
        private Infragistics.Win.UltraWinToolbars.UltraToolbarsDockArea _AlertsView_Toolbars_Dock_Area_Top;
        private Infragistics.Win.UltraWinToolbars.UltraToolbarsDockArea _AlertsView_Toolbars_Dock_Area_Bottom;
    }
}
