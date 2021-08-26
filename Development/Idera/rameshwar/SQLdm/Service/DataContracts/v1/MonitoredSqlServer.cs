using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Xml;
using Idera.SQLdm.Service.DataContracts.v1.Database;
using Idera.SQLdm.Common;

namespace Idera.SQLdm.Service.DataContracts.v1
{
    [DataContract]
    public class MonitoredSqlServer
    {
        //Upgrading data type from int to double to support Web UI modification requirement for health coefficient
        //SQLdm 10.1 -- Srishti Purohit
        private double healthIndexCoefficientForCriticalAlert = Common.Constants.HEALTH_INDEX_COEFFICIENT_CRITICAL_ALERT;
        private double healthIndexCoefficientForWarningAlert = Common.Constants.HEALTH_INDEX_COEFFICIENT_WARNING_ALERT;
        private double healthIndexCoefficientForInformationalAlert = Common.Constants.HEALTH_INDEX_COEFFICIENT_INFO_ALERT;
        private double? healthIndexInstanceScaleFactor = null;
        private double? healthIndexAvgTagsScaleFactor = null;

       

        [DataMember]
        public int SQLServerId { get; set; }

        [DataMember]
        public string InstanceName { get; set; }

        //SQLdm 10.1 - (Pulkit Puri) - to add friendly server name
        [DataMember]
        public string FriendlyServerName { get; set; }
        // end 10.1- (Pulkit Puri)

        [DataMember]
        public bool IsActive { get; set; }

        [DataMember]
        public string MostRecentSQLVersion { get; set; }

        [DataMember]
        public string InstanceEdition { get; set; }

        //[DataMember]
        public string RegisteredDate { get; set; }

        //[DataMember]
        public string maintenanceModeEnabled { get; set; }


        //[DataMember]
        public string IsSQLSafeConnected { get; set; }

        //[DataMember]
        public string IsVirtualized { get; set; }

        //[DataMember]
        public string lastScheduledRefreshTime { get; set; }

        //[DataMember]
        public string lastReorgStatisticsTime { get; set; }

        //[DataMember]
        public string lastAlertRefreshTime { get; set; }

        //[DataMember]
        public string lastDatabaseRefreshTime { get; set; }

        [DataMember]
        public string activeInfoAlertCount { get; set; }          //SQLdm 9.0 (Abhishek Joshi) --Web Console Improvements -to add Info alert count 

        [DataMember]
        public string activeWarningAlertCount { get; set; }       //SQLdm 9.0 (Abhishek Joshi) --Web Console Improvements  -to add warning alert count  
        
        [DataMember]
        public string activeCriticalAlertCount { get; set; }      //SQLdm 9.0 (Abhishek Joshi) --Web Console Improvements -to add critical alert count 

        //START - SQLdm 9.1 (Sanjali Makkar) --Service API Changes -to add Health Index
        public double HealthIndexCoefficientForCriticalAlert
        {
            get { return healthIndexCoefficientForCriticalAlert; }
            set { healthIndexCoefficientForCriticalAlert = value; }
        }

        public double HealthIndexCoefficientForWarningAlert
        {
            get { return healthIndexCoefficientForWarningAlert; }
            set { healthIndexCoefficientForWarningAlert = value; }
        }

        public double HealthIndexCoefficientForInformationalAlert
        {
            get { return healthIndexCoefficientForInformationalAlert; }
            set { healthIndexCoefficientForInformationalAlert = value; }
        }
        public double? HealthIndexInstanceScaleFactor
        {
            get { return healthIndexInstanceScaleFactor; }
            set { healthIndexInstanceScaleFactor = value; }
        }
        public double? HealthIndexAvgTagsScaleFactor
        {
            get { return healthIndexAvgTagsScaleFactor; }
            set { healthIndexAvgTagsScaleFactor = value; }
        }

        //The Health Index value of a specific instance calculated according to the base formula
        public decimal BaseHealthIndex
        {
            get
            {
                int criticalAlertsCount;
                int warningAlertsCount;
                int infoAlertsCount;
                if (int.TryParse(activeCriticalAlertCount, out criticalAlertsCount) && int.TryParse(activeWarningAlertCount, out warningAlertsCount) && int.TryParse(activeInfoAlertCount, out infoAlertsCount))
                {
                    if (healthIndexInstanceScaleFactor != null)
                        return (decimal)(healthIndexInstanceScaleFactor * ((HealthIndexCoefficientForCriticalAlert * criticalAlertsCount) + (HealthIndexCoefficientForWarningAlert * warningAlertsCount) + (HealthIndexCoefficientForInformationalAlert * infoAlertsCount)));
                    else if(healthIndexAvgTagsScaleFactor != null)
                        return (decimal)(healthIndexAvgTagsScaleFactor * ((HealthIndexCoefficientForCriticalAlert * criticalAlertsCount) + (HealthIndexCoefficientForWarningAlert * warningAlertsCount) + (HealthIndexCoefficientForInformationalAlert * infoAlertsCount)));
                    else
                        return (decimal)((HealthIndexCoefficientForCriticalAlert * criticalAlertsCount) + (HealthIndexCoefficientForWarningAlert * warningAlertsCount) + (HealthIndexCoefficientForInformationalAlert * infoAlertsCount));
                }
                else
                    return -1;
            }
            set { }
        }

        //Relative Health Index Value calculated with respect to the maximum value of Health Index out of all the instances
        [DataMember]
        public decimal HealthIndex
        {
            get
            {
                //DM CWF Integration - Commenting the following code as when MaxHealthIndex is less than 0, response cannot be sent in JSON format as it throws exception as per the following code block.
                //if (Repository.RepositoryHelper.MaxHealthIndex < 0)
                //{
                //    throw new InvalidOperationException("max health index cannot be negative");
                //}

                //if BaseHealthIndex > 0 for even one instance, MaxHealthIndex will always be > 0
                //if BaseHealthIndex <=0, it means there are no alerts and thus healthindex has to be reported to be 100%, as if BaseHealthIndex is 0
                return (BaseHealthIndex <= 0 || Repository.RepositoryHelper.MaxHealthIndex <= 0) ? 100 : (100 - ((BaseHealthIndex / Repository.RepositoryHelper.MaxHealthIndex) * 100));
            }
            set { }
        }
        //END - SQLdm 9.1 (Sanjali Makkar) --Service API Changes -to add Health Index


        //[DataMember]
        public string customCounterCount { get; set; }

        //[DataMember]
        public string isHistoricalStatus { get; set; }

        //[DataMember]
        public int alertsSnoozing;

        //[DataMember]
        public int numberThresholds = -1;

        [DataMember]
        public string BlockedSessionCount { get; set; }

        [DataMember]
        public IList<ServerStatisticsHistory> StatisticsHistory = null;

        [DataMember]
        public IList<MonitoredSqlServerDatabase> Databases { get; set; }

        [DataMember]
        public string AvgReponseTimeinMilliSeconds { get; set; }
        [DataMember]
        public string AvgCPUActivityPercentage { get; set; }
        [DataMember]
        public string AvgDiskTimePercent { get; set; }
        [DataMember]
        public string AvgOSAvailableMemoryInKilobytes { get; set; }
        [DataMember]
        public string AvgOSTotalPhysicalMemoryInKilobytes { get; set; }

        [DataMember]
        public string UtilizedSpaceInKilobytes { get; set; }  //SQLdm 8.5 (Ankit Srivastava): for Top X API- Tempdb Utilization

        [DataMember]
        public string NumOfQueries { get; set; }  //SQLdm 8.5 (Ankit Srivastava): for Top X API- Query Monitor Event

        [DataMember]
        public string DiskUtilizationPercentage { get; set; }  //SQLdm 8.5 (Ankit Srivastava): for Top X API- Disk Space

        [DataMember]
        public long? LastRunActiveUserSessions { get; set; } //SQLdm 8.5 (Gaurav Karwal): for instance heat map

        [DataMember]
        public int? LastRunCPUActivityPercentage { get; set; }  //SQLdm 8.5 (Gaurav Karwal): for instance heat map

        [DataMember]
        public double? LastRunSqlMemoryAllocatedInKilobytes { get; set; }  //SQLdm 8.5 (Gaurav Karwal): for instance heat map

        [DataMember]
        public double? LastRunSqlMemoryUsedInKilobytes { get; set; }  //SQLdm 8.5 (Gaurav Karwal): for instance heat map

        [DataMember]
        public int? LastRunIOActivityPercentage { get; set; }  //SQLdm 8.5 (Gaurav Karwal): for instance heat map


        [DataMember]
        public int? MaxSeverity
        {
            get;
            set;
        }

        [DataMember]
        public IList<Categories> Categories
        {
            get;
            set;
        }

        public class ServerStatisticsHistory
        {
            [DataMember]
            public string ReponseTimeinMilliSeconds { get; set; }
            [DataMember]
            public string CPUActivityPercentage { get; set; }
            [DataMember]
            public string DiskTimePercent { get; set; }
            [DataMember]
            public string OSAvailableMemoryInKilobytes { get; set; }
            [DataMember]
            public string OSTotalPhysicalMemoryInKilobytes { get; set; }
            [DataMember]
            public string IdlePercentage { get; set; }
            [DataMember(EmitDefaultValue = false)]
            public DateTime UTCCollectionDateTime { get; set; }
            [DataMember]
            public string SqlMemoryUsedInKilobytes { get; set; }
            [DataMember]
            public string SqlMemoryAllocatedInKilobytes { get; set; }
        }


        public MonitoredSqlServer()
        {
            // Default init
            IsActive = true;
            Databases = new List<MonitoredSqlServerDatabase>();

            StatisticsHistory = new List<ServerStatisticsHistory>();
            Categories = new List<Categories>();
        }

        public MonitoredSqlServer(System.Xml.XmlNode node)
            : this()
        {
            // TODO: Complete member initialization


            #region process attributes
            foreach (XmlAttribute attribute in node.Attributes)
            {
                string timeString;
                switch (attribute.Name)
                {
                    case "SQLServerID":
                        int sqlServerId;
                        if (Int32.TryParse(attribute.Value, out sqlServerId))
                        {
                            SQLServerId = sqlServerId;
                        }
                        break;
                    case "InstanceName":
                        InstanceName = attribute.Value;
                        break;

                    //SQLdm 10.1 - (Pulkit Puri) - to add friendly server name
                    case "FriendlyServerName":
                       FriendlyServerName = attribute.Value;
                        break;

                    //case "ServerVersion":
                    //    ServerVersion ver = new ServerVersion(attribute.Value);
                    //    if (update && (instanceVersion == null || !instanceVersion.ToString().Equals(ver.ToString())))

                    //    instanceVersion = ver;
                    //    break;
                    case "ServerEdition":
                        InstanceEdition = attribute.Value;
                        break;
                    case "ActiveInfoAlerts":
                        activeInfoAlertCount = Int32.Parse(attribute.Value).ToString();
                        break;
                    case "ActiveWarningAlerts":
                        activeWarningAlertCount = Int32.Parse(attribute.Value).ToString();
                        break;
                    case "ActiveCriticalAlerts":
                        activeCriticalAlertCount = Int32.Parse(attribute.Value).ToString();
                        break;
                    case "CustomCounterCount":
                        customCounterCount = Int32.Parse(attribute.Value).ToString();
                        break;
                    case "MaintenanceModeEnabled":
                        maintenanceModeEnabled = ("1".Equals(attribute.Value)).ToString();
                        break;
                    case "LastScheduledCollectionTime":
                        timeString = attribute.Value;
                        if (String.IsNullOrEmpty(timeString))
                        { }
                        else
                        {
                            try
                            {
                                lastScheduledRefreshTime = DateTime.Parse(timeString).ToLocalTime().ToString();
                            }
                            catch
                            {
                            }
                        }
                        break;
                    case "LastAlertRefreshTime":
                        timeString = attribute.Value;
                        if (String.IsNullOrEmpty(timeString))
                        { }
                        else
                        {
                            try
                            {
                                lastAlertRefreshTime = DateTime.Parse(timeString).ToLocalTime().ToString();
                            }
                            catch
                            {
                            }
                        }
                        break;
                    case "LastDatabaseRefreshTime":
                        timeString = attribute.Value;
                        if (String.IsNullOrEmpty(timeString))
                        { }
                        else
                        {
                            try
                            {
                                lastDatabaseRefreshTime = DateTime.Parse(timeString).ToLocalTime().ToString();
                            }
                            catch
                            {
                            }
                        }
                        break;
                    case "LastReorgStatisticsRunTime":
                        timeString = attribute.Value;
                        if (String.IsNullOrEmpty(timeString))
                        { }
                        else
                        {
                            try
                            {
                                lastReorgStatisticsTime = DateTime.Parse(timeString).ToLocalTime().ToString();
                            }
                            catch
                            {
                            }
                        }
                        break;
                    case "ThresholdCount":
                        {
                            int oldNumberThresholds = numberThresholds;
                            string value = attribute.Value;
                            if (!String.IsNullOrEmpty(value) && Int32.TryParse(value, out numberThresholds))
                            {
                            }
                        }
                        break;
                    case "AlertsSnoozing":
                        {
                            int oldNumberSnoozing = alertsSnoozing;
                            string value = attribute.Value;
                            if (!String.IsNullOrEmpty(value) && Int32.TryParse(value, out alertsSnoozing))
                            {
                            }
                        }
                        break;
                }
            }
            #endregion


            #region process categories
            string severity = null;
            int severityId = 0;
            foreach (XmlNode categoryNode in node.ChildNodes)
            {
                if (categoryNode.Attributes.Count == 0)
                    continue;

                string category = categoryNode.Attributes["Name"].Value;

                foreach (XmlNode statusNode in categoryNode.ChildNodes)
                {
                    if (statusNode.Name == "State")
                    {
                        foreach (XmlAttribute attribute in statusNode.Attributes)
                        {
                            switch (attribute.Name)
                            {
                                case "Severity":
                                    byte sev = 0;
                                    if (Byte.TryParse(attribute.Value, out sev))
                                    {
                                        severity = ((MonitoredState)Enum.ToObject(typeof(MonitoredState), sev)).ToString();
                                        severityId = Convert.ToInt32(sev);
                                    }
                                    break;
                            }
                        }
                    }

                    if (this.Categories.Where(c => c.name.Equals(category)).Count() > 0)
                    {
                        var cateory = this.Categories.Where(c => c.name.Equals(category)).First();
                        cateory.MaxSeverity = cateory.MaxSeverity > severityId ? cateory.MaxSeverity : severityId;
                    }
                    else
                    {
                        this.Categories.Add(new Categories { name = category, MaxSeverity = severityId });
                    }
                }

            }
            if (this.Categories.Count == 0)
            {
                this.MaxSeverity = 1;
            }
            else
            {
                this.MaxSeverity = this.Categories.Select(c => c.MaxSeverity).Max();
            }

            #endregion
        }
    }
}
