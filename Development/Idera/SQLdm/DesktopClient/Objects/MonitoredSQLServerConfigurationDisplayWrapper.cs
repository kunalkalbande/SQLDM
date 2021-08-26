using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Idera.SQLdm.Common;
using Idera.SQLdm.Common.Attributes;
using Idera.SQLdm.Common.Configuration;
using Idera.SQLdm.Common.Events;
using Idera.SQLdm.Common.Objects;
using Infragistics.Win;

namespace Idera.SQLdm.DesktopClient.Objects
{
    /// <summary>
    /// This class is used to display and modify server configuration for the Server Group Properties Panel
    /// Add read-only properties for display
    /// Read-write properties should also be added to MonitoredServerConfigurationOriginalSettings and FieldHasChanged to support change tracking in the view
    /// </summary>
    public class MonitoredSQLServerConfigurationDisplayWrapper
    {
        [System.AttributeUsage(System.AttributeTargets.Property)]
        public class MinAndMaxValue : System.Attribute
        {
            public int MinValue { get; set; }
            public int MaxValue { get; set; }

            public MinAndMaxValue(int minValue,int maxValue)
            {
                this.MinValue = minValue;
                this.MaxValue = maxValue;
            }
        }

        // Long story here
        // See propertiesGrid_InitializeLayout in ServerGroupPropertiesView.
        // These were originally regular timespan editors but when it came time to put in custom events 
        // (like the "oven timer" behavior) and the custom spinner buttons, it would not work within a grid.
        // The buttons would not appear and the custom validation events would not fire.
        // I found a thread here: http://forums.infragistics.com/forums/p/48274/256759.aspx
        // in which Infragistics explains "Embedded editors use a different notification mechanism than controls. 
        // You can use an UltraControlContainerEditor to provide a wrapper around your custom control and then assign 
        // that UltraControlContainerEditor instance to the EditorComponent property."
        //
        // So, the UltraTimeSpanEditorHelper creates this control, but further, there is a problem whereby
        // the control container is expecting that for a property "Value" that there will be a "ValueChanged" event
        // but in the case of a timespan editor, TimeSpan does not have a TimeSpanEditor and Value (type Object) is not 
        // able to directly cast to TimeSpan.  So this means that values that are using this control must
        // be of type Object.
        //
        // I have added an attribute HasTimeSpanEditor to MonitoredSQLServerConfigurationDisplayWrapper to
        // help identify these Objects which are supposted to be TimeSpans
        [System.AttributeUsage(System.AttributeTargets.Property)]
        public class HasTimeSpanEditor : System.Attribute
        {
        }


        /// <summary>
        /// Private class used to store original values
        /// This is used to set HasChanges and to undo changes
        /// </summary>
        private class MonitoredServerConfigurationOriginalSettings
        {
            public string PreferredClusterNode { get; private set; }
            
            [Auditable("Changed scheduled collection interval to")]
            public TimeSpan ScheduledCollectionInterval { get; private set; }
            
            public int ReorganizationMinimumTableSizeKB { get; private set; }

            [Auditable("Disabled collection of replication statistics")]
            public bool ReplicationMonitoringDisabled { get; private set; }
            
            [Auditable("Disabled extended history collection")]
            public bool ExtendedHistoryCollectionDisabled { get; private set; }
            
            [Auditable("Modified server ping interval to")]
            public TimeSpan ServerPingInterval { get; private set; }
            
            public TimeSpan DatabaseStatisticsInterval { get; private set; }

            public MonitoredServerConfigurationOriginalSettings(string preferredClusterNode, TimeSpan scheduledCollectionInterval, int reorganizationMinimumTableSizeKb, bool replicationMonitoringDisabled, bool extendedHistoryCollectionDisabled, TimeSpan serverPingInterval, TimeSpan databaseStatisticsInterval)
            {
                PreferredClusterNode = preferredClusterNode;
                ScheduledCollectionInterval = scheduledCollectionInterval;
                ReorganizationMinimumTableSizeKB = reorganizationMinimumTableSizeKb;
                ReplicationMonitoringDisabled = replicationMonitoringDisabled;
                ExtendedHistoryCollectionDisabled = extendedHistoryCollectionDisabled;
                ServerPingInterval = serverPingInterval;
                DatabaseStatisticsInterval = databaseStatisticsInterval;
            }

            public bool HasChanges(MonitoredSQLServerConfigurationDisplayWrapper config)
            {
                if (config.PreferredClusterNode != PreferredClusterNode) return true;
                if ((TimeSpan)config.ScheduledCollectionInterval != ScheduledCollectionInterval) return true;
                if (config.ReorganizationMinimumTableSizeKB != ReorganizationMinimumTableSizeKB) return true;
                if (config.ReplicationMonitoringDisabled != ReplicationMonitoringDisabled) return true;
                if (config.ExtendedHistoryCollectionDisabled != ExtendedHistoryCollectionDisabled) return true;
                if ((TimeSpan)config.ServerPingInterval != ServerPingInterval) return true;
                if ((TimeSpan)config.DatabaseStatisticsInterval != DatabaseStatisticsInterval) return true;
                return false;
            }

            public static void ResetChanges(MonitoredSQLServerConfigurationDisplayWrapper config)
            {
                config.PreferredClusterNode = config.originalSettings.PreferredClusterNode;
                config.ScheduledCollectionInterval = config.originalSettings.ScheduledCollectionInterval;
                config.ReorganizationMinimumTableSizeKB = config.originalSettings.ReorganizationMinimumTableSizeKB;
                config.ReplicationMonitoringDisabled = config.originalSettings.ReplicationMonitoringDisabled;
                config.ExtendedHistoryCollectionDisabled = config.originalSettings.ExtendedHistoryCollectionDisabled;
                config.ServerPingInterval = config.originalSettings.ServerPingInterval;
                config.DatabaseStatisticsInterval = config.originalSettings.DatabaseStatisticsInterval;
            }
        }

        private MonitoredSqlServerConfiguration config;
        private int id;
        private MonitoredServerConfigurationOriginalSettings originalSettings;
        

        public MonitoredSQLServerConfigurationDisplayWrapper(MonitoredSqlServerConfiguration config, int id)
        {
            this.config = config;
            this.id = id;
            originalSettings = new MonitoredServerConfigurationOriginalSettings(
                this.PreferredClusterNode,
                (TimeSpan)this.ScheduledCollectionInterval,
                this.ReorganizationMinimumTableSizeKB,
                this.ReplicationMonitoringDisabled,
                this.ExtendedHistoryCollectionDisabled,
                (TimeSpan)this.ServerPingInterval,
                (TimeSpan)this.DatabaseStatisticsInterval);
        }

        public int SQLServerID
        {
            get { return id; }
        }

        #region Change Tracking


        /// <summary>
        /// This tracks whether there have been any changes at all to the object
        /// </summary>
        public bool HasChanges { get { return originalSettings.HasChanges(this); } }


        /// <summary>
        /// This tracks whether a particular field that has been edited is different from the previous value for UI purposes
        /// </summary>
        public bool FieldHasChanged(string key, object value)
        {
            try
            {

                switch (key)
                {
                    case "PreferredClusterNode":
                        return ((string) value != originalSettings.PreferredClusterNode);
                    case "ScheduledCollectionInterval":
                        return ((TimeSpan) value != originalSettings.ScheduledCollectionInterval);
                    case "ReorganizationMinimumTableSizeKB":
                        return ((int) value != originalSettings.ReorganizationMinimumTableSizeKB);
                    case "ReplicationMonitoringDisabled":
                        return ((bool) value != originalSettings.ReplicationMonitoringDisabled);
                    case "ExtendedHistoryCollectionDisabled":
                        return ((bool) value != originalSettings.ExtendedHistoryCollectionDisabled);
                    case "ServerPingInterval":
                        return ((TimeSpan) value != originalSettings.ServerPingInterval);
                    case "DatabaseStatisticsInterval":
                        return ((TimeSpan) value != originalSettings.DatabaseStatisticsInterval);
                    default:
                        return false;
                }
            }
            catch (Exception e)
            {
                return false;
            }
        }

        #endregion

        #region Editable Fields

        private TimeSpan FixTimeSpan(object o)
        {
            if (o is FormattedTimeSpan)
                return ((FormattedTimeSpan)o).TimeSpan;
            else
                return (TimeSpan)o;

        }
        public string PreferredClusterNode
        {
            get { return config.PreferredClusterNode; }
            set
            {
                config.PreferredClusterNode = value;
            }
        }

        [MinAndMaxValue(Properties.Constants.MinScheduledRefreshIntervalSeconds, Properties.Constants.MaxScheduledRefreshIntervalSeconds)]
        [HasTimeSpanEditor()]
        [Auditable("Changed scheduled collection interval to")]
        // For why this is an object, check comment with HasTimeSpanEditor above
        public Object ScheduledCollectionInterval
        {
            get { return config.ScheduledCollectionInterval; }
            set
            {
                TimeSpan time = FixTimeSpan(value);
                
                if (time <= TimeSpan.Zero)
                {
                    new ArgumentOutOfRangeException("ScheduledCollectionInterval",
                                                    "The scheduled collection interval must be greater than zero.");
                }
                config.ScheduledCollectionInterval = time;
                config.AlertRefreshInMinutes = (config.ScheduledCollectionInterval.TotalMinutes > 1);
            }
        }

        [MinAndMaxValue(0, 999999)]
        public int ReorganizationMinimumTableSizeKB
        {
            get
            {
                return config.ReorganizationMinimumTableSize.Kilobytes.HasValue ? (int)config.ReorganizationMinimumTableSize.Kilobytes.Value : 0;
            }
            set
            {
                config.ReorganizationMinimumTableSize.Kilobytes = value;
            }
        }

        [Auditable("Disabled collection of replication statistics")]
        public bool ReplicationMonitoringDisabled
        {
            get { return config.ReplicationMonitoringDisabled; }
            set
            {
                config.ReplicationMonitoringDisabled = value;
            }
        }

        [Auditable("Disabled extended history collection")]
        public bool ExtendedHistoryCollectionDisabled
        {
            get { return config.ExtendedHistoryCollectionDisabled; }
            set
            {
                config.ExtendedHistoryCollectionDisabled = value;
            }
        }

        [MinAndMaxValue(Properties.Constants.MinServerPingIntervalSeconds, Properties.Constants.MaxServerPingIntervalSeconds)]
        [HasTimeSpanEditor()]
        [Auditable("Modified server ping interval to")]
        // For why this is an object, check comment with HasTimeSpanEditor above
        public Object ServerPingInterval
        {
            get { return config.ServerPingInterval; }
            set
            {
                TimeSpan time = FixTimeSpan(value);
                config.ServerPingInterval = time;
            }
        }

        [MinAndMaxValue(Properties.Constants.MinDatabaseStatisticsIntervalSeconds, Properties.Constants.MaxDatabaseStatisticsIntervalSeconds)]
        [HasTimeSpanEditor()]
        // For why this is an object, check comment with HasTimeSpanEditor above
        public Object DatabaseStatisticsInterval
        {
            get { return config.DatabaseStatisticsInterval; }
            set
            {
                TimeSpan time = FixTimeSpan(value);
                config.DatabaseStatisticsInterval = time;
            }
        }

        
        #endregion


        #region Fixed Fields

        public ServerVersion MostRecentSQLVersion { get { return config.MostRecentSQLVersion; } }
        public string MostRecentSQLEdition { get { return config.MostRecentSQLEdition; } }
        public bool JobAlerts { get { return config.JobAlerts; } }
        public bool ErrorlogAlerts { get { return config.ErrorlogAlerts; } }


        public string AuthenticationMode
        {
            get {
                if (config.ConnectionInfo.UseIntegratedSecurity)
                    return "Windows Authentication";
                else
                {
                    return "SQL Authentication: " + config.ConnectionInfo.UserName;
                }
                    
            }
        }

        public string ActiveClusterNode
        {
            get { return config.ActiveClusterNode; }
        }

       

        public string InstanceName
        {
            get { return config.InstanceName; }
        }
        
        public string MaintenanceMode
        {
            get
            {
                return config.MaintenanceMode.ToString();
            }
        }   

        public string WmiConfiguration
        {
            get
            {
                if (config.WmiConfig.DirectWmiEnabled)
                {
                    if (config.WmiConfig.DirectWmiConnectAsCollectionService)
                        return "Enabled (Direct)";

                    return String.Format("Enabled (Direct: {0})", config.WmiConfig.DirectWmiUserName);
                }
                if (!config.WmiConfig.OleAutomationDisabled)
                    return "Enabled (OLE Automation)";
            
                return "Disabled";
            }
        }

        public string QueryMonitorConfiguration
        {
            get { return config.QueryMonitorConfiguration.ToString(); }
        }

        public string QueryMonitorEnabled
        {
            get
            {
                if (config.QueryMonitorConfiguration != null)
                {
                    return config.QueryMonitorConfiguration.EnabledString;
                }
                else
                {
                    return "Disabled";
                }
            }
        }

        public string ActivityMonitorEnabled
        {
            get 
            {
                if (config.ActivityMonitorConfiguration != null)
                    return config.ActivityMonitorConfiguration.EnabledString;
                else
                    return "Disabled";
            }
        }


        public string QueryMonitorFilterTypes
        {
            get
            {
                if (config.QueryMonitorConfiguration != null)
                {
                    return config.QueryMonitorConfiguration.FilterTypeString;
                }
                else
                {
                    return "None";
                }
            }
        }

        public string QueryMonitorAdvancedFilters
        {
            get
            {
                if (config.QueryMonitorConfiguration != null)
                {
                    return config.QueryMonitorConfiguration.AdvancedFilterString;
                }
                else
                {
                    return "None";
                }
            }
        }

        public string DeadlockMonitoring
        {
            get
            {
                if (config.MostRecentSQLVersion != null && config.MostRecentSQLVersion.Major == 8)
                {
                    return "Not available on SQL 2000";
                }
                if (config.QueryMonitorConfiguration != null)
                {
                    if (config.ActivityMonitorConfiguration.DeadlockEventsEnabled)
                    {
                        if (config.ActivityMonitorConfiguration.Enabled)
                        {
                            return "Enabled / Running";
                        }
                        else
                        {
                            return "Enabled / Not Running";
                        }
                    }
                    else
                    {
                        return "Disabled";
                    }
                }
                return "";
            }
        }

        private DateTime? GrowthStatisticsStartTime
        {
            get { return config.GrowthStatisticsStartTime; }
           }

        private DateTime? ReorgStatisticsStartTime
        {
            get { return config.ReorgStatisticsStartTime; }
        }

        public DateTime? LastGrowthStatisticsRunTime
        {
            get { return config.LastGrowthStatisticsRunTime; }
        }

        public DateTime? LastReorgStatisticsRunTime
        {
            get { return config.LastReorgStatisticsRunTime; }
        }

        private string GrowthStatisticsDays
        {
            get
            {
                var sb = new StringBuilder();
                foreach (DayOfWeek d in Enum.GetValues(typeof(DayOfWeek)))
                {
                    if (MonitoredSqlServer.MatchDayOfWeek(d,config.GrowthStatisticsDays))
                    {
                        sb.Append(d.ToString().Substring(0, 3));
                        sb.Append(",");
                    }
                }
                if (sb.Length > 1)
                    sb.Remove(sb.Length - 1, 1);
                return sb.ToString();
            }
            
        }

       

        private string ReorgStatisticsDays
        {

            get
            {
                var sb = new StringBuilder();
                foreach (DayOfWeek d in Enum.GetValues(typeof(DayOfWeek)))
                {
                    if (MonitoredSqlServer.MatchDayOfWeek(d, config.ReorgStatisticsDays))
                    {
                        sb.Append(d.ToString().Substring(0, 3));
                        sb.Append(",");
                    }
                }
                if (sb.Length > 1)
                    sb.Remove(sb.Length - 1, 1);
                return sb.ToString();
            }
            
        }

        public string GrowthStatisticsDisplay
        {
            get
            {
                return GrowthStatisticsDays +
                       (GrowthStatisticsStartTime.HasValue ? " at " + GrowthStatisticsStartTime.Value.ToString("t") : "");
            }
        }

        public string ReorgStatisticsDisplay
        {
            get
            {
                return ReorgStatisticsDays +
                       (ReorgStatisticsStartTime.HasValue ? " at " + ReorgStatisticsStartTime.Value.ToString("t") : "");
            }
        }

        public int TableStatisticsExcludedDatabasesCount
        {
            get { return config.TableStatisticsExcludedDatabases.Count; }
           
        }

        public string TableStatisticsExcludedDatabasesString
        {
            get {

                if (config.TableStatisticsExcludedDatabases == null || config.TableStatisticsExcludedDatabases.Count == 0)
                {
                    return "None";
                }
                else
                {

                    return First100Chars(config.TableStatisticsExcludedDatabases);
                }

            }

        }

       

        public int CustomCountersCount
        {
            get { return config.CustomCounters != null ? config.CustomCounters.Count : 0; }
        }

        public string CustomCountersList
        {
            get
            {
                if (config.CustomCounters == null || config.CustomCounters.Count == 0)
                    return "None";
                else
                {
                    var defs = new List<string>();
                    MetricDefinitions metrics = ApplicationModel.Default.MetricDefinitions;
                    foreach (int i in config.CustomCounters)
                    {
                        MetricDescription? definition = metrics.GetMetricDescription(i);
                        if (definition.HasValue)
                        {
                            defs.Add(definition.Value.Name);
                        }
                    }

                    return First100Chars(defs);

                }
                    
            }
        }

       

       

        public string DiskCollectionSettings
        {
            get
            {
                if (config.DiskCollectionSettings.AutoDiscover)
                {
                   return "Drives auto-discovered";
                }
            
                  else
                {
                    if (config.DiskCollectionSettings == null || config.DiskCollectionSettings.Drives == null || config.DiskCollectionSettings.Drives.Length == 0)
                    {
                        return "None";
                    }
                    else
                    {
                        return First100Chars(config.DiskCollectionSettings.Drives.ToList());    
                    }

                    

                }
                
            }
        }

        [Auditable("Changed limit of DBCC Input Buffer executions to")]
        public string InputBufferLimiter
        {
            get
            {
                if (config.InputBufferLimited)
                {
                    return "Max " + config.InputBufferLimiter;
                }
                else
                {
                    return "Unlimited";
                }
            }
        }

        public string ActiveWaitsConfiguration
        {
            get
            {
                string collectedUsing = "using a SQL Server DMV";
                
                if (config.MostRecentSQLVersion != null)
                {
                    // if we are on 2017 plus and qs is enabled
                    if (this.config.MostRecentSQLVersion.Major >= 14 && this.config.ActiveWaitsConfiguration.EnabledQs)
                    {
                        //if this is sql 2017+
                        collectedUsing = " using Query Store";
                    }
                    //if we are on 2012 and xe is enabled
                    else if (config.MostRecentSQLVersion.Major >= 11 && config.ActiveWaitsConfiguration.EnabledXe)
                    {
                        //if this is sql 2012
                        collectedUsing = " using Extended Events";
                    }
                }

                var sb = new StringBuilder();
                if (config.MostRecentSQLVersion != null && config.MostRecentSQLVersion.Major == 8)
                {
                    sb.Append("Not available on SQL 2000.");
                    return sb.ToString();
                }
                if (!config.ActiveWaitsConfiguration.Enabled)
                {
                    sb.Append(string.Format("Collected {0}, only while view is open", collectedUsing));
                }
                else
                {
                    if (config.ActiveWaitsConfiguration.RunTime.HasValue)
                    {
                        sb.Append(string.Format("Collected {0}, while view is open and between ", collectedUsing));
                        sb.Append(config.ActiveWaitsConfiguration.StartTimeRelative);
                        sb.Append(" and ");
                        sb.Append(
                            config.ActiveWaitsConfiguration.StartTimeRelative.Add(
                                config.ActiveWaitsConfiguration.RunTime.Value));
                    }
                    else
                    {
                        sb.Append(string.Format("Collected {0}", collectedUsing));
                    }
                }
                    
                return sb.ToString();
                
            }
        }

        public string ActiveWaitsFilters
        {
            get
            {
                if (config.MostRecentSQLVersion != null && config.MostRecentSQLVersion.Major == 8)
                {
                     return ("n/a");
                }
                if (config.ActiveWaitsConfiguration != null && config.ActiveWaitsConfiguration.AdvancedConfiguration != null)
                {
                    StringBuilder sb = new StringBuilder();
                    var advancedConfiguration = config.ActiveWaitsConfiguration.AdvancedConfiguration;
                   
                        if (advancedConfiguration.ApplicationExcludeToString().Length > 0)
                        {
                            sb.Append("Application: ");
                            sb.Append(advancedConfiguration.ApplicationExcludeToString());
                        }
                        if (advancedConfiguration.DatabaseExcludeToString().Length > 0)
                        {
                            if (sb.Length > 0)
                                sb.Append(", ");
                            sb.Append("Database: ");
                            sb.Append(advancedConfiguration.DatabaseExcludeToString());
                        }
                        if (advancedConfiguration.SqlTextExcludeToString().Length > 0)
                        {
                            if (sb.Length > 0)
                                sb.Append(", ");
                            sb.Append("SQL: ");
                            sb.Append(advancedConfiguration.SqlTextExcludeToString());
                        }
                        if (advancedConfiguration.ExcludeDM)
                        {
                            if (sb.Length > 0)
                                sb.Append(", ");
                            sb.Append("Exclude SQLdm");
                        }
                        if (advancedConfiguration.Rowcount > 0)
                        {
                            if (sb.Length > 0)
                                sb.Append(", ");
                            sb.Append("Max Rows: ");
                            sb.Append(advancedConfiguration.Rowcount);
                        }


                    if (sb.Length > 0)
                        return sb.ToString();

                    return "None";
                }
                else
                {
                    return "None";
                }
            }
        }

       

       

        public string VCenter
        {
           get
            {
                 if (config.IsVirtualized && config.VirtualizationConfiguration != null )
                 {
                     return  config.VirtualizationConfiguration.VCName;
                 }
                return "";
            }

        }

        public string VMHost
        {
            get
            {
                 if (config.IsVirtualized && config.VirtualizationConfiguration != null )
                 {
                     return  config.VirtualizationConfiguration.VMName;
                 }
                 return "";
            }
        }

        public string BaselineTimePeriod
        {
            get
            {
                if (config.BaselineConfiguration != null && config.BaselineConfiguration.Template != null)
                {
                    return config.BaselineConfiguration.Template.TimePeriodString;
                }
                else
                {
                    var baselineConfig = new BaselineConfiguration();
                    return baselineConfig.Template.TimePeriodString;
                }
            }
        }

        public string BaselineDateRange
        {
            get
            {
                if (config.BaselineConfiguration != null && config.BaselineConfiguration.Template != null)
                {
                    return config.BaselineConfiguration.Template.DateRangeString;
                }
                else
                {
                    var baselineConfig = new BaselineConfiguration();
                    return baselineConfig.Template.DateRangeString;
                }
            }
        }
        //Sqldm 10.0 SQldm Srishti Purohit
        //Prescriptive analysis configuration
        public string AnalysisScheduledTimePeriod
        {
            get
            {
                var sb = new StringBuilder();
                foreach (DayOfWeek d in Enum.GetValues(typeof(DayOfWeek)))
                {
                    if (MonitoredSqlServer.MatchDayOfWeek(d, config.AnalysisConfiguration.SelectedDays))
                    {
                        sb.Append(d.ToString().Substring(0, 3));
                        sb.Append(",");
                    }
                }
                if (sb.Length > 1)
                    sb.Remove(sb.Length - 1, 1);

                sb.Append(" at ");
                sb.Append(config.AnalysisConfiguration.StartTime.TimeOfDay.ToString("t"));
                sb.Append(" for duration ");
                sb.Append(config.AnalysisConfiguration.AnalysisDuration.ToString());
                sb.Append(" mins. ");
                return sb.ToString();
            }
        }
        #endregion

        #region Methods

        public MonitoredSqlServerConfiguration GetConfiguration()
        {
            return this.config;
        }

        public static string First100Chars(IList list)
        {
            var sb = new StringBuilder();
            int i = 0;

            foreach (string d in list)
            {
                i++;
                if (sb.Length > 100 && i < list.Count)
                {
                    if (sb.Length > 1)
                        sb.Remove(sb.Length - 1, 1);
                    sb.Append(" ... ");
                    return sb.ToString();
                }
                else
                {
                    sb.Append(d);
                    sb.Append(",");
                }
            }
            if (sb.Length > 1)
                sb.Remove(sb.Length - 1, 1);

            return sb.ToString();
        }

        internal void SaveConfiguration()
        {
            ApplicationModel.Default.UpdateMonitoredSqlServer(id, config);
            originalSettings = new MonitoredServerConfigurationOriginalSettings(
                this.PreferredClusterNode,
                (TimeSpan)this.ScheduledCollectionInterval,
                this.ReorganizationMinimumTableSizeKB,
                this.ReplicationMonitoringDisabled,
                this.ExtendedHistoryCollectionDisabled,
                (TimeSpan)this.ServerPingInterval,
                (TimeSpan)this.DatabaseStatisticsInterval);
        }

        internal void ResetConfiguration()
        {
            MonitoredServerConfigurationOriginalSettings.ResetChanges(this);
        }
       

        #endregion

    }

}