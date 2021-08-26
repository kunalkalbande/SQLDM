namespace Idera.SQLdm.DesktopClient.Views.Servers.ServerGroup
{
    partial class ServerGroupDetailsView
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
            if (disposing)
            {
                // unhook event handlers to allow view to dispose
                 foreach (var key in ApplicationModel.Default.RepoActiveInstances.Keys)
                    ApplicationModel.Default.ActiveInstances.Changed -= ActiveInstances_Changed;
            }

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
            Infragistics.Win.UltraWinToolbars.PopupMenuTool popupMenuTool1 = new Infragistics.Win.UltraWinToolbars.PopupMenuTool("columnContextMenu");
            Infragistics.Win.UltraWinToolbars.ButtonTool buttonTool1 = new Infragistics.Win.UltraWinToolbars.ButtonTool("sortAscendingButton");
            Infragistics.Win.UltraWinToolbars.ButtonTool buttonTool2 = new Infragistics.Win.UltraWinToolbars.ButtonTool("sortDescendingButton");
            Infragistics.Win.UltraWinToolbars.StateButtonTool stateButtonTool1 = new Infragistics.Win.UltraWinToolbars.StateButtonTool("groupByThisColumnButton", "");
            Infragistics.Win.UltraWinToolbars.ButtonTool buttonTool3 = new Infragistics.Win.UltraWinToolbars.ButtonTool("toggleGroupByBoxButton");
            Infragistics.Win.UltraWinToolbars.ButtonTool buttonTool4 = new Infragistics.Win.UltraWinToolbars.ButtonTool("removeThisColumnButton");
            Infragistics.Win.UltraWinToolbars.ButtonTool buttonTool5 = new Infragistics.Win.UltraWinToolbars.ButtonTool("showColumnChooserButton");
            Infragistics.Win.UltraWinToolbars.ButtonTool buttonTool6 = new Infragistics.Win.UltraWinToolbars.ButtonTool("showColumnChooserButton");
            Infragistics.Win.Appearance appearance13 = new Infragistics.Win.Appearance();
            Infragistics.Win.UltraWinToolbars.ButtonTool buttonTool7 = new Infragistics.Win.UltraWinToolbars.ButtonTool("toggleGroupByBoxButton");
            Infragistics.Win.Appearance appearance14 = new Infragistics.Win.Appearance();
            Infragistics.Win.UltraWinToolbars.ButtonTool buttonTool8 = new Infragistics.Win.UltraWinToolbars.ButtonTool("sortAscendingButton");
            Infragistics.Win.Appearance appearance15 = new Infragistics.Win.Appearance();
            Infragistics.Win.UltraWinToolbars.ButtonTool buttonTool9 = new Infragistics.Win.UltraWinToolbars.ButtonTool("sortDescendingButton");
            Infragistics.Win.Appearance appearance16 = new Infragistics.Win.Appearance();
            Infragistics.Win.UltraWinToolbars.PopupMenuTool popupMenuTool2 = new Infragistics.Win.UltraWinToolbars.PopupMenuTool("instanceContextMenu");
            Infragistics.Win.UltraWinToolbars.ButtonTool buttonTool10 = new Infragistics.Win.UltraWinToolbars.ButtonTool("openInstanceButton");
            Infragistics.Win.UltraWinToolbars.ButtonTool buttonTool11 = new Infragistics.Win.UltraWinToolbars.ButtonTool("refreshInstanceButton");
            Infragistics.Win.UltraWinToolbars.ButtonTool buttonTool12 = new Infragistics.Win.UltraWinToolbars.ButtonTool("deleteInstanceButton");
            Infragistics.Win.UltraWinToolbars.ButtonTool buttonTool13 = new Infragistics.Win.UltraWinToolbars.ButtonTool("collapseAllGroupsButton");
            Infragistics.Win.UltraWinToolbars.ButtonTool buttonTool14 = new Infragistics.Win.UltraWinToolbars.ButtonTool("expandAllGroupsButton");
            Infragistics.Win.UltraWinToolbars.ButtonTool buttonTool15 = new Infragistics.Win.UltraWinToolbars.ButtonTool("printGridButton");
            Infragistics.Win.UltraWinToolbars.ButtonTool buttonTool16 = new Infragistics.Win.UltraWinToolbars.ButtonTool("exportGridButton");
            Infragistics.Win.UltraWinToolbars.ButtonTool buttonTool17 = new Infragistics.Win.UltraWinToolbars.ButtonTool("showInstancePropertiesButton");
            Infragistics.Win.UltraWinToolbars.ButtonTool buttonTool18 = new Infragistics.Win.UltraWinToolbars.ButtonTool("refreshInstanceButton");
            Infragistics.Win.Appearance appearance17 = new Infragistics.Win.Appearance();
            Infragistics.Win.UltraWinToolbars.ButtonTool buttonTool19 = new Infragistics.Win.UltraWinToolbars.ButtonTool("openInstanceButton");
            Infragistics.Win.UltraWinToolbars.ButtonTool buttonTool20 = new Infragistics.Win.UltraWinToolbars.ButtonTool("deleteInstanceButton");
            Infragistics.Win.Appearance appearance18 = new Infragistics.Win.Appearance();
            Infragistics.Win.UltraWinToolbars.ButtonTool buttonTool21 = new Infragistics.Win.UltraWinToolbars.ButtonTool("showInstancePropertiesButton");
            Infragistics.Win.Appearance appearance19 = new Infragistics.Win.Appearance();
            Infragistics.Win.UltraWinToolbars.ButtonTool buttonTool22 = new Infragistics.Win.UltraWinToolbars.ButtonTool("removeThisColumnButton");
            Infragistics.Win.UltraWinToolbars.StateButtonTool stateButtonTool2 = new Infragistics.Win.UltraWinToolbars.StateButtonTool("groupByThisColumnButton", "");
            Infragistics.Win.UltraWinToolbars.PopupMenuTool popupMenuTool3 = new Infragistics.Win.UltraWinToolbars.PopupMenuTool("gridContextMenu");
            Infragistics.Win.UltraWinToolbars.ButtonTool buttonTool23 = new Infragistics.Win.UltraWinToolbars.ButtonTool("collapseAllGroupsButton");
            Infragistics.Win.UltraWinToolbars.ButtonTool buttonTool24 = new Infragistics.Win.UltraWinToolbars.ButtonTool("expandAllGroupsButton");
            Infragistics.Win.UltraWinToolbars.ButtonTool buttonTool25 = new Infragistics.Win.UltraWinToolbars.ButtonTool("printGridButton");
            Infragistics.Win.UltraWinToolbars.ButtonTool buttonTool26 = new Infragistics.Win.UltraWinToolbars.ButtonTool("exportGridButton");
            Infragistics.Win.UltraWinToolbars.ButtonTool buttonTool27 = new Infragistics.Win.UltraWinToolbars.ButtonTool("exportGridButton");
            Infragistics.Win.Appearance appearance20 = new Infragistics.Win.Appearance();
            Infragistics.Win.UltraWinToolbars.ButtonTool buttonTool28 = new Infragistics.Win.UltraWinToolbars.ButtonTool("printGridButton");
            Infragistics.Win.Appearance appearance21 = new Infragistics.Win.Appearance();
            Infragistics.Win.UltraWinToolbars.ButtonTool buttonTool29 = new Infragistics.Win.UltraWinToolbars.ButtonTool("collapseAllGroupsButton");
            Infragistics.Win.UltraWinToolbars.ButtonTool buttonTool30 = new Infragistics.Win.UltraWinToolbars.ButtonTool("expandAllGroupsButton");
            Infragistics.Win.Appearance appearance22 = new Infragistics.Win.Appearance();
            Infragistics.Win.UltraWinGrid.UltraGridBand ultraGridBand1 = new Infragistics.Win.UltraWinGrid.UltraGridBand("Main Band", -1);
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn1 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("SQLServerID");
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn2 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("InstanceName", -1, null, 0, Infragistics.Win.UltraWinGrid.SortIndicator.Ascending, false);
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn3 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("UTCCollectionDateTime");
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn4 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("AgentServiceStatus", -1, 195913626);
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn5 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("OSAvailableMemoryInKilobytes");
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn6 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("BlockedProcesses");
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn7 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("BufferCacheHitRatioPercentage");
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn8 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("CheckpointWrites");
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn9 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("ClientComputers");
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn10 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("CPUActivityPercentage");
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn11 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("CPUTimeDelta");
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn12 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("CPUTimeRaw");
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn13 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("DiskQueueLength");
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn14 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("DiskTimePercent");
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn15 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("DTCServiceStatus", -1, 195913626);
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn16 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("FullScans");
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn17 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("FullTextSearchStatus", -1, 195913626);
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn18 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("IdleTimeDelta");
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn19 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("IdleTimePercentage");
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn20 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("IdleTimeRaw");
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn21 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("IOActivityPercentage");
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn22 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("IOTimeDelta");
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn23 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("IOTimeRaw");
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn24 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("LazyWriterWrites");
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn25 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("LockWaits");
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn26 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("Logins");
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn27 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("LogWrites");
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn28 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("SqlMemoryAllocatedInMegabytes");
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn29 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("SqlMemoryUsedInMegabytes");
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn30 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("OldestOpenTransactionsInMinutes");
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn31 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("OpenTransactions");
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn32 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("PacketErrors");
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn33 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("PacketsReceived");
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn34 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("PacketsSent");
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn35 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("PageErrors");
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn36 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("PageLifeExpectancy");
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn37 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("PageLookups");
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn38 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("PageReads");
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn39 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("PagesPerSecond");
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn40 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("PageSplits");
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn41 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("PageWrites");
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn42 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("PrivilegedTimePercent");
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn43 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("ProcedureCacheHitRatioPercentage");
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn44 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("ProcedureCacheSizeInKilobytes");
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn45 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("ProcedureCacheSizePercent");
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn46 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("ProcessorQueueLength");
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn47 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("ProcessorTimePercent");
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn48 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("ReadAheadPages");
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn49 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("ReplicationLatencyInSeconds");
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn50 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("ReplicationSubscribed");
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn51 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("ReplicationUndistributed");
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn52 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("ReplicationUnsubscribed");
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn53 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("ResponseTimeInMilliseconds");
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn54 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("ServerVersion");
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn55 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("SqlCompilations");
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn56 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("SqlRecompilations");
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn57 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("SqlServerServiceStatus", -1, 195913626);
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn58 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("SystemProcesses");
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn59 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("SystemProcessesConsumingCPU");
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn60 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("TableLockEscalations");
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn61 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("TempDBSizeInKilobytes");
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn62 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("TempDBSizePercent");
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn63 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("OSTotalPhysicalMemoryInKilobytes");
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn64 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("Transactions");
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn65 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("UserProcesses");
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn66 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("UserProcessesConsumingCPU");
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn67 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("UserTimePercent");
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn68 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("WorkFilesCreated");
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn69 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("WorkTablesCreated");
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn70 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("Severity", -1, 972251);
            Infragistics.Win.Appearance appearance23 = new Infragistics.Win.Appearance();
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn71 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("Status");
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn72 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("SQLBrowserServiceStatus", -1, 195913626);
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn73 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("SQLActiveDirectoryHelperServiceStatus", -1, 195913626);
            Infragistics.Win.Appearance appearance24 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance25 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance26 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance27 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance28 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance29 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance30 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance31 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance32 = new Infragistics.Win.Appearance();
            Infragistics.Win.ValueList valueList1 = new Infragistics.Win.ValueList(195913626);
            Infragistics.Win.ValueList valueList2 = new Infragistics.Win.ValueList(972251);
            Infragistics.Win.Appearance appearance33 = new Infragistics.Win.Appearance();
            this.toolbarsManager = new Idera.SQLdm.DesktopClient.Controls.ContextMenuManager(this.components);
            this.ServerGroupDetailsView2_Fill_Panel = new System.Windows.Forms.Panel();
            this.detailsGrid = new Infragistics.Win.UltraWinGrid.UltraGrid();
            this.ultraGridExcelExporter = new Infragistics.Win.UltraWinGrid.ExcelExport.UltraGridExcelExporter(this.components);
            this.saveFileDialog = new System.Windows.Forms.SaveFileDialog();
            this.ultraGridPrintDocument = new Infragistics.Win.UltraWinGrid.UltraGridPrintDocument(this.components);
            this.ultraPrintPreviewDialog = new Infragistics.Win.Printing.UltraPrintPreviewDialog(this.components);
            this.lblNoSqlServers = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.toolbarsManager)).BeginInit();
            this.ServerGroupDetailsView2_Fill_Panel.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.detailsGrid)).BeginInit();
            this.SuspendLayout();
            // 
            // toolbarsManager
            // 
            this.toolbarsManager.DesignerFlags = 1;
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
            appearance13.Image = global::Idera.SQLdm.DesktopClient.Properties.Resources.ColumnChooser;
            buttonTool6.SharedPropsInternal.AppearancesSmall.Appearance = appearance13;
            buttonTool6.SharedPropsInternal.Caption = "Column Chooser";
            appearance14.Image = global::Idera.SQLdm.DesktopClient.Properties.Resources.GroupByBox;
            buttonTool7.SharedPropsInternal.AppearancesSmall.Appearance = appearance14;
            buttonTool7.SharedPropsInternal.Caption = "Group By Box";
            appearance15.Image = global::Idera.SQLdm.DesktopClient.Properties.Resources.SortAscending;
            buttonTool8.SharedPropsInternal.AppearancesSmall.Appearance = appearance15;
            buttonTool8.SharedPropsInternal.Caption = "Sort Ascending";
            appearance16.Image = global::Idera.SQLdm.DesktopClient.Properties.Resources.SortDescending;
            buttonTool9.SharedPropsInternal.AppearancesSmall.Appearance = appearance16;
            buttonTool9.SharedPropsInternal.Caption = "Sort Descending";
            popupMenuTool2.SharedPropsInternal.Caption = "instanceContextMenu";
            buttonTool11.InstanceProps.IsFirstInGroup = true;
            buttonTool12.InstanceProps.IsFirstInGroup = true;
            buttonTool13.InstanceProps.IsFirstInGroup = true;
            buttonTool15.InstanceProps.IsFirstInGroup = true;
            buttonTool17.InstanceProps.IsFirstInGroup = true;
            popupMenuTool2.Tools.AddRange(new Infragistics.Win.UltraWinToolbars.ToolBase[] {
            buttonTool10,
            buttonTool11,
            buttonTool12,
            buttonTool13,
            buttonTool14,
            buttonTool15,
            buttonTool16,
            buttonTool17});
            appearance17.Image = global::Idera.SQLdm.DesktopClient.Properties.Resources.ToolbarRefresh;
            buttonTool18.SharedPropsInternal.AppearancesSmall.Appearance = appearance17;
            buttonTool18.SharedPropsInternal.Caption = "Refresh Alerts";
            buttonTool19.SharedPropsInternal.Caption = "Open";
            appearance18.Image = global::Idera.SQLdm.DesktopClient.Properties.Resources.Delete;
            buttonTool20.SharedPropsInternal.AppearancesSmall.Appearance = appearance18;
            buttonTool20.SharedPropsInternal.Caption = "Delete";
            buttonTool20.SharedPropsInternal.Shortcut = System.Windows.Forms.Shortcut.Del;
            appearance19.Image = global::Idera.SQLdm.DesktopClient.Properties.Resources.Properties;
            buttonTool21.SharedPropsInternal.AppearancesSmall.Appearance = appearance19;
            buttonTool21.SharedPropsInternal.Caption = "Properties...";
            buttonTool22.SharedPropsInternal.Caption = "Remove This Column";
            stateButtonTool2.MenuDisplayStyle = Infragistics.Win.UltraWinToolbars.StateButtonMenuDisplayStyle.DisplayCheckmark;
            stateButtonTool2.SharedPropsInternal.Caption = "Group By This Column";
            popupMenuTool3.SharedPropsInternal.Caption = "gridContextMenu";
            buttonTool25.InstanceProps.IsFirstInGroup = true;
            popupMenuTool3.Tools.AddRange(new Infragistics.Win.UltraWinToolbars.ToolBase[] {
            buttonTool23,
            buttonTool24,
            buttonTool25,
            buttonTool26});
            appearance20.Image = global::Idera.SQLdm.DesktopClient.Properties.Resources.Export16x16;
            buttonTool27.SharedPropsInternal.AppearancesSmall.Appearance = appearance20;
            buttonTool27.SharedPropsInternal.Caption = "Export To Excel";
            appearance21.Image = global::Idera.SQLdm.DesktopClient.Properties.Resources.Print16x16;
            buttonTool28.SharedPropsInternal.AppearancesSmall.Appearance = appearance21;
            buttonTool28.SharedPropsInternal.Caption = "Print";
            buttonTool29.SharedPropsInternal.Caption = "Collapse All Groups";
            buttonTool30.SharedPropsInternal.Caption = "Expand All Groups";
            this.toolbarsManager.Tools.AddRange(new Infragistics.Win.UltraWinToolbars.ToolBase[] {
            popupMenuTool1,
            buttonTool6,
            buttonTool7,
            buttonTool8,
            buttonTool9,
            popupMenuTool2,
            buttonTool18,
            buttonTool19,
            buttonTool20,
            buttonTool21,
            buttonTool22,
            stateButtonTool2,
            popupMenuTool3,
            buttonTool27,
            buttonTool28,
            buttonTool29,
            buttonTool30});
            this.toolbarsManager.BeforeToolDropdown += new Infragistics.Win.UltraWinToolbars.BeforeToolDropdownEventHandler(this.toolbarsManager_BeforeToolDropdown);
            this.toolbarsManager.ToolClick += new Infragistics.Win.UltraWinToolbars.ToolClickEventHandler(this.toolbarsManager_ToolClick);
            // 
            // ServerGroupDetailsView2_Fill_Panel
            // 
            this.ServerGroupDetailsView2_Fill_Panel.Controls.Add(this.detailsGrid);
            this.ServerGroupDetailsView2_Fill_Panel.Cursor = System.Windows.Forms.Cursors.Default;
            this.ServerGroupDetailsView2_Fill_Panel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.ServerGroupDetailsView2_Fill_Panel.Location = new System.Drawing.Point(0, 0);
            this.ServerGroupDetailsView2_Fill_Panel.Name = "ServerGroupDetailsView2_Fill_Panel";
            this.ServerGroupDetailsView2_Fill_Panel.Size = new System.Drawing.Size(453, 433);
            this.ServerGroupDetailsView2_Fill_Panel.TabIndex = 0;
            // 
            // detailsGrid
            // 
            appearance22.BackColor = System.Drawing.SystemColors.Window;
            appearance22.BorderColor = System.Drawing.SystemColors.InactiveCaption;
            this.detailsGrid.DisplayLayout.Appearance = appearance22;
            ultraGridColumn1.Format = "";
            ultraGridColumn1.Header.Caption = "SQLdm ID";
            ultraGridColumn1.Header.VisiblePosition = 2;
            ultraGridColumn1.Hidden = true;
            ultraGridColumn2.Header.Caption = "Instance";
            ultraGridColumn2.Header.Fixed = true;
            ultraGridColumn2.Header.VisiblePosition = 1;
            ultraGridColumn3.Format = "G";
            ultraGridColumn3.Header.Caption = "Last Refresh";
            ultraGridColumn3.Header.VisiblePosition = 3;
            ultraGridColumn4.Header.Caption = "SQL Agent Status";
            ultraGridColumn4.Header.VisiblePosition = 7;
            ultraGridColumn5.Header.Caption = "Available Memory (MB)";
            ultraGridColumn5.Header.VisiblePosition = 10;
            ultraGridColumn6.Format = "N0";
            ultraGridColumn6.Header.Caption = "Blocked Processes";
            ultraGridColumn6.Header.VisiblePosition = 11;
            ultraGridColumn7.Format = "0.00\\%";
            ultraGridColumn7.Header.Caption = "Buffer Cache Hit Ratio";
            ultraGridColumn7.Header.VisiblePosition = 12;
            ultraGridColumn8.Format = "N0";
            ultraGridColumn8.Header.Caption = "Checkpoint Writes";
            ultraGridColumn8.Header.VisiblePosition = 13;
            ultraGridColumn9.Format = "N0";
            ultraGridColumn9.Header.Caption = "Client Computers";
            ultraGridColumn9.Header.VisiblePosition = 14;
            ultraGridColumn10.Format = "0.00\\%";
            ultraGridColumn10.Header.Caption = "CPU Activity";
            ultraGridColumn10.Header.VisiblePosition = 15;
            ultraGridColumn11.ExcludeFromColumnChooser = Infragistics.Win.UltraWinGrid.ExcludeFromColumnChooser.True;
            ultraGridColumn11.Header.VisiblePosition = 16;
            ultraGridColumn11.Hidden = true;
            ultraGridColumn12.ExcludeFromColumnChooser = Infragistics.Win.UltraWinGrid.ExcludeFromColumnChooser.True;
            ultraGridColumn12.Header.VisiblePosition = 17;
            ultraGridColumn12.Hidden = true;
            ultraGridColumn13.Format = "N2";
            ultraGridColumn13.Header.Caption = "Disk Queue Length";
            ultraGridColumn13.Header.VisiblePosition = 18;
            ultraGridColumn14.Format = "0.00\\%";
            ultraGridColumn14.Header.Caption = "% Disk Time";
            ultraGridColumn14.Header.VisiblePosition = 19;
            ultraGridColumn15.Header.Caption = "DTC Status";
            ultraGridColumn15.Header.VisiblePosition = 8;
            ultraGridColumn16.Format = "N0";
            ultraGridColumn16.Header.Caption = "Full Scans";
            ultraGridColumn16.Header.VisiblePosition = 20;
            ultraGridColumn17.Header.Caption = "Full Text Search Status";
            ultraGridColumn17.Header.VisiblePosition = 9;
            ultraGridColumn18.ExcludeFromColumnChooser = Infragistics.Win.UltraWinGrid.ExcludeFromColumnChooser.True;
            ultraGridColumn18.Header.VisiblePosition = 21;
            ultraGridColumn18.Hidden = true;
            ultraGridColumn19.Format = "0.00\\%";
            ultraGridColumn19.Header.Caption = "Idle Time";
            ultraGridColumn19.Header.VisiblePosition = 22;
            ultraGridColumn20.ExcludeFromColumnChooser = Infragistics.Win.UltraWinGrid.ExcludeFromColumnChooser.True;
            ultraGridColumn20.Header.VisiblePosition = 23;
            ultraGridColumn20.Hidden = true;
            ultraGridColumn21.Format = "0.00\\%";
            ultraGridColumn21.Header.Caption = "I/O Activity";
            ultraGridColumn21.Header.VisiblePosition = 24;
            ultraGridColumn22.ExcludeFromColumnChooser = Infragistics.Win.UltraWinGrid.ExcludeFromColumnChooser.True;
            ultraGridColumn22.Header.VisiblePosition = 25;
            ultraGridColumn22.Hidden = true;
            ultraGridColumn23.ExcludeFromColumnChooser = Infragistics.Win.UltraWinGrid.ExcludeFromColumnChooser.True;
            ultraGridColumn23.Header.VisiblePosition = 26;
            ultraGridColumn23.Hidden = true;
            ultraGridColumn24.Format = "N0";
            ultraGridColumn24.Header.Caption = "Lazy Writer Writes";
            ultraGridColumn24.Header.VisiblePosition = 27;
            ultraGridColumn25.Format = "N0";
            ultraGridColumn25.Header.Caption = "Lock Waits";
            ultraGridColumn25.Header.VisiblePosition = 28;
            ultraGridColumn26.Format = "N0";
            ultraGridColumn26.Header.VisiblePosition = 29;
            ultraGridColumn27.Format = "N0";
            ultraGridColumn27.Header.Caption = "Log Writes";
            ultraGridColumn27.Header.VisiblePosition = 30;
            ultraGridColumn28.Format = "N0";
            ultraGridColumn28.Header.Caption = "Memory Allocated (MB)";
            ultraGridColumn28.Header.VisiblePosition = 31;
            ultraGridColumn29.Format = "N0";
            ultraGridColumn29.Header.Caption = "Memory Used (MB)";
            ultraGridColumn29.Header.VisiblePosition = 32;
            ultraGridColumn30.Format = "N0";
            ultraGridColumn30.Header.Caption = "Oldest Open Transactions (mins)";
            ultraGridColumn30.Header.VisiblePosition = 33;
            ultraGridColumn31.Format = "N0";
            ultraGridColumn31.Header.Caption = "Open Transactions";
            ultraGridColumn31.Header.VisiblePosition = 34;
            ultraGridColumn32.Format = "N0";
            ultraGridColumn32.Header.Caption = "Packet Errors";
            ultraGridColumn32.Header.VisiblePosition = 35;
            ultraGridColumn33.Format = "N0";
            ultraGridColumn33.Header.Caption = "Packets Received";
            ultraGridColumn33.Header.VisiblePosition = 36;
            ultraGridColumn34.Format = "N0";
            ultraGridColumn34.Header.Caption = "Packets Sent";
            ultraGridColumn34.Header.VisiblePosition = 37;
            ultraGridColumn35.Format = "N0";
            ultraGridColumn35.Header.Caption = "Page Errors";
            ultraGridColumn35.Header.VisiblePosition = 38;
            ultraGridColumn36.Format = "N0";
            ultraGridColumn36.Header.Caption = "Page Life Expectancy";
            ultraGridColumn36.Header.VisiblePosition = 39;
            ultraGridColumn37.Format = "N0";
            ultraGridColumn37.Header.Caption = "Page Lookups";
            ultraGridColumn37.Header.VisiblePosition = 40;
            ultraGridColumn38.Format = "N0";
            ultraGridColumn38.Header.Caption = "Page Reads";
            ultraGridColumn38.Header.VisiblePosition = 41;
            ultraGridColumn39.Format = "N2";
            ultraGridColumn39.Header.Caption = "Pages Per Second";
            ultraGridColumn39.Header.VisiblePosition = 42;
            ultraGridColumn40.Format = "N0";
            ultraGridColumn40.Header.Caption = "Page Splits";
            ultraGridColumn40.Header.VisiblePosition = 43;
            ultraGridColumn41.Format = "N0";
            ultraGridColumn41.Header.Caption = "Page Writes";
            ultraGridColumn41.Header.VisiblePosition = 44;
            ultraGridColumn42.Format = "0.00\\%";
            ultraGridColumn42.Header.Caption = "Privileged Time";
            ultraGridColumn42.Header.VisiblePosition = 45;
            ultraGridColumn43.Format = "0.00\\%";
            ultraGridColumn43.Header.Caption = "Procedure Cache Hit Ratio";
            ultraGridColumn43.Header.VisiblePosition = 46;
            ultraGridColumn44.Format = "N0";
            ultraGridColumn44.Header.Caption = "Procedure Cache Size (KB)";
            ultraGridColumn44.Header.VisiblePosition = 47;
            ultraGridColumn45.Format = "0.00\\%";
            ultraGridColumn45.Header.Caption = "Procedure Cache Size Percent";
            ultraGridColumn45.Header.VisiblePosition = 48;
            ultraGridColumn46.Format = "N2";
            ultraGridColumn46.Header.Caption = "Processor Queue Length";
            ultraGridColumn46.Header.VisiblePosition = 49;
            ultraGridColumn47.Format = "0.00\\%";
            ultraGridColumn47.Header.Caption = "Processor Time";
            ultraGridColumn47.Header.VisiblePosition = 50;
            ultraGridColumn48.Format = "N0";
            ultraGridColumn48.Header.Caption = "Read Ahead Pages";
            ultraGridColumn48.Header.VisiblePosition = 51;
            ultraGridColumn49.Format = "N2";
            ultraGridColumn49.Header.Caption = "Replication Latency";
            ultraGridColumn49.Header.VisiblePosition = 52;
            ultraGridColumn50.Format = "N0";
            ultraGridColumn50.Header.Caption = "Replication Subscribed";
            ultraGridColumn50.Header.VisiblePosition = 53;
            ultraGridColumn51.Format = "N0";
            ultraGridColumn51.Header.Caption = "Replication Undistributed";
            ultraGridColumn51.Header.VisiblePosition = 54;
            ultraGridColumn52.Format = "N0";
            ultraGridColumn52.Header.Caption = "Replication Unsubscribed";
            ultraGridColumn52.Header.VisiblePosition = 55;
            ultraGridColumn53.Format = "N0";
            ultraGridColumn53.Header.Caption = "Response Time (ms)";
            ultraGridColumn53.Header.VisiblePosition = 56;
            ultraGridColumn54.Header.Caption = "SQL Server Version";
            ultraGridColumn54.Header.VisiblePosition = 4;
            ultraGridColumn55.Format = "N0";
            ultraGridColumn55.Header.Caption = "SQL Compilations";
            ultraGridColumn55.Header.VisiblePosition = 57;
            ultraGridColumn56.Format = "N0";
            ultraGridColumn56.Header.Caption = "SQL Recompilations";
            ultraGridColumn56.Header.VisiblePosition = 58;
            ultraGridColumn57.Header.Caption = "SQL Server Status";
            ultraGridColumn57.Header.VisiblePosition = 6;
            ultraGridColumn58.Format = "N0";
            ultraGridColumn58.Header.Caption = "System Processes";
            ultraGridColumn58.Header.VisiblePosition = 59;
            ultraGridColumn59.Format = "N0";
            ultraGridColumn59.Header.Caption = "System Processes Consuming CPU";
            ultraGridColumn59.Header.VisiblePosition = 60;
            ultraGridColumn60.Format = "N0";
            ultraGridColumn60.Header.Caption = "Table Lock Escalations";
            ultraGridColumn60.Header.VisiblePosition = 61;
            ultraGridColumn61.Format = "N0";
            ultraGridColumn61.Header.Caption = "Tempdb Size (KB)";
            ultraGridColumn61.Header.VisiblePosition = 62;
            ultraGridColumn62.Format = "0.00\\%";
            ultraGridColumn62.Header.Caption = "Tempdb Size Percent";
            ultraGridColumn62.Header.VisiblePosition = 63;
            ultraGridColumn63.Format = "N0";
            ultraGridColumn63.Header.Caption = "Physical Memory (MB)";
            ultraGridColumn63.Header.VisiblePosition = 64;
            ultraGridColumn64.Format = "N0";
            ultraGridColumn64.Header.VisiblePosition = 65;
            ultraGridColumn65.Format = "N0";
            ultraGridColumn65.Header.Caption = "User Processes";
            ultraGridColumn65.Header.VisiblePosition = 66;
            ultraGridColumn66.Format = "N0";
            ultraGridColumn66.Header.Caption = "User Processes Consuming CPU";
            ultraGridColumn66.Header.VisiblePosition = 67;
            ultraGridColumn67.Format = "0.00\\%";
            ultraGridColumn67.Header.Caption = "User Time";
            ultraGridColumn67.Header.VisiblePosition = 68;
            ultraGridColumn68.Format = "N0";
            ultraGridColumn68.Header.Caption = "Work Files Created";
            ultraGridColumn68.Header.VisiblePosition = 69;
            ultraGridColumn69.Format = "N0";
            ultraGridColumn69.Header.Caption = "Work Tables Created";
            ultraGridColumn69.Header.VisiblePosition = 70;
            appearance23.FontData.BoldAsString = "True";
            appearance23.ForeColor = System.Drawing.Color.Red;
            appearance23.TextHAlignAsString = "Center";
            ultraGridColumn70.Header.Appearance = appearance23;
            ultraGridColumn70.Header.Caption = "!";
            ultraGridColumn70.Header.Fixed = true;
            ultraGridColumn70.Header.VisiblePosition = 0;
            ultraGridColumn70.MaxWidth = 20;
            ultraGridColumn70.MinWidth = 20;
            ultraGridColumn70.Width = 20;
            ultraGridColumn71.Header.VisiblePosition = 5;
            ultraGridColumn71.Hidden = true;
            ultraGridColumn71.Width = 235;
            ultraGridColumn72.Header.Caption = "SQL Browser Service Status";
            ultraGridColumn72.Header.VisiblePosition = 71;
            ultraGridColumn73.Header.Caption = "SQL Active Directory Helper Service Status";
            ultraGridColumn73.Header.VisiblePosition = 72;
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
            ultraGridColumn25,
            ultraGridColumn26,
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
            ultraGridColumn40,
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
            ultraGridColumn52,
            ultraGridColumn53,
            ultraGridColumn54,
            ultraGridColumn55,
            ultraGridColumn56,
            ultraGridColumn57,
            ultraGridColumn58,
            ultraGridColumn59,
            ultraGridColumn60,
            ultraGridColumn61,
            ultraGridColumn62,
            ultraGridColumn63,
            ultraGridColumn64,
            ultraGridColumn65,
            ultraGridColumn66,
            ultraGridColumn67,
            ultraGridColumn68,
            ultraGridColumn69,
            ultraGridColumn70,
            ultraGridColumn71,
            ultraGridColumn72,
            ultraGridColumn73});
            this.detailsGrid.DisplayLayout.BandsSerializer.Add(ultraGridBand1);
            this.detailsGrid.DisplayLayout.BorderStyle = Infragistics.Win.UIElementBorderStyle.None;
            this.detailsGrid.DisplayLayout.CaptionVisible = Infragistics.Win.DefaultableBoolean.False;
            appearance24.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(235)))), ((int)(((byte)(235)))), ((int)(((byte)(235)))));
            appearance24.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(140)))), ((int)(((byte)(140)))), ((int)(((byte)(140)))));
            this.detailsGrid.DisplayLayout.GroupByBox.Appearance = appearance24;
            appearance25.ForeColor = System.Drawing.SystemColors.GrayText;
            this.detailsGrid.DisplayLayout.GroupByBox.BandLabelAppearance = appearance25;
            this.detailsGrid.DisplayLayout.GroupByBox.BorderStyle = Infragistics.Win.UIElementBorderStyle.Solid;
            this.detailsGrid.DisplayLayout.GroupByBox.Hidden = true;
            appearance26.BackColor = System.Drawing.SystemColors.ControlLightLight;
            appearance26.BackColor2 = System.Drawing.SystemColors.Control;
            appearance26.BackGradientStyle = Infragistics.Win.GradientStyle.Horizontal;
            appearance26.ForeColor = System.Drawing.SystemColors.GrayText;
            this.detailsGrid.DisplayLayout.GroupByBox.PromptAppearance = appearance26;
            this.detailsGrid.DisplayLayout.MaxColScrollRegions = 1;
            this.detailsGrid.DisplayLayout.MaxRowScrollRegions = 1;
            this.detailsGrid.DisplayLayout.Override.AllowAddNew = Infragistics.Win.UltraWinGrid.AllowAddNew.No;
            this.detailsGrid.DisplayLayout.Override.AllowDelete = Infragistics.Win.DefaultableBoolean.False;
            this.detailsGrid.DisplayLayout.Override.AllowUpdate = Infragistics.Win.DefaultableBoolean.False;
            this.detailsGrid.DisplayLayout.Override.BorderStyleCell = Infragistics.Win.UIElementBorderStyle.Solid;
            this.detailsGrid.DisplayLayout.Override.BorderStyleRow = Infragistics.Win.UIElementBorderStyle.Solid;
            appearance27.BackColor = System.Drawing.SystemColors.Window;
            this.detailsGrid.DisplayLayout.Override.CardAreaAppearance = appearance27;
            appearance28.BorderColor = System.Drawing.Color.Silver;
            appearance28.TextTrimming = Infragistics.Win.TextTrimming.EllipsisCharacter;
            this.detailsGrid.DisplayLayout.Override.CellAppearance = appearance28;
            this.detailsGrid.DisplayLayout.Override.CellClickAction = Infragistics.Win.UltraWinGrid.CellClickAction.RowSelect;
            this.detailsGrid.DisplayLayout.Override.CellPadding = 0;
            this.detailsGrid.DisplayLayout.Override.FixedHeaderIndicator = Infragistics.Win.UltraWinGrid.FixedHeaderIndicator.None;
            this.detailsGrid.DisplayLayout.Override.GroupByColumnsHidden = Infragistics.Win.DefaultableBoolean.False;
            appearance29.BackColor = System.Drawing.SystemColors.Control;
            appearance29.BackColor2 = System.Drawing.SystemColors.ControlDark;
            appearance29.BackGradientAlignment = Infragistics.Win.GradientAlignment.Element;
            appearance29.BackGradientStyle = Infragistics.Win.GradientStyle.Horizontal;
            appearance29.BorderColor = System.Drawing.SystemColors.Window;
            this.detailsGrid.DisplayLayout.Override.GroupByRowAppearance = appearance29;
            this.detailsGrid.DisplayLayout.Override.GroupByRowInitialExpansionState = Infragistics.Win.UltraWinGrid.GroupByRowInitialExpansionState.Expanded;
            appearance30.TextHAlignAsString = "Left";
            this.detailsGrid.DisplayLayout.Override.HeaderAppearance = appearance30;
            this.detailsGrid.DisplayLayout.Override.HeaderClickAction = Infragistics.Win.UltraWinGrid.HeaderClickAction.SortMulti;
            this.detailsGrid.DisplayLayout.Override.MaxSelectedRows = 1;
            appearance31.BackColor = System.Drawing.SystemColors.Window;
            appearance31.BorderColor = System.Drawing.Color.Silver;
            this.detailsGrid.DisplayLayout.Override.RowAppearance = appearance31;
            this.detailsGrid.DisplayLayout.Override.RowSelectors = Infragistics.Win.DefaultableBoolean.False;
            this.detailsGrid.DisplayLayout.Override.SelectTypeCell = Infragistics.Win.UltraWinGrid.SelectType.None;
            this.detailsGrid.DisplayLayout.Override.SelectTypeCol = Infragistics.Win.UltraWinGrid.SelectType.None;
            this.detailsGrid.DisplayLayout.Override.SelectTypeGroupByRow = Infragistics.Win.UltraWinGrid.SelectType.None;
            this.detailsGrid.DisplayLayout.Override.SelectTypeRow = Infragistics.Win.UltraWinGrid.SelectType.Single;
            appearance32.BackColor = System.Drawing.SystemColors.ControlLight;
            this.detailsGrid.DisplayLayout.Override.TemplateAddRowAppearance = appearance32;
            this.detailsGrid.DisplayLayout.ScrollBounds = Infragistics.Win.UltraWinGrid.ScrollBounds.ScrollToFill;
            this.detailsGrid.DisplayLayout.ScrollStyle = Infragistics.Win.UltraWinGrid.ScrollStyle.Immediate;
            this.detailsGrid.DisplayLayout.UseFixedHeaders = true;
            valueList1.Key = "serviceStateValueList";
            valueList1.PreferredDropDownSize = new System.Drawing.Size(0, 0);
            appearance33.ImageHAlign = Infragistics.Win.HAlign.Center;
            valueList2.Appearance = appearance33;
            valueList2.DisplayStyle = Infragistics.Win.ValueListDisplayStyle.Picture;
            valueList2.Key = "severityValueList";
            valueList2.PreferredDropDownSize = new System.Drawing.Size(0, 0);
            this.detailsGrid.DisplayLayout.ValueLists.AddRange(new Infragistics.Win.ValueList[] {
            valueList1,
            valueList2});
            this.detailsGrid.DisplayLayout.ViewStyle = Infragistics.Win.UltraWinGrid.ViewStyle.SingleBand;
            this.detailsGrid.DisplayLayout.ViewStyleBand = Infragistics.Win.UltraWinGrid.ViewStyleBand.OutlookGroupBy;
            this.detailsGrid.Dock = System.Windows.Forms.DockStyle.Fill;
            this.detailsGrid.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.detailsGrid.Location = new System.Drawing.Point(0, 0);
            this.detailsGrid.Name = "detailsGrid";
            this.detailsGrid.Size = new System.Drawing.Size(453, 433);
            this.detailsGrid.TabIndex = 0;
            this.detailsGrid.InitializeLayout += new Infragistics.Win.UltraWinGrid.InitializeLayoutEventHandler(this.detailsGrid_InitializeLayout);
            this.detailsGrid.AfterSelectChange += new Infragistics.Win.UltraWinGrid.AfterSelectChangeEventHandler(this.detailsGrid_AfterSelectChange);
            this.detailsGrid.DoubleClickRow += new Infragistics.Win.UltraWinGrid.DoubleClickRowEventHandler(this.detailsGrid_DoubleClickRow);
            this.detailsGrid.MouseDown += new System.Windows.Forms.MouseEventHandler(this.detailsGrid_MouseDown);
            // 
            // ultraGridPrintDocument
            // 
            this.ultraGridPrintDocument.DocumentName = "Server Group";
            this.ultraGridPrintDocument.Grid = this.detailsGrid;
            // 
            // ultraPrintPreviewDialog
            // 
            this.ultraPrintPreviewDialog.Document = this.ultraGridPrintDocument;
            this.ultraPrintPreviewDialog.Name = "ultraPrintPreviewDialog";
            // 
            // lblNoSqlServers
            // 
            this.lblNoSqlServers.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lblNoSqlServers.Location = new System.Drawing.Point(0, 0);
            this.lblNoSqlServers.Name = "viewStatusLabel";
            this.lblNoSqlServers.Padding = new System.Windows.Forms.Padding(0, 5, 0, 0);
            this.lblNoSqlServers.Size = new System.Drawing.Size(662, 553);
            this.lblNoSqlServers.TabIndex = 2;
            this.lblNoSqlServers.Text = "< status label >";
            this.lblNoSqlServers.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            // 
            // ServerGroupDetailsView
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.ServerGroupDetailsView2_Fill_Panel);
            this.Controls.Add(this.lblNoSqlServers);
            this.Name = "ServerGroupDetailsView";
            this.Size = new System.Drawing.Size(453, 433);
            this.Load += new System.EventHandler(this.ServerGroupDetailsView_Load);
            ((System.ComponentModel.ISupportInitialize)(this.toolbarsManager)).EndInit();
            this.ServerGroupDetailsView2_Fill_Panel.ResumeLayout(false);
            this.ServerGroupDetailsView2_Fill_Panel.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.detailsGrid)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private Idera.SQLdm.DesktopClient.Controls.ContextMenuManager toolbarsManager;
        private System.Windows.Forms.Panel ServerGroupDetailsView2_Fill_Panel;
        private Infragistics.Win.UltraWinGrid.UltraGrid detailsGrid;
        private Infragistics.Win.UltraWinGrid.ExcelExport.UltraGridExcelExporter ultraGridExcelExporter;
        private System.Windows.Forms.SaveFileDialog saveFileDialog;
        private Infragistics.Win.UltraWinGrid.UltraGridPrintDocument ultraGridPrintDocument;
        private Infragistics.Win.Printing.UltraPrintPreviewDialog ultraPrintPreviewDialog;
        private System.Windows.Forms.Label lblNoSqlServers;
    }
}
