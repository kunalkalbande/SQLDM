using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.Serialization;
using Idera.SQLdm.Common.Auditing;
using Idera.SQLdm.Common.Attributes;
using Idera.SQLdm.Common.Objects;
using System.Reflection;
using System.Xml.Serialization;
using System.Xml;
using System.IO;

namespace Idera.SQLdm.Common.Configuration
{
    [Serializable]
    public class AnalysisConfiguration
    {
        private bool useDefault;
        private DateTime analysisStartTime;
        //setting default values for work load
        private int analysisDuration = 1;
        private short selectedDays;
        private int sqlServerID;
        //To set default configuration for blocked categories implementing list of master CategoryID as per table data available
        private Dictionary<int, string> allCategories = new Dictionary<int, string>();

        private bool productionServer;

        private bool OLTP;

        private bool isActive = true;
        private bool schedulingStatus = false;

        private int includeDatabase;
        private string includeDatabaseName;

        private string filterApplication;

        private List<int> blockedCategoryID;
        private Dictionary<int, string> blockedCategories;

        private List<int> blockedDatabaseIDList;
        private Dictionary<int, string> blockedDatabases;

        private List<string> blockedRecommendationID;

        private AnalysisConfiguration analysisConfigTemplate;
        // For default view
        public AnalysisConfiguration()
        {
            // allCategories = new Dictionary<int, string> { { 1, "Blocking Processes" }, { 2, "Disk" }, { 3, "Database Configuration" }, { 4, "DeadLocks" }, { 5, "Index Optimization" }, { 6, "Long Running Jobs" }, { 7, "Memory" }, { 8, "Network" }, { 9, "Open Transactions" }, { 10, "Processor" }, { 11, "Query Optimization" }, { 12, "Disaster Recovery" }, { 13, "Security" }, { 14, "Server Configuration" }, { 15, "System Page Files" }, { 16, "System Upgrade Suggestion" }, { 17, "Virtual Log Files" }, { 18, "Wait Stats" } };
        }
        // For default view
        public AnalysisConfiguration(int sqlServerID)
            : this()
        {
            this.sqlServerID = sqlServerID;
        }

        public AnalysisConfiguration(int sqlServerID, bool productionServer, bool OLTP, DateTime analysisStartTime, int analysisDuration, short selectedDays, int includeDatabase, string includeDatabaseName, string filterApplication, List<int> blockedCategoryID, Dictionary<int, string> blockCategoryIDWithName, List<int> blockedDatabaseID, Dictionary<int, string> blockDatabaseIDWithName, List<string> blockedRecommendationID, bool scheduleStatus)
            : this(sqlServerID, productionServer, OLTP, analysisStartTime, analysisDuration, selectedDays, includeDatabase, includeDatabaseName, filterApplication, blockedCategoryID, blockCategoryIDWithName, blockedDatabaseID, blockDatabaseIDWithName, blockedRecommendationID)
        {
            this.schedulingStatus = scheduleStatus;
        }

        //for saved view
        public AnalysisConfiguration(int sqlServerID, bool productionServer, bool OLTP, DateTime analysisStartTime, int analysisDuration, short selectedDays, int includeDatabase, string includeDatabaseName, string filterApplication, List<int> blockedCategoryID, Dictionary<int, string> blockCategoryIDWithName, List<int> blockedDatabaseID, Dictionary<int, string> blockDatabaseIDWithName, List<string> blockedRecommendationID)
            : this()
        {
            useDefault = false;
            this.sqlServerID = sqlServerID;
            this.productionServer = productionServer;
            this.OLTP = OLTP;
            this.analysisStartTime = analysisStartTime;
            this.analysisDuration = analysisDuration;
            this.selectedDays = selectedDays;
            this.includeDatabase = includeDatabase;
            this.includeDatabaseName = includeDatabaseName;
            this.filterApplication = filterApplication;
            this.blockedCategoryID = blockedCategoryID;
            this.blockedDatabaseIDList = blockedDatabaseID;
            this.blockedRecommendationID = blockedRecommendationID;
            this.blockedCategories = blockCategoryIDWithName;
            this.blockedDatabases = blockDatabaseIDWithName;

            selectedDays |= MonitoredSqlServer.DayOfWeekToShort(DayOfWeek.Monday);
            selectedDays |= MonitoredSqlServer.DayOfWeekToShort(DayOfWeek.Tuesday);
            selectedDays |= MonitoredSqlServer.DayOfWeekToShort(DayOfWeek.Wednesday);
            selectedDays |= MonitoredSqlServer.DayOfWeekToShort(DayOfWeek.Thursday);
            selectedDays |= MonitoredSqlServer.DayOfWeekToShort(DayOfWeek.Friday);

        }

        [AuditableAttribute(false)]
        public bool UseDefault
        {
            get { return useDefault; }
            set { useDefault = value; }
        }

        [AuditableAttribute(false)]
        public DateTime StartTime
        {
            get { return analysisStartTime; }
            set { analysisStartTime = value; }
        }

        [AuditableAttribute(false)]
        public int AnalysisDuration
        {
            get { return analysisDuration; }
            set { analysisDuration = value; }
        }

        [AuditableAttribute(false)]
        public short SelectedDays
        {
            get { return selectedDays; }
            set { selectedDays = value; }
        }

        public int SqlServerID
        {
            get { return sqlServerID; }
            set { sqlServerID = value; }
        }
        public bool ProductionServer
        {
            get { return productionServer; }
            set { productionServer = value; }
        }
        public bool IsOLTP
        {
            get { return OLTP; }
            set { OLTP = value; }
        }

        public bool SchedulingStatus
        {
            get { return schedulingStatus; }
            set { schedulingStatus = value; }
        }

        public bool IsActive
        {
            get { return isActive; }
            set { isActive = value; }
        }
        public int IncludeDatabase
        {
            get { return includeDatabase; }
            set { includeDatabase = value; }
        }
        public string IncludeDatabaseName
        {
            get { return includeDatabaseName; }
            set { includeDatabaseName = value; }
        }
        public string FilterApplication
        {
            get { return filterApplication; }
            set { filterApplication = value; }
        }
        public List<int> BlockedCategoryID
        {
            get
            {
                if (blockedCategoryID == null)
                {
                    if (allCategories != null)
                    {
                        blockedCategoryID = new List<int>();
                        foreach (var key in allCategories.Keys)
                        {
                            blockedCategoryID.Add(key);
                        }
                    }
                    return blockedCategoryID;
                }
                else
                    return blockedCategoryID;
            }
            set { blockedCategoryID = value; }
        }
        public Dictionary<int, string> BlockedCategories
        {
            get
            {
                if (blockedCategories == null)
                {
                    if (allCategories != null)
                    {
                        blockedCategories = new Dictionary<int, string>();
                        foreach (KeyValuePair<int,string> tempCategory in allCategories)
                        {
                            blockedCategories.Add(tempCategory.Key, tempCategory.Value);
                        }
                    }
                    return blockedCategories;
                }
                else
                    return blockedCategories;

            }
            set { blockedCategories = value; }
        }
        public List<int> BlockedDatabaseIDList
        {
            get { return blockedDatabaseIDList; }
            set { blockedDatabaseIDList = value; }
        }
        public Dictionary<int, string> BlockedDatabases
        {
            get { return blockedDatabases; }
            set { blockedDatabases = value; }
        }

        //public IDictionary<int, string> BlockedDatabaseIDDictionary
        //{
        //    get { return blockedDatabaseIDDictionary; }
        //    set { blockedDatabaseIDDictionary = value; }
        //}
        public List<string> BlockedRecommendationID
        {
            get { return blockedRecommendationID; }
            set { blockedRecommendationID = value; }
        }

        #region ISerializable Members

        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            analysisConfigTemplate = new AnalysisConfiguration(sqlServerID);
            analysisConfigTemplate.StartTime = analysisStartTime;
            analysisConfigTemplate.AnalysisDuration = analysisDuration;
            analysisConfigTemplate.IsOLTP = OLTP;
            analysisConfigTemplate.ProductionServer = productionServer;

            //info.AddValue("UseDefault", template.UseDefault);
            info.AddValue("StartTime", analysisStartTime.ToUniversalTime());
            info.AddValue("Duration", analysisDuration);
            info.AddValue("SelectedDays", selectedDays);
            info.AddValue("IsOLTP", OLTP);
        }

        #endregion

        #region Methods
        /// <summary>
        /// Returns an Auditable Entity
        /// </summary>
        /// <returns></returns>
        public AuditableEntity GetAuditableEntity()
        {
            AuditableEntity entity = new AuditableEntity();

            PropertyInfo[] properties = analysisConfigTemplate.GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance);

            foreach (PropertyInfo property in properties)
            {
                var attributes = property.GetCustomAttributes(typeof(AuditableAttribute), true);
                String propertyName = property.Name;

                if (attributes.Length > 0)
                {
                    AuditableAttribute attribute = (AuditableAttribute)attributes[0];

                    if (!attribute.IsAuditable)
                    {
                        continue;
                    }

                    if (!String.IsNullOrEmpty(attribute.NewPropertyName))
                    {
                        propertyName = attribute.NewPropertyName;
                    }

                    entity.AddMetadataProperty(propertyName, property.GetValue(analysisConfigTemplate, null).ToString());
                }
            }


            return entity;
        }

        /// <summary>
        /// Returns an Auditable Entity based on an oldValue
        /// </summary>
        /// <param name="oldValue"></param>
        /// <returns></returns>
        public AuditableEntity GetAuditableEntity(IAuditable oldValue)
        {
            AuditableEntity entity = new AuditableEntity();
            AnalysisConfiguration oldConfiguration = oldValue as AnalysisConfiguration;

            List<PropertiesComparer.PropertiesData> propertiesChanged = null;

            if (oldConfiguration != null && HasChangedProperties(oldConfiguration, out propertiesChanged))
            {
                // Build metadata.
                propertiesChanged.ForEach(property => entity.AddMetadataProperty(property.Name, property.Value));
            }

            // If the configuration does not changed, returns an empty entity.
            return entity;
        }

        /// <summary>
        /// Try to get the changed properties in to a list. Returns true if has succeeded and false in other case.
        /// </summary>
        /// <param name="oldConfiguration">Old object</param>
        /// <param name="propertiesChanged">The changed properties.</param>
        /// <returns>Returns true if has succeeded and false in other case.</returns>
        private bool HasChangedProperties(AnalysisConfiguration oldConfiguration,
            out List<PropertiesComparer.PropertiesData> propertiesChanged)
        {
            PropertiesComparer comparer = new PropertiesComparer();
            // Get changed properties.
            propertiesChanged = comparer.GetNewProperties(oldConfiguration, this.analysisConfigTemplate);

            return propertiesChanged.Count > 0;
        }

        //to Convert array of strings to List<int>
        private static List<int> GetINTListFromArray(string[] arrToconvert)
        {
            List<int> list = new List<int>();
            foreach (string item in arrToconvert)
            {
                list.Add(Convert.ToInt32(item));
            }
            return list;
        }
        #endregion

    }

    [Serializable()]
    public class AnalysisTemplate
    {
        //[AuditableAttribute("Time Period")]
        //public string TimePeriodString
        //{
        //    get
        //    {
        //        var sb = new StringBuilder();
        //        foreach (DayOfWeek d in Enum.GetValues(typeof (DayOfWeek)))
        //        {
        //            if (MonitoredSqlServer.MatchDayOfWeek(d, selectedDays))
        //            {
        //                sb.Append(d.ToString().Substring(0, 3));
        //                sb.Append(",");
        //            }
        //        }
        //        if (sb.Length > 1)
        //            sb.Remove(sb.Length - 1, 1);

        //        sb.Append(" between ");
        //        sb.Append(StartDate.ToString("t"));
        //        sb.Append(" and ");
        //        sb.Append(AnalysisDuration.ToString("t"));
        //        return sb.ToString();
        //    }
        //}

        //[Auditable("Date Range")]
        //public string DateRangeString
        //{
        //    get
        //    {
        //        if (UseDefault)
        //            return "Automatic (Last 7 days)";
        //        else
        //        {
        //            return "Custom (" + StartDate.ToString("d") + " - " + AnalysisDuration.ToString("d") + ")";
        //        }
        //    }
        //}


    }

    public enum AnalysisCollectorType
    {
        Startup,
        Interval,
        Shutdown,
        DatabaseSpecific
    }
}
