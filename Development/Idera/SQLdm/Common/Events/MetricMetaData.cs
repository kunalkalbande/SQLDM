//------------------------------------------------------------------------------
// <copyright file="MetricMetaData.cs" company="Idera, Inc.">
//     Copyright (c) Idera, Inc. All rights reserved.
// </copyright>
//------------------------------------------------------------------------------
namespace Idera.SQLdm.Common.Events
{
    using System;
    using System.Collections.Generic;
    using System.Collections.Specialized;
    using System.ComponentModel;
    using Idera.SQLdm.Common.Objects;
    using Idera.SQLdm.Common.Properties;
    using Idera.SQLdm.Common.Snapshots;
    using Idera.SQLdm.Common.Messages;

    public class MetricMetaData
    {
        protected static BBS.TracerX.Logger LOG = BBS.TracerX.Logger.GetLogger("MetricMetaData");

        private int metricID;
//        private Messages.Category logCategory;
        private Messages.Status   logEventId;
        protected bool processNotifications;

        private Type valueType;
        private MetricClass metricType;
        private bool configurable;
        private bool multiValued;
        private int minValue;
        private int maxValue;
        private object defaultWarningValue;
        private object defaultCriticalValue;
 //       private string mask;
        private ThresholdOptions options;

        private string[] resourceKeys;

        public MetricMetaData()
        {
//            logCategory = Idera.SQLdm.Common.Messages.Category.General;
            processNotifications = true;
            valueType = typeof(object);
            metricType = MetricClass.ServerThreshold;
            configurable = true;
            maxValue = 100;
            options = ThresholdOptions.NumericValue | ThresholdOptions.OKDisabled;
        }

        public MetricMetaData(Metric metric, Messages.Status status, MetricClass metricType, string bodyKey, string headerKey, string toDoKey) 
            : this()
        {
            this.metricID = (int)metric;
            this.logEventId = status;
            this[MessageType.Body]   = bodyKey;
            this[MessageType.Header] = headerKey;
            this[MessageType.ToDo]   = toDoKey;
            this.metricType = metricType;
        }

        public MetricMetaData(Metric metric, MetricClass metricType, string bodyKey, string headerKey, string toDoKey)
            : this()
        {
            this.metricID = (int)metric;
            this[MessageType.Body] = bodyKey;
            this[MessageType.Header] = headerKey;
            this[MessageType.ToDo] = toDoKey;
            this.metricType = metricType;
        }

        public string this[MessageType type]
        {
            get
            {
                int i = (int) type;
                if (i < resourceKeys.Length)
                    return resourceKeys[i];
                return "Unknown Message Type";
            }
            set
            {
                if (resourceKeys == null)
                {
                    int n = Enum.GetValues(type.GetType()).Length;
                    resourceKeys = new string[n];
                }

                int i = (int)type;
                resourceKeys[i] = value;
            }
        }

        public MetricClass MetricClass
        {
            get { return metricType; }
            set { metricType = value; }
        }

        public Type ValueType
        {
            get { return valueType;  }
            set { valueType = value; }
        }

        public ThresholdOptions Options
        {
            get { return options;  }
            set { options = value; }
        }

        public int MinValue
        {
            get { return minValue; }
            set { minValue = value; }
        }
        public int MaxValue
        {
            get { return maxValue; }
            set { maxValue = value; }
        }

        /// <summary>
        /// Visual upper bound is used by the desktop client alert config dialog to
        /// set the upper bound of the gauge.  If the metric is set to calculate this
        /// value then it will try to return a value, based on the current critical 
        /// threshold value, that will cause the red portion to be at least 20% of 
        /// the gauge.
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public virtual int GetVisualUpperBound(int value)
        {
            if ((Options & ThresholdOptions.CalculateMaxValue) != ThresholdOptions.CalculateMaxValue)
                return MaxValue;

            int ub = (int)(1.2d * value);

            if (ub < 20)
                ub = MinValue + 20;

            return ub > MaxValue ? MaxValue : ub;
        }

        public bool MultiValued
        {
            get { return multiValued; }
            set { multiValued = value; }
        }
        public bool Configurable
        {
            get { return configurable; }
            set { configurable = value; }
        }
        public object DefaultWarningValue
        {
            get { return defaultWarningValue;  }
            set { defaultWarningValue = value; }
        }
        public object DefaultCriticalValue
        {
            get { return defaultCriticalValue; }
            set { defaultCriticalValue = value; }
        }
        public bool HasAdditionalData
        {
            get { return (Options & ThresholdOptions.AdditionalData) == ThresholdOptions.AdditionalData; }
        }

        public bool ProcessNotifications
        {
            get { return processNotifications;  }
        }

        public int MetricID
        {
            get
            {
                return metricID;
            }
        }

        public Metric Metric
        {
            get
            {
                try
                {
                    return (Metric) Enum.ToObject(typeof (Metric), metricID);   
                } 
                catch
                {
                    return Metric.Custom;    
                }
            }
        }

        public string GetStringResource(string key, string defaultValue)
        {
            try
            {
                return Resources.ResourceManager.GetString(key);
            }
            catch (Exception)
            {
                /* */
            }
            return defaultValue;
        }

        /// <summary>
        /// Most metrics only have a single status that is used when the value gets 
        /// out of whack.  Others may need to look at some state values to determine
        /// the proper Status to return.  If this is necessary then the implementation
        /// must override this method. 
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public virtual Status GetStatus(object value)
        {
            return this.logEventId;   
        }

//        protected object[] concat(string serverName, string databaseName, string tableName, MonitoredState state, DateTime timestamp, params object[] values)
//        {
//            object[] result = new object[5 + values.Length];
//            result[0] = serverName;
//            result[1] = databaseName;
//            result[2] = tableName;
//            result[3] = state;
//            result[4] = timestamp;
//            for (int i = 0; i < values.Length; i++)
//                result[5 + i] = values[i];
//            return result;
//        }

        protected string GetMonitoredObjectName(BaseEvent baseEvent, string defaultName)
        {
            MonitoredObjectName mo = baseEvent.MonitoredObject;
            if (mo.IsServer)
                return mo.ServerName.Trim();
            if (mo.IsDatabase)
                return mo.DatabaseName;
            if (mo.IsTable)
                return mo.TableName;
            return defaultName;
        }

        /// <summary>
        /// Returns [ServerName, Sampled Value, MonitoredObject Name]
        /// </summary>
        /// <param name="baseEvent"></param>
        /// <param name="snapshot"></param>
        /// <returns></returns>
        public virtual object[] GetMessageData(Snapshot snapshot, BaseEvent baseEvent)
        {
            MonitoredObjectName mon = baseEvent.MonitoredObject;
            string serverName = mon != null
                                    ? mon.ServerName
                                    : snapshot != null
                                    ? snapshot.ServerName
                                    : ""; 

            return new object[]
                {
                    serverName,
                    baseEvent.Value,
                    GetMonitoredObjectName(baseEvent, serverName)
                };
        }

        public string FormatMessage(Snapshot snapshot, BaseEvent baseEvent, string resourceKey)
        {
            return "Deprecated message formatter used.  This is a bug.";
//            return String.Format(GetStringResource(resourceKey, baseEvent.Metric.ToString()), GetMessageData(snapshot, baseEvent));
        }

        public virtual string FormatMessage(Snapshot snapshot, BaseEvent baseEvent, MessageType messageType)
        {
            return FormatMessage(snapshot, baseEvent, this[messageType]);
        }

     }

    public class ServiceStateMetricMetaData : MetricMetaData
    {
        protected SortedList<int, Status> statusMap = new SortedList<int, Status>();
        protected Status defaultStatus;

        public ServiceStateMetricMetaData(Metric metric, MetricClass metricType)
            : base(metric, metricType, "_Body", "_Header", "_Todo")
        {
            MultiValued = true;
            ValueType = typeof (ServiceState);
            Options = ThresholdOptions.OKDisabled | ThresholdOptions.ContainedValueList;
        }

        

        public override object[] GetMessageData(Snapshot snapshot, BaseEvent baseEvent)
        {
            MonitoredObjectName mon = baseEvent.MonitoredObject;
            string serverName = mon != null
                                    ? mon.ServerName
                                    : snapshot != null
                                    ? snapshot.ServerName
                                    : "";

            Exception e = snapshot == null ? null : snapshot.Error;
            string message = e != null ? e.Message : "";

            return new object[]
                {
                    serverName,
                    baseEvent.Value,
                    GetMonitoredObjectName(baseEvent, serverName),
                    message
                };
        }

        public SortedList<int,Status> GetValueMap()
        {
            return statusMap;
        }

        public override Status GetStatus(object value)
        {
            Status result;
            ServiceState serviceState = (ServiceState)value;
            if (statusMap.TryGetValue((int)serviceState, out result))
                return result;
            return defaultStatus;
        }

        public override string FormatMessage(Snapshot snapshot, BaseEvent baseEvent, MessageType messageType)
        {
            Status status = GetStatus(baseEvent.Value);
            string key = status.ToString() + this[messageType];
            return FormatMessage(snapshot, baseEvent, key);
        }
    }

    public abstract class ConfigurationOptionMetricMetaData : MetricMetaData
    {
        public ConfigurationOptionMetricMetaData(Metric metric, Messages.Status status, MetricClass metricType, string bodyKey, string headerKey, string toDoKey)
            : base(metric, status, metricType, bodyKey, headerKey, toDoKey)
        {
            ValueType = typeof(OptionStatus);
            Options = ThresholdOptions.OKDisabled | ThresholdOptions.MutuallyExclusive;
            Configurable = false;
        }

        public override object[] GetMessageData(Snapshot snapshot, BaseEvent baseEvent)
        {
            MonitoredObjectName mon = baseEvent.MonitoredObject;
            string serverName = mon != null
                                    ? mon.ServerName
                                    : snapshot != null
                                    ? snapshot.ServerName
                                    : "";

            return new object[]
                {
                    serverName,
                    baseEvent.Value.ToString().ToLower()
                };
        }
    }

    public static class MetricMetaDataFactory
    {
        private static BBS.TracerX.Logger LOG = BBS.TracerX.Logger.GetLogger("MetricMetaDataFactory");
        private static Dictionary<Metric, MetricMetaData> metaDataMap;

        static MetricMetaDataFactory()
        {
            metaDataMap = new Dictionary<Metric, MetricMetaData>();
            foreach (Type type in typeof(MetricMetaDataFactory).GetNestedTypes())
            {
                MetricMetaData instance = Activator.CreateInstance(type) as MetricMetaData;
                if (instance != null)
                {
                    metaDataMap.Add(instance.Metric, instance);
                }
            }
        }

        public static MetricMetaData GetMetaData(Metric metric)
        {
            MetricMetaData metadata = null;
            metaDataMap.TryGetValue(metric, out metadata);
            return metadata;
        }
        ////[START] - SQLdm 9.0 (Ankit Srivastava) - CWF Integration - Added two additional metadata classes

        //public class  PreferredNodeUnavailabilityMetaData: MetricMetaData
        //{
        //    public PreferredNodeUnavailabilityMetaData()
        //        : base(
        //            Metric.PreferredNodeUnavailability, Idera.SQLdm.Common.Messages.Status.Clrstatus, MetricClass.ServerThreshold,
        //            "prefNode_Body", "prefNode_Header", "prefNode_Todo")
        //    {
        //        ValueType = typeof(int);
        //    }
        //}

        //public class RepositoryGroomingTimedOutMetaData : MetricMetaData
        //{
        //    public RepositoryGroomingTimedOutMetaData()
        //        : base(
        //            Metric.RepositoryGroomingTimedOut, Idera.SQLdm.Common.Messages.Status.Clrstatus, MetricClass.ServerThreshold,
        //            "GroomingTimeout_Body", "GroomingTimeout_Header", "GroomingTimeout_Todo")
        //    {
        //        ValueType = typeof(int);
        //    }
        //}

		//[END] - SQLdm 9.0 (Ankit Srivastava) - CWF Integration - Added two additional metadata classes
        /* --------------------------------------------------------------------------- */
        /// <summary>
        /// note: The Value property of the event should contain the percentage to use 
        /// in the display message.  BaseEvent.Value is set by the Collection Service.
        /// </summary>
        /* --------------------------------------------------------------------------- */
        public class CPUUsageMetaData : MetricMetaData
        {
            public CPUUsageMetaData()
                : base(
                    Metric.SQLCPUUsagePct, Idera.SQLdm.Common.Messages.Status.Cpu, MetricClass.ServerThreshold,
                    "Cpu_Body", "Cpu_Header", "Cpu_Todo")
            {
                ValueType = typeof (int);
            }
        }

        public class UserConnectionMetaData : MetricMetaData
        {
            public UserConnectionMetaData()
                : base(
                    Metric.UserConnectionPct, Idera.SQLdm.Common.Messages.Status.Connections, MetricClass.ServerThreshold, 
                    "Connections_Body", "Connections_Header", "Connections_Todo")
            {
                DefaultWarningValue = 80;
                DefaultCriticalValue = 95;
                ValueType = typeof(int);
            }

            public override object[] GetMessageData(Snapshot snapshot, BaseEvent baseEvent)
            {
                ScheduledRefresh refresh = snapshot as ScheduledRefresh;
                long userProcesses = refresh.Server.SystemProcesses.CurrentUserProcesses.HasValue 
                    ? refresh.Server.SystemProcesses.CurrentUserProcesses.Value 
                    : 0;

                return new object[]
                {
                    snapshot.ServerName,
                    baseEvent.Value,
                    userProcesses
                };
            }
        }

        // can't find in alertdata
//        public class HighestThreadsPctMetaData : MetricMetaData
//        {
//            public HighestThreadsPctMetaData()
//                : base(
//                    Metric.HighestThreadsPct, Idera.SQLdm.Common.Messages.Status.Memory, MetricClass.ServerThreshold,
//                    "Memory_Body", "Memory_Header", "Memory_Todo")
//            {
//                ValueType = typeof (int);
//            }
//        }
//
//        // cant find in alertdata
//        public class CurrentThreadsPctMetaData : MetricMetaData
//        {
//            public CurrentThreadsPctMetaData()
//                : base(
//                    Metric.CurrentThreadsPct, Idera.SQLdm.Common.Messages.Status.Memory, MetricClass.ServerThreshold,
//                    "Memory_Body", "Memory_Header", "Memory_Todo")
//            {
//                ValueType = typeof (int);
//            }
//        }


        public class NonDistributedTransactionsMetaData : MetricMetaData
        {
            public NonDistributedTransactionsMetaData()
                : base(
                    Metric.NonDistributedTrans, Idera.SQLdm.Common.Messages.Status.Nondistributed,
                    MetricClass.ServerThreshold, "Nondistributed_Body", "Nondistributed_Header", "Nondistributed_Todo")
            {
                ValueType = typeof(int);
                DefaultWarningValue = 1;
                DefaultCriticalValue = 5;
                MaxValue = 0x7FFFFFFF;
                Options = ThresholdOptions.OKDisabled | ThresholdOptions.NumericValue | ThresholdOptions.CalculateMaxValue;
            }
        }

        public class NonSubscribedTranNoMetaData : MetricMetaData
        {
            public NonSubscribedTranNoMetaData()
                : base(
                    Metric.NonSubscribedTransNum, Idera.SQLdm.Common.Messages.Status.Nonsubscribed,
                    MetricClass.ServerThreshold, "Nonsubscribed_Body", "Nonsubscribed_Header", "Nonsubscribed_Todo")
            {
                ValueType = typeof (int);
                DefaultWarningValue = 1;
                DefaultCriticalValue = 5;
                MaxValue = 0x7FFFFFFF;
                Options = ThresholdOptions.OKDisabled | ThresholdOptions.NumericValue | ThresholdOptions.CalculateMaxValue;
            }
        }

        public class OldestOpenTransMinutesMetaData : MetricMetaData
        {
            public OldestOpenTransMinutesMetaData()
                : base(
                    Metric.OldestOpenTransMinutes, Idera.SQLdm.Common.Messages.Status.Opentran,
                    MetricClass.ServerThreshold, "Opentran_Body", "Opentran_Header", "Opentran_Todo")
            {
                ValueType = typeof(int);
            }

            public override object[] GetMessageData(Snapshot snapshot, BaseEvent baseEvent)
            {
                object[] result = new object[] { "", 0, "", "", "", "", 0, "" };
                OpenTransaction xa = baseEvent.AdditionalData as OpenTransaction;

                if (xa != null)
                {
                    result[0] = snapshot.ServerName;
                    result[1] = baseEvent.Value;
                    result[2] = xa.Database;
                    result[3] = xa.Application;
                    result[4] = xa.Workstation;
                    result[5] = xa.UserName;
                    result[6] = xa.Spid;
                    result[7] = xa.Command;
                } 
                return result;
            }

            public override string FormatMessage(Snapshot snapshot, BaseEvent baseEvent, MessageType messageType)
            {
//                if (baseEvent.MonitoredState != MonitoredState.OK)
                    return base.FormatMessage(snapshot, baseEvent, messageType);

//                string resourceKey = "OldestopenEnded_" + messageType.ToString();

//                return String.Format(GetStringResource(resourceKey, baseEvent.Metric.ToString()), GetMessageData(snapshot, baseEvent));
            }
        }


        public class ReorganisationPctMetaData : MetricMetaData
        {
            public ReorganisationPctMetaData()
                : base(
                    Metric.ReorganisationPct, Idera.SQLdm.Common.Messages.Status.Reorg, MetricClass.ServerThreshold,
                    "Reorg_Body", "Reorg_Header", "Reorg_Todo")
            {
                ValueType = typeof(int);
            }

            public override object[] GetMessageData(Snapshot snapshot, BaseEvent baseEvent)
            {
                return baseEvent.AdditionalData as object[];
            }
        }

        public class DatabaseSizeMetaData : MetricMetaData
        {
            public DatabaseSizeMetaData()
                : base(
                    Metric.DatabaseSizePct, Idera.SQLdm.Common.Messages.Status.Dbsize, MetricClass.ServerThreshold,
                    "Dbsize_Body", "Dbsize_Header", "Dbsize_Todo")
            {
                ValueType = typeof (int);
            }
        }

        public class LogSizeMetaData : MetricMetaData
        {
            public LogSizeMetaData()
                : base(
                    Metric.TransLogSize, Idera.SQLdm.Common.Messages.Status.Logsize, MetricClass.ServerThreshold,
                    "Logsize_Body", "Logsize_Header", "Logsize_Todo")
            {
                ValueType = typeof (int);
            }
        }

        public class SQLExecutiveStatusMetaData : ServiceStateMetricMetaData
        {
            public SQLExecutiveStatusMetaData()
                : base(Metric.AgentServiceStatus, MetricClass.ServerThreshold)
            {
                defaultStatus = Status.Execother;
                statusMap.Add((int)ServiceState.Running, Status.Execrunning);
                statusMap.Add((int)ServiceState.Paused, Status.Execstopped);
                statusMap.Add((int)ServiceState.Stopped, Status.Execstopped);
                statusMap.Add((int)ServiceState.NotInstalled, Status.Execother);
                statusMap.Add((int)ServiceState.UnableToMonitor, Status.Execother);
            }
        }

        public class ReadWriteErrorsMetaData : MetricMetaData
        {
            public ReadWriteErrorsMetaData()
                : base(
                    Metric.ReadWriteErrors, Idera.SQLdm.Common.Messages.Status.Io, MetricClass.ServerThreshold,
                    "Io_Body", "Io_Header", "Io_Todo")
            {
                ValueType = typeof (int);
                Options = ThresholdOptions.OKDisabled | ThresholdOptions.NumericValue | ThresholdOptions.MutuallyExclusive;
                Configurable = false;
            }
        }

        public class ServerStatusMetaData : ServiceStateMetricMetaData
        {
            public ServerStatusMetaData()
                : base(Metric.SqlServiceStatus, MetricClass.ServerThreshold)
            {
                defaultStatus = Status.Serverother;
                statusMap.Add((int)ServiceState.Running, Status.Serverrunning);
                statusMap.Add((int)ServiceState.Paused, Status.Serverpaused);
                statusMap.Add((int)ServiceState.Stopped, Status.Serverstopped);
                statusMap.Add((int)ServiceState.NotInstalled, Status.Serverother);
                statusMap.Add((int)ServiceState.UnableToMonitor, Status.Serverother);
            }
        }

        public class SQLMemoryMetaData : MetricMetaData
        {
            public SQLMemoryMetaData()
                : base(
                    Metric.SQLMemoryUsagePct, Idera.SQLdm.Common.Messages.Status.Memory, MetricClass.ServerThreshold,
                    "Memory_Body", "Memory_Header", "Memory_Todo")
            {
                ValueType = typeof (int);
            }
        }

        public class DatabaseStatusMetaData : MetricMetaData
        {
            public DatabaseStatusMetaData()
                : base(
                    Metric.DatabaseStatus, Idera.SQLdm.Common.Messages.Status.Dbstatus, MetricClass.ServerThreshold,
                    "Dbstatus_Body", "Dbstatus_Header", "Dbstatus_Todo")
            {
                ValueType = typeof(DBStatus);
                MultiValued = true;
                Options = ThresholdOptions.OKDisabled | ThresholdOptions.ContainedValueList;
            }
        }

/*
        public class DMStopMetaData : MetricMetaData
        {
            public DMStopMetaData()
                : base(
                    Metric.DMStop, Idera.SQLdm.Common.Messages.Status.Applicationstopped, MetricClass.ServerThreshold,
                    "Applicationstopped_Body", "Applicationstopped_Header", "Applicationstopped_Todo")
            {
            }

            public override object[] GetMessageData(Snapshot snapshot, BaseEvent baseEvent)
            {
                return new object[]
                    {
                        GetMonitoredObjectName(baseEvent, "Unknown")
                    };
            }
        }

        public class DMStartMetaData : MetricMetaData
        {
            public DMStartMetaData()
                : base(
                    Metric.DMStart, Idera.SQLdm.Common.Messages.Status.Applicationstarted, MetricClass.ServerThreshold,
                    "Applicationstarted_Body", "Applicationstarted_Header", "Applicationstarted_Todo")
            {
            }

            public override object[] GetMessageData(Snapshot snapshot, BaseEvent baseEvent)
            {
                return new object[]
                    {
                        GetMonitoredObjectName(baseEvent, "Unknown")
                    };
            }
        }
 */

        public class NonSubscribedTranTimeMetaData : MetricMetaData
        {
            public NonSubscribedTranTimeMetaData()
                : base(
                    Metric.NonSubscribedTransTime, Idera.SQLdm.Common.Messages.Status.Nonsubscribedtime,
                    MetricClass.ServerThreshold, "Nonsubscribedtime_Body", "Nonsubscribedtime_Header",
                    "Nonsubscribedtime_Todo")
            {
                ValueType = typeof (int);
                DefaultWarningValue = 30;
                DefaultCriticalValue = 60;
                MaxValue = 10000;
                Options = ThresholdOptions.OKDisabled | ThresholdOptions.NumericValue | ThresholdOptions.CalculateMaxValue;
            }
        }

        public class DTCStatusMetaData : ServiceStateMetricMetaData
        {
            public DTCStatusMetaData()
                : base(Metric.DtcServiceStatus, MetricClass.ServerThreshold)
            {
                defaultStatus = Status.Dtcother;
                statusMap.Add((int)ServiceState.Running, Status.Dtcrunning);
                statusMap.Add((int)ServiceState.Paused, Status.Dtcstopped);
                statusMap.Add((int)ServiceState.Stopped, Status.Dtcstopped);
                statusMap.Add((int)ServiceState.NotInstalled, Status.Dtcother);
                statusMap.Add((int)ServiceState.UnableToMonitor, Status.Dtcother);
            }
        }

        public class FullTextSearchMetaData : ServiceStateMetricMetaData
        {
            public FullTextSearchMetaData()
                : base(Metric.FullTextServiceStatus, MetricClass.ServerThreshold)
            {
                defaultStatus = Status.Fulltextother;
                statusMap.Add((int)ServiceState.Running, Status.Fulltextrunning);
                statusMap.Add((int)ServiceState.Paused, Status.Fulltextother);
                statusMap.Add((int)ServiceState.Stopped, Status.Fulltextother);
                statusMap.Add((int)ServiceState.NotInstalled, Status.Fulltextother);
                statusMap.Add((int)ServiceState.UnableToMonitor, Status.Fulltextother);
            }
        }

        // cant find this one in alertdata
        public class IndexRowHitsMetaData : MetricMetaData
        {
            public IndexRowHitsMetaData()
                : base(
                    Metric.IndexRowHits, Idera.SQLdm.Common.Messages.Status.Memory, MetricClass.ServerThreshold,
                    "Memory_Body", "Memory_Header", "Memory_Todo")
            {
                ValueType = typeof (int);
            }
        }

        // cant find this on in alertdata
        public class FullTextRefreshHoursMetaData : MetricMetaData
        {
            public FullTextRefreshHoursMetaData()
                : base(
                    Metric.FullTextRefreshHours, Idera.SQLdm.Common.Messages.Status.Memory, MetricClass.ServerThreshold,
                    "Memory_Body", "Memory_Header", "Memory_Todo")
            {
                ValueType = typeof (int);
            }
        }

        public class ServerResponseMetaData : MetricMetaData
        {
            public ServerResponseMetaData()
                : base(
                    Metric.ServerResponseTime, Idera.SQLdm.Common.Messages.Status.Serverresponsetime,
                    MetricClass.ServerThreshold, "Serverresponsetime_Body", "Serverresponsetime_Header",
                    "Serverresponsetime_Todo")
            {
                ValueType = typeof (int);
                MaxValue = 10000;
            }
        }

        public class OSMetricsStatusMetaData : ServiceStateMetricMetaData
        {
            public OSMetricsStatusMetaData()
                : base(Metric.OSMetricsStatus, MetricClass.ServerThreshold)
            {
                ValueType = typeof (OSMetricsStatus);
                defaultStatus = Status.Osmetricsstatusother;
                statusMap.Add((int)OSMetricsStatus.Available, Status.Osmetricsstatusavail);
                statusMap.Add((int)OSMetricsStatus.Disabled, Status.Osmetricsstatusdisabled);
                statusMap.Add((int)OSMetricsStatus.OLEAutomationUnavailable, Status.Osmetricsstatusother);
                statusMap.Add((int)OSMetricsStatus.WMIServiceUnreachable, Status.Osmetricsstatusunavail);
            }
        }

        public class OSMemoryAvailableBytesMetaData : MetricMetaData
        {   
            public OSMemoryAvailableBytesMetaData()
                : base(
                    Metric.OSMemoryUsagePct, Idera.SQLdm.Common.Messages.Status.Osmemoryusedpct,
                    MetricClass.ServerThreshold, "Osmemoryusedpct_Body", "Osmemoryusedpct_Header",
                    "Osmemoryusedpct_Todo")
            {
                ValueType = typeof (int);
                DefaultWarningValue = 20;
                DefaultCriticalValue = 10;
            }
        }

        public class OSMemoryPagesPerSecondMetaData : MetricMetaData
        {
            public OSMemoryPagesPerSecondMetaData()
                : base(
                    Metric.OSMemoryPagesPerSecond, Idera.SQLdm.Common.Messages.Status.Osmemorypagespersecond,
                    MetricClass.ServerThreshold, "Osmemorypagespersecond_Body", "Osmemorypagespersecond_Header",
                    "Osmemorypagespersecond_Todo")
            {
                ValueType = typeof (int);
                DefaultWarningValue = 100;
                DefaultCriticalValue = 1000;
                MaxValue = 10000;
                Options = ThresholdOptions.OKDisabled | ThresholdOptions.NumericValue | ThresholdOptions.CalculateMaxValue;
            }
        }

        public class OSCPUProcessorTimeMetaData : MetricMetaData
        {
            public OSCPUProcessorTimeMetaData()
                : base(
                    Metric.OSCPUUsagePct, Idera.SQLdm.Common.Messages.Status.Oscpuprocessortime,
                    MetricClass.ServerThreshold, "Oscpuprocessortime_Body", "Oscpuprocessortime_Header",
                    "Oscpuprocessortime_Todo")
            {
                ValueType = typeof (int);
                DefaultWarningValue = 75;
                DefaultCriticalValue = 90;
            }
        }

        public class OSCPUPrivilegedTimeMetaData : MetricMetaData
        {
            public OSCPUPrivilegedTimeMetaData()
                : base(
                    Metric.OSCPUPrivilegedTimePct, Idera.SQLdm.Common.Messages.Status.Oscpuprivilegedtime,
                    MetricClass.ServerThreshold, "Oscpuprivilegedtime_Body", "Oscpuprivilegedtime_Header",
                    "Oscpuprivilegedtime_Todo")
            {
                ValueType = typeof (int);
                DefaultWarningValue = 75;
                DefaultCriticalValue = 90;
            }
        }

        public class OSCPUUserTimeMetaData : MetricMetaData
        {
            public OSCPUUserTimeMetaData()
                : base(
                    Metric.OSUserCPUUsagePct, Idera.SQLdm.Common.Messages.Status.Oscpuusertime,
                    MetricClass.ServerThreshold, "Oscpuusertime_Body", "Oscpuusertime_Header", "Oscpuusertime_Todo")
            {
                ValueType = typeof (int);
                DefaultWarningValue = 75;
                DefaultCriticalValue = 90;
            }
        }

        public class OSCPUProcessorQueueLengthMetaData : MetricMetaData
        {
            public OSCPUProcessorQueueLengthMetaData()
                : base(
                    Metric.OSCPUProcessorQueueLength, Idera.SQLdm.Common.Messages.Status.Oscpuprocessorqueuelength,
                    MetricClass.ServerThreshold, "Oscpuprocessorqueuelength_Body", "Oscpuprocessorqueuelength_Header",
                    "Oscpuprocessorqueuelength_Todo")
            {
                ValueType = typeof (int);
                DefaultWarningValue = 3;
                DefaultCriticalValue = 5;
                MaxValue = 999;
                Options = ThresholdOptions.OKDisabled | ThresholdOptions.NumericValue | ThresholdOptions.CalculateMaxValue;
            }
        }

        public class OSDiskAverageDiskQueueLengthMetaData : MetricMetaData
        {
            public OSDiskAverageDiskQueueLengthMetaData()
                : base(
                    Metric.OSDiskAverageDiskQueueLength, Idera.SQLdm.Common.Messages.Status.Osdiskaveragediskqueuelength,
                    MetricClass.ServerThreshold, "Osdiskaveragediskqueuelength_Body",
                    "Osdiskaveragediskqueuelength_Header", "Osdiskaveragediskqueuelength_Todo")
            {
                ValueType = typeof (int);
                DefaultWarningValue = 3;
                DefaultCriticalValue = 5;
                MaxValue = 999;
                Options = ThresholdOptions.OKDisabled | ThresholdOptions.NumericValue | ThresholdOptions.CalculateMaxValue;
            }
        }

        public class OSDiskPhysicalDiskTimeMetaData : MetricMetaData
        {
            public OSDiskPhysicalDiskTimeMetaData()
                : base(
                    Metric.OSDiskPhysicalDiskTimePct, Idera.SQLdm.Common.Messages.Status.Osdiskphysicaldisktime, MetricClass.ServerThreshold, 
                    "Osdiskphysicaldisktime_Body", "Osdiskphysicaldisktime_Header","Osdiskphysicaldisktime_Todo")
            {
                ValueType = typeof (int);
                DefaultWarningValue = 75;
                DefaultCriticalValue = 90;
            }
        }

        public class ResourceAlertMetaData : MetricMetaData
        {
            public ResourceAlertMetaData()
                : base(
                    Metric.ResourceAlert, Idera.SQLdm.Common.Messages.Status.Resourcelimit, MetricClass.ServerThreshold,
                    "Resourcelimit_Body", "Resourcelimit_Header", "Resourcelimit_Todo")
            {
                ValueType = typeof(int);
                MaxValue = 999;
                Options = ThresholdOptions.OKDisabled | ThresholdOptions.NumericValue | ThresholdOptions.CalculateMaxValue;
            }

            public override object[] GetMessageData(Snapshot snapshot, BaseEvent baseEvent)
            {
                string serverName = string.Empty, 
                       sessionID = "N/A", 
                       loginTime = "N/A", 
                       cpu = string.Empty;

                if(baseEvent != null)
                {
                    MonitoredObjectName mon = baseEvent.MonitoredObject;
                    if(mon != null)
                    {
                        serverName = mon.ServerName;
                        sessionID = mon.AdditionalQualifiers[0];
                    }
                    else if(snapshot != null)
                    {
                        serverName = snapshot.ServerName;
                    }
                    DateTime? ldt = (DateTime?) baseEvent.AdditionalData;
                    if (ldt != null)
                    {
                        loginTime = ldt.Value.ToLocalTime().ToString();
                    }
                    cpu = string.Format("{0:F3}", baseEvent.Value);
                }

                return new object[]
                {
                    serverName,
                    cpu,
                    sessionID,
                    loginTime
                };
            }
        }

        public class BlockingAlertMetaData : MetricMetaData
        {
            public BlockingAlertMetaData()
                :
                    base(
                    Metric.BlockingAlert, Idera.SQLdm.Common.Messages.Status.Blockingprocess,
                    MetricClass.ServerThreshold, "Blockingprocess_Body", "Blockingprocess_Header", "Blockingprocess_Todo")
            {
                MaxValue = 600;
                ValueType = typeof(Int32);
                Options = ThresholdOptions.OKDisabled | ThresholdOptions.NumericValue;
            }

            public override object[] GetMessageData(Snapshot snapshot, BaseEvent baseEvent)
            {
                object[] result = new object[] { "", 0, "", 0, "", "", "", "" };
                BlockingSession bs = baseEvent.AdditionalData as BlockingSession;

                if (bs != null)
                {
                    result[0] = snapshot.ServerName;
                    result[1] = baseEvent.Value;
                    result[2] = bs.Databasename;
                    result[3] = bs.Spid;
                    result[4] = bs.Login;
                    result[5] = bs.Application;
                    result[6] = bs.Host;
                    result[7] = bs.InputBuffer;
                }
                return result;
            }
        }

        public class LongJobsMetaData : MetricMetaData
        {
            public LongJobsMetaData()
                : base(
                    Metric.LongJobs, Idera.SQLdm.Common.Messages.Status.Longrunningjob, MetricClass.ServerThreshold,
                    "Longrunningjob_Body", "Longrunningjob_Header", "Longrunningjob_Todo")
            {
                ValueType = typeof(Int32);
                MaxValue = 999;
                Options =   ThresholdOptions.OKDisabled | 
                            ThresholdOptions.NumericValue | 
                            ThresholdOptions.AdditionalData |
                            ThresholdOptions.CalculateMaxValue;
            }

            public override object[] GetMessageData(Snapshot snapshot, BaseEvent baseEvent)
            {
                object[] result = new object[] { "", 0, "", 0, 0, 0, null, "" };
                AgentJobRunning ajr = baseEvent.AdditionalData as AgentJobRunning;

                if (ajr != null)
                {
                    result[0] = snapshot.ServerName;
                    result[1] = baseEvent.Value;
                    result[2] = ajr.JobName;
                    result[3] = (int)ajr.RunTimePercentOver;
                    result[4] = (int)ajr.AverageDurationInSeconds;
                    result[5] = ajr.RunningTime.TotalSeconds;
                    result[6] = ajr.StartedAt.HasValue ? ajr.StartedAt.Value.ToLocalTime() : (DateTime?)null;
                    result[7] = ajr.StepName;
                }
                return result;
            }
        }

        public class BombedJobsMetaData : MetricMetaData
        {
            public BombedJobsMetaData()
                :
                    base(
                    Metric.BombedJobs, Idera.SQLdm.Common.Messages.Status.Bombedjob, MetricClass.ServerThreshold,
                    "Bombedjob_Body", "Bombedjob_Header", "Bombedjob_Todo")
            {
                // only enable/disable the options
                Configurable = false;
                ValueType = typeof(Int32);
                // can't change OK, value is numeric, may addl data, only one item can be enabled
                Options = ThresholdOptions.OKDisabled | ThresholdOptions.NumericValue | ThresholdOptions.AdditionalData | ThresholdOptions.MutuallyExclusive;
            }

            public override object[] GetMessageData(Snapshot snapshot, BaseEvent baseEvent)
            {
                object[] result = new object[] { "", "", "", null, "", 0, 0, "", "" };
                AgentJobFailure ajf = baseEvent.AdditionalData as AgentJobFailure;

                if (ajf != null)
                {
                    result[0] = snapshot.ServerName;
                    result[1] = baseEvent.Value;
                    result[2] = ajf.JobName;
                    result[3] = ajf.RunTime.HasValue ? ajf.RunTime.Value.ToLocalTime() : (DateTime?)null;
                    result[4] = ajf.StepName;
                    result[5] = ajf.SqlSeverity;
                    result[6] = ajf.SqlMessageId;
                    result[7] = ajf.ErrorMessage;
                    result[8] = ajf.Command;
                }
                return result;
            }
        }

        public class MaintenanceModeMetaData : MetricMetaData
        {
            public MaintenanceModeMetaData()
                : base(Metric.MaintenanceMode, MetricClass.Hardcoded, "_Body", "_Header", "_Todo")
            {
                ValueType = typeof(OptionStatus);
                Options = ThresholdOptions.OKDisabled | ThresholdOptions.MutuallyExclusive;
                Configurable = false;

                // write to alert log only
                processNotifications = false;
            }

            public override Status GetStatus(object value)
            {
                return ((OptionStatus)value) == OptionStatus.Enabled ? Status.Maintenancemodestart : Status.Maintenancemodeend;
            }

            public override string FormatMessage(Snapshot snapshot, BaseEvent baseEvent, MessageType messageType)
            {
                Status status = GetStatus(baseEvent.Value);
                string key = status.ToString() + this[messageType];
                return FormatMessage(snapshot, baseEvent, key);
            }
        }

//        public class LoginHasAdministratorRightsMetaData : MetricMetaData
//        {
//            public LoginHasAdministratorRightsMetaData()
//                : base(
//                    Metric.LoginHasAdministratorRights, Idera.SQLdm.Common.Messages.Status.Memory,
//                    MetricClass.ServerThreshold, "Memory_Body", "Memory_Header", "Memory_Todo")
//            {
//                ValueType = typeof(int);
//            }
//        }
//
//
//        public class MaxConnectionsMetaData : MetricMetaData
//        {
//            public MaxConnectionsMetaData()
//                : base(
//                    Metric.MaxConnections, Idera.SQLdm.Common.Messages.Status.Memory, MetricClass.ServerThreshold,
//                    "Memory_Body", "Memory_Header", "Memory_Todo")
//            {
//                ValueType = typeof(int);
//            }
//        }
//
//
//        public class PhysicalMemoryMetaData : MetricMetaData
//        {
//            public PhysicalMemoryMetaData()
//                : base(
//                    Metric.PhysicalMemory, Idera.SQLdm.Common.Messages.Status.Memory, MetricClass.ServerThreshold,
//                    "Memory_Body", "Memory_Header", "Memory_Todo")
//            {
//                ValueType = typeof(int);
//            }
//        }
//
//        public class ProductVersionMetaData : MetricMetaData
//        {
//            public ProductVersionMetaData()
//                : base(
//                    Metric.ProductVersion, Idera.SQLdm.Common.Messages.Status.Memory, MetricClass.ServerThreshold,
//                    "Memory_Body", "Memory_Header", "Memory_Todo")
//            {
//                ValueType = typeof(int);
//            }
//        }

//        public class ServerOpenTransactionsMetaData : MetricMetaData
//        {
//            public ServerOpenTransactionsMetaData()
//                : base(
//                    Metric.ServerOpenTransactions, Idera.SQLdm.Common.Messages.Status.Opentran,
//                    MetricClass.ServerThreshold, "Opentran_Body", "Opentran_Header", "Opentran_Todo")
//            {
//                ValueType = typeof(int);
//            }
//        }

        public class DatabaseFileSizeMetaData : MetricMetaData
        {
            public DatabaseFileSizeMetaData()
                : base(
                    Metric.DatabaseFileSize, Idera.SQLdm.Common.Messages.Status.Dbsize, MetricClass.ServerThreshold,
                    "Dbsize_Body", "Dbsize_Header", "Dbsize_Todo")
            {
                ValueType = typeof(int);
            }
        }

        public class CLRStatusMetaData : ConfigurationOptionMetricMetaData
        {
            public CLRStatusMetaData()
                : base(
                    Metric.CLRStatus, Idera.SQLdm.Common.Messages.Status.Clrstatus, MetricClass.ServerThreshold,
                    "Clrstatus_Body", "Clrstatus_Header", "Clrstatus_Todo")
            {
            }
        }

        public class OLEAutomationStatusMetaData : ConfigurationOptionMetricMetaData
        {
            public OLEAutomationStatusMetaData()
                : base(
                    Metric.OLEAutomationStatus, Idera.SQLdm.Common.Messages.Status.Automationstatus, MetricClass.ServerThreshold,
                    "Automationstatus_Body", "Automationstatus_Header", "Automationstatus_Todo")
            {}
        }

        public class QueryMonitorStatusMetaData : MetricMetaData
        {
            public QueryMonitorStatusMetaData()
                : base(
                    Metric.QueryMonitorStatus, Idera.SQLdm.Common.Messages.Status.Querymonitorstatus, MetricClass.ServerThreshold,
                    "Querymonitorstatus_Body", "Querymonitorstatus_Header", "Querymonitorstatus_Todo")
            {
                MaxValue = 1000;
                DefaultWarningValue = 0;
                DefaultCriticalValue = 1;
                ValueType = typeof(int);
                Options = ThresholdOptions.OKDisabled | ThresholdOptions.NumericValue;
            }
        }


//        public class SQLdmCollectionServiceStatusMetaData : MetricMetaData
//        {
//            public SQLdmCollectionServiceStatusMetaData()
//                : base(
//                    Metric.SQLdmCollectionServiceStatus, Idera.SQLdm.Common.Messages.Status.Collectionservicestatus,
//                    MetricClass.ServerThreshold, "Collectionservicestatus_Body", "Collectionservicestatus_Header", "Collectionservicestatus_Todo")
//            {
//                ValueType = typeof(SQLdmServiceState);
//            }
//        }

        public class SQLdmCollectionServiceStatusMetaData : ServiceStateMetricMetaData
        {
            public SQLdmCollectionServiceStatusMetaData()
                : base(Metric.SQLdmCollectionServiceStatus, MetricClass.Processes)
            {
                ValueType = typeof(SQLdmServiceState);
                defaultStatus = Status.Collectionservicestatus;
            }
        }


        public class AgentXPStatusMetaData : ConfigurationOptionMetricMetaData
        {
            public AgentXPStatusMetaData()
                : base(
                    Metric.AgentXPStatus, Idera.SQLdm.Common.Messages.Status.Agentxpstatus, MetricClass.ServerThreshold,
                    "Agentxpstatus_Body", "Agentxpstatus_Header", "Agentxpstatus_Todo")
            {}
        }

        public class WMIStatusMetaData : MetricMetaData
        {
            public WMIStatusMetaData()
                : base(
                    Metric.WMIStatus, Idera.SQLdm.Common.Messages.Status.Wmistatus, MetricClass.ServerThreshold,
                    "Wmistatus_Body", "Wmistatus_Header", "Wmistatus_Todo")
            {
                ValueType = typeof(OptionStatus);
                Options = ThresholdOptions.OKDisabled | ThresholdOptions.MutuallyExclusive;
                Configurable = false;
            }
        }

        public class ClientComputersMetaData : MetricMetaData
        {
            public ClientComputersMetaData()
                : base(
                    Metric.ClientComputers, Idera.SQLdm.Common.Messages.Status.Clientcomputers,
                    MetricClass.ServerThreshold, "Clientcomputers_Body", "Clientcomputers_Header",
                    "Clientcomputers_Todo")
            {
                ValueType = typeof(int);
                DefaultWarningValue = 25;
                DefaultCriticalValue = 50;
                MaxValue = 100000;
                Options = ThresholdOptions.OKDisabled | ThresholdOptions.NumericValue | ThresholdOptions.CalculateMaxValue;
            }
        }
        public class BlockedSessionsMetaData : MetricMetaData
        {
            public BlockedSessionsMetaData()
                : base(
                    Metric.BlockedSessions, Idera.SQLdm.Common.Messages.Status.Blockingsessions,
                    MetricClass.ServerThreshold, "Blockingsessions_Body", "Blockingsessions_Header",
                    "Blockingsessions_Todo")
            {
                ValueType = typeof(int);
                DefaultWarningValue = 1;
                DefaultCriticalValue = 1;
                MaxValue = 10000;
                Options = ThresholdOptions.OKDisabled | ThresholdOptions.NumericValue | ThresholdOptions.CalculateMaxValue;
            }
        }
        public class DataUsedPercentMetaData : MetricMetaData
        {
            public DataUsedPercentMetaData()
                : base(
                    Metric.DataUsedPct, Idera.SQLdm.Common.Messages.Status.Datausedpercent,
                    MetricClass.ServerThreshold, "Datausedpercent_Body", "Datausedpercent_Header",
                    "Datausedpercent_Todo")
            {
                ValueType = typeof(int);
                DefaultWarningValue = 75;
                DefaultCriticalValue = 90;
            }
        }
        public class LogUsedPercentMetaData : MetricMetaData
        {
            public LogUsedPercentMetaData()
                : base(
                    Metric.LogUsedPct, Idera.SQLdm.Common.Messages.Status.Logusedpercent,
                    MetricClass.ServerThreshold, "Logusedpercent_Body", "Logusedpercent_Header",
                    "Logusedpercent_Todo")
            {
                ValueType = typeof(int);
                DefaultWarningValue = 75;
                DefaultCriticalValue = 90;
            }
        }



    }  // class MetricMetaDataFactory

    [Serializable]
    public class MetricInfo
    {
        public static readonly Dictionary<Metric,MetricInfo> MetricInfoMap = new Dictionary<Metric,MetricInfo>();

        public Metric metric;
        public string name;
        public string category;
        public string description;
        public string comments;
        public int rank;

        public MetricInfo(Metric metric, string name, string category, string description, string comments, int rank)
        {
            this.metric = metric;
            this.name = name;
            this.description = description;
            this.category = category;
            this.comments = comments;
            this.rank = rank;
        }

        public Metric Metric
        {
            get { return metric;  }
        }
        public string Name
        {
            get { return name; }
        }
        public string Description
        {
            get { return description; }
        }
        public string Category
        {
            get { return category; }
        }
        public string Comments
        {
            get { return comments;  }
            set { comments = value; }
        }
        public int Rank
        {
            get { return rank; }
        }

    }


}
