//------------------------------------------------------------------------------
// <copyright file="NotificationRule.cs" company="Idera, Inc.">
//     Copyright (c) Idera, Inc. All rights reserved.
// </copyright>
//------------------------------------------------------------------------------
namespace Idera.SQLdm.Common.Notification
{
    using System;
    using System.Collections.Generic;
    using System.Runtime.Serialization;
    using System.Threading;
    using System.Xml.Serialization;
    using Idera.SQLdm.Common.Events;
    using Idera.SQLdm.Common.Notification.Providers;
    using System.Runtime.Serialization.Formatters.Binary;
    using System.IO;
    using Idera.SQLdm.Common.Data;
    using Wintellect.PowerCollections;

    /// <summary>
    /// A notification rule mapping an event to a destination if it passes the specified condition.
    /// </summary>
    [Serializable]
    public class NotificationRule : ISerializable, ICloneable, IUnknownElementHandler
    {
        private static BBS.TracerX.Logger LOG = BBS.TracerX.Logger.GetLogger("NotificationRule");

      

        private Guid ruleId;
        private bool enabled;
        // boolean variable to check if and/or checkbox is checked 
        private bool isMetricsWithAndChecked;
        private bool isRankValueChecked;
        private string rankValue;
        private string cmbRankValue;
        private bool isMetricSeverityChecked;
        private bool isMetricSeverityAndUnchecked = true;
        private string metricSeverityValue;
        private string description;
        private List<int> metrics;
        private ServerNameRule serverNameComparison;
        private ServerTagRule serverTagComparison;
        private SnapshotTimeRule snapshotTimeComparison;
        private MetricStateRule stateComparison;
        private MetricStateChangeRule stateChangeComparison;
        private List<NotificationDestinationInfo> destinations;
        private ReaderWriterLock sync = new ReaderWriterLock();
        //START SQLDM 10.1 Barkha Khatri -- adding installation action for rule
        public delegate void InstallionAction(System.Data.SqlClient.SqlTransaction xa, Guid ruleId);

        [XmlIgnore]
        public bool IsEditCheckAlertRankResponse = false;
        [XmlIgnore]
        public bool IsAlertResponseDailogClick = false;
        [XmlIgnore]
        public bool IsMetricsSheverityEdit = false;
        [XmlIgnore]
        public bool IsMetricSheverityDialogOk = false;
        [XmlIgnore]
        public bool IsClickOnGrid = false;

        [XmlIgnore]
        public InstallionAction installationAction { get; set; }
        //END SQLDM 10.1 Barkha Khatri -- adding installation action for rule
        public NotificationRule()
        {

            // rule is enabled by default
            enabled = true;
            // all rules must have a state comparison so create and enable it
            StateComparison.Enabled = true;
            // set option to only notify on change
            StateChangeComparison.Enabled = true;

            // all rules must have a metric so create the metric list (it being not null infers it is enabled)
            //MetricIDs = new List<int>();
            metrics = null;
        }

        public NotificationRule(SerializationInfo info, StreamingContext context)
        {
            ruleId = (Guid)info.GetValue("ruleId", typeof (Guid));
            enabled = info.GetBoolean("enabled");
            description = info.GetString("description");
            rankValue = info.GetString("rankValue");
            metricSeverityValue = info.GetString("metricSeverityValue");
            metrics = info.GetValue("metrics", typeof (List<int>)) as List<int>;
          
            // setting the isMetricsWithAndChecked boolean variable from the info object which will be used to check and/or checkbox in AddConditions() in NotificationRuleDialog.cs Class
            try
            {
                isRankValueChecked = info.GetBoolean("isRankValueChecked");
                isMetricSeverityChecked = info.GetBoolean("isMetricSeverityChecked");
                isMetricsWithAndChecked = info.GetBoolean("isMetricsWithAndChecked");
               
            }
            catch (Exception)
            {
                isMetricsWithAndChecked = false;
                isRankValueChecked = false;
            }
            serverNameComparison = info.GetValue("serverNameComparison", typeof(ServerNameRule)) as ServerNameRule;
            snapshotTimeComparison = info.GetValue("snapshotTimeComparison", typeof(SnapshotTimeRule)) as SnapshotTimeRule;
            stateComparison = info.GetValue("stateComparison", typeof(MetricStateRule)) as MetricStateRule;
            stateChangeComparison = info.GetValue("stateChangeComparison", typeof(MetricStateChangeRule)) as MetricStateChangeRule;
            destinations = info.GetValue("destinations", typeof(List<NotificationDestinationInfo>)) as List<NotificationDestinationInfo>;
            try
            {
                serverTagComparison = info.GetValue("serverTagComparison", typeof (ServerTagRule)) as ServerTagRule;
            } catch (Exception)
            {
                serverTagComparison = null;
            }
        }

        [XmlAttribute()]
        public Guid Id
        {
            get { return ruleId; }
            set { ruleId = value; }
        }

        [XmlAttribute()]
        public bool Enabled
        {
            get { return enabled; }
            set { enabled = value; }
        }

        [XmlAttribute()]
        public string Description
        {
            get
            {
                if (description == null)
                    description = "";
                return description;
            }
            set { description = value; }
        }

        // this property is being left for conversion purposes.  notification rules
        // stored in the repository prior to 5.5 will contain a list of metric names
        // in the xml instead of a list of integers.  
        [XmlElement(ElementName = "Metrics"), XmlElement(typeof(Metric))]
        public List<Metric> Metrics
        {
            set
            {
                sync.AcquireWriterLock(0);
                if (value == null)
                    MetricIDs = null;
                else
                {
                    List<int> metricList = new List<int>();
                    foreach (Metric metric in value)
                    {
                        metricList.Add((int)metric);
                    }
                    MetricIDs = metricList;
                }
                sync.ReleaseWriterLock();
            }
        }

        public bool IsRankValueChecked
        {
            get { return isRankValueChecked; }
            set { isRankValueChecked = value; }
        }
     
        public string RankValue
        {
            get { return rankValue; }
            set { rankValue = value; }
        }
        public bool IsMetricSeverityChecked
        {
            get { return isMetricSeverityChecked; }
            set { isMetricSeverityChecked = value; }
        }
        public bool IsMetricSeverityAndUnchecked
        {
            get { return isMetricSeverityAndUnchecked; }
            set { isMetricSeverityAndUnchecked = value; }
        }
        public string MetricSeverityValue
        {
            get { return metricSeverityValue; }
            set { metricSeverityValue = value; }
        }
        // isMetricsWithAndChecked boolean variable get , set properties
        public bool IsMetricsWithAndChecked
        {
            get { return isMetricsWithAndChecked; }
            set { isMetricsWithAndChecked = value; }
        }

        [XmlIgnore]
        public string CmbRankValue
        {
            get { return cmbRankValue; }
            set { cmbRankValue = value; }
        }
        
        // used by the deserializer to handle backlevel xml that contains metric names
        // instead of the id of the metric.  We get called back when the serializer encounters
        // a node id does not handle.  We call AddMetric to convert the name to an id and add 
        // it to the collection.
        public void AddMetric(Metric metric)
        {
            sync.AcquireWriterLock(0);
            if (MetricIDs == null)
                MetricIDs = new List<int>();
            
            if (!MetricIDs.Contains((int)metric))
            {
                MetricIDs.Add((int)metric);
            }
            sync.ReleaseWriterLock();
        }

        [XmlElement(ElementName = "MetricIDs"), XmlElement(typeof(int))]
        public List<int> MetricIDs
        {
            get
            {
                sync.AcquireReaderLock(0);
                List<int> result = metrics;
                sync.ReleaseReaderLock();
                return result;
            }
            set
            {
                sync.AcquireWriterLock(0);
                metrics = value;
                sync.ReleaseWriterLock();
            }
        }


        [XmlElement(ElementName = "Destinations")]
        [XmlElement(typeof(SnmpDestination))]
        [XmlElement(typeof(SmtpDestination))]
        [XmlElement(typeof(EventLogDestination))]
        [XmlElement(typeof(EnableQMDestination))]
        [XmlElement(typeof(TaskDestination))]
        [XmlElement(typeof(ProgramDestination))]
        [XmlElement(typeof(JobDestination))]
        [XmlElement(typeof(SqlDestination))]
        [XmlElement(typeof(PulseDestination))]
        [XmlElement(typeof(EnablePADestination))]
        [XmlElement(typeof(EnableQWaitsDestination))]
        [XmlElement(typeof(PowerShellDestination))]
        [XmlElement(typeof(SCOMAlertDestination))]
        [XmlElement(typeof(SCOMEventDestination))]
        public List<NotificationDestinationInfo> Destinations
        {
            get
            {
                if (destinations == null)
                    destinations = new List<NotificationDestinationInfo>();
                return destinations;
            }
            set
            {
                destinations = value;
            }
        }

        [XmlElement(ElementName = "ServerNames")]
        public ServerNameRule ServerNameComparison
        {
            get
            {
                if (serverNameComparison == null)
                    serverNameComparison = new ServerNameRule();
                return serverNameComparison;
            }
            set
            {
                serverNameComparison = value;
            }
        }

        [XmlElement(ElementName = "ServerTags")]
        public ServerTagRule ServerTagComparison
        {
            get
            {
                if (serverTagComparison == null)
                    serverTagComparison = new ServerTagRule();
                return serverTagComparison;
            }
            set
            {
                serverTagComparison = value;
            }
        }


        [XmlElement(ElementName = "SnapshotTime")]
        public SnapshotTimeRule SnapshotTimeComparison
        {
            get
            {
                if (snapshotTimeComparison == null)
                    snapshotTimeComparison = new SnapshotTimeRule();
                return snapshotTimeComparison;
            }
            set
            {
                snapshotTimeComparison = value;
            }
        }

        [XmlElement(ElementName = "StateValue")]
        public MetricStateRule StateComparison
        {
            get
            {
                if (stateComparison == null)
                    stateComparison = new MetricStateRule();
                return stateComparison;
            }
            set
            {
                stateComparison = value;
            }
        }

        [XmlElement(ElementName = "StateChange")]
        public MetricStateChangeRule StateChangeComparison
        {
            get
            {
                if (stateChangeComparison == null)
                    stateChangeComparison = new MetricStateChangeRule();
                return stateChangeComparison;
            }
            set
            {
                stateChangeComparison = value;
            }
        }

        public bool AllMetrics
        {
            get
            {
                if (metrics == null)
                    return true;
                else
                    return false;
            }
            set
            {
                if (value == true)
                    metrics = null;
                else
                    if (metrics == null)
                        MetricIDs = new List<int>();
            }
        }

        public void Validate(bool checkDescription)
        {
            if (checkDescription)
            {
                if (String.IsNullOrEmpty(this.Description))
                    throw new ActionRuleValidationException(true, false, false, "Input Required", "Please enter a description for your rule.");
            }

            if (!AllMetrics && MetricIDs.Count < 1)
            {
                throw new ActionRuleValidationException(false, false, true, "Metric Required", "Please click the red metric link and select one or more metrics.");
            }

            if (serverNameComparison != null && serverNameComparison.Enabled)
            {
                if (serverNameComparison.ServerNames.Count == 0)
                    throw new ActionRuleValidationException(false, false, true, "Server Name Required", "Please click red 'specified list' link and select one or more server names");
            }

            if (serverTagComparison != null && serverTagComparison.Enabled)
            {
                if (serverTagComparison.ServerTagIds.Count == 0)
                    throw new ActionRuleValidationException(false, false, true, "Server Tag Required", "Please click the red 'specified list' link and select one or more tag names");
            }

            // if StateComparison is missing or disabled - create/enable it
            if (stateComparison == null || !stateComparison.Enabled)
            {
                StateComparison.Enabled = true;
            }
            
            // if StateComparison is ever without a value selected - select some defaults
            if (!stateComparison.IsCritical && !stateComparison.IsWarning && !stateComparison.IsOK && !stateComparison.IsInformational)
                stateComparison.IsWarning = stateComparison.IsCritical = true;

//            if (stateChangeComparison != null && stateChangeComparison.Enabled)
//            {
//                throw new ApplicationException("The state change component of the rule is not valid.");
//            }

//            if (stateChangeComparison == null || !stateChangeComparison.Enabled)
//            {
//                if (stateComparison == null || !stateChangeComparison.Enabled)
//                    throw new ApplicationException("You must specify either a state equals and/or a state change component to the rule.");
//            }

            if (snapshotTimeComparison != null && snapshotTimeComparison.Enabled)
            {
                if (!snapshotTimeComparison.IsValid())
                    throw new ActionRuleValidationException(false, false, true, "Time Rule Error", "Please correct the time of day portion of your rule.");
            }

            if (destinations == null || destinations.Count == 0)
            {
                throw new ActionRuleValidationException(false, true, false, "Action Required", "Use the Add button to select one or more actions to run when this rule matches an alert.");
            } else
            {
                bool actionEnabled = false;
                foreach (NotificationDestinationInfo destination in destinations)
                {
                    if (destination.Enabled)
                    {
                        actionEnabled = true;
                        destination.Validate();
                    }
                }
                if (!actionEnabled)
                    throw new ActionRuleValidationException(false, true, false, "One Enabled Action Required", "Please enable one of the actions for your rule.");
            }

        }

        public bool Matches(int metricID)
        {
            bool result = true;
            if (AllMetrics)
                result = true;
            else
            {
                sync.AcquireReaderLock(0);
                List<int> list = MetricIDs;
                result = list.Contains(metricID);
                sync.ReleaseReaderLock();
            }
            return result;
        }

        public bool Matches(string serverInstanceName)
        {
            if (ServerNameComparison.Enabled && ServerNameComparison.ServerNames.Count > 0)
            {
                if (serverInstanceName == null)
                    return false;
                return ServerNameComparison.ServerNames.Contains(serverInstanceName);
            }
            return !(ServerTagComparison.Enabled && ServerTagComparison.ServerTagIds.Count > 0);
        }

        public bool Matches(ICollection<int> serverTags)
        {
            if (ServerTagComparison.Enabled && ServerTagComparison.ServerTagIds.Count > 0)
            {
                if (serverTags == null || serverTags.Count == 0)
                    return false;

                foreach (int tagId in ServerTagComparison.ServerTagIds)
                {
                    if (serverTags.Contains(tagId))
                    {
                        return true;        
                    }
                }
                return false;
            }
            return !(ServerNameComparison.Enabled && ServerNameComparison.ServerNames.Count > 0);
        }

        public bool Matches(DateTime occuranceDateTime)
        {
            SnapshotTimeRule str = SnapshotTimeComparison;
            if (str.Enabled)
            {
                // first test day of week
                if (str.AnyDays)
                {
                    bool dowtest = false;
                    switch (occuranceDateTime.DayOfWeek)
                    {
                        case DayOfWeek.Sunday:
                            dowtest = str.Sunday;
                            break;
                        case DayOfWeek.Monday:
                            dowtest = str.Monday;
                            break;
                        case DayOfWeek.Tuesday:
                            dowtest = str.Tuesday;
                            break;
                        case DayOfWeek.Wednesday:
                            dowtest = str.Wednesday;
                            break;
                        case DayOfWeek.Thursday:
                            dowtest = str.Thursday;
                            break;
                        case DayOfWeek.Friday:
                            dowtest = str.Friday;
                            break;
                        case DayOfWeek.Saturday:
                            dowtest = str.Saturday;
                            break;
                    }
                    if (!dowtest)
                        return false;
                }
                TimeSpan occuranceTime =
                    new TimeSpan(occuranceDateTime.Hour, occuranceDateTime.Minute, 0);

                if (str.IsStartTimeSet)
                {
                    if (!str.IsEndTimeSet)
                    {
                        if (occuranceTime < str.StartTime)
                            return false;
                    }
                    else
                    {
                        if (str.EndTime > str.StartTime)
                        {
                            if (!(occuranceTime >= str.StartTime && occuranceTime <= str.EndTime))
                                return false;
                        }
                        else
                        {
                            if (!(occuranceTime >= str.StartTime || occuranceTime <= str.EndTime))
                                return false;
                        }
                    }
                } else
                if (str.IsEndTimeSet)
                {
                    if (occuranceTime > str.EndTime)
                        return false;
                }
            }
            return true;
        }

        public bool Matches(StateChangeEvent stateChangeEvent)
        {
            if (StateChangeComparison.Enabled)
            {
                return Matches(stateChangeEvent.MonitoredState);
//                if (!StateChangeComparison.NewState.Contains(stateChangeEvent.MonitoredState))
//                    return false;
//                if (!StateChangeComparison.PreviousState.Contains(stateChangeEvent.PreviousState))
//                    return false;
            }
            return true;
        }



        public bool Matches(ThresholdViolationEvent thresholdViolationEvent)
        {
            if (StateComparison.Enabled)
            {
                return Matches(thresholdViolationEvent.MonitoredState);
            }
            return true;
        }

        public bool Matches(MonitoredState state)
        {
            switch (state)
            {
                case MonitoredState.OK:
                    return StateComparison.IsOK;
                case MonitoredState.Warning:
                    return StateComparison.IsWarning;
                case MonitoredState.Critical:
                    return StateComparison.IsCritical;
                case MonitoredState.Informational:
                    return StateComparison.IsInformational;
            }
            return true;
        }

        public override string ToString()
        {
            return (String.IsNullOrEmpty(description) ? Id.ToString() : description);
        }

        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            string ConcatenateRankValue = cmbRankValue + " " + rankValue;
            info.AddValue("ruleId", ruleId);
            info.AddValue("enabled", enabled);
            info.AddValue("description",description);
            info.AddValue("rankValue", ConcatenateRankValue);
            info.AddValue("metricSeverityValue", metricSeverityValue);
            info.AddValue("metrics", metrics);
            // setting the isMetricsWithAndChecked value in the SerializationInfo info object
            try
            {
                info.AddValue("isRankValueChecked", isRankValueChecked);
                info.AddValue("isMetricSeverityChecked", isMetricSeverityChecked);
                info.AddValue("isMetricsWithAndChecked", isMetricsWithAndChecked);
              
            }
            catch (Exception e)
            {
                info.AddValue("isRankValueChecked", false);
                info.AddValue("isMetricSeverityChecked", false);
                info.AddValue("isMetricsWithAndChecked", false);
            }
            info.AddValue("serverNameComparison",serverNameComparison);
            info.AddValue("serverTagComparison", serverTagComparison);
            info.AddValue("snapshotTimeComparison",snapshotTimeComparison);
            info.AddValue("stateComparison", stateComparison);
            info.AddValue("stateChangeComparison",stateChangeComparison);
            info.AddValue("destinations", destinations);
        }

        public object Clone()
        {
            BinaryFormatter formatter = new BinaryFormatter();
            using (MemoryStream stream = new MemoryStream())
            {
                formatter.Serialize(stream, this);
                stream.Seek(0, SeekOrigin.Begin);
                return formatter.Deserialize(stream);
            }
        }

        #region IUnknownElementHandler Members

        public void HandleUnknownElement(System.Xml.XmlElement element)
        {
            try
            {
                switch (element.Name)
                {
                    case "Metric":
                    case "Metrics":
                        string text = element.InnerText;
                        if (!String.IsNullOrEmpty(text))
                        {
                            Metric metric = (Metric) Enum.Parse(typeof (Metric), text);
                            AddMetric(metric);
                        }
                        break;
                    default:
                        LOG.ErrorFormat("Notification rule with id={0} has an unrecognized xml element: {1}", Id, element.OuterXml);
                        break;
                }
            }
            catch (Exception e)
            {
                LOG.ErrorFormat("Exception deserializing notification rule: ({0}) {1}", element.OuterXml, e);
            }
        }

        #endregion

    }

    public sealed class ActionRuleValidationException : Exception
    {
        private readonly bool descriptionFU;
        private readonly bool actionFU;
        private readonly bool ruleFU;
        private readonly string message2;

        public ActionRuleValidationException(bool descriptionMessedUp, bool actionMessedUp, bool ruleMessedUp, string message, string message2) : base(message)
        {
            descriptionFU = descriptionMessedUp;
            actionFU = actionMessedUp;
            ruleFU = ruleMessedUp;
            this.message2 = message2;
        }

        public bool DescriptionMessage
        {
            get { return descriptionFU; }
        }
        public bool ActionMessage
        {
            get { return actionFU; }
        }
        public bool RuleMessage
        {
            get { return ruleFU; }
        }
        public string Message2
        {
            get { return message2; }
        }
    }

}
