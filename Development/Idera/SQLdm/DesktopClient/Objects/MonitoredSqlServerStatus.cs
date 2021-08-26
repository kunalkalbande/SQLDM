//------------------------------------------------------------------------------
// <copyright file="MonitoredSqlServerState.cs" company="Idera, Inc.">
//     Copyright (c) Idera, Inc. All rights reserved.
// </copyright>
//------------------------------------------------------------------------------
namespace Idera.SQLdm.DesktopClient.Objects
{
    using System;
    using System.Collections.Generic;
    using System.Drawing;
    using System.Text;
    using System.Xml;
    using Idera.SQLdm.Common;
    using Idera.SQLdm.Common.Events;
    using Wintellect.PowerCollections;
    using Idera.SQLdm.Common.Configuration;
    using Idera.SQLdm.DesktopClient.Properties;
    using Idera.SQLdm.DesktopClient.Helpers;
using Idera.SQLdm.Common.Objects;

    public class MonitoredSqlServerStatus
    {
        private static BBS.TracerX.Logger LOG = BBS.TracerX.Logger.GetLogger("MonitoredSqlServerStatus");

        private int instanceID;
        private MonitoredSqlServerWrapper instance;

        private string instanceName;
        private ServerVersion instanceVersion;
        private string instanceEdition;
        private MonitoredState severity; 
        private bool maintenanceModeEnabled;
        private DateTime? lastScheduledRefreshTime;
        private DateTime? lastReorgStatisticsTime;
        private DateTime? lastAlertRefreshTime;
        private DateTime? lastDatabaseRefreshTime;
        private int activeInfoAlertCount;
        private int activeWarningAlertCount;
        private int activeCriticalAlertCount;
        private int customCounterCount;
        public readonly Dictionary<string, DatabaseStatus> DatabaseMap;
        private OrderedMultiDictionary<MetricCategory, Issue> issuesMap;
        private Issue[] topIssues = new Issue[0];//SQLdm 10.0 (Tarun Sapra)- Initiating this member here, to handle null reference exceptions in case of azure instance
        private bool isHistoricalStatus = false;
        private int alertsSnoozing = 0;
        private int numberThresholds = -1;
        private bool allAlertsSnoozed = false;

        public MonitoredSqlServerStatus()
        {
            DatabaseMap = new Dictionary<string, DatabaseStatus>();
            issuesMap = new OrderedMultiDictionary<MetricCategory, Issue>(false);
        }

        public MonitoredSqlServerStatus(XmlNode node) : this()
        {
            Initialize(node, false);
        }

        internal static MonitoredSqlServerStatus CreateDesignTimeStatus()
        {
            var config = new MonitoredSqlServerConfiguration("Design");
            var server = new MonitoredSqlServer(1, DateTime.Now, config);
            var wrapper = new MonitoredSqlServerWrapper(server);
            var version = new ServerVersion("10.50.2500");
            try
            {
                return new MonitoredSqlServerStatus()
                {
                    instanceID = 1,
                    instance = wrapper,
                    activeCriticalAlertCount = 8,
                    activeWarningAlertCount = 4,
                    activeInfoAlertCount = 2,
                    customCounterCount = 1,
                    instanceName = "Design",
                    instanceVersion = version,
                    instanceEdition = "Developer",
                    severity = MonitoredState.Critical,
                    maintenanceModeEnabled = false,
                    lastReorgStatisticsTime = DateTime.Now - TimeSpan.FromHours(4),
                    lastScheduledRefreshTime = DateTime.Now - TimeSpan.FromMinutes(6),
                    lastDatabaseRefreshTime = DateTime.Now - TimeSpan.FromMinutes(6),
                    lastAlertRefreshTime = DateTime.Now - TimeSpan.FromMinutes(6),
                    topIssues = new Issue[0],
                    numberThresholds = 0
                };
            }
            catch (Exception e)
            {
                /* */
            }
            return null;
        }

        /// <summary>
        /// Gets the status object updated during the last background refresh.
        /// </summary>
        /// <param name="instanceId"></param>
        /// <returns></returns>
        public static MonitoredSqlServerStatus FromBackgroundRefresh(int instanceId)
        {
            return ApplicationModel.Default.GetInstanceStatus(instanceId);
        }

        /// <summary>
        /// Queries the database and constructs a status object for the specified instance.
        /// </summary>
        /// <param name="instanceId"></param>
        /// <returns></returns>
        public static MonitoredSqlServerStatus GetStatus(int instanceId)
        {
            MonitoredSqlServerStatus status = null;
            using (LOG.DebugCall("GetStatus"))
            {
                try
                {
                    SqlConnectionInfo connectionInfo = Settings.Default.ActiveRepositoryConnection.ConnectionInfo;
                    XmlDocument document = RepositoryHelper.GetMonitoredSqlServerStatus(connectionInfo, instanceId);
                    if (document.DocumentElement.ChildNodes.Count > 0)
                    {
                        XmlNode node = document.DocumentElement.ChildNodes[0];
                        status = new MonitoredSqlServerStatus(node);
                    }
                }
                catch (Exception e)
                {
                    LOG.Error("Status query failed for server id=" + instanceId, e);
                    throw;
                }
                return status;
            }
        }

        public ICollection<Issue> this[MetricCategory category]
        {
            get
            {
                if (category == MetricCategory.Custom)
                {
                    List<Issue> customIssues = new List<Issue>();
                    ICollection<Issue> issues = issuesMap.Values;
                    foreach (Issue issue in issues)
                    {
                        if (!Enum.IsDefined(typeof (Metric), (int) issue.Metric))
                        {
                            customIssues.Add(issue);
                        }
                    }
                    return customIssues;
                }
                return issuesMap[category];
            }
        }

        public ICollection<Issue> this[Metric metric]
        {
            get
            {
                List<Issue> result = new List<Issue>();

                if (issuesMap != null)
                {
                    foreach (KeyValuePair<MetricCategory, ICollection<Issue>> entry in issuesMap.ToArray())
                    {
                        foreach (Issue issue in entry.Value)
                        {
                            if (issue.Metric == metric)
                                result.Add(issue);
                        }
                        if (result.Count > 0)
                            break;
                    }
                }

                return result;
            }
        }

        public MonitoredSqlServerWrapper Instance
        {
            get { return instance; }
            set 
            { 
                if (instance == null && value != null)
                {
                    instance = value;
//                    instance.Changed += new EventHandler<MonitoredSqlServerChangedEventArgs>(instance_Changed);
                } 
            }
        }

        public ServerVersion InstanceVersion
        {
            get { return instanceVersion; }
        }

        public string InstanceEdition
        {
            get { return instanceEdition == null ? "" : instanceEdition; }
        }

        public int ActiveInfoAlertCount
        {
            get { return activeInfoAlertCount; }
        }

        public int ActiveWarningAlertCount
        {
            get { return activeWarningAlertCount;  }
        }
        
        public int ActiveCriticalAlertCount
        {
            get { return activeCriticalAlertCount; }
        }

        public int CustomCounterCount
        {
            get { return customCounterCount; }
        }

        public DateTime? LastScheduledRefreshTime
        {
            get { return lastScheduledRefreshTime; }
        }

        public DateTime? LastReorgStatisticsTime
        {
            get { return lastReorgStatisticsTime; }
        }

        public DateTime? LastAlertRefreshTime
        {
            get { return lastAlertRefreshTime; }
        }

        public DateTime? LastDatabaseRefreshTime
        {
            get { return lastDatabaseRefreshTime; }
        }

        /// <summary>
        /// Update the guts of the status object and return bool if we need to 
        /// fire the server changed event.
        /// </summary>
        /// <param name="node"></param>
        /// <returns></returns>
        public bool Update(XmlNode node)
        {
            return Initialize(node, true);
        }

        private bool Initialize(XmlNode node, bool update)
        {
            bool fireEvent = false;

            MonitoredState oldSeverity = severity;

            OrderedSet<Issue> topIssues = new OrderedSet<Issue>();
            foreach (XmlAttribute attribute in node.Attributes)
            {
                string timeString;
                switch (attribute.Name)
                {
                    case "SQLServerID":
                        if (!update)
                            Int32.TryParse(attribute.Value, out this.instanceID);
                        break;
                    case "InstanceName":
                        instanceName = attribute.Value;
                        break;
                    case "ServerVersion":
                        ServerVersion ver = new ServerVersion(attribute.Value);
                        if (update && (instanceVersion == null || !instanceVersion.ToString().Equals(ver.ToString())))
                            fireEvent = true;
                        instanceVersion = ver;
                        break;
                    case "ServerEdition":
                        if (update && instanceEdition != attribute.Value)
                            fireEvent = true;
                        instanceEdition = attribute.Value;
                        break;
                    case "ActiveInfoAlerts":
                        activeInfoAlertCount = Int32.Parse(attribute.Value);
                        break;
                    case "ActiveWarningAlerts":
                        activeWarningAlertCount = Int32.Parse(attribute.Value);
                        break;
                    case "ActiveCriticalAlerts":
                        activeCriticalAlertCount = Int32.Parse(attribute.Value);
                        break;
                    case "CustomCounterCount":
                        customCounterCount = Int32.Parse(attribute.Value);
                        break;
                    case "MaintenanceModeEnabled":
                        bool oldMaintMode = maintenanceModeEnabled;
                        maintenanceModeEnabled = ("1".Equals(attribute.Value));
                        if (update && oldMaintMode != maintenanceModeEnabled)
                            fireEvent = true;
                        // make the instance match the database
                        if (instance != null && instance.MaintenanceModeEnabled != maintenanceModeEnabled)
                            instance.MaintenanceModeEnabled = maintenanceModeEnabled;
                        break;
                    case "LastScheduledCollectionTime":
                        timeString = attribute.Value;
                        if (String.IsNullOrEmpty(timeString))
                            lastScheduledRefreshTime = null;
                        else
                        {
                            try
                            {
                                lastScheduledRefreshTime = DateTime.Parse(timeString).ToLocalTime();
                            } 
                            catch
                            {
                            }
                        }
                        break;
                    case "LastAlertRefreshTime":
                        timeString = attribute.Value;
                        if (String.IsNullOrEmpty(timeString))
                            lastAlertRefreshTime = null;
                        else
                        {
                            try
                            {
                                lastAlertRefreshTime = DateTime.Parse(timeString).ToLocalTime();
                            }
                            catch
                            {
                            }
                        }
                        break;
                    case "LastDatabaseRefreshTime":
                        timeString = attribute.Value;
                        if (String.IsNullOrEmpty(timeString))
                            lastDatabaseRefreshTime = null;
                        else
                        {
                            try
                            {
                                lastDatabaseRefreshTime = DateTime.Parse(timeString).ToLocalTime();
                            }
                            catch
                            {
                            }
                        }
                        break;
                    case "LastReorgStatisticsRunTime":
                        timeString = attribute.Value;
                        if (String.IsNullOrEmpty(timeString))
                            lastReorgStatisticsTime = null;
                        else
                        {
                            try
                            {
                                lastReorgStatisticsTime = DateTime.Parse(timeString).ToLocalTime();
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
                                if (update && oldNumberThresholds != numberThresholds)
                                    fireEvent = true;
                            }
                        }
                        break;
                    case "AlertsSnoozing":
                        {
                            int oldNumberSnoozing = alertsSnoozing;
                            string value = attribute.Value;
                            if (!String.IsNullOrEmpty(value) && Int32.TryParse(value, out alertsSnoozing))
                            {
                                if (update && oldNumberSnoozing != alertsSnoozing)
                                    fireEvent = true;
                            }
                        }
                        break;
                }
            }

            allAlertsSnoozed = (alertsSnoozing >= numberThresholds);

            topIssues.Clear();
            issuesMap.Clear();
            DatabaseMap.Clear();

            if (Instance == null)
            {
                if (ApplicationModel.Default.ActiveInstances.Contains(instanceID))
                    Instance = ApplicationModel.Default.ActiveInstances[instanceID];
            }

            // process the child nodes
            foreach (XmlNode categoryNode in node.ChildNodes)
            {
                if (categoryNode.Attributes.Count == 0)
                    continue;

                string category = categoryNode.Attributes["Name"].Value;
                MetricCategory metricCategory;
                if (Enum.IsDefined(typeof(MetricCategory), category))
                {
                    metricCategory = (MetricCategory) Enum.Parse(typeof (MetricCategory), category);
                }
                else
                {
                    metricCategory = MetricCategory.General;
                }

                foreach (XmlNode statusNode in categoryNode.ChildNodes)
                {
                    if (statusNode.Name == "Database")
                    {
                        // only process database nodes in the <Databases> parent
                        if (!"Databases".Equals(category))
                            continue;

                        try
                        {
                            string dbName = statusNode.Attributes["Name"].Value;
                            DatabaseStatus dbStatus;
                            if (!DatabaseMap.TryGetValue(dbName, out dbStatus))
                            {
                                string id = statusNode.Attributes["Id"].Value;
                                string sysdb = statusNode.Attributes["IsSystemDatabase"].Value;
                                dbStatus = new DatabaseStatus(this, Int32.Parse(id), dbName, sysdb != "0");
                                DatabaseMap.Add(dbName, dbStatus);
                            }
                            ParseDatabaseStatus(dbStatus, statusNode);
                        }
                        catch (Exception e)
                        {
                            LOG.WarnFormat("Exeption parsing database status: {0}",e);
                        }
                    } else
                    if (statusNode.Name == "State")
                    {
                        MonitoredState issueSeverity = MonitoredState.None;
                        Metric metric = Metric.ProductVersion;
                        int rank = 0;
                        string subject = null;
                        DateTime occurenceTime = default(DateTime);
                        foreach (XmlAttribute attribute in statusNode.Attributes)
                        {
                            switch (attribute.Name)
                            {
                                case "Rank":
                                    Int32.TryParse(attribute.Value, out rank);
                                    break;
                                case "Severity":
                                    byte sev = 0;
                                    if (Byte.TryParse(attribute.Value, out sev))
                                    {
                                        issueSeverity = (MonitoredState)Enum.ToObject(typeof(MonitoredState), sev);
                                    }
                                    break;
                                case "Metric":
                                    int imetric = 0;
                                    if (Int32.TryParse(attribute.Value, out imetric))
                                    {
                                        metric = (Metric)Enum.ToObject(typeof(Metric), imetric);
                                    }
                                    break;
                                case "Subject":
                                    subject = attribute.Value;
                                    break;
                                case "OccurenceTime":
                                    string timeString = attribute.Value;
                                    if (String.IsNullOrEmpty(timeString))
                                        occurenceTime = default(DateTime);
                                    else
                                    {
                                        try
                                        {
                                            occurenceTime = DateTime.Parse(timeString).ToLocalTime();
                                        }
                                        catch
                                        {
                                        }
                                    }
                                    break;
                            }
                        }

                        Issue issue = new Issue(metric, issueSeverity, rank, subject, occurenceTime);
                        issuesMap.Add(metricCategory, issue);
                        topIssues.Add(issue);
                    }
                }
            }

            this.topIssues = topIssues.ToArray();

            if (!IsSupportedVersion)
                this.severity = MonitoredState.Critical;
            else
                if (lastAlertRefreshTime != null && topIssues.Count == 0)
                    this.severity = MonitoredState.OK;
                else
                    // If the top issue is Informational, then the server state is still OK
                    if (topIssues.Count > 0 && topIssues[0].Severity != MonitoredState.Informational)
                        this.severity = topIssues[0].Severity;
                    else
                        this.severity = MonitoredState.OK;

            if (update || oldSeverity != severity)
                fireEvent = true;

            return fireEvent;
        }

//        void instance_Changed(object sender, MonitoredSqlServerChangedEventArgs e)
//        {
//            if (e.Instance.MaintenanceModeEnabled != this.IsInMaintenanceMode)
//                this.IsInMaintenanceMode = e.Instance.MaintenanceModeEnabled;
//        }

        private void ParseDatabaseStatus(DatabaseStatus dbStatus, XmlNode statusNode)
        {
            OrderedSet<Issue> topIssues = new OrderedSet<Issue>();

            foreach (XmlNode childNode in statusNode.ChildNodes)
            {
                if (childNode.Name == "State")
                {
                    MonitoredState issueSeverity = MonitoredState.None;
                    Metric metric = Metric.ProductVersion;
                    int rank = 0;
                    string subject = null;
                    DateTime occurenceTime = default(DateTime);
                    foreach (XmlAttribute attribute in childNode.Attributes)
                    {
                        switch (attribute.Name)
                        {
                            case "Rank":
                                Int32.TryParse(attribute.Value, out rank);
                                break;
                            case "Severity":
                                byte sev = 0;
                                if (Byte.TryParse(attribute.Value, out sev))
                                {
                                    issueSeverity = (MonitoredState)Enum.ToObject(typeof(MonitoredState), sev);
                                }
                                break;
                            case "Metric":
                                int imetric = 0;
                                if (Int32.TryParse(attribute.Value, out imetric))
                                {
                                    metric = (Metric)Enum.ToObject(typeof(Metric), imetric);
                                }
                                break;
                            case "Subject":
                                subject = attribute.Value;
                                break;
                            case "OccurenceTime":
                                string timeString = attribute.Value;
                                if (String.IsNullOrEmpty(timeString))
                                    occurenceTime = default(DateTime);
                                else
                                {
                                    try
                                    {
                                        occurenceTime = DateTime.Parse(timeString).ToLocalTime();
                                    }
                                    catch
                                    {
                                    }
                                }
                                break;
                        }
                    }
                    Issue issue = new Issue(metric, issueSeverity, rank, subject, occurenceTime);
                    topIssues.Add(issue);
                }
                else
                if (childNode.Name == "Table")
                {
                    string tableName = TableStatus.GetTableName(childNode.Attributes["Schema"].Value, childNode.Attributes["Name"].Value);
                    TableStatus tableStatus;
                    if (!dbStatus.TableStatus.TryGetValue(tableName, out tableStatus))
                    {
                        string id = childNode.Attributes["Id"].Value;
                        string systable = childNode.Attributes["IsSystemTable"].Value;
                        tableStatus = new TableStatus(dbStatus, Int32.Parse(id), tableName, systable != "0");
                        dbStatus.TableStatus.Add(tableName, tableStatus);
                    }
                    ParseTableStatus(tableStatus, childNode);
                }
            }

            dbStatus.IssueCount = topIssues.Count;
            int count = dbStatus.IssueCount;
            for (int i = 0; i < count; i++)
                dbStatus.Issues[i] = topIssues[i];
        }

        private void ParseTableStatus(TableStatus tableStatus, XmlNode tableNode)
        {
            int fragmentation = 0;
            string frag = tableNode.Attributes["Fragmentation"].Value;
            Int32.TryParse(frag, out fragmentation);

            MonitoredState state = MonitoredState.OK;

            XmlAttribute sevAttribute = tableNode.Attributes["Severity"];
            if (sevAttribute != null)
            {
                byte sevValue = 1;
                Byte.TryParse(sevAttribute.Value, out sevValue);
                state = (MonitoredState)Enum.ToObject(typeof (MonitoredState), sevValue);
            }
            if (state != MonitoredState.OK)
            {
                string subject = String.Format("Table {0} is {1}% fragmented", tableStatus.TableName, fragmentation);
                Issue issue = new Issue(Metric.ReorganisationPct, state, 50, subject, DateTime.MinValue);
                tableStatus.IssueCount = 1;
                tableStatus.Issues[0] = issue;
            }
        }

        public int InstanceID
        {
            get { return instanceID;  }
            set { instanceID = value; }
        }
        
        public string InstanceName
        {
            get { return instanceName; }
            set { instanceName = value; }
        }
        
        public MonitoredState Severity
        {
            get { return severity; }
            set { severity = value; }
        }

        public bool IsSupportedVersion
        {
            get 
            { 
                if (instanceVersion != null && !instanceVersion.IsSupported)
                    return false;
                //SQLdm 8.6 (Ankit Srivastava) - Commented the part which makes Express Edition Unsupported 
                //if (instanceEdition != null && instanceEdition.ToLower().Equals("express edition"))
                //    return false;
                
                return true;
            }
        }

        public int ThresholdCount
        {
            get { return numberThresholds; }
        }

        public int SnoozingAlertCount
        {
            get { return alertsSnoozing; }
        }

        public bool AreAllAlertsSnoozed
        {
            get { return allAlertsSnoozed; }
        }

        public void SetAlertsSnoozed(int countSnoozed)
        {
            if (!isHistoricalStatus)
            {
                if (countSnoozed == -2)
                    countSnoozed = numberThresholds;
                else
                if (countSnoozed == -1)
                    countSnoozed = alertsSnoozing + 1;
                
                allAlertsSnoozed = countSnoozed == numberThresholds;
                if (allAlertsSnoozed)
                {
                    if (severity > MonitoredState.OK)
                    {
                        severity = MonitoredState.OK;
                    }
                }
                alertsSnoozing = countSnoozed;
            }
        }

        public bool IsInMaintenanceMode
        {
            get
            {
                if (IsHistoricalStatus)
                {
                    // We do not store snapshots for servers that are in Maintenance Mode, so when
                    // using a status object that represents historical status, Maintenance Mode
                    // should be ignored.
                    return false;
                }
                else
                {
                    return instance == null ? maintenanceModeEnabled : instance.MaintenanceModeEnabled;
                }
            }
        }

        public bool IsRefreshAvailable
        {
            get { return lastAlertRefreshTime != null;  }
        }

        public bool IsOffline
        {
            get
            {
                // see if there is an active alert on the sql server status
                if (issuesMap.ContainsKey(MetricCategory.Services))
                {
                    foreach (Issue issue in issuesMap[MetricCategory.Services])
                    {
                        if (issue.Metric == Metric.SqlServiceStatus)
                            return true;
                    }
                }

                return false;
            }
        }

        public bool IsHistoricalStatus
        {
            get { return isHistoricalStatus; }
            set { isHistoricalStatus = value; }
        }

        public string ServerImageKey
        {
            get
            {
                if (IsInMaintenanceMode)
                    return "ServerMaintenanceMode";
                if (!IsSupportedVersion)
                    return "ServerCritical";
                if (!IsRefreshAvailable && topIssues.Length == 0)
                    return "ServerOK";

                if (IsRefreshAvailable || topIssues.Length > 0)
                {
                    switch (Severity)
                    {
                        case MonitoredState.Critical:
                            return "ServerCritical";
                        case MonitoredState.Warning:
                            return "ServerWarning";
                        case MonitoredState.OK:
                            return "ServerOK";
                    }
                }
                else
                if (!IsRefreshAvailable)
                    return "ServerOK";

                return "ServerInformation";
            }
        }
        public string AzureImageKey
        {
            get
            {
                if (IsInMaintenanceMode)
                    return "CloudServerMaintenance";
                if (!IsSupportedVersion)
                    return "CloudServerCritical";
                if (!IsRefreshAvailable && topIssues.Length == 0)
                    return "CloudServerOk";

                if (IsRefreshAvailable || topIssues.Length > 0)
                {
                    switch (Severity)
                    {
                        case MonitoredState.Critical:
                            return "CloudServerCritical";
                        case MonitoredState.Warning:
                            return "CloudServerWarning";
                        case MonitoredState.OK:
                            return "CloudServerOk";
                    }
                }
                else
                if (!IsRefreshAvailable)
                    return "CloudServerOk";

                return "CloudServerInformation";
            }
        }

        public Image ServerImage
        {
            get
            {
                if (IsInMaintenanceMode)
                    return global::Idera.SQLdm.DesktopClient.Properties.Resources.ServerMaintenanceMode;
                if (!IsSupportedVersion)
                    return global::Idera.SQLdm.DesktopClient.Properties.Resources.ServerCritical;

                if (IsRefreshAvailable || topIssues.Length > 0)
                {
                    switch (Severity)
                    {
                        case MonitoredState.Critical:
                            return global::Idera.SQLdm.DesktopClient.Properties.Resources.ServerCritical;
                        case MonitoredState.Warning:
                            return global::Idera.SQLdm.DesktopClient.Properties.Resources.ServerWarning;
                        case MonitoredState.OK:
                            return global::Idera.SQLdm.DesktopClient.Properties.Resources.ServerOK16x16;
                    }
                } else
                if (!IsRefreshAvailable)
                    return global::Idera.SQLdm.DesktopClient.Properties.Resources.ServerOK16x16;
                
                return global::Idera.SQLdm.DesktopClient.Properties.Resources.ServerInformation;
            }
        }

        public Image AzureImage
        {
            get
            {
                if (IsInMaintenanceMode)
                    return global::Idera.SQLdm.DesktopClient.Properties.Resources.ServerMaintenanceMode;
                if (!IsSupportedVersion)
                    return global::Idera.SQLdm.DesktopClient.Properties.Resources.CloudServerCritical;

                if (IsRefreshAvailable || topIssues.Length > 0)
                {
                    switch (Severity)
                    {
                        case MonitoredState.Critical:
                            return global::Idera.SQLdm.DesktopClient.Properties.Resources.CloudServerCritical;
                        case MonitoredState.Warning:
                            return global::Idera.SQLdm.DesktopClient.Properties.Resources.CloudServerWarning;
                        case MonitoredState.OK:
                            return global::Idera.SQLdm.DesktopClient.Properties.Resources.CloudServerNormal;
                    }
                }
                else
                if (!IsRefreshAvailable)
                    return global::Idera.SQLdm.DesktopClient.Properties.Resources.CloudServerNormal;

                return global::Idera.SQLdm.DesktopClient.Properties.Resources.CloudServerInformation;
            }
        }
        public Image ToolTipHeadingAzureImage
        {
            get
            {
                switch (ServerImageKey)
                {
                    case "ServerMaintenanceMode":
                        return global::Idera.SQLdm.DesktopClient.Properties.Resources.CloudServerMaintenance;
                    case "CloudCriticalSmall":
                        return global::Idera.SQLdm.DesktopClient.Properties.Resources.CloudServerCritical;
                    case "CloudWarningSmall":
                        return global::Idera.SQLdm.DesktopClient.Properties.Resources.CloudServerWarning;
                    case "CloudOkSmall":
                        return global::Idera.SQLdm.DesktopClient.Properties.Resources.CloudServerNormal;
                    default:
                        return global::Idera.SQLdm.DesktopClient.Properties.Resources.InformationSmall;
                }
            }
        }

        public Image ToolTipHeadingImage
        {
            get
            {
                switch (ServerImageKey)
                {
                    case "ServerMaintenanceMode":
                        return global::Idera.SQLdm.DesktopClient.Properties.Resources.MaintenanceMode16x16;
                    case "ServerCritical":
                        return global::Idera.SQLdm.DesktopClient.Properties.Resources.StatusCriticalSmall;
                    case "ServerWarning":
                        return global::Idera.SQLdm.DesktopClient.Properties.Resources.StatusWarningSmall;
                    case "ServerOK":
                        return global::Idera.SQLdm.DesktopClient.Properties.Resources.StatusOKSmall;
                    default:
                        return global::Idera.SQLdm.DesktopClient.Properties.Resources.InformationSmall;
                }
            }
        }

        public string ToolTipHeading
        {
            get
            {
                if (IsInMaintenanceMode)
                    return String.Format("{0} is in maintenance mode", instance.DisplayInstanceName);
                if (!IsSupportedVersion)
                {
                    if (!InstanceVersion.IsSupported)
                        return String.Format("Instance version ({0}) is not supported by SQL Diagnostic Manager",
                                             instanceVersion);
                    else
                        return String.Format("Instance version ({0} {1}) is not supported by SQL Diagnostic Manager",
                                             instanceVersion, instanceEdition);
                }
                if (this.lastScheduledRefreshTime == null && this.topIssues.Length == 0)
                    return String.Format("No scheduled refresh data is available for {0}", instance.DisplayInstanceName);

                return String.Format("{0} - {1}", instance.DisplayInstanceName, severity.ToString());
            }
        }

        public string GetToolTip(MetricCategory category)
        {
            if (IsInMaintenanceMode)
                return "";

            ICollection<Issue> issues = null;
            if (category == MetricCategory.All)
            {
                issues = topIssues;
            } else
            {
                issues = this[category];
            }
            if (issues == null || issues.Count == 0)
                return "";

            int count = 0;
            StringBuilder buffer = new StringBuilder();
            foreach (Issue issue in issues)
            {
                if (buffer.Length > 0)
                    buffer.Append("\n");

                buffer.AppendFormat("{0}. {1}",
                    ++count,
                    issue.Subject
                    );
                if (count >= 3)
                    break;
            }
            return buffer.ToString();
        }

        public string ToolTip
        {
            get
            {
                return GetToolTip(MetricCategory.All);
            }
        }

        public static string GetServerImageKey(int instanceID)
        {
            MonitoredSqlServerStatus status = ApplicationModel.Default.GetInstanceStatus(instanceID);
            return status != null ?
                status.ServerImageKey :
                "ServerOK";
        }
        public static string GetAzureImageKey(int instanceID)
        {
            MonitoredSqlServerStatus status = ApplicationModel.Default.GetInstanceStatus(instanceID);
            return status != null ?
                status.AzureImageKey :
                "CloudServerOk";
        }
        public static Image GetSeverityImage(MonitoredState severity)
        {
            switch (severity)
            {
                case MonitoredState.Critical:
                    return global::Idera.SQLdm.DesktopClient.Properties.Resources.StatusCriticalSmall;
                case MonitoredState.Warning:
                    return global::Idera.SQLdm.DesktopClient.Properties.Resources.StatusWarningSmall;
                case MonitoredState.OK:
                    return global::Idera.SQLdm.DesktopClient.Properties.Resources.StatusOKSmall;
                default:
                    return global::Idera.SQLdm.DesktopClient.Properties.Resources.InformationSmall;
            }
        }
        public static Image GetSeverityAzureImage(MonitoredState severity)
        {
            switch (severity)
            {
                case MonitoredState.Critical:
                    return global::Idera.SQLdm.DesktopClient.Properties.Resources.CloudServerCritical;
                case MonitoredState.Warning:
                    return global::Idera.SQLdm.DesktopClient.Properties.Resources.CloudServerWarning;
                case MonitoredState.OK:
                    return global::Idera.SQLdm.DesktopClient.Properties.Resources.CloudServerOk;
                default:
                    return global::Idera.SQLdm.DesktopClient.Properties.Resources.CloudServerInformation;
            }
        }

    }

    public class DatabaseStatus
    {
        public readonly int DatabaseId;
        public readonly string DatabaseName;
        public readonly bool IsSystemDatabase;
        public readonly MonitoredSqlServerStatus ServerStatus;
        public readonly Dictionary<string,TableStatus> TableStatus;
        public readonly Issue[] Issues;
        private int issueCount;

        public DatabaseStatus(MonitoredSqlServerStatus serverStatus, int databaseId, string databaseName, bool system)
        {
            DatabaseId = databaseId;
            DatabaseName = databaseName;
            IsSystemDatabase = system;
            ServerStatus = serverStatus;
            TableStatus = new Dictionary<string, TableStatus>();
            Issues = new Issue[3];
        }

        public int IssueCount
        {
            get { return issueCount;  }
            set
            {
                if (value <= Issues.Length)
                    issueCount = value;
                else
                    issueCount = Issues.Length;
            }
        }
    }

    public class TableStatus
    {
        public readonly int TableId;
        public readonly string TableName;
        public readonly bool IsSystemTable;
        public readonly DatabaseStatus DatabaseStatus;
        public readonly Issue[] Issues;
        private int issueCount;

        public TableStatus(DatabaseStatus databaseStatus, int tableId, string tableName, bool system)
        {
            TableId = tableId;
            TableName = tableName;
            IsSystemTable = system;
            DatabaseStatus = databaseStatus;
            Issues = new Issue[3];
        }

        public static string GetTableName(string schema, string table)
        {
            return schema + "." + table;
        }

        public int IssueCount
        {
            get { return issueCount; }
            set
            {
                if (value <= Issues.Length)
                    issueCount = value;
                else
                    issueCount = Issues.Length;
            }
        }
    }

    public class Issue : IComparable
    {
        public readonly Metric Metric;
        public readonly MonitoredState Severity;
        public readonly string Subject;
        public readonly int Rank;
        private DateTime occurenceTime = DateTime.MinValue;

        internal Issue(Metric metric, MonitoredState severity, int rank, string subject, DateTime occurenceTime)
        {
            Metric = metric;
            Severity = severity;
            Rank = rank;
            Subject = subject ?? "";
            OccurenceTime = occurenceTime;
        }

        public DateTime OccurenceTime
        {
            get { return occurenceTime; }
            set { occurenceTime = value; }
        }

        public int CompareTo(object obj)
        {
            if (!(obj is Issue))
                throw new ArgumentException("obj is null or an incompatable type");
            
            Issue other = (Issue)obj;

            int rc = -Severity.CompareTo(other.Severity);
            if (rc == 0)
            {
                rc = Rank.CompareTo(other.Rank);
                if (rc == 0)
                {
                    rc = -OccurenceTime.CompareTo(other.OccurenceTime);
                    if (rc == 0)
                    {
                        rc = Metric.CompareTo(other.Metric);
                        if (rc == 0)
                        {
                            if (Subject == null)
                            {
                                rc = (other.Subject == null) ? 0 : -1;
                            }
                            else if (other.Subject == null)
                                rc = 1;

                            rc = Subject.CompareTo(other.Subject);
                        }
                    }
                }
            }
            return rc;
        }

        public override bool Equals(object obj)
        {
            return this.CompareTo(obj) == 0;            
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        public static bool operator !=(Issue left, Issue right)
        {
            return left.CompareTo(right) != 0;    
        }

        public static bool operator ==(Issue left, Issue right)
        {
            return left.CompareTo(right) == 0;
        }

    }
}
