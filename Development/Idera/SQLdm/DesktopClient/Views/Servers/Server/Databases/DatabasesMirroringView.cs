using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using ChartFX.WinForms;
using Idera.SQLdm.Common;
using Idera.SQLdm.Common.Auditing;
using Idera.SQLdm.Common.Configuration;
using Idera.SQLdm.Common.Objects;
using Idera.SQLdm.Common.Services;
using Idera.SQLdm.Common.Snapshots;
using Idera.SQLdm.Common.UI.Dialogs;
using Idera.SQLdm.DesktopClient.Controls;
using Idera.SQLdm.DesktopClient.Dialogs;
using Idera.SQLdm.DesktopClient.Helpers;
using Idera.SQLdm.DesktopClient.Objects;
using Idera.SQLdm.DesktopClient.Properties;
using Infragistics.Win;
using Infragistics.Win.UltraWinGrid;
using Infragistics.Win.UltraWinTabControl;
using Infragistics.Win.UltraWinToolbars;
using Wintellect.PowerCollections;
using ColumnHeader = Infragistics.Win.UltraWinGrid.ColumnHeader;
using Idera.SQLdm.Common.Events;
using System.Diagnostics;
using System.ComponentModel;
using Idera.SQLdm.Common.Configuration.ServerActions;
using Idera.SQLdm.Common.Objects.ApplicationSecurity;
using Infragistics.Windows.Themes;

namespace Idera.SQLdm.DesktopClient.Views.Servers.Server.Databases
{
    internal partial class DatabasesMirroringView : ServerBaseView, IDatabasesView
    {
        #region constants

        private const string AllDatabasesItemText = "< All Databases >";
        private const string NO_ITEMS = @"There are no mirrored databases to show in this view.";
        private const string UNABLE_TO_UPDATE = @"Unable to update data for this view.";
        private const string DB_NOT_SUPPORTED = "Database {0} cannot be selected.\nDatabase compatibility level 60 is not supported.";
        private const string NOT_MONITORING_BOTH_PARTNERS = "Note: You are not monitoring both mirroring partners. For complete coverage of mirroring metrics please monitor both partners.";
        private const string PARTNER_REFRESH_ERROR = "An error has occurred while trying to connect to the mirroring partner {0}.";
        private DateTime? historicalSnapshotDateTime;
        #endregion
        #region fields
        
        private enum ViewStatus
        {
            NoItems,
            FailingOver,
            Suspending,
            Refreshing,
            Normal
        }
        private ViewStatus currentViewStatus = ViewStatus.Normal;
        /// <summary>
        /// This is the database that has been selected by the combo box
        /// </summary>
        private string selectedDatabaseFilter;

        /// <summary>
        /// set during the population of the history grid
        /// </summary>
        private bool blnPopulatingHistory = false;
        /// <summary>
        /// set during the population of the details tab
        /// </summary>
        private bool blnPopulatingDetails = false;
        /// <summary>
        /// set during the population of the details tab
        /// </summary>
        private bool blnPopulatingPartnerDetails = false;
        /// <summary>
        /// Datatable containing the available mirrored databases for the selected server
        /// </summary>
        private DataTable mirroredDatabasesDataTable;
        /// <summary>
        /// Datatable containign all history for the selected server and database
        /// </summary>
        private DataTable historyDataTable;
        /// <summary>
        /// This config is for the initial harvesting of all mirrored databases on a specific server
        /// </summary>
        private MirrorMonitoringRealtimeConfiguration mirroringRealtimeConfig;
        /// <summary>
        /// After selecting a specific mirroring session, this config is used to gather the realtime details of the partne
        /// </summary>
        private MirrorMonitoringRealtimeConfiguration partnerConfig;
        /// <summary>
        /// After selecting a specific mirroring session, this config is used to gather the realtime details of the local instance
        /// </summary>
        private MirrorMonitoringRealtimeConfiguration localConfig;
        /// <summary>
        /// This config is for harvesting the history associated with a mirroring session
        /// </summary>
        private MirrorMonitoringHistoryConfiguration historyConfig;
        /// <summary>
        /// set once the mirrored databases grid has been initialized
        /// </summary>
        private bool initializedMirroredDatabasesGrid;
        /// <summary>
        /// set once the history grid has been initialized
        /// </summary>
        private bool initializedMirroringHistoryGrid;
        /// <summary>
        /// this flags the completion of the asynchronous population of the combo box
        /// Not used yet
        /// </summary>
        //private bool availableDatabasesInitialized = false;
        /// <summary>
        /// Flags the completion of the initial population of the mirrored databases combo and the grid
        /// </summary>
        private bool blnMirroredDatabasesComboInitialized;
        /// <summary>
        /// Times the collection of the list of mirrored databases
        /// </summary>
        private Stopwatch mirroredDatabasesRefreshStopwatch = new Stopwatch();
        /// <summary>
        /// Times the collection of the history (local and partner)
        /// </summary>
        private Stopwatch mirroringHistoryRefreshStopwatch = new Stopwatch();
        /// <summary>
        /// Times the collection of the details (local)
        /// </summary>
        private Stopwatch mirroringDetailsRefreshStopwatch = new Stopwatch();
        /// <summary>
        /// Times the collection of the details (partner)
        /// </summary>
        private Stopwatch partnerDetailsRefreshStopwatch = new Stopwatch();       
        private Stopwatch failOverStopwatch = new Stopwatch();
        private Stopwatch suspendResumeStopwatch = new Stopwatch();
        private Stopwatch setOperationalStatusStopwatch = new Stopwatch();
        /// <summary>
        /// Refresh busy flags 
        /// 
        /// 1 - Local details
        /// 2 - partner details
        /// </summary>
        private int intRefreshBusyFlags;
        /// <summary>
        /// Databases selected in the mirrored databases grid
        /// </summary>
        private string selectedDatabase;
        /// <summary>
        /// Server selected in the mirrored databases grid
        /// </summary>
        private string selectedServer;
        /// <summary>
        /// Partner in the selected row of the mirrored databases grid
        /// </summary>
        private string selectedPartner;
        /// <summary>
        /// Lock object to ensure that updateData is thread safe
        /// </summary>
        private static readonly object updatingMirroredDatabasesLock = new object();
        //private DataTable currentDataTable;
        //private DatabaseSummaryConfiguration configuration;
        //private MirrorMonitoringRealtimeSnapshot currentSnapshot = null;
        /// <summary>
        /// Contains all data pertaining to the partner of the selected server in the mirroring session
        /// </summary>
        private MirrorMonitoringRealtimeSnapshot partnerSnapshot = null;
        /// <summary>
        /// All data pertaining to the selected server's mirrored database in the selected mirroring session
        /// </summary>
        private MirrorMonitoringRealtimeSnapshot localSnapshot = null;
        /// <summary>
        /// Contains all history data for the selected database and server (selected server or partner)
        /// </summary>
        private MirrorMonitoringHistorySnapshot historySnapshot = null;
        /// <summary>
        /// Contains a sorted list of all of the mirrored databases on the selected server
        /// </summary>
        SortedList<string, MirrorMonitoringDatabaseDetail> mirroredDatabases = new SortedList<string, MirrorMonitoringDatabaseDetail>();

        private List<string> selectedDatabaseArgument = new List<string>();
        SortedList<string, string> selectedDatabases = new SortedList<string, string>();
        //private bool selectionChanged = false;
        //SortedList<string, Pair<string, string>> selectedFiles = new SortedList<string, Pair<string, string>>();
        //SortedList<string, Pair<string, string>> selectedFileGroups = new SortedList<string, Pair<string, string>>();
        //SortedList<string, string> selectedLogs = new SortedList<string, string>();
        //private bool showDiskOtherFiles = true;
        //private static readonly object updateLock = new object();
        //private Chart contextMenuSelectedChart = null;
        //private Control focused = null;
        private UltraGridColumn selectedColumn = null;
        private UltraGrid selectedGrid = null;


        //last Settings values used to determine if changed for saving when leaving
        private int lastMainSplitterDistance = 0;
        private GridSettings lastmirroringGridSettings = null;
        private GridSettings lasthistoryGridSettings = null;
        
        public event EventHandler MirroredDatabasesGridGroupByBoxVisibleChanged;
        public event EventHandler MirroringHistoryGridGroupByBoxVisibleChanged;
        public event EventHandler ActionsAllowedChanged;

        #endregion
        #region Constructors
        public DatabasesMirroringView(int instanceId): base(instanceId)
        {
            InitializeComponent();
            mirroringRealtimeConfig = new MirrorMonitoringRealtimeConfiguration(instanceId);
            historicalSnapshotStatusLinkLabel.Text = Properties.Resources.HistoryModeUnsupportedViewLabel;
            InitializeMirroredDatabasesTable();
            InitializeHistoryDataTable();
            AdaptFontSize();
            SetGridTheme();
            ThemeManager.CurrentThemeChanged += new EventHandler(OnCurrentThemeChanged);
        }
        #endregion
        #region IDatabasesView Members

        public string SelectedDatabaseFilter
        {
            get { return selectedDatabaseFilter; }
        }
        public override DateTime? HistoricalSnapshotDateTime
        {
            get { return historicalSnapshotDateTime; }
            set { historicalSnapshotDateTime = value; }
        }
        #endregion
        #region Properties
        public bool MirroredDatabasesGridGroupByBoxVisible
        {
            get { return !mirroredDatabasesGrid.DisplayLayout.GroupByBox.Hidden; }
            set
            {
                mirroredDatabasesGrid.DisplayLayout.GroupByBox.Hidden = !value;

                if (MirroredDatabasesGridGroupByBoxVisibleChanged != null)
                {
                    MirroredDatabasesGridGroupByBoxVisibleChanged(this, EventArgs.Empty);
                }
            }
        }
        public bool MirroringHistoryGridGroupByBoxVisible
        {
            get { return !historyGrid.DisplayLayout.GroupByBox.Hidden; }
            set
            {
                historyGrid.DisplayLayout.GroupByBox.Hidden = !value;

                if (MirroringHistoryGridGroupByBoxVisibleChanged != null)
                {
                    MirroringHistoryGridGroupByBoxVisibleChanged(this, EventArgs.Empty);
                }
            }
        }
        /// <summary>
        /// Is failing over now possible given the server hosting the selected session.
        /// We must be monitoring both partners
        /// we must have modify rights
        /// Safety level must be FULL
        /// </summary>
        public bool ActionsAllowed
        {
            get {
                //if we are in history mode then disable actions
                if (HistoricalSnapshotDateTime != null) return false;
                if (mirroredDatabases.Count == 0) return false;
                if(mirroredDatabasesGrid.Selected.Rows.Count > 0)
                {
                    MirrorMonitoringDatabaseDetail detail = getSelectedMirroredDatabaseDetails();
                    if (detail.PrincipalName.ToLower() != "unknown")
                    {
                        //are we monitoring the local instance?
                        int intServer = ServerNameToID(detail.PrincipalName);
                        int intMirror = detail.MirrorName.ToLower().Equals("unknown")?-1:ServerNameToID(detail.MirrorName);
                        bool blnThreadsAreBusy = false;
                        
                        if((refreshMirroringDetailsBackgroundWorker != null) && (refreshMirroringDetailsBackgroundWorker.IsBusy)) blnThreadsAreBusy = true;
                        if((refreshPartnerDetailsBackgroundWorker != null) && (refreshPartnerDetailsBackgroundWorker.IsBusy)) blnThreadsAreBusy = true;
                        if((failOverBackgroundWorker != null) && (failOverBackgroundWorker.IsBusy)) blnThreadsAreBusy = true;
                        if((suspendResumeBackgroundWorker != null) && (suspendResumeBackgroundWorker.IsBusy)) blnThreadsAreBusy = true;

                        if (intServer >= 0 && intMirror >= 0 && 
                            ((detail.CurrentMirroringMetrics.MirroringState == MirroringMetrics.MirroringStateEnum.Synchronized) || detail.CurrentMirroringMetrics.MirroringState == MirroringMetrics.MirroringStateEnum.Suspended)
                            && (!blnThreadsAreBusy) && currentViewStatus== ViewStatus.Normal)
                        {
                            if ((ApplicationModel.Default.UserToken.GetServerPermission(intServer) >= PermissionType.Modify)
                                && (ApplicationModel.Default.UserToken.GetServerPermission(intMirror)>= PermissionType.Modify)
                                && (detail.SafetyLevel==MirrorMonitoringDatabaseDetail.SafetyLevelEnum.Full))
                            {
                                return true;
                            }
                        }
                    }
                }

                return false; 
            }
        }
        /// <summary>
        /// Is setting operational status possible given the server hosting the selected session.
        /// We must be monitoring the both partners
        /// we must have modify rights
        /// Safety level must be FULL
        /// </summary>
        public bool OperationalStatusChangesAllowed
        {
            get
            {
                if (mirroredDatabasesGrid.Selected.Rows.Count > 0)
                {
                    MirrorMonitoringDatabaseDetail detail = getSelectedMirroredDatabaseDetails();
                    if (detail.PrincipalName.ToLower() != "unknown")
                    {
                        //are we monitoring the local instance?
                        int intServer = ServerNameToID(detail.PrincipalName);
                        int intMirror = detail.MirrorName.ToLower().Equals("unknown") ? -1 : ServerNameToID(detail.MirrorName);

                        if (intServer >= 0 && intMirror >= 0)
                        {
                            if ((ApplicationModel.Default.UserToken.GetServerPermission(intServer) >= PermissionType.Modify)
                                && (ApplicationModel.Default.UserToken.GetServerPermission(intMirror) >= PermissionType.Modify))
                            {
                                return true;
                            }
                        }
                    }
                }

                return false;
            }
        }

    #endregion
        
        private void SetViewStatus(ViewStatus status)
        {
            currentViewStatus = status;

            if (ActionsAllowedChanged != null)
            {
                ActionsAllowedChanged(this, EventArgs.Empty);
            }

            switch (status)
            {
                case ViewStatus.NoItems:
                    databasesGridStatusLabel.Text = NO_ITEMS;
                    mirroredDatabasesGrid.Visible = false;
                    ApplicationController.Default.ClearCustomStatus();
                    mirrorMonitoringHistoryPeriodComboBox.Enabled = false;
                    break;
                case ViewStatus.Refreshing:
                case ViewStatus.Suspending:   
                case ViewStatus.FailingOver:
                    mirrorMonitoringHistoryPeriodComboBox.Enabled = false;
                    break;
                default:
                    mirroredDatabasesComboBox.Enabled = true;
                    mirroredDatabasesGrid.Enabled = true;
                    mirroredDatabasesGrid.Visible = true;
                    mirroringDetailsTabControl.Enabled = true;
                    mirrorMonitoringHistoryPeriodComboBox.Enabled = true;
                    break;
            }
        }
        public void DatabasesMirroringView_Load(object sender, System.EventArgs e)
        {
            ApplySettings();
            SetViewStatus(ViewStatus.Refreshing);
        }

        private void historyGrid_MouseDown(object sender, MouseEventArgs e)
        {
            selectedGrid = historyGrid;

            if (e.Button == MouseButtons.Right)
            {
                UIElement selectedElement =
                    ((UltraGrid)sender).DisplayLayout.UIElement.ElementFromPoint(new Point(e.X, e.Y));
                object contextObject = selectedElement.GetContext(typeof(ColumnHeader));

                if (contextObject is ColumnHeader)
                {
                    ColumnHeader columnHeader = contextObject as ColumnHeader;
                    selectedColumn = columnHeader.Column;

                    toolbarsManager.Tools["groupByThisColumnButton"].SharedProps.Visible = true;
                    toolbarsManager.Tools["toggleGroupByBoxButton"].SharedProps.Visible = true;
                    toolbarsManager.Tools["removeThisColumnButton"].SharedProps.Visible = true;
                    toolbarsManager.Tools["showColumnChooserButton"].SharedProps.Visible = true;
                    ((StateButtonTool)toolbarsManager.Tools["groupByThisColumnButton"]).Checked =
                        selectedColumn.IsGroupByColumn;
                    toolbarsManager.SetContextMenuUltra(((UltraGrid)sender), "columnContextMenu");
                }
                else
                {
                    contextObject = selectedElement.GetAncestor(typeof(RowUIElement));

                    if (contextObject is RowUIElement)
                    {
                        RowUIElement row = contextObject as RowUIElement;
                        row.Row.Selected = true;

                        toolbarsManager.SetContextMenuUltra(((UltraGrid)sender), "gridContextMenu");
                    }
                    else
                    {
                        toolbarsManager.SetContextMenuUltra(((UltraGrid)sender), "gridContextMenu");
                    }
                }
            }
        }

        private static void historyGrid_InitializeLayout(object sender, InitializeLayoutEventArgs e)
        {
            e.Layout.Bands[0].Columns[0].AutoSizeMode = ColumnAutoSizeMode.VisibleRows;
            e.Layout.Bands[0].Columns["Time Recorded"].Format = "G";
            e.Layout.Bands[0].Columns["Oldest Unsent Transaction"].Format = "G";
            e.Layout.Bands[0].Columns["Unsent Log"].Format = "###,###,##0 KB";
            e.Layout.Bands[0].Columns["Send Rate"].Format = "###,###,##0 KB/sec";
            e.Layout.Bands[0].Columns["New Transaction Rate"].Format = "###,###,##0 KB/sec";
            e.Layout.Bands[0].Columns["Unrestored Log"].Format = "###,###,##0 KB";
            e.Layout.Bands[0].Columns["Restore Rate"].Format = "###,###,##0 KB/sec";
            e.Layout.Bands[0].Columns["Mirror Commit Overhead"].Format = "###,###,##0.## milliseconds";
        }

        /// <summary>
        /// When a new database is selected, initialize the gathering of all relevant metrics
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void mirroredDatabasesComboBox_SelectionChanged(object sender, EventArgs e)
        {
            //if at least one database has been selected
            if (mirroredDatabasesComboBox.SelectedItem != null)
            {

                ApplicationController.Default.ClearCustomStatus();

                //If the combobox is selecting "All Databases"
                if (mirroredDatabasesComboBox.SelectedItem.DisplayText == AllDatabasesItemText)
                {
                    selectedDatabaseFilter = null;
                    mirroredDatabasesDataTable.DefaultView.RowFilter = null;
                    ApplicationController.Default.SetCustomStatus(
                                                String.Format("{0} Mirrored Database{1}",
                                                              mirroredDatabasesDataTable.DefaultView.Count,
                                                              mirroredDatabasesDataTable.DefaultView.Count == 1 ? string.Empty : "s")
                                                );
                }
                else if (mirroredDatabasesComboBox.Items.Count > 1) //if there is at least one database
                {
                    selectedDatabaseFilter = mirroredDatabasesComboBox.SelectedItem.DisplayText;

                    //Filter the datatable on the name of the selected database
                    mirroredDatabasesDataTable.DefaultView.RowFilter =
                        string.Format("[Database Name] = '{0}'", selectedDatabaseFilter.Replace("'", "''"));

                    //clear the grid that will show all mirrored databases
                    mirroredDatabasesGrid.Selected.Rows.Clear();

                    UltraGridRow[] nonGroupByRows = mirroredDatabasesGrid.Rows.GetAllNonGroupByRows();
                    if (nonGroupByRows.Length > 0)
                    {
                        mirroredDatabasesGrid.Selected.Rows.Add(nonGroupByRows[0]);
                    }
                    ApplicationController.Default.SetCustomStatus("Filter Applied", "Databases: 1 Item");
                }

                mirroredDatabasesDataTable.DefaultView.RowStateFilter = DataViewRowState.CurrentRows;
                mirroredDatabasesGrid.Focus();
            }
        }

        private void refreshDatabasesButton_Click(object sender, EventArgs e)
        {
            blnMirroredDatabasesComboInitialized = false;
            ApplicationController.Default.ActiveView.CancelRefresh();
            ApplicationController.Default.ActiveView.RefreshView();
        }
        void mouseEnter_refreshDatabasesButton(Object Sender, EventArgs e)
        {
            if (Settings.Default.ColorScheme == "Dark")
                this.appearance1.Image = global::Idera.SQLdm.DesktopClient.Properties.Resources.ToolbarRefreshHover;
        }
        void mouseLeave_refreshDatabasesButton(Object Sender, EventArgs e)
        {
            if (Settings.Default.ColorScheme == "Dark")
                appearance1.Image = global::Idera.SQLdm.DesktopClient.Properties.Resources.ToolbarRefresh;
        }

        private void mirroredDatabasesGrid_InitializeLayout(object sender, InitializeLayoutEventArgs e)
        {
            //save all labels from design time
            lblUnsentLog.Tag = lblUnsentLog.Text;
            lblUnrestoredLog.Tag = lblUnrestoredLog.Text;
            lblMirrorAddress.Tag = lblMirrorAddress.Text;
            lblMirrorCommitOverhead.Tag = lblMirrorCommitOverhead.Text;
            lblOperatingMode.Tag = lblOperatingMode.Text;
            lblPrincipalAddress.Tag = lblPrincipalAddress.Text;
            lblRateofNewTrans.Tag = lblRateofNewTrans.Text;
            lblRestoreRate.Tag = lblRestoreRate.Text;
            lblSendRate.Tag = lblSendRate.Text;
            lblTimeToRestore.Tag = lblTimeToRestore.Text;
            lblTimeToSend.Tag = lblTimeToSend.Text;
            lblOldestUnsent.Tag = lblOldestUnsent.Text;
            lblTimeToSendAndRestore.Tag = lblTimeToSendAndRestore.Text;
            lblWitnessAddress.Tag = lblWitnessAddress.Text;
            mirrorGroup.Tag = mirrorGroup.Text;
            principalGroup.Tag = principalGroup.Text;
        }

        private void mirroredDatabasesGrid_AfterSelectChange(object sender, AfterSelectChangeEventArgs e)
        {
            //if there are no selected rows then return
            if (mirroredDatabasesGrid.Rows.Count <= 0 || mirroredDatabasesGrid.Selected.Rows.Count <= 0) return;
            if (ActionsAllowedChanged != null)
            {
                ActionsAllowedChanged(this, EventArgs.Empty);
            }

            if (mirroringDetailsTabControl.SelectedTab.Text.Equals("History"))
            {

                StartMirroringHistoryRefresh();
                return;
            }

            StartMirroringDetailsRefresh();
        }

        private void mirroredDatabasesGrid_MouseDown(object sender, MouseEventArgs e)
        {
            selectedGrid = mirroredDatabasesGrid;

            if (e.Button == MouseButtons.Right)
            {
                UIElement selectedElement =
                    ((UltraGrid)sender).DisplayLayout.UIElement.ElementFromPoint(new Point(e.X, e.Y));
                object contextObject = selectedElement.GetContext(typeof(ColumnHeader));

                if (contextObject is ColumnHeader)
                {
                    ColumnHeader columnHeader = contextObject as ColumnHeader;
                    selectedColumn = columnHeader.Column;

                    toolbarsManager.Tools["groupByThisColumnButton"].SharedProps.Visible = true;
                    toolbarsManager.Tools["toggleGroupByBoxButton"].SharedProps.Visible = true;
                    toolbarsManager.Tools["removeThisColumnButton"].SharedProps.Visible = true;
                    toolbarsManager.Tools["showColumnChooserButton"].SharedProps.Visible = true;
                    ((StateButtonTool)toolbarsManager.Tools["groupByThisColumnButton"]).Checked =
                        selectedColumn.IsGroupByColumn;
                    toolbarsManager.SetContextMenuUltra(((UltraGrid)sender), "columnContextMenu");
                }
                else
                {
                    contextObject = selectedElement.GetAncestor(typeof(RowUIElement));

                    if (contextObject is RowUIElement)
                    {
                        RowUIElement row = contextObject as RowUIElement;
                        row.Row.Selected = true;

                        ((ButtonTool)toolbarsManager.Tools["mirroringFailOver"]).SharedProps.Enabled = ActionsAllowed;
                        ((ButtonTool)toolbarsManager.Tools["mirroringSuspendResume"]).SharedProps.Enabled = ActionsAllowed;
                        ((ButtonTool)toolbarsManager.Tools["markSessionNormal"]).SharedProps.Enabled = OperationalStatusChangesAllowed;
                        ((ButtonTool)toolbarsManager.Tools["markSessionFailedOver"]).SharedProps.Enabled = OperationalStatusChangesAllowed;
                        ((ButtonTool)toolbarsManager.Tools["markSessionRoleAgnostic"]).SharedProps.Enabled = OperationalStatusChangesAllowed;

                        toolbarsManager.SetContextMenuUltra(((UltraGrid)sender), "detailsGridContextMenu");
                    }
                    else
                    {
                        toolbarsManager.SetContextMenuUltra(((UltraGrid)sender), "gridContextMenu");
                    }
                }
            }
        }

        /// <summary>
        /// Details & history tab changed
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void tabControl_SelectedTabChanged(object sender, SelectedTabChangedEventArgs e)
        {
            if (e.Tab.Text.Equals("Status"))
            {
                StartMirroringDetailsRefresh();
            }
            else
            {
                StartMirroringHistoryRefresh();
            }
        }

        /// <summary>
        /// Select the time period over which you wish to view historical data
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void mirrorMonitoringHistoryPeriodComboBox_ValueChanged(object sender, EventArgs e)
        {
            if (blnPopulatingHistory) return;
            if (!mirroringDetailsTabControl.SelectedTab.Text.Equals("History")) return;
            StartMirroringHistoryRefresh();
            return;
        }

        private void historyLocalViewRadioButton_CheckedChanged(object sender, EventArgs e)
        {
            if (!historyLocalViewRadioButton.Checked) return;

            if (!mirroringDetailsTabControl.SelectedTab.Text.Equals("History")) return;
            StartMirroringHistoryRefresh();
            return;
        }

        private void historyPartnerViewRadioButton_CheckedChanged(object sender, EventArgs e)
        {
            if (!historyPartnerViewRadioButton.Checked) return;

            if (!mirroringDetailsTabControl.SelectedTab.Text.Equals("History")) return;
            StartMirroringHistoryRefresh();
            return;
        }

        private void InitializeMirroredDatabasesTable()
        {
            mirroredDatabasesDataTable = new DataTable();
            mirroredDatabasesDataTable.Columns.Add("Database Name", typeof(string));
            mirroredDatabasesDataTable.Columns.Add("Server Instance", typeof(string));
            mirroredDatabasesDataTable.Columns.Add("Current Role", typeof(string));
            mirroredDatabasesDataTable.Columns.Add("Partner Instance", typeof(string));
            mirroredDatabasesDataTable.Columns.Add("Mirroring State", typeof(int));
            mirroredDatabasesDataTable.Columns.Add("Witness Connection", typeof(int));
            mirroredDatabasesDataTable.Columns.Add("Operational State", typeof(int));
            mirroredDatabasesDataTable.Columns.Add("Operating Mode", typeof(string));
            
            mirroredDatabasesDataTable.PrimaryKey = new DataColumn[] { mirroredDatabasesDataTable.Columns["Database Name"] };
            mirroredDatabasesDataTable.CaseSensitive = true;
            mirroredDatabasesDataTable.DefaultView.Sort = "Database Name";

            mirroredDatabasesGrid.DataSource = mirroredDatabasesDataTable;
            mirroredDatabasesGrid.DrawFilter = new HideFocusRectangleDrawFilter();
            mirroredDatabasesGrid.InitializeLayout+=new InitializeLayoutEventHandler(mirroredDatabasesGrid_InitializeLayout);
        }

        private void InitializeHistoryDataTable()
        {
            historyDataTable = new DataTable();
            historyGrid.InitializeLayout += historyGrid_InitializeLayout;
            historyGrid.MouseDown += new MouseEventHandler(historyGrid_MouseDown);
            historyDataTable.Columns.Add("Time Recorded", typeof(DateTime));
            historyDataTable.Columns.Add("Role", typeof(string));
            historyDataTable.Columns.Add("Mirroring State", typeof(int));
            historyDataTable.Columns.Add("Witness Connection", typeof(int));
            historyDataTable.Columns.Add("Unsent Log", typeof(int));
            historyDataTable.Columns.Add("Time to send", typeof(string));
            historyDataTable.Columns.Add("Send Rate", typeof(int));
            historyDataTable.Columns.Add("New Transaction Rate", typeof(int));
            historyDataTable.Columns.Add("Oldest Unsent Transaction", typeof(String));
            historyDataTable.Columns.Add("Unrestored Log", typeof(int));
            historyDataTable.Columns.Add("Time to restore", typeof(string));
            historyDataTable.Columns.Add("Restore Rate", typeof(int));
            historyDataTable.Columns.Add("Mirror Commit Overhead", typeof(int));
            historyDataTable.Columns.Add("intTimeSpan", typeof(int));

            //historyDataTable.PrimaryKey = new DataColumn[] { historyDataTable.Columns["Time Recorded"] };
            
            historyDataTable.CaseSensitive = true;

            historyDataTable.DefaultView.Sort = "Time Recorded";

            historyGrid.DataSource = historyDataTable;
            historyGrid.DrawFilter = new HideFocusRectangleDrawFilter();
        }

        /// <summary>
        /// Get the ID of the server if it is monitored
        /// </summary>
        /// <param name="ServerName"></param>
        /// <returns>integer Server ID</returns>
        private static int ServerNameToID(string ServerName)
        {
            try
            {
                //MonitoredSqlServerCollection tmpServers = ApplicationModel.Default.ActiveInstances;
                foreach(MonitoredSqlServer tmpServer in ApplicationModel.Default.ActiveInstances)
                {
                    if(tmpServer.InstanceName.ToLower().Equals(ServerName.ToLower())) return tmpServer.Id;
                }
                return RepositoryHelper.GetAlternateServerID(Settings.Default.ActiveRepositoryConnection.ConnectionInfo,
                                                     ServerName);
            }
            catch
            {
                return -1;
            }
        }

        /// <summary>
        /// Extract the metrics from the mirror metrics object and write them to the labels
        /// </summary>
        /// <param name="metrics"></param>
        private void ExtractMetrics(MirroringMetrics metrics, bool IsLocal)
        {
            //The local server could be a principal or a mirror
            //if the local is principal then local time goes to the frame label
            //else local time gies to the mirror label
            if (metrics.Role == MirroringMetrics.MirroringRoleEnum.Principal)
            {
                //Local time is the time on the local server instance when the row (metrics) data was gathered
                //if these are the metrics of the principal then write the local time to the frame label
                principalGroup.Text = IsLocal?((string)principalGroup.Tag) + " (" + metrics.LocalTime + ")":(string)principalGroup.Tag;
                lblUnsentLog.Text = ((string)lblUnsentLog.Tag) + " " + metrics.UnsentLog.ToString("###,###,##0") + " KB";
                lblSendRate.Text = ((string)lblSendRate.Tag) + " " + metrics.SendRate.ToString("###,###,##0") + " KB/sec";
                lblTimeToSend.Text = ((string)lblTimeToSend.Tag) + " " + metrics.TimeToSend;
                lblRateofNewTrans.Text = ((string)lblRateofNewTrans.Tag) + " " + metrics.TransactionsPerSec.ToString("###,###,##0") + " KB/sec";
                lblOldestUnsent.Text = ((string)lblOldestUnsent.Tag) + " " +
                    FormatTimeSpan(metrics.OldestUnsentTransaction, false);
                    //metrics.LocalTime.Subtract(metrics.TimeBehind.AddHours(TimeZone.CurrentTimeZone.GetUtcOffset(metrics.TimeBehind).Hours)),false);
            }
            else
            {
                //Local time is the time on the local server instance when the row (metrics) data was gathered
                //if these are the metrics of the mirror then write the local time to the frame label
                mirrorGroup.Text = IsLocal?(string)mirrorGroup.Tag + " (" + metrics.LocalTime + ")":(string)mirrorGroup.Tag;
                lblUnrestoredLog.Text = ((string)lblUnrestoredLog.Tag) + " " + metrics.UnrestoredLog.ToString("###,###,##0") + " KB";
                lblRestoreRate.Text = ((string)lblRestoreRate.Tag) + " " + metrics.RecoveryRate.ToString("###,###,##0") + " KB/sec";
                lblTimeToRestore.Text = ((string)lblTimeToRestore.Tag) + " " + metrics.TimeToRestore;
            }
            
            //general
            lblMirrorCommitOverhead.Text = ((string)lblMirrorCommitOverhead.Tag) + " " + metrics.AverageDelay + " milliseconds";
            lblTimeToSendAndRestore.Text = ((string)lblTimeToSendAndRestore.Tag) + " " + metrics.TimeToSendAndRestore;

        }

       
        /// <summary>
        /// Populate the mirrored databases combo box, defaults selectedIndex to that of selectedDatabaseFilter
        /// </summary>
        /// <param name="databases"></param>
        public void PopulateMirroredDatabasesCombo(IDictionary<string, MirrorMonitoringDatabaseDetail> databases)
        {
            //clear the available databases combo box
            mirroredDatabasesComboBox.Items.Clear();
            
            //if there are database details in the dictionary
            if (databases != null && databases.Count > 0)
            {
                //enable the combo box
                mirroredDatabasesComboBox.Enabled = true;
                mirroredDatabasesComboBox.Items.Add(null, AllDatabasesItemText);

                //add names of all mirrored databases to the combo box
                foreach (string database in databases.Keys)
                {
                    mirroredDatabasesComboBox.Items.Add(database, database);
                }

                blnMirroredDatabasesComboInitialized = true;
            }
            else//if there are no mirrored databases
            {
                mirroredDatabasesComboBox.Enabled = false;
                mirroredDatabasesComboBox.Items.Add(null, string.Format("< {0} >", NO_ITEMS));
            }

            mirroredDatabasesComboBox.SelectedIndex = FindDatabaseIndex(selectedDatabaseFilter);
        }

        /// <summary>
        /// Insert mirroring history into the table
        /// </summary>
        /// <param name="data"></param>
        public void UpdateHistoryData(object data)
        {
            if (data != null && data is MirrorMonitoringHistorySnapshot)
            {
                lock (updatingMirroredDatabasesLock)
                {
                    historyGrid.SuspendLayout();
                    try
                    {
                        MirrorMonitoringHistorySnapshot snapshot = data as MirrorMonitoringHistorySnapshot;

                        if (snapshot.Error == null)
                        {
                            historyDataTable.Clear();

                            if (snapshot.Metrics != null && snapshot.Metrics.Count > 0)
                            {
                                                                
                                historyDataTable.BeginLoadData();

                                try
                                {
                                    //now update any matching databases or add new ones
                                    foreach (MirroringMetrics metric in snapshot.Metrics)
                                    {
                                        historyDataTable.LoadDataRow(
                                                new object[]
                                    {
                                        metric.TimeRecorded.AddHours(TimeZone.CurrentTimeZone.GetUtcOffset(metric.TimeRecorded).Hours),
                                        metric.Role,
                                        metric.MirroringState,
                                        metric.WitnessStatus,
                                        metric.UnsentLog,
                                        metric.TimeToSend.HasValue?metric.TimeToSend.ToString():"",
                                        metric.SendRate,
                                        metric.LogGenerationRate,
                                        //metric.TimeBehind.AddHours(TimeZone.CurrentTimeZone.GetUtcOffset(metric.TimeBehind).Hours),
                                        //FormatTimeSpan(metric.LocalTime.Subtract(metric.TimeBehind.AddHours(TimeZone.CurrentTimeZone.GetUtcOffset(metric.TimeBehind).Hours)),false),
                                        FormatTimeSpan(metric.OldestUnsentTransaction, false),
                                        metric.UnrestoredLog,
                                        metric.TimeToRestore.HasValue?metric.TimeToRestore.ToString():"",
                                        metric.RecoveryRate,
                                        metric.AverageDelay,
                                        metric.OldestUnsentTransaction.TotalSeconds
                                    }
                                    , true);
                                    }
                                }
                                catch (Exception e)
                                {
                                    Log.Error(e.Message.ToString());
                                }
                                historyDataTable.EndLoadData();
                                historyGrid.Visible = true;

                                historyGrid.DisplayLayout.Bands[0].Columns["Oldest Unsent Transaction"].SortComparer = new srtComparer();

                                if (!initializedMirroringHistoryGrid)
                                {
                                    if (lasthistoryGridSettings != null)
                                    {
                                        GridSettings.ApplySettingsToGrid(lasthistoryGridSettings, historyGrid);

                                        initializedMirroringHistoryGrid = true;
                                    }
                                    else if (snapshot.Metrics.Count > 0)
                                    {
                                        foreach (UltraGridColumn column in historyGrid.DisplayLayout.Bands[0].Columns)
                                        {
                                            column.PerformAutoResize(PerformAutoSizeType.AllRowsInBand, true);
                                            column.Width = Math.Min(column.Width, historyGrid.Width / 2);
                                        }
                                        historyGrid.DisplayLayout.Bands[0].Columns["Time to send"].Hidden = true;
                                        historyGrid.DisplayLayout.Bands[0].Columns["Time to restore"].Hidden = true;
                                        initializedMirroringHistoryGrid = true;
                                    }
                                }
                            }
                            else
                            {
                                //ShowMirroringDetailsGridStatusMessage("No mirroring history data has been returned. Please check that the mirroring partners can communicate.", true);
                                historyGrid.Visible = true;
                                ApplicationController.Default.ClearCustomStatus();
                            }

                            ApplicationController.Default.OnRefreshActiveViewCompleted(
                                new RefreshActiveViewCompletedEventArgs(DateTime.Now));
                        }
                        else
                        {
                            mirroredDatabasesComboBox.Enabled = false;
                            mirroredDatabasesComboBox.Items.Clear();
                            mirroredDatabasesComboBox.Items.Add(null, string.Format("< {0} >", UNABLE_TO_UPDATE));

                            ApplicationController.Default.ClearCustomStatus();
                            ApplicationController.Default.OnRefreshActiveViewCompleted(
                                new RefreshActiveViewCompletedEventArgs(DateTime.Now, snapshot.Error));
                        }

                        //I am commenting this out pending some performance testing
                        //It looks horribly slow on my laptop
                        //foreach (UltraGridRow gridRow in historyGrid.Rows.GetAllNonGroupByRows())
                        //{
                        //    DataRowView dataRowView = (DataRowView)gridRow.ListObject;
                        //    DataRow dataRow = dataRowView.Row;

                        //    AlertConfiguration alertConfig = ApplicationModel.Default.GetAlertConfiguration(instanceId);
                        //    if (alertConfig != null)
                        //    {
                        //        UpdateCellColor(Metric.MirrorCommitOverhead, alertConfig, gridRow, "Mirror Commit Overhead", 1);
                        //        UpdateCellColor(Metric.OldestUnsentMirroringTran, alertConfig, gridRow, "Oldest Unsent Transaction", 1);
                        //        UpdateCellColor(Metric.UnsentLogThreshold, alertConfig, gridRow, "Unsent Log", 1);
                        //        UpdateCellColor(Metric.UnrestoredLog, alertConfig, gridRow, "Unrestored Log", 1);
                        //        UpdateCellColor(Metric.MirroringSessionsStatus, alertConfig, gridRow, "Mirroring State", 1);
                        //    }
                        //}
                    }
                    finally
                    {
                        historyGrid.ResumeLayout();
                    }
                    
                }
            }

        }
        
        /// <summary>
        /// Overrides UpdateData in the view object. Is the callback for all scheduled refreshes
        /// Returns all realtime data. This is used for the population of the combo box as well as the grid.
        /// Combo and mirrored databases datatables contain exactly the same mirrored databases.
        /// The data table does filter on whatever is selected in the combo box
        /// </summary>
        /// <param name="returnedData"></param>
        public override void UpdateData(object returnedData)
        {
            bool blnPopulateDetails = false;
            SetViewStatus(ViewStatus.Refreshing);
            try
            {
                //if valid data has been returned
                if (returnedData != null && returnedData is Pair<Snapshot, Dictionary<Guid, MirroringSession>>)
                {
                    Pair<Snapshot, Dictionary<Guid, MirroringSession>> data = (Pair<Snapshot, Dictionary<Guid, MirroringSession>>)returnedData;
                    lock (updatingMirroredDatabasesLock)
                    {
                        MirrorMonitoringRealtimeSnapshot snapshot = data.First as MirrorMonitoringRealtimeSnapshot;

                        //if there was no error during collection
                        if (snapshot.Error == null)
                        {
                            PopulateMirroredDatabasesCombo(snapshot.Databases);

                            if (!blnMirroredDatabasesComboInitialized)
                            {
                                selectedGrid = mirroredDatabasesGrid;
                            }
                            SetViewStatus(ViewStatus.Normal);

                            //if the snapshot does contain valid mirrored databases
                            if (snapshot.Databases != null && snapshot.Databases.Count > 0)
                            {
                                //load the data table
                                mirroredDatabasesDataTable.BeginLoadData();

                                // remove any databases that have been deleted
                                List<DataRow> deleteRows = new List<DataRow>();
                                //go through each row of current data
                                foreach (DataRow row in mirroredDatabasesDataTable.Rows)
                                {
                                    //if the latest snapshot does not contain a database that is
                                    //in the datatable representing the data that
                                    //is in the grid then add the name of that database to a 
                                    //tobedeleted list
                                    if (!snapshot.Databases.ContainsKey((string)row["Database Name"]))
                                    {
                                        deleteRows.Add(row);
                                    }
                                }

                                //delete each outdated entry
                                foreach (DataRow row in deleteRows)
                                {
                                    mirroredDatabasesDataTable.Rows.Remove(row);
                                }

                                mirroredDatabases.Clear();

                                Dictionary<Guid, MirroringSession> preferredConfig = data.Second;// managementService.GetMirroringPreferredConfig();

                                //now update any matching databases or add new ones
                                foreach (MirrorMonitoringDatabaseDetail db in snapshot.Databases.Values)
                                {

                                    MirroringSession.MirroringPreferredConfig operationalStatus = MirroringSession.MirroringPreferredConfig.Delete;

                                    if (preferredConfig != null)
                                    {
                                        if (preferredConfig.ContainsKey(db.MirroringGuid))
                                        {
                                            operationalStatus = preferredConfig[db.MirroringGuid].getCurrentOperationalState
                                                (ServerNameToID(db.PrincipalName), ServerNameToID(db.MirrorName));
                                        }
                                    }

                                    mirroredDatabasesDataTable.LoadDataRow(
                                            new object[]
                                    {
                                        db.DatabaseName,
                                        db.ServerInstance,
                                        db.CurrentMirroringMetrics.Role,
                                        db.Partner,
                                        db.CurrentMirroringMetrics.MirroringState,
                                        db.CurrentMirroringMetrics.WitnessStatus, operationalStatus, db.OperatingMode
                                    }, true);
                                    mirroredDatabases.Add(db.DatabaseName, db);
                                }
                                mirroredDatabasesDataTable.EndLoadData();

                                //if the mirrored database grid has not yet been initialized
                                if (!initializedMirroredDatabasesGrid)
                                {
                                    //if previous saved settings have been found
                                    if (lastmirroringGridSettings != null)
                                    {
                                        GridSettings.ApplySettingsToGrid(lastmirroringGridSettings, mirroredDatabasesGrid);

                                        initializedMirroredDatabasesGrid = true;
                                    }
                                    else if (snapshot.Databases.Count > 0) //no previous settings were found but databases have been returned
                                    {
                                        //for each column
                                        foreach (UltraGridColumn column in mirroredDatabasesGrid.DisplayLayout.Bands[0].Columns)
                                        {
                                            //do an auto resize
                                            column.PerformAutoResize(PerformAutoSizeType.AllRowsInBand, true);
                                            //allow the width of a column to be at most, half the width of the grid
                                            column.Width = Math.Min(column.Width, mirroredDatabasesGrid.Width / 2);
                                        }

                                        initializedMirroredDatabasesGrid = true;
                                    }
                                    //if there are rows in the grid but none have been selected
                                    if (mirroredDatabasesGrid.Rows.Count > 0 && mirroredDatabasesGrid.Selected.Rows.Count == 0)
                                    {
                                        mirroredDatabasesGrid.Rows[0].Selected = true;
                                    }
                                }
                                //currentSnapshot = snapshot;

                                SetViewStatus(ViewStatus.Normal);
                            }
                            else //databases is null or there are no databases
                            {
                                SetViewStatus(ViewStatus.NoItems);
                                //Clear the combo
                                mirroredDatabasesComboBox.Enabled = false;
                                mirroredDatabasesComboBox.Items.Clear();
                                mirroredDatabasesComboBox.Items.Add(null, string.Format("< {0} >", NO_ITEMS));
                                //Clear the grid
                                mirroredDatabasesDataTable.BeginLoadData();
                                mirroredDatabasesDataTable.Rows.Clear();
                                mirroredDatabases.Clear();
                                mirroredDatabasesDataTable.EndLoadData();

                                //Clear the details from the tab
                                //local
                                ExtractMetrics(new MirroringMetrics(), true);

                                //remote
                                ExtractMetrics(new MirroringMetrics(), false);

                                //Clear History grid
                                historyDataTable.BeginLoadData();
                                historyDataTable.Rows.Clear();
                                historyDataTable.EndLoadData();
                            }

                            ApplicationController.Default.SetCustomStatus(
                                String.Format("{0} Mirrored Database{1}",
                                              mirroredDatabases.Count,
                                              mirroredDatabases.Count == 1 ? string.Empty : "s")
                                );

                            ApplicationController.Default.OnRefreshActiveViewCompleted(
                                new RefreshActiveViewCompletedEventArgs(DateTime.Now));
                        }
                        else   //if an error occurred in the refresh
                        {
                            //Clear the combo
                            mirroredDatabasesComboBox.Enabled = false;
                            mirroredDatabasesComboBox.Items.Clear();
                            mirroredDatabasesComboBox.Items.Add(null, string.Format("< {0} >", UNABLE_TO_UPDATE));
                            //Clear the grid
                            mirroredDatabasesDataTable.BeginLoadData();
                            mirroredDatabasesDataTable.Rows.Clear();
                            mirroredDatabases.Clear();
                            mirroredDatabasesDataTable.EndLoadData();
                            
                            //Clear the details from the tab
                            //local
                            ExtractMetrics(new MirroringMetrics(), true);
                            
                            //remote
                            ExtractMetrics(new MirroringMetrics(), false);

                            //Clear History grid
                            historyDataTable.BeginLoadData();
                            historyDataTable.Rows.Clear();
                            historyDataTable.EndLoadData();

                            ApplicationController.Default.ClearCustomStatus();
                            ApplicationController.Default.OnRefreshActiveViewCompleted(
                                new RefreshActiveViewCompletedEventArgs(DateTime.Now, snapshot.Error));
                            return;
                        }


                        //Update cell colors depending on the alerts that have been configured
                        foreach (UltraGridRow gridRow in mirroredDatabasesGrid.Rows.GetAllNonGroupByRows())
                        {
                            DataRowView dataRowView = (DataRowView)gridRow.ListObject;
                            DataRow dataRow = dataRowView.Row;
                            if (gridRow.Selected && !blnPopulateDetails)
                            {
                                blnPopulateDetails = true;
                            }

                            string strDB = dataRow["Database Name"].ToString();
                            MirrorMonitoringDatabaseDetail DBDetail = mirroredDatabases[strDB];
                            gridRow.ToolTipText = DBDetail.PrincipalName + "-->" + DBDetail.MirrorName;

                            AlertConfiguration alertConfig = ApplicationModel.Default.GetAlertConfiguration(instanceId);
                            if (alertConfig != null)
                            {
                                UpdateCellColor(Metric.MirroringSessionsStatus, alertConfig, gridRow, "Mirroring State", 1);
                                UpdateCellColor(Metric.MirroringSessionNonPreferredConfig, alertConfig, gridRow, "Operational State", 1);
                                UpdateCellColor(Metric.MirroringSessionNonPreferredConfig, alertConfig, gridRow, "Current Role", 1);
                                UpdateCellColor(Metric.MirroringWitnessConnection, alertConfig, gridRow, "Witness Connection", 1);
                            }
                        }

                        if (blnPopulateDetails)
                        {
                            //if there are no selected rows then return
                            if (mirroredDatabasesGrid.Rows.Count <= 0 || mirroredDatabasesGrid.Selected.Rows.Count <= 0) return;


                            if (mirroringDetailsTabControl.SelectedTab.Text.Equals("History"))
                            {
                                if (!blnPopulatingHistory) StartMirroringHistoryRefresh();
                                return;
                            }

                            StartMirroringDetailsRefresh();
                        }

                        if (ActionsAllowedChanged != null)
                        {
                            ActionsAllowedChanged(this, EventArgs.Empty);
                        }
                    }
                }
            }
            finally
            {
                SetViewStatus(ViewStatus.Normal);
            }
        }

        /// <summary>
        /// Search the available databases combo box for a match and return the index of the match if one is found
        /// </summary>
        /// <param name="database"></param>
        /// <returns></returns>
        private int FindDatabaseIndex(string database)
        {
            int index = -1;

            for (int i = 0; i < mirroredDatabasesComboBox.Items.Count; i++)
            {
                if (string.CompareOrdinal(mirroredDatabasesComboBox.Items[i].DataValue as string, database) == 0)
                {
                    index = i;
                    break;
                }
            }

            return index;
        }

        /// <summary>
        /// Formatting of timespans is limited so this function formats the timespan in the way we want
        /// </summary>
        /// <param name="span"></param>
        /// <param name="showSign"></param>
        /// <returns></returns>
        private static string FormatTimeSpan(TimeSpan span, bool showSign)
        {
            string sign = String.Empty;
            if (showSign && (span > TimeSpan.Zero)) sign = "+";
            //if the span is negative by a second (we will show the second so we must add the sign)
            if (span < TimeSpan.Zero & Math.Abs(span.TotalSeconds) >= 1) sign = "-";
            return sign + Math.Abs(span.Days).ToString("00") + "." +
                Math.Abs(span.Hours).ToString("00") + ":" +
                Math.Abs(span.Minutes).ToString("00");
            // +":" +
            //span.Seconds.ToString("00");
            //+ "." + span.Milliseconds.ToString("00");
        }

        private void UpdateCellColor(Metric metric, AlertConfiguration alertConfig, UltraGridRow gridRow, string columnName, int adjustmentMultiplier)
        {
            AlertConfigurationItem alertConfigItem = alertConfig[metric, String.Empty]; // Will need to update this if ever called with a metric that supports multi-thresholds

            if (alertConfigItem != null)
            {

                    UltraGridCell cell = gridRow.Cells[columnName];
                    if (cell != null)
                    {
                        DataRowView dataRowView = (DataRowView) gridRow.ListObject;
                        DataRow dataRow = dataRowView.Row;
                        if (dataRow.IsNull(columnName))
                        {
                            cell.Appearance.ResetBackColor();
                        }
                        else
                        {
                            //if this is the current role column we actually want to highlight based on deviation from preferred role
                            if (columnName.ToLower().Equals("current role")) { columnName = "Operational State"; }

                            IComparable value = (IComparable) dataRow[columnName];
                            
                            if (metric == Metric.MirroringSessionNonPreferredConfig)
                            {
                                //if the preferred config value is normal or dont care then value must be 0 (good) else set value to 1 (bad)
                                value = ((value.Equals("Not set") || value.Equals(1) || value.Equals(-1)))?0:1;
                            }
                            if (metric == Metric.MirroringWitnessConnection) value = value.Equals(2) ? 1 : 0;
                            //else
                            //    if (metric == Metric.OldestOpenTransMinutes)
                            //    {
                            //        if (value == null)
                            //            value = 0;
                            //        else
                            //        {
                            //            TimeSpan diff = DateTime.Now - (DateTime)value;
                            //            value = (diff.TotalMinutes < 0) ? 0 : diff.TotalMinutes;
                            //        }
                            //    }
                            //if (value != null && adjustmentMultiplier != 1)
                            //{
                            //    double dbl = (double)Convert.ChangeType(value, typeof(double));
                            //    value = dbl * adjustmentMultiplier;
                            //}

                            switch (alertConfigItem.GetSeverity(value))
                            {
                                case MonitoredState.Informational:
                                    cell.Appearance.BackColor = Color.Blue;
                                    cell.Appearance.ForeColor = Color.White;
                                    break;
                                case MonitoredState.Warning:
                                    cell.Appearance.BackColor = Color.Gold;
                                    cell.Appearance.ResetForeColor();
                                    break;
                                case MonitoredState.Critical:
                                    cell.Appearance.BackColor = Color.Red;
                                    cell.Appearance.ForeColor = Color.White;
                                    break;
                                default:
                                    cell.Appearance.ResetBackColor();
                                    cell.Appearance.ResetForeColor();
                                    break;
                            }
                        }
                }
            }
        }

        public override void SetArgument(object argument)
        {
            selectedDatabaseArgument = null;
            selectedDatabaseFilter = null;

            if (argument is string)
            {
                selectedDatabaseFilter = argument as string;
            }
            else if (argument is MonitoredObjectName && ((MonitoredObjectName)argument).IsDatabase)
            {
                selectedDatabaseFilter = ((MonitoredObjectName)argument).DatabaseName;
            }
            else if (argument is IList<string>)
            {
                IList<string> selection = argument as IList<string>;

                if (selection.Count == 1)
                {
                    selectedDatabaseFilter = selection[0];
                }
                else
                {
                    selectedDatabaseArgument = new List<string>(selection);
                }
            }

            if (selectedDatabaseFilter == null && mirroredDatabasesComboBox.Items.Count > 0)
            {
                mirroredDatabasesComboBox.SelectedIndex = 0;
            }
            else if (selectedDatabaseFilter != null && mirroredDatabasesComboBox.Items.Count > 0)
            {
                mirroredDatabasesComboBox.SelectedIndex = FindDatabaseIndex(selectedDatabaseFilter);
            }
            else if (selectedDatabaseArgument != null)
            {
                selectedDatabases = new SortedList<string, string>();
                mirroredDatabasesGrid.Selected.Rows.Clear();
                foreach (string database in selectedDatabaseArgument)
                {
                    foreach (UltraGridRow row in mirroredDatabasesGrid.Rows.GetAllNonGroupByRows())
                    {
                        if ((string)row.Cells["Database Name"].Value == database)
                        {
                            DataRowView dataRow = (DataRowView)row.ListObject;
                            if (dataRow["Version Compatibility"] != System.DBNull.Value
                                && (float)dataRow["Version Compatibility"] < 6.5f)
                            {
                                ApplicationMessageBox.ShowWarning(this, String.Format(DB_NOT_SUPPORTED, database));
                            }
                            else
                            {
                                selectedDatabases.Add(database, database);
                                row.Selected = true;
                                mirroredDatabasesGrid.ActiveRowScrollRegion.ScrollRowIntoView(row);
                            }
                            break;
                        }
                    }
                }
                if (selectedDatabases.Count > 0)
                {
                    selectedDatabaseArgument = null;
                }
            }
        }

        /// <summary>
        /// Kicks off the collection of mirroring History
        /// </summary>
        private void StartMirroringHistoryRefresh()
        {
            //Do not try and get the details if ither the local or remot partners are still busy
            if (refreshMirroringHistoryBackgroundWorker != null && refreshMirroringHistoryBackgroundWorker != null)
            {
                if (refreshMirroringHistoryBackgroundWorker.IsBusy)
                {
                    SetViewStatus(ViewStatus.Refreshing);
                    return;
                }
            }            
            //if there are no rows in the grid then we cannot show history so return
            //or more than 1 row was selected
            if (mirroredDatabasesGrid.Rows.Count <= 0 || mirroredDatabasesGrid.Selected.Rows.Count <= 0 || mirroredDatabasesGrid.Selected.Rows.Count > 1) return;

            if (mirrorMonitoringHistoryPeriodComboBox.Value == null) mirrorMonitoringHistoryPeriodComboBox.SelectedIndex = 0;

            //since we are here we know that there are rows in the grid
            //There will only be a single selected row
            foreach (UltraGridRow row in mirroredDatabasesGrid.Selected.Rows)
            {
                //if this is not a data row then go no further
                if (!row.IsDataRow) continue;

                //create a new details object of the selected row
                MirrorMonitoringDatabaseDetail details =
                    mirroredDatabases[(string)row.Cells["Database Name"].Value];
                
                selectedDatabase = details.DatabaseName;
                
                //take the name of the principal or the mirror depending on the radio buttons
                selectedServer = historyLocalViewRadioButton.Checked
                                           ? details.ServerInstance
                                           : details.Partner;

                ShowMirroringDetailsGridStatusMessage(Idera.SQLdm.Common.Constants.LOADING,true);
                
                //Log.Info(string.Format("Refreshing table details for {0}...", historyGrid.Selected.Rows[0].Cells["Qualified Table Name"].Value));

                mirroringHistoryRefreshStopwatch.Reset();
                mirroringHistoryRefreshStopwatch.Start();
                InitializeMirroringHistoryBackgroundWorker();
                SetViewStatus(ViewStatus.Refreshing);
                refreshMirroringHistoryBackgroundWorker.RunWorkerAsync();

            }
        }
        /// <summary>
        /// Show a label over the lower panel - details or history
        /// </summary>
        /// <param name="message"></param>
        /// <param name="blnOverGrid"></param>
        private void ShowMirroringDetailsGridStatusMessage(string message, bool blnOverGrid)
        {
            if (blnOverGrid)
            {
                mirroringDetailsTabControlStatusLabel.Dock = DockStyle.Bottom;
                mirroringDetailsTabControlStatusLabel.Height = historyGrid.Height;
            }else
            {
                mirroringDetailsTabControlStatusLabel.Dock = DockStyle.Fill;
            }
            mirroringDetailsTabControlStatusLabel.Text = message;
            mirroringDetailsTabControlStatusLabel.BringToFront();
            mirroringDetailsTabControlStatusLabel.Visible = true;
        }
        /// <summary>
        /// Hide the label field
        /// </summary>
        /// <returns></returns>
        private void HideMirroringDetailsGridStatusMessage()
        {
            mirroringDetailsTabControlStatusLabel.Visible = false;
            mirroringDetailsTabControl.Visible = true;
            mirroringDetailsTabControlStatusLabel.SendToBack();
        }
        private MirrorMonitoringDatabaseDetail getSelectedMirroredDatabaseDetails()
        {
            //if there are no rows in the grid then we cannot show history so return
            //or more than 1 row was selected
            if (mirroredDatabasesGrid.Rows.Count <= 0 || mirroredDatabasesGrid.Selected.Rows.Count <= 0 || mirroredDatabasesGrid.Selected.Rows.Count > 1) return null;

            if (mirrorMonitoringHistoryPeriodComboBox.Value == null)
                mirrorMonitoringHistoryPeriodComboBox.SelectedIndex = 0;

            //since we are here we know that there are rows in the grid
            //There will only be a single selected row
            foreach (UltraGridRow row in mirroredDatabasesGrid.Selected.Rows)
            {
                //if this is not a data row then go no further
                if (!row.IsDataRow) continue;

                //create a new details object of the selected row
                return mirroredDatabases[(string)row.Cells["Database Name"].Value];
            }
            return null;
        }

        #region Mirroring History Background Worker
        /// <summary>
        /// Initialize the mirroring history backgroundworker
        /// </summary>
        private void InitializeMirroringHistoryBackgroundWorker()
        {
            if (refreshMirroringHistoryBackgroundWorker != null)
            {
                if (refreshMirroringHistoryBackgroundWorker.IsBusy) refreshMirroringHistoryBackgroundWorker.CancelAsync();
                refreshMirroringHistoryBackgroundWorker = null;
                InitializeMirroringHistoryBackgroundWorker();

            }
            else
            {
                refreshMirroringHistoryBackgroundWorker = new BackgroundWorker();
                refreshMirroringHistoryBackgroundWorker.WorkerSupportsCancellation = true;
                refreshMirroringHistoryBackgroundWorker.DoWork +=
                    new DoWorkEventHandler(refreshMirroringHistoryBackgroundWorker_DoWork);
                refreshMirroringHistoryBackgroundWorker.RunWorkerCompleted +=
                    new RunWorkerCompletedEventHandler(refreshMirroringHistoryBackgroundWorker_RunWorkerCompleted);
            }
        }
        /// <summary>
        /// Kicks of the asynchronous gathering of mirroring history
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void refreshMirroringHistoryBackgroundWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            if (System.Threading.Thread.CurrentThread.Name == null) System.Threading.Thread.CurrentThread.Name = "RefreshMirroringHistory";

            if (blnPopulatingHistory)
            {
                return;
            }
            
            
            blnPopulatingHistory = true;

            try
            {
                if (refreshMirroringHistoryBackgroundWorker.CancellationPending)
                {
                    e.Cancel = true;
                    return;
                }

                //get the id of the selected server
                int MonitoredServerID = ServerNameToID(selectedServer);

                IManagementService managementService =
                ManagementServiceHelper.GetDefaultService(Settings.Default.ActiveRepositoryConnection.ConnectionInfo);

                //if the monitored server is being monitored
                if (MonitoredServerID > 0)
                {
                    historyConfig = new MirrorMonitoringHistoryConfiguration(MonitoredServerID,
                                                                             selectedDatabase,
                                                                             mirrorMonitoringHistoryPeriodComboBox.SelectedIndex + 1);

                    historySnapshot = managementService.GetMirrorMonitoringHistory(historyConfig);


                    e.Result = historySnapshot;
                }
                else
                {
                    e.Result = new Exception("The server you are trying to access is not monitored by " +
                                        Application.ProductName);
                }

            }
            catch (Exception ex)
            {
                blnPopulatingHistory = false;
                e.Result = ex;
            }
        }

        /// <summary>
        /// This is the final callback once the mirroring history has been gathered
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void refreshMirroringHistoryBackgroundWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            try
            {
                //If this call has been cancelled or the result is null (this would happen if we are waiting
                //for results of the last call)
                if(e.Cancelled)
                {
                    blnPopulatingHistory = false;
                    return;
                }
                if (e.Result == null) return;

                mirroringHistoryRefreshStopwatch.Stop();

                if (!e.Cancelled)
                {
                    if ((e.Error == null) && !(e.Result is Exception))
                    {
                        MirrorMonitoringHistorySnapshot snapshot = e.Result as MirrorMonitoringHistorySnapshot;

                        if (snapshot != null)
                        {
                            if (snapshot.Error == null)
                            {
                                HideMirroringDetailsGridStatusMessage();

                                Log.Info(
                                    string.Format("Refresh mirroring details completed for {0} (Duration = {1}).",
                                                  snapshot.MirroredDatabase,
                                                  mirroringHistoryRefreshStopwatch.Elapsed));

                                UpdateHistoryData(snapshot);

                            }
                            else
                            {
                                ShowMirroringDetailsGridStatusMessage("An error occurred while refreshing table details.",true);

                                Log.Error("An error occurred while refreshing mirroring details.", snapshot.Error);
                            }
                        }
                        else
                        {
                            ShowMirroringDetailsGridStatusMessage("No mirroring details are available.",false);
                        }
                    }
                    else
                    {
                        Exception ex = e.Result as Exception;
                        if (ex == null)
                        {
                            ShowMirroringDetailsGridStatusMessage("An error occurred while refreshing mirroring details.",false);
                            Log.Error("An error occurred while refreshing table details.", e.Error);
                        }
                        else
                        {
                            ShowMirroringDetailsGridStatusMessage(ex.Message.ToString(),true);
                        }
                    }
                }
            }
            finally
            {
                SetViewStatus(ViewStatus.Normal);
                blnPopulatingHistory = false;
            }
        }
        #endregion

        #region Mirroring Details Background Workers
        /// <summary>
        /// Initialize the mirroring details backgroundworker
        /// </summary>
        private void InitializeMirroringDetailsBackgroundWorker()
        {
            if (refreshMirroringDetailsBackgroundWorker != null)
            {
                if (refreshMirroringDetailsBackgroundWorker.IsBusy) refreshMirroringDetailsBackgroundWorker.CancelAsync();

                refreshMirroringDetailsBackgroundWorker = null;
                InitializeMirroringDetailsBackgroundWorker();

            }
            else
            {
                refreshMirroringDetailsBackgroundWorker = new BackgroundWorker();
                refreshMirroringDetailsBackgroundWorker.WorkerSupportsCancellation = true;
                refreshMirroringDetailsBackgroundWorker.DoWork +=
                    new DoWorkEventHandler(refreshMirroringDetailsBackgroundWorker_DoWork);
                refreshMirroringDetailsBackgroundWorker.RunWorkerCompleted +=
                    new RunWorkerCompletedEventHandler(refreshMirroringDetailsBackgroundWorker_RunWorkerCompleted);
            }
        }
        /// <summary>
        /// Initialize the partner details backgroundworker
        /// </summary>
        private void InitializePartnerDetailsBackgroundWorker()
        {
            if (refreshPartnerDetailsBackgroundWorker != null)
            {
                if (refreshPartnerDetailsBackgroundWorker.IsBusy) refreshPartnerDetailsBackgroundWorker.CancelAsync();

                refreshPartnerDetailsBackgroundWorker = null;
                InitializePartnerDetailsBackgroundWorker();

            }
            else
            {
                refreshPartnerDetailsBackgroundWorker = new BackgroundWorker();
                refreshPartnerDetailsBackgroundWorker.WorkerSupportsCancellation = true;
                refreshPartnerDetailsBackgroundWorker.DoWork +=
                    new DoWorkEventHandler(refreshPartnerDetailsBackgroundWorker_DoWork);
                refreshPartnerDetailsBackgroundWorker.RunWorkerCompleted +=
                    new RunWorkerCompletedEventHandler(refreshPartnerDetailsBackgroundWorker_RunWorkerCompleted);
            }
        }

        /// <summary>
        /// Start refreshing the mirroring details
        /// </summary>
        private void StartMirroringDetailsRefresh()
        {
            //if (refreshMirroringDetailsBackgroundWorker.IsBusy) refreshMirroringDetailsBackgroundWorker.CancelAsync();

            // There is only one selected row which we iterate through
            foreach (UltraGridRow row in mirroredDatabasesGrid.Selected.Rows)
            {
                //if the selected row is not a data row then we do not want to process further
                if (!row.IsDataRow) continue;

                //get the details of the current selected row
                MirrorMonitoringDatabaseDetail details =
                    mirroredDatabases[(string)row.Cells["Database Name"].Value];

                //save the name of the database whose details we want
                selectedDatabase = details.DatabaseName;
                selectedServer = details.ServerInstance;
                selectedPartner = details.Partner;

                //get metrics from the underlying data which we got when the form loaded
                //principal metrics
                ExtractMetrics(details.CurrentMirroringMetrics, details.ServerInstance.Equals(details.PrincipalName));

                //mirror metrics
                ExtractMetrics(details.CurrentMirroringMetrics, details.ServerInstance.Equals(details.Partner));

                //general
                lblMirrorAddress.Text = ((string)lblMirrorAddress.Tag) + " " + details.MirrorAddress;
                lblOperatingMode.Text = ((string)lblOperatingMode.Tag) + " " + details.OperatingMode;
                lblPrincipalAddress.Text = ((string)lblPrincipalAddress.Tag) + " " + details.PrincipalAddress;
                lblWitnessAddress.Text = ((string)lblWitnessAddress.Tag) + " " + details.WitnessAddress;

                int MonitoredServerID = ServerNameToID(details.PrincipalName);
                int PartnerServerID = ServerNameToID(details.Partner);

                if (MonitoredServerID == 0 || PartnerServerID == 0)
                {
                   lblDetailsMessage.Text = NOT_MONITORING_BOTH_PARTNERS;
                   pnlMessage.Visible = true;
                   pnlMessage.BringToFront();
                   pnlDetails.Dock = DockStyle.None;
                   pnlDetails.Top = pnlMessage.Top + pnlMessage.Height;
                }
                else
                {
                   pnlMessage.Visible = false;
                   pnlDetails.Dock = DockStyle.Fill;
                   pnlDetails.Top = pnlMessage.Top;
                }

                //We now have all the data we can get from the grid and we must kick of asynchronous gathering of 
                //additional details

                //Do not try and get the details if ither the local or remot partners are still busy
                if(refreshMirroringDetailsBackgroundWorker != null && refreshPartnerDetailsBackgroundWorker != null)
                {
                    if(refreshMirroringDetailsBackgroundWorker.IsBusy ||
                        refreshPartnerDetailsBackgroundWorker.IsBusy) 
                    {
                        SetViewStatus(ViewStatus.Refreshing);
                        return;
                    }
                }

                localSnapshot = null;
                partnerSnapshot = null;

                mirroringDetailsRefreshStopwatch.Reset();
                mirroringDetailsRefreshStopwatch.Start();
                InitializeMirroringDetailsBackgroundWorker();
                refreshMirroringDetailsBackgroundWorker.RunWorkerAsync();

                partnerDetailsRefreshStopwatch.Reset();
                partnerDetailsRefreshStopwatch.Start();
                InitializePartnerDetailsBackgroundWorker();
                SetViewStatus(ViewStatus.Refreshing);
                refreshPartnerDetailsBackgroundWorker.RunWorkerAsync();

            }

            if (mirroredDatabasesGrid.Selected.Rows.Count == 0)
            {
                HideMirroringDetailsGridStatusMessage();
            }
        }


        /// <summary>
        /// Kicks of the asynchronous gathering of mirroring details
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void refreshMirroringDetailsBackgroundWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            if (System.Threading.Thread.CurrentThread.Name == null) System.Threading.Thread.CurrentThread.Name = "RefreshMirroringDetails";

            if (blnPopulatingDetails) return;

            blnPopulatingDetails = true;

            try
            {
                if (refreshMirroringDetailsBackgroundWorker.CancellationPending)
                {
                    e.Cancel = true;
                    return;
                }

                IManagementService managementService =
                    ManagementServiceHelper.GetDefaultService(Settings.Default.ActiveRepositoryConnection.ConnectionInfo);

                //get the id of the monitored instance
                int MonitoredServerID = ServerNameToID(selectedServer);
                int PartnerServerID = ServerNameToID(selectedPartner);
                if (MonitoredServerID == 0 || PartnerServerID == 0)
                {
                    intRefreshBusyFlags = intRefreshBusyFlags | (MonitoredServerID == 0 ? 1 : 0);
                    intRefreshBusyFlags = intRefreshBusyFlags | (PartnerServerID == 0 ? 2 : 0);
                }

                //if the server is being monitored
                if (MonitoredServerID > 0)
                {
                    localConfig = new MirrorMonitoringRealtimeConfiguration(MonitoredServerID);

                    //local meaning local to where the sql is running
                    localSnapshot = managementService.GetMirrorMonitoringRealtime(localConfig);

                }
                e.Result = localSnapshot;
            }
            
            
            catch(Exception ex)
            {
                e.Result = ex;
                blnPopulatingDetails = false;
            }
        }
        /// <summary>
        /// This is the final callback once the mirroring details have been gathered
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void refreshMirroringDetailsBackgroundWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            //bool blnresultOne=false;
            //bool blnresultTwo=false;
            
            //If this call has been cancelled or the result is null (this would happen if we are waiting
            //for results of the last call)
            if(e.Cancelled)
            {
                blnPopulatingDetails = false;
                return;
            }
            if (e.Result == null) return;

            try
            {
                mirroringDetailsRefreshStopwatch.Stop();
                MirroringMetrics.WitnessStatusEnum localWitnessStatus = MirroringMetrics.WitnessStatusEnum.NoWitness;
                if (!e.Cancelled)
                {
                    if (e.Error == null)
                    {
                        MirrorMonitoringRealtimeSnapshot result = (MirrorMonitoringRealtimeSnapshot)e.Result;

                        if (result != null)
                        {
                            if (result.Error == null)
                            {
                                if (result.Databases.Count > 0)
                                {
                                    HideMirroringDetailsGridStatusMessage();

                                    mirroringDetailsTabControlStatusLabel.SendToBack();
                                    mirroringDetailsTabControlStatusLabel.Visible = false;

                                    MirrorMonitoringDatabaseDetail localDetails = result.Databases[selectedDatabase];
                                    //blnresultOne = true;

                                    ExtractMetrics(localDetails.CurrentMirroringMetrics, true);

                                    lblOperatingMode.Text = ((string)lblOperatingMode.Tag) + " " + localDetails.OperatingMode;

                                    //if the partner is the mirror then get the address of the mirror 
                                    //else the partner must be the principal
                                    if (localDetails.MirrorName.Equals(localDetails.Partner))
                                    {
                                        //We can only get the address of a partner, not a local address
                                        lblMirrorAddress.Text = ((string)lblMirrorAddress.Tag) + " " + localDetails.MirrorAddress;
                                    }
                                    else
                                    {
                                        lblPrincipalAddress.Text = ((string)lblPrincipalAddress.Tag) + " " + localDetails.PrincipalAddress;
                                    }
                                    localWitnessStatus = localDetails.CurrentMirroringMetrics.WitnessStatus;
                                    lblWitnessAddress.Text = ((string)lblWitnessAddress.Tag) + " " + localDetails.WitnessAddress;
                                }
                            }
                            else
                            {
                                ShowMirroringDetailsGridStatusMessage(
                                    "An error occurred while refreshing mirroring details for local server.", false);
                                Log.Error("An error occurred while refreshing mirroring details.", result.Error);
                                ApplicationController.Default.ClearCustomStatus();
                                ApplicationController.Default.OnRefreshActiveViewCompleted(
                                    new RefreshActiveViewCompletedEventArgs(DateTime.Now, result.Error));
                            }
                        }
                        //if we did not get two resultsets or one of them had an exception
                        if (result.Error != null || partnerSnapshot == null || (partnerSnapshot != null && partnerSnapshot.Error != null))
                        {
         
                            if (result.Error != null || localSnapshot == null || (partnerSnapshot != null && partnerSnapshot.Error != null))
                            {
                                if (partnerSnapshot != null && partnerSnapshot.Error != null)
                                {
                                    lblDetailsMessage.Text = string.Format(PARTNER_REFRESH_ERROR, partnerSnapshot.ServerName);
                                }
                                else
                                {
                                    lblDetailsMessage.Text = string.Format(PARTNER_REFRESH_ERROR, result.ServerName);
                                }
                            }
                            pnlMessage.Visible = true;
                            pnlMessage.BringToFront();
                            pnlDetails.Dock = DockStyle.None;
                            pnlDetails.Top = pnlMessage.Top + pnlMessage.Height;
                        }
                        else
                        {
                            pnlMessage.Visible = false;
                            pnlDetails.Dock = DockStyle.Fill;
                            pnlDetails.Top = pnlMessage.Top;
                        }
                        SetViewStatus(ViewStatus.Normal);
                    }
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex.Message);
                ApplicationController.Default.ClearCustomStatus();
                ApplicationController.Default.OnRefreshActiveViewCompleted(
                    new RefreshActiveViewCompletedEventArgs(DateTime.Now, ex));
            }
            finally
            {
                SetViewStatus(ViewStatus.Normal);
                blnPopulatingDetails = false;
            }
        }

        void refreshPartnerDetailsBackgroundWorker_DoWork(object sender, System.ComponentModel.DoWorkEventArgs e)
        {
            if (System.Threading.Thread.CurrentThread.Name == null) System.Threading.Thread.CurrentThread.Name = "RefreshMirroringPartnerDetails";

            if (blnPopulatingPartnerDetails) return;

            blnPopulatingPartnerDetails = true;

            if (refreshPartnerDetailsBackgroundWorker.CancellationPending)
            {
                e.Cancel = true;
                return;
            }

            IManagementService managementService =
                ManagementServiceHelper.GetDefaultService(Settings.Default.ActiveRepositoryConnection.ConnectionInfo);

            //get the id of the partner instance
            int MonitoredServerID = ServerNameToID(selectedPartner);
            try
            {
                //if it is monitored
                if (MonitoredServerID > 0)
                {
                    partnerConfig = new MirrorMonitoringRealtimeConfiguration(MonitoredServerID);
                    partnerSnapshot = managementService.GetMirrorMonitoringRealtime(partnerConfig);

                }
                else
                {
                    partnerSnapshot = null;
                }

                e.Result = partnerSnapshot;
            }
            catch (Exception ex)
            {
                e.Result = ex;
                blnPopulatingPartnerDetails = false;
            }
        }

        void refreshPartnerDetailsBackgroundWorker_RunWorkerCompleted(object sender, System.ComponentModel.RunWorkerCompletedEventArgs e)
        {
            //bool blnresultTwo=false;
            
            //If this call has been cancelled or the result is null (this would happen if we are waiting
            //for results of the last call)
            if (e.Cancelled)
            {
                blnPopulatingPartnerDetails = false;
                return;
            }
            if (e.Result == null) return;

            try
            {
                partnerDetailsRefreshStopwatch.Stop();
                //MirroringMetrics.WitnessStatusEnum localWitnessStatus = MirroringMetrics.WitnessStatusEnum.NoWitness;
                if (!e.Cancelled)
                {
                    if (e.Error == null)
                    {
                        MirrorMonitoringRealtimeSnapshot result = e.Result as MirrorMonitoringRealtimeSnapshot;

                        if (result != null)
                        {
                                                        
                            if (result != null)
                            {
                                if (result.Error == null)
                                {
                                    if (result.Databases.Count > 0)
                                    {
                                        MirrorMonitoringDatabaseDetail partnerDetails =
                                            result.Databases[selectedDatabase];
                                        //blnresultTwo = true;

                                        ExtractMetrics(partnerDetails.CurrentMirroringMetrics, true);

                                        lblOperatingMode.Text = ((string) lblOperatingMode.Tag) + " " +
                                                                partnerDetails.OperatingMode;

                                        //if the partner is the mirror then get the address of the mirror 
                                        //else the partner must be the principal
                                        if (partnerDetails.MirrorName.Equals(partnerDetails.Partner))
                                        {
                                            //We can only get the address of a partner, not a local address
                                            if (partnerDetails.MirrorAddress != "")
                                                lblMirrorAddress.Text = ((string) lblMirrorAddress.Tag) + " " +
                                                                        partnerDetails.MirrorAddress;
                                        }
                                        else
                                        {
                                            if (partnerDetails.PrincipalAddress != "")
                                                lblPrincipalAddress.Text = ((string) lblPrincipalAddress.Tag) + " " +
                                                                           partnerDetails.PrincipalAddress;
                                        }

                                        if (partnerDetails.WitnessAddress != "No Witness")
                                        {
                                            //if the local partner think there is a different witness than the remote partner
                                            if(!lblWitnessAddress.Text.Equals(((string) lblWitnessAddress.Tag) + " " +
                                                                     partnerDetails.WitnessAddress)){
                                                lblWitnessAddress.Text = ((string) lblWitnessAddress.Tag) + " " +
                                                                     partnerDetails.WitnessAddress +
                                                                     " (local partner connection error)";
                                            }

                                        }
                                    }
                                }
                                else //if the partner refresh got an exception
                                {
                                    //log it
                                    //ShowMirroringDetailsGridStatusMessage(
                                    //    "An error occurred while refreshing mirroring details for partner.", true);
                                    Log.Error("An error occurred while refreshing mirroring details for partner.",
                                              result.Error);
                                    ApplicationController.Default.ClearCustomStatus();
                                    ApplicationController.Default.OnRefreshActiveViewCompleted(
                                        new RefreshActiveViewCompletedEventArgs(DateTime.Now, result.Error));
                                }
                            }
                        }
                        //if we did not get two resultsets or one of them had an exception
                        if (result.Error!=null)
                        {
                            //if data was not returned for this  query
                            if(result.Error!=null || partnerSnapshot == null)
                            {
                                lblDetailsMessage.Text=string.Format(PARTNER_REFRESH_ERROR, result.ServerName);
                            }
                            pnlMessage.Visible = true;
                            pnlMessage.BringToFront();
                            pnlDetails.Dock = DockStyle.None;
                            pnlDetails.Top = pnlMessage.Top + pnlMessage.Height;
                        }
                        else
                        {
                            pnlMessage.Visible = false;
                            pnlDetails.Dock = DockStyle.Fill;
                            pnlDetails.Top = pnlMessage.Top;
                        }
                        SetViewStatus(ViewStatus.Normal);
                    }
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex.Message);
                ApplicationController.Default.ClearCustomStatus();
                ApplicationController.Default.OnRefreshActiveViewCompleted(
                    new RefreshActiveViewCompletedEventArgs(DateTime.Now, ex));
            }
            finally
            {
                SetViewStatus(ViewStatus.Normal);
                blnPopulatingPartnerDetails = false;
            }
        }

        #endregion

        public void InitializeFailoverBackgroundWorker()
        {
            if (failOverBackgroundWorker != null)
            {
                if (failOverBackgroundWorker.IsBusy)
                {
                    failOverBackgroundWorker.CancelAsync();
                    failOverBackgroundWorker = null;
                    InitializeFailoverBackgroundWorker();
                }
            }
            else
            {
                failOverBackgroundWorker = new BackgroundWorker();
                failOverBackgroundWorker.WorkerSupportsCancellation = true;
                failOverBackgroundWorker.DoWork += new DoWorkEventHandler(failOverBackgroundWorker_DoWork);
                failOverBackgroundWorker.RunWorkerCompleted += new RunWorkerCompletedEventHandler(failOverBackgroundWorker_RunWorkerCompleted);
                failOverBackgroundWorker.WorkerSupportsCancellation = true;
            }
        }

        void failOverBackgroundWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            failOverStopwatch.Stop();

            if (!e.Cancelled)
            {
                if (e.Error == null)
                {
                    if(e.Result is ServerActionSnapshot)
                    {
                        ServerActionSnapshot snapshot = e.Result as ServerActionSnapshot;
                        ApplicationController.Default.ActiveView.CancelRefresh();
                        ApplicationController.Default.ActiveView.RefreshView();                        
                    }

                }
            }
            SetViewStatus(ViewStatus.Normal);

        }

        void failOverBackgroundWorker_DoWork(object sender, DoWorkEventArgs e)
        {

            if (System.Threading.Thread.CurrentThread.Name == null) System.Threading.Thread.CurrentThread.Name = "FailOverMirroring";

            IManagementService managementService =
                ManagementServiceHelper.GetDefaultService(Settings.Default.ActiveRepositoryConnection.ConnectionInfo);

            if (failOverBackgroundWorker.CancellationPending)
            {
                e.Cancel = true;
            }

            try
            {
                MirrorMonitoringDatabaseDetail detail = getSelectedMirroredDatabaseDetails();

                AuditingEngine.SetContextData(Settings.Default.ActiveRepositoryConnection.ConnectionInfo.ActiveRepositoryUser);

                if (detail != null)
                {
                    if (detail.CurrentMirroringMetrics.Role == MirroringMetrics.MirroringRoleEnum.Principal)
                    {
                        MirroringPartnerActionConfiguration configuration = new MirroringPartnerActionConfiguration(ServerNameToID(detail.ServerInstance), detail.DatabaseName, MirroringPartnerActionConfiguration.MirroringPartnerActions.Failover);
                        e.Result = (Snapshot)managementService.SendMirroringPartnerAction(configuration);
                    }
                    else
                    {
                        MirroringPartnerActionConfiguration configuration = new MirroringPartnerActionConfiguration(ServerNameToID(detail.Partner), detail.DatabaseName, MirroringPartnerActionConfiguration.MirroringPartnerActions.Failover);
                        e.Result = (Snapshot)managementService.SendMirroringPartnerAction(configuration);
                    }
                    System.Threading.Thread.Sleep(1000);
                }
            }
            catch (Exception ex)
            {
                e.Result = ex;
            }
        }

        public void InitializeSuspendResumeBackgroundWorker()
        {
            if (suspendResumeBackgroundWorker != null)
            {
                if (suspendResumeBackgroundWorker.IsBusy)
                {
                    suspendResumeBackgroundWorker.CancelAsync();
                    suspendResumeBackgroundWorker = null;
                    InitializeSuspendResumeBackgroundWorker();
                }
            }
            else
            {
                suspendResumeBackgroundWorker = new BackgroundWorker();
                suspendResumeBackgroundWorker.WorkerSupportsCancellation = true;
                suspendResumeBackgroundWorker.DoWork += new DoWorkEventHandler(suspendResumeBackgroundWorker_DoWork);
                suspendResumeBackgroundWorker.RunWorkerCompleted += new RunWorkerCompletedEventHandler(suspendResumeBackgroundWorker_RunWorkerCompleted);
                suspendResumeBackgroundWorker.WorkerSupportsCancellation = true;
            }
        }

        void suspendResumeBackgroundWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            suspendResumeStopwatch.Stop();

            if (!e.Cancelled)
            {
                if (e.Error == null)
                {
                    if (e.Result is ServerActionSnapshot)
                    {
                        ServerActionSnapshot snapshot = e.Result as ServerActionSnapshot;
                        ApplicationController.Default.ActiveView.CancelRefresh();
                        ApplicationController.Default.ActiveView.RefreshView();
                    }
                    if (e.Result is Exception)
                    {
                        throw ((Exception)e.Result);
                    }
                }
            }
            else
            {
                SetViewStatus(ViewStatus.Normal);
            }
        }

        void suspendResumeBackgroundWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            if (System.Threading.Thread.CurrentThread.Name == null) System.Threading.Thread.CurrentThread.Name = "SuspendResumeMirroring";

            IManagementService managementService =
                ManagementServiceHelper.GetDefaultService(Settings.Default.ActiveRepositoryConnection.ConnectionInfo);
            
            
            
            try
            {
                MirrorMonitoringDatabaseDetail detail = getSelectedMirroredDatabaseDetails();

                if (detail != null)
                {
                    AuditingEngine.SetContextData(Settings.Default.ActiveRepositoryConnection.ConnectionInfo.ActiveRepositoryUser);

                    if (detail.CurrentMirroringMetrics.MirroringState == MirroringMetrics.MirroringStateEnum.Synchronized)
                    {
                        MirroringPartnerActionConfiguration configuration = new MirroringPartnerActionConfiguration(ServerNameToID(detail.PrincipalName), detail.DatabaseName, MirroringPartnerActionConfiguration.MirroringPartnerActions.Suspend);
                        e.Result = (Snapshot)managementService.SendMirroringPartnerAction(configuration);
                        System.Threading.Thread.Sleep(1000);
                    }
                    else if (detail.CurrentMirroringMetrics.MirroringState == MirroringMetrics.MirroringStateEnum.Suspended)
                    {
                        MirroringPartnerActionConfiguration configuration = new MirroringPartnerActionConfiguration(ServerNameToID(detail.PrincipalName), detail.DatabaseName, MirroringPartnerActionConfiguration.MirroringPartnerActions.Resume);
                        e.Result = (Snapshot)managementService.SendMirroringPartnerAction(configuration);
                        System.Threading.Thread.Sleep(1000);
                    }
                    else
                    {
                        e.Result = new Exception("You can only suspend or resume a synchronized or a suspended session");
                    }
                }
            }
            catch (Exception ex)
            {
                e.Result = ex;
            }
        }
        #region Persistent Settings
        public override void ApplySettings()
        {
            // Fixed panel is second panel, so restore size of second panel
            lastMainSplitterDistance = splitContainer1.Height - Settings.Default.DatabasesMirroringViewMainSplitter;
            if (lastMainSplitterDistance > 0)
            {
                splitContainer1.SplitterDistance = lastMainSplitterDistance;
            }
            else
            {
                lastMainSplitterDistance = splitContainer1.Height - splitContainer1.SplitterDistance;
            }

            if (Settings.Default.MirroredDatabasesViewMirroredDatabasesGrid is GridSettings)
            {
                lastmirroringGridSettings = Settings.Default.MirroredDatabasesViewMirroredDatabasesGrid;
                GridSettings.ApplySettingsToGrid(lastmirroringGridSettings, mirroredDatabasesGrid);
            }
            if (Settings.Default.MirroredDatabasesViewHistoryGrid is GridSettings)
            {
                lasthistoryGridSettings = Settings.Default.MirroredDatabasesViewHistoryGrid;
                GridSettings.ApplySettingsToGrid(lasthistoryGridSettings, historyGrid);
            }

        }

        public override void SaveSettings()
        {
            try
            {
                GridSettings mainGridSettings = GridSettings.GetSettings(mirroredDatabasesGrid);
                lastmirroringGridSettings = Settings.Default.MirroredDatabasesViewMirroredDatabasesGrid = mainGridSettings;

                GridSettings historyGridSettings = GridSettings.GetSettings(historyGrid);
                lasthistoryGridSettings = Settings.Default.MirroredDatabasesViewHistoryGrid = historyGridSettings;

                if (lastMainSplitterDistance != splitContainer1.Height - splitContainer1.SplitterDistance)
                {
                    // Fixed panel is second panel, so save size of second panel
                    lastMainSplitterDistance = Settings.Default.DatabasesMirroringViewMainSplitter =
                        splitContainer1.Height - splitContainer1.SplitterDistance;
                }
            }
            catch (Exception e)
            {
                Log.Error("An error occurred while saving the mirroring view settings.", e);
            }
        }
        #endregion
        #region Public Functions

        public override void ShowHelp()
        {
            ApplicationHelper.ShowHelpTopic(HelpTopics.DatabasesMirroringView);
        }

        public void ToggleMirroredDatabasesGroupByBox()
        {
            if (mirroredDatabasesGrid != null)
            {
                MirroredDatabasesGridGroupByBoxVisible = !MirroredDatabasesGridGroupByBoxVisible;
            }
        }

        public void ToggleMirroringHistoryGroupByBox()
        {
            if (historyGrid != null)
            {
                MirroringHistoryGridGroupByBoxVisible = !MirroringHistoryGridGroupByBoxVisible;
            }
        }
        
        public void FailOverToPartner()
        {
            MirrorMonitoringDatabaseDetail detail = getSelectedMirroredDatabaseDetails();
            
            //string message = "";
            if(detail.CurrentMirroringMetrics.MirroringState != MirroringMetrics.MirroringStateEnum.Synchronized)
            {
                ApplicationMessageBox.ShowInfo(this, "The mirroring session is not synchronized. This session can not be failed over.");
                return;
            }

            if (ApplicationMessageBox.ShowQuestion(this, String.Format("Are you sure you want to perform a fail-over to {0}?", detail.MirrorName)) == DialogResult.Yes)
            {
                //ServerVersion verPrincipal = ApplicationModel.Default.GetInstanceStatus(ServerNameToID(detail.PrincipalName)).InstanceVersion;
                //ServerVersion verMirror = ApplicationModel.Default.GetInstanceStatus(ServerNameToID(detail.MirrorName)).InstanceVersion;

                ////if the versions differ
                //if (!verPrincipal.Major.Equals(verMirror.Major) | !verPrincipal.Minor.Equals(verMirror.Minor))
                //{
                //    //verify that the user really, really wants to do this
                //    if(ApplicationMessageBox.ShowQuestion(this, String.Format("You are about to perform a fail-over between disparate versions of SQL Server ({0} to {1}).  This is not recommended. Are you sure want to continue?", verPrincipal.ToString(), verMirror.ToString()))== DialogResult.No)
                //    {
                //        return;
                //    }
                //}
                failOverStopwatch.Reset();
                failOverStopwatch.Start();
                InitializeFailoverBackgroundWorker();
                SetViewStatus(ViewStatus.FailingOver);
                failOverBackgroundWorker.RunWorkerAsync();
            }
        }

        public void SuspendResumeSession()
        {
            MirrorMonitoringDatabaseDetail detail = getSelectedMirroredDatabaseDetails();

            string message = "";
            switch(detail.CurrentMirroringMetrics.MirroringState)
            {
                case MirroringMetrics.MirroringStateEnum.Synchronized:
                    message = "Are you sure you want to suspend the selected mirroring session?";
                    break;
                case MirroringMetrics.MirroringStateEnum.Suspended:
                    //message = "Are you sure you want to resume the selected mirroring session?";
                    break;
                default:
                    ApplicationMessageBox.ShowInfo(this, "The mirroring session is not in a state that allows suspend or resume.");
                    return;
            }

            if(message != "" && (ApplicationMessageBox.ShowQuestion(this, message) != DialogResult.Yes)) return;

            suspendResumeStopwatch.Reset();
            suspendResumeStopwatch.Start();
            InitializeSuspendResumeBackgroundWorker();
            SetViewStatus(ViewStatus.Suspending);
            suspendResumeBackgroundWorker.RunWorkerAsync();
        }

        /// <summary>
        /// This is method is called whenever a refresh is requested
        /// It kicks of a backgroundworker to do the actual refresh and we get UpdateData when it returns
        /// </summary>
        /// <returns></returns>
        public override object DoRefreshWork()
        {
            if (refreshMirroringHistoryBackgroundWorker.IsBusy) refreshMirroringHistoryBackgroundWorker.CancelAsync();
            if (refreshMirroringDetailsBackgroundWorker.IsBusy) refreshMirroringDetailsBackgroundWorker.CancelAsync();

            IManagementService managementService =
                ManagementServiceHelper.GetDefaultService(Settings.Default.ActiveRepositoryConnection.ConnectionInfo);
            
                Snapshot _snapshot = (Snapshot)managementService.GetMirrorMonitoringRealtime(mirroringRealtimeConfig);

                Dictionary<Guid, MirroringSession> _preferredConfig = managementService.GetMirroringPreferredConfig();

            return new Pair<Snapshot, Dictionary<Guid, MirroringSession>>(_snapshot, _preferredConfig);
        }

        #endregion
        #region Refresh overrides
        public override void RefreshView()
        {
            if (HistoricalSnapshotDateTime == null)
            {
                DatabasesMirroring_Fill_Panel.Visible = true;

                base.RefreshView();

            }
            else
            {
                if (ActionsAllowedChanged != null)
                {
                    ActionsAllowedChanged(this, EventArgs.Empty);
                }
                DatabasesMirroring_Fill_Panel.Visible = false;
                ApplicationController.Default.SetRefreshStatusText(Properties.Resources.HistoryModeStatusBarLabel);
            }
        }

        //cancel all asyncronous calls
        public override void CancelRefresh()
        {
            refreshAvailableDatabasesBackgroundWorker.CancelAsync();
            refreshMirroringHistoryBackgroundWorker.CancelAsync();
            refreshMirroringDetailsBackgroundWorker.CancelAsync();
            base.CancelRefresh();
        }
        #endregion
        #region GetAvailableDatabases Asynchronous
        /// <summary>
        /// This function is not called at present
        /// </summary>
        private void RefreshAvailableDatabases()
        {
            if (!refreshAvailableDatabasesBackgroundWorker.IsBusy)
            {
                mirroredDatabasesComboBox.Enabled = false;
                mirroredDatabasesComboBox.Items.Clear();
                mirroredDatabasesComboBox.Items.Add(null, "< "+ Idera.SQLdm.Common.Constants.LOADING+" >");
                mirroredDatabasesComboBox.SelectedIndex = 0;

                Log.Info("Refreshing available databases...");
                mirroredDatabasesRefreshStopwatch.Reset();
                mirroredDatabasesRefreshStopwatch.Start();
                refreshAvailableDatabasesBackgroundWorker.RunWorkerAsync();
            }
        }

        private void refreshAvailableDatabasesBackgroundWorker_DoWork(object sender, System.ComponentModel.DoWorkEventArgs e)
        {
                if (System.Threading.Thread.CurrentThread.Name == null) System.Threading.Thread.CurrentThread.Name = "RefreshAvailableDatabasesWorker";

                IManagementService managementService =
                    ManagementServiceHelper.GetDefaultService(Settings.Default.ActiveRepositoryConnection.ConnectionInfo);

                e.Result = managementService.GetDatabases(instanceId, false, true);

                if (refreshAvailableDatabasesBackgroundWorker.CancellationPending)
                {
                    e.Cancel = true;
                }
            
        }

        private void refreshAvailableDatabasesBackgroundWorker_RunWorkerCompleted(object sender,
                                                                                  RunWorkerCompletedEventArgs e)
        {
            mirroredDatabasesRefreshStopwatch.Stop();
            mirroredDatabasesComboBox.Items.Clear();

            if (!e.Cancelled)
            {
                if (e.Error == null)
                {
                    Log.Info(string.Format("Refresh available databases completed (Duration = {0}).", mirroredDatabasesRefreshStopwatch.Elapsed));
                    IDictionary<string, bool> availableDatabases = e.Result as IDictionary<string, bool>;

                    if (availableDatabases != null && availableDatabases.Count > 0)
                    {
                        foreach (string database in availableDatabases.Keys)
                        {
                            mirroredDatabasesComboBox.Items.Add(database, database);
                        }

                        int index = -1;

                        mirroredDatabasesComboBox.SelectedIndex = index >= 0 ? index : 0;

                        //availableDatabasesInitialized = true;
                        mirroredDatabasesComboBox.Enabled = true;
                    }

                    if (mirroredDatabasesComboBox.Items.Count == 0)
                    {
                        //availableDatabasesInitialized = false;
                        mirroredDatabasesComboBox.Items.Add(null,
                                                             "< No databases are currently available. Please refresh again... >");
                        mirroredDatabasesComboBox.Enabled = false;
                    }
                }
                else
                {
                    //availableDatabasesInitialized = false;
                    mirroredDatabasesComboBox.Items.Add(null,
                                                         "< Error retrieving databases. >");
                    mirroredDatabasesComboBox.Enabled = false;
                    //ShowTablesGridStatusMessage("Unable to update data for this view.");
                    //ShowTableDetailsStatusMessage("Unable to update data for this view.");
                    ApplicationController.Default.ClearCustomStatus();
                    ApplicationController.Default.OnRefreshActiveViewCompleted(
                        new RefreshActiveViewCompletedEventArgs(DateTime.Now, e.Error));
                }
            }
        }
        #endregion
        void toolbarsManager_BeforeToolDropdown(object sender, Infragistics.Win.UltraWinToolbars.BeforeToolDropdownEventArgs e)
        {
            if (e.Tool.Key == "gridDataContextMenu" || e.Tool.Key == "detailsGridContextMenu")
            {
                    ((PopupMenuTool)e.Tool).Tools["mirroringSuspendResume"].SharedProps.Visible =
                        ((PopupMenuTool)e.Tool).Tools["mirroringFailOver"].SharedProps.Visible =
                            ActionsAllowed;
                    ((PopupMenuTool)e.Tool).Tools["markSessionNormal"].SharedProps.Visible = OperationalStatusChangesAllowed;
                    ((PopupMenuTool)e.Tool).Tools["markSessionFailedOver"].SharedProps.Visible = OperationalStatusChangesAllowed;
                    ((PopupMenuTool)e.Tool).Tools["markSessionRoleAgnostic"].SharedProps.Visible = OperationalStatusChangesAllowed;

            }

            if (e.Tool.Key == "gridDataContextMenu" || e.Tool.Key == "gridContextMenu")
            {
                bool isGrouped = selectedGrid.Rows.Count > 0 && selectedGrid.Rows[0].IsGroupByRow;
                ((PopupMenuTool)e.Tool).Tools["collapseAllGroupsButton"].SharedProps.Enabled =
                    ((PopupMenuTool)e.Tool).Tools["expandAllGroupsButton"].SharedProps.Enabled = isGrouped;
            }
            if (e.Tool.Key == "columnContextMenu")
            {
                int minCantForEnable = UltraGridHelper.GetNotHiddenColumns(selectedGrid);
                bool enableTool = minCantForEnable > 1 ? true : false;

                ((PopupMenuTool)e.Tool).Tools["removeThisColumnButton"].SharedProps.Enabled = enableTool;
            }
        }

        private void toolbarsManager_ToolClick(object sender, ToolClickEventArgs e)
        {
            switch (e.Tool.Key)
            {
                case "sortAscendingButton":
                    SortSelectedColumnAscending();
                    break;
                case "sortDescendingButton":
                    SortSelectedColumnDescending();
                    break;
                case "toggleGroupByBoxButton":
                    ToggleGroupByBox();
                    break;
                case "groupByThisColumnButton":
                    GroupBySelectedColumn(((StateButtonTool)e.Tool).Checked);
                    break;
                case "removeThisColumnButton":
                    RemoveSelectedColumn();
                    break;
                case "showColumnChooserButton":
                    ShowColumnChooser();
                    break;
                case "printGridButton":
                    PrintGrid();
                    break;
                case "exportGridButton":
                    SaveGrid();
                    break;
                case "mirroringFailOver":
                    FailOverToPartner();
                    break;
                case "mirroringSuspendResume":
                    SuspendResumeSession();
                    break;
                case "collapseAllGroupsButton":
                    if (selectedGrid != null)
                    {
                        CollapseAllGroups(selectedGrid);
                    }
                    break;
                case "expandAllGroupsButton":
                    if (selectedGrid != null)
                    {
                        ExpandAllGroups(selectedGrid);
                    }
                    break;
                case "markSessionNormal":
                    setOperationalStatus(MirroringSession.MirroringPreferredConfig.Normal);
                    break;
                case "markSessionFailedOver":
                    setOperationalStatus(MirroringSession.MirroringPreferredConfig.FailedOver);
                    break;
                case "markSessionRoleAgnostic":
                    setOperationalStatus(MirroringSession.MirroringPreferredConfig.Delete);
                    break;
            }
        }
        #region Grid
        private void setOperationalStatus(MirroringSession.MirroringPreferredConfig opStatus)
        {
            setOperationalStatusStopwatch.Reset();
            setOperationalStatusStopwatch.Start();
            InitializeSuspendResumeBackgroundWorker();
            setOperationalStatusBackgroundWorker.RunWorkerAsync(opStatus);
        }

        private void SortSelectedColumnAscending()
        {
            if (selectedColumn != null && selectedGrid != null)
            {
                selectedGrid.DisplayLayout.Bands[0].SortedColumns.Clear();
                selectedGrid.DisplayLayout.Bands[0].SortedColumns.Add(selectedColumn, false, false);
            }
        }

        private void SortSelectedColumnDescending()
        {
            if (selectedColumn != null && selectedGrid != null)
            {
                selectedGrid.DisplayLayout.Bands[0].SortedColumns.Clear();
                selectedGrid.DisplayLayout.Bands[0].SortedColumns.Add(selectedColumn, true, false);
            }
        }

        public void ToggleGroupByBox()
        {
            if (selectedGrid == mirroredDatabasesGrid)
            {
                MirroredDatabasesGridGroupByBoxVisible = !MirroredDatabasesGridGroupByBoxVisible;
            }
            else if (selectedGrid == historyGrid)
            {
                selectedGrid.DisplayLayout.GroupByBox.Hidden = !selectedGrid.DisplayLayout.GroupByBox.Hidden;
            }
        }

        private void GroupBySelectedColumn(bool GroupBy)
        {
            if (selectedColumn != null && selectedGrid != null)
            {
                if (GroupBy)
                {
                    selectedGrid.DisplayLayout.Bands[0].SortedColumns.Add(selectedColumn, false, true);
                }
                else
                {
                    selectedGrid.DisplayLayout.Bands[0].SortedColumns.Remove(selectedColumn);
                }
            }
        }

        private void RemoveSelectedColumn()
        {
            if (selectedColumn != null)
            {
                selectedColumn.Hidden = true;
            }
        }

        private void CollapseAllGroups(UltraGrid grid)
        {
            grid.Rows.CollapseAll(true);
        }

        private void ExpandAllGroups(UltraGrid grid)
        {
            grid.Rows.ExpandAll(true);
        }

        private void ShowColumnChooser()
        {
            if (selectedGrid != null)
            {
                SimpleUltraGridColumnChooserDialog dialog = new SimpleUltraGridColumnChooserDialog(selectedGrid);
                dialog.Show(this);
            }
        }

        private void PrintGrid()
        {
            if (selectedGrid != null)
            {
                ultraGridPrintDocument.Grid = selectedGrid;
                ultraPrintPreviewDialog.Document = ultraGridPrintDocument;
                ultraGridPrintDocument.DefaultPageSettings.Landscape = true;
                ultraGridPrintDocument.Header.TextLeft =
                    string.Format("{0} - {1}",
                                  ApplicationModel.Default.ActiveInstances[instanceId].InstanceName,
                                  DateTime.Now.ToString("G")
                        );
                ultraGridPrintDocument.Footer.TextCenter = "Page [Page #]";

                ultraPrintPreviewDialog.ShowDialog();
            }
        }

        private void SaveGrid()
        {
            if (selectedGrid != null)
            {
                saveFileDialog.DefaultExt = "xls";
                if (selectedGrid == mirroredDatabasesGrid)
                {
                    saveFileDialog.FileName = "Mirrored Databases";
                }
                else
                {
                    saveFileDialog.FileName = "Mirroring History";
                }
                saveFileDialog.Filter = "Excel Workbook (*.xls)|*.xls";
                saveFileDialog.Title = "Save as Excel Spreadsheet";

                if (saveFileDialog.ShowDialog() == DialogResult.OK)
                {
                    try
                    {
                        ultraGridExcelExporter.Export(selectedGrid, saveFileDialog.FileName);
                    }
                    catch (Exception ex)
                    {
                        ApplicationMessageBox.ShowError(this, "An error occurred while exporting data.", ex);
                    }
                }
            }
        }
        
#endregion
        public void InitializeSetOperationalStatusBackgroundWorker()
        {
            if (setOperationalStatusBackgroundWorker != null)
            {
                if (setOperationalStatusBackgroundWorker.IsBusy)
                {
                    setOperationalStatusBackgroundWorker.CancelAsync();
                    setOperationalStatusBackgroundWorker = null;
                    InitializeSetOperationalStatusBackgroundWorker();
                }
            }
            else
            {
                setOperationalStatusBackgroundWorker = new BackgroundWorker();
                setOperationalStatusBackgroundWorker.WorkerSupportsCancellation = true;
                setOperationalStatusBackgroundWorker.DoWork += new DoWorkEventHandler(suspendResumeBackgroundWorker_DoWork);
                setOperationalStatusBackgroundWorker.RunWorkerCompleted += new RunWorkerCompletedEventHandler(suspendResumeBackgroundWorker_RunWorkerCompleted);
                setOperationalStatusBackgroundWorker.WorkerSupportsCancellation = true;
            }
        }

        private void setOperationalStatusBackgroundWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            if (System.Threading.Thread.CurrentThread.Name == null) System.Threading.Thread.CurrentThread.Name = "setOperationalStatus";

            IManagementService managementService =
                ManagementServiceHelper.GetDefaultService(Settings.Default.ActiveRepositoryConnection.ConnectionInfo);

            try
            {
                MirrorMonitoringDatabaseDetail detail = getSelectedMirroredDatabaseDetails();
                if (e.Argument is MirroringSession.MirroringPreferredConfig)
                {
                    if (detail != null)
                    {
                        int principalId = ServerNameToID(detail.PrincipalName);
                        int mirrorId = ServerNameToID(detail.MirrorName);

                        if (principalId == -1 || mirrorId == -1)
                        {
                            throw new Exception("You must be monitoring both partners in the mirroring session to be able to save a preferred state.");
                        }
                        MirroringSession session = new MirroringSession(detail.MirroringGuid, principalId, 
                            mirrorId,detail.DatabaseName,
                            detail.WitnessAddress, (MirroringSession.MirroringPreferredConfig)e.Argument);

                        managementService.SetMirroringPreferredConfig(session);
                    }
                }
            }
            catch (Exception ex)
            {
                e.Result = ex;
            }

        }
        private void setOperationalStatusBackgroundWorker_RunWorkerCompleted(object sender, System.ComponentModel.RunWorkerCompletedEventArgs e)
        {
            setOperationalStatusStopwatch.Stop();

            if (!e.Cancelled)
            {
                if (e.Error == null)
                {
                    ApplicationController.Default.ActiveView.CancelRefresh();
                    ApplicationController.Default.ActiveView.RefreshView();
                    if (e.Result is Exception)
                    {
                        throw ((Exception)e.Result);
                    }
                }
            }
        }

        public class srtComparer : IComparer
        {
            public srtComparer()
            {
            }

            public int Compare(object x, object y)
            {
                
                UltraGridCell xCell = (UltraGridCell)x;
                UltraGridCell yCell = (UltraGridCell)y;

                return (int)xCell.Row.Cells["intTimeSpan"].Value - (int)yCell.Row.Cells["intTimeSpan"].Value;
            }
        }
        
        private void historicalSnapshotStatusLinkLabel_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            ApplicationController.Default.SetActiveViewToRealTimeMode();
        }

        /// <summary>
        /// Adapts the size of the font for this control in case of OS font changes.
        /// </summary>
        private void AdaptFontSize()
        {
            AutoScaleFontHelper.Default.AutoScaleControl(this, AutoScaleFontHelper.ControlType.Container);
        }

        void OnCurrentThemeChanged(object sender, EventArgs e)
        {
            SetGridTheme();
            if (Settings.Default.ColorScheme == "Dark")
            {
                if (!refreshDatabasesButton.Enabled)
                    appearance1.Image = Helpers.ImageUtils.ChangeOpacity(global::Idera.SQLdm.DesktopClient.Properties.Resources.ToolbarRefresh, 0.50F);
                this.refreshDatabasesButton.UseOsThemes = DefaultableBoolean.False;
                this.refreshDatabasesButton.UseAppStyling = false;
                this.refreshDatabasesButton.ButtonStyle = UIElementButtonStyle.FlatBorderless;
            }
            else
            {
                this.refreshDatabasesButton.UseAppStyling = true;
            }
        }

        private void SetGridTheme()
        {
            // Update UltraGrid Theme
            var themeManager = new GridThemeManager();
            themeManager.updateGridTheme(this.historyGrid);
            themeManager.updateGridTheme(this.transactionLogsGrid);
            themeManager.updateGridTheme(this.mirroredDatabasesGrid);
        }
    }
}
