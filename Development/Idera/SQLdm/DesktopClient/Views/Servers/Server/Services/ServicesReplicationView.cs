using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Windows.Forms;
using Idera.SQLdm.Common;
using Idera.SQLdm.Common.Configuration;
using Idera.SQLdm.Common.Events;
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
using Infragistics.Win.UltraWinToolbars;
using Wintellect.PowerCollections;
using ColumnHeader = Infragistics.Win.UltraWinGrid.ColumnHeader;
using System.Diagnostics;
using System.ComponentModel;
using System.Threading;
using Idera.SQLdm.Common.Objects.Replication;
using ChartFX.WinForms;
using Idera.SQLdm.Common.Objects.ApplicationSecurity;
using System.Globalization;

namespace Idera.SQLdm.DesktopClient.Views.Servers.Server.Services
{
    internal partial class ServicesReplicationView : ServerBaseView, IShowFilterDialog
    {
        #region constants
        private const int REPLTIMEOUT = 10000;
        private const string THREAD_TIMEOUT = @"Thread timeout occurred prior to acquiring data from the {0}.";
        private const string UNSUBSCRIBED_SESSION = "This selected session has no subscriber configured";
        private const string SERVER_NOTFULFILLING_ROLE = "This server is not serving as a {0}. The metric does not exist on this instance.";
        private const string NO_ITEMS = @"There are no items to show in this view.";
        private const string UNABLE_TO_UPDATE = @"Unable to update data for this view.";
        private const string SELECT_DATABASE = @"Select a valid distributor database from the publisher transactions.";
        private const string STATUS_NOT_INSTALLED = @"Replication is not installed on this server.";
        private const string STATUS_UNKNOWN = @"Replication status is currently unavailable.";
        private const string PUBLISHER_NO_ROWS = "No rows were returned from the publisher. \n SQL Agent Status: {0}";
        private const string STATUS_COLLECTION_DISABLED = "Replication statistics collection is disabled.";
        private const string SERVER_NOT_MONITORED = "The {0} server {1} is not currently monitored by SQL Diagnostic Manager and cannot be reached for information.";
        private const string CANT_REACH_DISTRIBUTOR = "The publisher is unable to contact the distributor {0}";
        private const string DISTRIBUTOR_FORMAT = "{0}.{1}";
        private const string ReplicationDisabledMessage =
            "The collection of replication statistics is currently disabled. Click here to configure the collection of replication statistics now.";
        private const string ReplicationMonitoringDisabled = "Replication monitoring is disabled";
        #endregion

        #region fields
        private DateTime? historicalSnapshotDateTime;
        private bool initialized;
        private bool distributionInitialized;
        private DataTable topologyDataTable;
        private DataTable distributorQueueDataTable;
        private DataTable publisherDataTable;
        private DataTable subscriberDataTable;
        private DataTable distributorDataTable;
        //private ReplicationType _currentlySelectedReplicationType = ReplicationType.Transaction;
        /// <summary>
        /// stores the previous value so we can do a delta on it
        /// </summary>
        private Dictionary<int,Pair<DateTime, long>> previousSubscribedCollection;

        private DataTable distributorMergeDataTable;
        private DataTable publisherMergeDataTable;
        private DataTable subscriberMergeDataTable;

        private DataTable publisherHistoryDataTable = new DataTable();
        private DataTable distributorHistoryDataTable = new DataTable();

        private PublisherDetailsConfiguration publisherConfiguration;
        private DistributorDetailsConfiguration configurationDistributors;
        private ReplicationFilter replicationFilter;
        private PublisherDetails currentSnapshot = null;
        private DistributorDetails distributorSnapshot = null;
        private bool isReplicationServer = true;
        private string replicationStatusDisplay = string.Empty;

        private static readonly object updateLock = new object();
        private UltraGrid selectedGrid = null;
        private UltraGridColumn selectedColumn = null;
        private Control focused = null;

        //last Settings values used to determine if changed for saving when leaving
        private int lastSplitterDistance = 0;
        private int lastGridSplitterDistance = 0;
        private GridSettings lastMainGridSettings = null;
        private GridSettings lastDistributorGridSettings = null;
        
        private Stopwatch DistributionDetailsRefreshStopwatch = new Stopwatch();
        private Stopwatch PublicationDetailsRefreshStopwatch = new Stopwatch();

        private bool blnPopulatingReplicationDetails = false;
        //private bool blnPopulatingPublicationDetails = false;

        private int LastInstanceId = -1;
        private int intSelectedServerRoleBitMask = 0;
        #endregion

        #region constructors

        public ServicesReplicationView(int instanceId)
            : base(instanceId)
        {
            InitializeComponent();

            historicalSnapshotStatusLinkLabel.Text = Properties.Resources.HistoryModeUnsupportedViewLabel;
            HideFocusRectangleDrawFilter hideFocusRectangleDrawFilter = new HideFocusRectangleDrawFilter();
            distributorTransactionsGrid.DrawFilter = hideFocusRectangleDrawFilter;
            grdReplicationTopology.DrawFilter = hideFocusRectangleDrawFilter;

            grdReplicationTopology.Tag = topologyHeaderStripLabel;
            //distributorTransactionsGrid.Tag = distributorTransactionQueueHeaderStripLabel;

            if (ApplicationModel.Default.ActiveInstances.Contains(instanceId))
            {
                MonitoredSqlServerWrapper instanceWrapper = ApplicationModel.Default.ActiveInstances[instanceId];
                instanceWrapper.Changed += new EventHandler<MonitoredSqlServerChangedEventArgs>(MonitoredSqlServerChanged);
                UpdateOperationalStatus(instanceWrapper.Instance);
            }

            //create the custom filter and then set the configs to match it
            replicationFilter = new ReplicationFilter();
            publisherConfiguration = new PublisherDetailsConfiguration(instanceId, string.Empty);
            configurationDistributors = new DistributorDetailsConfiguration(0, string.Empty, string.Empty, string.Empty,string.Empty, string.Empty, Idera.SQLdm.Common.Objects.Replication.ReplicationType.Transaction);
            configurationDistributors.FilterTimeSpan = replicationFilter.FilterTimeSpan;

            lblTopologyGridStatus.Text = distributorTransactionsGridStatusLabel.Text = Idera.SQLdm.Common.Constants.LOADING;

            grdReplicationTopology.Visible = distributorTransactionsGrid.Visible = false;

            Settings.Default.PropertyChanged += Settings_PropertyChanged;
            
            InitializeTopologyDataTable();

            InitializePublisherDataTable();
            InitializeDistributorDataTable();
            InitializeMergeDistributionDataTables();
            
            InitializeSubscriberDataTable();
            
            InitializeDistributorQueueDataTable();

            InitializeDistributorHistoryDataTable();
            InitializePublisherHistoryDataTable();

            InitializeCharts();
            AdaptFontSize();
        }

        #endregion

        #region Properties

        public event EventHandler FilterChanged;
        public event EventHandler ReplicationGraphVisibleChanged;
        public event EventHandler TopologyGridGroupByBoxVisibleChanged;
        public event EventHandler DistributorGridGroupByBoxVisibleChanged;

        public override DateTime? HistoricalSnapshotDateTime
        {
            get { return historicalSnapshotDateTime; }
            set { historicalSnapshotDateTime = value; }
        }

        /// <summary>
        /// Get or Set the replication graph visibility
        /// </summary>
        public bool ReplicationGraphsVisible
        {
            get { return splitContainer.Panel1.Visible; }
            set
            {
                splitContainer.Panel1.Visible = value;
                splitContainer.Panel1Collapsed = !value;

                if (ReplicationGraphVisibleChanged != null)
                {
                    ReplicationGraphVisibleChanged(this, EventArgs.Empty);
                }
            }
        }

        /// <summary>
        /// Get or Set the Topology Grid GroupByBox visibility and trigger state update event if changed
        /// </summary>
        public bool TopologyGridGroupByBoxVisible
        {
            get { return !grdReplicationTopology.DisplayLayout.GroupByBox.Hidden; }
            set
            {
                grdReplicationTopology.DisplayLayout.GroupByBox.Hidden = !value;

                if (TopologyGridGroupByBoxVisibleChanged != null)
                {
                    TopologyGridGroupByBoxVisibleChanged(this, EventArgs.Empty);
                }
            }
        }

        /// <summary>
        /// Get or Set the Distributor Grid GroupByBox visibility and trigger state update event if changed
        /// </summary>
        public bool DistributorGridGroupByBoxVisible
        {
            get { return !distributorTransactionsGrid.DisplayLayout.GroupByBox.Hidden; }
            set
            {
                distributorTransactionsGrid.DisplayLayout.GroupByBox.Hidden = !value;

                if (DistributorGridGroupByBoxVisibleChanged != null)
                {
                    DistributorGridGroupByBoxVisibleChanged(this, EventArgs.Empty);
                }
            }
        }

        /// <summary>
        /// Get the current configuration settings to manage state for current selections
        /// </summary>
        public ReplicationFilter Configuration
        {
            get { return replicationFilter; }
        }

        #endregion

        #region methods
        /// <summary>
        /// Convert the status key to an int that is used by the valuelist
        /// </summary>
        /// <param name="statusKey"></param>
        /// <returns></returns>
        private int GetServerStatusAsInt(string statusKey)
        {
            switch (statusKey)
            {
                case "ServerMaintenanceMode":
                    return 0;
                case "ServerCritical":
                    return 1;
                case "ServerWarning":
                    return 2;
                case "ServerOK":
                    return 3;
                case "ServerInformation":
                    return 4;
                case "Server":
                    return 5;
            }
            return 5;
        }

        /// <summary>
        /// Set the tooltips for the status columns based on the server status
        /// </summary>
        /// <param name="grid"></param>
        private static void SetServerStatusTooltips(UltraGridBase grid)
        {
            foreach (UltraGridRow row in grid.Rows)
            {
                if (row.Cells == null) continue;

                int intServer = 0;

                MonitoredSqlServerStatus publisherStatus = null;
                MonitoredSqlServerStatus distributorStatus = null;
                MonitoredSqlServerStatus subscriberStatus = null;

                int.TryParse(row.Cells["PublisherServerID"].Value.ToString(), out intServer);
                if (intServer > 0) publisherStatus = ApplicationModel.Default.GetInstanceStatus(intServer);
                row.Cells["Publisher Status"].ToolTipText = publisherStatus != null ? publisherStatus.ToolTipHeading + '\n' + '\n' + publisherStatus.ToolTip : string.Format(SERVER_NOT_MONITORED, "publisher", row.Cells["PublisherInstance"].Value);

                int.TryParse(row.Cells["DistributorServerID"].Value.ToString(), out intServer);
                if (intServer > 0) distributorStatus = ApplicationModel.Default.GetInstanceStatus(intServer);
                row.Cells["Distributor Status"].ToolTipText = distributorStatus != null ? distributorStatus.ToolTipHeading + '\n' + '\n' + distributorStatus.ToolTip : string.Format(SERVER_NOT_MONITORED, "distributor", row.Cells["DistributorInstance"].Value);

                int.TryParse(row.Cells["SubscriberServerID"].Value.ToString(), out intServer);
                if (intServer > 0) subscriberStatus = ApplicationModel.Default.GetInstanceStatus(intServer);
                row.Cells["Subscriber Status"].ToolTipText = subscriberStatus != null ? subscriberStatus.ToolTipHeading + '\n' + '\n' + subscriberStatus.ToolTip : string.Format(SERVER_NOT_MONITORED, "subscriber", row.Cells["SubscriberInstance"].Value);
            }
        }
        /// <summary>
        /// Navigate to the required replication participant
        /// </summary>
        /// <param name="role"></param>
        /// <param name="grid"></param>
        private static void NavigateToServer(ReplicationRole role, UltraGrid grid)
        {
            foreach(UltraGridRow row in grid.Selected.Rows)
            {
                int intServer = 0;
                switch (role)
                {
                    case ReplicationRole.Publisher:
                        int.TryParse(row.Cells["PublisherServerID"].Value.ToString(), out intServer);
                        if(intServer> 0) ApplicationController.Default.ShowServerView(intServer,ServerViews.ServicesReplication);
                        break;
                    case ReplicationRole.Distributor:
                        int.TryParse(row.Cells["DistributorServerID"].Value.ToString(), out intServer);
                        if(intServer> 0) ApplicationController.Default.ShowServerView(intServer,ServerViews.ServicesReplication);
                        break;
                    case ReplicationRole.Subscriber:
                        int.TryParse(row.Cells["SubscriberServerID"].Value.ToString(), out intServer);
                        if(intServer> 0) ApplicationController.Default.ShowServerView(intServer,ServerViews.ServicesReplication);
                        break;
                }
                break;
            }
        }
        //return true if the user has modify rights to the target server
        private static bool UserHasRightsToServer(string ServerID)
        {
            int intServerID = 0;
            int.TryParse(ServerID, out intServerID);
            if (intServerID == 0) return false;
            if (!ApplicationModel.Default.ActiveInstances.Contains(intServerID)) return false;
            return ApplicationModel.Default.UserToken.GetServerPermission(intServerID) >= PermissionType.Modify ? true : false;

        }
        public override void ShowHelp()
        {
            ApplicationHelper.ShowHelpTopic(HelpTopics.ServicesReplicationView);
        }

        public override void ApplySettings()
        {
            ReplicationGraphsVisible = !Settings.Default.HideReplicationTrendGraphs;

            lastSplitterDistance = Settings.Default.ServicesReplicationViewMainSplitter;
            if (lastSplitterDistance > 0)splitContainer.SplitterDistance = lastSplitterDistance;

            lastGridSplitterDistance = Settings.Default.ServicesReplicationViewGridSplitter;
            if (lastGridSplitterDistance > 0) splitGrids.SplitterDistance = lastGridSplitterDistance;

            if (Settings.Default.ServicesReplicationViewTopologyGrid is GridSettings)
            {
                lastMainGridSettings = Settings.Default.ServicesReplicationViewTopologyGrid;
                GridSettings.ApplySettingsToGrid(lastMainGridSettings, grdReplicationTopology);
                // force a change so ribbon stays in sync
                TopologyGridGroupByBoxVisible = TopologyGridGroupByBoxVisible;
            }

            if (Settings.Default.ServicesReplicationViewDistributorGrid is GridSettings)
            {
                lastDistributorGridSettings = Settings.Default.ServicesReplicationViewDistributorGrid;
                GridSettings.ApplySettingsToGrid(lastDistributorGridSettings, distributorTransactionsGrid);
                // force a change so ribbon stays in sync
                DistributorGridGroupByBoxVisible = DistributorGridGroupByBoxVisible;
            }
        }

        public override void SaveSettings()
        {
            GridSettings publisherGridSettings = GridSettings.GetSettings(grdReplicationTopology);
            GridSettings distributorGridSettings = GridSettings.GetSettings(distributorTransactionsGrid);
            
            Settings.Default.HideReplicationTrendGraphs = !ReplicationGraphsVisible;

            // save all settings only if anything has changed
            if (lastSplitterDistance != splitContainer.SplitterDistance
                || lastGridSplitterDistance != splitGrids.SplitterDistance
                || !publisherGridSettings.Equals(lastMainGridSettings)
                || !distributorGridSettings.Equals(lastDistributorGridSettings))
            {
                lastSplitterDistance =
                    Settings.Default.ServicesReplicationViewMainSplitter = splitContainer.SplitterDistance;
                lastGridSplitterDistance =
                    Settings.Default.ServicesReplicationViewGridSplitter = splitGrids.SplitterDistance;

                lastMainGridSettings =
                    Settings.Default.ServicesReplicationViewTopologyGrid = publisherGridSettings;
                lastDistributorGridSettings =
                    Settings.Default.ServicesReplicationViewDistributorGrid = distributorGridSettings;
            }
        }

        #region Refresh View

        public override void RefreshView()
        {
            if (HistoricalSnapshotDateTime == null)
            {
                ServicesReplicationView_Fill_Panel.Visible = true;
                base.RefreshView();
            }
            else
            {
                ServicesReplicationView_Fill_Panel.Visible = false;
                ApplicationController.Default.SetRefreshStatusText(Properties.Resources.HistoryModeStatusBarLabel);
            }
        }
        /// <summary>
        /// Fetches replication details
        /// </summary>
        /// <returns>Pair of PublisherDetails, DistributorDetails</returns>
        public object GetReplicationDetails()
        {
            IManagementService managementService =
                ManagementServiceHelper.GetDefaultService(Settings.Default.ActiveRepositoryConnection.ConnectionInfo);

            Pair<PublisherDetails, Object> configs = new Pair<PublisherDetails, Object>();

            configs.First = managementService.GetPublisherDetails(publisherConfiguration);

            if (!initialized && configs.First.Error == null)
            {
                currentSnapshot = configs.First;
                if (currentSnapshot.ReplicationStatus == ReplicationState.NotInstalled)
                {
                    isReplicationServer = false;
                    replicationStatusDisplay = STATUS_NOT_INSTALLED;
                }
                else if (currentSnapshot.ReplicationStatus == ReplicationState.Unknown)
                {
                    isReplicationServer = false;
                    replicationStatusDisplay = STATUS_UNKNOWN;
                }
                else if (currentSnapshot.ReplicationStatus == ReplicationState.CollectionDisabled)
                {
                    isReplicationServer = false;
                    replicationStatusDisplay = STATUS_COLLECTION_DISABLED;
                }
                else
                {
                    isReplicationServer = true;
                }
            }

            if (isReplicationServer)
            {
                //if (currentSnapshot != null)
                //{
                //    int? distributorId = null;
                //    foreach (MonitoredSqlServer server in ApplicationModel.Default.ActiveInstances)
                //    {
                //        if (server.InstanceName.Equals(currentSnapshot.Distributor, StringComparison.CurrentCultureIgnoreCase))
                //        {
                //            distributorId = server.Id;
                //            break;
                //        }
                //    }
                //    if (distributorId.HasValue)
                //    {
                //        configurationDistributors.PublisherName = currentSnapshot.ServerName;
                //        configurationDistributors.DistributionDatabase = currentSnapshot.DistributionDatabase;
                //        configurationDistributors.MonitoredServerId = distributorId.Value;

                //        configs.Second = (DistributorDetails)managementService.GetDistributorDetails(configurationDistributors);
                //    }
                //    else
                //    {
                //        configs.Second = string.Format(SERVER_NOT_MONITORED, currentSnapshot.Distributor);
                //    }
                //}
            }
            else
            {
                configs.Second = replicationStatusDisplay;
            }

            return configs;
        }

        public override object DoRefreshWork()
        {
            SortedDictionary<DateTime, ReplicationTrendDataSample> trendHistory = null;
            Pair<Dictionary<int, ReplicationSession>, SortedDictionary<DateTime, ReplicationTrendDataSample>> returnPair = new Pair<Dictionary<int, ReplicationSession>, SortedDictionary<DateTime, ReplicationTrendDataSample>>();
            
            //Gets the replication topology data
            Dictionary<int, ReplicationSession> sessions = RepositoryHelper.GetReplicationTopology(Settings.Default.ActiveRepositoryConnection.ConnectionInfo, instanceId);

            if (LastInstanceId != InstanceId)
            {
                publisherHistoryDataTable.Clear();
                distributorHistoryDataTable.Clear();
                LastInstanceId = InstanceId;
            }

            //Gets the historical contents of the charts if we dont have content already
            if (publisherHistoryDataTable.Rows.Count == 0 || distributorHistoryDataTable.Rows.Count == 0)
            {
                trendHistory = RepositoryHelper.GetReplicationChartHistory(Settings.Default.ActiveRepositoryConnection.ConnectionInfo, instanceId, ApplicationModel.Default.HistoryTimeValue.RealTimeMinutes);
            }
            else
            {
                DateTime mostRecent = DateTime.MinValue;
                DateTime recentScheduledRefresh = DateTime.MinValue;
                bool blnFetchHistory = false;

                DataRow[] found = publisherHistoryDataTable.Select("UTCCollectionDateTime = max(UTCCollectionDateTime)");
                
                for (int i = 0; i < found.Length;)
                {
                    DateTime.TryParse(found[i]["UtcCollectionDateTime"].ToString(),out mostRecent);
                    break;//only need the first one if there are multiple
                }
                
                MonitoredSqlServerStatus status = ApplicationModel.Default.GetInstanceStatus(instanceId);
                
                if(status != null)
                {
                    if(status.LastScheduledRefreshTime.HasValue)
                        recentScheduledRefresh = (DateTime)status.LastScheduledRefreshTime;
                }
                //if we have no data (statement below will never fire because mostRecent is not nullable)
                //if (mostRecent == null) blnFetchHistory = true;
                //-ve means earlier, +ve means later
                //if history contains more recent data
                if (mostRecent.ToLocalTime().CompareTo(recentScheduledRefresh) < 0) blnFetchHistory = true;
                
                if(blnFetchHistory)
                {
                    trendHistory = RepositoryHelper.GetReplicationChartHistory(Settings.Default.ActiveRepositoryConnection.ConnectionInfo, instanceId, ApplicationModel.Default.HistoryTimeValue.RealTimeMinutes);
                }
            }

            returnPair.First = sessions;
            returnPair.Second = trendHistory;

            return returnPair;
        }

        public override void HandleBackgroundWorkerError(Exception e)
        {

            lblTopologyGridStatus.Text =
                distributorTransactionsGridStatusLabel.Text = UNABLE_TO_UPDATE;
            base.HandleBackgroundWorkerError(e);
        }

        public override void UpdateData(object data)
        {
            Dictionary<int, ReplicationSession> sessions = null;
            
            //Image publisherStatusImage = null;
            //Image distributorStatusImage = null;
            //Image subscriberStatusImage = null;
            bool blnReplicationDisabled = false;

            UpdateOperationalStatus(ApplicationModel.Default.ActiveInstances[instanceId]);
            
            MonitoredSqlServerStatus status = ApplicationModel.Default.GetInstanceStatus(InstanceId);
            
            string trendHeading = status != null ?
                string.Format("Replication Trends as of {0}", status.LastScheduledRefreshTime) :
                "Replication Trends";
            
            if(operationalStatusPanel.Visible)
            {
                blnReplicationDisabled = true;
                trendHeading = "Replication Monitor Disabled";
            }
            
            lblReplicationTrends.Text = trendHeading;

            if (data != null && data is Pair<Dictionary<int, ReplicationSession>, SortedDictionary<DateTime, ReplicationTrendDataSample>>)
            {
                if (!blnReplicationDisabled)
                    sessions = ((Pair<Dictionary<int, ReplicationSession>, SortedDictionary<DateTime, ReplicationTrendDataSample>>)data).First;

                if (sessions != null)
                {
                    lock (updateLock)
                    {
                        #region Table Headings
                        //topologyDataTable.Columns.Add("Articles", typeof(int));
                        //topologyDataTable.Columns.Add("Subscription Status", typeof(int));
                        //topologyDataTable.Columns.Add("Publisher", typeof(string));
                        //topologyDataTable.Columns.Add("Distributor", typeof(string));
                        //topologyDataTable.Columns.Add("Subscriber", typeof(string));
                        //topologyDataTable.Columns.Add("Publication", typeof(string));
                        //topologyDataTable.Columns.Add("PublicationDescription", typeof(string));
                        //topologyDataTable.Columns.Add("Replication Type", typeof(int));
                        //topologyDataTable.Columns.Add("PublisherServerID", typeof(int));
                        //topologyDataTable.Columns.Add("DistributorServerID", typeof(int));
                        //topologyDataTable.Columns.Add("SubscriberServerID", typeof(int));
                        //topologyDataTable.Columns.Add("PublisherInstance", typeof(string));
                        //topologyDataTable.Columns.Add("PublisherDB", typeof(string));
                        //topologyDataTable.Columns.Add("SubscriberInstance", typeof(string));
                        //topologyDataTable.Columns.Add("SubscriberDB", typeof(string));
                        //topologyDataTable.Columns.Add("DistributorInstance", typeof(string));
                        //topologyDataTable.Columns.Add("DistributorDB", typeof(string));
                        #endregion

                        topologyDataTable.BeginLoadData();

                        // remove any databases that have been deleted
                        List<DataRow> deleteRows = new List<DataRow>();

                        foreach (DataRow row in topologyDataTable.Rows)
                        {
                            //int key = (session.PublisherInstance + "." + session.PublisherDB + "." + session.Publication + "." + (session.SubscriberDB??"")).GetHashCode();
                            //row["Publisher"] contains both the instance and the db
                            if (!sessions.ContainsKey(
                                (row["Publisher"] + "." 
                                + row["Publication"] + "."
                                + (row["SubscriberInstance"] ?? "") + "."
                                + (row["SubscriberDB"] ?? "")).GetHashCode()))
                            {
                                deleteRows.Add(row);
                            }
                        }
                        
                        foreach (DataRow row in deleteRows)
                        {
                            topologyDataTable.Rows.Remove(row);
                        }

                        MonitoredSqlServerStatus publisherStatus = null;
                        MonitoredSqlServerStatus distributorStatus = null;
                        MonitoredSqlServerStatus subscriberStatus = null;
                        
                        //now update any matching databases or add new ones
                        foreach (ReplicationSession session in sessions.Values)
                        {
                            string publisherStatusKey = null;
                            string distributorStatusKey = null;
                            string subscriberStatusKey = null;

                            int publisherStatusint = 5;
                            int distributorStatusint = 5;
                            int subscriberStatusint = 5;

                            distributorStatus = ApplicationModel.Default.GetInstanceStatus(session.DistributorSQLServerID ?? -1);
                            publisherStatus = ApplicationModel.Default.GetInstanceStatus(session.PublisherSQLServerID ?? -1);
                            subscriberStatus = ApplicationModel.Default.GetInstanceStatus(session.SubscriberSQLServerID ?? -1);

                            
                            publisherStatusKey = (publisherStatus != null)? publisherStatus.ServerImageKey:"Server";
                            distributorStatusKey = (distributorStatus != null) ? distributorStatus.ServerImageKey : "Server";
                            subscriberStatusKey = (subscriberStatus != null) ? subscriberStatus.ServerImageKey : "Server";

                            publisherStatusint = GetServerStatusAsInt(publisherStatusKey);
                            distributorStatusint = GetServerStatusAsInt(distributorStatusKey);
                            subscriberStatusint = GetServerStatusAsInt(subscriberStatusKey);

                            intSelectedServerRoleBitMask = intSelectedServerRoleBitMask | (session.PublisherSQLServerID == instanceId ? 1 : 0);
                            intSelectedServerRoleBitMask = intSelectedServerRoleBitMask | (session.DistributorSQLServerID == instanceId ? 2 : 0);
                            intSelectedServerRoleBitMask = intSelectedServerRoleBitMask | (session.SubscriberSQLServerID == instanceId ? 4 : 0);

                            //distributorStatusImage = distributorStatus != null ? 
                            //    distributorStatus.ServerImage : Properties.Resources.Server;
                            
                            //publisherStatusImage = publisherStatus != null
                            //    ? publisherStatus.ServerImage: Properties.Resources.Server;
                            
                            //subscriberStatusImage = subscriberStatus != null
                            //    ? subscriberStatus.ServerImage: Properties.Resources.Server;

                            topologyDataTable.LoadDataRow(
                            new object[]
                                {
                                    session.ArticleCount,
                                    publisherStatusint,
                                    session.PublisherInstanceAndDB,
                                    distributorStatusint,
                                    session.DistributorInstanceAndDB,
                                    subscriberStatusint,
                                    session.SubscriberInstanceAndDB,
                                    session.Publication,
                                    session.PublicationDescription,
                                    session.ReplicationType,
                                    session.PublisherSQLServerID,
                                    session.DistributorSQLServerID,
                                    session.SubscriberSQLServerID,
                                    session.PublisherInstance,
                                    session.PublisherDB,
                                    session.SubscriberInstance,
                                    session.SubscriberDB,
                                    session.DistributorInstance,
                                    session.DistributorDB
                                }, true);
                            
                            //if we have no previous values collection then create one
                            if (previousSubscribedCollection == null)
                                previousSubscribedCollection = new Dictionary<int, Pair<DateTime, long>>();

                            int intHash = (session.PublisherInstanceAndDB + "."
                                + session.Publication + "." 
                                + session.SubscriberInstance??"" + "."
                                + session.SubscriberDB??"").GetHashCode();

                            //do we have an entry for this session? update else add
                            if(previousSubscribedCollection.ContainsKey(intHash))
                            {
                                //if there is an entry do an update (if this is more recent) 
                                if (session.LastSnapshotDateTime.CompareTo(previousSubscribedCollection[intHash].First) > 0)
                                {
                                    previousSubscribedCollection[intHash] = new Pair<DateTime, long>(session.LastSnapshotDateTime, session.SubscribedTransactions);
                                }
                            }
                            else
                            {
                                previousSubscribedCollection.Add(intHash, new Pair<DateTime, long>(session.LastSnapshotDateTime, session.SubscribedTransactions));
                            }
                        }
                        lblTopologyGridStatus.Hide();
                        topologyDataTable.EndLoadData();

                        string roleDescription = "";
                        
                        if ((intSelectedServerRoleBitMask & 1) == 1) roleDescription = roleDescription + "Publisher, ";
                        if ((intSelectedServerRoleBitMask & 2) == 2) roleDescription = roleDescription + "Distributor, ";
                        if ((intSelectedServerRoleBitMask & 4) == 4) roleDescription = roleDescription + "Subscriber, ";
                        
                        if (intSelectedServerRoleBitMask == 0)
                        {
                            roleDescription = roleDescription + "No Replication, ";
                            ReplicationDetailsTabControl.Enabled = false;
                            lblTopologyGridStatus.Text = NO_ITEMS;
                            lblTopologyGridStatus.Show();
                            lblTopologyGridStatus.BringToFront();
                        }
                        else
                        {
                            ReplicationDetailsTabControl.Enabled = true;
                        }

                        if(roleDescription.Length > 2) roleDescription = roleDescription.Substring(0, roleDescription.Length-2);

                        topologyHeaderStripLabel.Text = "Replication Topology" + " (Local roles - " + roleDescription + ")";

                        grdReplicationTopology.DisplayLayout.Bands[0].SortedColumns.RefreshSort(true);
                        grdReplicationTopology.Visible = true;

                        SetServerStatusTooltips(grdReplicationTopology);

                        if (!initialized)
                        {
                            if (lastMainGridSettings != null)
                            {
                                GridSettings.ApplySettingsToGrid(lastMainGridSettings, grdReplicationTopology);

                                initialized = true;
                            }
                            else if (sessions.Count > 0)
                            {
                                foreach (UltraGridColumn column in grdReplicationTopology.DisplayLayout.Bands[0].Columns)
                                {
                                    column.PerformAutoResize(PerformAutoSizeType.AllRowsInBand, true);
                                    column.Width = Math.Min(column.Width, grdReplicationTopology.Width / 2);
                                }

                                initialized = true;
                            }

                            if (grdReplicationTopology.Rows.Count > 0 && grdReplicationTopology.Selected.Rows.Count == 0)
                            {
                                grdReplicationTopology.Rows[0].Selected = true;
                            }
                        }

                        ApplicationController.Default.SetCustomStatus(String.Format("Replication Session{0}:  {1}",
                                    topologyDataTable.Rows.Count == 1 ? string.Empty : "s",
                                    topologyDataTable.Rows.Count)
                            );
                    }
                }
                else //session is null
                {
                    topologyHeaderStripLabel.Text = "Replication Topology";
                    topologyDataTable.Clear();
                    lblTopologyGridStatus.Text = NO_ITEMS;
                    lblTopologyGridStatus.Show();
                    lblTopologyGridStatus.BringToFront();
                    grdReplicationTopology.Visible = false;
                    distributorTransactionsGridStatusLabel.Text = SELECT_DATABASE;
                    distributorTransactionsGrid.Visible = false;
                    ApplicationController.Default.ClearCustomStatus();
                }
            }


            if (((Pair<Dictionary<int, ReplicationSession>, SortedDictionary<DateTime, ReplicationTrendDataSample>>)data).Second != null)
            {
                lock (updateLock)
                {
                    PopulateCharts(((Pair<Dictionary<int, ReplicationSession>, SortedDictionary<DateTime, ReplicationTrendDataSample>>)data).Second, intSelectedServerRoleBitMask);

                }
            }
            
            //Populate details for the selected replication session
            if (grdReplicationTopology.Selected.Rows.Count > 0)
            {
                // There is only one selected row which we iterate through
                foreach (UltraGridRow row in grdReplicationTopology.Selected.Rows)
                {
                    //if the selected row is not a data row then we do not want to process further
                    if (!row.IsDataRow) continue;
                    StartDistributionDetailsRefresh(row);
                }
            }
            else
            {
                lblTransactionalPublisherStatus.Text = "No Replication selected.";
                lblTransactionalDistributorStatus.Text = "No Replication selected.";
                lblTransactionalSubscriberStatus.Text = "No Replication selected.";
                lblTransactionalPublisherStatus.Visible = true;
                lblTransactionalDistributorStatus.Visible = true;
                lblTransactionalSubscriberStatus.Visible = true;
                grdPublisher.Visible = false;
                grdDistributor.Visible = false;
                grdSubscriber.Visible = false;
            }

            ApplicationController.Default.OnRefreshActiveViewCompleted(
                        new RefreshActiveViewCompletedEventArgs(DateTime.Now));
        }
        /// <summary>
        /// Groom old chart data
        /// </summary>
        private void GroomChartData()
        {
            DateTime groomThreshold =
                DateTime.Now.Subtract(TimeSpan.FromMinutes(Settings.Default.RealTimeChartHistoryLimitInMinutes));

            if (publisherHistoryDataTable != null)
            {
                DataRow[] groomedRows = publisherHistoryDataTable.Select(string.Format("UTCCollectionDateTime < #{0}#", groomThreshold.ToString(CultureInfo.InvariantCulture)));

                foreach (DataRow row in groomedRows)
                {
                    row.Delete();
                }
                publisherHistoryDataTable.AcceptChanges();
            }
            if (distributorHistoryDataTable != null)
            {

                DataRow[] groomedRows = distributorHistoryDataTable.Select(string.Format("UTCCollectionDateTime < #{0}#", groomThreshold.ToString(CultureInfo.InvariantCulture)));
                foreach (DataRow row in groomedRows)
                {
                    row.Delete();
                }
                distributorHistoryDataTable.AcceptChanges();
            }

        }

        /// <summary>
        /// Populate the charts with history data
        /// </summary>
        /// <param name="trendHistory"></param>
        /// <param name="intReplRoleStatus"></param>
        private void PopulateCharts(SortedDictionary<DateTime, ReplicationTrendDataSample> trendHistory, int intReplRoleStatus)
        {
            if(operationalStatusPanel.Visible)
            {
                lblNonDistributedCountChartStatus.Text = STATUS_COLLECTION_DISABLED;
                lblNonDistributedCountChartStatus.Visible = true;
                trendNonDistributedQueue.Visible = false;
                lblNonDistributedTimeChartStatus.Text = STATUS_COLLECTION_DISABLED;
                lblNonDistributedTimeChartStatus.Visible = true;
                trendNonDistributedTime.Visible = false;

                lblNonSubscribedCountChartStatus.Text = STATUS_COLLECTION_DISABLED;
                lblNonSubscribedCountChartStatus.Visible = true;
                trendNonSubscribedQueue.Visible = false;
                lblNonSubscribedTimeChartStatus.Text = STATUS_COLLECTION_DISABLED;
                lblNonSubscribedTimeChartStatus.Visible = true;
                trendNonSubscribedTime.Visible = false;
                return;
            }

            foreach (ReplicationTrendDataSample sample in trendHistory.Values)
            {
                DateTime localTime = DateTime.MinValue;
                if (sample.SampleTime.HasValue)
                    localTime = sample.SampleTime.Value.ToLocalTime();
                publisherHistoryDataTable.LoadDataRow(new object[]{
                localTime,
                sample.NonDistributedLatency,
                sample.NonDistributedCount}, true);
                
                //publisherHistoryDataTable.LoadDataRow(new object[]{
                //localTime,
                //sample.NonDistributedLatency.HasValue?sample.NonDistributedLatency:null,
                //sample.NonDistributedCount.HasValue?sample.NonDistributedCount:null},true);
                distributorHistoryDataTable.LoadDataRow(new object[]{
                localTime,
                sample.NonSubscribedLatency,
                sample.SubscribedCount,
                sample.NonSubscribedCount}, true);

                //distributorHistoryDataTable.LoadDataRow(new object[]{
                //localTime,
                //sample.NonSubscribedLatency.HasValue?sample.NonSubscribedLatency:null,
                //sample.SubscribedCount.HasValue?sample.SubscribedCount:null,
                //sample.NonSubscribedCount.HasValue?sample.NonSubscribedCount:null}, true);
            }

            GroomChartData();

            DateTime viewFilter =
                DateTime.Now.Subtract(TimeSpan.FromMinutes(ApplicationModel.Default.HistoryTimeValue.RealTimeMinutes));

            if (publisherHistoryDataTable!= null)
            {
                publisherHistoryDataTable.DefaultView.RowFilter = string.Format("UTCCollectionDateTime > #{0}#", viewFilter.ToString(CultureInfo.InvariantCulture));
            }
            if (distributorHistoryDataTable != null)
            {
                distributorHistoryDataTable.DefaultView.RowFilter = string.Format("UTCCollectionDateTime > #{0}#", viewFilter.ToString(CultureInfo.InvariantCulture));
            }

            trendNonDistributedQueue.DataSource = publisherHistoryDataTable;
            trendNonDistributedTime.DataSource = publisherHistoryDataTable;

            trendNonSubscribedQueue.DataSource = distributorHistoryDataTable;
            trendNonSubscribedTime.DataSource = distributorHistoryDataTable;

            if ((intReplRoleStatus & 1) != 1)
            {
                lblNonDistributedCountChartStatus.Text = string.Format(SERVER_NOTFULFILLING_ROLE, "publisher");
                lblNonDistributedCountChartStatus.Visible = true;
                trendNonDistributedQueue.Visible = false;
                lblNonDistributedTimeChartStatus.Text = string.Format(SERVER_NOTFULFILLING_ROLE, "publisher");
                lblNonDistributedTimeChartStatus.Visible = true;
                trendNonDistributedTime.Visible = false;
            }
            else
            {
                lblNonDistributedCountChartStatus.Visible = false;
                trendNonDistributedQueue.Visible = true;
                lblNonDistributedTimeChartStatus.Visible = false;
                trendNonDistributedTime.Visible = true;
            }
            if ((intReplRoleStatus & 2) != 2)
            {
                lblNonSubscribedCountChartStatus.Text = string.Format(SERVER_NOTFULFILLING_ROLE, "distributor");
                lblNonSubscribedCountChartStatus.Visible = true;
                trendNonSubscribedQueue.Visible = false;
                lblNonSubscribedTimeChartStatus.Text = string.Format(SERVER_NOTFULFILLING_ROLE, "distributor");
                lblNonSubscribedTimeChartStatus.Visible = true;
                trendNonSubscribedTime.Visible = false;
            }
            else
            {
                lblNonSubscribedCountChartStatus.Visible = false;
                trendNonSubscribedQueue.Visible = true;
                lblNonSubscribedTimeChartStatus.Visible = false;
                trendNonSubscribedTime.Visible = true;
            }
        }

        private void UpdateCellColor(Metric metric, AlertConfiguration alertConfig, UltraGridRow gridRow, string columnName)
        {
            AlertConfigurationItem alertConfigItem = alertConfig[metric, String.Empty]; // Will need to update this if this metric ever supports multi-thresholds
            if (alertConfigItem != null)
            {
                UltraGridCell cell = gridRow.Cells[columnName];
                if (cell != null)
                {
                    DataRowView dataRowView = (DataRowView)gridRow.ListObject;
                    DataRow dataRow = dataRowView.Row;
                    if (dataRow.IsNull(columnName))
                    {
                        cell.Appearance.ResetBackColor();
                    }
                    else
                    {
                        object value = dataRow[columnName];
                        if (value is TimeSpan && columnName.Equals(columnName))
                            value = ((TimeSpan)value).TotalSeconds;

                        switch (alertConfigItem.GetSeverity((IComparable)value))
                        {
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

        #endregion

        public void ShowFilter()
        {
            ReplicationFilter selectFilter = new ReplicationFilter();
            selectFilter.UpdateValues(replicationFilter);
            GenericFilterDialog dialog = new GenericFilterDialog(selectFilter);

            DialogResult result = dialog.ShowDialog(this);
            if (result == DialogResult.OK)
            {
                replicationFilter.UpdateValues(selectFilter);
                configurationDistributors.FilterTimeSpan = replicationFilter.FilterTimeSpan;

                ApplicationController.Default.ActiveView.CancelRefresh();
                ApplicationController.Default.RefreshActiveView();
            }
            if (FilterChanged != null)
            {
                // This must be called regardless of the result because cancel will change button state
                FilterChanged(this, EventArgs.Empty);
            }
        }
        /// <summary>
        /// show or hide the replication topology group by box
        /// </summary>
        public void ToggleTopologyGroupByBox()
        {
            TopologyGridGroupByBoxVisible = !TopologyGridGroupByBoxVisible;
        }
        /// <summary>
        /// Show or hide the Distribution queue group by box
        /// </summary>
        public void ToggleDistributorGroupByBox()
        {
            DistributorGridGroupByBoxVisible = !DistributorGridGroupByBoxVisible;
        }
        /// <summary>
        /// show or hide the replication graphs
        /// </summary>
        public void ToggleReplicationGraphsVisible()
        {
            ReplicationGraphsVisible = !ReplicationGraphsVisible;
        }

        #endregion

        #region helpers

        private static Control GetFocusedControl(ControlCollection controls)
        {
            Control focusedControl = null;

            foreach (Control control in controls)
            {
                if (control.Focused)
                {
                    focusedControl = control;
                }
                else if (control.ContainsFocus)
                {
                    return GetFocusedControl(control.Controls);
                }
            }

            return focusedControl ?? controls[0];
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
                foreach (MonitoredSqlServer tmpServer in ApplicationModel.Default.ActiveInstances)
                {
                    if (tmpServer.InstanceName.ToLower().Equals(ServerName.ToLower())) return tmpServer.Id;
                }
                return -1;
            }
            catch
            {
                return -1;
            }
        }

        #region DataTables
        private void InitializePublisherDataTable()
        {
            publisherDataTable = new DataTable();
            publisherDataTable.Columns.Add("Publication Name", typeof(string));
            publisherDataTable.Columns.Add("Description", typeof(string));
            publisherDataTable.Columns.Add("Non-Distributed Trans", typeof(int));
            publisherDataTable.Columns.Add("Replication Rate", typeof(string));
            publisherDataTable.Columns.Add("Replication Latency", typeof(string));

            publisherDataTable.CaseSensitive = true;

            publisherDataTable.DefaultView.Sort = "[Publication Name]";

            grdPublisher.DataSource = publisherDataTable;

            grdPublisher.DisplayLayout.Bands[0].Columns["Publication Name"].CellActivation = Activation.NoEdit;
            grdPublisher.DisplayLayout.Bands[0].Columns["Description"].CellActivation = Activation.NoEdit;
            grdPublisher.DisplayLayout.Bands[0].Columns["Non-Distributed Trans"].CellActivation = Activation.NoEdit;
            grdPublisher.DisplayLayout.Bands[0].Columns["Replication Rate"].CellActivation = Activation.NoEdit;
            grdPublisher.DisplayLayout.Bands[0].Columns["Replication Latency"].CellActivation = Activation.NoEdit;

            grdPublisher.DisplayLayout.Override.HeaderClickAction = HeaderClickAction.Select;

            grdPublisher.DisplayLayout.Bands[0].Columns["Publication Name"].SortIndicator = SortIndicator.None;
            grdPublisher.DisplayLayout.Bands[0].Columns["Description"].SortIndicator = SortIndicator.None;
            grdPublisher.DisplayLayout.Bands[0].Columns["Non-Distributed Trans"].SortIndicator = SortIndicator.None;
            grdPublisher.DisplayLayout.Bands[0].Columns["Replication Rate"].SortIndicator = SortIndicator.None;
            grdPublisher.DisplayLayout.Bands[0].Columns["Replication Latency"].SortIndicator = SortIndicator.None;

            grdPublisher.DisplayLayout.Bands[0].Columns["Non-Distributed Trans"].Format = "###,###,##0";
            //grdPublisher.DisplayLayout.Bands[0].Columns["Replication Rate"].Format = "###,###,##0";
            //grdPublisher.DisplayLayout.Bands[0].Columns["Non-Distributed Trans"].Format = "###,###,##0";
            //grdPublisher.DisplayLayout.Bands[0].Columns["Replication Latency"].Format = "###,###,##0.0";

        }
        private void InitializeMergeDistributionDataTables()
        {
            distributorMergeDataTable = new DataTable();
            distributorMergeDataTable.CaseSensitive = true;

            subscriberMergeDataTable = new DataTable();
            subscriberMergeDataTable.CaseSensitive = true;

            publisherMergeDataTable = new DataTable();
            publisherMergeDataTable.CaseSensitive = true;

            distributorMergeDataTable.Columns.Add("Agent Name", typeof(string));
            distributorMergeDataTable.Columns.Add("Last Action", typeof(string));
            distributorMergeDataTable.Columns.Add("Delivery Rate", typeof(float));
            distributorMergeDataTable.DefaultView.Sort = "Agent Name";

            publisherMergeDataTable.Columns.Add("Publisher Inserted", typeof(int));
            publisherMergeDataTable.Columns.Add("Publisher Updated", typeof(int));
            publisherMergeDataTable.Columns.Add("Publisher Deleted", typeof(int));
            publisherMergeDataTable.Columns.Add("Publisher Conflicts", typeof(int));

            subscriberMergeDataTable.Columns.Add("Subscriber Inserted", typeof(int));
            subscriberMergeDataTable.Columns.Add("Subscriber Updated", typeof(int));
            subscriberMergeDataTable.Columns.Add("Subscriber Deleted", typeof(int));
            subscriberMergeDataTable.Columns.Add("Subscriber Conflicts", typeof(int));

            grdDistributorMerge.DataSource = distributorMergeDataTable;
            grdMergeSubscriber.DataSource = subscriberMergeDataTable;
            grdMergePublisher.DataSource = publisherMergeDataTable;

            grdDistributorMerge.DisplayLayout.Bands[0].Columns["Agent Name"].CellActivation = Activation.NoEdit;
            grdDistributorMerge.DisplayLayout.Bands[0].Columns["Last Action"].CellActivation = Activation.NoEdit;
            grdDistributorMerge.DisplayLayout.Bands[0].Columns["Delivery Rate"].CellActivation = Activation.NoEdit;

            grdMergePublisher.DisplayLayout.Bands[0].Columns["Publisher Inserted"].CellActivation = Activation.NoEdit;
            grdMergePublisher.DisplayLayout.Bands[0].Columns["Publisher Updated"].CellActivation = Activation.NoEdit;
            grdMergePublisher.DisplayLayout.Bands[0].Columns["Publisher Deleted"].CellActivation = Activation.NoEdit;
            grdMergePublisher.DisplayLayout.Bands[0].Columns["Publisher Conflicts"].CellActivation = Activation.NoEdit;

            grdMergeSubscriber.DisplayLayout.Bands[0].Columns["Subscriber Inserted"].CellActivation = Activation.NoEdit;
            grdMergeSubscriber.DisplayLayout.Bands[0].Columns["Subscriber Updated"].CellActivation = Activation.NoEdit;
            grdMergeSubscriber.DisplayLayout.Bands[0].Columns["Subscriber Deleted"].CellActivation = Activation.NoEdit;
            grdMergeSubscriber.DisplayLayout.Bands[0].Columns["Subscriber Conflicts"].CellActivation = Activation.NoEdit;

            grdDistributorMerge.DisplayLayout.Override.HeaderClickAction = HeaderClickAction.Select;
            grdDistributorMerge.DisplayLayout.Bands[0].Columns["Agent Name"].SortIndicator = SortIndicator.None;
            grdDistributorMerge.DisplayLayout.Bands[0].Columns["Last Action"].SortIndicator = SortIndicator.None;
            grdDistributorMerge.DisplayLayout.Bands[0].Columns["Delivery Rate"].SortIndicator = SortIndicator.None;

            grdMergePublisher.DisplayLayout.Override.HeaderClickAction = HeaderClickAction.Select;
            grdMergePublisher.DisplayLayout.Bands[0].Columns["Publisher Inserted"].SortIndicator = SortIndicator.None;
            grdMergePublisher.DisplayLayout.Bands[0].Columns["Publisher Updated"].SortIndicator = SortIndicator.None;
            grdMergePublisher.DisplayLayout.Bands[0].Columns["Publisher Deleted"].SortIndicator = SortIndicator.None;
            grdMergePublisher.DisplayLayout.Bands[0].Columns["Publisher Conflicts"].SortIndicator = SortIndicator.None;

            grdMergeSubscriber.DisplayLayout.Override.HeaderClickAction = HeaderClickAction.Select;
            grdMergeSubscriber.DisplayLayout.Bands[0].Columns["Subscriber Inserted"].SortIndicator = SortIndicator.None;
            grdMergeSubscriber.DisplayLayout.Bands[0].Columns["Subscriber Updated"].SortIndicator = SortIndicator.None;
            grdMergeSubscriber.DisplayLayout.Bands[0].Columns["Subscriber Deleted"].SortIndicator = SortIndicator.None;
            grdMergeSubscriber.DisplayLayout.Bands[0].Columns["Subscriber Conflicts"].SortIndicator = SortIndicator.None;

            grdDistributorMerge.DisplayLayout.Bands[0].Columns["Delivery Rate"].Format = "###,###,##0.0";

            grdMergePublisher.DisplayLayout.Bands[0].Columns["Publisher Inserted"].Format = "###,###,##0";
            grdMergePublisher.DisplayLayout.Bands[0].Columns["Publisher Updated"].Format = "###,###,##0";
            grdMergePublisher.DisplayLayout.Bands[0].Columns["Publisher Deleted"].Format = "###,###,##0";
            grdMergePublisher.DisplayLayout.Bands[0].Columns["Publisher Conflicts"].Format = "###,###,##0";

            grdMergeSubscriber.DisplayLayout.Bands[0].Columns["Subscriber Inserted"].Format = "###,###,##0";
            grdMergeSubscriber.DisplayLayout.Bands[0].Columns["Subscriber Updated"].Format = "###,###,##0";
            grdMergeSubscriber.DisplayLayout.Bands[0].Columns["Subscriber Deleted"].Format = "###,###,##0";
            grdMergeSubscriber.DisplayLayout.Bands[0].Columns["Subscriber Conflicts"].Format = "###,###,##0";
        }

        private void InitializeDistributorDataTable()
        {
            distributorDataTable = new DataTable();
            distributorDataTable.CaseSensitive = true;
            

            #region Heading
        //if (!dataReader.IsDBNull(0)) dr["SubscriberName"] = dataReader.GetString(0);
            //if (!dataReader.IsDBNull(1)) dr["MergeSubscriptionstatus"] = dataReader.GetInt32(1);
            //if (!dataReader.IsDBNull(2)) dr["SubscriberDB"] = dataReader.GetString(2);
            //if (!dataReader.IsDBNull(3)) dr["Type"] = dataReader.GetInt32(3);
            //if (!dataReader.IsDBNull(4)) dr["Agent Name"] = dataReader.GetString(4);
            //if (!dataReader.IsDBNull(5)) dr["Last Action"] = dataReader.GetString(5);
            //if (!dataReader.IsDBNull(9)) dr["Delivery Rate"] = dataReader.GetDouble(9);
            //if (!dataReader.IsDBNull(10)) dr["Publisher Insertcount"] = dataReader.GetInt32(10);
            //if (!dataReader.IsDBNull(11)) dr["Publisher Updatecount"] = dataReader.GetInt32(11);
            //if (!dataReader.IsDBNull(12)) dr["Publisher Deletecount"] = dataReader.GetInt32(12);
            //if (!dataReader.IsDBNull(13)) dr["Publisher Conflicts"] = dataReader.GetInt32(13);
            //if (!dataReader.IsDBNull(14)) dr["Subscriber Insertcount"] = dataReader.GetInt32(14);
            //if (!dataReader.IsDBNull(15)) dr["Subscriber Updatecount"] = dataReader.GetInt32(15);
            //if (!dataReader.IsDBNull(16)) dr["Subscriber Deletecount"] = dataReader.GetInt32(16);
            //if (!dataReader.IsDBNull(17)) dr["Subscriber Conflicts"] = dataReader.GetInt32(17);
#endregion

            distributorDataTable.Columns.Add("Subscribed", typeof(int));
            distributorDataTable.Columns.Add("Non-subscribed", typeof(int));
            distributorDataTable.Columns.Add("Subscription Latency", typeof(float));
            
            distributorDataTable.Columns.Add("Subscription Rate", typeof(string));
            distributorDataTable.Columns.Add("Time Until Synchronized", typeof(string));

            grdDistributor.DataSource = distributorDataTable;
            grdDistributor.DisplayLayout.Bands[0].Columns["Subscribed"].CellActivation = Activation.NoEdit;
            grdDistributor.DisplayLayout.Bands[0].Columns["Non-subscribed"].CellActivation = Activation.NoEdit;
            grdDistributor.DisplayLayout.Bands[0].Columns["Subscription Latency"].CellActivation = Activation.NoEdit;
            grdDistributor.DisplayLayout.Bands[0].Columns["Subscription Rate"].CellActivation = Activation.NoEdit;
            grdDistributor.DisplayLayout.Bands[0].Columns["Time Until Synchronized"].CellActivation = Activation.NoEdit;

            grdDistributor.DisplayLayout.Override.HeaderClickAction = HeaderClickAction.Select;

            grdDistributor.DisplayLayout.Bands[0].Columns["Subscribed"].SortIndicator = SortIndicator.None;
            grdDistributor.DisplayLayout.Bands[0].Columns["Non-subscribed"].SortIndicator = SortIndicator.None;
            grdDistributor.DisplayLayout.Bands[0].Columns["Subscription Latency"].SortIndicator = SortIndicator.None;
            grdDistributor.DisplayLayout.Bands[0].Columns["Subscription Rate"].SortIndicator = SortIndicator.None;
            grdDistributor.DisplayLayout.Bands[0].Columns["Time Until Synchronized"].SortIndicator = SortIndicator.None;

            grdDistributor.DisplayLayout.Bands[0].Columns["Subscribed"].Format = "###,###,##0";
            grdDistributor.DisplayLayout.Bands[0].Columns["Non-subscribed"].Format = "###,###,##0";
            grdDistributor.DisplayLayout.Bands[0].Columns["Subscription Latency"].Format = "###,###,##0.0 s";

            //I am hiding this for now because it is an absolute value and not a useful metric compared to the rate
            #if DEBUG
            grdDistributor.DisplayLayout.Bands[0].Columns["Subscribed"].Hidden = false;
            #else
            grdDistributor.DisplayLayout.Bands[0].Columns["Subscribed"].Hidden = true;
            #endif

            grdDistributor.DisplayLayout.Bands[0].Columns["Subscribed"].ExcludeFromColumnChooser = ExcludeFromColumnChooser.True;

        }

        private void InitializeSubscriberDataTable()
        {
            subscriberDataTable = new DataTable();
            subscriberDataTable.Columns.Add("Subscription Type", typeof(int));
            subscriberDataTable.Columns.Add("Last Updated Time", typeof(string));
            subscriberDataTable.Columns.Add("Last Sync Status", typeof(int));
            subscriberDataTable.Columns.Add("Last Sync Summary", typeof(string));
            subscriberDataTable.Columns.Add("Last Sync Time", typeof(string));

            grdSubscriber.DataSource = subscriberDataTable;

            grdSubscriber.DisplayLayout.Bands[0].Columns["Last Updated Time"].SortIndicator = SortIndicator.None;
            grdSubscriber.DisplayLayout.Bands[0].Columns["Last Sync Status"].SortIndicator = SortIndicator.None;
            grdSubscriber.DisplayLayout.Bands[0].Columns["Last Sync Summary"].SortIndicator = SortIndicator.None;
            grdSubscriber.DisplayLayout.Bands[0].Columns["Last Sync Time"].SortIndicator = SortIndicator.None;
            grdSubscriber.DisplayLayout.Bands[0].Columns["Subscription Type"].SortIndicator = SortIndicator.None;

            grdSubscriber.DisplayLayout.Override.HeaderClickAction = HeaderClickAction.Select;

            subscriberDataTable.CaseSensitive = true;

            subscriberDataTable.DefaultView.Sort = "[Last Updated Time]";

            
        }

        //populate the value list and associate it with the releavnt columns
        private void CreateServerStatusValueList()
        {
            grdReplicationTopology.DisplayLayout.ValueLists["ServerStatusValueList"].ValueListItems.Clear();
            
            ValueListItem listItem = new ValueListItem(0, "Maintenance Mode");
            //ValueListItem listItem = new ValueListItem("ServerMaintenanceMode", null);
            listItem.Appearance.Image = global::Idera.SQLdm.DesktopClient.Properties.Resources.ServerMaintenanceMode;
            grdReplicationTopology.DisplayLayout.ValueLists["ServerStatusValueList"].ValueListItems.Add(listItem);

            listItem = new ValueListItem(1, "Critical");
            //listItem = new ValueListItem("ServerCritical", "");
            listItem.Appearance.Image = global::Idera.SQLdm.DesktopClient.Properties.Resources.ServerCritical;
            grdReplicationTopology.DisplayLayout.ValueLists["ServerStatusValueList"].ValueListItems.Add(listItem);
            
            listItem = new ValueListItem(2, "Warning");
            //listItem = new ValueListItem("ServerWarning", "");
            listItem.Appearance.Image = global::Idera.SQLdm.DesktopClient.Properties.Resources.ServerWarning;
            grdReplicationTopology.DisplayLayout.ValueLists["ServerStatusValueList"].ValueListItems.Add(listItem);
            
            listItem = new ValueListItem(3, "OK");
            //listItem = new ValueListItem("ServerOK", "");
            listItem.Appearance.Image = global::Idera.SQLdm.DesktopClient.Properties.Resources.ServerOK16x16;
            grdReplicationTopology.DisplayLayout.ValueLists["ServerStatusValueList"].ValueListItems.Add(listItem);

            listItem = new ValueListItem(4, "Information");
            //listItem = new ValueListItem("ServerInformation", "");
            listItem.Appearance.Image = global::Idera.SQLdm.DesktopClient.Properties.Resources.ServerInformation;
            grdReplicationTopology.DisplayLayout.ValueLists["ServerStatusValueList"].ValueListItems.Add(listItem);

            //listItem = new ValueListItem("Server", "");
            listItem = new ValueListItem(5, "Unknown");
            listItem.Appearance.Image = global::Idera.SQLdm.DesktopClient.Properties.Resources.Server;
            grdReplicationTopology.DisplayLayout.ValueLists["ServerStatusValueList"].ValueListItems.Add(listItem);
            
            ValueList myValueList = grdReplicationTopology.DisplayLayout.ValueLists["ServerStatusValueList"];
            UltraGridBand band = grdReplicationTopology.DisplayLayout.Bands[0];

            band.Columns["Distributor Status"].ValueList = myValueList;
            band.Columns["Publisher Status"].ValueList = myValueList;
            band.Columns["Subscriber Status"].ValueList = myValueList;

            band.Columns["Publisher Status"].GroupByEvaluator = new StatusGroupByEvaluator(grdReplicationTopology.DisplayLayout.ValueLists["ServerStatusValueList"]);
            band.Columns["Distributor Status"].GroupByEvaluator = new StatusGroupByEvaluator(grdReplicationTopology.DisplayLayout.ValueLists["ServerStatusValueList"]);
            band.Columns["Subscriber Status"].GroupByEvaluator = new StatusGroupByEvaluator(grdReplicationTopology.DisplayLayout.ValueLists["ServerStatusValueList"]);

        }

        private void InitializeTopologyDataTable()
        {

            topologyDataTable = new DataTable();
            topologyDataTable.Columns.Add("Articles", typeof(int));
            //removing this because the distributor has a field that fulfills the same role
            //that appears to be more reliable
            //topologyDataTable.Columns.Add("Subscription Status", typeof(int));
            topologyDataTable.Columns.Add("Publisher Status", typeof(int));
            topologyDataTable.Columns.Add("Publisher", typeof(string));
            topologyDataTable.Columns.Add("Distributor Status", typeof(int));
            topologyDataTable.Columns.Add("Distributor", typeof(string));
            topologyDataTable.Columns.Add("Subscriber Status", typeof(int));
            topologyDataTable.Columns.Add("Subscriber", typeof(string));
            topologyDataTable.Columns.Add("Publication", typeof(string));
            topologyDataTable.Columns.Add("Publication Description", typeof(string));
            topologyDataTable.Columns.Add("Replication Type", typeof(int));
            topologyDataTable.Columns.Add("PublisherServerID", typeof(int));
            topologyDataTable.Columns.Add("DistributorServerID", typeof(int));
            topologyDataTable.Columns.Add("SubscriberServerID", typeof(int));
            topologyDataTable.Columns.Add("PublisherInstance", typeof(string));
            topologyDataTable.Columns.Add("PublisherDB", typeof(string));
            topologyDataTable.Columns.Add("SubscriberInstance", typeof(string));
            topologyDataTable.Columns.Add("SubscriberDB", typeof(string));
            topologyDataTable.Columns.Add("DistributorInstance", typeof(string));
            topologyDataTable.Columns.Add("DistributorDB", typeof(string));

            topologyDataTable.Columns["SubscriberInstance"].AllowDBNull = true;
            topologyDataTable.Columns["SubscriberDB"].AllowDBNull = true;
            topologyDataTable.Columns["Subscriber"].AllowDBNull = true;
            topologyDataTable.Columns["SubscriberServerID"].AllowDBNull = true;
            topologyDataTable.Columns["Subscriber Status"].AllowDBNull = true;

            topologyDataTable.Columns["SubscriberInstance"].Unique = false;
            topologyDataTable.Columns["SubscriberDB"].Unique = false;
            topologyDataTable.Columns["Subscriber"].Unique = false;
            topologyDataTable.Columns["SubscriberServerID"].Unique = false;
            topologyDataTable.Columns["Subscriber Status"].Unique = false;
            
            DataColumn[] primaryKey = new DataColumn[] {
                topologyDataTable.Columns["PublisherInstance"], 
                topologyDataTable.Columns["PublisherDB"], 
                topologyDataTable.Columns["Publication"],
                topologyDataTable.Columns["SubscriberInstance"],
                topologyDataTable.Columns["SubscriberDB"]};

            topologyDataTable.PrimaryKey = primaryKey;

            topologyDataTable.CaseSensitive = true;
            
            topologyDataTable.DefaultView.Sort = "[Publisher]";

            grdReplicationTopology.DataSource = topologyDataTable;

            CreateServerStatusValueList();
        }

        private void InitializeDistributorQueueDataTable()
        {
            distributorQueueDataTable = new DataTable();
            distributorQueueDataTable.Columns.Add("Entry Date", typeof(DateTime));
            distributorQueueDataTable.Columns.Add("Publication Database", typeof(string));
            distributorQueueDataTable.Columns.Add("Subscription Database", typeof(string));
            distributorQueueDataTable.Columns.Add("Wait Time", typeof(TimeSpan));
            distributorQueueDataTable.Columns.Add("Command", typeof(string));

            //currentDataTable.PrimaryKey = new DataColumn[] { currentDataTable.Columns["Publication Database"] };
            distributorQueueDataTable.CaseSensitive = true;

            distributorQueueDataTable.DefaultView.Sort = "[Publication Database]";

            distributorTransactionsGrid.DataSource = distributorQueueDataTable;
        }

        private void InitializePublisherHistoryDataTable()
        {
            publisherHistoryDataTable = new DataTable("PublisherHistoryDataTable");

            publisherHistoryDataTable.Columns.Add("UTCCollectionDateTime", typeof(DateTime));
            publisherHistoryDataTable.Columns.Add("DistributionLatencyInSeconds", typeof(double));
            publisherHistoryDataTable.Columns.Add("ReplicationUndistributed", typeof(long));

            trendNonDistributedQueue.DataSource = publisherHistoryDataTable;
            trendNonDistributedTime.DataSource = publisherHistoryDataTable;

            publisherHistoryDataTable.DefaultView.Sort = "UTCCollectionDateTime";
        }

        private void InitializeDistributorHistoryDataTable()
        {
            distributorHistoryDataTable = new DataTable("DistributorHistoryDataTable");

            distributorHistoryDataTable.Columns.Add("UTCCollectionDateTime", typeof(DateTime));
            distributorHistoryDataTable.Columns.Add("ReplicationLatencyInSeconds", typeof(double));
            distributorHistoryDataTable.Columns.Add("ReplicationSubscribed", typeof(long));
            distributorHistoryDataTable.Columns.Add("ReplicationUnsubscribed", typeof(long));

            trendNonSubscribedQueue.DataSource = distributorHistoryDataTable;
            trendNonSubscribedTime.DataSource = distributorHistoryDataTable;

            distributorHistoryDataTable.DefaultView.Sort = "UTCCollectionDateTime";
        }
        #endregion

        #region grid

        private void SortSelectedColumnAscending()
        {
            if (selectedColumn != null)
            {
                selectedColumn.Band.SortedColumns.Clear();
                selectedColumn.Band.SortedColumns.Add(selectedColumn, false, false);
            }
        }

        private void SortSelectedColumnDescending()
        {
            if (selectedColumn != null)
            {
                selectedColumn.Band.SortedColumns.Clear();
                selectedColumn.Band.SortedColumns.Add(selectedColumn, true, false);
            }
        }

        private void GroupBySelectedColumn(bool GroupBy)
        {
            if (selectedColumn != null)
            {
                if (GroupBy)
                {
                    selectedColumn.Band.SortedColumns.Add(selectedColumn, false, true);
                }
                else
                {
                    selectedColumn.Band.SortedColumns.Remove(selectedColumn);
                }
            }
        }

        private void ToggleGroupByBox(UltraGrid grid)
        {
            if (grid == grdReplicationTopology)
            {
                ToggleTopologyGroupByBox();
            }
            else
            {
                ToggleDistributorGroupByBox();
            }
        }

        private void RemoveSelectedColumn()
        {
            if (selectedColumn != null)
            {
                selectedColumn.Hidden = true;
            }
        }

        private static void CollapseAllGroups(UltraGridBase grid)
        {
            grid.Rows.CollapseAll(true);
        }

        private static void ExpandAllGroups(UltraGridBase grid)
        {
            grid.Rows.ExpandAll(true);
        }

        private void ShowColumnChooser(UltraGrid grid)
        {
            SimpleUltraGridColumnChooserDialog dialog = new SimpleUltraGridColumnChooserDialog(grid);
            dialog.Show(this);
        }

        private void PrintGrid(UltraGrid grid)
        {
            ultraGridPrintDocument.Grid = grid;
            ultraPrintPreviewDialog.Document = ultraGridPrintDocument;
            ultraGridPrintDocument.DefaultPageSettings.Landscape = true;
            string title;
            if (grid.Tag is ToolStripItem)
            {
                title = ((ToolStripItem)grid.Tag).Text;
            }
            else if (grid.Tag is string)
            {
                title = (string)grid.Tag;
            }
            else
            {
                title = "Replication";
            }
            ultraGridPrintDocument.DocumentName = title;
            ultraGridPrintDocument.Header.TextLeft =
                string.Format("{0} - {1} as of {2}",
                              ApplicationModel.Default.ActiveInstances[instanceId].InstanceName,
                              title,
                              DateTime.Now.ToString("G")
                    );
            ultraGridPrintDocument.Footer.TextCenter = "Page [Page #]";

            ultraPrintPreviewDialog.ShowDialog();
        }

        private void SaveGrid(UltraGrid grid)
        {
            saveFileDialog.DefaultExt = "xls";
            string title;
            if (grid.Tag is ToolStripItem)
            {
                title = ((ToolStripItem)grid.Tag).Text;
            }
            else if (grid.Tag is string)
            {
                title = (string)grid.Tag;
            }
            else
            {
                title = "Replication";
            }
            title = ExportHelper.GetValidFileName(title, true);

            saveFileDialog.FileName = title;
            saveFileDialog.Filter = "Excel Workbook (*.xls)|*.xls";
            saveFileDialog.Title = "Save as Excel Spreadsheet";
            if (saveFileDialog.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    ultraGridExcelExporter.Export(grid, saveFileDialog.FileName);
                }
                catch (Exception ex)
                {
                    ApplicationMessageBox.ShowError(this, "Unable to export data", ex);
                }
            }
        }

        #endregion

        #endregion

        #region Event Handlers

        //private void ReplicationDetailsTabControl_ActiveTabChanged(object sender, Infragistics.Win.UltraWinTabControl.ActiveTabChangedEventArgs e)
        //{
            
        //    if (e.Tab.Key == tabDistributorQueue.Tab.Key)
        //    {
        //        RefreshView();
        //    }
        //    ReplicationDetailsTabControl.ActiveTab = e.Tab;
        //    ReplicationDetailsTabControl.SelectedTab = e.Tab;
        //}

        //private void ReplicationDetailsTabControl_TabIndexChanged(object sender, EventArgs e)
        //{
        //    if (ReplicationDetailsTabControl.SelectedTab.Key == tabDistributorQueue.Tab.Key)
        //    {
        //        RefreshView();
        //    }
        //    base.OnTabIndexChanged(e);
        //}

        private void ReplicationDetailsTabControl_SelectedTabChanged(object sender, Infragistics.Win.UltraWinTabControl.SelectedTabChangedEventArgs e)
        {
            //if (e.Tab == tabDistributorQueue.Tab)
            //{

                RefreshView();
            //}
        }

        #region Splitter Focus

        private void splitContainer_MouseDown(object sender, MouseEventArgs e)
        {
            focused = GetFocusedControl(Controls);
        }

        private void splitContainer_MouseUp(object sender, MouseEventArgs e)
        {
            if (focused != null)
            {
                focused.Focus();
                focused = null;
            }
        }

        #endregion

        #region toolbars

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
                case "groupByThisColumnButton":
                    GroupBySelectedColumn(((StateButtonTool)e.Tool).Checked);
                    break;
                case "removeThisColumnButton":
                    RemoveSelectedColumn();
                    break;
                case "showColumnChooserButton":
                    if (selectedGrid != null)
                    {
                        ShowColumnChooser(selectedGrid);
                    }
                    break;
                case "toggleGroupByBoxButton":
                    if (selectedGrid != null)
                    {
                        ToggleGroupByBox(selectedGrid);
                    }
                    break;
                case "printGridButton":
                    if (selectedGrid != null)
                    {
                        PrintGrid(selectedGrid);
                    }
                    break;
                case "exportGridButton":
                    if (selectedGrid != null)
                    {
                        SaveGrid(selectedGrid);
                    }
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
                case "navigateToPublisher":
                    if (selectedGrid != null)
                        NavigateToServer(ReplicationRole.Publisher, selectedGrid);
                    break;
                case "navigateToDistributor":
                    if (selectedGrid != null)
                        NavigateToServer(ReplicationRole.Distributor, selectedGrid);
                    break;
                case "navigateToSubscriber":
                    if (selectedGrid != null)
                        NavigateToServer(ReplicationRole.Subscriber, selectedGrid);
                    break;

            }
        }

        private void toolbarsManager_BeforeToolDropdown(object sender, BeforeToolDropdownEventArgs e)
        {
            if (e.Tool.Key == "gridDataContextMenu" || e.Tool.Key == "gridContextMenu")
            {
                bool isGrouped = selectedGrid.Rows.Count > 0 && selectedGrid.Rows[0].IsGroupByRow;
                ((PopupMenuTool)e.Tool).Tools["collapseAllGroupsButton"].SharedProps.Enabled =
                    ((PopupMenuTool)e.Tool).Tools["expandAllGroupsButton"].SharedProps.Enabled = isGrouped;

                ((PopupMenuTool)e.Tool).Tools["navigateToPublisher"].SharedProps.Visible = true;
                ((PopupMenuTool)e.Tool).Tools["navigateToDistributor"].SharedProps.Visible = true;
                ((PopupMenuTool)e.Tool).Tools["navigateToSubscriber"].SharedProps.Visible = true;
            }
            if (e.Tool.Key == "columnContextMenu")
            {
                int minCantForEnable = UltraGridHelper.GetNotHiddenColumns(selectedGrid);
                bool enableTool = minCantForEnable > 1 ? true : false;

                ((PopupMenuTool)e.Tool).Tools["removeThisColumnButton"].SharedProps.Enabled = enableTool;
            }
        }

        #endregion

        #region grid

        private void grdReplicationTopology_AfterSelectChange(object sender, AfterSelectChangeEventArgs e)
        {
            //lblTopologyGridStatus.Text = LOADING;
            //grdReplicationTopology.Visible = false;

            //clear the labels because they are no longer relevant
            lblTransactionalPublisherStatus.Text = "";
            lblTransactionalDistributorStatus.Text = "";
            lblTransactionalPublisherStatus.Text = "";
            lblMergeDistributorStatus.Text = "";
            
            //set the default message to be displayed on the status labels
            UltraGridRow row = grdReplicationTopology.Selected.Rows[0];
            int intServer;


            int.TryParse(row.Cells["PublisherServerID"].Value.ToString(), out intServer);
            string publisherMessage = ApplicationModel.Default.ActiveInstances.Contains(intServer) ?
               Idera.SQLdm.Common.Constants.LOADING : string.Format(SERVER_NOT_MONITORED, "publisher", row.Cells["PublisherInstance"].Value);

            int.TryParse(row.Cells["DistributorServerID"].Value.ToString(), out intServer);
            string distributorMessage = ApplicationModel.Default.ActiveInstances.Contains(intServer) ?
                Idera.SQLdm.Common.Constants.LOADING : string.Format(SERVER_NOT_MONITORED, "distributor", row.Cells["DistributorInstance"].Value);

            int.TryParse(row.Cells["SubscriberServerID"].Value.ToString(), out intServer);
            string subscriberMessage = ApplicationModel.Default.ActiveInstances.Contains(intServer) ?
                Idera.SQLdm.Common.Constants.LOADING : string.Format(SERVER_NOT_MONITORED, "subscriber", row.Cells["SubscriberInstance"].Value);

            lblMergeDistributorStatus.Text = distributorMessage;
            lblTransactionalDistributorStatus.Text = distributorMessage;
            lblTransactionalPublisherStatus.Text = publisherMessage;
            lblTransactionalSubscriberStatus.Text = subscriberMessage;

            lblMergeDistributorStatus.Visible = true;
            lblTransactionalDistributorStatus.Visible = true;
            lblTransactionalPublisherStatus.Visible = true;
            lblTransactionalSubscriberStatus.Visible = true;
            
            grdPublisher.Visible = false;
            grdDistributor.Visible = false;
            grdSubscriber.Visible = false;
            grdDistributorMerge.Visible = false;
            grdMergePublisher.Visible = false;
            grdMergeSubscriber.Visible = false;

            StartDistributionDetailsRefresh();
        }

        private void grdReplicationTopology_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                selectedGrid = grdReplicationTopology;
                UIElement selectedElement =
                    ((UltraGrid)sender).DisplayLayout.UIElement.ElementFromPoint(new Point(e.X, e.Y));
                object contextObject = selectedElement.GetContext(typeof(ColumnHeader));

                if (contextObject is ColumnHeader)
                {
                    ColumnHeader columnHeader = contextObject as ColumnHeader;
                    selectedColumn = columnHeader.Column;

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
                        
                        //add navigation
                        ((ButtonTool)toolbarsManager.Tools["navigateToPublisher"]).SharedProps.Enabled = UserHasRightsToServer(row.Row.Cells["PublisherServerID"].Value.ToString());
                        ((ButtonTool)toolbarsManager.Tools["navigateToDistributor"]).SharedProps.Enabled = UserHasRightsToServer(row.Row.Cells["DistributorServerID"].Value.ToString());
                        ((ButtonTool)toolbarsManager.Tools["navigateToSubscriber"]).SharedProps.Enabled = UserHasRightsToServer(row.Row.Cells["SubscriberServerID"].Value.ToString());

                        toolbarsManager.SetContextMenuUltra(((UltraGrid)sender), "gridContextMenu");
                    }
                    else
                    {
                        toolbarsManager.SetContextMenuUltra(((UltraGrid)sender), "gridContextMenu");
                    }
                }
            }
            else
            {
                StartDistributionDetailsRefresh();
            }
        }

        private void distributorTransactionsGrid_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                selectedGrid = distributorTransactionsGrid;
                UIElement selectedElement =
                    ((UltraGrid)sender).DisplayLayout.UIElement.ElementFromPoint(new Point(e.X, e.Y));
                object contextObject = selectedElement.GetContext(typeof(ColumnHeader));

                if (contextObject is ColumnHeader)
                {
                    ColumnHeader columnHeader = contextObject as ColumnHeader;
                    selectedColumn = columnHeader.Column;

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

        #endregion

        private void ServicesReplicationView_Load(object sender, EventArgs e)
        {
            ApplySettings();
        }

        private void operationalStatusLabel_MouseEnter(object sender, EventArgs e)
        {
            operationalStatusLabel.ForeColor = Color.Black;
            operationalStatusLabel.BackColor = Color.FromArgb(255, 189, 105);
            operationalStatusImage.BackColor = Color.FromArgb(255, 189, 105);
        }

        private void operationalStatusLabel_MouseLeave(object sender, EventArgs e)
        {
            operationalStatusLabel.ForeColor = Color.Black;
            operationalStatusLabel.BackColor = Color.FromArgb(211, 211, 211);
            operationalStatusImage.BackColor = Color.FromArgb(211, 211, 211);
        }

        private void operationalStatusLabel_MouseDown(object sender, MouseEventArgs e)
        {
            operationalStatusLabel.ForeColor = Color.White;
            operationalStatusLabel.BackColor = Color.FromArgb(251, 140, 60);
            operationalStatusImage.BackColor = Color.FromArgb(251, 140, 60);
        }

        private void operationalStatusLabel_MouseUp(object sender, MouseEventArgs e)
        {
            operationalStatusLabel.ForeColor = Color.Black;
            operationalStatusLabel.BackColor = Color.FromArgb(255, 189, 105);
            operationalStatusImage.BackColor = Color.FromArgb(255, 189, 105);

            if (HistoricalSnapshotDateTime != null)
            {
                operationalStatusPanel.Visible = false;
                ApplicationController.Default.SetActiveViewToRealTimeMode();
            }
            else
            {
                ShowConfigurationProperties();
            }
        }

        private void MonitoredSqlServerChanged(object sender, MonitoredSqlServerChangedEventArgs e)
        {
            UpdateOperationalStatus(e.Instance);
        }

        private void historicalSnapshotStatusLinkLabel_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            ApplicationController.Default.SetActiveViewToRealTimeMode();
        }

        private void Settings_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            switch (e.PropertyName)
            {
                case "RealTimeChartVisibleLimitInMinutes":
                    UpdateRealTimeChartDataFilter();
                    break;
            }
        }
        #endregion

        private void UpdateOperationalStatus(MonitoredSqlServer instance)
        {
            if (HistoricalSnapshotDateTime != null)
            {
                operationalStatusLabel.Text = Properties.Resources.HistoryModeOperationalStatusLabel;
                operationalStatusPanel.Visible = true;
            }
            else if (instance != null && instance.ReplicationMonitoringDisabled)
            {
                operationalStatusLabel.Text = ReplicationDisabledMessage;
                operationalStatusPanel.Visible = true;
            }
            else
            {
                operationalStatusPanel.Visible = false;
            }
        }

        public void ShowConfigurationProperties()
        {
            try
            {
                MonitoredSqlServerInstancePropertiesDialog dialog =
                    new MonitoredSqlServerInstancePropertiesDialog(instanceId);
                dialog.SelectedPropertyPage = MonitoredSqlServerInstancePropertiesDialogPropertyPages.Replication;
                dialog.ShowDialog(this);
            }
            catch (Exception e)
            {
                ApplicationMessageBox.ShowError(ParentForm,
                                                "An error occurred while loading the instance properties dialog.", e);
            }
        }

        private void UpdateRealTimeChartDataFilter()
        {
            if (publisherHistoryDataTable != null)
            {
                DateTime viewFilter =
                    DateTime.Now.Subtract(TimeSpan.FromMinutes(ApplicationModel.Default.HistoryTimeValue.RealTimeMinutes));

                publisherHistoryDataTable.DefaultView.RowFilter = string.Format("UTCCollectionDateTime > #{0}#", viewFilter.ToString(CultureInfo.InvariantCulture));
            }
            if (distributorHistoryDataTable != null)
            {
                DateTime viewFilter =
                    DateTime.UtcNow.Subtract(TimeSpan.FromMinutes(ApplicationModel.Default.HistoryTimeValue.RealTimeMinutes));

                distributorHistoryDataTable.DefaultView.RowFilter = string.Format("UTCCollectionDateTime > #{0}#", viewFilter.ToString(CultureInfo.InvariantCulture));
            }

        }

        #region Refresh replication details
        private void StartDistributionDetailsRefresh()
        {
            // There is only one selected row which we iterate through
            foreach (UltraGridRow row in grdReplicationTopology.Selected.Rows)
            {
                //if the selected row is not a data row then we do not want to process further
                if (!row.IsDataRow) continue;
                StartDistributionDetailsRefresh(row);
            }
        }
        /// <summary>
        /// Start refreshing the replication details
        /// </summary>
        private void StartDistributionDetailsRefresh(UltraGridRow row)
        {

            switch ((ReplicationType)(int)row.Cells["Replication Type"].Value)
            {
                case ReplicationType.Merge:
                    //_currentlySelectedReplicationType = ReplicationType.Merge;

                    tabDistributorQueue.Tab.Visible = false;
                    
                    if (ReplicationDetailsTabControl.Tabs[1].Active)
                        ReplicationDetailsTabControl.Tabs["Transactional"].Selected = true;

                    tabDetailedOverview.Tab.Text = "Detailed Overview - Merge";
                    fmeDistributorLabel.Visible = fmeDistributor.Visible = true;
                    lblMergeDistributorStatus.Hide();
                    break;
                case ReplicationType.Transaction:
                    //_currentlySelectedReplicationType = ReplicationType.Transaction;
                    tabDistributorQueue.Tab.Visible = true;
                    if (ReplicationDetailsTabControl.ActiveTab.TabPage == tabDistributorQueue)
                    {
                        distributorTransactionsGridStatusLabel.Text = Idera.SQLdm.Common.Constants.LOADING;
                    }
                    tabDetailedOverview.Tab.Text = "Detailed Overview - Transactional";
                    fmeDistributorLabel.Visible = fmeDistributor.Visible = true;
                    fmeDistributor.BringToFront();
                    fmeDistributorLabel.BringToFront();
                    lblMergeDistributorStatus.Hide();
                    break;
                case ReplicationType.Snapshot:
                    //_currentlySelectedReplicationType = ReplicationType.Snapshot;

                    tabDistributorQueue.Tab.Visible = false;
                    lblMergeDistributorStatus.Text = "Snapshot replication details are not supported";
                    lblMergeDistributorStatus.Visible = true;
                    lblMergeDistributorStatus.BringToFront();
                    fmeDistributor.SendToBack();
                    fmeDistributorLabel.SendToBack();
                    fmeMergeOverview.Visible = true;
                    fmeMergeOverview.BringToFront();
                    if (ReplicationDetailsTabControl.Tabs[1].Active)
                        ReplicationDetailsTabControl.Tabs["Transactional"].Selected = true;
                    return;
            }
            //get the details of the current selected row
            UltraGridRow distributorServerRow = row;

            //Do not try and get the details if ither the local or remote partners are still busy
            if (refreshDistributorDetailsBackgroundWorker != null)
            {
                if (refreshDistributorDetailsBackgroundWorker.IsBusy) return;
            }

            DistributionDetailsRefreshStopwatch.Reset();
            DistributionDetailsRefreshStopwatch.Start();
            
            InitializeDistributionDetailsBackgroundWorker();

            if (refreshDistributorDetailsBackgroundWorker != null)
                refreshDistributorDetailsBackgroundWorker.RunWorkerAsync(distributorServerRow);
        }

        /// <summary>
        /// Initialize the Distribution details backgroundworker
        /// </summary>
        private void InitializeDistributionDetailsBackgroundWorker()
        {
            if (refreshDistributorDetailsBackgroundWorker != null)
            {
                if (refreshDistributorDetailsBackgroundWorker.IsBusy) refreshDistributorDetailsBackgroundWorker.CancelAsync();

                refreshDistributorDetailsBackgroundWorker = null;
                InitializeDistributionDetailsBackgroundWorker();

            }
            else
            {
                refreshDistributorDetailsBackgroundWorker = new BackgroundWorker();
                refreshDistributorDetailsBackgroundWorker.WorkerSupportsCancellation = true;
                refreshDistributorDetailsBackgroundWorker.DoWork +=
                    new DoWorkEventHandler(refreshDistributionDetailsBackgroundWorker_DoWork);
                refreshDistributorDetailsBackgroundWorker.RunWorkerCompleted +=
                    new RunWorkerCompletedEventHandler(refreshDistributionDetailsBackgroundWorker_RunWorkerCompleted);
            }
        }

        /// <summary>
        /// Kicks of the asynchronous gathering of the distribution details
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void refreshDistributionDetailsBackgroundWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            if (Thread.CurrentThread.Name == null) Thread.CurrentThread.Name = "RefreshDistributionQueue";

            UltraGridRow currentRow;
            PublisherDetailsRefresh publisherRefresh = null;
            SubscriberDetailsRefresh subscriberRefresh = null;
            DistributorDetails distributorResult = null;
            PublisherDetails publisherResult = null;
            SubscriberDetails subscriberResult = null;

            bool publisherFoundInTopology = false;
            bool subscriberFoundInTopology = false;
            bool distributorFoundInTopology = false;

            if (blnPopulatingReplicationDetails) return;

            blnPopulatingReplicationDetails = true;

            try
            {
                if (refreshDistributorDetailsBackgroundWorker.CancellationPending)
                {
                    e.Cancel = true;
                    return;
                }

                IManagementService managementService =
                    ManagementServiceHelper.GetDefaultService(Settings.Default.ActiveRepositoryConnection.ConnectionInfo);

                if (e.Argument is UltraGridRow)
                {
                    currentRow = (UltraGridRow)(e.Argument);
                }
                else
                {
                    return;
                }

                ReplicationType replicationType = (ReplicationType)(int.Parse(currentRow.Cells["Replication Type"].Value.ToString()));
                if (replicationType == ReplicationType.Transaction)
                {
                    int PublisherID = -1;
                    //if the publisher field is not blank
                    if (currentRow.Cells["PublisherServerID"].Value is int)
                    {
                        //get the instance id
                        PublisherID = (int)currentRow.Cells["PublisherServerID"].Value;
                        //publisher is really only in topology if active
                        publisherFoundInTopology = ApplicationModel.Default.ActiveInstances.Contains(PublisherID);
                    }
                    string publisherMessage = !publisherFoundInTopology ? string.Format(SERVER_NOT_MONITORED, "publisher", currentRow.Cells["PublisherInstance"].Value) : null;
                    if (publisherFoundInTopology)
                    {
                        publisherRefresh = new PublisherDetailsRefresh(new object[]{
                           PublisherID,
                           currentRow.Cells["PublisherInstance"].Value.ToString(),
                           currentRow.Cells["PublisherDB"].Value.ToString(),
                           currentRow.Cells["Publication"].Value.ToString(),
                           currentRow.Cells["Publication Description"].Value.ToString(),
                           publisherMessage});
                    }

                    int SubscriberID = -1;
                    //if the subscriber field is not blank
                    if(currentRow.Cells["SubscriberServerID"].Value is int)
                    {
                        //get the instance id
                        SubscriberID = (int)currentRow.Cells["SubscriberServerID"].Value;
                        //subscriber is really only in topology if active
                        subscriberFoundInTopology = ApplicationModel.Default.ActiveInstances.Contains(SubscriberID);
                    }

                    string subscriberMessage = !subscriberFoundInTopology? string.Format(SERVER_NOT_MONITORED, "subscriber", currentRow.Cells["SubscriberInstance"].Value) : null;

                    subscriberRefresh = new SubscriberDetailsRefresh(new object[]{
                         SubscriberID,
                         currentRow.Cells["SubscriberDB"].Value.ToString(),
                         currentRow.Cells["Publication"].Value.ToString(),
                         subscriberMessage});

                }

                if (replicationType == ReplicationType.Transaction)
                {
                    if(publisherFoundInTopology) publisherRefresh.Start();
                    if (subscriberRefresh != null) subscriberRefresh.Start();
                }
                
                //get the id of the monitored instance
                int DistributorServerID = -1;
                if(currentRow.Cells["DistributorServerID"].Value is int)
                {
                    //get the instance id
                    DistributorServerID = (int)currentRow.Cells["DistributorServerID"].Value;
                    //if the distributor is no longer active then treat it as if it was not found
                    distributorFoundInTopology = ApplicationModel.Default.ActiveInstances.Contains(DistributorServerID);
                }

                string distributorMessage = !distributorFoundInTopology ? string.Format(SERVER_NOT_MONITORED, "distributor", currentRow.Cells["DistributorInstance"].Value) : null;

                //if the server is being monitored
                if (distributorFoundInTopology)
                {
                    string distributorInstance = currentRow.Cells["DistributorInstance"].Value.ToString();
                    string distributorDB = currentRow.Cells["DistributorDB"].Value.ToString();

                    configurationDistributors.PublisherName = currentRow.Cells["PublisherInstance"].Value.ToString();
                    configurationDistributors.DistributionDatabase = currentRow.Cells["DistributorDB"].Value.ToString();
                    configurationDistributors.PublicationDatabase = currentRow.Cells["PublisherDB"].Value.ToString(); ;
                    configurationDistributors.Publication = currentRow.Cells["Publication"].Value.ToString();
                    configurationDistributors.SubscriptionDatabase = currentRow.Cells["SubscriberDB"].Value.ToString();
                    configurationDistributors.SubscriptionServer = currentRow.Cells["SubscriberInstance"].Value.ToString();

                    configurationDistributors.GetQueue = (ReplicationDetailsTabControl.SelectedTab == tabDistributorQueue.Tab ? true : false);

                    if (currentRow.Cells["Replication Type"].Value.ToString().Length > 0)
                    {
                        configurationDistributors.ReplicationType = (ReplicationType)(int.Parse(currentRow.Cells["Replication Type"].Value.ToString()));
                    }

                    configurationDistributors.MonitoredServerId = DistributorServerID;
                    
                    //if we have previous values
                    if (previousSubscribedCollection != null)
                    {
                        int intHash = (currentRow.Cells["Publisher"].Value + "." 
                            + currentRow.Cells["Publication"].Value + "."
                            + currentRow.Cells["SubscriberInstance"].Value + "."
                            + currentRow.Cells["SubscriberDB"].Value).GetHashCode();

                        //if we have previous values for this session
                        if(previousSubscribedCollection.ContainsKey(intHash))
                        {
                            //pass the previous values to the configuration
                            configurationDistributors.LastSampleTime = previousSubscribedCollection[intHash].First;
                            configurationDistributors.LastSubscribedTranCount = previousSubscribedCollection[intHash].Second;
                        }
                    }

                    //get the distibutor details
                    distributorResult = managementService.GetDistributorDetails(configurationDistributors);

                    distributorResult.DistributorInstance = distributorInstance;
                    distributorResult.DistributorDatabase = distributorDB;
                }
                else
                {
                    distributorResult = new DistributorDetails(distributorMessage);
                    distributorResult.SelectedReplicationType = replicationType;
                }

                if (publisherRefresh != null)
                {
                    if (publisherRefresh.WorkerThread.ThreadState != System.Threading.ThreadState.Unstarted)
                    {
                        if (!publisherRefresh.WorkerThread.Join(REPLTIMEOUT))
                        {
                            publisherResult = new PublisherDetails(string.Format(THREAD_TIMEOUT, "publisher"));
                        }
                        else
                        {
                            publisherResult = publisherRefresh.Result;
                        }
                    }
                }
                if (subscriberRefresh != null)
                {
                    if (subscriberRefresh.WorkerThread.ThreadState != System.Threading.ThreadState.Unstarted)
                    {
                        if (!subscriberRefresh.WorkerThread.Join(REPLTIMEOUT))
                        {
                            subscriberResult = new SubscriberDetails(string.Format(THREAD_TIMEOUT, "subscriber"));
                        }
                        else
                        {
                            subscriberResult = subscriberRefresh.Result;
                        }
                    }
                }
                e.Result = new Triple<PublisherDetails, DistributorDetails, SubscriberDetails>(publisherResult, distributorResult, subscriberResult);
            }

            catch (Exception ex)
            {
                e.Result = ex;
                blnPopulatingReplicationDetails = false;
            }
        }
        /// <summary>
        /// This is the final callback once the replication details have been gathered
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void refreshDistributionDetailsBackgroundWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            DistributorDetails resultDistributor = null;

            //If this call has been cancelled or the result is null (this would happen if we are waiting
            //for results of the last call)
            if (e.Cancelled)
            {
                blnPopulatingReplicationDetails = false;
                return;
            }
            
            if (e.Result == null)
            {
                blnPopulatingReplicationDetails = false;
                fmeDistributor.Text = "Distributor";
                fmeDistributorLabel.Text = "";
                lblTransactionalDistributorStatus.Text = "No information was returned from the distributor.";
                lblTransactionalDistributorStatus.Visible = true;
                grdDistributor.Visible = false;

                return;
            }

            try
            {
                if (!e.Cancelled)
                {
                    if (e.Error == null && (e.Result is Triple<PublisherDetails, DistributorDetails, SubscriberDetails>))
                    {
                        resultDistributor =
                            ((Triple<PublisherDetails, DistributorDetails, SubscriberDetails>) e.Result).Second;
                    }
                }
                DistributionDetailsRefreshStopwatch.Stop();

                if (!(e.Result is Triple<PublisherDetails, DistributorDetails, SubscriberDetails>))
                {
                    Log.Error("Invalid return type found!", e.Result);

                    lblTransactionalDistributorStatus.Text = "No results returned.";
                    lblTransactionalDistributorStatus.Visible = true;
                    grdDistributor.Visible = false;
                    lblTransactionalPublisherStatus.Text = "No results returned.";
                    lblTransactionalPublisherStatus.Visible = true;
                    grdPublisher.Visible = false;
                    lblTransactionalSubscriberStatus.Text = "No results returned.";
                    lblTransactionalSubscriberStatus.Visible = true;
                    grdSubscriber.Visible = false;

                    ApplicationController.Default.ClearCustomStatus();
                    ApplicationController.Default.OnRefreshActiveViewCompleted(
                        new RefreshActiveViewCompletedEventArgs(DateTime.Now, new Exception(e.Result.ToString())));

                    return;
                }
                else
                {
                    if (resultDistributor != null)
                        if (resultDistributor.SelectedReplicationType == ReplicationType.Transaction)
                        {
                            InterpretPublisherDetailsResults(
                                ((Triple<PublisherDetails, DistributorDetails, SubscriberDetails>) e.Result).First);
                            InterpretSubscriberDetailsResults(
                                ((Triple<PublisherDetails, DistributorDetails, SubscriberDetails>) e.Result).Third);

                            ApplicationController.Default.OnRefreshActiveViewCompleted(
                                new RefreshActiveViewCompletedEventArgs(DateTime.Now));

                        }
                }

                if (!e.Cancelled)
                {
                    if (e.Error == null)
                    {
                        if (resultDistributor.Error == null)
                        {
                            if (resultDistributor.DistributionDetails.Rows.Count == 0)
                            {
                                if (resultDistributor.ReplicationStatus == ReplicationState.CollectionDisabled)
                                {
                                    lblTransactionalDistributorStatus.Text = ReplicationMonitoringDisabled;
                                }
                                else
                                {
                                    lblTransactionalDistributorStatus.Text = string.Format("No distributor details were returned from {0}", resultDistributor.DistributorInstance + "." + resultDistributor.DistributorDatabase);
                                }
                                lblTransactionalDistributorStatus.Visible = true;
                                grdDistributor.Visible = false;
                                fmeDistributor.Text = "Distributor";
                                fmeDistributorLabel.Text = "";
                                layoutTransactionalOverview.Visible = true;
                                fmeMergeOverview.Visible = false;
                            }
                            else
                            {
                                lblTransactionalDistributorStatus.Visible = false;
                                grdDistributor.Visible = true;
                                fmeDistributor.Text = "Distributor";
                                fmeDistributorLabel.Text = resultDistributor.DistributorInstance + "." + resultDistributor.DistributorDatabase;
                                fmeMergeOverview.Text = fmeDistributor.Text + ":" + fmeDistributorLabel.Text;

                                PopulateDistributorGrid(resultDistributor);
                            }
                            
                            //this should only be done if getQueue is true
                            PopulateDistributorQueue(resultDistributor);

                            ApplicationController.Default.OnRefreshActiveViewCompleted(
                                new RefreshActiveViewCompletedEventArgs(DateTime.Now));

                        }
                        else
                        {
                            //An error occurred - show error messages in the frames for publisher and subscriber and distributor
                            if(resultDistributor.SelectedReplicationType != ReplicationType.Merge)
                            {
                                fmeMergeOverview.Hide();
                                layoutTransactionalOverview.Show();
                                lblMergeDistributorStatus.Visible = false;
                                lblTransactionalDistributorStatus.Text = resultDistributor.Error.Message;
                                lblTransactionalDistributorStatus.Visible = true;
                                grdDistributor.Visible = false;
                            }
                            else
                            {
                                grdDistributorMerge.Visible = false;
                                grdMergeSubscriber.Visible = false;
                                grdMergePublisher.Visible = false;
                                layoutTransactionalOverview.Hide();
                                fmeMergeOverview.Show();
                                lblMergeDistributorStatus.Text = resultDistributor.Error.Message;
                                lblMergeDistributorStatus.Show();
                                lblMergeDistributorStatus.BringToFront();
                            }

                            Log.Error("An error occurred while refreshing distribution details.", resultDistributor.Error);
                            ApplicationController.Default.ClearCustomStatus();
                            ApplicationController.Default.OnRefreshActiveViewCompleted(
                                new RefreshActiveViewCompletedEventArgs(DateTime.Now, resultDistributor.Error));
                        }
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
                blnPopulatingReplicationDetails = false;
            }
        }

        private void InterpretSubscriberDetailsResults(SubscriberDetails result)
        {
            try
            {
                fmeSubscriber.Text = "Subscriber";
                fmeSubscriberLabel.Text = "";
                subscriberDataTable.Clear();

                if (result != null)
                {
                    if (result.Error == null)
                    {
                        try
                        {
                            if (result.ReplicationStatus == ReplicationState.CollectionDisabled)
                            {
                                lblTransactionalSubscriberStatus.Visible = true;
                                grdSubscriber.Visible = false;
                                lblTransactionalSubscriberStatus.Text = ReplicationMonitoringDisabled;
                            }
                            else
                            {
                                lblTransactionalSubscriberStatus.Visible = false;
                                grdSubscriber.Visible = true;

                                subscriberDataTable.BeginLoadData();
                                subscriberDataTable.Clear();

                                subscriberDataTable.LoadDataRow(
                                            new object[]{
                                        result.SubscriptionType,
                                        result.LastUpdated,
                                        result.LastSyncStatus,
                                        result.LastSyncSummary,
                                        result.LastSyncTime != DateTime.MinValue?result.LastSyncTime.ToString():"N\\A"
                                            }, true);


                                subscriberDataTable.EndLoadData();

                                grdSubscriber.DisplayLayout.Bands[0].SortedColumns.RefreshSort(true);
                                grdSubscriber.Visible = true;

                            }
                        }
                        catch (Exception e)
                        {
                            Log.Debug(e);
                        }

                        fmeSubscriber.Text = "Subscriber";
                        fmeSubscriberLabel.Text = result.ServerName + "." + result.SubscriberDatabase;
                    }
                    else
                    {
                        fmeSubscriber.Text = "Subscriber";
                        fmeSubscriberLabel.Text = "";
                        subscriberDataTable.Clear();
                        grdSubscriber.Refresh();

                        lblTransactionalSubscriberStatus.Text = result.Error.Message;
                        lblTransactionalSubscriberStatus.Visible = true;

                        grdSubscriber.Visible = false;

                        Log.Error("An error occurred while refreshing subscription details.", result.Error);
                        ApplicationController.Default.ClearCustomStatus();
                        ApplicationController.Default.OnRefreshActiveViewCompleted(
                            new RefreshActiveViewCompletedEventArgs(DateTime.Now, result.Error));
                    }
                }
                else
                {
                    fmeSubscriber.Text = "Subscriber";
                    fmeSubscriberLabel.Text = "";
                    subscriberDataTable.Clear();
                    
                    grdSubscriber.Refresh();
                    
                    string errorMessage = UNSUBSCRIBED_SESSION;
                                       
                    lblTransactionalSubscriberStatus.Text = errorMessage;
                    lblTransactionalSubscriberStatus.Visible = true;
                    
                    grdSubscriber.Visible = false;

                    Log.Error("An error occurred while refreshing subscription details.", errorMessage);
                    
                    ApplicationController.Default.ClearCustomStatus();
                    ApplicationController.Default.OnRefreshActiveViewCompleted(
                        new RefreshActiveViewCompletedEventArgs(DateTime.Now, new Exception(errorMessage)));
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex.Message);

                ApplicationController.Default.ClearCustomStatus();
                ApplicationController.Default.OnRefreshActiveViewCompleted(
                    new RefreshActiveViewCompletedEventArgs(DateTime.Now, ex));
            }
        }

        private void InterpretPublisherDetailsResults(PublisherDetails result)
        {
            try
            {
                fmePublisher.Text = "Publisher";
                fmePublisherLabel.Text = "";
                publisherDataTable.Clear();

                if (result != null)
                {
                    if (result.Error == null)
                    {
                        if (result.ReplicationStatus == ReplicationState.CollectionDisabled)
                        {
                            lblTransactionalPublisherStatus.Visible = true;
                            grdPublisher.Visible = false;
                            lblTransactionalPublisherStatus.Text = ReplicationMonitoringDisabled;
                            fmePublisher.Text = "Publisher";
                            fmePublisherLabel.Text = result.PublisherInstance;
                        }
                        else
                        {
                            lblTransactionalPublisherStatus.Visible = false;
                            grdPublisher.Visible = true;

                            string errorMessage = PopulatePublisherGrid(result);
                            if (!string.IsNullOrEmpty(errorMessage))
                            {
                                lblTransactionalPublisherStatus.Text = errorMessage;
                                lblTransactionalPublisherStatus.Show();
                                lblTransactionalPublisherStatus.BringToFront();   
                            }
                            fmePublisher.Text = "Publisher";
                            fmePublisherLabel.Text = result.PublisherInstance + "." + result.PublisherDatabase;
                        }
                    }
                    else
                    {
                        fmePublisher.Text = "Publisher";
                        fmePublisherLabel.Text = "";
                        publisherDataTable.Clear();
                        grdPublisher.Refresh();
                        lblTransactionalPublisherStatus.Text = result.Error.Message;
                        lblTransactionalPublisherStatus.Visible = true;
                        grdPublisher.Visible = false;

                        Log.Error("An error occurred while refreshing publication details.", result.Error);
                        ApplicationController.Default.ClearCustomStatus();
                        ApplicationController.Default.OnRefreshActiveViewCompleted(
                            new RefreshActiveViewCompletedEventArgs(DateTime.Now, result.Error));
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
        }
        #endregion
        #region Details grids population
        /// <summary>
        /// Passes publisher grid details back althoughj this should not be required
        /// We should only get a single row back
        /// </summary>
        /// <param name="details"></param>
        private string PopulatePublisherGrid(PublisherDetails details)
        {
            double totalLatency = 0.0;
            long totalTransactions = 0;
            long replicatedTransactions = 0;
            double replicationRate = 0;
            double replicationLatency = 0.0;

            try
            {
                publisherDataTable.BeginLoadData();
                publisherDataTable.Clear();
                    //if no rows were returned
                if (details.PublishedDatabases.Rows.Count == 0)
                {
                    string message = string.Format(PUBLISHER_NO_ROWS, details.SQLAgentStatus??"Unknown");
                    if(details.SQLAgentStatus == null) return message;

                    string status = details.SQLAgentStatus.Trim('.');

                    //if there are enough characters to search for running
                    if (!status.Equals("Running"))
                    {
                        message += "\nPlease ensure that the SQL Agent service is running.";
                    }

                    message += "\nThe replication process is dependent upon the SQL Server Agent service.";
                    //check if the agent is running
                    return message;
                }
            
                foreach (DataRow row in details.PublishedDatabases.Rows)
                {
                    long.TryParse(row["replicatedTransactions"].ToString(), out replicatedTransactions);
                    double.TryParse(row["replicationRate"].ToString(), out replicationRate);
                    double.TryParse(row["replicationLatency"].ToString(), out replicationLatency);

                    publisherDataTable.LoadDataRow(
                        new object[]
                        {
                            details.PublicationName,
                            details.PublicationDescription,
                            replicatedTransactions,
                            replicationRate.ToString("N1") + " trans/s",
                            replicationLatency.ToString("N1") + " s"
                        }, true);

                    totalLatency += replicationLatency;
                    totalTransactions += replicatedTransactions;
                }
                

                publisherDataTable.EndLoadData();
                
                grdPublisher.DisplayLayout.Bands[0].SortedColumns.RefreshSort(true);
                grdPublisher.Visible = true;
            }
            catch (Exception e)
            {
                return e.Message;
            }
            return string.Empty;
        }

        private void AddPublisherSample(ReplicationTrendDataSample sample)
        {
            if (distributorHistoryDataTable == null) return;
            
                publisherHistoryDataTable.LoadDataRow(new object[]{
                sample.SampleTime,
                sample.NonDistributedLatency.HasValue?sample.NonDistributedLatency:0,
                sample.NonDistributedCount.HasValue?sample.NonDistributedCount:0},true);
        }

        private void AddDistributorSample(ReplicationTrendDataSample sample)
        {
            if (distributorHistoryDataTable == null) return;

            distributorHistoryDataTable.LoadDataRow(new object[]{
                sample.SampleTime,
                sample.NonSubscribedLatency.HasValue?sample.NonSubscribedLatency:0,
                sample.SubscribedCount.HasValue?sample.SubscribedCount:0,
                sample.NonSubscribedCount.HasValue?sample.NonSubscribedCount:0}, true);
        }

        
        /// <summary>
        /// Populate the details view with the distributor specific metrics - not the queue
        /// </summary>
        /// <param name="distributorDetails"></param>
        private void PopulateDistributorGrid(DistributorDetails distributorDetails)
        {
            double totalLatency = 0d;
            long totalSubscribed = 0L;
            long totalNonSubscribed = 0L;

            string subscriberdb = distributorDetails.DefaultSubscriptionDatabase;

            try
            {
                    if (configurationDistributors.ReplicationType == ReplicationType.Merge)
                    {
                        distributorMergeDataTable.BeginLoadData();
                        subscriberMergeDataTable.BeginLoadData();
                        publisherMergeDataTable.BeginLoadData();

                        distributorMergeDataTable.Clear();
                        subscriberMergeDataTable.Clear();
                        publisherMergeDataTable.Clear();
                        
                        //Get the currently selected
                        DataRow[] found = distributorDetails.DistributionDetails.Select("SubscriberDB = '" + subscriberdb.Replace("'","''") + "'");
                        if (found.Length == 0) return;

                        DataRow row = found[0];
                        //foreach (DataRow row in distributorDetails.DistributionDetails.Rows)
                        //{
                            #region Headings of grid and datatable
                            //distributorDataTable.Columns.Add("Agent Name", typeof(string));
                            //distributorDataTable.Columns.Add("Last Action", typeof(string));
                            //distributorDataTable.Columns.Add("Delivery Rate", typeof(float));
                            //distributorDataTable.Columns.Add("Publisher Inserted", typeof(int));
                            //distributorDataTable.Columns.Add("Publisher Updated", typeof(int));
                            //distributorDataTable.Columns.Add("Publisher Deleted", typeof(int));
                            //distributorDataTable.Columns.Add("Publisher Conflicts", typeof(int));
                            //distributorDataTable.Columns.Add("Subscriber Inserted", typeof(int));
                            //distributorDataTable.Columns.Add("Subscriber Updated", typeof(int));
                            //distributorDataTable.Columns.Add("Subscriber Deleted", typeof(int));
                            //distributorDataTable.Columns.Add("Subscriber Conflicts", typeof(int));
                            //distributorDataTable.DefaultView.Sort = "Agent Name";

                            //distributionDetails.Columns.Add("SubscriberName", typeof(string));
                            //distributionDetails.Columns.Add("MergeSubscriptionstatus", typeof(string));
                            //distributionDetails.Columns.Add("SubscriberDB", typeof(string));
                            //distributionDetails.Columns.Add("Type", typeof(int));
                            //distributionDetails.Columns.Add("Agent Name", typeof(string));
                            //distributionDetails.Columns.Add("Last Action", typeof(string));
                            //distributionDetails.Columns.Add("Delivery Rate", typeof(int));
                            //distributionDetails.Columns.Add("Publisher Insertcount", typeof(int));
                            //distributionDetails.Columns.Add("Publisher Updatecount", typeof(int));
                            //distributionDetails.Columns.Add("Publisher Deletecount", typeof(int));
                            //distributionDetails.Columns.Add("Publisher Conflicts", typeof(int));
                            //distributionDetails.Columns.Add("Subscriber Insertcount", typeof(int));
                            //distributionDetails.Columns.Add("Subscriber Updatecount", typeof(int));
                            //distributionDetails.Columns.Add("Subscriber Deletecount", typeof(int));
                            //distributionDetails.Columns.Add("Subscriber Conflicts", typeof(int));
                            #endregion

                            distributorMergeDataTable.LoadDataRow(new object[]
                            {
                            !string.IsNullOrEmpty(row["Agent Name"].ToString()) ? row["Agent Name"].ToString() : "",
                            !string.IsNullOrEmpty(row["Last Action"].ToString()) ? row["Last Action"].ToString() : "",
                            !string.IsNullOrEmpty(row["Delivery Rate"].ToString()) ? row["Delivery Rate"].ToString() : "0"
                            }, true);
                            

                            publisherMergeDataTable.LoadDataRow(new object[]
                            {
                            !string.IsNullOrEmpty(row["Publisher Insertcount"].ToString()) ? row["Publisher Insertcount"].ToString() : "0",
                            !string.IsNullOrEmpty(row["Publisher Updatecount"].ToString()) ? row["Publisher Updatecount"].ToString() : "0",
                            !string.IsNullOrEmpty(row["Publisher Deletecount"].ToString()) ? row["Publisher Deletecount"].ToString() : "0",
                            !string.IsNullOrEmpty(row["Publisher Conflicts"].ToString()) ? row["Publisher Conflicts"].ToString() : "0"
                            }, true);

                            subscriberMergeDataTable.LoadDataRow(new object[]
                            {
                            !string.IsNullOrEmpty(row["Subscriber Insertcount"].ToString()) ? row["Subscriber Insertcount"].ToString() : "0",
                            !string.IsNullOrEmpty(row["Subscriber Updatecount"].ToString()) ? row["Subscriber Updatecount"].ToString() : "0",
                            !string.IsNullOrEmpty(row["Subscriber Deletecount"].ToString()) ? row["Subscriber Deletecount"].ToString() : "0",
                            !string.IsNullOrEmpty(row["Subscriber Conflicts"].ToString()) ? row["Subscriber Conflicts"].ToString() : "0"
                            }, true);
                        //}

                        distributorMergeDataTable.EndLoadData();
                        publisherMergeDataTable.EndLoadData();
                        subscriberMergeDataTable.EndLoadData();

                        grdDistributorMerge.DataSource = distributorMergeDataTable;
                        grdMergeSubscriber.DataSource = subscriberMergeDataTable;
                        grdMergePublisher.DataSource = publisherMergeDataTable;

                        grdDistributorMerge.DisplayLayout.Bands[0].SortedColumns.RefreshSort(true);
                        grdMergeSubscriber.DisplayLayout.Bands[0].SortedColumns.RefreshSort(true);
                        grdMergePublisher.DisplayLayout.Bands[0].SortedColumns.RefreshSort(true);
                        grdDistributorMerge.Visible = true;
                        grdMergeSubscriber.Visible = true;
                        grdMergePublisher.Visible = true;
                        layoutTransactionalOverview.Visible = false;
                        //layoutMergeOverview.Visible = true;
                        fmeMergeOverview.Visible = true;
                    }
                    else //if transactional replication
                    {
                        distributorDataTable.BeginLoadData();
                        distributorDataTable.Clear();

                        foreach (DataRow row in distributorDetails.DistributionDetails.Rows)
                        {
                            int lngSubscribed = 0;
                            int lngNonSubscribed = 0;
                            float lngSubscriptionLatency = 0;
                            double subscriptionRate = 0;
                            string publisherInstanceAndDB = null;
                            string publication = null;

                            DateTime catchUpTime = DateTime.MinValue;
                            DateTime sampleTime = DateTime.MinValue;

                            if (!string.IsNullOrEmpty(row["Subscribed"].ToString())) int.TryParse(row["Subscribed"].ToString(), out lngSubscribed);
                            if (!string.IsNullOrEmpty(row["Non-Subscribed"].ToString())) int.TryParse(row["Non-Subscribed"].ToString(), out lngNonSubscribed);
                            if (!string.IsNullOrEmpty(row["SubscriptionLatency"].ToString())) float.TryParse(row["SubscriptionLatency"].ToString(), out lngSubscriptionLatency);
                            if (!string.IsNullOrEmpty(row["SubscriptionRate"].ToString())) double.TryParse(row["SubscriptionRate"].ToString(),out subscriptionRate);
                            if (!string.IsNullOrEmpty(row["CatchUpTime"].ToString())) catchUpTime = (DateTime)row["CatchUpTime"];
                            if (!string.IsNullOrEmpty(row["Publisher"].ToString())) publisherInstanceAndDB = row["Publisher"].ToString();
                            if (!string.IsNullOrEmpty(row["Publication"].ToString())) publication = row["Publication"].ToString();
                            if (!string.IsNullOrEmpty(row["SampleTime"].ToString())) sampleTime = (DateTime)row["SampleTime"];


                            TimeSpan timeTillSynced = catchUpTime - sampleTime.ToLocalTime();

                            if (timeTillSynced.TotalMilliseconds < 0.0) timeTillSynced = new TimeSpan(0);

                            DateTime dteTimeTillSynced = DateTime.MinValue.Add(timeTillSynced);

                            if (subscriptionRate < 0) subscriptionRate = 0;

                            //Commented till we decide we must return all the rows so we can add the sum into the real-time charts
                            //if(publication == grdReplicationTopology.Selected.Rows[0].Cells["Publication"].Value.ToString()){
                                //string timeToSync = catchUpTime == DateTime.MinValue ? "" : dteTimeTillSynced.ToString();
                                distributorDataTable.LoadDataRow(
                                    new object[]
                                {
                                     #region Headings
                                //    if (!dataReader.IsDBNull(0)) dr["Articles"] = dataReader.GetInt32(0);
                                //    if (!dataReader.IsDBNull(1)) dr["PublisherDBID"] = dataReader.GetInt32(1);
                                //    if (!dataReader.IsDBNull(2)) dr["PublisherID"] = dataReader.GetInt16(2);
                                //    if (!dataReader.IsDBNull(3)) dr["Publisher"] = dataReader.GetString(3);
                                //    if (!dataReader.IsDBNull(4)) dr["Distributor"] = dataReader.GetString(4);
                                //    if (!dataReader.IsDBNull(5)) dr["Subscriber"] = dataReader.GetString(5);
                                //    if (!dataReader.IsDBNull(6)) dr["SubscriptionType"] = dataReader.GetString(6);
                                //    if (!dataReader.IsDBNull(7)) dr["Sync Type"] = dataReader.GetString(7);
                                //    if (!dataReader.IsDBNull(8)) dr["PublicationID"] = dataReader.GetInt32(8);
                                //    if (!dataReader.IsDBNull(9)) dr["Publication"] = dataReader.GetString(9);
                                //    if (!dataReader.IsDBNull(10)) dr["SubscriberID"] = dataReader.GetInt16(10);
                                //    if (!dataReader.IsDBNull(11)) dr["AgentID"] = dataReader.GetInt32(11);
                                //    if (!dataReader.IsDBNull(12)) dr["Subscribed"] = dataReader.GetInt64(12);
                                //    if (!dataReader.IsDBNull(13)) dr["Non-Subscribed"] = dataReader.GetInt64(13);
                                //    if (!dataReader.IsDBNull(14)) dr["SubscriptionLatency"] = dataReader.GetInt32(14);
                                    //distributionDetails.Columns.Add("SubscriptionRate", typeof(string));
                                    //distributionDetails.Columns.Add("CatchUpTime", typeof(DateTime));
                                    //distributionDetails.Columns.Add("SampleTime", typeof(DateTime));

            #endregion
                                    lngSubscribed,          //subscribed
                                    lngNonSubscribed,       //non-subscribed
                                    lngSubscriptionLatency,  //latency
                                    subscriptionRate.ToString("N1") + " cmds/s",
                                    dteTimeTillSynced==DateTime.MinValue?"N\\A":dteTimeTillSynced.ToString("H:mm:ss.ff")
                                }, true);

                                //if we have no previous values collection then create one
                                if (previousSubscribedCollection == null)
                                    previousSubscribedCollection = new Dictionary<int, Pair<DateTime, long>>();

                                int intHash = (publisherInstanceAndDB + "." 
                                    + publication + "."
                                    + distributorDetails.SubscriptionServer + "."
                                    + distributorDetails.DefaultSubscriptionDatabase).GetHashCode();

                                //do we have an entry for this session? update else add
                                if (previousSubscribedCollection.ContainsKey(intHash))
                                {
                                    //if there is an entry do an update (if this is more recent) 
                                    if (sampleTime.CompareTo(previousSubscribedCollection[intHash].First) > 0)
                                    {
                                        previousSubscribedCollection[intHash] = new Pair<DateTime, long>(sampleTime, lngSubscribed);
                                    }
                                }
                                else
                                {
                                    previousSubscribedCollection.Add(intHash, new Pair<DateTime, long>(sampleTime, lngSubscribed));
                                }
                            //}
                            //keep totals for graphs
                            totalSubscribed += lngSubscribed;
                            totalNonSubscribed += lngNonSubscribed;
                            totalLatency += lngSubscriptionLatency;
                        }
                        
                        distributorDataTable.EndLoadData();

                        //AddDistributorSample(new ReplicationTrendDataSample(distributorDetails.TimeStamp,
                        //    totalLatency, 
                        //    null, 
                        //    totalSubscribed,
                        //    totalNonSubscribed, null));

                        grdDistributor.DisplayLayout.Bands[0].SortedColumns.RefreshSort(true);
                        layoutTransactionalOverview.Visible = true;
                        //layoutMergeOverview.Visible = false;
                        fmeMergeOverview.Visible = false;
                    }
            }
            catch (Exception e)
            {
                Log.Error(e);
            }
        }
        /// <summary>
        /// Dummy function for putting some data in the subscriber grid
        /// </summary>
        private void PopulateSubscriberGrid(SubscriberDetails result)
        {
            //subscriberDataTable.Columns.Add("Subscription Type", typeof(int));
            //subscriberDataTable.Columns.Add("Last Updated Time", typeof(DateTime));
            //subscriberDataTable.Columns.Add("Last Sync Status", typeof(int));
            //subscriberDataTable.Columns.Add("Last Sync Summary", typeof(string));
            //subscriberDataTable.Columns.Add("Last Sync Time", typeof(DateTime));

            try
            {
                subscriberDataTable.BeginLoadData();

                subscriberDataTable.Clear();

                subscriberDataTable.LoadDataRow(
                        new object[]
                    {
                        result.SubscriptionType,
                        result.LastUpdated,
                        result.LastSyncStatus,
                        result.LastSyncSummary,
                        result.LastSyncTime
                    }, true);

                subscriberDataTable.EndLoadData();

                grdSubscriber.DisplayLayout.Bands[0].SortedColumns.RefreshSort(true);
                grdSubscriber.Visible = true;
            }
            catch (Exception e)
            {
                Log.Debug(e);
            }

        }
        /// <summary>
        /// Populate Distributor details into distributor grid
        /// </summary>
        /// <param name="details"></param>
        private void PopulateDistributorQueue(DistributorDetails details)
        {
                if (details != null)
                {
                    if (details is DistributorDetails)
                    {
                        lock (updateLock)
                        {
                            DistributorDetails snapshot = details as DistributorDetails;

                            if (snapshot.Error == null)
                            {
                                if (snapshot.NonSubscribedTransactions != null
                                    && snapshot.NonSubscribedTransactions.Rows.Count > 0)
                                {
                                    distributorQueueDataTable.BeginLoadData();

                                    distributorQueueDataTable.Clear();

                                    //now update any matching databases or add new ones
                                    foreach (DataRow row in snapshot.NonSubscribedTransactions.Rows)
                                    {
                                        TimeSpan? waitTime = null;
                                        if (snapshot.TimeStamp != null)
                                        {
                                            waitTime = snapshot.TimeStamp.Value.Subtract((DateTime)row["EntryTime"]);
                                        }
                                        distributorQueueDataTable.LoadDataRow(
                                                new object[]
                                            {
                                                row["EntryTime"] != null ? (object)((DateTime)row["EntryTime"]).ToLocalTime() : null,
                                                row["PublisherDatabase"],
                                                row["Subscriber"],
                                                waitTime,
                                                row["Command"]
                                            }, true);
                                    }

                                    distributorQueueDataTable.EndLoadData();

                                    if (!distributionInitialized)
                                    {
                                        foreach (UltraGridColumn column in distributorTransactionsGrid.DisplayLayout.Bands[0].Columns)
                                        {
                                            if (column.Key != "Command")
                                            {
                                                column.PerformAutoResize(PerformAutoSizeType.AllRowsInBand, true);
                                            }
                                        }

                                        if (distributorTransactionsGrid.Rows.Count > 0 && distributorTransactionsGrid.Selected.Rows.Count == 0)
                                        {
                                            distributorTransactionsGrid.Rows[0].Selected = true;
                                        }

                                        distributionInitialized = true;
                                    }
                                    distributorSnapshot = snapshot;

                                    distributorTransactionsGrid.DisplayLayout.Bands[0].SortedColumns.RefreshSort(true);
                                    distributorTransactionsGrid.Visible = true;

                                    //alertConfig = ApplicationModel.Default.GetAlertConfiguration(instanceId);
                                    //if (alertConfig != null)
                                    //{
                                    //    foreach (UltraGridRow gridRow in distributorTransactionsGrid.Rows.GetAllNonGroupByRows())
                                    //    {
                                    //        UpdateCellColor(Metric.NonSubscribedTransTime, alertConfig, gridRow, "Wait Time");
                                    //    }
                                    //}

                                    ApplicationController.Default.SetCustomStatus(
                                        String.Format("Replication Session{0}: {1}",
                                                topologyDataTable.Rows.Count == 1 ? string.Empty : "s",
                                                topologyDataTable.Rows.Count
                                                ),
                                        String.Format("Non-subscribed queue: {0} Item{1}",
                                                distributorQueueDataTable.Rows.Count,
                                                distributorQueueDataTable.Rows.Count == 1 ? string.Empty : "s")
                                        );
                                }
                                else
                                {
                                    distributorQueueDataTable.Clear();
                                    distributorTransactionsGridStatusLabel.Text = NO_ITEMS;
                                    distributorTransactionsGrid.Visible = false;

                                    ApplicationController.Default.SetCustomStatus(
                                        String.Format("Replication Session{0}: {1}",
                                            topologyDataTable.Rows.Count == 1 ? string.Empty : "s",        
                                            topologyDataTable.Rows.Count
                                                )
                                        );
                                }
                            }
                        }
                    }
                }
        }
        #endregion
        /// <summary>
        /// Thread for refreshing publication details
        /// </summary>
        internal class PublisherDetailsRefresh
        {
            private object[] _parameters = null;
            private bool blnPopulatingPublicationDetails;
            private PublisherDetails _result;

            private Thread _workerThread = null;

            internal PublisherDetailsRefresh(object[] parameters)
                : base()
            {
                _parameters = parameters;
                
                _workerThread = new Thread(publicationDetails_DoWork);
                
            }
            public Thread WorkerThread
            {
                get{return _workerThread;}
            }
            
            public void Start()
            {
                try
                {
                    _workerThread.Start(_parameters);
                }
                catch(ThreadAbortException)
                {

                }
            }

            public PublisherDetails Result
            {
                get{return _result;}
            }
            /// <summary>
            /// Work method of publication details thread
            /// </summary>
            /// <param name="parameters"></param>
            private void publicationDetails_DoWork(object parameters)
            {
                if (Thread.CurrentThread.Name == null)
                    Thread.CurrentThread.Name = "publicationDetails_DoWork";

                PublisherDetails returnDetails = null;

                if (blnPopulatingPublicationDetails) return;

                blnPopulatingPublicationDetails = true;

                try
                {
                    IManagementService managementService =
                        ManagementServiceHelper.GetDefaultService(Settings.Default.ActiveRepositoryConnection.ConnectionInfo);

                    int? intPublisherServerID = null;
                    string strPublisherDB = null;
                    string strPublisherInstance = null;
                    string strPublicationName = null;
                    string strPublicationDescription = null;
                    string strErrorMessage = null;

                    if (parameters is object[])
                    {
                        object[] paramarray = (object[])(parameters);
                        for (int i = 0; i < paramarray.Length; i++)
                        {
                            switch (i)
                            {
                                case 0:
                                    intPublisherServerID = paramarray[i] as int?;
                                    break;
                                case 1:
                                    strPublisherInstance = paramarray[i] as string;
                                    break;
                                case 2:
                                    strPublisherDB = paramarray[i] as string;
                                    break;
                                case 3:
                                    strPublicationName = paramarray[i] as string;
                                    break;
                                case 4:
                                    strPublicationDescription = paramarray[i] as string;
                                    break;
                                case 5:
                                    strErrorMessage = paramarray[i] as string;
                                    break;
                            }
                        }
                    }
                    else return;

                    //if the server is being monitored
                    if (intPublisherServerID.HasValue)
                    {
                        PublisherDetailsConfiguration publisherConfiguration = new PublisherDetailsConfiguration(intPublisherServerID.Value, strPublisherDB);

                        publisherConfiguration.MonitoredServerId = intPublisherServerID.Value;
                        publisherConfiguration.PublisherDatabase = strPublisherDB;

                        string publisherInstance = strPublisherInstance;
                        string publisherdb = strPublisherDB;
                        string publicationName = strPublicationName;
                        string publicationDescription = strPublicationDescription;

                        if (ApplicationModel.Default.ActiveInstances.Contains(publisherConfiguration.MonitoredServerId))
                        {
                            returnDetails = managementService.GetPublisherDetails(publisherConfiguration);

                            returnDetails.PublisherInstance = publisherInstance;
                            returnDetails.PublisherDatabase = publisherdb;
                            returnDetails.PublicationName = publicationName;
                            returnDetails.PublicationDescription = publicationDescription;
                        }
                        else
                        {
                            returnDetails = new PublisherDetails(strErrorMessage);
                        }
                    }
                    _result = returnDetails;
                }

                catch (Exception)
                {
                    blnPopulatingPublicationDetails = false;
                }
            }
        }
        /// <summary>
        /// Thread for refreshing subscriber details
        /// </summary>
        internal class SubscriberDetailsRefresh
        {
            private readonly object[] _parameters;
            private bool blnPopulatingSubscriberDetails;
            private SubscriberDetails _result;

            private readonly Thread _workerThread;

            internal SubscriberDetailsRefresh(object[] parameters)
                : base()
            {
                _parameters = parameters;
                _workerThread = new Thread(subscriberDetails_DoWork);

            }
            
            public Thread WorkerThread
            {
                get { return _workerThread; }
            }

            public void Start()
            {
                try
                {
                    _workerThread.Start(_parameters);
                }
                catch (ThreadAbortException)
                {

                }
            }

            public SubscriberDetails Result
            {
                get { return _result; }
            }
            /// <summary>
            /// Work method of publication details thread
            /// </summary>
            /// <param name="parameters"></param>
            private void subscriberDetails_DoWork(object parameters)
            {
                if (Thread.CurrentThread.Name == null)
                    Thread.CurrentThread.Name = "subscriberDetails_DoWork";

                SubscriberDetails returnDetails = null;

                if (blnPopulatingSubscriberDetails) return;

                blnPopulatingSubscriberDetails = true;

                try
                {
                    IManagementService managementService =
                        ManagementServiceHelper.GetDefaultService(Settings.Default.ActiveRepositoryConnection.ConnectionInfo);

                    int? intSubscriberServerID = null;
                    string strSubscriberDB = null;
                    string strPublication = null;
                    string strErrorMessage = null;

                    if (parameters is object[])
                    {
                        object[] paramarray = (object[])(parameters);
                        for (int i = 0; i < paramarray.Length; i++)
                        {
                            switch (i)
                            {
                                case 0:
                                    intSubscriberServerID = paramarray[i] as int?;
                                    break;
                                case 1:
                                    strSubscriberDB = paramarray[i] as string;
                                    break;
                                case 2:
                                    strPublication = paramarray[i] as string;
                                    break;
                                case 3:
                                    strErrorMessage = paramarray[i] as string;
                                    break;
                            }
                        }
                    }
                    else return;

                    //if the server is being monitored
                    if (intSubscriberServerID.HasValue)
                    {
                        SubscriberDetailsConfiguration subscriberConfiguration = new SubscriberDetailsConfiguration(intSubscriberServerID.Value, strSubscriberDB,strPublication);

                        if (ApplicationModel.Default.ActiveInstances.Contains(subscriberConfiguration.MonitoredServerId))
                        {
                            returnDetails = managementService.GetSubscriberDetails(subscriberConfiguration);
                        }
                        else
                        {
                            returnDetails = new SubscriberDetails(strErrorMessage);
                        }

                    }
                    _result = returnDetails;
                }

                catch (Exception)
                {
                    blnPopulatingSubscriberDetails = false;
                }
            }
        }
        #region Chart Initialization
        private void InitializeCharts()
        {
            InitializeNonDistributedTimeChart();
            InitializeNonDistributedCountChart();
            InitializeNonSubscribedTimeChart();
            InitializeNonSubscribedQueueChart();
        }

        /// <summary>
        /// stalled at the publisher as per repl_counters
        /// </summary>
        private void InitializeNonDistributedTimeChart()
        {
            trendNonDistributedTime.AxisX.LabelsFormat.Format = AxisFormat.Time;
            trendNonDistributedTime.Tag = "Non-Distributed Time";
            trendNonDistributedTime.ToolTipFormat = "%s\n%v s at %x";
            trendNonDistributedTime.AxisY.DataFormat.Decimals = 1;
            trendNonDistributedTime.Printer.Orientation = PageOrientation.Landscape;
            trendNonDistributedTime.Printer.Compress = true;
            trendNonDistributedTime.Printer.ForceColors = true;
            trendNonDistributedTime.Printer.Document.DocumentName = "Non Distributed Time Chart";

            FieldMap dateFieldMap = new FieldMap("UTCCollectionDateTime", FieldUsage.XValue);
            FieldMap nonDistributedTimeFieldMap = new FieldMap("DistributionLatencyInSeconds", FieldUsage.Value);
            nonDistributedTimeFieldMap.DisplayName = "Distribution Latency";
            trendNonDistributedTime.DataSourceSettings.Fields.AddRange(
                new FieldMap[] { dateFieldMap, nonDistributedTimeFieldMap });
        }

        /// <summary>
        /// This is the undistributed as per sp_replcounters.replicated transactions
        /// </summary>
        public void InitializeNonDistributedCountChart()
        {
            trendNonDistributedQueue.AxisX.LabelsFormat.Format = AxisFormat.Time;
            trendNonDistributedQueue.Tag = "Non distributed queue";
            trendNonDistributedQueue.ToolTipFormat = "%s\n%v transactions at %x";
            trendNonDistributedQueue.AxisY.Min = 0;
            trendNonDistributedQueue.AxisY.AutoScale = true;
            //trendNonDistributedQueue.AxisY.DataFormat.Decimals = 2;
            trendNonDistributedQueue.Printer.Orientation = PageOrientation.Landscape;
            trendNonDistributedQueue.Printer.Compress = true;
            trendNonDistributedQueue.Printer.ForceColors = true;
            trendNonDistributedQueue.Printer.Document.DocumentName = "Non distributed transaction Chart";

            FieldMap dateFieldMap = new FieldMap("UTCCollectionDateTime", FieldUsage.XValue);
            FieldMap totalNonDistributedCountFieldMap = new FieldMap("ReplicationUndistributed", FieldUsage.Value);
            totalNonDistributedCountFieldMap.DisplayName = "Non-distributed queue";

            trendNonDistributedQueue.DataSourceSettings.Fields.AddRange(new FieldMap[]
                                                                 {
                                                                     dateFieldMap,
                                                                     totalNonDistributedCountFieldMap
                                                                 });
        }

        /// <summary>
        /// Has stalled at the distributor
        /// </summary>
        private void InitializeNonSubscribedTimeChart()
        {
            trendNonSubscribedTime.AxisX.LabelsFormat.Format = AxisFormat.Time;
            trendNonSubscribedTime.Tag = "Non subscribed latency";
            trendNonSubscribedTime.ToolTipFormat = "%s\n%v s at %x";
            trendNonSubscribedTime.AxisY.DataFormat.Decimals = 1;
            trendNonSubscribedTime.AxisY.AutoScale = true;
            trendNonSubscribedTime.Printer.Orientation = PageOrientation.Landscape;
            trendNonSubscribedTime.Printer.Compress = true;
            trendNonSubscribedTime.Printer.ForceColors = true;
            trendNonSubscribedTime.Printer.Document.DocumentName = "Non-subscribed latency Chart";

            FieldMap dateFieldMap = new FieldMap("UTCCollectionDateTime", FieldUsage.XValue);
            FieldMap NonSubscribedTimeFieldMap = new FieldMap("ReplicationLatencyInSeconds", FieldUsage.Value);
            NonSubscribedTimeFieldMap.DisplayName = "Subscription Latency";
            trendNonSubscribedTime.DataSourceSettings.Fields.AddRange(
                new FieldMap[] { dateFieldMap, NonSubscribedTimeFieldMap });
        }
        /// <summary>
        /// subscribed and unsubscribed stalled on the distributor
        /// </summary>
        private void InitializeNonSubscribedQueueChart()
        {
            trendNonSubscribedQueue.AxisX.LabelsFormat.Format = AxisFormat.Time;
            trendNonSubscribedQueue.Tag = "Non subscribed transactions";
            trendNonSubscribedQueue.ToolTipFormat = "%s\n%v commands at %x";
            trendNonSubscribedQueue.AxisY.Min = 0;
            trendNonSubscribedQueue.AxisY.AutoScale = true;
            trendNonSubscribedQueue.Printer.Orientation = PageOrientation.Landscape;
            trendNonSubscribedQueue.Printer.Compress = true;
            trendNonSubscribedQueue.Printer.ForceColors = true;
            trendNonSubscribedQueue.Printer.Document.DocumentName = "Non-subscribed transactions";

            FieldMap dateFieldMap = new FieldMap("UTCCollectionDateTime", FieldUsage.XValue);
            //FieldMap subscribedFieldMap = new FieldMap("ReplicationSubscribed", FieldUsage.Value);
            //subscribedFieldMap.DisplayName = "Subscribed";
            FieldMap nonSubscribedFieldMap = new FieldMap("ReplicationUnsubscribed", FieldUsage.Value);
            nonSubscribedFieldMap.DisplayName = "Non-Subscribed";

            trendNonSubscribedQueue.DataSourceSettings.Fields.AddRange(
                new FieldMap[] { dateFieldMap, nonSubscribedFieldMap });
        }
        #endregion

        public class StatusGroupByEvaluator : IGroupByEvaluatorEx
        {
            private ValueList valueList = null;
            public StatusGroupByEvaluator(ValueList valueList)
            {
                this.valueList = valueList;
            }

            public object GetGroupByValue(UltraGridGroupByRow groupbyRow, UltraGridRow row)
            {
                ValueListItem item = valueList.FindByDataValue(groupbyRow.Value);
                if (item != null)
                {
                    return item.DisplayText;
                }

                return groupbyRow.Value;

            }

            public bool DoesGroupContainRow(UltraGridGroupByRow groupbyRow, UltraGridRow row)
            {
                string groupbyValue = groupbyRow.Value as string;
                ValueListItem item = valueList.FindByDataValue(row.Cells[groupbyRow.Column].Value);

                if (item == null || groupbyValue == null)
                {
                    return false;
                }

                return groupbyValue.Equals(item.DisplayText);
            }

            public int Compare(UltraGridCell cell1, UltraGridCell cell2)
            {
                if (cell1.Value.Equals(cell2.Value)) return 0;
                if (cell2.Value.ToString() != "" && cell1.Value.ToString() != "")
                {
                    if ((int)cell1.Value < (int)cell2.Value) return -1;
                }
                return 1;
            }
        }
        /// <summary>
        /// Adapts the size of the font for this control in case of OS font changes.
        /// </summary>
        private void AdaptFontSize()
        {            
            AutoScaleFontHelper.Default.AutoScaleControl(this, AutoScaleFontHelper.ControlType.Container);
        }
    }
}
