using System;
using System.Collections.Generic;
using Idera.SQLdm.Common.Attributes;
using Idera.SQLdm.Common.Auditing;
using Idera.SQLdm.Common.Configuration;


namespace Idera.SQLdm.Common.Objects
{
    using Snapshots;

    [Serializable]
    public sealed class MonitoredSqlServer:IAuditable
    {
        public enum OleAutomationExecutionContext : int { InProc = 1, OutOfProc = 4, Both = 5 };

        private int id;
        private DateTime registeredDate;
        private DateTime earliestData;
        private SqlConnectionInfo connectionInfo;
        private bool isActive;
        private Guid collectionServiceId;
        private TimeSpan scheduledCollectionInterval;
        private TimeSpan databaseStatisticsInterval = TimeSpan.FromMinutes(60);
        private bool maintenanceModeEnabled;
        private QueryMonitorConfiguration queryMonitorConfiguration;
        private ActivityMonitorConfiguration activityMonitorConfiguration;

        
        //private DateTime? reorgStatisticsStartTime;
       //private DateTime? lastReorgStatisticsRunTime;
        
        //private DateTime? previousReorgStatisticsRunTime;
        
        //private Int16? reorgStatisticsDays;
        //private List<string> tableStatisticsExcludedDatabases;
        private TimeSpan customCounterTimeout = TimeSpan.FromSeconds(180);
        private List<int> customCounters;
        private bool replicationMonitoringDisabled;
        private MaintenanceMode maintenanceMode;
        private bool extendedHistoryCollectionDisabled = false;
  //      private bool oleAutomationDisabled = false;
        private List<int> tags;
        private DiskCollectionSettings diskSettings;
        private int inputBufferLimiter = 500;
        private bool inputBufferLimited = false;
        private string activeClusterNode = null;
        private string preferredClusterNode = null;
        private ActiveWaitsConfiguration activeWaitsConfiguration;
        private ClusterCollectionSetting clusterCollectionSetting = Snapshots.ClusterCollectionSetting.Default;
        private TimeSpan serverPingInterval = new TimeSpan(0, 0, 30);
        private string friendlyServerName = null;
        private TableGrowthConfiguration tableGrowthConfiguration;
        private TableFragmentationConfiguration tableFragmentationConfiguration;
        private VirtualizationConfiguration virtualizationConfiguration;
        private BaselineConfiguration baselineConfiguration;
        //10.0 SQLdm srishti purohit -- doctors analysis UI configuration object
        private AnalysisConfiguration analysisConfiguration;
        
        //[START] SQLdm 10.0 (Rajesh Gupta): Baseline Enhancement - Added a list for multiple BaselineConfiguration 
        private Dictionary<int,BaselineConfiguration> baselineConfigurationList;
        //[END] SQLdm 10.0 (Rajesh Gupta): Baseline Enhancement - Added a list for multiple BaselineConfiguration         
        // Flag that indicates whether the alerts are going to be refreshed in minutes or in seconds
        private bool alertRefreshInMinutes = true;
        private int? cloudProviderId = null; //SQLdm 10.0 (Tarun Sapra)- get the cloud provider id for the monitored instance of the sql server
        public int ClusterRepoId { get; set; }
        public int RepoId { get; set; }
        // Display flags

        //START: SQLdm 10.0 (Tarun Sapra): Minimal Cloud Support
        public int? CloudProviderId 
        {
            get { return cloudProviderId; }
            set { cloudProviderId = value; }
        }
        //END: SQLdm 10.0 (Tarun Sapra): Minimal Cloud Support

        public ServerVersion MostRecentSQLVersion {get; set;}
        public string MostRecentSQLEdition { get; set; }
        public bool JobAlerts { get; set; }
        public bool ErrorlogAlerts { get; set; }

        public WmiConfiguration WmiConfig { get; set; }

        public SQLsafeRepositoryConfiguration SQLsafeConfig { get; set; }

       // SQLdm 10.1 (Barkha Khatri)adding a property for sysadmin check
        public bool IsUserSysAdmin { get; set; }

        
        public long? LastRunActiveUserSessions { get; set; } //SQLdm 8.5 (Gaurav Karwal): for instance heat map

        
        public int? LastRunCPUActivityPercentage { get; set; }  //SQLdm 8.5 (Gaurav Karwal): for instance heat map

        
        public double? LastRunSqlMemoryAllocatedInKilobytes { get; set; }  //SQLdm 8.5 (Gaurav Karwal): for instance heat map

        
        public double? LastRunSqlMemoryUsedInKilobytes { get; set; }  //SQLdm 8.5 (Gaurav Karwal): for instance heat map

        
        public int? LastRunIOActivityPercentage { get; set; }  //SQLdm 8.5 (Gaurav Karwal): for instance heat map


        private MonitoredSqlServer(int id, DateTime registeredDate)
        {
            if (id < 0)
            {
                throw new ArgumentOutOfRangeException("identifier",
                                                      "The instance identifier must be greater than or equal to zero.");
            }

            this.id = id;
            this.registeredDate = registeredDate;
            this.earliestData = registeredDate.ToLocalTime();
            tableGrowthConfiguration = new TableGrowthConfiguration(id);
            tableFragmentationConfiguration = new TableFragmentationConfiguration(id);
            JobAlerts = false;
            ErrorlogAlerts = false;
            WmiConfig = new WmiConfiguration();
        }


        public MonitoredSqlServer(
            int id,
            DateTime registeredDate,
            bool isActive,
            Guid collectionServiceId,
            SqlConnectionInfo connectionInfo,
            TimeSpan scheduledCollectionInterval,
            bool maintenanceModeEnabled,
            QueryMonitorConfiguration queryMonitorConfiguration,
            ActivityMonitorConfiguration activityMonitorConfiguration,
            MaintenanceMode maintenanceMode)
            : this(id, registeredDate)
        {
            if (connectionInfo == null)
            {
                throw new ArgumentNullException("connectionInfo");
            }

            this.isActive = isActive;
            this.collectionServiceId = collectionServiceId;
            this.connectionInfo = connectionInfo;
            this.scheduledCollectionInterval = scheduledCollectionInterval;
            this.maintenanceModeEnabled = maintenanceModeEnabled;
            this.queryMonitorConfiguration = queryMonitorConfiguration;
            this.activityMonitorConfiguration = activityMonitorConfiguration;
            this.maintenanceMode = maintenanceMode;
            this.activeWaitsConfiguration = new ActiveWaitsConfiguration(id);
            tableGrowthConfiguration = new TableGrowthConfiguration(id);
            tableFragmentationConfiguration = new TableFragmentationConfiguration(id);
        }

        public MonitoredSqlServer(
            int id,
            DateTime registeredDate,
            MonitoredSqlServerConfiguration configuration) : this(id, registeredDate)
        {
            if (configuration == null)
            {
                throw new ArgumentNullException("configuration");
            }
            tableGrowthConfiguration = new TableGrowthConfiguration(id);
            tableFragmentationConfiguration = new TableFragmentationConfiguration(id);
            maintenanceMode = new MaintenanceMode();
            connectionInfo = configuration.ConnectionInfo;
            isActive = configuration.IsActive;
            maintenanceModeEnabled = configuration.MaintenanceModeEnabled;
            scheduledCollectionInterval = configuration.ScheduledCollectionInterval;
            databaseStatisticsInterval = configuration.DatabaseStatisticsInterval;
            collectionServiceId = configuration.CollectionServiceId;
            queryMonitorConfiguration = configuration.QueryMonitorConfiguration;
            activityMonitorConfiguration = configuration.ActivityMonitorConfiguration;
            tableGrowthConfiguration.GrowthStatisticsStartTime = configuration.GrowthStatisticsStartTime;
            tableFragmentationConfiguration.FragmentationStatisticsStartTime = configuration.ReorgStatisticsStartTime;
            tableGrowthConfiguration.LastGrowthStatisticsRunTime = configuration.LastGrowthStatisticsRunTime;
            tableFragmentationConfiguration.LastFragmentationStatisticsRunTime= configuration.LastReorgStatisticsRunTime;
            tableFragmentationConfiguration.FragmentationStatisticsDays = configuration.ReorgStatisticsDays;
            tableGrowthConfiguration.GrowthStatisticsDays = configuration.GrowthStatisticsDays;
            tableGrowthConfiguration.TableStatisticsExcludedDatabases = configuration.TableStatisticsExcludedDatabases;
            tableFragmentationConfiguration.TableStatisticsExcludedDatabases = configuration.TableStatisticsExcludedDatabases;
            tableFragmentationConfiguration.FragmentationMinimumTableSize = configuration.ReorganizationMinimumTableSize;
            CustomCounters = configuration.CustomCounters;
            ReplicationMonitoringDisabled = configuration.ReplicationMonitoringDisabled;
            ExtendedHistoryCollectionDisabled = configuration.ExtendedHistoryCollectionDisabled;
            maintenanceMode.MaintenanceModeType = configuration.MaintenanceMode.MaintenanceModeType;
            maintenanceMode.MaintenanceModeStart = configuration.MaintenanceMode.MaintenanceModeStart;
            maintenanceMode.MaintenanceModeStop = configuration.MaintenanceMode.MaintenanceModeStop;
            maintenanceMode.MaintenanceModeDuration = configuration.MaintenanceMode.MaintenanceModeDuration;
            maintenanceMode.MaintenanceModeDays = configuration.MaintenanceMode.MaintenanceModeDays;
            maintenanceMode.MaintenanceModeRecurringStart = configuration.MaintenanceMode.MaintenanceModeRecurringStart;
            maintenanceMode.MaintenanceModeMonth = configuration.MaintenanceMode.MaintenanceModeMonth;
            maintenanceMode.MaintenanceModeSpecificDay = configuration.MaintenanceMode.MaintenanceModeSpecificDay;
            maintenanceMode.MaintenanceModeWeekOrdinal = configuration.MaintenanceMode.MaintenanceModeWeekOrdinal;
            maintenanceMode.MaintenanceModeWeekDay = configuration.MaintenanceMode.MaintenanceModeWeekDay;
            maintenanceMode.MaintenanceModeMonthDuration = configuration.MaintenanceMode.MaintenanceModeMonthDuration;
            maintenanceMode.MaintenanceModeMonthRecurringStart = configuration.MaintenanceMode.MaintenanceModeMonthRecurringStart;
            maintenanceMode.MaintenanceModeOnDemand = configuration.MaintenanceMode.MaintenanceModeOnDemand;
            tags = configuration.Tags;
            diskSettings = configuration.DiskCollectionSettings;
            inputBufferLimiter = configuration.InputBufferLimiter;
            inputBufferLimited = configuration.InputBufferLimited;
            activeClusterNode = configuration.ActiveClusterNode;
            preferredClusterNode = configuration.PreferredClusterNode;
            activeWaitsConfiguration = configuration.ActiveWaitsConfiguration;
            clusterCollectionSetting = configuration.ClusterCollectionSetting;
            serverPingInterval = configuration.ServerPingInterval;
            virtualizationConfiguration = configuration.VirtualizationConfiguration;
            baselineConfiguration = configuration.BaselineConfiguration;
			//[START] SQLdm 10.0 (Rajesh Gupta): Baseline Enhancement - Added a list for multiple BaselineConfiguration 
            baselineConfigurationList = configuration.BaselineConfigurationList;
            //[END] SQLdm 10.0 (Rajesh Gupta): Baseline Enhancement - Added a list for multiple BaselineConfiguration 
            analysisConfiguration = configuration.AnalysisConfiguration;

            var wmi = configuration.WmiConfig;
            WmiConfig.OleAutomationDisabled = wmi.OleAutomationDisabled;
            WmiConfig.DirectWmiEnabled = wmi.DirectWmiEnabled;
            WmiConfig.DirectWmiConnectAsCollectionService = wmi.DirectWmiConnectAsCollectionService;
            WmiConfig.DirectWmiUserName = wmi.DirectWmiUserName;
            WmiConfig.DirectWmiPassword = wmi.DirectWmiPassword;

            SQLsafeConfig = configuration.SQLsafeConfig;
            IsUserSysAdmin = configuration.IsUserSysAdmin;
            CloudProviderId = configuration.CloudProviderId;//SQLdm 10.0 (Tarun Sapra)- Minimal Cloud Support
            FriendlyServerName = configuration.FriendlyServerName;//10.3 Bug fix - SQLDM-25513 : [Friend names keep being deleted]
        }

        public ActiveWaitsConfiguration ActiveWaitsConfiguration
        {
            get { return activeWaitsConfiguration; }
            set{ activeWaitsConfiguration = value; }
        }

        public string ActiveClusterNode
        {
            get { return activeClusterNode; }
            set { activeClusterNode = value != null ? value.ToLower() : null; }
        }

        public string FriendlyServerName
        {
            get { return friendlyServerName; }
            set { friendlyServerName = value; }
        }

        public string DisplayInstanceName
        {
            get {
                string name = InstanceName;
                if (!string.IsNullOrEmpty(friendlyServerName))
                {
                    name = friendlyServerName + " (" + name + ")";
                }
                return name;
            }
        }

        public string PreferredClusterNode
        {
            get { return preferredClusterNode; }
            set { preferredClusterNode = value != null ? value.ToLower() : null; }
        }

        // This is not saved to the repository from the Management Service
        public ClusterCollectionSetting ClusterCollectionSetting
        {
            get { return clusterCollectionSetting; }
            set { clusterCollectionSetting = value; }
        }

        // Flag that indicates whether the alerts are going to be refreshed in minutes or in seconds.
        public bool AlertRefresInMinutes
        {
            get { return alertRefreshInMinutes; }
            set { alertRefreshInMinutes = value; }
        }

        public int Id
        {
            get { return id; }
        }

        public string InstanceName
        {
            get { return connectionInfo.InstanceName; }
        }

        public DateTime RegisteredDate
        {
            get { return registeredDate; }
        }

        public DateTime EarliestData 
        {
            get { return earliestData; }
            set { earliestData = value; }
        }

        public SqlConnectionInfo ConnectionInfo
        {
            get { return connectionInfo; }
        }

        public bool IsActive
        {
            get { return isActive; }
        }

        public bool MaintenanceModeEnabled
        {
            get { return maintenanceModeEnabled; }
            set { maintenanceModeEnabled = value; }
        }

        public MaintenanceMode MaintenanceMode
        {
            get { return (maintenanceMode); }
            set { maintenanceMode = value; }
        }

        [Auditable("Changed scheduled collection interval to")]
        public TimeSpan ScheduledCollectionInterval
        {
            get { return scheduledCollectionInterval; }
        }

        public TimeSpan DatabaseStatisticsInterval
        {
            get { return databaseStatisticsInterval; }
            set { databaseStatisticsInterval = value; }
        }

        public Guid CollectionServiceId
        {
            get { return collectionServiceId; }
        }

        public QueryMonitorConfiguration QueryMonitorConfiguration
        {
            get { return queryMonitorConfiguration; }
        }
        
        public ActivityMonitorConfiguration ActivityMonitorConfiguration
        {
            get { return activityMonitorConfiguration; }
        }
        
        public DiskCollectionSettings DiskCollectionSettings
        {
            get
            {
                if (diskSettings == null)
                    diskSettings = new DiskCollectionSettings();
                return diskSettings;
            }
            set
            {
                diskSettings = value;
            }
        }

        [Auditable("Changed limit of DBCC Input Buffer executions to")]
        public int InputBufferLimiter
        {
            get { return inputBufferLimiter; }
            set { inputBufferLimiter = value; }
        }

        [Auditable("Enabled limit for DBCC Input Buffer executions")]
        public bool InputBufferLimited
        {
            get { return inputBufferLimited; }
            set { inputBufferLimited = value; }
        }


        public MonitoredSqlServerConfiguration GetConfiguration()
        {
            MonitoredSqlServerConfiguration configuration = new MonitoredSqlServerConfiguration(connectionInfo);
            configuration.IsActive = isActive;
            configuration.CollectionServiceId = collectionServiceId;
            configuration.ScheduledCollectionInterval = scheduledCollectionInterval;
            configuration.DatabaseStatisticsInterval = databaseStatisticsInterval;
            configuration.MaintenanceModeEnabled = maintenanceModeEnabled;
            configuration.QueryMonitorConfiguration = queryMonitorConfiguration;
            configuration.ActivityMonitorConfiguration = activityMonitorConfiguration;
            configuration.GrowthStatisticsStartTime = tableGrowthConfiguration.GrowthStatisticsStartTime;
            configuration.LastGrowthStatisticsRunTime = tableGrowthConfiguration.LastGrowthStatisticsRunTime;
            configuration.LastReorgStatisticsRunTime =
                tableFragmentationConfiguration.LastFragmentationStatisticsRunTime;
            configuration.ReorgStatisticsStartTime = tableFragmentationConfiguration.FragmentationStatisticsStartTime;
            configuration.GrowthStatisticsDays = tableGrowthConfiguration.GrowthStatisticsDays;
            configuration.ReorgStatisticsDays = tableFragmentationConfiguration.FragmentationStatisticsDays;
            configuration.TableStatisticsExcludedDatabases = tableGrowthConfiguration.TableStatisticsExcludedDatabases;
            configuration.TableStatisticsExcludedDatabases = tableFragmentationConfiguration.TableStatisticsExcludedDatabases;
            configuration.ReorganizationMinimumTableSize = ReorganizationMinimumTableSize;
            if (HasCustomCounters)
                configuration.CustomCounters = new List<int>(CustomCounters);
            configuration.ReplicationMonitoringDisabled = ReplicationMonitoringDisabled;
            configuration.ExtendedHistoryCollectionDisabled = ExtendedHistoryCollectionDisabled;
            configuration.MaintenanceMode.MaintenanceModeType = maintenanceMode.MaintenanceModeType;
            configuration.MaintenanceMode.MaintenanceModeStart = maintenanceMode.MaintenanceModeStart;
            configuration.MaintenanceMode.MaintenanceModeStop = maintenanceMode.MaintenanceModeStop;
            configuration.MaintenanceMode.MaintenanceModeDuration = maintenanceMode.MaintenanceModeDuration;
            configuration.MaintenanceMode.MaintenanceModeDays = maintenanceMode.MaintenanceModeDays;
            configuration.MaintenanceMode.MaintenanceModeRecurringStart = maintenanceMode.MaintenanceModeRecurringStart;
            configuration.MaintenanceMode.MaintenanceModeMonth = maintenanceMode.MaintenanceModeMonth;
            configuration.MaintenanceMode.MaintenanceModeSpecificDay = maintenanceMode.MaintenanceModeSpecificDay;
            configuration.MaintenanceMode.MaintenanceModeWeekOrdinal = maintenanceMode.MaintenanceModeWeekOrdinal;
            configuration.MaintenanceMode.MaintenanceModeWeekDay = maintenanceMode.MaintenanceModeWeekDay;
            configuration.MaintenanceMode.MaintenanceModeMonthDuration = maintenanceMode.MaintenanceModeMonthDuration;
            configuration.MaintenanceMode.MaintenanceModeMonthRecurringStart = maintenanceMode.MaintenanceModeMonthRecurringStart;
            configuration.MaintenanceMode.MaintenanceModeOnDemand = maintenanceMode.MaintenanceModeOnDemand;
            if (tags != null && tags.Count != 0)
                configuration.Tags = new List<int>(tags);

            if (diskSettings != null)
                configuration.DiskCollectionSettings = new DiskCollectionSettings(diskSettings);

            configuration.InputBufferLimiter = inputBufferLimiter;
            configuration.InputBufferLimited = inputBufferLimited;
            configuration.ActiveClusterNode = activeClusterNode;
            configuration.PreferredClusterNode = preferredClusterNode;
            configuration.ActiveWaitsConfiguration = activeWaitsConfiguration;
            configuration.ClusterCollectionSetting = clusterCollectionSetting;
            configuration.ServerPingInterval = serverPingInterval;
            configuration.VirtualizationConfiguration = virtualizationConfiguration;
            configuration.BaselineConfiguration = baselineConfiguration;
            //10.0 SQLdm srishti purohit -- doctors analysis UI configuration object
            configuration.AnalysisConfiguration = analysisConfiguration;
			//[START] SQLdm 10.0 (Rajesh Gupta): Baseline Enhancement - Added a list for multiple BaselineConfiguration 
            configuration.BaselineConfigurationList = baselineConfigurationList;
            //[END] SQLdm 10.0 (Rajesh Gupta): Baseline Enhancement - Added a list for multiple BaselineConfiguration 
            configuration.FriendlyServerName = friendlyServerName;

            configuration.AlertRefreshInMinutes = this.alertRefreshInMinutes;

            configuration.MostRecentSQLEdition = this.MostRecentSQLEdition;
            configuration.MostRecentSQLVersion = this.MostRecentSQLVersion;
            configuration.JobAlerts = this.JobAlerts;
            configuration.ErrorlogAlerts = this.ErrorlogAlerts;
            configuration.CloudProviderId = this.CloudProviderId;

            var wmi = configuration.WmiConfig;
            wmi.OleAutomationDisabled = WmiConfig.OleAutomationDisabled;
            wmi.DirectWmiEnabled = WmiConfig.DirectWmiEnabled;
            wmi.DirectWmiConnectAsCollectionService = WmiConfig.DirectWmiConnectAsCollectionService;
            wmi.DirectWmiUserName = WmiConfig.DirectWmiUserName;
            wmi.DirectWmiPassword = WmiConfig.DirectWmiPassword;

            configuration.SQLsafeConfig = this.SQLsafeConfig;
            configuration.IsUserSysAdmin = this.IsUserSysAdmin;
            //Getting configuration from DB for Analysis Configuration
            configuration.AnalysisConfiguration = analysisConfiguration;
            

            return configuration;
        }

        //public DateTime? GrowthStatisticsStartTime
        //{
        //    get { return growthStatisticsStartTime; }
        //    set { growthStatisticsStartTime = value; }
        //}

        //public DateTime? ReorgStatisticsStartTime
        //{
        //    get { return reorgStatisticsStartTime; }
        //    set { reorgStatisticsStartTime = value; }
        //}

        //public DateTime? LastGrowthStatisticsRunTime
        //{
        //    get { return lastGrowthStatisticsRunTime; }
        //    set { lastGrowthStatisticsRunTime = value; }
        //}

        //public DateTime? LastReorgStatisticsRunTime
        //{
        //    get { return lastReorgStatisticsRunTime; }
        //    set { lastReorgStatisticsRunTime = value; }
        //}

        ///// <summary>
        ///// This stores the previous growth statistics only when growth has just been run
        ///// </summary>
        //public DateTime? PreviousGrowthStatisticsRunTime
        //{
        //    get { return previousGrowthStatisticsRunTime; }
        //    set { previousGrowthStatisticsRunTime = value; }
        //}

        /// <summary>
        /// This stores the previous reorg statistics only when reorg has just been run
        /// </summary>
        //public DateTime? PreviousReorgStatisticsRunTime
        //{
        //    get { return previousReorgStatisticsRunTime; }
        //    set { previousReorgStatisticsRunTime = value; }
        //}

        //public short? GrowthStatisticsDays
        //{
        //    get { return growthStatisticsDays; }
        //    set { growthStatisticsDays = value; }
        //}

        //public short? ReorgStatisticsDays
        //{
        //    get { return reorgStatisticsDays; }
        //    set { reorgStatisticsDays = value; }
        //}

        //public List<string> TableStatisticsExcludedDatabases
        //{
        //    get
        //    {
        //        if (tableStatisticsExcludedDatabases == null)
        //            tableStatisticsExcludedDatabases = new List<string>();

        //        return tableStatisticsExcludedDatabases;
        //    }
        //    set
        //    {
        //        if (value == null)
        //            tableStatisticsExcludedDatabases = null;
        //        else
        //            tableStatisticsExcludedDatabases = new List<string>(value);
        //    }
        //}

        public FileSize ReorganizationMinimumTableSize
        {
            get
            {
                if (TableFragmentationConfiguration == null)
                {
                    TableFragmentationConfiguration = new TableFragmentationConfiguration(id);
                }

                return tableFragmentationConfiguration.FragmentationMinimumTableSize;
            }
            set
            {
                if (TableFragmentationConfiguration == null)
                {
                    TableFragmentationConfiguration = new TableFragmentationConfiguration(id);
                }

                TableFragmentationConfiguration.FragmentationMinimumTableSize = value;
            }
        }

        public TableGrowthConfiguration TableGrowthConfiguration
        {
            get { return tableGrowthConfiguration; }
            set { tableGrowthConfiguration = value; }
        }

        public TableFragmentationConfiguration TableFragmentationConfiguration
        {
            get { return tableFragmentationConfiguration; }
            set { tableFragmentationConfiguration = value; }
        }

        public TimeSpan CustomCounterTimeout
        {
            get { return customCounterTimeout; }
            set { customCounterTimeout = value; }
        }

        public bool HasCustomCounters
        {
            get { return customCounters != null && customCounters.Count > 0; }
        }

        public List<int> CustomCounters
        {
            get
            {
                if (customCounters == null)
                    customCounters = new List<int>();
                return customCounters;
            }
            set
            {
                if (value == null || value.Count == 0)
                    customCounters = null;
                else
                    customCounters = value;
            }
        }

        public bool HasTags
        {
            get { return tags != null && tags.Count > 0; }
        }

        public List<int> Tags
        {
            get
            {
                if (tags == null)
                    tags = new List<int>();
                return tags;
            }
            set
            {
                if (value == null || value.Count == 0)
                    tags = null;
                else
                    tags = value;
            }
        }

        public void AddTag(int tagId)
        {   
            if (!Tags.Contains(tagId))
                tags.Add(tagId);
        }

        public void RemoveTag(int tagId)
        {
            if (tags != null)
                tags.Remove(tagId);

        }

        public void RemoveTags(ICollection<int> tagIds)
        {
            if (tags != null && tagIds.Count > 0)
            {
                foreach (int tagId in tagIds)
                    tags.Remove(tagId);
            }
        }

        public OleAutomationExecutionContext OleAutomationContext
        {
            get { return WmiConfig.OleAutomationContext; }
            set { WmiConfig.OleAutomationContext = value; }
        }

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

        public bool OleAutomationUseDisabled
        {
            get { return WmiConfig.OleAutomationDisabled; }
            set { WmiConfig.OleAutomationDisabled = value; }
        }

        [Auditable("Modified server ping interval to")]
        public TimeSpan ServerPingInterval
        {
            get { return serverPingInterval; }
            set { serverPingInterval = value; }
        }

        public bool IsVirtualized
        {
            get
            {
                if (VirtualizationConfiguration != null && !string.IsNullOrEmpty(VirtualizationConfiguration.InstanceUUID))
                    return true;
                else
                    return false;
            }
        }

        public VirtualizationConfiguration VirtualizationConfiguration
        {
            get { return virtualizationConfiguration; }
            set { virtualizationConfiguration = value; }
        }

        public BaselineConfiguration BaselineConfiguration
        {
            get { return baselineConfiguration; }
            set { baselineConfiguration = value; }
        }
        //[START] SQLdm 10.0 (Rajesh Gupta): Baseline Enhancement - Added a list for multiple BaselineConfiguration 
        public Dictionary<int,BaselineConfiguration> BaselineConfigurationList
        {
            get { return baselineConfigurationList; }
            set { baselineConfigurationList = value; }
        }
        //[END] SQLdm 10.0 (Rajesh Gupta): Baseline Enhancement - Added a list for multiple BaselineConfiguration 

        //10.0 SQLdm srishti purohit -- doctors analysis UI configuration object
        public AnalysisConfiguration AnalysisConfiguration
        {
            get { return analysisConfiguration; }
            set { analysisConfiguration = value; }
        }
        public bool IsSQLsafeConnected
        {
            get { return (SQLsafeConfig != null && SQLsafeConfig.SQLsafeConnectionInfo != null); }
        }

        /// <summary>
        /// Shallow copy of this object.
        /// </summary>
        /// <returns></returns>
        public MonitoredSqlServer Clone()
        {
            MonitoredSqlServer clone = (MonitoredSqlServer)MemberwiseClone();
            if (HasCustomCounters)
                clone.customCounters = new List<int>(CustomCounters);
            return clone;
        }

        public override string ToString()
        {
            return InstanceName;
        }



        public static short DayOfWeekToShort(DayOfWeek day)
        {
            switch (day)
            {
                case DayOfWeek.Sunday:
                    return 1;
                case DayOfWeek.Monday:
                    return 4;
                case DayOfWeek.Tuesday:
                    return 8;
                case DayOfWeek.Wednesday:
                    return 16;
                case DayOfWeek.Thursday:
                    return 32;
                case DayOfWeek.Friday:
                    return 64;
                case DayOfWeek.Saturday:
                    return 128;
                default:
                    return 0;
            }
        }

        public static short DayOfWeekToShort(List<DayOfWeek> days)
        {
            short dayOfWeekShort = 0;
            foreach (DayOfWeek day in days)
            {
                dayOfWeekShort += DayOfWeekToShort(day);
            }
            return dayOfWeekShort;
        }

        public static bool MatchDayOfWeek(DayOfWeek day, short? dayOfWeekShort)
        {
            if (!dayOfWeekShort.HasValue)
                return false;

            switch (day)
            {
                case DayOfWeek.Sunday:
                    return ((dayOfWeekShort & 1) == 1);
                case DayOfWeek.Monday:
                    return ((dayOfWeekShort & 4) == 4);
                case DayOfWeek.Tuesday:
                    return ((dayOfWeekShort & 8) == 8);
                case DayOfWeek.Wednesday:
                    return ((dayOfWeekShort & 16) == 16);
                case DayOfWeek.Thursday:
                    return ((dayOfWeekShort & 32) == 32);
                case DayOfWeek.Friday:
                    return ((dayOfWeekShort & 64) == 64);
                case DayOfWeek.Saturday:
                    return ((dayOfWeekShort & 128) == 128);
                default:
                    return false;
            }
        }

        public static List<DayOfWeek> GetDays(short? dayOfWeekShort)
        {
            List<DayOfWeek> result = new List<DayOfWeek>();
            if (!dayOfWeekShort.HasValue)
                return result;

            if ((dayOfWeekShort & 1) == 1)
                result.Add(DayOfWeek.Sunday);
            if ((dayOfWeekShort & 4) == 4)
                result.Add(DayOfWeek.Monday);
            if ((dayOfWeekShort & 8) == 8)
                result.Add(DayOfWeek.Tuesday);
            if ((dayOfWeekShort & 16) == 16)
                result.Add(DayOfWeek.Wednesday);
            if ((dayOfWeekShort & 32) == 32)
                result.Add(DayOfWeek.Thursday);
            if ((dayOfWeekShort & 64) == 64)
                result.Add(DayOfWeek.Friday);
            if ((dayOfWeekShort & 128) == 128)
                result.Add(DayOfWeek.Saturday);

            return result;
        }

        public AuditableEntity GetAuditableEntity()
        {
            AuditableEntity entityResult = new AuditableEntity();
            entityResult.Name = this.InstanceName;
            

            return entityResult;
        }

        public AuditableEntity GetAuditableEntity(IAuditable oldValue)
        {
            AuditableEntity entityResult = new AuditableEntity();
            entityResult.Name = this.InstanceName;

            PropertiesComparer comparer = new PropertiesComparer();
            var propertiesChanged = comparer.GetNewProperties(oldValue, this);
            foreach (var property in propertiesChanged)
            {
                entityResult.AddMetadataProperty(property.Name, property.Value);
            }

            return entityResult;
        }
    }
}
