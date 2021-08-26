//------------------------------------------------------------------------------
// <copyright file="ServerOverviewConfiguration.cs" company="Idera, Inc.">
//     Copyright (c) Idera, Inc. All rights reserved.
// </copyright>
//------------------------------------------------------------------------------
using System.Collections.Generic;
using Idera.SQLdm.Common.Snapshots;

namespace Idera.SQLdm.Common.Configuration
{
    using System;
    using Idera.SQLdm.Common.VMware;

    /// <summary>
    /// Configuration object for server overview on-demand probe
    /// </summary>
    [Serializable]
    public class ServerOverviewConfiguration : OnDemandConfiguration
    {
        #region fields

        private OSMetrics previousOSMetrics;
        private ServerStatistics previousServerStatistics;
        private VMwareVirtualMachine previousVconfig;
        private DateTime? lastRefresh = null;
        private DateTime? serverStartupTime = null;
        private LockStatistics previousLockStatistics = null;
        private Dictionary<string, DiskDrive> previousDiskDrives = null;
        private Dictionary<string, DatabaseStatistics> previousDbStatistics = null;
        private Dictionary<string, FileActivityFile> previousFileActivity = null;
        private WaitStatisticsSnapshot previousWaitStatistics = null;
        private TempdbSummaryStatistics previousTempdbStatistics = null;
        private CustomCounterCollectionSnapshot previousCustomCounters = null;

        private List<CustomCounterConfiguration> customCounterConfigurations = null;

        #endregion

        #region constructors

        public ServerOverviewConfiguration(int monitoredServerId)
            : this(monitoredServerId, new OSMetrics(), new ServerStatistics(), null, null, null, null, null, null, null, null,null)
        {
        }

        public ServerOverviewConfiguration(int monitoredServerId, ServerSummarySnapshots previousSnapshots)
            : this(monitoredServerId, previousSnapshots.ServerOverviewSnapshot)
        {
            previousCustomCounters = previousSnapshots.CustomCounterCollectionSnapshot;
        }

        public ServerOverviewConfiguration(int monitoredServerId, ServerOverview previousServerOverview)
            : this(monitoredServerId,
            previousServerOverview != null ? previousServerOverview.OSMetricsStatistics : new OSMetrics(),
            previousServerOverview != null ? previousServerOverview.Statistics : new ServerStatistics(),
            previousServerOverview != null ? previousServerOverview.TimeStamp : null,
            previousServerOverview != null ? previousServerOverview.ServerStartupTime : null,
            previousServerOverview != null ? previousServerOverview.LockCounters : null,
            previousServerOverview != null ? previousServerOverview.DiskDrives : null,
            previousServerOverview != null ? previousServerOverview.DbStatistics : null,
            previousServerOverview != null ? previousServerOverview.FileActivity : null,
            previousServerOverview != null ? previousServerOverview.TempdbStatistics : null,
            previousServerOverview != null ? previousServerOverview.WaitStats : null,
            previousServerOverview !=  null ?  previousServerOverview.VMConfig : null)
        {

        }

        public ServerOverviewConfiguration(int monitoredServerId,OSMetrics previousOSMetricsSnapshot, 
            ServerStatistics previousServerStatisticsSnapshot, DateTime? lastRefresh,
            DateTime? serverStartupTime, LockStatistics previousLockStatistics, Dictionary<string, DiskDrive> previousDiskDrives, Dictionary<string, DatabaseStatistics> previousDbStatistics, Dictionary<string, FileActivityFile> previousFileActivity, TempdbSummaryStatistics previousTempdbStatistics, WaitStatisticsSnapshot previousWaitStatistics,VMwareVirtualMachine previousVconfig)
            : base(monitoredServerId)
        {
            previousOSMetrics = previousOSMetricsSnapshot;
            previousServerStatistics = previousServerStatisticsSnapshot;
            this.lastRefresh = lastRefresh;
            this.serverStartupTime = serverStartupTime;
            this.previousLockStatistics = previousLockStatistics;
            this.previousDiskDrives = previousDiskDrives;
            this.previousDbStatistics = previousDbStatistics;
            this.previousFileActivity = previousFileActivity;
            this.previousTempdbStatistics = previousTempdbStatistics;
            this.previousWaitStatistics = previousWaitStatistics;
            this.previousVconfig = previousVconfig;
        }
        
        #endregion

        #region properties


        public OSMetrics PreviousOSMetrics
        {
            get { return previousOSMetrics; }
            set { previousOSMetrics = value; }
        }

        public VMwareVirtualMachine PreviousVconfig
        {
            get { return previousVconfig;}
            set { previousVconfig =  value;}
        }

        public ServerStatistics PreviousServerStatistics
        {
            get { return previousServerStatistics; }
            set { previousServerStatistics = value; }
        }

        public LockStatistics PreviousLockStatistics
        {
            get { return previousLockStatistics; }
            set { previousLockStatistics = value; }
        }


        public DateTime? LastRefresh
        {
            get { return lastRefresh; }
            set { lastRefresh = value; }
        }


        public DateTime? ServerStartupTime
        {
            get { return serverStartupTime; }
            set { serverStartupTime = value; }
        }

        public Dictionary<string, DiskDrive> PreviousDiskDrives
        {
            get { return previousDiskDrives; }
            set { previousDiskDrives = value; }
        }

        public Dictionary<string, DatabaseStatistics> PreviousDbStatistics
        {
            get { return previousDbStatistics; }
            set { previousDbStatistics = value; }
        }

        public Dictionary<string, FileActivityFile> PreviousFileActivity
        {
            get { return previousFileActivity; }
            set { previousFileActivity = value; }
        }

        public TempdbSummaryStatistics PreviousTempdbStatistics
        {
            get { return previousTempdbStatistics; }
            set { previousTempdbStatistics = value; }
        }

        public WaitStatisticsSnapshot PreviousWaitStatistics
        {
            get { return previousWaitStatistics; }
            set { previousWaitStatistics = value; }
        }

        public List<CustomCounterConfiguration> CustomCounterConfigurations
        {
            get { return customCounterConfigurations; }
            set { customCounterConfigurations = value; }
        }

        public CustomCounterCollectionSnapshot PreviousCustomCounters
        {
            get { return previousCustomCounters; }
            set { previousCustomCounters = value; }
        }

        #endregion

        #region methods

        #endregion


    }
}
