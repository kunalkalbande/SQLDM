//------------------------------------------------------------------------------
// <copyright file="ScheduledRefresh.cs" company="Idera, Inc.">
//     Copyright (c) Idera, Inc. All rights reserved.
// </copyright>
//------------------------------------------------------------------------------
// Change Log   ----------------------------------------------------------------
// Modified By          :   Pruthviraj Nikam
// Modification ID      :   M1
// Date                 :   04-Feb-2018
// Description          :   Done changes for New Azure SQL DB alerts.
//----------------------------------------------------------------------------
using Wintellect.PowerCollections;

namespace Idera.SQLdm.Common.Snapshots
{
    using System;
    using System.Collections.Generic;
    using Idera.SQLdm.Common.Events;
    using Idera.SQLdm.Common.Objects;
    using Cloud;
    /// <summary>
    /// Represents a standard scheduled refresh of a target server
    /// </summary>
    [Serializable]
    public sealed class ScheduledRefresh : AlertableSnapshot
    {
        #region fields

        /// <summary>
        /// Current snapshot AlwaysOn Availability Groups. Empty snapshot as default value.
        /// </summary>
        private AlwaysOnAvailabilityGroupsSnapshot availabilityGroupsSnapshot = new AlwaysOnAvailabilityGroupsSnapshot();

        private ServerOverview server = null;
        
        private Dictionary<Guid, MirrorMonitoringDatabaseDetail> mirroredDatabases = new Dictionary<Guid, MirrorMonitoringDatabaseDetail>();
        private Dictionary<Guid, MirrorMonitoringDatabaseDetail> _noLongerMirroredDatabases = new Dictionary<Guid, MirrorMonitoringDatabaseDetail>();
        private MonitoredSqlServer monitoredServer = null;
        
        private List<QueryMonitorStatement> queryMonitorStatements = new List<QueryMonitorStatement>();
        private Dictionary<string, byte[]> queryMonitorSignatures = new Dictionary<string, byte[]>();
        private List<DeadlockInfo> deadlocks = new List<DeadlockInfo>();
        private Dictionary<long, BlockingSessionInfo> blockingSessions = new Dictionary<long, BlockingSessionInfo>();
        private List<int> blockedSpids = new List<int>();
        private ScheduledRefreshAlerts alerts = new ScheduledRefreshAlerts();
        private ReplicationSummary replication = new ReplicationSummary();
        private SessionSnapshot sessionList = null;
        private LockDetails lockList = null;
        //private Exception databaseSizeError = new Exception("Removed functionality - work in progress");
        private Exception oldestOpenTransactionError = null;
        private Exception replicationError = null;
        private Exception mirroringError = null;
        private Exception queryMonitorError = null;
        private Exception activityMonitorError = null;
        private Memory memoryStatistics = new Memory();
        private Dictionary<int, CustomCounterSnapshot> customCounters = new Dictionary<int, CustomCounterSnapshot>();
        private Dictionary<string, DiskDrive> diskDrives = new Dictionary<string, DiskDrive>();
        private Dictionary<int, Dictionary<int, Pair<int, string>>> reorgRetryTables =
            new Dictionary<int, Dictionary<int, Pair<int, string>>>();

        private TableFragmentationCollectorStatus tableFragmentationStatus =TableFragmentationCollectorStatus.Stopped;
        private bool requestReconfiguration = false;
        private WaitStatisticsSnapshot waitStats;
        private ActiveWaitsSnapshot activeWaits;
        private bool isConnectionTestSnapshot = false;
        private FileSize tempdbFileSize = new FileSize(0);
        private FileSize versionStoreSize = new FileSize(0);
        private List<Metric> alertableMetrics = new List<Metric>();
        private ConfigurationSnapshot configurationDetails = null;

        //private List<SQLsafeOperation> ss_Operations = new List<SQLsafeOperation>(); 
        
        #endregion

        #region constructors

        public ScheduledRefresh(MonitoredSqlServer monitoredServer) : base(monitoredServer.ConnectionInfo.InstanceName)
        {
            MonitoredServer = monitoredServer.Clone();
            Server = new ServerOverview(monitoredServer.ConnectionInfo);
            waitStats = new WaitStatisticsSnapshot(monitoredServer.ConnectionInfo);
            alertableMetrics.AddRange(AlertingMetrics.ScheduledRefreshMetrics);
        }

        #endregion

        #region properties

        public override List<Metric> AlertableMetrics
        {
            get { return alertableMetrics; }
            set { alertableMetrics = value; }
        }
        
        public override AlertableSnapshotType  SnapshotType
        {
            get { return AlertableSnapshotType.ScheduledRefresh; }
        }

        public ActiveWaitsSnapshot ActiveWaits
        {
            get { return activeWaits; }
            internal set { activeWaits = value; }
        }

        public ServerOverview Server
        {
            get { return server; }
            set { server = value; }
        }

        public bool IsConnectionTestSnapshot
        {
            get { return isConnectionTestSnapshot; }
            internal set { isConnectionTestSnapshot = value; }
        }

        public List<QueryMonitorStatement> QueryMonitorStatements
        {
            get { return queryMonitorStatements; }
            internal set { queryMonitorStatements = value; }
        }


        public Dictionary<string, byte[]> QueryMonitorSignatures
        {
            get { return queryMonitorSignatures; }
            internal set { queryMonitorSignatures = value; }
        }

        public List<DeadlockInfo> Deadlocks
        {
            get { return deadlocks; }
            internal set { deadlocks = value; }
        }

        /// <summary>
        /// List of blocking sessions gathered from Activity Monitor
        /// There will be one per blocked process so expect duplicates
        /// </summary>
        public Dictionary<long, BlockingSessionInfo> BlockingSessions
        {
            get { return blockingSessions; }
            internal set { blockingSessions = value; }
        }

        public List<int> BlockedSpids 
        {
            get { return blockedSpids; }
            set { blockedSpids = value; }
        }

        public Dictionary<String, DatabaseStatistics> DbStatistics
        {
            get { return Server.DbStatistics; }
            internal set { Server.DbStatistics = value; }
        }
        
        /// <summary>
        /// List of all mirrored databases on this server
        /// </summary>
        public Dictionary<Guid, MirrorMonitoringDatabaseDetail> MirroredDatabases
        {
            get { return mirroredDatabases; }
            internal set { mirroredDatabases = value; }
        }

        /// <summary>
        /// Databases that are in the workload list of preferred mirroring configurations on this server
        /// </summary>
        public Dictionary<Guid, MirrorMonitoringDatabaseDetail> noLongerMirroredDatabases
        {
            get { return _noLongerMirroredDatabases; }
            internal set { _noLongerMirroredDatabases = value; }
        }
        
        public Dictionary<string, DiskDrive> DiskDrives
        {
            get { return diskDrives; }
            internal set { diskDrives = value; }
        }


        /// <summary>
        /// Gets or sets the monitored server.
        /// </summary>
        /// <value>The monitored server.</value>
        public MonitoredSqlServer MonitoredServer
        {
            get { return monitoredServer; }
            private set { 
                monitoredServer = value;
                if (monitoredServer != null)
                {
                    this.Id = monitoredServer.Id;
                }
            }
        }

        

        public ScheduledRefreshAlerts Alerts
        {
            get { return alerts; }
            internal set { alerts = value; }
        }


        public ReplicationSummary Replication
        {
            get { return replication; }
            internal set { replication = value; }
        }

		public SessionSnapshot SessionList
        {
            get { return sessionList; }
            internal set { 
                sessionList = value; 
                if (Server != null)
                {
                    Server.SystemProcesses = SessionList.SystemProcesses;
                }
            }
        }

        public LockDetails LockList
        {
            get { return lockList; }
            internal set { lockList = value; }
        }

        //public Exception DatabaseSizeError
        //{
        //    get { return databaseSizeError; }
        //    internal set { databaseSizeError = value; }
        //}

        public Exception QueryMonitorError
        {
            get { return queryMonitorError; }
            internal set { queryMonitorError = value; }
        }

        public Exception ActivityMonitorError
        {
            get { return activityMonitorError; }
            internal set { activityMonitorError = value; }
        }

        /// <summary>
        /// A failure here should not normally be alerted on because of common false-positives
        /// </summary>
        public Exception OldestOpenTransactionError
        {
            get { return oldestOpenTransactionError; }
            internal set { oldestOpenTransactionError = value; }
        }


        public Exception ReplicationError
        {
            get { return replicationError; }
            internal set { replicationError = value; }
        }

        public Exception MirroringError
        {
            get { return mirroringError; }
            internal set { mirroringError = value; }
        }

        public Memory MemoryStatistics
        {
            get { return memoryStatistics; }
            internal set { memoryStatistics = value; }
        }


        public Dictionary<int, CustomCounterSnapshot> CustomCounters
        {
            get { return customCounters; }
            internal set { customCounters = value; }
        }


        internal Dictionary<int, Dictionary<int, Pair<int, string>>> ReorgRetryTables
        {
            get { return reorgRetryTables; }
            set { reorgRetryTables = value; }
        }

        //internal List<int> ReorgRetryDatabases
        //{
        //    get { return reorgRetryDatabases; }
        //    set { reorgRetryDatabases = value; }
        //}

        public TableFragmentationCollectorStatus TableFragmentationStatus
        {
            get { return tableFragmentationStatus; }
            set { tableFragmentationStatus = value; }
        }


        public bool RequestReconfiguration
        {
            get { return requestReconfiguration; }
            set { requestReconfiguration = value; }
        }

        public WaitStatisticsSnapshot WaitStats
        {
            get { return waitStats; }
            internal set { waitStats = value; }
        }


        public FileSize TempdbFileSize
        {
            get
            {
                if (tempdbFileSize.Kilobytes == 0)
                {
                    if (DbStatistics != null && DbStatistics.ContainsKey("tempdb") &&
                        DbStatistics["tempdb"].Files != null
                        && DbStatistics["tempdb"].Files.Count > 0)
                    {
                        foreach (FileActivityFile file in DbStatistics["tempdb"].Files.Values)
                        {

                            if (file is TempdbFileActivity)
                            {
                                if (((TempdbFileActivity) file).VersionStore != null)
                                {
                                    versionStoreSize.Kilobytes +=
                                        ((TempdbFileActivity) file).VersionStore.Kilobytes;
                                }
                                if (((TempdbFileActivity) file).FileSize != null)
                                {
                                    tempdbFileSize.Kilobytes += ((TempdbFileActivity) file).FileSize.Kilobytes;
                                }
                            }
                        }
                    }
                }
                return tempdbFileSize;
            }
        }

        public FileSize VersionStoreSize
        {
            get
            {
                if (versionStoreSize.Kilobytes == 0)
                {
                    if (DbStatistics != null && DbStatistics.ContainsKey("tempdb") &&
                        DbStatistics["tempdb"].Files != null
                        && DbStatistics["tempdb"].Files.Count > 0)
                    {
                        foreach (FileActivityFile file in DbStatistics["tempdb"].Files.Values)
                        {

                            if (file is TempdbFileActivity)
                            {
                                if (((TempdbFileActivity)file).VersionStore != null)
                                {
                                    versionStoreSize.Kilobytes +=
                                        ((TempdbFileActivity)file).VersionStore.Kilobytes;
                                }
                                if (((TempdbFileActivity)file).FileSize != null)
                                {
                                    tempdbFileSize.Kilobytes += ((TempdbFileActivity)file).FileSize.Kilobytes;
                                }
                            }
                        }
                    }
                }
                return versionStoreSize;
            }
            internal set { versionStoreSize = value; }
        }

        /// <summary>
        /// Current snapshot for the AlwaysOn Availability Groups.
        /// </summary>
        public AlwaysOnAvailabilityGroupsSnapshot AvailabilityGroupsSnapshot
        {
            get { return availabilityGroupsSnapshot; }
            set { availabilityGroupsSnapshot = value; }
        }

        /// <summary>
        /// Configuration snapshot.
        /// </summary>
        public ConfigurationSnapshot ConfigurationDetails
        {
            get { return configurationDetails; }
            set { configurationDetails = value; }
        }

        //public List<SQLsafeOperation> SQLsafeOperations
        //{
        //    get { return ss_Operations; }
        //    set { ss_Operations = value; }
        //}
        
        #endregion

        #region events

        #endregion

        #region methods

        #endregion

        #region interface implementations

        #endregion

        #region nested types

        #endregion
    }
}
