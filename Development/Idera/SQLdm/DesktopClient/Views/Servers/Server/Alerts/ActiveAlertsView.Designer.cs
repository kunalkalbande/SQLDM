using Idera.SQLdm.DesktopClient.Helpers;

namespace Idera.SQLdm.DesktopClient.Views.Servers.Server.Alerts
{
    partial class ActiveAlertsView
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

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            Infragistics.Win.Appearance appearance1 = new Infragistics.Win.Appearance();
            Infragistics.Win.UltraWinGrid.UltraGridBand ultraGridBand1 = new Infragistics.Win.UltraWinGrid.UltraGridBand("Band 0", -1);
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn13 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("AlertID");
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn14 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("UTCOccurrenceDateTime");
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn15 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("ServerName");
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn16 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("DatabaseName");
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn17 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("TableName");
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn18 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("Active", -1, 84474157);
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn19 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("Metric", -1, 29722407);
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn20 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("Severity", -1, 29767626);
            Infragistics.Win.Appearance appearance30 = new Infragistics.Win.Appearance();
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn21 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("StateEvent", -1, 17251657);
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn22 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("Value");
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn23 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("Heading", -1, null, 0, Infragistics.Win.UltraWinGrid.SortIndicator.Descending, false);
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn24 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("Message");
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn25 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("ServerType");
            Infragistics.Win.Appearance appearance3 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance4 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance5 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance6 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance7 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance8 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance9 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance10 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance11 = new Infragistics.Win.Appearance();
            Infragistics.Win.ValueList valueList1 = new Controls.CustomControls.CustomValueList(29722407);
            Infragistics.Win.ValueList valueList2 = new Controls.CustomControls.CustomValueList(29767626);
            Infragistics.Win.Appearance appearance12 = new Infragistics.Win.Appearance();
            Infragistics.Win.ValueList valueList3 = new Controls.CustomControls.CustomValueList(17251657);
            Infragistics.Win.ValueList valueList4 = new Controls.CustomControls.CustomValueList(84474157);
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
            Infragistics.Win.UltraWinToolbars.PopupMenuTool popupMenuTool1 = new Infragistics.Win.UltraWinToolbars.PopupMenuTool("columnContextMenu");
            Infragistics.Win.UltraWinToolbars.ButtonTool buttonTool1 = new Controls.CustomControls.CustomButtonTool("sortAscendingButton");
            Infragistics.Win.UltraWinToolbars.ButtonTool buttonTool2 = new Controls.CustomControls.CustomButtonTool("sortDescendingButton");
            Infragistics.Win.UltraWinToolbars.StateButtonTool stateButtonTool1 = new Infragistics.Win.UltraWinToolbars.StateButtonTool("groupByThisColumnButton", "");
            Infragistics.Win.UltraWinToolbars.ButtonTool buttonTool3 = new Controls.CustomControls.CustomButtonTool("toggleGroupByBoxButton");
            Infragistics.Win.UltraWinToolbars.ButtonTool buttonTool4 = new Controls.CustomControls.CustomButtonTool("removeThisColumnButton");
            Infragistics.Win.UltraWinToolbars.ButtonTool buttonTool5 = new Controls.CustomControls.CustomButtonTool("showColumnChooserButton");
            Infragistics.Win.UltraWinToolbars.ButtonTool buttonTool6 = new Controls.CustomControls.CustomButtonTool("toggleColumnTextButton");
            Infragistics.Win.UltraWinToolbars.ButtonTool buttonTool7 = new Controls.CustomControls.CustomButtonTool("showColumnChooserButton");
            Infragistics.Win.Appearance appearance13 = new Infragistics.Win.Appearance();
            Infragistics.Win.UltraWinToolbars.ButtonTool buttonTool8 = new Controls.CustomControls.CustomButtonTool("toggleGroupByBoxButton");
            Infragistics.Win.Appearance appearance14 = new Infragistics.Win.Appearance();
            Infragistics.Win.UltraWinToolbars.ButtonTool buttonTool9 = new Controls.CustomControls.CustomButtonTool("sortAscendingButton");
            Infragistics.Win.Appearance appearance15 = new Infragistics.Win.Appearance();
            Infragistics.Win.UltraWinToolbars.ButtonTool buttonTool10 = new Controls.CustomControls.CustomButtonTool("sortDescendingButton");
            Infragistics.Win.Appearance appearance16 = new Infragistics.Win.Appearance();
            Infragistics.Win.UltraWinToolbars.ButtonTool buttonTool11 = new Controls.CustomControls.CustomButtonTool("removeThisColumnButton");
            Infragistics.Win.UltraWinToolbars.ButtonTool buttonTool12 = new Controls.CustomControls.CustomButtonTool("toggleColumnTextButton");
            Infragistics.Win.UltraWinToolbars.StateButtonTool stateButtonTool2 = new Infragistics.Win.UltraWinToolbars.StateButtonTool("groupByThisColumnButton", "");
            Infragistics.Win.UltraWinToolbars.PopupMenuTool popupMenuTool2 = new Infragistics.Win.UltraWinToolbars.PopupMenuTool("gridContextMenu");
            Infragistics.Win.UltraWinToolbars.ButtonTool buttonTool41 = new Controls.CustomControls.CustomButtonTool("viewDeadlockDetailsButton");
            Infragistics.Win.UltraWinToolbars.ButtonTool buttonTool43 = new Controls.CustomControls.CustomButtonTool("viewBlockDetailsButton");
            Infragistics.Win.UltraWinToolbars.ButtonTool buttonTool33 = new Controls.CustomControls.CustomButtonTool("viewRealTimeSnapshotButton");
            Infragistics.Win.UltraWinToolbars.ButtonTool buttonTool34 = new Controls.CustomControls.CustomButtonTool("viewHistoricalSnapshotButton");
            Infragistics.Win.UltraWinToolbars.ButtonTool buttonTool38 = new Controls.CustomControls.CustomButtonTool("viewAlertHelpButton");
            Infragistics.Win.UltraWinToolbars.ButtonTool buttonTool13 = new Controls.CustomControls.CustomButtonTool("showDetailsButton");
            Infragistics.Win.UltraWinToolbars.ButtonTool buttonTool14 = new Controls.CustomControls.CustomButtonTool("clearAlertButton");
            Infragistics.Win.UltraWinToolbars.ButtonTool buttonTool15 = new Controls.CustomControls.CustomButtonTool("clearAllAlertsButton");
            Infragistics.Win.UltraWinToolbars.ButtonTool buttonTool16 = new Controls.CustomControls.CustomButtonTool("editAlertConfigurationButton");
            Infragistics.Win.UltraWinToolbars.ButtonTool buttonTool36 = new Controls.CustomControls.CustomButtonTool("snoozeAlertButton");
            Infragistics.Win.UltraWinToolbars.ButtonTool buttonTool17 = new Controls.CustomControls.CustomButtonTool("collapseAllGroupsButton");
            Infragistics.Win.UltraWinToolbars.ButtonTool buttonTool18 = new Controls.CustomControls.CustomButtonTool("expandAllGroupsButton");
            Infragistics.Win.UltraWinToolbars.ButtonTool buttonTool19 = new Controls.CustomControls.CustomButtonTool("printGridButton");
            Infragistics.Win.UltraWinToolbars.ButtonTool buttonTool20 = new Controls.CustomControls.CustomButtonTool("exportGridButton");
            Infragistics.Win.UltraWinToolbars.ButtonTool buttonTool21 = new Controls.CustomControls.CustomButtonTool("copyToClipboardButton");
            Infragistics.Win.UltraWinToolbars.ButtonTool buttonTool22 = new Controls.CustomControls.CustomButtonTool("printGridButton");
            Infragistics.Win.Appearance appearance17 = new Infragistics.Win.Appearance();
            Infragistics.Win.UltraWinToolbars.ButtonTool buttonTool23 = new Controls.CustomControls.CustomButtonTool("exportGridButton");
            Infragistics.Win.Appearance appearance18 = new Infragistics.Win.Appearance();
            Infragistics.Win.UltraWinToolbars.ButtonTool buttonTool24 = new Controls.CustomControls.CustomButtonTool("editAlertConfigurationButton");
            Infragistics.Win.UltraWinToolbars.ButtonTool buttonTool25 = new Controls.CustomControls.CustomButtonTool("showDetailsButton");
            Infragistics.Win.UltraWinToolbars.ButtonTool buttonTool26 = new Controls.CustomControls.CustomButtonTool("collapseAllGroupsButton");
            Infragistics.Win.UltraWinToolbars.ButtonTool buttonTool27 = new Controls.CustomControls.CustomButtonTool("expandAllGroupsButton");
            Infragistics.Win.UltraWinToolbars.ButtonTool buttonTool28 = new Controls.CustomControls.CustomButtonTool("copyToClipboardButton");
            Infragistics.Win.Appearance appearance19 = new Infragistics.Win.Appearance();
            Infragistics.Win.UltraWinToolbars.ButtonTool buttonTool29 = new Controls.CustomControls.CustomButtonTool("clearAlertButton");
            Infragistics.Win.UltraWinToolbars.ButtonTool buttonTool30 = new Controls.CustomControls.CustomButtonTool("clearAllAlertsButton");
            Infragistics.Win.UltraWinToolbars.ButtonTool buttonTool31 = new Controls.CustomControls.CustomButtonTool("viewRealTimeSnapshotButton");
            Infragistics.Win.Appearance appearance31 = new Infragistics.Win.Appearance();
            Infragistics.Win.UltraWinToolbars.ButtonTool buttonTool32 = new Controls.CustomControls.CustomButtonTool("viewHistoricalSnapshotButton");
            Infragistics.Win.UltraWinToolbars.ButtonTool buttonTool35 = new Controls.CustomControls.CustomButtonTool("snoozeAlertButton");
            Infragistics.Win.UltraWinToolbars.ButtonTool buttonTool37 = new Controls.CustomControls.CustomButtonTool("viewAlertHelpButton");
            Infragistics.Win.UltraWinToolbars.ButtonTool buttonTool39 = new Controls.CustomControls.CustomButtonTool("viewDeadlockDetailsButton");
            Infragistics.Win.UltraWinToolbars.ButtonTool buttonTool40 = new Controls.CustomControls.CustomButtonTool("viewBlockDetailsButton");
            this.activeAlerts_Fill_Panel = new Idera.SQLdm.DesktopClient.Controls.GradientPanel();
            this.splitContainer = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomSplitContainer();
            this.alertsGrid = new Infragistics.Win.UltraWinGrid.UltraGrid();
            this.alertsViewDataSource = new Idera.SQLdm.DesktopClient.Views.Alerts.AlertsViewDataSource();
            this.forecastPanel = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomPanel ();
            this.forecastHeaderStrip = new Idera.SQLdm.DesktopClient.Controls.HeaderStrip();
            this.toolStripLabel1 = new System.Windows.Forms.ToolStripLabel();
            this.toggleForecastPanelButton = new System.Windows.Forms.ToolStripButton();
            this.alertForecastPanel = new Idera.SQLdm.DesktopClient.Controls.AlertForecastPanel();
            this.operationalStatusPanel = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomPanel ();
            this.operationalStatusImage = new System.Windows.Forms.PictureBox();
            this.operationalStatusLabel = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomLabel();
            this.detailsPanel = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomPanel ();
            this.detailsContentPanel = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomTableLayoutPanel();
            this.showDetailsLinkLabel = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomLinkLabel();
            this.helpHistoryLinkLabel2 = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomLinkLabel();
            this.headerLabel = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomLabel();
            this.helpHistoryLinkLabel1 = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomLinkLabel();
            this.label2 = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomLabel();
            this.label3 = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomLabel();
            this.label4 = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomLabel();
            this.label5 = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomLabel();
            this.label6 = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomLabel();
            this.label8 = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomLabel();
            this.label9 = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomLabel();
            this.label10 = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomLabel();
            this.ServerTypelabel = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomLabel();
            this.ServerTypeText = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomLabel();
            this.panel1 = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomPanel ();
            this.severityLabel = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomLabel();
            this.severityImage = new System.Windows.Forms.PictureBox();
            this.label7 = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomLabel();
            this.panel2 = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomPanel ();
            this.transitionLabel = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomLabel();
            this.transitionImage = new System.Windows.Forms.PictureBox();
            this.label11 = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomLabel();
            this.metricLabel = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomLabel();
            this.messageTextBox = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomTextBox();
            this.showRealTimeViewLinkLabel = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomLinkLabel();
            this.detailsHeaderStrip = new Idera.SQLdm.DesktopClient.Controls.HeaderStrip();
            this.detailsHeaderStripLabel = new System.Windows.Forms.ToolStripLabel();
            this.toggleDetailsPanelButton = new System.Windows.Forms.ToolStripButton();
            this.noSelectionLabel = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomLabel();
            this._activeAlerts_Fill_Panel_Toolbars_Dock_Area_Left = new Infragistics.Win.UltraWinToolbars.UltraToolbarsDockArea();
            this.toolbarsManager = new Idera.SQLdm.DesktopClient.Controls.ContextMenuManager(this.components);
            this._activeAlerts_Fill_Panel_Toolbars_Dock_Area_Right = new Infragistics.Win.UltraWinToolbars.UltraToolbarsDockArea();
            this._activeAlerts_Fill_Panel_Toolbars_Dock_Area_Top = new Infragistics.Win.UltraWinToolbars.UltraToolbarsDockArea();
            this._activeAlerts_Fill_Panel_Toolbars_Dock_Area_Bottom = new Infragistics.Win.UltraWinToolbars.UltraToolbarsDockArea();
            this.ultraGridPrintDocument = new Infragistics.Win.UltraWinGrid.UltraGridPrintDocument(this.components);
            this.ultraGridExcelExporter = new Infragistics.Win.UltraWinGrid.ExcelExport.UltraGridExcelExporter(this.components);
            this.ultraPrintPreviewDialog = new Infragistics.Win.Printing.UltraPrintPreviewDialog(this.components);
            this.saveFileDialog = new System.Windows.Forms.SaveFileDialog();
            this.activeAlerts_Fill_Panel.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer)).BeginInit();
            this.splitContainer.Panel1.SuspendLayout();
            this.splitContainer.Panel2.SuspendLayout();
            this.splitContainer.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.alertsGrid)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.alertsViewDataSource)).BeginInit();
            this.forecastPanel.SuspendLayout();
            this.forecastHeaderStrip.SuspendLayout();
            this.operationalStatusPanel.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.operationalStatusImage)).BeginInit();
            this.detailsPanel.SuspendLayout();
            this.detailsContentPanel.SuspendLayout();
            this.panel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.severityImage)).BeginInit();
            this.panel2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.transitionImage)).BeginInit();
            this.detailsHeaderStrip.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.toolbarsManager)).BeginInit();
            this.SuspendLayout();
            // 
            // activeAlerts_Fill_Panel
            // 
            this.activeAlerts_Fill_Panel.BackColor2 = System.Drawing.SystemColors.Control;
            this.activeAlerts_Fill_Panel.BorderColor = System.Drawing.Color.Black;
            this.activeAlerts_Fill_Panel.Controls.Add(this.splitContainer);
            this.activeAlerts_Fill_Panel.Controls.Add(this._activeAlerts_Fill_Panel_Toolbars_Dock_Area_Left);
            this.activeAlerts_Fill_Panel.Controls.Add(this._activeAlerts_Fill_Panel_Toolbars_Dock_Area_Right);
            this.activeAlerts_Fill_Panel.Controls.Add(this._activeAlerts_Fill_Panel_Toolbars_Dock_Area_Top);
            this.activeAlerts_Fill_Panel.Controls.Add(this._activeAlerts_Fill_Panel_Toolbars_Dock_Area_Bottom);
            this.activeAlerts_Fill_Panel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.activeAlerts_Fill_Panel.Location = new System.Drawing.Point(0, 0);
            this.activeAlerts_Fill_Panel.Name = "activeAlerts_Fill_Panel";
            this.activeAlerts_Fill_Panel.Padding = new System.Windows.Forms.Padding(1);
            this.activeAlerts_Fill_Panel.Size = new System.Drawing.Size(680, 580);
            this.activeAlerts_Fill_Panel.TabIndex = 0;
            // 
            // splitContainer
            // 
            this.splitContainer.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(203)))), ((int)(((byte)(203)))), ((int)(((byte)(203)))));
            this.splitContainer.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer.Location = new System.Drawing.Point(1, 1);
            this.splitContainer.Name = "splitContainer";
            this.splitContainer.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // splitContainer.Panel1
            // 
            this.splitContainer.Panel1.Controls.Add(this.alertsGrid);
            this.splitContainer.Panel1.Controls.Add(this.forecastPanel);
            this.splitContainer.Panel1.Controls.Add(this.operationalStatusPanel);
            this.splitContainer.Panel1.Padding = new System.Windows.Forms.Padding(0, 1, 0, 1);
            // 
            // splitContainer.Panel2
            // 
            this.splitContainer.Panel2.AutoScroll = true;
            this.splitContainer.Panel2.Controls.Add(this.detailsPanel);
            this.splitContainer.Size = new System.Drawing.Size(678, 578);
            this.splitContainer.SplitterDistance = 234;
            this.splitContainer.TabIndex = 17;
            this.splitContainer.MouseDown += new System.Windows.Forms.MouseEventHandler(this.splitContainer_MouseDown);
            this.splitContainer.MouseUp += new System.Windows.Forms.MouseEventHandler(this.splitContainer_MouseUp);
            // 
            // alertsGrid
            // 
            this.alertsGrid.DataMember = "Band 0";
            this.alertsGrid.DataSource = this.alertsViewDataSource;
            appearance1.BackColor = System.Drawing.SystemColors.Window;
            appearance1.BorderColor = System.Drawing.SystemColors.InactiveCaption;
            this.alertsGrid.DisplayLayout.Appearance = appearance1;
            this.alertsGrid.DisplayLayout.AutoFitStyle = Infragistics.Win.UltraWinGrid.AutoFitStyle.ExtendLastColumn;
            ultraGridColumn13.AutoCompleteMode = Infragistics.Win.AutoCompleteMode.Append;
            ultraGridColumn13.ExcludeFromColumnChooser = Infragistics.Win.UltraWinGrid.ExcludeFromColumnChooser.True;
            ultraGridColumn13.Header.VisiblePosition = 12;
            ultraGridColumn13.Hidden = true;
            ultraGridColumn13.Width = 75;
            ultraGridColumn14.AutoCompleteMode = Infragistics.Win.AutoCompleteMode.Append;
            ultraGridColumn14.AutoSizeMode = Infragistics.Win.UltraWinGrid.ColumnAutoSizeMode.None;
            ultraGridColumn14.Format = "G";
            ultraGridColumn14.GroupByMode = Infragistics.Win.UltraWinGrid.GroupByMode.OutlookDate;
            ultraGridColumn14.Header.Caption = "Time";
            ultraGridColumn14.Header.VisiblePosition = 3;
            ultraGridColumn14.Style = Infragistics.Win.UltraWinGrid.ColumnStyle.DateTime;
            ultraGridColumn14.Width = 131;
            ultraGridColumn15.AutoCompleteMode = Infragistics.Win.AutoCompleteMode.Append;
            ultraGridColumn15.AutoSizeMode = Infragistics.Win.UltraWinGrid.ColumnAutoSizeMode.None;
            ultraGridColumn15.ExcludeFromColumnChooser = Infragistics.Win.UltraWinGrid.ExcludeFromColumnChooser.True;
            ultraGridColumn15.Header.Caption = "Instance";
            ultraGridColumn15.Header.VisiblePosition = 4;
            ultraGridColumn15.Hidden = true;
            ultraGridColumn15.Width = 179;
            ultraGridColumn16.AutoCompleteMode = Infragistics.Win.AutoCompleteMode.Append;
            ultraGridColumn16.Header.Caption = "Database";
            ultraGridColumn16.Header.VisiblePosition = 7;
            ultraGridColumn16.Hidden = true;
            ultraGridColumn16.Width = 122;
            ultraGridColumn17.AutoCompleteMode = Infragistics.Win.AutoCompleteMode.Append;
            ultraGridColumn17.Header.VisiblePosition = 9;
            ultraGridColumn17.Hidden = true;
            ultraGridColumn17.Width = 131;
            ultraGridColumn18.AutoCompleteMode = Infragistics.Win.AutoCompleteMode.Append;
            ultraGridColumn18.ExcludeFromColumnChooser = Infragistics.Win.UltraWinGrid.ExcludeFromColumnChooser.False;
            ultraGridColumn18.Header.VisiblePosition = 2;
            ultraGridColumn18.Hidden = true;
            ultraGridColumn18.Width = 50;
            ultraGridColumn19.AutoCompleteMode = Infragistics.Win.AutoCompleteMode.Append;
            ultraGridColumn19.CellDisplayStyle = Infragistics.Win.UltraWinGrid.CellDisplayStyle.FormattedText;
            ultraGridColumn19.Header.VisiblePosition = 8;
            ultraGridColumn19.Hidden = true;
            ultraGridColumn19.Width = 52;
            ultraGridColumn20.AutoCompleteMode = Infragistics.Win.AutoCompleteMode.Append;
            ultraGridColumn20.ColumnChooserCaption = "Severity";
            ultraGridColumn20.ExcludeFromColumnChooser = Infragistics.Win.UltraWinGrid.ExcludeFromColumnChooser.False;
            appearance30.FontData.BoldAsString = "True";
            appearance30.ForeColor = System.Drawing.Color.Red;
            appearance30.TextHAlignAsString = "Center";
            ultraGridColumn20.Header.Appearance = appearance30;
            ultraGridColumn20.Header.Caption = "!";
            ultraGridColumn20.Header.VisiblePosition = 0;
            ultraGridColumn20.Width = 20;
            if (AutoScaleSizeHelper.isScalingRequired)
                ultraGridColumn20.Width = 50;
            ultraGridColumn21.AutoCompleteMode = Infragistics.Win.AutoCompleteMode.Append;
            ultraGridColumn21.Header.Caption = "Change";
            ultraGridColumn21.Header.VisiblePosition = 1;
            ultraGridColumn21.Width = 134;
            ultraGridColumn22.AutoCompleteMode = Infragistics.Win.AutoCompleteMode.Append;
            ultraGridColumn22.ExcludeFromColumnChooser = Infragistics.Win.UltraWinGrid.ExcludeFromColumnChooser.True;
            ultraGridColumn22.Header.VisiblePosition = 10;
            ultraGridColumn22.Hidden = true;
            ultraGridColumn22.Width = 64;
            ultraGridColumn23.AutoCompleteMode = Infragistics.Win.AutoCompleteMode.Append;
            ultraGridColumn23.Header.Caption = "Summary";
            ultraGridColumn23.Header.VisiblePosition = 6;
            ultraGridColumn23.Width = 411;
            ultraGridColumn24.AutoCompleteMode = Infragistics.Win.AutoCompleteMode.Append;
            ultraGridColumn24.Header.Caption = "Details";
            ultraGridColumn24.Header.VisiblePosition = 11;
            ultraGridColumn24.Hidden = true;
            ultraGridColumn24.Width = 95;
            ultraGridColumn25.AutoCompleteMode = Infragistics.Win.AutoCompleteMode.Append;
            ultraGridColumn25.Header.Caption = "Server Type";
            ultraGridColumn25.Header.VisiblePosition = 5;
            ultraGridColumn25.Width = 240;
            ultraGridBand1.Columns.AddRange(new object[] {
            ultraGridColumn13,
            ultraGridColumn14,
            ultraGridColumn15,
            ultraGridColumn16,
            ultraGridColumn17,
            ultraGridColumn18,
            ultraGridColumn19,
            ultraGridColumn20,
            ultraGridColumn21,
            ultraGridColumn22,
            ultraGridColumn23,
            ultraGridColumn24,
            ultraGridColumn25});
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
            this.alertsGrid.Location = new System.Drawing.Point(0, 131);
            this.alertsGrid.Name = "alertsGrid";
            this.alertsGrid.Size = new System.Drawing.Size(678, 102);
            this.alertsGrid.TabIndex = 4;
            this.alertsGrid.InitializeGroupByRow += new Infragistics.Win.UltraWinGrid.InitializeGroupByRowEventHandler(this.alertsGrid_InitializeGroupByRow);
            this.alertsGrid.AfterRowRegionSize += new Infragistics.Win.UltraWinGrid.RowScrollRegionEventHandler(this.alertsGrid_AfterRowRegionSize);
            this.alertsGrid.AfterSelectChange += new Infragistics.Win.UltraWinGrid.AfterSelectChangeEventHandler(this.alertsGrid_AfterSelectChange);
            this.alertsGrid.DoubleClickRow += new Infragistics.Win.UltraWinGrid.DoubleClickRowEventHandler(this.alertsGrid_DoubleClickRow);
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
            ultraDataColumn2.AllowDBNull = Infragistics.Win.DefaultableBoolean.True;
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
            // forecastPanel
            // 
            this.forecastPanel.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(203)))), ((int)(((byte)(203)))), ((int)(((byte)(203)))));
            this.forecastPanel.Controls.Add(this.forecastHeaderStrip);
            this.forecastPanel.Controls.Add(this.alertForecastPanel);
            this.forecastPanel.Dock = System.Windows.Forms.DockStyle.Top;
            this.forecastPanel.Location = new System.Drawing.Point(0, 28);
            this.forecastPanel.Margin = new System.Windows.Forms.Padding(0);
            this.forecastPanel.Name = "forecastPanel";
            this.forecastPanel.Padding = new System.Windows.Forms.Padding(0, 1, 0, 3);
            this.forecastPanel.Size = new System.Drawing.Size(678, 103);
            this.forecastPanel.TabIndex = 3;
            // 
            // forecastHeaderStrip
            // 
            this.forecastHeaderStrip.AutoSize = false;
            this.forecastHeaderStrip.Font = new System.Drawing.Font("Tahoma", 8.25F);
            this.forecastHeaderStrip.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(75)))), ((int)(((byte)(75)))), ((int)(((byte)(75)))));
            this.forecastHeaderStrip.GripStyle = System.Windows.Forms.ToolStripGripStyle.Hidden;
            this.forecastHeaderStrip.HotTrackEnabled = false;
            this.forecastHeaderStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripLabel1,
            this.toggleForecastPanelButton});
            this.forecastHeaderStrip.Location = new System.Drawing.Point(0, 1);
            this.forecastHeaderStrip.Name = "forecastHeaderStrip";
            this.forecastHeaderStrip.Padding = new System.Windows.Forms.Padding(0);
            this.forecastHeaderStrip.Size = new System.Drawing.Size(678, 19);
            this.forecastHeaderStrip.Style = Idera.SQLdm.DesktopClient.Controls.HeaderStripStyle.Small;
            this.forecastHeaderStrip.TabIndex = 0;
            // 
            // toolStripLabel1
            // 
            this.toolStripLabel1.Font = new System.Drawing.Font("Arial", 8F, System.Drawing.FontStyle.Bold);
            this.toolStripLabel1.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(245)))), ((int)(((byte)(245)))));
            this.toolStripLabel1.Name = "toolStripLabel1";
            this.toolStripLabel1.Size = new System.Drawing.Size(99, 16);
            this.toolStripLabel1.Text = "12 Hour Forecast";
            // 
            // toggleForecastPanelButton
            // 
            this.toggleForecastPanelButton.Alignment = System.Windows.Forms.ToolStripItemAlignment.Right;
            this.toggleForecastPanelButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.toggleForecastPanelButton.Image = global::Idera.SQLdm.DesktopClient.Properties.Resources.Office2007Close;
            this.toggleForecastPanelButton.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None;
            this.toggleForecastPanelButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toggleForecastPanelButton.Name = "toggleForecastPanelButton";
            this.toggleForecastPanelButton.Size = new System.Drawing.Size(23, 16);
            this.toggleForecastPanelButton.Click += new System.EventHandler(this.toggleForecastPanelButton_Click);
            // 
            // alertForecastPanel
            // 
            this.alertForecastPanel.BackColor = System.Drawing.Color.White;
            this.alertForecastPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.alertForecastPanel.InHistoryMode = false;
            this.alertForecastPanel.Location = new System.Drawing.Point(0, 1);
            this.alertForecastPanel.Margin = new System.Windows.Forms.Padding(0);
            this.alertForecastPanel.MinimumSize = new System.Drawing.Size(383, 78);
            this.alertForecastPanel.Name = "alertForecastPanel";
            this.alertForecastPanel.Size = new System.Drawing.Size(678, 99);
            this.alertForecastPanel.TabIndex = 0;
            // 
            // operationalStatusPanel
            // 
            this.operationalStatusPanel.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(203)))), ((int)(((byte)(203)))), ((int)(((byte)(203)))));
            this.operationalStatusPanel.Controls.Add(this.operationalStatusImage);
            this.operationalStatusPanel.Controls.Add(this.operationalStatusLabel);
            this.operationalStatusPanel.Dock = System.Windows.Forms.DockStyle.Top;
            this.operationalStatusPanel.Location = new System.Drawing.Point(0, 1);
            this.operationalStatusPanel.Name = "operationalStatusPanel";
            this.operationalStatusPanel.Size = new System.Drawing.Size(678, 27);
            this.operationalStatusPanel.TabIndex = 16;
            this.operationalStatusPanel.Visible = false;
            // 
            // operationalStatusImage
            // 
            this.operationalStatusImage.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(212)))), ((int)(((byte)(212)))), ((int)(((byte)(212)))));
            this.operationalStatusImage.Image = global::Idera.SQLdm.DesktopClient.Properties.Resources.StatusWarningSmall;
            this.operationalStatusImage.Location = new System.Drawing.Point(7, 5);
            this.operationalStatusImage.Name = "operationalStatusImage";
            this.operationalStatusImage.Size = new System.Drawing.Size(16, 16);
            this.operationalStatusImage.SizeMode = System.Windows.Forms.PictureBoxSizeMode.AutoSize;
            this.operationalStatusImage.TabIndex = 3;
            this.operationalStatusImage.TabStop = false;
            // 
            // operationalStatusLabel
            // 
            this.operationalStatusLabel.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.operationalStatusLabel.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(212)))), ((int)(((byte)(212)))), ((int)(((byte)(212)))));
            this.operationalStatusLabel.ForeColor = System.Drawing.Color.Black;
            this.operationalStatusLabel.Location = new System.Drawing.Point(4, 3);
            this.operationalStatusLabel.Name = "operationalStatusLabel";
            this.operationalStatusLabel.Padding = new System.Windows.Forms.Padding(20, 0, 0, 0);
            this.operationalStatusLabel.Size = new System.Drawing.Size(670, 21);
            this.operationalStatusLabel.TabIndex = 0;
            this.operationalStatusLabel.Text = "< status message >";
            this.operationalStatusLabel.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.operationalStatusLabel.MouseDown += new System.Windows.Forms.MouseEventHandler(this.operationalStatusLabel_MouseDown);
            this.operationalStatusLabel.MouseEnter += new System.EventHandler(this.operationalStatusLabel_MouseEnter);
            this.operationalStatusLabel.MouseLeave += new System.EventHandler(this.operationalStatusLabel_MouseLeave);
            this.operationalStatusLabel.MouseUp += new System.Windows.Forms.MouseEventHandler(this.operationalStatusLabel_MouseUp);
            // 
            // detailsPanel
            // 
            this.detailsPanel.BackColor = System.Drawing.SystemColors.Control;
            this.detailsPanel.Controls.Add(this.detailsContentPanel);
            this.detailsPanel.Controls.Add(this.detailsHeaderStrip);
            this.detailsPanel.Controls.Add(this.noSelectionLabel);
            this.detailsPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.detailsPanel.Location = new System.Drawing.Point(0, 0);
            this.detailsPanel.Name = "detailsPanel";
            this.detailsPanel.Size = new System.Drawing.Size(678, 340);
            this.detailsPanel.TabIndex = 2;
            // 
            // detailsContentPanel
            // 
            this.detailsContentPanel.ColumnCount = 7;
            this.detailsContentPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 70F));
            this.detailsContentPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 180F));
            this.detailsContentPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 70F));
            this.detailsContentPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 150F));
            this.detailsContentPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 80F));
            this.detailsContentPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 220F));
            this.detailsContentPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 150F));
            this.detailsContentPanel.Controls.Add(this.showDetailsLinkLabel, 6, 3);
            this.detailsContentPanel.Controls.Add(this.helpHistoryLinkLabel2, 6, 2);
            this.detailsContentPanel.Controls.Add(this.headerLabel, 1, 3);
            this.detailsContentPanel.Controls.Add(this.ServerTypelabel, 4, 0);
            this.detailsContentPanel.Controls.Add(this.ServerTypeText, 5, 0);
            this.detailsContentPanel.Controls.Add(this.helpHistoryLinkLabel1, 6, 1);
            this.detailsContentPanel.Controls.Add(this.label2, 0, 0);
            this.detailsContentPanel.Controls.Add(this.label3, 2, 0);
            this.detailsContentPanel.Controls.Add(this.label4, 0, 2);
            this.detailsContentPanel.Controls.Add(this.label5, 2, 2);
            this.detailsContentPanel.Controls.Add(this.label6, 0, 3);
            this.detailsContentPanel.Controls.Add(this.label8, 3, 0);
            this.detailsContentPanel.Controls.Add(this.label9, 1, 2);
            this.detailsContentPanel.Controls.Add(this.label10, 3, 2);
            this.detailsContentPanel.Controls.Add(this.panel1, 1, 0);
            this.detailsContentPanel.Controls.Add(this.label7, 0, 1);
            this.detailsContentPanel.Controls.Add(this.panel2, 1, 1);
            this.detailsContentPanel.Controls.Add(this.label11, 2, 1);
            this.detailsContentPanel.Controls.Add(this.metricLabel, 3, 1);
            this.detailsContentPanel.Controls.Add(this.messageTextBox, 0, 4);
            this.detailsContentPanel.Controls.Add(this.showRealTimeViewLinkLabel, 6, 0);
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
            this.detailsContentPanel.Size = new System.Drawing.Size(678, 321);
            this.detailsContentPanel.TabIndex = 8;
            // 
            // showDetailsLinkLabel
            // 
            this.showDetailsLinkLabel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.showDetailsLinkLabel.Location = new System.Drawing.Point(558, 60);
            this.showDetailsLinkLabel.Name = "showDetailsLinkLabel";
            this.showDetailsLinkLabel.Size = new System.Drawing.Size(117, 20);
            this.showDetailsLinkLabel.TabIndex = 17;
            this.showDetailsLinkLabel.TabStop = true;
            this.showDetailsLinkLabel.Text = "Show Block Details";
            this.showDetailsLinkLabel.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.showDetailsLinkLabel.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.showDetailsLinkLabel_LinkClicked);
            // 
            // helpHistoryLinkLabel2
            // 
            this.helpHistoryLinkLabel2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.helpHistoryLinkLabel2.Location = new System.Drawing.Point(558, 40);
            this.helpHistoryLinkLabel2.Name = "helpHistoryLinkLabel2";
            this.helpHistoryLinkLabel2.Size = new System.Drawing.Size(117, 20);
            this.helpHistoryLinkLabel2.TabIndex = 6;
            this.helpHistoryLinkLabel2.TabStop = true;
            this.helpHistoryLinkLabel2.Text = "helpHistoryLinkLabel2";
            this.helpHistoryLinkLabel2.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.helpHistoryLinkLabel2.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.helpHistoryLinkLabel2_LinkClicked);
            // 
            // headerLabel
            // 
            this.detailsContentPanel.SetColumnSpan(this.headerLabel, 3);
            this.headerLabel.DataBindings.Add(new System.Windows.Forms.Binding("Text", this.alertsViewDataSource, "Band 0.Heading", true));
            this.headerLabel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.headerLabel.Location = new System.Drawing.Point(65, 63);
            this.headerLabel.Margin = new System.Windows.Forms.Padding(0, 3, 3, 0);
            this.headerLabel.Name = "headerLabel";
            this.headerLabel.Size = new System.Drawing.Size(487, 17);
            this.headerLabel.TabIndex = 16;
            this.headerLabel.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // helpHistoryLinkLabel1
            // 
            this.helpHistoryLinkLabel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.helpHistoryLinkLabel1.Location = new System.Drawing.Point(558, 20);
            this.helpHistoryLinkLabel1.Name = "helpHistoryLinkLabel1";
            this.helpHistoryLinkLabel1.Size = new System.Drawing.Size(117, 20);
            this.helpHistoryLinkLabel1.TabIndex = 5;
            this.helpHistoryLinkLabel1.TabStop = true;
            this.helpHistoryLinkLabel1.Text = "helpHistoryLinkLabel1";
            this.helpHistoryLinkLabel1.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.helpHistoryLinkLabel1.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.helpHistoryLinkLabel1_LinkClicked);
            // 
            // label2
            // 
            this.label2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.label2.Location = new System.Drawing.Point(3, 3);
            this.label2.Margin = new System.Windows.Forms.Padding(3, 3, 3, 0);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(59, 17);
            this.label2.TabIndex = 0;
            this.label2.Text = "Severity:";
            this.label2.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // label3
            // 
            this.label3.Dock = System.Windows.Forms.DockStyle.Fill;
            this.label3.Location = new System.Drawing.Point(326, 3);
            this.label3.Margin = new System.Windows.Forms.Padding(3, 3, 3, 0);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(59, 17);
            this.label3.TabIndex = 1;
            this.label3.Text = "Time:";
            this.label3.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // label4
            // 
            this.label4.Dock = System.Windows.Forms.DockStyle.Fill;
            this.label4.Location = new System.Drawing.Point(3, 43);
            this.label4.Margin = new System.Windows.Forms.Padding(3, 3, 3, 0);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(59, 17);
            this.label4.TabIndex = 2;
            this.label4.Text = "Server:";
            this.label4.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // label5
            // 
            this.label5.Dock = System.Windows.Forms.DockStyle.Fill;
            this.label5.Location = new System.Drawing.Point(326, 43);
            this.label5.Margin = new System.Windows.Forms.Padding(3, 3, 3, 0);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(59, 17);
            this.label5.TabIndex = 3;
            this.label5.Text = "Database:";
            this.label5.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // label6
            // 
            this.label6.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.label6.Location = new System.Drawing.Point(3, 63);
            this.label6.Margin = new System.Windows.Forms.Padding(3, 3, 3, 0);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(59, 17);
            this.label6.TabIndex = 4;
            this.label6.Text = "Summary:";
            this.label6.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // label8
            // 
            this.label8.DataBindings.Add(new System.Windows.Forms.Binding("Text", this.alertsViewDataSource, "Band 0.UTCOccurrenceDateTime", true));
            this.label8.Dock = System.Windows.Forms.DockStyle.Fill;
            this.label8.Location = new System.Drawing.Point(391, 3);
            this.label8.Margin = new System.Windows.Forms.Padding(3, 3, 3, 0);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(161, 17);
            this.label8.TabIndex = 1;
            this.label8.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // label9
            // 
            this.label9.DataBindings.Add(new System.Windows.Forms.Binding("Text", this.alertsViewDataSource, "Band 0.ServerName", true));
            this.label9.Dock = System.Windows.Forms.DockStyle.Fill;
            this.label9.Location = new System.Drawing.Point(65, 43);
            this.label9.Margin = new System.Windows.Forms.Padding(0, 3, 3, 0);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(255, 17);
            this.label9.TabIndex = 0;
            this.label9.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // label10
            // 
            this.label10.DataBindings.Add(new System.Windows.Forms.Binding("Text", this.alertsViewDataSource, "Band 0.DatabaseName", true));
            this.label10.Dock = System.Windows.Forms.DockStyle.Fill;
            this.label10.Location = new System.Drawing.Point(391, 43);
            this.label10.Margin = new System.Windows.Forms.Padding(3, 3, 3, 0);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(161, 17);
            this.label10.TabIndex = 3;
            this.label10.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
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
            // panel1
            // 
            this.panel1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.panel1.Controls.Add(this.severityLabel);
            this.panel1.Controls.Add(this.severityImage);
            this.panel1.Location = new System.Drawing.Point(65, 0);
            this.panel1.Margin = new System.Windows.Forms.Padding(0);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(258, 20);
            this.panel1.TabIndex = 10;
            // 
            // severityLabel
            // 
            this.severityLabel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.severityLabel.Location = new System.Drawing.Point(16, 0);
            this.severityLabel.Name = "severityLabel";
            this.severityLabel.Size = new System.Drawing.Size(242, 20);
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
            this.label7.AutoSize = true;
            this.label7.Dock = System.Windows.Forms.DockStyle.Fill;
            this.label7.Location = new System.Drawing.Point(3, 23);
            this.label7.Margin = new System.Windows.Forms.Padding(3, 3, 3, 0);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(59, 17);
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
            this.panel2.Location = new System.Drawing.Point(65, 20);
            this.panel2.Margin = new System.Windows.Forms.Padding(0);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(258, 20);
            this.panel2.TabIndex = 12;
            // 
            // transitionLabel
            // 
            this.transitionLabel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.transitionLabel.Location = new System.Drawing.Point(16, 0);
            this.transitionLabel.Name = "transitionLabel";
            this.transitionLabel.Size = new System.Drawing.Size(242, 20);
            this.transitionLabel.TabIndex = 0;
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
            this.label11.Dock = System.Windows.Forms.DockStyle.Fill;
            this.label11.Location = new System.Drawing.Point(326, 23);
            this.label11.Margin = new System.Windows.Forms.Padding(3, 3, 3, 0);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(59, 17);
            this.label11.TabIndex = 13;
            this.label11.Text = "Metric:";
            this.label11.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // metricLabel
            // 
            this.metricLabel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.metricLabel.Location = new System.Drawing.Point(391, 23);
            this.metricLabel.Margin = new System.Windows.Forms.Padding(3, 3, 3, 0);
            this.metricLabel.Name = "metricLabel";
            this.metricLabel.Size = new System.Drawing.Size(161, 17);
            this.metricLabel.TabIndex = 2;
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
            this.messageTextBox.Size = new System.Drawing.Size(672, 235);
            this.messageTextBox.TabIndex = 8;
            // 
            // showRealTimeViewLinkLabel
            // 
            this.showRealTimeViewLinkLabel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.showRealTimeViewLinkLabel.Location = new System.Drawing.Point(558, 0);
            this.showRealTimeViewLinkLabel.Name = "showRealTimeViewLinkLabel";
            this.showRealTimeViewLinkLabel.Size = new System.Drawing.Size(117, 20);
            this.showRealTimeViewLinkLabel.TabIndex = 4;
            this.showRealTimeViewLinkLabel.TabStop = true;
            this.showRealTimeViewLinkLabel.Text = "Show Real Time View";
            this.showRealTimeViewLinkLabel.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.showRealTimeViewLinkLabel.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.showRealTimeViewLinkLabel_LinkClicked);
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
            this.detailsHeaderStrip.Size = new System.Drawing.Size(678, 19);
            this.detailsHeaderStrip.Style = Idera.SQLdm.DesktopClient.Controls.HeaderStripStyle.Small;
            this.detailsHeaderStrip.TabIndex = 0;
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
            this.noSelectionLabel.Size = new System.Drawing.Size(678, 340);
            this.noSelectionLabel.TabIndex = 9;
            this.noSelectionLabel.Text = "Select an alert to view its details.";
            this.noSelectionLabel.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            // 
            // _activeAlerts_Fill_Panel_Toolbars_Dock_Area_Left
            // 
            this._activeAlerts_Fill_Panel_Toolbars_Dock_Area_Left.AccessibleRole = System.Windows.Forms.AccessibleRole.Grouping;
            this._activeAlerts_Fill_Panel_Toolbars_Dock_Area_Left.BackColor = System.Drawing.SystemColors.Control;
            this._activeAlerts_Fill_Panel_Toolbars_Dock_Area_Left.DockedPosition = Infragistics.Win.UltraWinToolbars.DockedPosition.Left;
            this._activeAlerts_Fill_Panel_Toolbars_Dock_Area_Left.ForeColor = System.Drawing.SystemColors.ControlText;
            this._activeAlerts_Fill_Panel_Toolbars_Dock_Area_Left.Location = new System.Drawing.Point(1, 1);
            this._activeAlerts_Fill_Panel_Toolbars_Dock_Area_Left.Name = "_activeAlerts_Fill_Panel_Toolbars_Dock_Area_Left";
            this._activeAlerts_Fill_Panel_Toolbars_Dock_Area_Left.Size = new System.Drawing.Size(0, 578);
            this._activeAlerts_Fill_Panel_Toolbars_Dock_Area_Left.ToolbarsManager = this.toolbarsManager;
            // 
            // toolbarsManager
            // 
            this.toolbarsManager.DesignerFlags = 0;
            this.toolbarsManager.DockWithinContainer = this.activeAlerts_Fill_Panel;
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
            buttonTool41,
            buttonTool43,
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
            buttonTool40.SharedPropsInternal.Caption = "Show Block Details...";
            buttonTool40.SharedPropsInternal.Visible = false;
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
            buttonTool40});
            this.toolbarsManager.BeforeToolDropdown += new Infragistics.Win.UltraWinToolbars.BeforeToolDropdownEventHandler(this.toolbarsManager_BeforeToolDropdown);
            this.toolbarsManager.ToolClick += new Infragistics.Win.UltraWinToolbars.ToolClickEventHandler(this.toolbarsManager_ToolClick);
            // 
            // _activeAlerts_Fill_Panel_Toolbars_Dock_Area_Right
            // 
            this._activeAlerts_Fill_Panel_Toolbars_Dock_Area_Right.AccessibleRole = System.Windows.Forms.AccessibleRole.Grouping;
            this._activeAlerts_Fill_Panel_Toolbars_Dock_Area_Right.BackColor = System.Drawing.SystemColors.Control;
            this._activeAlerts_Fill_Panel_Toolbars_Dock_Area_Right.DockedPosition = Infragistics.Win.UltraWinToolbars.DockedPosition.Right;
            this._activeAlerts_Fill_Panel_Toolbars_Dock_Area_Right.ForeColor = System.Drawing.SystemColors.ControlText;
            this._activeAlerts_Fill_Panel_Toolbars_Dock_Area_Right.Location = new System.Drawing.Point(679, 1);
            this._activeAlerts_Fill_Panel_Toolbars_Dock_Area_Right.Name = "_activeAlerts_Fill_Panel_Toolbars_Dock_Area_Right";
            this._activeAlerts_Fill_Panel_Toolbars_Dock_Area_Right.Size = new System.Drawing.Size(0, 578);
            this._activeAlerts_Fill_Panel_Toolbars_Dock_Area_Right.ToolbarsManager = this.toolbarsManager;
            // 
            // _activeAlerts_Fill_Panel_Toolbars_Dock_Area_Top
            // 
            this._activeAlerts_Fill_Panel_Toolbars_Dock_Area_Top.AccessibleRole = System.Windows.Forms.AccessibleRole.Grouping;
            this._activeAlerts_Fill_Panel_Toolbars_Dock_Area_Top.BackColor = System.Drawing.SystemColors.Control;
            this._activeAlerts_Fill_Panel_Toolbars_Dock_Area_Top.DockedPosition = Infragistics.Win.UltraWinToolbars.DockedPosition.Top;
            this._activeAlerts_Fill_Panel_Toolbars_Dock_Area_Top.ForeColor = System.Drawing.SystemColors.ControlText;
            this._activeAlerts_Fill_Panel_Toolbars_Dock_Area_Top.Location = new System.Drawing.Point(1, 1);
            this._activeAlerts_Fill_Panel_Toolbars_Dock_Area_Top.Name = "_activeAlerts_Fill_Panel_Toolbars_Dock_Area_Top";
            this._activeAlerts_Fill_Panel_Toolbars_Dock_Area_Top.Size = new System.Drawing.Size(678, 0);
            this._activeAlerts_Fill_Panel_Toolbars_Dock_Area_Top.ToolbarsManager = this.toolbarsManager;
            // 
            // _activeAlerts_Fill_Panel_Toolbars_Dock_Area_Bottom
            // 
            this._activeAlerts_Fill_Panel_Toolbars_Dock_Area_Bottom.AccessibleRole = System.Windows.Forms.AccessibleRole.Grouping;
            this._activeAlerts_Fill_Panel_Toolbars_Dock_Area_Bottom.BackColor = System.Drawing.SystemColors.Control;
            this._activeAlerts_Fill_Panel_Toolbars_Dock_Area_Bottom.DockedPosition = Infragistics.Win.UltraWinToolbars.DockedPosition.Bottom;
            this._activeAlerts_Fill_Panel_Toolbars_Dock_Area_Bottom.ForeColor = System.Drawing.SystemColors.ControlText;
            this._activeAlerts_Fill_Panel_Toolbars_Dock_Area_Bottom.Location = new System.Drawing.Point(1, 579);
            this._activeAlerts_Fill_Panel_Toolbars_Dock_Area_Bottom.Name = "_activeAlerts_Fill_Panel_Toolbars_Dock_Area_Bottom";
            this._activeAlerts_Fill_Panel_Toolbars_Dock_Area_Bottom.Size = new System.Drawing.Size(678, 0);
            this._activeAlerts_Fill_Panel_Toolbars_Dock_Area_Bottom.ToolbarsManager = this.toolbarsManager;
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
            // ActiveAlertsView
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.activeAlerts_Fill_Panel);
            this.Name = "ActiveAlertsView";
            this.Size = new System.Drawing.Size(680, 580);
            this.Load += new System.EventHandler(this.ActiveAlertsView_Load);
            this.activeAlerts_Fill_Panel.ResumeLayout(false);
            this.splitContainer.Panel1.ResumeLayout(false);
            this.splitContainer.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer)).EndInit();
            this.splitContainer.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.alertsGrid)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.alertsViewDataSource)).EndInit();
            this.forecastPanel.ResumeLayout(false);
            this.forecastHeaderStrip.ResumeLayout(false);
            this.forecastHeaderStrip.PerformLayout();
            this.operationalStatusPanel.ResumeLayout(false);
            this.operationalStatusPanel.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.operationalStatusImage)).EndInit();
            this.detailsPanel.ResumeLayout(false);
            this.detailsContentPanel.ResumeLayout(false);
            this.detailsContentPanel.PerformLayout();
            this.panel1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.severityImage)).EndInit();
            this.panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.transitionImage)).EndInit();
            this.detailsHeaderStrip.ResumeLayout(false);
            this.detailsHeaderStrip.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.toolbarsManager)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private Idera.SQLdm.DesktopClient.Controls.GradientPanel activeAlerts_Fill_Panel;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomPanel  detailsPanel;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomTableLayoutPanel detailsContentPanel;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomLinkLabel helpHistoryLinkLabel1;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomLabel headerLabel;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomLabel label2;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomLabel label3;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomLabel label4;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomLabel label5;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomLabel label6;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomLabel label8;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomLabel label9;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomLabel label10;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomLabel ServerTypeText;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomLabel ServerTypelabel;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomPanel  panel1;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomLabel severityLabel;
        private System.Windows.Forms.PictureBox severityImage;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomLabel label7;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomPanel  panel2;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomLabel transitionLabel;
        private System.Windows.Forms.PictureBox transitionImage;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomLabel label11;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomLabel metricLabel;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomTextBox messageTextBox;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomLinkLabel showRealTimeViewLinkLabel;
        private Idera.SQLdm.DesktopClient.Controls.HeaderStrip detailsHeaderStrip;
        private System.Windows.Forms.ToolStripLabel detailsHeaderStripLabel;
        private System.Windows.Forms.ToolStripButton toggleDetailsPanelButton;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomLabel noSelectionLabel;
        private Idera.SQLdm.DesktopClient.Views.Alerts.AlertsViewDataSource alertsViewDataSource;
        private Infragistics.Win.UltraWinGrid.UltraGrid alertsGrid;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomPanel  operationalStatusPanel;
        private System.Windows.Forms.PictureBox operationalStatusImage;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomLabel operationalStatusLabel;
        private Infragistics.Win.UltraWinGrid.UltraGridPrintDocument ultraGridPrintDocument;
        private Infragistics.Win.UltraWinGrid.ExcelExport.UltraGridExcelExporter ultraGridExcelExporter;
        private Infragistics.Win.Printing.UltraPrintPreviewDialog ultraPrintPreviewDialog;
        private System.Windows.Forms.SaveFileDialog saveFileDialog;
        private Idera.SQLdm.DesktopClient.Controls.ContextMenuManager toolbarsManager;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomLinkLabel helpHistoryLinkLabel2;
        private System.Windows.Forms.SplitContainer splitContainer;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomPanel  forecastPanel;
        private Controls.HeaderStrip forecastHeaderStrip;
        private System.Windows.Forms.ToolStripLabel toolStripLabel1;
        private System.Windows.Forms.ToolStripButton toggleForecastPanelButton;
        private Controls.AlertForecastPanel alertForecastPanel;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomLinkLabel showDetailsLinkLabel;
        private Infragistics.Win.UltraWinToolbars.UltraToolbarsDockArea _activeAlerts_Fill_Panel_Toolbars_Dock_Area_Left;
        private Infragistics.Win.UltraWinToolbars.UltraToolbarsDockArea _activeAlerts_Fill_Panel_Toolbars_Dock_Area_Right;
        private Infragistics.Win.UltraWinToolbars.UltraToolbarsDockArea _activeAlerts_Fill_Panel_Toolbars_Dock_Area_Top;
        private Infragistics.Win.UltraWinToolbars.UltraToolbarsDockArea _activeAlerts_Fill_Panel_Toolbars_Dock_Area_Bottom;
    }
}
