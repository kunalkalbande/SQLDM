using System;
using System.ComponentModel;
using Idera.SQLdm.Common.Snapshots;
using System.Collections.Generic;
using Idera.SQLdm.Common.Objects;
using Idera.SQLdm.Common.Auditing;
using Idera.SQLdm.Common.Attributes;
namespace Idera.SQLdm.Common.Configuration
{
    [Serializable]
    public sealed class MonitoredSqlServerConfiguration : IAuditable
    {
        private bool isActive;
        private Guid collectionServiceId;
        private SqlConnectionInfo connectionInfo;
        private TimeSpan scheduledCollectionInterval;
        private TimeSpan databaseStatisticsInterval = TimeSpan.FromMinutes(60);
        private bool maintenanceModeEnabled;
        private QueryMonitorConfiguration queryMonitorConfiguration;
        private ActivityMonitorConfiguration activityMonitorConfiguration;
        private DateTime? growthStatisticsStartTime;
        private DateTime? lastGrowthStatisticsRunTime;
        private DateTime? reorgStatisticsStartTime;
        private DateTime? lastReorgStatisticsRunTime;
        private Int16? growthStatisticsDays;
        private Int16? reorgStatisticsDays;
        private List<string> tableStatisticsExcludedDatabases;
        private FileSize reorgMinimumTableSize;
        private List<int> customCounters;
        private bool replicationMonitoringDisabled;
        private MaintenanceMode maintenanceMode;
        private bool extendedHistoryCollectionDisabled = false;
        private List<int> tags;
        private DiskCollectionSettings diskSettings;
        private int inputBufferLimiter = 500;
        private bool inputBufferLimited = false;
        private string activeClusterNode = null;
        private string preferredClusterNode = null;
        private ActiveWaitsConfiguration activeWaitsConfiguration;
        private ClusterCollectionSetting clusterCollectionSetting = Snapshots.ClusterCollectionSetting.Default;
        private TimeSpan serverPingInterval = new TimeSpan(0, 0, 30);
        private int alertTemplateID = -1;
        private VirtualizationConfiguration virtualizationConfiguration = null;
        private bool alertRefreshInMinutes = true;
        private BaselineConfiguration baselineConfiguration = null;
        //10.0 SQLdm srishti purohit -- doctors UI configuration object
        private AnalysisConfiguration analysisConfiguration = null;
		//[START] SQLdm 10.0 (Rajesh Gupta): Baseline Enhancement - Added a list for multiple BaselineConfiguration 
        private Dictionary<int,BaselineConfiguration> baselineConfigurationList = null;
        //[END] SQLdm 10.0 (Rajesh Gupta): Baseline Enhancement - Added a list for multiple BaselineConfiguration  
        private string friendlyServerName = null;
        private int? cloudProviderId = null;//SQLdm 10.0 (Tarun Sapra)- Added this for knowing if this database is a cloud data base or not
        //SQLdm 10.1(Barkha Khatri) adding sysAdmin flag
        private bool isUserSysAdmin = false;
        private string awsAccessKey = string.Empty;
        private string awsSecretKey = string.Empty;
        private string awsRegionEndpoint = string.Empty;

                // Display flags
        public ServerVersion MostRecentSQLVersion { get; set; }
        public string MostRecentSQLEdition { get; set; }

        [Auditable(false)]
        public bool JobAlerts { get; set; }

        [Auditable(false)]
        public bool ErrorlogAlerts { get; set; }

        // WMI
        public WmiConfiguration WmiConfig { get; set; }

        //SQLsafe
        public SQLsafeRepositoryConfiguration SQLsafeConfig { get; set; }

        //SQLdm 10.1(Barkha Khatri) adding sysAdmin flag
        public bool IsUserSysAdmin
        {
            get { return isUserSysAdmin; }
            set { isUserSysAdmin = value; }
        }

        public string AwsAccessKey
        {
            get { return awsAccessKey; }
            set { awsAccessKey = value; }
        }

        public string AwsSecretKey
        {
            get { return awsSecretKey; }
            set { awsSecretKey = value; }
        }

        public string AwsRegionEndpoint
        {
            get { return awsRegionEndpoint; }
            set { awsRegionEndpoint = value; }
        }

        public MonitoredSqlServerConfiguration() : this(new SqlConnectionInfo())
        {
        }

        public MonitoredSqlServerConfiguration(string instanceName) : this(new SqlConnectionInfo(instanceName))
        {
        }

        public MonitoredSqlServerConfiguration(SqlConnectionInfo connectionInfo)
        {
            if (connectionInfo == null)
            {
                throw new ArgumentNullException("connectionInfo");
            }

            isActive = true;
            collectionServiceId = Guid.Empty;
            this.connectionInfo = connectionInfo;
            scheduledCollectionInterval = TimeSpan.FromMinutes(6);
            maintenanceModeEnabled = false;
            maintenanceMode = new MaintenanceMode();
            queryMonitorConfiguration =
                new QueryMonitorConfiguration(false, false, false, false, TimeSpan.FromMilliseconds(500), TimeSpan.Zero,
                                              0, 0, new FileSize(1024), 2, 1000, new AdvancedQueryMonitorConfiguration(),
                                              false, true); //SQLdm 9.1 (Ankit Srivastava) -- Fixed Open functional isssue--changed default value according to the implementaion
                                                            //SQLdm 9.0 (Ankit Srivastava): Query Monitoring with Extended Event Session-- passed default values of  the traceenabled,collectQueryPlans to the constructor

            activityMonitorConfiguration = new ActivityMonitorConfiguration(true, false, true, false, 30,
                queryMonitorConfiguration.TraceFileSize,
                queryMonitorConfiguration.TraceFileRollovers,
                queryMonitorConfiguration.RecordsPerRefresh,
                new AdvancedQueryMonitorConfiguration(), true);//SQLdm 9.1 (Ankit Srivastava): Activity Monitoring with Extended Events-- passed default value of  the traceEnabled


            growthStatisticsStartTime = new DateTime(1900, 1, 1, 3, 0, 0);
            reorgStatisticsStartTime = new DateTime(1900, 1, 1, 3, 0, 0);
            activeWaitsConfiguration = null;
            clusterCollectionSetting = Snapshots.ClusterCollectionSetting.Default;
            WmiConfig = new WmiConfiguration();
        }

        //START: SQLdm 10.0 (Tarun Sapra)- Added this for knowing if this database is a cloud data base or not
        public int? CloudProviderId{
            get { return cloudProviderId; }
            set { cloudProviderId = value; }
        }
        //END: SQLdm 10.0 (Tarun Sapra)- Added this for knowing if this database is a cloud data base or not

        public bool AlertRefreshInMinutes
        {
            get { return alertRefreshInMinutes; }
            set { alertRefreshInMinutes = value; }
        }

        public string ActiveClusterNode
        {
            get { return activeClusterNode; }
            set { activeClusterNode = value; }
        }

        [AuditableAttribute("Set Preferred Cluster Node to")]
        public string PreferredClusterNode
        {
            get { return preferredClusterNode; }
            set { preferredClusterNode = value; }
        }

        [AuditableAttribute("Friendly Name of the Instance")]
        public string FriendlyServerName
        {
            get { return friendlyServerName; }
            set { friendlyServerName = value; }
        }
        
        public string InstanceName
        {
            get { return connectionInfo.InstanceName; }
            set { connectionInfo.InstanceName = value; }
        }

        public SqlConnectionInfo ConnectionInfo
        {
            get { return connectionInfo; }
        }

        [Auditable(false)]
        public bool MaintenanceModeEnabled
        {
            get { return maintenanceModeEnabled; }
            set { maintenanceModeEnabled = value; }
        }

        public bool IsActive
        {
            get { return isActive; }
            set { isActive = value; }
        }

        public MaintenanceMode MaintenanceMode
        {
            get { return maintenanceMode; }
            set { maintenanceMode = value; }
        }

        [Auditable("Changed scheduled collection interval to")]
        public TimeSpan ScheduledCollectionInterval
        {
            get { return scheduledCollectionInterval; }
            set
            {
                if (scheduledCollectionInterval <= TimeSpan.Zero)
                {
                    new ArgumentOutOfRangeException("ScheduledCollectionInterval",
                                                    "The scheduled collection interval must be greater than zero.");
                }

                scheduledCollectionInterval = value;
            }
        }

        [Auditable("Database Statistics Interval")]
        public TimeSpan DatabaseStatisticsInterval
        {
            get { return databaseStatisticsInterval; }
            set
            {
                if (databaseStatisticsInterval <= TimeSpan.Zero)
                {
                    new ArgumentOutOfRangeException("DatabaseStatisticsInterval",
                                                    "The scheduled collection interval must be greater than zero.");
                }

                databaseStatisticsInterval = value;
            }
        }

        public Guid CollectionServiceId
        {
            get { return collectionServiceId; }
            set { collectionServiceId = value; }
        }

        // This is not saved to the repository from the Management Service
        public ClusterCollectionSetting ClusterCollectionSetting
        {
            get { return clusterCollectionSetting; }
            set { clusterCollectionSetting = value; }
        }

        public QueryMonitorConfiguration QueryMonitorConfiguration
        {
            get { return queryMonitorConfiguration; }
            set
            {
                if (value == null)
                {
                    throw new ArgumentNullException("value");
                }

                queryMonitorConfiguration = value;
            }
        }
        public ActivityMonitorConfiguration ActivityMonitorConfiguration
        {
            get { return activityMonitorConfiguration; }
            set
            {
                if (value == null)
                {
                    throw new ArgumentNullException("value");
                }

                activityMonitorConfiguration = value;
            }
        }

        [Auditable(false)]
        public DateTime? GrowthStatisticsStartTime
        {
            get { return growthStatisticsStartTime; }
            set { growthStatisticsStartTime = value; }
        }

        [Auditable(false)]
        public DateTime? ReorgStatisticsStartTime
        {
            get { return reorgStatisticsStartTime; }
            set { reorgStatisticsStartTime = value; }
        }

        [Auditable(false)]
        public DateTime? LastGrowthStatisticsRunTime
        {
            get { return lastGrowthStatisticsRunTime; }
            set { lastGrowthStatisticsRunTime = value; }
        }

        [Auditable(false)]
        public DateTime? LastReorgStatisticsRunTime
        {
            get { return lastReorgStatisticsRunTime; }
            set { lastReorgStatisticsRunTime = value; }
        }

        [Auditable(false)]
        public short? GrowthStatisticsDays
        {
            get { return growthStatisticsDays; }
            set { growthStatisticsDays = value; }
        }
       
        [Auditable(false)]
        public short? ReorgStatisticsDays
        {
            get { return reorgStatisticsDays; }
            set { reorgStatisticsDays = value; }
        }

        public List<string> TableStatisticsExcludedDatabases
        {
            get
            {
                if (tableStatisticsExcludedDatabases == null)
                    tableStatisticsExcludedDatabases = new List<string>();
                return tableStatisticsExcludedDatabases;
            }
            set
            {
                if (value == null)
                    tableStatisticsExcludedDatabases = null;
                else
                    tableStatisticsExcludedDatabases = new List<string>(value);
            }
        }

        public FileSize ReorganizationMinimumTableSize
        {
            get
            {
                if (reorgMinimumTableSize == null)
                {
                    reorgMinimumTableSize = new FileSize();
                    reorgMinimumTableSize.Pages = 1000;
                }
                return reorgMinimumTableSize;
            }
            set
            {
                reorgMinimumTableSize = value;
            }
        }

        public List<int> CustomCounters
        {
            get
            {
                return customCounters;
            }
            set
            {
                if (value == null)
                    customCounters = null;
                else
                    customCounters = new List<int>(value);
            }
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

        [Auditable(false)]
        public bool OleAutomationDisabled
        {
            get { return WmiConfig.OleAutomationDisabled; }
            set { WmiConfig.OleAutomationDisabled = value; }
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
                if (value == null)
                    tags = null;
                else
                    tags = new List<int>(value);
            }
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

        [Auditable("Limit Executions of DBCC Inputbuffer Changed to")]
        public int InputBufferLimiter
        {
            get { return inputBufferLimiter; }
            set { inputBufferLimiter = value; }
        }

        [Auditable("Limit Executions of DBCC Inputbuffer Enabled")]
        public bool InputBufferLimited
        {
            get { return inputBufferLimited; }
            set { inputBufferLimited = value; }
        }

        public ActiveWaitsConfiguration ActiveWaitsConfiguration
        {
            get { return activeWaitsConfiguration; }
            set { activeWaitsConfiguration = value; }
        }

        [Auditable("Modified server ping interval to")]
        public TimeSpan ServerPingInterval
        {
            get { return serverPingInterval; }
            set { serverPingInterval = value; }
        }

        // Only used when adding a new monitored server
        public int AlertTemplateID
        {
            get { return alertTemplateID; }
            set { alertTemplateID = value; }
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
        public AnalysisConfiguration AnalysisConfiguration
        {
            get { return analysisConfiguration; }
            set { analysisConfiguration = value; }
        }
		//[START] SQLdm 10.0 (Rajesh Gupta): Baseline Enhancement - Added a list for multiple BaselineConfiguration 
        public Dictionary<int,BaselineConfiguration> BaselineConfigurationList
        {
            get { return baselineConfigurationList; }
            set { baselineConfigurationList = value; }
        }
        //[END] SQLdm 10.0 (Rajesh Gupta): Baseline Enhancement - Added a list for multiple BaselineConfiguration 
        public bool IsSQLsafeConnected
        {
            get { return (SQLsafeConfig != null && SQLsafeConfig.SQLsafeConnectionInfo != null); }
        }

        /// <summary>
        /// Creates a new AuditableEntity object that contains the name of the alert
        /// </summary>
        /// <returns></returns>
        public AuditableEntity GetAuditableEntity()
        {
            AuditableEntity entity = new AuditableEntity();
            entity.Name = this.InstanceName;
            return entity;
        }

        /// <summary>
        /// Creates a new AuditableEntity comparing the current MonitoredSqlServerConfiguration with one that has the values before the user change them.
        /// </summary>
        /// <param name="oldValue">An MonitoredSqlServerConfiguration that let this Alert to compare and create a new AuditableEntity</param>
        /// <returns></returns>
        public AuditableEntity GetAuditableEntity(IAuditable oldValue)
        {
            AuditableEntity entity = GetAuditableEntity();
            CompareTo(oldValue, ref entity);
            return entity;
        }

        public void CompareTo(IAuditable oldValue, ref AuditableEntity entity)
        {
            PropertiesComparer comparer = new PropertiesComparer();
            var propertiesChanged = comparer.GetNewProperties(oldValue, this);
            foreach (var property in propertiesChanged)
            {
                entity.AddMetadataProperty(property.Name, property.Value);
            }
        }
    }
}
