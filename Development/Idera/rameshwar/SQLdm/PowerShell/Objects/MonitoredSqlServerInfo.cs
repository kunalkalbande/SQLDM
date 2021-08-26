using BBS.TracerX;
using Idera.SQLdm.Common.Attributes;
using Idera.SQLdm.Common.Snapshots;

namespace Idera.SQLdm.PowerShell.Objects
{
    using System;
    using System.Collections.Generic;
    using System.Management.Automation;
    using System.Net;
    using Helpers;
    using Idera.SQLdm.Common.Configuration;
    using Idera.SQLdm.Common.Objects;
    using Wintellect.PowerCollections;

    public class MonitoredSqlServerInfo : ICloneable
    {
        private static Logger LOG = Logger.GetLogger("MonitoredSqlServerInfo");
        private SQLdmDriveInfo drive;
        private int id;
        private SqlConnectionInfo connectionInfo;
        private bool isActive;

        private Guid collectionServiceId;
        private int scheduledCollectionIntervalMinutes;
        private QueryMonitorConfigurationInfo queryMonitorConfiguration;
        private ActivityMonitorConfigurationInfo activityMonitorConfiguration;
        private MaintenanceModeConfigurationInfo maintenanceModeConfiguration;
        private TableStatisticsCollectionConfigurationInfo tableStatisticsCollectionConfiguration;
        private bool replicationMonitoringDisabled;
        private bool extendedHistoryCollectionDisabled;
        private bool oleAutomationDisabled;
        private Set<string> tags;
        private DiskCollectionSettings diskSettings;
        private int inputBufferLimiter = 500;
        private bool inputBufferLimited;
        private string activeClusterNode;
        private string preferredClusterNode;
        private ActiveWaitsConfiguration activeWaitsConfiguration;
        private ClusterCollectionSetting clusterCollectionSetting = ClusterCollectionSetting.Default;


        public enum OleAutomationExecutionContext : int { InProc = 1, OutOfProc = 4, Both = 5 };
        
        private TimeSpan databaseStatisticsInterval = TimeSpan.FromMinutes(60);

        private bool maintenanceModeEnabled;
        private TimeSpan customCounterTimeout = TimeSpan.FromSeconds(180);
        private List<int> customCounters;
        private TimeSpan serverPingInterval = new TimeSpan(0, 0, 30);
        private VirtualizationConfiguration virtualizationConfiguration;
        private BaselineConfiguration baselineConfiguration;
        //10.0 SQLdm srishti purohit -- doctors UI configuration object
        private AnalysisConfiguration analysisConfiguration;
        
        // Flag that indicates whether the alerts are going to be refreshed in minutes or in seconds
        private bool alertRefreshInMinutes = true;
        //10.1 SQLdm srishti purohit -- Powershell Snap-in Added Functions
        private string friendlyServerName;
        private int serverVersion;

        // Display flags
        public WmiConfiguration WmiConfig { get; set; }

        public MonitoredSqlServerInfo(SQLdmDriveInfo drive)
        {
            isActive = true;            
            scheduledCollectionIntervalMinutes = 6;
            queryMonitorConfiguration = null;
            activityMonitorConfiguration = null;
            maintenanceModeConfiguration = null;
            tableStatisticsCollectionConfiguration = null;
            connectionInfo = new SqlConnectionInfo();
            diskSettings = new DiskCollectionSettings();
            WmiConfig = new WmiConfiguration();
            OLEAutomationDisabled = true;
            this.drive = drive;
            activeWaitsConfiguration = new ActiveWaitsConfiguration(-1);
        }

        public MonitoredSqlServerInfo(MonitoredSqlServer mss, SQLdmDriveInfo drive)
        {
            id = mss.Id;
            collectionServiceId = mss.CollectionServiceId;
            connectionInfo = mss.ConnectionInfo;
            scheduledCollectionIntervalMinutes = (int)mss.ScheduledCollectionInterval.TotalMinutes;
            maintenanceModeConfiguration = new MaintenanceModeConfigurationInfo(mss.MaintenanceMode);
            queryMonitorConfiguration = new QueryMonitorConfigurationInfo(mss.QueryMonitorConfiguration);
            activityMonitorConfiguration = new ActivityMonitorConfigurationInfo(mss.ActivityMonitorConfiguration);
            tableStatisticsCollectionConfiguration = new TableStatisticsCollectionConfigurationInfo(mss);
            replicationMonitoringDisabled = mss.ReplicationMonitoringDisabled;
            extendedHistoryCollectionDisabled = mss.ExtendedHistoryCollectionDisabled;
            oleAutomationDisabled = mss.OleAutomationUseDisabled;
            diskSettings = mss.DiskCollectionSettings;
            inputBufferLimiter = mss.InputBufferLimiter;
            //Capturing current value of Friendly Name for instance in object
            //SQLdm 10.1 (Srishti Purohit) Power shell 
            friendlyServerName = mss.FriendlyServerName;
            if(mss.MostRecentSQLVersion != null)
                serverVersion = mss.MostRecentSQLVersion.Major;

            inputBufferLimited = mss.InputBufferLimited;
            activeClusterNode = mss.ActiveClusterNode;
            preferredClusterNode = mss.PreferredClusterNode;
            activeWaitsConfiguration = mss.ActiveWaitsConfiguration;
            clusterCollectionSetting = mss.ClusterCollectionSetting;
            maintenanceModeEnabled = mss.MaintenanceModeEnabled;
            databaseStatisticsInterval = mss.DatabaseStatisticsInterval;
            customCounters = mss.CustomCounters;
            ReplicationMonitoringDisabled = mss.ReplicationMonitoringDisabled;
            ExtendedHistoryCollectionDisabled = mss.ExtendedHistoryCollectionDisabled;
            serverPingInterval = mss.ServerPingInterval;
            virtualizationConfiguration = mss.VirtualizationConfiguration;
            baselineConfiguration = mss.BaselineConfiguration;
            analysisConfiguration = mss.AnalysisConfiguration;

            var wmi = mss.WmiConfig;
            WmiConfig = new WmiConfiguration();
            if (wmi != null)
            {
                WmiConfig.OleAutomationDisabled = wmi.OleAutomationDisabled;
                WmiConfig.DirectWmiEnabled = wmi.DirectWmiEnabled;
                WmiConfig.DirectWmiConnectAsCollectionService = wmi.DirectWmiConnectAsCollectionService;
                WmiConfig.DirectWmiUserName = wmi.DirectWmiUserName;
                WmiConfig.DirectWmiPassword = wmi.DirectWmiPassword;
            }
            else
            {
                WmiConfig.DirectWmiUserName = null;
            }


            this.drive = drive;
            if (mss.HasTags)
            {
                tags = GetTagNames(mss.InstanceName, mss.Tags, drive); 
            }
            isActive = mss.IsActive;

        }

        public MonitoredSqlServerInfo(MonitoredSqlServerInfo copy)
        {
            connectionInfo = copy.connectionInfo.Clone();
            id = copy.id;
            collectionServiceId = copy.collectionServiceId;
            isActive = copy.IsActive;
            scheduledCollectionIntervalMinutes = copy.ScheduledCollectionIntervalMinutes;
            queryMonitorConfiguration = (QueryMonitorConfigurationInfo)copy.QueryMonitorConfiguration.Clone();
            activityMonitorConfiguration = (ActivityMonitorConfigurationInfo)copy.ActivityMonitorConfiguration.Clone();
            maintenanceModeConfiguration = (MaintenanceModeConfigurationInfo)copy.MaintenanceMode.Clone();
            tableStatisticsCollectionConfiguration =
                (TableStatisticsCollectionConfigurationInfo) copy.TableStatisticsCollectionConfiguration.Clone();

            replicationMonitoringDisabled = copy.replicationMonitoringDisabled;
            extendedHistoryCollectionDisabled = copy.extendedHistoryCollectionDisabled;
            oleAutomationDisabled = copy.OLEAutomationDisabled;
            diskSettings = copy.DiskSettings;
            inputBufferLimiter = copy.InputBufferLimiter;
            //Capturing current value of Friendly Name for instance in object
            //SQLdm 10.1 (Srishti Purohit) Power shell 
            friendlyServerName = copy.FriendlyServerName;
            serverVersion = copy.serverVersion;

            inputBufferLimited = copy.InputBufferLimited;
            activeClusterNode = copy.ActiveClusterNode;
            preferredClusterNode = copy.PreferredClusterNode;
            activeWaitsConfiguration = copy.ActiveWaitsConfiguration;
            clusterCollectionSetting = copy.ClusterCollectionSetting;
            
            maintenanceModeEnabled = copy.MaintenanceModeEnabled;
            databaseStatisticsInterval = copy.databaseStatisticsInterval;
            customCounters = copy.CustomCounters;
            ReplicationMonitoringDisabled = copy.ReplicationMonitoringDisabled;
            ExtendedHistoryCollectionDisabled = copy.ExtendedHistoryCollectionDisabled;
            serverPingInterval = copy.ServerPingInterval;
            virtualizationConfiguration = copy.VirtualizationConfiguration;
            baselineConfiguration = copy.BaselineConfiguration;
            analysisConfiguration = copy.AnalysisConfiguration;

            var wmi = copy.WmiConfig;
            WmiConfig = new WmiConfiguration();
            if (wmi != null)
            {
                WmiConfig.OleAutomationDisabled = wmi.OleAutomationDisabled;
                WmiConfig.DirectWmiEnabled = wmi.DirectWmiEnabled;
                WmiConfig.DirectWmiConnectAsCollectionService = wmi.DirectWmiConnectAsCollectionService;
                WmiConfig.DirectWmiUserName = wmi.DirectWmiUserName;
                WmiConfig.DirectWmiPassword = wmi.DirectWmiPassword;
            }
            else
            {
                WmiConfig.DirectWmiUserName = null;
            }

            //
            // drive necessary to have connection info for querying tag info
            drive = copy.drive;
            // copy the set of tags
            if (copy.tags != null && copy.tags.Count > 0)
                tags = new Set<string>(copy.tags, new CaseInsensitiveEqualityComparer());
        }

        /// <summary>
        /// Drive used to create this here object.  May be null if drive was created from a casting conversion operator.
        /// </summary>
        internal SQLdmDriveInfo Drive
        {
            get { return drive;  }
            set { drive = value; }
        }

        internal int Id
        {
            get { return id;  }
            set { id = value; }
        }

        internal PSCredential Credential
        {
            set
            {
                if (value == null || String.IsNullOrEmpty(value.UserName))
                {
                    connectionInfo.UseIntegratedSecurity = true;
                    connectionInfo.UserName = null;
                    connectionInfo.Password = null;
                } else
                {
                    NetworkCredential netCred = value.GetNetworkCredential();
                    connectionInfo.UserName = netCred.UserName;
                    connectionInfo.Password = netCred.Password;
                    connectionInfo.UseIntegratedSecurity = String.IsNullOrEmpty(netCred.UserName);
                }
            }
        }

        public string InstanceName
        {
            get { return connectionInfo.InstanceName; }
            internal set { connectionInfo.InstanceName = value; }
        }

        [Auditable("Encrypted Data")]
        public bool EncryptData
        {
            get { return connectionInfo.EncryptData;  }
            set { connectionInfo.EncryptData = value; }
        }

        [Auditable("Enabled Trust Server Certificate encryption option")]
        public bool TrustServerCertificate
        {
            get { return connectionInfo.TrustServerCertificate;  }
            set { connectionInfo.TrustServerCertificate = value; }
        }

        public bool IsActive
        {
            get { return isActive; }
            set { isActive = value; }
        }

        public MaintenanceModeConfigurationInfo MaintenanceMode 
        {
            get
            {
                if (maintenanceModeConfiguration == null)
                    maintenanceModeConfiguration = new MaintenanceModeConfigurationInfo();

                return maintenanceModeConfiguration;
            }
            set { maintenanceModeConfiguration = value; }
        }


        public int ScheduledCollectionIntervalMinutes
        {
            get { return scheduledCollectionIntervalMinutes; }
            set
            {
                if (scheduledCollectionIntervalMinutes <= 0)
                {
                    throw new ArgumentOutOfRangeException("ScheduledCollectionIntervalMinutes",
                                                    "The scheduled collection interval must be greater than zero.");
                }

                scheduledCollectionIntervalMinutes = value;
            }
        }

        public QueryMonitorConfigurationInfo QueryMonitorConfiguration
        {
            get
            {
                if (queryMonitorConfiguration == null)
                    queryMonitorConfiguration = new QueryMonitorConfigurationInfo();

                return queryMonitorConfiguration;
            }
            set { queryMonitorConfiguration = value; }
        }

        public ActivityMonitorConfigurationInfo ActivityMonitorConfiguration
        {
            get
            {
                if (activityMonitorConfiguration == null)
                    activityMonitorConfiguration = new ActivityMonitorConfigurationInfo();

                return activityMonitorConfiguration;
            }
            set { activityMonitorConfiguration = value; }
        }

        // Currently unsupported to change

        internal DiskCollectionSettings DiskSettings
        {
            get { return diskSettings; }
        }

        internal ClusterCollectionSetting ClusterCollectionSetting
        {
            get { return clusterCollectionSetting; }
        }

        internal ActiveWaitsConfiguration ActiveWaitsConfiguration
        {
            get { return activeWaitsConfiguration; }
        }

        internal int InputBufferLimiter
        {
            get { return inputBufferLimiter; }
            set { inputBufferLimiter = value; }
        }

        internal bool InputBufferLimited
        {
            get { return inputBufferLimited; }
            set { inputBufferLimited = value; }
        }

        internal bool AlertRefreshInMinutes
        {
            get { return alertRefreshInMinutes; }
            set { alertRefreshInMinutes = value; }
        }

        internal BaselineConfiguration BaselineConfiguration
        {
            get { return baselineConfiguration; }
        }

        internal VirtualizationConfiguration VirtualizationConfiguration
        {
            get { return virtualizationConfiguration; }
        }

        //10.0 SQLdm srishti purohit -- doctors UI configuration object
        
        internal AnalysisConfiguration AnalysisConfiguration
        {
            get { return analysisConfiguration; }
            set { analysisConfiguration = value; }
        }
        internal TimeSpan ServerPingInterval
        {
            get { return serverPingInterval; }
        }

        internal List<int> CustomCounters
        {
            get { return customCounters; }
        }

        internal TimeSpan CustomCounterTimeout
        {
            get { return customCounterTimeout; }
        }

        internal bool MaintenanceModeEnabled
        {
            get { return maintenanceModeEnabled; }
            set { maintenanceModeEnabled = value; }
        }

        

        internal string ActiveClusterNode
        {
            get { return activeClusterNode; }
        }

        internal string PreferredClusterNode
        {
            get { return preferredClusterNode; }
        }

        internal string FriendlyServerName
        {
            get { return friendlyServerName; }
            set { friendlyServerName = value; }
        }
        internal int ServerVersion
        {
            get { return serverVersion; }
            set { serverVersion = value; }
        }
        // end unsupported to change

        internal int DatabaseStatisticsIntervalMinutes
        {
            get { return Convert.ToInt32(databaseStatisticsInterval.TotalMinutes); }
            set
            {
                if (value < 1)
                {
                    throw new ArgumentOutOfRangeException("DatabaseStatisticsIntervalMinutes",
                                                    "The database collection interval must be at least 1 minute.");
                } 
                databaseStatisticsInterval = TimeSpan.FromMinutes(value);
            }
        }

        //SQLDM-30516. Add New parameter for setting "Alert if server is inaccessible" in general settings
        internal int ServerInaccessibleAlertIntervalMinutes
        {
            get { return Convert.ToInt32(serverPingInterval.TotalMinutes); }
            set
            {
                if (value > 10)
                {
                    throw new ArgumentOutOfRangeException("ServerInAccessibleAlertMinutes",
                                                    "The Server InAccessible Alert Minutes must be less than 10 minutes.");
                }
                serverPingInterval = TimeSpan.FromMinutes(value);
            }
        }

        public TableStatisticsCollectionConfigurationInfo TableStatisticsCollectionConfiguration
        {
            get
            {
                if (tableStatisticsCollectionConfiguration == null)
                    tableStatisticsCollectionConfiguration = new TableStatisticsCollectionConfigurationInfo();

                return tableStatisticsCollectionConfiguration;
            }
            set { tableStatisticsCollectionConfiguration = value; }
        }

        public void AddTags(IEnumerable<string> values)
        {
            if (values != null)
            {
                GetTags().AddMany(values);
            }
        }

        public void RemoveTags(IEnumerable<string> values)
        {
            if (values != null)
            {
                GetTags().RemoveMany(values);
            }
        }

        private Set<string> GetTags()
        {
            if (tags == null)
                tags = new Set<string>(new CaseInsensitiveEqualityComparer());

            return tags;
        }

        public string[] Tags
        {
            get { return Algorithms.Sort(GetTags());  }
            set
            {
                tags = null;
                if (value != null)
                {
                    GetTags().AddMany(value);
                }
            }
        }

//        public List<int> CustomCounters
//        {
//            get
//            {
//                return customCounters;
//            }
//            set
//            {
//                if (value == null)
//                    customCounters = null;
//                else
//                    customCounters = new List<int>(value);
//            }
//        }

        internal static Set<string> GetTagNames(string instanceName, IEnumerable<int> tagIds, SQLdmDriveInfo drive)
        {
            using (LOG.DebugCall("GetTagNames"))
            {
                Tag tag;
                Set<string> result = new Set<string>(new CaseInsensitiveEqualityComparer());

                Dictionary<int, Tag> tagsByName = drive.Tags;
                foreach (int tagId in tagIds)
                {
                    if (tagsByName.TryGetValue(tagId, out tag))
                    {
                        result.Add(tag.Name);
                    }
                    else
                    {
                        String message =
                            String.Format("Tag id '{0}' for server '{1}' is invalid.", tagId, instanceName);
                        LOG.Error(message);
                        throw new ApplicationException(message);
                    }
                }
                return result;
            }
        }

        internal static Set<int> GetTagIds(string instanceName, IEnumerable<string> tagNames, SQLdmDriveInfo drive)
        {
            using (LOG.DebugCall("GetTagIds"))
            {
                Tag tag;
                Set<int> result = new Set<int>();

                Dictionary<string, Tag> tagsByName = drive.TagsByName;
                foreach (string tagName in tagNames)
                {
                    if (tagsByName.TryGetValue(tagName.ToLower(), out tag))
                    {
                        result.Add(tag.Id);
                    }
                    else
                    {
                        String message =
                            String.Format("Tag name '{0}' for server '{1}' is invalid.", tagName, instanceName);
                        LOG.Error(message);
                        throw new ApplicationException(message);
                    }
                }
                return result;
            }
        }


        public static explicit operator MonitoredSqlServerConfiguration(MonitoredSqlServerInfo input)
        {
            using (LOG.DebugCall())
            {
                MonitoredSqlServerConfiguration result = new MonitoredSqlServerConfiguration(input.connectionInfo);
                result.ScheduledCollectionInterval = TimeSpan.FromMinutes(input.ScheduledCollectionIntervalMinutes);
                result.CollectionServiceId = input.collectionServiceId;

                // Query Monitor settings
                result.QueryMonitorConfiguration = input.QueryMonitorConfiguration.GetInternalConfiguration();

                //Activity Monitor
                result.ActivityMonitorConfiguration = input.ActivityMonitorConfiguration.GetInternalConfiguration();

                // Quiet time collection settings
                TableStatisticsCollectionConfigurationInfo tscci = input.TableStatisticsCollectionConfiguration;
                result.ReorgStatisticsDays = (short) tscci.CollectionDays;
                result.ReorgStatisticsStartTime = SQLdmProvider.Jan_1_1900 + tscci.CollectionStartTime;
                result.GrowthStatisticsDays = (short) tscci.CollectionDays;
                result.GrowthStatisticsStartTime = SQLdmProvider.Jan_1_1900 + tscci.CollectionStartTime;
                result.TableStatisticsExcludedDatabases = tscci.ExcludedDatabases;
                result.ReorganizationMinimumTableSize.Kilobytes = tscci.ReorgMinimumTableSizeK;

                // Maintenance Mode settings
                result.MaintenanceMode = input.MaintenanceMode.GetInternalConfiguration();
                result.MaintenanceModeEnabled = result.MaintenanceMode.MaintenanceModeType == MaintenanceModeType.Always;

                result.ReplicationMonitoringDisabled = input.replicationMonitoringDisabled;
                result.ExtendedHistoryCollectionDisabled = input.extendedHistoryCollectionDisabled;
                result.OleAutomationDisabled = input.oleAutomationDisabled;


                result.DiskCollectionSettings = input.diskSettings;
                result.InputBufferLimiter = input.InputBufferLimiter;
                result.InputBufferLimited = input.InputBufferLimited;
                result.ActiveClusterNode = input.ActiveClusterNode;
                result.PreferredClusterNode = input.PreferredClusterNode;
                result.ActiveWaitsConfiguration = input.ActiveWaitsConfiguration;
                result.ClusterCollectionSetting = input.ClusterCollectionSetting;

                result.MaintenanceModeEnabled = input.MaintenanceModeEnabled;
                result.DatabaseStatisticsInterval = input.databaseStatisticsInterval;
                result.CustomCounters = input.CustomCounters;
                result.ReplicationMonitoringDisabled = input.ReplicationMonitoringDisabled;
                result.ExtendedHistoryCollectionDisabled = input.ExtendedHistoryCollectionDisabled;
                result.ServerPingInterval = input.ServerPingInterval;
                result.VirtualizationConfiguration = input.VirtualizationConfiguration;
                result.BaselineConfiguration = input.BaselineConfiguration;
                result.AnalysisConfiguration = input.AnalysisConfiguration;

                var wmi = input.WmiConfig;
                result.WmiConfig = new WmiConfiguration();
                if (wmi != null)
                {
                    result.WmiConfig.OleAutomationDisabled = wmi.OleAutomationDisabled;
                    result.WmiConfig.DirectWmiEnabled = wmi.DirectWmiEnabled;
                    result.WmiConfig.DirectWmiConnectAsCollectionService = wmi.DirectWmiConnectAsCollectionService;
                    result.WmiConfig.DirectWmiUserName = wmi.DirectWmiUserName;
                    result.WmiConfig.DirectWmiPassword = wmi.DirectWmiPassword;
                }
                else
                {
                    result.WmiConfig.DirectWmiUserName = null;
                }

                //START SQLdm 10.1 (Srishti Purohit): Powershell Snap-in Added Functions
               
                result.FriendlyServerName = input.friendlyServerName;
                //END SQLdm 10.1 (Srishti Purohit): Powershell Snap-in Added Functions


                Tag tag;
                string[] tags = input.Tags;
                if (tags.Length > 0)
                {
                    if (input.Drive != null)
                    {
                        Set<int> tagIds = GetTagIds(input.InstanceName, tags, input.Drive);
                        result.Tags = new List<int>(tagIds);
                    }
                    else
                        LOG.WarnFormat(
                            "MonitoredSqlServerInfo '{0}' does not have a valid drive assigned.  (Tags may be whack in configuration object)",
                            input.InstanceName);
                }
                return result;
            }
        }

//        public static explicit operator MonitoredSqlServerInfo(MonitoredSqlServerConfiguration input)
//        {
//            MonitoredSqlServerInfo result = new MonitoredSqlServerInfo();
//            result.connectionInfo = input.ConnectionInfo;
//            result.ExtendedHistoryCollectionDisabled = input.ExtendedHistoryCollectionDisabled;
//            result.IsActive = input.IsActive;
//            result.MaintenanceMode = new MaintenanceModeConfigurationInfo(input.MaintenanceMode);
//            result.QueryMonitorConfiguration = new QueryMonitorConfigurationInfo(input.QueryMonitorConfiguration);
//            result.ReplicationMonitoringDisabled = input.ReplicationMonitoringDisabled;
//            result.ScheduledCollectionIntervalMinutes = (int)input.ScheduledCollectionInterval.TotalMinutes;
//            result.TableStatisticsCollectionConfiguration = new TableStatisticsCollectionConfigurationInfo(input);
//
//            return null;
//        }

//        public static explicit operator MonitoredSqlServerInfo(MonitoredSqlServer server)
//        {
//            return new MonitoredSqlServerInfo(server);
//        }

        [Auditable("Disabled collection of replication statistics")]
        public bool ReplicationMonitoringDisabled
        {
            get { return replicationMonitoringDisabled; }
            set { replicationMonitoringDisabled = value; }
        }

        [Auditable("Disabled extended history collection")]
        public bool ExtendedHistoryCollectionDisabled
        {
            get { return extendedHistoryCollectionDisabled; }
            set { extendedHistoryCollectionDisabled = value; }
        }

        public bool OLEAutomationDisabled
        {
            get { return oleAutomationDisabled; }
            set { oleAutomationDisabled = value; }
        }


        #region ICloneable Members

        public object Clone()
        {
            return new MonitoredSqlServerInfo(this);
        }

        #endregion

    }
}