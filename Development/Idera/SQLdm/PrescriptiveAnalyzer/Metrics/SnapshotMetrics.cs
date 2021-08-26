using System;
using System.Collections.Generic;
using System.Text;
using Idera.SQLdm.PrescriptiveAnalyzer.Common.Values;
using BBS.TracerX;

namespace Idera.SQLdm.PrescriptiveAnalyzer.Metrics
{
    public class SnapshotMetrics : BaseMetrics
    {
        public SnapshotValues Previous { get; set; }
        public SnapshotValues Current { get; private set; }
        private static Logger _logX = Logger.GetLogger("SnapshotMetrics");

        public string SQLVersionString { get; set; }

        public SampledServerResourcesMetrics SampledServerResourcesMetrics { get; set; }
        //public BlockingProcessMetrics BlockingProcessMetrics { get; set; }
        //public LongRunningJobsMetrics LongRunningJobsMetrics { get; set; }
        //public DeadlockMetrics DeadlockMetrics { get; set; }
        public DatabaseInfoSnapshotMetrics DatabaseInfoSnapshotMetrics { get; set; }
        public SnapshotMetricOptions Options { get; set; }

        public NUMANodeCountersMetrics NUMANodeCountersMetrics { get; set; }

        public WMINetworkRedirectorMetrics WMINetworkRedirectorMetrics { get; set; }
        public WMITCPMetrics WMITCPMetrics { get; set; }
        public WMITCPv4Metrics WMITCPv4Metrics { get; set; }
        public WMITCPv6Metrics WMITCPv6Metrics { get; set; }
        public WMINetworkInterfaceMetrics WMINetworkInterfaceMetrics { get; set; }
        public WMIPerfOSProcessorMetrics WMIPerfOSProcessorMetrics { get; set; }
        public WMIPerfOSSystemMetrics WMIPerfOSSystemMetrics { get; set; }
        public WMIProcessorMetrics WMIProcessorMetrics { get; set; }
        public WMIProcessMetrics WMIProcessMetrics { get; set; }
        public WMIPerfDiskLogicalDiskMetrics WMIPerfDiskLogicalDiskMetrics { get; set; }
        public WMIPerfDiskPhysicalDiskMetrics WMIPerfDiskPhysicalDiskMetrics { get; set; }
        public WMIPageFileMetrics WMIPageFileMetrics { get; set; }
        public WMIEncryptableVolumeMetrics WMIEncryptableVolumeMetrics { get; set; }
        public WMIBiosMetrics WMIBiosMetrics { get; set; }
        public WMIPerfOSMemoryMetrics WMIPerfOSMemoryMetrics { get; set; }
        public WMIServiceMetrics WMIServiceMetrics { get; set; }
        public WMIComputerSystemMetrics WMIComputerSystemMetrics { get; set; }
        public WMIPhysicalMemoryMetrics WMIPhysicalMemoryMetrics { get; set; }
        public WMIVolumeMetrics WMIVolumeMetrics { get; set; }

        public ServerPropertiesMetrics ServerPropertiesMetrics { get; set; }
        public ServerConfigurationMetrics ServerConfigurationMetrics { get; set; }

        //public TransactionMetrics TransactionMetrics { get; set; }

        public WaitStatsMetrics WaitStatsMetrics { get; set; }
        public WaitingBatchesMetrics WaitingBatchesMetrics { get; set; }
        public WorstIndexFillFactorMetrics WorstIndexFillFactorMetrics { get; set; }


        public BackupAndRecoveryMetrics BackupAndRecoveryMetrics { get; set; }
        public DBSecurityMetrics DBSecurityMetrics { get; set; }
        public DisabledIndexMetrics DisabledIndexMetrics { get; set; }
        public FragmentedIndexesMetrics FragmentedIndexesMetrics { get; set; }
        public HighIndexUpdatesMetrics HighIndexUpdatesMetrics { get; set; }
        public OutOfDateStatsMetrics OutOfDateStatsMetrics { get; set; }
        public SQLModuleOptionsMetrics SQLModuleOptionsMetrics { get; set; }
        public OverlappingIndexesMetrics OverlappingIndexesMetrics { get; set; }
        public IndexContentionMetrics IndexContentionMetrics { get; set; }
        public DBPropertiesMetrics DBPropertiesMetrics { get; set; }
        //SQLDm 10.0  - Srishti Purohit - New Recommendations - SDR-I23 Adding new metrics
        public NonIncrementalColStatMetrics NonIncrementalColStatsMetrics { get; set; }
        public HashIndexMetrics HashIndexMetrics { get; set; }
        public QueryStoreMetrics QueryStoreMetrics { get; set; }
        //SQLDm 10.0  - Srishti Purohit - New Recommendations - SDR-I29 Adding new metrics
        public RarelyUsedIndexOnInMemoryTableMetrics RarelyUsedIndexOnInMemoryTableMetrics { get; set; }
        //SQLDm 10.0  - Srishti Purohit - New Recommendations - SDR-Q46,Q47,Q48,Q49,Q50 Adding new metrics
        public QueryAnalyzerMetrics QueryAnalyzerMetrics { get; set; }
        //SQLDm 10.0  - Srishti Purohit - New Recommendations - SDR-I30 Adding new metrics
        public ColumnStoreIndexMetrics ColumnStoreIndexMetrics { get; set; }
        //SQLDm 10.0  - Srishti Purohit - New Recommendations - SDR-I31 Adding new metrics
        public FilteredColumnNotInKeyOfFilteredIndexMetrics FilteredColumnNotInKeyOfFilteredIndexMetrics { get; set; }
        //SQLDm 10.0  - Srishti Purohit - New Recommendations - SDR-Q43 Adding new metrics
        public HighCPUTimeProcedureMetrics HighCPUTimeProcedureMetrics { get; set; }
        //SQLDm 10.0  - Srishti Purohit - New Recommendations - SDR-I24 Adding new metrics
        public LargeTableStatsMetrics LargeTableStatsMetrics { get; set; }
        //SQLDm 10.0  - Srishti Purohit - New Recommendations - SDR-M33 Adding new metrics
        public BufferPoolExtIOMetrics BufferPoolExtIOMetrics { get; set; }

        public void AddSnapshot(Idera.SQLdm.Common.Snapshots.PrescriptiveAnalyticsSnapshot snapshot, SnapshotMetricOptions options)
        {
            if (snapshot == null) { return; }

            this.Options = options;

            SampledServerResourcesMetrics = new  SampledServerResourcesMetrics ();
            SampledServerResourcesMetrics.AddSnapshot(snapshot);

            DatabaseInfoSnapshotMetrics = new  DatabaseInfoSnapshotMetrics();
            DatabaseInfoSnapshotMetrics.AddSnapshot(snapshot);

            NUMANodeCountersMetrics = new  NUMANodeCountersMetrics();
            NUMANodeCountersMetrics.AddSnapshot(snapshot);

            WMINetworkRedirectorMetrics = new  WMINetworkRedirectorMetrics();
            WMINetworkRedirectorMetrics.AddSnapshot(snapshot);

            WMITCPMetrics = new  WMITCPMetrics();
            WMITCPMetrics.AddSnapshot(snapshot);

            WMITCPv4Metrics = new  WMITCPv4Metrics();
            WMITCPv4Metrics.AddSnapshot(snapshot);

            WMITCPv6Metrics = new  WMITCPv6Metrics();
            WMITCPv6Metrics.AddSnapshot(snapshot);

            WMINetworkInterfaceMetrics = new  WMINetworkInterfaceMetrics ();
            WMINetworkInterfaceMetrics.AddSnapshot(snapshot);

            WMIPerfOSProcessorMetrics = new  WMIPerfOSProcessorMetrics();
            WMIPerfOSProcessorMetrics.AddSnapshot(snapshot);

            WMIPerfOSSystemMetrics = new  WMIPerfOSSystemMetrics();
            WMIPerfOSSystemMetrics.AddSnapshot(snapshot);

            WMIProcessorMetrics = new  WMIProcessorMetrics();
            WMIProcessorMetrics.AddSnapshot(snapshot);

            WMIProcessMetrics = new  WMIProcessMetrics();
            WMIProcessMetrics.AddSnapshot(snapshot);

            WMIPerfDiskLogicalDiskMetrics = new  WMIPerfDiskLogicalDiskMetrics();
            WMIPerfDiskLogicalDiskMetrics.AddSnapshot(snapshot);

            WMIPerfDiskPhysicalDiskMetrics = new  WMIPerfDiskPhysicalDiskMetrics ();
            WMIPerfDiskPhysicalDiskMetrics.AddSnapshot(snapshot);

            WMIPageFileMetrics = new  WMIPageFileMetrics();
            WMIPageFileMetrics.AddSnapshot(snapshot);

            WMIEncryptableVolumeMetrics = new  WMIEncryptableVolumeMetrics();
            WMIEncryptableVolumeMetrics.AddSnapshot(snapshot);

            WMIBiosMetrics = new  WMIBiosMetrics();
           // WMIBiosMetrics.AddSnapshot(snapshot);//To be add snapshot

            WMIPerfOSMemoryMetrics = new  WMIPerfOSMemoryMetrics();
            WMIPerfOSMemoryMetrics.AddSnapshot(snapshot);

            WMIServiceMetrics = new  WMIServiceMetrics();
            WMIServiceMetrics.AddSnapshot(snapshot);

            WMIComputerSystemMetrics = new  WMIComputerSystemMetrics();
            WMIComputerSystemMetrics.AddSnapshot(snapshot);

            WMIPhysicalMemoryMetrics = new  WMIPhysicalMemoryMetrics();
            WMIPhysicalMemoryMetrics.AddSnapshot(snapshot);

            WMIVolumeMetrics = new  WMIVolumeMetrics();
            WMIVolumeMetrics.AddSnapshot(snapshot);

            ServerPropertiesMetrics = new  ServerPropertiesMetrics();
            ServerPropertiesMetrics.AddSnapshot(snapshot);

            WaitStatsMetrics = new  WaitStatsMetrics();
            WaitStatsMetrics.AddSnapshot(snapshot);

            WaitingBatchesMetrics = new  WaitingBatchesMetrics();
            WaitingBatchesMetrics.AddSnapshot(snapshot);

            WorstIndexFillFactorMetrics = new  WorstIndexFillFactorMetrics();
            WorstIndexFillFactorMetrics.AddSnapshot(snapshot);

            //Older
            BackupAndRecoveryMetrics = new BackupAndRecoveryMetrics();
            BackupAndRecoveryMetrics.AddSnapshot(snapshot);

            DBPropertiesMetrics = new DBPropertiesMetrics();
            DBPropertiesMetrics.AddSnapshot(snapshot);

            DBSecurityMetrics = new DBSecurityMetrics();
            DBSecurityMetrics.AddSnapshot(snapshot);

            FragmentedIndexesMetrics = new FragmentedIndexesMetrics();
            FragmentedIndexesMetrics.AddSnapshot(snapshot);

            DisabledIndexMetrics = new DisabledIndexMetrics();
            DisabledIndexMetrics.AddSnapshot(snapshot);

            HighIndexUpdatesMetrics = new HighIndexUpdatesMetrics();
            HighIndexUpdatesMetrics.AddSnapshot(snapshot);

            IndexContentionMetrics = new IndexContentionMetrics();
            IndexContentionMetrics.AddSnapshot(snapshot);

            OutOfDateStatsMetrics = new OutOfDateStatsMetrics();
            OutOfDateStatsMetrics.AddSnapshot(snapshot);

            OverlappingIndexesMetrics = new OverlappingIndexesMetrics();
            OverlappingIndexesMetrics.AddSnapshot(snapshot);

            SQLModuleOptionsMetrics = new SQLModuleOptionsMetrics();
            SQLModuleOptionsMetrics.AddSnapshot(snapshot);

            ServerConfigurationMetrics = new ServerConfigurationMetrics();
            ServerConfigurationMetrics.AddSnapshot(snapshot);
            //SQLdm 10.0 - Srishti Puohit - New Recommendations - SDR-I23
            NonIncrementalColStatsMetrics = new NonIncrementalColStatMetrics();
            NonIncrementalColStatsMetrics.AddSnapshot(snapshot);
            //SQLdm 10.0 - Srishti Puohit - New Recommendations - SDR-I25, 26, 28           
            HashIndexMetrics = new HashIndexMetrics();
            HashIndexMetrics.AddSnapshot(snapshot);
            //SQLdm 10.0 - Srishti Puohit - New Recommendations - SDR-Q39, SDR-Q40, SDR-Q41,SDR-Q42         
            QueryStoreMetrics = new QueryStoreMetrics();
            QueryStoreMetrics.AddSnapshot(snapshot);
            //SQLdm 10.0 - Srishti Puohit - New Recommendations - SDR-I29
            RarelyUsedIndexOnInMemoryTableMetrics = new RarelyUsedIndexOnInMemoryTableMetrics();
            RarelyUsedIndexOnInMemoryTableMetrics.AddSnapshot(snapshot);
            //SQLdm 10.0 - Srishti Puohit - New Recommendations - SDR-Q46,Q47,Q48,Q49,Q50
            QueryAnalyzerMetrics = new QueryAnalyzerMetrics();
            QueryAnalyzerMetrics.AddSnapshot(snapshot);
            //SQLdm 10.0 - Srishti Puohit - New Recommendations - SDR-I30
            ColumnStoreIndexMetrics = new ColumnStoreIndexMetrics();
            ColumnStoreIndexMetrics.AddSnapshot(snapshot);
            //SQLdm 10.0 - Srishti Puohit - New Recommendations - SDR-I31
            FilteredColumnNotInKeyOfFilteredIndexMetrics = new FilteredColumnNotInKeyOfFilteredIndexMetrics();
            FilteredColumnNotInKeyOfFilteredIndexMetrics.AddSnapshot(snapshot);
            //SQLdm 10.0 - Srishti Puohit - New Recommendations - SDR-Q43
            HighCPUTimeProcedureMetrics = new HighCPUTimeProcedureMetrics();
            HighCPUTimeProcedureMetrics.AddSnapshot(snapshot);
            //SQLdm 10.0 - Srishti Puohit - New Recommendations - SDR-I24
            LargeTableStatsMetrics = new LargeTableStatsMetrics();
            LargeTableStatsMetrics.AddSnapshot(snapshot);
            //SQLdm 10.0 - Srishti Puohit - New Recommendations - SDR-M33
            BufferPoolExtIOMetrics = new BufferPoolExtIOMetrics();
            BufferPoolExtIOMetrics.AddSnapshot(snapshot);
            UpdateCurrentValues();
        }

        private void UpdateCurrentValues()
        {
            using (_logX.InfoCall("SnapshotMetrics.UpdateCurrentValues()"))
            {
                if (null == Current) Current = new SnapshotValues(Previous);
                Current.SQLVersionString = SQLVersionString;
                if (IsValid(WMINetworkInterfaceMetrics))
                {
                    Current.ActiveNetworkCards = WMINetworkInterfaceMetrics.ActiveNetworkCards;
                    Current.TotalNetworkBandwidth = WMINetworkInterfaceMetrics.TotalNetworkBandwidth;
                    _logX.InfoFormat("ActiveNetworkCards:{0}  TotalNetworkBandwidth:{1}", Current.ActiveNetworkCards, Current.TotalNetworkBandwidth);
                }
                if (IsValid(WMIPerfOSProcessorMetrics))
                {
                    UInt64 affinity = 0;
                    if (IsValid(ServerPropertiesMetrics)) affinity = ServerPropertiesMetrics.AffinityMask;
                    Current.AllowedProcessorCount = WMIPerfOSProcessorMetrics.GetProcessorCount(affinity);
                    _logX.InfoFormat("AllowedProcessorCount:{0}  Affinity:{1}", Current.AllowedProcessorCount, affinity);
                }
                if (IsValid(WMIProcessorMetrics))
                {
                    Current.TotalMaxClockSpeed = WMIProcessorMetrics.TotalMaxClockSpeed;
                    Current.TotalNumberOfLogicalProcessors = WMIProcessorMetrics.TotalNumberOfLogicalProcessors;
                    _logX.InfoFormat("TotalMaxClockSpeed:{0}  TotalNumberOfLogicalProcessors:{1}", Current.TotalMaxClockSpeed, Current.TotalNumberOfLogicalProcessors);
                }
                if (IsValid(ServerPropertiesMetrics))
                {
                    Current.TotalPhysicalMemory = ServerPropertiesMetrics.PhysicalMemory;
                    Current.MaxServerMemory = ServerPropertiesMetrics.MaxServerMemory;
                    _logX.InfoFormat("TotalPhysicalMemory:{0}  MaxServerMemory:{1}", Current.TotalPhysicalMemory, Current.MaxServerMemory);
                    Current.WindowsVersion = ServerPropertiesMetrics.WindowsVersion;
                    Current.ProductVersion = ServerPropertiesMetrics.ProductVersion;
                    _logX.InfoFormat("WindowsVersion:{0}  ProductVersion:{1}", Current.WindowsVersion, Current.ProductVersion);
                }
                if (IsValid(WMIPhysicalMemoryMetrics))
                {
                    if (WMIPhysicalMemoryMetrics.TotalCapacity > Current.TotalPhysicalMemory)
                        Current.TotalPhysicalMemory = WMIPhysicalMemoryMetrics.TotalCapacity;
                    _logX.InfoFormat("WMIPhysicalMemoryMetrics - TotalPhysicalMemory:{0}  Current:{1}", WMIPhysicalMemoryMetrics.TotalCapacity, Current.TotalPhysicalMemory);
                }
            }
        }
        private bool IsValid(BaseMetrics i)
        {
            using (_logX.DebugCall("SnapshotMetrics.IsValid()"))
            {
                if (null == i) { _logX.Debug("null ICollector"); return (false); }
                if (i.IsDataValid) return (true);
                _logX.DebugFormat("{0} is not valid. ", i.ToString());
                return (false);
            }
        }
    }
}
