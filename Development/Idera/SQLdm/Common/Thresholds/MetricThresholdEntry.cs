//------------------------------------------------------------------------------
// <copyright file="ServerThresholdEntry.cs" company="Idera, Inc.">
//     Copyright (c) Idera, Inc. All rights reserved.
// </copyright>
//------------------------------------------------------------------------------

using Idera.SQLdm.Common.Auditing;
using Idera.SQLdm.Common.Attributes;

namespace Idera.SQLdm.Common.Thresholds
{
    using System;
    using System.Runtime.Serialization;
    using Configuration;
    using Idera.SQLdm.Common.Events;
    using Idera.SQLdm.Common.Objects;
    using Snapshots;

    /// <summary>
    /// Contains the warning and critical thresholds for a specific server and event.
    /// </summary>
    [Serializable]
    public class MetricThresholdEntry : ISerializable, IAuditable 
    {
        #region fields

        private int monitoredServerID;
        private int metricID;
        private string metricInstanceName = "";
        private bool enabled;
        private Threshold infoThreshold;
        private Threshold warningThreshold;
        private Threshold criticalThreshold;
        private Threshold baselineInfoThreshold;
        private Threshold baselineWarningThreshold;
        private Threshold baselineCriticalThreshold;
        private object data;
        private bool thresholdEnabled;

        //10.0 srishti purohit -- for baseline alert modifications
        private bool isBaselineEnabled = false;
        private ThresholdState state = ThresholdState.Unchanged;

        public static string DEFAULT_THRESHOLD_NAME = "< Default Threshold >";

        // transient
        private MonitoredSqlServer monitoredServer;

        #endregion

        #region constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="T:MetricThresholdEntry"/> class.
        /// </summary>
        public MetricThresholdEntry()
        {
            enabled = true;
            thresholdEnabled = true;
            isBaselineEnabled = false;
        }

        protected MetricThresholdEntry(SerializationInfo info, StreamingContext context)
        {
            monitoredServerID = info.GetInt32("monitoredServerID");
            metricID = info.GetInt32("metricID");
            enabled = info.GetBoolean("enabled");
            metricInstanceName = info.GetString("metricInstanceName");
            infoThreshold = (Threshold)info.GetValue("infoThreshold", typeof(Threshold));
            warningThreshold = (Threshold)info.GetValue("warningThreshold", typeof(Threshold));
            criticalThreshold = (Threshold)info.GetValue("criticalThreshold", typeof(Threshold));
            data = info.GetValue("data", typeof(object));
            thresholdEnabled = info.GetBoolean("thresholdEnabled");
            //10.0 srishti purohit -- for baseline alert modifications
            isBaselineEnabled = info.GetBoolean("isBaselineEnabled");
            baselineInfoThreshold = (Threshold)info.GetValue("baselineInfoThreshold", typeof(Threshold));
            baselineWarningThreshold = (Threshold)info.GetValue("baselineWarningThreshold", typeof(Threshold));
            baselineCriticalThreshold = (Threshold)info.GetValue("baselineCriticalThreshold", typeof(Threshold));
            state = (ThresholdState) info.GetValue("state", typeof (ThresholdState));
        }

        //public MetricThresholdEntry(MonitoredSqlServer monitoredServer, int metricID, Threshold warningThreshold, Threshold criticalThreshold) : this()
        //{
        //    MonitoredServer = monitoredServer;
        //    MetricID = metricID;
        //    WarningThreshold = warningThreshold;
        //    CriticalThreshold = criticalThreshold;
        //}

        //public MetricThresholdEntry(MonitoredSqlServer monitoredServer, Metric metric, Threshold warningThreshold,Threshold criticalThreshold)
        //    : this(monitoredServer, (int) metric, warningThreshold, criticalThreshold)
        //{
        //}

        //public MetricThresholdEntry(int monitoredServerId, int metricID, Threshold warningThreshold, Threshold criticalThreshold)
        //    : this()
        //{
        //    MonitoredServerID = monitoredServerId;
        //    MetricID = metricID;
        //    WarningThreshold = warningThreshold;
        //    CriticalThreshold = criticalThreshold;
        //}

        //public MetricThresholdEntry(int monitoredServerId, Metric metric, Threshold warningThreshold, Threshold criticalThreshold)
        //    : this(monitoredServerId, (int) metric, warningThreshold, criticalThreshold)
        //{
        //}

// Once this bastard gets full implemented I should only need these constructors below and the previous 4 contructors can be removed
        // no usage found
        public MetricThresholdEntry(MonitoredSqlServer monitoredServer, int metricID, string metricInstanceName, Threshold warningThreshold, Threshold criticalThreshold, Threshold infoThreshold)
            : this(monitoredServer, metricID, warningThreshold, criticalThreshold, infoThreshold)
        {
            MetricInstanceName = metricInstanceName;
        }

        // used by the constructor above, and the two below
        public MetricThresholdEntry(MonitoredSqlServer monitoredServer, int metricID, Threshold warningThreshold, Threshold criticalThreshold, Threshold infoThreshold)
            : this()
        {
            MonitoredServer = monitoredServer;
            MetricID = metricID;
            InfoThreshold = infoThreshold;
            WarningThreshold = warningThreshold;
            CriticalThreshold = criticalThreshold;
        }

        // No usage found
        public MetricThresholdEntry(MonitoredSqlServer monitoredServer, Metric metric, string metricInstanceName, Threshold warningThreshold, Threshold criticalThreshold, Threshold infoThreshold)
            : this(monitoredServer, (int)metric, warningThreshold, criticalThreshold, infoThreshold)
        {
            MetricInstanceName = metricInstanceName;
        }

        // Used in ImportableServer.cs, but the dialog that uses that class doesn't seem to be used anywhere
        public MetricThresholdEntry(MonitoredSqlServer monitoredServer, Metric metric, Threshold warningThreshold, Threshold criticalThreshold, Threshold infoThreshold)
            : this(monitoredServer, (int)metric, warningThreshold, criticalThreshold, infoThreshold)
        {
        }

        // used in DesktopClient.RepositoryHelper.GetMetricThresholds(), Common.AlertConfiguration.cs (AlertConfigurationItem constructor)
        //      and the two constructors below.
        public MetricThresholdEntry(int monitoredServerId, int metricID, Threshold warningThreshold, Threshold criticalThreshold, Threshold infoThreshold)
            : this()
        {
            MonitoredServerID = monitoredServerId;
            MetricID = metricID;
            InfoThreshold = infoThreshold;
            WarningThreshold = warningThreshold;
            CriticalThreshold = criticalThreshold;
        }
        //10.0 Srishti purohit // For baseline alert
        //Making copy of State grid to give baseline alert functioanlity
        // used in DesktopClient.RepositoryHelper.GetMetricThresholds(), Common.AlertConfiguration.cs (AlertConfigurationItem constructor)
        //      and the two constructors below.
        public MetricThresholdEntry(int monitoredServerId, int metricID, Threshold warningThreshold, Threshold criticalThreshold, Threshold infoThreshold, Threshold warningThresholdBaseline= null, Threshold criticalThresholdBaseline= null, Threshold infoThresholdBaseline= null)
            : this()
        {
            MonitoredServerID = monitoredServerId;
            MetricID = metricID;
            InfoThreshold = infoThreshold;
            WarningThreshold = warningThreshold;
            CriticalThreshold = criticalThreshold;
            BaselineInfoThreshold = infoThresholdBaseline;
            BaselineWarningThreshold = warningThresholdBaseline;
            BaselineCriticalThreshold = criticalThresholdBaseline;
        }

        // used in ManagementService.RespoitoryHelper.GetMetricThresholds()
        public MetricThresholdEntry(int monitoredServerId, int metric, string metricInstanceName, Threshold warningThreshold, Threshold criticalThreshold, Threshold infoThreshold, Threshold warningThresholdBaseline= null, Threshold criticalThresholdBaseline= null, Threshold infoThresholdBaseline= null)
            : this(monitoredServerId, metric, warningThreshold, criticalThreshold, infoThreshold, warningThresholdBaseline, criticalThresholdBaseline, infoThresholdBaseline)
        {
            MetricInstanceName = metricInstanceName;
        }

        // used in NotificationManager.cs
        public MetricThresholdEntry(int monitoredServerId, Metric metric, Threshold warningThreshold, Threshold criticalThreshold, Threshold infoThreshold)
            : this(monitoredServerId, (int)metric, warningThreshold, criticalThreshold, infoThreshold)
        {
        }

        #endregion

        #region properties

        /// <summary>
        /// Gets or sets the id of the monitored server.  Setting MonitoredServer also
        /// sets this value.  
        /// </summary>
        /// <value>The id.</value>
        [AuditableAttribute(false)]
        public int MonitoredServerID
        {
            get
            {   // prefer the id in the object over just the id stored in this object
                if (monitoredServer != null)
                    return monitoredServer.Id;

                return monitoredServerID;
            }
            set { monitoredServerID = value; }
        }

        /// <summary>
        /// Gets or sets the monitored server and monitored server id.  It is not required 
        /// to set this value if you are going to set the monitored server id.  Only the id 
        /// is serialized, not the actual monitored sql server object.
        /// </summary>
        /// <value>The monitored server.</value>
        public MonitoredSqlServer MonitoredServer
        {
            get { return monitoredServer; }
            set
            {
                if (value == null)
                    throw new ArgumentNullException("monitoredServer");
                monitoredServer = value;
                monitoredServerID = monitoredServer.Id;
            }
        }

        /// <summary>
        /// Gets or sets the metric.
        /// </summary>
        /// <value>The metric.</value>
        public int MetricID
        {
            get { return metricID; }
            set { metricID = value; }
        }

        /// <summary>
        /// Gets or sets the metric instance name
        /// </summary>
        /// <value>The metric.</value>
        public string MetricInstanceName
        {
            get
            {
                return metricInstanceName == string.Empty ? DEFAULT_THRESHOLD_NAME : metricInstanceName;
            }
            set { metricInstanceName = value; }
        }

        public bool IsDefaultThreshold
        {
            get { return MetricInstanceName == DEFAULT_THRESHOLD_NAME; }
        }

        public ThresholdState State
        {
            get { return state; }
            set { state = value; }
        }

        public InstanceType MetricInstanceType
        {
            get
            {
                switch (MetricID)
                {
                    //START: SQLdm 9.1 (Abhishek Joshi) -Filegroup improvements --classifying it as database type metric
                    case (int)Metric.FilegroupSpaceFullPct:
                    case (int)Metric.FilegroupSpaceFullSize:
                    //END: SQLdm 9.1 (Abhishek Joshi) -Filegroup improvements --classifying it as database type metric
                    // SQLdm 10.0 (Vandana Gogna) - Database backup alerts
                    case (int)Metric.DatabaseLastBackupDate:
                    case (int)Metric.DatabaseSizePct:
                    case (int)Metric.DatabaseSizeMb:
                    case (int)Metric.DatabaseStatus:
                    case (int)Metric.TransLogSizeMb:
                    case (int)Metric.TransLogSize:
                    case (int)Metric.DatabaseFileSize:
                    case (int)Metric.SSBackupOperation:
                    case (int)Metric.SSDefragOperation:
                    case (int)Metric.SSRestoreOperation:
                    case (int)Metric.PreferredNodeUnavailability: //SQLdm 8.6 (Ankit Srivastava) --Preferred Node feature -Added InstanceType for newly added metric
                        return InstanceType.Database;
                        break;
                    case (int)Metric.DiskReadsPerSecond:
                    case (int)Metric.DiskTransfersPerSecond:
                    case (int)Metric.DiskWritesPerSecond:
                    case (int)Metric.AverageDiskMillisecondsPerRead:
                    case (int)Metric.AverageDiskMillisecondsPerTransfer:
                    case (int)Metric.AverageDiskMillisecondsPerWrite:
                    case (int)Metric.OSDiskFull:
                    case (int)Metric.OsDiskFreeMb:
                        return InstanceType.Disk;
                        break;
                    case (int)Metric.JobCompletion:
                    case (int)Metric.BombedJobs:
                    case (int)Metric.LongJobs:
                    case (int)Metric.LongJobsMinutes:
                        return InstanceType.Job;
                        break;
                    default:
                        return InstanceType.Unknown;
                        break;
                }
            }
        }

        /// <summary>
        /// Gets or sets the informational threshold.
        /// </summary>
        /// <value>The warning threshold.</value>
        public Threshold InfoThreshold
        {
            get { return infoThreshold; }
            set
            {
                infoThreshold = value;
            }
        }

        /// <summary>
        /// Gets or sets the warning threshold.
        /// </summary>
        /// <value>The warning threshold.</value>
        public Threshold WarningThreshold
        {
            get { return warningThreshold; }
            set
            {
                warningThreshold = value;
            }
        }

        /// <summary>
        /// Gets or sets the critical threshold.
        /// </summary>
        /// <value>The critical threshold.</value>
        public Threshold CriticalThreshold
        {
            get { return criticalThreshold; }
            set
            {
                criticalThreshold = value;
            }
        }

        /// <summary>
        /// Gets/Sets the status of the metric Instance
        /// </summary>
        public bool IsEnabled
        {
            // If metric does not support multi-instance thresholds this value should equal IsThresholdEnabled
            get { return enabled; }
            set { enabled = value; }
        }

        /// <summary>
        /// Gets/Sets the status of the metric itself
        /// </summary>
        public bool IsThresholdEnabled
        {
            // If metric does not support multi-instance thresholds this value should equal IsEnabled
            get { return thresholdEnabled; }
            set { thresholdEnabled = value; }
        }

        //START - 10.0 srishti purohit -- for baseline alert modifications

        public bool IsBaselineEnabled
        {
            get { return isBaselineEnabled; }
            set { isBaselineEnabled = value; }
        }
        /// <summary>
        /// Gets or sets the informational threshold.
        /// </summary>
        /// <value>The warning threshold.</value>
        public Threshold BaselineInfoThreshold
        {
            get { return baselineInfoThreshold; }
            set
            {
                baselineInfoThreshold = value;
            }
        }

        /// <summary>
        /// Gets or sets the warning threshold.
        /// </summary>
        /// <value>The warning threshold.</value>
        public Threshold BaselineWarningThreshold
        {
            get { return baselineWarningThreshold; }
            set
            {
                baselineWarningThreshold = value;
            }
        }

        /// <summary>
        /// Gets or sets the critical threshold.
        /// </summary>
        /// <value>The critical threshold.</value>
        public Threshold BaselineCriticalThreshold
        {
            get { return baselineCriticalThreshold; }
            set
            {
                baselineCriticalThreshold = value;
            }
        }
        //END -10.0 srishti purohit -- for baseline alert modifications

        /// <summary>
        /// Additional data used for processing the alert.  It is not used in the test to determine
        /// if the violation testing code and is more for configuration of the probes.
        /// </summary>
        [AuditableAttribute(false)]
        public object Data
        {
            get
            {
                return data;
            }
            set { this.data = value; }
        }

        public SnoozeInfo SnoozeInfo
        {
            get
            {
                AdvancedAlertConfigurationSettings settings = Data as AdvancedAlertConfigurationSettings;
                return settings != null ? settings.SnoozeInfo : null;
            }
        }

        public bool IsAlertSnoozed(DateTime timestampUTC)
        {
            SnoozeInfo snoozeInfo = SnoozeInfo;
            return snoozeInfo != null && snoozeInfo.IsSnoozed(timestampUTC);
        }

        #endregion

        #region events

        #endregion

        #region methods

        #endregion

        #region interface implementations

        #endregion

        #region nested types

        #endregion


        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue("monitoredServerID", monitoredServerID);
            info.AddValue("metricID", metricID);
            info.AddValue("enabled", enabled);
            info.AddValue("infoThreshold", infoThreshold);
            info.AddValue("warningThreshold", warningThreshold);
            info.AddValue("criticalThreshold", criticalThreshold);
            info.AddValue("data", data);
            info.AddValue("metricInstanceName", metricInstanceName);
            info.AddValue("thresholdEnabled", thresholdEnabled);
            //10.0 srishti purohit -- for baseline alert modifications
            info.AddValue("isBaselineEnabled", isBaselineEnabled);
            info.AddValue("baselineInfoThreshold", baselineInfoThreshold);
            info.AddValue("baselineWarningThreshold", baselineWarningThreshold);
            info.AddValue("baselineCriticalThreshold", baselineCriticalThreshold);
            info.AddValue("state", state);
        }

        /// <summary>
        /// Get an Auditable Entity from Tag
        /// </summary>
        /// <returns> Returns an Auditable Entity</returns>
        public AuditableEntity GetAuditableEntity()
        {
            AuditableEntity auditable = new AuditableEntity();
            auditable.Name = GetConvertMetricIDToString(this.MetricID);
            return auditable;
        }


        /// <summary>
        /// Get an Auditable Entity from Tag
        /// </summary>
        /// <param name="oldValue">IAuditable</param>
        /// <returns> Returns an Auditable Entity</returns>
        public AuditableEntity GetAuditableEntity(IAuditable oldValue)
        {
            AuditableEntity auditable = GetAuditableEntity();

            PropertiesComparer comparer = new PropertiesComparer();
            var propertiesChanged = comparer.GetNewProperties(oldValue, this);
            
            foreach (var property in propertiesChanged)
            {
                auditable.AddMetadataProperty(property.Name, property.Value);
            }

            return auditable;
        }


        /// <summary>
        /// Gets the name of the alert from id
        /// </summary>
        /// <param name="metricId">int</param>
        /// <returns>string </returns>
        private string GetConvertMetricIDToString(int metricId)
        {
            try
            {
                Metric metric;
                if (Enum.IsDefined(typeof(Metric), metricId))
                {
                    metric =  (Metric)Enum.ToObject(typeof(Metric), metricId);
                    return metric.ToString();
                }

            }
            catch(Exception e)
            {
                //TODO need to log here
                return String.Empty;
            }

            return String.Empty;
        }


    }
    public enum ThresholdState
    {
        Unchanged,
        Added,
        Changed,
        Deleted
    }

}
