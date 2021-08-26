//------------------------------------------------------------------------------
// <copyright file="ActivityProfilerConfiguration.cs" company="Idera, Inc.">
//     Copyright (c) Idera, Inc. All rights reserved.
// </copyright>
//------------------------------------------------------------------------------
using System.Collections.Generic;
using System.Text;
using Idera.SQLdm.Common.Attributes;
using Idera.SQLdm.Common.Snapshots;

namespace Idera.SQLdm.Common.Configuration
{
    using System;

// Supress the warning regarding overrides
#pragma warning disable 0659
    /// <summary>
    /// Configuration object for Active Waits probe
    /// </summary>
    [Serializable]
    public class ActivityMonitorConfiguration
    {
        //START SQLdm 9.1 (Ankit Srivastava): Activity Monitoring with Extended Events  -- Added new constant fields
        public const string FileNameXEsessionDefault = "AMExtendedEventLog.xel";
        public const bool EnabledXeDefault = true;
        public const int FileSizeXeMBDefault = 1;
        public const int FileSizeRolloverXeDefault = 3;
        public const int RecordsPerRefreshXeDefault = 1000;
        public const int MaxMemoryXeMBDefault = 1;
        public const XeEventRetentionMode EventRetentionModeXeDefault = XeEventRetentionMode.ALLOW_MULTIPLE_EVENT_LOSS;
        public const int MaxDispatchLatencyXeDefault = 300;
        public const int MaxEventSizeXembDefault = 1;
        public const XEMemoryPartitionMode MemoryPartitionModeXeDefault = XEMemoryPartitionMode.NONE;
        public const bool TrackCausalityXeDefault = false;
        public const bool StartupStateXeDefault = false;
        //END SQLdm 9.1 (Ankit Srivastava): Activity Monitoring with Extended Events  -- Added new constant fields

        private bool enabled;
        private bool deadlockEventsEnabled = false;
        private int blockedProcessThreshold = 30;
        private bool blockingEventsEnabled = false;
        private bool autogrowEventsEnabled = true;
        private FileSize traceFileSize = new FileSize(1024);
        private int traceFileRollovers = 3;
        private int recordsPerRefresh = 100;
        private AdvancedQueryMonitorConfiguration advancedConfiguration;
        private DateTime? stopTimeUTC = null;

        //START SQLdm 9.1 (Ankit Srivastava): Activity Monitoring with Extended Events  -- Added new fields
        private bool traceMonitoringEnabled = true;
        private int fileSizeXEMB = FileSizeXeMBDefault;
        private int fileSizeRolloverXE = FileSizeRolloverXeDefault;
        private int recordsPerRefreshXE = RecordsPerRefreshXeDefault;
        private int maxMemoryXEMB = MaxMemoryXeMBDefault;
        private XeEventRetentionMode eventRetentionModeXE = EventRetentionModeXeDefault;
        private int maxDispatchLatencyXE = MaxDispatchLatencyXeDefault;
        private int maxEventSizeXEMB = MaxEventSizeXembDefault;
        private XEMemoryPartitionMode memoryPartitionModeXE = XEMemoryPartitionMode.NONE;
        private bool trackCausalityXE = TrackCausalityXeDefault;
        private bool startupStateXE = StartupStateXeDefault;
        private string fileNameXEsession = FileNameXEsessionDefault;
        //END SQLdm 9.1 (Ankit Srivastava): Activity Monitoring with Extended Events  -- Added new fields

        public ActivityMonitorConfiguration(bool enabled, bool deadlockEventsEnabled, bool blockingEventsEnabled, bool autogrowEventsEnabled, int blockedProcessThreshold,
                                         FileSize traceFileSize, int traceFileRollovers, int recordsPerRefresh,
                                         AdvancedQueryMonitorConfiguration advancedConfiguration, bool traceMonitoringEnabled)//SQLdm 9.1 (Ankit Srivastava): Activity Monitoring with Extended Events-- modifed the Constructor with a new param
            :
            this(enabled, deadlockEventsEnabled, blockingEventsEnabled, autogrowEventsEnabled, blockedProcessThreshold, traceFileSize, traceFileRollovers, recordsPerRefresh, advancedConfiguration, null, traceMonitoringEnabled)
        {
            
        }

        public ActivityMonitorConfiguration(bool enabled, bool deadlockEventsEnabled, bool blockingEventsEnabled, bool autogrowEventsEnabled, int blockedProcessThreshold,
                                         FileSize traceFileSize, int traceFileRollovers, int recordsPerRefresh,
                                         AdvancedQueryMonitorConfiguration advancedConfiguration,
                                            DateTime? stopTimeUTC, bool traceMonitoringEnabled)//SQLdm 9.1 (Ankit Srivastava): Activity Monitoring with Extended Events-- modifed the Constructor with a new param
        {
            this.enabled = enabled;
            this.deadlockEventsEnabled = deadlockEventsEnabled;
            this.blockingEventsEnabled = blockingEventsEnabled;
            this.autogrowEventsEnabled = autogrowEventsEnabled;
            this.blockedProcessThreshold = blockedProcessThreshold;
            this.traceFileSize = traceFileSize;
            this.traceFileRollovers = traceFileRollovers;
            this.recordsPerRefresh = recordsPerRefresh;
            this.advancedConfiguration = advancedConfiguration;
            this.stopTimeUTC = stopTimeUTC;
            this.traceMonitoringEnabled = traceMonitoringEnabled;//SQLdm 9.1 (Ankit Srivastava): Activity Monitoring with Extended Events-- assigned the value to new field 
        }


        [Auditable("Enabled Monitoring of non-query activities")]
        public bool Enabled
        {
            get { return enabled;  }
            set { enabled = value; }
        }

        //START SQLdm 9.1 (Ankit Srivastava): Activity Monitoring with Extended Events - Added new properties
        [Auditable("Activity Monitoring by trace enabled")]
        public bool TraceMonitoringEnabled
        {
            get { return traceMonitoringEnabled; }
            set { traceMonitoringEnabled = value; }
        }

        /// <summary>
        /// Size of the XE output file
        /// </summary>
        [Auditable(false)]
        public int FileSizeXeMB
        {
            get { return fileSizeXEMB; }
            set { fileSizeXEMB = value; }
        }
        /// <summary>
        /// Number of XE files to allow before rollover
        /// </summary>
        [Auditable(false)]
        public int FileSizeRolloverXe
        {
            get { return fileSizeRolloverXE; }
            set { fileSizeRolloverXE = value; }
        }
        /// <summary>
        /// Number of records to select back from XE file
        /// </summary>
        public int RecordsPerRefreshXe
        {
            get { return recordsPerRefreshXE; }
            set { recordsPerRefreshXE = value; }
        }
        /// <summary>
        /// Sets the maximum amount of memory in kilobytes (KB) or megabytes (MB) that the session allocates for event buffering.
        /// </summary>
        [Auditable(false)]
        public int MaxMemoryXeMB
        {
            get { return maxMemoryXEMB; }
            set { maxMemoryXEMB = value; }
        }
        /// <summary>
        /// Specifies the event retention mode to use for handling event loss. There are three possible values:
        /// </summary>
        public XeEventRetentionMode EventRetentionModeXe
        {
            get { return eventRetentionModeXE; }
            set { eventRetentionModeXE = value; }
        }
        /// <summary>
        /// Specifies how long events are held in the buffers before being dispatched to the session targets. The time is in seconds, with a minimum value of one second. 
        /// A value of zero or INFINITE flushes the buffers only when they are full.
        /// </summary>
        [Auditable(false)]
        public int MaxDispatchLatencyXe
        {
            get { return maxDispatchLatencyXE; }
            set { maxDispatchLatencyXE = value; }
        }

        /// <summary>
        /// Sets the maximum size in memory for events. This option should be used to allow single events that are larger than the MAX_MEMORY setting. 
        /// Allowable sizes are in either kilobytes (KB) or megabytes (MB). 
        /// If a size in KB is specified, then the minimum value is 64KB.
        /// </summary>
        [Auditable(false)]
        public int MaxEventSizeXemb
        {
            get { return maxEventSizeXEMB; }
            set { maxEventSizeXEMB = value; }
        }
        /// <summary>
        /// Specifies how buffers are created and partitioned. There are three valid values for this option:
        /// NONE
        /// A single set of buffers is created within the SQL Server instance.
        /// PER_NODE
        /// A set of buffers is created for each NUMA node.
        /// PER_CPU
        /// A set of buffers is created for each CPU.
        /// This option can be used on larger servers to partition the buffers for better performance.
        /// </summary>
        public XEMemoryPartitionMode MemoryPartitionModeXe
        {
            get { return memoryPartitionModeXE; }
            set { memoryPartitionModeXE = value; }
        }

        /// <summary>
        /// Allows related events on different SPIDs to be correlated together. Valid values are ON and OFF.
        /// </summary>
        [Auditable(false)]
        public bool TrackCausalityXe
        {
            get { return trackCausalityXE; }
            set { trackCausalityXE = value; }
        }
        /// <summary>
        /// Specifies whether or not the event session starts automatically when the instance of SQL Server starts up. Valid values are ON and OFF
        /// </summary>
        [Auditable(false)]
        public bool StartupStateXe
        {
            get { return startupStateXE; }
            set { startupStateXE = value; }
        }
        /// <summary>
        /// Name of the XE session output file
        /// </summary>
        [Auditable(false)]
        public string FileNameXEsession
        {
            get { return fileNameXEsession; }
            set { fileNameXEsession = value; }
        }
        
        //END SQLdm 9.1 (Ankit Srivastava): Activity Monitoring with Extended Events - Added new properties

        [Auditable("Enabled capturing of deadlock events")]
        public bool DeadlockEventsEnabled
        {
            get { return deadlockEventsEnabled; }
        }

        [Auditable("Enabled capturing of Blocking events")]
        public bool BlockingEventsEnabled
        {
            get { return blockingEventsEnabled; }
            set { blockingEventsEnabled = value; }
        }

        [Auditable("Enabled capturing of Autogrow events")]
        public bool AutoGrowEventsEnabled
        {
            get { return autogrowEventsEnabled; }
        }

        [AuditableAttribute(false)]
        public int BlockedProcessThreshold
        {
            get { return blockedProcessThreshold; }
            set { blockedProcessThreshold = value; }
        }

        public FileSize TraceFileSize
        {
            get { return traceFileSize; }
        }

        public int TraceFileRollovers
        {
            get { return traceFileRollovers; }
        }

        public int RecordsPerRefresh
        {
            get { return recordsPerRefresh; }
        }

        [Auditable(false)]
        public AdvancedQueryMonitorConfiguration AdvancedConfiguration
        {
            get
            {
                if (advancedConfiguration == null)
                {
                    advancedConfiguration = new AdvancedQueryMonitorConfiguration();
                }

                return advancedConfiguration;
            }
        }

        public DateTime? StopTimeUTC
        {
            get { return stopTimeUTC; }
            set { stopTimeUTC = value; }
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        public override bool Equals(object obj)
        {
            if (this == obj) return true;
            ActivityMonitorConfiguration activityProfilerConfiguration = obj as ActivityMonitorConfiguration;
            if (activityProfilerConfiguration == null) return false;
            if (!Equals(enabled, activityProfilerConfiguration.enabled)) return false;
            if (!Equals(deadlockEventsEnabled, activityProfilerConfiguration.deadlockEventsEnabled)) return false;
            if (!Equals(autogrowEventsEnabled, activityProfilerConfiguration.autogrowEventsEnabled)) return false;
            if (!Equals(blockingEventsEnabled, activityProfilerConfiguration.blockingEventsEnabled)) return false;
            if (!Equals(traceFileSize, activityProfilerConfiguration.traceFileSize)) return false;
            if (traceFileRollovers != activityProfilerConfiguration.traceFileRollovers) return false;
            if (recordsPerRefresh != activityProfilerConfiguration.recordsPerRefresh) return false;
            if (!Equals(advancedConfiguration,activityProfilerConfiguration.advancedConfiguration)) return false;
            if (!Equals(stopTimeUTC, stopTimeUTC)) return false;
            return true;
        }

        #region Formatting Methods

        // These properties and methods are used to return string data for display
        [Auditable(false)]
        public string EnabledString
        {
           get
            {
                return enabled ? "Enabled" : "Disabled";
            } 
        }

        [Auditable(false)]
        public string CollectionString
        {
            get
            {
                StringBuilder sb = new StringBuilder();
                
                sb.Append(deadlockEventsEnabled ? "Deadlocks, " : "");
                sb.Append(blockingEventsEnabled ? "Blocking, " : "");
                sb.Append(autogrowEventsEnabled ? "Autogrow, " : "");

                if (sb.Length > 2)
                    sb.Remove(sb.Length - 2, 2);

                return sb.ToString();
            }
        }

        //[Auditable(false)]
        //public string FilterTypeString
        //{
        //    get
        //    {
        //        StringBuilder sb = new StringBuilder();

        //        sb.Append(durationFilter.TotalMilliseconds > 0 ? durationFilter.TotalMilliseconds + "ms duration, " : "");
        //        sb.Append(cpuUsageFilter.TotalMilliseconds > 0 ? cpuUsageFilter.TotalMilliseconds + "ms cpu, " : "");
        //        sb.Append(logicalDiskReads > 0 ? logicalDiskReads + "reads, " : "");
        //        sb.Append(physicalDiskWrites > 0 ? physicalDiskWrites + "writes, " : "");
                
        //        if (sb.Length > 2)
        //            sb.Remove(sb.Length - 2, 2);

        //        return sb.ToString();
        //    }
        //}

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append(EnabledString);
            if (enabled)
            {
                sb.Append(" - ");
                sb.Append("Collection: ");
                sb.Append(CollectionString);

                sb.Append(" Filters: ");

                string filterString = "";// FilterTypeString;

                if (filterString.Length > 0)
                {
                    sb.Append(filterString);
                }
                else
                {
                    sb.Append("(none)");
                }

            }

            return sb.ToString();
        }

        #endregion

      
        }



    
}
