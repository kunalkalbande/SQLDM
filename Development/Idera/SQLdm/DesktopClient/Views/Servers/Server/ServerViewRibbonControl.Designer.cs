using Idera.SQLdm.DesktopClient.Views.Servers.Server.Overview;

namespace Idera.SQLdm.DesktopClient.Views.Servers.Server
{
    partial class ServerViewRibbonControl
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
                if (tooltip != null) tooltip.Dispose();

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
            this.components = new System.ComponentModel.Container();
            Infragistics.Win.UltraWinToolbars.OptionSet optionSet1 = new Infragistics.Win.UltraWinToolbars.OptionSet("servicesTabViewOptionSet");
            Infragistics.Win.UltraWinToolbars.OptionSet optionSet2 = new Infragistics.Win.UltraWinToolbars.OptionSet("databasesTabViewOptionSet");
            Infragistics.Win.UltraWinToolbars.OptionSet optionSet3 = new Infragistics.Win.UltraWinToolbars.OptionSet("resourcesTabViewOptionSet");
            Infragistics.Win.UltraWinToolbars.OptionSet optionSet4 = new Infragistics.Win.UltraWinToolbars.OptionSet("overviewTabViewOptionSet");
            Infragistics.Win.UltraWinToolbars.OptionSet optionSet5 = new Infragistics.Win.UltraWinToolbars.OptionSet("sessionsTabViewOptionSet");
            Infragistics.Win.UltraWinToolbars.OptionSet optionSet6 = new Infragistics.Win.UltraWinToolbars.OptionSet("queriesTabViewOptionSet");
            //10.0 SQLdm Srishti Purohit
            Infragistics.Win.UltraWinToolbars.OptionSet optionSetAnalysis = new Infragistics.Win.UltraWinToolbars.OptionSet("analysisTabViewOptionSet");
            Infragistics.Win.UltraWinToolbars.RibbonTab ribbonTab1 = new Infragistics.Win.UltraWinToolbars.RibbonTab("Overview");
            Infragistics.Win.UltraWinToolbars.RibbonGroup ribbonGroup1 = new Infragistics.Win.UltraWinToolbars.RibbonGroup("overviewTabViewsGroup");
            Infragistics.Win.UltraWinToolbars.StateButtonTool stateButtonTool1 = new Infragistics.Win.UltraWinToolbars.StateButtonTool("overviewTabSummaryViewButton", "overviewTabViewOptionSet");
            Infragistics.Win.UltraWinToolbars.StateButtonTool stateButtonTool2 = new Infragistics.Win.UltraWinToolbars.StateButtonTool("overviewTabDetailsViewButton", "overviewTabViewOptionSet");
            Infragistics.Win.UltraWinToolbars.StateButtonTool stateButtonTool3 = new Infragistics.Win.UltraWinToolbars.StateButtonTool("overviewTabConfigurationViewButton", "overviewTabViewOptionSet");
            Infragistics.Win.UltraWinToolbars.StateButtonTool stateButtonTool4 = new Infragistics.Win.UltraWinToolbars.StateButtonTool("overviewTabAlertsViewButton", "overviewTabViewOptionSet");
            Infragistics.Win.UltraWinToolbars.RibbonGroup ribbonGroup2 = new Infragistics.Win.UltraWinToolbars.RibbonGroup("overviewTabSummaryViewHistoryGroup");
            Infragistics.Win.UltraWinToolbars.StateButtonTool stateButtonTool5 = new Infragistics.Win.UltraWinToolbars.StateButtonTool("toggleHistoryBrowserButton", "");
            Infragistics.Win.UltraWinToolbars.ButtonTool buttonTool1 = new Infragistics.Win.UltraWinToolbars.ButtonTool("showPreviousSnapshotButton");
            Infragistics.Win.UltraWinToolbars.ButtonTool buttonTool2 = new Infragistics.Win.UltraWinToolbars.ButtonTool("showNextSnapshotButton");
            Infragistics.Win.UltraWinToolbars.RibbonGroup ribbonGroup3 = new Infragistics.Win.UltraWinToolbars.RibbonGroup("overviewTabSummaryViewActionsGroup");
            Infragistics.Win.UltraWinToolbars.ButtonTool buttonTool3 = new Infragistics.Win.UltraWinToolbars.ButtonTool("stopServerButton");
            Infragistics.Win.UltraWinToolbars.ButtonTool buttonTool4 = new Infragistics.Win.UltraWinToolbars.ButtonTool("refreshAlertsButton");
            Infragistics.Win.UltraWinToolbars.StateButtonTool stateButtonTool6 = new Infragistics.Win.UltraWinToolbars.StateButtonTool("maintenanceModeButton", "");
            Infragistics.Win.UltraWinToolbars.RibbonGroup ribbonGroup4 = new Infragistics.Win.UltraWinToolbars.RibbonGroup("overviewTabSummaryViewOptionsGroup");
            Infragistics.Win.UltraWinToolbars.ButtonTool buttonTool5 = new Infragistics.Win.UltraWinToolbars.ButtonTool("toggleServerSummaryViewShowAnimationsButton");
            Infragistics.Win.UltraWinToolbars.RibbonGroup ribbonGroup5 = new Infragistics.Win.UltraWinToolbars.RibbonGroup("overviewTabConfigurationViewShowHideGroup");
            Infragistics.Win.UltraWinToolbars.ButtonTool buttonTool6 = new Infragistics.Win.UltraWinToolbars.ButtonTool("toggleServerConfigurationDetailsButton");
            Infragistics.Win.UltraWinToolbars.ButtonTool buttonTool7 = new Infragistics.Win.UltraWinToolbars.ButtonTool("toggleServerConfigurationGroupByBoxButton");
            Infragistics.Win.UltraWinToolbars.RibbonGroup ribbonGroup6 = new Infragistics.Win.UltraWinToolbars.RibbonGroup("overviewTabDetailsViewShowHideGroup");
            Infragistics.Win.UltraWinToolbars.ButtonTool buttonTool8 = new Infragistics.Win.UltraWinToolbars.ButtonTool("toggleServerDetailsChartButton");
            Infragistics.Win.UltraWinToolbars.ButtonTool buttonTool9 = new Infragistics.Win.UltraWinToolbars.ButtonTool("toggleServerDetailsGroupByBoxButton");
            Infragistics.Win.UltraWinToolbars.ButtonTool buttonTool10 = new Infragistics.Win.UltraWinToolbars.ButtonTool("toggleServerDetailsPropertiesButton");
            Infragistics.Win.UltraWinToolbars.RibbonGroup ribbonGroup7 = new Infragistics.Win.UltraWinToolbars.RibbonGroup("overviewTabActiveAlertsShowHideGroup");
            Infragistics.Win.UltraWinToolbars.ButtonTool buttonTool11 = new Infragistics.Win.UltraWinToolbars.ButtonTool("toggleActiveAlertsForecastButton");
            Infragistics.Win.UltraWinToolbars.ButtonTool buttonTool12 = new Infragistics.Win.UltraWinToolbars.ButtonTool("toggleActiveAlertsDetailsButton");
            Infragistics.Win.UltraWinToolbars.ButtonTool buttonTool13 = new Infragistics.Win.UltraWinToolbars.ButtonTool("toggleActiveAlertsGroupByBoxButton");
            Infragistics.Win.UltraWinToolbars.RibbonGroup ribbonGroup8 = new Infragistics.Win.UltraWinToolbars.RibbonGroup("overviewTabDetailsViewFilterGroup");
            Infragistics.Win.UltraWinToolbars.ButtonTool buttonTool14 = new Infragistics.Win.UltraWinToolbars.ButtonTool("detailsViewShowAllCountersButton");
            Infragistics.Win.UltraWinToolbars.ButtonTool buttonTool15 = new Infragistics.Win.UltraWinToolbars.ButtonTool("detailsViewShowCustomCountersButton");
            Infragistics.Win.UltraWinToolbars.RibbonGroup ribbonGroup9 = new Infragistics.Win.UltraWinToolbars.RibbonGroup("overviewTabPulseGroup");
            Infragistics.Win.UltraWinToolbars.ButtonTool buttonTool16 = new Infragistics.Win.UltraWinToolbars.ButtonTool("pulseProfileButton");
            Infragistics.Win.UltraWinToolbars.ButtonTool buttonTool17 = new Infragistics.Win.UltraWinToolbars.ButtonTool("pulsePostButton");
            Infragistics.Win.UltraWinToolbars.ButtonTool buttonTool18 = new Infragistics.Win.UltraWinToolbars.ButtonTool("pulseFollowButton");
            Infragistics.Win.Appearance appearance1 = new Infragistics.Win.Appearance();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ServerViewRibbonControl));
            Infragistics.Win.UltraWinToolbars.RibbonTab ribbonTab2 = new Infragistics.Win.UltraWinToolbars.RibbonTab("Sessions");
            Infragistics.Win.UltraWinToolbars.RibbonGroup ribbonGroup10 = new Infragistics.Win.UltraWinToolbars.RibbonGroup("sessionsTabViewsGroup");
            Infragistics.Win.UltraWinToolbars.StateButtonTool stateButtonTool7 = new Infragistics.Win.UltraWinToolbars.StateButtonTool("sessionsTabSummaryViewButton", "sessionsTabViewOptionSet");
            Infragistics.Win.UltraWinToolbars.StateButtonTool stateButtonTool8 = new Infragistics.Win.UltraWinToolbars.StateButtonTool("sessionsTabDetailsViewButton", "sessionsTabViewOptionSet");
            Infragistics.Win.UltraWinToolbars.StateButtonTool stateButtonTool9 = new Infragistics.Win.UltraWinToolbars.StateButtonTool("sessionsTabLocksViewButton", "sessionsTabViewOptionSet");
            Infragistics.Win.UltraWinToolbars.StateButtonTool stateButtonTool10 = new Infragistics.Win.UltraWinToolbars.StateButtonTool("sessionsTabBlockingViewButton", "sessionsTabViewOptionSet");
            Infragistics.Win.UltraWinToolbars.RibbonGroup ribbonGroup11 = new Infragistics.Win.UltraWinToolbars.RibbonGroup("sessionsTabHistoryGroup");
            Infragistics.Win.UltraWinToolbars.StateButtonTool stateButtonTool11 = new Infragistics.Win.UltraWinToolbars.StateButtonTool("toggleHistoryBrowserButton", "");
            Infragistics.Win.UltraWinToolbars.ButtonTool buttonTool19 = new Infragistics.Win.UltraWinToolbars.ButtonTool("showPreviousSnapshotButton");
            Infragistics.Win.UltraWinToolbars.ButtonTool buttonTool20 = new Infragistics.Win.UltraWinToolbars.ButtonTool("showNextSnapshotButton");
            Infragistics.Win.UltraWinToolbars.RibbonGroup ribbonGroup12 = new Infragistics.Win.UltraWinToolbars.RibbonGroup("sessionsTabDetailsActionsGroup");
            Infragistics.Win.UltraWinToolbars.ButtonTool buttonTool21 = new Infragistics.Win.UltraWinToolbars.ButtonTool("traceSessionsSessionButton");
            Infragistics.Win.UltraWinToolbars.ButtonTool buttonTool22 = new Infragistics.Win.UltraWinToolbars.ButtonTool("killSessionsSessionButton");
            Infragistics.Win.UltraWinToolbars.RibbonGroup ribbonGroup13 = new Infragistics.Win.UltraWinToolbars.RibbonGroup("sessionsTabDetailsFilterGroup");
            Infragistics.Win.UltraWinToolbars.StateButtonTool stateButtonTool12 = new Infragistics.Win.UltraWinToolbars.StateButtonTool("filterSessionsButton", "");
            Infragistics.Win.UltraWinToolbars.ButtonTool buttonTool23 = new Infragistics.Win.UltraWinToolbars.ButtonTool("showActiveSessionsOnlyButton");
            Infragistics.Win.UltraWinToolbars.ButtonTool buttonTool24 = new Infragistics.Win.UltraWinToolbars.ButtonTool("showUserSessionsOnlyButton");
            Infragistics.Win.UltraWinToolbars.ButtonTool buttonTool25 = new Infragistics.Win.UltraWinToolbars.ButtonTool("showBlockedSessionsButton");
            Infragistics.Win.UltraWinToolbars.RibbonGroup ribbonGroup14 = new Infragistics.Win.UltraWinToolbars.RibbonGroup("sessionsTabDetailsShowHideGroup");
            Infragistics.Win.UltraWinToolbars.ButtonTool buttonTool26 = new Infragistics.Win.UltraWinToolbars.ButtonTool("toggleSessionsDetailsDetailsPanelButton");
            Infragistics.Win.UltraWinToolbars.ButtonTool buttonTool27 = new Infragistics.Win.UltraWinToolbars.ButtonTool("toggleSessionsDetailsGroupByBoxButton");
            Infragistics.Win.UltraWinToolbars.RibbonGroup ribbonGroup15 = new Infragistics.Win.UltraWinToolbars.RibbonGroup("sessionsTabLocksActionsGroup");
            Infragistics.Win.UltraWinToolbars.ButtonTool buttonTool28 = new Infragistics.Win.UltraWinToolbars.ButtonTool("traceSessionsLocksSessionButton");
            Infragistics.Win.UltraWinToolbars.ButtonTool buttonTool29 = new Infragistics.Win.UltraWinToolbars.ButtonTool("killSessionsLocksSessionButton");
            Infragistics.Win.UltraWinToolbars.RibbonGroup ribbonGroup16 = new Infragistics.Win.UltraWinToolbars.RibbonGroup("sessionsTabLocksFilterGroup");
            Infragistics.Win.UltraWinToolbars.StateButtonTool stateButtonTool13 = new Infragistics.Win.UltraWinToolbars.StateButtonTool("filterLocksButton", "");
            Infragistics.Win.UltraWinToolbars.ButtonTool buttonTool30 = new Infragistics.Win.UltraWinToolbars.ButtonTool("showBlockedLocksOnlyButton");
            Infragistics.Win.UltraWinToolbars.ButtonTool buttonTool31 = new Infragistics.Win.UltraWinToolbars.ButtonTool("showBlockingLocksOnlyButton");
            Infragistics.Win.UltraWinToolbars.RibbonGroup ribbonGroup17 = new Infragistics.Win.UltraWinToolbars.RibbonGroup("sessionsTabLocksShowHideGroup");
            Infragistics.Win.UltraWinToolbars.ButtonTool buttonTool32 = new Infragistics.Win.UltraWinToolbars.ButtonTool("toggleSessionsLocksChartButton");
            Infragistics.Win.UltraWinToolbars.ButtonTool buttonTool33 = new Infragistics.Win.UltraWinToolbars.ButtonTool("toggleSessionsLocksGroupByBoxButton");
            Infragistics.Win.UltraWinToolbars.RibbonGroup ribbonGroup18 = new Infragistics.Win.UltraWinToolbars.RibbonGroup("sessionsTabBlockingActionsGroup");
            Infragistics.Win.UltraWinToolbars.ButtonTool buttonTool34 = new Infragistics.Win.UltraWinToolbars.ButtonTool("traceSessionsBlockingSessionButton");
            Infragistics.Win.UltraWinToolbars.ButtonTool buttonTool35 = new Infragistics.Win.UltraWinToolbars.ButtonTool("killSessionsBlockingSessionButton");
            Infragistics.Win.UltraWinToolbars.RibbonGroup ribbonGroup19 = new Infragistics.Win.UltraWinToolbars.RibbonGroup("sessionsTabBlockingTypeGroup");
            Infragistics.Win.UltraWinToolbars.ButtonTool buttonTool36 = new Infragistics.Win.UltraWinToolbars.ButtonTool("showSessionsBlockingBySessionButton");
            Infragistics.Win.UltraWinToolbars.ButtonTool buttonTool37 = new Infragistics.Win.UltraWinToolbars.ButtonTool("showSessionsBlockingByLockButton");
            Infragistics.Win.UltraWinToolbars.RibbonGroup ribbonGroup20 = new Infragistics.Win.UltraWinToolbars.RibbonGroup("sessionsTabBlockingShowHideGroup");
            Infragistics.Win.UltraWinToolbars.ButtonTool buttonTool38 = new Infragistics.Win.UltraWinToolbars.ButtonTool("toggleSessionsBlockingChartButton");
            Infragistics.Win.UltraWinToolbars.RibbonGroup ribbonGroup21 = new Infragistics.Win.UltraWinToolbars.RibbonGroup("sessionsTabPulseGroup");
            Infragistics.Win.UltraWinToolbars.ButtonTool buttonTool39 = new Infragistics.Win.UltraWinToolbars.ButtonTool("pulseProfileButton");
            Infragistics.Win.UltraWinToolbars.ButtonTool buttonTool40 = new Infragistics.Win.UltraWinToolbars.ButtonTool("pulsePostButton");
            Infragistics.Win.UltraWinToolbars.ButtonTool buttonTool41 = new Infragistics.Win.UltraWinToolbars.ButtonTool("pulseFollowButton");
            Infragistics.Win.Appearance appearance2 = new Infragistics.Win.Appearance();
            Infragistics.Win.UltraWinToolbars.RibbonTab ribbonTab3 = new Infragistics.Win.UltraWinToolbars.RibbonTab("Queries");
            Infragistics.Win.UltraWinToolbars.RibbonGroup ribbonGroup22 = new Infragistics.Win.UltraWinToolbars.RibbonGroup("queriesTabViewsGroup");
            Infragistics.Win.UltraWinToolbars.StateButtonTool stateButtonTool14 = new Infragistics.Win.UltraWinToolbars.StateButtonTool("queriesTabSignatureModeViewButton", "queriesTabViewOptionSet");
            Infragistics.Win.UltraWinToolbars.StateButtonTool stateButtonTool15 = new Infragistics.Win.UltraWinToolbars.StateButtonTool("queriesTabStatementModeViewButton", "queriesTabViewOptionSet");
            Infragistics.Win.UltraWinToolbars.StateButtonTool stateButtonTool16 = new Infragistics.Win.UltraWinToolbars.StateButtonTool("queriesTabHistoryModeViewButton", "queriesTabViewOptionSet");
            Infragistics.Win.UltraWinToolbars.StateButtonTool stateButtonTool17 = new Infragistics.Win.UltraWinToolbars.StateButtonTool("resourcesTabWaitStatsActiveViewButton", "queriesTabViewOptionSet");
            Infragistics.Win.UltraWinToolbars.RibbonGroup ribbonGroup23 = new Infragistics.Win.UltraWinToolbars.RibbonGroup("queriesTabHistoryGroup");
            Infragistics.Win.UltraWinToolbars.StateButtonTool stateButtonTool18 = new Infragistics.Win.UltraWinToolbars.StateButtonTool("toggleHistoryBrowserButton", "");
            Infragistics.Win.UltraWinToolbars.ButtonTool buttonTool42 = new Infragistics.Win.UltraWinToolbars.ButtonTool("showPreviousSnapshotButton");
            Infragistics.Win.UltraWinToolbars.ButtonTool buttonTool43 = new Infragistics.Win.UltraWinToolbars.ButtonTool("showNextSnapshotButton");
            Infragistics.Win.UltraWinToolbars.RibbonGroup ribbonGroup24 = new Infragistics.Win.UltraWinToolbars.RibbonGroup("queriesTabPropertiesGroup");
            Infragistics.Win.UltraWinToolbars.ButtonTool buttonTool44 = new Infragistics.Win.UltraWinToolbars.ButtonTool("configureQueryMonitorButton");
            Infragistics.Win.UltraWinToolbars.RibbonGroup ribbonGroup25 = new Infragistics.Win.UltraWinToolbars.RibbonGroup("queriesTabFilterGroup");
            Infragistics.Win.UltraWinToolbars.StateButtonTool stateButtonTool19 = new Infragistics.Win.UltraWinToolbars.StateButtonTool("filterQueryMonitorButton", "");
            Infragistics.Win.UltraWinToolbars.ButtonTool buttonTool45 = new Infragistics.Win.UltraWinToolbars.ButtonTool("showQueryMonitorSqlStatementsButton");
            Infragistics.Win.UltraWinToolbars.ButtonTool buttonTool46 = new Infragistics.Win.UltraWinToolbars.ButtonTool("showQueryMonitorStoredProceduresButton");
            Infragistics.Win.UltraWinToolbars.ButtonTool buttonTool47 = new Infragistics.Win.UltraWinToolbars.ButtonTool("showQueryMonitorSqlBatchesButton");
            Infragistics.Win.UltraWinToolbars.RibbonGroup ribbonGroup26 = new Infragistics.Win.UltraWinToolbars.RibbonGroup("queriesTabShowHideGroup");
            Infragistics.Win.UltraWinToolbars.ButtonTool buttonTool48 = new Infragistics.Win.UltraWinToolbars.ButtonTool("toggleQueryMonitorFiltersButton");
            Infragistics.Win.UltraWinToolbars.ButtonTool buttonTool49 = new Infragistics.Win.UltraWinToolbars.ButtonTool("toggleQueryMonitorGridButton");
            Infragistics.Win.UltraWinToolbars.ButtonTool buttonTool50 = new Infragistics.Win.UltraWinToolbars.ButtonTool("toggleQueryMonitorGroupByBoxButton");
            Infragistics.Win.UltraWinToolbars.RibbonGroup ribbonGroup27 = new Infragistics.Win.UltraWinToolbars.RibbonGroup("queriesTabPulseGroup");
            Infragistics.Win.UltraWinToolbars.ButtonTool buttonTool51 = new Infragistics.Win.UltraWinToolbars.ButtonTool("pulseProfileButton");
            Infragistics.Win.UltraWinToolbars.ButtonTool buttonTool52 = new Infragistics.Win.UltraWinToolbars.ButtonTool("pulsePostButton");
            Infragistics.Win.UltraWinToolbars.ButtonTool buttonTool53 = new Infragistics.Win.UltraWinToolbars.ButtonTool("pulseFollowButton");
            Infragistics.Win.Appearance appearance3 = new Infragistics.Win.Appearance();
            Infragistics.Win.UltraWinToolbars.RibbonTab ribbonTab4 = new Infragistics.Win.UltraWinToolbars.RibbonTab("Resources");
            Infragistics.Win.UltraWinToolbars.RibbonGroup ribbonGroup28 = new Infragistics.Win.UltraWinToolbars.RibbonGroup("resourcesTabViewsGroup");
            Infragistics.Win.UltraWinToolbars.StateButtonTool stateButtonTool20 = new Infragistics.Win.UltraWinToolbars.StateButtonTool("resourcesTabSummaryViewButton", "resourcesTabViewOptionSet");
            Infragistics.Win.UltraWinToolbars.StateButtonTool stateButtonTool21 = new Infragistics.Win.UltraWinToolbars.StateButtonTool("resourcesTabCpuViewButton", "resourcesTabViewOptionSet");
            Infragistics.Win.UltraWinToolbars.StateButtonTool stateButtonTool22 = new Infragistics.Win.UltraWinToolbars.StateButtonTool("resourcesTabMemoryViewButton", "resourcesTabViewOptionSet");
            Infragistics.Win.UltraWinToolbars.StateButtonTool stateButtonTool23 = new Infragistics.Win.UltraWinToolbars.StateButtonTool("resourcesTabDiskViewButton", "resourcesTabViewOptionSet");
            Infragistics.Win.UltraWinToolbars.StateButtonTool stateButtonTool24 = new Infragistics.Win.UltraWinToolbars.StateButtonTool("resourcesTabDiskSizeViewButton", "");
            Infragistics.Win.UltraWinToolbars.StateButtonTool stateButtonTool25 = new Infragistics.Win.UltraWinToolbars.StateButtonTool("resourcesTabFileActivityViewButton", "resourcesTabViewOptionSet");
            Infragistics.Win.UltraWinToolbars.StateButtonTool stateButtonTool26 = new Infragistics.Win.UltraWinToolbars.StateButtonTool("resourcesTabProcedureCacheViewButton", "resourcesTabViewOptionSet");
            Infragistics.Win.UltraWinToolbars.StateButtonTool stateButtonTool27 = new Infragistics.Win.UltraWinToolbars.StateButtonTool("resourcesTabWaitStatsViewButton", "resourcesTabViewOptionSet");
            Infragistics.Win.UltraWinToolbars.RibbonGroup ribbonGroup29 = new Infragistics.Win.UltraWinToolbars.RibbonGroup("resourcesTabHistoryGroup");
            Infragistics.Win.UltraWinToolbars.StateButtonTool stateButtonTool28 = new Infragistics.Win.UltraWinToolbars.StateButtonTool("toggleHistoryBrowserButton", "");
            Infragistics.Win.UltraWinToolbars.ButtonTool buttonTool54 = new Infragistics.Win.UltraWinToolbars.ButtonTool("showPreviousSnapshotButton");
            Infragistics.Win.UltraWinToolbars.ButtonTool buttonTool55 = new Infragistics.Win.UltraWinToolbars.ButtonTool("showNextSnapshotButton");
            Infragistics.Win.UltraWinToolbars.RibbonGroup ribbonGroup30 = new Infragistics.Win.UltraWinToolbars.RibbonGroup("resourcesTabActionsGroup");
            Infragistics.Win.UltraWinToolbars.ButtonTool buttonTool56 = new Infragistics.Win.UltraWinToolbars.ButtonTool("clearProcedureCacheButton");
            Infragistics.Win.UltraWinToolbars.RibbonGroup ribbonGroup31 = new Infragistics.Win.UltraWinToolbars.RibbonGroup("resourcesTabProcedureCacheViewShowHideGroup");
            Infragistics.Win.UltraWinToolbars.ButtonTool buttonTool57 = new Infragistics.Win.UltraWinToolbars.ButtonTool("toggleProcedureCacheChartsButton");
            Infragistics.Win.UltraWinToolbars.ButtonTool buttonTool58 = new Infragistics.Win.UltraWinToolbars.ButtonTool("toggleProcedureCacheGroupByBoxButton");
            Infragistics.Win.UltraWinToolbars.RibbonGroup ribbonGroup81 = new Infragistics.Win.UltraWinToolbars.RibbonGroup("resourcesTabDiskSizeViewShowHideGroup");
            Infragistics.Win.UltraWinToolbars.ButtonTool buttonTool242 = new Infragistics.Win.UltraWinToolbars.ButtonTool("toggleDiskSizeViewGroupByBoxButton");
            Infragistics.Win.UltraWinToolbars.RibbonGroup ribbonGroup32 = new Infragistics.Win.UltraWinToolbars.RibbonGroup("actionsRibbonGroup");
            Infragistics.Win.UltraWinToolbars.ButtonTool buttonTool59 = new Infragistics.Win.UltraWinToolbars.ButtonTool("resourcesQueryWaitsConfigureTopXButton");
            Infragistics.Win.UltraWinToolbars.ButtonTool buttonTool60 = new Infragistics.Win.UltraWinToolbars.ButtonTool("resourcesQueryWaitsConfigureButton");
            Infragistics.Win.UltraWinToolbars.RibbonGroup ribbonGroup33 = new Infragistics.Win.UltraWinToolbars.RibbonGroup("resourcesTabWaitStatsViewShowHideGroup");
            Infragistics.Win.UltraWinToolbars.ButtonTool buttonTool61 = new Infragistics.Win.UltraWinToolbars.ButtonTool("toggleWaitStatsGroupByBoxButton");
            Infragistics.Win.UltraWinToolbars.RibbonGroup ribbonGroup34 = new Infragistics.Win.UltraWinToolbars.RibbonGroup("resourcesShowFileActivityFilterGroup");
            Infragistics.Win.UltraWinToolbars.ButtonTool buttonTool62 = new Infragistics.Win.UltraWinToolbars.ButtonTool("resourcesShowFileActivityFilterButton");
            Infragistics.Win.UltraWinToolbars.RibbonGroup ribbonGroup35 = new Infragistics.Win.UltraWinToolbars.RibbonGroup("resourcesTabPulseGroup");
            Infragistics.Win.UltraWinToolbars.ButtonTool buttonTool63 = new Infragistics.Win.UltraWinToolbars.ButtonTool("pulseProfileButton");
            Infragistics.Win.UltraWinToolbars.ButtonTool buttonTool64 = new Infragistics.Win.UltraWinToolbars.ButtonTool("pulsePostButton");
            Infragistics.Win.UltraWinToolbars.ButtonTool buttonTool65 = new Infragistics.Win.UltraWinToolbars.ButtonTool("pulseFollowButton");
            Infragistics.Win.Appearance appearance4 = new Infragistics.Win.Appearance();
            Infragistics.Win.UltraWinToolbars.RibbonTab ribbonTab5 = new Infragistics.Win.UltraWinToolbars.RibbonTab("Databases");
            Infragistics.Win.UltraWinToolbars.RibbonGroup ribbonGroup36 = new Infragistics.Win.UltraWinToolbars.RibbonGroup("databasesTabViewsGroup");
            Infragistics.Win.UltraWinToolbars.StateButtonTool stateButtonTool29 = new Infragistics.Win.UltraWinToolbars.StateButtonTool("databasesTabSummaryViewButton", "databasesTabViewOptionSet");
            Infragistics.Win.UltraWinToolbars.StateButtonTool stateButtonTool30 = new Infragistics.Win.UltraWinToolbars.StateButtonTool("databasesTabTempdbSummaryViewButton", "databasesTabViewOptionSet");
            Infragistics.Win.UltraWinToolbars.StateButtonTool stateButtonTool31 = new Infragistics.Win.UltraWinToolbars.StateButtonTool("databasesTabConfigurationViewButton", "databasesTabViewOptionSet");
            Infragistics.Win.UltraWinToolbars.StateButtonTool stateButtonTool32 = new Infragistics.Win.UltraWinToolbars.StateButtonTool("databasesTabFilesViewButton", "databasesTabViewOptionSet");
            Infragistics.Win.UltraWinToolbars.StateButtonTool stateButtonTool33 = new Infragistics.Win.UltraWinToolbars.StateButtonTool("databasesTabBackupRestoreHistoryViewButton", "databasesTabViewOptionSet");
            Infragistics.Win.UltraWinToolbars.StateButtonTool stateButtonTool34 = new Infragistics.Win.UltraWinToolbars.StateButtonTool("databasesTabTablesIndexesViewButton", "databasesTabViewOptionSet");
            Infragistics.Win.UltraWinToolbars.StateButtonTool stateButtonTool35 = new Infragistics.Win.UltraWinToolbars.StateButtonTool("databasesTabMirroringViewButton", "databasesTabViewOptionSet");
            Infragistics.Win.UltraWinToolbars.RibbonGroup ribbonGroup37 = new Infragistics.Win.UltraWinToolbars.RibbonGroup("databasesTabSummaryViewFilterGroup");
            Infragistics.Win.UltraWinToolbars.ButtonTool buttonTool66 = new Infragistics.Win.UltraWinToolbars.ButtonTool("databasesSummaryShowUserDatabasesOnlyButton");
            Infragistics.Win.UltraWinToolbars.RibbonGroup ribbonGroup38 = new Infragistics.Win.UltraWinToolbars.RibbonGroup("databasesTabSummaryViewShowHideGroup");
            Infragistics.Win.UltraWinToolbars.ButtonTool buttonTool67 = new Infragistics.Win.UltraWinToolbars.ButtonTool("toggleDatabasesSummaryChartsButton");
            Infragistics.Win.UltraWinToolbars.ButtonTool buttonTool68 = new Infragistics.Win.UltraWinToolbars.ButtonTool("toggleDatabasesSummaryGroupByBoxButton");
            Infragistics.Win.UltraWinToolbars.RibbonGroup ribbonGroup39 = new Infragistics.Win.UltraWinToolbars.RibbonGroup("databasesTabConfigurationViewFilterGroup");
            Infragistics.Win.UltraWinToolbars.ButtonTool buttonTool69 = new Infragistics.Win.UltraWinToolbars.ButtonTool("databasesConfigurationShowUserDatabasesOnlyButton");
            Infragistics.Win.UltraWinToolbars.RibbonGroup ribbonGroup40 = new Infragistics.Win.UltraWinToolbars.RibbonGroup("databasesTabConfigurationViewShowHideGroup");
            Infragistics.Win.UltraWinToolbars.ButtonTool buttonTool70 = new Infragistics.Win.UltraWinToolbars.ButtonTool("toggleDatabasesConfigurationGroupByBoxButton");
            Infragistics.Win.UltraWinToolbars.RibbonGroup ribbonGroup41 = new Infragistics.Win.UltraWinToolbars.RibbonGroup("databasesTabBackupRestoreHistoryViewFilterGroup");
            Infragistics.Win.UltraWinToolbars.StateButtonTool stateButtonTool36 = new Infragistics.Win.UltraWinToolbars.StateButtonTool("filterDatabasesBackupRestoreHistoryButton", "");
            Infragistics.Win.UltraWinToolbars.ButtonTool buttonTool71 = new Infragistics.Win.UltraWinToolbars.ButtonTool("databasesBackupRestoreHistoryShowUserDatabasesOnlyButton");
            Infragistics.Win.UltraWinToolbars.ButtonTool buttonTool72 = new Infragistics.Win.UltraWinToolbars.ButtonTool("databasesBackupRestoreHistoryShowBackupsButton");
            Infragistics.Win.UltraWinToolbars.ButtonTool buttonTool73 = new Infragistics.Win.UltraWinToolbars.ButtonTool("databasesBackupRestoreHistoryShowRestoresButton");
            Infragistics.Win.UltraWinToolbars.RibbonGroup ribbonGroup42 = new Infragistics.Win.UltraWinToolbars.RibbonGroup("databasesTabBackupRestoreHistoryViewShowHideGroup");
            Infragistics.Win.UltraWinToolbars.ButtonTool buttonTool74 = new Infragistics.Win.UltraWinToolbars.ButtonTool("toggleDatabasesBackupRestoreHistoryDatabasesGroupByBoxButton");
            Infragistics.Win.UltraWinToolbars.ButtonTool buttonTool75 = new Infragistics.Win.UltraWinToolbars.ButtonTool("toggleDatabasesBackupRestoreHistoryGroupByBoxButton");
            Infragistics.Win.UltraWinToolbars.RibbonGroup ribbonGroup43 = new Infragistics.Win.UltraWinToolbars.RibbonGroup("databasesTabFilesViewFilterGroup");
            Infragistics.Win.UltraWinToolbars.ButtonTool buttonTool76 = new Infragistics.Win.UltraWinToolbars.ButtonTool("databasesFilesShowUserDatabasesOnlyButton");
            Infragistics.Win.UltraWinToolbars.RibbonGroup ribbonGroup44 = new Infragistics.Win.UltraWinToolbars.RibbonGroup("databasesTabFilesViewShowHideGroup");
            Infragistics.Win.UltraWinToolbars.ButtonTool buttonTool77 = new Infragistics.Win.UltraWinToolbars.ButtonTool("toggleDatabasesFilesGroupByBoxButton");
            Infragistics.Win.UltraWinToolbars.RibbonGroup ribbonGroup45 = new Infragistics.Win.UltraWinToolbars.RibbonGroup("databasesTabTablesViewActionsGroup");
            Infragistics.Win.UltraWinToolbars.ButtonTool buttonTool78 = new Infragistics.Win.UltraWinToolbars.ButtonTool("updateTableStatisticsButton");
            Infragistics.Win.UltraWinToolbars.ButtonTool buttonTool79 = new Infragistics.Win.UltraWinToolbars.ButtonTool("rebuildTableIndexesButton");
            Infragistics.Win.UltraWinToolbars.RibbonGroup ribbonGroup46 = new Infragistics.Win.UltraWinToolbars.RibbonGroup("databasesTabTablesViewFilterGroup");
            Infragistics.Win.UltraWinToolbars.ButtonTool buttonTool80 = new Infragistics.Win.UltraWinToolbars.ButtonTool("databasesTablesShowUserDatabasesOnlyButton");
            Infragistics.Win.UltraWinToolbars.ButtonTool buttonTool81 = new Infragistics.Win.UltraWinToolbars.ButtonTool("databasesTablesShowUserTablesOnlyButton");
            Infragistics.Win.UltraWinToolbars.RibbonGroup ribbonGroup47 = new Infragistics.Win.UltraWinToolbars.RibbonGroup("databasesTabTablesViewShowHideGroup");
            Infragistics.Win.UltraWinToolbars.ButtonTool buttonTool82 = new Infragistics.Win.UltraWinToolbars.ButtonTool("toggleDatabasesTablesTableDetailsButton");
            Infragistics.Win.UltraWinToolbars.ButtonTool buttonTool83 = new Infragistics.Win.UltraWinToolbars.ButtonTool("toggleDatabasesTablesGroupByBoxButton");
            Infragistics.Win.UltraWinToolbars.RibbonGroup ribbonGroup48 = new Infragistics.Win.UltraWinToolbars.RibbonGroup("databasesTabMirroringViewShowHideGroup");
            Infragistics.Win.UltraWinToolbars.ButtonTool buttonTool84 = new Infragistics.Win.UltraWinToolbars.ButtonTool("toggleDatabasesMirroredDatabasesGroupByBoxButton");
            Infragistics.Win.UltraWinToolbars.ButtonTool buttonTool85 = new Infragistics.Win.UltraWinToolbars.ButtonTool("toggleDatabaseMirroringHistoryGroupByBoxButton");
            Infragistics.Win.UltraWinToolbars.RibbonGroup ribbonGroup49 = new Infragistics.Win.UltraWinToolbars.RibbonGroup("databasesTabMirroringViewActionsGroup");
            Infragistics.Win.UltraWinToolbars.ButtonTool buttonTool86 = new Infragistics.Win.UltraWinToolbars.ButtonTool("databasesTabMirroringViewFailOverButton");
            Infragistics.Win.UltraWinToolbars.ButtonTool buttonTool87 = new Infragistics.Win.UltraWinToolbars.ButtonTool("databasesTabMirroringViewPauseButton");
            Infragistics.Win.Appearance appearance5 = new Infragistics.Win.Appearance();
            Infragistics.Win.UltraWinToolbars.RibbonGroup ribbonGroup50 = new Infragistics.Win.UltraWinToolbars.RibbonGroup("databasesTabPulseGroup");
            Infragistics.Win.UltraWinToolbars.ButtonTool buttonTool88 = new Infragistics.Win.UltraWinToolbars.ButtonTool("pulseProfileButton");
            Infragistics.Win.UltraWinToolbars.ButtonTool buttonTool89 = new Infragistics.Win.UltraWinToolbars.ButtonTool("pulsePostButton");
            Infragistics.Win.UltraWinToolbars.ButtonTool buttonTool90 = new Infragistics.Win.UltraWinToolbars.ButtonTool("pulseFollowButton");
            Infragistics.Win.UltraWinToolbars.RibbonGroup ribbonGroup51 = new Infragistics.Win.UltraWinToolbars.RibbonGroup("databasesTabHistoryGroup");
            Infragistics.Win.UltraWinToolbars.StateButtonTool stateButtonTool37 = new Infragistics.Win.UltraWinToolbars.StateButtonTool("toggleHistoryBrowserButton", "");
            Infragistics.Win.UltraWinToolbars.ButtonTool buttonTool91 = new Infragistics.Win.UltraWinToolbars.ButtonTool("showPreviousSnapshotButton");
            Infragistics.Win.UltraWinToolbars.ButtonTool buttonTool92 = new Infragistics.Win.UltraWinToolbars.ButtonTool("showNextSnapshotButton");
            Infragistics.Win.UltraWinToolbars.RibbonGroup ribbonGroup52 = new Infragistics.Win.UltraWinToolbars.RibbonGroup("databasesTabTempdbViewFilterGroup");
            Infragistics.Win.UltraWinToolbars.StateButtonTool stateButtonTool38 = new Infragistics.Win.UltraWinToolbars.StateButtonTool("filterDatabasesTempdbSessionsButton", "");
            Infragistics.Win.UltraWinToolbars.RibbonGroup ribbonGroup53 = new Infragistics.Win.UltraWinToolbars.RibbonGroup("databasesTabTempdbViewShowHideGroup");
            Infragistics.Win.UltraWinToolbars.ButtonTool buttonTool93 = new Infragistics.Win.UltraWinToolbars.ButtonTool("toggleDatabasesTempdbGroupByBoxButton");
            Infragistics.Win.Appearance appearance6 = new Infragistics.Win.Appearance();
            Infragistics.Win.UltraWinToolbars.RibbonTab ribbonTab6 = new Infragistics.Win.UltraWinToolbars.RibbonTab("Services");
            Infragistics.Win.UltraWinToolbars.RibbonGroup ribbonGroup54 = new Infragistics.Win.UltraWinToolbars.RibbonGroup("servicesTabViewsGroup");
            Infragistics.Win.UltraWinToolbars.StateButtonTool stateButtonTool39 = new Infragistics.Win.UltraWinToolbars.StateButtonTool("servicesTabSummaryViewButton", "servicesTabViewOptionSet");
            Infragistics.Win.UltraWinToolbars.StateButtonTool stateButtonTool40 = new Infragistics.Win.UltraWinToolbars.StateButtonTool("servicesTabSqlAgentJobsViewButton", "servicesTabViewOptionSet");
            Infragistics.Win.UltraWinToolbars.StateButtonTool stateButtonTool41 = new Infragistics.Win.UltraWinToolbars.StateButtonTool("servicesTabFullTextSearchViewButton", "servicesTabViewOptionSet");
            Infragistics.Win.UltraWinToolbars.StateButtonTool stateButtonTool42 = new Infragistics.Win.UltraWinToolbars.StateButtonTool("servicesTabReplicationViewButton", "servicesTabViewOptionSet");
            Infragistics.Win.UltraWinToolbars.RibbonGroup ribbonGroup55 = new Infragistics.Win.UltraWinToolbars.RibbonGroup("servicesTabServicesSummaryActionGroup");
            Infragistics.Win.UltraWinToolbars.ButtonTool buttonTool94 = new Infragistics.Win.UltraWinToolbars.ButtonTool("startServicesServiceButton");
            Infragistics.Win.UltraWinToolbars.ButtonTool buttonTool95 = new Infragistics.Win.UltraWinToolbars.ButtonTool("stopServicesServiceButton");
            Infragistics.Win.UltraWinToolbars.RibbonGroup ribbonGroup56 = new Infragistics.Win.UltraWinToolbars.RibbonGroup("servicesTabServicesSummaryShowHideGroup");
            Infragistics.Win.UltraWinToolbars.ButtonTool buttonTool96 = new Infragistics.Win.UltraWinToolbars.ButtonTool("toggleServicesSummaryChartsButton");
            Infragistics.Win.UltraWinToolbars.ButtonTool buttonTool97 = new Infragistics.Win.UltraWinToolbars.ButtonTool("toggleServicesSummaryGroupByBoxVisibleButton");
            Infragistics.Win.UltraWinToolbars.RibbonGroup ribbonGroup57 = new Infragistics.Win.UltraWinToolbars.RibbonGroup("servicesTabAgentJobsActionGroup");
            Infragistics.Win.UltraWinToolbars.ButtonTool buttonTool98 = new Infragistics.Win.UltraWinToolbars.ButtonTool("startServicesAgentButton");
            Infragistics.Win.UltraWinToolbars.ButtonTool buttonTool99 = new Infragistics.Win.UltraWinToolbars.ButtonTool("stopServicesAgentButton");
            Infragistics.Win.UltraWinToolbars.ButtonTool buttonTool100 = new Infragistics.Win.UltraWinToolbars.ButtonTool("startAgentJobButton");
            Infragistics.Win.UltraWinToolbars.ButtonTool buttonTool101 = new Infragistics.Win.UltraWinToolbars.ButtonTool("stopAgentJobButton");
            Infragistics.Win.UltraWinToolbars.RibbonGroup ribbonGroup58 = new Infragistics.Win.UltraWinToolbars.RibbonGroup("servicesTabAgentJobsFilterGroup");
            Infragistics.Win.UltraWinToolbars.StateButtonTool stateButtonTool43 = new Infragistics.Win.UltraWinToolbars.StateButtonTool("filterAgentJobsButton", "");
            Infragistics.Win.UltraWinToolbars.ButtonTool buttonTool102 = new Infragistics.Win.UltraWinToolbars.ButtonTool("showAgentJobsAllButton");
            Infragistics.Win.UltraWinToolbars.ButtonTool buttonTool103 = new Infragistics.Win.UltraWinToolbars.ButtonTool("showAgentJobsFailedOnlyButton");
            Infragistics.Win.UltraWinToolbars.ButtonTool buttonTool104 = new Infragistics.Win.UltraWinToolbars.ButtonTool("showAgentJobsRunningOnlyButton");
            Infragistics.Win.UltraWinToolbars.RibbonGroup ribbonGroup59 = new Infragistics.Win.UltraWinToolbars.RibbonGroup("servicesTabAgentJobsShowHideGroup");
            Infragistics.Win.UltraWinToolbars.ButtonTool buttonTool105 = new Infragistics.Win.UltraWinToolbars.ButtonTool("toggleServicesAgentJobsHistoryVisibleButton");
            Infragistics.Win.UltraWinToolbars.ButtonTool buttonTool106 = new Infragistics.Win.UltraWinToolbars.ButtonTool("toggleServicesAgentJobsGroupByBoxVisibleButton");
            Infragistics.Win.UltraWinToolbars.RibbonGroup ribbonGroup60 = new Infragistics.Win.UltraWinToolbars.RibbonGroup("servicesTabFullTextSearchActionGroup");
            Infragistics.Win.UltraWinToolbars.ButtonTool buttonTool107 = new Infragistics.Win.UltraWinToolbars.ButtonTool("startServicesFullTextSearchButton");
            Infragistics.Win.UltraWinToolbars.ButtonTool buttonTool108 = new Infragistics.Win.UltraWinToolbars.ButtonTool("stopServicesFullTextSearchButton");
            Infragistics.Win.UltraWinToolbars.ButtonTool buttonTool109 = new Infragistics.Win.UltraWinToolbars.ButtonTool("optimizeFullTextSearchCatalogButton");
            Infragistics.Win.UltraWinToolbars.ButtonTool buttonTool110 = new Infragistics.Win.UltraWinToolbars.ButtonTool("repopulateFullTextSearchCatalogButton");
            Infragistics.Win.UltraWinToolbars.ButtonTool buttonTool111 = new Infragistics.Win.UltraWinToolbars.ButtonTool("rebuildFullTextSearchCatalogButton");
            Infragistics.Win.UltraWinToolbars.RibbonGroup ribbonGroup61 = new Infragistics.Win.UltraWinToolbars.RibbonGroup("servicesTabFullTextSearchShowHideGroup");
            Infragistics.Win.UltraWinToolbars.ButtonTool buttonTool112 = new Infragistics.Win.UltraWinToolbars.ButtonTool("toggleServicesFullTextSearchDetailsButton");
            Infragistics.Win.UltraWinToolbars.ButtonTool buttonTool113 = new Infragistics.Win.UltraWinToolbars.ButtonTool("toggleServicesFullTextSearchGroupByBoxVisibleButton");
            Infragistics.Win.UltraWinToolbars.RibbonGroup ribbonGroup62 = new Infragistics.Win.UltraWinToolbars.RibbonGroup("servicesTabReplicationFilterGroup");
            Infragistics.Win.UltraWinToolbars.StateButtonTool stateButtonTool44 = new Infragistics.Win.UltraWinToolbars.StateButtonTool("filterReplicationButton", "");
            Infragistics.Win.UltraWinToolbars.RibbonGroup ribbonGroup63 = new Infragistics.Win.UltraWinToolbars.RibbonGroup("servicesTabReplicationShowHideGroup");
            Infragistics.Win.UltraWinToolbars.ButtonTool buttonTool114 = new Infragistics.Win.UltraWinToolbars.ButtonTool("toggleServicesReplicationTopologyGroupByBoxVisibleButton");
            Infragistics.Win.UltraWinToolbars.ButtonTool buttonTool115 = new Infragistics.Win.UltraWinToolbars.ButtonTool("toggleServicesReplicationDistributorGroupByBoxVisibleButton");
            Infragistics.Win.UltraWinToolbars.ButtonTool buttonTool116 = new Infragistics.Win.UltraWinToolbars.ButtonTool("toggleServicesReplicationReplicationGraphsVisibleButton");
            Infragistics.Win.UltraWinToolbars.RibbonGroup ribbonGroup64 = new Infragistics.Win.UltraWinToolbars.RibbonGroup("servicesTabPulseGroup");
            Infragistics.Win.UltraWinToolbars.ButtonTool buttonTool117 = new Infragistics.Win.UltraWinToolbars.ButtonTool("pulseProfileButton");
            Infragistics.Win.UltraWinToolbars.ButtonTool buttonTool118 = new Infragistics.Win.UltraWinToolbars.ButtonTool("pulsePostButton");
            Infragistics.Win.UltraWinToolbars.ButtonTool buttonTool119 = new Infragistics.Win.UltraWinToolbars.ButtonTool("pulseFollowButton");
            Infragistics.Win.Appearance appearance7 = new Infragistics.Win.Appearance();
            Infragistics.Win.UltraWinToolbars.RibbonTab ribbonTab7 = new Infragistics.Win.UltraWinToolbars.RibbonTab("Logs");
            Infragistics.Win.UltraWinToolbars.RibbonGroup ribbonGroup65 = new Infragistics.Win.UltraWinToolbars.RibbonGroup("logsTabFilterGroup");
            Infragistics.Win.UltraWinToolbars.StateButtonTool stateButtonTool45 = new Infragistics.Win.UltraWinToolbars.StateButtonTool("filterLogsButton", "");
            Infragistics.Win.UltraWinToolbars.ButtonTool buttonTool120 = new Infragistics.Win.UltraWinToolbars.ButtonTool("filterLogsShowErrorsButton");
            Infragistics.Win.UltraWinToolbars.ButtonTool buttonTool121 = new Infragistics.Win.UltraWinToolbars.ButtonTool("filterLogsShowWarningsButton");
            Infragistics.Win.UltraWinToolbars.ButtonTool buttonTool122 = new Infragistics.Win.UltraWinToolbars.ButtonTool("filterLogsShowInformationalButton");
            Infragistics.Win.UltraWinToolbars.RibbonGroup ribbonGroup66 = new Infragistics.Win.UltraWinToolbars.RibbonGroup("logsTabSearchGroup");
            Infragistics.Win.UltraWinToolbars.ButtonTool buttonTool123 = new Infragistics.Win.UltraWinToolbars.ButtonTool("searchLogsButton");
            Infragistics.Win.UltraWinToolbars.RibbonGroup ribbonGroup67 = new Infragistics.Win.UltraWinToolbars.RibbonGroup("logsTabActionsGroup");
            Infragistics.Win.UltraWinToolbars.ButtonTool buttonTool124 = new Infragistics.Win.UltraWinToolbars.ButtonTool("cycleServerLogButton");
            Infragistics.Win.UltraWinToolbars.ButtonTool buttonTool125 = new Infragistics.Win.UltraWinToolbars.ButtonTool("configureServerLogButton");
            Infragistics.Win.UltraWinToolbars.RibbonGroup ribbonGroup68 = new Infragistics.Win.UltraWinToolbars.RibbonGroup("logsTabShowHideGroup");
            Infragistics.Win.UltraWinToolbars.ButtonTool buttonTool126 = new Infragistics.Win.UltraWinToolbars.ButtonTool("toggleAvailableLogsButton");
            Infragistics.Win.UltraWinToolbars.ButtonTool buttonTool127 = new Infragistics.Win.UltraWinToolbars.ButtonTool("toggleLogDetailsButton");
            Infragistics.Win.UltraWinToolbars.ButtonTool buttonTool128 = new Infragistics.Win.UltraWinToolbars.ButtonTool("toggleLogGroupByBoxButton");
            Infragistics.Win.UltraWinToolbars.RibbonGroup ribbonGroup69 = new Infragistics.Win.UltraWinToolbars.RibbonGroup("logsTabPulseGroup");
            Infragistics.Win.UltraWinToolbars.ButtonTool buttonTool129 = new Infragistics.Win.UltraWinToolbars.ButtonTool("pulseProfileButton");
            Infragistics.Win.UltraWinToolbars.ButtonTool buttonTool130 = new Infragistics.Win.UltraWinToolbars.ButtonTool("pulsePostButton");
            Infragistics.Win.UltraWinToolbars.ButtonTool buttonTool131 = new Infragistics.Win.UltraWinToolbars.ButtonTool("pulseFollowButton");
            //10.0 SQLdm srishti purohit
            Infragistics.Win.UltraWinToolbars.RibbonTab ribbonTabAnalysis = new Infragistics.Win.UltraWinToolbars.RibbonTab("Analyze");
            Infragistics.Win.UltraWinToolbars.RibbonGroup ribbonGroupRunAnalysis = new Infragistics.Win.UltraWinToolbars.RibbonGroup("analyzeTabRunGroup");
            

            Infragistics.Win.Appearance appearance8 = new Infragistics.Win.Appearance();
            Infragistics.Win.UltraWinToolbars.StateButtonTool stateButtonTool46 = new Infragistics.Win.UltraWinToolbars.StateButtonTool("overviewTabSummaryViewButton", "overviewTabViewOptionSet");
            Infragistics.Win.Appearance appearance9 = new Infragistics.Win.Appearance();
            Infragistics.Win.UltraWinToolbars.StateButtonTool stateButtonTool47 = new Infragistics.Win.UltraWinToolbars.StateButtonTool("overviewTabDetailsViewButton", "overviewTabViewOptionSet");
            Infragistics.Win.Appearance appearance10 = new Infragistics.Win.Appearance();
            Infragistics.Win.UltraWinToolbars.StateButtonTool stateButtonTool48 = new Infragistics.Win.UltraWinToolbars.StateButtonTool("overviewTabConfigurationViewButton", "overviewTabViewOptionSet");
            Infragistics.Win.Appearance appearance11 = new Infragistics.Win.Appearance();
            Infragistics.Win.UltraWinToolbars.ButtonTool buttonTool132 = new Infragistics.Win.UltraWinToolbars.ButtonTool("stopServerButton");
            Infragistics.Win.Appearance appearance12 = new Infragistics.Win.Appearance();
            Infragistics.Win.UltraWinToolbars.StateButtonTool stateButtonTool49 = new Infragistics.Win.UltraWinToolbars.StateButtonTool("sessionsTabSummaryViewButton", "sessionsTabViewOptionSet");
            Infragistics.Win.Appearance appearance13 = new Infragistics.Win.Appearance();
            Infragistics.Win.UltraWinToolbars.ButtonTool buttonTool133 = new Infragistics.Win.UltraWinToolbars.ButtonTool("killSessionsSessionButton");
            Infragistics.Win.Appearance appearance14 = new Infragistics.Win.Appearance();
            Infragistics.Win.UltraWinToolbars.StateButtonTool stateButtonTool50 = new Infragistics.Win.UltraWinToolbars.StateButtonTool("sessionsTabDetailsViewButton", "sessionsTabViewOptionSet");
            Infragistics.Win.Appearance appearance15 = new Infragistics.Win.Appearance();
            Infragistics.Win.UltraWinToolbars.StateButtonTool stateButtonTool51 = new Infragistics.Win.UltraWinToolbars.StateButtonTool("filterSessionsButton", "");
            Infragistics.Win.Appearance appearance16 = new Infragistics.Win.Appearance();
            Infragistics.Win.UltraWinToolbars.ButtonTool buttonTool134 = new Infragistics.Win.UltraWinToolbars.ButtonTool("showUserSessionsOnlyButton");
            Infragistics.Win.Appearance appearance17 = new Infragistics.Win.Appearance();
            Infragistics.Win.UltraWinToolbars.ButtonTool buttonTool135 = new Infragistics.Win.UltraWinToolbars.ButtonTool("toggleSessionsDetailsDetailsPanelButton");
            Infragistics.Win.Appearance appearance18 = new Infragistics.Win.Appearance();
            Infragistics.Win.UltraWinToolbars.ButtonTool buttonTool136 = new Infragistics.Win.UltraWinToolbars.ButtonTool("toggleSessionsDetailsGroupByBoxButton");
            Infragistics.Win.Appearance appearance19 = new Infragistics.Win.Appearance();
            Infragistics.Win.UltraWinToolbars.ButtonTool buttonTool137 = new Infragistics.Win.UltraWinToolbars.ButtonTool("showActiveSessionsOnlyButton");
            Infragistics.Win.Appearance appearance20 = new Infragistics.Win.Appearance();
            Infragistics.Win.UltraWinToolbars.ButtonTool buttonTool138 = new Infragistics.Win.UltraWinToolbars.ButtonTool("showBlockedSessionsButton");
            Infragistics.Win.Appearance appearance21 = new Infragistics.Win.Appearance();
            Infragistics.Win.UltraWinToolbars.ButtonTool buttonTool139 = new Infragistics.Win.UltraWinToolbars.ButtonTool("toggleServerConfigurationDetailsButton");
            Infragistics.Win.Appearance appearance22 = new Infragistics.Win.Appearance();
            Infragistics.Win.UltraWinToolbars.ButtonTool buttonTool140 = new Infragistics.Win.UltraWinToolbars.ButtonTool("toggleServerConfigurationGroupByBoxButton");
            Infragistics.Win.Appearance appearance23 = new Infragistics.Win.Appearance();
            Infragistics.Win.UltraWinToolbars.ButtonTool buttonTool141 = new Infragistics.Win.UltraWinToolbars.ButtonTool("toggleServerDetailsChartButton");
            Infragistics.Win.Appearance appearance24 = new Infragistics.Win.Appearance();
            Infragistics.Win.UltraWinToolbars.ButtonTool buttonTool142 = new Infragistics.Win.UltraWinToolbars.ButtonTool("toggleServerDetailsGroupByBoxButton");
            Infragistics.Win.Appearance appearance25 = new Infragistics.Win.Appearance();
            Infragistics.Win.UltraWinToolbars.StateButtonTool stateButtonTool52 = new Infragistics.Win.UltraWinToolbars.StateButtonTool("sessionsTabLocksViewButton", "sessionsTabViewOptionSet");
            Infragistics.Win.Appearance appearance26 = new Infragistics.Win.Appearance();
            Infragistics.Win.UltraWinToolbars.StateButtonTool stateButtonTool53 = new Infragistics.Win.UltraWinToolbars.StateButtonTool("sessionsTabBlockingViewButton", "sessionsTabViewOptionSet");
            Infragistics.Win.Appearance appearance27 = new Infragistics.Win.Appearance();
            Infragistics.Win.UltraWinToolbars.ButtonTool buttonTool143 = new Infragistics.Win.UltraWinToolbars.ButtonTool("toggleSessionsLocksGroupByBoxButton");
            Infragistics.Win.Appearance appearance28 = new Infragistics.Win.Appearance();
            Infragistics.Win.UltraWinToolbars.ButtonTool buttonTool144 = new Infragistics.Win.UltraWinToolbars.ButtonTool("toggleSessionsLocksChartButton");
            Infragistics.Win.Appearance appearance29 = new Infragistics.Win.Appearance();
            Infragistics.Win.UltraWinToolbars.StateButtonTool stateButtonTool54 = new Infragistics.Win.UltraWinToolbars.StateButtonTool("filterLocksButton", "");
            Infragistics.Win.Appearance appearance30 = new Infragistics.Win.Appearance();
            Infragistics.Win.UltraWinToolbars.ButtonTool buttonTool145 = new Infragistics.Win.UltraWinToolbars.ButtonTool("showBlockedLocksOnlyButton");
            Infragistics.Win.Appearance appearance31 = new Infragistics.Win.Appearance();
            Infragistics.Win.UltraWinToolbars.ButtonTool buttonTool146 = new Infragistics.Win.UltraWinToolbars.ButtonTool("showBlockingLocksOnlyButton");
            Infragistics.Win.Appearance appearance32 = new Infragistics.Win.Appearance();
            Infragistics.Win.UltraWinToolbars.ButtonTool buttonTool147 = new Infragistics.Win.UltraWinToolbars.ButtonTool("showBlockedOrBlockingLocksButton");
            Infragistics.Win.Appearance appearance33 = new Infragistics.Win.Appearance();
            Infragistics.Win.UltraWinToolbars.StateButtonTool stateButtonTool55 = new Infragistics.Win.UltraWinToolbars.StateButtonTool("filterBlockingButton", "");
            Infragistics.Win.Appearance appearance34 = new Infragistics.Win.Appearance();
            Infragistics.Win.UltraWinToolbars.ButtonTool buttonTool148 = new Infragistics.Win.UltraWinToolbars.ButtonTool("toggleSessionsBlockingChartButton");
            Infragistics.Win.Appearance appearance35 = new Infragistics.Win.Appearance();
            Infragistics.Win.UltraWinToolbars.ButtonTool buttonTool149 = new Infragistics.Win.UltraWinToolbars.ButtonTool("toggleSessionsBlockingGroupByBoxButton");
            Infragistics.Win.Appearance appearance36 = new Infragistics.Win.Appearance();
            Infragistics.Win.UltraWinToolbars.StateButtonTool stateButtonTool56 = new Infragistics.Win.UltraWinToolbars.StateButtonTool("filterQueryMonitorButton", "");
            Infragistics.Win.Appearance appearance37 = new Infragistics.Win.Appearance();
            Infragistics.Win.UltraWinToolbars.ButtonTool buttonTool150 = new Infragistics.Win.UltraWinToolbars.ButtonTool("showQueryMonitorSqlStatementsButton");
            Infragistics.Win.Appearance appearance38 = new Infragistics.Win.Appearance();
            Infragistics.Win.UltraWinToolbars.ButtonTool buttonTool151 = new Infragistics.Win.UltraWinToolbars.ButtonTool("showQueryMonitorStoredProceduresButton");
            Infragistics.Win.Appearance appearance39 = new Infragistics.Win.Appearance();
            Infragistics.Win.UltraWinToolbars.ButtonTool buttonTool152 = new Infragistics.Win.UltraWinToolbars.ButtonTool("showQueryMonitorSqlBatchesButton");
            Infragistics.Win.Appearance appearance40 = new Infragistics.Win.Appearance();
            Infragistics.Win.UltraWinToolbars.ButtonTool buttonTool153 = new Infragistics.Win.UltraWinToolbars.ButtonTool("toggleQueryMonitorFiltersButton");
            Infragistics.Win.Appearance appearance41 = new Infragistics.Win.Appearance();
            Infragistics.Win.UltraWinToolbars.ButtonTool buttonTool154 = new Infragistics.Win.UltraWinToolbars.ButtonTool("toggleQueryMonitorGroupByBoxButton");
            Infragistics.Win.Appearance appearance42 = new Infragistics.Win.Appearance();
            Infragistics.Win.UltraWinToolbars.ButtonTool buttonTool155 = new Infragistics.Win.UltraWinToolbars.ButtonTool("showSessionsBlockingByLockButton");
            Infragistics.Win.Appearance appearance43 = new Infragistics.Win.Appearance();
            Infragistics.Win.UltraWinToolbars.ButtonTool buttonTool156 = new Infragistics.Win.UltraWinToolbars.ButtonTool("showSessionsBlockingBySessionButton");
            Infragistics.Win.Appearance appearance44 = new Infragistics.Win.Appearance();
            Infragistics.Win.UltraWinToolbars.StateButtonTool stateButtonTool57 = new Infragistics.Win.UltraWinToolbars.StateButtonTool("maintenanceModeButton", "");
            Infragistics.Win.Appearance appearance45 = new Infragistics.Win.Appearance();
            Infragistics.Win.UltraWinToolbars.StateButtonTool stateButtonTool58 = new Infragistics.Win.UltraWinToolbars.StateButtonTool("filterLogsButton", "");
            Infragistics.Win.Appearance appearance46 = new Infragistics.Win.Appearance();
            Infragistics.Win.UltraWinToolbars.ButtonTool buttonTool157 = new Infragistics.Win.UltraWinToolbars.ButtonTool("filterLogsShowErrorsButton");
            Infragistics.Win.Appearance appearance47 = new Infragistics.Win.Appearance();
            Infragistics.Win.UltraWinToolbars.ButtonTool buttonTool158 = new Infragistics.Win.UltraWinToolbars.ButtonTool("filterLogsShowWarningsButton");
            Infragistics.Win.Appearance appearance48 = new Infragistics.Win.Appearance();
            Infragistics.Win.UltraWinToolbars.ButtonTool buttonTool159 = new Infragistics.Win.UltraWinToolbars.ButtonTool("filterLogsShowInformationalButton");
            Infragistics.Win.Appearance appearance49 = new Infragistics.Win.Appearance();
            Infragistics.Win.UltraWinToolbars.ButtonTool buttonTool160 = new Infragistics.Win.UltraWinToolbars.ButtonTool("searchLogsButton");
            Infragistics.Win.Appearance appearance50 = new Infragistics.Win.Appearance();
            Infragistics.Win.UltraWinToolbars.ButtonTool buttonTool161 = new Infragistics.Win.UltraWinToolbars.ButtonTool("cycleServerLogButton");
            Infragistics.Win.Appearance appearance51 = new Infragistics.Win.Appearance();
            Infragistics.Win.UltraWinToolbars.ButtonTool buttonTool162 = new Infragistics.Win.UltraWinToolbars.ButtonTool("configureServerLogButton");
            Infragistics.Win.Appearance appearance52 = new Infragistics.Win.Appearance();
            Infragistics.Win.UltraWinToolbars.ButtonTool buttonTool163 = new Infragistics.Win.UltraWinToolbars.ButtonTool("toggleAvailableLogsButton");
            Infragistics.Win.Appearance appearance53 = new Infragistics.Win.Appearance();
            Infragistics.Win.UltraWinToolbars.ButtonTool buttonTool164 = new Infragistics.Win.UltraWinToolbars.ButtonTool("toggleLogDetailsButton");
            Infragistics.Win.Appearance appearance54 = new Infragistics.Win.Appearance();
            Infragistics.Win.UltraWinToolbars.ButtonTool buttonTool165 = new Infragistics.Win.UltraWinToolbars.ButtonTool("toggleLogGroupByBoxButton");
            Infragistics.Win.Appearance appearance55 = new Infragistics.Win.Appearance();
            Infragistics.Win.UltraWinToolbars.StateButtonTool stateButtonTool59 = new Infragistics.Win.UltraWinToolbars.StateButtonTool("resourcesTabSummaryViewButton", "resourcesTabViewOptionSet");
            Infragistics.Win.Appearance appearance56 = new Infragistics.Win.Appearance();
            Infragistics.Win.UltraWinToolbars.StateButtonTool stateButtonTool60 = new Infragistics.Win.UltraWinToolbars.StateButtonTool("resourcesTabCpuViewButton", "resourcesTabViewOptionSet");
            Infragistics.Win.Appearance appearance57 = new Infragistics.Win.Appearance();
            Infragistics.Win.UltraWinToolbars.StateButtonTool stateButtonTool61 = new Infragistics.Win.UltraWinToolbars.StateButtonTool("resourcesTabMemoryViewButton", "resourcesTabViewOptionSet");
            Infragistics.Win.Appearance appearance58 = new Infragistics.Win.Appearance();
            Infragistics.Win.UltraWinToolbars.StateButtonTool stateButtonTool62 = new Infragistics.Win.UltraWinToolbars.StateButtonTool("resourcesTabDiskViewButton", "resourcesTabViewOptionSet");
            Infragistics.Win.Appearance appearance59 = new Infragistics.Win.Appearance();
            Infragistics.Win.UltraWinToolbars.StateButtonTool stateButtonTool63 = new Infragistics.Win.UltraWinToolbars.StateButtonTool("resourcesTabProcedureCacheViewButton", "resourcesTabViewOptionSet");
            Infragistics.Win.Appearance appearance60 = new Infragistics.Win.Appearance();
            Infragistics.Win.UltraWinToolbars.ButtonTool buttonTool166 = new Infragistics.Win.UltraWinToolbars.ButtonTool("clearProcedureCacheButton");
            Infragistics.Win.Appearance appearance61 = new Infragistics.Win.Appearance();
            Infragistics.Win.UltraWinToolbars.ButtonTool buttonTool167 = new Infragistics.Win.UltraWinToolbars.ButtonTool("toggleProcedureCacheChartsButton");
            Infragistics.Win.Appearance appearance62 = new Infragistics.Win.Appearance();
            Infragistics.Win.UltraWinToolbars.ButtonTool buttonTool168 = new Infragistics.Win.UltraWinToolbars.ButtonTool("toggleProcedureCacheGroupByBoxButton");
            Infragistics.Win.Appearance appearance63 = new Infragistics.Win.Appearance();
            Infragistics.Win.UltraWinToolbars.ButtonTool buttonTool169 = new Infragistics.Win.UltraWinToolbars.ButtonTool("traceSessionsSessionButton");
            Infragistics.Win.Appearance appearance64 = new Infragistics.Win.Appearance();
            Infragistics.Win.UltraWinToolbars.ButtonTool buttonTool170 = new Infragistics.Win.UltraWinToolbars.ButtonTool("traceSessionsLocksSessionButton");
            Infragistics.Win.Appearance appearance65 = new Infragistics.Win.Appearance();
            Infragistics.Win.UltraWinToolbars.ButtonTool buttonTool171 = new Infragistics.Win.UltraWinToolbars.ButtonTool("killSessionsLocksSessionButton");
            Infragistics.Win.Appearance appearance66 = new Infragistics.Win.Appearance();
            Infragistics.Win.UltraWinToolbars.ButtonTool buttonTool172 = new Infragistics.Win.UltraWinToolbars.ButtonTool("traceSessionsBlockingSessionButton");
            Infragistics.Win.Appearance appearance67 = new Infragistics.Win.Appearance();
            Infragistics.Win.UltraWinToolbars.ButtonTool buttonTool173 = new Infragistics.Win.UltraWinToolbars.ButtonTool("killSessionsBlockingSessionButton");
            Infragistics.Win.Appearance appearance68 = new Infragistics.Win.Appearance();
            Infragistics.Win.UltraWinToolbars.StateButtonTool stateButtonTool64 = new Infragistics.Win.UltraWinToolbars.StateButtonTool("databasesTabSummaryViewButton", "databasesTabViewOptionSet");
            Infragistics.Win.Appearance appearance69 = new Infragistics.Win.Appearance();
            Infragistics.Win.UltraWinToolbars.StateButtonTool stateButtonTool65 = new Infragistics.Win.UltraWinToolbars.StateButtonTool("databasesTabConfigurationViewButton", "databasesTabViewOptionSet");
            Infragistics.Win.Appearance appearance70 = new Infragistics.Win.Appearance();
            Infragistics.Win.UltraWinToolbars.StateButtonTool stateButtonTool66 = new Infragistics.Win.UltraWinToolbars.StateButtonTool("databasesTabBackupRestoreHistoryViewButton", "databasesTabViewOptionSet");
            Infragistics.Win.Appearance appearance71 = new Infragistics.Win.Appearance();
            Infragistics.Win.UltraWinToolbars.StateButtonTool stateButtonTool67 = new Infragistics.Win.UltraWinToolbars.StateButtonTool("databasesTabTablesIndexesViewButton", "databasesTabViewOptionSet");
            Infragistics.Win.Appearance appearance72 = new Infragistics.Win.Appearance();
            Infragistics.Win.UltraWinToolbars.StateButtonTool stateButtonTool68 = new Infragistics.Win.UltraWinToolbars.StateButtonTool("databasesTabFilesViewButton", "databasesTabViewOptionSet");
            Infragistics.Win.Appearance appearance73 = new Infragistics.Win.Appearance();
            Infragistics.Win.UltraWinToolbars.ButtonTool buttonTool174 = new Infragistics.Win.UltraWinToolbars.ButtonTool("databasesSummaryShowUserDatabasesOnlyButton");
            Infragistics.Win.Appearance appearance74 = new Infragistics.Win.Appearance();
            Infragistics.Win.UltraWinToolbars.ButtonTool buttonTool175 = new Infragistics.Win.UltraWinToolbars.ButtonTool("toggleDatabasesSummaryChartsButton");
            Infragistics.Win.Appearance appearance75 = new Infragistics.Win.Appearance();
            Infragistics.Win.UltraWinToolbars.ButtonTool buttonTool176 = new Infragistics.Win.UltraWinToolbars.ButtonTool("toggleDatabasesSummaryGroupByBoxButton");
            Infragistics.Win.Appearance appearance76 = new Infragistics.Win.Appearance();
            Infragistics.Win.UltraWinToolbars.ButtonTool buttonTool177 = new Infragistics.Win.UltraWinToolbars.ButtonTool("toggleDatabasesConfigurationGroupByBoxButton");
            Infragistics.Win.Appearance appearance77 = new Infragistics.Win.Appearance();
            Infragistics.Win.UltraWinToolbars.ButtonTool buttonTool178 = new Infragistics.Win.UltraWinToolbars.ButtonTool("databasesConfigurationShowUserDatabasesOnlyButton");
            Infragistics.Win.Appearance appearance78 = new Infragistics.Win.Appearance();
            Infragistics.Win.UltraWinToolbars.ButtonTool buttonTool179 = new Infragistics.Win.UltraWinToolbars.ButtonTool("databasesBackupRestoreHistoryShowUserDatabasesOnlyButton");
            Infragistics.Win.Appearance appearance79 = new Infragistics.Win.Appearance();
            Infragistics.Win.UltraWinToolbars.ButtonTool buttonTool180 = new Infragistics.Win.UltraWinToolbars.ButtonTool("databasesBackupRestoreHistoryShowBackupsButton");
            Infragistics.Win.Appearance appearance80 = new Infragistics.Win.Appearance();
            Infragistics.Win.UltraWinToolbars.ButtonTool buttonTool181 = new Infragistics.Win.UltraWinToolbars.ButtonTool("databasesBackupRestoreHistoryShowRestoresButton");
            Infragistics.Win.Appearance appearance81 = new Infragistics.Win.Appearance();
            Infragistics.Win.UltraWinToolbars.StateButtonTool stateButtonTool69 = new Infragistics.Win.UltraWinToolbars.StateButtonTool("filterDatabasesBackupRestoreHistoryButton", "");
            Infragistics.Win.Appearance appearance82 = new Infragistics.Win.Appearance();
            Infragistics.Win.UltraWinToolbars.ButtonTool buttonTool182 = new Infragistics.Win.UltraWinToolbars.ButtonTool("toggleDatabasesBackupRestoreHistoryDatabasesGroupByBoxButton");
            Infragistics.Win.Appearance appearance83 = new Infragistics.Win.Appearance();
            Infragistics.Win.UltraWinToolbars.ButtonTool buttonTool183 = new Infragistics.Win.UltraWinToolbars.ButtonTool("toggleDatabasesBackupRestoreHistoryGroupByBoxButton");
            Infragistics.Win.Appearance appearance84 = new Infragistics.Win.Appearance();
            Infragistics.Win.UltraWinToolbars.ButtonTool buttonTool184 = new Infragistics.Win.UltraWinToolbars.ButtonTool("databasesFilesShowUserDatabasesOnlyButton");
            Infragistics.Win.Appearance appearance85 = new Infragistics.Win.Appearance();
            Infragistics.Win.UltraWinToolbars.ButtonTool buttonTool185 = new Infragistics.Win.UltraWinToolbars.ButtonTool("toggleDatabasesFilesGroupByBoxButton");
            Infragistics.Win.Appearance appearance86 = new Infragistics.Win.Appearance();
            Infragistics.Win.UltraWinToolbars.ButtonTool buttonTool186 = new Infragistics.Win.UltraWinToolbars.ButtonTool("databasesTablesShowUserDatabasesOnlyButton");
            Infragistics.Win.Appearance appearance87 = new Infragistics.Win.Appearance();
            Infragistics.Win.UltraWinToolbars.ButtonTool buttonTool187 = new Infragistics.Win.UltraWinToolbars.ButtonTool("databasesTablesShowUserTablesOnlyButton");
            Infragistics.Win.Appearance appearance88 = new Infragistics.Win.Appearance();
            Infragistics.Win.UltraWinToolbars.ButtonTool buttonTool188 = new Infragistics.Win.UltraWinToolbars.ButtonTool("toggleDatabasesTablesTableDetailsButton");
            Infragistics.Win.Appearance appearance89 = new Infragistics.Win.Appearance();
            Infragistics.Win.UltraWinToolbars.ButtonTool buttonTool189 = new Infragistics.Win.UltraWinToolbars.ButtonTool("toggleDatabasesTablesGroupByBoxButton");
            Infragistics.Win.Appearance appearance90 = new Infragistics.Win.Appearance();
            Infragistics.Win.UltraWinToolbars.ButtonTool buttonTool190 = new Infragistics.Win.UltraWinToolbars.ButtonTool("updateTableStatisticsButton");
            Infragistics.Win.Appearance appearance91 = new Infragistics.Win.Appearance();
            Infragistics.Win.UltraWinToolbars.ButtonTool buttonTool191 = new Infragistics.Win.UltraWinToolbars.ButtonTool("rebuildTableIndexesButton");
            Infragistics.Win.Appearance appearance92 = new Infragistics.Win.Appearance();
            Infragistics.Win.UltraWinToolbars.StateButtonTool stateButtonTool70 = new Infragistics.Win.UltraWinToolbars.StateButtonTool("servicesTabSqlAgentJobsViewButton", "servicesTabViewOptionSet");
            Infragistics.Win.Appearance appearance93 = new Infragistics.Win.Appearance();
            Infragistics.Win.UltraWinToolbars.StateButtonTool stateButtonTool71 = new Infragistics.Win.UltraWinToolbars.StateButtonTool("servicesTabSummaryViewButton", "servicesTabViewOptionSet");
            Infragistics.Win.Appearance appearance94 = new Infragistics.Win.Appearance();
            Infragistics.Win.UltraWinToolbars.StateButtonTool stateButtonTool72 = new Infragistics.Win.UltraWinToolbars.StateButtonTool("servicesTabFullTextSearchViewButton", "servicesTabViewOptionSet");
            Infragistics.Win.Appearance appearance95 = new Infragistics.Win.Appearance();
            Infragistics.Win.UltraWinToolbars.StateButtonTool stateButtonTool73 = new Infragistics.Win.UltraWinToolbars.StateButtonTool("servicesTabReplicationViewButton", "servicesTabViewOptionSet");
            Infragistics.Win.Appearance appearance96 = new Infragistics.Win.Appearance();
            Infragistics.Win.UltraWinToolbars.StateButtonTool stateButtonTool74 = new Infragistics.Win.UltraWinToolbars.StateButtonTool("filterAgentJobsButton", "");
            Infragistics.Win.Appearance appearance97 = new Infragistics.Win.Appearance();
            Infragistics.Win.UltraWinToolbars.ButtonTool buttonTool192 = new Infragistics.Win.UltraWinToolbars.ButtonTool("showAgentJobsAllButton");
            Infragistics.Win.Appearance appearance98 = new Infragistics.Win.Appearance();
            Infragistics.Win.UltraWinToolbars.ButtonTool buttonTool193 = new Infragistics.Win.UltraWinToolbars.ButtonTool("showAgentJobsRunningOnlyButton");
            Infragistics.Win.Appearance appearance99 = new Infragistics.Win.Appearance();
            Infragistics.Win.UltraWinToolbars.ButtonTool buttonTool194 = new Infragistics.Win.UltraWinToolbars.ButtonTool("showAgentJobsFailedOnlyButton");
            Infragistics.Win.Appearance appearance100 = new Infragistics.Win.Appearance();
            Infragistics.Win.UltraWinToolbars.ButtonTool buttonTool195 = new Infragistics.Win.UltraWinToolbars.ButtonTool("toggleServicesAgentJobsHistoryVisibleButton");
            Infragistics.Win.Appearance appearance101 = new Infragistics.Win.Appearance();
            Infragistics.Win.UltraWinToolbars.ButtonTool buttonTool196 = new Infragistics.Win.UltraWinToolbars.ButtonTool("toggleServicesAgentJobsGroupByBoxVisibleButton");
            Infragistics.Win.Appearance appearance102 = new Infragistics.Win.Appearance();
            Infragistics.Win.UltraWinToolbars.StateButtonTool stateButtonTool75 = new Infragistics.Win.UltraWinToolbars.StateButtonTool("filterReplicationButton", "");
            Infragistics.Win.Appearance appearance103 = new Infragistics.Win.Appearance();
            Infragistics.Win.UltraWinToolbars.ButtonTool buttonTool197 = new Infragistics.Win.UltraWinToolbars.ButtonTool("toggleServicesReplicationTopologyGroupByBoxVisibleButton");
            Infragistics.Win.Appearance appearance104 = new Infragistics.Win.Appearance();
            Infragistics.Win.UltraWinToolbars.ButtonTool buttonTool198 = new Infragistics.Win.UltraWinToolbars.ButtonTool("toggleServicesReplicationDistributorGroupByBoxVisibleButton");
            Infragistics.Win.Appearance appearance105 = new Infragistics.Win.Appearance();
            Infragistics.Win.UltraWinToolbars.ButtonTool buttonTool199 = new Infragistics.Win.UltraWinToolbars.ButtonTool("optimizeFullTextSearchCatalogButton");
            Infragistics.Win.Appearance appearance106 = new Infragistics.Win.Appearance();
            Infragistics.Win.UltraWinToolbars.ButtonTool buttonTool200 = new Infragistics.Win.UltraWinToolbars.ButtonTool("rebuildFullTextSearchCatalogButton");
            Infragistics.Win.Appearance appearance107 = new Infragistics.Win.Appearance();
            Infragistics.Win.UltraWinToolbars.ButtonTool buttonTool201 = new Infragistics.Win.UltraWinToolbars.ButtonTool("repopulateFullTextSearchCatalogButton");
            Infragistics.Win.Appearance appearance108 = new Infragistics.Win.Appearance();
            Infragistics.Win.UltraWinToolbars.ButtonTool buttonTool202 = new Infragistics.Win.UltraWinToolbars.ButtonTool("startServicesServiceButton");
            Infragistics.Win.Appearance appearance109 = new Infragistics.Win.Appearance();
            Infragistics.Win.UltraWinToolbars.ButtonTool buttonTool203 = new Infragistics.Win.UltraWinToolbars.ButtonTool("stopServicesServiceButton");
            Infragistics.Win.Appearance appearance110 = new Infragistics.Win.Appearance();
            Infragistics.Win.UltraWinToolbars.ButtonTool buttonTool204 = new Infragistics.Win.UltraWinToolbars.ButtonTool("toggleServicesSummaryGroupByBoxVisibleButton");
            Infragistics.Win.Appearance appearance111 = new Infragistics.Win.Appearance();
            Infragistics.Win.UltraWinToolbars.ButtonTool buttonTool205 = new Infragistics.Win.UltraWinToolbars.ButtonTool("toggleServicesFullTextSearchGroupByBoxVisibleButton");
            Infragistics.Win.Appearance appearance112 = new Infragistics.Win.Appearance();
            Infragistics.Win.UltraWinToolbars.ButtonTool buttonTool206 = new Infragistics.Win.UltraWinToolbars.ButtonTool("startServicesAgentButton");
            Infragistics.Win.Appearance appearance113 = new Infragistics.Win.Appearance();
            Infragistics.Win.UltraWinToolbars.ButtonTool buttonTool207 = new Infragistics.Win.UltraWinToolbars.ButtonTool("stopServicesAgentButton");
            Infragistics.Win.Appearance appearance114 = new Infragistics.Win.Appearance();
            Infragistics.Win.UltraWinToolbars.ButtonTool buttonTool208 = new Infragistics.Win.UltraWinToolbars.ButtonTool("toggleServicesSummaryChartsButton");
            Infragistics.Win.Appearance appearance115 = new Infragistics.Win.Appearance();
            Infragistics.Win.UltraWinToolbars.ButtonTool buttonTool209 = new Infragistics.Win.UltraWinToolbars.ButtonTool("toggleServicesFullTextSearchDetailsButton");
            Infragistics.Win.Appearance appearance116 = new Infragistics.Win.Appearance();
            Infragistics.Win.UltraWinToolbars.ButtonTool buttonTool210 = new Infragistics.Win.UltraWinToolbars.ButtonTool("startServicesFullTextSearchButton");
            Infragistics.Win.Appearance appearance117 = new Infragistics.Win.Appearance();
            Infragistics.Win.UltraWinToolbars.ButtonTool buttonTool211 = new Infragistics.Win.UltraWinToolbars.ButtonTool("stopServicesFullTextSearchButton");
            Infragistics.Win.Appearance appearance118 = new Infragistics.Win.Appearance();
            Infragistics.Win.UltraWinToolbars.ButtonTool buttonTool212 = new Infragistics.Win.UltraWinToolbars.ButtonTool("configureQueryMonitorButton");
            Infragistics.Win.Appearance appearance119 = new Infragistics.Win.Appearance();
            Infragistics.Win.UltraWinToolbars.ButtonTool buttonTool213 = new Infragistics.Win.UltraWinToolbars.ButtonTool("toggleServerSummaryViewShowAnimationsButton");
            Infragistics.Win.Appearance appearance120 = new Infragistics.Win.Appearance();
            Infragistics.Win.UltraWinToolbars.StateButtonTool stateButtonTool76 = new Infragistics.Win.UltraWinToolbars.StateButtonTool("toggleHistoryBrowserButton", "");
            Infragistics.Win.Appearance appearance121 = new Infragistics.Win.Appearance();
            Infragistics.Win.UltraWinToolbars.ButtonTool buttonTool214 = new Infragistics.Win.UltraWinToolbars.ButtonTool("refreshAlertsButton");
            Infragistics.Win.Appearance appearance122 = new Infragistics.Win.Appearance();
            Infragistics.Win.UltraWinToolbars.ButtonTool buttonTool215 = new Infragistics.Win.UltraWinToolbars.ButtonTool("showPreviousSnapshotButton");
            Infragistics.Win.Appearance appearance123 = new Infragistics.Win.Appearance();
            Infragistics.Win.UltraWinToolbars.ButtonTool buttonTool216 = new Infragistics.Win.UltraWinToolbars.ButtonTool("showNextSnapshotButton");
            Infragistics.Win.Appearance appearance124 = new Infragistics.Win.Appearance();
            Infragistics.Win.UltraWinToolbars.ButtonTool buttonTool217 = new Infragistics.Win.UltraWinToolbars.ButtonTool("detailsViewShowAllCountersButton");
            Infragistics.Win.Appearance appearance125 = new Infragistics.Win.Appearance();
            Infragistics.Win.UltraWinToolbars.ButtonTool buttonTool218 = new Infragistics.Win.UltraWinToolbars.ButtonTool("detailsViewShowCustomCountersButton");
            Infragistics.Win.Appearance appearance126 = new Infragistics.Win.Appearance();
            Infragistics.Win.UltraWinToolbars.ButtonTool buttonTool219 = new Infragistics.Win.UltraWinToolbars.ButtonTool("startAgentJobButton");
            Infragistics.Win.Appearance appearance127 = new Infragistics.Win.Appearance();
            Infragistics.Win.UltraWinToolbars.ButtonTool buttonTool220 = new Infragistics.Win.UltraWinToolbars.ButtonTool("stopAgentJobButton");
            Infragistics.Win.Appearance appearance128 = new Infragistics.Win.Appearance();
            Infragistics.Win.UltraWinToolbars.StateButtonTool stateButtonTool77 = new Infragistics.Win.UltraWinToolbars.StateButtonTool("overviewTabAlertsViewButton", "overviewTabViewOptionSet");
            Infragistics.Win.Appearance appearance129 = new Infragistics.Win.Appearance();
            Infragistics.Win.UltraWinToolbars.ButtonTool buttonTool221 = new Infragistics.Win.UltraWinToolbars.ButtonTool("toggleActiveAlertsGroupByBoxButton");
            Infragistics.Win.Appearance appearance130 = new Infragistics.Win.Appearance();
            Infragistics.Win.UltraWinToolbars.ButtonTool buttonTool222 = new Infragistics.Win.UltraWinToolbars.ButtonTool("toggleActiveAlertsDetailsButton");
            Infragistics.Win.Appearance appearance131 = new Infragistics.Win.Appearance();
            Infragistics.Win.UltraWinToolbars.StateButtonTool stateButtonTool78 = new Infragistics.Win.UltraWinToolbars.StateButtonTool("queriesTabSignatureModeViewButton", "queriesTabViewOptionSet");
            Infragistics.Win.Appearance appearance132 = new Infragistics.Win.Appearance();
            Infragistics.Win.UltraWinToolbars.StateButtonTool stateButtonTool79 = new Infragistics.Win.UltraWinToolbars.StateButtonTool("queriesTabStatementModeViewButton", "queriesTabViewOptionSet");
            Infragistics.Win.Appearance appearance133 = new Infragistics.Win.Appearance();
            Infragistics.Win.UltraWinToolbars.StateButtonTool stateButtonTool80 = new Infragistics.Win.UltraWinToolbars.StateButtonTool("databasesTabMirroringViewButton", "databasesTabViewOptionSet");
            Infragistics.Win.Appearance appearance134 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance135 = new Infragistics.Win.Appearance();
            Infragistics.Win.UltraWinToolbars.ButtonTool buttonTool223 = new Infragistics.Win.UltraWinToolbars.ButtonTool("toggleDatabasesMirroredDatabasesGroupByBoxButton");
            Infragistics.Win.Appearance appearance136 = new Infragistics.Win.Appearance();
            Infragistics.Win.UltraWinToolbars.ButtonTool buttonTool224 = new Infragistics.Win.UltraWinToolbars.ButtonTool("toggleDatabaseMirroringHistoryGroupByBoxButton");
            Infragistics.Win.Appearance appearance137 = new Infragistics.Win.Appearance();
            Infragistics.Win.UltraWinToolbars.ButtonTool buttonTool225 = new Infragistics.Win.UltraWinToolbars.ButtonTool("databasesTabMirroringViewFailOverButton");
            Infragistics.Win.Appearance appearance138 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance139 = new Infragistics.Win.Appearance();
            Infragistics.Win.UltraWinToolbars.ButtonTool buttonTool226 = new Infragistics.Win.UltraWinToolbars.ButtonTool("databasesTabMirroringViewPauseButton");
            Infragistics.Win.Appearance appearance140 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance141 = new Infragistics.Win.Appearance();
            Infragistics.Win.UltraWinToolbars.ButtonTool buttonTool227 = new Infragistics.Win.UltraWinToolbars.ButtonTool("toggleServicesReplicationReplicationGraphsVisibleButton");
            Infragistics.Win.Appearance appearance142 = new Infragistics.Win.Appearance();
            Infragistics.Win.UltraWinToolbars.StateButtonTool stateButtonTool81 = new Infragistics.Win.UltraWinToolbars.StateButtonTool("resourcesTabWaitStatsViewButton", "resourcesTabViewOptionSet");
            Infragistics.Win.Appearance appearance143 = new Infragistics.Win.Appearance();
            Infragistics.Win.UltraWinToolbars.StateButtonTool stateButtonTool82 = new Infragistics.Win.UltraWinToolbars.StateButtonTool("Active Waits", "");
            Infragistics.Win.UltraWinToolbars.StateButtonTool stateButtonTool83 = new Infragistics.Win.UltraWinToolbars.StateButtonTool("resourcesTabWaitStatsActiveViewButton", "queriesTabViewOptionSet");
            Infragistics.Win.Appearance appearance144 = new Infragistics.Win.Appearance();
            Infragistics.Win.UltraWinToolbars.ButtonTool buttonTool228 = new Infragistics.Win.UltraWinToolbars.ButtonTool("resourcesQueryWaitsConfigureTopXButton");
            Infragistics.Win.Appearance appearance145 = new Infragistics.Win.Appearance();
            Infragistics.Win.UltraWinToolbars.ButtonTool buttonTool229 = new Infragistics.Win.UltraWinToolbars.ButtonTool("resourcesQueryWaitsConfigureButton");
            Infragistics.Win.Appearance appearance146 = new Infragistics.Win.Appearance();
            Infragistics.Win.UltraWinToolbars.ButtonTool buttonTool230 = new Infragistics.Win.UltraWinToolbars.ButtonTool("toggleWaitStatsGroupByBoxButton");
            Infragistics.Win.Appearance appearance147 = new Infragistics.Win.Appearance();
            Infragistics.Win.UltraWinToolbars.ButtonTool buttonTool231 = new Infragistics.Win.UltraWinToolbars.ButtonTool("toggleQueryMonitorGridButton");
            Infragistics.Win.Appearance appearance148 = new Infragistics.Win.Appearance();
            Infragistics.Win.UltraWinToolbars.StateButtonTool stateButtonTool84 = new Infragistics.Win.UltraWinToolbars.StateButtonTool("queriesTabHistoryModeViewButton", "queriesTabViewOptionSet");
            Infragistics.Win.Appearance appearance149 = new Infragistics.Win.Appearance();
            Infragistics.Win.UltraWinToolbars.StateButtonTool stateButtonTool85 = new Infragistics.Win.UltraWinToolbars.StateButtonTool("resourcesTabFileActivityViewButton", "resourcesTabViewOptionSet");
            Infragistics.Win.Appearance appearance150 = new Infragistics.Win.Appearance();
            Infragistics.Win.UltraWinToolbars.ButtonTool buttonTool232 = new Infragistics.Win.UltraWinToolbars.ButtonTool("showFileActivityFilterButton");
            Infragistics.Win.UltraWinToolbars.ButtonTool buttonTool233 = new Infragistics.Win.UltraWinToolbars.ButtonTool("resourcesShowFileActivityFilterButton");
            Infragistics.Win.UltraWinToolbars.ButtonTool buttonTool234 = new Infragistics.Win.UltraWinToolbars.ButtonTool("pulseProfileButton");
            Infragistics.Win.Appearance appearance151 = new Infragistics.Win.Appearance();
            Infragistics.Win.UltraWinToolbars.ButtonTool buttonTool235 = new Infragistics.Win.UltraWinToolbars.ButtonTool("pulsePostButton");
            Infragistics.Win.Appearance appearance152 = new Infragistics.Win.Appearance();
            Infragistics.Win.UltraWinToolbars.ButtonTool buttonTool236 = new Infragistics.Win.UltraWinToolbars.ButtonTool("pulseFollowButton");
            Infragistics.Win.Appearance appearance153 = new Infragistics.Win.Appearance();
            Infragistics.Win.UltraWinToolbars.ButtonTool buttonTool237 = new Infragistics.Win.UltraWinToolbars.ButtonTool("toggleActiveAlertsForecastButton");
            Infragistics.Win.Appearance appearance154 = new Infragistics.Win.Appearance();
            Infragistics.Win.UltraWinToolbars.ButtonTool buttonTool238 = new Infragistics.Win.UltraWinToolbars.ButtonTool("toggleServerDetailsPropertiesButton");
            Infragistics.Win.Appearance appearance155 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance156 = new Infragistics.Win.Appearance();
            Infragistics.Win.UltraWinToolbars.ButtonTool buttonTool239 = new Infragistics.Win.UltraWinToolbars.ButtonTool("databasesTabTempdbSummaryView");
            Infragistics.Win.Appearance appearance157 = new Infragistics.Win.Appearance();
            Infragistics.Win.UltraWinToolbars.StateButtonTool stateButtonTool86 = new Infragistics.Win.UltraWinToolbars.StateButtonTool("databasesTabTempdbSummaryViewButton", "databasesTabViewOptionSet");
            Infragistics.Win.Appearance appearance158 = new Infragistics.Win.Appearance();
            Infragistics.Win.UltraWinToolbars.ButtonTool buttonTool240 = new Infragistics.Win.UltraWinToolbars.ButtonTool("History Browser");
            Infragistics.Win.UltraWinToolbars.StateButtonTool stateButtonTool87 = new Infragistics.Win.UltraWinToolbars.StateButtonTool("filterDatabasesTempdbSessionsButton", "");
            Infragistics.Win.Appearance appearance159 = new Infragistics.Win.Appearance();
            Infragistics.Win.UltraWinToolbars.ButtonTool buttonTool241 = new Infragistics.Win.UltraWinToolbars.ButtonTool("toggleDatabasesTempdbGroupByBoxButton");
            Infragistics.Win.Appearance appearance160 = new Infragistics.Win.Appearance();
            Infragistics.Win.UltraWinToolbars.StateButtonTool stateButtonTool88 = new Infragistics.Win.UltraWinToolbars.StateButtonTool("resourcesTabDiskSizeViewButton", "");
            Infragistics.Win.Appearance appearanceRunAnalysis = new Infragistics.Win.Appearance();
            Infragistics.Win.UltraWinToolbars.StateButtonTool stateButtonToolRunAnalysis = new Infragistics.Win.UltraWinToolbars.StateButtonTool("analysisRunAnalysisButton", "analysisTabViewOptionSet");
            
            this.subViewContainerPanel = new System.Windows.Forms.Panel();
            this._ServerViewContainer_Toolbars_Dock_Area_Left = new Infragistics.Win.UltraWinToolbars.UltraToolbarsDockArea();
            this.toolbarsManager = new Idera.SQLdm.DesktopClient.Controls.ContextMenuManager(this.components);
            this._ServerViewContainer_Toolbars_Dock_Area_Right = new Infragistics.Win.UltraWinToolbars.UltraToolbarsDockArea();
            this._ServerViewContainer_Toolbars_Dock_Area_Top = new Infragistics.Win.UltraWinToolbars.UltraToolbarsDockArea();
            this._ServerViewContainer_Toolbars_Dock_Area_Bottom = new Infragistics.Win.UltraWinToolbars.UltraToolbarsDockArea();
            ((System.ComponentModel.ISupportInitialize)(this.toolbarsManager)).BeginInit();
            this.SuspendLayout();
            // 
            // subViewContainerPanel
            // 
            this.subViewContainerPanel.Cursor = System.Windows.Forms.Cursors.Default;
            this.subViewContainerPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.subViewContainerPanel.Location = new System.Drawing.Point(0, 0);
            this.subViewContainerPanel.Name = "subViewContainerPanel";
            this.subViewContainerPanel.Size = new System.Drawing.Size(743, 615);
            this.subViewContainerPanel.TabIndex = 0;
            // 
            // _ServerViewContainer_Toolbars_Dock_Area_Left
            // 
            this._ServerViewContainer_Toolbars_Dock_Area_Left.AccessibleRole = System.Windows.Forms.AccessibleRole.Grouping;
            this._ServerViewContainer_Toolbars_Dock_Area_Left.BackColor = System.Drawing.SystemColors.Control;
            this._ServerViewContainer_Toolbars_Dock_Area_Left.DockedPosition = Infragistics.Win.UltraWinToolbars.DockedPosition.Left;
            this._ServerViewContainer_Toolbars_Dock_Area_Left.ForeColor = System.Drawing.SystemColors.ControlText;
            this._ServerViewContainer_Toolbars_Dock_Area_Left.Location = new System.Drawing.Point(0, 0);
            this._ServerViewContainer_Toolbars_Dock_Area_Left.Name = "_ServerViewContainer_Toolbars_Dock_Area_Left";
            this._ServerViewContainer_Toolbars_Dock_Area_Left.Size = new System.Drawing.Size(0, 615);
            this._ServerViewContainer_Toolbars_Dock_Area_Left.ToolbarsManager = this.toolbarsManager;
            // 
            // toolbarsManager
            // 
            this.toolbarsManager.DesignerFlags = 1;
            this.toolbarsManager.DockWithinContainer = this;
            this.toolbarsManager.Office2007UICompatibility = false;
            optionSet1.AllowAllUp = false;
            optionSet2.AllowAllUp = false;
            optionSet3.AllowAllUp = false;
            optionSet4.AllowAllUp = false;
            optionSet5.AllowAllUp = false;
            optionSet6.AllowAllUp = false;
            this.toolbarsManager.OptionSets.Add(optionSet1);
            this.toolbarsManager.OptionSets.Add(optionSet2);
            this.toolbarsManager.OptionSets.Add(optionSet3);
            this.toolbarsManager.OptionSets.Add(optionSet4);
            this.toolbarsManager.OptionSets.Add(optionSet5);
            this.toolbarsManager.OptionSets.Add(optionSet6);
            this.toolbarsManager.Ribbon.FileMenuStyle = Infragistics.Win.UltraWinToolbars.FileMenuStyle.None;
            ribbonTab1.Caption = "Overview";
            ribbonGroup1.Caption = "Views";
            stateButtonTool1.InstanceProps.MinimumSizeOnRibbon = Infragistics.Win.UltraWinToolbars.RibbonToolSize.Large;
            stateButtonTool1.InstanceProps.PreferredSizeOnRibbon = Infragistics.Win.UltraWinToolbars.RibbonToolSize.Large;
            stateButtonTool1.MenuDisplayStyle = Infragistics.Win.UltraWinToolbars.StateButtonMenuDisplayStyle.DisplayCheckmark;
            stateButtonTool2.InstanceProps.MinimumSizeOnRibbon = Infragistics.Win.UltraWinToolbars.RibbonToolSize.Large;
            stateButtonTool2.InstanceProps.PreferredSizeOnRibbon = Infragistics.Win.UltraWinToolbars.RibbonToolSize.Large;
            stateButtonTool2.MenuDisplayStyle = Infragistics.Win.UltraWinToolbars.StateButtonMenuDisplayStyle.DisplayCheckmark;
            stateButtonTool3.InstanceProps.MinimumSizeOnRibbon = Infragistics.Win.UltraWinToolbars.RibbonToolSize.Large;
            stateButtonTool3.InstanceProps.PreferredSizeOnRibbon = Infragistics.Win.UltraWinToolbars.RibbonToolSize.Large;
            stateButtonTool3.MenuDisplayStyle = Infragistics.Win.UltraWinToolbars.StateButtonMenuDisplayStyle.DisplayCheckmark;
            stateButtonTool4.InstanceProps.MinimumSizeOnRibbon = Infragistics.Win.UltraWinToolbars.RibbonToolSize.Large;
            stateButtonTool4.InstanceProps.PreferredSizeOnRibbon = Infragistics.Win.UltraWinToolbars.RibbonToolSize.Large;
            stateButtonTool4.MenuDisplayStyle = Infragistics.Win.UltraWinToolbars.StateButtonMenuDisplayStyle.DisplayCheckmark;
            ribbonGroup1.Tools.AddRange(new Infragistics.Win.UltraWinToolbars.ToolBase[] {
            stateButtonTool1,
            stateButtonTool2,
            stateButtonTool3,
            stateButtonTool4});
            ribbonGroup2.Caption = "History";
            stateButtonTool5.InstanceProps.PreferredSizeOnRibbon = Infragistics.Win.UltraWinToolbars.RibbonToolSize.Large;
            buttonTool1.InstanceProps.IsFirstInGroup = true;
            buttonTool1.InstanceProps.MinimumSizeOnRibbon = Infragistics.Win.UltraWinToolbars.RibbonToolSize.Large;
            buttonTool2.InstanceProps.MinimumSizeOnRibbon = Infragistics.Win.UltraWinToolbars.RibbonToolSize.Large;
            ribbonGroup2.Tools.AddRange(new Infragistics.Win.UltraWinToolbars.ToolBase[] {
            stateButtonTool5,
            buttonTool1,
            buttonTool2});
            ribbonGroup3.Caption = "Actions";
            buttonTool3.InstanceProps.PreferredSizeOnRibbon = Infragistics.Win.UltraWinToolbars.RibbonToolSize.Large;
            buttonTool4.InstanceProps.MinimumSizeOnRibbon = Infragistics.Win.UltraWinToolbars.RibbonToolSize.Large;
            stateButtonTool6.InstanceProps.PreferredSizeOnRibbon = Infragistics.Win.UltraWinToolbars.RibbonToolSize.Large;
            ribbonGroup3.Tools.AddRange(new Infragistics.Win.UltraWinToolbars.ToolBase[] {
            buttonTool3,
            buttonTool4,
            stateButtonTool6});
            ribbonGroup4.Caption = "Options";
            ribbonGroup4.Tools.AddRange(new Infragistics.Win.UltraWinToolbars.ToolBase[] {
            buttonTool5});
            ribbonGroup5.Caption = "Show/Hide";
            ribbonGroup5.Tools.AddRange(new Infragistics.Win.UltraWinToolbars.ToolBase[] {
            buttonTool6,
            buttonTool7});
            ribbonGroup6.Caption = "Show/Hide";
            ribbonGroup6.Tools.AddRange(new Infragistics.Win.UltraWinToolbars.ToolBase[] {
            buttonTool8,
            buttonTool9,
            buttonTool10});
            ribbonGroup7.Caption = "Show/Hide";
            ribbonGroup7.Tools.AddRange(new Infragistics.Win.UltraWinToolbars.ToolBase[] {
            buttonTool11,
            buttonTool12,
            buttonTool13});
            ribbonGroup8.Caption = "Filter";
            ribbonGroup8.Tools.AddRange(new Infragistics.Win.UltraWinToolbars.ToolBase[] {
            buttonTool14,
            buttonTool15});
            ribbonGroup9.Caption = "Newsfeed";
            buttonTool16.InstanceProps.MinimumSizeOnRibbon = Infragistics.Win.UltraWinToolbars.RibbonToolSize.Large;
            buttonTool16.InstanceProps.PreferredSizeOnRibbon = Infragistics.Win.UltraWinToolbars.RibbonToolSize.Large;
            buttonTool17.InstanceProps.MinimumSizeOnRibbon = Infragistics.Win.UltraWinToolbars.RibbonToolSize.Large;
            buttonTool17.InstanceProps.PreferredSizeOnRibbon = Infragistics.Win.UltraWinToolbars.RibbonToolSize.Large;
            buttonTool18.InstanceProps.MinimumSizeOnRibbon = Infragistics.Win.UltraWinToolbars.RibbonToolSize.Large;
            buttonTool18.InstanceProps.PreferredSizeOnRibbon = Infragistics.Win.UltraWinToolbars.RibbonToolSize.Large;
            ribbonGroup9.Tools.AddRange(new Infragistics.Win.UltraWinToolbars.ToolBase[] {
            buttonTool16,
            buttonTool17,
            buttonTool18});
            ribbonTab1.Groups.AddRange(new Infragistics.Win.UltraWinToolbars.RibbonGroup[] {
            ribbonGroup1,
            ribbonGroup2,
            ribbonGroup3,
            ribbonGroup4,
            ribbonGroup5,
            ribbonGroup6,
            ribbonGroup7,
            ribbonGroup8,
            ribbonGroup9});
            appearance1.Image = ((object)(resources.GetObject("appearance1.Image")));
            ribbonTab1.Settings.TabItemAppearance = appearance1;
            ribbonTab2.Caption = "Sessions";
            ribbonGroup10.Caption = "Views";
            stateButtonTool7.InstanceProps.MinimumSizeOnRibbon = Infragistics.Win.UltraWinToolbars.RibbonToolSize.Large;
            stateButtonTool7.InstanceProps.PreferredSizeOnRibbon = Infragistics.Win.UltraWinToolbars.RibbonToolSize.Large;
            stateButtonTool8.InstanceProps.MinimumSizeOnRibbon = Infragistics.Win.UltraWinToolbars.RibbonToolSize.Large;
            stateButtonTool9.InstanceProps.MinimumSizeOnRibbon = Infragistics.Win.UltraWinToolbars.RibbonToolSize.Large;
            stateButtonTool10.InstanceProps.MinimumSizeOnRibbon = Infragistics.Win.UltraWinToolbars.RibbonToolSize.Large;
            ribbonGroup10.Tools.AddRange(new Infragistics.Win.UltraWinToolbars.ToolBase[] {
            stateButtonTool7,
            stateButtonTool8,
            stateButtonTool9,
            stateButtonTool10});
            ribbonGroup11.Caption = "History";
            stateButtonTool11.InstanceProps.MinimumSizeOnRibbon = Infragistics.Win.UltraWinToolbars.RibbonToolSize.Large;
            buttonTool19.InstanceProps.IsFirstInGroup = true;
            buttonTool19.InstanceProps.MinimumSizeOnRibbon = Infragistics.Win.UltraWinToolbars.RibbonToolSize.Large;
            buttonTool20.InstanceProps.MinimumSizeOnRibbon = Infragistics.Win.UltraWinToolbars.RibbonToolSize.Large;
            ribbonGroup11.Tools.AddRange(new Infragistics.Win.UltraWinToolbars.ToolBase[] {
            stateButtonTool11,
            buttonTool19,
            buttonTool20});
            ribbonGroup12.Caption = "Actions";
            buttonTool21.InstanceProps.MinimumSizeOnRibbon = Infragistics.Win.UltraWinToolbars.RibbonToolSize.Large;
            buttonTool22.InstanceProps.MinimumSizeOnRibbon = Infragistics.Win.UltraWinToolbars.RibbonToolSize.Large;
            ribbonGroup12.Tools.AddRange(new Infragistics.Win.UltraWinToolbars.ToolBase[] {
            buttonTool21,
            buttonTool22});
            ribbonGroup13.Caption = "Filter";
            stateButtonTool12.InstanceProps.MinimumSizeOnRibbon = Infragistics.Win.UltraWinToolbars.RibbonToolSize.Large;
            ribbonGroup13.Tools.AddRange(new Infragistics.Win.UltraWinToolbars.ToolBase[] {
            stateButtonTool12,
            buttonTool23,
            buttonTool24,
            buttonTool25});
            ribbonGroup14.Caption = "Show/Hide";
            ribbonGroup14.Tools.AddRange(new Infragistics.Win.UltraWinToolbars.ToolBase[] {
            buttonTool26,
            buttonTool27});
            ribbonGroup15.Caption = "Actions";
            buttonTool28.InstanceProps.MinimumSizeOnRibbon = Infragistics.Win.UltraWinToolbars.RibbonToolSize.Large;
            buttonTool29.InstanceProps.MinimumSizeOnRibbon = Infragistics.Win.UltraWinToolbars.RibbonToolSize.Large;
            ribbonGroup15.Tools.AddRange(new Infragistics.Win.UltraWinToolbars.ToolBase[] {
            buttonTool28,
            buttonTool29});
            ribbonGroup16.Caption = "Filter";
            stateButtonTool13.InstanceProps.MinimumSizeOnRibbon = Infragistics.Win.UltraWinToolbars.RibbonToolSize.Large;
            ribbonGroup16.Tools.AddRange(new Infragistics.Win.UltraWinToolbars.ToolBase[] {
            stateButtonTool13,
            buttonTool30,
            buttonTool31});
            ribbonGroup17.Caption = "Show/Hide";
            ribbonGroup17.Tools.AddRange(new Infragistics.Win.UltraWinToolbars.ToolBase[] {
            buttonTool32,
            buttonTool33});
            ribbonGroup18.Caption = "Actions";
            buttonTool34.InstanceProps.MinimumSizeOnRibbon = Infragistics.Win.UltraWinToolbars.RibbonToolSize.Large;
            buttonTool35.InstanceProps.MinimumSizeOnRibbon = Infragistics.Win.UltraWinToolbars.RibbonToolSize.Large;
            ribbonGroup18.Tools.AddRange(new Infragistics.Win.UltraWinToolbars.ToolBase[] {
            buttonTool34,
            buttonTool35});
            ribbonGroup19.Caption = "Blocking Tree";
            ribbonGroup19.Tools.AddRange(new Infragistics.Win.UltraWinToolbars.ToolBase[] {
            buttonTool36,
            buttonTool37});
            ribbonGroup20.Caption = "Show/Hide";
            ribbonGroup20.Tools.AddRange(new Infragistics.Win.UltraWinToolbars.ToolBase[] {
            buttonTool38});
            ribbonGroup21.Caption = "Newsfeed";
            buttonTool39.InstanceProps.MinimumSizeOnRibbon = Infragistics.Win.UltraWinToolbars.RibbonToolSize.Large;
            buttonTool40.InstanceProps.MinimumSizeOnRibbon = Infragistics.Win.UltraWinToolbars.RibbonToolSize.Large;
            buttonTool41.InstanceProps.MinimumSizeOnRibbon = Infragistics.Win.UltraWinToolbars.RibbonToolSize.Large;
            ribbonGroup21.Tools.AddRange(new Infragistics.Win.UltraWinToolbars.ToolBase[] {
            buttonTool39,
            buttonTool40,
            buttonTool41});
            ribbonTab2.Groups.AddRange(new Infragistics.Win.UltraWinToolbars.RibbonGroup[] {
            ribbonGroup10,
            ribbonGroup11,
            ribbonGroup12,
            ribbonGroup13,
            ribbonGroup14,
            ribbonGroup15,
            ribbonGroup16,
            ribbonGroup17,
            ribbonGroup18,
            ribbonGroup19,
            ribbonGroup20,
            ribbonGroup21});
            appearance2.Image = ((object)(resources.GetObject("appearance2.Image")));
            ribbonTab2.Settings.TabItemAppearance = appearance2;
            ribbonTab3.Caption = "Queries";
            ribbonGroup22.Caption = "View";
            stateButtonTool14.InstanceProps.PreferredSizeOnRibbon = Infragistics.Win.UltraWinToolbars.RibbonToolSize.Large;
            stateButtonTool15.InstanceProps.PreferredSizeOnRibbon = Infragistics.Win.UltraWinToolbars.RibbonToolSize.Large;
            stateButtonTool16.InstanceProps.PreferredSizeOnRibbon = Infragistics.Win.UltraWinToolbars.RibbonToolSize.Large;
            stateButtonTool17.InstanceProps.MinimumSizeOnRibbon = Infragistics.Win.UltraWinToolbars.RibbonToolSize.Normal;
            stateButtonTool17.InstanceProps.PreferredSizeOnRibbon = Infragistics.Win.UltraWinToolbars.RibbonToolSize.Large;
            ribbonGroup22.Tools.AddRange(new Infragistics.Win.UltraWinToolbars.ToolBase[] {
            stateButtonTool14,
            stateButtonTool15,
            stateButtonTool16,
            stateButtonTool17});
            ribbonGroup23.Caption = "History";
            stateButtonTool18.InstanceProps.MinimumSizeOnRibbon = Infragistics.Win.UltraWinToolbars.RibbonToolSize.Large;
            buttonTool42.InstanceProps.IsFirstInGroup = true;
            buttonTool42.InstanceProps.MinimumSizeOnRibbon = Infragistics.Win.UltraWinToolbars.RibbonToolSize.Large;
            buttonTool43.InstanceProps.MinimumSizeOnRibbon = Infragistics.Win.UltraWinToolbars.RibbonToolSize.Large;
            ribbonGroup23.Tools.AddRange(new Infragistics.Win.UltraWinToolbars.ToolBase[] {
            stateButtonTool18,
            buttonTool42,
            buttonTool43});
            ribbonGroup24.Caption = "Properties";
            buttonTool44.InstanceProps.MinimumSizeOnRibbon = Infragistics.Win.UltraWinToolbars.RibbonToolSize.Large;
            buttonTool44.InstanceProps.PreferredSizeOnRibbon = Infragistics.Win.UltraWinToolbars.RibbonToolSize.Large;
            ribbonGroup24.Tools.AddRange(new Infragistics.Win.UltraWinToolbars.ToolBase[] {
            buttonTool44});
            ribbonGroup25.Caption = "Filter";
            stateButtonTool19.InstanceProps.MinimumSizeOnRibbon = Infragistics.Win.UltraWinToolbars.RibbonToolSize.Large;
            ribbonGroup25.Tools.AddRange(new Infragistics.Win.UltraWinToolbars.ToolBase[] {
            stateButtonTool19,
            buttonTool45,
            buttonTool46,
            buttonTool47});
            ribbonGroup26.Caption = "Show/Hide";
            ribbonGroup26.Tools.AddRange(new Infragistics.Win.UltraWinToolbars.ToolBase[] {
            buttonTool48,
            buttonTool49,
            buttonTool50});
            ribbonGroup27.Caption = "Newsfeed";
            buttonTool51.InstanceProps.MinimumSizeOnRibbon = Infragistics.Win.UltraWinToolbars.RibbonToolSize.Large;
            buttonTool52.InstanceProps.MinimumSizeOnRibbon = Infragistics.Win.UltraWinToolbars.RibbonToolSize.Large;
            buttonTool53.InstanceProps.MinimumSizeOnRibbon = Infragistics.Win.UltraWinToolbars.RibbonToolSize.Large;
            ribbonGroup27.Tools.AddRange(new Infragistics.Win.UltraWinToolbars.ToolBase[] {
            buttonTool51,
            buttonTool52,
            buttonTool53});
            ribbonTab3.Groups.AddRange(new Infragistics.Win.UltraWinToolbars.RibbonGroup[] {
            ribbonGroup22,
            ribbonGroup23,
            ribbonGroup24,
            ribbonGroup25,
            ribbonGroup26,
            ribbonGroup27});
            appearance3.Image = ((object)(resources.GetObject("appearance3.Image")));
            ribbonTab3.Settings.TabItemAppearance = appearance3;
            ribbonTab4.Caption = "Resources";
            ribbonGroup28.Caption = "Views";
            stateButtonTool20.Checked = true;
            stateButtonTool20.InstanceProps.MinimumSizeOnRibbon = Infragistics.Win.UltraWinToolbars.RibbonToolSize.Large;
            stateButtonTool21.InstanceProps.MinimumSizeOnRibbon = Infragistics.Win.UltraWinToolbars.RibbonToolSize.Large;
            stateButtonTool22.InstanceProps.MinimumSizeOnRibbon = Infragistics.Win.UltraWinToolbars.RibbonToolSize.Large;
            stateButtonTool23.InstanceProps.MinimumSizeOnRibbon = Infragistics.Win.UltraWinToolbars.RibbonToolSize.Large;            
            stateButtonTool26.InstanceProps.MinimumSizeOnRibbon = Infragistics.Win.UltraWinToolbars.RibbonToolSize.Large;
            stateButtonTool27.InstanceProps.PreferredSizeOnRibbon = Infragistics.Win.UltraWinToolbars.RibbonToolSize.Large;
            if (true)
            {
                ribbonGroup28.Tools.AddRange(new Infragistics.Win.UltraWinToolbars.ToolBase[] {
            stateButtonTool20,
            stateButtonTool21,
            stateButtonTool22,
            stateButtonTool23,
            stateButtonTool26,
            stateButtonTool27});
            }
            else
            {
                stateButtonTool24.InstanceProps.MinimumSizeOnRibbon = Infragistics.Win.UltraWinToolbars.RibbonToolSize.Large;
                stateButtonTool25.InstanceProps.MinimumSizeOnRibbon = Infragistics.Win.UltraWinToolbars.RibbonToolSize.Normal;
                stateButtonTool25.InstanceProps.PreferredSizeOnRibbon = Infragistics.Win.UltraWinToolbars.RibbonToolSize.Large;
                ribbonGroup28.Tools.AddRange(new Infragistics.Win.UltraWinToolbars.ToolBase[] {
            stateButtonTool20,
            stateButtonTool21,
            stateButtonTool22,
            stateButtonTool23,
            stateButtonTool24,
            stateButtonTool25,
            stateButtonTool26,
            stateButtonTool27});
            }
            ribbonGroup29.Caption = "History";
            stateButtonTool28.InstanceProps.MinimumSizeOnRibbon = Infragistics.Win.UltraWinToolbars.RibbonToolSize.Large;
            buttonTool54.InstanceProps.IsFirstInGroup = true;
            buttonTool54.InstanceProps.MinimumSizeOnRibbon = Infragistics.Win.UltraWinToolbars.RibbonToolSize.Large;
            buttonTool54.InstanceProps.PreferredSizeOnRibbon = Infragistics.Win.UltraWinToolbars.RibbonToolSize.Large;
            buttonTool55.InstanceProps.MinimumSizeOnRibbon = Infragistics.Win.UltraWinToolbars.RibbonToolSize.Large;
            ribbonGroup29.Tools.AddRange(new Infragistics.Win.UltraWinToolbars.ToolBase[] {
            stateButtonTool28,
            buttonTool54,
            buttonTool55});
            ribbonGroup30.Caption = "Actions";
            buttonTool56.InstanceProps.MinimumSizeOnRibbon = Infragistics.Win.UltraWinToolbars.RibbonToolSize.Large;
            ribbonGroup30.Tools.AddRange(new Infragistics.Win.UltraWinToolbars.ToolBase[] {
            buttonTool56});
            ribbonGroup31.Caption = "Show/Hide";
            ribbonGroup31.Tools.AddRange(new Infragistics.Win.UltraWinToolbars.ToolBase[] {
            buttonTool57,
            buttonTool58});
            ribbonGroup81.Caption = "Show/Hide";
            ribbonGroup81.Tools.AddRange(new Infragistics.Win.UltraWinToolbars.ToolBase[] {
            buttonTool242});
            ribbonGroup32.Caption = "Actions";
            buttonTool59.InstanceProps.IsFirstInGroup = true;
            buttonTool59.InstanceProps.PreferredSizeOnRibbon = Infragistics.Win.UltraWinToolbars.RibbonToolSize.Large;
            buttonTool60.InstanceProps.PreferredSizeOnRibbon = Infragistics.Win.UltraWinToolbars.RibbonToolSize.Large;
            ribbonGroup32.Tools.AddRange(new Infragistics.Win.UltraWinToolbars.ToolBase[] {
            buttonTool59,
            buttonTool60});
            ribbonGroup33.Caption = "Show/Hide";
            ribbonGroup33.Tools.AddRange(new Infragistics.Win.UltraWinToolbars.ToolBase[] {
            buttonTool61});
            ribbonGroup34.Caption = "Show/Hide";
            ribbonGroup34.Tools.AddRange(new Infragistics.Win.UltraWinToolbars.ToolBase[] {
            buttonTool62});
            ribbonGroup35.Caption = "Newsfeed";
            buttonTool63.InstanceProps.MinimumSizeOnRibbon = Infragistics.Win.UltraWinToolbars.RibbonToolSize.Large;
            buttonTool64.InstanceProps.MinimumSizeOnRibbon = Infragistics.Win.UltraWinToolbars.RibbonToolSize.Large;
            buttonTool65.InstanceProps.MinimumSizeOnRibbon = Infragistics.Win.UltraWinToolbars.RibbonToolSize.Large;
            ribbonGroup35.Tools.AddRange(new Infragistics.Win.UltraWinToolbars.ToolBase[] {
            buttonTool63,
            buttonTool64,
            buttonTool65});
            ribbonTab4.Groups.AddRange(new Infragistics.Win.UltraWinToolbars.RibbonGroup[] {
            ribbonGroup28,
            ribbonGroup29,
            ribbonGroup30,
            ribbonGroup31,
            ribbonGroup32,
            ribbonGroup33,
            ribbonGroup34,
            ribbonGroup35,
            ribbonGroup81});
            appearance4.Image = ((object)(resources.GetObject("appearance4.Image")));
            ribbonTab4.Settings.TabItemAppearance = appearance4;
            ribbonTab5.Caption = "Databases";
            ribbonGroup36.Caption = "Views";
            stateButtonTool29.InstanceProps.MinimumSizeOnRibbon = Infragistics.Win.UltraWinToolbars.RibbonToolSize.Large;
            stateButtonTool29.InstanceProps.PreferredSizeOnRibbon = Infragistics.Win.UltraWinToolbars.RibbonToolSize.Large;
            stateButtonTool30.InstanceProps.MinimumSizeOnRibbon = Infragistics.Win.UltraWinToolbars.RibbonToolSize.Large;
            stateButtonTool31.InstanceProps.MinimumSizeOnRibbon = Infragistics.Win.UltraWinToolbars.RibbonToolSize.Large;
            stateButtonTool32.InstanceProps.MinimumSizeOnRibbon = Infragistics.Win.UltraWinToolbars.RibbonToolSize.Large;
            stateButtonTool33.InstanceProps.MinimumSizeOnRibbon = Infragistics.Win.UltraWinToolbars.RibbonToolSize.Large;
            stateButtonTool34.InstanceProps.MinimumSizeOnRibbon = Infragistics.Win.UltraWinToolbars.RibbonToolSize.Large;
            stateButtonTool35.InstanceProps.MinimumSizeOnRibbon = Infragistics.Win.UltraWinToolbars.RibbonToolSize.Large;
            ribbonGroup36.Tools.AddRange(new Infragistics.Win.UltraWinToolbars.ToolBase[] {
            stateButtonTool29,
            stateButtonTool30,
            stateButtonTool31,
            stateButtonTool32,
            stateButtonTool33,
            stateButtonTool34,
            stateButtonTool35});
            ribbonGroup37.Caption = "Filter";
            ribbonGroup37.Tools.AddRange(new Infragistics.Win.UltraWinToolbars.ToolBase[] {
            buttonTool66});
            ribbonGroup38.Caption = "Show/Hide";
            ribbonGroup38.Tools.AddRange(new Infragistics.Win.UltraWinToolbars.ToolBase[] {
            buttonTool67,
            buttonTool68});
            ribbonGroup39.Caption = "Filter";
            ribbonGroup39.Tools.AddRange(new Infragistics.Win.UltraWinToolbars.ToolBase[] {
            buttonTool69});
            ribbonGroup40.Caption = "Show/Hide";
            ribbonGroup40.Tools.AddRange(new Infragistics.Win.UltraWinToolbars.ToolBase[] {
            buttonTool70});
            ribbonGroup41.Caption = "Filter";
            stateButtonTool36.InstanceProps.MinimumSizeOnRibbon = Infragistics.Win.UltraWinToolbars.RibbonToolSize.Large;
            stateButtonTool36.InstanceProps.PreferredSizeOnRibbon = Infragistics.Win.UltraWinToolbars.RibbonToolSize.Large;
            ribbonGroup41.Tools.AddRange(new Infragistics.Win.UltraWinToolbars.ToolBase[] {
            stateButtonTool36,
            buttonTool71,
            buttonTool72,
            buttonTool73});
            ribbonGroup42.Caption = "Show/Hide";
            ribbonGroup42.Tools.AddRange(new Infragistics.Win.UltraWinToolbars.ToolBase[] {
            buttonTool74,
            buttonTool75});
            ribbonGroup43.Caption = "Filter";
            ribbonGroup43.Tools.AddRange(new Infragistics.Win.UltraWinToolbars.ToolBase[] {
            buttonTool76});
            ribbonGroup44.Caption = "Show/Hide";
            ribbonGroup44.Tools.AddRange(new Infragistics.Win.UltraWinToolbars.ToolBase[] {
            buttonTool77});
            ribbonGroup45.Caption = "Actions";
            buttonTool78.InstanceProps.MinimumSizeOnRibbon = Infragistics.Win.UltraWinToolbars.RibbonToolSize.Large;
            buttonTool79.InstanceProps.MinimumSizeOnRibbon = Infragistics.Win.UltraWinToolbars.RibbonToolSize.Large;
            ribbonGroup45.Tools.AddRange(new Infragistics.Win.UltraWinToolbars.ToolBase[] {
            buttonTool78,
            buttonTool79});
            ribbonGroup46.Caption = "Filter";
            ribbonGroup46.Tools.AddRange(new Infragistics.Win.UltraWinToolbars.ToolBase[] {
            buttonTool80,
            buttonTool81});
            ribbonGroup47.Caption = "Show/Hide";
            ribbonGroup47.Tools.AddRange(new Infragistics.Win.UltraWinToolbars.ToolBase[] {
            buttonTool82,
            buttonTool83});
            ribbonGroup48.Caption = "Show/Hide";
            ribbonGroup48.Tools.AddRange(new Infragistics.Win.UltraWinToolbars.ToolBase[] {
            buttonTool84,
            buttonTool85});
            ribbonGroup49.Caption = "Actions";
            buttonTool86.InstanceProps.MinimumSizeOnRibbon = Infragistics.Win.UltraWinToolbars.RibbonToolSize.Large;
            appearance5.Image = ((object)(resources.GetObject("appearance5.Image")));
            buttonTool87.InstanceProps.AppearancesSmall.Appearance = appearance5;
            buttonTool87.InstanceProps.MinimumSizeOnRibbon = Infragistics.Win.UltraWinToolbars.RibbonToolSize.Large;
            ribbonGroup49.Tools.AddRange(new Infragistics.Win.UltraWinToolbars.ToolBase[] {
            buttonTool86,
            buttonTool87});
            ribbonGroup50.Caption = "Newsfeed";
            buttonTool88.InstanceProps.MinimumSizeOnRibbon = Infragistics.Win.UltraWinToolbars.RibbonToolSize.Large;
            buttonTool89.InstanceProps.MinimumSizeOnRibbon = Infragistics.Win.UltraWinToolbars.RibbonToolSize.Large;
            buttonTool90.InstanceProps.MinimumSizeOnRibbon = Infragistics.Win.UltraWinToolbars.RibbonToolSize.Large;
            ribbonGroup50.Tools.AddRange(new Infragistics.Win.UltraWinToolbars.ToolBase[] {
            buttonTool88,
            buttonTool89,
            buttonTool90});
            ribbonGroup51.Caption = "History";
            stateButtonTool37.InstanceProps.MinimumSizeOnRibbon = Infragistics.Win.UltraWinToolbars.RibbonToolSize.Large;
            stateButtonTool37.InstanceProps.PreferredSizeOnRibbon = Infragistics.Win.UltraWinToolbars.RibbonToolSize.Large;
            buttonTool91.InstanceProps.IsFirstInGroup = true;
            buttonTool91.InstanceProps.MinimumSizeOnRibbon = Infragistics.Win.UltraWinToolbars.RibbonToolSize.Large;
            buttonTool91.InstanceProps.PreferredSizeOnRibbon = Infragistics.Win.UltraWinToolbars.RibbonToolSize.Large;
            buttonTool92.InstanceProps.MinimumSizeOnRibbon = Infragistics.Win.UltraWinToolbars.RibbonToolSize.Large;
            buttonTool92.InstanceProps.PreferredSizeOnRibbon = Infragistics.Win.UltraWinToolbars.RibbonToolSize.Large;
            ribbonGroup51.Tools.AddRange(new Infragistics.Win.UltraWinToolbars.ToolBase[] {
            stateButtonTool37,
            buttonTool91,
            buttonTool92});
            ribbonGroup52.Caption = "Filter";
            stateButtonTool38.InstanceProps.MinimumSizeOnRibbon = Infragistics.Win.UltraWinToolbars.RibbonToolSize.Large;
            stateButtonTool38.InstanceProps.PreferredSizeOnRibbon = Infragistics.Win.UltraWinToolbars.RibbonToolSize.Large;
            ribbonGroup52.Tools.AddRange(new Infragistics.Win.UltraWinToolbars.ToolBase[] {
            stateButtonTool38});
            ribbonGroup53.Caption = "Show/Hide";
            ribbonGroup53.Tools.AddRange(new Infragistics.Win.UltraWinToolbars.ToolBase[] {
            buttonTool93});
            ribbonTab5.Groups.AddRange(new Infragistics.Win.UltraWinToolbars.RibbonGroup[] {
            ribbonGroup36,
            ribbonGroup37,
            ribbonGroup38,
            ribbonGroup39,
            ribbonGroup40,
            ribbonGroup41,
            ribbonGroup42,
            ribbonGroup43,
            ribbonGroup44,
            ribbonGroup45,
            ribbonGroup46,
            ribbonGroup47,
            ribbonGroup48,
            ribbonGroup49,
            ribbonGroup50,
            ribbonGroup51,
            ribbonGroup52,
            ribbonGroup53});
            appearance6.Image = ((object)(resources.GetObject("appearance6.Image")));
            ribbonTab5.Settings.TabItemAppearance = appearance6;
            ribbonTab6.Caption = "Services";
            ribbonGroup54.Caption = "Views";
            stateButtonTool39.InstanceProps.MinimumSizeOnRibbon = Infragistics.Win.UltraWinToolbars.RibbonToolSize.Large;
            stateButtonTool40.InstanceProps.PreferredSizeOnRibbon = Infragistics.Win.UltraWinToolbars.RibbonToolSize.Large;
            stateButtonTool41.InstanceProps.MinimumSizeOnRibbon = Infragistics.Win.UltraWinToolbars.RibbonToolSize.Large;
            stateButtonTool42.InstanceProps.PreferredSizeOnRibbon = Infragistics.Win.UltraWinToolbars.RibbonToolSize.Large;
            ribbonGroup54.Tools.AddRange(new Infragistics.Win.UltraWinToolbars.ToolBase[] {
            stateButtonTool39,
            stateButtonTool40,
            stateButtonTool41,
            stateButtonTool42});
            ribbonGroup55.Caption = "Actions";
            buttonTool94.InstanceProps.MinimumSizeOnRibbon = Infragistics.Win.UltraWinToolbars.RibbonToolSize.Large;
            buttonTool95.InstanceProps.MinimumSizeOnRibbon = Infragistics.Win.UltraWinToolbars.RibbonToolSize.Large;
            ribbonGroup55.Tools.AddRange(new Infragistics.Win.UltraWinToolbars.ToolBase[] {
            buttonTool94,
            buttonTool95});
            ribbonGroup56.Caption = "Show/Hide";
            ribbonGroup56.Tools.AddRange(new Infragistics.Win.UltraWinToolbars.ToolBase[] {
            buttonTool96,
            buttonTool97});
            ribbonGroup57.Caption = "Actions";
            buttonTool98.InstanceProps.MinimumSizeOnRibbon = Infragistics.Win.UltraWinToolbars.RibbonToolSize.Large;
            buttonTool99.InstanceProps.MinimumSizeOnRibbon = Infragistics.Win.UltraWinToolbars.RibbonToolSize.Large;
            buttonTool100.InstanceProps.IsFirstInGroup = true;
            buttonTool100.InstanceProps.MinimumSizeOnRibbon = Infragistics.Win.UltraWinToolbars.RibbonToolSize.Large;
            buttonTool101.InstanceProps.MinimumSizeOnRibbon = Infragistics.Win.UltraWinToolbars.RibbonToolSize.Large;
            ribbonGroup57.Tools.AddRange(new Infragistics.Win.UltraWinToolbars.ToolBase[] {
            buttonTool98,
            buttonTool99,
            buttonTool100,
            buttonTool101});
            ribbonGroup58.Caption = "Filter";
            stateButtonTool43.InstanceProps.MinimumSizeOnRibbon = Infragistics.Win.UltraWinToolbars.RibbonToolSize.Large;
            ribbonGroup58.Tools.AddRange(new Infragistics.Win.UltraWinToolbars.ToolBase[] {
            stateButtonTool43,
            buttonTool102,
            buttonTool103,
            buttonTool104});
            ribbonGroup59.Caption = "Show/Hide";
            ribbonGroup59.Tools.AddRange(new Infragistics.Win.UltraWinToolbars.ToolBase[] {
            buttonTool105,
            buttonTool106});
            ribbonGroup60.Caption = "Actions";
            buttonTool107.InstanceProps.MinimumSizeOnRibbon = Infragistics.Win.UltraWinToolbars.RibbonToolSize.Large;
            buttonTool108.InstanceProps.MinimumSizeOnRibbon = Infragistics.Win.UltraWinToolbars.RibbonToolSize.Large;
            buttonTool109.InstanceProps.IsFirstInGroup = true;
            buttonTool109.InstanceProps.MinimumSizeOnRibbon = Infragistics.Win.UltraWinToolbars.RibbonToolSize.Large;
            buttonTool110.InstanceProps.IsFirstInGroup = true;
            buttonTool110.InstanceProps.MinimumSizeOnRibbon = Infragistics.Win.UltraWinToolbars.RibbonToolSize.Large;
            buttonTool111.InstanceProps.MinimumSizeOnRibbon = Infragistics.Win.UltraWinToolbars.RibbonToolSize.Large;
            ribbonGroup60.Tools.AddRange(new Infragistics.Win.UltraWinToolbars.ToolBase[] {
            buttonTool107,
            buttonTool108,
            buttonTool109,
            buttonTool110,
            buttonTool111});
            ribbonGroup61.Caption = "Show/Hide";
            ribbonGroup61.Tools.AddRange(new Infragistics.Win.UltraWinToolbars.ToolBase[] {
            buttonTool112,
            buttonTool113});
            ribbonGroup62.Caption = "Filter";
            stateButtonTool44.InstanceProps.MinimumSizeOnRibbon = Infragistics.Win.UltraWinToolbars.RibbonToolSize.Large;
            ribbonGroup62.Tools.AddRange(new Infragistics.Win.UltraWinToolbars.ToolBase[] {
            stateButtonTool44});
            ribbonGroup63.Caption = "Show/Hide";
            ribbonGroup63.Tools.AddRange(new Infragistics.Win.UltraWinToolbars.ToolBase[] {
            buttonTool114,
            buttonTool115,
            buttonTool116});
            ribbonGroup64.Caption = "Newsfeed";
            buttonTool117.InstanceProps.MinimumSizeOnRibbon = Infragistics.Win.UltraWinToolbars.RibbonToolSize.Large;
            buttonTool118.InstanceProps.MinimumSizeOnRibbon = Infragistics.Win.UltraWinToolbars.RibbonToolSize.Large;
            buttonTool119.InstanceProps.MinimumSizeOnRibbon = Infragistics.Win.UltraWinToolbars.RibbonToolSize.Large;
            ribbonGroup64.Tools.AddRange(new Infragistics.Win.UltraWinToolbars.ToolBase[] {
            buttonTool117,
            buttonTool118,
            buttonTool119});
            ribbonTab6.Groups.AddRange(new Infragistics.Win.UltraWinToolbars.RibbonGroup[] {
            ribbonGroup54,
            ribbonGroup55,
            ribbonGroup56,
            ribbonGroup57,
            ribbonGroup58,
            ribbonGroup59,
            ribbonGroup60,
            ribbonGroup61,
            ribbonGroup62,
            ribbonGroup63,
            ribbonGroup64});
            appearance7.Image = ((object)(resources.GetObject("appearance7.Image")));
            ribbonTab6.Settings.TabItemAppearance = appearance7;
            ribbonTab7.Caption = "Logs";
            ribbonGroup65.Caption = "Filter";
            stateButtonTool45.InstanceProps.MinimumSizeOnRibbon = Infragistics.Win.UltraWinToolbars.RibbonToolSize.Large;
            ribbonGroup65.Tools.AddRange(new Infragistics.Win.UltraWinToolbars.ToolBase[] {
            stateButtonTool45,
            buttonTool120,
            buttonTool121,
            buttonTool122});
            ribbonGroup66.Caption = "Search";
            buttonTool123.InstanceProps.MinimumSizeOnRibbon = Infragistics.Win.UltraWinToolbars.RibbonToolSize.Large;
            ribbonGroup66.Tools.AddRange(new Infragistics.Win.UltraWinToolbars.ToolBase[] {
            buttonTool123});
            ribbonGroup67.Caption = "Actions";
            buttonTool124.InstanceProps.MinimumSizeOnRibbon = Infragistics.Win.UltraWinToolbars.RibbonToolSize.Large;
            buttonTool125.InstanceProps.IsFirstInGroup = true;
            buttonTool125.InstanceProps.MinimumSizeOnRibbon = Infragistics.Win.UltraWinToolbars.RibbonToolSize.Large;
            ribbonGroup67.Tools.AddRange(new Infragistics.Win.UltraWinToolbars.ToolBase[] {
            buttonTool124,
            buttonTool125});
            ribbonGroup68.Caption = "Show/Hide";
            ribbonGroup68.Tools.AddRange(new Infragistics.Win.UltraWinToolbars.ToolBase[] {
            buttonTool126,
            buttonTool127,
            buttonTool128});
            ribbonGroup69.Caption = "Newsfeed";
            buttonTool129.InstanceProps.MinimumSizeOnRibbon = Infragistics.Win.UltraWinToolbars.RibbonToolSize.Large;
            buttonTool130.InstanceProps.MinimumSizeOnRibbon = Infragistics.Win.UltraWinToolbars.RibbonToolSize.Large;
            buttonTool131.InstanceProps.MinimumSizeOnRibbon = Infragistics.Win.UltraWinToolbars.RibbonToolSize.Large;
            ribbonGroup69.Tools.AddRange(new Infragistics.Win.UltraWinToolbars.ToolBase[] {
            buttonTool129,
            buttonTool130,
            buttonTool131});
            ribbonTab7.Groups.AddRange(new Infragistics.Win.UltraWinToolbars.RibbonGroup[] {
            ribbonGroup65,
            ribbonGroup66,
            ribbonGroup67,
            ribbonGroup68,
            ribbonGroup69});
            appearance8.Image = ((object)(resources.GetObject("appearance8.Image")));
            ribbonTab7.Settings.TabItemAppearance = appearance8;
            this.toolbarsManager.Ribbon.NonInheritedRibbonTabs.AddRange(new Infragistics.Win.UltraWinToolbars.RibbonTab[] {
            ribbonTab1,
            ribbonTab2,
            ribbonTab3,
            ribbonTab4,
            ribbonTab5,
            ribbonTab6,
            ribbonTab7});
            this.toolbarsManager.Ribbon.QuickAccessToolbar.Visible = false;
            this.toolbarsManager.Ribbon.Visible = true;
            this.toolbarsManager.SettingsKey = "";
            this.toolbarsManager.ShowFullMenusDelay = 500;
            stateButtonTool46.MenuDisplayStyle = Infragistics.Win.UltraWinToolbars.StateButtonMenuDisplayStyle.DisplayCheckmark;
            stateButtonTool46.OptionSetKey = "overviewTabViewOptionSet";
            appearance9.Image = ((object)(resources.GetObject("appearance9.Image")));
            stateButtonTool46.SharedPropsInternal.AppearancesLarge.Appearance = appearance9;
            stateButtonTool46.SharedPropsInternal.Caption = "Dashboard";
            stateButtonTool47.MenuDisplayStyle = Infragistics.Win.UltraWinToolbars.StateButtonMenuDisplayStyle.DisplayCheckmark;
            stateButtonTool47.OptionSetKey = "overviewTabViewOptionSet";
            appearance10.Image = ((object)(resources.GetObject("appearance10.Image")));
            stateButtonTool47.SharedPropsInternal.AppearancesLarge.Appearance = appearance10;
            stateButtonTool47.SharedPropsInternal.Caption = "Details";
            stateButtonTool48.MenuDisplayStyle = Infragistics.Win.UltraWinToolbars.StateButtonMenuDisplayStyle.DisplayCheckmark;
            stateButtonTool48.OptionSetKey = "overviewTabViewOptionSet";
            appearance11.Image = ((object)(resources.GetObject("appearance11.Image")));
            stateButtonTool48.SharedPropsInternal.AppearancesLarge.Appearance = appearance11;
            stateButtonTool48.SharedPropsInternal.Caption = "Configuration";
            appearance12.Image = ((object)(resources.GetObject("appearance12.Image")));
            buttonTool132.SharedPropsInternal.AppearancesLarge.Appearance = appearance12;
            buttonTool132.SharedPropsInternal.Caption = "Stop Server";
            buttonTool132.SharedPropsInternal.Visible = false;
            stateButtonTool49.OptionSetKey = "sessionsTabViewOptionSet";
            appearance13.Image = ((object)(resources.GetObject("appearance13.Image")));
            stateButtonTool49.SharedPropsInternal.AppearancesLarge.Appearance = appearance13;
            stateButtonTool49.SharedPropsInternal.Caption = "Summary";
            appearance14.Image = ((object)(resources.GetObject("appearance14.Image")));
            buttonTool133.SharedPropsInternal.AppearancesLarge.Appearance = appearance14;
            buttonTool133.SharedPropsInternal.Caption = "Kill Session";
            buttonTool133.SharedPropsInternal.Enabled = false;
            stateButtonTool50.OptionSetKey = "sessionsTabViewOptionSet";
            appearance15.Image = ((object)(resources.GetObject("appearance15.Image")));
            stateButtonTool50.SharedPropsInternal.AppearancesLarge.Appearance = appearance15;
            stateButtonTool50.SharedPropsInternal.Caption = "Details";
            appearance16.Image = ((object)(resources.GetObject("appearance16.Image")));
            stateButtonTool51.SharedPropsInternal.AppearancesLarge.Appearance = appearance16;
            stateButtonTool51.SharedPropsInternal.Caption = "Filter";
            appearance17.Image = ((object)(resources.GetObject("appearance17.Image")));
            buttonTool134.SharedPropsInternal.AppearancesSmall.Appearance = appearance17;
            buttonTool134.SharedPropsInternal.Caption = "User Only";
            appearance18.Image = ((object)(resources.GetObject("appearance18.Image")));
            buttonTool135.SharedPropsInternal.AppearancesSmall.Appearance = appearance18;
            buttonTool135.SharedPropsInternal.Caption = "Details";
            appearance19.Image = ((object)(resources.GetObject("appearance19.Image")));
            buttonTool136.SharedPropsInternal.AppearancesSmall.Appearance = appearance19;
            buttonTool136.SharedPropsInternal.Caption = "Group By Box";
            appearance20.Image = ((object)(resources.GetObject("appearance20.Image")));
            buttonTool137.SharedPropsInternal.AppearancesSmall.Appearance = appearance20;
            buttonTool137.SharedPropsInternal.Caption = "Active Only";
            appearance21.Image = ((object)(resources.GetObject("appearance21.Image")));
            buttonTool138.SharedPropsInternal.AppearancesSmall.Appearance = appearance21;
            buttonTool138.SharedPropsInternal.Caption = "Blocked";
            appearance22.Image = ((object)(resources.GetObject("appearance22.Image")));
            buttonTool139.SharedPropsInternal.AppearancesSmall.Appearance = appearance22;
            buttonTool139.SharedPropsInternal.Caption = "Details";
            appearance23.Image = ((object)(resources.GetObject("appearance23.Image")));
            buttonTool140.SharedPropsInternal.AppearancesSmall.Appearance = appearance23;
            buttonTool140.SharedPropsInternal.Caption = "Group By Box";
            appearance24.Image = ((object)(resources.GetObject("appearance24.Image")));
            buttonTool141.SharedPropsInternal.AppearancesSmall.Appearance = appearance24;
            buttonTool141.SharedPropsInternal.Caption = "Chart";
            appearance25.Image = ((object)(resources.GetObject("appearance25.Image")));
            buttonTool142.SharedPropsInternal.AppearancesSmall.Appearance = appearance25;
            buttonTool142.SharedPropsInternal.Caption = "Group By Box";
            stateButtonTool52.OptionSetKey = "sessionsTabViewOptionSet";
            appearance26.Image = ((object)(resources.GetObject("appearance26.Image")));
            stateButtonTool52.SharedPropsInternal.AppearancesLarge.Appearance = appearance26;
            stateButtonTool52.SharedPropsInternal.Caption = "Locks";
            stateButtonTool53.OptionSetKey = "sessionsTabViewOptionSet";
            appearance27.Image = ((object)(resources.GetObject("appearance27.Image")));
            stateButtonTool53.SharedPropsInternal.AppearancesLarge.Appearance = appearance27;
            stateButtonTool53.SharedPropsInternal.Caption = "Blocking";
            appearance28.Image = ((object)(resources.GetObject("appearance28.Image")));
            buttonTool143.SharedPropsInternal.AppearancesSmall.Appearance = appearance28;
            buttonTool143.SharedPropsInternal.Caption = "Group By Box";
            appearance29.Image = ((object)(resources.GetObject("appearance29.Image")));
            buttonTool144.SharedPropsInternal.AppearancesSmall.Appearance = appearance29;
            buttonTool144.SharedPropsInternal.Caption = "Chart";
            appearance30.Image = ((object)(resources.GetObject("appearance30.Image")));
            stateButtonTool54.SharedPropsInternal.AppearancesLarge.Appearance = appearance30;
            stateButtonTool54.SharedPropsInternal.Caption = "Filter";
            appearance31.Image = ((object)(resources.GetObject("appearance31.Image")));
            buttonTool145.SharedPropsInternal.AppearancesSmall.Appearance = appearance31;
            buttonTool145.SharedPropsInternal.Caption = "Show Blocked";
            appearance32.Image = ((object)(resources.GetObject("appearance32.Image")));
            buttonTool146.SharedPropsInternal.AppearancesSmall.Appearance = appearance32;
            buttonTool146.SharedPropsInternal.Caption = "Show Blocking";
            appearance33.Image = ((object)(resources.GetObject("appearance33.Image")));
            buttonTool147.SharedPropsInternal.AppearancesSmall.Appearance = appearance33;
            buttonTool147.SharedPropsInternal.Caption = "Blocked or Blocking";
            appearance34.Image = ((object)(resources.GetObject("appearance34.Image")));
            stateButtonTool55.SharedPropsInternal.AppearancesLarge.Appearance = appearance34;
            stateButtonTool55.SharedPropsInternal.Caption = "Filter";
            appearance35.Image = ((object)(resources.GetObject("appearance35.Image")));
            buttonTool148.SharedPropsInternal.AppearancesSmall.Appearance = appearance35;
            buttonTool148.SharedPropsInternal.Caption = "Chart";
            appearance36.Image = ((object)(resources.GetObject("appearance36.Image")));
            buttonTool149.SharedPropsInternal.AppearancesSmall.Appearance = appearance36;
            buttonTool149.SharedPropsInternal.Caption = "Group By Box";
            appearance37.Image = ((object)(resources.GetObject("appearance37.Image")));
            stateButtonTool56.SharedPropsInternal.AppearancesLarge.Appearance = appearance37;
            stateButtonTool56.SharedPropsInternal.Caption = "Advanced Filter";
            appearance38.Image = ((object)(resources.GetObject("appearance38.Image")));
            buttonTool150.SharedPropsInternal.AppearancesSmall.Appearance = appearance38;
            buttonTool150.SharedPropsInternal.Caption = "Show SQL Statements";
            buttonTool150.SharedPropsInternal.Visible = false;
            appearance39.Image = ((object)(resources.GetObject("appearance39.Image")));
            buttonTool151.SharedPropsInternal.AppearancesSmall.Appearance = appearance39;
            buttonTool151.SharedPropsInternal.Caption = "Show Stored Procedures";
            buttonTool151.SharedPropsInternal.Visible = false;
            appearance40.Image = ((object)(resources.GetObject("appearance40.Image")));
            buttonTool152.SharedPropsInternal.AppearancesSmall.Appearance = appearance40;
            buttonTool152.SharedPropsInternal.Caption = "Show SQL Batches";
            buttonTool152.SharedPropsInternal.Visible = false;
            appearance41.Image = ((object)(resources.GetObject("appearance41.Image")));
            buttonTool153.SharedPropsInternal.AppearancesSmall.Appearance = appearance41;
            buttonTool153.SharedPropsInternal.Caption = "Filters";
            appearance42.Image = ((object)(resources.GetObject("appearance42.Image")));
            buttonTool154.SharedPropsInternal.AppearancesSmall.Appearance = appearance42;
            buttonTool154.SharedPropsInternal.Caption = "Group By Box";
            appearance43.Image = ((object)(resources.GetObject("appearance43.Image")));
            buttonTool155.SharedPropsInternal.AppearancesSmall.Appearance = appearance43;
            buttonTool155.SharedPropsInternal.Caption = "By Lock";
            appearance44.Image = ((object)(resources.GetObject("appearance44.Image")));
            buttonTool156.SharedPropsInternal.AppearancesSmall.Appearance = appearance44;
            buttonTool156.SharedPropsInternal.Caption = "By Session";
            appearance45.Image = ((object)(resources.GetObject("appearance45.Image")));
            stateButtonTool57.SharedPropsInternal.AppearancesLarge.Appearance = appearance45;
            stateButtonTool57.SharedPropsInternal.Caption = "Maintenance Mode";
            appearance46.Image = ((object)(resources.GetObject("appearance46.Image")));
            stateButtonTool58.SharedPropsInternal.AppearancesLarge.Appearance = appearance46;
            stateButtonTool58.SharedPropsInternal.Caption = "Filter";
            appearance47.Image = ((object)(resources.GetObject("appearance47.Image")));
            buttonTool157.SharedPropsInternal.AppearancesSmall.Appearance = appearance47;
            buttonTool157.SharedPropsInternal.Caption = "Show Errors";
            appearance48.Image = ((object)(resources.GetObject("appearance48.Image")));
            buttonTool158.SharedPropsInternal.AppearancesSmall.Appearance = appearance48;
            buttonTool158.SharedPropsInternal.Caption = "Show Warnings";
            appearance49.Image = ((object)(resources.GetObject("appearance49.Image")));
            buttonTool159.SharedPropsInternal.AppearancesSmall.Appearance = appearance49;
            buttonTool159.SharedPropsInternal.Caption = "Show Informational";
            appearance50.Image = ((object)(resources.GetObject("appearance50.Image")));
            buttonTool160.SharedPropsInternal.AppearancesLarge.Appearance = appearance50;
            buttonTool160.SharedPropsInternal.Caption = "Search";
            appearance51.Image = ((object)(resources.GetObject("appearance51.Image")));
            buttonTool161.SharedPropsInternal.AppearancesLarge.Appearance = appearance51;
            buttonTool161.SharedPropsInternal.Caption = "Cycle Server Log";
            appearance52.Image = ((object)(resources.GetObject("appearance52.Image")));
            buttonTool162.SharedPropsInternal.AppearancesLarge.Appearance = appearance52;
            buttonTool162.SharedPropsInternal.Caption = "Configure";
            buttonTool162.SharedPropsInternal.ToolTipText = "Configure Log Limit";
            appearance53.Image = ((object)(resources.GetObject("appearance53.Image")));
            buttonTool163.SharedPropsInternal.AppearancesSmall.Appearance = appearance53;
            buttonTool163.SharedPropsInternal.Caption = "Logs";
            appearance54.Image = ((object)(resources.GetObject("appearance54.Image")));
            buttonTool164.SharedPropsInternal.AppearancesSmall.Appearance = appearance54;
            buttonTool164.SharedPropsInternal.Caption = "Details";
            appearance55.Image = ((object)(resources.GetObject("appearance55.Image")));
            buttonTool165.SharedPropsInternal.AppearancesSmall.Appearance = appearance55;
            buttonTool165.SharedPropsInternal.Caption = "Group By Box";
            stateButtonTool59.Checked = true;
            stateButtonTool59.OptionSetKey = "resourcesTabViewOptionSet";
            appearance56.Image = ((object)(resources.GetObject("appearance56.Image")));
            stateButtonTool59.SharedPropsInternal.AppearancesLarge.Appearance = appearance56;
            stateButtonTool59.SharedPropsInternal.Caption = "Summary";
            stateButtonTool60.OptionSetKey = "resourcesTabViewOptionSet";
            appearance57.Image = ((object)(resources.GetObject("appearance57.Image")));
            stateButtonTool60.SharedPropsInternal.AppearancesLarge.Appearance = appearance57;
            stateButtonTool60.SharedPropsInternal.Caption = "CPU";
            stateButtonTool61.OptionSetKey = "resourcesTabViewOptionSet";
            appearance58.Image = ((object)(resources.GetObject("appearance58.Image")));
            stateButtonTool61.SharedPropsInternal.AppearancesLarge.Appearance = appearance58;
            stateButtonTool61.SharedPropsInternal.Caption = "Memory";
            stateButtonTool62.OptionSetKey = "resourcesTabViewOptionSet";
            appearance59.Image = ((object)(resources.GetObject("appearance59.Image")));
            stateButtonTool62.SharedPropsInternal.AppearancesLarge.Appearance = appearance59;
            stateButtonTool62.SharedPropsInternal.Caption = "Disk";
            stateButtonTool63.OptionSetKey = "resourcesTabViewOptionSet";
            appearance60.Image = ((object)(resources.GetObject("appearance60.Image")));
            stateButtonTool63.SharedPropsInternal.AppearancesLarge.Appearance = appearance60;
            stateButtonTool63.SharedPropsInternal.Caption = "Procedure Cache";
            appearance61.Image = ((object)(resources.GetObject("appearance61.Image")));
            buttonTool166.SharedPropsInternal.AppearancesLarge.Appearance = appearance61;
            buttonTool166.SharedPropsInternal.Caption = "Clear Cache";
            appearance62.Image = ((object)(resources.GetObject("appearance62.Image")));
            buttonTool167.SharedPropsInternal.AppearancesSmall.Appearance = appearance62;
            buttonTool167.SharedPropsInternal.Caption = "Charts";
            appearance63.Image = ((object)(resources.GetObject("appearance63.Image")));
            buttonTool168.SharedPropsInternal.AppearancesSmall.Appearance = appearance63;
            buttonTool168.SharedPropsInternal.Caption = "Group By Box";
            appearance64.Image = ((object)(resources.GetObject("appearance64.Image")));
            buttonTool169.SharedPropsInternal.AppearancesLarge.Appearance = appearance64;
            buttonTool169.SharedPropsInternal.Caption = "Trace Session";
            buttonTool169.SharedPropsInternal.Enabled = false;
            appearance65.Image = ((object)(resources.GetObject("appearance65.Image")));
            buttonTool170.SharedPropsInternal.AppearancesLarge.Appearance = appearance65;
            buttonTool170.SharedPropsInternal.Caption = "Trace Session";
            buttonTool170.SharedPropsInternal.Enabled = false;
            appearance66.Image = ((object)(resources.GetObject("appearance66.Image")));
            buttonTool171.SharedPropsInternal.AppearancesLarge.Appearance = appearance66;
            buttonTool171.SharedPropsInternal.Caption = "Kill Session";
            buttonTool171.SharedPropsInternal.Enabled = false;
            appearance67.Image = ((object)(resources.GetObject("appearance67.Image")));
            buttonTool172.SharedPropsInternal.AppearancesLarge.Appearance = appearance67;
            buttonTool172.SharedPropsInternal.Caption = "Trace Session";
            buttonTool172.SharedPropsInternal.Enabled = false;
            appearance68.Image = ((object)(resources.GetObject("appearance68.Image")));
            buttonTool173.SharedPropsInternal.AppearancesLarge.Appearance = appearance68;
            buttonTool173.SharedPropsInternal.Caption = "Kill Session";
            buttonTool173.SharedPropsInternal.Enabled = false;
            stateButtonTool64.OptionSetKey = "databasesTabViewOptionSet";
            appearance69.Image = ((object)(resources.GetObject("appearance69.Image")));
            stateButtonTool64.SharedPropsInternal.AppearancesLarge.Appearance = appearance69;
            stateButtonTool64.SharedPropsInternal.Caption = "Summary";
            stateButtonTool65.OptionSetKey = "databasesTabViewOptionSet";
            appearance70.Image = ((object)(resources.GetObject("appearance70.Image")));
            stateButtonTool65.SharedPropsInternal.AppearancesLarge.Appearance = appearance70;
            stateButtonTool65.SharedPropsInternal.Caption = "Configuration";
            stateButtonTool66.OptionSetKey = "databasesTabViewOptionSet";
            appearance71.Image = ((object)(resources.GetObject("appearance71.Image")));
            stateButtonTool66.SharedPropsInternal.AppearancesLarge.Appearance = appearance71;
            stateButtonTool66.SharedPropsInternal.Caption = "Backups && Restores";
            stateButtonTool67.OptionSetKey = "databasesTabViewOptionSet";
            appearance72.Image = ((object)(resources.GetObject("appearance72.Image")));
            stateButtonTool67.SharedPropsInternal.AppearancesLarge.Appearance = appearance72;
            stateButtonTool67.SharedPropsInternal.Caption = "Tables && Indexes";
            stateButtonTool68.OptionSetKey = "databasesTabViewOptionSet";
            appearance73.Image = ((object)(resources.GetObject("appearance73.Image")));
            stateButtonTool68.SharedPropsInternal.AppearancesLarge.Appearance = appearance73;
            stateButtonTool68.SharedPropsInternal.Caption = "Files";
            appearance74.Image = ((object)(resources.GetObject("appearance74.Image")));
            buttonTool174.SharedPropsInternal.AppearancesSmall.Appearance = appearance74;
            buttonTool174.SharedPropsInternal.Caption = "User Databases Only";
            appearance75.Image = ((object)(resources.GetObject("appearance75.Image")));
            buttonTool175.SharedPropsInternal.AppearancesSmall.Appearance = appearance75;
            buttonTool175.SharedPropsInternal.Caption = "Charts";
            appearance76.Image = ((object)(resources.GetObject("appearance76.Image")));
            buttonTool176.SharedPropsInternal.AppearancesSmall.Appearance = appearance76;
            buttonTool176.SharedPropsInternal.Caption = "Group By Box";
            appearance77.Image = ((object)(resources.GetObject("appearance77.Image")));
            buttonTool177.SharedPropsInternal.AppearancesSmall.Appearance = appearance77;
            buttonTool177.SharedPropsInternal.Caption = "Group By Box";
            appearance78.Image = ((object)(resources.GetObject("appearance78.Image")));
            buttonTool178.SharedPropsInternal.AppearancesSmall.Appearance = appearance78;
            buttonTool178.SharedPropsInternal.Caption = "User Databases Only";
            appearance79.Image = ((object)(resources.GetObject("appearance79.Image")));
            buttonTool179.SharedPropsInternal.AppearancesSmall.Appearance = appearance79;
            buttonTool179.SharedPropsInternal.Caption = "User Databases Only";
            buttonTool179.SharedPropsInternal.Visible = false;
            appearance80.Image = ((object)(resources.GetObject("appearance80.Image")));
            buttonTool180.SharedPropsInternal.AppearancesSmall.Appearance = appearance80;
            buttonTool180.SharedPropsInternal.Caption = "Show Backups";
            appearance81.Image = ((object)(resources.GetObject("appearance81.Image")));
            buttonTool181.SharedPropsInternal.AppearancesSmall.Appearance = appearance81;
            buttonTool181.SharedPropsInternal.Caption = "Show Restores";
            appearance82.Image = ((object)(resources.GetObject("appearance82.Image")));
            stateButtonTool69.SharedPropsInternal.AppearancesLarge.Appearance = appearance82;
            stateButtonTool69.SharedPropsInternal.Caption = "Filter";
            appearance83.Image = ((object)(resources.GetObject("appearance83.Image")));
            buttonTool182.SharedPropsInternal.AppearancesSmall.Appearance = appearance83;
            buttonTool182.SharedPropsInternal.Caption = "Databases Group By Box";
            appearance84.Image = ((object)(resources.GetObject("appearance84.Image")));
            buttonTool183.SharedPropsInternal.AppearancesSmall.Appearance = appearance84;
            buttonTool183.SharedPropsInternal.Caption = "History Group By Box";
            appearance85.Image = ((object)(resources.GetObject("appearance85.Image")));
            buttonTool184.SharedPropsInternal.AppearancesSmall.Appearance = appearance85;
            buttonTool184.SharedPropsInternal.Caption = "User Databases Only";
            appearance86.Image = ((object)(resources.GetObject("appearance86.Image")));
            buttonTool185.SharedPropsInternal.AppearancesSmall.Appearance = appearance86;
            buttonTool185.SharedPropsInternal.Caption = "Group By Box";
            appearance87.Image = ((object)(resources.GetObject("appearance87.Image")));
            buttonTool186.SharedPropsInternal.AppearancesSmall.Appearance = appearance87;
            buttonTool186.SharedPropsInternal.Caption = "User Databases Only";
            buttonTool186.SharedPropsInternal.Visible = false;
            appearance88.Image = ((object)(resources.GetObject("appearance88.Image")));
            buttonTool187.SharedPropsInternal.AppearancesSmall.Appearance = appearance88;
            buttonTool187.SharedPropsInternal.Caption = "User Tables Only";
            appearance89.Image = ((object)(resources.GetObject("appearance89.Image")));
            buttonTool188.SharedPropsInternal.AppearancesSmall.Appearance = appearance89;
            buttonTool188.SharedPropsInternal.Caption = "Table Details";
            appearance90.Image = ((object)(resources.GetObject("appearance90.Image")));
            buttonTool189.SharedPropsInternal.AppearancesSmall.Appearance = appearance90;
            buttonTool189.SharedPropsInternal.Caption = "Group By Box";
            appearance91.Image = ((object)(resources.GetObject("appearance91.Image")));
            buttonTool190.SharedPropsInternal.AppearancesLarge.Appearance = appearance91;
            buttonTool190.SharedPropsInternal.Caption = "Update Statistics";
            buttonTool190.SharedPropsInternal.Enabled = false;
            buttonTool190.SharedPropsInternal.ToolTipText = "Updates index statistics for the selected tables.";
            appearance92.Image = ((object)(resources.GetObject("appearance92.Image")));
            buttonTool191.SharedPropsInternal.AppearancesLarge.Appearance = appearance92;
            buttonTool191.SharedPropsInternal.Caption = "Rebuild Indexes";
            buttonTool191.SharedPropsInternal.Enabled = false;
            buttonTool191.SharedPropsInternal.ToolTipText = "Rebuild indexes for the selected tables.";
            stateButtonTool70.OptionSetKey = "servicesTabViewOptionSet";
            appearance93.Image = ((object)(resources.GetObject("appearance93.Image")));
            stateButtonTool70.SharedPropsInternal.AppearancesLarge.Appearance = appearance93;
            stateButtonTool70.SharedPropsInternal.Caption = "SQL Agent Jobs";
            stateButtonTool71.OptionSetKey = "servicesTabViewOptionSet";
            appearance94.Image = ((object)(resources.GetObject("appearance94.Image")));
            stateButtonTool71.SharedPropsInternal.AppearancesLarge.Appearance = appearance94;
            stateButtonTool71.SharedPropsInternal.Caption = "Summary";
            stateButtonTool72.OptionSetKey = "servicesTabViewOptionSet";
            appearance95.Image = ((object)(resources.GetObject("appearance95.Image")));
            stateButtonTool72.SharedPropsInternal.AppearancesLarge.Appearance = appearance95;
            stateButtonTool72.SharedPropsInternal.Caption = "Full-Text Search";
            stateButtonTool73.OptionSetKey = "servicesTabViewOptionSet";
            appearance96.Image = ((object)(resources.GetObject("appearance96.Image")));
            stateButtonTool73.SharedPropsInternal.AppearancesLarge.Appearance = appearance96;
            stateButtonTool73.SharedPropsInternal.Caption = "Replication";
            appearance97.Image = ((object)(resources.GetObject("appearance97.Image")));
            stateButtonTool74.SharedPropsInternal.AppearancesLarge.Appearance = appearance97;
            stateButtonTool74.SharedPropsInternal.Caption = "Filter";
            appearance98.Image = ((object)(resources.GetObject("appearance98.Image")));
            buttonTool192.SharedPropsInternal.AppearancesSmall.Appearance = appearance98;
            buttonTool192.SharedPropsInternal.Caption = "All Jobs";
            appearance99.Image = ((object)(resources.GetObject("appearance99.Image")));
            buttonTool193.SharedPropsInternal.AppearancesSmall.Appearance = appearance99;
            buttonTool193.SharedPropsInternal.Caption = "Running Jobs";
            appearance100.Image = ((object)(resources.GetObject("appearance100.Image")));
            buttonTool194.SharedPropsInternal.AppearancesSmall.Appearance = appearance100;
            buttonTool194.SharedPropsInternal.Caption = "Failed Jobs";
            appearance101.Image = ((object)(resources.GetObject("appearance101.Image")));
            buttonTool195.SharedPropsInternal.AppearancesSmall.Appearance = appearance101;
            buttonTool195.SharedPropsInternal.Caption = "Job History";
            appearance102.Image = ((object)(resources.GetObject("appearance102.Image")));
            buttonTool196.SharedPropsInternal.AppearancesSmall.Appearance = appearance102;
            buttonTool196.SharedPropsInternal.Caption = "Group By Box";
            appearance103.Image = ((object)(resources.GetObject("appearance103.Image")));
            stateButtonTool75.SharedPropsInternal.AppearancesLarge.Appearance = appearance103;
            stateButtonTool75.SharedPropsInternal.Caption = "Filter";
            appearance104.Image = ((object)(resources.GetObject("appearance104.Image")));
            buttonTool197.SharedPropsInternal.AppearancesSmall.Appearance = appearance104;
            buttonTool197.SharedPropsInternal.Caption = "Topology Group By Box";
            appearance105.Image = ((object)(resources.GetObject("appearance105.Image")));
            buttonTool198.SharedPropsInternal.AppearancesSmall.Appearance = appearance105;
            buttonTool198.SharedPropsInternal.Caption = "Non-Subscribed Queue Group By Box";
            appearance106.Image = ((object)(resources.GetObject("appearance106.Image")));
            buttonTool199.SharedPropsInternal.AppearancesLarge.Appearance = appearance106;
            buttonTool199.SharedPropsInternal.Caption = "Optimize";
            buttonTool199.SharedPropsInternal.Enabled = false;
            appearance107.Image = ((object)(resources.GetObject("appearance107.Image")));
            buttonTool200.SharedPropsInternal.AppearancesLarge.Appearance = appearance107;
            buttonTool200.SharedPropsInternal.Caption = "Rebuild";
            buttonTool200.SharedPropsInternal.Enabled = false;
            appearance108.Image = ((object)(resources.GetObject("appearance108.Image")));
            buttonTool201.SharedPropsInternal.AppearancesLarge.Appearance = appearance108;
            buttonTool201.SharedPropsInternal.Caption = "Repopulate";
            buttonTool201.SharedPropsInternal.Enabled = false;
            appearance109.Image = ((object)(resources.GetObject("appearance109.Image")));
            buttonTool202.SharedPropsInternal.AppearancesLarge.Appearance = appearance109;
            buttonTool202.SharedPropsInternal.Caption = "Start";
            buttonTool202.SharedPropsInternal.Enabled = false;
            appearance110.Image = ((object)(resources.GetObject("appearance110.Image")));
            buttonTool203.SharedPropsInternal.AppearancesLarge.Appearance = appearance110;
            buttonTool203.SharedPropsInternal.Caption = "Stop";
            buttonTool203.SharedPropsInternal.Enabled = false;
            appearance111.Image = ((object)(resources.GetObject("appearance111.Image")));
            buttonTool204.SharedPropsInternal.AppearancesSmall.Appearance = appearance111;
            buttonTool204.SharedPropsInternal.Caption = "Group By Box";
            appearance112.Image = ((object)(resources.GetObject("appearance112.Image")));
            buttonTool205.SharedPropsInternal.AppearancesSmall.Appearance = appearance112;
            buttonTool205.SharedPropsInternal.Caption = "Group By Box";
            appearance113.Image = ((object)(resources.GetObject("appearance113.Image")));
            buttonTool206.SharedPropsInternal.AppearancesLarge.Appearance = appearance113;
            buttonTool206.SharedPropsInternal.Caption = "Start Service";
            buttonTool206.SharedPropsInternal.Enabled = false;
            appearance114.Image = ((object)(resources.GetObject("appearance114.Image")));
            buttonTool207.SharedPropsInternal.AppearancesLarge.Appearance = appearance114;
            buttonTool207.SharedPropsInternal.Caption = "Stop Service";
            buttonTool207.SharedPropsInternal.Enabled = false;
            appearance115.Image = ((object)(resources.GetObject("appearance115.Image")));
            buttonTool208.SharedPropsInternal.AppearancesSmall.Appearance = appearance115;
            buttonTool208.SharedPropsInternal.Caption = "Chart";
            appearance116.Image = ((object)(resources.GetObject("appearance116.Image")));
            buttonTool209.SharedPropsInternal.AppearancesSmall.Appearance = appearance116;
            buttonTool209.SharedPropsInternal.Caption = "Catalog Details";
            appearance117.Image = ((object)(resources.GetObject("appearance117.Image")));
            buttonTool210.SharedPropsInternal.AppearancesLarge.Appearance = appearance117;
            buttonTool210.SharedPropsInternal.Caption = "Start Service";
            buttonTool210.SharedPropsInternal.Enabled = false;
            appearance118.Image = ((object)(resources.GetObject("appearance118.Image")));
            buttonTool211.SharedPropsInternal.AppearancesLarge.Appearance = appearance118;
            buttonTool211.SharedPropsInternal.Caption = "Stop Service";
            buttonTool211.SharedPropsInternal.Enabled = false;
            appearance119.Image = ((object)(resources.GetObject("appearance119.Image")));
            buttonTool212.SharedPropsInternal.AppearancesLarge.Appearance = appearance119;
            buttonTool212.SharedPropsInternal.Caption = "Configure Activity Monitor";
            appearance120.Image = ((object)(resources.GetObject("appearance120.Image")));
            buttonTool213.SharedPropsInternal.AppearancesSmall.Appearance = appearance120;
            buttonTool213.SharedPropsInternal.Caption = "Show Animations";
            appearance121.Image = ((object)(resources.GetObject("appearance121.Image")));
            stateButtonTool76.SharedPropsInternal.AppearancesLarge.Appearance = appearance121;
            stateButtonTool76.SharedPropsInternal.Caption = "History Browser";
            appearance122.Image = ((object)(resources.GetObject("appearance122.Image")));
            buttonTool214.SharedPropsInternal.AppearancesLarge.Appearance = appearance122;
            buttonTool214.SharedPropsInternal.Caption = "Refresh Alerts";
            buttonTool214.SharedPropsInternal.Visible = false;
            appearance123.Image = ((object)(resources.GetObject("appearance123.Image")));
            buttonTool215.SharedPropsInternal.AppearancesLarge.Appearance = appearance123;
            buttonTool215.SharedPropsInternal.Caption = "Previous Snapshot";
            appearance124.Image = ((object)(resources.GetObject("appearance124.Image")));
            buttonTool216.SharedPropsInternal.AppearancesLarge.Appearance = appearance124;
            buttonTool216.SharedPropsInternal.Caption = "Next Snapshot";
            appearance125.Image = ((object)(resources.GetObject("appearance125.Image")));
            buttonTool217.SharedPropsInternal.AppearancesSmall.Appearance = appearance125;
            buttonTool217.SharedPropsInternal.Caption = "Show All";
            appearance126.Image = ((object)(resources.GetObject("appearance126.Image")));
            buttonTool218.SharedPropsInternal.AppearancesSmall.Appearance = appearance126;
            buttonTool218.SharedPropsInternal.Caption = "Custom Counters";
            appearance127.Image = ((object)(resources.GetObject("appearance127.Image")));
            buttonTool219.SharedPropsInternal.AppearancesLarge.Appearance = appearance127;
            buttonTool219.SharedPropsInternal.Caption = "Start Job";
            buttonTool219.SharedPropsInternal.Enabled = false;
            appearance128.Image = ((object)(resources.GetObject("appearance128.Image")));
            buttonTool220.SharedPropsInternal.AppearancesLarge.Appearance = appearance128;
            buttonTool220.SharedPropsInternal.Caption = "Stop Job";
            buttonTool220.SharedPropsInternal.Enabled = false;
            stateButtonTool77.MenuDisplayStyle = Infragistics.Win.UltraWinToolbars.StateButtonMenuDisplayStyle.DisplayCheckmark;
            stateButtonTool77.OptionSetKey = "overviewTabViewOptionSet";
            appearance129.Image = ((object)(resources.GetObject("appearance129.Image")));
            stateButtonTool77.SharedPropsInternal.AppearancesLarge.Appearance = appearance129;
            stateButtonTool77.SharedPropsInternal.Caption = "Active Alerts";
            appearance130.Image = ((object)(resources.GetObject("appearance130.Image")));
            buttonTool221.SharedPropsInternal.AppearancesSmall.Appearance = appearance130;
            buttonTool221.SharedPropsInternal.Caption = "Group By Box";
            appearance131.Image = ((object)(resources.GetObject("appearance131.Image")));
            buttonTool222.SharedPropsInternal.AppearancesSmall.Appearance = appearance131;
            buttonTool222.SharedPropsInternal.Caption = "Details";
            stateButtonTool78.OptionSetKey = "queriesTabViewOptionSet";
            appearance132.Image = global::Idera.SQLdm.DesktopClient.Properties.Resources.QueriesSignature;
            stateButtonTool78.SharedPropsInternal.AppearancesLarge.Appearance = appearance132;
            stateButtonTool78.SharedPropsInternal.Caption = "Signature Mode";
            stateButtonTool79.OptionSetKey = "queriesTabViewOptionSet";
            appearance133.Image = global::Idera.SQLdm.DesktopClient.Properties.Resources.QueriesThumbnail;
            stateButtonTool79.SharedPropsInternal.AppearancesLarge.Appearance = appearance133;
            stateButtonTool79.SharedPropsInternal.Caption = "Statement Mode";
            stateButtonTool80.OptionSetKey = "databasesTabViewOptionSet";
            appearance134.Image = ((object)(resources.GetObject("appearance134.Image")));
            stateButtonTool80.SharedPropsInternal.AppearancesLarge.Appearance = appearance134;
            appearance135.Image = ((object)(resources.GetObject("appearance135.Image")));
            stateButtonTool80.SharedPropsInternal.AppearancesSmall.Appearance = appearance135;
            stateButtonTool80.SharedPropsInternal.Caption = "Mirroring";
            appearance136.Image = ((object)(resources.GetObject("appearance136.Image")));
            buttonTool223.SharedPropsInternal.AppearancesSmall.Appearance = appearance136;
            buttonTool223.SharedPropsInternal.Caption = "Mirrored Databases Group By Box";
            appearance137.Image = ((object)(resources.GetObject("appearance137.Image")));
            buttonTool224.SharedPropsInternal.AppearancesSmall.Appearance = appearance137;
            buttonTool224.SharedPropsInternal.Caption = "Mirroring History Group By Box";
            appearance138.Image = ((object)(resources.GetObject("appearance138.Image")));
            buttonTool225.SharedPropsInternal.AppearancesLarge.Appearance = appearance138;
            appearance139.Image = ((object)(resources.GetObject("appearance139.Image")));
            buttonTool225.SharedPropsInternal.AppearancesSmall.Appearance = appearance139;
            buttonTool225.SharedPropsInternal.Caption = "Fail Over";
            appearance140.Image = ((object)(resources.GetObject("appearance140.Image")));
            buttonTool226.SharedPropsInternal.AppearancesLarge.Appearance = appearance140;
            appearance141.Image = ((object)(resources.GetObject("appearance141.Image")));
            buttonTool226.SharedPropsInternal.AppearancesSmall.Appearance = appearance141;
            buttonTool226.SharedPropsInternal.Caption = "Suspend\\Resume";
            buttonTool226.SharedPropsInternal.ToolTipText = "Suspend\\resume the selected mirroring session";
            appearance142.Image = ((object)(resources.GetObject("appearance142.Image")));
            buttonTool227.SharedPropsInternal.AppearancesSmall.Appearance = appearance142;
            buttonTool227.SharedPropsInternal.Caption = "Replication Charts";
            stateButtonTool81.OptionSetKey = "resourcesTabViewOptionSet";
            appearance143.Image = ((object)(resources.GetObject("appearance143.Image")));
            stateButtonTool81.SharedPropsInternal.AppearancesLarge.Appearance = appearance143;
            stateButtonTool81.SharedPropsInternal.Caption = "Server Waits";
            stateButtonTool81.SharedPropsInternal.CustomizerCaption = "Server Wait Statistics";
            stateButtonTool82.SharedPropsInternal.Caption = "Query Waits";
            stateButtonTool82.SharedPropsInternal.CustomizerCaption = "Query Wait Statistics";
            stateButtonTool83.OptionSetKey = "queriesTabViewOptionSet";
            appearance144.Image = ((object)(resources.GetObject("appearance144.Image")));
            stateButtonTool83.SharedPropsInternal.AppearancesLarge.Appearance = appearance144;
            stateButtonTool83.SharedPropsInternal.Caption = "Query Waits";
            stateButtonTool83.SharedPropsInternal.CustomizerCaption = "Query Wait Statistics";
            appearance145.Image = ((object)(resources.GetObject("appearance145.Image")));
            buttonTool228.SharedPropsInternal.AppearancesLarge.Appearance = appearance145;
            buttonTool228.SharedPropsInternal.Caption = "Filter";
            appearance146.Image = ((object)(resources.GetObject("appearance146.Image")));
            buttonTool229.SharedPropsInternal.AppearancesLarge.Appearance = appearance146;
            buttonTool229.SharedPropsInternal.Caption = "Configure";
            appearance147.Image = ((object)(resources.GetObject("appearance147.Image")));
            buttonTool230.SharedPropsInternal.AppearancesSmall.Appearance = appearance147;
            buttonTool230.SharedPropsInternal.Caption = "Group By Box";
            appearance148.Image = ((object)(resources.GetObject("appearance148.Image")));
            buttonTool231.SharedPropsInternal.AppearancesSmall.Appearance = appearance148;
            buttonTool231.SharedPropsInternal.Caption = "List";
            stateButtonTool84.OptionSetKey = "queriesTabViewOptionSet";
            appearance149.Image = ((object)(resources.GetObject("appearance149.Image")));
            stateButtonTool84.SharedPropsInternal.AppearancesLarge.Appearance = appearance149;
            stateButtonTool84.SharedPropsInternal.Caption = "Query History";
            stateButtonTool85.OptionSetKey = "resourcesTabViewOptionSet";
            appearance150.Image = ((object)(resources.GetObject("appearance150.Image")));
            stateButtonTool85.SharedPropsInternal.AppearancesLarge.Appearance = appearance150;
            stateButtonTool85.SharedPropsInternal.Caption = "File Activity";
            stateButtonTool85.SharedPropsInternal.CustomizerCaption = "File Activity";
            buttonTool232.SharedPropsInternal.Caption = "Show Filter";
            buttonTool233.SharedPropsInternal.Caption = "Show Filter";
            appearance151.Image = ((object)(resources.GetObject("appearance151.Image")));
            buttonTool234.SharedPropsInternal.AppearancesLarge.Appearance = appearance151;
            buttonTool234.SharedPropsInternal.Caption = "View Server Profile";
            appearance152.Image = ((object)(resources.GetObject("appearance152.Image")));
            buttonTool235.SharedPropsInternal.AppearancesLarge.Appearance = appearance152;
            buttonTool235.SharedPropsInternal.Caption = "Post to Server Wall";
            appearance153.Image = ((object)(resources.GetObject("appearance153.Image")));
            buttonTool236.SharedPropsInternal.AppearancesLarge.Appearance = appearance153;
            buttonTool236.SharedPropsInternal.Caption = "Follow Server";
            appearance154.Image = ((object)(resources.GetObject("appearance154.Image")));
            buttonTool237.SharedPropsInternal.AppearancesSmall.Appearance = appearance154;
            buttonTool237.SharedPropsInternal.Caption = "12 Hour Forecast";
            appearance155.Image = ((object)(resources.GetObject("appearance155.Image")));
            buttonTool238.SharedPropsInternal.AppearancesLarge.Appearance = appearance155;
            appearance156.Image = ((object)(resources.GetObject("appearance156.Image")));
            buttonTool238.SharedPropsInternal.AppearancesSmall.Appearance = appearance156;
            buttonTool238.SharedPropsInternal.Caption = "Properties";
            appearance157.Image = ((object)(resources.GetObject("appearance157.Image")));
            buttonTool239.SharedPropsInternal.AppearancesLarge.Appearance = appearance157;
            buttonTool239.SharedPropsInternal.Caption = "Tempdb Summary";
            stateButtonTool86.OptionSetKey = "databasesTabViewOptionSet";
            appearance158.Image = global::Idera.SQLdm.DesktopClient.Properties.Resources.tempdb_32;
            stateButtonTool86.SharedPropsInternal.AppearancesLarge.Appearance = appearance158;
            stateButtonTool86.SharedPropsInternal.Caption = "Tempdb Summary";
            buttonTool240.SharedPropsInternal.Caption = "History Browser";
            appearance159.Image = global::Idera.SQLdm.DesktopClient.Properties.Resources.FilterLarge;
            stateButtonTool87.SharedPropsInternal.AppearancesLarge.Appearance = appearance159;
            stateButtonTool87.SharedPropsInternal.Caption = "Sessions Filter";
            appearance160.Image = global::Idera.SQLdm.DesktopClient.Properties.Resources.RibbonCheckboxUnchecked;
            buttonTool241.SharedPropsInternal.AppearancesSmall.Appearance = appearance160;
            buttonTool241.SharedPropsInternal.Caption = "Group By Box";
            //10.0 SQLdm srishti purohit -- dostoc's UI
            ribbonTabAnalysis.Caption = "Analysis";
            ribbonGroupRunAnalysis.Caption = "Run Analysis";
            stateButtonToolRunAnalysis.MenuDisplayStyle = Infragistics.Win.UltraWinToolbars.StateButtonMenuDisplayStyle.DisplayCheckmark;
            stateButtonToolRunAnalysis.OptionSetKey = "analysisTabViewOptionSet";
            appearanceRunAnalysis.Image = ((object)(resources.GetObject("appearance10.Image")));
            stateButtonToolRunAnalysis.SharedPropsInternal.AppearancesLarge.Appearance = appearanceRunAnalysis;
            stateButtonToolRunAnalysis.SharedPropsInternal.Caption = "Run Analysis";

            stateButtonToolRunAnalysis.InstanceProps.MinimumSizeOnRibbon = Infragistics.Win.UltraWinToolbars.RibbonToolSize.Large;
            stateButtonToolRunAnalysis.InstanceProps.PreferredSizeOnRibbon = Infragistics.Win.UltraWinToolbars.RibbonToolSize.Large;
            stateButtonToolRunAnalysis.MenuDisplayStyle = Infragistics.Win.UltraWinToolbars.StateButtonMenuDisplayStyle.DisplayCheckmark;

            ribbonGroupRunAnalysis.Tools.AddRange(new Infragistics.Win.UltraWinToolbars.ToolBase[]{
                stateButtonToolRunAnalysis
            });
            ribbonTabAnalysis.Groups.AddRange(new Infragistics.Win.UltraWinToolbars.RibbonGroup[]{
                ribbonGroupRunAnalysis
            });
            ribbonTabAnalysis.Settings.TabItemAppearance = appearanceRunAnalysis;
            
            
            this.toolbarsManager.Tools.AddRange(new Infragistics.Win.UltraWinToolbars.ToolBase[] {
            stateButtonTool46,
            stateButtonTool47,
            stateButtonTool48,
            buttonTool132,
            stateButtonTool49,
            buttonTool133,
            stateButtonTool50,
            stateButtonTool51,
            buttonTool134,
            buttonTool135,
            buttonTool136,
            buttonTool137,
            buttonTool138,
            buttonTool139,
            buttonTool140,
            buttonTool141,
            buttonTool142,
            stateButtonTool52,
            stateButtonTool53,
            buttonTool143,
            buttonTool144,
            stateButtonTool54,
            buttonTool145,
            buttonTool146,
            buttonTool147,
            stateButtonTool55,
            buttonTool148,
            buttonTool149,
            stateButtonTool56,
            buttonTool150,
            buttonTool151,
            buttonTool152,
            buttonTool153,
            buttonTool154,
            buttonTool155,
            buttonTool156,
            stateButtonTool57,
            stateButtonTool58,
            buttonTool157,
            buttonTool158,
            buttonTool159,
            buttonTool160,
            buttonTool161,
            buttonTool162,
            buttonTool163,
            buttonTool164,
            buttonTool165,
            stateButtonTool59,
            stateButtonTool60,
            stateButtonTool61,
            stateButtonTool62,
            stateButtonTool63,
            buttonTool166,
            buttonTool167,
            buttonTool168,
            buttonTool169,
            buttonTool170,
            buttonTool171,
            buttonTool172,
            buttonTool173,
            stateButtonTool64,
            stateButtonTool65,
            stateButtonTool66,
            stateButtonTool67,
            stateButtonTool68,
            buttonTool174,
            buttonTool175,
            buttonTool176,
            buttonTool177,
            buttonTool178,
            buttonTool179,
            buttonTool180,
            buttonTool181,
            stateButtonTool69,
            buttonTool182,
            buttonTool183,
            buttonTool184,
            buttonTool185,
            buttonTool186,
            buttonTool187,
            buttonTool188,
            buttonTool189,
            buttonTool190,
            buttonTool191,
            stateButtonTool70,
            stateButtonTool71,
            stateButtonTool72,
            stateButtonTool73,
            stateButtonTool74,
            buttonTool192,
            buttonTool193,
            buttonTool194,
            buttonTool195,
            buttonTool196,
            stateButtonTool75,
            buttonTool197,
            buttonTool198,
            buttonTool199,
            buttonTool200,
            buttonTool201,
            buttonTool202,
            buttonTool203,
            buttonTool204,
            buttonTool205,
            buttonTool206,
            buttonTool207,
            buttonTool208,
            buttonTool209,
            buttonTool210,
            buttonTool211,
            buttonTool212,
            buttonTool213,
            stateButtonTool76,
            buttonTool214,
            buttonTool215,
            buttonTool216,
            buttonTool217,
            buttonTool218,
            buttonTool219,
            buttonTool220,
            stateButtonTool77,
            buttonTool221,
            buttonTool222,
            stateButtonTool78,
            stateButtonTool79,
            stateButtonTool80,
            buttonTool223,
            buttonTool224,
            buttonTool225,
            buttonTool226,
            buttonTool227,
            stateButtonTool81,
            stateButtonTool82,
            stateButtonTool83,
            buttonTool228,
            buttonTool229,
            buttonTool230,
            buttonTool231,
            stateButtonTool84,
            stateButtonTool85,
            buttonTool232,
            buttonTool233,
            buttonTool234,
            buttonTool235,
            buttonTool236,
            buttonTool237,
            buttonTool238,
            buttonTool239,
            stateButtonTool86,
            buttonTool240,
            stateButtonTool87,
            buttonTool241,
            stateButtonTool88});
            this.toolbarsManager.AfterRibbonTabSelected += new Infragistics.Win.UltraWinToolbars.RibbonTabEventHandler(this.toolbarsManager_AfterRibbonTabSelected);
            this.toolbarsManager.BeforeToolbarListDropdown += new Infragistics.Win.UltraWinToolbars.BeforeToolbarListDropdownEventHandler(this.toolbarsManager_BeforeToolbarListDropdown);
            this.toolbarsManager.ToolClick += new Infragistics.Win.UltraWinToolbars.ToolClickEventHandler(this.toolbarsManager_ToolClick);
            this.toolbarsManager.MouseEnterElement += new Infragistics.Win.UIElementEventHandler(this.toolbarsManager_MouseEnterElement);
            this.toolbarsManager.MouseLeaveElement += new Infragistics.Win.UIElementEventHandler(this.toolbarsManager_MouseLeaveElement);
            // 
            // _ServerViewContainer_Toolbars_Dock_Area_Right
            // 
            this._ServerViewContainer_Toolbars_Dock_Area_Right.AccessibleRole = System.Windows.Forms.AccessibleRole.Grouping;
            this._ServerViewContainer_Toolbars_Dock_Area_Right.BackColor = System.Drawing.SystemColors.Control;
            this._ServerViewContainer_Toolbars_Dock_Area_Right.DockedPosition = Infragistics.Win.UltraWinToolbars.DockedPosition.Right;
            this._ServerViewContainer_Toolbars_Dock_Area_Right.ForeColor = System.Drawing.SystemColors.ControlText;
            this._ServerViewContainer_Toolbars_Dock_Area_Right.Location = new System.Drawing.Point(743, 0);
            this._ServerViewContainer_Toolbars_Dock_Area_Right.Name = "_ServerViewContainer_Toolbars_Dock_Area_Right";
            this._ServerViewContainer_Toolbars_Dock_Area_Right.Size = new System.Drawing.Size(0, 615);
            this._ServerViewContainer_Toolbars_Dock_Area_Right.ToolbarsManager = this.toolbarsManager;
            // 
            // _ServerViewContainer_Toolbars_Dock_Area_Top
            // 
            this._ServerViewContainer_Toolbars_Dock_Area_Top.AccessibleRole = System.Windows.Forms.AccessibleRole.Grouping;
            this._ServerViewContainer_Toolbars_Dock_Area_Top.BackColor = System.Drawing.SystemColors.Control;
            this._ServerViewContainer_Toolbars_Dock_Area_Top.DockedPosition = Infragistics.Win.UltraWinToolbars.DockedPosition.Top;
            this._ServerViewContainer_Toolbars_Dock_Area_Top.ForeColor = System.Drawing.SystemColors.ControlText;
            this._ServerViewContainer_Toolbars_Dock_Area_Top.Location = new System.Drawing.Point(0, 0);
            this._ServerViewContainer_Toolbars_Dock_Area_Top.Name = "_ServerViewContainer_Toolbars_Dock_Area_Top";
            this._ServerViewContainer_Toolbars_Dock_Area_Top.Size = new System.Drawing.Size(743, 0);
            this._ServerViewContainer_Toolbars_Dock_Area_Top.ToolbarsManager = this.toolbarsManager;
            // 
            // _ServerViewContainer_Toolbars_Dock_Area_Bottom
            // 
            this._ServerViewContainer_Toolbars_Dock_Area_Bottom.AccessibleRole = System.Windows.Forms.AccessibleRole.Grouping;
            this._ServerViewContainer_Toolbars_Dock_Area_Bottom.BackColor = System.Drawing.SystemColors.Control;
            this._ServerViewContainer_Toolbars_Dock_Area_Bottom.DockedPosition = Infragistics.Win.UltraWinToolbars.DockedPosition.Bottom;
            this._ServerViewContainer_Toolbars_Dock_Area_Bottom.ForeColor = System.Drawing.SystemColors.ControlText;
            this._ServerViewContainer_Toolbars_Dock_Area_Bottom.Location = new System.Drawing.Point(0, 615);
            this._ServerViewContainer_Toolbars_Dock_Area_Bottom.Name = "_ServerViewContainer_Toolbars_Dock_Area_Bottom";
            this._ServerViewContainer_Toolbars_Dock_Area_Bottom.Size = new System.Drawing.Size(743, 0);
            this._ServerViewContainer_Toolbars_Dock_Area_Bottom.ToolbarsManager = this.toolbarsManager;
            // 
            // ServerViewRibbonControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(235)))), ((int)(((byte)(235)))), ((int)(((byte)(235)))));
            this.Controls.Add(this.subViewContainerPanel);
            this.Controls.Add(this._ServerViewContainer_Toolbars_Dock_Area_Left);
            this.Controls.Add(this._ServerViewContainer_Toolbars_Dock_Area_Right);
            this.Controls.Add(this._ServerViewContainer_Toolbars_Dock_Area_Top);
            this.Controls.Add(this._ServerViewContainer_Toolbars_Dock_Area_Bottom);
            this.Name = "ServerViewRibbonControl";
            this.Size = new System.Drawing.Size(743, 615);
            ((System.ComponentModel.ISupportInitialize)(this.toolbarsManager)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private Idera.SQLdm.DesktopClient.Controls.ContextMenuManager toolbarsManager;
        private System.Windows.Forms.Panel subViewContainerPanel;
        private Infragistics.Win.UltraWinToolbars.UltraToolbarsDockArea _ServerViewContainer_Toolbars_Dock_Area_Left;
        private Infragistics.Win.UltraWinToolbars.UltraToolbarsDockArea _ServerViewContainer_Toolbars_Dock_Area_Right;
        private Infragistics.Win.UltraWinToolbars.UltraToolbarsDockArea _ServerViewContainer_Toolbars_Dock_Area_Top;
        private Infragistics.Win.UltraWinToolbars.UltraToolbarsDockArea _ServerViewContainer_Toolbars_Dock_Area_Bottom;
    }
}
