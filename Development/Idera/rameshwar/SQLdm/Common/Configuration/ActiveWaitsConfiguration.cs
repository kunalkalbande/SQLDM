//------------------------------------------------------------------------------
// <copyright file="ActiveWaitsConfiguration.cs" company="Idera, Inc.">
//     Copyright (c) Idera, Inc. All rights reserved.
// </copyright>
//------------------------------------------------------------------------------
using System.Collections.Generic;
using Idera.SQLdm.Common.Attributes;

namespace Idera.SQLdm.Common.Configuration
{
    using System;

    public interface IContinuousConfiguration
    {
        bool IsMaster
        {
            get;
            set;
        }

        [AuditableAttribute(false)]
        Guid ClientSessionId
        { 
            get;
        }

        int CollectionTimeSeconds
        {
            get; set;
        }

        bool ReadyForCollection
        {
            get;
        }

        [AuditableAttribute(false)]
        DateTime StartTimeUTC
        {
            get;
            set;
        }


        TimeSpan? RunTime
        {
            get;
            set;
        }


        bool InCollectionWindow
        {
            get;
        }


        [Auditable(false)]
        bool InPickupWindow
        {
            get;
        }

        TimeSpan WaitForPickupTime
        { 
            get;
        }

        IContinuousConfiguration CombineConfiguration(List<IContinuousConfiguration> configurations);
    }

// Supress the warning regarding overrides
#pragma warning disable 0659
    /// <summary>
    /// Configuration object for Active Waits probe
    /// </summary>
    [Serializable]
    public class ActiveWaitsConfiguration : OnDemandConfiguration, IContinuousConfiguration
    {
        #region fields

        public const string FileNameXEsessionDefault = "dm7XEWaits.xel";
        
        // SQLDM-29633 (Varun) Query Monitor default option
        public const bool EnabledQsDefault = false;

        public const bool EnabledXeDefault = true;
        public const int FileSizeXeMBDefault = 5; // SQLdm 9.0 (Ankit Srivastava ) Changed the default file XE size 
        public const int FileSizeRolloverXeDefault = 3;
        public const int RecordsPerRefreshXeDefault = 1000;
        public const int MaxMemoryXeMBDefault = 1;
        public const XeEventRetentionMode EventRetentionModeXeDefault = XeEventRetentionMode.ALLOW_MULTIPLE_EVENT_LOSS;
        public const int MaxDispatchLatencyXeDefault = 300;
        public const int MaxEventSizeXembDefault = 1;
        public const XEMemoryPartitionMode MemoryPartitionModeXeDefault = XEMemoryPartitionMode.NONE;
        public const bool TrackCausalityXeDefault = false;
        public const bool StartupStateXeDefault = false;


        #endregion

        private int collectionTimeSeconds = 30;
        private DateTime startTimeUTC = DateTime.MinValue;
        private TimeSpan? runTime = TimeSpan.FromMinutes(3);
        private TimeSpan waitForPickupTime = TimeSpan.FromSeconds(30);
        private DateTime startTimeRelative = DateTime.MinValue;
        private bool enabled = false;
        private int loopTimeMilliseconds = 500;
        private AdvancedQueryFilterConfiguration advancedConfiguration = new AdvancedQueryFilterConfiguration();
        private bool enabledXE = EnabledXeDefault;
        private bool enabledQs = EnabledQsDefault;
        private int fileSizeXEMB = FileSizeXeMBDefault;
        private int fileSizeRolloverXE = FileSizeRolloverXeDefault;
        private int recordsPerRefreshXE = RecordsPerRefreshXeDefault;
        private int maxMemoryXEMB = MaxMemoryXeMBDefault;
        private XeEventRetentionMode eventRetentionModeXE= EventRetentionModeXeDefault;
        private int maxDispatchLatencyXE = MaxDispatchLatencyXeDefault;
        private int maxEventSizeXEMB = MaxEventSizeXembDefault;
        private XEMemoryPartitionMode memoryPartitionModeXE = XEMemoryPartitionMode.NONE;
        private bool trackCausalityXE = TrackCausalityXeDefault;
        private bool startupStateXE = StartupStateXeDefault;
        private string fileNameXEsession = FileNameXEsessionDefault;
        private ActiveWaitsConfiguration prevConfiguration;
        private bool activeWaitsConfigChangeResponse = false;
        private bool isMaster = false;
        #region constructors

        public ActiveWaitsConfiguration(int monitoredServerId)
            : base(monitoredServerId)
        {
        }


        public ActiveWaitsConfiguration(int monitoredServerId, ActiveWaitsConfiguration oldConfiguration, ActiveWaitsConfiguration newConfiguration)
            : base(monitoredServerId)
        {
            if (oldConfiguration != null)
            {
                ClientSessionId = oldConfiguration.ClientSessionId;
                prevConfiguration = oldConfiguration;
            }

            if (newConfiguration != null)
            {
                this.collectionTimeSeconds = newConfiguration.CollectionTimeSeconds;
                this.startTimeUTC = newConfiguration.StartTimeUTC;
                this.runTime = newConfiguration.RunTime;
                this.waitForPickupTime = newConfiguration.WaitForPickupTime;
                this.startTimeRelative = newConfiguration.StartTimeRelative;
                this.enabled = newConfiguration.Enabled;
                this.loopTimeMilliseconds = newConfiguration.LoopTimeMilliseconds;
                this.advancedConfiguration = newConfiguration.AdvancedConfiguration;
                this.IsMaster= newConfiguration.IsMaster;

                this.enabledXE = newConfiguration.enabledXE;
                this.enabledQs = newConfiguration.enabledQs;
                this.fileSizeXEMB = newConfiguration.fileSizeXEMB;
                this.fileSizeRolloverXE = newConfiguration.fileSizeRolloverXE ;
                this.recordsPerRefreshXE = newConfiguration.recordsPerRefreshXE ;
                this.maxMemoryXEMB = newConfiguration.maxMemoryXEMB;
                this.eventRetentionModeXE= newConfiguration.eventRetentionModeXE;
                this.maxDispatchLatencyXE = newConfiguration.maxDispatchLatencyXE ;
                this.maxEventSizeXEMB = newConfiguration.maxEventSizeXEMB;
                this.memoryPartitionModeXE = newConfiguration.memoryPartitionModeXE;
                this.trackCausalityXE = newConfiguration.trackCausalityXE ;
                this.startupStateXE = newConfiguration.startupStateXE ;
                this.fileNameXEsession = newConfiguration.fileNameXEsession;
            }
        }

       #endregion

        #region properties

        public ActiveWaitsConfiguration PreviousConfiguration
        {
            get { return prevConfiguration; }
            set { prevConfiguration = value; }
        }

        public bool ActiveWaitsConfigChangeResponse
        {
            get { return activeWaitsConfigChangeResponse; }
            set { activeWaitsConfigChangeResponse = value; }
        }

        /// <summary>
        /// Enable SQL2012 active waits to be monitored using extended events
        /// </summary>
        [AuditableAttribute(@"Enabled use of extended events")]
        public bool EnabledXe
        {
            get { return enabledXE; }
            set { enabledXE = value; }
        }
        /// <summary>
        /// Enable SQL2016 active waits to be monitored using query store
        /// </summary>
        [AuditableAttribute(@"Enabled use of query store")]
        public bool EnabledQs
        {
            get { return enabledQs; }
            set { enabledQs = value; }
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
        
        /// <summary>
        /// Contains all of the advanced setting like exclusion filters
        /// </summary>
        public AdvancedQueryFilterConfiguration AdvancedConfiguration
        {
            get { return advancedConfiguration; }
            set { advancedConfiguration = value; }
        }
        /// <summary>
        /// Enable Query wait monitoring
        /// </summary>
        [Auditable("Enabled collection of query wait statistics")]
        public bool Enabled
        {
            get { return enabled; }
            set { enabled = value; }
        }

        /// <summary>
        /// Indicates that this is the configuration object that should take preference in all combines
        /// </summary>
        public bool IsMaster
        {
            get { return isMaster; }
            set { isMaster = value; }
        }

        /// <summary>
        /// The length of the collector before it returns data to the collection service
        /// </summary>
        public int CollectionTimeSeconds
        {
            get { return collectionTimeSeconds; }
            set { collectionTimeSeconds = value; }
        }

        /// <summary>
        /// If runtime has no value then we are alway ready for collection
        /// </summary>
        [Auditable(false)]
        new public bool ReadyForCollection
        {
            get
            {
                return (enabled && 
                    collectionTimeSeconds > 0
                    &&
                    (!RunTime.HasValue || InCollectionWindow )
                    );
            }
        }
        
        /// <summary>
        /// Tries to keep the data for duration of scheduled refresh to prevent it being lost when monitoring stops
        /// </summary>
        [Auditable(false)]
        public bool InPickupWindow
        {
            get
            {
                if (ReadyForCollection)
                    return true;

                if (StopTimeUTC < DateTime.Now.ToUniversalTime())
                {
                    if (TimeElapsedSinceShutdown.HasValue && TimeElapsedSinceShutdown <= waitForPickupTime)
                    {
                        return true;
                    }
                }
                return false;
            }
        }

        /// <summary>
        /// The time to start the collector
        /// </summary>
        [AuditableAttribute(false)]
        public DateTime StartTimeUTC
        {
            get { return startTimeUTC; }
            set { startTimeUTC = value; }
        }

        /// <summary>
        /// The server time to start the collector
        /// </summary>
        [AuditableAttribute("Enabled collection of query wait statistics at a specified time and duration")]
        public DateTime StartTimeRelative
        {
            get { return startTimeRelative; }

            set { startTimeRelative = value; }
        }

        /// <summary>
        /// The total length of time to run the collector
        /// Leave null for unlimited
        /// </summary>
        [AuditableAttribute("Collect query wait statistics duration")]
        public TimeSpan? RunTime
        {
            get { return runTime; }
            set { runTime = value; }
        }

        [AuditableAttribute(false)]
        public DateTime? StopTimeUTC
        {
            get
            {
                if (RunTime.HasValue)
                {
                    return StartTimeUTC + RunTime;
                }
                else
                {
                    return null;
                }
            }
        }

        /// <summary>
        /// Returns whether the current time is within the collector window
        /// </summary>
        [Auditable(false)]
        public bool InCollectionWindow
        {
            get
            {
                if (
                    startTimeUTC <= DateTime.Now.ToUniversalTime() 
                    && ((!RunTime.HasValue) || (startTimeUTC.Add(RunTime.Value) >= DateTime.Now.ToUniversalTime()))
                    )
                    return true;
                else
                    return false;
            }
        }

        /// <summary>
        /// Time remaining until scheduled collector shutdown
        /// </summary>
        [AuditableAttribute(false)]
        public TimeSpan? TimeElapsedSinceShutdown
        {
            get
            {
                if (StopTimeUTC.HasValue)
                    return DateTime.Now.ToUniversalTime() - StopTimeUTC.Value;
                else
                    return null;
            }
        }

        public TimeSpan WaitForPickupTime
        {
            get { return waitForPickupTime; }
            set { waitForPickupTime = value; }
        }

        public int LoopTimeMilliseconds
        {
            get { return loopTimeMilliseconds; }
            set { loopTimeMilliseconds = value; }
        }

        #endregion

        #region events

        #endregion

        #region methods

        public void UpdateStartTime(DateTime localtime, DateTime utctime)
        {
            if (StartTimeRelative > DateTime.MinValue)
            {
                startTimeUTC = StartTimeRelative + (utctime - localtime);
            }
            else
            {
                startTimeUTC = DateTime.MinValue;
            }
        }

        #endregion

        #region interface implementations

        public override bool Equals(object obj)
        {
            if (this == obj) return true;
            ActiveWaitsConfiguration activeWaitsConfiguration = obj as ActiveWaitsConfiguration;
            if (activeWaitsConfiguration == null) return false;
            if (collectionTimeSeconds != activeWaitsConfiguration.collectionTimeSeconds) return false;
            if (!Equals(startTimeUTC, activeWaitsConfiguration.startTimeUTC)) return false;
            if (!Equals(isMaster, activeWaitsConfiguration.isMaster)) return false;
            if (!Equals(runTime, activeWaitsConfiguration.runTime)) return false;
            if (!Equals(advancedConfiguration, activeWaitsConfiguration.advancedConfiguration)) return false;
            return true;
        }

        public IContinuousConfiguration CombineConfiguration(List<IContinuousConfiguration> configurations)
        {
            ActiveWaitsConfiguration returnConfig = null;

            foreach (ActiveWaitsConfiguration config in configurations)
            {
                if (returnConfig == null)
                {
                    returnConfig = config;
                }
                else
                {
                    if (config.ReadyForCollection)
                    {
                        if (config.CollectionTimeSeconds <
                            returnConfig.CollectionTimeSeconds)
                        {
                            returnConfig.CollectionTimeSeconds =
                                config.CollectionTimeSeconds;
                        }
                        //The idea here is that we can speed up the collection if viewing real-time
                        if (returnConfig.RunTime.HasValue && (!config.RunTime.HasValue || config.RunTime > returnConfig.RunTime))
                        {
                            returnConfig.RunTime = config.RunTime;
                        }

                        if (config.LoopTimeMilliseconds < returnConfig.LoopTimeMilliseconds)
                        {
                            returnConfig.LoopTimeMilliseconds = config.loopTimeMilliseconds;
                        }

                        if (config.EnabledQs || returnConfig.EnabledQs)
                        {
                            returnConfig.EnabledQs = true;
                        }

                        if (config.EnabledXe || returnConfig.EnabledXe)
                        {
                            returnConfig.EnabledXe = true;
                        }

                        //this config has been flagged as containing advanced config workload changes
                        if (config.IsMaster)
                        {
                            //return these adbanced settings
                            returnConfig.AdvancedConfiguration = config.AdvancedConfiguration;
                            //set all the othet configs to have the same advanced filter criteria since these are the latest
                            foreach (ActiveWaitsConfiguration otherConfig in configurations)
                            {
                                if(!otherConfig.advancedConfiguration.Equals(config.AdvancedConfiguration))
                                {
                                    otherConfig.advancedConfiguration = config.advancedConfiguration;
                                }
                            }
                            config.isMaster = false;
                        }

                    }
                }
            }
            return returnConfig;
        }
      
        #endregion

        #region nested types
        //REMOVED -SQLdm 9.1 (Ankit Srivastava) - Activity Monitoring with Extended Events - shifted enums out of the class to make them public
        #endregion

    }

    //START -SQLdm 9.1 (Ankit Srivastava) - Activity Monitoring with Extended Events - made the enums public
    /// <summary>
    /// Specifies the event retention mode to use for handling event loss. There are three possible values:
    /// </summary>
    public enum XeEventRetentionMode : int
    {
        ALLOW_SINGLE_EVENT_LOSS,
        ALLOW_MULTIPLE_EVENT_LOSS,
        NO_EVENT_LOSS
    }

    public enum XEMemoryPartitionMode : int
    {
        NONE,
        PER_NODE,
        PER_CPU
    }
    //END -SQLdm 9.1 (Ankit Srivastava) - Activity Monitoring with Extended Events - made the enums public
}
