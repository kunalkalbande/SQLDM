//------------------------------------------------------------------------------
// <copyright file="DBSecuritySnapshot.cs" company="Idera, Inc.">
//     Copyright (c) Idera, Inc. All rights reserved.
// </copyright>
//------------------------------------------------------------------------------
using System.Data;
using Idera.SQLdm.Common.Configuration;

namespace Idera.SQLdm.Common.Snapshots
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// Represents the DBSecurity info of a monitored server //SQLdm 10.0 (Praveen Suhalka) (Embedded SQLDoctor Analytics) -- new Snapshot class
    /// </summary>
    [Serializable]
    public sealed class PrescriptiveAnalyticsSnapshot : Snapshot
    {
        public PrescriptiveAnalyticsSnapshot()
        { }

        public PrescriptiveAnalyticsSnapshot(List<Snapshot> lstStartupSnapshots, List<Snapshot> lstIntervalSnapshots, List<Snapshot> lstShutdownSnapshots, List<Snapshot> lstDatabaseSnapshots)
        {
            #region dbsnapshots
            foreach (Snapshot snapshot in lstDatabaseSnapshots)
            {
                if (snapshot is DatabaseRankingSnapshot)
                {
                    this.databaseRankingSnapshotValue = (DatabaseRankingSnapshot)snapshot;
                    this.connectionString = ((DatabaseRankingSnapshot)snapshot).ConnectionString;
                }
                else if (snapshot is AdhocCachedPlanBytesSnapshot)
                {
                    this.adhocCachedPlanBytesSnapshotValue = (AdhocCachedPlanBytesSnapshot)snapshot;
                }
                else if (snapshot is LockedPageKBSnapshot)
                {
                    this.lockedPageKBSnapshotValue = (LockedPageKBSnapshot)snapshot;
                }
                else if (snapshot is QueryPlanEstRowsSnapshot)
                {
                    this.queryPlanEstRowsSnapshotValue = (QueryPlanEstRowsSnapshot)snapshot;
                }
                else if (snapshot is DatabaseConfigurationSnapshot)
                {
                    this.databaseConfigurationSnapshotValue = (DatabaseConfigurationSnapshot)snapshot;
                }
                else if (snapshot is BackupAndRecoverySnapshot)
                {
                    if (backupAndRecoverySnapshotValue == null) backupAndRecoverySnapshotValue = new List<BackupAndRecoverySnapshot>();
                    this.backupAndRecoverySnapshotValue.Add((BackupAndRecoverySnapshot)snapshot);
                }
                else if (snapshot is DBSecuritySnapshot)
                {
                    if (dBSecuritySnapshotValue == null) dBSecuritySnapshotValue = new List<DBSecuritySnapshot>();
                    this.dBSecuritySnapshotValue.Add((DBSecuritySnapshot)snapshot);
                }
                else if (snapshot is DisabledIndexesSnapshot)
                {
                    if (disabledIndexesSnapshotValue == null) disabledIndexesSnapshotValue = new List<DisabledIndexesSnapshot>();
                    this.disabledIndexesSnapshotValue.Add((DisabledIndexesSnapshot)snapshot);
                }
                else if (snapshot is FragmentedIndexesSnapshot)
                {
                    if (fragmentedIndexesSnapshotValue == null) fragmentedIndexesSnapshotValue = new List<FragmentedIndexesSnapshot>();
                    this.fragmentedIndexesSnapshotValue.Add((FragmentedIndexesSnapshot)snapshot);
                }
                else if (snapshot is HighIndexUpdatesSnapshot)
                {
                    if (highIndexUpdatesSnapshotValue == null) highIndexUpdatesSnapshotValue = new List<HighIndexUpdatesSnapshot>();
                    this.highIndexUpdatesSnapshotValue.Add((HighIndexUpdatesSnapshot)snapshot);
                }
                else if (snapshot is IndexContentionSnapshot)
                {
                    if (indexContentionSnapshotValue == null) indexContentionSnapshotValue = new List<IndexContentionSnapshot>();
                    this.indexContentionSnapshotValue.Add((IndexContentionSnapshot)snapshot);
                }
                else if (snapshot is OutOfDateStatsSnapshot)
                {
                    if (outOfDateStatsSnapshotValue == null) outOfDateStatsSnapshotValue = new List<OutOfDateStatsSnapshot>();
                    this.outOfDateStatsSnapshotValue.Add((OutOfDateStatsSnapshot)snapshot);
                }
                else if (snapshot is OverlappingIndexesSnapshot)
                {
                    if (overlappingIndexesSnapshotValue == null) overlappingIndexesSnapshotValue = new List<OverlappingIndexesSnapshot>();
                    this.overlappingIndexesSnapshotValue.Add((OverlappingIndexesSnapshot)snapshot);
                }
                else if (snapshot is SQLModuleOptionsSnapshot)
                {
                    if (sQLModuleOptionsSnapshotValue == null) sQLModuleOptionsSnapshotValue = new List<SQLModuleOptionsSnapshot>();
                    this.sQLModuleOptionsSnapshotValue.Add((SQLModuleOptionsSnapshot)snapshot);
                }
                    //SQLDm 10.0 - Srishti Purohit- New Recommendations - SDR-I23 Adding new Recomm
                else if (snapshot is NonIncrementalColumnStatSnapshot)
                {
                    if (nonIncrementalColStatsSnapshotValue == null) nonIncrementalColStatsSnapshotValue = new List<NonIncrementalColumnStatSnapshot>();
                    this.nonIncrementalColStatsSnapshotValue.Add((NonIncrementalColumnStatSnapshot)snapshot);
                }
                //SQLDm 10.0 - Srishti Purohit- New Recommendations - SDR-I25, SDR-I26, SDR-I27, SDR-I28 Adding new Recomm
                else if (snapshot is HashIndexSnapshot)
                {
                    if (hashIndexSnapshotValue == null) hashIndexSnapshotValue = new List<HashIndexSnapshot>();
                    this.hashIndexSnapshotValue.Add((HashIndexSnapshot)snapshot);
                }
                //SQLDm 10.0 - Srishti Purohit- New Recommendations - SDR-Q39 , 40, 41, 42 Adding new Recomm
                else if (snapshot is QueryStoreSnapshot)
                {
                    if (queryStoreSnapshotValue == null) queryStoreSnapshotValue = new List<QueryStoreSnapshot>();
                    this.queryStoreSnapshotValue.Add((QueryStoreSnapshot)snapshot);
                }
                //SQLDm 10.0 - Srishti Purohit- New Recommendations - SDR-I29 Adding new Recomm
                else if (snapshot is RarelyUsedIndexOnInMemoryTableSnapshot)
                {
                    if (rarelyUsedIndexOnInMemoryTableSnapshotValue == null) rarelyUsedIndexOnInMemoryTableSnapshotValue = new List<RarelyUsedIndexOnInMemoryTableSnapshot>();
                    this.rarelyUsedIndexOnInMemoryTableSnapshotValue.Add((RarelyUsedIndexOnInMemoryTableSnapshot)snapshot);
                }
                //SQLDm 10.0 - Srishti Purohit- New Recommendations - SDR-SDR-Q46,Q47,Q48,Q49,Q50 Adding new Recomm
                else if (snapshot is QueryAnalyzerSnapshot)
                {
                    if (queryAnalyzerSnapshotValue == null) queryAnalyzerSnapshotValue = new List<QueryAnalyzerSnapshot>();
                    this.queryAnalyzerSnapshotValue.Add((QueryAnalyzerSnapshot)snapshot);
                }
                //SQLDm 10.0 - Srishti Purohit- New Recommendations - SDR-I30 Adding new Recomm
                else if (snapshot is ColumnStoreIndexSnapshot)
                {
                    if (columnStoreIndexSnapshotValue == null) columnStoreIndexSnapshotValue = new List<ColumnStoreIndexSnapshot>();
                    this.columnStoreIndexSnapshotValue.Add((ColumnStoreIndexSnapshot)snapshot);
                }
                //SQLDm 10.0 - Srishti Purohit- New Recommendations - SDR-I31 Adding new Recomm
                else if (snapshot is FilteredColumnNotInKeyOfFilteredIndexSnapshot)
                {
                    if (filteredIndexValue == null) filteredIndexValue = new List<FilteredColumnNotInKeyOfFilteredIndexSnapshot>();
                    this.filteredIndexValue.Add((FilteredColumnNotInKeyOfFilteredIndexSnapshot)snapshot);
                }
                //SQLDm 10.0 - Srishti Purohit- New Recommendations - SDR-Q43 Adding new Recomm
                else if (snapshot is HighCPUTimeProcedureSnapshot)
                {
                    if (highCPUTimeProcedureValue == null) highCPUTimeProcedureValue = new List<HighCPUTimeProcedureSnapshot>();
                    this.highCPUTimeProcedureValue.Add((HighCPUTimeProcedureSnapshot)snapshot);
                }
                //SQLDm 10.0 - Srishti Purohit- New Recommendations - SDR-I24 Adding new Recomm
                else if (snapshot is LargeTableStatsSnapshot)
                {
                    if (largeTableStatsSnapshotSnapshotValue == null) largeTableStatsSnapshotSnapshotValue = new List<LargeTableStatsSnapshot>();
                    this.largeTableStatsSnapshotSnapshotValue.Add((LargeTableStatsSnapshot)snapshot);
                }
            }
            #endregion

            #region startupSnapshots
            foreach (Snapshot snapshot in lstStartupSnapshots)
            {
                if (snapshot is ConfigurationSnapshot)
                {
                    this.configurationSnapshotValueStartup = (ConfigurationSnapshot)snapshot;
                }
                else if (snapshot is GetMasterFilesSnapshot)
                {
                    this.getMasterFilesSnapshotValueStartup = (GetMasterFilesSnapshot)snapshot;
                }
               else if (snapshot is SampleServerResourcesSnapshot)
                {
                    this.sampleServerResourcesSnapshotValueStartup = (SampleServerResourcesSnapshot)snapshot;
                }
                else if (snapshot is ServerConfigurationSnapshot)
                {
                    this.serverConfigurationSnapshotValueStartup = (ServerConfigurationSnapshot)snapshot;
                }
                else if (snapshot is WaitingBatchesSnapshot)
                {
                    this.waitingBatchesSnapshotValueStartup = (WaitingBatchesSnapshot)snapshot;
                }
                else if (snapshot is WaitStatisticsSnapshot)
                {
                    this.waitStatisticsSnapshotValueStartup = (WaitStatisticsSnapshot)snapshot;
                }
                else if (snapshot is WmiVolumeSnapshot)
                {
                    this.wmiVolumeSnapshotValueStartup = (WmiVolumeSnapshot)snapshot;
                }
                else if (snapshot is WmiPerfOSSystemSnapshot)
                {
                    this.wmiPerfOSSystemSnapshotValueStartup = (WmiPerfOSSystemSnapshot)snapshot;
                }
                else if (snapshot is WmiPerfOSProcessorSnapshot)
                {
                    this.wmiPerfOSProcessorSnapshotValueStartup = (WmiPerfOSProcessorSnapshot)snapshot;
                }
                else if (snapshot is WmiPerfDiskPhysicalDiskSnapshot)
                {
                    this.wmiPerfDiskPhysicalDiskSnapshotValueStartup = (WmiPerfDiskPhysicalDiskSnapshot)snapshot;
                }
                else if (snapshot is WmiPerfDiskLogicalDiskSnapshot)
                {
                    this.wmiPerfDiskLogicalDiskSnapshotValueStartup = (WmiPerfDiskLogicalDiskSnapshot)snapshot;
                }
                else if (snapshot is WmiPageFileSnapshot)
                {
                    this.wmiPageFileSnapshotValueStartup = (WmiPageFileSnapshot)snapshot;
                }
                else if (snapshot is WmiNetworkRedirectorSnapshot)
                {
                    this.wmiNetworkRedirectorSnapshotValueStartup = (WmiNetworkRedirectorSnapshot)snapshot;
                }
                else if (snapshot is WmiNetworkInterfaceSnapshot)
                {
                    this.wmiNetworkInterfaceSnapshotValueStartup = (WmiNetworkInterfaceSnapshot)snapshot;
                }
                else if (snapshot is WmiTCPSnapshot)
                {
                    this.wmiTCPSnapshotValueStartup = (WmiTCPSnapshot)snapshot;
                }
                else if (snapshot is WmiTCPv4Snapshot)
                {
                    this.wmiTCPv4SnapshotValueStartup = (WmiTCPv4Snapshot)snapshot;
                }
                else if (snapshot is WmiTCPv6Snapshot)
                {
                    this.wmiTCPv6SnapshotValueStartup = (WmiTCPv6Snapshot)snapshot;
                }
            }
            #endregion

            #region shutdownSnapshots
            foreach (Snapshot snapshot in lstShutdownSnapshots)
            {
                if (snapshot is WmiTCPSnapshot)
                {
                    this.wmiTCPSnapshotValueShutdown = (WmiTCPSnapshot)snapshot;
                }
                else if (snapshot is WmiTCPv6Snapshot)
                {
                    this.wmiTCPv6SnapshotValueShutdown = (WmiTCPv6Snapshot)snapshot;
                }
                else if (snapshot is WmiTCPv4Snapshot)
                {
                    this.WmiTCPv4SnapshotValueShutdown = (WmiTCPv4Snapshot)snapshot;
                }
                else if (snapshot is WmiNetworkRedirectorSnapshot)
                {
                    this.wmiNetworkRedirectorSnapshotValueShutdown = (WmiNetworkRedirectorSnapshot)snapshot;
                }
                else if (snapshot is WmiPerfOSMemorySnapshot)
                {
                    this.wmiPerfOSMemorySnapshotValueShutdown = (WmiPerfOSMemorySnapshot)snapshot;
                }
                else if (snapshot is WmiPerfOSProcessorSnapshot)
                {
                    this.wmiPerfOSProcessorSnapshotValueShutdown = (WmiPerfOSProcessorSnapshot)snapshot;
                }
                else if (snapshot is WmiPerfDiskLogicalDiskSnapshot)
                {
                    this.WmiPerfDiskLogicalDiskSnapshotValueShutdown = (WmiPerfDiskLogicalDiskSnapshot)snapshot;
                }
                else if (snapshot is WmiPerfDiskPhysicalDiskSnapshot)
                {
                    this.wmiPerfDiskPhysicalDiskSnapshotValueShutdown = (WmiPerfDiskPhysicalDiskSnapshot)snapshot;
                }
                else if (snapshot is WmiPageFileSnapshot)
                {
                    this.wmiPageFileSnapshotValueShutdown = (WmiPageFileSnapshot)snapshot;
                }
                else if (snapshot is WmiNetworkInterfaceSnapshot)
                {
                    this.wmiNetworkInterfaceSnapshotValueShutdown = (WmiNetworkInterfaceSnapshot)snapshot;
                }
                else if (snapshot is SampleServerResourcesSnapshot)
                {
                    this.sampleServerResourcesSnapshotValueShutdown = (SampleServerResourcesSnapshot)snapshot;
                }
                else if (snapshot is WmiProcessSnapshot)
                {
                    this.wmiProcessSnapshotValueShutdown = (WmiProcessSnapshot)snapshot;
                }
                else if (snapshot is WorstFillFactorIndexesSnapshot)
                {
                    this.worstFillFactorIndexesSnapshotValueShutdown = (WorstFillFactorIndexesSnapshot)snapshot;
                }
                else if (snapshot is NUMANodeCountersSnapshot)
                {
                    this.nUMANodeCountersSnapshotValueShutdown = (NUMANodeCountersSnapshot)snapshot;
                }
                else if (snapshot is WmiEncryptableVolumeSnapshot)
                {
                    this.wmiEncryptableVolumeSnapshotValueShutdown = (WmiEncryptableVolumeSnapshot)snapshot;
                }
                else if (snapshot is WmiComputerSystemSnapshot)
                {
                    this.wmiComputerSystemSnapshotValueShutdown = (WmiComputerSystemSnapshot)snapshot;
                }
                else if (snapshot is WmiPhysicalMemorySnapshot)
                {
                    this.wmiPhysicalMemorySnapshotValueShutdown = (WmiPhysicalMemorySnapshot)snapshot;
                }
                else if (snapshot is WmiServiceSnapshot)
                {
                    this.wmiServiceSnapshotValueShutdown = (WmiServiceSnapshot)snapshot;
                }
            }
            #endregion

            #region intervalSnapshots
            foreach (Snapshot snapshot in lstIntervalSnapshots)
            {
                #region OlderCode
                /*Commented out as per previous model of having a list of snapshots. now we are having single snapshot with list of datatables or class objects
                if (snapshot is WmiPerfOSSystemSnapshot)
                {
                    if (wmiPerfOSSystemSnapshotValueInterval == null) wmiPerfOSSystemSnapshotValueInterval = new List<WmiPerfOSSystemSnapshot>();
                    this.wmiPerfOSSystemSnapshotValueInterval.Add((WmiPerfOSSystemSnapshot)snapshot);
                }

                else if (snapshot is WmiProcessorSnapshot)
                 {
                     if (wmiProcessorSnapshotValueInterval == null) wmiProcessorSnapshotValueInterval = new List<WmiProcessorSnapshot>();
                     this.wmiProcessorSnapshotValueInterval.Add((WmiProcessorSnapshot)snapshot);
                 }

                else if (snapshot is WaitingBatchesSnapshot)
                 {
                     if (waitingBatchesSnapshotValueInterval == null) waitingBatchesSnapshotValueInterval = new List<WaitingBatchesSnapshot>();
                     this.waitingBatchesSnapshotValueInterval.Add((WaitingBatchesSnapshot)snapshot);
                 }

                else if (snapshot is SampleServerResourcesSnapshot)
                 {
                     if (sampleServerResourcesSnapshotValueInterval == null) sampleServerResourcesSnapshotValueInterval = new List<SampleServerResourcesSnapshot>();
                     this.sampleServerResourcesSnapshotValueInterval.Add((SampleServerResourcesSnapshot)snapshot);
                 }
                 * */
                #endregion

                if (snapshot is WmiPerfOSSystemSnapshot)
                {
                    this.wmiPerfOSSystemSnapshotValueInterval = ((WmiPerfOSSystemSnapshot)snapshot);
                }

                else if (snapshot is WmiProcessorSnapshot)
                {
                    this.wmiProcessorSnapshotValueInterval = ((WmiProcessorSnapshot)snapshot);
                }

                else if (snapshot is WaitingBatchesSnapshot)
                {
                    this.waitingBatchesSnapshotValueInterval = ((WaitingBatchesSnapshot)snapshot);
                }

                else if (snapshot is SampleServerResourcesSnapshot)
                {
                    this.sampleServerResourcesSnapshotValueInterval = ((SampleServerResourcesSnapshot)snapshot);
                }
                //SQLDm 10.0 - Srishti Purohit- New Recommendations - SDR-M33 Adding new Recomm
                else if (snapshot is BufferPoolExtIOSnapshot)
                {
                    this.bufferPoolExtIOSnapshotValueInterval = (BufferPoolExtIOSnapshot)snapshot;
                }
            }
            #endregion
        }

        #region fields
        string machineName = string.Empty;

        public string MachineName { get { return machineName; } set { machineName = value; } }

        string connectionString = string.Empty;

        public string ConnectionString { get { return connectionString; } set { connectionString = value; } }

        //Start:Startup Snapshots
        GetMasterFilesSnapshot getMasterFilesSnapshotValueStartup;

        public GetMasterFilesSnapshot GetMasterFilesSnapshotValueStartup
        {
            get { return getMasterFilesSnapshotValueStartup; }
            set { getMasterFilesSnapshotValueStartup = value; }
        }
        WmiPerfDiskPhysicalDiskSnapshot wmiPerfDiskPhysicalDiskSnapshotValueStartup;

        public WmiPerfDiskPhysicalDiskSnapshot WmiPerfDiskPhysicalDiskSnapshotValueStartup
        {
            get { return wmiPerfDiskPhysicalDiskSnapshotValueStartup; }
            set { wmiPerfDiskPhysicalDiskSnapshotValueStartup = value; }
        }
        WmiNetworkRedirectorSnapshot wmiNetworkRedirectorSnapshotValueStartup;

        public WmiNetworkRedirectorSnapshot WmiNetworkRedirectorSnapshotValueStartup
        {
            get { return wmiNetworkRedirectorSnapshotValueStartup; }
            set { wmiNetworkRedirectorSnapshotValueStartup = value; }
        }
        WmiTCPSnapshot wmiTCPSnapshotValueStartup;

        public WmiTCPSnapshot WmiTCPSnapshotValueStartup
        {
            get { return wmiTCPSnapshotValueStartup; }
            set { wmiTCPSnapshotValueStartup = value; }
        }
        WmiTCPv6Snapshot wmiTCPv6SnapshotValueStartup;

        public WmiTCPv6Snapshot WmiTCPv6SnapshotValueStartup
        {
            get { return wmiTCPv6SnapshotValueStartup; }
            set { wmiTCPv6SnapshotValueStartup = value; }
        }
        WmiTCPv4Snapshot wmiTCPv4SnapshotValueStartup;

        public WmiTCPv4Snapshot WmiTCPv4SnapshotValueStartup
        {
            get { return wmiTCPv4SnapshotValueStartup; }
            set { wmiTCPv4SnapshotValueStartup = value; }
        }
        WmiNetworkInterfaceSnapshot wmiNetworkInterfaceSnapshotValueStartup;

        public WmiNetworkInterfaceSnapshot WmiNetworkInterfaceSnapshotValueStartup
        {
            get { return wmiNetworkInterfaceSnapshotValueStartup; }
            set { wmiNetworkInterfaceSnapshotValueStartup = value; }
        }
        WmiPerfDiskLogicalDiskSnapshot wmiPerfDiskLogicalDiskSnapshotValueStartup;

        public WmiPerfDiskLogicalDiskSnapshot WmiPerfDiskLogicalDiskSnapshotValueStartup
        {
            get { return wmiPerfDiskLogicalDiskSnapshotValueStartup; }
            set { wmiPerfDiskLogicalDiskSnapshotValueStartup = value; }
        }
        WmiPerfOSSystemSnapshot wmiPerfOSSystemSnapshotValueStartup;

        public WmiPerfOSSystemSnapshot WmiPerfOSSystemSnapshotValueStartup
        {
            get { return wmiPerfOSSystemSnapshotValueStartup; }
            set { wmiPerfOSSystemSnapshotValueStartup = value; }
        }

        WmiPerfOSProcessorSnapshot wmiPerfOSProcessorSnapshotValueStartup;

        public WmiPerfOSProcessorSnapshot WmiPerfOSProcessorSnapshotValueStartup
        {
            get { return wmiPerfOSProcessorSnapshotValueStartup; }
            set { wmiPerfOSProcessorSnapshotValueStartup = value; }
        }
        WmiVolumeSnapshot wmiVolumeSnapshotValueStartup;

        public WmiVolumeSnapshot WmiVolumeSnapshotValueStartup
        {
            get { return wmiVolumeSnapshotValueStartup; }
            set { wmiVolumeSnapshotValueStartup = value; }
        }
        WaitStatisticsSnapshot waitStatisticsSnapshotValueStartup;

        public WaitStatisticsSnapshot WaitStatisticsSnapshotValueStartup
        {
            get { return waitStatisticsSnapshotValueStartup; }
            set { waitStatisticsSnapshotValueStartup = value; }
        }
        ConfigurationSnapshot configurationSnapshotValueStartup;

        public ConfigurationSnapshot ConfigurationSnapshotValueStartup
        {
            get { return configurationSnapshotValueStartup; }
            set { configurationSnapshotValueStartup = value; }
        }
        SampleServerResourcesSnapshot sampleServerResourcesSnapshotValueStartup;

        public SampleServerResourcesSnapshot SampleServerResourcesSnapshotValueStartup
        {
            get { return sampleServerResourcesSnapshotValueStartup; }
            set { sampleServerResourcesSnapshotValueStartup = value; }
        }
        ServerConfigurationSnapshot serverConfigurationSnapshotValueStartup;

        public ServerConfigurationSnapshot ServerConfigurationSnapshotValueStartup
        {
            get { return serverConfigurationSnapshotValueStartup; }
            set { serverConfigurationSnapshotValueStartup = value; }
        }
        WmiPageFileSnapshot wmiPageFileSnapshotValueStartup;

        public WmiPageFileSnapshot WmiPageFileSnapshotValueStartup
        {
            get { return wmiPageFileSnapshotValueStartup; }
            set { wmiPageFileSnapshotValueStartup = value; }
        }
        //End:Startup Snapshots

        //Start:ShutDown Collectors
        WmiTCPSnapshot wmiTCPSnapshotValueShutdown;

        public WmiTCPSnapshot WmiTCPSnapshotValueShutdown
        {
            get { return wmiTCPSnapshotValueShutdown; }
            set { wmiTCPSnapshotValueShutdown = value; }
        }
        WmiTCPv6Snapshot wmiTCPv6SnapshotValueShutdown;

        public WmiTCPv6Snapshot WmiTCPv6SnapshotValueShutdown
        {
            get { return wmiTCPv6SnapshotValueShutdown; }
            set { wmiTCPv6SnapshotValueShutdown = value; }
        }
        WmiTCPv4Snapshot wmiTCPv4SnapshotValueShutdown;

        public WmiTCPv4Snapshot WmiTCPv4SnapshotValueShutdown
        {
            get { return wmiTCPv4SnapshotValueShutdown; }
            set { wmiTCPv4SnapshotValueShutdown = value; }
        }
        WmiNetworkRedirectorSnapshot wmiNetworkRedirectorSnapshotValueShutdown;

        public WmiNetworkRedirectorSnapshot WmiNetworkRedirectorSnapshotValueShutdown
        {
            get { return wmiNetworkRedirectorSnapshotValueShutdown; }
            set { wmiNetworkRedirectorSnapshotValueShutdown = value; }
        }
        WmiPerfOSMemorySnapshot wmiPerfOSMemorySnapshotValueShutdown;//This is not repeating

        public WmiPerfOSMemorySnapshot WmiPerfOSMemorySnapshotValueShutdown
        {
            get { return wmiPerfOSMemorySnapshotValueShutdown; }
            set { wmiPerfOSMemorySnapshotValueShutdown = value; }
        }

        WmiPerfOSProcessorSnapshot wmiPerfOSProcessorSnapshotValueShutdown;

        public WmiPerfOSProcessorSnapshot WmiPerfOSProcessorSnapshotValueShutdown
        {
            get { return wmiPerfOSProcessorSnapshotValueShutdown; }
            set { wmiPerfOSProcessorSnapshotValueShutdown = value; }
        }
        WmiPerfDiskLogicalDiskSnapshot wmiPerfDiskLogicalDiskSnapshotValueShutdown;

        public WmiPerfDiskLogicalDiskSnapshot WmiPerfDiskLogicalDiskSnapshotValueShutdown
        {
            get { return wmiPerfDiskLogicalDiskSnapshotValueShutdown; }
            set { wmiPerfDiskLogicalDiskSnapshotValueShutdown = value; }
        }
        WmiPerfDiskPhysicalDiskSnapshot wmiPerfDiskPhysicalDiskSnapshotValueShutdown;

        public WmiPerfDiskPhysicalDiskSnapshot WmiPerfDiskPhysicalDiskSnapshotValueShutdown
        {
            get { return wmiPerfDiskPhysicalDiskSnapshotValueShutdown; }
            set { wmiPerfDiskPhysicalDiskSnapshotValueShutdown = value; }
        }
        WmiPageFileSnapshot wmiPageFileSnapshotValueShutdown;

        public WmiPageFileSnapshot WmiPageFileSnapshotValueShutdown
        {
            get { return wmiPageFileSnapshotValueShutdown; }
            set { wmiPageFileSnapshotValueShutdown = value; }
        }
        WmiNetworkInterfaceSnapshot wmiNetworkInterfaceSnapshotValueShutdown;

        public WmiNetworkInterfaceSnapshot WmiNetworkInterfaceSnapshotValueShutdown
        {
            get { return wmiNetworkInterfaceSnapshotValueShutdown; }
            set { wmiNetworkInterfaceSnapshotValueShutdown = value; }
        }
        SampleServerResourcesSnapshot sampleServerResourcesSnapshotValueShutdown;

        public SampleServerResourcesSnapshot SampleServerResourcesSnapshotValueShutdown
        {
            get { return sampleServerResourcesSnapshotValueShutdown; }
            set { sampleServerResourcesSnapshotValueShutdown = value; }
        }
        //repeating till here
        WmiProcessSnapshot wmiProcessSnapshotValueShutdown;

        public WmiProcessSnapshot WmiProcessSnapshotValueShutdown
        {
            get { return wmiProcessSnapshotValueShutdown; }
            set { wmiProcessSnapshotValueShutdown = value; }
        }
        WorstFillFactorIndexesSnapshot worstFillFactorIndexesSnapshotValueShutdown;

        public WorstFillFactorIndexesSnapshot WorstFillFactorIndexesSnapshotValueShutdown
        {
            get { return worstFillFactorIndexesSnapshotValueShutdown; }
            set { worstFillFactorIndexesSnapshotValueShutdown = value; }
        }
        NUMANodeCountersSnapshot nUMANodeCountersSnapshotValueShutdown;

        public NUMANodeCountersSnapshot NUMANodeCountersSnapshotValueShutdown
        {
            get { return nUMANodeCountersSnapshotValueShutdown; }
            set { nUMANodeCountersSnapshotValueShutdown = value; }
        }
        WmiEncryptableVolumeSnapshot wmiEncryptableVolumeSnapshotValueShutdown;

        public WmiEncryptableVolumeSnapshot WmiEncryptableVolumeSnapshotValueShutdown
        {
            get { return wmiEncryptableVolumeSnapshotValueShutdown; }
            set { wmiEncryptableVolumeSnapshotValueShutdown = value; }
        }
        WmiComputerSystemSnapshot wmiComputerSystemSnapshotValueShutdown;

        public WmiComputerSystemSnapshot WmiComputerSystemSnapshotValueShutdown
        {
            get { return wmiComputerSystemSnapshotValueShutdown; }
            set { wmiComputerSystemSnapshotValueShutdown = value; }
        }
        WmiPhysicalMemorySnapshot wmiPhysicalMemorySnapshotValueShutdown;

        public WmiPhysicalMemorySnapshot WmiPhysicalMemorySnapshotValueShutdown
        {
            get { return wmiPhysicalMemorySnapshotValueShutdown; }
            set { wmiPhysicalMemorySnapshotValueShutdown = value; }
        }
        WmiServiceSnapshot wmiServiceSnapshotValueShutdown;

        public WmiServiceSnapshot WmiServiceSnapshotValueShutdown
        {
            get { return wmiServiceSnapshotValueShutdown; }
            set { wmiServiceSnapshotValueShutdown = value; }
        }
        //End:ShutDown Collectors

        //Start:interval collectors
        WmiPerfOSSystemSnapshot wmiPerfOSSystemSnapshotValueInterval;

        public WmiPerfOSSystemSnapshot WmiPerfOSSystemSnapshotValueInterval
        {
            get { return wmiPerfOSSystemSnapshotValueInterval; }
            set { wmiPerfOSSystemSnapshotValueInterval = value; }
        }


       WmiProcessorSnapshot wmiProcessorSnapshotValueInterval;

        public WmiProcessorSnapshot WmiProcessorSnapshotValueInterval
        {
            get { return wmiProcessorSnapshotValueInterval; }
            set { wmiProcessorSnapshotValueInterval = value; }
        }

        WaitingBatchesSnapshot waitingBatchesSnapshotValueInterval;

        public WaitingBatchesSnapshot WaitingBatchesSnapshotValueInterval
        {
            get { return waitingBatchesSnapshotValueInterval; }
            set { waitingBatchesSnapshotValueInterval = value; }
        }
        SampleServerResourcesSnapshot sampleServerResourcesSnapshotValueInterval;

        public SampleServerResourcesSnapshot SampleServerResourcesSnapshotValueInterval
        {
            get { return sampleServerResourcesSnapshotValueInterval; }
            set { sampleServerResourcesSnapshotValueInterval = value; }
        }
        //End:interval collectors


        List<BackupAndRecoverySnapshot> backupAndRecoverySnapshotValue;

        public List<BackupAndRecoverySnapshot> BackupAndRecoverySnapshotList
        {
            get { return backupAndRecoverySnapshotValue; }
            set { backupAndRecoverySnapshotValue = value; }
        }

        List<DBSecuritySnapshot> dBSecuritySnapshotValue;

        public List<DBSecuritySnapshot> DBSecuritySnapshotList
        {
            get { return dBSecuritySnapshotValue; }
            set { dBSecuritySnapshotValue = value; }
        }

        List<DisabledIndexesSnapshot> disabledIndexesSnapshotValue;

        public List<DisabledIndexesSnapshot> DisabledIndexesSnapshotList
        {
            get { return disabledIndexesSnapshotValue; }
            set { disabledIndexesSnapshotValue = value; }
        }
        List<FragmentedIndexesSnapshot> fragmentedIndexesSnapshotValue;

        public List<FragmentedIndexesSnapshot> FragmentedIndexesSnapshotList
        {
            get { return fragmentedIndexesSnapshotValue; }
            set { fragmentedIndexesSnapshotValue = value; }
        }

        List<HighIndexUpdatesSnapshot> highIndexUpdatesSnapshotValue;

        public List<HighIndexUpdatesSnapshot> HighIndexUpdatesSnapshotList
        {
            get { return highIndexUpdatesSnapshotValue; }
            set { highIndexUpdatesSnapshotValue = value; }
        }
        List<IndexContentionSnapshot> indexContentionSnapshotValue;

        public List<IndexContentionSnapshot> IndexContentionSnapshotList
        {
            get { return indexContentionSnapshotValue; }
            set { indexContentionSnapshotValue = value; }
        }

        List<OutOfDateStatsSnapshot> outOfDateStatsSnapshotValue;

        public List<OutOfDateStatsSnapshot> OutOfDateStatsSnapshotList
        {
            get { return outOfDateStatsSnapshotValue; }
            set { outOfDateStatsSnapshotValue = value; }
        }
        List<OverlappingIndexesSnapshot> overlappingIndexesSnapshotValue;

        public List<OverlappingIndexesSnapshot> OverlappingIndexesSnapshotList
        {
            get { return overlappingIndexesSnapshotValue; }
            set { overlappingIndexesSnapshotValue = value; }
        }

        List<SQLModuleOptionsSnapshot> sQLModuleOptionsSnapshotValue;

        public List<SQLModuleOptionsSnapshot> SQLModuleOptionsSnapshotList
        {
            get { return sQLModuleOptionsSnapshotValue; }
            set { sQLModuleOptionsSnapshotValue = value; }
        }

        DatabaseRankingSnapshot databaseRankingSnapshotValue;

        public DatabaseRankingSnapshot DatabaseRankingSnapshotValue
        {
            get { return databaseRankingSnapshotValue; }
            set { databaseRankingSnapshotValue = value; }
        }


        AdhocCachedPlanBytesSnapshot adhocCachedPlanBytesSnapshotValue;

        public AdhocCachedPlanBytesSnapshot AdhocCachedPlanBytesSnapshotValue
        {
            get { return adhocCachedPlanBytesSnapshotValue; }
            set { adhocCachedPlanBytesSnapshotValue = value; }
        }





        LockedPageKBSnapshot lockedPageKBSnapshotValue;

        public LockedPageKBSnapshot LockedPageKBSnapshotValue
        {
            get { return lockedPageKBSnapshotValue; }
            set { lockedPageKBSnapshotValue = value; }
        }


        QueryPlanEstRowsSnapshot queryPlanEstRowsSnapshotValue;

        public QueryPlanEstRowsSnapshot QueryPlanEstRowsSnapshotValue
        {
            get { return queryPlanEstRowsSnapshotValue; }
            set { queryPlanEstRowsSnapshotValue = value; }
        }


        WaitingBatchesSnapshot waitingBatchesSnapshotValueStartup;

        public WaitingBatchesSnapshot WaitingBatchesSnapshotValueStartup
        {
            get { return waitingBatchesSnapshotValueStartup; }
            set { waitingBatchesSnapshotValueStartup = value; }
        }


        DatabaseConfigurationSnapshot databaseConfigurationSnapshotValue;

        public DatabaseConfigurationSnapshot DatabaseConfigurationSnapshotValue
        {
            get { return databaseConfigurationSnapshotValue; }
            set { databaseConfigurationSnapshotValue = value; }
        }

        //Srishti Purohit SQLdm 10.0 - New Recommendations - SDR-I23
        List<NonIncrementalColumnStatSnapshot> nonIncrementalColStatsSnapshotValue;

        public List<NonIncrementalColumnStatSnapshot> NonIncrementalColStatsSnapshotList
        {
            get { return nonIncrementalColStatsSnapshotValue; }
            set { nonIncrementalColStatsSnapshotValue = value; }
        }
        //Srishti Purohit SQLdm 10.0 - New Recommendations - SDR-I25 , 26, 28
        List<HashIndexSnapshot> hashIndexSnapshotValue;

        public List<HashIndexSnapshot> HashIndexSnapshotList
        {
            get { return hashIndexSnapshotValue; }
            set { hashIndexSnapshotValue = value; }
        }
        //Srishti Purohit SQLdm 10.0 - New Recommendations - SDR-Q39,Q40,Q41, Q42
        List<QueryStoreSnapshot> queryStoreSnapshotValue;

        public List<QueryStoreSnapshot> QueryStoreSnapshotList
        {
            get { return queryStoreSnapshotValue; }
            set { queryStoreSnapshotValue = value; }
        }
        //Srishti Purohit SQLdm 10.0 - New Recommendations - SDR-I29
        List<RarelyUsedIndexOnInMemoryTableSnapshot> rarelyUsedIndexOnInMemoryTableSnapshotValue;

        public List<RarelyUsedIndexOnInMemoryTableSnapshot> RarelyUsedIndexOnInMemoryTableSnapshotList
        {
            get { return rarelyUsedIndexOnInMemoryTableSnapshotValue; }
            set { rarelyUsedIndexOnInMemoryTableSnapshotValue = value; }
        }
        //Srishti Purohit SQLdm 10.0 - New Recommendations - SDR-Q46,Q47,Q48,Q49,Q50
        List<QueryAnalyzerSnapshot> queryAnalyzerSnapshotValue;

        public List<QueryAnalyzerSnapshot> QueryAnalyzerSnapshotList
        {
            get { return queryAnalyzerSnapshotValue; }
            set { queryAnalyzerSnapshotValue = value; }
        }

        //Srishti Purohit SQLdm 10.0 - New Recommendations - SDR-I30
        List<ColumnStoreIndexSnapshot> columnStoreIndexSnapshotValue;

        public List<ColumnStoreIndexSnapshot> ColumnStoreIndexSnapshotList
        {
            get { return columnStoreIndexSnapshotValue; }
            set { columnStoreIndexSnapshotValue = value; }
        }
        //Srishti Purohit SQLdm 10.0 - New Recommendations - SDR-I31
        List<FilteredColumnNotInKeyOfFilteredIndexSnapshot> filteredIndexValue;

        public List<FilteredColumnNotInKeyOfFilteredIndexSnapshot> FilteredIndexSnapshotList
        {
            get { return filteredIndexValue; }
            set { filteredIndexValue = value; }
        }
        //Srishti Purohit SQLdm 10.0 - New Recommendations - SDR-Q43
        List<HighCPUTimeProcedureSnapshot> highCPUTimeProcedureValue;

        public List<HighCPUTimeProcedureSnapshot> HighCPUTimeProcedureSnapshotList
        {
            get { return highCPUTimeProcedureValue; }
            set { highCPUTimeProcedureValue = value; }
        }
        //Srishti Purohit SQLdm 10.0 - New Recommendations - SDR-I24
        List<LargeTableStatsSnapshot> largeTableStatsSnapshotSnapshotValue;

        public List<LargeTableStatsSnapshot> LargeTableStatsSnapshotList
        {
            get { return largeTableStatsSnapshotSnapshotValue; }
            set { largeTableStatsSnapshotSnapshotValue = value; }
        }
        //Srishti Purohit SQLdm 10.0 - New Recommendations - SDR-M33
        BufferPoolExtIOSnapshot bufferPoolExtIOSnapshotValueInterval;

        public BufferPoolExtIOSnapshot BufferPoolExtIOSnapshotValueInterval
        {
            get { return bufferPoolExtIOSnapshotValueInterval; }
            set { bufferPoolExtIOSnapshotValueInterval = value; }
        }


























        internal PrescriptiveAnalyticsSnapshot(SqlConnectionInfo info)
            : base(info.InstanceName)
        {

        }

        #endregion

        #region properties



        #endregion







    }
}
