namespace Idera.SQLdm.DesktopClient.Views
{
    using Idera.SQLdm.DesktopClient.Helpers;

    partial class SQLdmTodayView
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
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn27 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("TaskID", -1, null, 0, Infragistics.Win.UltraWinGrid.SortIndicator.Descending, false);
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn28 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("Status", -1, 38482376);
            Infragistics.Win.Appearance appearance34 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance35 = new Infragistics.Win.Appearance();
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn29 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("Subject");
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn30 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("Message");
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn31 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("ServerName");
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn32 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("Comments");
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn33 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("Owner");
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn34 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("CreatedOn");
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn35 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("CompletedOn");
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn36 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("Metric", -1, 38532641);
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn37 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("Severity", -1, 38456063);
            Infragistics.Win.Appearance appearance36 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance37 = new Infragistics.Win.Appearance();
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn38 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("Value");
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn39 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("EventID");
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn40 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("StatusBoolean");
            Infragistics.Win.Appearance appearance38 = new Infragistics.Win.Appearance();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(SQLdmTodayView));
            Infragistics.Win.Appearance appearance7 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance8 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance9 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance10 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance11 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance12 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance13 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance14 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance15 = new Infragistics.Win.Appearance();
            Infragistics.Win.ValueList valueList1 = new Infragistics.Win.ValueList(38456063);
            Infragistics.Win.ValueList valueList2 = new Infragistics.Win.ValueList(38482376);
            Infragistics.Win.ValueList valueList3 = new Infragistics.Win.ValueList(38532641);
            Infragistics.Win.UltraWinDataSource.UltraDataColumn ultraDataColumn1 = new Infragistics.Win.UltraWinDataSource.UltraDataColumn("TaskID");
            Infragistics.Win.UltraWinDataSource.UltraDataColumn ultraDataColumn2 = new Infragistics.Win.UltraWinDataSource.UltraDataColumn("Status");
            Infragistics.Win.UltraWinDataSource.UltraDataColumn ultraDataColumn3 = new Infragistics.Win.UltraWinDataSource.UltraDataColumn("Subject");
            Infragistics.Win.UltraWinDataSource.UltraDataColumn ultraDataColumn4 = new Infragistics.Win.UltraWinDataSource.UltraDataColumn("Message");
            Infragistics.Win.UltraWinDataSource.UltraDataColumn ultraDataColumn5 = new Infragistics.Win.UltraWinDataSource.UltraDataColumn("ServerName");
            Infragistics.Win.UltraWinDataSource.UltraDataColumn ultraDataColumn6 = new Infragistics.Win.UltraWinDataSource.UltraDataColumn("Comments");
            Infragistics.Win.UltraWinDataSource.UltraDataColumn ultraDataColumn7 = new Infragistics.Win.UltraWinDataSource.UltraDataColumn("Owner");
            Infragistics.Win.UltraWinDataSource.UltraDataColumn ultraDataColumn8 = new Infragistics.Win.UltraWinDataSource.UltraDataColumn("CreatedOn");
            Infragistics.Win.UltraWinDataSource.UltraDataColumn ultraDataColumn9 = new Infragistics.Win.UltraWinDataSource.UltraDataColumn("CompletedOn");
            Infragistics.Win.UltraWinDataSource.UltraDataColumn ultraDataColumn10 = new Infragistics.Win.UltraWinDataSource.UltraDataColumn("Metric");
            Infragistics.Win.UltraWinDataSource.UltraDataColumn ultraDataColumn11 = new Infragistics.Win.UltraWinDataSource.UltraDataColumn("Severity");
            Infragistics.Win.UltraWinDataSource.UltraDataColumn ultraDataColumn12 = new Infragistics.Win.UltraWinDataSource.UltraDataColumn("Value");
            Infragistics.Win.UltraWinDataSource.UltraDataColumn ultraDataColumn13 = new Infragistics.Win.UltraWinDataSource.UltraDataColumn("EventID");
            Infragistics.Win.UltraWinDataSource.UltraDataColumn ultraDataColumn14 = new Infragistics.Win.UltraWinDataSource.UltraDataColumn("StatusBoolean");
            Infragistics.Win.Appearance appearance2 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance39 = new Infragistics.Win.Appearance();
            Infragistics.Win.UltraWinTabControl.UltraTab ultraTab1 = new Infragistics.Win.UltraWinTabControl.UltraTab();
            Infragistics.Win.Appearance appearance41 = new Infragistics.Win.Appearance();
            Infragistics.Win.UltraWinTabControl.UltraTab ultraTab2 = new Infragistics.Win.UltraWinTabControl.UltraTab();
            Infragistics.Win.Appearance appearance40 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance16 = new Infragistics.Win.Appearance();
            Infragistics.Win.UltraWinGrid.UltraGridBand ultraGridBand2 = new Infragistics.Win.UltraWinGrid.UltraGridBand("Band 0", -1);
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn41 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("AlertID");
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn42 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("UTCOccurrenceDateTime");
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn43 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("ServerName");
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn44 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("DatabaseName");
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn45 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("TableName");
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn46 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("Active");
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn47 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("Metric", -1, 34096297);
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn48 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("Severity", -1, 34107751);
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn49 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("StateEvent", -1, 34116813);
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn50 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("Value");
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn51 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("Heading");
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn52 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("Message");
            Infragistics.Win.Appearance appearance17 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance18 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance19 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance20 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance21 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance22 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance23 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance24 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance25 = new Infragistics.Win.Appearance();
            Infragistics.Win.ValueList valueList4 = new Infragistics.Win.ValueList(34096297);
            Infragistics.Win.ValueList valueList5 = new Infragistics.Win.ValueList(34107751);
            Infragistics.Win.ValueList valueList6 = new Infragistics.Win.ValueList(34116813);
            Infragistics.Win.UltraWinDataSource.UltraDataColumn ultraDataColumn15 = new Infragistics.Win.UltraWinDataSource.UltraDataColumn("AlertID");
            Infragistics.Win.UltraWinDataSource.UltraDataColumn ultraDataColumn16 = new Infragistics.Win.UltraWinDataSource.UltraDataColumn("UTCOccurrenceDateTime");
            Infragistics.Win.UltraWinDataSource.UltraDataColumn ultraDataColumn17 = new Infragistics.Win.UltraWinDataSource.UltraDataColumn("ServerName");
            Infragistics.Win.UltraWinDataSource.UltraDataColumn ultraDataColumn18 = new Infragistics.Win.UltraWinDataSource.UltraDataColumn("DatabaseName");
            Infragistics.Win.UltraWinDataSource.UltraDataColumn ultraDataColumn19 = new Infragistics.Win.UltraWinDataSource.UltraDataColumn("TableName");
            Infragistics.Win.UltraWinDataSource.UltraDataColumn ultraDataColumn20 = new Infragistics.Win.UltraWinDataSource.UltraDataColumn("Active");
            Infragistics.Win.UltraWinDataSource.UltraDataColumn ultraDataColumn21 = new Infragistics.Win.UltraWinDataSource.UltraDataColumn("Metric");
            Infragistics.Win.UltraWinDataSource.UltraDataColumn ultraDataColumn22 = new Infragistics.Win.UltraWinDataSource.UltraDataColumn("Severity");
            Infragistics.Win.UltraWinDataSource.UltraDataColumn ultraDataColumn23 = new Infragistics.Win.UltraWinDataSource.UltraDataColumn("StateEvent");
            Infragistics.Win.UltraWinDataSource.UltraDataColumn ultraDataColumn24 = new Infragistics.Win.UltraWinDataSource.UltraDataColumn("Value");
            Infragistics.Win.UltraWinDataSource.UltraDataColumn ultraDataColumn25 = new Infragistics.Win.UltraWinDataSource.UltraDataColumn("Heading");
            Infragistics.Win.UltraWinDataSource.UltraDataColumn ultraDataColumn26 = new Infragistics.Win.UltraWinDataSource.UltraDataColumn("Message");
            Infragistics.Win.UltraWinToolbars.PopupMenuTool popupMenuTool1 = new Infragistics.Win.UltraWinToolbars.PopupMenuTool("columnContextMenu");
            Infragistics.Win.UltraWinToolbars.ButtonTool buttonTool1 = new Infragistics.Win.UltraWinToolbars.ButtonTool("sortAscendingButton");
            Infragistics.Win.UltraWinToolbars.ButtonTool buttonTool2 = new Infragistics.Win.UltraWinToolbars.ButtonTool("sortDescendingButton");
            Infragistics.Win.UltraWinToolbars.StateButtonTool stateButtonTool1 = new Infragistics.Win.UltraWinToolbars.StateButtonTool("groupByThisColumnButton", "");
            Infragistics.Win.UltraWinToolbars.ButtonTool buttonTool3 = new Infragistics.Win.UltraWinToolbars.ButtonTool("toggleGroupByBoxButton");
            Infragistics.Win.UltraWinToolbars.ButtonTool buttonTool4 = new Infragistics.Win.UltraWinToolbars.ButtonTool("removeThisColumnButton");
            Infragistics.Win.UltraWinToolbars.ButtonTool buttonTool5 = new Infragistics.Win.UltraWinToolbars.ButtonTool("showColumnChooserButton");
            Infragistics.Win.UltraWinToolbars.ButtonTool buttonTool6 = new Infragistics.Win.UltraWinToolbars.ButtonTool("showColumnChooserButton");
            Infragistics.Win.Appearance appearance26 = new Infragistics.Win.Appearance();
            Infragistics.Win.UltraWinToolbars.ButtonTool buttonTool7 = new Infragistics.Win.UltraWinToolbars.ButtonTool("toggleGroupByBoxButton");
            Infragistics.Win.Appearance appearance27 = new Infragistics.Win.Appearance();
            Infragistics.Win.UltraWinToolbars.ButtonTool buttonTool8 = new Infragistics.Win.UltraWinToolbars.ButtonTool("sortAscendingButton");
            Infragistics.Win.Appearance appearance28 = new Infragistics.Win.Appearance();
            Infragistics.Win.UltraWinToolbars.ButtonTool buttonTool9 = new Infragistics.Win.UltraWinToolbars.ButtonTool("sortDescendingButton");
            Infragistics.Win.Appearance appearance29 = new Infragistics.Win.Appearance();
            Infragistics.Win.UltraWinToolbars.ButtonTool buttonTool10 = new Infragistics.Win.UltraWinToolbars.ButtonTool("removeThisColumnButton");
            Infragistics.Win.UltraWinToolbars.StateButtonTool stateButtonTool2 = new Infragistics.Win.UltraWinToolbars.StateButtonTool("groupByThisColumnButton", "");
            Infragistics.Win.UltraWinToolbars.PopupMenuTool popupMenuTool2 = new Infragistics.Win.UltraWinToolbars.PopupMenuTool("gridContextMenu");
            Infragistics.Win.UltraWinToolbars.ButtonTool buttonTool48 = new Infragistics.Win.UltraWinToolbars.ButtonTool("viewDeadlockDetailsButton");
            Infragistics.Win.UltraWinToolbars.ButtonTool buttonTool38 = new Infragistics.Win.UltraWinToolbars.ButtonTool("viewAlertRealTimeSnapshotButton");
            Infragistics.Win.UltraWinToolbars.ButtonTool buttonTool39 = new Infragistics.Win.UltraWinToolbars.ButtonTool("viewAlertHistoricalSnapshotButton");
            Infragistics.Win.UltraWinToolbars.ButtonTool buttonTool43 = new Infragistics.Win.UltraWinToolbars.ButtonTool("viewAlertHelpButton");
            Infragistics.Win.UltraWinToolbars.ButtonTool buttonTool11 = new Infragistics.Win.UltraWinToolbars.ButtonTool("clearAlertButton");
            Infragistics.Win.UltraWinToolbars.ButtonTool buttonTool12 = new Infragistics.Win.UltraWinToolbars.ButtonTool("clearAllAlertsButton");
            Infragistics.Win.UltraWinToolbars.ButtonTool buttonTool13 = new Infragistics.Win.UltraWinToolbars.ButtonTool("configureAlertsButton");
            Infragistics.Win.UltraWinToolbars.ButtonTool buttonTool47 = new Infragistics.Win.UltraWinToolbars.ButtonTool("snoozeAlertButton");
            Infragistics.Win.UltraWinToolbars.ButtonTool buttonTool14 = new Infragistics.Win.UltraWinToolbars.ButtonTool("copyToClipboardButton");
            Infragistics.Win.UltraWinToolbars.ButtonTool buttonTool15 = new Infragistics.Win.UltraWinToolbars.ButtonTool("printGridButton");
            Infragistics.Win.Appearance appearance30 = new Infragistics.Win.Appearance();
            Infragistics.Win.UltraWinToolbars.ButtonTool buttonTool16 = new Infragistics.Win.UltraWinToolbars.ButtonTool("exportGridButton");
            Infragistics.Win.Appearance appearance31 = new Infragistics.Win.Appearance();
            Infragistics.Win.UltraWinToolbars.ButtonTool buttonTool17 = new Infragistics.Win.UltraWinToolbars.ButtonTool("configureAlertsButton");
            Infragistics.Win.UltraWinToolbars.PopupMenuTool popupMenuTool3 = new Infragistics.Win.UltraWinToolbars.PopupMenuTool("taskGridContextMenu");
            Infragistics.Win.UltraWinToolbars.ButtonTool buttonTool46 = new Infragistics.Win.UltraWinToolbars.ButtonTool("viewTaskRealTimeSnapshotButton");
            Infragistics.Win.UltraWinToolbars.ButtonTool buttonTool45 = new Infragistics.Win.UltraWinToolbars.ButtonTool("viewTaskHistoricalSnapshotButton");
            Infragistics.Win.UltraWinToolbars.PopupMenuTool popupMenuTool4 = new Infragistics.Win.UltraWinToolbars.PopupMenuTool("taskStatusMenu");
            Infragistics.Win.UltraWinToolbars.ButtonTool buttonTool18 = new Infragistics.Win.UltraWinToolbars.ButtonTool("deleteButton");
            Infragistics.Win.UltraWinToolbars.ButtonTool buttonTool20 = new Infragistics.Win.UltraWinToolbars.ButtonTool("copyToClipboardButton");
            Infragistics.Win.UltraWinToolbars.ButtonTool buttonTool21 = new Infragistics.Win.UltraWinToolbars.ButtonTool("propertiesButton");
            Infragistics.Win.UltraWinToolbars.ButtonTool buttonTool22 = new Infragistics.Win.UltraWinToolbars.ButtonTool("deleteButton");
            Infragistics.Win.UltraWinToolbars.ButtonTool buttonTool23 = new Infragistics.Win.UltraWinToolbars.ButtonTool("showAssociatedViewButton");
            Infragistics.Win.UltraWinToolbars.ButtonTool buttonTool24 = new Infragistics.Win.UltraWinToolbars.ButtonTool("propertiesButton");
            Infragistics.Win.Appearance appearance65 = new Infragistics.Win.Appearance();
            Infragistics.Win.UltraWinToolbars.PopupMenuTool popupMenuTool5 = new Infragistics.Win.UltraWinToolbars.PopupMenuTool("taskStatusMenu");
            Infragistics.Win.UltraWinToolbars.ButtonTool buttonTool25 = new Infragistics.Win.UltraWinToolbars.ButtonTool("notStartedButton");
            Infragistics.Win.UltraWinToolbars.ButtonTool buttonTool26 = new Infragistics.Win.UltraWinToolbars.ButtonTool("inProgressButton");
            Infragistics.Win.UltraWinToolbars.ButtonTool buttonTool27 = new Infragistics.Win.UltraWinToolbars.ButtonTool("onHoldButton");
            Infragistics.Win.UltraWinToolbars.ButtonTool buttonTool28 = new Infragistics.Win.UltraWinToolbars.ButtonTool("completedButton");
            Infragistics.Win.UltraWinToolbars.ButtonTool buttonTool29 = new Infragistics.Win.UltraWinToolbars.ButtonTool("notStartedButton");
            Infragistics.Win.UltraWinToolbars.ButtonTool buttonTool30 = new Infragistics.Win.UltraWinToolbars.ButtonTool("inProgressButton");
            Infragistics.Win.UltraWinToolbars.ButtonTool buttonTool31 = new Infragistics.Win.UltraWinToolbars.ButtonTool("onHoldButton");
            Infragistics.Win.UltraWinToolbars.ButtonTool buttonTool32 = new Infragistics.Win.UltraWinToolbars.ButtonTool("completedButton");
            Infragistics.Win.UltraWinToolbars.ButtonTool buttonTool33 = new Infragistics.Win.UltraWinToolbars.ButtonTool("copyToClipboardButton");
            Infragistics.Win.Appearance appearance32 = new Infragistics.Win.Appearance();
            Infragistics.Win.UltraWinToolbars.ButtonTool buttonTool34 = new Infragistics.Win.UltraWinToolbars.ButtonTool("clearAlertButton");
            Infragistics.Win.UltraWinToolbars.ButtonTool buttonTool35 = new Infragistics.Win.UltraWinToolbars.ButtonTool("clearAllAlertsButton");
            Infragistics.Win.UltraWinToolbars.ButtonTool buttonTool36 = new Infragistics.Win.UltraWinToolbars.ButtonTool("viewAlertRealTimeSnapshotButton");
            Infragistics.Win.Appearance appearance43 = new Infragistics.Win.Appearance();
            Infragistics.Win.UltraWinToolbars.ButtonTool buttonTool37 = new Infragistics.Win.UltraWinToolbars.ButtonTool("viewAlertHistoricalSnapshotButton");
            Infragistics.Win.UltraWinToolbars.ButtonTool buttonTool40 = new Infragistics.Win.UltraWinToolbars.ButtonTool("viewTaskRealTimeSnapshotButton");
            Infragistics.Win.UltraWinToolbars.ButtonTool buttonTool41 = new Infragistics.Win.UltraWinToolbars.ButtonTool("viewTaskHistoricalSnapshotButton");
            Infragistics.Win.UltraWinToolbars.ButtonTool buttonTool19 = new Infragistics.Win.UltraWinToolbars.ButtonTool("snoozeAlertButton");
            Infragistics.Win.UltraWinToolbars.ButtonTool buttonTool42 = new Infragistics.Win.UltraWinToolbars.ButtonTool("viewAlertHelpButton");
            Infragistics.Win.UltraWinToolbars.ButtonTool buttonTool44 = new Infragistics.Win.UltraWinToolbars.ButtonTool("viewDeadlockDetailsButton");
            this.ultraTabPageControl1 = new Infragistics.Win.UltraWinTabControl.UltraTabPageControl();
            this.tasksPanel = new System.Windows.Forms.Panel();
            this.tasksGrid = new Infragistics.Win.UltraWinGrid.UltraGrid();
            this.tasksGridDataSource = new Idera.SQLdm.DesktopClient.Views.TaskGridDataSource(this.components);
            this.tasksStatusLabel = new System.Windows.Forms.Label();
            this.ultraTabPageControl2 = new Infragistics.Win.UltraWinTabControl.UltraTabPageControl();
            this.panel1 = new System.Windows.Forms.Panel();
            this.alertForecastPanel1 = new Idera.SQLdm.DesktopClient.Controls.AlertForecastPanel();
            this.contentPanel = new Idera.SQLdm.DesktopClient.Controls.GradientPanel();
            this.contentSplitter = new System.Windows.Forms.SplitContainer();
            this.contentLayoutPanel = new System.Windows.Forms.TableLayoutPanel();
            this.columnDividerLabel = new System.Windows.Forms.Label();
            this.statusSummaryPanel = new System.Windows.Forms.Panel();
            this.statusSummaryContentPanel = new System.Windows.Forms.Panel();
            this.statusSummaryLabel = new System.Windows.Forms.LinkLabel();
            this.statusSummaryPictureBox = new System.Windows.Forms.PictureBox();
            this.okServersLabel = new System.Windows.Forms.LinkLabel();
            this.statusSummaryDividerLabel = new System.Windows.Forms.Label();
            this.maintenanceModeServersLabel = new System.Windows.Forms.LinkLabel();
            this.statusSummaryDescriptionLabel = new System.Windows.Forms.LinkLabel();
            this.warningServersLabel = new System.Windows.Forms.LinkLabel();
            this.criticalServersPictureBox = new System.Windows.Forms.PictureBox();
            this.criticalServersLabel = new System.Windows.Forms.LinkLabel();
            this.warningServersPictureBox = new System.Windows.Forms.PictureBox();
            this.okServersPictureBox = new System.Windows.Forms.PictureBox();
            this.maintenanceModeServersPictureBox = new System.Windows.Forms.PictureBox();
            this.statusSummaryLoadingLabel = new System.Windows.Forms.Label();
            this.statusSummaryHeaderLabel = new Idera.SQLdm.DesktopClient.Controls.HeaderLabel();
            this.tasksForecastTabGroup = new Infragistics.Win.UltraWinTabControl.UltraTabControl();
            this.ultraTabSharedControlsPage1 = new Infragistics.Win.UltraWinTabControl.UltraTabSharedControlsPage();
            this.alertsPanel = new System.Windows.Forms.Panel();
            this.alertsGrid = new Infragistics.Win.UltraWinGrid.UltraGrid();
            this.alertsViewDataSource = new Idera.SQLdm.DesktopClient.Views.Alerts.AlertsViewDataSource();
            this.recentAlertsHeaderLabel = new Idera.SQLdm.DesktopClient.Controls.HeaderLabel();
            this.alertsStatusLabel = new System.Windows.Forms.Label();
            this.commonTasksPanel = new System.Windows.Forms.Panel();
            this.commonTasksLayoutPanel = new System.Windows.Forms.TableLayoutPanel();
            this.generateReportsFeatureButton = new Idera.SQLdm.DesktopClient.Controls.FeatureButton();
            this.manageTasksFeatureButton = new Idera.SQLdm.DesktopClient.Controls.FeatureButton();
            this.monitorAlertLogFeatureButton = new Idera.SQLdm.DesktopClient.Controls.FeatureButton();
            this.addServersFeatureButton = new Idera.SQLdm.DesktopClient.Controls.FeatureButton();
            this.commonTasksHeaderStrip = new Idera.SQLdm.DesktopClient.Controls.HeaderStrip();
            this.commonTasksHeaderTitleLabel = new System.Windows.Forms.ToolStripLabel();
            this.toggleCommonTasksButton = new System.Windows.Forms.ToolStripButton();
            this.headerPanel = new System.Windows.Forms.Panel();
            this.headerDividerLabel = new System.Windows.Forms.Label();
            this.refreshTimeLabel = new System.Windows.Forms.Label();
            this.refreshDateLabel = new System.Windows.Forms.Label();
            this.headerStrip = new Idera.SQLdm.DesktopClient.Controls.HeaderStrip();
            this.titleLabel = new System.Windows.Forms.ToolStripLabel();
            this.toolbarsManager = new Idera.SQLdm.DesktopClient.Controls.ContextMenuManager(this.components);
            this.ultraGridPrintDocument = new Infragistics.Win.UltraWinGrid.UltraGridPrintDocument(this.components);
            this.ultraGridExcelExporter = new Infragistics.Win.UltraWinGrid.ExcelExport.UltraGridExcelExporter(this.components);
            this.ultraPrintPreviewDialog = new Infragistics.Win.Printing.UltraPrintPreviewDialog(this.components);
            this.saveFileDialog = new System.Windows.Forms.SaveFileDialog();
            this.ultraTabPageControl1.SuspendLayout();
            this.tasksPanel.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.tasksGrid)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.tasksGridDataSource)).BeginInit();
            this.ultraTabPageControl2.SuspendLayout();
            this.panel1.SuspendLayout();
            this.contentPanel.SuspendLayout();
            this.contentSplitter.Panel1.SuspendLayout();
            this.contentSplitter.Panel2.SuspendLayout();
            this.contentSplitter.SuspendLayout();
            this.contentLayoutPanel.SuspendLayout();
            this.statusSummaryPanel.SuspendLayout();
            this.statusSummaryContentPanel.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.statusSummaryPictureBox)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.criticalServersPictureBox)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.warningServersPictureBox)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.okServersPictureBox)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.maintenanceModeServersPictureBox)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.tasksForecastTabGroup)).BeginInit();
            this.tasksForecastTabGroup.SuspendLayout();
            this.alertsPanel.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.alertsGrid)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.alertsViewDataSource)).BeginInit();
            this.commonTasksPanel.SuspendLayout();
            this.commonTasksLayoutPanel.SuspendLayout();
            this.commonTasksHeaderStrip.SuspendLayout();
            this.headerPanel.SuspendLayout();
            this.headerStrip.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.toolbarsManager)).BeginInit();
            this.SuspendLayout();
            // 
            // ultraTabPageControl1
            // 
            this.ultraTabPageControl1.Controls.Add(this.tasksPanel);
            this.ultraTabPageControl1.Location = new System.Drawing.Point(1, 23);
            this.ultraTabPageControl1.Name = "ultraTabPageControl1";
            this.ultraTabPageControl1.Size = new System.Drawing.Size(392, 138);
            // 
            // tasksPanel
            // 
            this.tasksPanel.Controls.Add(this.tasksGrid);
            this.tasksPanel.Controls.Add(this.tasksStatusLabel);
            this.tasksPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tasksPanel.Location = new System.Drawing.Point(0, 0);
            this.tasksPanel.Name = "tasksPanel";
            this.tasksPanel.Size = new System.Drawing.Size(392, 138);
            this.tasksPanel.TabIndex = 5;
            // 
            // tasksGrid
            // 
            this.tasksGrid.DataMember = "Band 0";
            this.tasksGrid.DataSource = this.tasksGridDataSource;
            appearance1.BackColor = System.Drawing.SystemColors.Window;
            appearance1.BorderColor = System.Drawing.SystemColors.InactiveCaption;
            this.tasksGrid.DisplayLayout.Appearance = appearance1;
            this.tasksGrid.DisplayLayout.AutoFitStyle = Infragistics.Win.UltraWinGrid.AutoFitStyle.ExtendLastColumn;
            ultraGridColumn27.AutoCompleteMode = Infragistics.Win.AutoCompleteMode.Append;
            ultraGridColumn27.ExcludeFromColumnChooser = Infragistics.Win.UltraWinGrid.ExcludeFromColumnChooser.True;
            ultraGridColumn27.Header.VisiblePosition = 0;
            ultraGridColumn27.Hidden = true;
            ultraGridColumn28.AutoCompleteMode = Infragistics.Win.AutoCompleteMode.Append;
            appearance34.ImageHAlign = Infragistics.Win.HAlign.Center;
            ultraGridColumn28.CellAppearance = appearance34;
            appearance35.ImageHAlign = Infragistics.Win.HAlign.Center;
            ultraGridColumn28.Header.Appearance = appearance35;
            ultraGridColumn28.Header.Caption = "";
            ultraGridColumn28.Header.ToolTipText = "Status";
            ultraGridColumn28.Header.VisiblePosition = 2;
            ultraGridColumn28.MaxWidth = 24;
            ultraGridColumn28.MinWidth = 24;
            ultraGridColumn28.Width = 24;
            ultraGridColumn29.AutoCompleteMode = Infragistics.Win.AutoCompleteMode.Append;
            ultraGridColumn29.CellActivation = Infragistics.Win.UltraWinGrid.Activation.NoEdit;
            ultraGridColumn29.Header.VisiblePosition = 8;
            ultraGridColumn30.AutoCompleteMode = Infragistics.Win.AutoCompleteMode.Append;
            ultraGridColumn30.Header.VisiblePosition = 9;
            ultraGridColumn30.Hidden = true;
            ultraGridColumn31.AutoCompleteMode = Infragistics.Win.AutoCompleteMode.Append;
            ultraGridColumn31.Header.Caption = "Instance";
            ultraGridColumn31.Header.VisiblePosition = 7;
            ultraGridColumn31.Hidden = true;
            ultraGridColumn32.AutoCompleteMode = Infragistics.Win.AutoCompleteMode.Append;
            ultraGridColumn32.Header.VisiblePosition = 10;
            ultraGridColumn32.Hidden = true;
            ultraGridColumn33.AutoCompleteMode = Infragistics.Win.AutoCompleteMode.Append;
            ultraGridColumn33.Header.VisiblePosition = 6;
            ultraGridColumn34.AutoCompleteMode = Infragistics.Win.AutoCompleteMode.Append;
            ultraGridColumn34.CellActivation = Infragistics.Win.UltraWinGrid.Activation.NoEdit;
            ultraGridColumn34.Format = "G";
            ultraGridColumn34.Header.Caption = "Date Created";
            ultraGridColumn34.Header.VisiblePosition = 4;
            ultraGridColumn34.Hidden = true;
            ultraGridColumn35.AutoCompleteMode = Infragistics.Win.AutoCompleteMode.Append;
            ultraGridColumn35.Format = "G";
            ultraGridColumn35.Header.VisiblePosition = 5;
            ultraGridColumn35.Hidden = true;
            ultraGridColumn36.AutoCompleteMode = Infragistics.Win.AutoCompleteMode.Append;
            ultraGridColumn36.Header.VisiblePosition = 11;
            ultraGridColumn36.Hidden = true;
            ultraGridColumn37.AutoCompleteMode = Infragistics.Win.AutoCompleteMode.Append;
            ultraGridColumn37.CellActivation = Infragistics.Win.UltraWinGrid.Activation.NoEdit;
            appearance36.ImageHAlign = Infragistics.Win.HAlign.Center;
            ultraGridColumn37.CellAppearance = appearance36;
            appearance37.FontData.BoldAsString = "True";
            appearance37.ForeColor = System.Drawing.Color.Red;
            appearance37.TextHAlignAsString = "Center";
            ultraGridColumn37.Header.Appearance = appearance37;
            ultraGridColumn37.Header.Caption = "!";
            ultraGridColumn37.Header.ToolTipText = "Severity";
            ultraGridColumn37.Header.VisiblePosition = 1;
            ultraGridColumn37.MaxWidth = 24;
            ultraGridColumn37.MinWidth = 24;
            ultraGridColumn37.Width = 24;
            ultraGridColumn38.AutoCompleteMode = Infragistics.Win.AutoCompleteMode.Append;
            ultraGridColumn38.ExcludeFromColumnChooser = Infragistics.Win.UltraWinGrid.ExcludeFromColumnChooser.True;
            ultraGridColumn38.Header.VisiblePosition = 12;
            ultraGridColumn38.Hidden = true;
            ultraGridColumn39.AutoCompleteMode = Infragistics.Win.AutoCompleteMode.Append;
            ultraGridColumn39.ExcludeFromColumnChooser = Infragistics.Win.UltraWinGrid.ExcludeFromColumnChooser.True;
            ultraGridColumn39.Header.VisiblePosition = 13;
            ultraGridColumn39.Hidden = true;
            ultraGridColumn40.AutoCompleteMode = Infragistics.Win.AutoCompleteMode.Append;
            appearance38.Image = ((object)(resources.GetObject("appearance38.Image")));
            appearance38.ImageHAlign = Infragistics.Win.HAlign.Center;
            appearance38.TextHAlignAsString = "Center";
            ultraGridColumn40.Header.Appearance = appearance38;
            ultraGridColumn40.Header.Caption = "";
            ultraGridColumn40.Header.ToolTipText = "Completed";
            ultraGridColumn40.Header.VisiblePosition = 3;
            ultraGridColumn40.MaxWidth = 24;
            ultraGridColumn40.MinWidth = 24;
            ultraGridColumn40.Width = 24;
            ultraGridBand1.Columns.AddRange(new object[] {
            ultraGridColumn27,
            ultraGridColumn28,
            ultraGridColumn29,
            ultraGridColumn30,
            ultraGridColumn31,
            ultraGridColumn32,
            ultraGridColumn33,
            ultraGridColumn34,
            ultraGridColumn35,
            ultraGridColumn36,
            ultraGridColumn37,
            ultraGridColumn38,
            ultraGridColumn39,
            ultraGridColumn40});
            this.tasksGrid.DisplayLayout.BandsSerializer.Add(ultraGridBand1);
            this.tasksGrid.DisplayLayout.BorderStyle = Infragistics.Win.UIElementBorderStyle.None;
            this.tasksGrid.DisplayLayout.CaptionVisible = Infragistics.Win.DefaultableBoolean.False;
            appearance7.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(227)))), ((int)(((byte)(239)))), ((int)(((byte)(255)))));
            appearance7.BackColor2 = System.Drawing.SystemColors.ControlDark;
            appearance7.BackGradientStyle = Infragistics.Win.GradientStyle.None;
            appearance7.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(101)))), ((int)(((byte)(147)))), ((int)(((byte)(207)))));
            this.tasksGrid.DisplayLayout.GroupByBox.Appearance = appearance7;
            appearance8.ForeColor = System.Drawing.SystemColors.GrayText;
            this.tasksGrid.DisplayLayout.GroupByBox.BandLabelAppearance = appearance8;
            this.tasksGrid.DisplayLayout.GroupByBox.BorderStyle = Infragistics.Win.UIElementBorderStyle.Solid;
            this.tasksGrid.DisplayLayout.GroupByBox.Hidden = true;
            appearance9.BackColor = System.Drawing.SystemColors.ControlLightLight;
            appearance9.BackColor2 = System.Drawing.SystemColors.Control;
            appearance9.BackGradientStyle = Infragistics.Win.GradientStyle.Horizontal;
            appearance9.ForeColor = System.Drawing.SystemColors.GrayText;
            this.tasksGrid.DisplayLayout.GroupByBox.PromptAppearance = appearance9;
            this.tasksGrid.DisplayLayout.LoadStyle = Infragistics.Win.UltraWinGrid.LoadStyle.LoadOnDemand;
            this.tasksGrid.DisplayLayout.MaxColScrollRegions = 1;
            this.tasksGrid.DisplayLayout.MaxRowScrollRegions = 1;
            this.tasksGrid.DisplayLayout.NewColumnLoadStyle = Infragistics.Win.UltraWinGrid.NewColumnLoadStyle.Hide;
            this.tasksGrid.DisplayLayout.Override.AllowAddNew = Infragistics.Win.UltraWinGrid.AllowAddNew.No;
            this.tasksGrid.DisplayLayout.Override.AllowDelete = Infragistics.Win.DefaultableBoolean.False;
            this.tasksGrid.DisplayLayout.Override.AllowMultiCellOperations = Infragistics.Win.UltraWinGrid.AllowMultiCellOperation.Copy;
            this.tasksGrid.DisplayLayout.Override.AllowUpdate = Infragistics.Win.DefaultableBoolean.True;
            this.tasksGrid.DisplayLayout.Override.BorderStyleCell = Infragistics.Win.UIElementBorderStyle.Solid;
            this.tasksGrid.DisplayLayout.Override.BorderStyleRow = Infragistics.Win.UIElementBorderStyle.Solid;
            appearance10.BackColor = System.Drawing.SystemColors.Window;
            this.tasksGrid.DisplayLayout.Override.CardAreaAppearance = appearance10;
            appearance11.BorderColor = System.Drawing.Color.Silver;
            appearance11.TextTrimming = Infragistics.Win.TextTrimming.EllipsisCharacter;
            this.tasksGrid.DisplayLayout.Override.CellAppearance = appearance11;
            this.tasksGrid.DisplayLayout.Override.CellClickAction = Infragistics.Win.UltraWinGrid.CellClickAction.RowSelect;
            this.tasksGrid.DisplayLayout.Override.CellPadding = 0;
            this.tasksGrid.DisplayLayout.Override.FixedHeaderIndicator = Infragistics.Win.UltraWinGrid.FixedHeaderIndicator.None;
            this.tasksGrid.DisplayLayout.Override.GroupByColumnsHidden = Infragistics.Win.DefaultableBoolean.False;
            appearance12.BackColor = System.Drawing.SystemColors.Control;
            appearance12.BackColor2 = System.Drawing.SystemColors.ControlDark;
            appearance12.BackGradientAlignment = Infragistics.Win.GradientAlignment.Element;
            appearance12.BackGradientStyle = Infragistics.Win.GradientStyle.Horizontal;
            appearance12.BorderColor = System.Drawing.SystemColors.Window;
            this.tasksGrid.DisplayLayout.Override.GroupByRowAppearance = appearance12;
            this.tasksGrid.DisplayLayout.Override.GroupByRowInitialExpansionState = Infragistics.Win.UltraWinGrid.GroupByRowInitialExpansionState.Expanded;
            appearance13.TextHAlignAsString = "Left";
            this.tasksGrid.DisplayLayout.Override.HeaderAppearance = appearance13;
            this.tasksGrid.DisplayLayout.Override.HeaderClickAction = Infragistics.Win.UltraWinGrid.HeaderClickAction.SortMulti;
            appearance14.BackColor = System.Drawing.SystemColors.Window;
            appearance14.BorderColor = System.Drawing.Color.Silver;
            this.tasksGrid.DisplayLayout.Override.RowAppearance = appearance14;
            this.tasksGrid.DisplayLayout.Override.RowSelectors = Infragistics.Win.DefaultableBoolean.False;
            this.tasksGrid.DisplayLayout.Override.SelectTypeCell = Infragistics.Win.UltraWinGrid.SelectType.None;
            this.tasksGrid.DisplayLayout.Override.SelectTypeCol = Infragistics.Win.UltraWinGrid.SelectType.None;
            this.tasksGrid.DisplayLayout.Override.SelectTypeRow = Infragistics.Win.UltraWinGrid.SelectType.Single;
            appearance15.BackColor = System.Drawing.SystemColors.ControlLight;
            this.tasksGrid.DisplayLayout.Override.TemplateAddRowAppearance = appearance15;
            this.tasksGrid.DisplayLayout.ScrollBounds = Infragistics.Win.UltraWinGrid.ScrollBounds.ScrollToFill;
            this.tasksGrid.DisplayLayout.ScrollStyle = Infragistics.Win.UltraWinGrid.ScrollStyle.Immediate;
            valueList1.DisplayStyle = Infragistics.Win.ValueListDisplayStyle.Picture;
            valueList1.Key = "Severity";
            valueList1.PreferredDropDownSize = new System.Drawing.Size(0, 0);
            valueList2.DisplayStyle = Infragistics.Win.ValueListDisplayStyle.Picture;
            valueList2.Key = "Status";
            valueList2.PreferredDropDownSize = new System.Drawing.Size(0, 0);
            valueList3.DisplayStyle = Infragistics.Win.ValueListDisplayStyle.DisplayText;
            valueList3.Key = "Metrics";
            valueList3.PreferredDropDownSize = new System.Drawing.Size(0, 0);
            this.tasksGrid.DisplayLayout.ValueLists.AddRange(new Infragistics.Win.ValueList[] {
            valueList1,
            valueList2,
            valueList3});
            this.tasksGrid.DisplayLayout.ViewStyle = Infragistics.Win.UltraWinGrid.ViewStyle.SingleBand;
            this.tasksGrid.DisplayLayout.ViewStyleBand = Infragistics.Win.UltraWinGrid.ViewStyleBand.OutlookGroupBy;
            this.tasksGrid.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tasksGrid.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.tasksGrid.Location = new System.Drawing.Point(0, 0);
            this.tasksGrid.Name = "tasksGrid";
            this.tasksGrid.Size = new System.Drawing.Size(392, 138);
            this.tasksGrid.TabIndex = 4;
            this.tasksGrid.UpdateMode = Infragistics.Win.UltraWinGrid.UpdateMode.OnCellChangeOrLostFocus;
            this.tasksGrid.DoubleClickRow += new Infragistics.Win.UltraWinGrid.DoubleClickRowEventHandler(this.tasksGrid_DoubleClickRow);
            this.tasksGrid.MouseClick += new System.Windows.Forms.MouseEventHandler(this.tasksGrid_MouseClick);
            this.tasksGrid.MouseDown += new System.Windows.Forms.MouseEventHandler(this.tasksGrid_MouseDown);
            // 
            // tasksGridDataSource
            // 
            this.tasksGridDataSource.AllowAdd = false;
            this.tasksGridDataSource.AllowDelete = false;
            ultraDataColumn1.DataType = typeof(int);
            ultraDataColumn1.ReadOnly = Infragistics.Win.DefaultableBoolean.True;
            ultraDataColumn2.AllowDBNull = Infragistics.Win.DefaultableBoolean.False;
            ultraDataColumn2.DataType = typeof(byte);
            ultraDataColumn2.ReadOnly = Infragistics.Win.DefaultableBoolean.False;
            ultraDataColumn3.AllowDBNull = Infragistics.Win.DefaultableBoolean.False;
            ultraDataColumn3.ReadOnly = Infragistics.Win.DefaultableBoolean.True;
            ultraDataColumn4.AllowDBNull = Infragistics.Win.DefaultableBoolean.True;
            ultraDataColumn4.ReadOnly = Infragistics.Win.DefaultableBoolean.True;
            ultraDataColumn5.AllowDBNull = Infragistics.Win.DefaultableBoolean.True;
            ultraDataColumn5.ReadOnly = Infragistics.Win.DefaultableBoolean.True;
            ultraDataColumn6.AllowDBNull = Infragistics.Win.DefaultableBoolean.True;
            ultraDataColumn6.ReadOnly = Infragistics.Win.DefaultableBoolean.False;
            ultraDataColumn7.AllowDBNull = Infragistics.Win.DefaultableBoolean.True;
            ultraDataColumn7.ReadOnly = Infragistics.Win.DefaultableBoolean.False;
            ultraDataColumn8.AllowDBNull = Infragistics.Win.DefaultableBoolean.False;
            ultraDataColumn8.DataType = typeof(System.DateTime);
            ultraDataColumn8.ReadOnly = Infragistics.Win.DefaultableBoolean.True;
            ultraDataColumn9.AllowDBNull = Infragistics.Win.DefaultableBoolean.True;
            ultraDataColumn9.DataType = typeof(System.DateTime);
            ultraDataColumn9.ReadOnly = Infragistics.Win.DefaultableBoolean.True;
            ultraDataColumn10.AllowDBNull = Infragistics.Win.DefaultableBoolean.True;
            ultraDataColumn10.DataType = typeof(int);
            ultraDataColumn10.ReadOnly = Infragistics.Win.DefaultableBoolean.True;
            ultraDataColumn11.AllowDBNull = Infragistics.Win.DefaultableBoolean.True;
            ultraDataColumn11.DataType = typeof(byte);
            ultraDataColumn11.ReadOnly = Infragistics.Win.DefaultableBoolean.True;
            ultraDataColumn12.AllowDBNull = Infragistics.Win.DefaultableBoolean.True;
            ultraDataColumn12.DataType = typeof(double);
            ultraDataColumn12.ReadOnly = Infragistics.Win.DefaultableBoolean.True;
            ultraDataColumn13.AllowDBNull = Infragistics.Win.DefaultableBoolean.True;
            ultraDataColumn13.DataType = typeof(int);
            ultraDataColumn13.ReadOnly = Infragistics.Win.DefaultableBoolean.True;
            ultraDataColumn14.AllowDBNull = Infragistics.Win.DefaultableBoolean.True;
            ultraDataColumn14.DataType = typeof(bool);
            ultraDataColumn14.ReadOnly = Infragistics.Win.DefaultableBoolean.False;
            this.tasksGridDataSource.Band.Columns.AddRange(new object[] {
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
            ultraDataColumn13,
            ultraDataColumn14});
            this.tasksGridDataSource.KeyIndex = 0;
            // 
            // tasksStatusLabel
            // 
            this.tasksStatusLabel.AutoEllipsis = true;
            this.tasksStatusLabel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tasksStatusLabel.Location = new System.Drawing.Point(0, 0);
            this.tasksStatusLabel.Name = "tasksStatusLabel";
            this.tasksStatusLabel.Padding = new System.Windows.Forms.Padding(0, 5, 0, 0);
            this.tasksStatusLabel.Size = new System.Drawing.Size(392, 138);
            this.tasksStatusLabel.TabIndex = 3;
            this.tasksStatusLabel.Text = "There are no active items.";
            this.tasksStatusLabel.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            // 
            // ultraTabPageControl2
            // 
            this.ultraTabPageControl2.Controls.Add(this.panel1);
            this.ultraTabPageControl2.Location = new System.Drawing.Point(-10000, -10000);
            this.ultraTabPageControl2.Name = "ultraTabPageControl2";
            this.ultraTabPageControl2.Size = new System.Drawing.Size(392, 138);
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.alertForecastPanel1);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel1.Location = new System.Drawing.Point(0, 0);
            this.panel1.Margin = new System.Windows.Forms.Padding(0);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(392, 138);
            this.panel1.TabIndex = 1;
            // 
            // alertForecastPanel1
            // 
            this.alertForecastPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.alertForecastPanel1.InHistoryMode = false;
            this.alertForecastPanel1.Location = new System.Drawing.Point(0, 0);
            this.alertForecastPanel1.Margin = new System.Windows.Forms.Padding(0);
            this.alertForecastPanel1.MinimumSize = new System.Drawing.Size(383, 78);
            this.alertForecastPanel1.Name = "alertForecastPanel1";
            this.alertForecastPanel1.Size = new System.Drawing.Size(392, 138);
            this.alertForecastPanel1.TabIndex = 0;
            // 
            // contentPanel
            // 
            this.contentPanel.BackColor = System.Drawing.Color.White;
            this.contentPanel.BackColor2 = System.Drawing.Color.White;
            this.contentPanel.BorderColor = System.Drawing.Color.White;
            this.contentPanel.Controls.Add(this.contentSplitter);
            this.contentPanel.Controls.Add(this.commonTasksPanel);
            this.contentPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.contentPanel.FillStyle = Idera.SQLdm.DesktopClient.Controls.GradientPanelFillStyle.Solid;
            this.contentPanel.Location = new System.Drawing.Point(0, 98);
            this.contentPanel.Name = "contentPanel";
            this.contentPanel.Padding = new System.Windows.Forms.Padding(1);
            this.contentPanel.ShowBorder = false;
            this.contentPanel.Size = new System.Drawing.Size(657, 482);
            this.contentPanel.TabIndex = 2;
            // 
            // contentSplitter
            // 
            this.contentSplitter.Dock = System.Windows.Forms.DockStyle.Fill;
            this.contentSplitter.FixedPanel = System.Windows.Forms.FixedPanel.Panel1;
            this.contentSplitter.Location = new System.Drawing.Point(1, 1);
            this.contentSplitter.Name = "contentSplitter";
            this.contentSplitter.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // contentSplitter.Panel1
            // 
            this.contentSplitter.Panel1.Controls.Add(this.contentLayoutPanel);
            this.contentSplitter.Panel1MinSize = 170;
            // 
            // contentSplitter.Panel2
            // 
            this.contentSplitter.Panel2.Controls.Add(this.alertsPanel);
            this.contentSplitter.Size = new System.Drawing.Size(655, 340);
            this.contentSplitter.SplitterDistance = 170;
            this.contentSplitter.TabIndex = 3;
            this.contentSplitter.MouseDown += new System.Windows.Forms.MouseEventHandler(this.contentSplitter_MouseDown);
            this.contentSplitter.MouseUp += new System.Windows.Forms.MouseEventHandler(this.contentSplitter_MouseUp);
            // 
            // contentLayoutPanel
            // 
            this.contentLayoutPanel.ColumnCount = 3;
            this.contentLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 38.1762F));
            this.contentLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 6F));
            this.contentLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 61.8238F));
            this.contentLayoutPanel.Controls.Add(this.columnDividerLabel, 1, 0);
            this.contentLayoutPanel.Controls.Add(this.statusSummaryPanel, 0, 0);
            this.contentLayoutPanel.Controls.Add(this.tasksForecastTabGroup, 2, 0);
            this.contentLayoutPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.contentLayoutPanel.Location = new System.Drawing.Point(0, 0);
            this.contentLayoutPanel.Name = "contentLayoutPanel";
            this.contentLayoutPanel.RowCount = 1;
            this.contentLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.contentLayoutPanel.Size = new System.Drawing.Size(655, 170);
            this.contentLayoutPanel.TabIndex = 2;
            // 
            // columnDividerLabel
            // 
            this.columnDividerLabel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)));
            this.columnDividerLabel.BackColor = System.Drawing.Color.LightGray;
            this.columnDividerLabel.Location = new System.Drawing.Point(250, 0);
            this.columnDividerLabel.Name = "columnDividerLabel";
            this.columnDividerLabel.Size = new System.Drawing.Size(1, 170);
            this.columnDividerLabel.TabIndex = 3;
            // 
            // statusSummaryPanel
            // 
            this.statusSummaryPanel.Controls.Add(this.statusSummaryContentPanel);
            this.statusSummaryPanel.Controls.Add(this.statusSummaryLoadingLabel);
            this.statusSummaryPanel.Controls.Add(this.statusSummaryHeaderLabel);
            this.statusSummaryPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.statusSummaryPanel.Location = new System.Drawing.Point(3, 3);
            this.statusSummaryPanel.Name = "statusSummaryPanel";
            this.statusSummaryPanel.Size = new System.Drawing.Size(241, 164);
            this.statusSummaryPanel.TabIndex = 5;
            // 
            // statusSummaryContentPanel
            // 
            this.statusSummaryContentPanel.Controls.Add(this.statusSummaryLabel);
            this.statusSummaryContentPanel.Controls.Add(this.statusSummaryPictureBox);
            this.statusSummaryContentPanel.Controls.Add(this.okServersLabel);
            this.statusSummaryContentPanel.Controls.Add(this.statusSummaryDividerLabel);
            this.statusSummaryContentPanel.Controls.Add(this.maintenanceModeServersLabel);
            this.statusSummaryContentPanel.Controls.Add(this.statusSummaryDescriptionLabel);
            this.statusSummaryContentPanel.Controls.Add(this.warningServersLabel);
            this.statusSummaryContentPanel.Controls.Add(this.criticalServersPictureBox);
            this.statusSummaryContentPanel.Controls.Add(this.criticalServersLabel);
            this.statusSummaryContentPanel.Controls.Add(this.warningServersPictureBox);
            this.statusSummaryContentPanel.Controls.Add(this.okServersPictureBox);
            this.statusSummaryContentPanel.Controls.Add(this.maintenanceModeServersPictureBox);
            this.statusSummaryContentPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.statusSummaryContentPanel.Location = new System.Drawing.Point(0, 20);
            this.statusSummaryContentPanel.Name = "statusSummaryContentPanel";
            this.statusSummaryContentPanel.Size = new System.Drawing.Size(241, 144);
            this.statusSummaryContentPanel.TabIndex = 4;
            this.statusSummaryContentPanel.Visible = false;
            // 
            // statusSummaryLabel
            // 
            this.statusSummaryLabel.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.statusSummaryLabel.AutoEllipsis = true;
            this.statusSummaryLabel.DisabledLinkColor = System.Drawing.Color.FromArgb(((int)(((byte)(85)))), ((int)(((byte)(85)))), ((int)(((byte)(85)))));
            this.statusSummaryLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.statusSummaryLabel.LinkBehavior = System.Windows.Forms.LinkBehavior.HoverUnderline;
            this.statusSummaryLabel.LinkColor = System.Drawing.SystemColors.ControlText;
            this.statusSummaryLabel.Location = new System.Drawing.Point(64, 13);
            this.statusSummaryLabel.Name = "statusSummaryLabel";
            this.statusSummaryLabel.Size = new System.Drawing.Size(122, 20);
            this.statusSummaryLabel.TabIndex = 17;
            this.statusSummaryLabel.TabStop = true;
            this.statusSummaryLabel.Text = "<summary>";
            this.statusSummaryLabel.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.statusSummaryLabel.UseCompatibleTextRendering = true;
            this.statusSummaryLabel.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.ServersLabel_LinkClicked);
            // 
            // statusSummaryPictureBox
            // 
            this.statusSummaryPictureBox.Image = global::Idera.SQLdm.DesktopClient.Properties.Resources.StatusSummaryInformationLarge;
            this.statusSummaryPictureBox.Location = new System.Drawing.Point(8, 8);
            this.statusSummaryPictureBox.Name = "statusSummaryPictureBox";
            this.statusSummaryPictureBox.Size = new System.Drawing.Size(48, 48);
            this.statusSummaryPictureBox.TabIndex = 2;
            this.statusSummaryPictureBox.TabStop = false;
            // 
            // okServersLabel
            // 
            this.okServersLabel.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.okServersLabel.AutoEllipsis = true;
            this.okServersLabel.LinkBehavior = System.Windows.Forms.LinkBehavior.HoverUnderline;
            this.okServersLabel.LinkColor = System.Drawing.SystemColors.ControlText;
            this.okServersLabel.Location = new System.Drawing.Point(45, 107);
            this.okServersLabel.Name = "okServersLabel";
            this.okServersLabel.Size = new System.Drawing.Size(172, 13);
            this.okServersLabel.TabIndex = 16;
            this.okServersLabel.TabStop = true;
            this.okServersLabel.Text = "{0} server(s) in OK state";
            this.okServersLabel.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.ServersLabel_LinkClicked);
            // 
            // statusSummaryDividerLabel
            // 
            this.statusSummaryDividerLabel.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.statusSummaryDividerLabel.BackColor = System.Drawing.Color.LightGray;
            this.statusSummaryDividerLabel.Location = new System.Drawing.Point(5, 59);
            this.statusSummaryDividerLabel.Name = "statusSummaryDividerLabel";
            this.statusSummaryDividerLabel.Size = new System.Drawing.Size(231, 1);
            this.statusSummaryDividerLabel.TabIndex = 4;
            // 
            // maintenanceModeServersLabel
            // 
            this.maintenanceModeServersLabel.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.maintenanceModeServersLabel.AutoEllipsis = true;
            this.maintenanceModeServersLabel.LinkBehavior = System.Windows.Forms.LinkBehavior.HoverUnderline;
            this.maintenanceModeServersLabel.LinkColor = System.Drawing.SystemColors.ControlText;
            this.maintenanceModeServersLabel.Location = new System.Drawing.Point(45, 127);
            this.maintenanceModeServersLabel.Name = "maintenanceModeServersLabel";
            this.maintenanceModeServersLabel.Size = new System.Drawing.Size(172, 13);
            this.maintenanceModeServersLabel.TabIndex = 15;
            this.maintenanceModeServersLabel.TabStop = true;
            this.maintenanceModeServersLabel.Text = "{0} server(s) in Maintenance Mode";
            this.maintenanceModeServersLabel.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.ServersLabel_LinkClicked);
            // 
            // statusSummaryDescriptionLabel
            // 
            this.statusSummaryDescriptionLabel.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.statusSummaryDescriptionLabel.AutoEllipsis = true;
            this.statusSummaryDescriptionLabel.DisabledLinkColor = System.Drawing.Color.FromArgb(((int)(((byte)(85)))), ((int)(((byte)(85)))), ((int)(((byte)(85)))));
            this.statusSummaryDescriptionLabel.LinkBehavior = System.Windows.Forms.LinkBehavior.HoverUnderline;
            this.statusSummaryDescriptionLabel.LinkColor = System.Drawing.SystemColors.ControlText;
            this.statusSummaryDescriptionLabel.Location = new System.Drawing.Point(66, 38);
            this.statusSummaryDescriptionLabel.Name = "statusSummaryDescriptionLabel";
            this.statusSummaryDescriptionLabel.Size = new System.Drawing.Size(120, 18);
            this.statusSummaryDescriptionLabel.TabIndex = 5;
            this.statusSummaryDescriptionLabel.TabStop = true;
            this.statusSummaryDescriptionLabel.Text = "<description>";
            this.statusSummaryDescriptionLabel.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.ServersLabel_LinkClicked);
            // 
            // warningServersLabel
            // 
            this.warningServersLabel.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.warningServersLabel.AutoEllipsis = true;
            this.warningServersLabel.LinkBehavior = System.Windows.Forms.LinkBehavior.HoverUnderline;
            this.warningServersLabel.LinkColor = System.Drawing.SystemColors.ControlText;
            this.warningServersLabel.Location = new System.Drawing.Point(45, 87);
            this.warningServersLabel.Name = "warningServersLabel";
            this.warningServersLabel.Size = new System.Drawing.Size(172, 13);
            this.warningServersLabel.TabIndex = 14;
            this.warningServersLabel.TabStop = true;
            this.warningServersLabel.Text = "{0} server(s) in Warning state";
            this.warningServersLabel.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.ServersLabel_LinkClicked);
            // 
            // criticalServersPictureBox
            // 
            this.criticalServersPictureBox.Image = global::Idera.SQLdm.DesktopClient.Properties.Resources.StatusCriticalSmall;
            this.criticalServersPictureBox.Location = new System.Drawing.Point(24, 65);
            this.criticalServersPictureBox.Name = "criticalServersPictureBox";
            this.criticalServersPictureBox.Size = new System.Drawing.Size(16, 16);
            this.criticalServersPictureBox.TabIndex = 6;
            this.criticalServersPictureBox.TabStop = false;
            // 
            // criticalServersLabel
            // 
            this.criticalServersLabel.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.criticalServersLabel.AutoEllipsis = true;
            this.criticalServersLabel.LinkBehavior = System.Windows.Forms.LinkBehavior.HoverUnderline;
            this.criticalServersLabel.LinkColor = System.Drawing.SystemColors.ControlText;
            this.criticalServersLabel.Location = new System.Drawing.Point(45, 67);
            this.criticalServersLabel.Name = "criticalServersLabel";
            this.criticalServersLabel.Size = new System.Drawing.Size(172, 13);
            this.criticalServersLabel.TabIndex = 13;
            this.criticalServersLabel.TabStop = true;
            this.criticalServersLabel.Text = "{0} server(s) in Critical state";
            this.criticalServersLabel.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.ServersLabel_LinkClicked);
            // 
            // warningServersPictureBox
            // 
            this.warningServersPictureBox.Image = global::Idera.SQLdm.DesktopClient.Properties.Resources.StatusWarningSmall;
            this.warningServersPictureBox.Location = new System.Drawing.Point(24, 85);
            this.warningServersPictureBox.Name = "warningServersPictureBox";
            this.warningServersPictureBox.Size = new System.Drawing.Size(16, 16);
            this.warningServersPictureBox.TabIndex = 7;
            this.warningServersPictureBox.TabStop = false;
            // 
            // okServersPictureBox
            // 
            this.okServersPictureBox.Image = global::Idera.SQLdm.DesktopClient.Properties.Resources.StatusOKSmall;
            this.okServersPictureBox.Location = new System.Drawing.Point(24, 105);
            this.okServersPictureBox.Name = "okServersPictureBox";
            this.okServersPictureBox.Size = new System.Drawing.Size(16, 16);
            this.okServersPictureBox.TabIndex = 9;
            this.okServersPictureBox.TabStop = false;
            // 
            // maintenanceModeServersPictureBox
            // 
            this.maintenanceModeServersPictureBox.Image = global::Idera.SQLdm.DesktopClient.Properties.Resources.MaintenanceMode16x16;
            this.maintenanceModeServersPictureBox.Location = new System.Drawing.Point(24, 125);
            this.maintenanceModeServersPictureBox.Name = "maintenanceModeServersPictureBox";
            this.maintenanceModeServersPictureBox.Size = new System.Drawing.Size(16, 16);
            this.maintenanceModeServersPictureBox.TabIndex = 8;
            this.maintenanceModeServersPictureBox.TabStop = false;
            // 
            // statusSummaryLoadingLabel
            // 
            this.statusSummaryLoadingLabel.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.statusSummaryLoadingLabel.AutoEllipsis = true;
            this.statusSummaryLoadingLabel.Location = new System.Drawing.Point(2, 23);
            this.statusSummaryLoadingLabel.Name = "statusSummaryLoadingLabel";
            this.statusSummaryLoadingLabel.Size = new System.Drawing.Size(236, 17);
            this.statusSummaryLoadingLabel.TabIndex = 4;
            this.statusSummaryLoadingLabel.Text = Idera.SQLdm.Common.Constants.LOADING;
            this.statusSummaryLoadingLabel.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            // 
            // statusSummaryHeaderLabel
            // 
            this.statusSummaryHeaderLabel.BackColor = System.Drawing.SystemColors.Control;
            this.statusSummaryHeaderLabel.Dock = System.Windows.Forms.DockStyle.Top;
            this.statusSummaryHeaderLabel.HeaderText = "Status Summary";
            this.statusSummaryHeaderLabel.Location = new System.Drawing.Point(0, 0);
            this.statusSummaryHeaderLabel.Name = "statusSummaryHeaderLabel";
            this.statusSummaryHeaderLabel.Size = new System.Drawing.Size(241, 20);
            this.statusSummaryHeaderLabel.TabIndex = 1;
            // 
            // tasksForecastTabGroup
            // 
            appearance2.BackColor = System.Drawing.Color.White;
            this.tasksForecastTabGroup.Appearance = appearance2;
            this.tasksForecastTabGroup.Controls.Add(this.ultraTabSharedControlsPage1);
            this.tasksForecastTabGroup.Controls.Add(this.ultraTabPageControl1);
            this.tasksForecastTabGroup.Controls.Add(this.ultraTabPageControl2);
            this.tasksForecastTabGroup.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tasksForecastTabGroup.Location = new System.Drawing.Point(256, 3);
            this.tasksForecastTabGroup.Name = "tasksForecastTabGroup";
            this.tasksForecastTabGroup.SharedControlsPage = this.ultraTabSharedControlsPage1;
            this.tasksForecastTabGroup.Size = new System.Drawing.Size(396, 164);
            appearance39.BackColor = System.Drawing.Color.White;
            this.tasksForecastTabGroup.TabHeaderAreaAppearance = appearance39;
            this.tasksForecastTabGroup.TabIndex = 6;
            appearance41.BackColor = System.Drawing.Color.White;
            appearance41.FontData.BoldAsString = "True";
            ultraTab1.Appearance = appearance41;
            ultraTab1.FixedWidth = 100;
            ultraTab1.TabPage = this.ultraTabPageControl1;
            ultraTab1.Text = "To Do List";
            appearance40.BackColor = System.Drawing.Color.White;
            appearance40.FontData.BoldAsString = "True";
            ultraTab2.Appearance = appearance40;
            ultraTab2.FixedWidth = 135;
            ultraTab2.TabPage = this.ultraTabPageControl2;
            ultraTab2.Text = "12 Hour Forecast";
            this.tasksForecastTabGroup.Tabs.AddRange(new Infragistics.Win.UltraWinTabControl.UltraTab[] {
            ultraTab1,
            ultraTab2});
            this.tasksForecastTabGroup.SelectedTabChanged += new Infragistics.Win.UltraWinTabControl.SelectedTabChangedEventHandler(this.tasksForecastTabGroup_SelectedTabChanged);
            // 
            // ultraTabSharedControlsPage1
            // 
            this.ultraTabSharedControlsPage1.Location = new System.Drawing.Point(-10000, -10000);
            this.ultraTabSharedControlsPage1.Name = "ultraTabSharedControlsPage1";
            this.ultraTabSharedControlsPage1.Size = new System.Drawing.Size(392, 138);
            // 
            // alertsPanel
            // 
            this.alertsPanel.Controls.Add(this.alertsGrid);
            this.alertsPanel.Controls.Add(this.recentAlertsHeaderLabel);
            this.alertsPanel.Controls.Add(this.alertsStatusLabel);
            this.alertsPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.alertsPanel.Location = new System.Drawing.Point(0, 0);
            this.alertsPanel.Name = "alertsPanel";
            this.alertsPanel.Size = new System.Drawing.Size(655, 166);
            this.alertsPanel.TabIndex = 2;
            // 
            // alertsGrid
            // 
            this.alertsGrid.DataMember = "Band 0";
            this.alertsGrid.DataSource = this.alertsViewDataSource;
            appearance16.BackColor = System.Drawing.SystemColors.Window;
            appearance16.BorderColor = System.Drawing.SystemColors.InactiveCaption;
            this.alertsGrid.DisplayLayout.Appearance = appearance16;
            this.alertsGrid.DisplayLayout.AutoFitStyle = Infragistics.Win.UltraWinGrid.AutoFitStyle.ExtendLastColumn;
            ultraGridColumn41.AutoCompleteMode = Infragistics.Win.AutoCompleteMode.Append;
            ultraGridColumn41.ExcludeFromColumnChooser = Infragistics.Win.UltraWinGrid.ExcludeFromColumnChooser.True;
            ultraGridColumn41.Header.VisiblePosition = 9;
            ultraGridColumn41.Hidden = true;
            ultraGridColumn42.AutoCompleteMode = Infragistics.Win.AutoCompleteMode.Append;
            ultraGridColumn42.Format = "G";
            ultraGridColumn42.Header.Caption = "Time";
            ultraGridColumn42.Header.VisiblePosition = 2;
            ultraGridColumn42.Width = 131;
            ultraGridColumn43.AutoCompleteMode = Infragistics.Win.AutoCompleteMode.Append;
            ultraGridColumn43.Header.Caption = "Instance";
            ultraGridColumn43.Header.VisiblePosition = 3;
            ultraGridColumn43.Width = 179;
            ultraGridColumn44.AutoCompleteMode = Infragistics.Win.AutoCompleteMode.Append;
            ultraGridColumn44.Header.VisiblePosition = 4;
            ultraGridColumn44.Hidden = true;
            ultraGridColumn45.AutoCompleteMode = Infragistics.Win.AutoCompleteMode.Append;
            ultraGridColumn45.Header.VisiblePosition = 5;
            ultraGridColumn45.Hidden = true;
            ultraGridColumn46.AutoCompleteMode = Infragistics.Win.AutoCompleteMode.Append;
            ultraGridColumn46.ExcludeFromColumnChooser = Infragistics.Win.UltraWinGrid.ExcludeFromColumnChooser.True;
            ultraGridColumn46.Header.VisiblePosition = 10;
            ultraGridColumn46.Hidden = true;
            ultraGridColumn47.AutoCompleteMode = Infragistics.Win.AutoCompleteMode.Append;
            ultraGridColumn47.Header.VisiblePosition = 6;
            ultraGridColumn47.Hidden = true;
            ultraGridColumn48.AutoCompleteMode = Infragistics.Win.AutoCompleteMode.Append;
            ultraGridColumn48.Header.VisiblePosition = 0;
            ultraGridColumn49.AutoCompleteMode = Infragistics.Win.AutoCompleteMode.Append;
            ultraGridColumn49.Header.Caption = "Change";
            ultraGridColumn49.Header.VisiblePosition = 1;
            ultraGridColumn49.Width = 134;
            ultraGridColumn50.AutoCompleteMode = Infragistics.Win.AutoCompleteMode.Append;
            ultraGridColumn50.ExcludeFromColumnChooser = Infragistics.Win.UltraWinGrid.ExcludeFromColumnChooser.True;
            ultraGridColumn50.Header.VisiblePosition = 11;
            ultraGridColumn50.Hidden = true;
            ultraGridColumn51.AutoCompleteMode = Infragistics.Win.AutoCompleteMode.Append;
            ultraGridColumn51.Header.Caption = "Description";
            ultraGridColumn51.Header.VisiblePosition = 7;
            ultraGridColumn52.AutoCompleteMode = Infragistics.Win.AutoCompleteMode.Append;
            ultraGridColumn52.Header.VisiblePosition = 8;
            ultraGridColumn52.Hidden = true;
            ultraGridBand2.Columns.AddRange(new object[] {
            ultraGridColumn41,
            ultraGridColumn42,
            ultraGridColumn43,
            ultraGridColumn44,
            ultraGridColumn45,
            ultraGridColumn46,
            ultraGridColumn47,
            ultraGridColumn48,
            ultraGridColumn49,
            ultraGridColumn50,
            ultraGridColumn51,
            ultraGridColumn52});
            this.alertsGrid.DisplayLayout.BandsSerializer.Add(ultraGridBand2);
            this.alertsGrid.DisplayLayout.BorderStyle = Infragistics.Win.UIElementBorderStyle.None;
            this.alertsGrid.DisplayLayout.CaptionVisible = Infragistics.Win.DefaultableBoolean.False;
            appearance17.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(227)))), ((int)(((byte)(239)))), ((int)(((byte)(255)))));
            appearance17.BackColor2 = System.Drawing.SystemColors.ControlDark;
            appearance17.BackGradientStyle = Infragistics.Win.GradientStyle.None;
            appearance17.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(101)))), ((int)(((byte)(147)))), ((int)(((byte)(207)))));
            this.alertsGrid.DisplayLayout.GroupByBox.Appearance = appearance17;
            appearance18.ForeColor = System.Drawing.SystemColors.GrayText;
            this.alertsGrid.DisplayLayout.GroupByBox.BandLabelAppearance = appearance18;
            this.alertsGrid.DisplayLayout.GroupByBox.BorderStyle = Infragistics.Win.UIElementBorderStyle.Solid;
            this.alertsGrid.DisplayLayout.GroupByBox.Hidden = true;
            appearance19.BackColor = System.Drawing.SystemColors.ControlLightLight;
            appearance19.BackColor2 = System.Drawing.SystemColors.Control;
            appearance19.BackGradientStyle = Infragistics.Win.GradientStyle.Horizontal;
            appearance19.ForeColor = System.Drawing.SystemColors.GrayText;
            this.alertsGrid.DisplayLayout.GroupByBox.PromptAppearance = appearance19;
            this.alertsGrid.DisplayLayout.MaxColScrollRegions = 1;
            this.alertsGrid.DisplayLayout.MaxRowScrollRegions = 1;
            this.alertsGrid.DisplayLayout.Override.AllowAddNew = Infragistics.Win.UltraWinGrid.AllowAddNew.No;
            this.alertsGrid.DisplayLayout.Override.AllowColSizing = Infragistics.Win.UltraWinGrid.AllowColSizing.Free;
            this.alertsGrid.DisplayLayout.Override.AllowDelete = Infragistics.Win.DefaultableBoolean.False;
            this.alertsGrid.DisplayLayout.Override.AllowUpdate = Infragistics.Win.DefaultableBoolean.False;
            this.alertsGrid.DisplayLayout.Override.BorderStyleCell = Infragistics.Win.UIElementBorderStyle.Solid;
            this.alertsGrid.DisplayLayout.Override.BorderStyleRow = Infragistics.Win.UIElementBorderStyle.Solid;
            appearance20.BackColor = System.Drawing.SystemColors.Window;
            this.alertsGrid.DisplayLayout.Override.CardAreaAppearance = appearance20;
            appearance21.BorderColor = System.Drawing.Color.Silver;
            appearance21.TextTrimming = Infragistics.Win.TextTrimming.EllipsisCharacter;
            this.alertsGrid.DisplayLayout.Override.CellAppearance = appearance21;
            this.alertsGrid.DisplayLayout.Override.CellClickAction = Infragistics.Win.UltraWinGrid.CellClickAction.RowSelect;
            this.alertsGrid.DisplayLayout.Override.CellPadding = 0;
            this.alertsGrid.DisplayLayout.Override.FixedHeaderIndicator = Infragistics.Win.UltraWinGrid.FixedHeaderIndicator.None;
            this.alertsGrid.DisplayLayout.Override.GroupByColumnsHidden = Infragistics.Win.DefaultableBoolean.False;
            appearance22.BackColor = System.Drawing.SystemColors.Control;
            appearance22.BackColor2 = System.Drawing.SystemColors.ControlDark;
            appearance22.BackGradientAlignment = Infragistics.Win.GradientAlignment.Element;
            appearance22.BackGradientStyle = Infragistics.Win.GradientStyle.Horizontal;
            appearance22.BorderColor = System.Drawing.SystemColors.Window;
            this.alertsGrid.DisplayLayout.Override.GroupByRowAppearance = appearance22;
            this.alertsGrid.DisplayLayout.Override.GroupByRowInitialExpansionState = Infragistics.Win.UltraWinGrid.GroupByRowInitialExpansionState.Expanded;
            appearance23.TextHAlignAsString = "Left";
            this.alertsGrid.DisplayLayout.Override.HeaderAppearance = appearance23;
            this.alertsGrid.DisplayLayout.Override.HeaderClickAction = Infragistics.Win.UltraWinGrid.HeaderClickAction.SortMulti;
            this.alertsGrid.DisplayLayout.Override.MaxSelectedRows = 1;
            appearance24.BackColor = System.Drawing.SystemColors.Window;
            appearance24.BorderColor = System.Drawing.Color.Silver;
            this.alertsGrid.DisplayLayout.Override.RowAppearance = appearance24;
            this.alertsGrid.DisplayLayout.Override.RowSelectors = Infragistics.Win.DefaultableBoolean.False;
            this.alertsGrid.DisplayLayout.Override.SelectTypeCell = Infragistics.Win.UltraWinGrid.SelectType.None;
            this.alertsGrid.DisplayLayout.Override.SelectTypeCol = Infragistics.Win.UltraWinGrid.SelectType.None;
            this.alertsGrid.DisplayLayout.Override.SelectTypeRow = Infragistics.Win.UltraWinGrid.SelectType.Single;
            appearance25.BackColor = System.Drawing.SystemColors.ControlLight;
            this.alertsGrid.DisplayLayout.Override.TemplateAddRowAppearance = appearance25;
            this.alertsGrid.DisplayLayout.ScrollBounds = Infragistics.Win.UltraWinGrid.ScrollBounds.ScrollToFill;
            this.alertsGrid.DisplayLayout.ScrollStyle = Infragistics.Win.UltraWinGrid.ScrollStyle.Immediate;
            this.alertsGrid.DisplayLayout.UseFixedHeaders = true;
            valueList4.DisplayStyle = Infragistics.Win.ValueListDisplayStyle.DisplayText;
            valueList4.Key = "Metrics";
            valueList4.PreferredDropDownSize = new System.Drawing.Size(0, 0);
            valueList5.DisplayStyle = Infragistics.Win.ValueListDisplayStyle.Picture;
            valueList5.Key = "Severity";
            valueList5.PreferredDropDownSize = new System.Drawing.Size(0, 0);
            valueList6.DisplayStyle = Infragistics.Win.ValueListDisplayStyle.DisplayTextAndPicture;
            valueList6.Key = "Transitions";
            valueList6.PreferredDropDownSize = new System.Drawing.Size(0, 0);
            this.alertsGrid.DisplayLayout.ValueLists.AddRange(new Infragistics.Win.ValueList[] {
            valueList4,
            valueList5,
            valueList6});
            this.alertsGrid.DisplayLayout.ViewStyle = Infragistics.Win.UltraWinGrid.ViewStyle.SingleBand;
            this.alertsGrid.DisplayLayout.ViewStyleBand = Infragistics.Win.UltraWinGrid.ViewStyleBand.OutlookGroupBy;
            this.alertsGrid.Dock = System.Windows.Forms.DockStyle.Fill;
            this.alertsGrid.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.alertsGrid.Location = new System.Drawing.Point(0, 20);
            this.alertsGrid.Name = "alertsGrid";
            this.alertsGrid.Size = new System.Drawing.Size(655, 146);
            this.alertsGrid.TabIndex = 1;
            this.alertsGrid.DoubleClickRow += new Infragistics.Win.UltraWinGrid.DoubleClickRowEventHandler(this.alertsGrid_DoubleClickRow);
            this.alertsGrid.MouseDown += new System.Windows.Forms.MouseEventHandler(this.alertsGrid_MouseDown);
            // 
            // alertsViewDataSource
            // 
            this.alertsViewDataSource.AllowAdd = false;
            this.alertsViewDataSource.AllowDelete = false;
            ultraDataColumn15.AllowDBNull = Infragistics.Win.DefaultableBoolean.False;
            ultraDataColumn15.DataType = typeof(long);
            ultraDataColumn16.AllowDBNull = Infragistics.Win.DefaultableBoolean.False;
            ultraDataColumn16.DataType = typeof(System.DateTime);
            ultraDataColumn17.AllowDBNull = Infragistics.Win.DefaultableBoolean.True;
            ultraDataColumn18.AllowDBNull = Infragistics.Win.DefaultableBoolean.True;
            ultraDataColumn19.AllowDBNull = Infragistics.Win.DefaultableBoolean.True;
            ultraDataColumn20.AllowDBNull = Infragistics.Win.DefaultableBoolean.True;
            ultraDataColumn20.DataType = typeof(bool);
            ultraDataColumn21.AllowDBNull = Infragistics.Win.DefaultableBoolean.True;
            ultraDataColumn21.DataType = typeof(int);
            ultraDataColumn22.AllowDBNull = Infragistics.Win.DefaultableBoolean.True;
            ultraDataColumn22.DataType = typeof(byte);
            ultraDataColumn23.AllowDBNull = Infragistics.Win.DefaultableBoolean.True;
            ultraDataColumn23.DataType = typeof(byte);
            ultraDataColumn24.AllowDBNull = Infragistics.Win.DefaultableBoolean.True;
            ultraDataColumn24.DataType = typeof(double);
            ultraDataColumn25.AllowDBNull = Infragistics.Win.DefaultableBoolean.True;
            ultraDataColumn26.AllowDBNull = Infragistics.Win.DefaultableBoolean.True;
            this.alertsViewDataSource.Band.Columns.AddRange(new object[] {
            ultraDataColumn15,
            ultraDataColumn16,
            ultraDataColumn17,
            ultraDataColumn18,
            ultraDataColumn19,
            ultraDataColumn20,
            ultraDataColumn21,
            ultraDataColumn22,
            ultraDataColumn23,
            ultraDataColumn24,
            ultraDataColumn25,
            ultraDataColumn26});
            this.alertsViewDataSource.KeyIndex = 0;
            // 
            // recentAlertsHeaderLabel
            // 
            this.recentAlertsHeaderLabel.BackColor = System.Drawing.SystemColors.Control;
            this.recentAlertsHeaderLabel.Dock = System.Windows.Forms.DockStyle.Top;
            this.recentAlertsHeaderLabel.HeaderText = "Active Alerts";
            this.recentAlertsHeaderLabel.Location = new System.Drawing.Point(0, 0);
            this.recentAlertsHeaderLabel.Name = "recentAlertsHeaderLabel";
            this.recentAlertsHeaderLabel.Size = new System.Drawing.Size(655, 20);
            this.recentAlertsHeaderLabel.TabIndex = 0;
            // 
            // alertsStatusLabel
            // 
            this.alertsStatusLabel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.alertsStatusLabel.Location = new System.Drawing.Point(0, 0);
            this.alertsStatusLabel.Name = "alertsStatusLabel";
            this.alertsStatusLabel.Padding = new System.Windows.Forms.Padding(0, 5, 0, 0);
            this.alertsStatusLabel.Size = new System.Drawing.Size(655, 166);
            this.alertsStatusLabel.TabIndex = 2;
            this.alertsStatusLabel.Text = "There are no active items.";
            this.alertsStatusLabel.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            // 
            // commonTasksPanel
            // 
            this.commonTasksPanel.Controls.Add(this.commonTasksLayoutPanel);
            this.commonTasksPanel.Controls.Add(this.commonTasksHeaderStrip);
            this.commonTasksPanel.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.commonTasksPanel.Location = new System.Drawing.Point(1, 341);
            this.commonTasksPanel.Name = "commonTasksPanel";
            this.commonTasksPanel.Size = new System.Drawing.Size(655, 140);
            this.commonTasksPanel.TabIndex = 1;
            // 
            // commonTasksLayoutPanel
            // 
            this.commonTasksLayoutPanel.ColumnCount = 2;
            this.commonTasksLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.commonTasksLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.commonTasksLayoutPanel.Controls.Add(this.generateReportsFeatureButton, 1, 1);
            this.commonTasksLayoutPanel.Controls.Add(this.manageTasksFeatureButton, 0, 1);
            this.commonTasksLayoutPanel.Controls.Add(this.monitorAlertLogFeatureButton, 1, 0);
            this.commonTasksLayoutPanel.Controls.Add(this.addServersFeatureButton, 0, 0);
            this.commonTasksLayoutPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.commonTasksLayoutPanel.Location = new System.Drawing.Point(0, 25);
            this.commonTasksLayoutPanel.Name = "commonTasksLayoutPanel";
            this.commonTasksLayoutPanel.RowCount = 2;
            this.commonTasksLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.commonTasksLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.commonTasksLayoutPanel.Size = new System.Drawing.Size(655, 115);
            this.commonTasksLayoutPanel.TabIndex = 2;
            // 
            // generateReportsFeatureButton
            // 
            this.generateReportsFeatureButton.DescriptionFont = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.generateReportsFeatureButton.DescriptionText = "Create reports on historical performance trends";
            this.generateReportsFeatureButton.Dock = System.Windows.Forms.DockStyle.Fill;
            this.generateReportsFeatureButton.HeaderColor = System.Drawing.Color.Red;
            this.generateReportsFeatureButton.HeaderFont = new System.Drawing.Font("Arial", 11F, System.Drawing.FontStyle.Bold);
            this.generateReportsFeatureButton.HeaderText = "Generate Reports";
            this.generateReportsFeatureButton.Image = global::Idera.SQLdm.DesktopClient.Properties.Resources.ReportsFeature;
            this.generateReportsFeatureButton.Location = new System.Drawing.Point(330, 60);
            this.generateReportsFeatureButton.MinimumSize = new System.Drawing.Size(0, 40);
            this.generateReportsFeatureButton.Name = "generateReportsFeatureButton";
            this.generateReportsFeatureButton.Size = new System.Drawing.Size(322, 52);
            this.generateReportsFeatureButton.TabIndex = 3;
            this.generateReportsFeatureButton.MouseClick += new System.Windows.Forms.MouseEventHandler(this.generateReportsFeatureButton_MouseClick);
            // 
            // manageTasksFeatureButton
            // 
            this.manageTasksFeatureButton.DescriptionFont = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.manageTasksFeatureButton.DescriptionText = "Track critical work tasks";
            this.manageTasksFeatureButton.Dock = System.Windows.Forms.DockStyle.Fill;
            this.manageTasksFeatureButton.HeaderColor = System.Drawing.Color.Red;
            this.manageTasksFeatureButton.HeaderFont = new System.Drawing.Font("Arial", 11F, System.Drawing.FontStyle.Bold);
            this.manageTasksFeatureButton.HeaderText = "Manage To Do Items";
            this.manageTasksFeatureButton.Image = global::Idera.SQLdm.DesktopClient.Properties.Resources.TasksFeature;
            this.manageTasksFeatureButton.Location = new System.Drawing.Point(3, 60);
            this.manageTasksFeatureButton.MinimumSize = new System.Drawing.Size(0, 40);
            this.manageTasksFeatureButton.Name = "manageTasksFeatureButton";
            this.manageTasksFeatureButton.Size = new System.Drawing.Size(321, 52);
            this.manageTasksFeatureButton.TabIndex = 2;
            this.manageTasksFeatureButton.MouseClick += new System.Windows.Forms.MouseEventHandler(this.manageTasksFeatureButton_MouseClick);
            // 
            // monitorAlertLogFeatureButton
            // 
            this.monitorAlertLogFeatureButton.DescriptionFont = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.monitorAlertLogFeatureButton.DescriptionText = "Monitor alerts raised by SQLDM in real-time";
            this.monitorAlertLogFeatureButton.Dock = System.Windows.Forms.DockStyle.Fill;
            this.monitorAlertLogFeatureButton.HeaderColor = System.Drawing.Color.Red;
            this.monitorAlertLogFeatureButton.HeaderFont = new System.Drawing.Font("Arial", 11F, System.Drawing.FontStyle.Bold);
            this.monitorAlertLogFeatureButton.HeaderText = "Monitor Alert Log";
            this.monitorAlertLogFeatureButton.Image = global::Idera.SQLdm.DesktopClient.Properties.Resources.AlertsFeature;
            this.monitorAlertLogFeatureButton.Location = new System.Drawing.Point(330, 3);
            this.monitorAlertLogFeatureButton.MinimumSize = new System.Drawing.Size(0, 40);
            this.monitorAlertLogFeatureButton.Name = "monitorAlertLogFeatureButton";
            this.monitorAlertLogFeatureButton.Size = new System.Drawing.Size(322, 51);
            this.monitorAlertLogFeatureButton.TabIndex = 1;
            this.monitorAlertLogFeatureButton.MouseClick += new System.Windows.Forms.MouseEventHandler(this.monitorAlertLogFeatureButton_MouseClick);
            // 
            // addServersFeatureButton
            // 
            this.addServersFeatureButton.DescriptionFont = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.addServersFeatureButton.DescriptionText = "Add, remove and configure monitored SQL Servers";
            this.addServersFeatureButton.Dock = System.Windows.Forms.DockStyle.Fill;
            this.addServersFeatureButton.HeaderColor = System.Drawing.Color.Red;
            this.addServersFeatureButton.HeaderFont = new System.Drawing.Font("Arial", 11F, System.Drawing.FontStyle.Bold);
            this.addServersFeatureButton.HeaderText = "Manage Servers";
            this.addServersFeatureButton.Image = global::Idera.SQLdm.DesktopClient.Properties.Resources.AddServersFeature;
            this.addServersFeatureButton.Location = new System.Drawing.Point(3, 3);
            this.addServersFeatureButton.MinimumSize = new System.Drawing.Size(0, 40);
            this.addServersFeatureButton.Name = "addServersFeatureButton";
            this.addServersFeatureButton.Size = new System.Drawing.Size(321, 51);
            this.addServersFeatureButton.TabIndex = 0;
            this.addServersFeatureButton.MouseClick += new System.Windows.Forms.MouseEventHandler(this.addServersFeatureButton_MouseClick);
            // 
            // commonTasksHeaderStrip
            // 
            this.commonTasksHeaderStrip.AutoSize = false;
            this.commonTasksHeaderStrip.Font = new System.Drawing.Font("Arial", 12.75F, System.Drawing.FontStyle.Bold);
            this.commonTasksHeaderStrip.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(21)))), ((int)(((byte)(66)))), ((int)(((byte)(139)))));
            this.commonTasksHeaderStrip.GripStyle = System.Windows.Forms.ToolStripGripStyle.Hidden;
            this.commonTasksHeaderStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.commonTasksHeaderTitleLabel,
            this.toggleCommonTasksButton});
            this.commonTasksHeaderStrip.Location = new System.Drawing.Point(0, 0);
            this.commonTasksHeaderStrip.Name = "commonTasksHeaderStrip";
            this.commonTasksHeaderStrip.Padding = new System.Windows.Forms.Padding(0, 2, 0, 0);
            this.commonTasksHeaderStrip.Size = new System.Drawing.Size(655, 25);
            this.commonTasksHeaderStrip.TabIndex = 1;
            this.commonTasksHeaderStrip.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler(this.commonTasksHeaderStrip_MouseDoubleClick);
            // 
            // commonTasksHeaderTitleLabel
            // 
            this.commonTasksHeaderTitleLabel.Name = "commonTasksHeaderTitleLabel";
            this.commonTasksHeaderTitleLabel.Size = new System.Drawing.Size(129, 20);
            this.commonTasksHeaderTitleLabel.Text = "Common Tasks";
            // 
            // toggleCommonTasksButton
            // 
            this.toggleCommonTasksButton.Alignment = System.Windows.Forms.ToolStripItemAlignment.Right;
            this.toggleCommonTasksButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.toggleCommonTasksButton.Image = global::Idera.SQLdm.DesktopClient.Properties.Resources.DownArrows;
            this.toggleCommonTasksButton.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None;
            this.toggleCommonTasksButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toggleCommonTasksButton.Name = "toggleCommonTasksButton";
            this.toggleCommonTasksButton.Size = new System.Drawing.Size(23, 20);
            this.toggleCommonTasksButton.Text = "Hide Common Tasks";
            this.toggleCommonTasksButton.Click += new System.EventHandler(this.toggleCommonTasksButton_Click);
            // 
            // headerPanel
            // 
            this.headerPanel.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(191)))), ((int)(((byte)(204)))), ((int)(((byte)(204)))));
            this.headerPanel.BackgroundImage = global::Idera.SQLdm.DesktopClient.Properties.Resources.TodayPageHeader;
            this.headerPanel.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.headerPanel.Controls.Add(this.headerDividerLabel);
            this.headerPanel.Controls.Add(this.refreshTimeLabel);
            this.headerPanel.Controls.Add(this.refreshDateLabel);
            this.headerPanel.Dock = System.Windows.Forms.DockStyle.Top;
            this.headerPanel.Location = new System.Drawing.Point(0, 25);
            this.headerPanel.Name = "headerPanel";
            this.headerPanel.Padding = new System.Windows.Forms.Padding(1);
            this.headerPanel.Size = new System.Drawing.Size(657, 73);
            this.headerPanel.TabIndex = 1;
            // 
            // headerDividerLabel
            // 
            this.headerDividerLabel.BackColor = System.Drawing.Color.LightGray;
            this.headerDividerLabel.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.headerDividerLabel.Location = new System.Drawing.Point(1, 71);
            this.headerDividerLabel.Name = "headerDividerLabel";
            this.headerDividerLabel.Size = new System.Drawing.Size(655, 1);
            this.headerDividerLabel.TabIndex = 2;
            // 
            // refreshTimeLabel
            // 
            this.refreshTimeLabel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.refreshTimeLabel.BackColor = System.Drawing.Color.Transparent;
            this.refreshTimeLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.refreshTimeLabel.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(89)))), ((int)(((byte)(89)))), ((int)(((byte)(89)))));
            this.refreshTimeLabel.ImageAlign = System.Drawing.ContentAlignment.TopLeft;
            this.refreshTimeLabel.Location = new System.Drawing.Point(461, 47);
            this.refreshTimeLabel.Name = "refreshTimeLabel";
            this.refreshTimeLabel.Size = new System.Drawing.Size(191, 25);
            this.refreshTimeLabel.TabIndex = 1;
            this.refreshTimeLabel.Text = "12:00 AM";
            this.refreshTimeLabel.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // refreshDateLabel
            // 
            this.refreshDateLabel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.refreshDateLabel.BackColor = System.Drawing.Color.Transparent;
            this.refreshDateLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.refreshDateLabel.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(89)))), ((int)(((byte)(89)))), ((int)(((byte)(89)))));
            this.refreshDateLabel.ImageAlign = System.Drawing.ContentAlignment.TopLeft;
            this.refreshDateLabel.Location = new System.Drawing.Point(408, 27);
            this.refreshDateLabel.Name = "refreshDateLabel";
            this.refreshDateLabel.Size = new System.Drawing.Size(244, 25);
            this.refreshDateLabel.TabIndex = 0;
            this.refreshDateLabel.Text = "Monday, January 1, 2007";
            this.refreshDateLabel.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // headerStrip
            // 
            this.headerStrip.AutoSize = false;
            this.headerStrip.Font = new System.Drawing.Font("Arial", 12.75F, System.Drawing.FontStyle.Bold);
            this.headerStrip.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(21)))), ((int)(((byte)(66)))), ((int)(((byte)(139)))));
            this.headerStrip.GripStyle = System.Windows.Forms.ToolStripGripStyle.Hidden;
            this.headerStrip.HeaderImage = global::Idera.SQLdm.DesktopClient.Properties.Resources.ToolbarHome;
            this.headerStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.titleLabel});
            this.headerStrip.Location = new System.Drawing.Point(0, 0);
            this.headerStrip.Name = "headerStrip";
            this.headerStrip.Padding = new System.Windows.Forms.Padding(20, 2, 0, 0);
            this.headerStrip.Size = new System.Drawing.Size(657, 25);
            this.headerStrip.TabIndex = 0;
            // 
            // titleLabel
            // 
            this.titleLabel.Name = "titleLabel";
            this.titleLabel.Size = new System.Drawing.Size(119, 20);
            this.titleLabel.Text = "SQLDM Today";
            // 
            // toolbarsManager
            // 
            this.toolbarsManager.DesignerFlags = 0;
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
            appearance26.Image = global::Idera.SQLdm.DesktopClient.Properties.Resources.ColumnChooser;
            buttonTool6.SharedPropsInternal.AppearancesSmall.Appearance = appearance26;
            buttonTool6.SharedPropsInternal.Caption = "Column Chooser";
            appearance27.Image = global::Idera.SQLdm.DesktopClient.Properties.Resources.GroupByBox;
            buttonTool7.SharedPropsInternal.AppearancesSmall.Appearance = appearance27;
            buttonTool7.SharedPropsInternal.Caption = "Group By Box";
            appearance28.Image = global::Idera.SQLdm.DesktopClient.Properties.Resources.SortAscending;
            buttonTool8.SharedPropsInternal.AppearancesSmall.Appearance = appearance28;
            buttonTool8.SharedPropsInternal.Caption = "Sort Ascending";
            appearance29.Image = global::Idera.SQLdm.DesktopClient.Properties.Resources.SortDescending;
            buttonTool9.SharedPropsInternal.AppearancesSmall.Appearance = appearance29;
            buttonTool9.SharedPropsInternal.Caption = "Sort Descending";
            buttonTool10.SharedPropsInternal.Caption = "Remove This Column";
            stateButtonTool2.MenuDisplayStyle = Infragistics.Win.UltraWinToolbars.StateButtonMenuDisplayStyle.DisplayCheckmark;
            stateButtonTool2.SharedPropsInternal.Caption = "Group By This Column";
            popupMenuTool2.SharedPropsInternal.Caption = "gridContextMenu";
            buttonTool11.InstanceProps.IsFirstInGroup = true;
            buttonTool13.InstanceProps.IsFirstInGroup = true;
            buttonTool14.InstanceProps.IsFirstInGroup = true;
            popupMenuTool2.Tools.AddRange(new Infragistics.Win.UltraWinToolbars.ToolBase[] {
            buttonTool48,
            buttonTool38,
            buttonTool39,
            buttonTool43,
            buttonTool11,
            buttonTool12,
            buttonTool13,
            buttonTool47,
            buttonTool14});
            appearance30.Image = global::Idera.SQLdm.DesktopClient.Properties.Resources.Print16x16;
            buttonTool15.SharedPropsInternal.AppearancesSmall.Appearance = appearance30;
            buttonTool15.SharedPropsInternal.Caption = "Print";
            appearance31.Image = global::Idera.SQLdm.DesktopClient.Properties.Resources.Export16x16;
            buttonTool16.SharedPropsInternal.AppearancesSmall.Appearance = appearance31;
            buttonTool16.SharedPropsInternal.Caption = "Export To Excel";
            buttonTool17.SharedPropsInternal.Caption = "Configure Alerts...";
            popupMenuTool3.SharedPropsInternal.Caption = "taskGridContextMenu";
            popupMenuTool4.InstanceProps.IsFirstInGroup = true;
            buttonTool20.InstanceProps.IsFirstInGroup = true;
            buttonTool21.InstanceProps.IsFirstInGroup = true;
            popupMenuTool3.Tools.AddRange(new Infragistics.Win.UltraWinToolbars.ToolBase[] {
            buttonTool46,
            buttonTool45,
            popupMenuTool4,
            buttonTool18,
            buttonTool20,
            buttonTool21});
            buttonTool22.SharedPropsInternal.Caption = "Delete";
            buttonTool23.SharedPropsInternal.Caption = "Show Associated View";
            appearance65.FontData.BoldAsString = "True";
            buttonTool24.SharedPropsInternal.AppearancesSmall.Appearance = appearance65;
            buttonTool24.SharedPropsInternal.Caption = "Properties";
            popupMenuTool5.SharedPropsInternal.Caption = "Set Status";
            popupMenuTool5.Tools.AddRange(new Infragistics.Win.UltraWinToolbars.ToolBase[] {
            buttonTool25,
            buttonTool26,
            buttonTool27,
            buttonTool28});
            buttonTool29.SharedPropsInternal.Caption = "Not Started";
            buttonTool30.SharedPropsInternal.Caption = "In Progress";
            buttonTool31.SharedPropsInternal.Caption = "On Hold";
            buttonTool32.SharedPropsInternal.Caption = "Completed";
            appearance32.Image = global::Idera.SQLdm.DesktopClient.Properties.Resources.copy;
            buttonTool33.SharedPropsInternal.AppearancesSmall.Appearance = appearance32;
            buttonTool33.SharedPropsInternal.Caption = "Copy To Clipboard";
            buttonTool34.SharedPropsInternal.Caption = "Clear Alert";
            buttonTool35.SharedPropsInternal.Caption = "Clear All Alerts of this Type for this Instance";
            appearance43.FontData.BoldAsString = "True";
            buttonTool36.SharedPropsInternal.AppearancesSmall.Appearance = appearance43;
            buttonTool36.SharedPropsInternal.Caption = "Show Real Time View";
            buttonTool37.SharedPropsInternal.Caption = "Show Historical View";
            buttonTool40.SharedPropsInternal.Caption = "Show Real Time View";
            buttonTool40.SharedPropsInternal.DisplayStyle = Infragistics.Win.UltraWinToolbars.ToolDisplayStyle.DefaultForToolType;
            buttonTool41.SharedPropsInternal.Caption = "Show Historical View";
            buttonTool19.SharedPropsInternal.Caption = "Snooze Alert...";
            buttonTool42.SharedPropsInternal.Caption = "Show Alert Help";
            buttonTool44.SharedPropsInternal.Caption = "Show Deadlock Details...";
            buttonTool44.SharedPropsInternal.Visible = false;
            this.toolbarsManager.Tools.AddRange(new Infragistics.Win.UltraWinToolbars.ToolBase[] {
            popupMenuTool1,
            buttonTool6,
            buttonTool7,
            buttonTool8,
            buttonTool9,
            buttonTool10,
            stateButtonTool2,
            popupMenuTool2,
            buttonTool15,
            buttonTool16,
            buttonTool17,
            popupMenuTool3,
            buttonTool22,
            buttonTool23,
            buttonTool24,
            popupMenuTool5,
            buttonTool29,
            buttonTool30,
            buttonTool31,
            buttonTool32,
            buttonTool33,
            buttonTool34,
            buttonTool35,
            buttonTool36,
            buttonTool37,
            buttonTool40,
            buttonTool41,
            buttonTool19,
            buttonTool42,
            buttonTool44});
            this.toolbarsManager.BeforeToolDropdown += new Infragistics.Win.UltraWinToolbars.BeforeToolDropdownEventHandler(this.toolbarsManager_BeforeToolDropdown);
            this.toolbarsManager.ToolClick += new Infragistics.Win.UltraWinToolbars.ToolClickEventHandler(this.toolbarsManager_ToolClick);
            // 
            // ultraGridPrintDocument
            // 
            this.ultraGridPrintDocument.DocumentName = "";
            // 
            // ultraPrintPreviewDialog
            // 
            this.ultraPrintPreviewDialog.Document = this.ultraGridPrintDocument;
            this.ultraPrintPreviewDialog.Name = "ultraPrintPreviewDialog";
            // 
            // saveFileDialog
            // 
            this.saveFileDialog.DefaultExt = "xls";
            this.saveFileDialog.Filter = "Excel Workbook (*.xls)|*.xls";
            this.saveFileDialog.Title = "Save as Excel Spreadsheet";
            // 
            // SQLdmTodayView
            // 
            this.Controls.Add(this.contentPanel);
            this.Controls.Add(this.headerPanel);
            this.Controls.Add(this.headerStrip);
            this.Name = "SQLdmTodayView";
            this.Size = new System.Drawing.Size(657, 580);
            this.Load += new System.EventHandler(this.SQLdmTodayView_Load);
            this.ultraTabPageControl1.ResumeLayout(false);
            this.tasksPanel.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.tasksGrid)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.tasksGridDataSource)).EndInit();
            this.ultraTabPageControl2.ResumeLayout(false);
            this.panel1.ResumeLayout(false);
            this.contentPanel.ResumeLayout(false);
            this.contentSplitter.Panel1.ResumeLayout(false);
            this.contentSplitter.Panel2.ResumeLayout(false);
            this.contentSplitter.ResumeLayout(false);
            this.contentLayoutPanel.ResumeLayout(false);
            this.statusSummaryPanel.ResumeLayout(false);
            this.statusSummaryContentPanel.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.statusSummaryPictureBox)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.criticalServersPictureBox)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.warningServersPictureBox)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.okServersPictureBox)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.maintenanceModeServersPictureBox)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.tasksForecastTabGroup)).EndInit();
            this.tasksForecastTabGroup.ResumeLayout(false);
            this.alertsPanel.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.alertsGrid)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.alertsViewDataSource)).EndInit();
            this.commonTasksPanel.ResumeLayout(false);
            this.commonTasksLayoutPanel.ResumeLayout(false);
            this.commonTasksHeaderStrip.ResumeLayout(false);
            this.commonTasksHeaderStrip.PerformLayout();
            this.headerPanel.ResumeLayout(false);
            this.headerStrip.ResumeLayout(false);
            this.headerStrip.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.toolbarsManager)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private Idera.SQLdm.DesktopClient.Controls.HeaderStrip headerStrip;
        private System.Windows.Forms.ToolStripLabel titleLabel;
        private Idera.SQLdm.DesktopClient.Controls.GradientPanel contentPanel;
        private System.Windows.Forms.Panel headerPanel;
        private Idera.SQLdm.DesktopClient.Controls.FeatureButton addServersFeatureButton;
        private System.Windows.Forms.Panel commonTasksPanel;
        private Idera.SQLdm.DesktopClient.Controls.HeaderStrip commonTasksHeaderStrip;
        private System.Windows.Forms.ToolStripLabel commonTasksHeaderTitleLabel;
        private System.Windows.Forms.TableLayoutPanel commonTasksLayoutPanel;
        private System.Windows.Forms.ToolStripButton toggleCommonTasksButton;
        private Idera.SQLdm.DesktopClient.Controls.FeatureButton generateReportsFeatureButton;
        private Idera.SQLdm.DesktopClient.Controls.FeatureButton manageTasksFeatureButton;
        private Idera.SQLdm.DesktopClient.Controls.FeatureButton monitorAlertLogFeatureButton;
        private System.Windows.Forms.TableLayoutPanel contentLayoutPanel;
        private System.Windows.Forms.Panel alertsPanel;
        private Idera.SQLdm.DesktopClient.Controls.HeaderLabel recentAlertsHeaderLabel;
        private System.Windows.Forms.Label refreshTimeLabel;
        private System.Windows.Forms.Label refreshDateLabel;
        private System.Windows.Forms.Label headerDividerLabel;
        private Infragistics.Win.UltraWinGrid.UltraGrid alertsGrid;
        private System.Windows.Forms.Label columnDividerLabel;
        private System.Windows.Forms.Panel statusSummaryPanel;
        private Idera.SQLdm.DesktopClient.Controls.HeaderLabel statusSummaryHeaderLabel;
        private System.Windows.Forms.PictureBox statusSummaryPictureBox;
        private System.Windows.Forms.LinkLabel statusSummaryDescriptionLabel;
        private System.Windows.Forms.Label statusSummaryDividerLabel;
        private System.Windows.Forms.PictureBox criticalServersPictureBox;
        private System.Windows.Forms.PictureBox warningServersPictureBox;
        private System.Windows.Forms.PictureBox maintenanceModeServersPictureBox;
        private System.Windows.Forms.PictureBox okServersPictureBox;
        private System.Windows.Forms.LinkLabel criticalServersLabel;
        private System.Windows.Forms.LinkLabel warningServersLabel;
        private System.Windows.Forms.LinkLabel okServersLabel;
        private System.Windows.Forms.LinkLabel maintenanceModeServersLabel;
        private System.Windows.Forms.Label alertsStatusLabel;
        private System.Windows.Forms.LinkLabel statusSummaryLabel;
        private System.Windows.Forms.Panel statusSummaryContentPanel;
        private System.Windows.Forms.Label statusSummaryLoadingLabel;
        private Idera.SQLdm.DesktopClient.Views.Alerts.AlertsViewDataSource alertsViewDataSource;
        private TaskGridDataSource tasksGridDataSource;
        private System.Windows.Forms.SplitContainer contentSplitter;
        private Idera.SQLdm.DesktopClient.Controls.ContextMenuManager toolbarsManager;
        private Infragistics.Win.UltraWinGrid.UltraGridPrintDocument ultraGridPrintDocument;
        private Infragistics.Win.UltraWinGrid.ExcelExport.UltraGridExcelExporter ultraGridExcelExporter;
        private Infragistics.Win.Printing.UltraPrintPreviewDialog ultraPrintPreviewDialog;
        private System.Windows.Forms.SaveFileDialog saveFileDialog;
        private Infragistics.Win.UltraWinTabControl.UltraTabControl tasksForecastTabGroup;
        private Infragistics.Win.UltraWinTabControl.UltraTabSharedControlsPage ultraTabSharedControlsPage1;
        private Infragistics.Win.UltraWinTabControl.UltraTabPageControl ultraTabPageControl1;
        private System.Windows.Forms.Panel tasksPanel;
        private Infragistics.Win.UltraWinGrid.UltraGrid tasksGrid;
        private System.Windows.Forms.Label tasksStatusLabel;
        private Infragistics.Win.UltraWinTabControl.UltraTabPageControl ultraTabPageControl2;
        private Idera.SQLdm.DesktopClient.Controls.AlertForecastPanel alertForecastPanel1;
        private System.Windows.Forms.Panel panel1;
    }
}
