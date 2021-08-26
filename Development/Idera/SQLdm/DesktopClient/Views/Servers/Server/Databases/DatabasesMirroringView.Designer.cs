using Idera.SQLdm.DesktopClient.Properties;
using Infragistics.Win;
using Infragistics.Windows.Themes;
using System;
using System.Drawing;

namespace Idera.SQLdm.DesktopClient.Views.Servers.Server.Databases
{
    partial class DatabasesMirroringView
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
            Infragistics.Win.Appearance appearance2 = new Infragistics.Win.Appearance();
            Infragistics.Win.UltraWinGrid.UltraGridBand ultraGridBand1 = new Infragistics.Win.UltraWinGrid.UltraGridBand("Band 0", -1);
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn1 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("Time Recorded", -1, null, 1, Infragistics.Win.UltraWinGrid.SortIndicator.Descending, false);
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn2 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("Role");
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn3 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("Mirroring State", -1, 254688844);
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn4 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("Witness Connection", -1, 254817329, 0, Infragistics.Win.UltraWinGrid.SortIndicator.Ascending, false);
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn5 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("Unsent Log");
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn6 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("Time to send");
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn7 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("Send Rate");
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn8 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("New Transaction Rate");
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn9 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("Oldest Unsent Transaction");
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn10 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("Unrestored Log");
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn11 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("Time to restore");
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn12 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("Restore Rate");
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn13 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("Mirror Commit Overhead", 0);
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn14 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("intTimeSpan", 1);
            Infragistics.Win.Appearance appearance3 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance4 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance5 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance6 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance7 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance8 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance9 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance10 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance11 = new Infragistics.Win.Appearance();
            Infragistics.Win.ValueList valueList1 = new Controls.CustomControls.CustomValueList(1145164329);
            Infragistics.Win.ValueList valueList2 = new Controls.CustomControls.CustomValueList(254688844);
            Infragistics.Win.ValueListItem valueListItem21 = new Infragistics.Win.ValueListItem();
            Infragistics.Win.ValueListItem valueListItem22 = new Infragistics.Win.ValueListItem();
            Infragistics.Win.ValueListItem valueListItem23 = new Infragistics.Win.ValueListItem();
            Infragistics.Win.ValueListItem valueListItem24 = new Infragistics.Win.ValueListItem();
            Infragistics.Win.ValueListItem valueListItem25 = new Infragistics.Win.ValueListItem();
            Infragistics.Win.ValueList valueList3 = new Controls.CustomControls.CustomValueList(254817329);
            Infragistics.Win.ValueListItem valueListItem26 = new Infragistics.Win.ValueListItem();
            Infragistics.Win.ValueListItem valueListItem27 = new Infragistics.Win.ValueListItem();
            Infragistics.Win.ValueListItem valueListItem28 = new Infragistics.Win.ValueListItem();
            Infragistics.Win.ValueListItem valueListItem1 = new Infragistics.Win.ValueListItem();
            Infragistics.Win.ValueListItem valueListItem2 = new Infragistics.Win.ValueListItem();
            Infragistics.Win.ValueListItem valueListItem3 = new Infragistics.Win.ValueListItem();
            Infragistics.Win.ValueListItem valueListItem4 = new Infragistics.Win.ValueListItem();
            Infragistics.Win.ValueListItem valueListItem5 = new Infragistics.Win.ValueListItem();
            Infragistics.Win.ValueListItem valueListItem6 = new Infragistics.Win.ValueListItem();
            Infragistics.Win.ValueListItem valueListItem7 = new Infragistics.Win.ValueListItem();
            Infragistics.Win.ValueListItem valueListItem8 = new Infragistics.Win.ValueListItem();
            Infragistics.Win.Appearance appearance81 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance94 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance95 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance96 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance97 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance98 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance99 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance110 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance111 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance112 = new Infragistics.Win.Appearance();
            Infragistics.Win.ValueList valueList4 = new Controls.CustomControls.CustomValueList(1609175141);
            appearance1 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance12 = new Infragistics.Win.Appearance();
            Infragistics.Win.UltraWinGrid.UltraGridBand ultraGridBand2 = new Infragistics.Win.UltraWinGrid.UltraGridBand("Band 0", -1);
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn15 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("Database Name");
            Infragistics.Win.Appearance appearance13 = new Infragistics.Win.Appearance();
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn16 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("Server Instance");
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn17 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("Current Role");
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn18 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("Partner Instance", -1, 30742938);
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn19 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("Mirroring State", -1, 239127688);
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn20 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("Witness Connection", -1, 243636016, 0, Infragistics.Win.UltraWinGrid.SortIndicator.Ascending, false);
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn21 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("Operational State", 0, 238621516);
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn22 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("Operating Mode", 1);
            Infragistics.Win.Appearance appearance14 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance15 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance16 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance17 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance18 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance19 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance20 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance28 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance33 = new Infragistics.Win.Appearance();
            Infragistics.Win.ValueList valueList5 = new Controls.CustomControls.CustomValueList(238621516);
            Infragistics.Win.ValueListItem valueListItem29 = new Infragistics.Win.ValueListItem();
            Infragistics.Win.ValueListItem valueListItem30 = new Infragistics.Win.ValueListItem();
            Infragistics.Win.ValueListItem valueListItem31 = new Infragistics.Win.ValueListItem();
            Infragistics.Win.ValueList valueList6 = new Controls.CustomControls.CustomValueList(239127688);
            Infragistics.Win.ValueListItem valueListItem32 = new Infragistics.Win.ValueListItem();
            Infragistics.Win.ValueListItem valueListItem33 = new Infragistics.Win.ValueListItem();
            Infragistics.Win.ValueListItem valueListItem34 = new Infragistics.Win.ValueListItem();
            Infragistics.Win.ValueListItem valueListItem35 = new Infragistics.Win.ValueListItem();
            Infragistics.Win.ValueListItem valueListItem36 = new Infragistics.Win.ValueListItem();
            Infragistics.Win.ValueList valueList7 = new Controls.CustomControls.CustomValueList(243636016);
            Infragistics.Win.ValueListItem valueListItem37 = new Infragistics.Win.ValueListItem();
            Infragistics.Win.ValueListItem valueListItem38 = new Infragistics.Win.ValueListItem();
            Infragistics.Win.ValueListItem valueListItem39 = new Infragistics.Win.ValueListItem();
            Infragistics.Win.UltraWinTabControl.UltraTab ultraTab1 = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomUltraTab();
            Infragistics.Win.UltraWinTabControl.UltraTab ultraTab2 = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomUltraTab();
            ChartFX.WinForms.Adornments.GradientBackground gradientBackground1 = new ChartFX.WinForms.Adornments.GradientBackground();
            ChartFX.WinForms.SeriesAttributes seriesAttributes1 = new ChartFX.WinForms.SeriesAttributes();
            ChartFX.WinForms.SeriesAttributes seriesAttributes2 = new ChartFX.WinForms.SeriesAttributes();
            ChartFX.WinForms.SeriesAttributes seriesAttributes3 = new ChartFX.WinForms.SeriesAttributes();
            ChartFX.WinForms.SeriesAttributes seriesAttributes4 = new ChartFX.WinForms.SeriesAttributes();
            ChartFX.WinForms.SeriesAttributes seriesAttributes5 = new ChartFX.WinForms.SeriesAttributes();
            Infragistics.Win.UltraWinToolbars.PopupMenuTool popupMenuTool1 = new Infragistics.Win.UltraWinToolbars.PopupMenuTool("detailsGridContextMenu");
            Infragistics.Win.UltraWinToolbars.ButtonTool buttonTool8 = new Controls.CustomControls.CustomButtonTool("mirroringFailOver");
            Infragistics.Win.UltraWinToolbars.ButtonTool buttonTool10 = new Controls.CustomControls.CustomButtonTool("mirroringSuspendResume");
            Infragistics.Win.UltraWinToolbars.ButtonTool buttonTool30 = new Controls.CustomControls.CustomButtonTool("markSessionFailedOver");
            Infragistics.Win.UltraWinToolbars.ButtonTool buttonTool31 = new Controls.CustomControls.CustomButtonTool("markSessionNormal");
            Infragistics.Win.UltraWinToolbars.ButtonTool buttonTool32 = new Controls.CustomControls.CustomButtonTool("markSessionRoleAgnostic");
            Infragistics.Win.UltraWinToolbars.ButtonTool buttonTool22 = new Controls.CustomControls.CustomButtonTool("collapseAllGroupsButton");
            Infragistics.Win.UltraWinToolbars.ButtonTool buttonTool23 = new Controls.CustomControls.CustomButtonTool("expandAllGroupsButton");
            Infragistics.Win.UltraWinToolbars.ButtonTool buttonTool24 = new Controls.CustomControls.CustomButtonTool("printGridButton");
            Infragistics.Win.UltraWinToolbars.ButtonTool buttonTool25 = new Controls.CustomControls.CustomButtonTool("exportGridButton");
            Infragistics.Win.UltraWinToolbars.ButtonTool buttonTool1 = new Controls.CustomControls.CustomButtonTool("sortAscendingButton");
            Infragistics.Win.Appearance appearance21 = new Infragistics.Win.Appearance();
            Infragistics.Win.UltraWinToolbars.ButtonTool buttonTool2 = new Controls.CustomControls.CustomButtonTool("sortDescendingButton");
            Infragistics.Win.Appearance appearance22 = new Infragistics.Win.Appearance();
            Infragistics.Win.UltraWinToolbars.PopupMenuTool popupMenuTool2 = new Infragistics.Win.UltraWinToolbars.PopupMenuTool("columnContextMenu");
            Infragistics.Win.UltraWinToolbars.ButtonTool buttonTool5 = new Controls.CustomControls.CustomButtonTool("sortAscendingButton");
            Infragistics.Win.UltraWinToolbars.ButtonTool buttonTool6 = new Controls.CustomControls.CustomButtonTool("sortDescendingButton");
            Infragistics.Win.UltraWinToolbars.StateButtonTool stateButtonTool3 = new Infragistics.Win.UltraWinToolbars.StateButtonTool("groupByThisColumnButton", "");
            Infragistics.Win.UltraWinToolbars.ButtonTool buttonTool33 = new Controls.CustomControls.CustomButtonTool("toggleGroupByBoxButton");
            Infragistics.Win.UltraWinToolbars.ButtonTool buttonTool20 = new Controls.CustomControls.CustomButtonTool("removeThisColumnButton");
            Infragistics.Win.UltraWinToolbars.ButtonTool buttonTool21 = new Controls.CustomControls.CustomButtonTool("showColumnChooserButton");
            Infragistics.Win.UltraWinToolbars.ButtonTool buttonTool7 = new Controls.CustomControls.CustomButtonTool("mirroringFailOver");
            Infragistics.Win.Appearance appearance43 = new Infragistics.Win.Appearance();
            Infragistics.Win.UltraWinToolbars.PopupMenuTool popupMenuTool3 = new Infragistics.Win.UltraWinToolbars.PopupMenuTool("gridContextMenu");
            Infragistics.Win.UltraWinToolbars.ButtonTool buttonTool4 = new Controls.CustomControls.CustomButtonTool("collapseAllGroupsButton");
            Infragistics.Win.UltraWinToolbars.ButtonTool buttonTool26 = new Controls.CustomControls.CustomButtonTool("expandAllGroupsButton");
            Infragistics.Win.UltraWinToolbars.ButtonTool buttonTool27 = new Controls.CustomControls.CustomButtonTool("printGridButton");
            Infragistics.Win.UltraWinToolbars.ButtonTool buttonTool28 = new Controls.CustomControls.CustomButtonTool("exportGridButton");
            Infragistics.Win.UltraWinToolbars.ButtonTool buttonTool3 = new Controls.CustomControls.CustomButtonTool("collapseAllGroupsButton");
            Infragistics.Win.UltraWinToolbars.ButtonTool buttonTool9 = new Controls.CustomControls.CustomButtonTool("mirroringSuspendResume");
            Infragistics.Win.Appearance appearance30 = new Infragistics.Win.Appearance();
            Infragistics.Win.UltraWinToolbars.ButtonTool buttonTool11 = new Controls.CustomControls.CustomButtonTool("printGridButton");
            Infragistics.Win.Appearance appearance23 = new Infragistics.Win.Appearance();
            Infragistics.Win.UltraWinToolbars.ButtonTool buttonTool12 = new Controls.CustomControls.CustomButtonTool("exportGridButton");
            Infragistics.Win.Appearance appearance24 = new Infragistics.Win.Appearance();
            Infragistics.Win.UltraWinToolbars.ButtonTool buttonTool13 = new Controls.CustomControls.CustomButtonTool("removeThisColumnButton");
            Infragistics.Win.UltraWinToolbars.ButtonTool buttonTool14 = new Controls.CustomControls.CustomButtonTool("showColumnChooserButton");
            Infragistics.Win.Appearance appearance26 = new Infragistics.Win.Appearance();
            Infragistics.Win.UltraWinToolbars.ButtonTool buttonTool15 = new Controls.CustomControls.CustomButtonTool("toggleGroupByBoxButton");
            Infragistics.Win.Appearance appearance27 = new Infragistics.Win.Appearance();
            Infragistics.Win.UltraWinToolbars.ButtonTool buttonTool17 = new Controls.CustomControls.CustomButtonTool("expandAllGroupsButton");
            Infragistics.Win.UltraWinToolbars.StateButtonTool stateButtonTool1 = new Infragistics.Win.UltraWinToolbars.StateButtonTool("groupByThisColumnButton", "");
            Infragistics.Win.Appearance appearance29 = new Infragistics.Win.Appearance();
            Infragistics.Win.UltraWinToolbars.ButtonTool buttonTool16 = new Controls.CustomControls.CustomButtonTool("markSessionNormal");
            Infragistics.Win.UltraWinToolbars.ButtonTool buttonTool18 = new Controls.CustomControls.CustomButtonTool("markSessionFailedOver");
            Infragistics.Win.UltraWinToolbars.ButtonTool buttonTool29 = new Controls.CustomControls.CustomButtonTool("markSessionRoleAgnostic");
            this.statusTabPage = new Infragistics.Win.UltraWinTabControl.UltraTabPageControl();
            this.pnlMessage = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomPanel ();
            this.lblDetailsMessage = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomLabel();
            this.pnlDetails = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomPanel ();
            this.lblMirrorAddress = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomLabel();
            this.lblPrincipalAddress = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomLabel();
            this.lblOperatingMode = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomLabel();
            this.lblWitnessAddress = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomLabel();
            this.lblTimeToSendAndRestore = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomLabel();
            this.lblMirrorCommitOverhead = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomLabel();
            this.mirrorGroup = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomGroupBox();
            this.lblRestoreRate = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomLabel();
            this.lblTimeToRestore = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomLabel();
            this.lblUnrestoredLog = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomLabel();
            this.principalGroup = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomGroupBox();
            this.lblRateofNewTrans = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomLabel();
            this.lblSendRate = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomLabel();
            this.lblOldestUnsent = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomLabel();
            this.lblTimeToSend = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomLabel();
            this.lblUnsentLog = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomLabel();
            this.historyTabPage = new Infragistics.Win.UltraWinTabControl.UltraTabPageControl();
            this.panel2 = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomPanel ();
            this.historyGrid = new Infragistics.Win.UltraWinGrid.UltraGrid();
            this.panel1 = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomPanel ();
            this.panel3 = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomPanel ();
            this.historyPartnerViewRadioButton = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomRadioButton();
            this.historyLocalViewRadioButton = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomRadioButton();
            this.lblFilterLisyBy = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomLabel();
            this.mirrorMonitoringHistoryPeriodComboBox = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomUltraComboEditor();
            this.actionProgressControl = new MRG.Controls.UI.LoadingCircle();
            this.propertiesTabPage = new Infragistics.Win.UltraWinTabControl.UltraTabPageControl();
            this.transactionLogsGrid = new Infragistics.Win.UltraWinGrid.UltraGrid();
            this.DatabasesMirroring_Fill_Panel = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomPanel ();
            this.refreshDatabasesButton = new Infragistics.Win.Misc.UltraButton();
            this.contentContainerPanel = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomPanel ();
            this.splitContainer1 = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomSplitContainer();
            this.mirroredDatabasesGrid = new Infragistics.Win.UltraWinGrid.UltraGrid();
            this.databasesGridStatusLabel = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomLabel();
            this.mirroringDetailsTabControl = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomUltraTabControl();
            this.ultraTabSharedControlsPage1 = new Infragistics.Win.UltraWinTabControl.UltraTabSharedControlsPage();
            this.mirroringDetailsTabControlStatusLabel = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomLabel();
            this.mirroredDatabasesComboBox = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomUltraComboEditor();
            this.disksTabPage = new Infragistics.Win.UltraWinTabControl.UltraTabPageControl();
            this.diskUsagePanel = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomPanel ();
            this.currentDiskUsageChartContainerPanel = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomPanel();
            this.diskUsageChart = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomChart();
            this.diskUsageChartStatusLabel = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomLabel();
            this.refreshAvailableDatabasesBackgroundWorker = new System.ComponentModel.BackgroundWorker();
            this.refreshMirroringDetailsBackgroundWorker = new System.ComponentModel.BackgroundWorker();
            this.refreshMirroringHistoryBackgroundWorker = new System.ComponentModel.BackgroundWorker();
            this.toolbarsManager = new Idera.SQLdm.DesktopClient.Controls.ContextMenuManager(this.components);
            this.failOverBackgroundWorker = new System.ComponentModel.BackgroundWorker();
            this.suspendResumeBackgroundWorker = new System.ComponentModel.BackgroundWorker();
            this.saveFileDialog = new System.Windows.Forms.SaveFileDialog();
            this.ultraGridExcelExporter = new Infragistics.Win.UltraWinGrid.ExcelExport.UltraGridExcelExporter(this.components);
            this.ultraPrintPreviewDialog = new Infragistics.Win.Printing.UltraPrintPreviewDialog(this.components);
            this.ultraGridPrintDocument = new Infragistics.Win.UltraWinGrid.UltraGridPrintDocument(this.components);
            this.setOperationalStatusBackgroundWorker = new System.ComponentModel.BackgroundWorker();
            this.historicalSnapshotStatusLinkLabel = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomLinkLabel();
            this.refreshPartnerDetailsBackgroundWorker = new System.ComponentModel.BackgroundWorker();
            this.statusTabPage.SuspendLayout();
            this.pnlMessage.SuspendLayout();
            this.pnlDetails.SuspendLayout();
            this.mirrorGroup.SuspendLayout();
            this.principalGroup.SuspendLayout();
            this.historyTabPage.SuspendLayout();
            this.panel2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.historyGrid)).BeginInit();
            this.panel1.SuspendLayout();
            this.panel3.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.mirrorMonitoringHistoryPeriodComboBox)).BeginInit();
            this.propertiesTabPage.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.transactionLogsGrid)).BeginInit();
            this.DatabasesMirroring_Fill_Panel.SuspendLayout();
            this.contentContainerPanel.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.mirroredDatabasesGrid)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.mirroringDetailsTabControl)).BeginInit();
            this.mirroringDetailsTabControl.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.mirroredDatabasesComboBox)).BeginInit();
            this.disksTabPage.SuspendLayout();
            this.diskUsagePanel.SuspendLayout();
            this.currentDiskUsageChartContainerPanel.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.diskUsageChart)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.toolbarsManager)).BeginInit();
            this.SuspendLayout();
            // 
            // statusTabPage
            // 
            this.statusTabPage.AutoScroll = true;
            this.statusTabPage.Controls.Add(this.pnlMessage);
            this.statusTabPage.Controls.Add(this.pnlDetails);
            this.statusTabPage.Location = new System.Drawing.Point(2, 24);
            this.statusTabPage.Name = "statusTabPage";
            this.statusTabPage.Size = new System.Drawing.Size(685, 256);
            // 
            // pnlMessage
            // 
            this.pnlMessage.Controls.Add(this.lblDetailsMessage);
            this.pnlMessage.Dock = System.Windows.Forms.DockStyle.Top;
            this.pnlMessage.Location = new System.Drawing.Point(0, 0);
            this.pnlMessage.Name = "pnlMessage";
            this.pnlMessage.Size = new System.Drawing.Size(685, 24);
            this.pnlMessage.TabIndex = 14;
            this.pnlMessage.Visible = false;
            // 
            // lblDetailsMessage
            // 
            this.lblDetailsMessage.AutoSize = true;
            this.lblDetailsMessage.Location = new System.Drawing.Point(8, 5);
            this.lblDetailsMessage.Name = "lblDetailsMessage";
            this.lblDetailsMessage.Size = new System.Drawing.Size(593, 13);
            this.lblDetailsMessage.TabIndex = 0;
            this.lblDetailsMessage.Text = "Note: You are not monitoring both mirroring partners. For complete coverage of mi" +
    "rroring metrics please monitor both partners.";
            // 
            // pnlDetails
            // 
            this.pnlDetails.AutoScroll = true;
            this.pnlDetails.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.pnlDetails.BackColor = System.Drawing.Color.Transparent;
            this.pnlDetails.Controls.Add(this.lblMirrorAddress);
            this.pnlDetails.Controls.Add(this.lblPrincipalAddress);
            this.pnlDetails.Controls.Add(this.lblOperatingMode);
            this.pnlDetails.Controls.Add(this.lblWitnessAddress);
            this.pnlDetails.Controls.Add(this.lblTimeToSendAndRestore);
            this.pnlDetails.Controls.Add(this.lblMirrorCommitOverhead);
            this.pnlDetails.Controls.Add(this.mirrorGroup);
            this.pnlDetails.Controls.Add(this.principalGroup);
            this.pnlDetails.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pnlDetails.Location = new System.Drawing.Point(0, 0);
            this.pnlDetails.Name = "pnlDetails";
            this.pnlDetails.Size = new System.Drawing.Size(685, 256);
            this.pnlDetails.TabIndex = 13;
            // 
            // lblMirrorAddress
            // 
            this.lblMirrorAddress.AutoSize = true;
            this.lblMirrorAddress.BackColor = System.Drawing.Color.Transparent;
            this.lblMirrorAddress.Location = new System.Drawing.Point(344, 164);
            this.lblMirrorAddress.Name = "lblMirrorAddress";
            this.lblMirrorAddress.Size = new System.Drawing.Size(77, 13);
            this.lblMirrorAddress.TabIndex = 18;
            this.lblMirrorAddress.Text = "Mirror Address:";
            // 
            // lblPrincipalAddress
            // 
            this.lblPrincipalAddress.AutoSize = true;
            this.lblPrincipalAddress.BackColor = System.Drawing.Color.Transparent;
            this.lblPrincipalAddress.Location = new System.Drawing.Point(344, 185);
            this.lblPrincipalAddress.Name = "lblPrincipalAddress";
            this.lblPrincipalAddress.Size = new System.Drawing.Size(91, 13);
            this.lblPrincipalAddress.TabIndex = 17;
            this.lblPrincipalAddress.Text = "Principal Address:";
            // 
            // lblOperatingMode
            // 
            this.lblOperatingMode.BackColor = System.Drawing.Color.Transparent;
            this.lblOperatingMode.Location = new System.Drawing.Point(11, 198);
            this.lblOperatingMode.Name = "lblOperatingMode";
            this.lblOperatingMode.Size = new System.Drawing.Size(292, 30);
            this.lblOperatingMode.TabIndex = 16;
            this.lblOperatingMode.Text = "Operating mode:";
            // 
            // lblWitnessAddress
            // 
            this.lblWitnessAddress.AutoSize = true;
            this.lblWitnessAddress.BackColor = System.Drawing.Color.Transparent;
            this.lblWitnessAddress.Location = new System.Drawing.Point(343, 143);
            this.lblWitnessAddress.Name = "lblWitnessAddress";
            this.lblWitnessAddress.Size = new System.Drawing.Size(89, 13);
            this.lblWitnessAddress.TabIndex = 15;
            this.lblWitnessAddress.Text = "Witness Address:";
            // 
            // lblTimeToSendAndRestore
            // 
            this.lblTimeToSendAndRestore.BackColor = System.Drawing.Color.Transparent;
            this.lblTimeToSendAndRestore.Location = new System.Drawing.Point(11, 164);
            this.lblTimeToSendAndRestore.Name = "lblTimeToSendAndRestore";
            this.lblTimeToSendAndRestore.Size = new System.Drawing.Size(292, 30);
            this.lblTimeToSendAndRestore.TabIndex = 14;
            this.lblTimeToSendAndRestore.Text = "Time to send and restore all current log (estimated):";
            // 
            // lblMirrorCommitOverhead
            // 
            this.lblMirrorCommitOverhead.AutoSize = true;
            this.lblMirrorCommitOverhead.BackColor = System.Drawing.Color.Transparent;
            this.lblMirrorCommitOverhead.Location = new System.Drawing.Point(11, 143);
            this.lblMirrorCommitOverhead.Name = "lblMirrorCommitOverhead";
            this.lblMirrorCommitOverhead.Size = new System.Drawing.Size(120, 13);
            this.lblMirrorCommitOverhead.TabIndex = 13;
            this.lblMirrorCommitOverhead.Text = "Mirror commit overhead:";
            // 
            // mirrorGroup
            // 
            this.mirrorGroup.BackColor = System.Drawing.Color.Transparent;
            this.mirrorGroup.Controls.Add(this.lblRestoreRate);
            this.mirrorGroup.Controls.Add(this.lblTimeToRestore);
            this.mirrorGroup.Controls.Add(this.lblUnrestoredLog);
            this.mirrorGroup.Location = new System.Drawing.Point(345, 4);
            this.mirrorGroup.Name = "mirrorGroup";
            this.mirrorGroup.Size = new System.Drawing.Size(326, 132);
            this.mirrorGroup.TabIndex = 8;
            this.mirrorGroup.TabStop = false;
            this.mirrorGroup.Text = "Mirror log";
            // 
            // lblRestoreRate
            // 
            this.lblRestoreRate.AutoSize = true;
            this.lblRestoreRate.BackColor = System.Drawing.Color.Transparent;
            this.lblRestoreRate.Location = new System.Drawing.Point(6, 64);
            this.lblRestoreRate.Name = "lblRestoreRate";
            this.lblRestoreRate.Size = new System.Drawing.Size(100, 13);
            this.lblRestoreRate.TabIndex = 4;
            this.lblRestoreRate.Text = "Current restore rate:";
            // 
            // lblTimeToRestore
            // 
            this.lblTimeToRestore.AutoSize = true;
            this.lblTimeToRestore.BackColor = System.Drawing.Color.Transparent;
            this.lblTimeToRestore.Location = new System.Drawing.Point(6, 42);
            this.lblTimeToRestore.Name = "lblTimeToRestore";
            this.lblTimeToRestore.Size = new System.Drawing.Size(151, 13);
            this.lblTimeToRestore.TabIndex = 2;
            this.lblTimeToRestore.Text = "Time to restore log (estimated):";
            // 
            // lblUnrestoredLog
            // 
            this.lblUnrestoredLog.AutoSize = true;
            this.lblUnrestoredLog.BackColor = System.Drawing.Color.Transparent;
            this.lblUnrestoredLog.Location = new System.Drawing.Point(6, 20);
            this.lblUnrestoredLog.Name = "lblUnrestoredLog";
            this.lblUnrestoredLog.Size = new System.Drawing.Size(79, 13);
            this.lblUnrestoredLog.TabIndex = 1;
            this.lblUnrestoredLog.Text = "Unrestored log:";
            // 
            // principalGroup
            // 
            this.principalGroup.BackColor = System.Drawing.Color.Transparent;
            this.principalGroup.Controls.Add(this.lblRateofNewTrans);
            this.principalGroup.Controls.Add(this.lblSendRate);
            this.principalGroup.Controls.Add(this.lblOldestUnsent);
            this.principalGroup.Controls.Add(this.lblTimeToSend);
            this.principalGroup.Controls.Add(this.lblUnsentLog);
            this.principalGroup.Location = new System.Drawing.Point(5, 4);
            this.principalGroup.Name = "principalGroup";
            this.principalGroup.Size = new System.Drawing.Size(326, 132);
            this.principalGroup.TabIndex = 7;
            this.principalGroup.TabStop = false;
            this.principalGroup.Text = "Principal log";
            // 
            // lblRateofNewTrans
            // 
            this.lblRateofNewTrans.AutoSize = true;
            this.lblRateofNewTrans.BackColor = System.Drawing.Color.Transparent;
            this.lblRateofNewTrans.Location = new System.Drawing.Point(6, 108);
            this.lblRateofNewTrans.Name = "lblRateofNewTrans";
            this.lblRateofNewTrans.Size = new System.Drawing.Size(128, 13);
            this.lblRateofNewTrans.TabIndex = 5;
            this.lblRateofNewTrans.Text = "Rate of new transactions:";
            // 
            // lblSendRate
            // 
            this.lblSendRate.AutoSize = true;
            this.lblSendRate.BackColor = System.Drawing.Color.Transparent;
            this.lblSendRate.Location = new System.Drawing.Point(6, 86);
            this.lblSendRate.Name = "lblSendRate";
            this.lblSendRate.Size = new System.Drawing.Size(56, 13);
            this.lblSendRate.TabIndex = 4;
            this.lblSendRate.Text = "Send rate:";
            // 
            // lblOldestUnsent
            // 
            this.lblOldestUnsent.AutoSize = true;
            this.lblOldestUnsent.BackColor = System.Drawing.Color.Transparent;
            this.lblOldestUnsent.Location = new System.Drawing.Point(6, 42);
            this.lblOldestUnsent.Name = "lblOldestUnsent";
            this.lblOldestUnsent.Size = new System.Drawing.Size(185, 13);
            this.lblOldestUnsent.TabIndex = 3;
            this.lblOldestUnsent.Text = "Oldest unsent transaction (dd.hh:mm):";
            // 
            // lblTimeToSend
            // 
            this.lblTimeToSend.AutoSize = true;
            this.lblTimeToSend.BackColor = System.Drawing.Color.Transparent;
            this.lblTimeToSend.Location = new System.Drawing.Point(6, 64);
            this.lblTimeToSend.Name = "lblTimeToSend";
            this.lblTimeToSend.Size = new System.Drawing.Size(142, 13);
            this.lblTimeToSend.TabIndex = 2;
            this.lblTimeToSend.Text = "Time to send log (estimated):";
            // 
            // lblUnsentLog
            // 
            this.lblUnsentLog.AutoSize = true;
            this.lblUnsentLog.BackColor = System.Drawing.Color.Transparent;
            this.lblUnsentLog.Location = new System.Drawing.Point(6, 20);
            this.lblUnsentLog.Name = "lblUnsentLog";
            this.lblUnsentLog.Size = new System.Drawing.Size(61, 13);
            this.lblUnsentLog.TabIndex = 1;
            this.lblUnsentLog.Text = "Unsent log:";
            // 
            // historyTabPage
            // 
            this.historyTabPage.Controls.Add(this.panel2);
            this.historyTabPage.Controls.Add(this.panel1);
            this.historyTabPage.Location = new System.Drawing.Point(-10000, -10000);
            this.historyTabPage.Name = "historyTabPage";
            this.historyTabPage.Size = new System.Drawing.Size(685, 256);
            // 
            // panel2
            // 
            this.panel2.Controls.Add(this.historyGrid);
            this.panel2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel2.Location = new System.Drawing.Point(0, 39);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(685, 217);
            this.panel2.TabIndex = 13;
            // 
            // historyGrid
            // 
            appearance2.BackColor = System.Drawing.SystemColors.Window;
            appearance2.BorderColor = System.Drawing.SystemColors.InactiveCaption;
            this.historyGrid.DisplayLayout.Appearance = appearance2;
            this.historyGrid.DisplayLayout.AutoFitStyle = Infragistics.Win.UltraWinGrid.AutoFitStyle.ExtendLastColumn;
            ultraGridColumn1.AutoCompleteMode = Infragistics.Win.AutoCompleteMode.Append;
            ultraGridColumn1.Header.VisiblePosition = 0;
            ultraGridColumn2.AutoCompleteMode = Infragistics.Win.AutoCompleteMode.Append;
            ultraGridColumn2.Header.VisiblePosition = 1;
            ultraGridColumn2.Width = 50;
            ultraGridColumn3.AutoCompleteMode = Infragistics.Win.AutoCompleteMode.Append;
            ultraGridColumn3.Header.VisiblePosition = 2;
            ultraGridColumn4.AutoCompleteMode = Infragistics.Win.AutoCompleteMode.Append;
            ultraGridColumn4.Format = "";
            ultraGridColumn4.Header.VisiblePosition = 3;
            ultraGridColumn5.AutoCompleteMode = Infragistics.Win.AutoCompleteMode.Append;
            ultraGridColumn5.Format = "";
            ultraGridColumn5.Header.VisiblePosition = 4;
            ultraGridColumn6.AutoCompleteMode = Infragistics.Win.AutoCompleteMode.Append;
            ultraGridColumn6.Header.VisiblePosition = 5;
            ultraGridColumn7.AutoCompleteMode = Infragistics.Win.AutoCompleteMode.Append;
            ultraGridColumn7.Header.VisiblePosition = 6;
            ultraGridColumn8.AutoCompleteMode = Infragistics.Win.AutoCompleteMode.Append;
            ultraGridColumn8.Header.VisiblePosition = 7;
            ultraGridColumn9.AutoCompleteMode = Infragistics.Win.AutoCompleteMode.Append;
            ultraGridColumn9.Header.VisiblePosition = 8;
            ultraGridColumn10.AutoCompleteMode = Infragistics.Win.AutoCompleteMode.Append;
            ultraGridColumn10.Header.VisiblePosition = 9;
            ultraGridColumn11.AutoCompleteMode = Infragistics.Win.AutoCompleteMode.Append;
            ultraGridColumn11.Header.VisiblePosition = 10;
            ultraGridColumn12.AutoCompleteMode = Infragistics.Win.AutoCompleteMode.Append;
            ultraGridColumn12.Header.VisiblePosition = 11;
            ultraGridColumn13.Header.VisiblePosition = 12;
            ultraGridColumn14.DataType = typeof(long);
            ultraGridColumn14.ExcludeFromColumnChooser = Infragistics.Win.UltraWinGrid.ExcludeFromColumnChooser.True;
            ultraGridColumn14.Header.VisiblePosition = 13;
            ultraGridColumn14.Hidden = true;
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
            ultraGridColumn14});
            this.historyGrid.DisplayLayout.BandsSerializer.Add(ultraGridBand1);
            this.historyGrid.DisplayLayout.BorderStyle = Infragistics.Win.UIElementBorderStyle.None;
            this.historyGrid.DisplayLayout.CaptionVisible = Infragistics.Win.DefaultableBoolean.False;
            appearance3.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(235)))), ((int)(((byte)(235)))), ((int)(((byte)(235)))));
            appearance3.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(140)))), ((int)(((byte)(140)))), ((int)(((byte)(140)))));
            this.historyGrid.DisplayLayout.GroupByBox.Appearance = appearance3;
            appearance4.ForeColor = System.Drawing.SystemColors.GrayText;
            this.historyGrid.DisplayLayout.GroupByBox.BandLabelAppearance = appearance4;
            this.historyGrid.DisplayLayout.GroupByBox.BorderStyle = Infragistics.Win.UIElementBorderStyle.Solid;
            this.historyGrid.DisplayLayout.GroupByBox.Hidden = true;
            appearance5.BackColor = System.Drawing.SystemColors.ControlLightLight;
            appearance5.BackColor2 = System.Drawing.SystemColors.Control;
            appearance5.BackGradientStyle = Infragistics.Win.GradientStyle.Horizontal;
            appearance5.ForeColor = System.Drawing.SystemColors.GrayText;
            this.historyGrid.DisplayLayout.GroupByBox.PromptAppearance = appearance5;
            this.historyGrid.DisplayLayout.MaxColScrollRegions = 1;
            this.historyGrid.DisplayLayout.MaxRowScrollRegions = 1;
            this.historyGrid.DisplayLayout.Override.AllowAddNew = Infragistics.Win.UltraWinGrid.AllowAddNew.No;
            this.historyGrid.DisplayLayout.Override.AllowDelete = Infragistics.Win.DefaultableBoolean.False;
            this.historyGrid.DisplayLayout.Override.AllowUpdate = Infragistics.Win.DefaultableBoolean.False;
            this.historyGrid.DisplayLayout.Override.BorderStyleCell = Infragistics.Win.UIElementBorderStyle.Solid;
            this.historyGrid.DisplayLayout.Override.BorderStyleRow = Infragistics.Win.UIElementBorderStyle.Solid;
            appearance6.BackColor = System.Drawing.SystemColors.Window;
            this.historyGrid.DisplayLayout.Override.CardAreaAppearance = appearance6;
            appearance7.BorderColor = System.Drawing.Color.Silver;
            appearance7.TextTrimming = Infragistics.Win.TextTrimming.EllipsisCharacter;
            this.historyGrid.DisplayLayout.Override.CellAppearance = appearance7;
            this.historyGrid.DisplayLayout.Override.CellClickAction = Infragistics.Win.UltraWinGrid.CellClickAction.RowSelect;
            this.historyGrid.DisplayLayout.Override.CellPadding = 0;
            this.historyGrid.DisplayLayout.Override.FixedHeaderIndicator = Infragistics.Win.UltraWinGrid.FixedHeaderIndicator.None;
            this.historyGrid.DisplayLayout.Override.GroupByColumnsHidden = Infragistics.Win.DefaultableBoolean.False;
            appearance8.BackColor = System.Drawing.SystemColors.Control;
            appearance8.BackColor2 = System.Drawing.SystemColors.ControlDark;
            appearance8.BackGradientAlignment = Infragistics.Win.GradientAlignment.Element;
            appearance8.BackGradientStyle = Infragistics.Win.GradientStyle.Horizontal;
            appearance8.BorderColor = System.Drawing.SystemColors.Window;
            this.historyGrid.DisplayLayout.Override.GroupByRowAppearance = appearance8;
            this.historyGrid.DisplayLayout.Override.GroupByRowInitialExpansionState = Infragistics.Win.UltraWinGrid.GroupByRowInitialExpansionState.Expanded;
            appearance9.TextHAlignAsString = "Left";
            this.historyGrid.DisplayLayout.Override.HeaderAppearance = appearance9;
            this.historyGrid.DisplayLayout.Override.HeaderClickAction = Infragistics.Win.UltraWinGrid.HeaderClickAction.SortMulti;
            appearance10.BackColor = System.Drawing.SystemColors.Window;
            appearance10.BorderColor = System.Drawing.Color.Silver;
            this.historyGrid.DisplayLayout.Override.RowAppearance = appearance10;
            this.historyGrid.DisplayLayout.Override.RowSelectors = Infragistics.Win.DefaultableBoolean.False;
            this.historyGrid.DisplayLayout.Override.SelectTypeCell = Infragistics.Win.UltraWinGrid.SelectType.None;
            this.historyGrid.DisplayLayout.Override.SelectTypeCol = Infragistics.Win.UltraWinGrid.SelectType.None;
            this.historyGrid.DisplayLayout.Override.SelectTypeGroupByRow = Infragistics.Win.UltraWinGrid.SelectType.None;
            this.historyGrid.DisplayLayout.Override.SelectTypeRow = Infragistics.Win.UltraWinGrid.SelectType.Single;
            appearance11.BackColor = System.Drawing.SystemColors.ControlLight;
            this.historyGrid.DisplayLayout.Override.TemplateAddRowAppearance = appearance11;
            this.historyGrid.DisplayLayout.ScrollBounds = Infragistics.Win.UltraWinGrid.ScrollBounds.ScrollToFill;
            this.historyGrid.DisplayLayout.ScrollStyle = Infragistics.Win.UltraWinGrid.ScrollStyle.Immediate;
            this.historyGrid.DisplayLayout.UseFixedHeaders = true;
            valueList1.Key = "autoGrowthValueList";
            valueList1.PreferredDropDownSize = new System.Drawing.Size(0, 0);
            valueList2.Key = "mirroringState";
            valueList2.PreferredDropDownSize = new System.Drawing.Size(0, 0);
            valueListItem21.DataValue = 0;
            valueListItem21.DisplayText = "Suspended";
            valueListItem22.DataValue = 1;
            valueListItem22.DisplayText = "Disconnected";
            valueListItem23.DataValue = 2;
            valueListItem23.DisplayText = "Synchronizing";
            valueListItem24.DataValue = 3;
            valueListItem24.DisplayText = "Pending Failover";
            valueListItem25.DataValue = 4;
            valueListItem25.DisplayText = "Synchronized";
            valueList2.ValueListItems.AddRange(new Infragistics.Win.ValueListItem[] {
            valueListItem21,
            valueListItem22,
            valueListItem23,
            valueListItem24,
            valueListItem25});
            valueList3.Key = "witnessConnection";
            valueList3.PreferredDropDownSize = new System.Drawing.Size(0, 0);
            valueListItem26.DataValue = 0;
            valueListItem26.DisplayText = "No Witness";
            valueListItem27.DataValue = 1;
            valueListItem27.DisplayText = "Connected";
            valueListItem28.DataValue = 2;
            valueListItem28.DisplayText = "Disconnected";
            valueList3.ValueListItems.AddRange(new Infragistics.Win.ValueListItem[] {
            valueListItem26,
            valueListItem27,
            valueListItem28});
            this.historyGrid.DisplayLayout.ValueLists.AddRange(new Infragistics.Win.ValueList[] {
            valueList1,
            valueList2,
            valueList3});
            this.historyGrid.DisplayLayout.ViewStyle = Infragistics.Win.UltraWinGrid.ViewStyle.SingleBand;
            this.historyGrid.DisplayLayout.ViewStyleBand = Infragistics.Win.UltraWinGrid.ViewStyleBand.OutlookGroupBy;
            this.historyGrid.Dock = System.Windows.Forms.DockStyle.Fill;
            this.historyGrid.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.historyGrid.Location = new System.Drawing.Point(0, 0);
            this.historyGrid.Name = "historyGrid";
            this.historyGrid.Size = new System.Drawing.Size(685, 217);
            this.historyGrid.TabIndex = 10;
            // 
            // panel1
            // 
            this.panel1.BackColor = System.Drawing.Color.Transparent;
            this.panel1.Controls.Add(this.panel3);
            this.panel1.Controls.Add(this.lblFilterLisyBy);
            this.panel1.Controls.Add(this.mirrorMonitoringHistoryPeriodComboBox);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Top;
            this.panel1.Location = new System.Drawing.Point(0, 0);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(685, 39);
            this.panel1.TabIndex = 12;
            // 
            // panel3
            // 
            this.panel3.Controls.Add(this.historyPartnerViewRadioButton);
            this.panel3.Controls.Add(this.historyLocalViewRadioButton);
            this.panel3.Dock = System.Windows.Forms.DockStyle.Right;
            this.panel3.Location = new System.Drawing.Point(359, 0);
            this.panel3.Name = "panel3";
            this.panel3.Size = new System.Drawing.Size(326, 39);
            this.panel3.TabIndex = 16;
            // 
            // historyPartnerViewRadioButton
            // 
            this.historyPartnerViewRadioButton.AutoSize = true;
            this.historyPartnerViewRadioButton.BackColor = System.Drawing.Color.Transparent;
            this.historyPartnerViewRadioButton.Location = new System.Drawing.Point(174, 11);
            this.historyPartnerViewRadioButton.Name = "historyPartnerViewRadioButton";
            this.historyPartnerViewRadioButton.Size = new System.Drawing.Size(120, 17);
            this.historyPartnerViewRadioButton.TabIndex = 15;
            this.historyPartnerViewRadioButton.Text = "View Partner History";
            this.historyPartnerViewRadioButton.UseVisualStyleBackColor = false;
            this.historyPartnerViewRadioButton.CheckedChanged += new System.EventHandler(this.historyPartnerViewRadioButton_CheckedChanged);
            // 
            // historyLocalViewRadioButton
            // 
            this.historyLocalViewRadioButton.AutoSize = true;
            this.historyLocalViewRadioButton.BackColor = System.Drawing.Color.Transparent;
            this.historyLocalViewRadioButton.Checked = true;
            this.historyLocalViewRadioButton.Location = new System.Drawing.Point(22, 11);
            this.historyLocalViewRadioButton.Name = "historyLocalViewRadioButton";
            this.historyLocalViewRadioButton.Size = new System.Drawing.Size(146, 17);
            this.historyLocalViewRadioButton.TabIndex = 14;
            this.historyLocalViewRadioButton.TabStop = true;
            this.historyLocalViewRadioButton.Text = "View History (local server)";
            this.historyLocalViewRadioButton.UseVisualStyleBackColor = false;
            this.historyLocalViewRadioButton.CheckedChanged += new System.EventHandler(this.historyLocalViewRadioButton_CheckedChanged);
            // 
            // lblFilterLisyBy
            // 
            this.lblFilterLisyBy.AutoSize = true;
            this.lblFilterLisyBy.Location = new System.Drawing.Point(5, 11);
            this.lblFilterLisyBy.Name = "lblFilterLisyBy";
            this.lblFilterLisyBy.Size = new System.Drawing.Size(61, 13);
            this.lblFilterLisyBy.TabIndex = 15;
            this.lblFilterLisyBy.Text = "Filter list by:";
            // 
            // mirrorMonitoringHistoryPeriodComboBox
            // 
            this.mirrorMonitoringHistoryPeriodComboBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.mirrorMonitoringHistoryPeriodComboBox.DropDownStyle = Infragistics.Win.DropDownStyle.DropDownList;
            valueListItem1.DataValue = 0;
            valueListItem1.DisplayText = "Last two hours";
            valueListItem2.DataValue = 1;
            valueListItem2.DisplayText = "Last four hours";
            valueListItem3.DataValue = 2;
            valueListItem3.DisplayText = "Last eight hours";
            valueListItem4.DataValue = 3;
            valueListItem4.DisplayText = "Last day";
            valueListItem5.DataValue = 4;
            valueListItem5.DisplayText = "Last two days";
            valueListItem6.DataValue = 5;
            valueListItem6.DisplayText = "Last 100 records";
            valueListItem7.DataValue = 6;
            valueListItem7.DisplayText = "Last 500 records";
            valueListItem8.DataValue = 7;
            valueListItem8.DisplayText = "Last 1000 records";
            this.mirrorMonitoringHistoryPeriodComboBox.Items.AddRange(new Infragistics.Win.ValueListItem[] {
            valueListItem1,
            valueListItem2,
            valueListItem3,
            valueListItem4,
            valueListItem5,
            valueListItem6,
            valueListItem7,
            valueListItem8});
            this.mirrorMonitoringHistoryPeriodComboBox.LimitToList = true;
            this.mirrorMonitoringHistoryPeriodComboBox.Location = new System.Drawing.Point(72, 7);
            this.mirrorMonitoringHistoryPeriodComboBox.Name = "mirrorMonitoringHistoryPeriodComboBox";
            this.mirrorMonitoringHistoryPeriodComboBox.Size = new System.Drawing.Size(286, 21);
            this.mirrorMonitoringHistoryPeriodComboBox.SortStyle = Infragistics.Win.ValueListSortStyle.AscendingByValue;
            this.mirrorMonitoringHistoryPeriodComboBox.TabIndex = 14;
            this.mirrorMonitoringHistoryPeriodComboBox.ValueChanged += new System.EventHandler(this.mirrorMonitoringHistoryPeriodComboBox_ValueChanged);
            // 
            // actionProgressControl
            // 
            this.actionProgressControl.Active = false;
            this.actionProgressControl.BackColor = System.Drawing.Color.White;
            this.actionProgressControl.Color = System.Drawing.Color.DarkGray;
            this.actionProgressControl.Dock = System.Windows.Forms.DockStyle.Fill;
            this.actionProgressControl.InnerCircleRadius = 5;
            this.actionProgressControl.Location = new System.Drawing.Point(0, 1);
            this.actionProgressControl.Name = "actionProgressControl";
            this.actionProgressControl.NumberSpoke = 12;
            this.actionProgressControl.OuterCircleRadius = 11;
            this.actionProgressControl.RotationSpeed = 100;
            this.actionProgressControl.Size = new System.Drawing.Size(689, 200);
            this.actionProgressControl.SpokeThickness = 2;
            this.actionProgressControl.StylePreset = MRG.Controls.UI.LoadingCircle.StylePresets.MacOSX;
            this.actionProgressControl.TabIndex = 0;
            this.actionProgressControl.Visible = false;
            // 
            // propertiesTabPage
            // 
            this.propertiesTabPage.Controls.Add(this.transactionLogsGrid);
            this.propertiesTabPage.Location = new System.Drawing.Point(-10000, -10000);
            this.propertiesTabPage.Name = "propertiesTabPage";
            this.propertiesTabPage.Size = new System.Drawing.Size(685, 270);
            // 
            // transactionLogsGrid
            // 
            appearance81.BackColor = System.Drawing.SystemColors.Window;
            appearance81.BorderColor = System.Drawing.SystemColors.InactiveCaption;
            this.transactionLogsGrid.DisplayLayout.Appearance = appearance81;
            this.transactionLogsGrid.DisplayLayout.AutoFitStyle = Infragistics.Win.UltraWinGrid.AutoFitStyle.ExtendLastColumn;
            this.transactionLogsGrid.DisplayLayout.BorderStyle = Infragistics.Win.UIElementBorderStyle.None;
            this.transactionLogsGrid.DisplayLayout.CaptionVisible = Infragistics.Win.DefaultableBoolean.False;
            appearance94.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(227)))), ((int)(((byte)(239)))), ((int)(((byte)(255)))));
            appearance94.BackColor2 = System.Drawing.SystemColors.ControlDark;
            appearance94.BackGradientStyle = Infragistics.Win.GradientStyle.None;
            appearance94.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(101)))), ((int)(((byte)(147)))), ((int)(((byte)(207)))));
            this.transactionLogsGrid.DisplayLayout.GroupByBox.Appearance = appearance94;
            appearance95.ForeColor = System.Drawing.SystemColors.GrayText;
            this.transactionLogsGrid.DisplayLayout.GroupByBox.BandLabelAppearance = appearance95;
            this.transactionLogsGrid.DisplayLayout.GroupByBox.BorderStyle = Infragistics.Win.UIElementBorderStyle.Solid;
            this.transactionLogsGrid.DisplayLayout.GroupByBox.Hidden = true;
            appearance96.BackColor = System.Drawing.SystemColors.ControlLightLight;
            appearance96.BackColor2 = System.Drawing.SystemColors.Control;
            appearance96.BackGradientStyle = Infragistics.Win.GradientStyle.Horizontal;
            appearance96.ForeColor = System.Drawing.SystemColors.GrayText;
            this.transactionLogsGrid.DisplayLayout.GroupByBox.PromptAppearance = appearance96;
            this.transactionLogsGrid.DisplayLayout.MaxColScrollRegions = 1;
            this.transactionLogsGrid.DisplayLayout.MaxRowScrollRegions = 1;
            this.transactionLogsGrid.DisplayLayout.Override.AllowAddNew = Infragistics.Win.UltraWinGrid.AllowAddNew.No;
            this.transactionLogsGrid.DisplayLayout.Override.AllowDelete = Infragistics.Win.DefaultableBoolean.False;
            this.transactionLogsGrid.DisplayLayout.Override.AllowUpdate = Infragistics.Win.DefaultableBoolean.False;
            this.transactionLogsGrid.DisplayLayout.Override.BorderStyleCell = Infragistics.Win.UIElementBorderStyle.Solid;
            this.transactionLogsGrid.DisplayLayout.Override.BorderStyleRow = Infragistics.Win.UIElementBorderStyle.Solid;
            appearance97.BackColor = System.Drawing.SystemColors.Window;
            this.transactionLogsGrid.DisplayLayout.Override.CardAreaAppearance = appearance97;
            appearance98.BorderColor = System.Drawing.Color.Silver;
            appearance98.TextTrimming = Infragistics.Win.TextTrimming.EllipsisCharacter;
            this.transactionLogsGrid.DisplayLayout.Override.CellAppearance = appearance98;
            this.transactionLogsGrid.DisplayLayout.Override.CellClickAction = Infragistics.Win.UltraWinGrid.CellClickAction.RowSelect;
            this.transactionLogsGrid.DisplayLayout.Override.CellPadding = 0;
            this.transactionLogsGrid.DisplayLayout.Override.FixedHeaderIndicator = Infragistics.Win.UltraWinGrid.FixedHeaderIndicator.None;
            this.transactionLogsGrid.DisplayLayout.Override.GroupByColumnsHidden = Infragistics.Win.DefaultableBoolean.False;
            appearance99.BackColor = System.Drawing.SystemColors.Control;
            appearance99.BackColor2 = System.Drawing.SystemColors.ControlDark;
            appearance99.BackGradientAlignment = Infragistics.Win.GradientAlignment.Element;
            appearance99.BackGradientStyle = Infragistics.Win.GradientStyle.Horizontal;
            appearance99.BorderColor = System.Drawing.SystemColors.Window;
            this.transactionLogsGrid.DisplayLayout.Override.GroupByRowAppearance = appearance99;
            this.transactionLogsGrid.DisplayLayout.Override.GroupByRowInitialExpansionState = Infragistics.Win.UltraWinGrid.GroupByRowInitialExpansionState.Expanded;
            appearance110.TextHAlignAsString = "Left";
            this.transactionLogsGrid.DisplayLayout.Override.HeaderAppearance = appearance110;
            this.transactionLogsGrid.DisplayLayout.Override.HeaderClickAction = Infragistics.Win.UltraWinGrid.HeaderClickAction.SortMulti;
            appearance111.BackColor = System.Drawing.SystemColors.Window;
            appearance111.BorderColor = System.Drawing.Color.Silver;
            this.transactionLogsGrid.DisplayLayout.Override.RowAppearance = appearance111;
            this.transactionLogsGrid.DisplayLayout.Override.RowSelectors = Infragistics.Win.DefaultableBoolean.False;
            this.transactionLogsGrid.DisplayLayout.Override.SelectTypeCell = Infragistics.Win.UltraWinGrid.SelectType.None;
            this.transactionLogsGrid.DisplayLayout.Override.SelectTypeCol = Infragistics.Win.UltraWinGrid.SelectType.None;
            this.transactionLogsGrid.DisplayLayout.Override.SelectTypeGroupByRow = Infragistics.Win.UltraWinGrid.SelectType.None;
            this.transactionLogsGrid.DisplayLayout.Override.SelectTypeRow = Infragistics.Win.UltraWinGrid.SelectType.Extended;
            appearance112.BackColor = System.Drawing.SystemColors.ControlLight;
            this.transactionLogsGrid.DisplayLayout.Override.TemplateAddRowAppearance = appearance112;
            this.transactionLogsGrid.DisplayLayout.ScrollBounds = Infragistics.Win.UltraWinGrid.ScrollBounds.ScrollToFill;
            this.transactionLogsGrid.DisplayLayout.ScrollStyle = Infragistics.Win.UltraWinGrid.ScrollStyle.Immediate;
            this.transactionLogsGrid.DisplayLayout.UseFixedHeaders = true;
            valueList4.Key = "autoGrowthValueList";
            valueList4.PreferredDropDownSize = new System.Drawing.Size(0, 0);
            this.transactionLogsGrid.DisplayLayout.ValueLists.AddRange(new Infragistics.Win.ValueList[] {
            valueList4});
            this.transactionLogsGrid.DisplayLayout.ViewStyleBand = Infragistics.Win.UltraWinGrid.ViewStyleBand.OutlookGroupBy;
            this.transactionLogsGrid.Dock = System.Windows.Forms.DockStyle.Fill;
            this.transactionLogsGrid.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.transactionLogsGrid.Location = new System.Drawing.Point(0, 0);
            this.transactionLogsGrid.Name = "transactionLogsGrid";
            this.transactionLogsGrid.Size = new System.Drawing.Size(685, 270);
            this.transactionLogsGrid.TabIndex = 9;
            // 
            // DatabasesMirroring_Fill_Panel
            // 
            this.DatabasesMirroring_Fill_Panel.Controls.Add(this.refreshDatabasesButton);
            this.DatabasesMirroring_Fill_Panel.Controls.Add(this.contentContainerPanel);
            this.DatabasesMirroring_Fill_Panel.Controls.Add(this.mirroredDatabasesComboBox);
            this.DatabasesMirroring_Fill_Panel.Cursor = System.Windows.Forms.Cursors.Default;
            this.DatabasesMirroring_Fill_Panel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.DatabasesMirroring_Fill_Panel.Location = new System.Drawing.Point(0, 0);
            this.DatabasesMirroring_Fill_Panel.Name = "DatabasesMirroring_Fill_Panel";
            this.DatabasesMirroring_Fill_Panel.Size = new System.Drawing.Size(689, 517);
            this.DatabasesMirroring_Fill_Panel.TabIndex = 10;
            // 
            // refreshDatabasesButton
            //
            if (Settings.Default.ColorScheme == "Dark")
            {
                this.refreshDatabasesButton.UseAppStyling = false;
                this.refreshDatabasesButton.UseOsThemes = DefaultableBoolean.False;
                this.refreshDatabasesButton.ButtonStyle = UIElementButtonStyle.FlatBorderless;
            }
            else
            {
                this.refreshDatabasesButton.UseAppStyling = true;
            }
            this.refreshDatabasesButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            if (!refreshDatabasesButton.Enabled)
                appearance1.Image = Helpers.ImageUtils.ChangeOpacity(global::Idera.SQLdm.DesktopClient.Properties.Resources.ToolbarRefresh, 0.50F);
            else
                appearance1.Image = global::Idera.SQLdm.DesktopClient.Properties.Resources.ToolbarRefresh;
            appearance1.ImageHAlign = Infragistics.Win.HAlign.Center;
            this.refreshDatabasesButton.Appearance = appearance1;
            this.refreshDatabasesButton.Location = new System.Drawing.Point(663, 3);
            this.refreshDatabasesButton.Name = "refreshDatabasesButton";
            this.refreshDatabasesButton.ShowFocusRect = false;
            this.refreshDatabasesButton.ShowOutline = false;
            this.refreshDatabasesButton.Size = new System.Drawing.Size(23, 23);
            this.refreshDatabasesButton.TabIndex = 9;
            this.refreshDatabasesButton.Click += new System.EventHandler(this.refreshDatabasesButton_Click);
            this.refreshDatabasesButton.MouseEnterElement += new UIElementEventHandler(mouseEnter_refreshDatabasesButton);
            this.refreshDatabasesButton.MouseLeaveElement += new UIElementEventHandler(mouseLeave_refreshDatabasesButton);
            ThemeManager.CurrentThemeChanged += new EventHandler(OnCurrentThemeChanged);
            // 
            // contentContainerPanel
            // 
            this.contentContainerPanel.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.contentContainerPanel.Controls.Add(this.splitContainer1);
            this.contentContainerPanel.Location = new System.Drawing.Point(0, 28);
            this.contentContainerPanel.Name = "contentContainerPanel";
            this.contentContainerPanel.Size = new System.Drawing.Size(689, 489);
            this.contentContainerPanel.TabIndex = 8;
            // 
            // splitContainer1
            // 
            this.splitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer1.FixedPanel = System.Windows.Forms.FixedPanel.Panel2;
            this.splitContainer1.Location = new System.Drawing.Point(0, 0);
            this.splitContainer1.Name = "splitContainer1";
            this.splitContainer1.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // splitContainer1.Panel1
            // 
            this.splitContainer1.Panel1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(172)))), ((int)(((byte)(172)))), ((int)(((byte)(172)))));
            this.splitContainer1.Panel1.Controls.Add(this.actionProgressControl);
            this.splitContainer1.Panel1.Controls.Add(this.mirroredDatabasesGrid);
            this.splitContainer1.Panel1.Controls.Add(this.databasesGridStatusLabel);
            this.splitContainer1.Panel1.Padding = new System.Windows.Forms.Padding(0, 1, 0, 1);
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(172)))), ((int)(((byte)(172)))), ((int)(((byte)(172)))));
            this.splitContainer1.Panel2.Controls.Add(this.mirroringDetailsTabControl);
            this.splitContainer1.Panel2.Controls.Add(this.mirroringDetailsTabControlStatusLabel);
            this.splitContainer1.Panel2.Padding = new System.Windows.Forms.Padding(0, 1, 0, 0);
            this.splitContainer1.Size = new System.Drawing.Size(689, 489);
            this.splitContainer1.SplitterDistance = 202;
            this.splitContainer1.TabIndex = 0;
            // 
            // mirroredDatabasesGrid
            // 
            appearance12.BackColor = Color.White;
            appearance12.BorderColor = System.Drawing.SystemColors.InactiveCaption;
            this.mirroredDatabasesGrid.DisplayLayout.Appearance = appearance12;
            this.mirroredDatabasesGrid.DisplayLayout.AutoFitStyle = Infragistics.Win.UltraWinGrid.AutoFitStyle.ResizeAllColumns;
            ultraGridColumn15.AutoCompleteMode = Infragistics.Win.AutoCompleteMode.Append;
            appearance13.ImageHAlign = Infragistics.Win.HAlign.Center;
            ultraGridColumn15.Header.Appearance = appearance13;
            ultraGridColumn15.Header.Fixed = true;
            ultraGridColumn15.Header.VisiblePosition = 0;
            ultraGridColumn15.Width = 120;
            ultraGridColumn16.AutoCompleteMode = Infragistics.Win.AutoCompleteMode.Append;
            ultraGridColumn16.Header.Fixed = true;
            ultraGridColumn16.Header.VisiblePosition = 1;
            ultraGridColumn16.Width = 127;
            ultraGridColumn17.AutoCompleteMode = Infragistics.Win.AutoCompleteMode.Append;
            ultraGridColumn17.Header.VisiblePosition = 2;
            ultraGridColumn17.Width = 64;
            ultraGridColumn18.AutoCompleteMode = Infragistics.Win.AutoCompleteMode.Append;
            ultraGridColumn18.Header.VisiblePosition = 3;
            ultraGridColumn19.AutoCompleteMode = Infragistics.Win.AutoCompleteMode.Append;
            ultraGridColumn19.Header.VisiblePosition = 4;
            ultraGridColumn20.AutoCompleteMode = Infragistics.Win.AutoCompleteMode.Append;
            ultraGridColumn20.Format = "";
            ultraGridColumn20.Header.VisiblePosition = 5;
            ultraGridColumn21.AutoCompleteMode = Infragistics.Win.AutoCompleteMode.Append;
            ultraGridColumn21.Header.VisiblePosition = 6;
            ultraGridColumn22.Header.VisiblePosition = 7;
            ultraGridColumn22.Hidden = true;
            ultraGridBand2.Columns.AddRange(new object[] {
            ultraGridColumn15,
            ultraGridColumn16,
            ultraGridColumn17,
            ultraGridColumn18,
            ultraGridColumn19,
            ultraGridColumn20,
            ultraGridColumn21,
            ultraGridColumn22});
            this.mirroredDatabasesGrid.DisplayLayout.BandsSerializer.Add(ultraGridBand2);
            this.mirroredDatabasesGrid.DisplayLayout.BorderStyle = Infragistics.Win.UIElementBorderStyle.None;
            this.mirroredDatabasesGrid.DisplayLayout.CaptionVisible = Infragistics.Win.DefaultableBoolean.False;
            appearance14.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(235)))), ((int)(((byte)(235)))), ((int)(((byte)(235)))));
            appearance14.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(140)))), ((int)(((byte)(140)))), ((int)(((byte)(140)))));
            this.mirroredDatabasesGrid.DisplayLayout.GroupByBox.Appearance = appearance14;
            appearance15.ForeColor = System.Drawing.SystemColors.GrayText;
            this.mirroredDatabasesGrid.DisplayLayout.GroupByBox.BandLabelAppearance = appearance15;
            this.mirroredDatabasesGrid.DisplayLayout.GroupByBox.BorderStyle = Infragistics.Win.UIElementBorderStyle.Solid;
            this.mirroredDatabasesGrid.DisplayLayout.GroupByBox.Hidden = true;
            appearance16.BackColor = System.Drawing.SystemColors.ControlLightLight;
            appearance16.BackColor2 = System.Drawing.SystemColors.Control;
            appearance16.BackGradientStyle = Infragistics.Win.GradientStyle.Horizontal;
            appearance16.ForeColor = System.Drawing.SystemColors.GrayText;
            this.mirroredDatabasesGrid.DisplayLayout.GroupByBox.PromptAppearance = appearance16;
            this.mirroredDatabasesGrid.DisplayLayout.MaxColScrollRegions = 1;
            this.mirroredDatabasesGrid.DisplayLayout.MaxRowScrollRegions = 1;
            this.mirroredDatabasesGrid.DisplayLayout.Override.AllowAddNew = Infragistics.Win.UltraWinGrid.AllowAddNew.No;
            this.mirroredDatabasesGrid.DisplayLayout.Override.AllowDelete = Infragistics.Win.DefaultableBoolean.False;
            this.mirroredDatabasesGrid.DisplayLayout.Override.AllowUpdate = Infragistics.Win.DefaultableBoolean.False;
            this.mirroredDatabasesGrid.DisplayLayout.Override.BorderStyleCell = Infragistics.Win.UIElementBorderStyle.Solid;
            this.mirroredDatabasesGrid.DisplayLayout.Override.BorderStyleRow = Infragistics.Win.UIElementBorderStyle.Solid;
            appearance17.BackColor = System.Drawing.SystemColors.Window;
            this.mirroredDatabasesGrid.DisplayLayout.Override.CardAreaAppearance = appearance17;
            appearance18.BorderColor = System.Drawing.Color.Silver;
            appearance18.TextTrimming = Infragistics.Win.TextTrimming.EllipsisCharacter;
            this.mirroredDatabasesGrid.DisplayLayout.Override.CellAppearance = appearance18;
            this.mirroredDatabasesGrid.DisplayLayout.Override.CellClickAction = Infragistics.Win.UltraWinGrid.CellClickAction.RowSelect;
            this.mirroredDatabasesGrid.DisplayLayout.Override.CellPadding = 0;
            this.mirroredDatabasesGrid.DisplayLayout.Override.FixedHeaderIndicator = Infragistics.Win.UltraWinGrid.FixedHeaderIndicator.None;
            this.mirroredDatabasesGrid.DisplayLayout.Override.GroupByColumnsHidden = Infragistics.Win.DefaultableBoolean.False;
            appearance19.BackColor = System.Drawing.SystemColors.Control;
            appearance19.BackColor2 = System.Drawing.SystemColors.ControlDark;
            appearance19.BackGradientAlignment = Infragistics.Win.GradientAlignment.Element;
            appearance19.BackGradientStyle = Infragistics.Win.GradientStyle.Horizontal;
            appearance19.BorderColor = System.Drawing.SystemColors.Window;
            this.mirroredDatabasesGrid.DisplayLayout.Override.GroupByRowAppearance = appearance19;
            this.mirroredDatabasesGrid.DisplayLayout.Override.GroupByRowInitialExpansionState = Infragistics.Win.UltraWinGrid.GroupByRowInitialExpansionState.Expanded;
            appearance20.TextHAlignAsString = "Left";
            this.mirroredDatabasesGrid.DisplayLayout.Override.HeaderAppearance = appearance20;
            this.mirroredDatabasesGrid.DisplayLayout.Override.HeaderClickAction = Infragistics.Win.UltraWinGrid.HeaderClickAction.SortMulti;
            appearance28.BackColor = System.Drawing.SystemColors.Window;
            appearance28.BorderColor = System.Drawing.Color.Silver;
            this.mirroredDatabasesGrid.DisplayLayout.Override.RowAppearance = appearance28;
            this.mirroredDatabasesGrid.DisplayLayout.Override.RowSelectors = Infragistics.Win.DefaultableBoolean.False;
            this.mirroredDatabasesGrid.DisplayLayout.Override.SelectTypeCell = Infragistics.Win.UltraWinGrid.SelectType.None;
            this.mirroredDatabasesGrid.DisplayLayout.Override.SelectTypeCol = Infragistics.Win.UltraWinGrid.SelectType.None;
            this.mirroredDatabasesGrid.DisplayLayout.Override.SelectTypeGroupByRow = Infragistics.Win.UltraWinGrid.SelectType.None;
            this.mirroredDatabasesGrid.DisplayLayout.Override.SelectTypeRow = Infragistics.Win.UltraWinGrid.SelectType.Single;
            appearance33.BackColor = System.Drawing.SystemColors.ControlLight;
            this.mirroredDatabasesGrid.DisplayLayout.Override.TemplateAddRowAppearance = appearance33;
            this.mirroredDatabasesGrid.DisplayLayout.ScrollBounds = Infragistics.Win.UltraWinGrid.ScrollBounds.ScrollToFill;
            this.mirroredDatabasesGrid.DisplayLayout.ScrollStyle = Infragistics.Win.UltraWinGrid.ScrollStyle.Immediate;
            this.mirroredDatabasesGrid.DisplayLayout.UseFixedHeaders = true;
            valueList5.Key = "mirrorOperationalStatus";
            valueList5.PreferredDropDownSize = new System.Drawing.Size(0, 0);
            valueListItem29.DataValue = -1;
            valueListItem29.DisplayText = " Not set";
            valueListItem30.DataValue = 0;
            valueListItem30.DisplayText = "Failed Over";
            valueListItem31.DataValue = 1;
            valueListItem31.DisplayText = "Normal";
            valueList5.ValueListItems.AddRange(new Infragistics.Win.ValueListItem[] {
            valueListItem29,
            valueListItem30,
            valueListItem31});
            valueList6.Key = "mirroringState";
            valueList6.PreferredDropDownSize = new System.Drawing.Size(0, 0);
            valueListItem32.DataValue = 0;
            valueListItem32.DisplayText = "Suspended";
            valueListItem33.DataValue = 1;
            valueListItem33.DisplayText = "Disconnected";
            valueListItem34.DataValue = 2;
            valueListItem34.DisplayText = "Synchronizing";
            valueListItem35.DataValue = 3;
            valueListItem35.DisplayText = "Pending Failover";
            valueListItem36.DataValue = 4;
            valueListItem36.DisplayText = "Synchronized";
            valueList6.ValueListItems.AddRange(new Infragistics.Win.ValueListItem[] {
            valueListItem32,
            valueListItem33,
            valueListItem34,
            valueListItem35,
            valueListItem36});
            valueList7.DisplayStyle = Infragistics.Win.ValueListDisplayStyle.DisplayText;
            valueList7.Key = "witnessConnectionState";
            valueList7.PreferredDropDownSize = new System.Drawing.Size(0, 0);
            valueListItem37.DataValue = 0;
            valueListItem37.DisplayText = "No Witness";
            valueListItem38.DataValue = 1;
            valueListItem38.DisplayText = "Connected";
            valueListItem39.DataValue = 2;
            valueListItem39.DisplayText = "Disconnected";
            valueList7.ValueListItems.AddRange(new Infragistics.Win.ValueListItem[] {
            valueListItem37,
            valueListItem38,
            valueListItem39});
            this.mirroredDatabasesGrid.DisplayLayout.ValueLists.AddRange(new Infragistics.Win.ValueList[] {
            valueList5,
            valueList6,
            valueList7});
            this.mirroredDatabasesGrid.DisplayLayout.ViewStyle = Infragistics.Win.UltraWinGrid.ViewStyle.SingleBand;
            this.mirroredDatabasesGrid.DisplayLayout.ViewStyleBand = Infragistics.Win.UltraWinGrid.ViewStyleBand.OutlookGroupBy;
            this.mirroredDatabasesGrid.Dock = System.Windows.Forms.DockStyle.Fill;
            this.mirroredDatabasesGrid.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.mirroredDatabasesGrid.Location = new System.Drawing.Point(0, 1);
            this.mirroredDatabasesGrid.Name = "mirroredDatabasesGrid";
            this.mirroredDatabasesGrid.Size = new System.Drawing.Size(689, 200);
            this.mirroredDatabasesGrid.TabIndex = 7;
            this.mirroredDatabasesGrid.InitializeLayout += new Infragistics.Win.UltraWinGrid.InitializeLayoutEventHandler(this.mirroredDatabasesGrid_InitializeLayout);
            this.mirroredDatabasesGrid.AfterSelectChange += new Infragistics.Win.UltraWinGrid.AfterSelectChangeEventHandler(this.mirroredDatabasesGrid_AfterSelectChange);
            this.mirroredDatabasesGrid.MouseDown += new System.Windows.Forms.MouseEventHandler(this.mirroredDatabasesGrid_MouseDown);
            // 
            // databasesGridStatusLabel
            // 
            this.databasesGridStatusLabel.BackColor = System.Drawing.Color.White;
            this.databasesGridStatusLabel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.databasesGridStatusLabel.Location = new System.Drawing.Point(0, 1);
            this.databasesGridStatusLabel.Name = "databasesGridStatusLabel";
            this.databasesGridStatusLabel.Padding = new System.Windows.Forms.Padding(0, 5, 0, 0);
            this.databasesGridStatusLabel.Size = new System.Drawing.Size(689, 200);
            this.databasesGridStatusLabel.TabIndex = 6;
            this.databasesGridStatusLabel.Text = "< Status Message >";
            this.databasesGridStatusLabel.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            // 
            // mirroringDetailsTabControl
            // 
            this.mirroringDetailsTabControl.Controls.Add(this.ultraTabSharedControlsPage1);
            this.mirroringDetailsTabControl.Controls.Add(this.statusTabPage);
            this.mirroringDetailsTabControl.Controls.Add(this.historyTabPage);
            this.mirroringDetailsTabControl.Dock = System.Windows.Forms.DockStyle.Fill;
            this.mirroringDetailsTabControl.Location = new System.Drawing.Point(0, 1);
            this.mirroringDetailsTabControl.Name = "mirroringDetailsTabControl";
            this.mirroringDetailsTabControl.SharedControlsPage = this.ultraTabSharedControlsPage1;
            this.mirroringDetailsTabControl.Size = new System.Drawing.Size(689, 282);
            this.mirroringDetailsTabControl.TabIndex = 9;
            ultraTab1.TabPage = this.statusTabPage;
            ultraTab1.Text = "Status";
            ultraTab2.TabPage = this.historyTabPage;
            ultraTab2.Text = "History";
            this.mirroringDetailsTabControl.Tabs.AddRange(new Infragistics.Win.UltraWinTabControl.UltraTab[] {
            ultraTab1,
            ultraTab2});
            this.mirroringDetailsTabControl.SelectedTabChanged += new Infragistics.Win.UltraWinTabControl.SelectedTabChangedEventHandler(this.tabControl_SelectedTabChanged);
            // 
            // ultraTabSharedControlsPage1
            // 
            this.ultraTabSharedControlsPage1.Location = new System.Drawing.Point(-10000, -10000);
            this.ultraTabSharedControlsPage1.Name = "ultraTabSharedControlsPage1";
            this.ultraTabSharedControlsPage1.Size = new System.Drawing.Size(685, 256);
            // 
            // mirroringDetailsTabControlStatusLabel
            // 
            this.mirroringDetailsTabControlStatusLabel.BackColor = System.Drawing.Color.White;
            this.mirroringDetailsTabControlStatusLabel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.mirroringDetailsTabControlStatusLabel.Location = new System.Drawing.Point(0, 1);
            this.mirroringDetailsTabControlStatusLabel.Name = "mirroringDetailsTabControlStatusLabel";
            this.mirroringDetailsTabControlStatusLabel.Padding = new System.Windows.Forms.Padding(0, 5, 0, 0);
            this.mirroringDetailsTabControlStatusLabel.Size = new System.Drawing.Size(689, 282);
            this.mirroringDetailsTabControlStatusLabel.TabIndex = 8;
            this.mirroringDetailsTabControlStatusLabel.Text = "< Status Message >";
            this.mirroringDetailsTabControlStatusLabel.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            // 
            // mirroredDatabasesComboBox
            // 
            this.mirroredDatabasesComboBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.mirroredDatabasesComboBox.DropDownStyle = Infragistics.Win.DropDownStyle.DropDownList;
            this.mirroredDatabasesComboBox.Location = new System.Drawing.Point(3, 4);
            this.mirroredDatabasesComboBox.Name = "mirroredDatabasesComboBox";
            this.mirroredDatabasesComboBox.Size = new System.Drawing.Size(657, 21);
            this.mirroredDatabasesComboBox.SortStyle = Infragistics.Win.ValueListSortStyle.AscendingByValue;
            this.mirroredDatabasesComboBox.TabIndex = 7;
            this.mirroredDatabasesComboBox.SelectionChanged += new System.EventHandler(this.mirroredDatabasesComboBox_SelectionChanged);
            // 
            // disksTabPage
            // 
            this.disksTabPage.Controls.Add(this.diskUsagePanel);
            this.disksTabPage.Location = new System.Drawing.Point(-10000, -10000);
            this.disksTabPage.Name = "disksTabPage";
            this.disksTabPage.Padding = new System.Windows.Forms.Padding(3);
            this.disksTabPage.Size = new System.Drawing.Size(685, 270);
            // 
            // diskUsagePanel
            // 
            this.diskUsagePanel.Controls.Add(this.currentDiskUsageChartContainerPanel);
            this.diskUsagePanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.diskUsagePanel.Location = new System.Drawing.Point(3, 3);
            this.diskUsagePanel.Name = "diskUsagePanel";
            this.diskUsagePanel.Size = new System.Drawing.Size(679, 264);
            this.diskUsagePanel.TabIndex = 4;
            // 
            // currentDiskUsageChartContainerPanel
            // 
            this.currentDiskUsageChartContainerPanel.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(173)))), ((int)(((byte)(209)))), ((int)(((byte)(255)))));
            this.currentDiskUsageChartContainerPanel.Controls.Add(this.diskUsageChart);
            this.currentDiskUsageChartContainerPanel.Controls.Add(this.diskUsageChartStatusLabel);
            this.currentDiskUsageChartContainerPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.currentDiskUsageChartContainerPanel.Location = new System.Drawing.Point(0, 0);
            this.currentDiskUsageChartContainerPanel.Name = "currentDiskUsageChartContainerPanel";
            this.currentDiskUsageChartContainerPanel.Padding = new System.Windows.Forms.Padding(1, 0, 1, 1);
            this.currentDiskUsageChartContainerPanel.Size = new System.Drawing.Size(679, 264);
            this.currentDiskUsageChartContainerPanel.TabIndex = 12;
            // 
            // diskUsageChart
            // 
            this.diskUsageChart.AllSeries.BarShape = ChartFX.WinForms.BarShape.Cylinder;
            this.diskUsageChart.AllSeries.Gallery = ChartFX.WinForms.Gallery.Gantt;
            this.diskUsageChart.AllSeries.Stacked = ChartFX.WinForms.Stacked.Normal;
            gradientBackground1.ColorFrom = System.Drawing.Color.FromArgb(((int)(((byte)(210)))), ((int)(((byte)(210)))), ((int)(((byte)(210)))));
            this.diskUsageChart.Background = gradientBackground1;
            this.diskUsageChart.Border = new ChartFX.WinForms.Adornments.SimpleBorder(ChartFX.WinForms.Adornments.SimpleBorderType.None, System.Drawing.Color.FromArgb(((int)(((byte)(109)))), ((int)(((byte)(125)))), ((int)(((byte)(138))))));
            this.diskUsageChart.ContextMenus = false;
            this.diskUsageChart.Dock = System.Windows.Forms.DockStyle.Fill;
            this.diskUsageChart.Location = new System.Drawing.Point(1, 0);
            this.diskUsageChart.Name = "diskUsageChart";
            this.diskUsageChart.Palette = "Schemes.Classic";
            this.diskUsageChart.PlotAreaColor = System.Drawing.Color.White;
            this.diskUsageChart.RandomData.Series = 5;
            seriesAttributes4.Text = "SQL Data Used";
            seriesAttributes5.Text = "SQL Data Free";
            this.diskUsageChart.Series.AddRange(new ChartFX.WinForms.SeriesAttributes[] {
            seriesAttributes1,
            seriesAttributes2,
            seriesAttributes3,
            seriesAttributes4,
            seriesAttributes5});
            this.diskUsageChart.Size = new System.Drawing.Size(677, 263);
            this.diskUsageChart.TabIndex = 13;
            // 
            // diskUsageChartStatusLabel
            // 
            this.diskUsageChartStatusLabel.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(227)))), ((int)(((byte)(239)))), ((int)(((byte)(255)))));
            this.diskUsageChartStatusLabel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.diskUsageChartStatusLabel.Location = new System.Drawing.Point(1, 0);
            this.diskUsageChartStatusLabel.Name = "diskUsageChartStatusLabel";
            this.diskUsageChartStatusLabel.Size = new System.Drawing.Size(677, 263);
            this.diskUsageChartStatusLabel.TabIndex = 12;
            this.diskUsageChartStatusLabel.Text = Idera.SQLdm.Common.Constants.LOADING;
            this.diskUsageChartStatusLabel.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // refreshAvailableDatabasesBackgroundWorker
            // 
            this.refreshAvailableDatabasesBackgroundWorker.WorkerSupportsCancellation = true;
            this.refreshAvailableDatabasesBackgroundWorker.DoWork += new System.ComponentModel.DoWorkEventHandler(this.refreshAvailableDatabasesBackgroundWorker_DoWork);
            // 
            // refreshMirroringDetailsBackgroundWorker
            // 
            this.refreshMirroringDetailsBackgroundWorker.WorkerSupportsCancellation = true;
            // 
            // refreshMirroringHistoryBackgroundWorker
            // 
            this.refreshMirroringHistoryBackgroundWorker.WorkerSupportsCancellation = true;
            // 
            // toolbarsManager
            // 
            this.toolbarsManager.DesignerFlags = 1;
            this.toolbarsManager.ShowFullMenusDelay = 500;
            popupMenuTool1.SharedPropsInternal.Caption = "detailsGridContextMenu";
            buttonTool30.InstanceProps.IsFirstInGroup = true;
            buttonTool22.InstanceProps.IsFirstInGroup = true;
            buttonTool24.InstanceProps.IsFirstInGroup = true;
            popupMenuTool1.Tools.AddRange(new Infragistics.Win.UltraWinToolbars.ToolBase[] {
            buttonTool8,
            buttonTool10,
            buttonTool30,
            buttonTool31,
            buttonTool32,
            buttonTool22,
            buttonTool23,
            buttonTool24,
            buttonTool25});
            appearance21.Image = global::Idera.SQLdm.DesktopClient.Properties.Resources.SortAscending;
            buttonTool1.SharedPropsInternal.AppearancesSmall.Appearance = appearance21;
            buttonTool1.SharedPropsInternal.Caption = "Sort Ascending";
            appearance22.Image = global::Idera.SQLdm.DesktopClient.Properties.Resources.SortDescending;
            buttonTool2.SharedPropsInternal.AppearancesSmall.Appearance = appearance22;
            buttonTool2.SharedPropsInternal.Caption = "Sort Descending";
            popupMenuTool2.SharedPropsInternal.Caption = "columnsContextMenu";
            buttonTool20.InstanceProps.IsFirstInGroup = true;
            popupMenuTool2.Tools.AddRange(new Infragistics.Win.UltraWinToolbars.ToolBase[] {
            buttonTool5,
            buttonTool6,
            stateButtonTool3,
            buttonTool33,
            buttonTool20,
            buttonTool21});
            appearance43.Image = global::Idera.SQLdm.DesktopClient.Properties.Resources.DatabaseMirrorFailover16x16;
            buttonTool7.SharedPropsInternal.AppearancesSmall.Appearance = appearance43;
            buttonTool7.SharedPropsInternal.Caption = "Fail over to partner";
            popupMenuTool3.SharedPropsInternal.Caption = "gridContextMenu";
            buttonTool27.InstanceProps.IsFirstInGroup = true;
            popupMenuTool3.Tools.AddRange(new Infragistics.Win.UltraWinToolbars.ToolBase[] {
            buttonTool4,
            buttonTool26,
            buttonTool27,
            buttonTool28});
            buttonTool3.SharedPropsInternal.Caption = "Collapse All Groups";
            appearance30.Image = global::Idera.SQLdm.DesktopClient.Properties.Resources.QATPause;
            buttonTool9.SharedPropsInternal.AppearancesSmall.Appearance = appearance30;
            buttonTool9.SharedPropsInternal.Caption = "Suspend\\resume session";
            appearance23.Image = global::Idera.SQLdm.DesktopClient.Properties.Resources.Print16x16;
            buttonTool11.SharedPropsInternal.AppearancesSmall.Appearance = appearance23;
            buttonTool11.SharedPropsInternal.Caption = "Print Grid";
            appearance24.Image = global::Idera.SQLdm.DesktopClient.Properties.Resources.Export16x16;
            buttonTool12.SharedPropsInternal.AppearancesSmall.Appearance = appearance24;
            buttonTool12.SharedPropsInternal.Caption = "Export Grid";
            buttonTool13.SharedPropsInternal.Caption = "Remove This Column";
            appearance26.Image = global::Idera.SQLdm.DesktopClient.Properties.Resources.ColumnChooser;
            buttonTool14.SharedPropsInternal.AppearancesSmall.Appearance = appearance26;
            buttonTool14.SharedPropsInternal.Caption = "Column Chooser";
            appearance27.Image = global::Idera.SQLdm.DesktopClient.Properties.Resources.GroupByBox;
            buttonTool15.SharedPropsInternal.AppearancesSmall.Appearance = appearance27;
            buttonTool15.SharedPropsInternal.Caption = "Group By Box";
            buttonTool17.SharedPropsInternal.Caption = "Expand All Groups";
            appearance29.Image = global::Idera.SQLdm.DesktopClient.Properties.Resources.GroupByThisColumn;
            stateButtonTool1.SharedPropsInternal.AppearancesSmall.Appearance = appearance29;
            stateButtonTool1.SharedPropsInternal.Caption = "Group By This Column";
            buttonTool16.SharedPropsInternal.Caption = "Mark session as \"normal\" operational status";
            buttonTool18.SharedPropsInternal.Caption = "Mark session as \"failed over\"";
            buttonTool29.SharedPropsInternal.Caption = "Clear preferred role for this session";
            this.toolbarsManager.Tools.AddRange(new Infragistics.Win.UltraWinToolbars.ToolBase[] {
            popupMenuTool1,
            buttonTool1,
            buttonTool2,
            popupMenuTool2,
            buttonTool7,
            popupMenuTool3,
            buttonTool3,
            buttonTool9,
            buttonTool11,
            buttonTool12,
            buttonTool13,
            buttonTool14,
            buttonTool15,
            buttonTool17,
            stateButtonTool1,
            buttonTool16,
            buttonTool18,
            buttonTool29});
            this.toolbarsManager.BeforeToolDropdown += new Infragistics.Win.UltraWinToolbars.BeforeToolDropdownEventHandler(this.toolbarsManager_BeforeToolDropdown);
            this.toolbarsManager.ToolClick += new Infragistics.Win.UltraWinToolbars.ToolClickEventHandler(this.toolbarsManager_ToolClick);
            // 
            // failOverBackgroundWorker
            // 
            this.failOverBackgroundWorker.WorkerSupportsCancellation = true;
            this.failOverBackgroundWorker.DoWork += new System.ComponentModel.DoWorkEventHandler(this.failOverBackgroundWorker_DoWork);
            this.failOverBackgroundWorker.RunWorkerCompleted += new System.ComponentModel.RunWorkerCompletedEventHandler(this.failOverBackgroundWorker_RunWorkerCompleted);
            // 
            // suspendResumeBackgroundWorker
            // 
            this.suspendResumeBackgroundWorker.WorkerSupportsCancellation = true;
            this.suspendResumeBackgroundWorker.DoWork += new System.ComponentModel.DoWorkEventHandler(this.suspendResumeBackgroundWorker_DoWork);
            this.suspendResumeBackgroundWorker.RunWorkerCompleted += new System.ComponentModel.RunWorkerCompletedEventHandler(this.suspendResumeBackgroundWorker_RunWorkerCompleted);
            // 
            // ultraPrintPreviewDialog
            // 
            this.ultraPrintPreviewDialog.Name = "ultraPrintPreviewDialog";
            // 
            // setOperationalStatusBackgroundWorker
            // 
            this.setOperationalStatusBackgroundWorker.DoWork += new System.ComponentModel.DoWorkEventHandler(this.setOperationalStatusBackgroundWorker_DoWork);
            this.setOperationalStatusBackgroundWorker.RunWorkerCompleted += new System.ComponentModel.RunWorkerCompletedEventHandler(this.setOperationalStatusBackgroundWorker_RunWorkerCompleted);
            // 
            // historicalSnapshotStatusLinkLabel
            // 
            this.historicalSnapshotStatusLinkLabel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.historicalSnapshotStatusLinkLabel.Location = new System.Drawing.Point(0, 0);
            this.historicalSnapshotStatusLinkLabel.Name = "historicalSnapshotStatusLinkLabel";
            this.historicalSnapshotStatusLinkLabel.Size = new System.Drawing.Size(689, 517);
            this.historicalSnapshotStatusLinkLabel.TabIndex = 33;
            this.historicalSnapshotStatusLinkLabel.TabStop = true;
            this.historicalSnapshotStatusLinkLabel.Text = "This view does not support historical mode. Click here to switch to real-time mod" +
    "e.";
            this.historicalSnapshotStatusLinkLabel.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.historicalSnapshotStatusLinkLabel.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.historicalSnapshotStatusLinkLabel_LinkClicked);
            // 
            // refreshPartnerDetailsBackgroundWorker
            // 
            this.refreshPartnerDetailsBackgroundWorker.WorkerSupportsCancellation = true;
            this.refreshPartnerDetailsBackgroundWorker.DoWork += new System.ComponentModel.DoWorkEventHandler(this.refreshPartnerDetailsBackgroundWorker_DoWork);
            this.refreshPartnerDetailsBackgroundWorker.RunWorkerCompleted += new System.ComponentModel.RunWorkerCompletedEventHandler(this.refreshPartnerDetailsBackgroundWorker_RunWorkerCompleted);
            // 
            // DatabasesMirroringView
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(235)))), ((int)(((byte)(235)))), ((int)(((byte)(235)))));
            this.Controls.Add(this.DatabasesMirroring_Fill_Panel);
            this.Controls.Add(this.historicalSnapshotStatusLinkLabel);
            this.Name = "DatabasesMirroringView";
            this.Size = new System.Drawing.Size(689, 517);
            this.Load += new System.EventHandler(this.DatabasesMirroringView_Load);
            this.statusTabPage.ResumeLayout(false);
            this.pnlMessage.ResumeLayout(false);
            this.pnlMessage.PerformLayout();
            this.pnlDetails.ResumeLayout(false);
            this.pnlDetails.PerformLayout();
            this.mirrorGroup.ResumeLayout(false);
            this.mirrorGroup.PerformLayout();
            this.principalGroup.ResumeLayout(false);
            this.principalGroup.PerformLayout();
            this.historyTabPage.ResumeLayout(false);
            this.panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.historyGrid)).EndInit();
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.panel3.ResumeLayout(false);
            this.panel3.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.mirrorMonitoringHistoryPeriodComboBox)).EndInit();
            this.propertiesTabPage.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.transactionLogsGrid)).EndInit();
            this.DatabasesMirroring_Fill_Panel.ResumeLayout(false);
            this.DatabasesMirroring_Fill_Panel.PerformLayout();
            this.contentContainerPanel.ResumeLayout(false);
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).EndInit();
            this.splitContainer1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.mirroredDatabasesGrid)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.mirroringDetailsTabControl)).EndInit();
            this.mirroringDetailsTabControl.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.mirroredDatabasesComboBox)).EndInit();
            this.disksTabPage.ResumeLayout(false);
            this.diskUsagePanel.ResumeLayout(false);
            this.currentDiskUsageChartContainerPanel.ResumeLayout(false);
            this.currentDiskUsageChartContainerPanel.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.diskUsageChart)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.toolbarsManager)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomPanel  DatabasesMirroring_Fill_Panel;
        private Infragistics.Win.Misc.UltraButton refreshDatabasesButton;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomPanel  contentContainerPanel;
        private System.Windows.Forms.SplitContainer splitContainer1;
        private Infragistics.Win.UltraWinGrid.UltraGrid mirroredDatabasesGrid;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomLabel databasesGridStatusLabel;
        private Infragistics.Win.UltraWinTabControl.UltraTabPageControl disksTabPage;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomPanel  diskUsagePanel;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomPanel  currentDiskUsageChartContainerPanel;
        private ChartFX.WinForms.Chart diskUsageChart;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomLabel diskUsageChartStatusLabel;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomLabel mirroringDetailsTabControlStatusLabel;
        private Infragistics.Win.UltraWinEditors.UltraComboEditor mirroredDatabasesComboBox;
        private Infragistics.Win.UltraWinTabControl.UltraTabControl mirroringDetailsTabControl;
        private Infragistics.Win.UltraWinTabControl.UltraTabSharedControlsPage ultraTabSharedControlsPage1;
        private Infragistics.Win.UltraWinTabControl.UltraTabPageControl statusTabPage;
        private Infragistics.Win.UltraWinTabControl.UltraTabPageControl historyTabPage;
        private Infragistics.Win.UltraWinTabControl.UltraTabPageControl propertiesTabPage;
        private Infragistics.Win.UltraWinGrid.UltraGrid transactionLogsGrid;
        private System.ComponentModel.BackgroundWorker refreshAvailableDatabasesBackgroundWorker;
        private Idera.SQLdm.DesktopClient.Controls.ContextMenuManager toolbarsManager;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomPanel  panel2;
        private Infragistics.Win.UltraWinGrid.UltraGrid historyGrid;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomPanel  panel1;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomLabel lblFilterLisyBy;
        private Infragistics.Win.UltraWinEditors.UltraComboEditor mirrorMonitoringHistoryPeriodComboBox;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomPanel  panel3;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomRadioButton historyPartnerViewRadioButton;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomRadioButton historyLocalViewRadioButton;
        private System.ComponentModel.BackgroundWorker refreshMirroringHistoryBackgroundWorker;
        private System.ComponentModel.BackgroundWorker refreshMirroringDetailsBackgroundWorker;
        private System.ComponentModel.BackgroundWorker failOverBackgroundWorker;
        private System.ComponentModel.BackgroundWorker suspendResumeBackgroundWorker;
        private System.Windows.Forms.SaveFileDialog saveFileDialog;
        private Infragistics.Win.UltraWinGrid.ExcelExport.UltraGridExcelExporter ultraGridExcelExporter;
        private Infragistics.Win.Printing.UltraPrintPreviewDialog ultraPrintPreviewDialog;
        private Infragistics.Win.UltraWinGrid.UltraGridPrintDocument ultraGridPrintDocument;
        private System.ComponentModel.BackgroundWorker setOperationalStatusBackgroundWorker;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomPanel  pnlDetails;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomGroupBox mirrorGroup;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomLabel lblRestoreRate;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomLabel lblTimeToRestore;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomLabel lblUnrestoredLog;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomGroupBox principalGroup;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomLabel lblRateofNewTrans;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomLabel lblSendRate;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomLabel lblOldestUnsent;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomLabel lblTimeToSend;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomLabel lblUnsentLog;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomPanel  pnlMessage;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomLabel lblDetailsMessage;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomLabel lblMirrorAddress;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomLabel lblPrincipalAddress;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomLabel lblOperatingMode;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomLabel lblWitnessAddress;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomLabel lblTimeToSendAndRestore;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomLabel lblMirrorCommitOverhead;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomLinkLabel historicalSnapshotStatusLinkLabel;
        private MRG.Controls.UI.LoadingCircle actionProgressControl;
        private System.ComponentModel.BackgroundWorker refreshPartnerDetailsBackgroundWorker;
        private Infragistics.Win.Appearance appearance1;
    }
}
