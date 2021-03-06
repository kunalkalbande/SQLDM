using System.Drawing;
using System.Windows.Forms;
using Idera.Newsfeed.Plugins.UI;
using Idera.SQLdm.DesktopClient.Properties;

namespace Idera.SQLdm.DesktopClient.Views
{
    partial class NewSQLdmTodayView
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
                ApplicationModel.Default.ActiveInstances.Changed -= ActiveServers_Changed;
                ApplicationController.Default.BackgroundRefreshCompleted -= BackgroundRefreshCompleted;
                PulseController.Default.MoreInfoRequested -= PulseController_MoreInfoRequested;
                PulseController.Default.ActivityFeedLinkClicked -= PulseController_ActivityFeedLinkClicked;

                if (components != null)
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
            System.Diagnostics.Stopwatch stopWatch = new System.Diagnostics.Stopwatch();
            stopWatch.Start();
            this.components = new System.ComponentModel.Container();
            Infragistics.Win.Appearance appearance16 = new Infragistics.Win.Appearance();
            Infragistics.Win.UltraWinGrid.UltraGridBand ultraGridBand1 = new Infragistics.Win.UltraWinGrid.UltraGridBand("Band 0", -1);
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn13 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("AlertID");
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn14 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("UTCOccurrenceDateTime");
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn15 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("ServerName");
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn16 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("DatabaseName");
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn17 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("TableName");
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn18 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("Active");
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn19 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("Metric", -1, 34096297);
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn20 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("Severity", -1, 34107751);
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn21 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("StateEvent", -1, 34116813);
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn22 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("Value");
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn23 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("Heading");
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn24 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("Message");
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn25 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("ServerType");
            Infragistics.Win.Appearance appearance17 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance18 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance19 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance20 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance21 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance22 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance23 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance24 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance25 = new Infragistics.Win.Appearance();
            Infragistics.Win.ValueList valueList1 = new Controls.CustomControls.CustomValueList(34096297);
            Infragistics.Win.ValueList valueList2 = new Controls.CustomControls.CustomValueList(34107751);
            Infragistics.Win.ValueList valueList3 = new Controls.CustomControls.CustomValueList(34116813);
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
            Infragistics.Win.Appearance appearance1 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance2 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance3 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance4 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance39 = new Infragistics.Win.Appearance();
            Infragistics.Win.UltraWinTabControl.UltraTab ultraTab4 = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomUltraTab();
            Infragistics.Win.UltraWinTabControl.UltraTab ultraTab5 = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomUltraTab();
            Infragistics.Win.UltraWinTabControl.UltraTab ultraTab7 = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomUltraTab();
            Infragistics.Win.UltraWinTabControl.UltraTab ultraTab6 = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomUltraTab();
            Infragistics.Win.UltraWinToolbars.PopupMenuTool popupMenuTool1 = new Infragistics.Win.UltraWinToolbars.PopupMenuTool("columnContextMenu");
            Infragistics.Win.UltraWinToolbars.ButtonTool buttonTool1 = new Controls.CustomControls.CustomButtonTool("sortAscendingButton");
            Infragistics.Win.UltraWinToolbars.ButtonTool buttonTool2 = new Controls.CustomControls.CustomButtonTool("sortDescendingButton");
            Infragistics.Win.UltraWinToolbars.StateButtonTool stateButtonTool1 = new Infragistics.Win.UltraWinToolbars.StateButtonTool("groupByThisColumnButton", "");
            Infragistics.Win.UltraWinToolbars.ButtonTool buttonTool3 = new Controls.CustomControls.CustomButtonTool("toggleGroupByBoxButton");
            Infragistics.Win.UltraWinToolbars.ButtonTool buttonTool4 = new Controls.CustomControls.CustomButtonTool("removeThisColumnButton");
            Infragistics.Win.UltraWinToolbars.ButtonTool buttonTool5 = new Controls.CustomControls.CustomButtonTool("showColumnChooserButton");
            Infragistics.Win.UltraWinToolbars.ButtonTool buttonTool6 = new Controls.CustomControls.CustomButtonTool("showColumnChooserButton");
            Infragistics.Win.Appearance appearance26 = new Infragistics.Win.Appearance();
            Infragistics.Win.UltraWinToolbars.ButtonTool buttonTool7 = new Controls.CustomControls.CustomButtonTool("toggleGroupByBoxButton");
            Infragistics.Win.Appearance appearance27 = new Infragistics.Win.Appearance();
            Infragistics.Win.UltraWinToolbars.ButtonTool buttonTool8 = new Controls.CustomControls.CustomButtonTool("sortAscendingButton");
            Infragistics.Win.Appearance appearance28 = new Infragistics.Win.Appearance();
            Infragistics.Win.UltraWinToolbars.ButtonTool buttonTool9 = new Controls.CustomControls.CustomButtonTool("sortDescendingButton");
            Infragistics.Win.Appearance appearance29 = new Infragistics.Win.Appearance();
            Infragistics.Win.UltraWinToolbars.ButtonTool buttonTool10 = new Controls.CustomControls.CustomButtonTool("removeThisColumnButton");
            Infragistics.Win.UltraWinToolbars.StateButtonTool stateButtonTool2 = new Infragistics.Win.UltraWinToolbars.StateButtonTool("groupByThisColumnButton", "");
            Infragistics.Win.UltraWinToolbars.PopupMenuTool popupMenuTool2 = new Infragistics.Win.UltraWinToolbars.PopupMenuTool("gridContextMenu");
            Infragistics.Win.UltraWinToolbars.ButtonTool buttonTool48 = new Controls.CustomControls.CustomButtonTool("viewDeadlockDetailsButton");
            Infragistics.Win.UltraWinToolbars.ButtonTool buttonTool38 = new Controls.CustomControls.CustomButtonTool("viewAlertRealTimeSnapshotButton");
            Infragistics.Win.UltraWinToolbars.ButtonTool buttonTool39 = new Controls.CustomControls.CustomButtonTool("viewAlertHistoricalSnapshotButton");
            Infragistics.Win.UltraWinToolbars.ButtonTool buttonTool43 = new Controls.CustomControls.CustomButtonTool("viewAlertHelpButton");
            Infragistics.Win.UltraWinToolbars.ButtonTool buttonTool11 = new Controls.CustomControls.CustomButtonTool("clearAlertButton");
            Infragistics.Win.UltraWinToolbars.ButtonTool buttonTool12 = new Controls.CustomControls.CustomButtonTool("clearAllAlertsButton");
            Infragistics.Win.UltraWinToolbars.ButtonTool buttonTool13 = new Controls.CustomControls.CustomButtonTool("configureAlertsButton");
            Infragistics.Win.UltraWinToolbars.ButtonTool buttonTool47 = new Controls.CustomControls.CustomButtonTool("snoozeAlertButton");
            Infragistics.Win.UltraWinToolbars.ButtonTool buttonTool14 = new Controls.CustomControls.CustomButtonTool("copyToClipboardButton");
            Infragistics.Win.UltraWinToolbars.ButtonTool buttonTool15 = new Controls.CustomControls.CustomButtonTool("printGridButton");
            Infragistics.Win.Appearance appearance30 = new Infragistics.Win.Appearance();
            Infragistics.Win.UltraWinToolbars.ButtonTool buttonTool16 = new Controls.CustomControls.CustomButtonTool("exportGridButton");
            Infragistics.Win.Appearance appearance31 = new Infragistics.Win.Appearance();
            Infragistics.Win.UltraWinToolbars.ButtonTool buttonTool17 = new Controls.CustomControls.CustomButtonTool("configureAlertsButton");
            Infragistics.Win.UltraWinToolbars.PopupMenuTool popupMenuTool3 = new Infragistics.Win.UltraWinToolbars.PopupMenuTool("taskGridContextMenu");
            Infragistics.Win.UltraWinToolbars.ButtonTool buttonTool46 = new Controls.CustomControls.CustomButtonTool("viewTaskRealTimeSnapshotButton");
            Infragistics.Win.UltraWinToolbars.ButtonTool buttonTool45 = new Controls.CustomControls.CustomButtonTool("viewTaskHistoricalSnapshotButton");
            Infragistics.Win.UltraWinToolbars.PopupMenuTool popupMenuTool4 = new Infragistics.Win.UltraWinToolbars.PopupMenuTool("taskStatusMenu");
            Infragistics.Win.UltraWinToolbars.ButtonTool buttonTool18 = new Controls.CustomControls.CustomButtonTool("deleteButton");
            Infragistics.Win.UltraWinToolbars.ButtonTool buttonTool20 = new Controls.CustomControls.CustomButtonTool("copyToClipboardButton");
            Infragistics.Win.UltraWinToolbars.ButtonTool buttonTool21 = new Controls.CustomControls.CustomButtonTool("propertiesButton");
            Infragistics.Win.UltraWinToolbars.ButtonTool buttonTool22 = new Controls.CustomControls.CustomButtonTool("deleteButton");
            Infragistics.Win.UltraWinToolbars.ButtonTool buttonTool23 = new Controls.CustomControls.CustomButtonTool("showAssociatedViewButton");
            Infragistics.Win.UltraWinToolbars.ButtonTool buttonTool24 = new Controls.CustomControls.CustomButtonTool("propertiesButton");
            Infragistics.Win.Appearance appearance65 = new Infragistics.Win.Appearance();
            Infragistics.Win.UltraWinToolbars.PopupMenuTool popupMenuTool5 = new Infragistics.Win.UltraWinToolbars.PopupMenuTool("taskStatusMenu");
            Infragistics.Win.UltraWinToolbars.ButtonTool buttonTool25 = new Controls.CustomControls.CustomButtonTool("notStartedButton");
            Infragistics.Win.UltraWinToolbars.ButtonTool buttonTool26 = new Controls.CustomControls.CustomButtonTool("inProgressButton");
            Infragistics.Win.UltraWinToolbars.ButtonTool buttonTool27 = new Controls.CustomControls.CustomButtonTool("onHoldButton");
            Infragistics.Win.UltraWinToolbars.ButtonTool buttonTool28 = new Controls.CustomControls.CustomButtonTool("completedButton");
            Infragistics.Win.UltraWinToolbars.ButtonTool buttonTool29 = new Controls.CustomControls.CustomButtonTool("notStartedButton");
            Infragistics.Win.UltraWinToolbars.ButtonTool buttonTool30 = new Controls.CustomControls.CustomButtonTool("inProgressButton");
            Infragistics.Win.UltraWinToolbars.ButtonTool buttonTool31 = new Controls.CustomControls.CustomButtonTool("onHoldButton");
            Infragistics.Win.UltraWinToolbars.ButtonTool buttonTool32 = new Controls.CustomControls.CustomButtonTool("completedButton");
            Infragistics.Win.UltraWinToolbars.ButtonTool buttonTool33 = new Controls.CustomControls.CustomButtonTool("copyToClipboardButton");
            Infragistics.Win.Appearance appearance32 = new Infragistics.Win.Appearance();
            Infragistics.Win.UltraWinToolbars.ButtonTool buttonTool34 = new Controls.CustomControls.CustomButtonTool("clearAlertButton");
            Infragistics.Win.UltraWinToolbars.ButtonTool buttonTool35 = new Controls.CustomControls.CustomButtonTool("clearAllAlertsButton");
            Infragistics.Win.UltraWinToolbars.ButtonTool buttonTool36 = new Controls.CustomControls.CustomButtonTool("viewAlertRealTimeSnapshotButton");
            Infragistics.Win.Appearance appearance43 = new Infragistics.Win.Appearance();
            Infragistics.Win.UltraWinToolbars.ButtonTool buttonTool37 = new Controls.CustomControls.CustomButtonTool("viewAlertHistoricalSnapshotButton");
            Infragistics.Win.UltraWinToolbars.ButtonTool buttonTool40 = new Controls.CustomControls.CustomButtonTool("viewTaskRealTimeSnapshotButton");
            Infragistics.Win.UltraWinToolbars.ButtonTool buttonTool41 = new Controls.CustomControls.CustomButtonTool("viewTaskHistoricalSnapshotButton");
            Infragistics.Win.UltraWinToolbars.ButtonTool buttonTool19 = new Controls.CustomControls.CustomButtonTool("snoozeAlertButton");
            Infragistics.Win.UltraWinToolbars.ButtonTool buttonTool42 = new Controls.CustomControls.CustomButtonTool("viewAlertHelpButton");
            Infragistics.Win.UltraWinToolbars.ButtonTool buttonTool44 = new Controls.CustomControls.CustomButtonTool("viewDeadlockDetailsButton");
            Infragistics.Win.Appearance appearance5 = new Infragistics.Win.Appearance();
            this.ultraTabPageControl1 = new Infragistics.Win.UltraWinTabControl.UltraTabPageControl();
            this.panel7 = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomPanel ();
            //SQLdm 10.1 - (Pulkit Puri)
            this.newFeature04 = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomLabel();
            this.newFeature4 = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomLabel();
            this.newFeature5 = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomLabel();
            this.newFeature6 = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomLabel();
            this.newFeature7 = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomLabel();
            this.newFeature8 = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomLabel();
            this.newFeature03 = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomLabel();
            this.newFeature02 = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomLabel();
            this.pictureBox11 = new System.Windows.Forms.PictureBox();
            this.newFeature01 = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomLabel();
            this.label18 = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomLabel();
            this.connectionProgressBar = new Idera.SQLdm.DesktopClient.Controls.InfiniteProgressBar();
            this.tableLayoutPanel2 = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomTableLayoutPanel();
            this.pictureBox10 = new System.Windows.Forms.PictureBox();
            this.pictureBox3 = new System.Windows.Forms.PictureBox();
            this.pictureBox4 = new System.Windows.Forms.PictureBox();
            this.pictureBox5 = new System.Windows.Forms.PictureBox();
            this.pictureBox8 = new System.Windows.Forms.PictureBox();
            this.pictureBox9 = new System.Windows.Forms.PictureBox();
            this.label10 = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomLabel();
            this.label12 = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomLabel();
            this.label13 = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomLabel();
            this.label14 = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomLabel();
            this.label15 = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomLabel();
            this.label16 = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomLabel();
            this._trialCenterPanel = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomPanel ();
            this._trialCenterLabel2 = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomLabel();
            this._trialCenterLabel1 = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomLabel();
            this.pictureBox2 = new System.Windows.Forms.PictureBox();
            this.label22 = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomLabel();
            this.ultraTabPageControl2 = new Infragistics.Win.UltraWinTabControl.UltraTabPageControl();
            this.alertsGrid = new Infragistics.Win.UltraWinGrid.UltraGrid();
            this.alertsViewDataSource = new Idera.SQLdm.DesktopClient.Views.Alerts.AlertsViewDataSource();
            this.alertsStatusLabel = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomLabel();
            this.ultraTabPageControl4 = new Infragistics.Win.UltraWinTabControl.UltraTabPageControl();
            this._webConsolePanel = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomPanel ();
            this.ultraTabPageControl3 = new Infragistics.Win.UltraWinTabControl.UltraTabPageControl();
            this._pulseNewsFeedPanel = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomPanel ();
            this.label19_1 = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomLabel();
            this.label19_2 = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomLabel();
            this.webConsoleLinkLabel = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomLinkLabel();
            this.newFeature05 = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomLabel();
            this.pictureBox12 = new System.Windows.Forms.PictureBox();
            this.panel8 = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomPanel ();
            this.label11 = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomLabel();
            this._twitterLinkLabel = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomLinkLabel();
            this.label17 = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomLabel();
            this._facebookLinkLabel = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomLinkLabel();
            this.pictureBox7 = new System.Windows.Forms.PictureBox();
            this.pictureBox6 = new System.Windows.Forms.PictureBox();
            this.headerStrip = new Idera.SQLdm.DesktopClient.Controls.HeaderStrip();
            this.titleLabel = new System.Windows.Forms.ToolStripLabel();
            this.headerPanel = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomPanel ();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.refreshDateLabel = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomLabel();
            this.refreshTimeLabel = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomLabel();
            this.tableLayoutPanel1 = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomTableLayoutPanel();
            this.panel1 = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomPanel ();
            this.statusSummaryContentPanel = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomPanel ();
            this.statusSummaryLabel = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomLinkLabel();
            this.statusSummaryPictureBox = new System.Windows.Forms.PictureBox();
            this.maintenanceModeServersPictureBox = new System.Windows.Forms.PictureBox();
            this.okServersLabel = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomLinkLabel();
            this.okServersPictureBox = new System.Windows.Forms.PictureBox();
            this.maintenanceModeServersLabel = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomLinkLabel();
            this.warningServersPictureBox = new System.Windows.Forms.PictureBox();
            this.statusSummaryDescriptionLabel = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomLinkLabel();
            this.criticalServersLabel = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomLinkLabel();
            this.warningServersLabel = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomLinkLabel();
            this.criticalServersPictureBox = new System.Windows.Forms.PictureBox();
            this.statusSummaryLoadingLabel = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomLabel();
            this.panel5 = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomPanel ();
            this.label2 = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomLabel();
            this.label3 = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomLabel();
            this.panel3 = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomPanel ();
            this._contentTabControl = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomUltraTabControl();
            this.ultraTabSharedControlsPage1 = new Infragistics.Win.UltraWinTabControl.UltraTabSharedControlsPage();
            this.label1 = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomLabel();
            this.panel2 = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomPanel ();
            this._recentServersPanel = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomPanel ();
            this._recentServersStatusImage5 = new System.Windows.Forms.PictureBox();
            this._recentServersLinkLabel5 = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomLinkLabel();
            this._recentServersStatusImage4 = new System.Windows.Forms.PictureBox();
            this._recentServersLinkLabel4 = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomLinkLabel();
            this._recentServersStatusImage3 = new System.Windows.Forms.PictureBox();
            this._recentServersLinkLabel3 = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomLinkLabel();
            this._recentServersStatusImage2 = new System.Windows.Forms.PictureBox();
            this._recentServersLinkLabel2 = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomLinkLabel();
            this._recentServersStatusImage1 = new System.Windows.Forms.PictureBox();
            this._recentServersLinkLabel1 = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomLinkLabel();
            this.label6 = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomLabel();
            this.label7 = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomLabel();
            this._mostCriticalServersPanel = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomPanel ();
            this._mostCriticalServersStatusImage5 = new System.Windows.Forms.PictureBox();
            this._mostCriticalServersLinkLabel5 = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomLinkLabel();
            this._mostCriticalServersStatusImage4 = new System.Windows.Forms.PictureBox();
            this._mostCriticalServersLinkLabel4 = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomLinkLabel();
            this._mostCriticalServersStatusImage3 = new System.Windows.Forms.PictureBox();
            this._mostCriticalServersLinkLabel3 = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomLinkLabel();
            this._mostCriticalServersStatusImage2 = new System.Windows.Forms.PictureBox();
            this._mostCriticalServersLinkLabel2 = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomLinkLabel();
            this._mostCriticalServersStatusImage1 = new System.Windows.Forms.PictureBox();
            this._mostCriticalServersLinkLabel1 = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomLinkLabel();
            this.label4 = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomLabel();
            this.label5 = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomLabel();
            this.ultraPrintPreviewDialog = new Infragistics.Win.Printing.UltraPrintPreviewDialog(this.components);
            this.ultraGridPrintDocument = new Infragistics.Win.UltraWinGrid.UltraGridPrintDocument(this.components);
            this.toolbarsManager = new Idera.SQLdm.DesktopClient.Controls.ContextMenuManager(this.components);
            this.ultraGridExcelExporter = new Infragistics.Win.UltraWinGrid.ExcelExport.UltraGridExcelExporter(this.components);
            this.saveFileDialog = new System.Windows.Forms.SaveFileDialog();
            this.contentPanel = new Infragistics.Win.Misc.UltraPanel();
            this.osUnsupportedLabel = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomLabel();
            this._newsfeedLinkLabelRequirements = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomLinkLabel();
            this.ultraTabPageControl1.SuspendLayout();
            this.panel7.SuspendLayout();
            // this.BackColor = System.Drawing.Color.FromArgb(236, 235, 234);
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox11)).BeginInit();
            this.tableLayoutPanel2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox10)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox3)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox4)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox5)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox8)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox9)).BeginInit();
            this._trialCenterPanel.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox2)).BeginInit();
            this.ultraTabPageControl2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.alertsGrid)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.alertsViewDataSource)).BeginInit();
            this.ultraTabPageControl4.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox12)).BeginInit();
            this.ultraTabPageControl3.SuspendLayout();
            this.panel8.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox7)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox6)).BeginInit();
            this.headerStrip.SuspendLayout();
            this.headerPanel.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            this.tableLayoutPanel1.SuspendLayout();
            this.panel1.SuspendLayout();
            this.statusSummaryContentPanel.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.statusSummaryPictureBox)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.maintenanceModeServersPictureBox)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.okServersPictureBox)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.warningServersPictureBox)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.criticalServersPictureBox)).BeginInit();
            this.panel5.SuspendLayout();
            this.panel3.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this._contentTabControl)).BeginInit();
            this._contentTabControl.SuspendLayout();
            this.panel2.SuspendLayout();
            this._recentServersPanel.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this._recentServersStatusImage5)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this._recentServersStatusImage4)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this._recentServersStatusImage3)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this._recentServersStatusImage2)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this._recentServersStatusImage1)).BeginInit();
            this._mostCriticalServersPanel.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this._mostCriticalServersStatusImage5)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this._mostCriticalServersStatusImage4)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this._mostCriticalServersStatusImage3)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this._mostCriticalServersStatusImage2)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this._mostCriticalServersStatusImage1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.toolbarsManager)).BeginInit();
            this.contentPanel.ClientArea.SuspendLayout();
            this.contentPanel.SuspendLayout();
            this.SuspendLayout();
            // 
            // ultraTabPageControl1
            // 
            this.ultraTabPageControl1.Controls.Add(this.panel7);
            this.ultraTabPageControl1.Controls.Add(this.label22);
            this.ultraTabPageControl1.Location = new System.Drawing.Point(1, 26);
            this.ultraTabPageControl1.Name = "ultraTabPageControl1";
            this.ultraTabPageControl1.Size = new System.Drawing.Size(578, 503);
            // 
            // panel7
            // 
            this.panel7.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
            | System.Windows.Forms.AnchorStyles.Left)
            | System.Windows.Forms.AnchorStyles.Right)));
            this.panel7.AutoScroll = true;
            this.panel7.BackColor = System.Drawing.Color.Transparent;
            //SQLdm 10.1 - (Pulkit Puri)
            this.panel7.Controls.Add(this.newFeature8);
            this.panel7.Controls.Add(this.newFeature7);
            this.panel7.Controls.Add(this.newFeature6);
            this.panel7.Controls.Add(this.newFeature5);
            this.panel7.Controls.Add(this.newFeature4);
            this.panel7.Controls.Add(this.newFeature04);
            this.panel7.Controls.Add(this.newFeature03);
            this.panel7.Controls.Add(this.newFeature02);
            this.panel7.Controls.Add(this.pictureBox11);
            this.panel7.Controls.Add(this.newFeature01);
            this.panel7.Controls.Add(this.label18);
            this.panel7.Controls.Add(this.connectionProgressBar);
            this.panel7.Controls.Add(this.tableLayoutPanel2);
            this.panel7.Controls.Add(this._trialCenterPanel);
            this.panel7.Controls.Add(this.pictureBox2);
            this.panel7.Location = new System.Drawing.Point(12, 13);
            this.panel7.Name = "panel7";
            this.panel7.Size = new System.Drawing.Size(550, 700);
            this.panel7.TabIndex = 1;

            // SQLdm 10.3
            // newFeature8
            // 
            this.newFeature8.AutoSize = true;
            this.newFeature8.Font = new System.Drawing.Font("Arial", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.newFeature8.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(102)))), ((int)(((byte)(102)))), ((int)(((byte)(102)))));
            this.newFeature8.Location = new System.Drawing.Point(36, 536);
            this.newFeature8.Name = "newFeature8";
            this.newFeature8.Size = new System.Drawing.Size(234, 16);
            this.newFeature8.TabIndex = 58;
            this.newFeature8.Text = "▪   All alert priority levels were updated for existing alerts and for user customization.";
            this.newFeature8.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;

            // newFeature7
            // 
            this.newFeature7.AutoSize = true;
            this.newFeature7.Font = new System.Drawing.Font("Arial", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.newFeature7.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(102)))), ((int)(((byte)(102)))), ((int)(((byte)(102)))));
            this.newFeature7.Location = new System.Drawing.Point(36, 506);
            this.newFeature7.Name = "newFeature7";
            this.newFeature7.Size = new System.Drawing.Size(234, 16);
            this.newFeature7.TabIndex = 58;
            this.newFeature7.Text = "";
            this.newFeature7.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;

            // newFeature6
            // 
            this.newFeature6.AutoSize = true;
            this.newFeature6.Font = new System.Drawing.Font("Arial", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.newFeature6.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(102)))), ((int)(((byte)(102)))), ((int)(((byte)(102)))));
            this.newFeature6.Location = new System.Drawing.Point(36, 486);
            this.newFeature6.Name = "newFeature6";
            this.newFeature6.Size = new System.Drawing.Size(234, 16);
            this.newFeature6.TabIndex = 58;
            this.newFeature6.Text = "";
            this.newFeature6.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;

            // newFeature5
            // 
            this.newFeature5.AutoSize = true;
            this.newFeature5.Font = new System.Drawing.Font("Arial", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.newFeature5.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(102)))), ((int)(((byte)(102)))), ((int)(((byte)(102)))));
            this.newFeature5.Location = new System.Drawing.Point(35, 466);
            this.newFeature5.Name = "newFeature5";
            this.newFeature5.Size = new System.Drawing.Size(234, 16);
            this.newFeature5.TabIndex = 58;
            this.newFeature5.Text = "";
            this.newFeature5.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;

            // newFeature4
            // 
            this.newFeature4.AutoSize = true;
            this.newFeature4.Font = new System.Drawing.Font("Arial", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.newFeature4.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(102)))), ((int)(((byte)(102)))), ((int)(((byte)(102)))));
            this.newFeature4.Location = new System.Drawing.Point(72, 446);
            this.newFeature4.Name = "newFeature4";
            this.newFeature4.Size = new System.Drawing.Size(234, 16);
            this.newFeature4.TabIndex = 58;
            this.newFeature4.Text = "▪   Add, Edit, Import and collect Azure monitor metrics using the Azure Custom Counters.";
            this.newFeature4.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;

            // SQLdm 10.1 (Pulkit Puri)
            // newFeature04
            // 
            this.newFeature04.AutoSize = true;
            this.newFeature04.Font = new System.Drawing.Font("Arial", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.newFeature04.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(102)))), ((int)(((byte)(102)))), ((int)(((byte)(102)))));
            this.newFeature04.Location = new System.Drawing.Point(72, 426);
            this.newFeature04.Name = "newFeature04";
            this.newFeature04.Size = new System.Drawing.Size(234, 16);
            this.newFeature04.TabIndex = 57;
            //SQLdm 10.0 - (Ankit Nagpal)
            this.newFeature04.Text = "▪   Add Azure Custom Counters using Azure Application Profiles using REST interfaces.";
            this.newFeature04.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // newFeature03
            // 
            this.newFeature03.AutoSize = true;
            this.newFeature03.Font = new System.Drawing.Font("Arial", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.newFeature03.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(102)))), ((int)(((byte)(102)))), ((int)(((byte)(102)))));
            this.newFeature03.Location = new System.Drawing.Point(72, 406);
            this.newFeature03.Name = "newFeature03";
            this.newFeature03.Size = new System.Drawing.Size(234, 16);
            this.newFeature03.TabIndex = 56;
            //SQLdm 10.0 - (Ankit Nagpal)
            this.newFeature03.Text = "▪   Manage Azure application profile to manage Azure Subscription and Azure Application details.";
            this.newFeature03.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // newFeature02
            // 
            this.newFeature02.AutoSize = true;
            this.newFeature02.Font = new System.Drawing.Font("Arial", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.newFeature02.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(102)))), ((int)(((byte)(102)))), ((int)(((byte)(102)))));
            this.newFeature02.Location = new System.Drawing.Point(72, 375);
            this.newFeature02.Name = "newFeature02";
            this.newFeature02.Size = new System.Drawing.Size(234, 16);
            this.newFeature02.TabIndex = 55;
            this.newFeature02.Text = "▪   Show the recommendations that apply to each specific platform should be run when the target is that\n" +
                                     "     platform.";
            this.newFeature02.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // pictureBox11
            // 
            this.pictureBox11.Location = new System.Drawing.Point(0, 0);
            this.pictureBox11.Name = "pictureBox11";
            this.pictureBox11.Size = new System.Drawing.Size(100, 50);
            this.pictureBox11.TabIndex = 58;
            this.pictureBox11.TabStop = false;
            // 
            // newFeature01
            // 
            this.newFeature01.AutoSize = true;
            this.newFeature01.Font = new System.Drawing.Font("Arial", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.newFeature01.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(102)))), ((int)(((byte)(102)))), ((int)(((byte)(102)))));
            this.newFeature01.Location = new System.Drawing.Point(36, 354);
            this.newFeature01.Name = "newFeature01";
            this.newFeature01.Size = new System.Drawing.Size(390, 16);
            this.newFeature01.TabIndex = 52;
            //SQLdm 10.1 - (Pulkit Puri)
            this.newFeature01.Text = "▪  SQL Diagnostic Manager now includes Presciptive Analysis Improvements updated user interface that matches\n" +
                                     "   SQL Doctor workflows, and updated recommendations (specifically for Azure SQL and Amazon RDS).";
            this.newFeature01.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // label18
            // 
            this.label18.AutoSize = true;
            this.label18.Font = new System.Drawing.Font("Arial", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label18.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(74)))), ((int)(((byte)(101)))), ((int)(((byte)(132)))));
            this.label18.Location = new System.Drawing.Point(25, 321);
            this.label18.Name = "label18";
            this.label18.Size = new System.Drawing.Size(403, 22);
            this.label18.TabIndex = 51;
            
            this.label18.Text = "What’s New in SQL Diagnostic Manager 11.0";// SQLDM 10.1 (Pulkit Puri)--for new version
                                                                            // 
                                                                            // connectionProgressBar
                                                                            // 
            this.connectionProgressBar.Color1 = System.Drawing.Color.FromArgb(((int)(((byte)(137)))), ((int)(((byte)(137)))), ((int)(((byte)(137)))));
            this.connectionProgressBar.Color2 = System.Drawing.Color.White;
            this.connectionProgressBar.Location = new System.Drawing.Point(2, 299);
            this.connectionProgressBar.Name = "connectionProgressBar";
            this.connectionProgressBar.Size = new System.Drawing.Size(538, 1);
            this.connectionProgressBar.Speed = 15D;
            this.connectionProgressBar.Step = 5F;
            this.connectionProgressBar.TabIndex = 50;
            // 
            // tableLayoutPanel2
            // 
            this.tableLayoutPanel2.ColumnCount = 2;
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 40F));
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel2.Controls.Add(this.pictureBox10, 0, 5);
            this.tableLayoutPanel2.Controls.Add(this.pictureBox3, 0, 0);
            this.tableLayoutPanel2.Controls.Add(this.pictureBox4, 0, 1);
            this.tableLayoutPanel2.Controls.Add(this.pictureBox5, 0, 2);
            this.tableLayoutPanel2.Controls.Add(this.pictureBox8, 0, 3);
            this.tableLayoutPanel2.Controls.Add(this.pictureBox9, 0, 4);
            this.tableLayoutPanel2.Controls.Add(this.label10, 1, 0);
            this.tableLayoutPanel2.Controls.Add(this.label12, 1, 1);
            this.tableLayoutPanel2.Controls.Add(this.label13, 1, 2);
            this.tableLayoutPanel2.Controls.Add(this.label14, 1, 3);
            this.tableLayoutPanel2.Controls.Add(this.label15, 1, 4);
            this.tableLayoutPanel2.Controls.Add(this.label16, 1, 5);
            this.tableLayoutPanel2.Location = new System.Drawing.Point(262, 14);
            this.tableLayoutPanel2.Name = "tableLayoutPanel2";
            this.tableLayoutPanel2.RowCount = 6;
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 16.66667F));
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 16.66667F));
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 16.66667F));
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 16.66667F));
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 16.66667F));
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 16.66667F));
            this.tableLayoutPanel2.Size = new System.Drawing.Size(277, 272);
            this.tableLayoutPanel2.TabIndex = 2;
            // 
            // pictureBox10
            // 
            this.pictureBox10.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pictureBox10.Image = global::Idera.SQLdm.DesktopClient.Properties.Resources._6;
            this.pictureBox10.Location = new System.Drawing.Point(3, 228);
            this.pictureBox10.Name = "pictureBox10";
            this.pictureBox10.Size = new System.Drawing.Size(34, 41);
            this.pictureBox10.SizeMode = System.Windows.Forms.PictureBoxSizeMode.CenterImage;
            this.pictureBox10.TabIndex = 5;
            this.pictureBox10.TabStop = false;
            // 
            // pictureBox3
            // 
            this.pictureBox3.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pictureBox3.Image = global::Idera.SQLdm.DesktopClient.Properties.Resources._1;
            this.pictureBox3.Location = new System.Drawing.Point(3, 3);
            this.pictureBox3.Name = "pictureBox3";
            this.pictureBox3.Size = new System.Drawing.Size(34, 39);
            this.pictureBox3.SizeMode = System.Windows.Forms.PictureBoxSizeMode.CenterImage;
            this.pictureBox3.TabIndex = 0;
            this.pictureBox3.TabStop = false;
            // 
            // pictureBox4
            // 
            this.pictureBox4.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pictureBox4.Image = global::Idera.SQLdm.DesktopClient.Properties.Resources._2;
            this.pictureBox4.Location = new System.Drawing.Point(3, 48);
            this.pictureBox4.Name = "pictureBox4";
            this.pictureBox4.Size = new System.Drawing.Size(34, 39);
            this.pictureBox4.SizeMode = System.Windows.Forms.PictureBoxSizeMode.CenterImage;
            this.pictureBox4.TabIndex = 1;
            this.pictureBox4.TabStop = false;
            // 
            // pictureBox5
            // 
            this.pictureBox5.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pictureBox5.Image = global::Idera.SQLdm.DesktopClient.Properties.Resources._3;
            this.pictureBox5.Location = new System.Drawing.Point(3, 93);
            this.pictureBox5.Name = "pictureBox5";
            this.pictureBox5.Size = new System.Drawing.Size(34, 39);
            this.pictureBox5.SizeMode = System.Windows.Forms.PictureBoxSizeMode.CenterImage;
            this.pictureBox5.TabIndex = 2;
            this.pictureBox5.TabStop = false;
            // 
            // pictureBox8
            // 
            this.pictureBox8.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pictureBox8.Image = global::Idera.SQLdm.DesktopClient.Properties.Resources._4;
            this.pictureBox8.Location = new System.Drawing.Point(3, 138);
            this.pictureBox8.Name = "pictureBox8";
            this.pictureBox8.Size = new System.Drawing.Size(34, 39);
            this.pictureBox8.SizeMode = System.Windows.Forms.PictureBoxSizeMode.CenterImage;
            this.pictureBox8.TabIndex = 3;
            this.pictureBox8.TabStop = false;
            // 
            // pictureBox9
            // 
            this.pictureBox9.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pictureBox9.Image = global::Idera.SQLdm.DesktopClient.Properties.Resources._5;
            this.pictureBox9.Location = new System.Drawing.Point(3, 183);
            this.pictureBox9.Name = "pictureBox9";
            this.pictureBox9.Size = new System.Drawing.Size(34, 39);
            this.pictureBox9.SizeMode = System.Windows.Forms.PictureBoxSizeMode.CenterImage;
            this.pictureBox9.TabIndex = 4;
            this.pictureBox9.TabStop = false;
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.Dock = System.Windows.Forms.DockStyle.Fill;
            this.label10.Font = new System.Drawing.Font("Arial", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label10.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(102)))), ((int)(((byte)(102)))), ((int)(((byte)(102)))));
            this.label10.Location = new System.Drawing.Point(43, 0);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(231, 45);
            this.label10.TabIndex = 6;
            this.label10.Text = "Advanced Monitoring and Alerting";
            this.label10.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // label12
            // 
            this.label12.AutoSize = true;
            this.label12.Dock = System.Windows.Forms.DockStyle.Fill;
            this.label12.Font = new System.Drawing.Font("Arial", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label12.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(102)))), ((int)(((byte)(102)))), ((int)(((byte)(102)))));
            this.label12.Location = new System.Drawing.Point(43, 45);
            this.label12.Name = "label12";
            this.label12.Size = new System.Drawing.Size(231, 45);
            this.label12.TabIndex = 7;
            this.label12.Text = "Performance Diagnostics";
            this.label12.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // label13
            // 
            this.label13.AutoSize = true;
            this.label13.Dock = System.Windows.Forms.DockStyle.Fill;
            this.label13.Font = new System.Drawing.Font("Arial", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label13.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(102)))), ((int)(((byte)(102)))), ((int)(((byte)(102)))));
            this.label13.Location = new System.Drawing.Point(43, 90);
            this.label13.Name = "label13";
            this.label13.Size = new System.Drawing.Size(231, 45);
            this.label13.TabIndex = 8;
            this.label13.Text = "Query Performance Analysis";
            this.label13.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // label14
            // 
            this.label14.AutoSize = true;
            this.label14.Dock = System.Windows.Forms.DockStyle.Fill;
            this.label14.Font = new System.Drawing.Font("Arial", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label14.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(102)))), ((int)(((byte)(102)))), ((int)(((byte)(102)))));
            this.label14.Location = new System.Drawing.Point(43, 135);
            this.label14.Name = "label14";
            this.label14.Size = new System.Drawing.Size(231, 45);
            this.label14.TabIndex = 9;
            this.label14.Text = "Trends && Forecasts";
            this.label14.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // label15
            // 
            this.label15.AutoSize = true;
            this.label15.Dock = System.Windows.Forms.DockStyle.Fill;
            this.label15.Font = new System.Drawing.Font("Arial", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label15.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(102)))), ((int)(((byte)(102)))), ((int)(((byte)(102)))));
            this.label15.Location = new System.Drawing.Point(43, 180);
            this.label15.Name = "label15";
            this.label15.Size = new System.Drawing.Size(231, 45);
            this.label15.TabIndex = 10;
            this.label15.Text = "Mobile Monitoring";
            this.label15.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // label16
            // 
            this.label16.AutoSize = true;
            this.label16.Dock = System.Windows.Forms.DockStyle.Fill;
            this.label16.Font = new System.Drawing.Font("Arial", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label16.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(102)))), ((int)(((byte)(102)))), ((int)(((byte)(102)))));
            this.label16.Location = new System.Drawing.Point(43, 225);
            this.label16.Name = "label16";
            this.label16.Size = new System.Drawing.Size(231, 47);
            this.label16.TabIndex = 11;
            this.label16.Text = "Collaborative IDERA Newsfeed";
            this.label16.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // _trialCenterPanel
            // 
            this._trialCenterPanel.BackgroundImage = global::Idera.SQLdm.DesktopClient.Properties.Resources.SQLdmToday_buttonbackgrnd;
            this._trialCenterPanel.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Center;
            this._trialCenterPanel.Controls.Add(this._trialCenterLabel2);
            this._trialCenterPanel.Controls.Add(this._trialCenterLabel1);
            this._trialCenterPanel.Cursor = System.Windows.Forms.Cursors.Hand;
            this._trialCenterPanel.Location = new System.Drawing.Point(7, 210);
            this._trialCenterPanel.Name = "_trialCenterPanel";
            this._trialCenterPanel.Size = new System.Drawing.Size(257, 50);
            this._trialCenterPanel.TabIndex = 1;
            this._trialCenterPanel.Click += new System.EventHandler(this._trialCenterControl_Click);
            // 
            // _trialCenterLabel2
            // 
            this._trialCenterLabel2.AutoSize = true;
            this._trialCenterLabel2.Cursor = System.Windows.Forms.Cursors.Hand;
            this._trialCenterLabel2.Font = new System.Drawing.Font("Arial", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this._trialCenterLabel2.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(66)))), ((int)(((byte)(0)))));
            this._trialCenterLabel2.Location = new System.Drawing.Point(25, 26);
            this._trialCenterLabel2.Name = "_trialCenterLabel2";
            this._trialCenterLabel2.Size = new System.Drawing.Size(174, 16);
            this._trialCenterLabel2.TabIndex = 1;
            this._trialCenterLabel2.Text = "Visit the IDERA Trial Center";
            this._trialCenterLabel2.Click += new System.EventHandler(this._trialCenterControl_Click);
            // 
            // _trialCenterLabel1
            // 
            this._trialCenterLabel1.AutoSize = true;
            this._trialCenterLabel1.Cursor = System.Windows.Forms.Cursors.Hand;
            this._trialCenterLabel1.Font = new System.Drawing.Font("Arial", 9.75F, ((System.Drawing.FontStyle)((System.Drawing.FontStyle.Bold | System.Drawing.FontStyle.Italic))), System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this._trialCenterLabel1.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(51)))), ((int)(((byte)(51)))), ((int)(((byte)(51)))));
            this._trialCenterLabel1.Location = new System.Drawing.Point(25, 10);
            this._trialCenterLabel1.Name = "_trialCenterLabel1";
            this._trialCenterLabel1.Size = new System.Drawing.Size(124, 16);
            this._trialCenterLabel1.TabIndex = 0;
            this._trialCenterLabel1.Text = "See them in action";
            this._trialCenterLabel1.Click += new System.EventHandler(this._trialCenterControl_Click);
            // 
            // pictureBox2
            // 
            this.pictureBox2.Image = global::Idera.SQLdm.DesktopClient.Properties.Resources.SQLdmToday_6Things;
            this.pictureBox2.Location = new System.Drawing.Point(5, 44);
            this.pictureBox2.Name = "pictureBox2";
            this.pictureBox2.Size = new System.Drawing.Size(257, 147);
            this.pictureBox2.SizeMode = System.Windows.Forms.PictureBoxSizeMode.CenterImage;
            this.pictureBox2.TabIndex = 0;
            this.pictureBox2.TabStop = false;
            // 
            // label22
            // 
            this.label22.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
            | System.Windows.Forms.AnchorStyles.Right)));
            this.label22.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(137)))), ((int)(((byte)(137)))), ((int)(((byte)(137)))));
            this.label22.Location = new System.Drawing.Point(96, 0);
            this.label22.Name = "label22";
            this.label22.Size = new System.Drawing.Size(482, 1);
            this.label22.TabIndex = 3;
            // 
            // ultraTabPageControl2
            // 
            this.ultraTabPageControl2.Controls.Add(this.alertsGrid);
            this.ultraTabPageControl2.Controls.Add(this.alertsStatusLabel);
            this.ultraTabPageControl2.Location = new System.Drawing.Point(-10000, -10000);
            this.ultraTabPageControl2.Name = "ultraTabPageControl2";
            this.ultraTabPageControl2.Size = new System.Drawing.Size(578, 503);
            // 
            // alertsGrid
            // 
            this.alertsGrid.DataMember = "Band 0";
            this.alertsGrid.DataSource = this.alertsViewDataSource;
            appearance16.BackColor = System.Drawing.SystemColors.Window;
            appearance16.BorderColor = System.Drawing.SystemColors.InactiveCaption;
            this.alertsGrid.DisplayLayout.Appearance = appearance16;
            this.alertsGrid.DisplayLayout.AutoFitStyle = Infragistics.Win.UltraWinGrid.AutoFitStyle.ExtendLastColumn;
            ultraGridColumn13.AutoCompleteMode = Infragistics.Win.AutoCompleteMode.Append;
            ultraGridColumn13.ExcludeFromColumnChooser = Infragistics.Win.UltraWinGrid.ExcludeFromColumnChooser.True;
            
            ultraGridColumn13.Header.VisiblePosition = 10;
            ultraGridColumn13.Hidden = true;
            ultraGridColumn14.AutoCompleteMode = Infragistics.Win.AutoCompleteMode.Append;
            ultraGridColumn14.Format = "G";
            ultraGridColumn14.Header.Caption = "Time";
            ultraGridColumn14.Header.VisiblePosition = 2;
            ultraGridColumn14.Width = 131;
            ultraGridColumn15.AutoCompleteMode = Infragistics.Win.AutoCompleteMode.Append;
            ultraGridColumn15.Header.Caption = "Instance";
            ultraGridColumn15.Header.VisiblePosition = 3;
            ultraGridColumn15.Width = 141;
            ultraGridColumn16.AutoCompleteMode = Infragistics.Win.AutoCompleteMode.Append;
            ultraGridColumn16.Header.VisiblePosition = 4;
            ultraGridColumn16.Hidden = true;
            ultraGridColumn17.AutoCompleteMode = Infragistics.Win.AutoCompleteMode.Append;
            
            ultraGridColumn17.Header.VisiblePosition = 6;
            ultraGridColumn17.Hidden = true;
            ultraGridColumn18.AutoCompleteMode = Infragistics.Win.AutoCompleteMode.Append;
            ultraGridColumn18.ExcludeFromColumnChooser = Infragistics.Win.UltraWinGrid.ExcludeFromColumnChooser.True;
            ultraGridColumn18.Header.VisiblePosition = 10;
            ultraGridColumn18.Hidden = true;
            ultraGridColumn19.AutoCompleteMode = Infragistics.Win.AutoCompleteMode.Append;
            
            ultraGridColumn19.Header.VisiblePosition = 7;
            ultraGridColumn19.Hidden = true;
            ultraGridColumn20.AutoCompleteMode = Infragistics.Win.AutoCompleteMode.Append;
            ultraGridColumn20.Header.VisiblePosition = 0;
            ultraGridColumn20.Width = 49;
            ultraGridColumn21.AutoCompleteMode = Infragistics.Win.AutoCompleteMode.Append;
            ultraGridColumn21.Header.Caption = "Change";
            ultraGridColumn21.Header.VisiblePosition = 1;
            ultraGridColumn21.Hidden = true;
            ultraGridColumn21.Width = 134;
            ultraGridColumn22.AutoCompleteMode = Infragistics.Win.AutoCompleteMode.Append;
            ultraGridColumn22.ExcludeFromColumnChooser = Infragistics.Win.UltraWinGrid.ExcludeFromColumnChooser.True;
            
            ultraGridColumn22.Header.VisiblePosition = 12;
            ultraGridColumn22.Hidden = true;
            ultraGridColumn23.AutoCompleteMode = Infragistics.Win.AutoCompleteMode.Append;
            ultraGridColumn23.Header.Caption = "Description";
            
            ultraGridColumn23.Header.VisiblePosition = 8;
            ultraGridColumn24.AutoCompleteMode = Infragistics.Win.AutoCompleteMode.Append;
            
            ultraGridColumn24.Header.VisiblePosition = 9;
            ultraGridColumn24.Hidden = true;
            ultraGridColumn25.AutoCompleteMode = Infragistics.Win.AutoCompleteMode.Append;
            ultraGridColumn25.Header.Caption = "Server Type";
            ultraGridColumn25.Header.VisiblePosition = 5;
            ultraGridColumn25.Hidden = true;
            ultraGridColumn25.Width = 134;
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
            valueList1.DisplayStyle = Infragistics.Win.ValueListDisplayStyle.DisplayText;
            valueList1.Key = "Metrics";
            valueList1.PreferredDropDownSize = new System.Drawing.Size(0, 0);
            valueList2.DisplayStyle = Infragistics.Win.ValueListDisplayStyle.Picture;
            valueList2.Key = "Severity";
            valueList2.PreferredDropDownSize = new System.Drawing.Size(0, 0);
            valueList3.DisplayStyle = Infragistics.Win.ValueListDisplayStyle.DisplayTextAndPicture;
            valueList3.Key = "Transitions";
            valueList3.PreferredDropDownSize = new System.Drawing.Size(0, 0);
            this.alertsGrid.DisplayLayout.ValueLists.AddRange(new Infragistics.Win.ValueList[] {
            valueList1,
            valueList2,
            valueList3});
            this.alertsGrid.DisplayLayout.ViewStyle = Infragistics.Win.UltraWinGrid.ViewStyle.SingleBand;
            this.alertsGrid.DisplayLayout.ViewStyleBand = Infragistics.Win.UltraWinGrid.ViewStyleBand.OutlookGroupBy;
            this.alertsGrid.Dock = System.Windows.Forms.DockStyle.Fill;
            this.alertsGrid.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.alertsGrid.Location = new System.Drawing.Point(0, 0);
            this.alertsGrid.Name = "alertsGrid";
            this.alertsGrid.Size = new System.Drawing.Size(578, 503);
            this.alertsGrid.TabIndex = 2;
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
            // alertsStatusLabel
            // 
            this.alertsStatusLabel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.alertsStatusLabel.Location = new System.Drawing.Point(0, 0);
            this.alertsStatusLabel.Name = "alertsStatusLabel";
            this.alertsStatusLabel.Padding = new System.Windows.Forms.Padding(0, 5, 0, 0);
            this.alertsStatusLabel.Size = new System.Drawing.Size(578, 503);
            this.alertsStatusLabel.TabIndex = 3;
            this.alertsStatusLabel.Text = "There are no active items.";
            this.alertsStatusLabel.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            // 
            // ultraTabPageControl4
            // 
            this.ultraTabPageControl4.Controls.Add(this._webConsolePanel);
            this.ultraTabPageControl4.Location = new System.Drawing.Point(-10000, -10000);
            this.ultraTabPageControl4.Name = "ultraTabPageControl4";
            this.ultraTabPageControl4.Size = new System.Drawing.Size(578, 503);
            // 
            // _webConsolePanel
            // 
            this._webConsolePanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this._webConsolePanel.Location = new System.Drawing.Point(0, 0);
            this._webConsolePanel.Controls.Add(this.label19_1);
            this._webConsolePanel.Controls.Add(this.label19_2);
            this._webConsolePanel.Controls.Add(this.newFeature05);
            this._webConsolePanel.Controls.Add(this.webConsoleLinkLabel);
            this._webConsolePanel.Controls.Add(this.pictureBox12);
            this._webConsolePanel.Name = "_webConsolePanel";
            this._webConsolePanel.Size = new System.Drawing.Size(578, 675);
            // 
            // label19_1
            // 
            this.label19_1.AutoSize = true;
            this.label19_1.Font = new System.Drawing.Font("Arial", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label19_1.ForeColor = System.Drawing.Color.Black;
            this.label19_1.Location = new System.Drawing.Point(5, 15);
            this.label19_1.Name = "label19_1";
            this.label19_1.Size = new System.Drawing.Size(403, 20);
            this.label19_1.Text = "Welcome to the ";
            // 
            // label19_2
            // 
            this.label19_2.AutoSize = true;
            this.label19_2.Font = new System.Drawing.Font("Arial", 14.25F, System.Drawing.FontStyle.Bold | System.Drawing.FontStyle.Italic, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label19_2.ForeColor = System.Drawing.Color.OrangeRed;
            this.label19_2.Location = new System.Drawing.Point(135, 15);
            this.label19_2.Name = "label19_2";
            this.label19_2.Size = new System.Drawing.Size(403, 20);
            this.label19_2.Text = "SQL Diagnostic Manager Web Console";
            // 
            // newFeature05
            // 
            this.newFeature05.AutoSize = true;
            this.newFeature05.Font = new System.Drawing.Font("Arial", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.newFeature05.ForeColor = System.Drawing.Color.Black;
            this.newFeature05.Location = new System.Drawing.Point(5, 45);
            this.newFeature05.Name = "newFeature05";
            this.newFeature05.Size = new System.Drawing.Size(578, 200);
            this.newFeature05.Text = "SQL Diagnostic Manager features a web console to provide easy access to the health and performance of your SQL \n" +
                         "Server instances. This console provides read-only access to the alerts and data available in the management console. \n" +
                         "This allows you to provide easy access to your SQLDM users from anywhere as well as making this data to other \n " +
                         "groups with requiring any installation.";
            this.newFeature05.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // webConsoleLinkLabel
            // 
            this.webConsoleLinkLabel.AutoSize = true;
            this.webConsoleLinkLabel.Font = new System.Drawing.Font("Arial", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.webConsoleLinkLabel.LinkBehavior = System.Windows.Forms.LinkBehavior.AlwaysUnderline;
            this.webConsoleLinkLabel.LinkColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(102)))), ((int)(((byte)(204)))));
            this.webConsoleLinkLabel.Location = new System.Drawing.Point(80, 110);
            this.webConsoleLinkLabel.Name = "webConsoleLinkLabel";
            this.webConsoleLinkLabel.Size = new System.Drawing.Size(403, 22);
            this.webConsoleLinkLabel.Text = "Point your browser at ";
            this.webConsoleLinkLabel.TabStop = true;
            this.webConsoleLinkLabel.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.webConsoleLinkLabel_LinkClicked);
            // 
            // pictureBox12
            // 
            this.pictureBox12.Image = global::Idera.SQLdm.DesktopClient.Properties.Resources.SQLdmTodayWebConsole;
            this.pictureBox12.Location = new System.Drawing.Point(130, 135);
            this.pictureBox12.Name = "pictureBox12";
            this.pictureBox12.Size = new System.Drawing.Size(550, 300);
            this.pictureBox12.SizeMode = System.Windows.Forms.PictureBoxSizeMode.AutoSize;
            this.pictureBox12.TabStop = false;
            // 
            // ultraTabPageControl3
            // 
            this.ultraTabPageControl3.Controls.Add(this._pulseNewsFeedPanel);
            this.ultraTabPageControl3.Location = new System.Drawing.Point(-10000, -10000);
            this.ultraTabPageControl3.Name = "ultraTabPageControl3";
            this.ultraTabPageControl3.Size = new System.Drawing.Size(578, 503);
            // 
            // _pulseNewsFeedPanel
            // 
            this._pulseNewsFeedPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this._pulseNewsFeedPanel.Location = new System.Drawing.Point(0, 0);
            this._pulseNewsFeedPanel.Name = "_pulseNewsFeedPanel";
            this._pulseNewsFeedPanel.Size = new System.Drawing.Size(578, 503);
            this._pulseNewsFeedPanel.TabIndex = 2;
            // 
            // panel8
            // 
            this.panel8.BackColor = System.Drawing.Color.WhiteSmoke;
            this.tableLayoutPanel1.SetColumnSpan(this.panel8, 3);
            this.panel8.Controls.Add(this.label11);
            this.panel8.Controls.Add(this._twitterLinkLabel);
            this.panel8.Controls.Add(this.label17);
            this.panel8.Controls.Add(this._facebookLinkLabel);
            this.panel8.Controls.Add(this.pictureBox7);
            this.panel8.Controls.Add(this.pictureBox6);
            this.panel8.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel8.Location = new System.Drawing.Point(0, 605);
            this.panel8.Margin = new System.Windows.Forms.Padding(0);
            this.panel8.Name = "panel8";
            this.panel8.Size = new System.Drawing.Size(851, 35);
            this.panel8.TabIndex = 9;
            // 
            // label11
            // 
            this.label11.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(222)))), ((int)(((byte)(222)))), ((int)(((byte)(222)))));
            this.label11.Dock = System.Windows.Forms.DockStyle.Top;
            this.label11.Location = new System.Drawing.Point(0, 0);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(851, 1);
            this.label11.TabIndex = 55;
            // 
            // _twitterLinkLabel
            // 
            this._twitterLinkLabel.Anchor = System.Windows.Forms.AnchorStyles.Top;
            this._twitterLinkLabel.AutoSize = true;
            this._twitterLinkLabel.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this._twitterLinkLabel.LinkBehavior = System.Windows.Forms.LinkBehavior.HoverUnderline;
            this._twitterLinkLabel.LinkColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(102)))), ((int)(((byte)(204)))));
            this._twitterLinkLabel.Location = new System.Drawing.Point(480, 10);
            this._twitterLinkLabel.Name = "_twitterLinkLabel";
            this._twitterLinkLabel.Size = new System.Drawing.Size(46, 15);
            this._twitterLinkLabel.TabIndex = 54;
            this._twitterLinkLabel.TabStop = true;
            this._twitterLinkLabel.Text = "Twitter";
            this._twitterLinkLabel.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this._twitterLinkLabel_LinkClicked);
            // 
            // label17
            // 
            this.label17.Anchor = System.Windows.Forms.AnchorStyles.Top;
            this.label17.AutoSize = true;
            this.label17.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label17.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(70)))), ((int)(((byte)(70)))), ((int)(((byte)(70)))));
            this.label17.Location = new System.Drawing.Point(295, 10);
            this.label17.Name = "label17";
            this.label17.Size = new System.Drawing.Size(83, 15);
            this.label17.TabIndex = 50;
            this.label17.Text = "Follow us on: ";
            // 
            // _facebookLinkLabel
            // 
            this._facebookLinkLabel.Anchor = System.Windows.Forms.AnchorStyles.Top;
            this._facebookLinkLabel.AutoSize = true;
            this._facebookLinkLabel.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this._facebookLinkLabel.LinkBehavior = System.Windows.Forms.LinkBehavior.HoverUnderline;
            this._facebookLinkLabel.LinkColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(102)))), ((int)(((byte)(204)))));
            this._facebookLinkLabel.Location = new System.Drawing.Point(397, 10);
            this._facebookLinkLabel.Name = "_facebookLinkLabel";
            this._facebookLinkLabel.Size = new System.Drawing.Size(62, 15);
            this._facebookLinkLabel.TabIndex = 53;
            this._facebookLinkLabel.TabStop = true;
            this._facebookLinkLabel.Text = "Facebook";
            this._facebookLinkLabel.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this._facebookLinkLabel_LinkClicked);
            // 
            // pictureBox7
            // 
            this.pictureBox7.Anchor = System.Windows.Forms.AnchorStyles.Top;
            this.pictureBox7.Image = global::Idera.SQLdm.DesktopClient.Properties.Resources.facebook_16;
            this.pictureBox7.Location = new System.Drawing.Point(380, 9);
            this.pictureBox7.Name = "pictureBox7";
            this.pictureBox7.Size = new System.Drawing.Size(16, 16);
            this.pictureBox7.SizeMode = System.Windows.Forms.PictureBoxSizeMode.AutoSize;
            this.pictureBox7.TabIndex = 51;
            this.pictureBox7.TabStop = false;
            // 
            // pictureBox6
            // 
            this.pictureBox6.Anchor = System.Windows.Forms.AnchorStyles.Top;
            this.pictureBox6.Image = global::Idera.SQLdm.DesktopClient.Properties.Resources.twitter_16;
            this.pictureBox6.Location = new System.Drawing.Point(464, 9);
            this.pictureBox6.Name = "pictureBox6";
            this.pictureBox6.Size = new System.Drawing.Size(16, 16);
            this.pictureBox6.SizeMode = System.Windows.Forms.PictureBoxSizeMode.AutoSize;
            this.pictureBox6.TabIndex = 52;
            this.pictureBox6.TabStop = false;
            // 
            // headerStrip
            // 
            this.headerStrip.AutoSize = false;
            this.headerStrip.Font = new System.Drawing.Font("Arial", 12.75F, System.Drawing.FontStyle.Bold);
            this.headerStrip.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(75)))), ((int)(((byte)(75)))), ((int)(((byte)(75)))));
            this.headerStrip.GripStyle = System.Windows.Forms.ToolStripGripStyle.Hidden;
            this.headerStrip.HeaderImage = global::Idera.SQLdm.DesktopClient.Properties.Resources.ToolbarHome;
            this.headerStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.titleLabel});
            this.headerStrip.Location = new System.Drawing.Point(0, 0);
            this.headerStrip.Name = "headerStrip";
            this.headerStrip.Padding = new System.Windows.Forms.Padding(20, 2, 0, 0);
            this.headerStrip.Size = new System.Drawing.Size(851, 25);
            this.headerStrip.TabIndex = 1;
            // 
            // titleLabel
            // 
            this.titleLabel.Name = "titleLabel";
            this.titleLabel.Size = new System.Drawing.Size(119, 20);
            this.titleLabel.Text = "SQLDM Today";
            // 
            // headerPanel
            // 236,235,234
            this.headerPanel.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(236)))), ((int)(((byte)(235)))), ((int)(((byte)(234)))));
            this.headerPanel.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.tableLayoutPanel1.SetColumnSpan(this.headerPanel, 3);
            this.headerPanel.Controls.Add(this.pictureBox1);
            this.headerPanel.Controls.Add(this.refreshDateLabel);
            this.headerPanel.Controls.Add(this.refreshTimeLabel);
            this.headerPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.headerPanel.Location = new System.Drawing.Point(0, 0);
            this.headerPanel.Margin = new System.Windows.Forms.Padding(0);
            this.headerPanel.Name = "headerPanel";
            //this.headerPanel.Padding = new System.Windows.Forms.Padding(1);
            this.headerPanel.Size = new System.Drawing.Size(851, 30);
            this.headerPanel.TabIndex = 2;
            // 
            // pictureBox1
            // 
            this.pictureBox1.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.pictureBox1.Image = global::Idera.SQLdm.DesktopClient.Properties.Resources.SQLdmToday_ProductName;
            this.pictureBox1.Location = new System.Drawing.Point(0, 0);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(532, 45);
            this.pictureBox1.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.pictureBox1.TabIndex = 2;
            this.pictureBox1.TabStop = false;
            // 
            // refreshDateLabel
            // 
            this.refreshDateLabel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.refreshDateLabel.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(236)))), ((int)(((byte)(235)))), ((int)(((byte)(234)))));
            this.refreshDateLabel.Font = new System.Drawing.Font("Arial", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.refreshDateLabel.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(119)))), ((int)(((byte)(111)))), ((int)(((byte)(101)))));
            this.refreshDateLabel.ImageAlign = System.Drawing.ContentAlignment.TopLeft;
            this.refreshDateLabel.Location = new System.Drawing.Point(603, 3);
            this.refreshDateLabel.Name = "refreshDateLabel";
            this.refreshDateLabel.Size = new System.Drawing.Size(244, 18); //25
            this.refreshDateLabel.TabIndex = 0;
            this.refreshDateLabel.Text = "Monday, January 1, 2011";
            this.refreshDateLabel.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // refreshTimeLabel
            // 236,235,234
            this.refreshTimeLabel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.refreshTimeLabel.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(236)))), ((int)(((byte)(235)))), ((int)(((byte)(234)))));
            this.refreshTimeLabel.Font = new System.Drawing.Font("Arial", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.refreshTimeLabel.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(119)))), ((int)(((byte)(111)))), ((int)(((byte)(101)))));
            this.refreshTimeLabel.ImageAlign = System.Drawing.ContentAlignment.TopLeft;
            this.refreshTimeLabel.Location = new System.Drawing.Point(656, 23);
            this.refreshTimeLabel.Name = "refreshTimeLabel";
            this.refreshTimeLabel.Size = new System.Drawing.Size(191, 18); //25
            this.refreshTimeLabel.TabIndex = 1;
            this.refreshTimeLabel.Text = "12:00 AM";
            this.refreshTimeLabel.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.BackColor = System.Drawing.Color.White;
            this.tableLayoutPanel1.ColumnCount = 3;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 260F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 10F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel1.Controls.Add(this.panel1, 0, 1);
            this.tableLayoutPanel1.Controls.Add(this.headerPanel, 0, 0);
            this.tableLayoutPanel1.Controls.Add(this.panel3, 2, 1);
            this.tableLayoutPanel1.Controls.Add(this.panel8, 0, 3);
            this.tableLayoutPanel1.Controls.Add(this.panel2, 0, 2);
            this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel1.Location = new System.Drawing.Point(0, 25);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 4;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 45F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 180F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 35F));
            this.tableLayoutPanel1.Size = new System.Drawing.Size(851, 640);
            this.tableLayoutPanel1.TabIndex = 3;
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.statusSummaryContentPanel);
            this.panel1.Controls.Add(this.statusSummaryLoadingLabel);
            this.panel1.Controls.Add(this.panel5);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel1.Location = new System.Drawing.Point(0, 75);
            this.panel1.Margin = new System.Windows.Forms.Padding(0);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(260, 180);
            this.panel1.TabIndex = 1;
            // 
            // statusSummaryContentPanel
            // 
            this.statusSummaryContentPanel.Controls.Add(this.statusSummaryLabel);
            this.statusSummaryContentPanel.Controls.Add(this.statusSummaryPictureBox);
            this.statusSummaryContentPanel.Controls.Add(this.maintenanceModeServersPictureBox);
            this.statusSummaryContentPanel.Controls.Add(this.okServersLabel);
            this.statusSummaryContentPanel.Controls.Add(this.okServersPictureBox);
            this.statusSummaryContentPanel.Controls.Add(this.maintenanceModeServersLabel);
            this.statusSummaryContentPanel.Controls.Add(this.warningServersPictureBox);
            this.statusSummaryContentPanel.Controls.Add(this.statusSummaryDescriptionLabel);
            this.statusSummaryContentPanel.Controls.Add(this.criticalServersLabel);
            this.statusSummaryContentPanel.Controls.Add(this.warningServersLabel);
            this.statusSummaryContentPanel.Controls.Add(this.criticalServersPictureBox);
            this.statusSummaryContentPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.statusSummaryContentPanel.Location = new System.Drawing.Point(0, 27);
            this.statusSummaryContentPanel.Name = "statusSummaryContentPanel";
            this.statusSummaryContentPanel.Size = new System.Drawing.Size(260, 153);
            this.statusSummaryContentPanel.TabIndex = 46;
            // 
            // statusSummaryLabel
            // 
            this.statusSummaryLabel.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
            | System.Windows.Forms.AnchorStyles.Right)));
            this.statusSummaryLabel.AutoEllipsis = true;
            this.statusSummaryLabel.AutoSize = true;
            this.statusSummaryLabel.DisabledLinkColor = System.Drawing.Color.FromArgb(((int)(((byte)(85)))), ((int)(((byte)(85)))), ((int)(((byte)(85)))));
            this.statusSummaryLabel.Font = new System.Drawing.Font("Arial", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.statusSummaryLabel.LinkBehavior = System.Windows.Forms.LinkBehavior.HoverUnderline;
            this.statusSummaryLabel.LinkColor = System.Drawing.SystemColors.ControlText;
            this.statusSummaryLabel.Location = new System.Drawing.Point(69, 11);
            this.statusSummaryLabel.Name = "statusSummaryLabel";
            this.statusSummaryLabel.Size = new System.Drawing.Size(98, 24);
            this.statusSummaryLabel.TabIndex = 29;
            this.statusSummaryLabel.TabStop = true;
            this.statusSummaryLabel.Text = "<summary>";
            this.statusSummaryLabel.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.statusSummaryLabel.UseCompatibleTextRendering = true;
            this.statusSummaryLabel.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.ServersLabel_LinkClicked);
            // 
            // statusSummaryPictureBox
            // 
            this.statusSummaryPictureBox.Image = global::Idera.SQLdm.DesktopClient.Properties.Resources.StatusSummaryInformationLarge;
            this.statusSummaryPictureBox.Location = new System.Drawing.Point(13, 6);
            this.statusSummaryPictureBox.Name = "statusSummaryPictureBox";
            this.statusSummaryPictureBox.Size = new System.Drawing.Size(48, 48);
            this.statusSummaryPictureBox.TabIndex = 18;
            this.statusSummaryPictureBox.TabStop = false;
            // 
            // maintenanceModeServersPictureBox
            // 
            this.maintenanceModeServersPictureBox.Image = global::Idera.SQLdm.DesktopClient.Properties.Resources.MaintenanceMode16x16;
            this.maintenanceModeServersPictureBox.Location = new System.Drawing.Point(29, 127);
            this.maintenanceModeServersPictureBox.Name = "maintenanceModeServersPictureBox";
            this.maintenanceModeServersPictureBox.Size = new System.Drawing.Size(16, 16);
            this.maintenanceModeServersPictureBox.TabIndex = 23;
            this.maintenanceModeServersPictureBox.TabStop = false;
            // 
            // okServersLabel
            // 
            this.okServersLabel.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
            | System.Windows.Forms.AnchorStyles.Right)));
            this.okServersLabel.AutoEllipsis = true;
            this.okServersLabel.AutoSize = true;
            this.okServersLabel.Font = new System.Drawing.Font("Arial", 9F);
            this.okServersLabel.LinkBehavior = System.Windows.Forms.LinkBehavior.HoverUnderline;
            this.okServersLabel.LinkColor = System.Drawing.SystemColors.ControlText;
            this.okServersLabel.Location = new System.Drawing.Point(50, 107);
            this.okServersLabel.Name = "okServersLabel";
            this.okServersLabel.Size = new System.Drawing.Size(137, 15);
            this.okServersLabel.TabIndex = 28;
            this.okServersLabel.TabStop = true;
            this.okServersLabel.Text = "{0} server(s) in OK state";
            this.okServersLabel.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.ServersLabel_LinkClicked);
            // 
            // okServersPictureBox
            // 
            this.okServersPictureBox.Image = global::Idera.SQLdm.DesktopClient.Properties.Resources.StatusOKSmall;
            this.okServersPictureBox.Location = new System.Drawing.Point(29, 107);
            this.okServersPictureBox.Name = "okServersPictureBox";
            this.okServersPictureBox.Size = new System.Drawing.Size(16, 16);
            this.okServersPictureBox.TabIndex = 24;
            this.okServersPictureBox.TabStop = false;
            // 
            // maintenanceModeServersLabel
            // 
            this.maintenanceModeServersLabel.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
            | System.Windows.Forms.AnchorStyles.Right)));
            this.maintenanceModeServersLabel.AutoEllipsis = true;
            this.maintenanceModeServersLabel.AutoSize = true;
            this.maintenanceModeServersLabel.Font = new System.Drawing.Font("Arial", 9F);
            this.maintenanceModeServersLabel.LinkBehavior = System.Windows.Forms.LinkBehavior.HoverUnderline;
            this.maintenanceModeServersLabel.LinkColor = System.Drawing.SystemColors.ControlText;
            this.maintenanceModeServersLabel.Location = new System.Drawing.Point(50, 127);
            this.maintenanceModeServersLabel.Name = "maintenanceModeServersLabel";
            this.maintenanceModeServersLabel.Size = new System.Drawing.Size(193, 15);
            this.maintenanceModeServersLabel.TabIndex = 27;
            this.maintenanceModeServersLabel.TabStop = true;
            this.maintenanceModeServersLabel.Text = "{0} server(s) in Maintenance Mode";
            this.maintenanceModeServersLabel.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.ServersLabel_LinkClicked);
            // 
            // warningServersPictureBox
            // 
            this.warningServersPictureBox.Image = global::Idera.SQLdm.DesktopClient.Properties.Resources.StatusWarningSmall;
            this.warningServersPictureBox.Location = new System.Drawing.Point(29, 87);
            this.warningServersPictureBox.Name = "warningServersPictureBox";
            this.warningServersPictureBox.Size = new System.Drawing.Size(16, 16);
            this.warningServersPictureBox.TabIndex = 22;
            this.warningServersPictureBox.TabStop = false;
            // 
            // statusSummaryDescriptionLabel
            // 
            this.statusSummaryDescriptionLabel.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
            | System.Windows.Forms.AnchorStyles.Right)));
            this.statusSummaryDescriptionLabel.AutoEllipsis = true;
            this.statusSummaryDescriptionLabel.AutoSize = true;
            this.statusSummaryDescriptionLabel.DisabledLinkColor = System.Drawing.Color.FromArgb(((int)(((byte)(85)))), ((int)(((byte)(85)))), ((int)(((byte)(85)))));
            this.statusSummaryDescriptionLabel.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.statusSummaryDescriptionLabel.LinkBehavior = System.Windows.Forms.LinkBehavior.HoverUnderline;
            this.statusSummaryDescriptionLabel.LinkColor = System.Drawing.SystemColors.ControlText;
            this.statusSummaryDescriptionLabel.Location = new System.Drawing.Point(71, 36);
            this.statusSummaryDescriptionLabel.Name = "statusSummaryDescriptionLabel";
            this.statusSummaryDescriptionLabel.Size = new System.Drawing.Size(82, 15);
            this.statusSummaryDescriptionLabel.TabIndex = 20;
            this.statusSummaryDescriptionLabel.TabStop = true;
            this.statusSummaryDescriptionLabel.Text = "<description>";
            this.statusSummaryDescriptionLabel.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.ServersLabel_LinkClicked);
            // 
            // criticalServersLabel
            // 
            this.criticalServersLabel.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
            | System.Windows.Forms.AnchorStyles.Right)));
            this.criticalServersLabel.AutoEllipsis = true;
            this.criticalServersLabel.AutoSize = true;
            this.criticalServersLabel.Font = new System.Drawing.Font("Arial", 9F);
            this.criticalServersLabel.LinkBehavior = System.Windows.Forms.LinkBehavior.HoverUnderline;
            this.criticalServersLabel.LinkColor = System.Drawing.SystemColors.ControlText;
            this.criticalServersLabel.Location = new System.Drawing.Point(50, 67);
            this.criticalServersLabel.Name = "criticalServersLabel";
            this.criticalServersLabel.Size = new System.Drawing.Size(158, 15);
            this.criticalServersLabel.TabIndex = 25;
            this.criticalServersLabel.TabStop = true;
            this.criticalServersLabel.Text = "{0} server(s) in Critical state";
            this.criticalServersLabel.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.ServersLabel_LinkClicked);
            // 
            // warningServersLabel
            // 
            this.warningServersLabel.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
            | System.Windows.Forms.AnchorStyles.Right)));
            this.warningServersLabel.AutoEllipsis = true;
            this.warningServersLabel.AutoSize = true;
            this.warningServersLabel.Font = new System.Drawing.Font("Arial", 9F);
            this.warningServersLabel.LinkBehavior = System.Windows.Forms.LinkBehavior.HoverUnderline;
            this.warningServersLabel.LinkColor = System.Drawing.SystemColors.ControlText;
            this.warningServersLabel.Location = new System.Drawing.Point(50, 87);
            this.warningServersLabel.Name = "warningServersLabel";
            this.warningServersLabel.Size = new System.Drawing.Size(166, 15);
            this.warningServersLabel.TabIndex = 26;
            this.warningServersLabel.TabStop = true;
            this.warningServersLabel.Text = "{0} server(s) in Warning state";
            this.warningServersLabel.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.ServersLabel_LinkClicked);
            // 
            // criticalServersPictureBox
            // 
            this.criticalServersPictureBox.Image = global::Idera.SQLdm.DesktopClient.Properties.Resources.StatusCriticalSmall;
            this.criticalServersPictureBox.Location = new System.Drawing.Point(29, 67);
            this.criticalServersPictureBox.Name = "criticalServersPictureBox";
            this.criticalServersPictureBox.Size = new System.Drawing.Size(16, 16);
            this.criticalServersPictureBox.TabIndex = 21;
            this.criticalServersPictureBox.TabStop = false;
            // 
            // statusSummaryLoadingLabel
            // 
            this.statusSummaryLoadingLabel.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
            | System.Windows.Forms.AnchorStyles.Right)));
            this.statusSummaryLoadingLabel.AutoEllipsis = true;
            this.statusSummaryLoadingLabel.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.statusSummaryLoadingLabel.Location = new System.Drawing.Point(32, 38);
            this.statusSummaryLoadingLabel.Name = "statusSummaryLoadingLabel";
            this.statusSummaryLoadingLabel.Size = new System.Drawing.Size(246, 17);
            this.statusSummaryLoadingLabel.TabIndex = 5;
            this.statusSummaryLoadingLabel.Text = Idera.SQLdm.Common.Constants.LOADING;
            this.statusSummaryLoadingLabel.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            // 
            // panel5
            // 
            this.panel5.Controls.Add(this.label2);
            this.panel5.Controls.Add(this.label3);
            this.panel5.Dock = System.Windows.Forms.DockStyle.Top;
            this.panel5.Location = new System.Drawing.Point(0, 0);
            this.panel5.Name = "panel5";
            this.panel5.Size = new System.Drawing.Size(260, 27);
            this.panel5.TabIndex = 2;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Arial", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label2.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(102)))), ((int)(((byte)(112)))), ((int)(((byte)(131)))));
            this.label2.Location = new System.Drawing.Point(4, 4);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(118, 17);
            this.label2.TabIndex = 0;
            this.label2.Text = "Status Summary";
            // 
            // label3
            // 
            this.label3.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
            | System.Windows.Forms.AnchorStyles.Right)));
            this.label3.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(137)))), ((int)(((byte)(137)))), ((int)(((byte)(137)))));
            this.label3.Location = new System.Drawing.Point(128, 13);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(180, 1);
            this.label3.TabIndex = 1;
            // 
            // panel3
            // 
            this.panel3.Controls.Add(this._contentTabControl);
            this.panel3.Controls.Add(this.label1);
            this.panel3.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel3.Location = new System.Drawing.Point(270, 75);
            this.panel3.Margin = new System.Windows.Forms.Padding(0);
            this.panel3.Name = "panel3";
            this.tableLayoutPanel1.SetRowSpan(this.panel3, 2);
            this.panel3.Size = new System.Drawing.Size(581, 530);
            this.panel3.TabIndex = 3;
            // 
            // _contentTabControl
            // 
            appearance1.BackColor = System.Drawing.Color.White;
            appearance1.BackColor2 = System.Drawing.Color.White;
            appearance1.BorderColor = System.Drawing.Color.White;
            appearance1.FontData.BoldAsString = "True";
            appearance1.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(49)))), ((int)(((byte)(54)))), ((int)(((byte)(66)))));
            this._contentTabControl.ActiveTabAppearance = appearance1;
            appearance2.BackColor = System.Drawing.Color.WhiteSmoke;
            appearance2.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(210)))), ((int)(((byte)(210)))), ((int)(((byte)(210)))));
            appearance2.FontData.Name = "Arial";
            appearance2.FontData.SizeInPoints = 11.25F;
            appearance2.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(102)))), ((int)(((byte)(112)))), ((int)(((byte)(131)))));
            this._contentTabControl.Appearance = appearance2;
            appearance3.BackColor = System.Drawing.Color.White;
            appearance3.BorderColor = System.Drawing.Color.White;
            this._contentTabControl.ClientAreaAppearance = appearance3;
            this._contentTabControl.Controls.Add(this.ultraTabSharedControlsPage1);
            this._contentTabControl.Controls.Add(this.ultraTabPageControl1);
            this._contentTabControl.Controls.Add(this.ultraTabPageControl2);
            this._contentTabControl.Controls.Add(this.ultraTabPageControl4);
            this._contentTabControl.Controls.Add(this.ultraTabPageControl3);
            this._contentTabControl.Dock = System.Windows.Forms.DockStyle.Fill;
            appearance4.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(255)))), ((int)(((byte)(254)))));
            appearance4.BackColor2 = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(236)))), ((int)(((byte)(182)))));
            appearance4.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(137)))), ((int)(((byte)(137)))), ((int)(((byte)(137)))));
            appearance4.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(49)))), ((int)(((byte)(54)))), ((int)(((byte)(66)))));
            this._contentTabControl.HotTrackAppearance = appearance4;
            this._contentTabControl.InterTabSpacing = new Infragistics.Win.DefaultableInteger(0);
            this._contentTabControl.Location = new System.Drawing.Point(1, 0);
            this._contentTabControl.Name = "_contentTabControl";
            this._contentTabControl.SharedControlsPage = this.ultraTabSharedControlsPage1;
            this._contentTabControl.Size = new System.Drawing.Size(580, 530);
            this._contentTabControl.Style = Infragistics.Win.UltraWinTabControl.UltraTabControlStyle.Flat;
            this._contentTabControl.TabButtonStyle = Infragistics.Win.UIElementButtonStyle.Flat;
            appearance39.BackColor = Settings.Default.ColorScheme == "Dark" ? ColorTranslator.FromHtml(DarkThemeColorConstants.BackColor) : System.Drawing.Color.White;
            this._contentTabControl.TabHeaderAreaAppearance = appearance39;
            this._contentTabControl.TabIndex = 7;
            this._contentTabControl.TabLayoutStyle = Infragistics.Win.UltraWinTabs.TabLayoutStyle.SingleRowFixed;
            ultraTab4.TabPage = this.ultraTabPageControl1;
            ultraTab4.Text = "Get Started";
            ultraTab5.TabPage = this.ultraTabPageControl2;
            ultraTab5.Text = "Active Alerts";
            ultraTab7.TabPage = this.ultraTabPageControl4;
            ultraTab7.Text = "Web Console";
            ultraTab6.TabPage = this.ultraTabPageControl3;
            ultraTab6.Text = "Newsfeed";
            this._contentTabControl.Tabs.AddRange(new Infragistics.Win.UltraWinTabControl.UltraTab[] {
            ultraTab4,
            ultraTab5,
            ultraTab7,
            ultraTab6});
            this._contentTabControl.TabSize = new System.Drawing.Size(0, 25);
            this._contentTabControl.UseAppStyling = false;
            this._contentTabControl.UseFlatMode = Infragistics.Win.DefaultableBoolean.True;
            this._contentTabControl.UseHotTracking = Infragistics.Win.DefaultableBoolean.True;
            this._contentTabControl.UseOsThemes = Infragistics.Win.DefaultableBoolean.False;
            // 
            // ultraTabSharedControlsPage1
            // 
            this.ultraTabSharedControlsPage1.Location = new System.Drawing.Point(-10000, -10000);
            this.ultraTabSharedControlsPage1.Name = "ultraTabSharedControlsPage1";
            this.ultraTabSharedControlsPage1.Size = new System.Drawing.Size(578, 503);
            // 
            // label1
            // 
            this.label1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(137)))), ((int)(((byte)(137)))), ((int)(((byte)(137)))));
            this.label1.Dock = System.Windows.Forms.DockStyle.Left;
            this.label1.Location = new System.Drawing.Point(0, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(1, 530);
            this.label1.TabIndex = 0;
            // 
            // panel2
            // 
            this.panel2.Controls.Add(this._recentServersPanel);
            this.panel2.Controls.Add(this._mostCriticalServersPanel);
            this.panel2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel2.Location = new System.Drawing.Point(0, 255);
            this.panel2.Margin = new System.Windows.Forms.Padding(0);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(260, 350);
            this.panel2.TabIndex = 10;
            // 
            // _recentServersPanel
            // 
            this._recentServersPanel.Controls.Add(this._recentServersStatusImage5);
            this._recentServersPanel.Controls.Add(this._recentServersLinkLabel5);
            this._recentServersPanel.Controls.Add(this._recentServersStatusImage4);
            this._recentServersPanel.Controls.Add(this._recentServersLinkLabel4);
            this._recentServersPanel.Controls.Add(this._recentServersStatusImage3);
            this._recentServersPanel.Controls.Add(this._recentServersLinkLabel3);
            this._recentServersPanel.Controls.Add(this._recentServersStatusImage2);
            this._recentServersPanel.Controls.Add(this._recentServersLinkLabel2);
            this._recentServersPanel.Controls.Add(this._recentServersStatusImage1);
            this._recentServersPanel.Controls.Add(this._recentServersLinkLabel1);
            this._recentServersPanel.Controls.Add(this.label6);
            this._recentServersPanel.Controls.Add(this.label7);
            this._recentServersPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this._recentServersPanel.Location = new System.Drawing.Point(0, 160);
            this._recentServersPanel.Margin = new System.Windows.Forms.Padding(0);
            this._recentServersPanel.Name = "_recentServersPanel";
            this._recentServersPanel.Size = new System.Drawing.Size(260, 190);
            this._recentServersPanel.TabIndex = 4;
            // 
            // _recentServersStatusImage5
            // 
            this._recentServersStatusImage5.Image = global::Idera.SQLdm.DesktopClient.Properties.Resources.StatusCriticalSmall;
            this._recentServersStatusImage5.Location = new System.Drawing.Point(28, 131);
            this._recentServersStatusImage5.Name = "_recentServersStatusImage5";
            this._recentServersStatusImage5.Size = new System.Drawing.Size(16, 16);
            this._recentServersStatusImage5.TabIndex = 34;
            this._recentServersStatusImage5.TabStop = false;
            // 
            // _recentServersLinkLabel5
            // 
            this._recentServersLinkLabel5.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
            | System.Windows.Forms.AnchorStyles.Right)));
            this._recentServersLinkLabel5.AutoEllipsis = true;
            this._recentServersLinkLabel5.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this._recentServersLinkLabel5.LinkBehavior = System.Windows.Forms.LinkBehavior.HoverUnderline;
            this._recentServersLinkLabel5.LinkColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(102)))), ((int)(((byte)(204)))));
            this._recentServersLinkLabel5.Location = new System.Drawing.Point(49, 131);
            this._recentServersLinkLabel5.Name = "_recentServersLinkLabel5";
            this._recentServersLinkLabel5.Size = new System.Drawing.Size(248, 15);
            this._recentServersLinkLabel5.TabIndex = 35;
            this._recentServersLinkLabel5.TabStop = true;
            this._recentServersLinkLabel5.Text = "< Server 5 >";
            this._recentServersLinkLabel5.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.ServerLabel_LinkClicked);
            // 
            // _recentServersStatusImage4
            // 
            this._recentServersStatusImage4.Image = global::Idera.SQLdm.DesktopClient.Properties.Resources.StatusCriticalSmall;
            this._recentServersStatusImage4.Location = new System.Drawing.Point(28, 107);
            this._recentServersStatusImage4.Name = "_recentServersStatusImage4";
            this._recentServersStatusImage4.Size = new System.Drawing.Size(16, 16);
            this._recentServersStatusImage4.TabIndex = 32;
            this._recentServersStatusImage4.TabStop = false;
            // 
            // _recentServersLinkLabel4
            // 
            this._recentServersLinkLabel4.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
            | System.Windows.Forms.AnchorStyles.Right)));
            this._recentServersLinkLabel4.AutoEllipsis = true;
            this._recentServersLinkLabel4.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this._recentServersLinkLabel4.LinkBehavior = System.Windows.Forms.LinkBehavior.HoverUnderline;
            this._recentServersLinkLabel4.LinkColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(102)))), ((int)(((byte)(204)))));
            this._recentServersLinkLabel4.Location = new System.Drawing.Point(49, 107);
            this._recentServersLinkLabel4.Name = "_recentServersLinkLabel4";
            this._recentServersLinkLabel4.Size = new System.Drawing.Size(248, 15);
            this._recentServersLinkLabel4.TabIndex = 33;
            this._recentServersLinkLabel4.TabStop = true;
            this._recentServersLinkLabel4.Text = "< Server 4 >";
            this._recentServersLinkLabel4.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.ServerLabel_LinkClicked);
            // 
            // _recentServersStatusImage3
            // 
            this._recentServersStatusImage3.Image = global::Idera.SQLdm.DesktopClient.Properties.Resources.StatusCriticalSmall;
            this._recentServersStatusImage3.Location = new System.Drawing.Point(28, 83);
            this._recentServersStatusImage3.Name = "_recentServersStatusImage3";
            this._recentServersStatusImage3.Size = new System.Drawing.Size(16, 16);
            this._recentServersStatusImage3.TabIndex = 30;
            this._recentServersStatusImage3.TabStop = false;
            // 
            // _recentServersLinkLabel3
            // 
            this._recentServersLinkLabel3.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
            | System.Windows.Forms.AnchorStyles.Right)));
            this._recentServersLinkLabel3.AutoEllipsis = true;
            this._recentServersLinkLabel3.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this._recentServersLinkLabel3.LinkBehavior = System.Windows.Forms.LinkBehavior.HoverUnderline;
            this._recentServersLinkLabel3.LinkColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(102)))), ((int)(((byte)(204)))));
            this._recentServersLinkLabel3.Location = new System.Drawing.Point(49, 83);
            this._recentServersLinkLabel3.Name = "_recentServersLinkLabel3";
            this._recentServersLinkLabel3.Size = new System.Drawing.Size(248, 15);
            this._recentServersLinkLabel3.TabIndex = 31;
            this._recentServersLinkLabel3.TabStop = true;
            this._recentServersLinkLabel3.Text = "< Server 3 >";
            this._recentServersLinkLabel3.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.ServerLabel_LinkClicked);
            // 
            // _recentServersStatusImage2
            // 
            this._recentServersStatusImage2.Image = global::Idera.SQLdm.DesktopClient.Properties.Resources.StatusCriticalSmall;
            this._recentServersStatusImage2.Location = new System.Drawing.Point(28, 59);
            this._recentServersStatusImage2.Name = "_recentServersStatusImage2";
            this._recentServersStatusImage2.Size = new System.Drawing.Size(16, 16);
            this._recentServersStatusImage2.TabIndex = 28;
            this._recentServersStatusImage2.TabStop = false;
            // 
            // _recentServersLinkLabel2
            // 
            this._recentServersLinkLabel2.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
            | System.Windows.Forms.AnchorStyles.Right)));
            this._recentServersLinkLabel2.AutoEllipsis = true;
            this._recentServersLinkLabel2.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this._recentServersLinkLabel2.LinkBehavior = System.Windows.Forms.LinkBehavior.HoverUnderline;
            this._recentServersLinkLabel2.LinkColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(102)))), ((int)(((byte)(204)))));
            this._recentServersLinkLabel2.Location = new System.Drawing.Point(49, 59);
            this._recentServersLinkLabel2.Name = "_recentServersLinkLabel2";
            this._recentServersLinkLabel2.Size = new System.Drawing.Size(248, 15);
            this._recentServersLinkLabel2.TabIndex = 29;
            this._recentServersLinkLabel2.TabStop = true;
            this._recentServersLinkLabel2.Text = "< Server 2 >";
            this._recentServersLinkLabel2.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.ServerLabel_LinkClicked);
            // 
            // _recentServersStatusImage1
            // 
            this._recentServersStatusImage1.Image = global::Idera.SQLdm.DesktopClient.Properties.Resources.StatusCriticalSmall;
            this._recentServersStatusImage1.Location = new System.Drawing.Point(28, 35);
            this._recentServersStatusImage1.Name = "_recentServersStatusImage1";
            this._recentServersStatusImage1.Size = new System.Drawing.Size(16, 16);
            this._recentServersStatusImage1.TabIndex = 26;
            this._recentServersStatusImage1.TabStop = false;
            // 
            // _recentServersLinkLabel1
            // 
            this._recentServersLinkLabel1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
            | System.Windows.Forms.AnchorStyles.Right)));
            this._recentServersLinkLabel1.AutoEllipsis = true;
            this._recentServersLinkLabel1.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this._recentServersLinkLabel1.LinkBehavior = System.Windows.Forms.LinkBehavior.HoverUnderline;
            this._recentServersLinkLabel1.LinkColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(102)))), ((int)(((byte)(204)))));
            this._recentServersLinkLabel1.Location = new System.Drawing.Point(49, 35);
            this._recentServersLinkLabel1.Name = "_recentServersLinkLabel1";
            this._recentServersLinkLabel1.Size = new System.Drawing.Size(248, 15);
            this._recentServersLinkLabel1.TabIndex = 27;
            this._recentServersLinkLabel1.TabStop = true;
            this._recentServersLinkLabel1.Text = "< Server 1 >";
            this._recentServersLinkLabel1.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.ServerLabel_LinkClicked);
            // 
            // label6
            // 
            this.label6.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
            | System.Windows.Forms.AnchorStyles.Right)));
            this.label6.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(137)))), ((int)(((byte)(137)))), ((int)(((byte)(137)))));
            this.label6.Location = new System.Drawing.Point(118, 14);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(190, 1);
            this.label6.TabIndex = 1;
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Font = new System.Drawing.Font("Arial", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label7.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(102)))), ((int)(((byte)(112)))), ((int)(((byte)(131)))));
            this.label7.Location = new System.Drawing.Point(4, 5);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(110, 17);
            this.label7.TabIndex = 0;
            this.label7.Text = "Recent Servers";
            // 
            // _mostCriticalServersPanel
            // 
            this._mostCriticalServersPanel.Controls.Add(this._mostCriticalServersStatusImage5);
            this._mostCriticalServersPanel.Controls.Add(this._mostCriticalServersLinkLabel5);
            this._mostCriticalServersPanel.Controls.Add(this._mostCriticalServersStatusImage4);
            this._mostCriticalServersPanel.Controls.Add(this._mostCriticalServersLinkLabel4);
            this._mostCriticalServersPanel.Controls.Add(this._mostCriticalServersStatusImage3);
            this._mostCriticalServersPanel.Controls.Add(this._mostCriticalServersLinkLabel3);
            this._mostCriticalServersPanel.Controls.Add(this._mostCriticalServersStatusImage2);
            this._mostCriticalServersPanel.Controls.Add(this._mostCriticalServersLinkLabel2);
            this._mostCriticalServersPanel.Controls.Add(this._mostCriticalServersStatusImage1);
            this._mostCriticalServersPanel.Controls.Add(this._mostCriticalServersLinkLabel1);
            this._mostCriticalServersPanel.Controls.Add(this.label4);
            this._mostCriticalServersPanel.Controls.Add(this.label5);
            this._mostCriticalServersPanel.Dock = System.Windows.Forms.DockStyle.Top;
            this._mostCriticalServersPanel.Location = new System.Drawing.Point(0, 0);
            this._mostCriticalServersPanel.Margin = new System.Windows.Forms.Padding(0);
            this._mostCriticalServersPanel.Name = "_mostCriticalServersPanel";
            this._mostCriticalServersPanel.Size = new System.Drawing.Size(260, 160);
            this._mostCriticalServersPanel.TabIndex = 2;
            // 
            // _mostCriticalServersStatusImage5
            // 
            this._mostCriticalServersStatusImage5.Image = global::Idera.SQLdm.DesktopClient.Properties.Resources.StatusCriticalSmall;
            this._mostCriticalServersStatusImage5.Location = new System.Drawing.Point(28, 124);
            this._mostCriticalServersStatusImage5.Name = "_mostCriticalServersStatusImage5";
            this._mostCriticalServersStatusImage5.Size = new System.Drawing.Size(16, 16);
            this._mostCriticalServersStatusImage5.TabIndex = 34;
            this._mostCriticalServersStatusImage5.TabStop = false;
            // 
            // _mostCriticalServersLinkLabel5
            // 
            this._mostCriticalServersLinkLabel5.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
            | System.Windows.Forms.AnchorStyles.Right)));
            this._mostCriticalServersLinkLabel5.AutoEllipsis = true;
            this._mostCriticalServersLinkLabel5.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this._mostCriticalServersLinkLabel5.LinkBehavior = System.Windows.Forms.LinkBehavior.HoverUnderline;
            this._mostCriticalServersLinkLabel5.LinkColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(102)))), ((int)(((byte)(204)))));
            this._mostCriticalServersLinkLabel5.Location = new System.Drawing.Point(49, 124);
            this._mostCriticalServersLinkLabel5.Name = "_mostCriticalServersLinkLabel5";
            this._mostCriticalServersLinkLabel5.Size = new System.Drawing.Size(248, 15);
            this._mostCriticalServersLinkLabel5.TabIndex = 35;
            this._mostCriticalServersLinkLabel5.TabStop = true;
            this._mostCriticalServersLinkLabel5.Text = "< Server 5 >";
            this._mostCriticalServersLinkLabel5.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.ServerLabel_LinkClicked);
            // 
            // _mostCriticalServersStatusImage4
            // 
            this._mostCriticalServersStatusImage4.Image = global::Idera.SQLdm.DesktopClient.Properties.Resources.StatusCriticalSmall;
            this._mostCriticalServersStatusImage4.Location = new System.Drawing.Point(28, 100);
            this._mostCriticalServersStatusImage4.Name = "_mostCriticalServersStatusImage4";
            this._mostCriticalServersStatusImage4.Size = new System.Drawing.Size(16, 16);
            this._mostCriticalServersStatusImage4.TabIndex = 32;
            this._mostCriticalServersStatusImage4.TabStop = false;
            // 
            // _mostCriticalServersLinkLabel4
            // 
            this._mostCriticalServersLinkLabel4.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
            | System.Windows.Forms.AnchorStyles.Right)));
            this._mostCriticalServersLinkLabel4.AutoEllipsis = true;
            this._mostCriticalServersLinkLabel4.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this._mostCriticalServersLinkLabel4.LinkBehavior = System.Windows.Forms.LinkBehavior.HoverUnderline;
            this._mostCriticalServersLinkLabel4.LinkColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(102)))), ((int)(((byte)(204)))));
            this._mostCriticalServersLinkLabel4.Location = new System.Drawing.Point(49, 100);
            this._mostCriticalServersLinkLabel4.Name = "_mostCriticalServersLinkLabel4";
            this._mostCriticalServersLinkLabel4.Size = new System.Drawing.Size(248, 15);
            this._mostCriticalServersLinkLabel4.TabIndex = 33;
            this._mostCriticalServersLinkLabel4.TabStop = true;
            this._mostCriticalServersLinkLabel4.Text = "< Server 4 >";
            this._mostCriticalServersLinkLabel4.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.ServerLabel_LinkClicked);
            // 
            // _mostCriticalServersStatusImage3
            // 
            this._mostCriticalServersStatusImage3.Image = global::Idera.SQLdm.DesktopClient.Properties.Resources.StatusCriticalSmall;
            this._mostCriticalServersStatusImage3.Location = new System.Drawing.Point(28, 76);
            this._mostCriticalServersStatusImage3.Name = "_mostCriticalServersStatusImage3";
            this._mostCriticalServersStatusImage3.Size = new System.Drawing.Size(16, 16);
            this._mostCriticalServersStatusImage3.TabIndex = 30;
            this._mostCriticalServersStatusImage3.TabStop = false;
            // 
            // _mostCriticalServersLinkLabel3
            // 
            this._mostCriticalServersLinkLabel3.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
            | System.Windows.Forms.AnchorStyles.Right)));
            this._mostCriticalServersLinkLabel3.AutoEllipsis = true;
            this._mostCriticalServersLinkLabel3.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this._mostCriticalServersLinkLabel3.LinkBehavior = System.Windows.Forms.LinkBehavior.HoverUnderline;
            this._mostCriticalServersLinkLabel3.LinkColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(102)))), ((int)(((byte)(204)))));
            this._mostCriticalServersLinkLabel3.Location = new System.Drawing.Point(49, 76);
            this._mostCriticalServersLinkLabel3.Name = "_mostCriticalServersLinkLabel3";
            this._mostCriticalServersLinkLabel3.Size = new System.Drawing.Size(248, 15);
            this._mostCriticalServersLinkLabel3.TabIndex = 31;
            this._mostCriticalServersLinkLabel3.TabStop = true;
            this._mostCriticalServersLinkLabel3.Text = "< Server 3 >";
            this._mostCriticalServersLinkLabel3.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.ServerLabel_LinkClicked);
            // 
            // _mostCriticalServersStatusImage2
            // 
            this._mostCriticalServersStatusImage2.Image = global::Idera.SQLdm.DesktopClient.Properties.Resources.StatusCriticalSmall;
            this._mostCriticalServersStatusImage2.Location = new System.Drawing.Point(28, 52);
            this._mostCriticalServersStatusImage2.Name = "_mostCriticalServersStatusImage2";
            this._mostCriticalServersStatusImage2.Size = new System.Drawing.Size(16, 16);
            this._mostCriticalServersStatusImage2.TabIndex = 28;
            this._mostCriticalServersStatusImage2.TabStop = false;
            // 
            // _mostCriticalServersLinkLabel2
            // 
            this._mostCriticalServersLinkLabel2.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
            | System.Windows.Forms.AnchorStyles.Right)));
            this._mostCriticalServersLinkLabel2.AutoEllipsis = true;
            this._mostCriticalServersLinkLabel2.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this._mostCriticalServersLinkLabel2.LinkBehavior = System.Windows.Forms.LinkBehavior.HoverUnderline;
            this._mostCriticalServersLinkLabel2.LinkColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(102)))), ((int)(((byte)(204)))));
            this._mostCriticalServersLinkLabel2.Location = new System.Drawing.Point(49, 52);
            this._mostCriticalServersLinkLabel2.Name = "_mostCriticalServersLinkLabel2";
            this._mostCriticalServersLinkLabel2.Size = new System.Drawing.Size(248, 15);
            this._mostCriticalServersLinkLabel2.TabIndex = 29;
            this._mostCriticalServersLinkLabel2.TabStop = true;
            this._mostCriticalServersLinkLabel2.Text = "< Server 2 >";
            this._mostCriticalServersLinkLabel2.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.ServerLabel_LinkClicked);
            // 
            // _mostCriticalServersStatusImage1
            // 
            this._mostCriticalServersStatusImage1.Image = global::Idera.SQLdm.DesktopClient.Properties.Resources.StatusCriticalSmall;
            this._mostCriticalServersStatusImage1.Location = new System.Drawing.Point(28, 28);
            this._mostCriticalServersStatusImage1.Name = "_mostCriticalServersStatusImage1";
            this._mostCriticalServersStatusImage1.Size = new System.Drawing.Size(16, 16);
            this._mostCriticalServersStatusImage1.TabIndex = 26;
            this._mostCriticalServersStatusImage1.TabStop = false;
            // 
            // _mostCriticalServersLinkLabel1
            // 
            this._mostCriticalServersLinkLabel1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
            | System.Windows.Forms.AnchorStyles.Right)));
            this._mostCriticalServersLinkLabel1.AutoEllipsis = true;
            this._mostCriticalServersLinkLabel1.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this._mostCriticalServersLinkLabel1.LinkBehavior = System.Windows.Forms.LinkBehavior.HoverUnderline;
            this._mostCriticalServersLinkLabel1.LinkColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(102)))), ((int)(((byte)(204)))));
            this._mostCriticalServersLinkLabel1.Location = new System.Drawing.Point(49, 28);
            this._mostCriticalServersLinkLabel1.Name = "_mostCriticalServersLinkLabel1";
            this._mostCriticalServersLinkLabel1.Size = new System.Drawing.Size(248, 15);
            this._mostCriticalServersLinkLabel1.TabIndex = 27;
            this._mostCriticalServersLinkLabel1.TabStop = true;
            this._mostCriticalServersLinkLabel1.Text = "< Server 1 >";
            this._mostCriticalServersLinkLabel1.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.ServerLabel_LinkClicked);
            // 
            // label4
            // 
            this.label4.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
            | System.Windows.Forms.AnchorStyles.Right)));
            this.label4.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(137)))), ((int)(((byte)(137)))), ((int)(((byte)(137)))));
            this.label4.Location = new System.Drawing.Point(153, 14);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(155, 1);
            this.label4.TabIndex = 1;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Font = new System.Drawing.Font("Arial", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label5.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(102)))), ((int)(((byte)(112)))), ((int)(((byte)(131)))));
            this.label5.Location = new System.Drawing.Point(4, 5);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(143, 17);
            this.label5.TabIndex = 0;
            this.label5.Text = "Most Critical Servers";
            // 
            // ultraPrintPreviewDialog
            // 
            this.ultraPrintPreviewDialog.Document = this.ultraGridPrintDocument;
            this.ultraPrintPreviewDialog.Name = "ultraPrintPreviewDialog";
            // 
            // ultraGridPrintDocument
            // 
            this.ultraGridPrintDocument.DocumentName = "";
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
            // saveFileDialog
            // 
            this.saveFileDialog.DefaultExt = "xls";
            this.saveFileDialog.Filter = "Excel Workbook (*.xls)|*.xls";
            this.saveFileDialog.Title = "Save as Excel Spreadsheet";
            // 
            // contentPanel
            // 
            appearance5.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(60)))), ((int)(((byte)(60)))), ((int)(((byte)(60)))));
            this.contentPanel.Appearance = appearance5;
            this.contentPanel.BorderStyle = Infragistics.Win.UIElementBorderStyle.Solid;
            // 
            // contentPanel.ClientArea
            // 
            this.contentPanel.ClientArea.Controls.Add(this.tableLayoutPanel1);
            this.contentPanel.ClientArea.Controls.Add(this.headerStrip);
            this.contentPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.contentPanel.Location = new System.Drawing.Point(0, 0);
            this.contentPanel.Name = "contentPanel";
            this.contentPanel.Size = new System.Drawing.Size(853, 667);
            this.contentPanel.TabIndex = 13;
            // 
            // osUnsupportedLabel
            // 
            this.osUnsupportedLabel.Location = new System.Drawing.Point(0, 0);
            this.osUnsupportedLabel.Name = "osUnsupportedLabel";
            this.osUnsupportedLabel.Size = new System.Drawing.Size(100, 23);
            this.osUnsupportedLabel.TabIndex = 0;
            // 
            // _newsfeedLinkLabelRequirements
            // 
            this._newsfeedLinkLabelRequirements.Location = new System.Drawing.Point(0, 0);
            this._newsfeedLinkLabelRequirements.Name = "_newsfeedLinkLabelRequirements";
            this._newsfeedLinkLabelRequirements.Size = new System.Drawing.Size(100, 23);
            this._newsfeedLinkLabelRequirements.TabIndex = 0;
            // 
            // NewSQLdmTodayView
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.contentPanel);
            this.Name = "NewSQLdmTodayView";
            this.Size = new System.Drawing.Size(853, 667);
            this.Load += new System.EventHandler(this.NewSQLdmTodayView_Load);
            this.ultraTabPageControl1.ResumeLayout(false);
            this.panel7.ResumeLayout(false);
            this.panel7.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox11)).EndInit();
            this.tableLayoutPanel2.ResumeLayout(false);
            this.tableLayoutPanel2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox10)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox3)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox4)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox5)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox8)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox9)).EndInit();
            this._trialCenterPanel.ResumeLayout(false);
            this._trialCenterPanel.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox2)).EndInit();
            this.ultraTabPageControl2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.alertsGrid)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.alertsViewDataSource)).EndInit();
            this.ultraTabPageControl4.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox12)).EndInit();
            this.ultraTabPageControl3.ResumeLayout(false);
            this.panel8.ResumeLayout(false);
            this.panel8.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox7)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox6)).EndInit();
            this.headerStrip.ResumeLayout(false);
            this.headerStrip.PerformLayout();
            this.headerPanel.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            this.tableLayoutPanel1.ResumeLayout(false);
            this.panel1.ResumeLayout(false);
            this.statusSummaryContentPanel.ResumeLayout(false);
            this.statusSummaryContentPanel.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.statusSummaryPictureBox)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.maintenanceModeServersPictureBox)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.okServersPictureBox)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.warningServersPictureBox)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.criticalServersPictureBox)).EndInit();
            this.panel5.ResumeLayout(false);
            this.panel5.PerformLayout();
            this.panel3.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this._contentTabControl)).EndInit();
            this._contentTabControl.ResumeLayout(false);
            this.panel2.ResumeLayout(false);
            this._recentServersPanel.ResumeLayout(false);
            this._recentServersPanel.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this._recentServersStatusImage5)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this._recentServersStatusImage4)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this._recentServersStatusImage3)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this._recentServersStatusImage2)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this._recentServersStatusImage1)).EndInit();
            this._mostCriticalServersPanel.ResumeLayout(false);
            this._mostCriticalServersPanel.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this._mostCriticalServersStatusImage5)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this._mostCriticalServersStatusImage4)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this._mostCriticalServersStatusImage3)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this._mostCriticalServersStatusImage2)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this._mostCriticalServersStatusImage1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.toolbarsManager)).EndInit();
            this.contentPanel.ClientArea.ResumeLayout(false);
            this.contentPanel.ResumeLayout(false);
            this.ResumeLayout(false);
            stopWatch.Stop();
            StartUpTimeLog.DebugFormat("Time taken by NewSQLdmTodayView.InitializeComponent : {0}", stopWatch.ElapsedMilliseconds);
        }

        #endregion

        private Idera.SQLdm.DesktopClient.Controls.HeaderStrip headerStrip;
        private System.Windows.Forms.ToolStripLabel titleLabel;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomPanel  headerPanel;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomLabel refreshTimeLabel;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomLabel refreshDateLabel;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomTableLayoutPanel tableLayoutPanel1;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomLabel label1;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomPanel  panel1;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomLabel label3;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomLabel label2;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomPanel  _mostCriticalServersPanel;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomLabel label4;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomLabel label5;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomLinkLabel statusSummaryLabel;
        private System.Windows.Forms.PictureBox statusSummaryPictureBox;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomLinkLabel okServersLabel;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomLinkLabel maintenanceModeServersLabel;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomLinkLabel statusSummaryDescriptionLabel;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomLinkLabel warningServersLabel;
        private System.Windows.Forms.PictureBox criticalServersPictureBox;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomLinkLabel criticalServersLabel;
        private System.Windows.Forms.PictureBox warningServersPictureBox;
        private System.Windows.Forms.PictureBox okServersPictureBox;
        private System.Windows.Forms.PictureBox maintenanceModeServersPictureBox;
        private System.Windows.Forms.PictureBox _mostCriticalServersStatusImage1;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomLinkLabel _mostCriticalServersLinkLabel1;
        private System.Windows.Forms.PictureBox _mostCriticalServersStatusImage4;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomLinkLabel _mostCriticalServersLinkLabel4;
        private System.Windows.Forms.PictureBox _mostCriticalServersStatusImage3;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomLinkLabel _mostCriticalServersLinkLabel3;
        private System.Windows.Forms.PictureBox _mostCriticalServersStatusImage2;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomLinkLabel _mostCriticalServersLinkLabel2;
        private System.Windows.Forms.PictureBox _mostCriticalServersStatusImage5;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomLinkLabel _mostCriticalServersLinkLabel5;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomPanel  panel3;
        private Infragistics.Win.UltraWinTabControl.UltraTabControl _contentTabControl;
        private Infragistics.Win.UltraWinTabControl.UltraTabSharedControlsPage ultraTabSharedControlsPage1;
        private Infragistics.Win.UltraWinTabControl.UltraTabPageControl ultraTabPageControl1;
        private Infragistics.Win.UltraWinTabControl.UltraTabPageControl ultraTabPageControl2;
        private Infragistics.Win.UltraWinTabControl.UltraTabPageControl ultraTabPageControl4;
        private Infragistics.Win.UltraWinTabControl.UltraTabPageControl ultraTabPageControl3;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomLabel newFeature05;
        private Infragistics.Win.UltraWinGrid.UltraGrid alertsGrid;
        private Idera.SQLdm.DesktopClient.Views.Alerts.AlertsViewDataSource alertsViewDataSource;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomLabel alertsStatusLabel;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomPanel  statusSummaryContentPanel;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomPanel  panel5;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomLabel statusSummaryLoadingLabel;
        private Infragistics.Win.Printing.UltraPrintPreviewDialog ultraPrintPreviewDialog;
        private Infragistics.Win.UltraWinGrid.UltraGridPrintDocument ultraGridPrintDocument;
        private Idera.SQLdm.DesktopClient.Controls.ContextMenuManager toolbarsManager;
        private Infragistics.Win.UltraWinGrid.ExcelExport.UltraGridExcelExporter ultraGridExcelExporter;
        private System.Windows.Forms.SaveFileDialog saveFileDialog;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomPanel  _recentServersPanel;
        private System.Windows.Forms.PictureBox _recentServersStatusImage5;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomLinkLabel _recentServersLinkLabel5;
        private System.Windows.Forms.PictureBox _recentServersStatusImage4;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomLinkLabel _recentServersLinkLabel4;
        private System.Windows.Forms.PictureBox _recentServersStatusImage3;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomLinkLabel _recentServersLinkLabel3;
        private System.Windows.Forms.PictureBox _recentServersStatusImage2;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomLinkLabel _recentServersLinkLabel2;
        private System.Windows.Forms.PictureBox _recentServersStatusImage1;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomLinkLabel _recentServersLinkLabel1;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomLabel label6;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomLabel label7;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomPanel  _webConsolePanel;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomPanel  _pulseNewsFeedPanel;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomLinkLabel _twitterLinkLabel;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomLinkLabel _facebookLinkLabel;
        private System.Windows.Forms.PictureBox pictureBox6;
        private System.Windows.Forms.PictureBox pictureBox7;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomLabel label17;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomPanel  panel8;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomLabel label11;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomPanel  panel2;
        private System.Windows.Forms.PictureBox pictureBox1;
        private System.Windows.Forms.PictureBox pictureBox2;
        private Idera.SQLdm.DesktopClient.Controls.InfiniteProgressBar connectionProgressBar;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomPanel  panel7;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomPanel  _trialCenterPanel;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomLabel _trialCenterLabel1;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomTableLayoutPanel tableLayoutPanel2;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomLabel _trialCenterLabel2;
        private System.Windows.Forms.PictureBox pictureBox3;
        private System.Windows.Forms.PictureBox pictureBox10;
        private System.Windows.Forms.PictureBox pictureBox4;
        private System.Windows.Forms.PictureBox pictureBox5;
        private System.Windows.Forms.PictureBox pictureBox8;
        private System.Windows.Forms.PictureBox pictureBox9;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomLabel label10;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomLabel label12;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomLabel label13;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomLabel label14;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomLabel label15;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomLabel label16;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomLabel label18;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomLabel label19_1;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomLabel label19_2;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomLinkLabel webConsoleLinkLabel;
        private System.Windows.Forms.PictureBox pictureBox11;
        private System.Windows.Forms.PictureBox pictureBox12;
        //SQLdm 10.0 - (Ankit Nagpal)
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomLabel newFeature01;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomLabel newFeature02;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomLabel newFeature03;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomLabel newFeature04;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomLabel newFeature4;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomLabel newFeature5;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomLabel newFeature6;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomLabel newFeature7;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomLabel newFeature8;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomLabel newFeature06;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomLabel newFeature07;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomLabel newFeature08;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomLabel newFeature09;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomLabel newFeature10;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomLabel newFeature11;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomLabel label22;
        private Infragistics.Win.Misc.UltraPanel contentPanel;
    }
}
